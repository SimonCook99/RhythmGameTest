using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour{

    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject cardChoosingPanel;
    [SerializeField] private GameObject gameOverPanel;

    [SerializeField] private TextMeshProUGUI scoreToPassText;

    public static UIManager Instance {get; private set;}

    public event EventHandler OnGameStart;

    void Awake(){
        startPanel.SetActive(true);
        cardChoosingPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        Instance = this;
    }

    void Start(){
        GameloopManager.Instance.OnScoreGoalSet += ShowScoreToPass;

        GameloopManager.Instance.OnSongEndedUI += ShowCardChoosingPanel;
        GameloopManager.Instance.OnRunEnded += ShowGameOverPanel;
    }

    private void ShowGameOverPanel(object sender, EventArgs e){
        gameOverPanel.SetActive(true);
    }

    private void ShowCardChoosingPanel(object sender, EventArgs e){
        cardChoosingPanel.SetActive(true);

        //chiamo il metodo in CardManagerUI che si occupa di ricavare le carte e mostrarle seguendo il template
        CardManagerUI.Instance.ShowCards();

    }

    private void ShowScoreToPass(object sender, GameloopManager.ScoreToPass e){
    
        scoreToPassText.text = "Minimum required: " + e.levelScore.requiredScore.ToString();
    }

    public void StartGame(){
        startPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        OnGameStart?.Invoke(this, EventArgs.Empty);
    }

    //funzione chiamata dal tasto di retry del gameover, che ricarica la scena
    //ho provato la soluzione asincrona per chiamare il restartgame post caricamento scena, ma sembra che l'oggetto venga distrutto prima
    //VEDI COME RISOLVERE IN CASO, MAGARI FACENDO UN RESET MANUALE DEI DATI INVECE DI CARICARE LA SCENA (anche se ho dei dubbi sulla lista dei charting) o eventualmente lasciare solo il tasto di back to menù
    public void RestartGame(){
        StartCoroutine(RestartNewGame());
    }

    private IEnumerator RestartNewGame(){
        Debug.Log("Restarting game...");
        var currentScene = SceneManager.GetActiveScene().name;

        //caricamento asincrono della scena, per evitare blocchi durante il caricamento
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentScene);

        while (!asyncLoad.isDone){
            yield return null;
        }

        Debug.Log("Scene reloaded, starting game...");
        StartGame(); //solo una volta completato il caricamento della scena, chiamo StartGame() per far partire il gioco
    }

    public void BackToMainMenu(){
        /* startPanel.SetActive(true);
        cardChoosingPanel.SetActive(false);
        gameOverPanel.SetActive(false); */
        var currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    private void OnDestroy(){
        GameloopManager.Instance.OnScoreGoalSet -= ShowScoreToPass;
        GameloopManager.Instance.OnSongEndedUI -= ShowCardChoosingPanel;
        GameloopManager.Instance.OnRunEnded -= ShowGameOverPanel;
    }

    public GameObject GetCardChoosingPanel(){
        return cardChoosingPanel;
    }

}
