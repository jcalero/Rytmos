using System;
using System.Collections.Generic;
using System.Text;
using ToyTools;


namespace ToyTools
{
	partial class ToyMP3Decoder
	{
		// Data buffer to keep compressed audio data.
		private BitStream bs;
		private int buffer_size = 8192*8*8;

		// Scalefactor
		private ScalefactorBandIndex[] ScfBandIndex;
		private Scalefactor[,] Scf;
		private ToyMP3Frame frame;
		private int[,] _is = new int[2, 576];
		public double[,] xr = new double[2, 576];

		// HuffmanTable
		public HuffmanTableQ[] hfq = new HuffmanTableQ[4];
		public HuffNodeQ[] HuffmanTreeQ = new HuffNodeQ[32];
		public HuffmanTable[] hf = new HuffmanTable[32];
		public HuffNode[] HuffmanTree = new HuffNode[32];

		// Samplemap
		private Sample[,] SampleMap     = new Sample[2, 576];
		private int BLOCK_SHORT = 0;
		private int BLOCK_LONG  = 1;
		private int[] PreemphasisTable = new int[]{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 1, 1, 1, 1, 2, 2, 3, 3, 3,
			2, 0
		};

		// Initializing misc variable.
		private bool       InversedMDCTInitializedFlag = false;
		private bool       SubbandSynthesysInitFlag    = false;
		private bool       AntialiasInitializedFlag    = false;

		private double[]   g_cs            = new double[8];
		private double[]   g_ca            = new double[8];
		private double[,]  SineWindow      = new double[4, 36];
		private double[,]  pfb             = new double[2, 576];
		private double[,]  pfbOut          = new double[2, 576];
		private double[,]  ImdctPrevRawout = new double[2,576];
		private double[,,] SubbandBuf      = new double[2, 16, 64];
		private int[]      SubbandBufIndex = new int[2];
		private double[,]  _Cos            = new double[64,32];

		// Counter of clipped data(too big/small)
		private int clip=0;

		// PCM data
		private Int16[] pcm = new Int16[1152*2];

		public bool DecodeFrame(ToyMP3Frame f)
		{
			frame = f;
			int PcmPoint = 0;

			// Adding data to the buffer
			bs.AddByteArray(frame.MainData);

			// Checking the buffer size and removing used data.
			if(bs.Length > buffer_size)
			{
				bs.RemoveRange(
					0,
					bs.Length/8 - (frame.MainData.Length + frame.MainDataBegin)
					);
			}
			// Checking if position of the buffer to read exists.
			if(frame.MainDataBegin > (bs.Length/8 - frame.MainDataSize))
			{
				return false;
			}

			// Set reading location 
			bs.Position = bs.Length-(frame.MainData.Length*8 + frame.MainDataBegin*8);

			// 1ã¤ã®ãƒ•ãƒ¬ãƒ¼ãƒ ã¯2å€‹ã®ã‚°ãƒ©ãƒ‹ãƒ¥ãƒ¼ãƒ«ã‚»ãƒƒãƒˆã‚’æŒã¤ã€‚
			for(int gr=0; gr<2; gr++)
			{
				// Insie of granule-set, there exists glanule*frame.Channels
				for(int ch=0; ch<frame.Channels; ch++)
				{
					// End position of "Part2~3" (Scalefactor + Huffmancode)
					int p23_end_pos = bs.Position+frame.granule[ch, gr].Part23Length;

					DecodeScalefactor(frame.granule[ch,gr], ch, gr);
					if(!DecodeHuffmanCode(ch, gr, p23_end_pos))
					{
						return false;
					}
					bs.Position = p23_end_pos;
				}
				
				// Generate sample map
				for(int ch=0; ch<frame.Channels; ch++)
				{
					CreateSampleMap(ch,gr);
				}
				// Dequantize(and reorder)
				for(int ch=0; ch<frame.Channels; ch++)
				{
					Dequantize(ch,gr);
				}

				// Joint Stereo decode
				JointStereoDecode();

				// AntiAlias
				for(int ch=0; ch<frame.Channels; ch++)
				{
					Antialias(ch,gr);
				}

				// InverseMDCTSynthesys
				for(int ch=0; ch<frame.Channels; ch++)
				{
					InverseMDCTSynthesys(ch, gr);
				}

				// SubbandSynthesys
				for(int ch=0; ch<frame.Channels; ch++)
				{
					SubbandSynthesys(ch);
				}

				// Generate PCM
				CreatePcm(gr);
				PcmPoint += 576 * frame.Channels;
				
			}
			//PcmDataNum = PcmPoint;
			return true;
		}

