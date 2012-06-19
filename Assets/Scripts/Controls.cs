using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour
{

    #region Fields
    private bool devMode;
    private DevScript devScript;
    #endregion

    #region Functions

    void Awake()
    {
        devScript = (DevScript)gameObject.GetComponent("DevScript");
        Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Escape) || Input.GetKey("escape"))
        {
            Application.Quit();
            return;
        }
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            devScript.DevMode = !devScript.DevMode;
        }
    }


    #endregion
}
