using System;
using ToyTools;

namespace ToyTools
{
	class GranuleInfo
	{
		private int part2_3_length = 0;
		private int big_values     = 0;
		private int global_gain    = 0;
		private int scalefac_compress = 0;
		private int window_switching_flag = 0;
		private int block_type = 0;
		private int mixed_block_flag = 0;
		private int[] table_select  = new int[]{0,0,0};
		private int[] subblock_gain = new int[]{0,0,0};
		private int region0_count = 0;
		private int region1_count = 0;
		private int preflag = 0;
		private int scalefac_scale = 0;
		private int count1_table_select = 0;

		private int[,] sf_com_table;

		public GranuleInfo()
		{
			sf_com_table = new int[,]{
				{0, 0, 0, 0, 3, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4},
				{0, 1, 2, 3, 0, 1, 2, 3, 1, 2, 3, 1, 2, 3, 2, 3}
			};
		}

		// Getter and Setter
		public int Part23Length
		{
			get { return part2_3_length; }
			set { part2_3_length = value; }
		}
		public int BigValues
		{
			get { return big_values; }
			set { big_values = value; }
		}
		public int GlobalGain
		{
			get { return global_gain; }
			set { global_gain = value; }
		}
		public int ScalefacCompress
		{
			get { return scalefac_compress; }
			set { scalefac_compress = value; }
		}
		public int[] Slen
		{
			get
			{
				return new int[]{
					sf_com_table[0, ScalefacCompress],
					sf_com_table[1, ScalefacCompress]
				};
			}
		}
		public int WindowSwitchingFlag
		{
			get { return window_switching_flag; }
			set
			{
				if(value < 0 || value > 1)
				{
					throw new Exception("Invalid data.");
				}
				else
				{
					window_switching_flag = value;
				}
			}
		}
		public int BlockType
		{
			get { return block_type; }
			set { block_type = value; }
		}
		public int MixedBlockFlag
		{
			get { return mixed_block_flag; }
			set
			{
				if(value < 0 || value > 1)
				{
					throw new Exception("Invalid data.");
				}
				else
				{
					mixed_block_flag = value;
				}
			}
		}
		public bool SetTableSelect(int idx, int v)
		{
			table_select[idx] = v;
			return true;
		}		
		public int GetTableSelect(int idx)
		{
			return table_select[idx];
		}
		public bool SetSubblockGain(int idx, int v)
		{
			subblock_gain[idx] = v;
			return true;
		}
		public int GetSubblockGain(int idx)
		{
			return subblock_gain[idx];
		}
		public int Region0Count
		{
			get { return region0_count; }
			set { region0_count = value; }
		}
		public int Region1Count
		{
			get { return region1_count; }
			set { region1_count = value; }
		}
		public int PreFlag
		{
			get { return preflag; }
			set
			{
				if(value < 0 || value > 1)
				{
					throw new Exception("Invalid data");
				}
				else
				{
					preflag = value;
				}
			}
		}
		public int ScalefacScale
		{
			get { return scalefac_scale; }
			set
			{
				if(value < 0 || value > 1)
				{
					throw new Exception("Invalid data.");
				}
				else
				{
					scalefac_scale = value;
				}
			}
		}
		public int Count1TableSelect
		{
			get { return count1_table_select; }
			set { count1_table_select = value; }
		}
		public bool IsMixedBlock
		{
			get
			{
				return (block_type == 2 && mixed_block_flag == 1);
			}
		}
		public bool IsLongBlock
		{
			get
			{
				return (block_type != 2);
			}
		}
		public bool IsShortBlock
		{
			get
			{
				return (block_type ==2 && mixed_block_flag == 0);
			}
		}
	}
}