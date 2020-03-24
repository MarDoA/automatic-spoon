using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : RayBase
{
    public Vector3[] wayPoints;
    Vector3[] globalWayPoints;
    public LayerMask passengerMask;

    public bool cycle;
    public float waitTime;
    public float speed;
    [Range(0, 2)]
    public float easeAmount;

    int fromWayPointIndex = 0;
    float percentBetweenPoints;
    float nextMoveTime;

    List<PassengersMovement> passengersMovement;
    Dictionary<Transform, Controller2D> passengerDic = new Dictionary<Transform, Controller2D>();
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        globalWayPoints = new Vector3[wayPoints.Length];
        for (int i = 0; i < wayPoints.Length; i++)
        {
            globalWayPoints[i] = wayPoints[i] + transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRaycastPoints();

        Vector3 vel = CalculatePlatMove();

        calculatePassengersMovement(vel);

        MovePassengers(true);
        transform.Translate(vel);
        MovePassengers(false);
    }
    float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
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
    Vector3 CalculatePlatMove()
    {
        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }
        fromWayPointIndex %= globalWayPoints.Length;
        int toWayPoint = (fromWayPointIndex + 1) % globalWayPoints.Length;
        float distanceBetween = Vector3.Distance(globalWayPoints[fromWayPointIndex], globalWayPoints[toWayPoint]);
        percentBetweenPoints += Time.deltaTime * speed / distanceBetween;
        percentBetweenPoints = Mathf.Clamp01(percentBetweenPoints);
        float easedPercent = Ease(percentBetweenPoints);

        Vector3 newPos = Vector3.Lerp(globalWayPoints[fromWayPointIndex], globalWayPoints[toWayPoint], easedPercent);
        if (percentBetweenPoints >= 1)
        {
            nextMoveTime = Time.time + waitTime;
            percentBetweenPoints = 0;
            fromWayPointIndex++;
            if (!cycle)
            {
                if (fromWayPointIndex >= globalWayPoints.Length - 1)
                {
                    fromWayPointIndex = 0;
                    System.Array.Reverse(globalWayPoints);
                }
            }
        }
        return newPos - transform.position;
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

    private void OnDrawGizmos()
    {
        if (wayPoints != null)
        {
            Gizmos.color = Color.red;
            float size = 0.3f;
            for (int i = 0; i < wayPoints.Length; i++)
            {
                Vector3 globalWayPoint = (Application.isPlaying) ? globalWayPoints[i] : wayPoints[i] + transform.position;
                Gizmos.DrawLine(globalWayPoint - Vector3.up * size, globalWayPoint + Vector3.up * size);
                Gizmos.DrawLine(globalWayPoint - Vector3.left * size, globalWayPoint + Vector3.left * size);
            }
        }
    }
}
