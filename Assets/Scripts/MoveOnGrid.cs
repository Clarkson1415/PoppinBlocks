using Assets.Scripts;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(GameColour))]
public class MoveOnGrid : MonoBehaviour
{
    private GameColour gameColour;

    private void Start()
    {
        gameColour = GetComponent<GameColour>();
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
    public void MoveBy(Vector2 moveBy)
    {
        // check if collider that is a not trigger is in movement direction
        // Cast the ray and ignore your own collider
        RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, moveBy, 1f);

        // If hit this gameobject dont count it. If all colliders in the way are triggers only. we can move in them. Return if they are all NOT Triggers.
        var externalHits = hits.Where(x => x.collider.gameObject != this.gameObject);
        var solidHits = externalHits.Where(x => !x.collider.isTrigger);

        if (!solidHits.Any())
        {
            this.transform.Translate(new Vector3(moveBy.x, moveBy.y, 0));
            this.SnapToGrid();
            return;
        }

        // TODO if solid hits has a unit in it of a different colour we need to push it and whatever is also in that direction.
        // as long as there are no blockages in that direction i.e. no walls. Other units can be pushed.
        // if block in direction want to move is a different colour. - i don't think have to check for that? because would already be popped?
        Debug.Log("do this stuff");

        // If the thing to move is an obstacle. then push it. and whatever is in front of it as long as no blockages.
        // Units and obstacles will be tagged Pushable.
        RaycastHit2D itemToPush = Physics2D.Raycast(this.transform.position, moveBy, 1f);
        
        // check if every object in a chain of that direction is pushable recursively? if so move all the objects by 1 unit by moveBy
        if (itemToPush.collider.gameObject.tag == "Pushable")
        {

        }
    }
}
