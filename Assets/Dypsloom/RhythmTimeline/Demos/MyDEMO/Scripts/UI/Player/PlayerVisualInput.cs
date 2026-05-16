using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerVisualInput : MonoBehaviour{

    [Tooltip("Gli oggetti in scena che hanno lo sprite da mostrare")]
    [SerializeField] private GameObject leftGameobjectInput;
    [SerializeField] private GameObject downGameobjectInput;
    [SerializeField] private GameObject upGameobjectInput;
    [SerializeField] private GameObject rightGameobjectInput;


    [Tooltip("Sprite input di default")]
    [SerializeField] private Sprite leftDefaultSprite;
    [SerializeField] private Sprite downDefaultSprite;
    [SerializeField] private Sprite upDefaultSprite;
    [SerializeField] private Sprite rightDefaultSprite;

    [Tooltip("Sprite input per quando vengono premuti")]
    [SerializeField] private Sprite leftPressedSprite;
    [SerializeField] private Sprite downPressedSprite;
    [SerializeField] private Sprite upPressedSprite;
    [SerializeField] private Sprite rightPressedSprite;



    private void Update(){
        if(GameloopManager.Instance.GetState() == GameloopManager.State.Playing){
            CheckIfPressedInput();
        }
    }


    private void CheckIfPressedInput(){
        if(Keyboard.current.dKey.wasPressedThisFrame){
            leftGameobjectInput.GetComponent<SpriteRenderer>().sprite = leftPressedSprite;
            leftGameobjectInput.transform.localScale += new Vector3(1f,1f,1f);
        }

        if(Keyboard.current.dKey.wasReleasedThisFrame){
            leftGameobjectInput.GetComponent<SpriteRenderer>().sprite = leftDefaultSprite;
            leftGameobjectInput.transform.localScale -= new Vector3(1f,1f,1f);
        }

        if(Keyboard.current.fKey.wasPressedThisFrame){
            downGameobjectInput.GetComponent<SpriteRenderer>().sprite = downPressedSprite;
            downGameobjectInput.transform.localScale += new Vector3(1f,1f,1f);
        }

        if(Keyboard.current.fKey.wasReleasedThisFrame){
            downGameobjectInput.GetComponent<SpriteRenderer>().sprite = downDefaultSprite;
            downGameobjectInput.transform.localScale -= new Vector3(1f,1f,1f);
        }

        if(Keyboard.current.jKey.wasPressedThisFrame){
            upGameobjectInput.GetComponent<SpriteRenderer>().sprite = upPressedSprite;
            upGameobjectInput.transform.localScale += new Vector3(1f,1f,1f);
        }

        if(Keyboard.current.jKey.wasReleasedThisFrame){
            upGameobjectInput.GetComponent<SpriteRenderer>().sprite = upDefaultSprite;
            upGameobjectInput.transform.localScale -= new Vector3(1f,1f,1f);
        }

        if(Keyboard.current.kKey.wasPressedThisFrame){
            rightGameobjectInput.GetComponent<SpriteRenderer>().sprite = rightPressedSprite;
            rightGameobjectInput.transform.localScale += new Vector3(1f,1f,1f);
        }

        if(Keyboard.current.kKey.wasReleasedThisFrame){
            rightGameobjectInput.GetComponent<SpriteRenderer>().sprite = rightDefaultSprite;
            rightGameobjectInput.transform.localScale -= new Vector3(1f,1f,1f);
        }
    }
}
