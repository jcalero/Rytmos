using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

    #region Fields
    // Static screen side coordinates, gets set at launch of game.
    public static float screenLeft;
    public static float screenBottom;
    public static float screenTop;
    public static float screenRight;
    public static float screenMiddle;
    #endregion

    #region Functions
    void Awake() {
        // Make sure the game manager stays throughout all scenes.
        DontDestroyOnLoad(gameObject);

        // Define screen edges based on screen resolution (requires camera to be placed at XY origin, i.e. Vector3 (0, 0, _) )
        screenLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10)).x;
        screenBottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10)).y;
        screenRight = -screenLeft;
        screenTop = -screenBottom;
        screenMiddle = 0f;

        // Redundant checks given the camera is at (0,0). Saves (some) processing to not do this.
        //screenRight = Camera.main.ViewportToWorldPoint(new Vector3(1,0,10)).x;
        //screenTop = Camera.main.ViewportToWorldPoint(new Vector3(0,1,10)).y;
        //screenMiddleX = Camera.main.ViewportToWorldPoint(new Vector3(.5f, 0, 10)).x;
    }
    #endregion
}
