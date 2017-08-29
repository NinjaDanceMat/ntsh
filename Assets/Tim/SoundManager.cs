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
        soundEffects[index].source.volume = soundEffects[index].volume;
        soundEffects[index].source.pitch = Random.Range(1- soundEffects[index].pitchVariation,1+ soundEffects[index].pitchVariation);
        soundEffects[index].source.Play();
        
    }


}

[System.Serializable]
public class SoundEffect
{

    public AudioSource source;
    public AudioClip clip;
    public float volume;
    public float pitchVariation;
}


