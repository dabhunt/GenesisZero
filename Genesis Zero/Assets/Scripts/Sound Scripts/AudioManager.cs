﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Audio;

/* 
 * This script create an Audio Manager where all the music and sound effects are generated from
 * Supports three audio sources intended for music tracks including:
 * Fading in and out, cross fading between sources, and setting various parameters (volume, pitch, looping, etc)
 * Sound effect sources are generated independently to support multiple sound effects in one scene
 */

public class AudioManager : MonoBehaviour
{
	// Audio player components.
	public AudioSource Primary;
	public AudioSource Secondary;
	public AudioSource Tertiary;

	public AudioMixer mixer;
	List<Sound> Soundlist = new List<Sound>();
	List<string> SFXNames = new List<string>();
	private float setVolumeMaster;
	private float setVolumeMusic;
	private float setVolumeSound;
	private bool globalMute;
	private bool soundUpdate;
	private List<GameObject> soundPlayerChilds = new List<GameObject>();
	private GameObject playerObj;
	// Singleton instance.
	public static AudioManager instance = null;

	// Initialize the singleton instance.
	void Awake()
	{
		if (instance == null)
			instance = this;
		else
		{
			Destroy(gameObject);
			return;
		}

		// Hardcoded default values
		setVolumeMaster = AudioListener.volume;
		setVolumeMusic = 3.0f;
		setVolumeSound = 3.0f;
		globalMute = false;
		soundUpdate = true;
	}
	private void Start()
	{
		playerObj = GameObject.FindWithTag("Player");
		Object[] temp = Resources.LoadAll("Sounds/SFX");
		for (int i = 0; i < temp.Length; i++)
		{
			SFXNames.Add(temp[i].name);
		}
		//PlayFadeInTrack(1, "Music", "AmbientMusic", true, 5f);
		SetVolumeMusic(.7f);
		PlayFadeInTrack(1, "Music", "AmbientMusic", true, 7f);
		PlayTrack(2, "Music", "CombatMusic", true, true, 0, 1);
	}
	// Update is called once per frame
	void Update()
	{
		if (soundUpdate)
		{
			// Any sounds finished playing are removed
			for (int i = soundPlayerChilds.Count - 1; i >= 0; i--)
			{
				if (!soundPlayerChilds[i].GetComponent<AudioSource>().isPlaying)
				{
					Destroy(soundPlayerChilds[i]);
					soundPlayerChilds.RemoveAt(i);
				}
			}
		}
	}
	//=========================
	// Global Audio Functions
	//=========================

	public void TogglePauseAll()
	{
		AudioListener.pause = !AudioListener.pause;
	}

	public void ToggleMuteAll()
	{
		globalMute = !globalMute;
		if (globalMute)
			AudioListener.volume = 0;
		else
			AudioListener.volume = setVolumeMaster;
	}

	public void AdjustVolumeAll(float vol)
	{
		AudioListener.volume += vol;
	}

	public void SetVolumeMaster(float vol)
	{
		AudioListener.volume = vol;
	}

	//=======================
	// Channel functions
	//=======================

	// Utility functions

	public void AdjustVolumeChannel(int channel, float vol)
	{
		switch (channel)
		{
			case 1:
				Primary.volume += vol;
				break;
			case 2:
				Secondary.volume += vol;
				break;
			case 3:
				Tertiary.volume += vol;
				break;
			default:
				Debug.LogWarning("AdjustVolumeChannel: Channel " + channel + " not supported!");
				break;
		}
	}
	//--------------------------------------------------

	public void SetVolumeChannel(int channel, float vol)
	{
		switch (channel)
		{
			case 1:
				Primary.volume = vol;
				break;
			case 2:
				Secondary.volume = vol;
				break;
			case 3:
				Tertiary.volume = vol;
				break;
			default:
				Debug.LogWarning("SetVolumeChannel: Channel " + channel + " not supported!");
				break;
		}
	}

	public void SetVolumeMusic(float vol)
	{
		setVolumeMusic = Primary.volume = Secondary.volume = Tertiary.volume = vol;
	}
	public void SetVolumeSFX(float vol) 
	{
		
	}
	//--------------------------------------------------

	public void ClearChannel(int channel)
	{
		switch (channel)
		{
			case 1:
				Primary.clip = null;
				break;
			case 2:
				Secondary.clip = null;
				break;
			case 3:
				Tertiary.clip = null;
				break;
			default:
				Debug.LogWarning("ClearChannel: Channel " + channel + " not supported!");
				break;
		}
	}

	public void ClearAllChannels()
	{
		Primary.clip = Secondary.clip = Tertiary.clip = null;
	}

	//--------------------------------------------------

