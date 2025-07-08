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

        if (solidHits.Any())
        {
            return;
        }

        this.transform.Translate(new Vector3(moveBy.x, moveBy.y, 0));
        this.SnapToGrid();
    }
}
