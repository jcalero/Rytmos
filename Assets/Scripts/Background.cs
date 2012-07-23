using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour, PeakListener {

    #region Fields
    private float timer;
	private float lastTime;
    int directionU;
    int directionV;
	private static float intensity;
	private static float targetIntensity;
	private static float decay;
	private static bool increase;
    #endregion

    #region Functions

    void Start() {
        directionU = Random.Range(-1, 1);
        directionV = Random.Range(-1, 1);
		intensity = 0.1f;
		targetIntensity = intensity;
		increase = false;
		decay = 0.01f;
		lastTime = Time.realtimeSinceStartup;
		PeakTriggerManager.addSelfToListenerList(this);
        if (directionU == 0)
            directionU = 1;
        if (directionV == 0)
            directionU = 1;
    }

    void Update() {
//        if (timer > 2.5) {
//            directionU = Random.Range(-1, 1);
//            directionV = Random.Range(-1, 1);
//            if (directionU == 0)
//                directionU = 1;
//            if (directionV == 0)
//                directionU = 1;
//            timer = 0;
//        }

        timer += Time.deltaTime;
		
		
//		float diff = Mathf.Abs(targetIntensity - intensity);
		if(timer > 0.05) {
			if(targetIntensity > intensity && increase) {
				intensity *= 1+decay+(targetIntensity - intensity);
			}
			else {
				increase = false;
				intensity *= (1-decay);
			}
			timer = 0f;
		}
		if(intensity < 0.05f) intensity = 0.05f;
//		Debug.Log("intensity: " + intensity);

        float rateU = intensity * Time.deltaTime * 10;
        float rateV = intensity * Time.deltaTime * 10;

        //Vector2 curOffset = renderer.material.GetTextureOffset("_MainTex");
        //renderer.material.SetTextureOffset("_MainTex", new Vector2(curOffset.x + rateU * directionU, curOffset.y + rateV * directionV));
        Vector4 curOffset = renderer.lightmapTilingOffset;
        renderer.lightmapTilingOffset = new Vector4(curOffset.x, curOffset.y, curOffset.z + rateU * (float)directionU, curOffset.w + rateV * (float)directionV);
    }
	
	public void onPeakTrigger(int channel, int intensity) {
		switch(channel) {
		case 0:
			targetIntensity = intensity/100f;
			increase = true;
			float timeDiff = Time.realtimeSinceStartup - lastTime;
			lastTime = Time.realtimeSinceStartup;
			if(timeDiff > 1f) timeDiff = 1f;
			else if(timeDiff < 0.1f) timeDiff = 0.1f;
			decay = timeDiff;
			break;
		case 3:
			directionU = Random.Range(-1, 1);
            directionV = Random.Range(-1, 1);
            if (directionU == 0)
                directionU = 1;
            if (directionV == 0)
                directionU = 1;
			break;
		default:
			break;
		}
		
	
	}
	public void setLoudFlag (int flag) {}
    #endregion
}
