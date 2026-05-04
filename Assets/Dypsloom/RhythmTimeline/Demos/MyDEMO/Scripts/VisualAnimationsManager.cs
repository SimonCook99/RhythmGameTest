using Dypsloom.RhythmTimeline.Core.Managers;
using UnityEngine;


//questo script gestisce le animazioni visive del gioco, con funzioni gestite tramite signal all'interno delle timeline di specifici brani
public class VisualAnimationsManager : MonoBehaviour{
    
    [SerializeField] private RhythmDirector rhythmDirector;

    [SerializeField] private Animator playerAnimator;

    void Start(){
    }



    //funziona che verrà chiamata dal signal che aumenterà la velocità del 20% in momenti specifici della canzone
    public void IncreaseSpeedSignal(){
        rhythmDirector.SetNoteSpeed(rhythmDirector.GetNoteSpeed() * 1.2f);
    }

    public void DecreaseSpeedSignal(){
        rhythmDirector.SetNoteSpeed(rhythmDirector.GetNoteSpeed() / 1.2f);
    }


    //funziona che verrà chiamata dal signal che resetterà la velocità al valore iniziale
    public void ResetSpeedSignal(){
        rhythmDirector.SetNoteSpeed(rhythmDirector.GetNoteSpeed() * 0.8f);
    }

    public void InputMovingSignal(){
        playerAnimator.SetTrigger("TriggerMove");
    }

}
