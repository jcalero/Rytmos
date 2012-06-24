using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {

    #region Fields
    private static GameObject touchPrefab;
    private static Sprite touchSprite;
    private static Level Instance;

    public GameObject colorFeedback;
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
        // Instantiate and set colours for the feedback particles.
        // TODO: This needs to be optimised, unecessary GetComponent calls, better to
        // save the ParticleSystem reference at Awake() and reuse it here.
        // Better yet, add them to the scene instead of instantiating?
        colorFeedback.GetComponent<ParticleSystem>().startColor = Color.green;
        Instantiate(colorFeedback, new Vector3(Game.screenLeft, Game.screenBottom, 0), colorFeedback.transform.localRotation);
        colorFeedback.GetComponent<ParticleSystem>().startColor = Color.cyan;
        Instantiate(colorFeedback, new Vector3(Game.screenMiddle, Game.screenBottom, 0), colorFeedback.transform.localRotation);
        colorFeedback.GetComponent<ParticleSystem>().startColor = Color.blue;
        Instantiate(colorFeedback, new Vector3(Game.screenRight, Game.screenBottom, 0), colorFeedback.transform.localRotation);
        colorFeedback.GetComponent<ParticleSystem>().startColor = Color.yellow;
        Instantiate(colorFeedback, new Vector3(Game.screenLeft, Game.screenTop, 0), colorFeedback.transform.localRotation);
        colorFeedback.GetComponent<ParticleSystem>().startColor = Color.red;
        Instantiate(colorFeedback, new Vector3(Game.screenMiddle, Game.screenTop, 0), colorFeedback.transform.localRotation);
        colorFeedback.GetComponent<ParticleSystem>().startColor = new Color(.5f, 0, .5f, 1);
        Instantiate(colorFeedback, new Vector3(Game.screenRight, Game.screenTop, 0), colorFeedback.transform.localRotation);
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
