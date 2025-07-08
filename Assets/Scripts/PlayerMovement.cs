using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Unit))]
    public class PlayerMovement : MonoBehaviour
    {
        private Vector2 moveInput;
        private Rigidbody2D rb;
        private MoveOnGrid moveOnGrid;
        private Unit unit;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            moveOnGrid = GetComponent<MoveOnGrid>();
            unit = GetComponent<Unit>();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;

            moveInput = context.ReadValue<Vector2>();

            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
                moveInput = new Vector2(Mathf.Sign(moveInput.x), 0);
            else if (Mathf.Abs(moveInput.y) > Mathf.Abs(moveInput.x))
                moveInput = new Vector2(0, Mathf.Sign(moveInput.y));
            else if (moveInput.x != 0)  // If equal, prefer X
                moveInput = new Vector2(Mathf.Sign(moveInput.x), 0);
            else
                return;

            this.moveOnGrid.MoveBy(moveInput);

            // check if touching another of the same colour.
            if (this.unit.IsTouchingAnother)
            {
                // this.unit.pop
                this.unit.Pop();

                // if player did not pop all guys in the level of the matching colour he lost. prompt to restart level.
                // undo would be nice but not time rn.

            }

            // TODO check if units in the scene popped after all popped to see if need to restart.
            // BUT this guy is inactive. pop deactivates. so idk
        }
    }
}
