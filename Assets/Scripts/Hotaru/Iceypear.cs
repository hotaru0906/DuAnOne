using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iceypear : MonoBehaviour
{
    public AudioClip iceStartSound;
    public AudioSource audioSource;
    void Start()
    {
        if (audioSource != null && iceStartSound != null)
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.loop = false;
            audioSource.PlayOneShot(iceStartSound);
        }
    }
    public void Iceyapear()
    {
        Destroy(gameObject);
    }
}
