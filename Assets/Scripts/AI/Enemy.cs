using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Animator animator;

    // Start is called before the first frame update
    protected  virtual void Start()
    {
        animator = GetComponent<Animator>();

    }


    public void DeathBehaviourFir()
    {
        GetComponent<Rigidbody2D>().isKinematic=true;
        GetComponent<Collider2D>().enabled = false;
      
    }

    public void DeathBehaviourSec()
    {
       
        Destroy(gameObject);
    }

}