	public void StopChannel(int channel)
	{
		switch (channel)
		{
			case 1:
				Primary.Stop();
				break;
			case 2:
				Secondary.Stop();
				break;
			case 3:
				Tertiary.Stop();
				break;
			default:
				Debug.LogWarning("StopChannel: Channel " + channel + " not supported!");
				break;
		}
	}

	public void StopAllChannels()
	{
		Primary.Stop();
		Secondary.Stop();
		Tertiary.Stop();
	}

	//--------------------------------------------------

	public void PauseChannel(int channel)
	{
		switch (channel)
		{
			case 1:
				Primary.Pause();
				break;
			case 2:
				Secondary.Pause();
				break;
			case 3:
				Tertiary.Pause();
				break;
			default:
				Debug.LogWarning("PauseChannel: Channel " + channel + " not supported!");
				break;
		}
	}

	public void PauseAllChannels()
	{
		Primary.Pause();
		Secondary.Pause();
		Tertiary.Pause();
	}

	//--------------------------------------------------

	public void UnPauseChannel(int channel)
	{
		switch (channel)
		{
			case 1:
				Primary.UnPause();
				break;
			case 2:
				Secondary.UnPause();
				break;
			case 3:
				Tertiary.UnPause();
				break;
			default:
				Debug.LogWarning("UnPauseChannel: Channel " + channel + " not supported!");
				break;
		}
	}

	public void UnPauseAllChannels()
	{
		Primary.UnPause();
		Secondary.UnPause();
		Tertiary.UnPause();
	}

	//--------------------------------------------------
	// Functions for fading
	//--------------------------------------------------

	public void FadeChannel(int channel, float vol, float fadeTime)
	{
		switch (channel)
		{
			case 1:
				Primary.DOFade(vol, fadeTime);
				break;
			case 2:
				Secondary.DOFade(vol, fadeTime);
				break;
			case 3:
				Tertiary.DOFade(vol, fadeTime);
				break;
			default:
				Debug.LogWarning("FadeChannel: Channel " + channel + " not supported!");
				break;
		}
	}

	public void FadeInChannel(int channel, float vol, float inTime)
	{
		SetVolumeChannel(channel, 0);
		FadeChannel(channel, vol, inTime);
	}

	public void FadeOutChannel(int channel, float outTime)
	{
		FadeChannel(channel, 0, outTime);
	}

	//--------------------------------------------------
	// Crossfading channels
	//--------------------------------------------------

	public void CrossFadeChannels(int outAudio, float outTime, int inAudio, float inTime)
	{
		FadeOutChannel(outAudio, outTime);
		FadeInChannel(inAudio, setVolumeMusic, inTime);
	}

	public void CrossFadeChannels(int outAudio, float outTime, int inAudio, float inVol, float inTime)
	{
		FadeOutChannel(outAudio, outTime);
		FadeInChannel(inAudio, inVol, inTime);
	}

	//--------------------------------------------------
	// Plays a specific channel
	//--------------------------------------------------

	public void PlayChannel(int channel)
	{
		switch (channel)
		{
			case 1:
				Primary.Play();
				break;
			case 2:
				Secondary.Play();
				break;
			case 3:
				Tertiary.Play();
				break;
			default:
				Debug.LogWarning("PlayChannel: Channel " + channel + " not supported!");
				break;
		}
	}

	//===========================
	// Private utility functions
	//===========================

	private void CopyToChannel(int channel, AudioSource origin)
	{
		switch (channel)
		{
			case 1:
				CopyAudioSource(origin, Primary);
				break;
			case 2:
				CopyAudioSource(origin, Secondary);
				break;
			case 3:
				CopyAudioSource(origin, Tertiary);
				break;
			default:
				Debug.LogWarning("CopyToChannel: Channel " + channel + " not supported!");
				break;
		}
	}

	private void CopyAudioSource(AudioSource origin, AudioSource target)
	{
		target.clip = origin.clip;
		target.volume = origin.volume;
		target.spatialBlend = origin.spatialBlend;
		target.pitch = origin.pitch;
		target.loop = origin.loop;
		target.panStereo = origin.panStereo;
		target.playOnAwake = origin.playOnAwake;
	}

	private AudioSource ImportAudio(string type, string name, float vol, float blend, float pit, bool loop, float pan, bool awake)
	{
		AudioSource s = gameObject.AddComponent<AudioSource>();

		AudioClip loadedSound = (AudioClip)Resources.Load("Sounds/" + type + "/" + name, typeof(AudioClip));
		s.outputAudioMixerGroup = mixer.FindMatchingGroups("Master/SFX")[0];
		s.clip = loadedSound;
		s.volume = vol;
		s.spatialBlend = blend;
		s.pitch = pit;
		s.loop = loop;
		s.panStereo = pan;
		s.playOnAwake = awake;

		return s;
	}

