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
    public Vector2 tama�o;
    public Vector2 tama�oabj;
    void Start()
    {
        
    }

 
    void Update()
    {
       enPiso =  Physics2D.OverlapBox((Vector2)transform.position + bottomOffset,tama�oabj,0,ground);

       enPared = Physics2D.OverlapBox((Vector2)transform.position + rightOffset,tama�o,0,ground) ||
                 Physics2D.OverlapBox((Vector2)transform.position + leftOffset,tama�o,0,ground);

        onRightWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, tama�o,0, ground);
        onLeftWall = Physics2D.OverlapBox((Vector2)transform.position + leftOffset, tama�o,0, ground);

        wallSide = onRightWall ? -1 : 1;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        Gizmos.DrawWireCube((Vector2)transform.position + bottomOffset, tama�oabj);
        Gizmos.DrawWireCube((Vector2)transform.position + rightOffset, tama�o);
        Gizmos.DrawWireCube((Vector2)transform.position + leftOffset, tama�o);
    }
}



