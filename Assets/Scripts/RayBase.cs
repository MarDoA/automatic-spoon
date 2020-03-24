using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayBase : MonoBehaviour
{
    [HideInInspector]
    public BoxCollider2D collider;
    public RaycastPoints raycastPoints;

    public const float skinWidth = .015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;
    public int jumpRayCount;
    public LayerMask collisionMask;
    [HideInInspector]
    public float horizonalRaySpace;
    [HideInInspector]
    public float verticalRaySpace;   

    public virtual void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        jumpRayCount = verticalRayCount * 2;
        CalculateRaySpace();
    }
    public void CalculateRaySpace()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);
        horizonalRaySpace = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpace = bounds.size.x / (verticalRayCount - 1);

    }
    public struct RaycastPoints
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
    public void UpdateRaycastPoints()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);
        raycastPoints.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastPoints.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastPoints.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastPoints.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }
}
