using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {

    #region Fields
    private static GameObject touchPrefab;
    private static Sprite touchSprite;
    private static Level Instance;
    private Color purple = new Color(.5f, 0, .5f, 1f);

    public GameObject[] particlesFeedback = new GameObject[6];
    public GameObject[] linePrefab = new GameObject[6];
    private LinkedSpriteManager spriteManager;
    #endregion

    #region Functions
    void Awake() {
        // Local static reference to this class.
        Instance = this;

        // Creates and hides the touch feedback sprite
        touchPrefab = GameObject.Find("TouchSprite");
        spriteManager = GameObject.Find("SpriteManager").GetComponent<LinkedSpriteManager>();
        touchSprite = spriteManager.AddSprite(touchPrefab, 0.25f, 0.25f, new Vector2(0f, 0.365f), new Vector2(0.63f, 0.63f), false);
        touchSprite.hidden = true;
    }

    void Start() {
        SetUpBorderLineFeedback();
        SetUpParticlesFeedback();
 }

    public static void ShowTouchSprite(Vector3 pos) {
        touchSprite.SetColor(singleColourSelect(Input.mousePosition) + new Color(0.3f, 0.3f, 0.3f));
        touchPrefab.transform.position = pos;
        touchSprite.hidden = false;
        iTween.ScaleFrom(touchPrefab, iTween.Hash("scale", new Vector3(1.3f, 1.3f, 1.3f), "time", 0.3f, "oncomplete", "HideTouchSprite", "oncompletetarget", Instance.gameObject));
    }

    private void HideTouchSprite() {
        touchSprite.hidden = true;
    }

    private void SetUpParticlesFeedback() {
        // Instantiate and set colours for the feedback particles.
        particlesFeedback[0].transform.localPosition = new Vector3(Game.screenLeft, Game.screenBottom, 0);
		particlesFeedback[1].transform.localPosition = new Vector3(Game.screenMiddle, Game.screenBottom, 0);
		particlesFeedback[2].transform.localPosition = new Vector3(Game.screenRight, Game.screenBottom, 0);
		particlesFeedback[3].transform.localPosition = new Vector3(Game.screenLeft, Game.screenTop, 0);
	    particlesFeedback[4].transform.localPosition = new Vector3(Game.screenMiddle, Game.screenTop, 0);
	    particlesFeedback[5].transform.localPosition = new Vector3(Game.screenRight, Game.screenTop, 0);
    }

    private void SetUpBorderLineFeedback() {
		/*
		 * Here we will possibly have to reallocate the possible lines if we are dealing with multiple lines
		 */
		
        //First line - Green to cyan
        linePrefab[0].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenLeft, Game.screenBottom));
        linePrefab[0].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenMiddle, Game.screenBottom));

        //Second line - Cyan to blue
        linePrefab[1].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenMiddle, Game.screenBottom));
        linePrefab[1].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenRight, Game.screenBottom));

        //Third line - blue to purple
        linePrefab[2].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenRight, Game.screenBottom));
        linePrefab[2].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenRight, Game.screenTop));
        
        //Fourth line - purple to red
        linePrefab[3].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenRight, Game.screenTop));
        linePrefab[3].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenMiddle, Game.screenTop));


        //Fifth line - red to yellow
        linePrefab[4].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenMiddle, Game.screenTop));
        linePrefab[4].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenLeft, Game.screenTop));

        //Sixth line - yellow to green
        linePrefab[5].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenLeft, Game.screenTop));
        linePrefab[5].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenLeft, Game.screenBottom));

        
        
        
    }

    /// <summary>
    /// Helper function to find the colour from a given screen coordinate. I.e. Input.mousePosition
    /// </summary>
    /// <param name="xy">The x,y coordinates of the screen location as Vector2</param>
    /// <returns>The color at the given coordinates</returns>
    public static Color singleColourSelect(Vector2 xy) {
        float normalizedX = xy.x - (Screen.width / 2);
        float normalizedY = xy.y - (Screen.height / 2);
        float angle = (Mathf.Rad2Deg * Mathf.Atan2(normalizedY, normalizedX));

        if (angle > 0) {
            if (angle < 60) {
                //Purple - top right.
                return new Color(.5f, 0f, .5f, 1f);
            } else {
                if (angle > 120) {
                    //Yellow - Top left
                    return Color.yellow;
                } else {
                    //Red - Top Middle
                    return Color.red;
                }
            }
        } else {
            if (angle > -60) {
                //Blue - Bottom right
                return Color.blue;
            } else {
                if (angle < -120) {
                    //Green - bottom left
                    return Color.green;
                } else {
                    //cyan - bottom middle
                    return Color.cyan;
                }
            }
        }
    }
    #endregion

}
