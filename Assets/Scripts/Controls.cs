using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour
{

    #region Fields
    #endregion

    #region Properties

    #endregion

    #region Functions

    void Awake()
    {
        Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (Application.platform == RuntimePlatform.Android)
        //{
            if (Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Escape) || Input.GetKey("escape"))
            {
                Application.Quit();
                return;
            }
        //}
    }

    #endregion
}
