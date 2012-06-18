using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{

    #region Fields
    private int textHeight = 300;
    private int textWidth = 400;
    private string menuText = "Welcome to Sperm Shooter!\n\nClick anywhere on the screen to kill the sperms, but make sure to match the color!";
    private int buttonHeight = 50;
    private int buttonWidth = 200;
	Player player = (Player) GameObject.Find("Player").GetComponent<Player>();
    #endregion

    #region Properties

    #endregion

    #region Functions

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - textWidth / 2, 40, textWidth, textHeight), menuText);
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
