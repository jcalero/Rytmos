using System.Text;
using System.Security.Cryptography;
using System;
using System.IO;

public static class MD5Utils {
	public static string MD5FromString(string strToEncrypt) {
		UTF8Encoding ue = new UTF8Encoding();
		byte[] bytes = ue.GetBytes(strToEncrypt);

		// encrypt bytes
		MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);

		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";

		for (int i = 0; i < hashBytes.Length; i++) {
			hashString += Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}

		return hashString.PadLeft(32, '0');
	}

	public static string MD5FromFile(string fileName) {
		FileStream file = new FileStream(fileName, FileMode.Open);
		MD5 md5 = new MD5CryptoServiceProvider();
		byte[] retVal = md5.ComputeHash(file);
		file.Close();

		return BitConverter.ToString(retVal).Replace("-", "").ToLower();	// hex string
	}

	public static string MD5FromBytes(byte[] byteArray) {
		MD5 md5 = new MD5CryptoServiceProvider();
		byte[] retVal = md5.ComputeHash(byteArray);

		return BitConverter.ToString(retVal).Replace("-", "").ToLower();	// hex string
	}
}
