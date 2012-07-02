using System;
using System.Diagnostics;
using ToyTools;

class ToyMP3Demo
{
	static void Main(string[] args)
	{
		Console.WriteLine("ToyTools MP3 Decoder Demo");
		if(args.Length != 3)
		{
			Console.WriteLine("Usage: ToyMP3Demo.exe <MP3 Filename> <OUTPUT FILENAME> <ch>");
			return;
		}

		var mp3     = new ToyTools.ToyMP3(args[0]);
		var output  = args[1];
		var frame   = new ToyTools.ToyMP3Frame();
		var decoder = new ToyTools.ToyMP3Decoder();
		var wout = new ToyWaveOut();

		// main loop
		var sw = new Stopwatch();
		var debug_framenum = 0;
		var loop = 0;
		wout.PublishWaveFile(output, int.Parse(args[2]));

		sw.Start();
		try{
			while(mp3.SeekMP3Frame(frame))
			{
				decoder.DecodeFrame(frame);
				debug_framenum++;
				wout.AddData(decoder.Pcm);
				loop++;
			}
		}
		catch(Exception e)
		{
			//Console.WriteLine(e);
		}
		sw.Stop();
		var ts = sw.Elapsed;
		wout.FinishWriting();
		Console.WriteLine(
			"spent time:{0} total frame:{1} clpped_samples:{2}",
			ts, debug_framenum, decoder.Clip);
	}
}
