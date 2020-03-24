using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragTest : RayBase
{
    bool start = false;
    Vector3 oldPoint;
    Vector3 velocity;
    public float gravity = -20;
    public float speed= 1;
    BoxCollider2D box;
    private Vector3 distance;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        oldPoint = Vector3.zero;
        box = GetComponent<BoxCollider2D>();
    }
    private void OnMouseDown()
    {
        distance = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)) - transform.position;
        start = true;
    }
    private void OnMouseUp()
    {
        start = false;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (start)
        {
            float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            Vector3 movePoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));
            movePoint.x -= distance.x;
            movePoint.y -= distance.y;
            //if (!movePoint.Equals(oldPoint))
            {
                oldPoint = movePoint;
                Vector3 dir = (movePoint - transform.position).normalized;
                velocity = dir * Time.deltaTime * speed;
                Move(ref velocity);
            }           
        }
        else
        {            
            velocity.y += gravity* Time.deltaTime;
            velocity.x = 0;
            Move(ref velocity);
        }
    }
    void Move(ref Vector3 vel)
    {
        //collisionInfo.reset();
        ////collisionInfo.velOld = vel;
        UpdateRaycastPoints();
        ////if (vel.y < 0)
        ////    DescendSlope(ref vel);
        box.enabled = false;
        if (vel.y != 0)
            verticalCollisions(ref vel);
        if (vel.x != 0)
            horizontalCollisions(ref vel);
        box.enabled = true;
        transform.Translate(vel);
    }

    void horizontalCollisions(ref Vector3 vel)
    {
        float directionX = Mathf.Sign(vel.x);
        float rayLenght = Mathf.Abs(vel.x) + skinWidth;
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayPoint = (directionX == -1) ? raycastPoints.bottomLeft : raycastPoints.bottomRight;
            rayPoint += Vector2.up * horizonalRaySpace * i;
            RaycastHit2D hit = Physics2D.Raycast(rayPoint, Vector2.up * directionX, rayLenght, collisionMask);
            Debug.DrawRay(rayPoint, Vector2.right * directionX * rayLenght, Color.red);
            if (hit)
            {
                vel.x = (hit.distance - skinWidth) * directionX;
                rayLenght = hit.distance;
            }
        }
    }

    void verticalCollisions(ref Vector3 vel)
    {
        float directionY = Mathf.Sign(vel.y);
        float rayLenght = Mathf.Abs(vel.y) + skinWidth;
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayPoint = (directionY == -1) ? raycastPoints.bottomLeft : raycastPoints.topLeft;
            rayPoint += Vector2.right * (verticalRaySpace * i + vel.x);
            RaycastHit2D hit = Physics2D.Raycast(rayPoint, Vector2.up * directionY, rayLenght, collisionMask);
            Debug.DrawRay(rayPoint, Vector2.up * directionY * rayLenght, Color.red);
            if (hit)
            {
                vel.y = (hit.distance - skinWidth) * directionY;
                rayLenght = hit.distance;

                //collisionInfo.above = directionY == 1;
                //collisionInfo.below = directionY == -1;
            }
        }
    }
}
