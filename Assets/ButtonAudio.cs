using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ButtonAudio : MonoBehaviour
{

    private AudioSource _audio;

	void Start ()
	{
	    _audio = GetComponent<AudioSource>();
	}

    public void PlaySound()
    {
        _audio.Play();
    }
}
