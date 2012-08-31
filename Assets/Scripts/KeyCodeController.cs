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
		string cheatHash = MD5Utils.MD5FromString(deviceID + code + secretKey);

		string post_url = checkCodeURL + "code=" + code + "&device=" + deviceID + "&hash=" + cheatHash;

		// Initialise web connection
		WWW hs_post = new WWW(post_url);
		yield return hs_post; // Wait until the download is done

		if (hs_post.error != null) {
			Debug.LogWarning("There was an error verifying the code: " + hs_post.error);
			MainMenu.SetRedeemErrorText("Error submitting, please try again.");
		} else if (hs_post.text.StartsWith("ERROR: Expired")) {
			Debug.Log(hs_post.text);
			MainMenu.SetRedeemErrorText("Error: Code has expired or active on a different device.");
		} else if (hs_post.text.StartsWith("ERROR: Invalid")) {
			Debug.Log(hs_post.text);
			MainMenu.SetRedeemErrorText("Error: Invalid code");
		} else if (hs_post.text.StartsWith("SUCCESS: ")) {
			Debug.Log(hs_post.text);
			MainMenu.VerifiedSuccess = true;
		}
	}
	#endregion
}
