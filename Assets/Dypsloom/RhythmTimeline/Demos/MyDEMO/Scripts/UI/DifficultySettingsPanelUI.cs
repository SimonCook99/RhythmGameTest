using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySettingsPanelUI : MonoBehaviour{

    [SerializeField] private DifficultyAmountSO difficultyAmountSO; //ScriptableObject che sarà modificato al click dei singoli bottoni

    //evento che sarà intercettato dall'audio manager per riprodurre un SFX quando si cambia difficoltà
    public event EventHandler OnDifficultySelectedAudio;

    private CanvasGroup difficulyPanelCanvasGroup;
    
    public static DifficultySettingsPanelUI instance;

    private void Awake(){
        instance = this;

        difficulyPanelCanvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetGameDifficulty(int difficultyAmount){
        difficultyAmountSO.difficultyAmount = difficultyAmount;
        OnDifficultySelectedAudio?.Invoke(this,EventArgs.Empty);
    }

    public void HideDifficultyPanel(){
        difficulyPanelCanvasGroup.alpha = 0;
        difficulyPanelCanvasGroup.blocksRaycasts = false;
        difficulyPanelCanvasGroup.interactable = false;
    }

}
