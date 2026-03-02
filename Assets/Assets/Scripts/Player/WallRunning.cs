using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerController))]
public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask wall;
    public float wallRunForce = 20f;
    public float wallClimbSpeed = 5f;
    public float maxWallRunTime = 1.5f;

    [Header("Detection")]
    public float wallCheckDistance = 0.7f;
    public float minJumpHeight = 1.5f;

    [Header("Wall Jump")]
    public float wallJumpUpForce = 7f;
    public float wallJumpSideForce = 8f;

    [Header("References")]
    public Transform orientation;

    private Rigidbody rb;
    private PlayerController pm;

    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;

    private bool wallLeft;
    private bool wallRight;

    private float wallRunTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerController>();

        if (orientation == null)
            orientation = pm.cameraTransform;
    }

    void Update()
    {
        CheckForWall();
        StateMachine();
        if (pm.wallrunning && Input.GetButtonDown("Jump"))
        {
            PerformWallJump();
        }
    }

    void FixedUpdate()
    {
        if (pm.wallrunning)
            WallRunMovement();
    }

    void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, wall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, wall);
    }

    bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight);
    }

    void StateMachine()
    {
        if (!pm.enableWallRun)
            return;

        float verticalInput = Input.GetAxisRaw("Vertical");

        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround())
        {
            if (!pm.wallrunning)
                StartWallRun();
        }
        else
        {
            if (pm.wallrunning)
                StopWallRun();
        }
    }

    void StartWallRun()
    {
        pm.wallrunning = true;
        wallRunTimer = maxWallRunTime;
        rb.useGravity = false;
        pm.jumpCount = 0;
    }

    void WallRunMovement()
    {
        wallRunTimer -= Time.fixedDeltaTime;

        if (wallRunTimer <= 0f)
        {
            StopWallRun();
            return;
        }

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up);

        if (Vector3.Dot(wallForward, orientation.forward) < 0)
            wallForward = -wallForward;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        rb.AddForce(-wallNormal * 100f, ForceMode.Force);
    }

    void PerformWallJump()
    {
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 jumpDirection = wallNormal + Vector3.up;

        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;

        rb.AddForce(
            wallNormal * wallJumpSideForce +
            Vector3.up * wallJumpUpForce,
            ForceMode.VelocityChange
        );

        pm.jumpCount = 1;

        StopWallRun();
    }

    void StopWallRun()
    {
        pm.wallrunning = false;
        rb.useGravity = true;
    }
}