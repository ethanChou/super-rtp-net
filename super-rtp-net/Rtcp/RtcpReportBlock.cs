using System;
using System.Text;

namespace super.rtp.net
{
   /// <summary>
   /// http://www.cnblogs.com/frkang/p/3347606.html
   /// </summary>
	public class RtcpReportBlock
	{
        /// <summary>
        /// SSRC_n(ͬ��Դ��ʶ��)��32����   �ڴ˽��ձ��������Ϣ����Դ��SSRC��ʶ����    
        /// </summary>
		private uint ssrc;
        /// <summary>
        /// �����ʣ�8����   �Դ�ǰһSR����RR��������������SSRC_n������RTP���ݰ��Ķ�ʧ������
        /// �Զ���С������ʽ��ʾ����ֵ����Ϊ��ʧ�������������յİ����������ڰ��ظ������°���ʧ��Ϊ��ֵ����������Ϊ�㡣
        /// ע�����յ���һ�����󣬽������޷�֪���Ժ�İ��Ƿ�ʧ���磺������һ�����ձ������ڴ�ĳ��Դ�������������ݰ�����ʧ����ô����Ϊ������Դ���ͽ��ձ���顣
        /// </summary>
		private uint fractionLost;
        /// <summary>
        /// �ۼư���ʧ����24����   �ӿ�ʼ���յ����ڣ���ԴSSRC_n������Դ��RTP���ݰ��Ķ�����������ֵ����Ϊ���������յİ�����ʵ�ʽ��յİ�����
        /// ���յİ������ƵĻ�ٵ��ġ����ڳٵ��İ���������ʧ���ڷ�������ʱ����������Ϊ��ֵ���������յİ�������Ϊ����չ����һ�������(�����)��ȥ���������š�    
        /// </summary>
		private uint cumulativePacketLost;
        /// <summary>
        /// ���յ�����չ��������кţ�32����   ��16���ذ�����ԴSSRC_n������߽������кţ���16��������Ӧ�����к����ڼ�������չ�����кš�
        /// ע����ͬһ�����еĲ�ͬ�����ߣ�������ʱ�����Բ�ͬ����������ͬ����չ�
        /// </summary>
		private uint extendedHighestSequenceNumberReceived;
        /// <summary>
        ///  ������������32����   RTP���ݰ�����ʱ��ͳ�Ʒ���Ĺ���ֵ��������λͬʱ�����λ�����޷�������������ʱ�䶶������Ϊһ�԰��н�������Է����ߵ�ʱ������ֵ��ƽ��ƫ��(ƽ����ľ���ֵ)��
        ///  �����µ�ʽ��ʾ����ֵ������������Դ���ʱ��Ĳ�ֵ����Դ���ʱ����ָ������RTPʱ����͵���ʱ�̽�����ʱ��ʱ��Ĳ�ֵ��
        ///  ��Si�ǰ�i�е�RTPʱ�����Ri�ǰ�i����ʱ�̣���λΪ��RTPʱ�����λ��������������i��j��D���Ա�ʾΪ  D(i��j)=(Rj-Sj)-(Ri-Si)��
        ///  ����ʱ�̶����������յ���ԴSSRC_n����ÿ�����ݰ�i���������㡣���øð���ǰһ��i-1��ƫ��D(������˳�򣬶������˳��)�����ݹ�ʽJ=J+(|D(i-1��i)|-J)/16���㡣���ۺ�ʱ���ͽ��ձ��棬���õ�ǰ��Jֵ��
        ///  �˴������Ķ�������������Э������ļ����������Բ�ͬʵ�ֵı��������Ч�Ľ��͡�    
        /// </summary>
		private uint interArrivalJitter;
        /// <summary>
        /// ��һSR����   (LSR)��32����   ���յ�������ԴSSRC_n������RTCP�����߱���(SR)��64λNTPʱ���־���м�32λ������û�н��յ�SR������ֵΪ�㡣
        /// </summary>
		private uint lastSRTimestamp;
        /// <summary>
        /// ����һSR��ʱ��(DLSR)��32����   �Ǵ��յ�����SSRC_n��SR�������ʹ˽��ձ����֮�����ʱ����1/65536��Ϊ��λ������δ�յ�����SSRC_n��SR��������ֵΪ�㡣    
        /// </summary>
		private uint delayLastSR;

