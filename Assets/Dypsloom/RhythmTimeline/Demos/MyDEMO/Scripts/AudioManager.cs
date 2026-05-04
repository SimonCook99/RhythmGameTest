using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour{

    [SerializeField] private AudioClip cardAppearanceSound;
    [SerializeField] private AudioClip cardClickSound;
    [SerializeField] private AudioClip euphoriaReadySound;
    [SerializeField] private AudioClip euphoriaActivatedSound;
    [SerializeField] private AudioClip maxErrorLimitIncreasedSound;
    [SerializeField] private AudioClip bossWarningSound;
    [SerializeField] private AudioClip bossDefeatedSound;

    [SerializeField] private AudioMixer mainMixer;



    private void Start(){

        UIManager.Instance.OnEuphoriaReadySound += PlayEuphoriaReadySound;
        UIManager.Instance.OnStopMenuSound += StopAmbientMusic;
        UIManager.Instance.OnBossWarningSound += PlayBossWarningSound;
        UIManager.Instance.OnBossDefeatedSound += PlayBossDefeatedSound;

        GameloopManager.Instance.OnCardsAppearSound += PlayCardAppearanceSound;
        GameloopManager.Instance.OnEuphoriaActivatedSound += PlayEuphoriaActivatedSound;
        GameloopManager.Instance.OnIncreasedMaxErrorLimitSound += PlayMaxErrorLimitSound;

        CardManagerUI.Instance.OnCardClickSound += PlayCardClickSound;
        
    }

    private void PlayBossDefeatedSound(object sender, EventArgs e){
        AudioSource.PlayClipAtPoint(bossDefeatedSound, Vector3.zero);
    }

    private void PlayBossWarningSound(object sender, EventArgs e){
        AudioSource.PlayClipAtPoint(bossWarningSound, Vector3.zero, 6f);
        mainMixer.SetFloat("MainVolume", 10f);

        StartCoroutine(ResetVolume());
    }

    private IEnumerator ResetVolume(){
        yield return new WaitForSeconds(3f);
        mainMixer.SetFloat("MainVolume", 0f);
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
        
    }
}
