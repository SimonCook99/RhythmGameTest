using System;
using System.Collections;
using DTT.UI.ProceduralUI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour{

    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject cardChoosingPanel;
    [SerializeField] private GameObject gameOverPanel;

    [SerializeField] private TextMeshProUGUI scoreToPassText;
    [SerializeField] private TextMeshProUGUI startCreditSongsText;

    [SerializeField] private TextMeshProUGUI scoreBuffText;
    [SerializeField] private GameObject euphoriaBar;
    [SerializeField] private GameObject euphoriaBarMeter;
    [SerializeField] private GameObject euphoriaButtonInput;


    [SerializeField] private GameObject errorLimitCounter; //oggetto contenitore principale
    [SerializeField] private GameObject errorLimitCounterMeter; //metro interno del counter, di cui viene modificato il fill amount
    [SerializeField] private TextMeshProUGUI errorLimitCounterText; //testo che mostra il numero di errori attuale

    [SerializeField] private GameObject bossWarningPanel;
    [SerializeField] private GameObject congratulationsPanel;

    public static UIManager Instance {get; private set;}

    public event EventHandler OnGameStart;
    public event EventHandler OnEuphoriaReadySound; //evento che sarà intercettato dall'audioMamager
    public event EventHandler OnStopMenuSound;
    public event EventHandler OnBossWarningSound;
    public event EventHandler OnBossDefeatedSound;

    void Awake(){
        startPanel.SetActive(true);
        cardChoosingPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        
        scoreBuffText.gameObject.SetActive(false);
        startCreditSongsText.gameObject.SetActive(false);

        euphoriaBar.SetActive(false);
        euphoriaButtonInput.SetActive(false);
        errorLimitCounter.SetActive(false);

        bossWarningPanel.SetActive(false);
        congratulationsPanel.SetActive(false);

        if(Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start(){
        GameloopManager.Instance.OnScoreGoalSet += ShowScoreToPass;

        GameloopManager.Instance.OnSongEndedUI += ShowCardChoosingPanel;
        GameloopManager.Instance.OnRunEnded += ShowGameOverPanel;

        GameloopManager.Instance.OnShowEuphoriaUI += ShowEuphoriaUI;
        GameloopManager.Instance.OnShowErrorLimitUI += ShowErrorLimitUI;

        GameloopManager.Instance.OnShowBossWarningUI += ShowBossWarningUI;
        GameloopManager.Instance.OnBossDefeatedUI += ShowCongratulationsPanel;

        GameloopManager.Instance.OnUpdateErrorLimitUI += UpdateErrorLimitUI;

        GameloopManager.Instance.OnUpdateEuphoriaUI += UpdateEuphoriaUI;

        ScoreChainbuffManager.Instance.OnChainBuffActivated += ShowPointsBoostText;
    }

    private void ShowCongratulationsPanel(object sender, EventArgs e){
        congratulationsPanel.SetActive(true);
        OnBossDefeatedSound?.Invoke(this, EventArgs.Empty);
    }

    private void ShowBossWarningUI(object sender, EventArgs e){
        bossWarningPanel.SetActive(true);
        OnBossWarningSound?.Invoke(this, EventArgs.Empty);
    }

    
    public void HideBossWarningPanel(){
        bossWarningPanel.SetActive(false);

        GameloopManager.Instance.GetPlayableDirector().Play(); //faccio partire la canzone una volta finita l'animazione del pannello
    }

    //il primo parametro è il countdown attuale, che scende dal timer massimo a 0, il secondo è il timer massimo
    private void UpdateEuphoriaUI(float fillAmount){
        euphoriaBarMeter.GetComponent<RoundedImage>().fillAmount = fillAmount;

        if(fillAmount >= 1f){
            euphoriaButtonInput.SetActive(true);
            OnEuphoriaReadySound?.Invoke(this, EventArgs.Empty);
        }else{
            euphoriaButtonInput.SetActive(false);
        }
    }


    //il primo parametro è il numero attuale di errori effettuati dal giocatore, il secondo è il limite massimo consentito
    private void UpdateErrorLimitUI(int errorCounter, float errorLimit){
        //aggiorno la UI del counter degli errori massimi, ad esempio cambiando il testo o attivando/disattivando dei simboli
        errorLimitCounterText.text = errorCounter.ToString();
        Debug.Log("Valore normalizzato: " + errorCounter / errorLimit);
        errorLimitCounterMeter.GetComponent<RoundedImage>().fillAmount = errorCounter / errorLimit;
    }

    private void ShowEuphoriaUI(object sender, EventArgs e){
        euphoriaBar.SetActive(true);
    }

    private void ShowErrorLimitUI(object sender, EventArgs e){
        errorLimitCounter.SetActive(true);
    }

    private void ShowPointsBoostText(object sender, float pointsBuff){
        scoreBuffText.text = "+" + pointsBuff.ToString();
        scoreBuffText.gameObject.SetActive(true);

        StartCoroutine(HideScoreBuffTextDelay(1.5f));
    }

    IEnumerator HideScoreBuffTextDelay(float delay){
        yield return new WaitForSeconds(delay);
        scoreBuffText.gameObject.SetActive(false);
    }

    private void ShowGameOverPanel(object sender, EventArgs e){
        gameOverPanel?.SetActive(true);
    }

    private void ShowCardChoosingPanel(object sender, EventArgs e){
        cardChoosingPanel?.SetActive(true);

        //chiamo il metodo in CardManagerUI che si occupa di ricavare le carte e mostrarle seguendo il template
        CardManagerUI.Instance.ShowCards();

    }

    private void ShowScoreToPass(object sender, GameloopManager.ScoreToPass e){
    
        scoreToPassText.text = "Minimum required: " + e.levelScore.requiredScore.ToString();
    }

    //funzione chiamata dal tasto Credits del menu principale, che mostra o nasconde il testo dei crediti delle canzoni
    public void ToggleCreditsText(){
        startCreditSongsText.gameObject.SetActive(!startCreditSongsText.gameObject.activeSelf);
    }

    public void StartGame(){
        startPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        OnStopMenuSound?.Invoke(this, EventArgs.Empty); //fermo la musica del menù
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

    public void Quit(){
        Application.Quit();
    }

    private void OnDestroy(){
        GameloopManager.Instance.OnScoreGoalSet -= ShowScoreToPass;
        GameloopManager.Instance.OnSongEndedUI -= ShowCardChoosingPanel;
        GameloopManager.Instance.OnRunEnded -= ShowGameOverPanel;

        GameloopManager.Instance.OnShowEuphoriaUI -= ShowEuphoriaUI;
        GameloopManager.Instance.OnShowErrorLimitUI -= ShowErrorLimitUI;

        GameloopManager.Instance.OnShowBossWarningUI -= ShowBossWarningUI;

        GameloopManager.Instance.OnUpdateErrorLimitUI -= UpdateErrorLimitUI;

        GameloopManager.Instance.OnUpdateEuphoriaUI -= UpdateEuphoriaUI;

        ScoreChainbuffManager.Instance.OnChainBuffActivated -= ShowPointsBoostText;
    }

    public GameObject GetCardChoosingPanel(){
        return cardChoosingPanel;
    }

}
