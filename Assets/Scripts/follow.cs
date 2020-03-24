using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class follow : MonoBehaviour
{
    public Transform child;
    public float z;
    // Update is called once per frame
    void Update()
    {
        child.position = new Vector3(transform.position.x, transform.position.y, child.position.z);
    }
    private void FixedUpdate()
    {
        child.position = new Vector3(transform.position.x, transform.position.y, child.position.z);
    }
}