		private void DecodeScalefactor(GranuleInfo granule, int ch, int gr)
		{
			int Slen1 = granule.Slen[0];
			int Slen2 = granule.Slen[1];

			// Mixed Block
			if(granule.IsMixedBlock)
			{
				for(int sfb=0; sfb<8; sfb++)
				{
					Scf[ch,gr].LongBlock[sfb]=bs.GetByInt(Slen1);
				}
				for(int sfb=3; sfb<6; sfb++)
				{
					for(int w=0; w<3; w++)
					{
						Scf[ch,gr].ShortBlock[w,sfb] = bs.GetByInt(Slen1);
					}
				}
				for(int sfb=6; sfb<12; sfb++)
				{
					for(int w=0; w<3; w++)
					{
						Scf[ch,gr].ShortBlock[w,sfb] = bs.GetByInt(Slen2);
					}
				}
			}
			// Short Block
			else if(granule.IsShortBlock)
			{
				for(int sfb=0; sfb<6; sfb++)
				{
					for(int w=0; w<3; w++)
					{
						Scf[ch,gr].ShortBlock[w,sfb] = bs.GetByInt(Slen1);
					}
				}
				for(int sfb=6; sfb<12; sfb++)
				{
					for(int w=0; w<3; w++)
					{
						Scf[ch,gr].ShortBlock[w,sfb] = bs.GetByInt(Slen2);
					}
				}
			}
			// Long Block
			else if(granule.IsLongBlock)
			{
				// In case granule0,
				// without exception, data should be taken from bitstream.
				if(gr == 0)
				{
					for(int sfb=0; sfb<11; sfb++)
					{
						Scf[ch,gr].LongBlock[sfb] = bs.GetByInt(Slen1);
					}
					for(int sfb=11; sfb<21; sfb++)
					{
						Scf[ch,gr].LongBlock[sfb] = bs.GetByInt(Slen2);
					}
				}
				else
				{
					// When processing granule1 and SCFSI is 1,
					// sdalefactor will shared with granule0.
					// Else, scalefactor should be got from bitstream.
					if(frame.GetScfsi(ch,0) == 1)
					{
						for(int sfb=0; sfb<6; sfb++)
						{
							Scf[ch,gr].LongBlock[sfb] = Scf[ch,0].LongBlock[sfb];
						}
					}
					else
					{
						for(int sfb=0; sfb<6; sfb++)
						{
							Scf[ch,gr].LongBlock[sfb] = bs.GetByInt(Slen1);
						}
					}
					if(frame.GetScfsi(ch,1) == 1)
					{
						for(int sfb=6; sfb<11; sfb++)
						{
							Scf[ch,gr].LongBlock[sfb] = Scf[ch,0].LongBlock[sfb];
						}
					}
					else
					{
						for(int sfb=6; sfb<11; sfb++)
						{
							Scf[ch, gr].LongBlock[sfb] = bs.GetByInt(Slen1);
						}
					}
					if(frame.GetScfsi(ch,2) == 1)
					{
						for(int sfb=11; sfb<16; sfb++)
						{
							Scf[ch,gr].LongBlock[sfb] = Scf[ch,0].LongBlock[sfb];
						}
					}
					else
					{
						for(int sfb=11; sfb<16; sfb++)
						{
							Scf[ch, gr].LongBlock[sfb] = bs.GetByInt(Slen2);
						}
					}
					if(frame.GetScfsi(ch, 3) == 1)
					{
						for(int sfb=16; sfb<21; sfb++)
						{
							Scf[ch,gr].LongBlock[sfb] = Scf[ch, 0].LongBlock[sfb];
						}
					}
					else
					{
						for(int sfb=16; sfb<21; sfb++)
						{
							Scf[ch,gr].LongBlock[sfb] = bs.GetByInt(Slen2);
						}
					}
				}
			}
		}

