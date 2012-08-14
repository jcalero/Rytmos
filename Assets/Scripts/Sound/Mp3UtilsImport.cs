using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Mp3UtilsWrapper
{
   
	class Mp3UtilsImport
	{    
		
		#region WORTHLESS_STUFF
		const string Mpg123Dll = @"libMp3Utils";
		
		//[?] We might have to import all the instance variables as well, but I have no idea how to do this for types C# doesn't have..
		// Here they are for reference:
		
		/*
		 
		//The main audio player
		AVAudioPlayer* audioPlayer;
		
		//Reference ("pointer") to the external audio file for reading
		ExtAudioFileRef extAFRef;
		
		//Number of channels in the audio file
		int extAFNumChannels;
		
		//Sample rate of audio file
		double extAFRateRatio;
		
		//Global BOOL to check whether we have finished reading yet.
		BOOL extAFReachedEOF;
				
		*/
		
		#endregion
				
		#region IMPORTED_FXNS
		
		/** Initalizes the MP3 player for playback, seeking, returning info, etc.
		 @param path - the path of the audio file
		 @return true if initalizes succeeds, false otherwise 
		 */
		
		//[?] Alright, we now take a char pointer and parse it to NSString using path = [NSString stringWithCString:str encoding:NSASCIIStringEncoding];
		[DllImport ("__Internal",CharSet = CharSet.Ansi)]
		public static extern bool MP3P_INIT(StringBuilder str);
	
		/** Plays the audio file
		 */
		[DllImport ("__Internal")]
		public static extern void MP3P_PLAY();
		
		/** Pauses the audio playback
		 */			 
		[DllImport ("__Internal")]	 
		public static extern void MP3P_PAUSE();
	
		/** Stops the audio playback
		 */		
		[DllImport ("__Internal")]
		public static extern void MP3P_STOP();

		/** Gets the number of channels in the audio file
		 @return the number of audio  channels
		 */		
		[DllImport ("__Internal")]
		public static extern int MP3P_GET_NUM_CHANNELS();

		/** Checks whether the file is currently being played by the AVAudioPlayer
		 @return true if file is playing, false otherwise
		 */
		[DllImport ("__Internal")]
		public static extern bool MP3P_IS_PLAYING();

		/** Sets the volume of the AVAudioPlayer
		 @param v - the desired volume (between 0.0 and 1.0)
		 */			
		[DllImport ("__Internal")]
		public static extern void MP3P_SET_VOLUME(float v);

		/** Returns the sample rate of the file
		 @return the sample rate in Hz
		 */			
		[DllImport ("__Internal")]
		public static extern float MP3P_GET_SAMPLE_RATE();

		/** Gets current playback time
		 @return the current playback time
		 */			
		[DllImport ("__Internal")]
		public static extern float MP3P_GET_CURRENT_TIME();

		/** Seeks to the specified time in the audio file (NOT TESTED, doesn't seem to work on simulator)
		 @param position - the desired time in seconds
		 @return true if seeking succeeds, false otherwise
		 */	
		[DllImport ("__Internal")]
		public static extern bool MP3P_SEEK(float position);

		/** As the name implies, cleans shit up
		 */		
		[DllImport ("__Internal")]
		public static extern void MP3P_CLEANUP();

		/** Initalizes the extAudioFile for reading
		 @param path - the path of the audio file
		 @param sampleRate - the sample rate of the file in Hz. Can be found using MP3P_GET_SAMPLE_RATE() (generally 44100)
		 @param numChannels - the number of channels in the file. Can be found using MP3P_GET_NUM_CHANNELS() (generally 2)
		 @return true if initialization succeded without errors, false otherwise
		 */
		
		//[?] Alright, we now take a char pointer and parse it to NSString using path = [NSString stringWithCString:str encoding:NSASCIIStringEncoding];
		[DllImport ("__Internal",CharSet = CharSet.Ansi)]
		public static extern bool MP3R_INIT(StringBuilder str, float sampleRate, int numChannels);

		/** Reads raw PCM data from the file. 
		 @param numFrames - number of frames to read (generally 1024 for FFT)
		 @param audio - a pointer to the reference float array to store the data in
		 @return the number of frames read
		 */
		
		// [?]: Uhh, how do we do float * => IntPtr in C#? Good luck Sam...			
		[DllImport ("__Internal")]
		public static extern int MP3R_READ( int numFrames, IntPtr audioBuffer);

		/** Clean shit up
		 @return Some sort of error code, 0 means everything worked fine
		 */			
		[DllImport ("__Internal")]
		public static extern int MP3R_CLEANUP();
		
		#endregion

	}

   
}
