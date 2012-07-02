using System;
using ToyTools;

namespace ToyTools
{
	class ToyMP3Frame
	{
		//
		// FrameHeader Information
		//
		private int id;
		private int layer;
		private int protection_bit;
		private int bitrate_index;
		private int frequency_index;
		private int padding_bit;
		private int private_bit;
		private int mode;
		private int mode_extention;
		private int copyright;
		private int original;
		private int emphasis;

		private int[] bitrate_table = new int[]
		{
			0, 32, 40, 48, 56, 64, 80, 96, 112,
			128, 160, 192, 224, 256, 320
		};
		private int[] frequency_table = new int[]
		{
			44100, 48000, 32000, 0
		};

		//
		// SitdeTable Information
		//
		private int CRC_check;
		private int main_data_begin;
		private int[,] scfsi = new int[2,4];
		public  GranuleInfo[,] granule = new GranuleInfo[2,2];

		//
		// Main Data
		//
		private byte[] main_data;

		public ToyMP3Frame()
		{
			// initialize
			CRC_check = 0;

			for(int i = 0; i < 2; i++)
			{
				for(int j = 0; j < 2; j++)
				{
					granule[i,j] = new GranuleInfo();
				}
			}
		}




		// Getter and Setter
		public int Id
		{
			get { return id; }
			set { id = value; }
		}
		public int Layer
		{
			get { return layer; }
			set { layer = value; }
		}
		public int ProtectionBit
		{
			get { return protection_bit; }
			set { protection_bit = value; }
		}
		public int BitrateIndex
		{
			get { return bitrate_index; }
			set { bitrate_index = value; }
		}
		public int FrequencyIndex
		{
			get { return frequency_index; }
			set { frequency_index = value; }
		}
		public int PaddingBit
		{
			get { return padding_bit; }
			set { padding_bit = value; }
		}
		public int PrivateBit
		{
			get { return private_bit; }
			set { private_bit = value; }
		}
		public int Mode
		{
			get { return mode; }
			set { mode = value; }
		}
		public int ModeExtention
		{
			get { return mode_extention; }
			set { mode_extention = value; }
		}
		public int Copyright
		{
			get { return copyright; }
			set { copyright = value; }
		}
		public int Original
		{
			get { return original; }
			set { original = value; }
		}
		public int Emphasis
		{
			get { return emphasis; }
			set { emphasis = value; }
		}
		public bool IsValidHeader
		{
			get
			{
				if(
					id != 1 || layer != 1 ||
					bitrate_index < 1 || 
					bitrate_index > 14 ||
					frequency_index < 0 ||
					frequency_index > 2 ||
					emphasis == 2
				)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}
		public int CRCCheck
		{
			get { return CRC_check; }
			set { CRC_check = value; }
		}
		public int Channels
		{
			get
			{
				if(mode == 3)
				{
					return 1;
				}
				else
				{
					return 2;
				}
			}
		}
		public int MainDataBegin
		{
			get { return main_data_begin; }
			set { main_data_begin = value; }
		}
		public int Bitrate
		{
			get
			{
				return bitrate_table[bitrate_index];
			}
		}
		public int SamplingFrequency
		{
			get
			{
				return frequency_table[frequency_index];
			}
		}
		public bool SetScfsi(int ch, int idx, int v)
		{
			scfsi[ch, idx] = v;
			return true;
		}
		public int GetScfsi(int ch, int idx)
		{
			return scfsi[ch, idx];
		}
		public int FrameSize
		{
			get
			{
				int i = (int)(144.0 * Bitrate * 1000 / SamplingFrequency);
				if(PaddingBit == 1) { i++; }
				return i;
			}
		}
		public int MainDataSize
		{
			get
			{
				int i = FrameSize;
				// FrameHeader(4 bytes)
				i -= 4;
				// CRC(1 byte) <- if exists
				if(ProtectionBit == 0)
				{
					i -= 2;
				}
				// SideTable size
				if(Channels == 1)
				{
					i -= 17;
				}
				else
				{
					i -= 32;
				}
				return i;
			}
		}
		public byte[] MainData
		{
			set
			{
				main_data = value;
			}
			get { return main_data; }
		}
		public int SampleNum
		{
			get { return 1152; }
		}
	}
}