using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#nullable enable

public class MoveOnGrid : MonoBehaviour
{
    private void Start()
    {
        this.SnapToGrid();
    }

    private void SnapToGrid()
    {
        this.transform.position = new Vector3(Mathf.Round(this.transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(this.transform.position.z));
    }

    private void SnapToGrid(Transform transToSnap)
    {
        transToSnap.position = new Vector3(Mathf.Round(transToSnap.position.x), Mathf.Round(transToSnap.position.y), Mathf.Round(transToSnap.position.z));
    }

    private float RayCastDist => this.GetComponent<ColouredUnit>().RaycastDistance;

    /// <summary>
    /// Move unit by offset. Returns true if successful.
    /// </summary>
    public bool TryMoveBy(Vector2 moveBy)
    {
        // check if collider that is a not trigger is in movement direction
        // Cast the ray and ignore your own collider
        RaycastHit2D itemInFront = Physics2D.Raycast(this.transform.position, moveBy, RayCastDist);

        // if item in front is a blockage return
        if (itemInFront.collider != null && !itemInFront.collider.isTrigger && !itemInFront.collider.gameObject.CompareTag("Pushable"))
        {
            return false;
        }

        // If there is no item in front move and return.
        if (itemInFront.collider == null || itemInFront.collider.isTrigger)
        {
            this.transform.Translate(new Vector3(moveBy.x, moveBy.y, 0));
            this.SnapToGrid();
            return true;
        }

        // check if every object in a chain of that direction is pushable recursively? if so move all the objects by 1 unit by moveBy
        // Get all adjacent pushable items in the direction.
        // If there is a collider thats not "Pushable" or an IsTrigger dont push.
        var adjacentPushablesInDirection = new List<Collider2D>
        {
            itemInFront.collider
        };

        while (true)
        {
            var next = Physics2D.Raycast(adjacentPushablesInDirection.Last().transform.position, moveBy, RayCastDist);

            // If empty space or blockage found return.
            // If empty space
            if (next.collider == null)
            {
                break;
            }
            else if (next.collider.isTrigger) 
            {
                break;
            }
            else if (next.collider != null && !next.collider.gameObject.CompareTag("Pushable"))
            {
                break;
            }

            if (next.collider != null)
            {
                adjacentPushablesInDirection.Add(next.collider);
            }
        }

        // if last pushable raycast is NOT an empty space we return. did NOT move.
        var lastItem = Physics2D.Raycast(adjacentPushablesInDirection.Last().transform.position, moveBy, RayCastDist);
        if (lastItem.collider != null|| (lastItem.collider != null && !lastItem.collider.isTrigger))
        {
            return false;
        }

        adjacentPushablesInDirection.Reverse();
        foreach (var pushable in adjacentPushablesInDirection)
        {
            // TODO rewrite this whole function so only Translate in 1 place and use MoveBy instead.
            pushable.transform.Translate(new Vector3(moveBy.x, moveBy.y, 0));
            this.SnapToGrid(pushable.transform);

            Debug.Log($"moved {pushable.name} to {pushable.transform.position}");
        }

        this.transform.Translate(new Vector3(moveBy.x, moveBy.y, 0));
        this.SnapToGrid();
        return true;
    }
}
