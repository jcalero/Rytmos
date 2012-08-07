using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour, PeakListener {

	#region Fields
	public GameObject BackgroundObject;
	public GameObject PlayerObject;
	public ParticleSystem CentreNotes;
	public UIAtlas GameAtlas;
	public LinkedSpriteManager BGSpriteManager;
	public ParticleSystem[] FeedbackStars;

	private bool bgIncrease;
	private float[] spriteValues = new float[6];

	private float originalPlayerX;
	private float originalPlayerY;
	private float enhancedPlayerX;
	private float enhancedPlayerY;
	private float maxPlayerX;
	private float maxPlayerY;
	
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
	private float originalCentreNotesEmissionRate;
	#endregion

	#region Functions
	void Awake() {
		// Load the background sprite
		spriteValues = SpriteTools.CalculateSprite(GameAtlas, "background");
		BGSpriteManager.AddSprite(BackgroundObject, spriteValues[0], spriteValues[1], (int)spriteValues[2], (int)spriteValues[3], (int)spriteValues[4], (int)spriteValues[5], false);

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

		originalPlayerX = PlayerObject.transform.localScale.x;
		originalPlayerY = PlayerObject.transform.localScale.z;
		enhancedPlayerX = originalPlayerX + (0.1f * originalPlayerX);
		enhancedPlayerY = originalPlayerY + (0.1f * originalPlayerY);
		maxPlayerX = 2*originalPlayerX;
		maxPlayerY = 2*originalPlayerY;
		bgIncrease = false;

		originalEmissionRate = FeedbackStars[0].emissionRate;
		originalParticleSpeed = FeedbackStars[0].playbackSpeed;
		enhancedEmissionRate = originalEmissionRate * 10f;
		enhancedParticleSpeed = originalParticleSpeed * 10f;
		
		originalCentreNotesEmissionRate = CentreNotes.emissionRate;
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

		PlayerObject.transform.localScale = calculatePlayerSize();
	}

	private IEnumerator FlashStars(int enemy, int intensity) {

		float y = FeedbackStars[enemy].playbackSpeed;
		float x = FeedbackStars[enemy].emissionRate;

		bool increase = x < enhancedEmissionRate || y < enhancedParticleSpeed ? true : false;

		do {

			if (increase) {
				x = enhancedEmissionRate;
				y = enhancedParticleSpeed;
				increase = false;
			} else if (x > originalEmissionRate || y > originalParticleSpeed) {
				x *= 1 - (intensity * Time.deltaTime * 3f);
				y *= 1 - (intensity * Time.deltaTime * 3f);
			}

			if (x < originalEmissionRate * 1.1f || y < originalEmissionRate * 1.1f) {
				x = originalEmissionRate;
				y = originalParticleSpeed;
			}

			FeedbackStars[enemy].playbackSpeed = y;
			FeedbackStars[enemy].emissionRate = x;
			yield return new WaitForSeconds(0.1f);
		} while (x != originalEmissionRate || y != originalParticleSpeed);
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

				if (!particlesActive[EnemySpawnScript.currentlySelectedEnemy]) {
					StartCoroutine(FlashStars(EnemySpawnScript.currentlySelectedEnemy, intensity));
					particlesActive[EnemySpawnScript.currentlySelectedEnemy] = true;
				}
				break;
			case 3:
				if (channelRestrictors[channel] == 0) {
					CentreNotes.Emit(20);
				}
				break;
			default:
				break;
		}
		channelRestrictors[channel] = (channelRestrictors[channel] + 1) % channelDivisors[channel];
	}
	public void setLoudFlag(int flag) {
		//Debug.Log("Speed is now: " + flag);
		switch (flag) {
			case 0:
				CentreNotes.startSpeed = 0f;
				CentreNotes.emissionRate = 0f;
				CentreNotes.startLifetime = 3f;
				break;
			case 1:
				CentreNotes.startSpeed = 0.7f;
				CentreNotes.emissionRate = originalCentreNotesEmissionRate;
				CentreNotes.startLifetime = 2f;
				break;
			case 2:
				CentreNotes.startSpeed = 1.5f;
				CentreNotes.emissionRate = originalCentreNotesEmissionRate;
				CentreNotes.startLifetime = 1.7f;
				break;
			case 3:
				CentreNotes.startSpeed = 2.5f;
				CentreNotes.emissionRate = originalCentreNotesEmissionRate;
				CentreNotes.startLifetime = 1.5f;
				break;
			case 4:
				CentreNotes.startSpeed = 4f;
				CentreNotes.emissionRate = originalCentreNotesEmissionRate;
				CentreNotes.startLifetime = 1.2f;
				break;
			case 5:
				CentreNotes.startSpeed = 6f;
				CentreNotes.emissionRate = originalCentreNotesEmissionRate;
				CentreNotes.startLifetime = 0.9f;
				break;
		}
	}

	private Vector3 calculatePlayerSize() {
		float y = PlayerObject.transform.localScale.z;
		float x = PlayerObject.transform.localScale.x;

		if (increase && x < maxPlayerX && y < maxPlayerY) {
			x *= 1 + (intensity * Time.deltaTime * 1.8f);
			y *= 1 + (intensity * Time.deltaTime * 1.8f);
		} else if ((x < enhancedPlayerX || y < enhancedPlayerY) && bgIncrease) {
			x *= 1 + (intensity * Time.deltaTime * 0.6f);
			y *= 1 + (intensity * Time.deltaTime * 0.6f);
		} else if ((x > enhancedPlayerX || y > enhancedPlayerY) && bgIncrease) {
			x = enhancedPlayerX;
			y = enhancedPlayerY;
			bgIncrease = false;
		} else if (!bgIncrease) {
			x *= 1 - (intensity * Time.deltaTime * 3.6f);
			y *= 1 - (intensity * Time.deltaTime * 3.6f);
		}
		if (x < originalPlayerX || y < originalPlayerY) {
			x = originalPlayerX;
			y = originalPlayerY;
			bgIncrease = true;
		}
		return new UnityEngine.Vector3(x, 0.3f, y);
	}
	#endregion
}
