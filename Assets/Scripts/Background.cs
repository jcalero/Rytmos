using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour, PeakListener {

    #region Fields
	public GameObject backgroundObject;
	private float originalBGX;
	private float originalBGY;
	private float enhancedBGX;
	private float enhancedBGY;
    private float timer;
	private float lastTime;
    float directionU;
    float directionV;
	private static float intensity;
	private static float targetIntensity;
	private static float decay;
	private static bool increase;
	private bool bgIncrease;
    #endregion

    #region Functions

    void Start() {
		changeDirectionOfShader();
		intensity = 0.1f;
		targetIntensity = intensity;
		increase = false;
		bgIncrease = false;
		decay = 0.01f;
		lastTime = Time.realtimeSinceStartup;
		PeakTriggerManager.addSelfToListenerList(this);
		originalBGX = backgroundObject.transform.localScale.x;
		originalBGY = backgroundObject.transform.localScale.y;
		enhancedBGX = originalBGX + (0.1f * originalBGX);
		enhancedBGY = originalBGY + (0.1f * originalBGY);
    }

    void Update() {

        timer += Time.deltaTime;
		
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
		Debug.Log("intensity: "+intensity);
		Debug.Log("target: "+targetIntensity);
		

        float rateU = intensity * Time.deltaTime * 10;
        float rateV = intensity * Time.deltaTime * 10;

        //Vector2 curOffset = renderer.material.GetTextureOffset("_MainTex");
        //renderer.material.SetTextureOffset("_MainTex", new Vector2(curOffset.x + rateU * directionU, curOffset.y + rateV * directionV));
        Vector4 curOffset = renderer.lightmapTilingOffset;
        renderer.lightmapTilingOffset = new Vector4(curOffset.x, curOffset.y, curOffset.z + rateU * directionU, curOffset.w + rateV * directionV);
		
		Debug.Log(backgroundObject.transform.localScale.x);
		
		backgroundObject.transform.localScale = calculateBackgroundSize();
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
			changeDirectionOfShader();
			break;
		default:
			break;
		}
		
	
	}
	public void setLoudFlag (int flag) {}
	
	private void changeDirectionOfShader() {
		directionU = Random.Range(-1f, 1f);
        directionV = Random.Range(-1f, 1f);
		if(directionU == 0 && directionV == 0) directionU = 1f;
	}
	
	public Vector3 calculateBackgroundSize() {
		float x = backgroundObject.transform.localScale.x;
		float y = backgroundObject.transform.localScale.y;
		
		if(increase) {
			x *= 1+(intensity * Time.deltaTime * 0.5f);
			y *= 1+(intensity * Time.deltaTime * 0.5f);
		} else if((x < enhancedBGX || y < enhancedBGY) && bgIncrease) {
			x *= 1+(intensity * Time.deltaTime * 0.25f);
			y *= 1+(intensity * Time.deltaTime * 0.25f);
		} else if((x > enhancedBGX || y > enhancedBGY) && bgIncrease) {
			x = enhancedBGX;
			y = enhancedBGY;
			bgIncrease = false;
		} else if(!bgIncrease) {
			x *= 1-(intensity * Time.deltaTime);
			y *= 1-(intensity * Time.deltaTime);
		}
		if(x < originalBGX || y < originalBGY) {
			x = originalBGX;
			y = originalBGY;
			bgIncrease = true;
		}
		return new UnityEngine.Vector3(x,y,0f);
	}
    #endregion
}
