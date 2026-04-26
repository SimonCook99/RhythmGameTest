using Dypsloom.RhythmTimeline.Core.Managers;
using UnityEngine;

public class VisualAnimationsManager : MonoBehaviour{
    
    [SerializeField] private RhythmDirector rhythmDirector;


    [SerializeField] private GameObject leftInputGameObject;
    [SerializeField] private GameObject rightInputGameObject;
    [SerializeField] private GameObject upInputGameObject;
    [SerializeField] private GameObject downInputGameObject;

    [SerializeField] private Animator playerAnimator;

    void Start(){
    }



    //funziona che verrà chiamata dal signal che aumenterà la velocità del 20% in momenti specifici della canzone
    public void IncreaseSpeedSignal(){
        rhythmDirector.SetNoteSpeed(rhythmDirector.GetNoteSpeed() * 1.2f);
    }


    //funziona che verrà chiamata dal signal che resetterà la velocità al valore iniziale
    public void ResetSpeedSignal(){
        rhythmDirector.SetNoteSpeed(rhythmDirector.GetNoteSpeed() * 0.8f);
    }

    public void InputMovingSignal(){
        playerAnimator.SetTrigger("TriggerMove");
    }

}
