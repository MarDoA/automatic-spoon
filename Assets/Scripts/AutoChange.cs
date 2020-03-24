using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoChange : MonoBehaviour
{
    Auto controll;

    private void Start()
    {
        controll = GetComponentInParent<Auto>();
    }
    private void OnMouseDown()
    {
        controll.Lever();
    }
}
