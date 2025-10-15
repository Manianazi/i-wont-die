using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class specialMoves : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D rb;
    private movimiento mov;
    private colisiones coll;
    public Transform playerobj;
    public Transform orientation;

    [Header("Rolling")]
    private float maxRolltime;
    public float rollForce;
    public float rollTimer ;
    private float rollDuration = 0.3f;
    public bool rolling;
    public float rollYScale;
    private float startRollYScale;
    private float momentumTimer;
    private float momentumDuration;
    [SerializeField] private bool canRoll = true;
    [SerializeField] private bool canApplyImpulse;
    [SerializeField] private float rollSpeed;
    private bool maintainRollVelocity = false;
    private bool wasGrounded;
  


    [Header("Inputs")]
    public KeyCode rollKey = KeyCode.LeftShift;
    public Vector2 dir;
    private float inputx;
    private float inputy;

    void Start()
    {
        mov = GetComponent<movimiento>();
        coll = GetComponent<colisiones>();
        playerobj = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        startRollYScale = playerobj.localScale.y;
    }


    void Update()
    {
        inputx = Input.GetAxisRaw("Horizontal");
        inputy = Input.GetAxisRaw("Vertical");
        dir = new Vector2(inputx, inputy);

      

        if (rollTimer == 0)
        {
            canRoll = false;
        }
        if (coll.enPiso && !wasGrounded)
        {
            canRoll = true;
        }
        wasGrounded = coll.enPiso;

        if (Input.GetKey(rollKey) && (inputx != 0))
        {
            startRoll();
        }

        if (Input.GetKeyUp(rollKey) && (rolling))
        {
            stopRoll();
        }
        if (maintainRollVelocity)
        {
            rollTimer -= Time.deltaTime;
            if (rollTimer <= 0f)
            {
                maintainRollVelocity = false;
            }
        }
    }
    private void startRoll()
    {
        Debug.Log("rolling");
        rolling = true;
       
        //if (rolling) return;
      
        playerobj.localScale = new Vector2(playerobj.localScale.x, rollYScale);

        if (coll.enPiso)
        {
            rb.AddForce(Vector2.down * 5f, ForceMode2D.Impulse);

        }
        if (coll.enPiso && canRoll && rolling)
        {

            
            rollTimer = rollDuration;
            rollTimer -= Time.deltaTime;
            float rollDir = Mathf.Sign(inputx);
            rollSpeed = rollDir * rollForce;
            
            canRoll = false;
            maintainRollVelocity = true;
            //rb.AddForce(dir * rollForce, ForceMode2D.Impulse);
            //rb.linearVelocity = new Vector2(rollSpeed, rb.linearVelocity.y);
        }
    }

    private void rollMovement()
    {

    }   
    private void stopRoll()
    {
        rolling = false;

        if (!rolling)
        {
            playerobj.localScale = new Vector2(playerobj.localScale.x, startRollYScale);

        }
    }
    private void FixedUpdate()
    {
        if (maintainRollVelocity)
        {

            rb.linearVelocity = new Vector2(rollSpeed, rb.linearVelocity.y);
        }
        if (rolling)
        {
            rollMovement();
        }

    }
   

}


