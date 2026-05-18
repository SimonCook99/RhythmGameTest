using Dypsloom.RhythmTimeline.Core.Input;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsPanelUI : MonoBehaviour{

    private const string PLAYER_BINDINGS = "InputBinding";

    //BOTTONI
    [SerializeField] private Button leftInputButton;
    [SerializeField] private Button leftInputButton2;
    [SerializeField] private Button downInputButton;
    [SerializeField] private Button downInputButton2;
    [SerializeField] private Button upInputButton;
    [SerializeField] private Button upInputButton2;
    [SerializeField] private Button rightInputButton;
    [SerializeField] private Button rightInputButton2;

    //TESTI
    [SerializeField] private TextMeshProUGUI leftInputText;
    [SerializeField] private TextMeshProUGUI leftInputText2;
    [SerializeField] private TextMeshProUGUI downInputText;
    [SerializeField] private TextMeshProUGUI downInputText2;
    [SerializeField] private TextMeshProUGUI upInputText;
    [SerializeField] private TextMeshProUGUI upInputText2;
    [SerializeField] private TextMeshProUGUI rightInputText;
    [SerializeField] private TextMeshProUGUI rightInputText2;

    [SerializeField] private GameObject pressToRebindPanel;


    [SerializeField] private RhythmInputManager rhythmInputManager;

    private SimpleInputActionKey[] inputs;

    private CanvasGroup optionsPanelCanvasGroup;

    private void Awake(){

        pressToRebindPanel.SetActive(false);

        inputs = rhythmInputManager.GetInputs();

        optionsPanelCanvasGroup = GetComponent<CanvasGroup>();
        
        ShowOrHidePressToRebindPanel(false);
        /* UpdateButtonVisuals(); */ //aggiorno i testi dei bottoni all'inizio


        //bottoni della prima riga di input (WASD, binding con indice 0)
        leftInputButton.onClick.AddListener(() => {
            ShowOrHidePressToRebindPanel(true);
            RebindInput(inputs[0].GetInputAction(), 0, 0);
        });
        downInputButton.onClick.AddListener(() => {
            ShowOrHidePressToRebindPanel(true);
            RebindInput(inputs[1].GetInputAction(), 0, 1);
        });
        upInputButton.onClick.AddListener(() => {
            ShowOrHidePressToRebindPanel(true);
            RebindInput(inputs[2].GetInputAction(), 0, 2);
        });
        rightInputButton.onClick.AddListener(() => {
            ShowOrHidePressToRebindPanel(true);
            RebindInput(inputs[3].GetInputAction(), 0, 3);
        });

        //bottoni della seconda riga di input (frecce direzionali, binding con indice 1)
        leftInputButton2.onClick.AddListener(() => {
            ShowOrHidePressToRebindPanel(true);
            RebindInput(inputs[0].GetInputAction(), 1, 0);
        });
        downInputButton2.onClick.AddListener(() => {
            ShowOrHidePressToRebindPanel(true);
            RebindInput(inputs[1].GetInputAction(), 1, 1);
        });
        upInputButton2.onClick.AddListener(() => {
            ShowOrHidePressToRebindPanel(true);
            RebindInput(inputs[2].GetInputAction(), 1, 2);
        });
        rightInputButton2.onClick.AddListener(() => {
            ShowOrHidePressToRebindPanel(true);
            RebindInput(inputs[3].GetInputAction(), 1, 3);
        });

        //Se ci sono già dei comandi salvati, impostarli in anticipo in modo da non perderli al ricaricamento del gioco
        for(int i = 0; i< inputs.Length; i++){
            string inputKey = PLAYER_BINDINGS + i;

            if(PlayerPrefs.HasKey(inputKey)){
                inputs[i].GetInputAction().LoadBindingOverridesFromJson(PlayerPrefs.GetString(inputKey));
            }
        }

        UpdateButtonVisuals(); //aggiorno i testi dei bottoni
        
    }

    public void HideOptionsPanel(){
        optionsPanelCanvasGroup.alpha = 0;
        optionsPanelCanvasGroup.blocksRaycasts = false;
        optionsPanelCanvasGroup.interactable = false;
    }

    public void ShowOrHidePressToRebindPanel(bool toggle){
        pressToRebindPanel.SetActive(toggle);
    }

    //il primo paramentro è l'inputAction su cui viene fatto il rebinding,
    // il secondo è l'intero che indica l'indice dell'array di bindings
    //il terzo è l'indice per identificare il tipo di input, in modo tale che vengano salvati 4 chiavi diverse in base all'inp
    private void RebindInput(InputAction inputAction, int rebindingIndex, int inputIndex){
        inputAction.Disable();
        
        inputAction.PerformInteractiveRebinding(rebindingIndex)
        .WithControlsHavingToMatchPath("<Keyboard>")
            .OnComplete(callback => {
                Debug.Log(callback.action.bindings[rebindingIndex].path);
                Debug.Log(callback.action.bindings[rebindingIndex].overridePath);

                callback.Dispose(); //non dovrebbe essere necessario ma per essere sicuri lo metto
                inputAction.Enable();

                //Salvo i comandi come JSON nella variabile PLAYER_BINDINGS
                Debug.Log(inputAction.SaveBindingOverridesAsJson());
                PlayerPrefs.SetString(
                    PLAYER_BINDINGS + inputIndex, 
                    inputAction.SaveBindingOverridesAsJson()
                );
                PlayerPrefs.Save();

                //aggiorno i testi del bottone col nuovo valore
                ShowOrHidePressToRebindPanel(false);
                UpdateButtonVisuals();
        }).Start();
    }

    private void UpdateButtonVisuals(){
        leftInputText.text = inputs[0].GetInputAction().bindings[0].ToDisplayString();
        downInputText.text = inputs[1].GetInputAction().bindings[0].ToDisplayString();
        upInputText.text = inputs[2].GetInputAction().bindings[0].ToDisplayString();
        rightInputText.text = inputs[3].GetInputAction().bindings[0].ToDisplayString();

        leftInputText2.text = inputs[0].GetInputAction().bindings[1].ToDisplayString();
        downInputText2.text = inputs[1].GetInputAction().bindings[1].ToDisplayString();
        upInputText2.text = inputs[2].GetInputAction().bindings[1].ToDisplayString();
        rightInputText2.text = inputs[3].GetInputAction().bindings[1].ToDisplayString();
    }
}