		private bool DecodeHuffmanCode(int ch, int gr, int endpos)
		{
			int idx = 0;
			// Reading and decoding BigValue.
			int Region1Start, Region2Start;
			GranuleInfo f_granule = frame.granule[ch, gr];

			if(f_granule.BlockType == 0)
			{
				Region1Start = ScfBandIndex[frame.FrequencyIndex].
					LongBlock[f_granule.Region0Count + 1];
				Region2Start = ScfBandIndex[frame.FrequencyIndex].
					LongBlock[f_granule.Region0Count + f_granule.Region1Count + 2];
			}
			else
			{
				Region1Start = 36;
				Region2Start = 576;
			}

			for(;idx <= 576-2 && idx<f_granule.BigValues*2
				;idx+=2)
			{
				int tbl;
				// Region0
				if(idx<Region1Start)
				{
					tbl=f_granule.GetTableSelect(0);
				}
				// Region1
				else if(idx<Region2Start)
				{
					tbl=f_granule.GetTableSelect(1);
				}
				// Region2
				else
				{
					tbl=f_granule.GetTableSelect(2);
				}
				LookupHuffman(ch, tbl, idx);
			}

			// Reading and decoding Count1
			for
			(
				; (idx<=576-4) && (bs.Position<endpos)
				; idx+=4
			)
			{
				LookupHuffmanQ(ch, f_granule.Count1TableSelect, idx);
			}

			for(; idx<576; idx++)
			{
				_is[ch,idx]=0;
			}
			return true;
		}

		private void LookupHuffman(int ch, int tbl, int idx)
		{
			var q = 0;
			var tmp_x = 0;
			var tmp_y = 0;
			var tmp_l = 0;
			var node = HuffmanTree[tbl];

			if(tbl != 0)
			{
				// Lookup in Huffman table
				for(int i = 0; i < 18; i++)
				{
					if(i == 18)
					{
						Console.WriteLine("Failed to decode Huffman code!");
						throw new Exception("Failed to decode huffman code.");
					}
					q = bs.GetByInt(1);
					if(node.n[q].X != -1 && node.n[q].Y != -1)
					{
						tmp_x = node.n[q].X;
						tmp_y = node.n[q].Y;
						tmp_l = linbits[tbl];
						break;
					}
					else
					{
						node = node.n[q];
					}
				}
			}
			else
			{
				tmp_x=0;tmp_y=0;tmp_l=0;
			}

			if(tmp_l>0 && tmp_x==15)
			{
				tmp_x += bs.GetByInt(tmp_l);
			}
			if(tmp_x>0 && bs.GetByInt(1)!=0)
			{
				tmp_x = -tmp_x;
			}
			if(tmp_l>0 && tmp_y==15)
			{
				tmp_y += bs.GetByInt(tmp_l);
			}
			if(tmp_y>0 && bs.GetByInt(1)!=0)
			{
				tmp_y = -tmp_y;
			}
			_is[ch,idx]   = tmp_x;
			_is[ch,idx+1] = tmp_y;
		}

