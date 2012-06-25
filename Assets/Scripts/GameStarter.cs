using UnityEngine;
using System.Collections;

public class GameStarter : MonoBehaviour {

    void Awake() {
        Application.LoadLevel("MainMenu");
    }
}
