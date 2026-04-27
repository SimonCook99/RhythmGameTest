using System;
using Dypsloom.RhythmTimeline.Scoring;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreChainbuffManager : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;

    public event EventHandler<float> OnChainBuffActivated;

    public static ScoreChainbuffManager Instance {get; private set;}

    private void Awake(){
        Instance = this;
    }

    private void Start(){
        scoreManager.OnContinueChain += HandleChainBuff;
    }

    private void HandleChainBuff(int chain){

        int chainBuffIndex = GameloopManager.Instance.GetChainBuffIndex();

        if(chainBuffIndex == 0) return;

    
        //quando raggiunge un multiplo di 20, controlla l'indice del chain buff e aumenta i punti di conseguenza
        if(chain % 20 == 0){
            
            if(chainBuffIndex == 1){ //il giocatore ha chain buff I
                Debug.Log("Buff di 500 punti ottenuto!");
                scoreManager.AddScoreFlat(500);
                OnChainBuffActivated?.Invoke(this, 500);
            }
            if(chainBuffIndex == 2){ //il giocatore ha chain buff II
                Debug.Log("Buff di 1000 punti ottenuto!");
                scoreManager.AddScoreFlat(1000);
                OnChainBuffActivated?.Invoke(this, 1000);
            }
            if(chainBuffIndex == 3){ //il giocatore ha chain buff III
                Debug.Log("Buff di 1500 punti ottenuto!");
                scoreManager.AddScore(1500);
                OnChainBuffActivated?.Invoke(this, 1500 * scoreManager.GetMultiplier());
            }
        }

    }
}
