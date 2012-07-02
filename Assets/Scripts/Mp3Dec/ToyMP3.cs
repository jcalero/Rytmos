using System;
using System.IO;
using ToyTools;

namespace ToyTools
{
	class ToyMP3
	{
		private BitStream     bs;
		private int           frame_seek_limit;

		public ToyMP3(string filename)
		{
			bs = new BitStream();
			bs.OpenFile(filename);
			frame_seek_limit = 40960;
		}

		// Seek Mpeg Frame Header and gether information
		public bool SeekMP3Frame(ToyMP3Frame frame)
		{
			for(;;)
			{
				for(int i=0; ; i++)
				{
					// search syncword
					if(bs.GetByInt(12) != 4095)
					{
						if(i > frame_seek_limit)
						{
							return false;
						}
						// back 1 byte (in order to check byte by byte)
						bs.Skip(-4);
						continue;
					}
					else
					{
						break;
					}
				}

				// Data must be casted into correct type.
				frame.Id =             bs.GetByInt(1);
				frame.Layer =          bs.GetByInt(2);
				frame.ProtectionBit =  bs.GetByInt(1);
				frame.BitrateIndex =   bs.GetByInt(4);
				frame.FrequencyIndex = bs.GetByInt(2);
				frame.PaddingBit =     bs.GetByInt(1);
				frame.PrivateBit =     bs.GetByInt(1);
				frame.Mode =           bs.GetByInt(2);
				frame.ModeExtention =  bs.GetByInt(2);
				frame.Copyright =      bs.GetByInt(1);
				frame.Original =       bs.GetByInt(1);
				frame.Emphasis =       bs.GetByInt(2);
				if(frame.IsValidHeader)
				{
					// escaping from this loop
					break;
				}
				else
				{
					// Step back 3 bytes, and continue seeking.
					bs.Skip(-24);
				}
			}
			// CRC word
			if(frame.ProtectionBit == 0)
			{
				frame.CRCCheck = bs.GetByInt(16);
			}
			else
			{
				frame.CRCCheck = 0;
			}
			if(!DecodeSideTableInformation(frame))
			{
				return false;
			}
			// Format check
			if(frame.Id != 1 || frame.Layer != 1 || frame.BitrateIndex == 0)
			{
				return false;
			}
			// MainData
			if(frame.MainDataSize < 0)
			{
				return false;
			}
			frame.MainData = bs.GetByByteArray(frame.MainDataSize);
			
			return true;
		}

		private bool DecodeSideTableInformation(ToyMP3Frame frame)
		{
			frame.MainDataBegin = bs.GetByInt(9);
			// Skipping Private Bits
			if(frame.Channels == 1)
			{
				bs.Skip(5);
			}
			else
			{
				bs.Skip(3);
			}

			// Get Scfsi
			for(int ch = 0; ch < frame.Channels; ch++)
			{
				for(int i = 0; i < 4; i++)
				{
					frame.SetScfsi(ch, i, bs.GetByInt(1));
				}
			}
			// Get Granule Info
			for(int g = 0; g < 2; g++)
			{
				for(int ch = 0; ch < frame.Channels; ch++)
				{
					frame.granule[ch, g].Part23Length        = bs.GetByInt(12);
					frame.granule[ch, g].BigValues           = bs.GetByInt(9);
					frame.granule[ch, g].GlobalGain          = bs.GetByInt(8);
					frame.granule[ch, g].ScalefacCompress    = bs.GetByInt(4);
					frame.granule[ch, g].WindowSwitchingFlag = bs.GetByInt(1);
					if(frame.granule[ch, g].WindowSwitchingFlag == 1)
					{
						frame.granule[ch, g].BlockType = bs.GetByInt(2);
						frame.granule[ch, g].MixedBlockFlag = bs.GetByInt(1);
						if(frame.granule[ch, g].BlockType == 0)
						{
							return false;
						}

						for(int w = 0; w < 2; w++)
						{
							frame.granule[ch, g].SetTableSelect(w, bs.GetByInt(5));
						}

						for(int w = 0; w < 3; w++)
						{
							frame.granule[ch, g].SetSubblockGain(w, bs.GetByInt(3));
						}

						if(
							(frame.granule[ch, g].BlockType == 2) &&
							(frame.granule[ch, g].MixedBlockFlag == 0))
						{
							frame.granule[ch, g].Region0Count= 8;
							frame.granule[ch, g].Region1Count= 36;
						}
						else
						{
							frame.granule[ch, g].Region0Count = 7;
							frame.granule[ch, g].Region1Count = 36;
						}
					}
					else
					{
						for(int w = 0; w < 3; w++)
						{
							frame.granule[ch, g].SetTableSelect(w, bs.GetByInt(5));
						}
						frame.granule[ch, g].Region0Count = bs.GetByInt(4);
						frame.granule[ch, g].Region1Count = bs.GetByInt(3);
						frame.granule[ch, g].MixedBlockFlag = 0;
						frame.granule[ch, g].BlockType = 0;
					}
					frame.granule[ch, g].PreFlag = bs.GetByInt(1);
					frame.granule[ch, g].ScalefacScale = bs.GetByInt(1);
					frame.granule[ch, g].Count1TableSelect = bs.GetByInt(1);
				}
			}
			return true;
		}

		public BitStream Bs
		{
			get { return bs; }
		}
	}
}