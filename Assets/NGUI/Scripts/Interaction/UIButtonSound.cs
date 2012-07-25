﻿//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Plays the specified sound.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Button Sound")]
public class UIButtonSound : MonoBehaviour
{
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
	}

	public AudioClip audioClip;
	public Trigger trigger = Trigger.OnClick;
	public float volume = 1f;
	public float pitch = 1f;

	void OnHover (bool isOver)
	{
		if (enabled && ((isOver && trigger == Trigger.OnMouseOver) || (!isOver && trigger == Trigger.OnMouseOut)))
		{
			volume = Game.EffectsVolume;
			NGUITools.PlaySound(audioClip, volume, pitch);
		}
	}

	void OnPress (bool isPressed)
	{
		if (enabled && ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnRelease)))
		{
			volume = Game.EffectsVolume;
			NGUITools.PlaySound(audioClip, volume, pitch);
		}
	}

	void OnClick ()
	{
		if (enabled && trigger == Trigger.OnClick)
		{
			volume = Game.EffectsVolume;
			NGUITools.PlaySound(audioClip, volume, pitch);
		}
	}
}