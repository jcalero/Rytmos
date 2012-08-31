using UnityEngine;
using System.Collections;

public class KeyCodeController : MonoBehaviour {

	#region Fields
	private static readonly string secretKey = "brokentoeC3P0";
	private static readonly string checkCodeURL = "http://rytmos-game.com/checkcode.php?";
	#endregion

	#region Functions

	public static IEnumerator CheckCode(int code) {
		string deviceID = SystemInfo.deviceUniqueIdentifier;
		Debug.Log("device ID: " + deviceID);
		string cheatHash = MD5Utils.MD5FromString(deviceID + code + secretKey);

		string post_url = checkCodeURL + "code=" + code + "&device=" + deviceID + "&hash=" + cheatHash;
		Debug.Log(post_url);

		// Initialise web connection
		WWW hs_post = new WWW(post_url);
		yield return hs_post; // Wait until the download is done

		if (hs_post.error != null) {
			Debug.LogWarning("There was an error verifying the code: " + hs_post.error);
			MainMenu.SetRedeemErrorText("Error submitting, please try again.");
		} else {
			Debug.Log(hs_post.text);
			MainMenu.SetRedeemErrorText("");
		}


	}
	#endregion
}
