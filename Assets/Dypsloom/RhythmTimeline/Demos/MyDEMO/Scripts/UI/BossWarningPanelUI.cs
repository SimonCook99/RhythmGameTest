using UnityEngine;

public class BossWarningPanelUI : MonoBehaviour{
    

    //questa funzione pubblica viene chiamata dalla scheda animation, all'ultimo frame dell'animazione del pannello di warning, tramite un evento
    //l'animation event trova solo evetnauli metodi di script che si trovino nello stesso gameobject da animare, quindi questa funzione si occupa di richiamare quella di UIManager
    public void HideWarningPanel(){
        UIManager.Instance.HideBossWarningPanel();
    }
}
