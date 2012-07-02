using System;
using System.IO;
using System.Collections.Generic;

namespace ToyTools
{

	/*
	 * Simple BitStream class for ToyMP3Decoder.
	 */ 
	class BitStream
	{
		private int position = 0;
		private List<byte> data = new List<byte>();

		public BitStream()
		{
		}

		//
		// OutPut Methods
		//
		public string GetByString(int len)
		{
			int spos    = 0; // start position (inside byte block)
			int bpos    = 0; // start position (read from Nth byte)
			int blocks  = 0;
			string bits = "";

			if(len < 0)
			{
				throw new Exception("Can't get data by 0 or minus length.");
			}
			else if(len == 0)
			{
				return "0";
			}
			else if(position + len > Length)
			{
				throw new Exception("Can't read anymore. (Bit Stream ends.)");
			}
			if(position != 0)
			{
				spos = position % 8;
			}
			bpos = position / 8;

			// data length  | can be location among...
			// -------------+-------------------------
			// 1bit         | 1byte
			// 2bits~9bits  | 2bytes
			// 10bits~17bits| 3bytes
			//     :        |    :

			blocks = (len / 8) + 2;
			if(blocks + bpos > data.Count)
			{
				blocks = data.Count - bpos;
			}
			for(int i=0; i < blocks; i++)
			{
				bits += Convert.ToString(data[bpos + i], 2).PadLeft(8, '0');
			}

			position += len;

			// cut unnecessary bits (exists right and lef), then return.
			return bits.Substring(spos, len);
		}
		
		public int GetByInt(int len)
		{
			if (len<0 || len>32)
			{
				Console.WriteLine("########## {0}",len);
				//throw new Exception("hoge:");
			}
			if(len == 0)
			{
				return 0;
			}
			
			int retval = 0;
			for (int i=0; i<len; ++i)
			{
				if (position > Length)
				{
					throw new Exception("Over:GetByInt");
				}
				
				int pos = position>>3;
				int bit = 1<<(7-(position & 7));
				if ((data[pos] & bit)!=0)
				{
					retval |= 1<<(len-1-i);
				}
				++position;
			}
			
			return retval;
		}
		public bool GetByBool()
		{
			return (GetByInt(1) == 1);
		}
		public byte[] GetByByteArray(int bytes)
		{
			if(position % 8 != 0)
			{
				throw new Exception(
					"GetByByteArray can be used only at byte border."
				);
			}
			var tmp = data.GetRange(position / 8, bytes).ToArray();
			position += bytes * 8;
			return tmp;
		}

		public bool OpenFile(string filename)
		{
			try{
				var fs = new System.IO.FileStream
					(
						filename,
						System.IO.FileMode.Open,
						System.IO.FileAccess.Read
					);
				var tmp = new byte[fs.Length];
				fs.Read(tmp, 0, tmp.Length);
				fs.Close();
				data = new List<byte>(tmp);
			}
			catch(Exception)
			{
				return false;
			}
			position = 0;
			return true;
		}
		public bool Open01String(string t)
		{
			// data length must be "X*8" bits.
			if(t.Length % 8 != 0)
			{
				string s = "";
				for(int i = 0; i < t.Length % 8; i++)
				{
					s += '0';
				}
				t += s;
			}
			var tmp = new byte[t.Length / 8];
			for(int i = 0; i < t.Length / 8; i++)
			{
				tmp[i] = (byte)Convert.ToInt32(t.Substring(i*8,8),2);
			}
			data = new List<byte>(tmp);
			position = 0;
			return true;
		}

		public void Skip(int step)
		{
			if(position + step > Length)
			{
				throw new Exception(
					"Position overflow. It's greater than length of stream.");
			}
			position += step;
		}

		public bool AddByteArray(byte[] bytes)
		{
			data.AddRange(bytes);
			return true;
		}

		public void RemoveRange(int index, int range)
		{
			data.RemoveRange(index, range);
			// unit of position is *BIT*
			// unit of index and range is *BYTE*
			if(position > index*8)
			{
				if(position > (index+range)*8)
				{
					position -= range*8; 
				}
				else
				{
					position = index*8;
				}
			}
		}

		public void Reset()
		{
			data = new List<byte>();
			position = 0;
		}
		
		public int Length
		{
			get { return data.Count * 8; }
		}

		public int Position
		{
			get { return position; }
			set
			{
				if(value < 0)
				{
					throw new Exception("Position can't be less than 0.");
				}
				position = value;
			}
		}
	}
}