		private void LookupHuffmanQ(int ch, int tbl, int idx)
		{
			var q = 0;
			var tmp_v = 0;
			var tmp_w = 0;
			var tmp_x = 0;
			var tmp_y = 0;
			var node = HuffmanTreeQ[tbl];

			for(int i = 0; i < 7; i++)
			{
				if(i == 7)
				{
					Console.WriteLine("Failed to decode Huffman code!");
					throw new Exception("Failed to decode huffman code.");
				}

				q = bs.GetByInt(1);
				if(node.n[q].X != -1)
				{
					tmp_v = node.n[q].V;
					tmp_w = node.n[q].W;
					tmp_x = node.n[q].X;
					tmp_y = node.n[q].Y;
					break;
				}
				else
				{
					node = node.n[q];
				}
			}
			if(tmp_v > 0 && bs.GetByInt(1) != 0)
			{
				tmp_v = -tmp_v;
			}
			if(tmp_w > 0 && bs.GetByInt(1) != 0)
			{
				tmp_w = -tmp_w;
			}
			if(tmp_x > 0 && bs.GetByInt(1) != 0)
			{
				tmp_x = -tmp_x;
			}
			if(tmp_y > 0 && bs.GetByInt(1) != 0)
			{
				tmp_y = -tmp_y;
			}
			_is[ch,idx]   = tmp_v;
			_is[ch,idx+1] = tmp_w;
			_is[ch,idx+2] = tmp_x;
			_is[ch,idx+3] = tmp_y;
		}

		private void CreateSampleMap(int ch, int gr)
		{
			int idx=0;
			GranuleInfo _gr=frame.granule[ch, gr];
			// Short Block
			if(_gr.IsShortBlock)
			{
				int[] ScfbIdx = ScfBandIndex[frame.FrequencyIndex].ShortBlock;

				for(int _sbi=0; _sbi<13; _sbi++)
				{
					int idx_s = idx;
					for(int _sub=0; _sub<3; _sub++)
					{
						int CriticalBandWidth=ScfbIdx[_sbi+1]-ScfbIdx[_sbi];
						for(int i=0; i<CriticalBandWidth; i++)
						{
							SampleMap[ch,idx] = new Sample();
							SampleMap[ch,idx].BlockType = BLOCK_SHORT;
							SampleMap[ch,idx].Subblock = _sub;
							SampleMap[ch,idx].Scale =
								0.5*(_gr.ScalefacScale+1.0)*
								Scf[ch,gr].ShortBlock[_sub,_sbi];
							SampleMap[ch, idx].OrderIndex =
								3*((idx-idx_s)%CriticalBandWidth)+
								(idx-idx_s)/CriticalBandWidth+idx_s;
							idx++;
						}
					}
				}
			}
			// Long Block
			else if(_gr.IsLongBlock)
			{
				int[] ScfbIdx = ScfBandIndex[frame.FrequencyIndex].LongBlock;
				int Scfb = 0;

				for(int i=0; i<576; i++)
				{
					SampleMap[ch,idx] = new Sample();
					SampleMap[ch,idx].BlockType =  BLOCK_LONG;
					SampleMap[ch,idx].Subblock = 0;
					SampleMap[ch,idx].OrderIndex = idx;
					if(idx >= ScfbIdx[Scfb+1])
					{
						Scfb++;
					}

					int _Scf = Scf[ch, gr].LongBlock[Scfb];
					if(_gr.PreFlag==1)
					{
						_Scf += PreemphasisTable[Scfb];
					}
					SampleMap[ch,idx].Scale =
						0.5*(_gr.ScalefacScale+1.0)*_Scf;
					idx++;
				}
			}
			// MixedBlock
			else
			{
				// LongBlock sample
				int[] ScfbIdx = ScfBandIndex[frame.FrequencyIndex].LongBlock;
				int Scfb = 0;
				for(int i=0; i<36; i++)
				{
					SampleMap[ch,idx] = new Sample();
					SampleMap[ch,idx].BlockType = BLOCK_LONG;
					SampleMap[ch,idx].Subblock = 0;
					SampleMap[ch,idx].OrderIndex = idx;
					if(idx >= ScfbIdx[Scfb + 1])
					{
						Scfb++;
					}

					int _Scf=Scf[ch,gr].LongBlock[Scfb];
					if(_gr.PreFlag==1)
					{
						_Scf += PreemphasisTable[Scfb];
					}
					SampleMap[ch, idx].Scale =
						0.5*(_gr.ScalefacScale+1.0)*_Scf;
					idx++;
				}

				// Short Block Samples
				ScfbIdx = ScfBandIndex[frame.FrequencyIndex].ShortBlock;

				for(Scfb=3; Scfb<13; Scfb++)
				{
					int idx_s=idx;
					for(int _sub=0; _sub<3; _sub++)
					{
						int CriticalBandWidth =
							ScfbIdx[Scfb + 1] - ScfbIdx[Scfb];
						for(int i=0; i<CriticalBandWidth; i++)
						{
							SampleMap[ch, idx].BlockType = BLOCK_SHORT;
							SampleMap[ch, idx].Subblock = _sub;
							int _Scf = Scf[ch, gr].ShortBlock[_sub, Scfb];
							SampleMap[ch, idx].Scale =
								0.5*(_gr.ScalefacScale+1.0)*_Scf;
							SampleMap[ch, idx].OrderIndex =
								3*((idx-idx_s)%CriticalBandWidth)+
								(idx-idx_s)/CriticalBandWidth+idx_s;
							idx++;
						}
					}
				}
			}
		}

