using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace super.rtp.net
{
    public class UdpSocket : DisposeObject
    {
        private Socket socket;
        private IPEndPoint localEP_;

        public UdpSocket(IPEndPoint localEndpoint)
        {
            this.localEP_ = localEndpoint;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.socket.DontFragment = false;
            this.socket.Bind(this.localEP_);
        }
        public UdpSocket()
        {
            this.socket = null;
        }

        ~UdpSocket()
        {
            if (this.socket != null)
            {
                IDisposable disposable = this.socket;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            Dispose(false);
        }
        public virtual Socket GetSocket()
        {
            return this.socket;
        }
        public virtual int Send(byte[] buffer, IPEndPoint remoteEndpoint)
        {
            return this.socket.SendTo(buffer, remoteEndpoint);
        }

        public virtual int Receive(byte[] buffer)
        {
            return this.socket.Receive(buffer);
        }


    }
}
