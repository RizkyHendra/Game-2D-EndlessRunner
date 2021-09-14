using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterSoundController : MonoBehaviour
{
    public AudioClip jump;
    private AudioSource audioPlayer;

   private void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
  public void PlayJump()
    {
        audioPlayer.PlayOneShot(jump);
    }
}