		private void Dequantize(int ch, int gr)
		{
			GranuleInfo _gr = frame.granule[ch, gr];
			double Gain = 0.25*(_gr.GlobalGain-210.0);
			for(int i=0; i<576; i ++)
			{
				if(SampleMap[ch, i].BlockType == BLOCK_SHORT)
				{
					Gain -= -2.0*_gr.GetSubblockGain(SampleMap[ch, i].Subblock);
				}

				xr[ch, SampleMap[ch,i].OrderIndex] = _is[ch, i]*
					Math.Pow(Math.Abs(_is[ch, i]), 1.0/3.0)*
					Math.Pow(2.0, Gain-SampleMap[ch, i].Scale);
			}
		}

		private void JointStereoDecode()
		{
			bool IsMSStereo = ((frame.Mode == 1) && (frame.ModeExtention >= 2));
			bool IsIStereo = (frame.ModeExtention == 1 || frame.ModeExtention == 3);

			int _IsIdx = 576;

			// I Stereo (is not implemented)
			if(IsIStereo)
			{
				Console.WriteLine("I-Stereo!!");
				throw new Exception("I-Stereo is not suported.");
			}
			if(IsMSStereo)
			{
				ProcessMSStereo(_IsIdx);
			}
		}

		private void ProcessMSStereo(int _IsIdx)
		{
			for(int i=0; i<_IsIdx; i++)
			{
				double l = (xr[0,i]+xr[1,i])/1.41421356;
				double r = (xr[0,i]-xr[1,i])/1.41421356;
				xr[0,i] = l;
				xr[1,i] = r;
			}
		}

		private void Antialias(int ch, int gr)
		{
			if(!AntialiasInitializedFlag)
			{
				InitAntialias();
				AntialiasInitializedFlag = true;
			}

			GranuleInfo _gr = frame.granule[ch, gr];
			if(_gr.IsShortBlock)
			{
				return;
			}
			int nl;
			if(_gr.IsLongBlock)
			{
				nl = 31;
			}
			else
			{
				nl = 1;
			}
			for(int k = 0; k < nl; k++)
			{
				for(int i = 0; i < 8; i++)
				{
					double a0 = xr[ch, (k*18)+18+i];
					double b0 = xr[ch, (k*18)+17-i];
					xr[ch, (k*18)+18+i] = a0*g_cs[i] + b0*g_ca[i];
					xr[ch, (k*18)+17-i] = b0*g_cs[i] - a0*g_ca[i];
				}
			}
		}

