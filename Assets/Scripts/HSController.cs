using UnityEngine;
using System.Collections;

public class HSController : MonoBehaviour {
    private string secretKey = "goldenck";   // Edit this value and make sure it's the same as the one stored on the server
    private bool highscoresLoaded = false;   // Sets to true if the highscores were loaded once.

    public string addScoreURL = "http://calero.se/rytmos/addscore.php?"; //be sure to add a ? to your url
    public string highscoreURL = "http://calero.se/rytmos/display.php?";
    public UILabel[] highscores;
    public UILabel[] names;
    public UILabel submittedLabel;
    public UILabel errorLabel;

    private static HSController instance;

    void Awake() {
        instance = this;
    }

    // remember to use StartCoroutine when calling this function!
    public static IEnumerator PostScores(string name, int score, string table) {
        //This connects to a server side php script that will add the name and score to a MySQL DB.
        // Supply it with a string representing the players name and the players score.
        string hash = MD5Test.Md5Sum(name + score + instance.secretKey);

        string post_url = instance.addScoreURL + "name=" + WWW.EscapeURL(name) + "&score=" + score + "&table=" + table + "&hash=" + hash;

        instance.submittedLabel.text = "Submitting...";
        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done

        if (hs_post.error != null) {
            print("There was an error posting the high score: " + hs_post.error);
            instance.submittedLabel.text = "";
            instance.errorLabel.text = "[FF2222]Error submitting, please try again.";
            Win.ShowSubmitBox();
        } else {
            instance.submittedLabel.text = "Highscore submitted";
        }
    }

    // Get the scores from the MySQL DB to display in a GUIText.
    // remember to use StartCoroutine when calling this function!
    public static IEnumerator GetScores(string table) {
        if (instance.highscoresLoaded) {
            for (int cnt = 0; cnt < instance.names.Length; cnt++) {
                instance.highscores[cnt].text = "";
                instance.names[cnt].text = "";
            }
        }
        for (int cnt = 0; cnt < instance.names.Length; cnt++) {
            instance.highscores[cnt].text = "Loading Scores";
            instance.names[cnt].text = "Loading Scores";
            string hs_name_url = instance.highscoreURL + "?position=" + cnt + "&field=1" + "&table=" + table;
            string hs_score_url = instance.highscoreURL + "?position=" + cnt + "&field=2" + "&table=" + table;
            WWW hs_get = new WWW(hs_name_url);
            WWW hs_get_score = new WWW(hs_score_url);
            yield return hs_get;
            yield return hs_get_score;

            if (hs_get.error != null) {
                print("There was an error getting the high score: " + hs_get.error);
            } else if (hs_get_score.error != null) {
                print("There was an error getting the high score: " + hs_get_score.error);
            } else {
                instance.names[cnt].text = (cnt + 1) + ". " + hs_get.text;
                instance.highscores[cnt].text = hs_get_score.text; // this is a GUIText that will display the scores in game.
            }
        }
        instance.highscoresLoaded = true;
        MainMenu.EnableReloadButton();
    }

}