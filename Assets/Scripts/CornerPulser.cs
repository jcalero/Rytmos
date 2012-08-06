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
    	ps.Emit(2);
    }
}
