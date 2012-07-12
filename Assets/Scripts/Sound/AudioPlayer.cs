using UnityEngine;
using System.Collections;

public class AudioPlayer : MonoBehaviour {
	
	private static AudioSource[] audioSources;
	private static int currentSource;
	private static int halfTimeInSamples;
	private static bool testFlag;
	private static bool bufferSource1;
	private static bool bufferSource2;
	private static float timer;
	private static int pauseSample;
	private static bool pauseFlag;
	private static bool playFlag;
	
	void Awake() {
		audioSources = gameObject.GetComponentsInChildren<AudioSource>();
		Debug.Log("AWAKE THE KRAKEN!");
		if(Game.Song != null && Game.Song != "") {
			if(Game.Song != AudioManager.getCurrentSong()) AudioManager.initMusic(Game.Song);
			else if(AudioManager.isSongLoaded()) AudioManager.reset();
		} else if (audioSources[0].clip != null) {
			if(!AudioManager.isSongLoaded()) {
				AudioManager.setCam(gameObject);
				AudioManager.initMusic("");
			}
		}
		while(!AudioManager.isSongLoaded()) {} // idlewait
		
		currentSource = 0;
		audioSources[0].clip = AudioManager.getAudioClip(1);
		audioSources[1].clip = AudioManager.getAudioClip(2);
		
		audioSources[0].velocityUpdateMode = AudioVelocityUpdateMode.Fixed;
		audioSources[1].velocityUpdateMode = AudioVelocityUpdateMode.Fixed;

		bufferSource1 = false;
		bufferSource2 = true;
		
		timer = 0f;
		pauseSample = 0;
		pauseFlag = false;
		playFlag = false;
	}
	// Use this for initialization
	void Start () {
		playFlag = true;
		audioSources[currentSource].Play();
	}
	
	// Update is called once per frame
	void Update () {
		if(playFlag && !Game.Paused) {
			
			if(AudioManager.lastSamples) {
				return;
			}
			
			if (timer >= AudioManager.audioLength){
				audioSources[currentSource].Stop();
			} else if(audioSources[0].clip != null && audioSources[1].clip != null && gameObject != null) {
				
				if(audioSources[0].timeSamples > 0 && audioSources[0].isPlaying && bufferSource2 && !audioSources[1].isPlaying) {
					StartCoroutine(AudioManager.updateMusic());
					audioSources[1].Play((ulong)(audioSources[0].clip.samples - audioSources[0].timeSamples));
					bufferSource2 = false;
					bufferSource1 = true;
					currentSource = 0;
				} else if(audioSources[1].timeSamples > 0 && audioSources[1].isPlaying && bufferSource1 && !audioSources[0].isPlaying) {
					StartCoroutine(AudioManager.updateMusic());
					audioSources[0].Play((ulong)(audioSources[1].clip.samples - audioSources[1].timeSamples));
					bufferSource1 = false;
					bufferSource2 = true;
					currentSource = 1;
				}
				
				timer += Time.deltaTime;
			}
		}
	}
	
	public static void play() {
		playFlag = true;
		audioSources[currentSource].Play();
	}
	
	public static void pause() {
		if(audioSources[0] != null && audioSources[1] != null){
			pauseFlag = true;
			if(audioSources[currentSource].isPlaying) {
				if(audioSources[currentSource].isPlaying) audioSources[currentSource].Pause();
				if(audioSources[currentSource == 1? 0:1].isPlaying) audioSources[currentSource == 1? 0:1].Stop();
				
				pauseSample = audioSources[currentSource].timeSamples;
			}
		}
	}
	
	public static void resume() {
		if(audioSources[0] != null && audioSources[1] != null){
			if(pauseFlag) {
				if(!audioSources[currentSource].isPlaying) audioSources[currentSource].Play();
				if(!audioSources[currentSource == 1? 0:1].isPlaying) audioSources[currentSource == 1? 0:1].Play((ulong)(audioSources[currentSource].clip.samples - pauseSample));
				pauseSample = 0;
				pauseFlag = false;
			}
		}
	}
}
