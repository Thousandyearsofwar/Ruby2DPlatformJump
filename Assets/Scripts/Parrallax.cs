using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parrallax : MonoBehaviour
{
    private GameObject Cam;
    private float startPosX;
    private float startPosY;
    public float MoveRate;
    public bool LockY;

    // Start is called before the first frame update
    void Start()
    {
        Cam = FindObjectOfType<Camera>().gameObject;
        startPosX =transform.position.x;
        startPosY =transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(LockY)
            transform.position = new Vector2(startPosX+Cam.transform.position.x * MoveRate, transform.position.y);
        else
            transform.position = new Vector2(startPosX + Cam.transform.position.x * MoveRate, startPosY+Cam.transform.position.y*MoveRate);
    }
}
