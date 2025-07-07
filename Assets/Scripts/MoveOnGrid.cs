using Assets.Scripts;
using UnityEngine;

public class MoveOnGrid : MonoBehaviour
{
    public bool IsTouchingAnother => false; // get if this guy is in square next to another of same colour.

    public GuyColour GuyColour;

    private void Start()
    {
        this.SnapToGrid();
    }

    private void SnapToGrid()
    {
        this.transform.position = new Vector3(Mathf.Round(this.transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(this.transform.position.z));
    }

    // togo in unit class
    // TODO detect when player next to guy. if all touching then pop. if player pushing then push.
    
    /// <summary>
    /// Move unit by offset.
    /// </summary>
    /// <param name="deltaX"></param>
    /// <param name="deltaY"></param>
    public void MoveBy(int deltaX, int deltaY)
    {

    }

    private void Pop()
    {

    }
}
