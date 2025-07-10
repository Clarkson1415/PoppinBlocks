using EasyTransition;
using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
#nullable enable

namespace Assets.Scripts
{
    [RequireComponent(typeof(ColouredUnit))]
    public class PlayerMovement : MonoBehaviour
    {
        /// <summary>
        /// If there is another player in the level this player leads onto NextPLayer. Once this is popped.
        /// </summary>
        [SerializeField] private ColouredUnit? NextPlayer;

        private AudioSource moveAudio;

        /// <summary>
        /// At least 1 player in scene has to start!
        /// </summary>
        [SerializeField] private bool ThisIsTheStartingPlayer;

        [SerializeField] private TransitionSettings transition;

        private Vector2 moveInput;
        private MoveOnGrid moveOnGrid;
        private ColouredUnit unitColour;

        private void Start()
        {
            Popped.ToPopOrIsPopping.Clear();

            moveOnGrid = GetComponent<MoveOnGrid>();
            unitColour = GetComponent<ColouredUnit>();

            this.GetComponent<PlayerInput>().enabled = false;

            if (!ThisIsTheStartingPlayer)
            {
                this.GetComponent<PlayerInput>().defaultActionMap = "NotCurrentPlayer";
            }
            else
            {
                this.GetComponent<PlayerInput>().defaultActionMap = "Player";
                this.GetComponent<PlayerInput>().enabled = true;
            }

            moveAudio = this.GetComponent<AudioSource>();
        }

        /// <summary>
        /// SKip to level
        /// </summary>
        /// <param name="context"></param>
        public void OnNumber(InputAction.CallbackContext context)
        {
            var numKey = context.control.name;
            GameLevels.LoadLevel(transition, $"Level {numKey}");
        }

        public void Quit(InputAction.CallbackContext context)
        {
            if (!context.started)
            {
                return;
            }

            Application.Quit();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;

            if (Popped.ToPopOrIsPopping.Contains(this.unitColour))
            {
                return;
            }

            moveInput = context.ReadValue<Vector2>();

            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
                moveInput = new Vector2(Mathf.Sign(moveInput.x), 0);
            else if (Mathf.Abs(moveInput.y) > Mathf.Abs(moveInput.x))
                moveInput = new Vector2(0, Mathf.Sign(moveInput.y));
            else if (moveInput.x != 0)  // If equal, prefer X
                moveInput = new Vector2(Mathf.Sign(moveInput.x), 0);
            else
                return;

            var wasMoved = this.moveOnGrid.TryMoveBy(moveInput);

            if (wasMoved)
            {
                this.moveAudio.Play();
            }

            Physics2D.SyncTransforms(); // Sync colliders to transforms.

            // check if touching another of the same colour.
            if (!this.unitColour.IsTouchingAnotherOfSameColour)
            {
                return;
            }

            this.unitColour.AddToPopChain();

            waitForAllPops ??= StartCoroutine(WaitForPopsThenAssessGameState());
        }

        private Coroutine? waitForAllPops;

        IEnumerator WaitForPopsThenAssessGameState()
        {
            foreach (var item in Popped.ToPopOrIsPopping.Where(x => x.gameObject.activeSelf))
            {
                item.Pop();
                yield return new WaitForSeconds(0.2f);
            }

            // while not all are finished popping wait.
            while (!Popped.ToPopOrIsPopping.Where(x => x.gameObject.activeSelf).All(x => x.animator.GetCurrentAnimatorStateInfo(0).IsName("Blank")))
            {
                yield return null;
            }

            // Won if all the objects have been popped.
            var all = FindObjectsByType<ColouredUnit>(FindObjectsSortMode.None);
            if (all.All(x => Popped.ToPopOrIsPopping.Contains(x)))
            {
                GameLevels.LevelCompleted(transition);
                waitForAllPops = null;
                yield break;
            }

            // check fail conditions here:
            if (this.IsFailed(all))
            {
                Debug.Log("You failed enter to restart.");
            }

            // deactivate all popped objects
            foreach (var square in Popped.ToPopOrIsPopping)
            {
                square.gameObject.SetActive(false);
            }

            // if another player other than this player, transfer controls.
            if (all.Any(x => x.TryGetComponent<PlayerMovement>(out var playernext) && playernext != this))
            {
                if (this.NextPlayer == null)
                {
                    Debug.LogError($"NO next player. {this.name}");
                    throw new NullReferenceException("No next player but need one.");
                }

                this.GetComponent<PlayerInput>().defaultActionMap = "NotCurrentPlayer";
                this.GetComponent<PlayerInput>().enabled = false;

                this.NextPlayer.GetComponent<PlayerInput>().enabled = true;
                this.NextPlayer.GetComponent<PlayerInput>().defaultActionMap = "Player";
                this.NextPlayer.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");

                this.gameObject.SetActive(false);
            }

            waitForAllPops = null;
        }

        private bool IsFailed(ColouredUnit[] allUnits)
        {
            var unpopped = allUnits.Where(x => !Popped.ToPopOrIsPopping.Contains(x));

            // if any players without a corresponding other ColouredUnit = fail
            // if any coloured units without a corresponding other player COlouredunit = fail
            // what if 2 playres of the same colour?
            // or what if I can change their colour at some Point?
            Debug.Log("Todo fail checks");
            return false;
        }

        public void Restart(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }

            Debug.Log("Restarting...");
            GameLevels.Reload(transition);
        }
    }
}
