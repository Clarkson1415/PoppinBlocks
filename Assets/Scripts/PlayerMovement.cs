using EasyTransition;
using System.Collections.Generic;
using UnityEngine;
#nullable enable

namespace Assets.Scripts
{
    [RequireComponent(typeof(ColouredUnit))]
    public class PlayerMovement : MonoBehaviour
    {
        /// <summary>
        /// If there is another player in the level this player leads onto NextPLayer. Once this is popped.
        /// </summary>
        //public List<PlayerMovement> NextPlayers = new();

        [SerializeField] private TransitionSettings transition;

        private MoveOnGrid moveOnGrid;

        [HideInInspector] public ColouredUnit colouredUnit;

        [SerializeField] private Dust dust;

        private void Awake()
        {
            moveOnGrid = GetComponent<MoveOnGrid>();
            colouredUnit = GetComponent<ColouredUnit>();
        }

        public bool TryMove(Vector2 moveInput)
        {
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
                moveInput = new Vector2(Mathf.Sign(moveInput.x), 0);
            else if (Mathf.Abs(moveInput.y) > Mathf.Abs(moveInput.x))
                moveInput = new Vector2(0, Mathf.Sign(moveInput.y));
            else if (moveInput.x != 0)  // If equal, prefer X
                moveInput = new Vector2(Mathf.Sign(moveInput.x), 0);
            else
                return false;

            var wasMoved = this.moveOnGrid.TryMoveBy(moveInput);

            if (wasMoved)
            {
                this.PlayDustAnimation(moveInput);
            }

            return wasMoved;
        }


        private float dustOffset = 1f;

        private void PlayDustAnimation(Vector2 moveInput)
        {
            if (moveInput == Vector2.zero)
                return;

            // Normalize input to avoid large offsets
            Vector3 offset = (Vector3)(-moveInput.normalized * dustOffset);

            // Position dust slightly behind current position
            dust.gameObject.transform.position = transform.position + offset;

            // Rotate to face movement direction
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            dust.gameObject.transform.rotation = Quaternion.Euler(0, 0, angle); // Adjust depending on sprite orientation

            // Play the animation trigger
            dust.PlayDust();

            // was moved by moveInput

            // set dust gameobject to face the direction of move input at the coordinates at this.transform - moveInput.
            // play animation. Trigger is "Dust"
        }
    }
}