		private void InitAntialias()
		{
			var fCi = new double[8]{
				-0.6,-0.535,-0.33,-0.185,-0.095,-0.041,-0.0142,-0.0037
			};

			for(int i=0; i<8; i++)
			{
				double sq = Math.Sqrt(1.0 + fCi[i]*fCi[i]);
				g_cs[i] = 1.0/sq;
				g_ca[i] = fCi[i]/sq;
			}
		}

		private void InverseMDCTSynthesys(int ch, int gr)
		{
			// Initialize
			if(!InversedMDCTInitializedFlag)
			{
				InitIMDCT();
				InversedMDCTInitializedFlag = true;
			}

			GranuleInfo _gr = frame.granule[ch, gr];
			int LongSubbandNum = 0;
			int WindowTypeLong = 0;

			if(_gr.IsLongBlock)
			{
				LongSubbandNum = 32;
				WindowTypeLong = _gr.BlockType;
			}
			else if(_gr.IsShortBlock)
			{
				LongSubbandNum = 0;
				WindowTypeLong = -1;
			}
			else if(_gr.IsMixedBlock)
			{
				LongSubbandNum = 2;
				WindowTypeLong = 0;
			}

			int _Subband = 0;
			int _Index = 0;

			// Long (and mixed) Block
			for(_Subband=0; _Subband<LongSubbandNum; _Subband++)
			{
				double[] Rawout = new double[36];
				for(int i=0; i<36; i++)
				{
					double _Sum = 0.0;
					for(int j=0; j<18; j++)
					{
						_Sum += xr[ch, j+_Subband*18]*Math.Cos
							(
								Math.PI/72.0*(2.0*i + 19.0)*(2.0*j + 1.0)
							);
					}
					Rawout[i] = _Sum * SineWindow[WindowTypeLong, i];
				}

				// Combine width last granule and saving;
				for(int ss=0; ss<18; ss++)
				{
					// First half of data + Second half of prev data.
					pfb[ch,_Index] = Rawout[ss] + ImdctPrevRawout[ch,_Index];
					// Keep second half of data for next granule.
					ImdctPrevRawout[ch, _Index] = Rawout[ss+18];
					_Index++;
				}
			}

			// ShortBlock
			for(; _Subband<32; _Subband++)
			{
				double[] Rawout = new double[36];
				for(int _Subblock=0; _Subblock<3; _Subblock++)
				{
					double[] RawoutTmp=new double[12];

					for(int i=0; i<12; i++)
					{
						double _Sum=0;
						for(int j=0; j<6; j++)
						{
							_Sum += xr[ch,_Subband*18+j*3+_Subblock]*
								Math.Cos(Math.PI/24.0*(2.0*i+7.0)*(2.0*j+1.0));
						}
						RawoutTmp[i] = _Sum*SineWindow[2,i];
					}

					for(int i=0; i<12; i++)
					{
						Rawout[6*_Subblock+i+6] += RawoutTmp[i];
					}
				}

				for(int ss=0; ss<18; ss++)
				{
					pfb[ch,_Index] = Rawout[ss]+ImdctPrevRawout[ch, _Index];
					ImdctPrevRawout[ch,_Index] = Rawout[ss+18];

					_Index++;
				}
			}
		}

