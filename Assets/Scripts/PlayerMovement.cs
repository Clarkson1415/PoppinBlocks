using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
#nullable enable

namespace Assets.Scripts
{
    [RequireComponent(typeof(Unit))]
    public class PlayerMovement : MonoBehaviour
    {
        /// <summary>
        /// If there is another player in the level this player leads onto NextPLayer. Once this is popped.
        /// </summary>
        [SerializeField] private Unit? NextPlayer;

        /// <summary>
        /// At least 1 player in scene has to start!
        /// </summary>
        [SerializeField] private bool ThisIsTheStartingPlayer;

        private Vector2 moveInput;
        private Rigidbody2D rb;
        private MoveOnGrid moveOnGrid;
        private Unit unit;

        private void Start()
        {
            GameLevels.Initialise();

            rb = GetComponent<Rigidbody2D>();
            moveOnGrid = GetComponent<MoveOnGrid>();
            unit = GetComponent<Unit>();

            if (!ThisIsTheStartingPlayer)
            {
                this.GetComponent<PlayerMovement>().enabled = false;
                this.GetComponent<PlayerInput>().enabled = false;
            }
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
            if (!this.unit.IsTouchingAnotherOfSameColour)
            {
                return;
            }
            
            // this.unit.pop
            this.unit.Pop();

            // Won if all the objects have been popped.
            var all = FindObjectsByType<Unit>(FindObjectsSortMode.None);
            if (all.All(x => Popped.hasPopped.Contains(x)))
            {
                StartCoroutine(WaitThenComplete());
                return;
            }
            // Continue on level if popped all of colour A, and there are some of colour B todo.
            // that is: if theres any that are not same as this player, we can continue; assign player controls to the next player guy to use.
            else if (all.Any(x => x.GetUnitColour != this.GetComponent<Unit>().GetUnitColour))
            {
                if (this.NextPlayer == null)
                {
                    Debug.LogError("NO next player.");
                    return;
                }

                this.NextPlayer.GetComponent<PlayerMovement>().enabled = true;
                this.NextPlayer.GetComponent<PlayerInput>().enabled = false;


                Debug.Log("deactivate this and all current Popped objects here");

                return;
            }
            else
            {
                // you fucked up
                Debug.Log("you Messed up Enter to restart");
                // will have restart at anytime button like 'l'
            }
        }

        public void Restart(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }

            Debug.Log("Restarting...");
            GameLevels.Reload();
        }

        private IEnumerator WaitThenComplete()
        {
            Debug.Log("wait for all animations to finish then show level completed.");
            yield return new WaitForSeconds(2f);
            GameLevels.LevelCompleted();
        }
    }
}
