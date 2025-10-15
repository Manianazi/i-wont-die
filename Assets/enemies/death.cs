using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class death : MonoBehaviour
{
    [Header("References")]
    private Transform trans;
    private movimiento movimiento;
    private specialMoves specialMoves;
    private Camera cam;
    private GameObject pj;


    [Header("Variables")]
    private Camera cameraposition;
    public Vector2 topLeft;
    public Vector2 topRight;
    public Vector2 bottomLeft;
    public Vector2 bottomRight;
    private float distance;
    private Vector2 direccion;

    public  bool spawn = false;

    public float lookTimelimit = 20f;
    [SerializeField] public float lookTime;
    [SerializeField] private float lookSpeed;

    [SerializeField] private float scanDuration = 6f;
    [SerializeField] private float searchDuration = 6f;
    [SerializeField] private float scanSpeed = 0.3f;

    [SerializeField] private float chaseRange = 10f;
    public float loseRange = 12f;
    public float searchRadius = 3f;
    public float searchSpeed;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float jitterAmount = 0.15f;

    [SerializeField] private int state = 0; // 0=scan,1=search,2=chase
    
    private float stateTimer = 0f;

    Vector3[] corners;
    int currentCorner = 0;

    void Start()
    {
        cam = Camera.main;
        pj = GameObject.FindWithTag("Player");
        state = 0;
        stateTimer = Time.time + scanDuration;

    }
    void Update()
    {


        if (cam == null) return;

        //Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        //Vector3 bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
        //Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, cam.nearClipPlane));
        //Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));
        corners = new Vector3[4];
        corners[0] = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane + 10f)); // bottomLeft
        corners[1] = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane + 10f)); // bottomRight
        corners[2] = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane + 10f)); // topRight
        corners[3] = cam.ViewportToWorldPoint(new Vector3(0, 1, cam.nearClipPlane + 10f)); // topLeft

        distance = Vector2.Distance(transform.position, pj.transform.position);
        direccion = (pj.transform.position - transform.position).normalized;
        if (spawn == true)
        {
            if (state == 0) // SCAN
            {
                ScanBehaviour();
                if (Time.time > stateTimer)
                {
                    state = 1;
                    stateTimer = Time.time + searchDuration;
                }
            }
            else if (state == 1) // SEARCH
            {
                SearchBehaviour();
                if (distance <= chaseRange)
                {
                    state = 2;
                }
            }
            else // state == 2 CHASE
            {
                ChaseBehaviour();

                if (distance > loseRange)
                {
                    // volvemos a buscar después de X segundos
                    state = 1;
                    stateTimer = Time.time + searchDuration;
                }
            }
        }
        Debug.DrawLine(bottomLeft, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, topRight, Color.green);
        Debug.DrawLine(topRight, topLeft, Color.blue);
        Debug.DrawLine(topLeft, bottomLeft, Color.yellow);
    }

    void ScanBehaviour()
    {
        Vector3 target = corners[currentCorner] + RandomJitter();
        transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * scanSpeed);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            currentCorner = (currentCorner + 1) % corners.Length;
        }
    }
    Vector3 currentSearchTarget = Vector3.zero;
    float lastSearchPick = 0f;
    void SearchBehaviour()
    {
        if (Time.time - lastSearchPick > 1.0f)
        {
            Vector3 center = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, cam.nearClipPlane + 10f));
            float rx = (Random.value - 0.5f) * 2f;
            float ry = (Random.value - 0.5f) * 2f;
            currentSearchTarget = center + new Vector3(rx, ry, 0f) * searchRadius;
            lastSearchPick = Time.time;
        }
        transform.position = Vector3.MoveTowards(transform.position, currentSearchTarget + RandomJitter(), Time.deltaTime * searchSpeed);

    }
    void ChaseBehaviour()
    {
        Vector3 playerPos = pj.transform.position;
        playerPos.z = transform.position.z;
        
        transform.position = Vector3.MoveTowards(transform.position, playerPos, Time.deltaTime * chaseSpeed);
    }
    Vector3 RandomJitter()
    {
        return new Vector3(
            Random.Range(-jitterAmount, jitterAmount),
            Random.Range(-jitterAmount, jitterAmount),
            0f
        );
    }
}
