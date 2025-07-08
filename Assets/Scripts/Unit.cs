using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
#nullable enable

namespace Assets.Scripts
{
    /// <summary>
    /// Represents a square guy.
    /// </summary>
    [RequireComponent(typeof(MoveOnGrid))]
    [RequireComponent(typeof(GameColour))]
    public class Unit : MonoBehaviour
    {
        public bool IsTouchingAnother => NearbyGuys().Select(x => x.squareColour.ThisGuysColour).Contains(this.squareColour.ThisGuysColour);

        [SerializeField] private float raycastRadius = 1.5f;

        private GameColour squareColour;

        private void Start()
        {
            this.squareColour = GetComponent<GameColour>();
        }

        private IEnumerable<Unit> NearbyGuys()
        {
            // Only check in 4 directions. corner to corner not allowed.
            // see playermvoement do same but for 4.

            RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, Vector2.up, 1f);
            hits.Concat(Physics2D.RaycastAll(this.transform.position, Vector2.right, 1f));
            hits.Concat(Physics2D.RaycastAll(this.transform.position, Vector2.down, 1f));
            hits.Concat(Physics2D.RaycastAll(this.transform.position, Vector2.left, 1f));

            return hits.Select(x => x.collider.GetComponent<Unit>()).Where(x => !Popped.hasPopped.Contains(x));
        }

        public void Pop()
        {
            // if this is touching another of same colour and active.
            // trigger pop on those ones.
            // then play pop animation on this one. then deactivate.

            // if this is the last unit to pop. idk how to check for that because if I check none touching it then could be multiple ends.
            // anyway if this is the last unit in the touch sequence to pop then trigger win screen.
            Debug.Log($"pop this: {this.gameObject.name}");
            Popped.hasPopped.Add(this);

            if (!this.IsTouchingAnother)
            {
                return;
            }

            foreach (var guy in this.NearbyGuys())
            {
                if (guy.squareColour.ThisGuysColour != squareColour.ThisGuysColour)
                {
                    continue;
                }

                guy.Pop();
            }
        }
    }
}
