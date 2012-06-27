using UnityEngine;
using System.Collections;

public class HSController : MonoBehaviour {
    private string secretKey = "goldenck"; // Edit this value and make sure it's the same as the one stored on the server
    public string addScoreURL = "http://calero.se/rytmos/addscore.php?"; //be sure to add a ? to your url
    public string highscoreURL = "http://calero.se/rytmos/display.php?";
    public UILabel[] highscores;
    public UILabel[] names;

    void Start() {
        StartCoroutine(GetScores());
    }

    // remember to use StartCoroutine when calling this function!
    IEnumerator PostScores(string name, int score) {
        //This connects to a server side php script that will add the name and score to a MySQL DB.
        // Supply it with a string representing the players name and the players score.
        string hash = MD5Test.Md5Sum(name + score + secretKey);

        string post_url = addScoreURL + "name=" + WWW.EscapeURL(name) + "&score=" + score + "&hash=" + hash;

        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done

        if (hs_post.error != null) {
            print("There was an error posting the high score: " + hs_post.error);
        }
    }

    // Get the scores from the MySQL DB to display in a GUIText.
    // remember to use StartCoroutine when calling this function!
    IEnumerator GetScores() {
        for (int cnt = 0; cnt < names.Length; cnt++) {
            highscores[cnt].text = "Loading Scores";
            names[cnt].text = "Loading Scores";
            string hs_name_url = highscoreURL + "?position=" + cnt + "&field=1";
            string hs_score_url = highscoreURL + "?position=" + cnt + "&field=2";
            WWW hs_get = new WWW(hs_name_url);
            WWW hs_get_score = new WWW(hs_score_url);
            yield return hs_get;
            yield return hs_get_score;

            if (hs_get.error != null) {
                print("There was an error getting the high score: " + hs_get.error);
            } else if (hs_get_score.error != null) {
                print("There was an error getting the high score: " + hs_get_score.error);
            } else {
                Debug.Log(hs_get.text);
                names[cnt].text = (cnt+1) + ". " + hs_get.text;
                highscores[cnt].text = hs_get_score.text; // this is a GUIText that will display the scores in game.
            }
        }
    }

}