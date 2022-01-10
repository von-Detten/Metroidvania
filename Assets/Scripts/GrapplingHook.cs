using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public GameObject anchor;
    public DistanceJoint2D joint;
    public LineRenderer ropeRenderer;
    public float grappleSpeed = 1.0f;

    //public PlayerMovement playerMovement;
    private Rigidbody2D anchorRb;
    private SpriteRenderer anchorSprite;

    public LayerMask grappableObjects;
    public float ropeMaxDistance = 10f;

    private bool isGrappled = false;

    private void Awake()
    {
        joint.enabled = false;
        anchorRb = anchor.GetComponent<Rigidbody2D>();
        anchorSprite = anchor.GetComponent<SpriteRenderer>();
        anchorSprite.enabled = false;
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
            PullIn();
        }
    }

    private void UpdateRopeRenderer()
    {
        Vector3[] positions = {transform.position, anchor.transform.position};
        ropeRenderer.SetPositions(positions);
    }

    private void Grapple()
    {
        if (isGrappled)
        {
            joint.enabled = false;
            anchorSprite.enabled = false;
            ropeRenderer.enabled = false;
            isGrappled = false;
            
        }
        Vector2 AimAt = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y) - GetPlayerPos();

        RaycastHit2D hitResult =  Physics2D.Raycast(GetPlayerPos(), AimAt, ropeMaxDistance, grappableObjects);
        if(hitResult.collider == null) //Disable
        {
            isGrappled = false;
            return;
        }
        else //Enable
        {
            anchorRb.transform.position = hitResult.point;
            joint.distance = Vector2.Distance(GetPlayerPos(), hitResult.point);
            joint.enabled = true;
            anchorSprite.enabled = true;
        }
        
        ropeRenderer.enabled = true;
        isGrappled = true;

    }

    private void PullIn()
    {
        float newDistance = joint.distance - 0.01f * grappleSpeed;
        if(newDistance >= 0)
        {
            joint.distance = newDistance;
        }
    }

    private Vector2 GetPlayerPos()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }
}
