using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardManagerUI : MonoBehaviour{

    //evento che scatta ogni volta che viene aggiunta, o più in generale modificata la lista delle carte del player
    public class CardChosenEventArgs : EventArgs{
        public CardSO chosenCard;
    }
    public event EventHandler<CardChosenEventArgs> OnPlayerCardsListModified;


    public GameObject cardContainer; //il contenitore dove al suo interno inseriamo le carte, con il layout group

    public GameObject cardTemplate; //il temaplate della carta da istanziare e impostare


    public static CardManagerUI Instance {get; private set;}

    public event EventHandler OnCardClickSound;

    void Awake(){
        cardTemplate.SetActive(false);
        
        Instance = this;
    }


    public void ShowCards(){

        //reset del cotenuto del pannello, togliendo le carte precedenti prima di creare le nuove
        foreach(Transform child in cardContainer.transform){
            if(child.gameObject == cardTemplate)
                continue;
            Destroy(child.gameObject);
        }


        List<CardSO> cardsToChoose = GameloopManager.Instance.GetCardToChooseList();

        foreach(CardSO card in cardsToChoose){

            GameObject newCard = Instantiate(cardTemplate, cardContainer.transform);
            newCard.SetActive(true);


            //se è presente uno sprite per l'upgrade lo imposto nell'angolo della carta, stessa cosa per il downgrade
            if(card.upgrade.sprite != null){

                newCard.transform.GetChild(0).Find("UpgradeSprite").GetComponent<Image>().sprite = card.upgrade.sprite;
            }
            newCard.transform.GetChild(0).Find("UpgradeTitle").GetComponent<TextMeshProUGUI>().text = card.upgrade.Name;
            newCard.transform.GetChild(0).Find("UpgradeDescription").GetComponent<TextMeshProUGUI>().text = card.upgrade.description;
            
            if(card.downgrade.sprite != null){
                newCard.transform.GetChild(2).Find("DowngradeSprite").GetComponent<Image>().sprite = card.downgrade.sprite;
            }
            newCard.transform.GetChild(2).Find("DowngradeTitle").GetComponent<TextMeshProUGUI>().text = card.downgrade.Name;
            newCard.transform.GetChild(2).Find("DowngradeDescription").GetComponent<TextMeshProUGUI>().text = card.downgrade.description;


            //aggiungo il listener al bottone, che se premuto fa scattare l'evento, passando la carta come parametro
            newCard.GetComponent<Button>().onClick.AddListener(() => {

                UIManager.Instance.GetCardChoosingPanel().SetActive(false);

                OnCardClickSound?.Invoke(this, EventArgs.Empty);

                OnPlayerCardsListModified?.Invoke(this, new CardChosenEventArgs{
                    chosenCard = card
                });
            });

        }
    }
    
}
