using UnityEngine;

using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{	
	Player player;
    public Animator animator;
    public static Dictionary<string, KeyCode> Controls = new Dictionary<string, KeyCode>();

    void Awake()
    {
        player = GetComponent<Player>();
        Controls.Clear();
        if (PlayerPrefs.GetInt("loadedLevel") != 0)
        {
            LoadPlayer();
        }
        if (!File.Exists(Application.persistentDataPath + "/settingsInfo.dat"))
        {
            Controls.Add("Right", KeyCode.D);
            Controls.Add("Left", KeyCode.A);
            Controls.Add("Jump", KeyCode.Space);
            Controls.Add("Code", KeyCode.C);
            Controls.Add("Tips", KeyCode.T);
            Controls.Add("Interact", KeyCode.E);
        }
        else
        {
            LoadControlSettings();
        }
    }
    
    void Update()
    {
        int horizontalInput = 0;
        if (Input.GetKey(Controls["Left"]))
        {
            gameObject.transform.localScale = new Vector2(-1, 1);
            horizontalInput = -1;
        }
        if (Input.GetKey(Controls["Right"]))
        {
            gameObject.transform.localScale = new Vector2(1, 1);
            horizontalInput = 1;
        }
        
        Vector2 directionalInput = new Vector2(horizontalInput, 0);
        player.SetDirectionalInput(directionalInput);
        
        if (player.controller.collisions.below)
        {   
            if ((Input.GetKey(Controls["Right"]) || Input.GetKey(Controls["Left"])))
            {
                animator.SetFloat("speed", 1f);
            }
            else
            {
                animator.SetFloat("speed", 0f);
            }
            animator.SetBool("is Jumping", false);
        }
        
        if (Input.GetKeyDown(Controls["Jump"]))
        {
            player.OnJumpInputDown();
            animator.SetBool("is Jumping", true);
        }
        
        if (Input.GetKeyUp(Controls["Jump"]))
        {
            player.OnJumpInputUp();
            animator.SetBool("is Jumping", true);
        }
    }
    
    void LoadPlayer()
    {
        string path = Application.persistentDataPath + "/playerInfo" + PlayerPrefs.GetInt("loadedLevel") + ".dat";
        if (File.Exists(path))
        {   
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();
            transform.position = new Vector3(data.x, data.y,-1f);
            GameProgressController.code = data.code;
            GameProgressController.tips = data.tips;
            GameProgressController.isDefeated = data.isDefeated;
            GameProgressController.errorMade = data.errorMade;
        }
    }
    
    private void LoadControlSettings()
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
    }
    
}