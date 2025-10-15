using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine;

public class movimiento : MonoBehaviour
{
    //timers
     private float LastOnGroundTime;
     private float LastOnWallTime;
     private float LastOnWallRightTime;
     private float LastOnWallLeftTime;
     private float LastPressedJumpTime;
     private float wallJumpStartTime;
     private float wallJumpTime = 0.12f;
    //Referencias
    private Rigidbody2D rb;
    private LayerMask ground;
    private colisiones coll;
    private specialMoves sm;
    private GameObject spawn;
    private GameObject cops;
    private GameObject win;
    [SerializeField] private death death;
    //Variables
    public float walljumplerp;
    private float coyote = 0.12f;
    
    private float jumpBufferTime = 0.12f;
    private float slideSpeed = 2.5f;
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
    [SerializeField] private float wallforceJump;
    [SerializeField] private float wallJumpForcex;
    [SerializeField] private float wallJumpForcey;
    [SerializeField] private Vector2 wallJumpForce;

    //Booleanos
    private bool isJumpFalling;
    private bool isJumpCut;
    private bool jump;
    private bool isJumping;
    public bool wallJumped;
    public bool isWallJumping;
    public bool isSliding;
    private bool isFacingRight = true;
    
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<colisiones>();
       sm = GetComponent<specialMoves>();
        spawn = GameObject.FindWithTag("spawn");
        cops = GameObject.FindWithTag("cops");
        GameObject enemyGO = GameObject.Find("enemy");
        win = GameObject.FindWithTag("win");
       
    }

    // Update is called once per frame
    void Update()
    {

        //timers
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;
        LastPressedJumpTime -= Time.deltaTime;
        
        
        //inputs
        moveInputx = Input.GetAxisRaw("Horizontal");
        moveInputy = Input.GetAxisRaw("Vertical");


        Vector2 dir = new Vector2(moveInputx, moveInputy);

        //chequeos 
        if (coll.enPiso)
            LastOnGroundTime = coyote;
        
        if (coll.onRightWall)
            LastOnWallRightTime = coyote;
        
        if (coll.onLeftWall)
            LastOnWallLeftTime = coyote;

        if (coll.enPared && !coll.enPiso && !isWallJumping)
        {
            WallSlide();

        }

        //colision piso
        if ( (Input.GetKeyDown(KeyCode.Space)))
        {

            LastPressedJumpTime = jumpBufferTime;
            isSliding = false;
            


        }
        //colision paredes
        if (coll.enPared && (coll.enPiso == false) && (Input.GetKeyDown(KeyCode.Space)) )
        {
            isSliding = false;
           
            

        }
        //jump cut

        if (rb.linearVelocityY < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallmultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocityY > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowjumpmultiplier - 1) * Time.deltaTime;
        }
       
        
        

        if (moveInputx >0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInputx < 0 && isFacingRight)
        {
            Flip(); 
        }
        //jump cheks

        if (isJumping && rb.linearVelocity.y < 0)
        {
            isJumping = false;
        }
       
        if (!isWallJumping)
        {
            isJumpFalling = true;

        }
            
        if (isWallJumping && Time.time - wallJumpStartTime > wallJumpTime)
        {
            isWallJumping = false;
        }
        if (!isJumping)
            isJumpFalling = false;

        if (CanJump() && LastPressedJumpTime > 0 && sm.rolling == false)
        {
            isJumping = true;
            isWallJumping = false;
            isJumpCut = false;
            isJumpFalling = false;
            Jump();
        }
        // walljump cheks
        else if (CanWallJump() && LastPressedJumpTime > 0)
        {
            isWallJumping = true;
            isJumping = false;
            isJumpCut = false;
            isJumpFalling = false;
            wallJumpStartTime = Time.time;
            
            int Dir = coll.onRightWall ? -1 : 1; WallJump(Dir);
        }
        LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);


    }


    private void WallJump(int Dir)
    {

        isWallJumping = true;


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


        rb.AddForce(force * wallforceJump, ForceMode2D.Impulse);


        //Invoke("timewallJumping",0.8f);
        //if (isWallJumping)
        //{
        //    rb.linearVelocity = new Vector2(wallJumpForcex * Dir, wallJumpForcey);
        //}



    }
    private  void TimewallJumping()
    {
        isWallJumping = false;
    }





    private void WallSlide() 
    {
        isSliding = true;
        if (!isWallJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -slideSpeed);
        }
            

    }
    private void Jump()
    {
        Debug.Log("saltooo");
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        rb.AddForce(Vector2.up * forceJump, ForceMode2D.Impulse);

        isJumping = true;
       
    }
      
    
    private void Run(float lerp)
    {
      
        isSliding = false;

        if (!isWallJumping)
        {
            float targetSpeed = moveInputx * velocity;

            targetSpeed = Mathf.Lerp(rb.linearVelocity.x, targetSpeed, lerp);

            float speedDiff = targetSpeed - rb.linearVelocity.x;
            float accelRate;

            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount : runDecclAmount;

            movement = speedDiff * accelRate;

            rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
        } 
        

    }
    private void FixedUpdate()
    {

        if ((!coll.enPared) || (coll.enPiso))
        {
            Run(1);
        }
    }
    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !isJumping;
    }
    private bool CanWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!isWallJumping ||
             (LastOnWallRightTime > 0 && lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && lastWallJumpDir == -1));
    }
    public bool CanSlide()
    {
        if (LastOnWallTime > 0 && !isJumping && !isWallJumping && LastOnGroundTime <= 0)
            return true;
        else
            return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("spawn"))
        {
            death.spawn = true;
        }
            if (collision.gameObject.CompareTag("cops"))
        {
            Respawn();
        }
            if ( collision.gameObject.CompareTag("win"))
        {
            SceneManager.LoadScene("Main Menu");
        }
        
    }
    private void Respawn()
    {
        SceneManager.LoadScene("Main Menu");
        Debug.Log("perdiste jaja");
    }    
}

