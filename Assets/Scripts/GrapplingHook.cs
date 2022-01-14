using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    /// <summary>
    /// A grappling hook that can be used to attach this gameObject to any collider specified in grappableObjects.
    /// This grappling hook pulls this gameObject to the position of the grappling hook after grappling.
    /// </summary>
    public GameObject anchor;
    public DistanceJoint2D joint;
    public LineRenderer ropeRenderer;
    public float grappleSpeed = 1.0f;

    private SpriteRenderer anchorSprite;

    public LayerMask grappableObjects;
    public float ropeMaxDistance = 10f;

    public bool isAttatched = false;

    private void Awake()
    {
        joint.enabled = false;
        anchorSprite = anchor.GetComponent<SpriteRenderer>();
        anchorSprite.enabled = false;
        ropeRenderer.enabled = true;
    }

    void Update()
    {
        if (isAttatched)
        {
            UpdateRopeRenderer();
            PullIn();
        }
    }

    /// <summary>
    /// Updates the rendered grappling hook connection
    /// </summary>
    private void UpdateRopeRenderer()
    {
        Vector3[] positions = {transform.position, anchor.transform.position};
        ropeRenderer.SetPositions(positions);
    }

    /// <summary>
    /// Used to attatch the gameObject.
    /// 
    /// Uses a raycast from the gameObject to the current mouse position with ropeMaxDistance as length.
    /// Releases current cast grapple and attaches to new position.
    /// If no attachable Object is found it just releases the grapple.
    /// </summary>
    public void Attatch()
    {
        if (isAttatched)
        {
            Release();
        }
        Vector2 AimAt = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y) - GetPlayerPos();

        RaycastHit2D hitResult =  Physics2D.Raycast(GetPlayerPos(), AimAt, ropeMaxDistance, grappableObjects);
        if(hitResult.collider == null) //Disable
        {
            anchor.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isAttatched = false;
            return;
        }
        else //Enable
        {
            anchor.transform.position = hitResult.point;
            joint.distance = Vector2.Distance(GetPlayerPos(), hitResult.point);
            joint.enabled = true;
            anchorSprite.enabled = true;
        }
        
        ropeRenderer.enabled = true;
        isAttatched = true;
    }

    public void Release()
    {
        joint.enabled = false;
        anchorSprite.enabled = false;
        ropeRenderer.enabled = false;
        isAttatched = false;
    }
    /// <summary>
    /// Used to decrease current DistanceJoint2D distance. This results into the player being pulled towards the anchor point.
    /// </summary>
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
