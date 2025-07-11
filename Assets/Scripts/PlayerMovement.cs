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

            Physics2D.SyncTransforms(); // Sync colliders to transforms.

            return wasMoved;
        }
    }
}
