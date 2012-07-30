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

	protected LinkedSpriteManager spriteManagerScript;
	protected LinkedSpriteManager bgSpriteManagerScript;
	protected Sprite touchSprite;                                     // The SpriteManager created sprite

	public static Color purple = new Color(.5f, 0, .5f, 1f);
	public static bool fourColors = false;							// Used for dealing with multiple colors - currently 4 or 6
	public static int EnemiesDespawned = 0;

	public GameObject[] particlesFeedback = new GameObject[6];      // The six feedback particles. Inspector reference. Location: LevelManager
	public GameObject spriteManager;                                // Reference to the SpriteManager. Inspector reference. Location: LevelManager
	public GameObject bgSpriteManager;                              // Reference to the SpriteManager. Inspector reference. Location: LevelManager
	public GameObject touchPrefab;                                  // The touch sprite. Inspector reference. Location: LevelManager
	public GameObject backgroundObject;
	public EnemySpawnScript enemySpawner;                           // The enemy spawn script.
	public Material bgNormal;
	public Material bgDark;
	#endregion

	#region Functions
	protected virtual void Awake() {
		// Local static reference to this class.
		if (Game.ColorMode == Game.NumOfColors.Four) fourColors = true;
		else fourColors = false;
		EnemiesDespawned = 0;
		Instance = this;
		spriteManagerScript = spriteManager.GetComponent<LinkedSpriteManager>();
		bgSpriteManagerScript = bgSpriteManager.GetComponent<LinkedSpriteManager>();
		Game.PowerupActive = Game.Powerups.None;
	}

	protected virtual void Start() {
		Game.Cheated = false;       // Reset cheated value

		if (Application.platform == RuntimePlatform.WindowsEditor) {
			bgSpriteManagerScript.material = bgDark;
			bgSpriteManager.renderer.material = bgDark;
		} else {
			bgSpriteManagerScript.material = bgNormal;
			bgSpriteManager.renderer.material = bgNormal;
		}
		
		bgSpriteManagerScript.AddSprite(backgroundObject, 1f, 1f, Vector2.zero, new Vector2(1f, 1f), false);

		// Create and hide the touch sprite
		touchSprite = spriteManagerScript.AddSprite(touchPrefab, 0.25f, 0.25f, new Vector2(0f, 0.365f), new Vector2(0.63f, 0.63f), false);
		touchSprite.hidden = true;

		// Set up level feedback stuff
		//SetUpBorderLineFeedback();
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
		for (int i = 0; i < numOfSpawns; i++) {
			int percentage = ess.spawnPositions[i];
			Instance.particlesFeedback[i].transform.localPosition = ess.findSpawnPositionVector(percentage);
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

	public static void SetUpParticlesFeedback(int particleNum, Vector3 position) {
		Instance.particlesFeedback[particleNum].transform.localPosition = position;
		Instance.particlesFeedback[particleNum].GetComponent<ParticleSystem>().startColor = Color.white;
	}

	public static void SetUpParticlesFeedback(int particleNum, Vector3 position, Color c) {
		Instance.particlesFeedback[particleNum].transform.localPosition = position;
		Instance.particlesFeedback[particleNum].GetComponent<ParticleSystem>().startColor = c;
	}

	//private void SetUpBorderLineFeedback() {
	//    /*
	//     * Here we will possibly have to reallocate the possible lines if we are dealing with different amounts of colours
	//     */	
	//    if (fourColors) {
	//        //First line - Cyan to Blue
	//        linePrefab[0].GetComponent<LineRenderer>().SetColors(Color.cyan, Color.blue);
	//        linePrefab[0].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenLeft, Game.screenBottom));
	//        linePrefab[0].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenRight, Game.screenBottom));
	//        //Second line - Blue to Red
	//        linePrefab[1].GetComponent<LineRenderer>().SetColors(Color.blue, Color.red);
	//        linePrefab[1].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenRight, Game.screenBottom));
	//        linePrefab[1].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenRight, Game.screenTop));
	//        //Third line - Red to Yellow
	//        linePrefab[2].GetComponent<LineRenderer>().SetColors(Color.red, Color.yellow);
	//        linePrefab[2].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenRight, Game.screenTop));
	//        linePrefab[2].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenLeft, Game.screenTop));
	//        //Fourth line - Yellow to Cyan
	//        linePrefab[3].GetComponent<LineRenderer>().SetColors(Color.yellow, Color.cyan);
	//        linePrefab[3].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenLeft, Game.screenTop));
	//        linePrefab[3].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenLeft, Game.screenBottom));
	//        //Move the other ones off the screen
	//        linePrefab[4].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenRight + 10, Game.screenTop + 10));
	//        linePrefab[4].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenMiddle + 10, Game.screenTop + 10));
	//        linePrefab[5].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenRight + 10, Game.screenTop + 10));
	//        linePrefab[5].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenMiddle + 10, Game.screenTop + 10));
	//    } else {
	//        //First line - Green to cyan
	//        linePrefab[0].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenLeft, Game.screenBottom));
	//        linePrefab[0].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenMiddle, Game.screenBottom));
	//        //Second line - Cyan to blue
	//        linePrefab[1].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenMiddle, Game.screenBottom));
	//        linePrefab[1].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenRight, Game.screenBottom));
	//        //Third line - blue to purple
	//        linePrefab[2].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenRight, Game.screenBottom));
	//        linePrefab[2].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenRight, Game.screenTop));
	//        //Fourth line - purple to red
	//        linePrefab[3].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenRight, Game.screenTop));
	//        linePrefab[3].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenMiddle, Game.screenTop));
	//        //Fifth line - red to yellow
	//        linePrefab[4].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenMiddle, Game.screenTop));
	//        linePrefab[4].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenLeft, Game.screenTop));
	//        //Sixth line - yellow to green
	//        linePrefab[5].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenLeft, Game.screenTop));
	//        linePrefab[5].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenLeft, Game.screenBottom));
	//    }
	//}

	/// <summary>
	/// Helper function to find the colour from a given screen coordinate. I.e. Input.mousePosition
	/// </summary>
	/// <param name="xy">The x,y coordinates of the screen location as Vector2</param>
	/// <returns>The color at the given coordinates</returns>
	public static Color singleColourSelect(Vector2 xy) {
		float angle = mouseAngle(xy);

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
				if (angle > -45) {
					//Blue - Bottom right
					return Color.blue;
				} else {
					if (angle < -145) {
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

	public static Color secondaryColourSelect(Vector2 xy) {
		float currAngle = mouseAngle(xy);
		if (Mathf.Abs(currAngle) >= 170) {
			if (currAngle > 0) return Color.green;
			else return Color.yellow;
		} else if (Mathf.Abs(currAngle) <= 10) {
			if (currAngle > 0) return Color.blue;
			else return Level.purple;
		} else if (currAngle > 110 && currAngle <= 120) {
			return Color.yellow;
		} else if ((currAngle > 120 && currAngle <= 130) || (currAngle > 50 && currAngle <= 60)) {
			return Color.red;
		} else if (currAngle >= 60 && currAngle < 70) {
			return Level.purple;
		} else if ((currAngle > -45 && currAngle <= -35) || (currAngle > -155 && currAngle <= -145)) {
			return Color.cyan;
		} else if (currAngle > -145 && currAngle <= -135) {
			return Color.green;
		} else if (currAngle > -55 && currAngle <= -45) {
			return Color.blue;
		} else {
			return Color.clear;
		}
	}

	public static Color continuousColourSelect(Vector2 xy) {
		float angle = mouseAngle(xy);

		if (angle > 0) {
			if (angle < 45) {
				return new Color((((angle / 45) * 64) + 64) / 255, 0, (191 - (angle / 45) * 63) / 255, 1);
			} else if (angle < 90) {
				return new Color(((((angle - 45) / 45) * 128) + 127) / 255, 0, (128 - ((angle - 45) / 45) * 128) / 255, 1);
			} else if (angle < 135) {
				return new Color(1, (angle - 90) / 45, 0, 1);
			} else {
				return new Color((255 - ((angle - 135) / 45) * 128) / 255, 1, 0, 1);
			}
		} else {
			if (angle > -45) {
				return new Color(((64 - ((angle / 45) * -64))) / 255, 0, (((angle / -45) * 63) + 191) / 255, 1);
			} else if (angle > -90) {
				return new Color(0, 1 - ((angle + 90) / 45), 1, 1);
			} else if (angle > -135) {
				return new Color(0, 1, (angle + 135) / 45, 1);
			} else {
				return new Color((128 - ((angle + 180) / 45) * 128) / 255, 1, 0, 1);
			}
		}
	}

	public static Color chunkyColorSelect(Vector2 xy) {
		float angle = mouseAngle(xy);	//Angle of the mouse
		int bandsize = 20;				//Size of the band where the transition happens (in degrees)
		int totalsize = 180; 			//Size of half the circle - should not change
		int cyanAdjust = 25;			//Size that cyan has been increased to match the visuals
		/*
		 * (cyan should be 120-60, but with cyanAdjust = 25, its 145-35 (transition occurs within those bounds)
		 * Color reference chart:
		 * Yellow : Color.yellow - 1, 1, 0, 1
		 * Red : Color.red - 1, 0, 0, 1
		 * Purple : Level.purple - .5f, 0, .5f, 1
		 * Blue : Color.blue - 0, 0, 1, 1
		 * Cyan : Color.cyan - 0, 1, 1, 1
		 * Green : Color.green - 0, 1, 0, 1
		 * Fractions below transition between these values
		 */
		if (angle > 0) {
			if (angle < (bandsize / 2)) {
				return new Color(.5f - (.25f * (((bandsize / 2) - angle) / (bandsize / 2))), 0, .5f + (.25f * (((bandsize / 2) - angle) / (bandsize / 2))), 1);
			} else if (angle >= (bandsize / 2) && angle < (totalsize / 3) - (bandsize / 2)) {
				return purple;
			} else if (angle >= (totalsize / 3) - (bandsize / 2) && angle < (totalsize / 3) + (bandsize / 2)) {
				return new Color(1 - (.5f * ((((totalsize / 3) + (bandsize / 2)) - angle) / bandsize)), 0, .5f * ((((totalsize / 3) + (bandsize / 2)) - angle) / bandsize), 1);
			} else if (angle >= (totalsize / 3) + (bandsize / 2) && angle < (2 * totalsize / 3) - (bandsize / 2)) {
				return Color.red;
			} else if (angle >= (2 * totalsize / 3) - (bandsize / 2) && angle < (2 * totalsize / 3) + (bandsize / 2)) {
				return new Color(1, 1 - ((((2 * totalsize / 3) + (bandsize / 2)) - angle) / bandsize), 0, 1);
			} else if (angle >= (2 * totalsize / 3) + (bandsize / 2) && angle < totalsize - (bandsize / 2)) {
				return Color.yellow;
			} else {
				return new Color(.5f + (.5f * ((totalsize - angle) / (bandsize / 2))), 1, 0, 1);
			}
		} else {

			if (angle > -(bandsize / 2)) {
				return new Color(.25f * (((bandsize / 2) + angle) / (bandsize / 2)), 0, 1 - (.25f * (((bandsize / 2) + angle) / (bandsize / 2))), 1);
			} else if (angle <= -(bandsize / 2) && angle > -((totalsize / 3) - (bandsize / 2) - cyanAdjust)) {
				return Color.blue;
			} else if (angle <= -((totalsize / 3) - (bandsize / 2) - cyanAdjust) && angle > -(((totalsize / 3) + (bandsize / 2)) - cyanAdjust)) {
				return new Color(0, 1 - ((((totalsize / 3) + (bandsize / 2) - cyanAdjust) + angle) / bandsize), 1, 1);
			} else if (angle <= -(((totalsize / 3) + (bandsize / 2)) - cyanAdjust) && angle > -(((2 * totalsize / 3) - (bandsize / 2)) + cyanAdjust)) {
				return Color.cyan;
			} else if (angle <= -(((2 * totalsize / 3) - (bandsize / 2)) + cyanAdjust) && angle > -((2 * totalsize / 3) + (bandsize / 2) + cyanAdjust)) {
				return new Color(0, 1, ((((2 * totalsize / 3) + (bandsize / 2) + cyanAdjust) + angle) / (bandsize)), 1);
			} else if (angle <= -((2 * totalsize / 3) + (bandsize / 2) + cyanAdjust) && angle > -(totalsize - (bandsize / 2))) {
				return Color.green;
			} else {
				return new Color(.5f - (.5f * ((totalsize + angle) / (bandsize / 2))), 1, 0, 1);
			}
		}
	}

	public static float mouseAngle(Vector2 xy) {
		float normalizedX = xy.x - (Screen.width / 2);
		float normalizedY = xy.y - (Screen.height / 2);
		return (Mathf.Rad2Deg * Mathf.Atan2(normalizedY, normalizedX));
	}

	public static Color singleColourSelect(int numColors) {
		switch (numColors) {
			case 0:
				return Color.blue;
			case 1:
				return Color.cyan;
			case 2:
				return Color.red;
			case 3:
				return Color.yellow;
			case 4:
				return Color.green;
			case 5:
				return Level.purple;
			default:
				return Color.black;
		}
	}
	#endregion

}
