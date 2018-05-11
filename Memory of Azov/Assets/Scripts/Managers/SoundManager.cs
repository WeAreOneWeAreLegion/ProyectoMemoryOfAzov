using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager> {

    public enum SoundRequest { P_Knock, P_OpenDoor, P_ButtonPush, E_Cry }

    #region Public Variables
    [Header("\t    Own Script Variables")]
    [Header("Player Sounds")]
    [Tooltip("Lista de sonidos de golpe")]
    public List<AudioClip> knockSounds;
    [Tooltip("Lista de sonidos de abrir puerta")]
    public List<AudioClip> openDoorSounds;
    [Tooltip("Lista de sonidos de pulsar boton")]
    public List<AudioClip> buttonPushSounds;

    [Header("Enemy Sounds")]
    [Tooltip("Lista de sonidos de enemigos gritando")]
    public List<AudioClip> enemyCrySound;
    #endregion

    #region Sound Getters Methods
    public AudioClip GetSoundByRequest(SoundRequest sr)
    {
        switch (sr)
        {
            case SoundRequest.P_Knock:
                return GetPlayerKnockSound();
            case SoundRequest.P_OpenDoor:
                return GetPlayerOpenDoorSound();
            case SoundRequest.P_ButtonPush:
                return GetPlayerButtonPushSound();
            case SoundRequest.E_Cry:
                return GetEnemyCrySound();
        }

        return null;
    }

    //Player Sounds
    private AudioClip GetPlayerKnockSound()
    {
        return knockSounds[Random.Range(0, knockSounds.Count)];
    }

    private AudioClip GetPlayerOpenDoorSound()
    {
        return openDoorSounds[Random.Range(0, openDoorSounds.Count)];
    }

    private AudioClip GetPlayerButtonPushSound()
    {
        return buttonPushSounds[Random.Range(0, buttonPushSounds.Count)];
    }

    //Enemy Sounds
    private AudioClip GetEnemyCrySound()
    {
        return enemyCrySound[Random.Range(0, enemyCrySound.Count)];
    }
    
    //Object Sounds
    #endregion

}
