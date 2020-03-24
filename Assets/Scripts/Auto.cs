using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Auto : RayBase
{
    public float speed = 4;
    public bool right = true;
    public LayerMask passengerMask;
    Controller2D player;
    
    List<PassengersMovement> passengersMovement;
    Dictionary<Transform, Controller2D> passengerDic = new Dictionary<Transform, Controller2D>();
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Controller2D>();
    }
    public void Lever()
    {
        if (right) { right = false; }
        else { right = true; }
    }
    struct PassengersMovement
    {
        public Transform transform;
        public Vector3 vel;
        public bool standing;
        //public bool moveBefore;

        public PassengersMovement(Transform _transform, Vector3 _vel, bool _standing/*, bool _moveBefore*/)
        {
            transform = _transform;
            vel = _vel;
            standing = _standing;
            //moveBefore = _moveBefore;
        }
    }
    // Update is called once per frame
    void Update()
    {
        UpdateRaycastPoints();

        calculatePassengersMovement();

        MovePassengers();
    }
    void MovePassengers()
    {
        foreach (PassengersMovement passenger in passengersMovement)
        {
            if (!passengerDic.ContainsKey(passenger.transform))
            {
                passengerDic.Add(passenger.transform, passenger.transform.GetComponent<Controller2D>());

            }
            
            passengerDic[passenger.transform].Move(passenger.vel);
            
        }
    }
    void calculatePassengersMovement()
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengersMovement = new List<PassengersMovement>();
        float direction = (right) ? 1 : -1;

        float raylength = skinWidth * 3;
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayPoint =raycastPoints.topLeft;
            rayPoint += Vector2.right * (verticalRaySpace * i);
            RaycastHit2D hit = Physics2D.Raycast(rayPoint, Vector2.up, raylength, passengerMask);
            Debug.DrawRay(rayPoint, Vector2.up * raylength, Color.red);
            if (hit)
            {
                if (!movedPassengers.Contains(hit.transform))
                {                   
                    float pushX = speed*Time.deltaTime ;
                    if (hit.collider.gameObject.CompareTag("Player"))
                    {
                        //pushX *= (player.movingRight == right) ? 1 : -1;
                        if(player.movingRight!= right)
                            player.changeDirection();
                        movedPassengers.Add(hit.transform);
                        passengersMovement.Add(new PassengersMovement(hit.transform, new Vector3(pushX, 0), true));
                    }
                    else
                    {
                        hit.transform.Translate(new Vector3(pushX * direction, 0));
                    }
                }

            }
        }

    }
}
