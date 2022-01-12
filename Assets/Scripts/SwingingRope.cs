using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwingingRope : MonoBehaviour
{
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
        if (Input.GetKeyDown(KeyCode.RightControl)) //Testing method
        {
            Debug.Log("Called Rope Attatch");
            Attatch();
        }
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

                //joint.distance = Vector2.Distance(hit, ConvertToV2(transform.position));
                joint.distance = hit.distance;
                ropeDistance = joint.distance;
                joint.enabled = true;
                
                UpdateRopeRenderer();
                isAttatched = true;
            }
        }
    }

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

    private void UpdateRope()
    {
        //Check whether old Point can be hit
        if (attatchedPoints.Count >= 2)
        {
            //Vector2 hitPointOld = RaycastToPoint(attatchedPoints[attatchedPoints.Count - 2]);
            Vector2 hitPointOld = RayFromTo(transform.position, attatchedPoints[attatchedPoints.Count - 2]).point;

            if(Vector2.Distance(hitPointOld, attatchedPoints[attatchedPoints.Count - 2]) <= 0.1f) //close enough to delete
            {
                anchor.transform.position = attatchedPoints[attatchedPoints.Count - 2];
                //joint.distance = Vector2.Distance(attatchedPoints[attatchedPoints.Count - 2], ConvertToV2(transform.position));
                attatchedPoints.RemoveAt(attatchedPoints.Count - 1);
                joint.distance = CalculateRemainingRope();
            }
        }

        //Raycast to last Point
        //Vector2 hitPoint = RaycastToPoint(attatchedPoints.Last());
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
            //joint.distance = Vector2.Distance(hitPoint, ConvertToV2(transform.position));
            joint.distance = CalculateRemainingRope();
        }
        
        //Create new Point + new ancor position
        //Update length
    }

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

    #region helper functions
    /// <summary>
    /// Cast a ray from Object Position to Mouse and returns hit Point. May return null
    /// </summary>
    /// <returns>null or hit point</returns>
    /*
    private Vector2 Raycast()
    {
        Vector2 playerPos = ConvertToV2(transform.position);
        Vector2 aimAt = ConvertToV2(Camera.main.ScreenToWorldPoint(Input.mousePosition)) - playerPos;
        return RaycastToPoint(aimAt);
    }

    private Vector2 RaycastToPoint(Vector2 vec2)
    {
        Vector2 playerPos = ConvertToV2(transform.position);
        Vector2 aimAt = vec2 - playerPos;
        return Physics2D.Raycast(playerPos, aimAt, ropeMaxDistance, grappableObjects).point;
    }
    */
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
