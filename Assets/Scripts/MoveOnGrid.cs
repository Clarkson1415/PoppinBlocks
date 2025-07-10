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

    private float rayCastDist => this.GetComponent<ColouredUnit>().raycastDistance;

    /// <summary>
    /// Move unit by offset.
    /// </summary>
    public void MoveBy(Vector2 moveBy)
    {
        // check if collider that is a not trigger is in movement direction
        // Cast the ray and ignore your own collider
        RaycastHit2D itemInFront = Physics2D.Raycast(this.transform.position, moveBy, rayCastDist);

        // if item in front is a blockage return
        if (itemInFront.collider != null && !itemInFront.collider.isTrigger && itemInFront.collider.gameObject.tag != "Pushable")
        {
            return;
        }

        // If there is no item in front move and return.
        if (itemInFront.collider == null || itemInFront.collider.isTrigger)
        {
            this.transform.Translate(new Vector3(moveBy.x, moveBy.y, 0));
            this.SnapToGrid();
            return;
        }

        // check if every object in a chain of that direction is pushable recursively? if so move all the objects by 1 unit by moveBy
        // Get all adjacent pushable items in the direction.
        // If there is a collider thats not "Pushable" or an IsTrigger dont push.
        var adjacentPushablesInDirection = new List<Collider2D>();
        adjacentPushablesInDirection.Add(itemInFront.collider);

        while (true)
        {
            var next = Physics2D.Raycast(adjacentPushablesInDirection.Last().transform.position, moveBy, rayCastDist);

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
            else if (next.collider != null && next.collider.gameObject.tag != "Pushable")
            {
                break;
            }

            adjacentPushablesInDirection.Add(next.collider);
        }

        // if last pushable raycast is NOT an empty space we return.
        var lastItem = Physics2D.Raycast(adjacentPushablesInDirection.Last().transform.position, moveBy, rayCastDist);
        if (lastItem.collider != null|| (lastItem.collider != null && !lastItem.collider.isTrigger))
        {
            return;
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
    }
}
