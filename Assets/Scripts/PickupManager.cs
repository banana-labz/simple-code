using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PickupManager : MonoBehaviour
{
    
    public int tipIndex;
    public string[] tipText;
    public string[] copyCode;

    private void Start()
    {
        GameProgressController.tipText[tipIndex] = GatherStrings(tipText);
        GameProgressController.CopyCode[tipIndex] = GatherStrings(copyCode);
        if (GameProgressController.tips[tipIndex] == true)
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (transform.parent.childCount == 4 && SceneManager.GetActiveScene().buildIndex == 1)
            {
                GameProgressController ui = transform.parent.parent.GetChild(0).GetComponent<GameProgressController>();
                ui.AdviceUI.SetActive(true);
                ui.AdviceText.text = "Для того щоб переглянути підказку натисніть T";
                Invoke("CloseAdvice", 7f);
            }
            GameProgressController.tips[tipIndex] = true;
            Destroy(gameObject);
        }
    }

    public string GatherStrings(string[] s)
    {
        string result = "";
        for(int i = 0; i < s.Length; i++)
        {
            result += (s[i] + Environment.NewLine);
        }
        return result;
    }
    private void CloseAdvice()
    {
        GameProgressController ui = GameProgressController.GetLastChild(transform.parent.parent).GetComponent<GameProgressController>();
        ui.AdviceUI.SetActive(false);
    }
}
