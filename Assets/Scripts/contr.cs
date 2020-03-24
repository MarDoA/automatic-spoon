using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class contr : RayBase
{
    Rigidbody2D rb;
    bool cool;

    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnMouseUp()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.isKinematic = false;
        cool = false;
    }
    private void OnMouseDown()
    {
        cool = true;
    }
    private void FixedUpdate()
    {
        if (cool)
        {
            float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            Vector3 movePoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));          
            if (checkIfPlayerAbove())
            {
                rb.constraints = RigidbodyConstraints2D.FreezeAll | RigidbodyConstraints2D.FreezeRotation;
                rb.isKinematic = true;
            }
            else
            {
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                rb.isKinematic = false;
                rb.MovePosition(movePoint);
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rigi = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rigi != null )
        {
            rigi.velocity = Vector2.zero;
        }
    }
    bool checkIfPlayerAbove()
    {
        UpdateRaycastPoints();
        float rayLenght = skinWidth * 2;
        for (int i = 0; i < verticalRayCount; i++)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            Vector2 point = raycastPoints.topLeft;
            point += Vector2.right * i * verticalRaySpace;
            RaycastHit2D hit = Physics2D.Raycast(point, Vector2.up, rayLenght, collisionMask);
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
            if (hit) { return true; }            
        }
        return false;
    }
}
