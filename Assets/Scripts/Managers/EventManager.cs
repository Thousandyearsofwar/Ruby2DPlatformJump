using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine;

enum EventEum {
GameStart,
GamePause,
SavePointEnter,
SavePointExit,
PlayerDead,
EnterNextLevel
}


public class EventManager : MonoBehaviour
{
    private Dictionary<string, UnityEvent> eventDictionary = new Dictionary<string, UnityEvent>();
    private static EventManager eventManager;

    public static EventManager GetEventManager {
        get {
            if (eventManager == null) {
                eventManager = GameObject.FindObjectOfType<EventManager>();
                if (eventManager == null){
                    GameObject gameObject = new GameObject();
                    eventManager = gameObject.AddComponent<EventManager>();
                }
            }
            return eventManager;
        }
    }

    private EventManager() { }

    public void StartListening(string eventName,UnityAction listener) {
        if (eventManager == null)
            return;
        UnityEvent thisEvent = null;
        if (eventManager.eventDictionary.TryGetValue(eventName, out thisEvent))
            thisEvent.AddListener(listener);
        else {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            eventManager.eventDictionary.Add(eventName,thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction listener) {
        if (eventManager == null)
            return;
        UnityEvent thisEvent = null;
        if (eventManager.eventDictionary.TryGetValue(eventName, out thisEvent))
            thisEvent.RemoveListener(listener);
    }

    public void TriggerEvent(string eventName) {
        UnityEvent thisEvent = null;
        if (eventManager.eventDictionary.TryGetValue(eventName, out thisEvent))
            thisEvent.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
