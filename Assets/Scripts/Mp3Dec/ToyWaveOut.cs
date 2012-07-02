using System;
using System.IO;

namespace ToyTools
{
	class ToyWaveOut
	{
		private string filename;

		// default setting (44.1kHz 16bit Stereo)
		private byte[] header_riff   = new byte[]{0x52,0x49,0x46,0x46}; // 'RIFF'
		private byte[] filesize      = new byte[]{0x00,0x00,0x00,0x00}; // FileSize-8
		private byte[] header_wave   = new byte[]{0x57,0x41,0x56,0x45}; // 'WAVE'
		private byte[] header_fmt    = new byte[]{0x66,0x6d,0x74,0x20}; // 'fmt '
		private byte[] fmt_chunk_num = new byte[]{0x10,0x00,0x00,0x00}; // 0x10 (fmt chunk byte size)
		private byte[] format_id     = new byte[]{0x01,0x00};           // 0x10 (Linear PCM)
		private byte[] channels      = new byte[]{0x02,0x00};           // 2ch Stereo
		private byte[] sampling_rate = new byte[]{0x44,0xac,0x00,0x00}; // 44.1kHz
		private byte[] data_speed    = new byte[]{0x10,0xb1,0x02,0x00}; // 44.1kHz*16bit*Stereo(2)/8
		private byte[] block_size    = new byte[]{0x04,0x00};
		private byte[] bit_per_sample= new byte[]{0x10,0x00};           // 16bitå›ºå®š
		private byte[] data_chunk    = new byte[]{0x64,0x61,0x74,0x61}; // 'd','a','t','a'
		private byte[] wave_length   = new byte[]{0x00,0x00,0x00,0x00};
		private int    ch = 2;
		private uint   _wl = 0;
		private int    pcm_block_length = 1152*2;

		private FileStream fs;
		public ToyWaveOut(){}

		public void PublishWaveFile(string fn, int _ch)
		{
			ch = _ch;
			// Set channel information
			if(ch == 1)
			{
				Console.WriteLine("Attention:Monoral Mode <toywaveout@toytools.toymp3decoder>");
				channels   = new byte[]{0x01,0x00};
				block_size = new byte[]{0x02,0x00};
				data_speed = new byte[]{0x88,58,0x01,0x00};
				//data_speed = new byte[]{0x10,0xb1,0x02,0x00};
				pcm_block_length = 1152;
			}
			else if(ch != 2)
			{
				throw new Exception("Invalid Channels.");
			}

			filename     = fn;
			wave_length  = new byte[]{0,0,0,0};

			fs = new FileStream(
				filename,
				System.IO.FileMode.Create,
				System.IO.FileAccess.Write);

			// Header
			fs.Write(header_riff,    0, header_riff.Length);
			fs.Write(filesize,       0, filesize.Length);
			fs.Write(header_wave,    0, header_wave.Length);
			fs.Write(header_fmt ,    0, header_fmt.Length);
			fs.Write(fmt_chunk_num,  0, fmt_chunk_num.Length);
			fs.Write(format_id,      0, format_id.Length);
			fs.Write(channels,       0, channels.Length);
			fs.Write(sampling_rate,  0, sampling_rate.Length);
			fs.Write(data_speed,     0, data_speed.Length);
			fs.Write(block_size,     0, block_size.Length);
			fs.Write(bit_per_sample, 0, bit_per_sample.Length);
			fs.Write(data_chunk,     0, data_chunk.Length);
			fs.Write(wave_length,    0, wave_length.Length);
		}
		public void AddData(Int16[] pcm)
		{
			var p = new byte[2];
			for(int i=0; i<pcm_block_length;i++)
			{
				p = BitConverter.GetBytes(pcm[i]);
				fs.Write(p, 0, p.Length);
			}
			
			// æ›¸ãè¾¼ã‚“ã ãƒ‡ãƒ¼ã‚¿ã®é•·ã•ã‚’ä¿æŒã—ã¦ãŠã
			_wl += (uint)(pcm_block_length*2);
		}
		public void FinishWriting()
		{
			var size = BitConverter.GetBytes(_wl);
			// ãƒ‡ãƒ¼ã‚¿ã‚µã‚¤ã‚ºã‚’æ›¸ãè¾¼ã‚€
			fs.Position = 40;
			fs.Write(size, 0, size.Length);
			fs.Close();
		}

		public int SamplingRate
		{
			set
			{
				sampling_rate = BitConverter.GetBytes((uint)value);
			}
		}
	}
}