using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;

#if !__NOIPENDPOINT__
using NetEndPoint = System.Net.IPEndPoint;
#endif

namespace Lidgren.Network
{
	public partial class NetPeer
	{
		internal static int GetMTU(IList<NetConnection> recipients)
		{
			int count = recipients.Count;

			int mtu = int.MaxValue;
			if (count < 1)
			{
#if DEBUG
				throw new NetException("GetMTU called with no recipients");
#else
				// we don't have access to the particular peer, so just use default MTU
				return NetPeerConfiguration.kDefaultMTU;
#endif
			}

			for(int i=0;i<count;i++)
			{
				var conn = recipients[i];
				int cmtu = conn.m_currentMTU;
				if (cmtu < mtu)
					mtu = cmtu;
			}
			return mtu;
		}

		/// <summary>
		/// Send a message to an unconnected host
		/// </summary>
		public void SendUnconnectedMessage(NetOutgoingMessage msg, string host, int port)
		{
			if (msg == null)
				throw new ArgumentNullException("msg");
			if (host == null)
				throw new ArgumentNullException("host");
			if (msg.m_isSent)
				throw new NetException("This message has already been sent! Use NetPeer.SendMessage() to send to multiple recipients efficiently");
			if (msg.LengthBytes > m_configuration.MaximumTransmissionUnit)
				throw new NetException("Unconnected messages too long! Must be shorter than NetConfiguration.MaximumTransmissionUnit (currently " + m_configuration.MaximumTransmissionUnit + ")");

			msg.m_isSent = true;
			msg.m_messageType = NetMessageType.Unconnected;

			var adr = NetUtility.Resolve(host);
			if (adr == null)
				throw new NetException("Failed to resolve " + host);

			Interlocked.Increment(ref msg.m_recyclingCount);
			m_unsentUnconnectedMessages.Enqueue(new NetTuple<NetEndPoint, NetOutgoingMessage>(new NetEndPoint(adr, port), msg));
		}

		/// <summary>
		/// Send a message to an unconnected host
		/// </summary>
		public void SendUnconnectedMessage(NetOutgoingMessage msg, NetEndPoint recipient)
		{
			if (msg == null)
				throw new ArgumentNullException("msg");
			if (recipient == null)
				throw new ArgumentNullException("recipient");
			if (msg.m_isSent)
				throw new NetException("This message has already been sent! Use NetPeer.SendMessage() to send to multiple recipients efficiently");
			if (msg.LengthBytes > m_configuration.MaximumTransmissionUnit)
				throw new NetException("Unconnected messages too long! Must be shorter than NetConfiguration.MaximumTransmissionUnit (currently " + m_configuration.MaximumTransmissionUnit + ")");

			msg.m_messageType = NetMessageType.Unconnected;
			msg.m_isSent = true;

			Interlocked.Increment(ref msg.m_recyclingCount);
			m_unsentUnconnectedMessages.Enqueue(new NetTuple<NetEndPoint, NetOutgoingMessage>(recipient, msg));
		}

		/// <summary>
		/// Send a message to an unconnected host
		/// </summary>
		public void SendUnconnectedMessage(NetOutgoingMessage msg, IList<NetEndPoint> recipients)
		{
			if (msg == null)
				throw new ArgumentNullException("msg");
			if (recipients == null)
				throw new ArgumentNullException("recipients");
			if (recipients.Count < 1)
				throw new NetException("recipients must contain at least one item");
			if (msg.m_isSent)
				throw new NetException("This message has already been sent! Use NetPeer.SendMessage() to send to multiple recipients efficiently");
			if (msg.LengthBytes > m_configuration.MaximumTransmissionUnit)
				throw new NetException("Unconnected messages too long! Must be shorter than NetConfiguration.MaximumTransmissionUnit (currently " + m_configuration.MaximumTransmissionUnit + ")");

			msg.m_messageType = NetMessageType.Unconnected;
			msg.m_isSent = true;

			Interlocked.Add(ref msg.m_recyclingCount, recipients.Count);
			foreach (NetEndPoint ep in recipients)
				m_unsentUnconnectedMessages.Enqueue(new NetTuple<NetEndPoint, NetOutgoingMessage>(ep, msg));
		}

		/// <summary>
		/// Send a message to this exact same netpeer (loopback)
		/// </summary>
		public void SendUnconnectedToSelf(NetOutgoingMessage om)
		{
			if (om == null)
				throw new ArgumentNullException("msg");
			if (om.m_isSent)
				throw new NetException("This message has already been sent! Use NetPeer.SendMessage() to send to multiple recipients efficiently");

			om.m_messageType = NetMessageType.Unconnected;
			om.m_isSent = true;

			if (m_configuration.IsMessageTypeEnabled(NetIncomingMessageType.UnconnectedData) == false)
			{
				Interlocked.Decrement(ref om.m_recyclingCount);
				return; // dropping unconnected message since it's not enabled for receiving
			}

			// convert outgoing to incoming
			NetIncomingMessage im = CreateIncomingMessage(NetIncomingMessageType.UnconnectedData, om.LengthBytes);
			im.Write(om);
			im.m_isFragment = false;
			im.m_receiveTime = NetTime.Now;
			im.m_senderConnection = null;
			im.m_senderEndPoint = m_socket.LocalEndPoint as NetEndPoint;
			NetException.Assert(im.m_bitLength == om.LengthBits);

			// recycle outgoing message
			Recycle(om);

			ReleaseMessage(im);
		}
    }
}
