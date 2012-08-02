using UnityEngine;
using System.Collections;

public class BackgroundStatic : MonoBehaviour, PeakListener {

	#region Fields
	public GameObject backgroundObject;
	private float originalBGX;
	private float originalBGY;
	private float enhancedBGX;
	private float enhancedBGY;
	private bool bgIncrease;
	
	private float originalSpotLightX;
	private float originalSpotLightY;
	private float enhancedSpotLightX;
	private float enhancedSpotLightY;
	private bool spotLightIncrease;
	
	private float timer;
	private float lastTime;
	float directionU;
	float directionV;
	private static float intensity;
	private static float targetIntensity;
	private static float decay;
	private static bool increase;
	#endregion

	#region Functions

	void Start() {
		changeDirectionOfShader();
		intensity = 0.1f;
		targetIntensity = intensity;
		increase = false;
		decay = 0.01f;
		lastTime = Time.realtimeSinceStartup;
		PeakTriggerManager.addSelfToListenerList(this);
		
		originalBGX = backgroundObject.transform.localScale.x;
		originalBGY = backgroundObject.transform.localScale.y;
		enhancedBGX = originalBGX + (0.1f * originalBGX);
		enhancedBGY = originalBGY + (0.1f * originalBGY);
		bgIncrease = false;
		
//		originalSpotLightX = 0.8f;
//		originalSpotLightY = 0.8f;
//		enhancedSpotLightX = 0.6f;
//		enhancedSpotLightY = 0.6f;
//		spotLightIncrease = false;
//      	renderer.lightmapTilingOffset = new Vector4(enhancedSpotLightX,enhancedSpotLightY,renderer.lightmapTilingOffset.z,renderer.lightmapTilingOffset.w);
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

		float rateU = intensity * Time.deltaTime * 10;
		float rateV = intensity * Time.deltaTime * 10;

//      Vector2 spotLightScale = calculateSpotLightScale();
//		Debug.Log("x: " + spotLightScale.x + "; y: " + spotLightScale.y);
//      	renderer.lightmapTilingOffset = new Vector4(spotLightScale.x,spotLightScale.y,renderer.lightmapTilingOffset.z,renderer.lightmapTilingOffset.w);
		
//		float z = renderer.lightmapTilingOffset.z - renderer.bounds.center.x;
//		float w = renderer.lightmapTilingOffset.w - renderer.bounds.center.y;
//		renderer.lightmapTilingOffset = new Vector4(spotLightScale.x,spotLightScale.y,0f,0f);
		
		renderer.lightmapTilingOffset = new Vector4(renderer.lightmapTilingOffset.x, renderer.lightmapTilingOffset.y, renderer.lightmapTilingOffset.z + rateU * directionU, renderer.lightmapTilingOffset.w + rateV * directionV);
		
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
	
	private Vector3 calculateBackgroundSize() {
		float x = backgroundObject.transform.localScale.x;
		float y = backgroundObject.transform.localScale.y;
		
		if(increase) {
			x *= 1+(intensity * Time.deltaTime * 0.125f);
			y *= 1+(intensity * Time.deltaTime * 0.125f);
		} else if((x < enhancedBGX || y < enhancedBGY) && bgIncrease) {
			x *= 1+(intensity * Time.deltaTime * 0.08f);
			y *= 1+(intensity * Time.deltaTime * 0.08f);
		} else if((x > enhancedBGX || y > enhancedBGY) && bgIncrease) {
			x = enhancedBGX;
			y = enhancedBGY;
			bgIncrease = false;
		} else if(!bgIncrease) {
			x *= 1-(intensity * Time.deltaTime * 0.25f);
			y *= 1-(intensity * Time.deltaTime * 0.25f);
		}
		if(x < originalBGX || y < originalBGY) {
			x = originalBGX;
			y = originalBGY;
			bgIncrease = true;
		}
		return new UnityEngine.Vector3(x,y,0f);
	}
	
	private Vector2 calculateSpotLightScale() {
		
		float x = renderer.lightmapTilingOffset.x;
		float y = renderer.lightmapTilingOffset.y;
		
		if(increase) {
			x *= 1-(intensity * Time.deltaTime * 5f);
			y *= 1-(intensity * Time.deltaTime * 5f);
		} else if((x > enhancedSpotLightX || y > enhancedSpotLightY) && spotLightIncrease) {
			x *= 1-(intensity * Time.deltaTime * 3f);
			y *= 1-(intensity * Time.deltaTime * 3f);
		} else if((x < enhancedSpotLightX || y < enhancedSpotLightY) && spotLightIncrease) {
			x = enhancedSpotLightX;
			y = enhancedSpotLightY;
			spotLightIncrease = false;
		} else if(!bgIncrease) {
			x *= 1+(intensity * Time.deltaTime * 10f);
			y *= 1+(intensity * Time.deltaTime * 10f);
		}
		if(x > originalSpotLightX || y > originalSpotLightY) {
			x = originalSpotLightX;
			y = originalSpotLightY;
			spotLightIncrease = true;
		}
		
		return new Vector2(x,y);
	}
	#endregion
}
