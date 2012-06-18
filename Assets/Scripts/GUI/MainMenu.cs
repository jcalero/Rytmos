using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{

    #region Fields
    private int textHeight = 300;
    private int textWidth = (int)(Screen.width / 1.15);
    private string menuText = "Welcome to Rytmos Development Build 1!\n\nInstructions:\nClick anywhere on the screen to kill the sperms,\nbut make sure to match the color!";
    private int buttonHeight = 50;
    private int buttonWidth = 200;
    Player player = new Player();
    #endregion

    #region Properties

    #endregion

    #region Functions

    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width / 2 - textWidth / 2, 10, textWidth, textHeight), menuText);
        if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth / 2,
                            Screen.height / 2 - buttonHeight / 2,
                            buttonWidth, buttonHeight),
                            "Start Game"))
        {
            player.ResetStats();
            Application.LoadLevel("Game");
        }
    }

    #endregion
}