		private void InitIMDCT()
		{
			for(int i=0; i<36; i++)
			{
				for(int j=0; j<4; j++)
				{
					SineWindow[j,i] = 0;
				}
			}
			for(int i=0; i<36; i++)
			{
				SineWindow[0,i] = Math.Sin(Math.PI/36*(i+0.5));
			}
			for(int i=0; i<18; i ++)
			{
				SineWindow[1,i] = Math.Sin(Math.PI/36*(i+0.5));
			}
			for(int i=18; i<24; i++)
			{
				SineWindow[1,i] = 1.0;
			}
			for(int i=24; i<30; i++)
			{
				SineWindow[1,i] = Math.Sin(Math.PI/12*(i+0.5-18));
			}
			for(int i=30; i<36; i++)
			{
				SineWindow[1,i] = 0.0;
			}

			// Longblock TYPE3
			for(int i=0; i<6; i++)
			{
				SineWindow[3,i] = 0.0;
			}
			for(int i=6; i<12; i++)
			{
				SineWindow[3,i] = Math.Sin(Math.PI/12*(i+0.5-6));
			}
			for(int i=12; i<18; i++)
			{
				SineWindow[3,i] = 1.0;
			}
			for(int i=18; i<36; i++)
			{
				SineWindow[3,i] = Math.Sin(Math.PI/36*(i+0.5));
			}

			// Short Block
			for(int i=0; i<12; i++)
			{
				SineWindow[2,i] = Math.Sin(Math.PI/12*(i+0.5));
			}
			for(int i=12; i<36; i++)
			{
				SineWindow[2,i] = 0.0;
			}
		}

		private void SubbandSynthesys(int ch)
		{
			if(!SubbandSynthesysInitFlag)
			{
				InitSubbandSynthesys();
				SubbandSynthesysInitFlag = true;
			}


			for(int ss=0; ss<18; ss++)
			{
				for(int i=0; i<64; i++)
				{
					double _Sum = 0.0;
					for(int j=0; j<32; j++)
					{
						double _Sig;
						if((ss%2 == 1) && (j%2 == 1))
						{
							_Sig = -1.0;
						}
						else
						{
							_Sig = 1.0;
						}
						_Sum += _Sig*pfb[ch, ss+j*18]*_Cos[i, j];
					}
					SubbandBuf[ch, SubbandBufIndex[ch], i] = _Sum;
				}

				for(int i=0; i<32; i++)
				{
					double _Sum = 0.0;

					for(int j=0; j<16; j++)
					{
						int Offset;
						if((j%2) == 0)
						{
							Offset = 0;
						}
						else
						{
							Offset = 32;
						}
						_Sum += PolyphaseCoefficient[j/2, Offset+i] *
							SubbandBuf[
								ch,
								(SubbandBufIndex[ch]+16-j)%16,
								Offset + i];
					}
					pfbOut[ch, ss*32+i] = _Sum;
				}
				SubbandBufIndex[ch] = (SubbandBufIndex[ch] + 1) % 16;
			}
		}

		private void InitSubbandSynthesys()
		{
			for(int i=0; i<64; i++)
			{
				for(int j=0; j<32; j++)
				{
					_Cos[i,j] = Math.Cos((2.0*j + 1.0)*(i + 16.0)*Math.PI/64);
				}
			}
		}
		

		private void CreatePcm(int gr)
		{
			// create 576 samples
			for(int i = 0; i < 576; i++)
			{
				// creating the sample per channel.
				for(int c = 0; c < frame.Channels; c++)
				{
					// clipping invalid sample(-32768~32767 is valid)
					int _Out = (int)(pfbOut[c, i] * 32768);
					if(_Out > 32767)
					{
						_Out = 32767;
						++clip;
					}
					else if(_Out < -32768)
					{
						_Out = -32768;
						++clip;
					}
					// [<---granule_margine--->+<-sample_just_made->]
					pcm[(576*gr*frame.Channels)+(i*frame.Channels)+c] = (Int16)_Out;
				}
			}
		}

		private void Reset()
		{
			ImdctPrevRawout = new double[2, 576];
			SubbandBuf = new double[2, 16, 64];
			SubbandBufIndex[0] = 0;
			SubbandBufIndex[1] = 0;
		}

		public int BufferSize
		{
			get { return buffer_size; }
		}

		public Int16[] Pcm
		{
			get { return pcm; }
		}

		public int Clip
		{
			get { return clip;}
		}
	}
}