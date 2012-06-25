using UnityEngine;
using System.Collections;

public class CornerPulser : MonoBehaviour {
    private ParticleSystem ps;
    
    void Start () {
        ps = gameObject.GetComponent<ParticleSystem>();	
    }

    void Update () {
        if(Input.GetMouseButton(0)) {
            ps.Emit(2);
        }

    }
}
