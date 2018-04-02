/* Copyright (c) 2010 Michael Lidgren

Permission is hereby granted, free of charge, to any person obtaining a copy of this software
and associated documentation files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom
the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Net;
using System.Threading;

#if !__NOIPENDPOINT__
using NetEndPoint = System.Net.IPEndPoint;
#endif

namespace Lidgren.Network
{
	/// <summary>
	/// Specialized version of NetPeer used for a "client" connection. It does not accept any incoming connections and maintains a ServerConnection property
	/// </summary>
	public class NetClient : NetPeer
	{
		/// <summary>
		/// Gets the connection to the server, if any
		/// </summary>
		public NetConnection ServerConnection
		{
			get
			{
				NetConnection retval = null;
				if (m_connections.Count > 0)
				{
					try
					{
						retval = m_connections[0];
					}
					catch
					{
						// preempted!
						return null;
					}
				}
				return retval;
			}
		}

		/// <summary>
		/// Gets the connection status of the server connection (or NetConnectionStatus.Disconnected if no connection)
		/// </summary>
		public NetConnectionStatus ConnectionStatus
		{
			get
			{
				var conn = ServerConnection;
				if (conn == null)
					return NetConnectionStatus.Disconnected;
				return conn.Status;
			}
		}

		/// <summary>
		/// NetClient constructor
		/// </summary>
		/// <param name="config"></param>
		public NetClient(NetPeerConfiguration config)
			: base(config)
		{
			config.AcceptIncomingConnections = false;
		}

		/// <summary>
		/// Connect to a remote server
		/// </summary>
		/// <param name="remoteEndPoint">The remote endpoint to connect to</param>
		/// <param name="hailMessage">The hail message to pass</param>
		/// <returns>server connection, or null if already connected</returns>
		public override NetConnection Connect(NetEndPoint remoteEndPoint, NetOutgoingMessage hailMessage)
		{
			lock (m_connections)
			{
				if (m_connections.Count > 0)
				{
					LogWarning("Connect attempt failed; Already connected");
					return null;
				}
			}

			lock (m_handshakes)
			{
				if (m_handshakes.Count > 0)
				{
					LogWarning("Connect attempt failed; Handshake already in progress");
					return null;
				}
			}

            SharpXNA.Network.Statistics.UploadedBytes += (uint)hailMessage.LengthBytes;
            SharpXNA.Network.Statistics.uploadBytesPerSecLast += (uint)hailMessage.LengthBytes;

            return base.Connect(remoteEndPoint, hailMessage);
		}

		/// <summary>
		/// Disconnect from server
		/// </summary>
		/// <param name="byeMessage">reason for disconnect</param>
		public void Disconnect(string byeMessage)
		{
			NetConnection serverConnection = ServerConnection;
			if (serverConnection == null)
			{
				lock (m_handshakes)
				{
					if (m_handshakes.Count > 0)
					{
						LogVerbose("Aborting connection attempt");
						foreach(var hs in m_handshakes)
							hs.Value.Disconnect(byeMessage);
						return;
					}
				}

				LogWarning("Disconnect requested when not connected!");
				return;
			}
			serverConnection.Disconnect(byeMessage);
            SharpXNA.Network.Statistics.UploadedBytes = 0;
            SharpXNA.Network.Statistics.uploadBytesPerSecLast = 0;
        }
        
		/// <summary>
		/// Sends message to server
		/// </summary>
		public NetSendResult Send(NetOutgoingMessage msg, NetDeliveryMethod method = NetDeliveryMethod.ReliableOrdered, int sequenceChannel = 0)
		{
			NetConnection serverConnection = ServerConnection;
			if (serverConnection == null)
			{
				LogWarning("Cannot send message, no server connection!");
				Recycle(msg);
				return NetSendResult.FailedNotConnected;
			}
            
            if (msg == null)
                throw new ArgumentNullException("msg");
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

            SharpXNA.Network.Statistics.UploadedBytes += (uint)msg.LengthBytes;
            SharpXNA.Network.Statistics.uploadBytesPerSecLast += (uint)msg.LengthBytes;

            bool suppressFragmentation = (method == NetDeliveryMethod.Unreliable || method == NetDeliveryMethod.UnreliableSequenced) && m_configuration.UnreliableSizeBehaviour != NetUnreliableSizeBehaviour.NormalFragmentation;

            int len = NetConstants.UnfragmentedMessageHeaderSize + msg.LengthBytes; // headers + length, faster than calling msg.GetEncodedSize
            if (len <= serverConnection.m_currentMTU || suppressFragmentation)
            {
                Interlocked.Increment(ref msg.m_recyclingCount);
                return serverConnection.EnqueueMessage(msg, method, sequenceChannel);
            }
            else
            {
                // message must be fragmented!
                if (serverConnection.m_status != NetConnectionStatus.Connected)
                    return NetSendResult.FailedNotConnected;
                return SendFragmentedMessage(msg, new NetConnection[] { serverConnection }, method, sequenceChannel);
            }
        }

		/// <summary>
		/// Returns a string that represents this object
		/// </summary>
		public override string ToString()
		{
			return "[NetClient " + ServerConnection + "]";
		}

	}
}
