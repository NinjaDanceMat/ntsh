using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance; 


    public List<SoundEffect> soundEffects = new List<SoundEffect>();

    public void Awake()
    {
        if (SoundManager.instance == null)
        {
            SoundManager.instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void TriggerClip(int index)
    {
        soundEffects[index].source.clip = soundEffects[index].clip;
        soundEffects[index].source.Play();
    }
}

[System.Serializable]
public class SoundEffect
{
    public AudioSource source;
    public AudioClip clip;
}
