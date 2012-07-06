using UnityEngine;
using System.Collections;
using System.IO;

public class FileWriter {
	
	public static void writeAnalysisData(string pathToMusicFile, float[][] peaks, int[] loudParts) {
			
		StreamWriter sw = new StreamWriter(convertToCacheFileName(pathToMusicFile));
		for(int i = 0; i < peaks.Length; i++) {
			sw.Write("c"+i+":");
			for(int j = 0; j < peaks[i].Length; j++) {
				if(peaks[i][j] > 0) {
					sw.Write(peaks[i][j]);
					sw.Write(';');
				}
			}
			sw.WriteLine();
		}
		sw.Write("lp:");
		foreach(int lP in loudParts) sw.Write(lP+";");
		sw.Flush();
		sw.Close();
	}
	
	public static string convertToCacheFileName(string pathToMusicFile) {
		int lastFileSeperator = Mathf.Max(pathToMusicFile.LastIndexOf('/'),pathToMusicFile.LastIndexOf('\\')) +1;
		string fileName = pathToMusicFile.Substring(lastFileSeperator,pathToMusicFile.LastIndexOf('.') - lastFileSeperator);
		#if UNITY_ANDROID 
			return (Application.persistentDataPath + "/" + fileName + ".ryt");
		#else
			return (Application.persistentDataPath + "\\" + fileName + ".ryt");
		#endif
	}
}
