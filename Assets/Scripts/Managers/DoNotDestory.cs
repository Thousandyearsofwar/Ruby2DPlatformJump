using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestory : MonoBehaviour
{
    GameObject[] doNotDestoryGOs;



    // Start is called before the first frame update
    void Start()
    {
        doNotDestoryGOs = new GameObject[gameObject.transform.childCount];
        for (int i = 0; i < gameObject.transform.childCount; i++)
            doNotDestoryGOs[i] = gameObject.transform.GetChild(i).gameObject;
        foreach (GameObject i in doNotDestoryGOs)
            DontDestroyOnLoad(i);
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