		public uint DelayLastSR
		{
			get
			{
				return this.delayLastSR;
			}
			set
			{
				this.delayLastSR = value;
			}
		}
		public uint LastSRTimestamp
		{
			get
			{
				return this.lastSRTimestamp;
			}
			set
			{
				this.lastSRTimestamp = value;
			}
		}
		public uint InterArrivalJitter
		{
			get
			{
				return this.interArrivalJitter;
			}
			set
			{
				this.interArrivalJitter = value;
			}
		}
		public uint ExtendedHighestSequenceNumberReceived
		{
			get
			{
				return this.extendedHighestSequenceNumberReceived;
			}
			set
			{
				this.extendedHighestSequenceNumberReceived = value;
			}
		}
		public uint CumulativePacketLost
		{
			get
			{
				return this.cumulativePacketLost;
			}
			set
			{
				this.cumulativePacketLost = value;
			}
		}
		public uint FractionLost
		{
			get
			{
				return this.fractionLost;
			}
			set
			{
				this.fractionLost = value;
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
		public void ParseData(byte[] buffer, ref uint offset)
		{
			this.SSRC = this.getUint(buffer, ref offset);
			uint fractionLost = (uint)buffer[offset];
			offset += 1u;
			this.FractionLost = fractionLost;
			uint num = (uint)((uint)buffer[offset] << 24);
			offset += 1u;
			uint num2 = (uint)((uint)buffer[offset] << 16);
			offset += 1u;
			uint num3 = (uint)((uint)buffer[offset] << 8);
			offset += 1u;
			this.CumulativePacketLost = (num | num2 | num3);
			this.ExtendedHighestSequenceNumberReceived = this.getUint(buffer, ref offset);
			this.InterArrivalJitter = this.getUint(buffer, ref offset);
			this.LastSRTimestamp = this.getUint(buffer, ref offset);
			this.DelayLastSR = this.getUint(buffer, ref offset);
		}
		public void ToByteArray(byte[] buffer, ref uint offset)
		{
			this.setUInt(buffer, ref offset, this.SSRC);
			buffer[offset] = (byte)(this.FractionLost & 255u);
			offset += 1u;
			buffer[offset] = (byte)(this.CumulativePacketLost >> 24 | 255u);
			offset += 1u;
			buffer[offset] = (byte)(this.CumulativePacketLost >> 16 | 255u);
			offset += 1u;
			buffer[offset] = (byte)(this.CumulativePacketLost >> 8 | 255u);
			offset += 1u;
			this.setUInt(buffer, ref offset, this.ExtendedHighestSequenceNumberReceived);
			this.setUInt(buffer, ref offset, this.InterArrivalJitter);
			this.setUInt(buffer, ref offset, this.LastSRTimestamp);
			this.setUInt(buffer, ref offset, this.DelayLastSR);
		}
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("##Begin {0}: ", base.GetType().ToString()));
			stringBuilder.AppendLine(string.Format("SSRC {0}", this.SSRC));
			stringBuilder.AppendLine(string.Format("FractionLost {0}", this.FractionLost));
			stringBuilder.AppendLine(string.Format("CumulativePacketLost {0}", this.CumulativePacketLost));
			stringBuilder.AppendLine(string.Format("ExtendedHighestSequenceNumberReceived {0}", this.ExtendedHighestSequenceNumberReceived));
			stringBuilder.AppendLine(string.Format("InterArrivalJitter {0}", this.InterArrivalJitter));
			stringBuilder.AppendLine(string.Format("LastSRTimestamp {0}", this.LastSRTimestamp));
			stringBuilder.AppendLine(string.Format("DelayLastSR {0}", this.DelayLastSR));
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
}
