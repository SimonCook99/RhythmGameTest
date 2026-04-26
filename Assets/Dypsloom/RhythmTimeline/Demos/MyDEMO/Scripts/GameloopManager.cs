using System;
using System.Collections.Generic;
using System.Linq;
using Dypsloom.RhythmTimeline.Core;
using Dypsloom.RhythmTimeline.Core.Managers;
using Dypsloom.RhythmTimeline.Effects;
using Dypsloom.RhythmTimeline.Scoring;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class GameloopManager : MonoBehaviour{

    public class ScoreToPass : EventArgs{
        public LevelScoreSO levelScore;
    }
    public event EventHandler<ScoreToPass> OnScoreGoalSet;

    public event EventHandler OnSongEndedUI;
    public event EventHandler OnRunEnded;

    private enum State{
        Menu,
        Playing,
        ChoosingCard
    }

    private State state;


    [SerializeField] private ScoreManager scoreManager;

    [SerializeField] private LevelScoreSO currentLevelScore; //livello attuale

    [SerializeField] private List<LevelScoreSO> levelScoresList; //lista dei punteggi richiesti nei vari livelli

    [SerializeField] private List<RhythmTimelineAsset> songChartsList; //lista del chart di tutte le canzoni presenti in game

    [SerializeField] private List<CardUpgradeSO> allUpgradesList; //lista che contiene tutti gli upgrades del gioco, che sarà poi modificata in base alle carte in possesso del giocatore
    [SerializeField] private List<CardDowngradeSO> allDowngradesList; //lista che contiene tutti gli downgrades del gioco, che sarà poi modificata in base alle carte in possesso del giocatore
    [SerializeField] private RhythmDirector rhythmDirector; //il rhythm director che gestisce la timeline del livello attuale

    [SerializeField] private PlayableDirector playableDirector;



    private List<CardSO> cardToChooseList; //lista che conterrà le carte che il giocatore potrà scegliere al termine della canzone, inizialmente vuota
    private int currentLevelIndex; //l'indice che aumenterà a fine canzone e regola il livello corrente dalla lista
    
    /* [SerializeField] */ private bool hasPlayerX2Base = false; //Variabile che monitora se il giocatore ha l'upgrade x2 base, aggiornando il punteggio di conseguenza
    /* [SerializeField] */ private bool hasPlayerX6 = false; //Variabile che monitora se il giocatore ha l'upgrade x6, aggiornando il punteggio di conseguenza
    /* [SerializeField] */ private bool hasHarderMultiplier = false; //Variabile che monitora se il giocatore ha il downgrade che rende più difficile raggiungere il moltiplicatore successivo
    /* [SerializeField] */ private bool hasMaxErrorLimit = false; //Variabile che monitora se il giocatore ha il downgrade "Max Error Limit", che limita il numero di errori massimi per canzone tramite un contatore dedicato
    
    [SerializeField] private bool hasDoubleError = false; //Variabile che monitora se il giocatore ha il downgrade "Double Error", che raddoppia la perdita di punti in caso di bad o miss (e anche il contatore in caso abbia anche il downgrade "Max Error Limit")
    [SerializeField] private ScoreSettings scoreSettingsSO; //file scriptableObject che contiene i punteggi delle varie accuracy, utile per raddoppiare i punti da sottrare in caso di bad o miss col downgrade "Double Error"
    
    [SerializeField] private bool hasEuphoria = false; //Variabile che monitora se il giocatore ha l'upgrade "Euphoria", che raddoppia temporaneamente il moltiplicatore
    [SerializeField] private float euphoriaDuration = 5f; //durata dell'effetto di euphoria, in secondi
    [SerializeField] private float euphoriaCooldown = 20f; //Il giocatore potrà attivarlo dopo almeno 20 secondi
    private bool euphoriaActive = false;
    
    /* [SerializeField] private int harderMultiplierCounter = 5; */ //quando il player ha il downgrade "HarderMultiplier", questo valore sarà sommato a quello base per settare il moltiplicatore

    //la List<Monobehaviour> serve come ponte per aggiungere gli elemetni dall'inspector, poi la lista di interfaccia è quella che viene utilizzata effettivamente nel metodo CheckProgressiveUpgradeOrDowngrade
    [SerializeField] private int chainBuffIndex = 0;
    [SerializeField] private List<ScriptableObject> chainBuffListSOPonte;
    private List<IEquipable> chainBuffListSO => chainBuffListSOPonte.OfType<IEquipable>().ToList();

    //[SerializeField] private List<CardUpgradeSO> chainBuffListSO; //lista con gli upgrade successivi di chain buff I


    [SerializeField] private int scrollSpeedIncreaseIndex = 0;
    [SerializeField] private List<ScriptableObject> scrollSpeedIncreaseListSOPonte;
    private List<IEquipable> scrollSpeedIncreaseListSO => scrollSpeedIncreaseListSOPonte.OfType<IEquipable>().ToList();
    //[SerializeField] private List<CardDowngradeSO> scrollSpeedIncreaseListSO; //lista con i downgrade successivi di scrollspeed+ I


    [SerializeField] private int[] defaultMultiplierChanges = new int[] {10, 20, 30, 40, 50}; //array che contiene i valori base a cui cambiare moltiplicatore, usato per resettare il contatore dell'harder multiplier a ogni inizio canzone
    private bool hasChangedMultiplier = false; //variabile che monitora se è stato già modificato l'array di moltiplicatore (in caso il gicoatore abbia harder multiplier), in modo da non incrementare i valori a ripetizione
    
    private int maxErrorLimitCounter = 0; //contatore che conterà il numero di volte in cui il giocatore romperà la combo con un bad o miss, arrivato a 20 sarà gameover


    public static GameloopManager Instance {get; private set;}

    


    void Awake(){
        currentLevelIndex = 0;
        currentLevelScore = levelScoresList[currentLevelIndex];
        cardToChooseList = new List<CardSO>();
        state = State.Menu;

        Instance = this;
    }

    void Start(){

        UIManager.Instance.OnGameStart += StartGame;

        playableDirector.stopped += VictoryCheck;

        scoreManager.OnBreakChain += HandleBreakChain;

        
    }

    private void HandleBreakChain(){

        if(!hasMaxErrorLimit) return; //se il giocatore non ha il downgrade del max error limit, non serve fare nulla quando rompe la combo


        //VEDI COME INTERCETTARE L'ULTIMA ACCURACY DELLA NOTA (bad o miss) E TOGLIERE IL DOPPIO DEL PUNTEGGIO IN BASE AL POSSESSO DEL DOWNGRADE "Double Error", IN MODO DA FARLO CONTARE ANCHE NEL CALCOLO DEL CONTATORE DEGLI ERRORI, CHE SE ARRIVA A 20 FA SCATTARE IL GAMEOVER
        /* if(hasDoubleError){

            if(scoreManager.GetAccuracy())

            scoreManager.AddScore(scoreSettingsSO.GetMissAccuracy().score);

            //se il giocatore ha anche il downgrade double error, allora raddoppia la perdita di punti e il contatore degli errori
            scoreManager.AddScore(-scoreSettingsSO.MissAccuracy.score); //sottraggo i punti come se fosse un miss, ma raddoppiati
            maxErrorLimitCounter++; //incremento una volta in più il contatore degli errori
        } else {
            scoreManager.AddScore(-scoreSettingsSO.BadAccuracyTable[0].score); //sottraggo i punti come se fosse un bad, senza raddoppiarli
        } */
        maxErrorLimitCounter++;
        Debug.Log("Numero di errori: " + maxErrorLimitCounter);

        if(maxErrorLimitCounter >= 20){
            Debug.Log("Hai raggiunto il limite massimo di errori! Riprova il livello.");

            //reimposto i valori iniziali
            currentLevelIndex = 0;
            currentLevelScore = levelScoresList[currentLevelIndex];
            state = State.Menu;
            maxErrorLimitCounter = 0;
            hasMaxErrorLimit = false;

            //ERRORE SU RhythmMixerBehaviour AL RIGO 18, MA NON SEMBRA ESSERE BLOCCANTE
            rhythmDirector.EndSong();
            playableDirector.Stop(); //fermo la canzone in corso

            //questo evento serve sia alla UI per mostrare il pannello di gameover, sia al player per svuotare le card in suo possesso
            OnRunEnded?.Invoke(this, EventArgs.Empty);
        }
        
    }

    private void VictoryCheck(PlayableDirector director){
        if(scoreManager.GetScore() >= currentLevelScore.requiredScore){
            Debug.Log("Hai vinto il livello!");

            state = State.ChoosingCard;

            cardToChooseList = CreateRandomCards();


            OnSongEndedUI?.Invoke(this, EventArgs.Empty);
            /* currentLevelIndex++;
            if(currentLevelIndex < levelScoresList.Count){
                currentLevelScore = levelScoresList[currentLevelIndex];
                StartGame(this, EventArgs.Empty);
            } else {
                Debug.Log("Hai vinto il gioco!");
            } */
        } else {
            Debug.Log("Hai perso! Riprova il livello.");

            //reimposto i valori iniziali
            currentLevelIndex = 0;
            currentLevelScore = levelScoresList[currentLevelIndex];
            state = State.Menu;
            
            //questo evento serve sia alla UI per mostrare il pannello di gameover, sia al player per svuotare le card in suo possesso
            OnRunEnded?.Invoke(this, EventArgs.Empty);
        }
    }


    private List<CardSO> CreateRandomCards(){
        List<CardSO> cardsList = new List<CardSO>();

        for(int i = 0; i < 3; i++){

            
            int randomUpgradeIndex = UnityEngine.Random.Range(0, allUpgradesList.Count);
            int randomDowngradeIndex = UnityEngine.Random.Range(0, allDowngradesList.Count);

            CardSO card = ScriptableObject.CreateInstance<CardSO>();

            card.upgrade = allUpgradesList[randomUpgradeIndex];
            card.downgrade = allDowngradesList[randomDowngradeIndex];

            cardsList.Add(card);
            
        }


        return cardsList;

    }

    private void StartGame(object sender, EventArgs e){

        //seleziona un chart iniziale random
        playableDirector.playableAsset = songChartsList[UnityEngine.Random.Range(0, songChartsList.Count)];
	    scoreManager.SetSong(playableDirector.playableAsset.ConvertTo<RhythmTimelineAsset>());
        playableDirector.Play();
        state = State.Playing;

        List<IEquipable> playerUpgradesList = Player.Instance.GetAllUpgradesList();
        List<IEquipable> playerDowngradesList = Player.Instance.GetAllDowngradesList();

        //questo metodo gestisce gli upgrade/downgrade che usano variabili booleane (il giocatore ce l'ha o non ce l'ha)
        CheckUpgradeOrDownGrade("X2 Base", ref hasPlayerX2Base, playerUpgradesList);
        CheckUpgradeOrDownGrade("X6", ref hasPlayerX6, playerUpgradesList);
        CheckUpgradeOrDownGrade("Euphoria", ref hasEuphoria, playerUpgradesList);

        CheckUpgradeOrDownGrade("Harder Multiplier", ref hasHarderMultiplier, playerDowngradesList);
        CheckUpgradeOrDownGrade("Max Error Limit", ref hasMaxErrorLimit, playerDowngradesList);

        //Reimposto il contatore dell'harder multiplier al suo valore iniziale, in caso sia stato aumentato nel corso della canzone precedente
        /* ResetHarderMultiplierCounter(); */

        //Triggero l'evento per permettere alla UI di mostrare il punteggio da raggiungere
        OnScoreGoalSet?.Invoke(this, new ScoreToPass{
                levelScore = currentLevelScore
            }
        );
    }


    //questa funzione, oltre che allo startGame, viene inserita anche nell'inspector all'evento di BreakChain
    /* public void ResetHarderMultiplierCounter(){
        harderMultiplierCounter = 5;
    } */


    //funzione che viene chiamata a inizio canzone, vede se è presente un determinato upgrade e in caso, modifica la sua variabile booleana dedicata
    private void CheckUpgradeOrDownGrade(string name, ref bool toggle, List<IEquipable> playerUpgradeOrDowngradeList){
        if(toggle) return; //se il giocatore ha già l'upgrade/downgrade in questione, non serve controllare di nuovo

        //se tra gli upgrade del giocatore c'è quello passato come parametro, allora cambio il valore della variabile booleana dedicata
        if(playerUpgradeOrDowngradeList.Find(x => x.Name == name) != null){
            toggle = true;
            HandleMultiplier(); //(SPECIFICO PER X2 BASE) chiamo manualmente la funzione per aggiornare il moltiplicatore allo start, ancora prima di qualsiasi nota
        }
    }

    //funzione che controlla se la carta che il giocatore ha appena scelto appartiene a un upgrade progressivo
    //(es. chain buff, scroll speed ecc.) e aggiornano la lista globale, aggiungendo il livello successivo dell'upgrade in questione da poter trovare
    private void CheckProgressiveUpgradeOrDowngrade(CardSO chosedCard, string name, List<IEquipable> progressionList, ref int index){        

        if(index >= progressionList.Count) return; //se l'indice è già al massimo, non serve controllare di nuovo

        //se quello che sto controllando è un upgrade allora controllo solo se l'upgrade preso è un upgrade progressivo,
        //stessa logica se la funzione sta controllando un downgrade
        if(progressionList[index].isUpgrade){
        
            if(chosedCard.upgrade.Name == name + " I"){
                index++;
                allUpgradesList.Add((CardUpgradeSO)progressionList[0]);
            }else if (chosedCard.upgrade.Name == name + " II"){
                index++;
                allUpgradesList.Add((CardUpgradeSO)progressionList[1]);
            }else if(chosedCard.upgrade.Name == name + " III"){
                index++;
            }

        }else if (progressionList[index].isDowngrade){

            if(chosedCard.downgrade.Name == name + " I"){
                index++;
                allDowngradesList.Add((CardDowngradeSO)progressionList[0]);
            }else if (chosedCard.downgrade.Name == name + " II"){
                index++;
                allDowngradesList.Add((CardDowngradeSO)progressionList[1]);
            }else if(chosedCard.downgrade.Name == name + " III"){
                index++;
            }

        }


    }

    public void NextLevel(CardSO chosenCard){

        //Rimuovere l'upgrade della carta dalla lista globale degli upgrade, in modo che non esca più come upgrade possibile per le carte future
        allUpgradesList.Remove(chosenCard.upgrade);
        
        
        //passo la card appena scelta, il nome dell'upgrade/downgrade da controllare, la lista di upgrade/downgrade in progressione di riferimento e l'indice relativo, utile a livello di gameplay
        //questo metodo gestisce gli upgrade/downgrade in progressione, usando un'indice da 1 a 3
        CheckProgressiveUpgradeOrDowngrade(chosenCard, "Chain Buff", chainBuffListSO, ref chainBuffIndex);
        CheckProgressiveUpgradeOrDowngrade(chosenCard, "Scrollspeed+", scrollSpeedIncreaseListSO, ref scrollSpeedIncreaseIndex);

        


        //Rimuovere il downgrade dalla lista globale degli downgrade, in modo che non esca più come upgrade possibile per le carte future
        allDowngradesList.Remove(chosenCard.downgrade);

        //rimuovere anche la canzone appena fatta dalla lista globale dei charts
        songChartsList.Remove(playableDirector.playableAsset.ConvertTo<RhythmTimelineAsset>());

        //aggiornare i vari valori, come l'indice, lo scoregoal da raggiungere, il counter di errori massimi ecc.
        currentLevelIndex++;

        if(maxErrorLimitCounter != 0){ //resetto il counter di errori in caso sia stato modificato durante la canzone precedente
            maxErrorLimitCounter = 0;
        }

        if(currentLevelIndex < levelScoresList.Count){
            currentLevelScore = levelScoresList[currentLevelIndex];
            StartGame(this, EventArgs.Empty); //chiamo la funzione startGame per la prossima canzone
        } else {
            Debug.Log("Hai vinto il gioco!");
        }

    }


    private void Update(){

        /* Debug.Log("State attuale: " + state); */

        switch(state){
            case State.Menu:
                //logica del menu
                break;
            case State.Playing:

                if(hasEuphoria && euphoriaCooldown > 0){
                    euphoriaCooldown -= Time.deltaTime;
                }

                if(euphoriaActive){
                    euphoriaDuration -= Time.deltaTime;
                    Debug.Log("Durata rimanente di Euphoria: " + euphoriaDuration + " secondi");
                
                    //riporta il moltiplicatore al valore precedente e resetto i contatori di euphoria
                    if(euphoriaDuration <= 0){
                        scoreManager.SetMultiplier(scoreManager.GetMultiplier() / 2);
                        euphoriaActive = false; 
                        euphoriaDuration = 5f; //resetto la durata per la prossima volta
                        euphoriaCooldown = 20f; //resetto il cooldown per la prossima volta
                        Debug.Log("Euphoria finita, moltiplicatore reimpostato!");
                    }
                }

                if(Keyboard.current.spaceKey.wasPressedThisFrame){
                    if(hasEuphoria && euphoriaCooldown <= 0){
                        euphoriaActive = true; //segno l'euphoria come attiva
                        scoreManager.SetMultiplier(scoreManager.GetMultiplier() * 2); //raddoppia il moltiplicatore attuale
                        Debug.Log("Euphoria attivata, moltiplicatore raddoppiato!");
                    }
                }

                //DEBUG RAPIDO PER TERMINARE LA CANZONE NON APPENA SI RAGGIUNGE L'OBIETTIVO IN PUNTI
                //DA RIMUOVERE
                if(Keyboard.current.pKey.wasPressedThisFrame){
                    scoreManager.AddScore(currentLevelScore.requiredScore);
                    rhythmDirector.EndSong();
                    VictoryCheck(playableDirector);
                }

                break;
            case State.ChoosingCard:
                //logica di scelta carta
                break;
        }

        
        
    }

    //funzione messa nell'inspector, nello score event receiver script
    public void HandleMultiplier(){

        int currentChain = scoreManager.GetChain();

        //moltiplicatore dell'euphoria, di default inattivo (x1) e che diventa x2 quando attivo
        int euphoriaMultiplier = 1;

        if(euphoriaActive){
            euphoriaMultiplier = 2;
        }

        if(hasHarderMultiplier && !hasChangedMultiplier){
            defaultMultiplierChanges[0] += 5; //valore minimo per il x2 (10 => 15)
            defaultMultiplierChanges[1] += 10; //valore minimo per il x3 (20 => 30)
            defaultMultiplierChanges[2] += 15; //valore minimo per il x4 (30 => 45)
            defaultMultiplierChanges[3] += 20; //valore minimo per il x5 (40 => 60)
            defaultMultiplierChanges[4] += 25; //valore minimo per il x6 (50 => 75)
            
            hasChangedMultiplier = true; //l'incremento viene fatto solo una volta
        }

        /* Debug.Log("HandlePointsAndMultiplier"); */

        //Se il player ha l'upgrade X2 Base, allora il moltiplicatore partirà da x2
        if(currentChain < defaultMultiplierChanges[0]){
            if(hasPlayerX2Base){
                scoreManager.SetMultiplier(2 * euphoriaMultiplier);
            } else {
                scoreManager.SetMultiplier(1 * euphoriaMultiplier);
            }
        }

        if(currentChain >= defaultMultiplierChanges[0] && currentChain < defaultMultiplierChanges[1]){
            
            if(hasPlayerX2Base){ //se il giocatore ha il x2 base, il moltiplicatore sarà x3
                scoreManager.SetMultiplier(3 * euphoriaMultiplier);
            } else {
                scoreManager.SetMultiplier(2 * euphoriaMultiplier);
            }
        }

        if(currentChain >= defaultMultiplierChanges[1] && currentChain < defaultMultiplierChanges[2]){

            if(hasPlayerX2Base){
                scoreManager.SetMultiplier(4 * euphoriaMultiplier);
            } else {
                scoreManager.SetMultiplier(3 * euphoriaMultiplier);
            }
            
        }

        if(currentChain >= defaultMultiplierChanges[2] && currentChain < defaultMultiplierChanges[3]){

            //il moltiplicatore può andare a x5 in anticipo solo se il giocatore ha sia il x2 base che il x6 raggiungibile
            if(hasPlayerX2Base && hasPlayerX6){
                scoreManager.SetMultiplier(5 * euphoriaMultiplier);
            }else{
                scoreManager.SetMultiplier(4 * euphoriaMultiplier);
            }

        }

        //il moltiplicatore può andare a x5 o x6 SOLO se ha l'upgrade x6 dedicato
        if(hasPlayerX6 && currentChain >= defaultMultiplierChanges[3] && currentChain < defaultMultiplierChanges[4]){

            //se ha anche il x2 base, allora il giocatore raggiungere il x6 in anticipo
            if(hasPlayerX2Base){
                scoreManager.SetMultiplier(6 * euphoriaMultiplier);
            } else {
                scoreManager.SetMultiplier(5 * euphoriaMultiplier);
            }
        }

        if(hasPlayerX6 && currentChain >= defaultMultiplierChanges[4]){
            scoreManager.SetMultiplier(6 * euphoriaMultiplier);
        }

    }

    public List<CardSO> GetCardToChooseList(){
        return cardToChooseList;
    }

    public int GetChainBuffIndex(){
        return chainBuffIndex;
    }

    public int GetScrollSpeedIndex(){
        return scrollSpeedIncreaseIndex;
    }


    private void OnDestroy(){
        UIManager.Instance.OnGameStart -= StartGame;
        playableDirector.stopped -= VictoryCheck;
    }
}
