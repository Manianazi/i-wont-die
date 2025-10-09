using UnityEngine;

public class movimiento : MonoBehaviour
{
    //timers
    [SerializeField] private float LastOnGroundTime;
    [SerializeField] private float LastOnWallTime;
    [SerializeField] private float LastOnWallRightTime;
    [SerializeField] private float LastOnWallLeftTime;
    [SerializeField] private float LastPressedJumpTime;
    

    //Referencias
    private Rigidbody2D rb;
    private LayerMask ground;
    private colisiones coll;

    //Variables
     private float wallCoyoteTime = 0.12f;
     private float jumpBufferTime = 0.12f;
    [SerializeField] private float wallforceJump;
    [SerializeField] private float slideSpeed = 2.5f;
    [SerializeField] private float fallmultiplier = 2.5f;
    [SerializeField] private float lowjumpmultiplier = 2f;
    [SerializeField] private float velocity;
    [SerializeField] private float forceJump = 2.5f;
    [SerializeField] private float runAccelAmount;
    [SerializeField] private float runDecclAmount;
    [SerializeField] private int lastWallJumpDir;
    private float movement;
    private float moveInputx;
    private float moveInputy;
    private Vector2 dir;
    private Vector2 force;
    [SerializeField] private Vector2 wallJumpForce;
    //Booleanos

    private bool Jump;
    private bool isJumping;
    public bool wallJumped;
    public bool isWallJumping;
    public bool isSliding;

    [SerializeField] private bool canWallJump;  
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<colisiones>();

    }  

    // Update is called once per frame
    void Update()
    {
        LastOnGroundTime -= Time.deltaTime;
		LastOnWallTime -= Time.deltaTime;
		LastOnWallRightTime -= Time.deltaTime;
		LastOnWallLeftTime -= Time.deltaTime;
		LastPressedJumpTime -= Time.deltaTime;



      moveInputx = Input.GetAxisRaw("Horizontal");
      moveInputy = Input.GetAxisRaw("Vertical");
       
        Vector2 dir = new Vector2(moveInputx,moveInputy);

       if(coll.enPared && !coll.enPiso || !isWallJumping)
        {
            wallSlide();

        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && (coll.enPiso == true))
        {
            LastPressedJumpTime = jumpBufferTime;
            isSliding = false;
            jump();


        }
        if (coll.enPared && (coll.enPiso == false)  && (Input.GetKeyDown(KeyCode.UpArrow)))
        {
            isSliding = false;
            isWallJumping = true;
            int Dir = coll.onRightWall ? -1 : 1;
            wallJump(Dir);

        }

            if (rb.linearVelocityY < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallmultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocityY > 0 && !(Input.GetKeyDown(KeyCode.UpArrow)))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowjumpmultiplier - 1) * Time.deltaTime;
        }
           LastOnWallTime = Mathf.Max(LastOnWallLeftTime,LastOnWallRightTime);

        if (coll.onRightWall)
        {
            LastOnWallRightTime = wallCoyoteTime;
        }
        if (coll.onLeftWall)
        {
            LastOnWallLeftTime = wallCoyoteTime;
        }

    }



    private void wallJump(int Dir)
    { 
    isSliding = false;
    LastOnGroundTime = 0;
    LastPressedJumpTime = 0;
    LastOnWallRightTime = 0;
    LastOnWallLeftTime = 0;



     force = new Vector2(wallJumpForce.x, wallJumpForce.y);

    force.x *= Dir;


    if (Mathf.Sign(rb.linearVelocity.x) != Mathf.Sign(force.x))
    {
        force.x -= rb.linearVelocity.x;
    }


    if (rb.linearVelocity.y < 0)
    {
        force.y -= rb.linearVelocity.y;
    }
    rb.AddForce(force* wallforceJump, ForceMode2D.Impulse);

}






    private void wallSlide() 
    {
        isSliding = true;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -slideSpeed);

    }
    private void jump()
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        rb.AddForce(Vector2.up * forceJump, ForceMode2D.Impulse);

        isJumping = true;
       
    }

    private void FixedUpdate()
    {
   
        float targetSpeed =moveInputx * velocity;

      targetSpeed = Mathf.Lerp(rb.linearVelocity.x, targetSpeed, 0.02f);
       float speedDiff = targetSpeed - rb.linearVelocity.x;            
      float accelRate;
      accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount : runDecclAmount;
         
      movement = speedDiff * accelRate;
     
        rb.linearVelocity = new Vector2(movement, rb.linearVelocityY);

       

       
    }
    private bool CanWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!isWallJumping ||
             (LastOnWallRightTime > 0 && lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && lastWallJumpDir == -1));
    }
}

