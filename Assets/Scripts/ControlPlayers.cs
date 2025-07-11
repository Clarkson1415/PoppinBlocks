using Assets.Scripts;
using EasyTransition;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class ControlPlayers : MonoBehaviour
{
    [SerializeField] private List<PlayerMovement> activePlayers = new();

    [SerializeField] private TransitionSettings transition;

    private AudioSource moveAudio;
    private Coroutine? waitForAllPops;

    private PauseScreen pauseScreen;

    private void Start()
    {
        pauseScreen = FindFirstObjectByType<PauseScreen>();

        Popped.ToPopHasPoppedOrIsPopping.Clear();

        moveAudio = this.GetComponent<AudioSource>();

        if (activePlayers.Count == 0)
        {
            Debug.LogError("Forgot to assign a player.");
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        var moveInput = context.ReadValue<Vector2>();


        // TODO put this in fixed update instead.
        foreach (var player in activePlayers)
        {
            if (Popped.ToPopHasPoppedOrIsPopping.Contains(player.colouredUnit))
            {
                return;
            }

            var wasMoved = player.TryMove(moveInput);

            if (wasMoved)
            {
                this.moveAudio.Play();
            }

            // Need to wait for OnTriggerEnter2D Events to update before this.
            // ?

            // check if any player is touching another of the same colour.
            if (player.colouredUnit.IsTouchingAnotherOfSameColour)
            {
                player.colouredUnit.AddToPopChain();
            }
        }

        waitForAllPops ??= StartCoroutine(PopAll());
    }

    private bool AnyToPopAreNotPopped()
    {
        return !Popped.ToPopHasPoppedOrIsPopping.Where(x => x.gameObject.activeSelf).All(x => x.animator.GetCurrentAnimatorStateInfo(0).IsName("Blank"));
    }

    IEnumerator PopAll()
    {
        foreach (var item in Popped.ToPopHasPoppedOrIsPopping.Where(x => x.gameObject.activeSelf))
        {
            item.Pop();
            yield return new WaitForSeconds(0.2f);
        }

        while (AnyToPopAreNotPopped())
        {
            yield return null;
        }

        CheckLevelState();

        waitForAllPops = null;
    }

    private void CheckLevelState()
    {
        // if All Coloured Units are popped. Have finished level.
        var allUnits = FindObjectsByType<ColouredUnit>(FindObjectsSortMode.None);
        if (allUnits.All(x => Popped.ToPopHasPoppedOrIsPopping.Contains(x)))
        {
            GameLevels.LevelCompleted(transition);
            return;
        }

        // check fail conditions here:
        if (this.IsFailed(allUnits))
        {
            Debug.Log("Check fail conditions.");
        }

        // deactivate all popped objects
        foreach (var square in Popped.ToPopHasPoppedOrIsPopping)
        {
            square.gameObject.SetActive(false);
        }

        // If no active players were popped this turn then return.
        if (!activePlayers.Any(x => Popped.ToPopHasPoppedOrIsPopping.Contains(x.colouredUnit)))
        {
            return;
        }

        // if a player was popped. And we have not won. we need to try change controls or we failed the level - as fail check is not implemented yet.
        if (allUnits.Any(x => !Popped.ToPopHasPoppedOrIsPopping.Contains(x) && x.TryGetComponent<PlayerMovement>(out var possiblePlayer)))
        {
            // if All active players have NO next player throw an error
            if (activePlayers.All(x => x.NextPlayers.Count == 0))
            {
                Debug.LogWarning("There is another player in scene that is NOT assigned to any next player and is NOT an initial player.");
            }

            ChangeOverPlayers();
        }
    }

    private void ChangeOverPlayers()
    {
        // change active players to the current active players next player.
        List<PlayerMovement> newActivePlayers = new();

        // Keep players that did not pop
        newActivePlayers.AddRange(activePlayers.Where(x => !Popped.ToPopHasPoppedOrIsPopping.Contains(x.colouredUnit)));

        // For all popped players change controls over or not if the list is empty anyways.
        var activePlayersThatPopped = activePlayers.Where(x => Popped.ToPopHasPoppedOrIsPopping.Contains(x.colouredUnit));
        foreach (var poppedPlayer in activePlayersThatPopped)
        {
            newActivePlayers.AddRange(poppedPlayer.NextPlayers);
        }

        activePlayers = newActivePlayers;
    }

    private bool IsFailed(ColouredUnit[] allUnits)
    {
        var unpopped = allUnits.Where(x => !Popped.ToPopHasPoppedOrIsPopping.Contains(x));

        // if any players without a corresponding other ColouredUnit = fail
        // if any coloured units without a corresponding other player COlouredunit = fail
        // what if 2 playres of the same colour?
        // or what if I can change their colour at some Point?
        return false;
    }

    public void ShowLevelSelect(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }

        pauseScreen.Toggle();
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
}
