using System;
using System.Runtime.InteropServices;
using System.Text;

namespace super.rtp.net
{
    public class RtcpSDESChunk : DisposeObject
    {
        private ChunkType type;
        private uint ssrc;
        private string cname;
        private string name;
        private string email;
        private string phone;
        private string loc;
        private string tool;
        private string note;
        private string priv;
        private string priv_prefix;

        public string PRIV_Prefix
        {
            get
            {
                return this.priv_prefix;
            }
            set
            {
                this.priv_prefix = value;
            }
        }
        public string PRIV
        {
            get
            {
                return this.priv;
            }
            set
            {
                this.priv = value;
            }
        }
        public string NOTE
        {
            get
            {
                return this.note;
            }
            set
            {
                this.note = value;
            }
        }
        public string TOOL
        {
            get
            {
                return this.tool;
            }
            set
            {
                this.tool = value;
            }
        }
        public string LOC
        {
            get
            {
                return this.loc;
            }
            set
            {
                this.loc = value;
            }
        }
        public string PHONE
        {
            get
            {
                return this.phone;
            }
            set
            {
                this.phone = value;
            }
        }
        public string EMAIL
        {
            get
            {
                return this.email;
            }
            set
            {
                this.email = value;
            }
        }
        public string NAME
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
        public string CNAME
        {
            get
            {
                return this.cname;
            }
            set
            {
                this.cname = value;
            }
        }
        public uint SSRC
        {
            get
            {
                return this.ssrc;
            }
            set
            {
                this.ssrc = value;
            }
        }
        public RtcpSDESChunk()
        {
            this.CNAME = string.Format("{0}@{1}", Environment.UserName, Environment.MachineName);
        }

