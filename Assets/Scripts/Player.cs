using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{

    public float Speed = 10;
    public Transform groundDetection;
    public Transform doubleWallDetection;
    public Transform jumpcheck;
    public Transform groundcheck;
    public float wallLenght;
    public float doubleWallLenght;
    public float jumpForce;
    public float waitTime;
    public float forwardForce;

    public bool fors;

    private Rigidbody2D RD2D;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool movingRight = true;
    public bool grounded = true;
    private int collisionCount = 0;
    private bool detected = false;
    // Start is called before the first frame update
    void Start()
    {
        RD2D = GetComponent<Rigidbody2D>();       
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //if (detected)
        //{
        //    grounded = false;
        //    detected = false;
        //}
        Move();
    }
    IEnumerator waits()
    {
        grounded = false;
        RD2D.AddForce(Vector2.up * jumpForce);
        yield return new WaitForSeconds(waitTime);
        if (movingRight)
        {
            RD2D.AddForce(Vector2.right * forwardForce);
        }
        else
        {
            RD2D.AddForce(Vector2.left * forwardForce);
        }
        detected = false;
              
    }
    private void Move()
    {       
        if (grounded&& !detected)
        {          
            RaycastHit2D groundInd = Physics2D.Raycast(groundDetection.position, Vector2.up, wallLenght);
            if (groundInd.collider)
            {             
                RaycastHit2D doubleWall = Physics2D.Raycast(doubleWallDetection.position, Vector2.up, doubleWallLenght);
                RaycastHit2D check = Physics2D.Raycast(jumpcheck.position, Vector2.up, doubleWallLenght);
                RaycastHit2D gcheck = Physics2D.Raycast(groundcheck.position, Vector2.down, 0.1f);
                if (!doubleWall.collider && !check.collider&&gcheck.collider)
                {
                    StartCoroutine(waits());
                    //jump();
                    detected = true;
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
                }
                              
            }
            else
            {
                transform.Translate(Vector2.right * Speed * Time.deltaTime);
            }
        }     
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        collisionCount--;
        if (collisionCount<= 0)
        {
            grounded = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collisionCount++;
        if (collision.collider.CompareTag("Box"))
        {
            RD2D.velocity = Vector3.zero;
            RD2D.angularVelocity = 0;
        }
        if (collisionCount > 0)
        {
            grounded = true;
        }
    }
}
