using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Game running parameter
    private int Point;
    public Transform savePointTrans;

    //UI paramater
    public TextMeshProUGUI ScoreText;
    private PlayerBetterController player;


    private static GameManager gameManager;
    public static GameManager GetGameManager
    {
        get
        {
            if (gameManager == null)
            {
                gameManager = GameObject.FindObjectOfType<GameManager>();
                if (gameManager == null)
                {
                    GameObject gameObject = new GameObject();
                    gameManager = gameObject.AddComponent<GameManager>();
                }
            }
            return gameManager;
        }
    }

    private GameManager() { }



    // Start is called before the first frame update
    void Start()
    {
        EventManager.GetEventManager.StartListening(EventEum.PlayerDead.ToString(),ResetPlayer);

        EventManager.GetEventManager.StartListening(EventEum.GameStart.ToString(),ResetPlayer);
        //EventManager.GetEventManager.StartListening(EventEum.GameStart.ToString(), GameStart);

        GetGameManager.savePointTrans = GameObject.FindGameObjectWithTag("savePoint_start").transform;
        player = GameObject.FindObjectOfType<PlayerBetterController>();
        Point = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpDateScore(int addPoint) {
        Point += addPoint;
        ScoreText.text = "Score:"+ Point;
    }

    public void ResetPlayer() {
        if(player==null)
            player = GameObject.FindObjectOfType<PlayerBetterController>();
        player.gameObject.transform.position= savePointTrans.position;           
    }

    public void ReachSavePoint(Transform savePoint) {
        savePointTrans = savePoint;
    }

    private void GameStart() {
       

    }

}
