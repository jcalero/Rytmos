using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {

    #region Fields
    private float timer;
    int directionU;
    int directionV;
	private static float intensity;
    #endregion

    #region Functions

    void Start() {
        directionU = Random.Range(-1, 1);
        directionV = Random.Range(-1, 1);
		intensity = 0f;
        if (directionU == 0)
            directionU = 1;
        if (directionV == 0)
            directionU = 1;
    }

    void Update() {
        if (timer > 2.5) {
            directionU = Random.Range(-1, 1);
            directionV = Random.Range(-1, 1);
            if (directionU == 0)
                directionU = 1;
            if (directionV == 0)
                directionU = 1;
            timer = 0;
        }

        timer += Time.deltaTime;

        float rateU = intensity * Time.deltaTime;
        float rateV = intensity * Time.deltaTime;

        //Vector2 curOffset = renderer.material.GetTextureOffset("_MainTex");
        //renderer.material.SetTextureOffset("_MainTex", new Vector2(curOffset.x + rateU * directionU, curOffset.y + rateV * directionV));
        Vector4 curOffset = renderer.lightmapTilingOffset;
        renderer.lightmapTilingOffset = new Vector4(curOffset.x, curOffset.y, curOffset.z + rateU * (float)directionU, curOffset.w + rateV * (float)directionV);
    }
	
	public static void changeSpeed(int newIntensity) {
		intensity = 0.5f*intensity + (0.5f*(float)newIntensity)/100f;
	}
    #endregion
}
