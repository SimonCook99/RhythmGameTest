using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;



public class DifficultySettingsButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{

    [SerializeField] private TextMeshProUGUI difficultyDescriptionText;


    [SerializeField] private List<GameObject> selectedBordersList; //lista dei bordi selezionati degli altri pulsanti, da disabilitare quando sarà cliccato il proprio

    [SerializeField] private GameObject selectedBorder; //il proprio bordo selezionato, che sarà attivo al click del bottone


    

    private void Awake(){
        difficultyDescriptionText.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData){
        difficultyDescriptionText.gameObject.SetActive(true);
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.05f, 1.05f, 1.05f);
    }

    public void OnPointerExit(PointerEventData eventData){
        difficultyDescriptionText.gameObject.SetActive(false);
        gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
    }


    public void selectedDifficultyVisual(){
        selectedBorder.SetActive(true);

        foreach(GameObject border in selectedBordersList){
            border.SetActive(false); //disattivo visivamente i bordi degli altri tasti
        }
        
    }
}
