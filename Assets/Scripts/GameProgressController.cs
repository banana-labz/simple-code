using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

using System;
using System.IO;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;

public class GameProgressController : MonoBehaviour
{

    public int tipsAmount;

    public string[] inputText;
    public int inputSize;

    public string[] outputText;
    public int outputSize;

    public string[] advice;

    private string CompilerErrors;

    public static string[] tipText;
    public static string[] CopyCode;

    public static bool[] tips;
    public static bool isDefeated;
    public static bool errorMade;
    public static bool isPaused;
    public static string code;

    private bool taskReviewed;

    public static bool ableToSubmit;

    public static bool ableToBattle;
    private float timeRemaining;
    private Text timer;


    public string[] questions;
    public string[] answers;
    public int[] corrects;
    private int TestNum;

    private GameObject PauseMenuUI;
    private GameObject TaskUI;
    private GameObject CodeUI;
    private GameObject DeathUI;
    private GameObject WinUI;
    private GameObject TipsUI;
    private GameObject SubmitButton;
    private GameObject ErrorUI;
    private GameObject BattleUI;
    public GameObject AdviceUI;
    private GameObject SettingsUI;
    private GameObject SaveUI;
    private GameObject PauseButton;
    private GameObject WaitForCompilation;
    public Text AdviceText;
    private InputField CodeInput;
    private Text ErrorText;

    private Process[] parallel;