	//==========================================================
	// PlayTrack() overloads
	// Instantiates the audio to one of the audio sources
	// and plays it automatically if set to do so
	//==========================================================

	// PlayTrack(int channel, string type, string name, bool loop, bool awake)
	// PlayTrack(int channel, string type, string name, float vol, float pit, bool loop, bool awake)
	// PlayFadeInTrack(int channel, string type, string name, bool loop, float inTime)
	// PlayFadeInTrack(int channel, string type, string name, float vol, float pit, float inTime)

	public void PlayTrack(int channel, string type, string name, bool loop, bool awake)
	{
		PlayTrack(channel, type, name, loop, awake, 1,1);
	}

	public void PlayTrack(int channel, string type, string name,  bool loop, bool awake, float vol, float pit)
	{
		//if the track is already playing, do nothing
		if (IsPlaying(name) != null)
			return;
		AudioSource temp = ImportAudio(type, name, vol, 0f, pit, loop, 0f, awake);
		CopyToChannel(channel, temp);
		Destroy(temp);
		if (awake)
		{
			PlayChannel(channel);
		}
	}

	public void PlayFadeInTrack(int channel, string type, string name, bool loop, float inTime)
	{
		AudioSource temp = ImportAudio(type, name, setVolumeMusic, 0f, 1f, loop, 0f, true);
		CopyToChannel(channel, temp);
		Destroy(temp);

		FadeInChannel(channel, 1, inTime);
		PlayChannel(channel);
	}

	public void PlayFadeInTrack(int channel, string type, string name, float vol, float pit, bool loop, float inTime)
	{
		AudioSource temp = ImportAudio(type, name, vol, 0f, pit, loop, 0f, true);
		CopyToChannel(channel, temp);
		Destroy(temp);

		FadeInChannel(channel, vol, inTime);
		PlayChannel(channel);
	}

	//==========================================================
	// Sound effect related functions
	//==========================================================

	// Function courtesy of Alan Teruel's workaround code
	private void PlayClipAtVector(AudioClip clip, Vector3 position, float delay, float vol, float pit, bool looping, bool surround)
	{
		// Add a new instance for the list of instances
		if (clip== null)
			return;
		GameObject dummyGameObject = new GameObject(clip.name);
		soundPlayerChilds.Add(dummyGameObject);
		int currentIndex = soundPlayerChilds.Count - 1;

		// Add the audio source component
		soundPlayerChilds[currentIndex].AddComponent<AudioSource>();
		soundPlayerChilds[currentIndex].transform.position = position;

		// Get the audio source component created
		AudioSource audioSource = soundPlayerChilds[currentIndex].GetComponent<AudioSource>();

		// Configure the audio source component
		audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("Master/SFX")[0];
		audioSource.clip = clip;
		audioSource.volume = vol;
		audioSource.pitch = pit;
		audioSource.loop = looping;
		audioSource.panStereo = 0f;
		if (surround)
			audioSource.spatialBlend = 1.0f;
		else
			audioSource.spatialBlend = 0f;

		// Starts playing the sound
		audioSource.PlayScheduled(AudioSettings.dspTime + delay);
	}

	private void PlayClipOnObject(AudioClip audioClip, GameObject obj, float delay, float vol, float pit, bool looping)
	{
		// Add a new instance for the list of instances
		if (audioClip == null)
			return;
		GameObject dummyGameObject = new GameObject(audioClip.name);
		soundPlayerChilds.Add(dummyGameObject);
		int currentIndex = soundPlayerChilds.Count - 1;

		// Add the audio source component
		soundPlayerChilds[currentIndex].AddComponent<AudioSource>();
		soundPlayerChilds[currentIndex].transform.parent = obj.transform;
		soundPlayerChilds[currentIndex].transform.position = obj.transform.position;

		// Get the audio source component created
		AudioSource audioSource = soundPlayerChilds[currentIndex].GetComponent<AudioSource>();

		// Configure the audio source component
		audioSource.clip = audioClip;
		audioSource.volume = vol;
		audioSource.pitch = pit;
		audioSource.loop = looping;
		audioSource.panStereo = 0f;
		audioSource.spatialBlend = 1.0f;

		// Starts playing the sound
		audioSource.PlayScheduled(AudioSettings.dspTime + delay);
	}

	public void AdjustVolumeSound(float vol)
	{
		setVolumeSound += vol;
		soundUpdate = false;
		for (int i = soundPlayerChilds.Count - 1; i >= 0; i--)
		{
			soundPlayerChilds[i].GetComponent<AudioSource>().volume += vol;
		}
		soundUpdate = true;
	}

