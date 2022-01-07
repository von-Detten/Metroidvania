using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public GameObject ropeHingeAnchor;
    public DistanceJoint2D ropeJoint;
    public LineRenderer ropeRenderer;

    //public PlayerMovement playerMovement;
    private bool isRopeAttatched;
    private Rigidbody2D ropeHingeAnchorRb;
    private SpriteRenderer ropeHingeAnchorSprite;

    public LayerMask ropeLayerMask;
    public float ropeMaxCastDistance = 20f;

    private bool isGrappled = false;

    private void Awake()
    {
        ropeJoint.enabled = false;
        ropeHingeAnchorRb = ropeHingeAnchor.GetComponent<Rigidbody2D>();
        ropeHingeAnchorSprite = ropeHingeAnchor.GetComponent<SpriteRenderer>();
        ropeHingeAnchorSprite.enabled = false;
        ropeRenderer.enabled = true;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Grapple();
            Debug.Log("Grappled");
        }
        if (isGrappled)
        {
            UpdateRopeRenderer();
        }
    }

    private void UpdateRopeRenderer()
    {
        Vector3[] positions = {transform.position, ropeHingeAnchor.transform.position};
        ropeRenderer.SetPositions(positions);
    }

    private void Grapple()
    {
        if (isGrappled)
        {
            ropeJoint.enabled = false;
            ropeHingeAnchorSprite.enabled = false;
            ropeRenderer.enabled = false;
            isGrappled = false;
            
        }
        Vector2 AimAt = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y) - GetPlayerPos();

        RaycastHit2D hitResult =  Physics2D.Raycast(GetPlayerPos(), AimAt, ropeMaxCastDistance, ropeLayerMask);
        if(hitResult.collider == null) //Disable
        {
            isGrappled = false;
            return;
        }
        else //Enable
        {
            ropeHingeAnchorRb.transform.position = hitResult.point;
            ropeJoint.distance = Vector2.Distance(GetPlayerPos(), hitResult.point);
            ropeJoint.enabled = true;
            ropeHingeAnchorSprite.enabled = true;
        }
        
        ropeRenderer.enabled = true;
        isGrappled = true;

    }

    private Vector2 GetPlayerPos()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }
}
