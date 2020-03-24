using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Controller2D))]
public class PlayerController : MonoBehaviour
{
    //public float jumpHeight = 4;
    //public float timeToReachApex = 0.4f;

    //float smoothGrounded = 0.07f;
    //float smoothAir = 0.2f;
    float velocityXSmooth;
    public float jumpVel = 8;
    float gravity;
    bool find = false;
    public int count = 0;
    public float moveSpeed = 1.65f;
    Vector3 velocity;

    Controller2D controller;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        gravity = -20;
        //jumpVel = Mathf.Abs(gravity) * timeToReachApex;
        //print(jumpVel + " " + gravity);
    }

    void FixedUpdate()
    {
        if (controller.collisionInfo.above || controller.collisionInfo.below)
        {
            velocity.y = 0;
        }
        //Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));        
        //float targetVelocityX = input.x * moveSpeed;
        //velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmooth, controller.collisionInfo.below ? smoothGrounded : smoothAir);
        velocity.y += gravity * Time.deltaTime;
        velocity.x = moveSpeed * 1;
        if (controller.collisionInfo.jump)
        {
            controller.collisionInfo.firstJump = true;
            velocity.y = jumpVel;
            velocity.x = 0;
            find = true;
        }
        if (controller.collisionInfo.firstJump&&transform.position.y>=controller.collisionInfo.jumpoint)
        {
            controller.collisionInfo.firstJump = false;
        }
        
        if (!controller.collisionInfo.below)
        {   
            velocity.x = 0;            
        }
        if (find)
        {
            if (transform.position.y >= controller.collisionInfo.jumpoint)
            {
                velocity.x = moveSpeed;
                count++;
                if (count >= 5)
                {
                    find = false;
                    count = 0;
                }
            }
        }
        controller.Move(velocity * Time.deltaTime);

        //controller.Move(Vector3.right * moveSpeed * Time.deltaTime);

    }
}
