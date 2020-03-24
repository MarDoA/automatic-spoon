using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RayBase
{

    //float maxClimbAngle = 70;
    //float maxDescendAngle = 70;
    public bool movingRight = true;

    //public float jumpVel = 0.5f;
    //Vector3 jumpoint;
   

    public CollisionInfo collisionInfo;
    public override void Start()
    {
        base.Start();
        collisionInfo.firstJump = false;
    }
    //IEnumerator waits(ref Vector3 vel)
    //{
    //    transform.Translate(jumpoint);
    //    yield return new WaitForSeconds(waitTime);
    //    if (movingRight)
    //    {
    //        RD2D.AddForce(Vector2.right * forwardForce);
    //    }
    //    else
    //    {
    //        RD2D.AddForce(Vector2.left * forwardForce);
    //    }
    //}
    bool checkIfAbove()
    {        
        float rayLenght = skinWidth * 2;
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 point = raycastPoints.topLeft;
            point += Vector2.right * i * verticalRaySpace;
            RaycastHit2D hit = Physics2D.Raycast(point, Vector2.up, rayLenght, collisionMask);
            if (hit)
            {
                return true;
            }
        }
        return false;
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
                
                collisionInfo.above = directionY == 1;
                collisionInfo.below = directionY == -1;
            }
        }
    }
    public void changeDirection()
    {
        if (movingRight)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            movingRight = false;
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            movingRight = true;
        }
        
    }
    void horizontalCollisions(ref Vector3 vel)
    {
        float directionX = movingRight ? 1 : -1;
        float rayLenght = Mathf.Abs(vel.x) + skinWidth;
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayPoint = (directionX == -1) ? raycastPoints.bottomLeft : raycastPoints.bottomRight;
            rayPoint += Vector2.up * (horizonalRaySpace * i);
            RaycastHit2D hit = Physics2D.Raycast(rayPoint, Vector2.right * directionX, rayLenght, collisionMask);
            Debug.DrawRay(rayPoint, Vector2.right * directionX * rayLenght, Color.red);
            if (hit)
            {
                if (checkIfAbove())
                {
                    changeDirection();
                    return;
                }
                if (collisionInfo.below)
                {
                    for (int e = 0; e < jumpRayCount; e++)
                    {                       
                        Vector2 wallPoint = (directionX == -1) ? raycastPoints.topLeft : raycastPoints.topRight;
                        if (e == 0){ wallPoint.y += 0.1f; }
                        if (e == 7) { wallPoint.y -= 0.3f; }
                        wallPoint += Vector2.up * (horizonalRaySpace * e);
                        RaycastHit2D hitwall = Physics2D.Raycast(wallPoint, Vector2.right * directionX, rayLenght, collisionMask);
                        Debug.DrawRay(wallPoint, Vector2.right * directionX * rayLenght, Color.red);
                        if (hitwall)
                        {
                            changeDirection();
                            return;
                        }                        
                    }
                    if (collisionInfo.below)
                    {
                        collisionInfo.jump = true;
                        collisionInfo.jumpoint = transform.position.y + 1.5f;
                        vel.x = 0;
                    }
                    break; 
                }
                else
                {
                    if (movingRight)
                    {
                        transform.eulerAngles = new Vector3(0, 180, 0);
                        movingRight = false;
                    }
                    else
                    {
                        transform.eulerAngles = new Vector3(0, 0, 0);
                        movingRight = true;
                    }
                    return;
                }
                //if (hit.distance == 0)
                //{
                //    continue;
                //}
                //float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                //if (i == 0 && slopeAngle <= maxClimbAngle)
                //{
                //    if (collisionInfo.descending)
                //    {
                //        collisionInfo.descending = false;
                //        vel = collisionInfo.velOld;
                //    }
                //    float distanceToSlope = 0;
                //    if (slopeAngle != collisionInfo.slopeAngleOld)
                //    {
                //        distanceToSlope = hit.distance - skinWidth;
                //        vel.x -= distanceToSlope * directionX;
                //    }
                //    ClimbSlope(ref vel, slopeAngle);
                //    vel.x += distanceToSlope * directionX;
                //}
                //if (!collisionInfo.climbing || slopeAngle > maxClimbAngle)
                //{
                //    vel.x = (hit.distance - skinWidth) * directionX;
                //    rayLenght = hit.distance;

                //    if (collisionInfo.climbing)
                //    {
                //        vel.y = Mathf.Tan(collisionInfo.sloopAngle * Mathf.Deg2Rad) * Mathf.Abs(vel.x);
                //    }
                //    collisionInfo.left = directionX == -1;
                //    collisionInfo.right = directionX == 1;
                //}
            }
        }
    }
    //void ClimbSlope(ref Vector3 vel, float angle)
    //{
    //    collisionInfo.climbing = true;
    //    float moveDistance = Mathf.Abs(vel.x);
    //    float climbingY = moveDistance * Mathf.Sin(angle * Mathf.Deg2Rad);
    //    if (vel.y <= climbingY)
    //    {
    //        vel.y = climbingY;
    //        vel.x = moveDistance * Mathf.Cos(angle * Mathf.Deg2Rad) * Mathf.Sign(vel.x);
    //        collisionInfo.below = true;
    //        collisionInfo.sloopAngle = angle;
    //    }
    //}
    //void DescendSlope(ref Vector3 vel)
    //{
    //    float directionX = Mathf.Sign(vel.x);
    //    Vector2 rayPoint = (directionX == -1) ? raycastPoints.bottomRight : raycastPoints.bottomLeft;
    //    RaycastHit2D hit = Physics2D.Raycast(rayPoint, Vector2.down, Mathf.Infinity, collisionMask);
    //    if (hit)
    //    {
    //        float angle = Vector2.Angle(hit.normal, Vector2.up);
    //        if (angle != 0 && angle <= maxDescendAngle)
    //        {
    //            if (Mathf.Sign(hit.normal.x) == directionX)
    //            {//detect distance based on the next movement vs. now
    //                if (hit.distance - skinWidth <= Mathf.Tan(angle * Mathf.Deg2Rad) * Mathf.Abs(vel.x))//(hit.distance <= skinwidth)
    //                {
    //                    float moveDistance = Mathf.Abs(vel.x);
    //                    float descentVelY = moveDistance * Mathf.Sin(angle * Mathf.Deg2Rad);
    //                    float descentVelX = moveDistance * Mathf.Cos(angle * Mathf.Deg2Rad) * Mathf.Sign(vel.x);
    //                    vel.y -= descentVelY;
    //                    vel.x = descentVelX;
    //                    collisionInfo.sloopAngle = angle;
    //                    collisionInfo.descending = true;
    //                    collisionInfo.below = true;
    //                }
    //            }
    //        }
    //    }
    //}
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;
        //public bool climbing, descending;
        public bool jump;
        public bool firstJump;
        //public float sloopAngle, slopeAngleOld;
        //public Vector3 velOld;
        public float jumpoint;
        public void reset()
        {
            above = below = false;
            left = right = false;
            jump = false;
            //climbing = descending = false;

            //slopeAngleOld = sloopAngle;
            //sloopAngle = 0;
        }
    }
    public void Move(Vector3 vel, bool standingplat = false)
    {
        collisionInfo.reset();
        //collisionInfo.velOld = vel;
        UpdateRaycastPoints();
        //if (vel.y < 0)
        //    DescendSlope(ref vel);
        if (vel.y != 0)
            verticalCollisions(ref vel);
        if (vel.x != 0)
            horizontalCollisions(ref vel);
        
        transform.Translate(vel);
        if (standingplat)
        {
            collisionInfo.below = true;
        }
    }
}
