using UnityEngine;
using System.Collections;
using System;
using System.Globalization;
/// <summary>
/// Level.cs
/// 
/// Level manager. Handles all the level feedback. Main class for potentially different level types that
/// inherit from this class.
/// </summary>
public class Level : MonoBehaviour {

    #region Fields
    private static Level Instance;                                  // The Instance of this class for self reference

    public static Color purple = new Color(.5f, 0, .5f, 1f);
    public static bool fourColors = false;							// Used for dealing with multiple colors - currently 4 or 6

    protected LinkedSpriteManager spriteManagerScript;
    protected Sprite touchSprite;                                     // The SpriteManager created sprite

    public GameObject[] particlesFeedback = new GameObject[6];      // The six feedback particles. Inspector reference. Location: LevelManager
    public GameObject[] linePrefab = new GameObject[6];             // The six feedback screen lines. Inspector reference. Location: LevelManager
    public GameObject spriteManager;                                // Reference to the SpriteManager. Inspector reference. Location: LevelManager
    public GameObject touchPrefab;                                  // The touch sprite. Inspector reference. Location: LevelManager
    public EnemySpawnScript enemySpawner;                           // The enemy spawn script.
    #endregion

    #region Functions
    protected virtual void Awake() {
        // Local static reference to this class.
		if(Game.ColorMode == Game.NumOfColors.Four) fourColors = true;
		else fourColors = false;
        Instance = this;
        spriteManagerScript = spriteManager.GetComponent<LinkedSpriteManager>();
    }

    protected virtual void Start() {
        Game.Cheated = false;       // Reset cheated value

        // Create and hide the touch sprite
        touchSprite = spriteManagerScript.AddSprite(touchPrefab, 0.25f, 0.25f, new Vector2(0f, 0.365f), new Vector2(0.63f, 0.63f), false);
        touchSprite.hidden = true;

        // Set up level feedback stuff
        SetUpBorderLineFeedback();
        //SetUpParticlesFeedback();
    }
    /// <summary>
    /// Shows the touch sprite at the "pos" location with the respective colour
    /// on that position
    /// </summary>
    /// <param name="pos">The position the sprite should spawn at</param>
    public static void ShowTouchSprite(Vector3 pos) {
        Instance.touchSprite.SetColor(singleColourSelect(Input.mousePosition) + new Color(0.3f, 0.3f, 0.3f));
        Instance.touchPrefab.transform.position = pos;
        Instance.touchSprite.hidden = false;
        // Animate the sprite "bouncing"
        iTween.ScaleFrom(Instance.touchPrefab, iTween.Hash("scale", new Vector3(1.3f, 1.3f, 1.3f), "time", 0.3f, "oncomplete", "HideTouchSprite", "oncompletetarget", Instance.gameObject));
    }

    /// <summary>
    /// Hides the touch sprite
    /// </summary>
    private void HideTouchSprite() {
        touchSprite.hidden = true;
    }

    /// <summary>
    /// Positions the particle feedbacks at the correct location of the screen
    /// </summary>
    public static void SetUpParticlesFeedback(int numOfSpawns, int enemy) {
		EnemySpawnScript ess = Instance.enemySpawner.GetComponent<EnemySpawnScript>();
		for(int i=0; i<numOfSpawns; i++) {
			int percentage = ess.spawnPositions[i];
			Instance.particlesFeedback[i].transform.localPosition = ess.findSpawnPositionVector(percentage);
			Debug.Log (enemy);
			Instance.particlesFeedback[i].GetComponent<ParticleSystem>().startColor = singleColourSelect(enemy);
		}
		/*
        if (fourColors) {
            particlesFeedback[1].transform.localPosition = new Vector3(Game.screenLeft, Game.screenBottom, 0); //Cyan
            particlesFeedback[2].transform.localPosition = new Vector3(Game.screenRight, Game.screenBottom, 0); //Blue
            particlesFeedback[3].transform.localPosition = new Vector3(Game.screenLeft, Game.screenTop, 0); //Yellow
            particlesFeedback[4].transform.localPosition = new Vector3(Game.screenRight, Game.screenTop, 0); //Red
        } else {
            particlesFeedback[0].transform.localPosition = new Vector3(Game.screenLeft, Game.screenBottom, 0); //Green
            particlesFeedback[1].transform.localPosition = new Vector3(Game.screenMiddle, Game.screenBottom, 0); //Cyan
            particlesFeedback[2].transform.localPosition = new Vector3(Game.screenRight, Game.screenBottom, 0); //Blue
            particlesFeedback[3].transform.localPosition = new Vector3(Game.screenLeft, Game.screenTop, 0); //Yellow
            particlesFeedback[4].transform.localPosition = new Vector3(Game.screenMiddle, Game.screenTop, 0); //Red
            particlesFeedback[5].transform.localPosition = new Vector3(Game.screenRight, Game.screenTop, 0); //Purple
        }
        */

    }

    private void SetUpBorderLineFeedback() {
        /*
         * Here we will possibly have to reallocate the possible lines if we are dealing with different amounts of colours
         */

        if (fourColors) {
            //First line - Cyan to Blue
            linePrefab[0].GetComponent<LineRenderer>().SetColors(Color.cyan, Color.blue);
            linePrefab[0].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenLeft, Game.screenBottom));
            linePrefab[0].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenRight, Game.screenBottom));

            //Second line - Blue to Red
            linePrefab[1].GetComponent<LineRenderer>().SetColors(Color.blue, Color.red);
            linePrefab[1].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenRight, Game.screenBottom));
            linePrefab[1].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenRight, Game.screenTop));

            //Third line - Red to Yellow
            linePrefab[2].GetComponent<LineRenderer>().SetColors(Color.red, Color.yellow);
            linePrefab[2].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenRight, Game.screenTop));
            linePrefab[2].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenLeft, Game.screenTop));

            //Fourth line - Yellow to Cyan
            linePrefab[3].GetComponent<LineRenderer>().SetColors(Color.yellow, Color.cyan);
            linePrefab[3].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenLeft, Game.screenTop));
            linePrefab[3].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenLeft, Game.screenBottom));

            //Move the other ones off the screen
            linePrefab[4].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenRight + 10, Game.screenTop + 10));
            linePrefab[4].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenMiddle + 10, Game.screenTop + 10));
            linePrefab[5].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenRight + 10, Game.screenTop + 10));
            linePrefab[5].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenMiddle + 10, Game.screenTop + 10));
        } else {
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


        if (fourColors) {
            if (angle > 0) {
                if (angle > 90)
                    return Color.yellow;
                else
                    return Color.red;
            } else {
                if (angle < -90)
                    return Color.cyan;
                else
                    return Color.blue;
            }
        } else {
            if (angle > 0) {
                if (angle < 60) {
                    //Purple - top right.
                    return purple;
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
    }
	
	public static Color singleColourSelect(int numColors) {
		switch (numColors) {
		case 0:
			return Color.blue;
		case 1:
			return Color.cyan;
		case 2:
			return Color.yellow;
		case 3:
			return Color.red;
		case 4:
			return Level.purple;
		case 5:
			return Color.green;
		default: 
			return Color.black;
		}
	}
    #endregion

}
