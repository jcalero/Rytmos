using UnityEngine;
using System.Collections;

public class Lose : MonoBehaviour
{

    #region Fields
    private int textHeight = 300;
    private int textWidth = (int)(Screen.width / 1.15);
    private string menuText = "You lost!\n\nYour final score was: " + Player.score + "\n\nPress the button to try again.";
    private int buttonHeight = 50;
    private int buttonWidth = 200;
    #endregion

    #region Properties

    #endregion

    #region Functions

    void OnGUI()
    {
        GUI.skin.box.wordWrap = true;
        GUI.Box(new Rect(Screen.width / 2 - textWidth / 2, 40, textWidth, textHeight), menuText);
        if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth / 2,
                            Screen.height / 2 - buttonHeight / 2,
                            buttonWidth, buttonHeight),
                            "Try again!"))
        {
            Application.LoadLevel("Game");
        }
    }

    #endregion
}
