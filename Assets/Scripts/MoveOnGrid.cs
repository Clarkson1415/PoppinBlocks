using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#nullable enable

public class MoveOnGrid : MonoBehaviour
{
    private Collider2D collider;

    private ColouredUnit colouredUnit;

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        colouredUnit = GetComponent<ColouredUnit>();
    }

    private void Start()
    {
        this.SnapToGrid();
    }

    private void SnapToGrid()
    {
        this.transform.position = new Vector3(Mathf.Round(this.transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(this.transform.position.z));
    }

    /// <summary>
    /// Move unit by offset. Returns true if successful.
    /// </summary>
    public bool TryMoveBy(Vector2 moveBy)
    {
        // check if collider that is a not trigger is in movement direction
        // Cast the ray and ignore your own collider
        RaycastHit2D itemInFront = Physics2D.Raycast(this.transform.position, moveBy, ColouredUnit.RaycastDistance);

        // if item in front is a blockage return
        if (itemInFront.collider != null && !itemInFront.collider.isTrigger && !itemInFront.collider.gameObject.CompareTag("Pushable"))
        {
            return false;
        }

        // If there is no item in front move and return.
        if (itemInFront.collider == null || itemInFront.collider.isTrigger)
        {
            MoveThenUpdateIfOnSwirler(moveBy);
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
            var next = Physics2D.Raycast(adjacentPushablesInDirection.Last().transform.position, moveBy, ColouredUnit.RaycastDistance);

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
        var ItemInfrontOfLastItem = Physics2D.Raycast(adjacentPushablesInDirection.Last().transform.position, moveBy, ColouredUnit.RaycastDistance);
        if (ItemInfrontOfLastItem.collider != null && !ItemInfrontOfLastItem.collider.isTrigger)
        {
            return false;
        }

        adjacentPushablesInDirection.Reverse();
        foreach (var pushable in adjacentPushablesInDirection)
        {
            if (pushable.TryGetComponent<MoveOnGrid>(out var pushablesMover))
            {
                pushablesMover.MoveThenUpdateIfOnSwirler(moveBy);
            }
            else
            {
                Debug.LogError($"Pushables should have move on grid missing on {this.name}");
            }
        }

        MoveThenUpdateIfOnSwirler(moveBy);

        return true;
    }

    private void MoveThenUpdateIfOnSwirler(Vector2 moveBy)
    {
        this.transform.Translate(new Vector3(moveBy.x, moveBy.y, 0));
        this.SnapToGrid();

        Physics2D.SyncTransforms();

        // Check if this guy is on a swirler and update colour here.
        var newContacts = new ContactFilter2D();
        newContacts.useTriggers = true;
        List<Collider2D> overlapping = new();
        if (this.collider.Overlap(newContacts, overlapping) > 0)
        {
            if (overlapping.Count == 0)
            {
                return;
            }

            var swirlerExists = overlapping.Any(x => x.TryGetComponent<ColourChanger>(out var _));

            if (swirlerExists)
            {
                var swirler = overlapping.First(x => x.TryGetComponent<ColourChanger>(out var _));
                this.colouredUnit.ChangeColour(swirler.GetComponent<ColourChanger>().Colour);
            }
        }
    }
}
