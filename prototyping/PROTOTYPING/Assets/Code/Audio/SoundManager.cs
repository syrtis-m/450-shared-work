using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    AudioSource audioSource;
    public AudioClip playerHurtSound;
    public AudioClip enemyHurtSound;
    public AudioClip playerMoveSound;
    public AudioClip enemyMoveSound;
    public AudioClip playerDeathSound;
    public AudioClip enemyDeathSound;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayPlayerSoundHurt()
    {
        audioSource.PlayOneShot(playerHurtSound);
    }
    public void PlayEnemySoundHurt()
    {
        audioSource.PlayOneShot(enemyHurtSound);
    }
    public void PlayPlayerSoundMove()
    {
        audioSource.PlayOneShot(playerMoveSound);
    }
    public void PlayEnemySoundMove()
    {
        audioSource.PlayOneShot(enemyMoveSound);
    }
    public void PlayPlayerSoundDeath()
    {
        audioSource.PlayOneShot(playerDeathSound);
    }
    public void PlayEnemySoundDeath()
    {
        audioSource.PlayOneShot(enemyDeathSound);
    }
}
