using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevator : MonoBehaviour
{
    /*
    public float maxY;
    public float minY;
    private void OnMouseDrag()
    {
        Vector3 mousePos = new Vector3(0, Input.mousePosition.y, 10);
        Vector3 objPos = Camera.main.ScreenToWorldPoint(mousePos);
        objPos.x = transform.position.x;
        if (objPos.y>maxY)
        {
            objPos.y = maxY;
        }
        if (objPos.y<minY)
        {
            objPos.y = minY;
        }
        transform.position = objPos;
    }
    */
    public Transform top1, top2,bottom1,bottom2,bottom3;
    public float up = 1;
    public float down = 0.1f;
    public float maxY = -4.5f;
    public float minY = -8f;
    public Transform player;

    public const float maxYpos = -4.5f;
    public const float minYpos = -8f;
    public const float maxYSafe = -6.6f;
    public const float minYSafe = -6.3f;
    private Vector3 dir;
    private bool playeron=false;
    void OnMouseDown()
    {
        m_lastMousePos = Input.mousePosition;
    }
    void checkPlayer()
    {
        maxY = maxYpos;
        minY = minYpos;
        if (playeron)
        {
            if (player.position.x + 0.6f <= transform.position.x - 1.4f
                || player.position.x - 0.6f >= transform.position.x + 1.4f)
            {
                maxY = maxYSafe;
            }
        }
        RaycastHit2D bo1 = Physics2D.Raycast(bottom1.position, Vector2.down, down);
        if (bo1.collider)
        {
            if (bo1.collider.transform.position.y+0.5f > minY&&!bo1.collider.CompareTag("tile"))
            {
                minY = bo1.collider.transform.position.y+0.5f;
            }
        }
        RaycastHit2D bo2 = Physics2D.Raycast(bottom2.position, Vector2.down, down);
        if (bo2.collider)
        {
            if (bo2.collider.transform.position.y + 0.5f > minY && !bo2.collider.CompareTag("tile"))
            {
                minY = bo2.collider.transform.position.y + 0.5f;
            }
        }
        RaycastHit2D bo3 = Physics2D.Raycast(bottom3.position, Vector2.down, down);
        if (bo3.collider)
        {
            if (bo3.collider.transform.position.y + 0.5f > minY && !bo3.collider.CompareTag("tile"))
            {
                minY = bo3.collider.transform.position.y + 0.5f;
            }
        }       
    }
    void OnMouseDrag()
    {
        checkPlayer();
        dir = Input.mousePosition - m_lastMousePos;
        dir.x =dir.z= 0;
        transform.Translate(dir*speed*Time.deltaTime);
        if (transform.position.y > maxY)
        {
            transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
        }
        if (transform.position.y-0.5f < minY)
        {
            transform.position = new Vector3(transform.position.x, minY+0.5f, transform.position.z);
        }
        m_lastMousePos = Input.mousePosition;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {


        if (collision.transform.position.y - 0.5f >= transform.position.y + 0.5f && !collision.collider.CompareTag("tile"))
        {
            playeron = true;
            collision.transform.parent = transform;
        }
        
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.position.y - 0.5f >= transform.position.y + 0.5f&&!collision.collider.CompareTag("tile"))
        {
            playeron = true; 
            collision.transform.parent = transform;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("tile"))
        {
            playeron = false;
        }
    }


#pragma warning disable 649
    [SerializeField] [Range(0f, 1f)] float speed = 0.25f;
#pragma warning restore 649

    Vector3 m_lastMousePos;

}