    private void Awake()
    {
        ableToBattle = false;
        GetLastChild(transform.parent).GetComponent<PlayerInput>().enabled = true;
        tipText = new string[tipsAmount];
        CopyCode = new string[tipsAmount];
        InitializeComponents();
        if (PlayerPrefs.GetInt("loadedLevel") == 0)
        {
            UpdateProgress();
            TaskUI.SetActive(true);
            isPaused = false;
            SwitchPause();
        }
        else
        {
            GetLastChild(transform.parent).GetComponent<PlayerInput>().enabled = true;
            TaskUI.SetActive(false);
            isPaused = false;
            PauseButton.SetActive(true);
            Destroy(AdviceUI);
        }
        DisablePanels();
        if (!Directory.Exists(Application.persistentDataPath + "TestFiles" + SceneManager.GetActiveScene().buildIndex.ToString()))
        {
            CreateTestFiles();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !DeathUI.activeInHierarchy
           && !WinUI.activeInHierarchy && !BattleUI.activeInHierarchy)
        {
            if (CodeUI.activeInHierarchy)
            {
                CloseCode();
                return;
            }
            if (TaskUI.activeInHierarchy)
            {
                CloseTask();
                return;
            }
            if (ErrorUI.activeInHierarchy)
            {
                ErrorUI.SetActive(false);
                return;
            }
            if (TipsUI.activeInHierarchy)
            {
                CloseTips();
                return;
            }
            if (SettingsUI.activeInHierarchy)
            {
                CloseSettings();
                return;
            }
            if (SaveUI.activeInHierarchy)
            {
                CloseSaves();
                 return;
            }
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
                CloseAdviceUI();
            }
        }
        if (Input.GetKeyDown(PlayerInput.Controls["Code"]) && !DeathUI.activeInHierarchy && !SaveUI.activeInHierarchy
           && !WinUI.activeInHierarchy && !BattleUI.activeInHierarchy && !TipsUI.activeInHierarchy && !SettingsUI.activeInHierarchy
           && !TaskUI.activeInHierarchy && !ErrorUI.activeInHierarchy && !CodeUI.activeInHierarchy )
        {
            CodeUI.SetActive(true);
            ChangeCode(false);
            SubmitButton.SetActive(false);
            if (!PauseMenuUI.activeInHierarchy)
            {
                SwitchPause();
            }
            CloseAdviceUI();
            CodeInput.text = code;
        }
        if (Input.GetKeyDown(PlayerInput.Controls["Tips"]) && !DeathUI.activeInHierarchy && !SaveUI.activeInHierarchy
           && !WinUI.activeInHierarchy && !BattleUI.activeInHierarchy && !TipsUI.activeInHierarchy && !SettingsUI.activeInHierarchy
           && !TaskUI.activeInHierarchy && !ErrorUI.activeInHierarchy && !CodeUI.activeInHierarchy)
        {
            TipsUI.SetActive(true);
            LoadTips();
            if (!PauseMenuUI.activeInHierarchy)
            {
                SwitchPause();
            }
            CloseAdviceUI();
        }
        if (Input.GetKeyDown(PlayerInput.Controls["Interact"]) && !DeathUI.activeInHierarchy
           && !WinUI.activeInHierarchy && !BattleUI.activeInHierarchy && !TipsUI.activeInHierarchy
           && !TaskUI.activeInHierarchy && !ErrorUI.activeInHierarchy && !SaveUI.activeInHierarchy
           && !SettingsUI.activeInHierarchy && !CodeUI.activeInHierarchy)
        {
            if (ableToBattle)
            {
                BattleUI.SetActive(true);
                SwitchPause();
                CloseAdviceUI();
                StartBattle();
            }
            if (ableToSubmit)
            {
                CodeUI.SetActive(true);
                ChangeCode(true);
                SwitchPause();
                CloseAdviceUI();
                CodeInput.text = code;
                SubmitButton.SetActive(true);
            }
        }
    }

    private void FixedUpdate()
    {
        if (BattleUI.activeInHierarchy)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                isDefeated = true;
                BattleUI.SetActive(false);
            }
            timer.text = Mathf.Round(timeRemaining).ToString();
        }
    }

    public void SubmitCode()
    {
        WaitForCompilation.SetActive(true);
        Invoke("CompileCode", 0.5f);
    }

    private void CompileCode()
    {
        string levelId = SceneManager.GetActiveScene().buildIndex.ToString();
        code = CodeInput.text;
        for (int i = 0; i < 5; i++)
        {
            File.Delete(Application.dataPath + "/myprogram" + i.ToString() + ".exe");
            if (File.Exists(Application.persistentDataPath + "/TestFiles" + levelId + "/output" + i.ToString() + ".txt"))
            {
                File.Delete(Application.persistentDataPath + "/TestFiles"
                    + levelId + "/output" + i.ToString() + ".txt");
            }
            string source =
                "using System;using System.IO;using System.Collections.Generic;using System.Linq;" +
                "using System.Text; public class Program{public static void Main(string[] args){" +
                "StreamReader str = new StreamReader(\"" + Application.persistentDataPath + "/InputFiles" + levelId + "/input" + i.ToString() + ".txt\");" +
                "StreamWriter wrt = new StreamWriter(\"" + Application.persistentDataPath + "/TestFiles" + levelId + "/output" + i.ToString() + ".txt\");"+
                code +
                "str.Close();wrt.Close();}}";
            
            CompilerParameters compilerParams = new CompilerParameters
            { OutputAssembly = Application.dataPath + "/myprogram" + i.ToString() + ".exe", GenerateExecutable = true };
            
            CSharpCodeCompiler codeCompiler = new CSharpCodeCompiler();
            codeCompiler.CompileAssemblyFromSource(compilerParams, source);
            CompilerResults results = codeCompiler.CompileAssemblyFromSource(compilerParams, source);
            
            
            if (results.Errors.HasErrors)
            {
                int j = 0;
                foreach (CompilerError err in results.Errors)
                {
                    CompilerErrors += (j + 1).ToString() + ": " + err.ErrorText + Environment.NewLine;
                    j++;
                }
                ShowErrorUI(true);
                return;
            }
        }
        Test();
    }

    private void LoadTips()
    {
        for(int i = 0; i < tips.Length; i++)
        {
            GameObject text = TipsUI.transform.GetChild(i).gameObject;
            if(tips[i])
            {
                text.transform.GetChild(0).GetComponent<Text>().text = tipText[i];
                if (text.transform.childCount > 1)
                {
                    text.transform.GetChild(1).GetComponent<InputField>().text = CopyCode[i];
                }
            }
            text.SetActive(tips[i]);
        }
    }
    
    public void Resume()
    {
        DisablePanels();
        SwitchPause();
    }
    
    public void Restart()
    {
        PlayerPrefs.SetInt("loadedLevel", 0);
        UpdateProgress();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void OpenWinUI()
    {
        DisablePanels();
        WinUI.SetActive(true);
        if (!isPaused)
        {
            SwitchPause();
        }
        WaitForCompilation.SetActive(false);
        WinUI.SetActive(true);
        LoadStars();
        SaveWinData();
    }

    void LoadStars()
    {
        if (errorMade)
        {
            WinUI.transform.GetChild(2).GetComponent<Image>().color = Color.black;
        }
        if (isDefeated)
        {
            WinUI.transform.GetChild(1).GetComponent<Image>().color = Color.black;
        }
    }

    void SaveWinData ()
    {
        int levelRank = 1;
        int levelId = SceneManager.GetActiveScene().buildIndex;
        LevelProgressData data = new LevelProgressData();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        if (!errorMade)
        {
            levelRank++;
        }
        if (!isDefeated)
        {
            levelRank++;
        }
        string path = Application.persistentDataPath + "/levelInfo.dat";
        if (File.Exists(path))
        {
            file = File.Open(path, FileMode.Open);
            data = (LevelProgressData)bf.Deserialize(file);
            file.Close();
            if (data.PassedCount < levelId)
            {
                data.PassedCount = levelId;
            }
            if (data.LevelRank[levelId] < levelRank)
            {
                data.LevelRank[levelId] = levelRank;
            }
        }
        else
        {
            data.PassedCount = 1;
            data.LevelRank = new int[data.PassedCount];
            data.LevelRank[0] = levelRank;
        }
        file = File.Create(path);
        bf.Serialize(file, data);
        file.Close();
    }

    private void StartBattle()
    {
        timeRemaining = 60f;
        System.Random random = new System.Random();
        TestNum = random.Next(0, questions.Length);
        Text quest = BattleUI.transform.GetChild(BattleUI.transform.childCount - 2).GetComponent<Text>();
        Text[] ans = new Text[4];
        for(int i = 0; i < 4; i++)
        {
            ans[i] = BattleUI.transform.GetChild(i).GetChild(0).GetComponent<Text>();
        }
        quest.text = questions[TestNum];
        ans[0].text = answers[TestNum*4];
        ans[1].text = answers[TestNum*4 + 1];
        ans[2].text = answers[TestNum*4 + 2];
        ans[3].text = answers[TestNum*4 + 3];
    }

    public void SubmitAnswer()
    {
        int k = EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex();
        if (k == corrects[TestNum])
        {
            BattleUI.SetActive(false);
            SwitchPause();
            Destroy(transform.parent.GetChild(3).GetChild(0).gameObject);
            transform.parent.GetChild(4).GetComponent<EnemyController>().AdditionalAdvice.SetActive(false);
            transform.parent.GetChild(4).GetComponent<EnemyController>().enabled = false;
            ableToBattle = false;
        }
        else
        {
            isDefeated = true;
            if (SceneManager.GetActiveScene().buildIndex != 1)
            {
                transform.parent.GetChild(4).GetComponent<EnemyController>().enabled = false;
            }
            SwitchPause();
            BattleUI.SetActive(false);
        }
    }

    public void OpenSaveUI()
    {
        SettingsUI.SetActive(false);
        SaveUI.SetActive(!SaveUI.activeInHierarchy);
    }
    
    public void OpenSettingsUI()
    {
        SaveUI.SetActive(false);
        SettingsUI.SetActive(!SettingsUI.activeInHierarchy);
    }
    
    public void OpenTaskUI()
    {
        SettingsUI.SetActive(false);
        SaveUI.SetActive(false);
        TaskUI.SetActive(true);
    }
    
    public void OpenDeathUI()
    {
        DeathUI.SetActive(true);
        GetLastChild(transform.parent).gameObject.SetActive(false);
        CloseAdviceUI();
    }
    
    private void ShowErrorUI(bool errors)
    {
        if (errors)
        {
            if (!isPaused)
            {
                SwitchPause();
            }
            ErrorUI.SetActive(true);
            ErrorText.text = CompilerErrors;
            errorMade = true;
        }
        else
        {
            if (!isPaused)
            {
                SwitchPause();
            }
            ErrorUI.SetActive(true);
            ErrorText.text = "На жаль програма не вирішила поставлену задачу.";
            errorMade = true;
        }
        WaitForCompilation.SetActive(false);
    }

    private void Test()
    {
        parallel = new Process[5];
        for(int i = 0; i < 5; i++)
        {
            parallel[i] = Process.Start(Application.dataPath + "/myprogram" + i.ToString() + ".exe");
            
            CompilerErrors = "";
        }
        parallel[4].WaitForExit(60);
        Invoke("CheckOutput",2f);
    }

    private void CheckOutput()
    {
        string levelId = SceneManager.GetActiveScene().buildIndex.ToString();
        int correct = 0;
        for (int i = 0; i < 5; i++)
        {
            if (!File.Exists(Application.persistentDataPath
                + "/TestFiles" + levelId + "/output" + i.ToString() + ".txt"))
            {
                Kill();
                ShowErrorUI(false);
                return;
            }
            StreamReader playerOutput = new StreamReader(Application.persistentDataPath
                + "/TestFiles" + levelId + "/output" + i.ToString() + ".txt");
            StreamReader realOutput = new StreamReader(Application.persistentDataPath
                + "/RealFiles" + levelId + "/output" + i.ToString() + ".txt");
            string test, real;
            test = playerOutput.ReadToEnd() + Environment.NewLine;
            real = realOutput.ReadToEnd();
            realOutput.Close();
            playerOutput.Close();
            if (test == real)
            {
                correct++;
            }
        }
        Kill();
        if (correct == 5)
        {
            OpenWinUI();
        }
        else
        {
            ShowErrorUI(false);
        }
    }

    public void ChangeCode(bool isFinish)
    {
        if (isFinish)
        {
            CodeUI.GetComponent<Image>().color = Color.white;
            CodeUI.GetComponent<Image>().sprite = Resources.Load<Sprite>("FinishCodePanel");
            CodeUI.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0);
            CodeInput.caretColor = Color.green;
            GetLastChild(CodeInput.transform).GetComponent<Text>().color = Color.green;
            CodeInput.transform.GetChild(1).GetComponent<Text>().color = Color.green;
        }
        else
        {
            CodeUI.GetComponent<Image>().color = new Color(0.0078125f, 0.44921875f, 0.17578125f, 1);
            CodeUI.GetComponent<Image>().sprite = Resources.Load<Sprite>("CodePanel");
            CodeUI.transform.GetChild(0).GetComponent<Image>().color = Color.white;
            CodeInput.caretColor = Color.black;
            GetLastChild(CodeInput.transform).GetComponent<Text>().color = Color.black;
            CodeInput.transform.GetChild(1).GetComponent<Text>().color = Color.black;
        }
    }
    
    public void GoToMain()
    {
        PlayerPrefs.SetInt("loadedLevel", 0);
        SceneManager.LoadScene(0);
    }
    
    public void Pause()
    {
         if (!Input.GetKey(KeyCode.Space))
         {
            PauseMenuUI.SetActive(true);
            SwitchPause();
         }
    }
    
    public void CloseCode()
    {
        code = CodeInput.text;
        CodeUI.SetActive(false);

        if (!PauseMenuUI.activeInHierarchy)
        {
            SwitchPause();
        }
    }
    
    public void CloseTask()
    {
        TaskUI.SetActive(false);
        if(!taskReviewed && SceneManager.GetActiveScene().buildIndex == 1)
        {
            AdviceUI.SetActive(true);
            AdviceText.text = advice[0];
            Invoke("CloseAdviceUI", 6f);
            taskReviewed = true;
        }
        if (!PauseMenuUI.activeInHierarchy)
        {
            SwitchPause();
        }
    }
    
    public void CloseTips()
    {
        TipsUI.SetActive(false);
        if (!PauseMenuUI.activeInHierarchy)
        {
            SwitchPause();
        }
    }
    
    public void CloseSettings()
    {
        SettingsUI.SetActive(false);
        if (!PauseMenuUI.activeInHierarchy)
        {
            SwitchPause();
        }
    }
    
    public void CloseSaves()
    {
        SaveUI.SetActive(false);
        if (!PauseMenuUI.activeInHierarchy)
        {
            SwitchPause();
        }
    }

    public void CloseErrors()
    {
        ErrorUI.SetActive(false);
        if (!CodeUI.activeInHierarchy)
        {
            SwitchPause();
        }
    }
    
    public void SwitchPause()
    {
        Transform player = GetLastChild(transform.parent);
        float HorizontalVelocity = player.GetComponent<Player>().velocity.x;
        float VerticalVelocity = player.GetComponent<Player>().velocity.y;
        if (!isPaused)
        {
            player.GetComponent<PlayerInput>().enabled = false;
            player.GetComponent<Player>().enabled = false;
            player.GetComponent<Controller2D>().enabled = false;
            player.GetComponent<Animator>().speed = 0f;
            isPaused = true;
            PauseButton.SetActive(false);
        }
        else
        {
            player.GetComponent<Controller2D>().enabled = true;
            player.GetComponent<Player>().enabled = true;
            player.GetComponent<Player>().velocity = new Vector2(HorizontalVelocity, VerticalVelocity);
            player.GetComponent<PlayerInput>().enabled = true;
            player.GetComponent<Animator>().speed = 1f;
            isPaused = false;
            PauseButton.SetActive(true);
        }

    }
    
    public void UpdateProgress()
    {
        code = "";
        tips = new bool[tipsAmount];
        isDefeated = false;
        errorMade = false;
        ableToBattle = false;
        ableToSubmit = false;
        CompilerErrors = "";
        timeRemaining = 60;
    }
    
    private void InitializeComponents()
    {
        PauseMenuUI = transform.GetChild(0).gameObject;
        TaskUI = transform.GetChild(1).gameObject;
        CodeUI = transform.GetChild(2).gameObject;
        DeathUI = transform.GetChild(3).gameObject;
        ErrorUI = transform.GetChild(4).gameObject;
        WinUI = transform.GetChild(5).gameObject;
        TipsUI = transform.GetChild(6).gameObject;
        BattleUI = transform.GetChild(7).gameObject;
        AdviceUI = transform.GetChild(8).gameObject;
        SettingsUI = transform.GetChild(9).gameObject;
        SaveUI = transform.GetChild(10).gameObject;
        PauseButton = transform.GetChild(11).gameObject;
        SubmitButton = CodeUI.transform.GetChild(CodeUI.transform.childCount - 2).gameObject;
        AdviceText = AdviceUI.transform.GetChild(0).GetComponent<Text>();
        CodeInput = CodeUI.transform.GetChild(0).GetComponent<InputField>();
        ErrorText = GetLastChild(ErrorUI.transform).GetComponent<Text>();
        WaitForCompilation = GetLastChild(CodeUI.transform).gameObject;
        if (BattleUI.transform.childCount != 0)
        {
            timer = GetLastChild(BattleUI.transform).GetComponent<Text>();
        }
    }
    
    private void DisablePanels()
    {
        PauseMenuUI.SetActive(false);
        CodeUI.SetActive(false);
        DeathUI.SetActive(false);
        ErrorUI.SetActive(false);
        WinUI.SetActive(false);
        TipsUI.SetActive(false);
        BattleUI.SetActive(false);
        if (AdviceUI != null)
        {
            AdviceUI.SetActive(false);
        }
        SettingsUI.SetActive(false);
        SaveUI.SetActive(false);
        WaitForCompilation.SetActive(false);
    }
    
    private void CloseAdviceUI()
    {
        if (AdviceUI != null)
        {
            AdviceUI.SetActive(false);
        }
    }
    
    private void CreateTestFiles()
    {
        int levelId = SceneManager.GetActiveScene().buildIndex;
        string path = Application.persistentDataPath + "/TestFiles" + levelId.ToString();
        Directory.CreateDirectory(path);
        path = Application.persistentDataPath + "/InputFiles" + levelId.ToString();
        Directory.CreateDirectory(path);
        path = Application.persistentDataPath + "/RealFiles" + levelId.ToString();
        Directory.CreateDirectory(path);
        for (int i = inputSize - 1; i < 5*inputSize; i+=inputSize)
        {
            path = Application.persistentDataPath  + "/InputFiles" + levelId.ToString() + "/input" + (i / inputSize).ToString() + ".txt";
            StreamWriter wrt = new StreamWriter(path);
            string s = "";
            for(int j = inputSize - 1; j >= 0; j--)
            {
                s += inputText[i - j] + Environment.NewLine;
            }
            wrt.WriteLine(s);
            wrt.Close();
        }
        for (int i = outputSize - 1; i < 5 * outputSize; i+=outputSize)
        {
            path = Application.persistentDataPath + "/RealFiles" + levelId.ToString() + "/output" + (i / outputSize).ToString() + ".txt";
            StreamWriter wrt = new StreamWriter(path);
            string s = "";
            for(int j = outputSize - 1; j >= 0; j--)
            {
                s += outputText[i - j] + Environment.NewLine;
            }
            wrt.WriteLine(s);
            wrt.Close();
        }
    }
    
    public static Transform GetLastChild (Transform transform)
    {
        return transform.GetChild(transform.childCount - 1);
    }
    
    public bool CheckProcess(string processName)
    {
        Process[] p = Process.GetProcessesByName(processName);
        if (p.Length == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void Kill()
    {
        for (int i = 0; i < 5; i++)
        {
            if (CheckProcess(Application.dataPath + "/myprogram" + i.ToString() + ".exe"))
            {
                parallel[i].Kill();
            }
            UnityEngine.Debug.Log(CheckProcess("myprogram" + i.ToString()));
        }
    }
}