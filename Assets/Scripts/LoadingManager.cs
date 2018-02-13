using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LoadingManager : MonoBehaviour
{
    private GameObject[] saves = new GameObject[7];

    private void Start()
    {
        for(int i = 0; i < 7; i++)
        {
            saves[i] = transform.GetChild(i).gameObject;
        }
        LoadSaves();
    }
    
    private void LoadSaves()
    {
        for(int i = 0; i < 7; i++)
        {
            if (File.Exists(Application.persistentDataPath + "/playerInfo" + (i + 1).ToString() + ".dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/playerInfo" + (i + 1).ToString() + ".dat", FileMode.Open);
                PlayerData data = (PlayerData)bf.Deserialize(file);
                file.Close();

                if (SceneManager.GetActiveScene().buildIndex == 0)
                {
                    saves[i].transform.GetChild(0).GetComponent<Text>().text = data.saveName;
                    saves[i].GetComponent<Image>().color = new Color(0.4f, 0.16f, 0.16f);
                    saves[i].transform.GetChild(1).GetComponent<Text>().text = data.levelId.ToString();
                    saves[i].transform.GetChild(2).GetComponent<Text>().text = CountTips(data.tips).ToString();
                }
                else
                {
                    saves[i].GetComponent<InputField>().text = data.saveName;
                    saves[i].transform.GetChild(saves[i].transform.childCount - 1).GetChild(0).GetComponent<Text>().text = "Перезаписать сохранение";
                }
            }
        }
    }
    
    public void LoadSave(int saveId)
    {
        PlayerPrefs.SetInt("loadedLevel", saveId);
        SceneManager.LoadScene(Convert.ToInt32(saves[saveId - 1].transform.GetChild(1).GetComponent<Text>().text));
    }
    
    public void CreateSave()
    {
        Transform clickedButton = EventSystem.current.currentSelectedGameObject.transform;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo" +
            (EventSystem.current.currentSelectedGameObject.transform.parent.GetSiblingIndex() + 1).ToString() +".dat");
        PlayerData data = new PlayerData();
        data.saveName = clickedButton.parent.GetComponent<InputField>().text;
        data.levelId = SceneManager.GetActiveScene().buildIndex;
        data.isDefeated = GameProgressController.isDefeated;
        data.errorMade = GameProgressController.errorMade;
        data.x = clickedButton.parent.parent.parent.parent.GetChild(1).position.x;
        data.x = clickedButton.parent.parent.parent.parent.GetChild(1).position.y;
        data.tips = GameProgressController.tips;
        data.code = GameProgressController.code;
        bf.Serialize(file, data);
        file.Close();
    }

    public int CountTips(bool[] array)
    {
        int sum = 0;
        for(int i = 0; i < array.Length; i++)
        {
            if (array[i])
            {
                sum++;
            }
        }
        return sum;
    }
}
[Serializable]
public struct PlayerData
{
    public string saveName;

    public int levelId;
    public bool isDefeated;
    public bool errorMade;

    public float x;
    public float y;

    public bool[] tips;

    public string code;
}
//Структура в которую записываются данные при загрузке игры и из которой они сохраняются.