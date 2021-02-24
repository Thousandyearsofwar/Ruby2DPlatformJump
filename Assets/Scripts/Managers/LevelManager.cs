using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

 

public class LevelManager : MonoBehaviour
{
    
    private int currentLevel=0;
    string[] LevelConfig = { "Level0", "Level1","Level2" };

    private static LevelManager levelManager;
    public static LevelManager GetLevelManager {
        get
        {
            if (levelManager == null) {
                levelManager = GameObject.FindObjectOfType<LevelManager>();
                if (levelManager == null) {
                    GameObject gameObject = new GameObject();
                    levelManager = gameObject.AddComponent<LevelManager>();
                }
            }
            return levelManager;
        }
    }

    private LevelManager() { }


    private void LoadNextLevel() {
        if (GetLevelManager == null)
            return;
        levelManager.currentLevel++;
        SceneManager.LoadScene(levelManager.LevelConfig[currentLevel]);
    }


    // Start is called before the first frame update
    void Start()
    {
        EventManager.GetEventManager.StartListening(EventEum.EnterNextLevel.ToString(), LoadNextLevel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
