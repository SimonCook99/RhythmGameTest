using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class YourCardsPanelUI : MonoBehaviour{

    [SerializeField] private GameObject cardContainer;

    [SerializeField] private GameObject cardTemplate;

    [SerializeField] private Button backToPauseButton;

    private void Awake(){

        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        backToPauseButton.onClick.AddListener(() => {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        });

        cardTemplate.SetActive(false);
    }

    void Start(){
        PausePanelUI.instance.OnShowYourCardsUI += ShowPlayerCards;
    }

    private void ShowPlayerCards(object sender, EventArgs e){

        //reset del cotenuto del pannello, togliendo le carte precedenti prima di creare le nuove
        foreach(Transform child in cardContainer.transform){
            if(child.gameObject == cardTemplate)
                continue;
            Destroy(child.gameObject);
        }

        List<CardSO> playerCards = Player.Instance.GetCardsList();

        foreach(CardSO playerCard in playerCards){

            GameObject newCard = Instantiate(cardTemplate, cardContainer.transform);
            newCard.SetActive(true);


            //se è presente uno sprite per l'upgrade lo imposto nell'angolo della carta, stessa cosa per il downgrade
            if(playerCard.upgrade.sprite != null){

                newCard.transform.GetChild(0).GetChild(0).Find("UpgradeSprite").GetComponent<Image>().sprite = playerCard.upgrade.sprite;
            }
            newCard.transform.GetChild(0).GetChild(0).Find("UpgradeTitle").GetComponent<TextMeshProUGUI>().text = playerCard.upgrade.Name;
            newCard.transform.GetChild(0).GetChild(0).Find("UpgradeDescription").GetComponent<TextMeshProUGUI>().text = playerCard.upgrade.description;
            
            if(playerCard.downgrade.sprite != null){
                newCard.transform.GetChild(0).GetChild(2).Find("DowngradeSprite").GetComponent<Image>().sprite = playerCard.downgrade.sprite;
            }
            newCard.transform.GetChild(0).GetChild(2).Find("DowngradeTitle").GetComponent<TextMeshProUGUI>().text = playerCard.downgrade.Name;
            newCard.transform.GetChild(0).GetChild(2).Find("DowngradeDescription").GetComponent<TextMeshProUGUI>().text = playerCard.downgrade.description;
        }
    }
}