using UnityEngine;
using System.Collections;
/// <summary>
/// CornerPulse.cs
/// 
/// Handles the emitting of corner/side particles for feedback.
/// </summary>
public class CornerPulser : MonoBehaviour {
    private ParticleSystem ps;

    void Start() {
        ps = gameObject.GetComponent<ParticleSystem>();
    }

    void Update() {
        // Emitt particles when the finger is on the screen or left mouse button is held down.
        if (Input.GetMouseButton(0)) {
            ps.Emit(2);
        }

    }
}
