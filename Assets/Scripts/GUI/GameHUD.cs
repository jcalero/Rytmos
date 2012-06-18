using UnityEngine;
using System.Collections;

public class GameHUD : MonoBehaviour
{

    #region Fields
    private int textHeight = 20;
    private int textWidth = 200;
    #endregion

    #region Properties

    #endregion

    #region Functions

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, textWidth, textHeight), "Score: " + Player.score);
        GUI.Label(new Rect(10, 30, textWidth, textHeight), "Health: " + Player.health);
        GUI.Label(new Rect(10, 50, textWidth, textHeight), "Energy: " + Player.energy);
    }

    #endregion
}
