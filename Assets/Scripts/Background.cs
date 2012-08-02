using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour, PeakListener {

	#region Fields
	public GameObject[] HexagonObjects;
	public ParticleSystem[] BGFireworks;
	public UIAtlas GameAtlas;
	public LinkedSpriteManager BGSpriteManager;

	private float[] originalBGX;
	private float[] originalBGY;
	private float[] enhancedBGX;
	private float[] enhancedBGY;
	private bool bgIncrease;
	private float[] spriteValues = new float[6];

	private int[] channelRestrictors;
	private int[] channelDivisors = new int[] { 1, 1, 16, 2, 2, 2 };

	private float timer;
	private float lastTime;
	private static float intensity;
	private static float targetIntensity;
	private static float decay;
	private static bool increase;
	#endregion

	#region Functions
	void Awake() {
		spriteValues = SpriteTools.CalculateSprite(GameAtlas, "bgHexagon");
		for (int cnt = 0; cnt < HexagonObjects.Length; cnt++ )
			BGSpriteManager.AddSprite(HexagonObjects[cnt], spriteValues[0], spriteValues[1], (int)spriteValues[2], (int)spriteValues[3], (int)spriteValues[4], (int)spriteValues[5], false);
		originalBGX = new float[HexagonObjects.Length];
		originalBGY = new float[HexagonObjects.Length];
		enhancedBGX = new float[HexagonObjects.Length];
		enhancedBGY = new float[HexagonObjects.Length];
		channelRestrictors = new int[AudioManager.peaks.Length];
	}

	void Start() {

		intensity = 0.1f;
		targetIntensity = intensity;
		increase = false;
		decay = 0.01f;
		lastTime = Time.realtimeSinceStartup;
		PeakTriggerManager.addSelfToListenerList(this);

		originalBGX[0] = HexagonObjects[0].transform.localScale.x;
		originalBGY[0] = HexagonObjects[0].transform.localScale.y;
		originalBGX[1] = HexagonObjects[1].transform.localScale.x;
		originalBGY[1] = HexagonObjects[1].transform.localScale.y;
		originalBGX[2] = HexagonObjects[2].transform.localScale.x;
		originalBGY[2] = HexagonObjects[2].transform.localScale.y;
		enhancedBGX[0] = originalBGX[0] + (0.1f * originalBGX[0]);
		enhancedBGY[0] = originalBGY[0] + (0.1f * originalBGY[0]);
		enhancedBGX[1] = originalBGX[1] + (0.1f * originalBGX[1]);
		enhancedBGY[1] = originalBGY[1] + (0.1f * originalBGY[1]);
		enhancedBGX[2] = originalBGX[2] + (0.1f * originalBGX[2]);
		enhancedBGY[2] = originalBGY[2] + (0.1f * originalBGY[2]);
		bgIncrease = false;

	}

	void Update() {

		timer += Time.deltaTime;

		if (timer > 0.05) {
			if (targetIntensity > intensity && increase) {
				intensity *= 1 + decay + (targetIntensity - intensity);
			} else {
				increase = false;
				intensity *= (1 - decay);
			}
			timer = 0f;
		}
		if (intensity < 0.05f) intensity = 0.05f;


		HexagonObjects[0].transform.Rotate(0, 0, Time.deltaTime * 10);
		HexagonObjects[1].transform.Rotate(0, 0, -Time.deltaTime * 7);
		HexagonObjects[2].transform.Rotate(0, 0, Time.deltaTime * 5);
		HexagonObjects[0].transform.localScale = calculateBackgroundSize(0);
		HexagonObjects[1].transform.localScale = calculateBackgroundSize(1);
		HexagonObjects[2].transform.localScale = calculateBackgroundSize(2);
	}

	public void onPeakTrigger(int channel, int intensity) {
		switch (channel) {
			case 0:
				targetIntensity = intensity / 100f;
				increase = true;
				float timeDiff = Time.realtimeSinceStartup - lastTime;
				lastTime = Time.realtimeSinceStartup;
				if (timeDiff > 1f) timeDiff = 1f;
				else if (timeDiff < 0.1f) timeDiff = 0.1f;
				decay = timeDiff;
				break;
			case 3:
				if (channelRestrictors[channel] == 0) {
					int chosenFireworks = Random.Range(0, BGFireworks.Length);
					if (intensity > 15) {
						BGFireworks[chosenFireworks].startSpeed = 2.5f;
						BGFireworks[chosenFireworks].startLifetime = 1;
					} else {
						BGFireworks[chosenFireworks].startSpeed = 1.5f;
						BGFireworks[chosenFireworks].startLifetime = 1.5f;
					}
					BGFireworks[chosenFireworks].Emit(30);
				}
				break;
			default:
				break;
		}
		channelRestrictors[channel] = (channelRestrictors[channel] + 1) % channelDivisors[channel];
	}
	public void setLoudFlag(int flag) { }

	private Vector3 calculateBackgroundSize(int index) {
		float y = HexagonObjects[index].transform.localScale.y;
		float x = HexagonObjects[index].transform.localScale.x;

		if (increase) {
			x *= 1 + (intensity * Time.deltaTime * 1.8f);
			y *= 1 + (intensity * Time.deltaTime * 1.8f);
		} else if ((x < enhancedBGX[index] || y < enhancedBGY[index]) && bgIncrease) {
			x *= 1 + (intensity * Time.deltaTime * 0.6f);
			y *= 1 + (intensity * Time.deltaTime * 0.6f);
		} else if ((x > enhancedBGX[index] || y > enhancedBGY[index]) && bgIncrease) {
			x = enhancedBGX[index];
			y = enhancedBGY[index];
			bgIncrease = false;
		} else if (!bgIncrease) {
			x *= 1 - (intensity * Time.deltaTime * 3.6f);
			y *= 1 - (intensity * Time.deltaTime * 3.6f);
		}
		if (x < originalBGX[index] || y < originalBGY[index]) {
			x = originalBGX[index];
			y = originalBGY[index];
			bgIncrease = true;
		}

		return new UnityEngine.Vector3(x, y, 0.3f);
	}
	#endregion
}
