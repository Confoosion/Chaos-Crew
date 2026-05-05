using UnityEngine;

public class WallCheck : MonoBehaviour
{
    private bool isTouchingWall = false;

    void OnTriggerEnter2D(Collider2D collider)
    {
        isTouchingWall = true;
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        isTouchingWall = false;
    }

    public bool IsTouching()
    {
        return(isTouchingWall);
    }

}
