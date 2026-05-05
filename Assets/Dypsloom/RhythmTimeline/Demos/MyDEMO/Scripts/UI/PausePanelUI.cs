using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PausePanelUI : MonoBehaviour{
    
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button yourCardsButton;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private TextMeshProUGUI countdownTextUI;

    public event EventHandler OnResumeGame;
    private bool hasResumed;

    private float resumeTimer;

    public static PausePanelUI instance;


    private void Awake(){

        resumeTimer = 3f;
        hasResumed = false;

        resumeButton.onClick.AddListener(ResumeGame);

        instance = this;
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
