using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player02 : MonoBehaviour
{
    //Rigidbody2D rb;
    public float Speed=1f;
    public float fallSpeed = 2f;
    public float wallLenght=1;
    public float doubleWallLenght = 2;
    public float jumpHieght = 1.5f;
    public float jumpSpeed = 7f;
    public float forwardThrust = 0.5f;
    public float forwardSpeed = 3f;
    public Transform topLeft;
    public Transform topRight;
    public Transform bottomLeft;
    public Transform bottomRight;
    public Transform bottomRight2;


    private float groundLevel = 0.05f;
    private float wallLe;
    private int collCount = 0;   
    private float maxPosy;
    private float maxPosx;
    private bool grounded = false;
    private bool movingRight = true;
    private bool jumping = false;
    private bool jumpdet = false;


    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        wallLe = bottomRight2.position.y - bottomRight.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!jumpdet)
        {
            checkGround();
        }
        Move();
    }
    private void checkGround()
    {
        RaycastHit2D groundCheck = Physics2D.Raycast(bottomRight.position, Vector2.down, groundLevel);
        if (groundCheck.collider)
        {
            grounded = true;
        }
        else
        {
            RaycastHit2D groundCheck2 = Physics2D.Raycast(bottomLeft.position, Vector2.down, groundLevel);
            if (groundCheck2.collider)
            {
                grounded = true;
            }
            else
            { 
                grounded = false;
            }
        }
    }
    IEnumerator Jump()
    {
        while (true)
        {
            if (transform.position.y >= maxPosy)
            {
                jumping = false;
                jumpdet = false;
            }
            if (jumping)
            {
                transform.Translate(Vector2.up * jumpSpeed * Time.smoothDeltaTime);
            }
            else if (!jumping)
            {
                if (!movingRight)
                {
                    if (transform.position.x < maxPosx)
                    {
                        transform.Translate(Vector2.right * forwardSpeed * Time.smoothDeltaTime);
                    }
                }
                else
                {
                    if (transform.position.x < maxPosx)
                    {
                        transform.Translate(Vector2.right * forwardSpeed * Time.smoothDeltaTime);
                    }
                }
                if (grounded)
                    StopAllCoroutines();
            }
            yield return new WaitForEndOfFrame();
        }
    }
    private void Move()
    {
        if (grounded)
        {           
            RaycastHit2D groundInd = Physics2D.Raycast(bottomRight.position, Vector2.up, wallLenght);
            if (groundInd.collider)
            {
                RaycastHit2D groundInd2 = Physics2D.Raycast(bottomRight2.position, Vector2.up,wallLenght-wallLe);
                if (groundInd.collider == groundInd2.collider)
                {   
                    RaycastHit2D doubleWall = Physics2D.Raycast(topRight.position, Vector2.up, doubleWallLenght);
                    if (!doubleWall.collider)
                    {
                        jumpdet = true;
                            maxPosy = transform.position.y + jumpHieght;
                            if (!movingRight)
                            {
                                maxPosx = transform.position.x + forwardThrust;
                                transform.position = new Vector3
                                (transform.position.x + 0.05f, transform.position.y, transform.position.z);
                            }
                            else
                            {
                                maxPosx = transform.position.x + forwardThrust;
                                transform.position = new Vector3
                                (transform.position.x - 0.05f, transform.position.y, transform.position.z);
                            }
                            transform.position = new Vector3
                                (transform.position.x, transform.position.y + 0.05f, transform.position.z);
                            grounded = false;
                            jumping = true;
                            StartCoroutine("Jump");

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
                else if (bottomRight.position.y <= groundInd.point.y && groundInd.point.y <= bottomRight.position.y+0.1f)
                {
                    transform.position = new Vector3
                        (transform.position.x, transform.position.y+0.03f, transform.position.z);
                }
            }
            else
            {
                transform.Translate(Vector2.right * Speed * Time.deltaTime);
            }
        }
        else if(!jumping&&!grounded)
        {
            transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
        }
    }
}
