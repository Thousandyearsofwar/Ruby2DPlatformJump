using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleAi : Enemy
{
    private Rigidbody2D rigidbody;
    private Collider2D collider;

    private bool FaceLeft;
    public bool Chase;
    public BoxCollider2D chaseRange;

    public Transform bottomBound;
    public Transform topBound;
    private float bottom, top;
    public float speed;
    private GameObject chaseTarget;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Chase = false;
        
        FaceLeft = true;
        

        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();

        top = topBound.position.y;
        bottom = bottomBound.position.y;
        rigidbody.velocity = new Vector2(0, -speed);

        transform.DetachChildren();
        

        Destroy(topBound.gameObject);
        Destroy(bottomBound.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        SwitchAnimation();
    }


    void Movement() {
        transform.localScale=new Vector3(FaceLeft?1:-1,1,1);
        if (!Chase)
        {
            if (transform.position.y > top)
                rigidbody.velocity = new Vector2(0, -speed);
            if (transform.position.y < bottom)
                rigidbody.velocity = new Vector2(0, speed);
        }
        if(Chase)
        {
            Vector3 chaseDir = (chaseTarget.transform.position - transform.position).normalized;
            FaceLeft = chaseDir.x < 0;
            rigidbody.velocity = chaseDir * speed;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player"))
        {
            Chase = true;
            chaseTarget = collision.gameObject;
        }
    }
    



    void SwitchAnimation() {
        animator.SetBool("Chase",Chase);
    }

    


}
