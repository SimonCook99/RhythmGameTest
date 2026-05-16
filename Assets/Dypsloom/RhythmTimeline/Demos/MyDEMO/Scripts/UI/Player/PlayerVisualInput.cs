using System;
using Dypsloom.RhythmTimeline.Core.Input;
using UnityEngine;

[Serializable]
public class InputVisual{
    public GameObject gameObjectInput;
    public SpriteRenderer inputSpriterenderer;
    public Sprite defaultSprite;
    public Sprite pressedSprite;
}

public class PlayerVisualInput : MonoBehaviour{

    //lista degli input visivi
    [SerializeField] private InputVisual[] inputsVisuals;


    //riferimento allo script di rhythmInputManager per vedere gli input della variabile TrackInput e gestire le visuals dinamicamente
    [SerializeField] private RhythmInputManager rhythmInputManager;
    private SimpleInputActionKey[] trackInputsArray;

    private void Awake(){
        trackInputsArray = rhythmInputManager.GetInputs();
    }


    private void Update(){
        if(GameloopManager.Instance.GetState() != GameloopManager.State.Playing){
            return;
        }

        CheckIfPressedInput();
    }

    private void CheckIfPressedInput(){

        //trackInputsArray tiene traccia degli input di rhythmInputManager, e agisce tramite i metodi GeiInputDown e GetInputUp dello script SimpleInputActionKey 
        for(int i = 0; i < trackInputsArray.Length; i++){
            if(trackInputsArray[i].GetInputDown()){
                inputsVisuals[i].inputSpriterenderer.sprite = inputsVisuals[i].pressedSprite;
                inputsVisuals[i].inputSpriterenderer.transform.localScale += new Vector3(1f,1f,1f);
            }

            if(trackInputsArray[i].GetInputUp()){
                inputsVisuals[i].inputSpriterenderer.sprite = inputsVisuals[i].defaultSprite;
                inputsVisuals[i].inputSpriterenderer.transform.localScale -= new Vector3(1f,1f,1f);
            }
        }
    }
}
