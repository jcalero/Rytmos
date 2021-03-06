using UnityEngine;
using System.Collections;
using System.IO;

/// <summary>
/// File writer. Writes the peaks and loudparts to a cache file.
/// </summary> 
public class FileWriter
{
	
	/// <summary>
	/// Writes the analysis data.
	/// </summary>
	/// <param name='pathToMusicFile'>
	/// Path to music file.
	/// </param>
	/// <param name='peaks'>
	/// Array of Peaks.
	/// </param>
	/// <param name='loudParts'>
	/// Array of Loud parts.
	/// </param>
	public static void writeAnalysisData (string pathToMusicFile, int[][] peaks, int[] loudParts, float variationFactor)
	{
		
		// Open
		StreamWriter sw = new StreamWriter (convertToCacheFileName (pathToMusicFile));
		
		// Write peaks to file
		for (int i = 0; i < peaks.Length; i++) {
			sw.Write ("c" + i + ":");
			for (int j = 0; j < peaks[i].Length; j++) {
					sw.Write (peaks [i] [j]);
					sw.Write (';');
			}
			sw.WriteLine ();
		}
		
		// Write loudness levels to file
		sw.Write ("lp:");
		foreach (int lP in loudParts)
			sw.Write (lP + ";");
		sw.WriteLine();
		
		// Write variation factor to file
		sw.Write("vf:");
		sw.Write(variationFactor);
		
		// Close
		sw.Flush ();
		sw.Close ();
	}
	
	/// <summary>
	/// Converts the path to the current music file to a path to the according cache file.
	/// </summary>
	/// <returns>
	/// The cache file path.
	/// </returns>
	/// <param name='pathToMusicFile'>
	/// Path to music file.
	/// </param>
	public static string convertToCacheFileName (string pathToMusicFile)
	{
		int lastFileSeperator = Mathf.Max (pathToMusicFile.LastIndexOf ('/'), pathToMusicFile.LastIndexOf ('\\')) + 1;
		string fileName = pathToMusicFile.Substring (lastFileSeperator, pathToMusicFile.LastIndexOf ('.') - lastFileSeperator);
		if (Application.platform == RuntimePlatform.Android) 
			return (Application.persistentDataPath + "/" + fileName + ".ryt");
		else if (Application.platform == RuntimePlatform.WindowsPlayer
			|| 	 Application.platform == RuntimePlatform.WindowsEditor)
			return (Application.persistentDataPath + "\\" + fileName + ".ryt");
		else {
			Debug.Log ("PLATFORM NOT SUPPORTED YET");
			return "";
		}
	}
}
