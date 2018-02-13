using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class SettingsController : MonoBehaviour
{
    private Dropdown ResolutionDropDown;
    private Dropdown QualityDropdown;

    private Resolution[] Resolutions;
    private Dictionary<string, KeyCode> Controls = new Dictionary<string, KeyCode>();

    private Text right, left, jump, code, tips, interact;

    private GameObject currentKey;

    private Color32 selectedColor = new Color(255, 180, 255);

    private void Awake()
    {
        if (!File.Exists(Application.persistentDataPath + "/settingsInfo.dat"))
        {
            LoadDefaultControlSettings();
        }
        else
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/settingsInfo.dat", FileMode.Open);
            SettingsData data = (SettingsData)bf.Deserialize(file);
            file.Close();
            Controls.Add("Right", data.right);
            Controls.Add("Left", data.left);
            Controls.Add("Jump", data.jump);
            Controls.Add("Code", data.code);
            Controls.Add("Tips", data.tips);
            Controls.Add("Interact", data.interact);

            right = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>();
            right.text = Controls["Right"].ToString();
            left = transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<Text>();
            left.text = Controls["Left"].ToString();
            jump = transform.GetChild(2).GetChild(2).GetChild(0).GetComponent<Text>();
            jump.text = Controls["Jump"].ToString();
            code = transform.GetChild(2).GetChild(3).GetChild(0).GetComponent<Text>();
            code.text = Controls["Code"].ToString();
            tips = transform.GetChild(2).GetChild(4).GetChild(0).GetComponent<Text>();
            tips.text = Controls["Tips"].ToString();
            interact = transform.GetChild(2).GetChild(5).GetChild(0).GetComponent<Text>();
            interact.text = Controls["Interact"].ToString();
        }
    }
    
    private void Start()
    {
        ResolutionDropDown = transform.GetChild(0).GetComponent<Dropdown>();
        QualityDropdown = transform.GetChild(1).GetComponent<Dropdown>();

        Resolutions = Screen.resolutions;
        ResolutionDropDown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = Resolutions.Length - 1; i >= 0; i--)
        {
            string option = Resolutions[i].width + " x " + Resolutions[i].height;
            options.Add(option);

            if (Resolutions[i].width == Screen.width
                && Resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = Resolutions.Length - i - 1;
            }
        }

        ResolutionDropDown.AddOptions(options);
        ResolutionDropDown.value = currentResolutionIndex;
        ResolutionDropDown.RefreshShownValue();

        options = new List<string>();
        QualityDropdown.ClearOptions();
        options = QualitySettings.names.ToList();
        options.Reverse();
        int currentQualityIndex = options.Count() - 1 - QualitySettings.GetQualityLevel();

        QualityDropdown.AddOptions(options);
        QualityDropdown.value = currentQualityIndex;
        QualityDropdown.RefreshShownValue();
        UpdateControlSettings();
    }
    
    private void OnGUI()
    {
        if (currentKey != null)
        {
            Event e = Event.current;
            if (e.isKey && e.keyCode != KeyCode.Escape)
            {
                Controls[currentKey.name] = e.keyCode;
                currentKey.transform.GetChild(0).GetComponent<Text>().text = e.keyCode.ToString();
                currentKey.transform.GetComponent<Image>().color = Color.white;
                currentKey = null;
            }
        }
    }
    
    public void ChangeKey()
    {
        if (currentKey != null)
        {
            currentKey.GetComponent<Image>().color = Color.white;
        }
        currentKey = EventSystem.current.currentSelectedGameObject;
        currentKey.GetComponent<Image>().color = selectedColor;
    }
    
    public void SelectQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    
    public void SelectResolution(int resolutionIndex)
    {
        Resolution resolution = Resolutions[Resolutions.Length - resolutionIndex - 1];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    
    private void SaveSettings()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/settingsInfo.dat");
        SettingsData data = new SettingsData();
        data.right = Controls["Right"];
        data.left = Controls["Left"];
        data.jump = Controls["Jump"];
        data.code = Controls["Code"];
        data.tips = Controls["Tips"];
        data.interact = Controls["Interact"];
        bf.Serialize(file, data);
        file.Close();
    }
    
    public void OnClickExitSettingsMenu()
    {
        SaveSettings();
        PlayerInput.Controls = Controls;
        gameObject.SetActive(false);
    }
    
    public void OnClickSetupDefaults()
    {
        Controls.Clear();
        LoadDefaultControlSettings();
        UpdateControlSettings();
    }
    
    private void LoadDefaultControlSettings()
    {
        Controls.Add("Right", KeyCode.D);
        Controls.Add("Left", KeyCode.A);
        Controls.Add("Jump", KeyCode.Space);
        Controls.Add("Code", KeyCode.C);
        Controls.Add("Tips", KeyCode.T);
        Controls.Add("Interact", KeyCode.E);
    }
    
    private void UpdateControlSettings()
    {
        right = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>();
        right.text = Controls["Right"].ToString();
        left = transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<Text>();
        left.text = Controls["Left"].ToString();
        jump = transform.GetChild(2).GetChild(2).GetChild(0).GetComponent<Text>();
        jump.text = Controls["Jump"].ToString();
        code = transform.GetChild(2).GetChild(3).GetChild(0).GetComponent<Text>();
        code.text = Controls["Code"].ToString();
        tips = transform.GetChild(2).GetChild(4).GetChild(0).GetComponent<Text>();
        tips.text = Controls["Tips"].ToString();
        interact = transform.GetChild(2).GetChild(5).GetChild(0).GetComponent<Text>();
        interact.text = Controls["Interact"].ToString();
    }
    
}

[Serializable]
public struct SettingsData
{
    public KeyCode right;
    public KeyCode left;
    public KeyCode jump;
    public KeyCode code;
    public KeyCode tips;
    public KeyCode interact;
}
