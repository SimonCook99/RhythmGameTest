using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PausePanelUI : MonoBehaviour{
    
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button yourCardsButton;
    [SerializeField] private GameObject yourCardsPanel;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private TextMeshProUGUI countdownTextUI;

    private bool hasResumed;

    private float resumeTimer;

    public static PausePanelUI instance;

    public event EventHandler OnResumeGame;
    public event EventHandler OnShowYourCardsUI;



    private void Awake(){

        resumeTimer = 3f;
        hasResumed = false;

        //non posso disattivare yorCardsPanel altrimenti la prima volta che lo chiamo non ascolta l'evento
        //quindiinizialmente è attivo con lo scale a 0 e poi viene portato a 1 quando viene cliccato il tasto
        yourCardsPanel.SetActive(true);
        yourCardsPanel.transform.localScale = Vector3.zero;

        resumeButton.onClick.AddListener(ResumeGame);
        yourCardsButton.onClick.AddListener(ShowYourCardsPanel);

        instance = this;
    }

    private void ShowYourCardsPanel(){
        yourCardsPanel.transform.localScale = Vector3.one;
        OnShowYourCardsUI?.Invoke(this, EventArgs.Empty);
    }

    private void Update(){
        if(hasResumed){
            countdownTextUI.gameObject.SetActive(true);

            transform.localScale = Vector3.zero;

            resumeTimer -= Time.unscaledDeltaTime;
            Debug.Log("Dovrebbe ora incrementare il timer, timer: " + resumeTimer);
            countdownTextUI.text = Mathf.Ceil(resumeTimer).ToString();

            if(resumeTimer <= 0f){
                OnResumeGame?.Invoke(this, EventArgs.Empty); //gameloopmanager ascolterà l'evento

                //resetto i dati
                countdownTextUI.gameObject.SetActive(false);
                resumeTimer = 3f;
                hasResumed = false;
                transform.localScale = Vector3.one;
                gameObject.SetActive(false);

            }
        }
    }

    private void ResumeGame(){
        hasResumed = true;
    }
}
