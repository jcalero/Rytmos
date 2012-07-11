using UnityEngine;
using System.Collections;

public class AudioPlayer : MonoBehaviour {
	
	private AudioSource[] audioSources;
	private int currentSource;
	private int halfTimeInSamples;
	private bool testFlag;
	private bool bufferSource1;
	private bool bufferSource2;
	private float timer;
	
	void Awake() {
		audioSources = gameObject.GetComponentsInChildren<AudioSource>();

		if(Game.Song != null && Game.Song != "") {
			if(Game.Song != AudioManager.getCurrentSong()) AudioManager.initMusic(Game.Song);
			// else reset audioclips!!!!
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
	}
	// Use this for initialization
	void Start () {		
		audioSources[currentSource].Play();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(AudioManager.lastSamples) {
			return;
		}
		
		if (timer >= AudioManager.audioLength){
			audioSources[currentSource].Stop();
		} else if(audioSources[0].clip != null && audioSources[1].clip != null && gameObject != null) {
			
			if(audioSources[0].timeSamples > 0 && bufferSource2) {
				StartCoroutine(AudioManager.updateMusic());
				audioSources[1].Play((ulong)(audioSources[0].clip.samples - audioSources[0].timeSamples));
				bufferSource2 = false;
				bufferSource1 = true;
			} else if(audioSources[1].timeSamples > 0 && bufferSource1) {
				StartCoroutine(AudioManager.updateMusic());
				audioSources[0].Play((ulong)(audioSources[1].clip.samples - audioSources[1].timeSamples));
				bufferSource1 = false;
				bufferSource2 = true;
			}
			
			timer += Time.deltaTime;
		}
	
	}
}
