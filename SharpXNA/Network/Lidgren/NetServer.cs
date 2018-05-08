using System;
using System.Collections.Generic;
using System.Threading;

namespace Lidgren.Network
{
	/// <summary>
	/// Specialized version of NetPeer used for "server" peers
	/// </summary>
	public class NetServer : NetPeer
	{
		/// <summary>
		/// NetServer constructor
		/// </summary>
		public NetServer(NetPeerConfiguration config)
			: base(config)
		{
			config.AcceptIncomingConnections = true;
		}

		/// <summary>
		/// Send a message to all connections
		/// </summary>
		/// <param name="msg">The message to send</param>
		/// <param name="method">How to deliver the message</param>
		public void Send(NetOutgoingMessage msg, NetDeliveryMethod method = NetDeliveryMethod.ReliableOrdered, int sequenceChannel = 0)
		{
			var all = this.Connections;
			if (all.Count <= 0) {
				if (msg.m_isSent == false)
					Recycle(msg);
				return;
			}

			SendTo(msg, all, method, sequenceChannel);
		}

		/// <summary>
		/// Send a message to all connections except one
		/// </summary>
		/// <param name="msg">The message to send</param>
		/// <param name="method">How to deliver the message</param>
		/// <param name="except">Don't send to this particular connection</param>
		/// <param name="sequenceChannel">Which sequence channel to use for the message</param>
		public void Send(NetOutgoingMessage msg, NetConnection except, NetDeliveryMethod method = NetDeliveryMethod.ReliableOrdered, int sequenceChannel = 0)
		{
			var all = this.Connections;
			if (all.Count <= 0) {
				if (msg.m_isSent == false)
					Recycle(msg);
				return;
			}

			if (except == null)
			{
				SendTo(msg, all, method, sequenceChannel);
				return;
			}

			var recipients = new List<NetConnection>(all.Count - 1);
			foreach (var conn in all)
				if (conn != except)
					recipients.Add(conn);

			if (recipients.Count > 0)
				SendTo(msg, recipients, method, sequenceChannel);
        }
        /// <summary>
        /// Send a message to a specific connection
        /// </summary>
        /// <param name="msg">The message to send</param>
        /// <param name="recipient">The recipient connection</param>
        /// <param name="method">How to deliver the message</param>
        /// <param name="sequenceChannel">Sequence channel within the delivery method</param>
        public NetSendResult SendTo(NetOutgoingMessage msg, NetConnection recipient, NetDeliveryMethod method = NetDeliveryMethod.ReliableOrdered, int sequenceChannel = 0)
        {
            if (msg == null)
                throw new ArgumentNullException("msg");
            if (recipient == null)
                throw new ArgumentNullException("recipient");
            if (sequenceChannel >= NetConstants.NetChannelsPerDeliveryMethod)
                throw new ArgumentOutOfRangeException("sequenceChannel");

            NetException.Assert(
                ((method != NetDeliveryMethod.Unreliable && method != NetDeliveryMethod.ReliableUnordered) ||
                ((method == NetDeliveryMethod.Unreliable || method == NetDeliveryMethod.ReliableUnordered) && sequenceChannel == 0)),
                "Delivery method " + method + " cannot use sequence channels other than 0!"
            );

            NetException.Assert(method != NetDeliveryMethod.Unknown, "Bad delivery method!");

            if (msg.m_isSent)
                throw new NetException("This message has already been sent! Use NetPeer.SendMessage() to send to multiple recipients efficiently");
            msg.m_isSent = true;

            Network.Statistics.UploadedBytes += (uint)msg.LengthBytes;
            Network.Statistics.uploadBytesPerSecLast += (uint)msg.LengthBytes;

            bool suppressFragmentation = (method == NetDeliveryMethod.Unreliable || method == NetDeliveryMethod.UnreliableSequenced) && m_configuration.UnreliableSizeBehaviour != NetUnreliableSizeBehaviour.NormalFragmentation;

            int len = NetConstants.UnfragmentedMessageHeaderSize + msg.LengthBytes; // headers + length, faster than calling msg.GetEncodedSize
            if (len <= recipient.m_currentMTU || suppressFragmentation)
            {
                Interlocked.Increment(ref msg.m_recyclingCount);
                return recipient.EnqueueMessage(msg, method, sequenceChannel);
            }
            else
            {
                // message must be fragmented!
                if (recipient.m_status != NetConnectionStatus.Connected)
                    return NetSendResult.FailedNotConnected;
                return SendFragmentedMessage(msg, new NetConnection[] { recipient }, method, sequenceChannel);
            }
        }

        /// <summary>
        /// Send a message to a list of connections
        /// </summary>
        /// <param name="msg">The message to send</param>
        /// <param name="recipients">The list of recipients to send to</param>
        /// <param name="method">How to deliver the message</param>
        /// <param name="sequenceChannel">Sequence channel within the delivery method</param>
        public void SendTo(NetOutgoingMessage msg, IList<NetConnection> recipients, NetDeliveryMethod method = NetDeliveryMethod.ReliableOrdered, int sequenceChannel = 0)
        {
            if (msg == null)
                throw new ArgumentNullException("msg");
            if (recipients == null)
            {
                if (msg.m_isSent == false)
                    Recycle(msg);
                throw new ArgumentNullException("recipients");
            }
            if (recipients.Count < 1)
            {
                if (msg.m_isSent == false)
                    Recycle(msg);
                //throw new NetException("recipients must contain at least one item");
                return;
            }
            if (method == NetDeliveryMethod.Unreliable || method == NetDeliveryMethod.ReliableUnordered)
                NetException.Assert(sequenceChannel == 0, "Delivery method " + method + " cannot use sequence channels other than 0!");
            if (msg.m_isSent)
                throw new NetException("This message has already been sent! Use NetPeer.SendMessage() to send to multiple recipients efficiently");
            msg.m_isSent = true;

            int mtu = GetMTU(recipients);

            int len = msg.GetEncodedSize();
            if (len <= mtu)
            {
                Interlocked.Add(ref msg.m_recyclingCount, recipients.Count);
                foreach (var conn in recipients)
                {
                    if (conn == null)
                    {
                        Interlocked.Decrement(ref msg.m_recyclingCount);
                        continue;
                    }
                    if (conn.m_isThrottled)
                    {
                        Interlocked.Decrement(ref msg.m_recyclingCount);
                        conn.m_throttledMessages.Add(new NetConnection.ThrottledMessage(msg, method, sequenceChannel));
                        continue;
                    }

                    Network.Statistics.UploadedBytes += (uint)msg.LengthBytes;
                    Network.Statistics.uploadBytesPerSecLast += (uint)msg.LengthBytes;

                    NetSendResult res = conn.EnqueueMessage(msg, method, sequenceChannel);
                    if (res == NetSendResult.Dropped)
                        Interlocked.Decrement(ref msg.m_recyclingCount);
                }
            }
            else
            {
                Network.Statistics.UploadedBytes += (uint)msg.LengthBytes;
                Network.Statistics.uploadBytesPerSecLast += (uint)msg.LengthBytes;

                // message must be fragmented!
                SendFragmentedMessage(msg, recipients, method, sequenceChannel);
            }
            return;
        }

        /// <summary>
        /// Returns a string that represents this object
        /// </summary>
        public override string ToString()
		{
			return "[NetServer " + ConnectionsCount + " connections]";
		}
	}
}
