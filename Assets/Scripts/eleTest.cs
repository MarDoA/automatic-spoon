using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eleTest : RayBase
{
    public float maxY, minY,speed,maxSpeed;
    private float rSpeed;
    public bool vertical;
    public LayerMask passengerMask,solidMask;
    
    Dictionary<Transform, Controller2D> passengerDic = new Dictionary<Transform, Controller2D>();
 
    Vector3 GetMouseWorldPos()
    {
        rSpeed = speed;
        Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        target.z = transform.position.z;
        if (vertical)
        {
            target.x = transform.position.x;
        }
        if (target.y>=maxY)
        {
            target.y = maxY;
            rSpeed = maxSpeed;
        }
        if (target.y<=minY)
        {
            target.y = minY;
            rSpeed = maxSpeed;        
        }
        return target;
    }
    Vector3 CalculatePlatMove()
    {
        return GetMouseWorldPos() - transform.position;
    }
    List<PassengersMovement> passengersMovement;
    struct PassengersMovement
    {
        public Transform transform;
        public Vector3 vel;
        public bool standing;
        public bool moveBefore;

        public PassengersMovement(Transform _transform, Vector3 _vel, bool _standing, bool _moveBefore)
        {
            transform = _transform;
            vel = _vel;
            standing = _standing;
            moveBefore = _moveBefore;
        }
    }
    void calculatePassengersMovement(Vector3 vel)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengersMovement = new List<PassengersMovement>();
        float directionX = Mathf.Sign(vel.x);
        float directionY = Mathf.Sign(vel.y);

        if (vel.y != 0)
        {
            float raylength = Mathf.Abs(vel.y) + skinWidth;
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayPoint = (directionY == -1) ? raycastPoints.bottomLeft : raycastPoints.topLeft;
                rayPoint += Vector2.right * (verticalRaySpace * i);
                RaycastHit2D hit = Physics2D.Raycast(rayPoint, Vector2.up * directionY, raylength, passengerMask);
                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = (directionY == 1) ? vel.x : 0;
                        float pushY = vel.y - (hit.distance - skinWidth) * directionY;

                        passengersMovement.Add(new PassengersMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    }

                }
            }
        }
        if (vel.x != 0)
        {
            float raylength = Mathf.Abs(vel.x) + skinWidth;
            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayPoint = (directionX == -1) ? raycastPoints.bottomLeft : raycastPoints.bottomRight;
                rayPoint += Vector2.up * (horizonalRaySpace * i);
                RaycastHit2D hit = Physics2D.Raycast(rayPoint, Vector2.up * directionX, raylength, passengerMask);
                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = vel.x - (hit.distance - skinWidth) * directionX;
                        float pushY = 0;

                        passengersMovement.Add(new PassengersMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                    }

                }
            }
        }
        if (directionY == -1 || vel.y == 0 && vel.x != 0)
        {
            float raylength = 2 * skinWidth;
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayPoint = raycastPoints.topLeft;
                rayPoint += Vector2.right * (verticalRaySpace * i);
                RaycastHit2D hit = Physics2D.Raycast(rayPoint, Vector2.up, raylength, passengerMask);
                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = vel.x;
                        float pushY = vel.y;

                        passengersMovement.Add(new PassengersMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                    }
                }
            }
        }
    }
    void MovePassengers(bool beforePlat)
    {
        foreach (PassengersMovement passenger in passengersMovement)
        {
            if (!passengerDic.ContainsKey(passenger.transform))
            {
                passengerDic.Add(passenger.transform, passenger.transform.GetComponent<Controller2D>());

            }
            if (passenger.moveBefore == beforePlat)
            {
                passengerDic[passenger.transform].Move(passenger.vel, passenger.standing);
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
                if (directionY==-1)
                {
                    vel.y = (hit.distance - skinWidth) * directionY;
                    rayLenght = hit.distance;
                }
            }
        }       
    }
    private void OnMouseDrag()
    {
        UpdateRaycastPoints();

        Vector3 vel = CalculatePlatMove() * Time.deltaTime *rSpeed;

        verticalCollisions(ref vel);
        calculatePassengersMovement(vel);
        MovePassengers(true);
        transform.Translate(vel);
        MovePassengers(false);       
    }

}
