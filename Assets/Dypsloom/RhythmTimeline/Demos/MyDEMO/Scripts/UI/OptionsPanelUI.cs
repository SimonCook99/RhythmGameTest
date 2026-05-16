using Dypsloom.RhythmTimeline.Core.Input;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsPanelUI : MonoBehaviour{

    [SerializeField] private Button leftInputButton;
    [SerializeField] private Button downInputButton;
    [SerializeField] private Button upInputButton;
    [SerializeField] private Button rightInputButton;
    
    [SerializeField] private TextMeshProUGUI leftInputText;
    [SerializeField] private TextMeshProUGUI downInputText;
    [SerializeField] private TextMeshProUGUI upInputText;
    [SerializeField] private TextMeshProUGUI rightInputText;
    
    [SerializeField] private RhythmInputManager rhythmInputManager;

    private SimpleInputActionKey[] inputs;

    private CanvasGroup optionsPanelCanvasGroup;

    private void Awake(){
        inputs = rhythmInputManager.GetInputs();

        optionsPanelCanvasGroup = GetComponent<CanvasGroup>();

        //float axisValue = inputs[0].GetAxisValue();
        
        
        leftInputText.text = inputs[0].GetKey().ToString();
        downInputText.text = inputs[1].GetKey().ToString();
        upInputText.text = inputs[2].GetKey().ToString();
        rightInputText.text = inputs[3].GetKey().ToString();

        leftInputButton.onClick.AddListener(() => {
            RebindInput(inputs[0]);
        });
        downInputButton.onClick.AddListener(() => {
            RebindInput(inputs[1]);
        });
        upInputButton.onClick.AddListener(() => {
            RebindInput(inputs[2]);
        });
        rightInputButton.onClick.AddListener(() => {
            RebindInput(inputs[3]);
        });
        
    }

    public void HideOptionsPanel(){
        optionsPanelCanvasGroup.alpha = 0;
        optionsPanelCanvasGroup.blocksRaycasts = false;
        optionsPanelCanvasGroup.interactable = false;
    }

    private void RebindInput(SimpleInputActionKey key){
        key.GetInputAction().Disable();
        
        key.GetInputAction().PerformInteractiveRebinding(0).OnComplete(callback => {
            Debug.Log(callback.action.bindings[1].path);
            Debug.Log(callback.action.bindings[1].overridePath);
            key.GetInputAction().Enable();
        }).Start();
    }
}
