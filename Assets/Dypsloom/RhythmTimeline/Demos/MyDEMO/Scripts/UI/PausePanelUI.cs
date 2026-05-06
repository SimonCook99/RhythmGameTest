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

    private CanvasGroup canvasGroup; //canvas group del pannello di pausa
    private CanvasGroup yourCardsPanelCanvasGroup; //canvas group del pannello yourCards

    public static PausePanelUI instance;

    public event EventHandler OnResumeGame;
    public event EventHandler OnShowYourCardsUI;



    private void Awake(){

        resumeTimer = 3f;
        hasResumed = false;
        canvasGroup = GetComponent<CanvasGroup>();

        //non posso disattivare yorCardsPanel altrimenti la prima volta che lo chiamo non ascolta l'evento
        //quindi prendo il suo canvasGroup e lo disattivo da lì, e lo riattivo nella funzione ShowYourCardsPanel
        yourCardsPanelCanvasGroup = yourCardsPanel.GetComponent<CanvasGroup>();
        yourCardsPanelCanvasGroup.alpha = 0;
        yourCardsPanelCanvasGroup.interactable = false;
        yourCardsPanelCanvasGroup.blocksRaycasts = false;

        resumeButton.onClick.AddListener(ResumeGame);
        yourCardsButton.onClick.AddListener(ShowYourCardsPanel);

        instance = this;
    }

    private void ShowYourCardsPanel(){
        yourCardsPanelCanvasGroup.alpha = 1;
        yourCardsPanelCanvasGroup.interactable = true;
        yourCardsPanelCanvasGroup.blocksRaycasts = true;
        
        OnShowYourCardsUI?.Invoke(this, EventArgs.Empty);
    }

    private void Update(){
        if(hasResumed){
            countdownTextUI.gameObject.SetActive(true);

            //rendo invisibile il pannello di pausa
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            resumeTimer -= Time.unscaledDeltaTime;
            Debug.Log("Dovrebbe ora incrementare il timer, timer: " + resumeTimer);
            countdownTextUI.text = Mathf.Ceil(resumeTimer).ToString();

            if(resumeTimer <= 0f){
                OnResumeGame?.Invoke(this, EventArgs.Empty); //gameloopmanager ascolterà l'evento

                //resetto i dati
                countdownTextUI.gameObject.SetActive(false);
                resumeTimer = 3f;
                hasResumed = false;

            }
        }
    }

    private void ResumeGame(){
        hasResumed = true;
    }
}
