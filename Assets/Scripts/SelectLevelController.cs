using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SelectLevelController : MonoBehaviour
{
    public int LevelCount;
    
    private void Start()
    {
        if (File.Exists(Application.persistentDataPath + "/levelInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/levelInfo.dat", FileMode.Open);
            LevelProgressData levelData = (LevelProgressData)bf.Deserialize(file);
            file.Close();

            Button[] level = new Button[LevelCount];
            for (int i = 0; i < LevelCount; i++)
            {
                level[i] = transform.GetChild(i + 2).GetComponent<Button>();
                if (i < levelData.PassedCount && levelData.LevelRank[i] != 0)
                {
                    level[i].transform.GetChild(level[i].transform.childCount - 1).gameObject.SetActive(false);
                    
                    level[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite =
                        Resources.Load<Sprite>("GoldLevelName");
                    
                    level[i].transform.GetChild(1).gameObject.SetActive(true);
                    
                    for (int j = 0; j < 3; j++)
                    {
                        Image star = level[i].transform.GetChild(1).GetChild(j).GetComponent<Image>();
                        if (j + 1 > levelData.LevelRank[i])
                        {
                            star.color = Color.black;
                        }
                    }
                }
                else
                {
                    if (i == 0 || (i - 1 < levelData.PassedCount && levelData.LevelRank[i - 1] != 0))
                    {
                        level[i].transform.GetChild(level[i].transform.childCount - 1).gameObject.SetActive(false);
                        
                    }
                    else
                    {
                        level[i].interactable = false;
                        
                        level[i].transform.GetChild(0).gameObject.GetComponent<Button>().interactable = false;
                        
                    }
                }
            }
        }
        else
        {
            Button[] level = new Button[LevelCount];
            for(int i = 1; i < LevelCount; i++)
            {
                level[i] = transform.GetChild(i + 2).GetComponent<Button>();
                level[i].interactable = false;
                level[i].transform.GetChild(0).gameObject.GetComponent<Button>().interactable = false;
            }
        }
    }
    
    public void ChangeClicked()
    {
        Image button = EventSystem.current.currentSelectedGameObject.GetComponent<Image>();

        Sprite AutumnGrass = Resources.Load<Sprite>("AutumnGrassMid");
        Sprite WinterGrass = Resources.Load<Sprite>("WinterGrassMid");
        Sprite SpringGrass = Resources.Load<Sprite>("SpringGrassMid");
        
        if (button.transform.localScale.z == 1)
        {
            button.sprite = AutumnGrass;
            button.transform.localScale = new Vector3(1, 1, 0.9999f);
        }
        else
        {
            if (button.transform.localScale.z == 0.9999f)
            {
                button.sprite = WinterGrass;
                button.transform.localScale = new Vector3(1, 1, 0.9998f);
            }
            else if (button.transform.localScale.z == 0.9998f)
            {
                button.sprite = SpringGrass;
                button.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        Sprite AutumnTree = Resources.Load<Sprite>("AutumnTree");
        Sprite WinterTree = Resources.Load<Sprite>("WinterTree");
        Sprite SpringTree = Resources.Load<Sprite>("SpringTree");
        if (button.transform.localScale.z == 0.95f)
        {
            button.sprite = AutumnTree;
            button.transform.localScale = new Vector3(1, 1, 0.94f);
        }
        else
        {
            if (button.transform.localScale.z == 0.94f)
            {
                button.sprite = WinterTree;
                button.transform.localScale = new Vector3(1, 1, 0.93f);
            }
            else if (button.transform.localScale.z == 0.93f)
            {
                button.sprite = SpringTree;
                button.transform.localScale = new Vector3(1, 1, 0.95f);
            }
        }

        Sprite SpringBush = Resources.Load<Sprite>("SpringBush");
        Sprite AutumnBush = Resources.Load<Sprite>("AutumnBush");
        Sprite WinterBush = Resources.Load<Sprite>("WinterBush");
        if (button.transform.localScale.z == 0.9f)
        {
            button.sprite = AutumnBush;
            button.transform.localScale = new Vector3(1, 1, 0.89f);
        }
        else
        {
            if (button.transform.localScale.z == 0.89f)
            {
                button.sprite = WinterBush;
                button.transform.localScale = new Vector3(1, 1, 0.88f);
            }
            else if (button.transform.localScale.z == 0.88f)
            {
                button.sprite = SpringBush;
                button.transform.localScale = new Vector3(1, 1, 0.9f);
            }
        }

        if (button.transform.childCount != 0)
        {
            Destroy(button.transform.GetChild(0).gameObject);
        }
    }
    
    public void SelectLevel(int LevelID)
    {
        PlayerPrefs.SetInt("loadedLevel", 0);
        SceneManager.LoadScene(LevelID);
    }
    
}
/*///////////////////////
    /////▄▀▀▀▄///////////////
    ▄███▀/◐///▌/////////////
    ////▌/////▐//////////////
    ///▐/////▐///////////////
    ////▌/////▐▄▄////////////
    ////▌////▄▀//▀▀▀▀▄///////
    ///▐////▐////////▀▀▄/////
    ///▐////▐▄//////////▀▄///
    ////▀▄////▀▄//////////▀▄
    /////▀▄▄▄▄▄█▄▄▄▄▄▄▄▄▄▄▄▀▄
    /////////////▌▌/▌▌///////
    /////////////▌▌/▌▌///////
    ///////////▄▄▌▌▄▌▌/////*/
[Serializable]
public struct LevelProgressData
{
    public int PassedCount;
    public int[] LevelRank;
}