using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogAi : Enemy
{
    private Rigidbody2D rigidbody;
    private Collider2D collider;

    public float speed;
    public float jumpForce;

    private bool FaceLeft;
    public Transform leftBound;
    public Transform rightBound;
    private float leftB, rightB;
    public LayerMask ground;

    // Start is called before the first frame update
    protected override void  Start()
    {
        base.Start();
        FaceLeft = true;


        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
        leftB = leftBound.position.x;
        rightB = rightBound.position.x;
        //transform.DetachChildren();
        Destroy(leftBound.gameObject);
        Destroy(rightBound.gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        SwitchAnimate();
    }

    private void Movement() {

        if (transform.position.x < leftB)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            FaceLeft = false;
        }
        else
        if (transform.position.x > rightB)
        {
            transform.localScale = new Vector3(1, 1, 1);
            FaceLeft = true;
        }
        if (FaceLeft)
        {
            if (collider.IsTouchingLayers(ground))
            {
                animator.SetBool("Jumping",true);
                rigidbody.velocity = new Vector2(-speed, jumpForce);
            }
        }
        else {
            if (collider.IsTouchingLayers(ground))
            {
                animator.SetBool("Jumping", true);//Idel-->Jumping
                rigidbody.velocity = new Vector2(speed, jumpForce);
            }
        }



    }

    private void SwitchAnimate() {
        if (animator.GetBool("Jumping")&& rigidbody.velocity.y < 0.1)//Jumping->Falling
        {
                animator.SetBool("Jumping",false);
                animator.SetBool("Falling",true);
        }
        if (animator.GetBool("Falling")&&collider.IsTouchingLayers(ground))//Falling->Idle
        {
            animator.SetBool("Falling",false);
        }
    }

}
