using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayScript : MonoBehaviour
{
public AudioSource source;
public AudioClip clip;


    void OnTriggerEnter(Collider other)
    {
        source.clip = clip;
        source.Play();
    }
}
