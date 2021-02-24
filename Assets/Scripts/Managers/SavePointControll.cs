using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePointControll : MonoBehaviour
{

    private BoxCollider2D boxCollider;
    private bool isTouch;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        EventManager.GetEventManager.StartListening(EventEum.SavePointEnter.ToString(), Save);
    }

    // Update is called once per frame
    void Update()
    {
       if(isTouch && Input.GetKeyDown(KeyCode.E))
            EventManager.GetEventManager.TriggerEvent("EnterNextLevel");
    }

    void Save() {
        GameManager.GetGameManager.ReachSavePoint(gameObject.transform);
    }





    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EventManager.GetEventManager.TriggerEvent("SavePointEnter");
            isTouch = true;
            
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EventManager.GetEventManager.TriggerEvent("SavePointExit");
            isTouch = false;
        }
    }
}
