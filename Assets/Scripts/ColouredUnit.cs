using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Represents something poppable.
    /// </summary>
    public class ColouredUnit : MonoBehaviour
    {
        public TileColour ThisGuysColour;
        
        public float raycastDistance = 0.6f;

        public Animator animator;

        public bool IsTouchingAnotherOfSameColour => this.IsAdjacentToSameColour();

        private RandomSoundPlayer randomSoundPlayer;

        private void Start()
        {
            this.GetComponent<SpriteRenderer>().color = RegisteredColours.GetColor(this.ThisGuysColour);
            this.animator = GetComponent<Animator>();
            this.randomSoundPlayer = this.GetComponentInChildren<RandomSoundPlayer>();
        }

        private bool IsAdjacentToSameColour()
        {
            var nearby = this.NearbyGuys();

            if (nearby == null || !nearby.Any())
            {
                return false;
            }

            var nearbyColouredUnits = nearby.Select(x => x.ThisGuysColour == this.ThisGuysColour);
            if (nearbyColouredUnits == null || !nearbyColouredUnits.Any())
            {
                return false;
            }

            if (!nearbyColouredUnits.Contains(this))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Nearby OBjects of all colours.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ColouredUnit> NearbyGuys()
        {
            // Only check in 4 directions. corner to corner not allowed.
            // see playermvoement do same but for 4.
            var directions = new Vector2[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
            var allHits = new List<RaycastHit2D>();

            foreach (var direction in directions)
            {
                var hits = Physics2D.Raycast(this.transform.position, direction, this.raycastDistance);
                if (hits.collider == null)
                {
                    continue;
                }

                allHits.Add(hits);//check this is not the player
            }

            return allHits.Where(x => x.collider != null).Select(x => x.collider.GetComponent<ColouredUnit>())
                .Where(x => x != null && !Popped.hasPopped.Contains(x) && x.gameObject != this.gameObject);
        }

        public void AddToPopChain()
        {
            if (Popped.hasPopped.Contains(this))
            {
                return;
            }

            Popped.hasPopped.Add(this);

            if (!this.IsTouchingAnotherOfSameColour)
            {
                return;
            }

            var adjacent = this.NearbyGuys();
            foreach (var guy in adjacent) // Directly 1 block in each 4 directions.
            {
                if (guy.ThisGuysColour != this.ThisGuysColour)
                {
                    continue;
                }

                guy.AddToPopChain();
            }
        }

        public void Pop()
        {
            StartCoroutine(DelayThenPop());
        }

        private IEnumerator DelayThenPop()
        {
            yield return new WaitForSeconds(0.3f);

            // if this is touching another of same colour and active.
            // trigger pop on those ones.
            // then play pop animation on this one. then deactivate.

            // if this is the last unit to pop. idk how to check for that because if I check none touching it then could be multiple ends.
            // anyway if this is the last unit in the touch sequence to pop then trigger win screen.
            Debug.Log($"pop this: {this.gameObject.name}");
            this.animator.SetTrigger("Pop");
            StartCoroutine(WaitThenPlayAPopSound());
        }

        [SerializeField] private float popAnimTimeTillSound = 0.4f;

        IEnumerator WaitThenPlayAPopSound()
        {
            yield return new WaitForSeconds(popAnimTimeTillSound);
            this.randomSoundPlayer.PlayRandomSound();
        }
    }
}
