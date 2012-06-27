using UnityEngine;
using System.Collections;
/// <summary>
/// GameHud.cs
/// 2012-06-27
/// 
/// Handles the heads up display (text and scores) during gameplay.
/// </summary>
public class GameHUD : MonoBehaviour {
    #region Fields
    // These public objects are set in the Editor/Inspector. Object: "UI Root (HUD)".
    public UILabel ScoreValueLabel;
    public UILabel HealthValueLabel;
    public UILabel EnergyValueLabel;
    public UILabel DevmodeLabel;
    #endregion

    #region Functions
    void Update() {
        ScoreValueLabel.text = "[FDD017]" + Player.score;
        HealthValueLabel.text = "[E55B3C]" + Player.health;
        EnergyValueLabel.text = "[736AFF]" + Player.energy;
        if (Game.DevMode) {
            DevmodeLabel.gameObject.active = true;
        } else {
            DevmodeLabel.gameObject.active = false;
        }
    }

    #endregion
}
