using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    private GameObject AboutGamePanel;
    private GameObject HelpPanel;
    private GameObject SelectLevelPanel;
    private GameObject SettingsPanel; 
    private GameObject LoadLevelPanel;

    private void Awake()
    {
        LoadLevelPanel = transform.GetChild(transform.childCount - 4).gameObject;
        SettingsPanel = transform.GetChild(transform.childCount - 3).gameObject;
        AboutGamePanel = transform.GetChild(transform.childCount - 2).gameObject;
        HelpPanel = GameProgressController.GetLastChild(transform).gameObject;
        SelectLevelPanel = GameProgressController.GetLastChild(transform.parent).gameObject;

        DisablePanels();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (AboutGamePanel.activeInHierarchy)
            {
                AboutGamePanel.SetActive(false);
            }
            if (HelpPanel.activeInHierarchy)
            {
                HelpPanel.SetActive(false);
            }
            if (SelectLevelPanel.activeInHierarchy)
            {
                SelectLevelPanel.SetActive(false);
            }
            if (SettingsPanel.activeInHierarchy)
            {
                SettingsPanel.SetActive(false);
            }
        }
    }
    
    /*
    ───────────────▄████████▄─ 
    ──────────────██▒▒▒▒▒▒▒▒██── 
    ─────────────██▒▒▒▒▒▒▒▒▒█ 
    ────────────██▒▒▒▒▒▒▒▒▒▒██─ 
    ───────────██▒▒▒▒▒▒▒▒▒██▀── 
    ──────────██▒▒▒▒▒▒▒▒▒▒██─── 
    ─────────██▒▒▒▒▒▒▒▒▒▒▒██──── 
    ────────██▒████▒████▒▒██── 
    ────────██▒▒▒▒▒▒▒▒▒▒▒▒██──── 
    ────────██▒────▒▒────▒██── 
    ────────██▒██──▒▒██──▒██── 
    ────────██▒────▒▒────▒██── 
    ────────██▒▒▒▒▒▒▒▒▒▒▒▒██──── 
    ───────██▒▒▒████████▒▒▒▒██─ 
    ─────██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██── 
    ───██▒▒██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██▒▒██ 
    ─██▒▒▒▒██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██▒▒▒▒█ 
    █▒▒▒▒██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██▒▒▒▒ 
    █▒▒▒▒██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██▒▒▒▒▒ 
    █▒▒████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒████▒ 
    ▀████▒▒▒▒
    */
    public void DestroyClicked()
    {
        GameObject Button = EventSystem.current.currentSelectedGameObject;
        Destroy(Button);
        if (Button.transform.childCount != 0)
        {
            EventSystem.current.currentSelectedGameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    
    public void OnClickHelp()
    {
        if (!HelpPanel.activeInHierarchy)
        {
            DisablePanels();
        }
        HelpPanel.SetActive(!HelpPanel.activeInHierarchy);
    }
    
    public void OnClickAboutGame()
    {   if (!AboutGamePanel.activeInHierarchy)
        {
            DisablePanels();
        }
        AboutGamePanel.SetActive(!AboutGamePanel.activeInHierarchy);
    }
    
    public void OnClickSwitchLevelMenu()
    {
        if (!SelectLevelPanel.activeInHierarchy)
        {
            DisablePanels();
        }
        SelectLevelPanel.SetActive(!SelectLevelPanel.activeInHierarchy);
    }
    
    public void OnClickLoadGame()
    {
        if (!LoadLevelPanel.activeInHierarchy)
        {
            DisablePanels();
        }

        LoadLevelPanel.SetActive(!LoadLevelPanel.activeInHierarchy);
    }
    
    public void OnClickSettings()
    {
        DisablePanels();
        SettingsPanel.SetActive(true);
    }
    
    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
    }
    
    private void DisablePanels()
    {
        LoadLevelPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        AboutGamePanel.SetActive(false);
        HelpPanel.SetActive(false);
    }
    
}