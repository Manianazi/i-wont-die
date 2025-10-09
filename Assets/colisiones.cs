using UnityEngine;
using UnityEngine.UIElements;

public class colisiones : MonoBehaviour
{
    public bool enPared;
    public bool enPiso;
    public bool onRightWall;
    public bool onLeftWall;
    public int wallSide;
    public LayerMask ground;

    public Vector2 bottomOffset, rightOffset, leftOffset;

   

    private Color debugCollisionColor = Color.red;
    public Vector2 tamaño;
    public Vector2 tamañoabj;
    void Start()
    {
        
    }

 
    void Update()
    {
       enPiso =  Physics2D.OverlapBox((Vector2)transform.position + bottomOffset,tamañoabj,0,ground);

       enPared = Physics2D.OverlapBox((Vector2)transform.position + rightOffset,tamaño,0,ground) ||
                 Physics2D.OverlapBox((Vector2)transform.position + leftOffset,tamaño,0,ground);

        onRightWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, tamaño,0, ground);
        onLeftWall = Physics2D.OverlapBox((Vector2)transform.position + leftOffset, tamaño,0, ground);

        wallSide = onRightWall ? -1 : 1;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        Gizmos.DrawWireCube((Vector2)transform.position + bottomOffset, tamañoabj);
        Gizmos.DrawWireCube((Vector2)transform.position + rightOffset, tamaño);
        Gizmos.DrawWireCube((Vector2)transform.position + leftOffset, tamaño);
    }
}



