using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour{

    [SerializeField] private AudioClip cardAppearanceSound;
    [SerializeField] private AudioClip cardClickSound;
    [SerializeField] private AudioClip difficultySettingsClickSound;
    [SerializeField] private AudioClip euphoriaReadySound;
    [SerializeField] private AudioClip euphoriaActivatedSound;
    [SerializeField] private AudioClip maxErrorLimitIncreasedSound;
    [SerializeField] private AudioClip bossWarningSound;
    [SerializeField] private AudioClip bossDefeatedSound;

    [SerializeField] private AudioMixer mainMixer;

    public static AudioManager instance;

    private void Awake(){
        instance = this;
    }



    private void Start(){

        UIManager.Instance.OnEuphoriaReadySound += PlayEuphoriaReadySound;
        UIManager.Instance.OnStopMenuSound += StopAmbientMusic;
        UIManager.Instance.OnBossWarningSound += PlayBossWarningSound;
        UIManager.Instance.OnBossDefeatedSound += PlayBossDefeatedSound;

        GameloopManager.Instance.OnCardsAppearSound += PlayCardAppearanceSound;
        GameloopManager.Instance.OnEuphoriaActivatedSound += PlayEuphoriaActivatedSound;
        GameloopManager.Instance.OnIncreasedMaxErrorLimitSound += PlayMaxErrorLimitSound;

        CardManagerUI.Instance.OnCardClickSound += PlayCardClickSound;

        DifficultySettingsPanelUI.instance.OnDifficultySelectedAudio += PlayDifficultySettingsSound;

        //in caso di back to menù dalla schermata di pausa, mi assicuro che al ricaricamento della scena, l'audio riprenda normalmente alla schermata iniziale
        AudioListener.pause = false;
        
    }

    private void PlayDifficultySettingsSound(object sender, EventArgs e){
        AudioSource.PlayClipAtPoint(difficultySettingsClickSound, Vector3.zero);
    }

    private void PlayBossDefeatedSound(object sender, EventArgs e){
        AudioSource.PlayClipAtPoint(bossDefeatedSound, Vector3.zero);
    }

    //creo un oggetto temporaneo che riproduce il suono e poi viene distrutto
    private void PlayBossWarningSound(object sender, EventArgs e){
        GameObject warningSoundGameobject = new GameObject("warningSoundGameobject");
        AudioSource source = warningSoundGameobject.AddComponent<AudioSource>();
        source.clip = bossWarningSound;
        source.PlayOneShot(bossWarningSound, 0.5f);

        Destroy(warningSoundGameobject, bossWarningSound.length);
    }

    private void PlayCardClickSound(object sender, EventArgs e){
        AudioSource.PlayClipAtPoint(cardClickSound, Vector3.zero);
    }

    private void PlayMaxErrorLimitSound(object sender, EventArgs e){
        Debug.Log("Suono aumento limite errori");
        AudioSource.PlayClipAtPoint(maxErrorLimitIncreasedSound, Vector3.zero, 4f);
    }
    
    private void PlayEuphoriaActivatedSound(object sender, EventArgs e){
        AudioSource.PlayClipAtPoint(euphoriaActivatedSound, Vector3.zero);
    }

    private void StopAmbientMusic(object sender, EventArgs e){
        GetComponent<AudioSource>().Stop();
    }

    private void PlayCardAppearanceSound(object sender, System.EventArgs e){
        AudioSource.PlayClipAtPoint(cardAppearanceSound, Vector3.zero);
    }

    private void PlayEuphoriaReadySound(object sender, System.EventArgs e){
        AudioSource.PlayClipAtPoint(euphoriaReadySound, Vector3.zero);
    }

    void OnDestroy(){
        UIManager.Instance.OnEuphoriaReadySound -= PlayEuphoriaReadySound;
        UIManager.Instance.OnStopMenuSound -= StopAmbientMusic;
        UIManager.Instance.OnBossWarningSound -= PlayBossWarningSound;
        UIManager.Instance.OnBossDefeatedSound -= PlayBossDefeatedSound;

        GameloopManager.Instance.OnCardsAppearSound -= PlayCardAppearanceSound;
        GameloopManager.Instance.OnEuphoriaActivatedSound -= PlayEuphoriaActivatedSound;
        GameloopManager.Instance.OnIncreasedMaxErrorLimitSound -= PlayMaxErrorLimitSound;

        CardManagerUI.Instance.OnCardClickSound -= PlayCardClickSound;

        DifficultySettingsPanelUI.instance.OnDifficultySelectedAudio -= PlayDifficultySettingsSound;
    }
}
