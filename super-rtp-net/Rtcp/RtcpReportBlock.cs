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
        /// SSRC_n(同步源标识符)：32比特   在此接收报告块中信息所属源的SSRC标识符。    
        /// </summary>
		private uint ssrc;
        /// <summary>
        /// 丢包率：8比特   自从前一SR包或RR包发送以来，从SSRC_n传来的RTP数据包的丢失比例。
        /// 以定点小数的形式表示。该值定义为损失包数／期望接收的包数。若由于包重复而导致包丢失数为负值，丢包率设为零。
        /// 注意在收到上一个包后，接收者无法知道以后的包是否丢失。如：若在上一个接收报告间隔内从某个源发出的所有数据包都丢失，那么将不为此数据源发送接收报告块。
        /// </summary>
		private uint fractionLost;
        /// <summary>
        /// 累计包丢失数：24比特   从开始接收到现在，从源SSRC_n发到本源的RTP数据包的丢包总数。该值定义为：期望接收的包数－实际接收的包数。
        /// 接收的包括复制的或迟到的。由于迟到的包不算作损失，在发生复制时丢包数可能为负值。期望接收的包数定义为：扩展的上一接收序号(随后定义)减去最初接收序号。    
        /// </summary>
		private uint cumulativePacketLost;
        /// <summary>
        /// 接收到的扩展的最高序列号：32比特   低16比特包含从源SSRC_n来的最高接收序列号，高16比特用相应的序列号周期计数器扩展该序列号。
        /// 注意在同一会议中的不同接收者，若启动时间明显不同，将产生不同的扩展项。
        /// </summary>
		private uint extendedHighestSequenceNumberReceived;
        /// <summary>
        ///  到达间隔抖动：32比特   RTP数据包到达时刻统计方差的估计值。测量单位同时间戳单位，用无符号整数表达。到达时间抖动定义为一对包中接收者相对发送者的时间间隔差值的平均偏差(平滑后的绝对值)。
        ///  如以下等式所示，该值等于两个包相对传输时间的差值。相对传输时间是指：包的RTP时间戳和到达时刻接收者时钟时间的差值。
        ///  若Si是包i中的RTP时间戳，Ri是包i到达时刻（单位为：RTP时间戳单位）。对于两个包i和j，D可以表示为  D(i，j)=(Rj-Sj)-(Ri-Si)；
        ///  到达时刻抖动可以在收到从源SSRC_n来的每个数据包i后连续计算。利用该包和前一包i-1的偏差D(按到达顺序，而非序号顺序)，根据公式J=J+(|D(i-1，i)|-J)/16计算。无论何时发送接收报告，都用当前的J值。
        ///  此处描述的抖动计算允许与协议独立的监视器对来自不同实现的报告进行有效的解释。    
        /// </summary>
		private uint interArrivalJitter;
        /// <summary>
        /// 上一SR报文   (LSR)：32比特   接收到的来自源SSRC_n的最新RTCP发送者报告(SR)的64位NTP时间标志的中间32位。若还没有接收到SR，该域值为零。
        /// </summary>
		private uint lastSRTimestamp;
        /// <summary>
        /// 自上一SR的时间(DLSR)：32比特   是从收到来自SSRC_n的SR包到发送此接收报告块之间的延时，以1/65536秒为单位。若还未收到来自SSRC_n的SR包，该域值为零。    
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
