using UnityEngine;
using System.Collections;

public class Win : MonoBehaviour
{

    #region Fields
    private int textWidth = (int)(Screen.width / 1.15);
    private string menuText1 = "Congratulations! You won!";
    private string menuText2 = "Press the button to play again!";
    private string scoreText = "Your final health was: " + Player.health + "\nYour final score was: " + Player.score + "\nYour total score is: " + (Player.score * (Player.health * 0.1));
    private int buttonHeight = 50;
    private int buttonWidth = 200;
    #endregion

    #region Functions
    void OnGUI()
    {
        GUI.skin.box.wordWrap = true;
        GUI.Box(new Rect(Screen.width / 2 - textWidth / 2, 45, textWidth, 30), menuText1);
        GUI.Box(new Rect(Screen.width /2 - textWidth /2, 90, textWidth, 300), scoreText);
        GUI.Box(new Rect(Screen.width / 2 - textWidth / 2, 160, textWidth, 30), menuText2);
        if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth / 2,
                            Screen.height / 2 - buttonHeight / 2,
                            buttonWidth, buttonHeight),
                            "One More Time!"))
        {
            Application.LoadLevel("Game");
        }
    }
    #endregion
}
