using UnityEngine;
using System.Collections;

public class GameHUD : MonoBehaviour
{

    #region Fields
    private int textHeight = 22;
    private int textWidth = 200;
    private string devText = "[DevMode Enabled]     1 - God Mode,    2 - Spawn Enemy,    3 - Massive Swarm";

    private DevScript d;
    #endregion

    #region Functions

    void Awake()
    {
        d = (DevScript)GameObject.Find("GameManager").GetComponent("DevScript");
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, textWidth, textHeight), "Score: " + Player.score);
        GUI.Label(new Rect(10, 30, textWidth, textHeight), "Health: " + Player.health);
        GUI.Label(new Rect(10, 50, textWidth, textHeight), "Energy: " + Player.energy);
        if (d.DevMode)
        {
            GUI.Label(new Rect(10, Screen.height - 30, Screen.width, textHeight), devText);
        }
    }

    #endregion
}