	public void SetVolumeSound(float vol)
	{
		setVolumeSound = vol;
		soundUpdate = false;
		for (int i = soundPlayerChilds.Count - 1; i >= 0; i--)
		{
			soundPlayerChilds[i].GetComponent<AudioSource>().volume = vol;
		}
		soundUpdate = true;
	}

	//==========================================================
	// PlaySound() & PlaySoundDelayed() overloads
	// Instantiates audio purposed as sound effects
	// Can be set to play after a set delay
	//==========================================================

	// PlaySound(string name)
	// PlaySound(string name, bool looping, float delay)
	// PlaySound(string name, float vol, float pit, float delay)
	// PlaySound(string name, float vol, float pit, bool loop, float delay)
	// PlaySound(string name, float vol, float pit, bool loop, Vector3 vector, float delay)
	// PlayAttachedSound(string name, GameObject obj, float vol, float pit, bool loop, float delay)

	public void PlaySound(string name)
	{
		PlaySound(name, setVolumeSound, 1, false, playerObj.transform.position, 0f);
	}

	public void PlaySound(string name, bool loop, float delay)
	{
		PlaySound(name, setVolumeSound, 1, loop, playerObj.transform.position, 0f);
	}
	public void PlaySound(string name, float vol, float pit)
	{
		PlaySound(name, vol, pit, false, playerObj.transform.position, 0f);
	}
	public void PlaySound(string name, float vol, float pit, bool loop, Vector3 vector)
	{
		PlaySound(name, vol, pit, loop, vector, 0f);
	}
	public void PlaySound(string name, float vol, float pit, bool loop, Vector3 vector, float delay)
	{
		AudioSource temp = ImportAudio("SFX", name, vol, 0f, pit, loop, 0f, true);
		if (temp == null)
		{
			Debug.LogError("Sound: " + name + "not found!");
			return;
		}
		if (temp.playOnAwake)
		{
			PlayClipAtVector(temp.clip, vector, delay, vol, pit, loop, true);
		}
		Destroy(temp);
	}

	public void PlayAttachedSound(string name, GameObject obj)
	{
		AudioSource temp = ImportAudio("SFX", name, setVolumeSound, 0f, 1f, false, 0f, true);

		if (temp.playOnAwake)
		{
			PlayClipOnObject(temp.clip, obj, 0f, 1f, 1f, false);
		}

		Destroy(temp);
	}

	public void PlayAttachedSound(string name, GameObject obj, float vol, float pit, bool loop, float delay)
	{
		AudioSource temp = ImportAudio("SFX", name, vol, 0f, pit, loop, 0f, true);

		if (temp.playOnAwake)
		{
			PlayClipOnObject(temp.clip, obj, delay, vol, pit, loop);
		}

		Destroy(temp);
	}
	//Takes a string name of the type of sound effect to play, and randomly chooses one to play
	//example string value would be "Walk" or "Gunshot" , which gets a random SFX_Walk-00 case sensitive
	//note that if you pass a string value that more than one type of sound has in it's name this will be problematic
	public void PlayRandomSFXType(string name)
	{
		bool found = false;
		List<string> type = new List<string>();
		for (int i = 0; i < SFXNames.Count; i ++)
		{
			if (SFXNames[i].Contains(name))
			{
				found = true;
				//add the string to the list with sounds of this type
				type.Add(SFXNames[i]);
			}
		}
		int rng = Random.Range(0, type.Count - 1);
		if (found)
		{
			//play random sound of that same name
			PlaySound(type[rng]);
		}
		else
		{
			Debug.LogWarning("Audio: " + name + " not found!");
			return;
		}
	}
	public AudioSource IsPlaying(string name)
	{
		for (int i = 0; i < soundPlayerChilds.Count; i++)
		{
			if (soundPlayerChilds[i].GetComponent<AudioSource>().isPlaying && soundPlayerChilds[i].name.Contains(name))
			{
				return soundPlayerChilds[i].GetComponent<AudioSource>();
			}
		}
		//Debug.Log("Audio: '" + name + "' not currently playing");
		return null;
	}
	public void StopSound(string name)
	{
		bool found = false;
		for (int i = 0; i < soundPlayerChilds.Count; i++)
		{
			if (soundPlayerChilds[i].GetComponent<AudioSource>().isPlaying && soundPlayerChilds[i].name.Contains(name))
			{
				found = true;
				soundPlayerChilds[i].GetComponent<AudioSource>().Stop();
			}
		}
		if (found == false)
		{
			Debug.LogWarning("Audio: '" + name + "' not found!");
		}
	}
	public void StopAllSounds()
	{
		for (int i = 0; i < soundPlayerChilds.Count; i++)
		{
			soundPlayerChilds[i].GetComponent<AudioSource>().Stop();
		}
	}

	public void PlaySoundOneShot(string name)
	{
		PlaySound(name);
	}
}