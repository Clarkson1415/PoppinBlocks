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
    [RequireComponent(typeof(Animator))]
    public class Unit : MonoBehaviour
    {
        public bool IsTouchingAnotherOfSameColour => this.IsAdjacentToSameColour();

        [SerializeField] private float raycastRadius = 1.5f;

        public TileColour GetUnitColour => this.squareColour.ThisGuysColour;

        private GameColour squareColour;

        private Animator animator;
        private void Start()
        {
            this.squareColour = GetComponent<GameColour>();
            this.animator = GetComponent<Animator>();
        }

        private bool IsAdjacentToSameColour()
        {

            var nearby = this.NearbyGuys();

            if (nearby == null || !nearby.Any())
            {
                return false;
            }

            var nearbyColours = nearby.Select(x => x.squareColour.ThisGuysColour);
            if (nearbyColours == null || !nearbyColours.Any())
            {
                return false;
            }

            if (!nearbyColours.Contains(this.squareColour.ThisGuysColour))
            {
                return false;
            }

            return true;
        }

        private IEnumerable<Unit> NearbyGuys()
        {
            // Only check in 4 directions. corner to corner not allowed.
            // see playermvoement do same but for 4.
            var directions = new Vector2[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
            var allHits = new List<RaycastHit2D>();

            foreach (var direction in directions)
            {
                var hits = Physics2D.RaycastAll(this.transform.position, direction, 1f);
                allHits.AddRange(hits);
            }

            return allHits
                .Select(x => x.collider.GetComponent<Unit>())
                .Where(x => x != null && !Popped.hasPopped.Contains(x));
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
            this.animator.SetTrigger("Pop");

            if (!this.IsTouchingAnotherOfSameColour)
            {
                return;
            }

            var adjacent = this.NearbyGuys();
            foreach (var guy in adjacent)
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
