using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace super.rtp.net
{
    public class RtcpCompoundPacket
    {
        private List<RtcpPacket> packetList;
        public List<RtcpPacket> Packets
        {
            get
            {
                return this.packetList;
            }
        }

        public RtcpCompoundPacket()
        {
            this.packetList = new List<RtcpPacket>();
        }

        public void AddPacket(RtcpPacket packet)
        {
            this.Packets.Add(packet);
        }
        [return: MarshalAs(UnmanagedType.U1)]
        public bool RemovePacket(RtcpPacket packet)
        {
            return this.Packets.Remove(packet);
        }
        public byte[] ToByteArray()
        {
            byte[] array = null;
            array = new byte[4096];
            uint newSize = 0u;
            List<RtcpPacket>.Enumerator enumerator = this.Packets.GetEnumerator();
            while (enumerator.MoveNext())
            {
                RtcpPacket current = enumerator.Current;
                current.ToByteArray(array, ref newSize);
            }
            Array.Resize<byte>(ref array, (int)newSize);
            return array;
        }
        public override string ToString()
        {
            uint num = 1u;
            string text = "";
            List<RtcpPacket>.Enumerator enumerator = this.Packets.GetEnumerator();
            while (enumerator.MoveNext())
            {
                RtcpPacket current = enumerator.Current;
                text += string.Format("Packet {0}\n{1}", num, current.ToString());
                num += 1u;
            }
            return text;
        }
        public List<RtcpPacket> ParseData(byte[] buffer, ref uint offset)
        {
            while (offset + 1 < buffer.Length)
            {
                RtcpType rt = (RtcpType)buffer[offset + 1];
                if ((byte)rt == 0)
                {
                    break;
                }
                RtcpType temprt = rt;
                temprt -= 200;
                switch (temprt)
                {
                    case (RtcpType)0:
                        {
                            RtcpPacket p = new RtcpSR();
                            p.ParseData(buffer, ref offset);
                            this.AddPacket(p);
                            break;
                        }
                    case (RtcpType)1:
                        {
                            RtcpPacket p = new RtcpRR();
                            p.ParseData(buffer, ref offset);
                            this.AddPacket(p);
                            break;
                        }
                    case (RtcpType)2:
                        {
                            RtcpPacket p = new RtcpSDES();
                            p.ParseData(buffer, ref offset);
                            this.AddPacket(p);
                            break;
                        }
                    case (RtcpType)3:
                        {
                            RtcpPacket p = new RtcpBYE();
                            p.ParseData(buffer, ref offset);
                            this.AddPacket(p);
                            break;
                        }
                    case (RtcpType)4:
                        {
                            RtcpPacket p = new RtcpAPP();
                            p.ParseData(buffer, ref offset);
                            this.AddPacket(p);
                            break;
                        }
                }
            }
            return this.Packets;
        }
    }
}
