using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace super.rtp.net
{
    public class RtpFrame : IDisposable
    {
        private List<RtpPacket> sl;
        private uint pushedBytes;
        private PacketComparer compare;
        public uint TotalPayloadSize
        {
            get
            {
                return this.pushedBytes;
            }
        }
        public uint PacketCount
        {
            get
            {
                return (uint)this.sl.Count;
            }
        }
        public RtpFrame()
        {
            this.sl = new List<RtpPacket>();
            this.pushedBytes = 0u;
            this.compare = new PacketComparer();
        }

        ~RtpFrame()
        {
            IDisposable disposable = this.sl as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
            IDisposable disposable2 = this.compare as IDisposable;
            if (disposable2 != null)
            {
                disposable2.Dispose();
            }
        }

        public void AddPacket(RtpPacket p)
        {
            this.pushedBytes += p.DataSize;
            this.sl.Add(p);
        }

        public byte[] GetAssembledFrame()
        {
            byte[] result;
            if (this.sl.Count == 0 || this.pushedBytes == 0u)
            {
                result = null;
            }
            else
            {
                byte[] array = new byte[this.pushedBytes];
                uint num = 0u;
                List<RtpPacket>.Enumerator enumerator = this.sl.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    RtpPacket current = enumerator.Current;
                    Array.Copy(current.DataPointer, 0, array, (int)num, (int)current.DataSize);
                    num += current.DataSize;
                }
                result = array;
            }
            return result;
        }
        public RtpPacket GetNextPacket()
        {
            RtpPacket result;
            if (this.sl.Count == 0)
            {
                result = null;
            }
            else
            {
                List<RtpPacket>.Enumerator enumerator = this.sl.GetEnumerator();
                enumerator.MoveNext();
                RtpPacket current = enumerator.Current;
                this.sl.RemoveAt(0);
                this.pushedBytes -= current.DataSize;
                result = current;
            }
            return result;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
