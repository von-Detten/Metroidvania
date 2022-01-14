using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwingingRope : MonoBehaviour
{
    /// <summary>
    /// A rope that can be used to attach this gameObject to any collider specified in grappableObjects.
    /// This rope automatically wraps and unwraps around colliders while keeping its total lenght.
    /// </summary>
    public Transform anchor;
    public DistanceJoint2D joint;
    public LineRenderer ropeRenderer;

    public LayerMask grappableObjects;
    public float ropeMaxDistance;

    public List<Vector2> attatchedPoints = new List<Vector2>();
    public bool isAttatched = false;
    private float ropeDistance;
    
    void Start()
    {
        joint.enabled = false;
        anchor.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isAttatched)
        {
            UpdateRopeRenderer();
        }
    }

    private void FixedUpdate()
    {
        if (isAttatched)
        {
            UpdateRope();
        }
    }

    /// <summary>
    /// Used to try to attach the rope.
    /// 
    /// Uses a raycast from the gameObject to the current mouse position with ropeMaxDistance as length.
    /// Releases current cast rope and attaches to new position.
    /// If no attachable Object is found it just releases the rope.
    /// </summary>
    public void Attatch()
    {
        if (isAttatched)
        {
            Release();
        }
        else
        {
            RaycastHit2D hit = RayPlayerToMouse();

            if (hit.transform == null)
            {
                Release();
                return;
            }
            else
            {
                anchor.gameObject.SetActive(true);
                anchor.transform.position = hit.point;
                attatchedPoints.Add(hit.point);

                joint.distance = hit.distance;
                ropeDistance = joint.distance;
                joint.enabled = true;
                
                UpdateRopeRenderer();
                isAttatched = true;
            }
        }
    }

    /// <summary>
    /// Used to releas current attached rope.
    /// </summary>
    public void Release()
    {
        if (!isAttatched)
        {
            return;
        }

        anchor.gameObject.SetActive(false);
        joint.enabled = false;
        attatchedPoints.Clear();
        ropeRenderer.enabled = false;
        isAttatched = false;
    }


    /// <summary>
    /// Used to calculate current wrapping points of the rope.
    /// 
    /// Tracks current attatched points in attatchedPoints.
    /// </summary>
    private void UpdateRope()
    {
        //Check whether old Point can be hit
        if (attatchedPoints.Count >= 2)
        {
            Vector2 hitPointOld = RayFromTo(transform.position, attatchedPoints[attatchedPoints.Count - 2]).point;

            if(Vector2.Distance(hitPointOld, attatchedPoints[attatchedPoints.Count - 2]) <= 0.1f) //close enough to delete
            {
                anchor.transform.position = attatchedPoints[attatchedPoints.Count - 2];
                attatchedPoints.RemoveAt(attatchedPoints.Count - 1);
                joint.distance = CalculateRemainingRope();
            }
        }

        //Raycast to last Point
        Vector2 hitPoint = RayFromTo(transform.position, attatchedPoints.Last()).point;
        //Check Distance between Points
        if (hitPoint == null)
        {
            Debug.LogError("Hit null while trying to hit last attatched point");
            return;
        }
        if (Vector2.Distance(hitPoint, attatchedPoints.Last()) <= 0.1f) //still similar
        {
            return;
        }
        else //Add new Rope segment
        {
            attatchedPoints.Add(hitPoint);
            anchor.transform.position = hitPoint;
            joint.distance = CalculateRemainingRope();
        }
    }

    /// <summary>
    /// Used to set the current rope wrappinng points, the initial attached point and current gameObject position for the LineRenderer to render the rope.
    /// </summary>
    private void UpdateRopeRenderer()
    {
        ropeRenderer.enabled = true;
        List<Vector3> ropePoints = new List<Vector3>();
        foreach (Vector2 item in attatchedPoints)
        {
            ropePoints.Add(new Vector3(item.x, item.y, 0));
        }
        ropePoints.Add(transform.position);

        ropeRenderer.positionCount = ropePoints.Count;
        ropeRenderer.SetPositions(ropePoints.ToArray());
    }

    #region internal helper functions
    /// <summary>
    /// Tracks rope length
    /// </summary>
    /// <returns>Rope length after wrapping</returns>
    private float CalculateRemainingRope()
    {
        if(attatchedPoints.Count >= 2)
        {
            float helper = ropeDistance;
            for (int i = 1; i < attatchedPoints.Count; i++)
            {
                helper -= Vector2.Distance(attatchedPoints[i], attatchedPoints[i - 1]);
            }
            return helper;
        }
        else
        {
            return ropeDistance;
        }
    }

    private RaycastHit2D RayPlayerToMouse()
    {
        return RayFromTo(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private RaycastHit2D RayFromTo(Vector2 from, Vector2 to)
    {
        Vector2 aimAt = to - from;
        return Physics2D.Raycast(from, aimAt, ropeMaxDistance, grappableObjects);
    }

    private Vector2 ConvertToV2 (Vector3 v3)
    {
        return new Vector2(v3.x, v3.y);
    }
    #endregion
}
