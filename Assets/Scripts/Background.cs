using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour, PeakListener {

	#region Fields
	public GameObject[] HexagonObjects;
	public ParticleSystem[] BGFireworks;
	public UIAtlas GameAtlas;
	public LinkedSpriteManager BGSpriteManager;
	public ParticleSystem[] FeedbackStars;

	private float[] originalBGX;
	private float[] originalBGY;
	private float[] enhancedBGX;
	private float[] enhancedBGY;
	private bool bgIncrease;
	private float[] spriteValues = new float[6];
	
	private float originalEmissionRate;
	private float enhancedEmissionRate;
	private float originalParticleSpeed;
	private float enhancedParticleSpeed;
	private bool[] particlesActive;

	private int[] channelRestrictors;
	private int[] channelDivisors = new int[] { 1, 1, 16, 2, 2, 2 };

	private float timer;
	private float lastTime;
	private static float intensity;
	private static float targetIntensity;
	private static float decay;
	private static bool increase;
	
	private int lastSelectedEnemy;
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
		particlesActive = new bool[FeedbackStars.Length];
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
		
		originalEmissionRate = FeedbackStars[0].emissionRate;
		originalParticleSpeed = FeedbackStars[0].playbackSpeed;
		enhancedEmissionRate = originalEmissionRate * 10f;
		enhancedParticleSpeed = originalParticleSpeed * 10f;
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
	
	private IEnumerator FlashStars(int enemy, int intensity) {
		
		float y = FeedbackStars[enemy].playbackSpeed;
		float x = FeedbackStars[enemy].emissionRate;
						
		bool increase = x < enhancedEmissionRate || y < enhancedParticleSpeed? true : false;
		
		do {
			
			if (increase) {
				x = enhancedEmissionRate;
				y = enhancedParticleSpeed;
				increase = false;
			} else if (x > originalEmissionRate || y > originalParticleSpeed) {
				x *= 1 - (intensity * Time.deltaTime * 3f);
				y *= 1 - (intensity * Time.deltaTime * 3f);
			}

			if(x < originalEmissionRate*1.1f || y < originalEmissionRate*1.1f) {
				x = originalEmissionRate;
				y = originalParticleSpeed;
			}
				
			FeedbackStars[enemy].playbackSpeed = y;
			FeedbackStars[enemy].emissionRate = x;
			yield return new WaitForSeconds(0.1f);
		} while(x != originalEmissionRate || y != originalParticleSpeed);
		particlesActive[enemy] = false;
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
				
				if(!particlesActive[EnemySpawnScript.currentlySelectedEnemy]) {
					StartCoroutine(FlashStars(EnemySpawnScript.currentlySelectedEnemy, intensity));
					particlesActive[EnemySpawnScript.currentlySelectedEnemy] = true;
				}
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