        ~RtcpSDESChunk()
        {
            Dispose(false);
        }
        public void ParseData(byte[] buffer, ref uint offset)
        {
            this.SSRC = this.getUint(buffer, ref offset);
            while (offset < buffer.Length)
            {
                if (buffer[offset] == 0)
                {
                    break;
                }
                this.type = (ChunkType)buffer[offset];
                offset += 1u;
                uint num = (uint)buffer[offset];
                offset += 1u;
                if (num + offset > (uint)buffer.Length)
                {
                    break;
                }
                ChunkType chunkType = this.type;
                chunkType--;
                switch (chunkType)
                {
                    case (ChunkType)0:
                        this.CNAME = Encoding.UTF8.GetString(buffer, (int)offset, (int)num);
                        break;
                    case ChunkType.SDES_CNAME:
                        this.NAME = Encoding.UTF8.GetString(buffer, (int)offset, (int)num);
                        break;
                    case ChunkType.SDES_NAME:
                        this.EMAIL = Encoding.UTF8.GetString(buffer, (int)offset, (int)num);
                        break;
                    case ChunkType.SDES_EMAIL:
                        this.PHONE = Encoding.UTF8.GetString(buffer, (int)offset, (int)num);
                        break;
                    case ChunkType.SDES_PHONE:
                        this.LOC = Encoding.UTF8.GetString(buffer, (int)offset, (int)num);
                        break;
                    case ChunkType.SDES_LOC:
                        this.TOOL = Encoding.UTF8.GetString(buffer, (int)offset, (int)num);
                        break;
                    case ChunkType.SDES_TOOL:
                        this.NOTE = Encoding.UTF8.GetString(buffer, (int)offset, (int)num);
                        break;
                    case ChunkType.SDES_NOTE:
                        {
                            uint num2 = (uint)buffer[offset];
                            offset += 1u;
                            this.PRIV_Prefix = Encoding.UTF8.GetString(buffer, (int)offset, (int)num2);
                            offset += num2;
                            uint num3 = num - num2;
                            this.PRIV = Encoding.UTF8.GetString(buffer, (int)offset, (int)num3);
                            offset += num3;
                            break;
                        }
                }
                offset += num;
            }
        }
        public void ToByteArray(byte[] buffer, ref uint offset)
        {
            this.setUInt(buffer, ref offset, this.SSRC);
            if (!string.IsNullOrEmpty(this.CNAME))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(this.CNAME);
                buffer[offset] = 1;
                offset += 1u;
                buffer[offset] = (byte)bytes.Length;
                offset += 1u;
                Array.Copy(bytes, 0, buffer, offset, bytes.Length);
                offset = (uint)(offset + bytes.Length);
            }
            if (!string.IsNullOrEmpty(this.NAME))
            {
                byte[] bytes2 = Encoding.UTF8.GetBytes(this.NAME);
                buffer[offset] = 2;
                offset += 1u;
                buffer[offset] = (byte)bytes2.Length;
                offset += 1u;
                Array.Copy(bytes2, 0, buffer, offset, bytes2.Length);
                offset = (uint)(offset + bytes2.Length);
            }
            if (!string.IsNullOrEmpty(this.EMAIL))
            {
                byte[] bytes3 = Encoding.UTF8.GetBytes(this.EMAIL);
                buffer[offset] = 3;
                offset += 1u;
                buffer[offset] = (byte)bytes3.Length;
                offset += 1u;
                Array.Copy(bytes3, 0, buffer, offset, bytes3.Length);
                offset = (uint)(offset + bytes3.Length);
            }
            if (!string.IsNullOrEmpty(this.PHONE))
            {
                byte[] bytes4 = Encoding.UTF8.GetBytes(this.PHONE);
                buffer[offset] = 4;
                offset += 1u;
                buffer[offset] = (byte)bytes4.Length;
                offset += 1u;
                Array.Copy(bytes4, 0, buffer, offset, bytes4.Length);
                offset = (uint)(offset + bytes4.Length);
            }
            if (!string.IsNullOrEmpty(this.LOC))
            {
                byte[] bytes5 = Encoding.UTF8.GetBytes(this.LOC);
                buffer[offset] = 5;
                offset += 1u;
                buffer[offset] = (byte)bytes5.Length;
                offset += 1u;
                Array.Copy(bytes5, 0, buffer, offset, bytes5.Length);
                offset = (uint)(offset + bytes5.Length);
            }
            if (!string.IsNullOrEmpty(this.TOOL))
            {
                byte[] bytes6 = Encoding.UTF8.GetBytes(this.TOOL);
                buffer[offset] = 6;
                offset += 1u;
                buffer[offset] = (byte)bytes6.Length;
                offset += 1u;
                Array.Copy(bytes6, 0, buffer, offset, bytes6.Length);
                offset = (uint)(offset + bytes6.Length);
            }
            if (!string.IsNullOrEmpty(this.PRIV) && !string.IsNullOrEmpty(this.PRIV_Prefix))
            {
                byte[] bytes7 = Encoding.UTF8.GetBytes(this.PRIV);
                byte[] bytes8 = Encoding.UTF8.GetBytes(this.PRIV_Prefix);
                buffer[offset] = 8;
                offset += 1u;
                buffer[offset] = (byte)bytes7.Length;
                offset += 1u;
                buffer[offset] = (byte)bytes8.Length;
                offset += 1u;
                Array.Copy(bytes8, 0, buffer, offset, bytes8.Length);
                offset = (uint)(offset + bytes8.Length);
                Array.Copy(bytes7, 0, buffer, offset, bytes7.Length);
                offset = (uint)(offset + bytes7.Length);
            }
            buffer[offset] = 0;
            offset += 1u;
        }
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Format("##Begin {0}: ", base.GetType().ToString()));
            if (!string.IsNullOrEmpty(this.CNAME))
            {
                stringBuilder.AppendLine(string.Format("CNAME: {0}", this.CNAME));
            }
            if (!string.IsNullOrEmpty(this.NAME))
            {
                stringBuilder.AppendLine(string.Format("NAME: {0}", this.NAME));
            }
            if (!string.IsNullOrEmpty(this.EMAIL))
            {
                stringBuilder.AppendLine(string.Format("EMAIL: {0}", this.EMAIL));
            }
            if (!string.IsNullOrEmpty(this.PHONE))
            {
                stringBuilder.AppendLine(string.Format("PHONE: {0}", this.PHONE));
            }
            if (!string.IsNullOrEmpty(this.LOC))
            {
                stringBuilder.AppendLine(string.Format("LOC: {0}", this.LOC));
            }
            if (!string.IsNullOrEmpty(this.TOOL))
            {
                stringBuilder.AppendLine(string.Format("TOOL: {0}", this.TOOL));
            }
            if (!string.IsNullOrEmpty(this.NOTE))
            {
                stringBuilder.AppendLine(string.Format("NOTE: {0}", this.NOTE));
            }
            if (!string.IsNullOrEmpty(this.PRIV))
            {
                stringBuilder.AppendLine(string.Format("PRIV: {0}", this.PRIV));
            }
            if (!string.IsNullOrEmpty(this.PRIV_Prefix))
            {
                stringBuilder.AppendLine(string.Format("PRIV_Prefix: {0}", this.PRIV_Prefix));
            }
            stringBuilder.AppendLine(string.Format("##End {0}: ", base.GetType().ToString()));
            return stringBuilder.ToString();
        }
        private uint getUint(byte[] buffer, ref uint offset)
        {
            uint num = (uint)((uint)buffer[offset] << 24);
            offset += 1u;
            uint num2 = (uint)((uint)buffer[offset] << 16);
            offset += 1u;
            uint num3 = (uint)((uint)buffer[offset] << 8);
            offset += 1u;
            uint num4 = (uint)buffer[offset];
            offset += 1u;
            return num | num2 | num3 | num4;
        }
        private void setUInt(byte[] buffer, ref uint offset, uint value)
        {
            buffer[offset] = (byte)(value >> 24 | 255u);
            offset += 1u;
            buffer[offset] = (byte)(value >> 16 | 255u);
            offset += 1u;
            buffer[offset] = (byte)(value >> 8 | 255u);
            offset += 1u;
            buffer[offset] = (byte)(value | 255u);
            offset += 1u;
        }
    }

    internal enum ChunkType
    {
        SDES_PRIV = 8,
        SDES_NOTE = 7,
        SDES_TOOL = 6,
        SDES_LOC = 5,
        SDES_PHONE = 4,
        SDES_EMAIL = 3,
        SDES_NAME = 2,
        SDES_CNAME = 1
    }
}
