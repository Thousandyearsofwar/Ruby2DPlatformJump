using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class GUIManager : MonoBehaviour
{
    private static GUIManager guiManager;

    public GameObject scoreText;
    public GameObject enterPanel;

    //GameStartMenu
    public GameObject gameStartMenu;
    public GameObject gamePauseMenu;

    private Dictionary<string, GameObject> GUIDictionary=new Dictionary<string, GameObject>();

    public static GUIManager GetGUIManager
    {
        get {
            if (guiManager == null)
            {
                guiManager = GameObject.FindObjectOfType<GUIManager>();
                if (guiManager == null) {
                    GameObject go = new GameObject();
                    guiManager = go.AddComponent<GUIManager>();
                }
            }
            return guiManager;
        }
    }




    private GUIManager() {
        
        
    }



    // Start is called before the first frame update
    void Start()
    {
        EventManager.GetEventManager.StartListening(EventEum.SavePointEnter.ToString(), ShowSavePointGUI);
        EventManager.GetEventManager.StartListening(EventEum.SavePointExit.ToString(), DisapSavePointGUI);

        EventManager.GetEventManager.StartListening(EventEum.GamePause.ToString(), GamePause);
        EventManager.GetEventManager.StartListening(EventEum.EnterNextLevel.ToString(),EnterNextLevel);
        
        //guiManager.GUIDictionary.Add("EnterPanel", enterPanel);
        //guiManager.GUIDictionary.Add("ScoreText", scoreText);
    }

        





    // Update is called once per frame
    void Update()
    {

    }
    //Listener func
    public void GameStart() {
        GetGUIManager.scoreText.SetActive(true);
        GetGUIManager.gameStartMenu.SetActive(false);
        GetGUIManager.gamePauseMenu.SetActive(false);
        EventManager.GetEventManager.TriggerEvent(EventEum.GameStart.ToString());
    }

    public void GameExit() {

        gameStartMenu.SetActive(true);

    }

    public void GamePause() {
        gameStartMenu.SetActive(false);
        gamePauseMenu.SetActive(true);
    }

    public void EnterNextLevel() {
        gamePauseMenu.SetActive(true);
    }


    public void ShowSavePointGUI() {
        //GameObject showUI;
        //GUIDictionary.TryGetValue("EnterPanel", out showUI);
        enterPanel.SetActive(true);
        enterPanel.GetComponent<Animator>().SetBool("Show", true);
    }

    public void DisapSavePointGUI() {
        enterPanel.GetComponent<Animator>().SetBool("Show",false);
        
    }


}
