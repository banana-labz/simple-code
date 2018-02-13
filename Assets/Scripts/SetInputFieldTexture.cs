using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SetInputFieldTexture : UIBehaviour
{

    public InputField inputField = null;

    IEnumerator Start()
    {
        yield return null;
        if (inputField == null)
            inputField = GetComponent<InputField>();

        if (inputField != null)
        {
            
            Transform caretGO = inputField.transform.GetChild(0);
            caretGO.GetComponent<CanvasRenderer>().SetMaterial(Graphic.defaultGraphicMaterial, Texture2D.whiteTexture);
        }
    }
}