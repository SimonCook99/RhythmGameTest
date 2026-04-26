using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private List<CardSO> playerCardsList; //lista delle carte che il giocatore possiede

    public static Player Instance {get; private set;}

    void Awake(){
        playerCardsList = new List<CardSO>();
        Instance = this;
    }

    void Start(){
        CardManagerUI.Instance.OnPlayerCardsListModified += UpdatePlayerCardsList;

        GameloopManager.Instance.OnRunEnded += GameOver;
    }


    //a run finita, se il player non raggiunge l'obiettivo di punteggio, svuoto la lista di cards in suo possesso
    private void GameOver(object sender, EventArgs e){
        playerCardsList.Clear();
        Debug.Log("Carte del giocatore dopo la sconfitta: " + playerCardsList.Count);
    }

    private void UpdatePlayerCardsList(object sender, CardManagerUI.CardChosenEventArgs e){
        playerCardsList.Add(e.chosenCard);
        Debug.Log("Carte del giocatore: " + playerCardsList.Count);
        Debug.Log("Upgrade carta: " + e.chosenCard.upgrade.Name);
        Debug.Log("Downgrade carta: " + e.chosenCard.downgrade.Name);

        //Una volta aggiunta la carta scelta del giocatore, passo al livello successivo con un nuovo brano (sempre random)
        GameloopManager.Instance.NextLevel(e.chosenCard);
    }


    //funzione pubblica per ricavare la lista di tutti gli upgrade correnti del giocatore
    public List<IEquipable> GetAllUpgradesList(){

        List<IEquipable> allUpgradesList = new List<IEquipable>();

        foreach(CardSO card in playerCardsList){
            allUpgradesList.Add(card.upgrade);
        }

        return allUpgradesList;
    }


    //funzione pubblica per ricavare la lista di tutti i downgrades correnti del giocatore
    public List<IEquipable> GetAllDowngradesList(){

        List<IEquipable> allDownGradesList = new List<IEquipable>();

        foreach(CardSO card in playerCardsList){
            allDownGradesList.Add(card.downgrade);
        }

        return allDownGradesList;
    }

    private void OnDestroy(){
        CardManagerUI.Instance.OnPlayerCardsListModified -= UpdatePlayerCardsList;
    }
}
