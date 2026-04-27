using System;
using Dypsloom.RhythmTimeline.Core.Managers;
using Unity.VisualScripting;
using UnityEngine;

public class ScrollSpeedManager : MonoBehaviour{

    [SerializeField] private RhythmDirector rhythmDirector;

    void Start(){
        CardManagerUI.Instance.OnPlayerCardsListModified += HandleScrollSpeed;
    }

    public void HandleScrollSpeed(object sender, CardManagerUI.CardChosenEventArgs e){
        //float newScrollSpeed;

        switch(e.chosenCard.downgrade.name){
            case "Scrollspeed+ I":
                Debug.Log("Scroll Speed I chosen, increasing note speed by 10%");
                rhythmDirector.SetNoteSpeed(rhythmDirector.GetNoteSpeed() * 1.1f); //aumento la velocità del 20%
                break;
            case "Scrollspeed+ II":
                Debug.Log("Scroll Speed II chosen, increasing note speed by 30%");
                rhythmDirector.SetNoteSpeed(rhythmDirector.GetNoteSpeed() * 1.2f); //moltiplico per 1.2 perchè ho considerato l'1.1 precedente, quindi circa il 30% della velocità iniziale
                break;
            case "Scrollspeed+ III":
                Debug.Log("Scroll Speed III chosen, increasing note speed by 50%");
                rhythmDirector.SetNoteSpeed(rhythmDirector.GetNoteSpeed() * 1.2f); //stesso discorso, 1.1+1.2+1.2 è circa il 50% della velocità iniziale (anche se il valore non sarà preciso dato che sono valori concatenate)
                break;
            default:
                Debug.Log("Il downgrade scelto non è di tipo scroll speed");
                //se il downgrade non è un aumento di velocità, non faccio nulla
                break;
        }

        
    }
}
