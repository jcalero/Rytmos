using UnityEngine;
using System.Collections;
/// <summary>
/// GameHud.cs
/// 2012-06-27
/// 
/// Handles the heads up display (text and scores) during gameplay.
/// </summary>
public class GameHUDOld : MonoBehaviour {
    #region Fields
    // These public objects are set in the Editor/Inspector. Object: "UI Root (HUD)".
    public UILabel ScoreLabel;
    public UILabel ScoreValueLabel;
    public UILabel HealthLabel;
    public UILabel HealthValueLabel;
    public UILabel EnergyLabel;
    public UILabel EnergyValueLabel;
    public UILabel DevmodeLabel;

    // The Game HUD camera coordinates
    private float screenLeft;
    private float screenTop;
    //private float screenRight;
    //private float screenBottom;

    // Label distance values
    private float labelSpacing = 0.1f;
    private float labelLeft = 0.065f;
    private float labelTop = 0.1f;
    private float valueLeft = 0.40f;
    #endregion

    #region Functions

    void Awake() {
        // Assign HUD camera coordinates
        screenLeft = NGUITools.FindCameraForLayer(LayerMask.NameToLayer("2D Menu")).ViewportToWorldPoint(new Vector3(0, 0, 0 )).x;
        screenTop = NGUITools.FindCameraForLayer(LayerMask.NameToLayer("2D Menu")).ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
        //screenRight = -screenLeft;
        //screenTop = -screenBottom;
    }

    void Start() {
        // Position the HUD relative to the screen
        ScoreLabel.transform.position = new Vector3(screenLeft + labelLeft, screenTop - labelTop, ScoreValueLabel.transform.position.z);
        EnergyLabel.transform.position = new Vector3(screenLeft + labelLeft, screenTop - labelTop - labelSpacing*2, ScoreValueLabel.transform.position.z);
        ScoreValueLabel.transform.position = new Vector3(screenLeft + labelLeft + valueLeft, screenTop - labelTop, ScoreValueLabel.transform.position.z);
        EnergyValueLabel.transform.position = new Vector3(screenLeft + labelLeft + valueLeft, screenTop - labelTop - labelSpacing * 2, ScoreValueLabel.transform.position.z);
    }

    void Update() {
        ScoreValueLabel.text = "[FDD017]" + Player.score;
        EnergyValueLabel.text = "[736AFF]" + Player.energy;
        if (Game.DevMode) {
            DevmodeLabel.gameObject.active = true;
        } else {
            DevmodeLabel.gameObject.active = false;
        }
    }

    #endregion
}
