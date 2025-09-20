using Assets.Scripts;
using EasyTransition;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
#nullable enable

[RequireComponent(typeof(AudioSource))]
public class ControlPlayers : MonoBehaviour
{
    [SerializeField] private List<PlayerMovement> activePlayers = new();

    [SerializeField] private TransitionSettings transition;

    private AudioSource moveAudio;
    private Coroutine? waitForAllPops;

    private PauseScreen pauseScreen;

    private WinComponentController WonLevelText;
    
    public static ControlPlayers? Instance()
    {
        var playerControllers = FindObjectsByType<ControlPlayers>(FindObjectsSortMode.None);
        if (playerControllers.Length > 1)
        {
            throw new ArgumentOutOfRangeException("There should never be more than 1 ControlPlayers in a scene.");
        }

        return FindObjectsByType<ControlPlayers>(FindObjectsSortMode.None).FirstOrDefault();
    }

    private void Awake()
    {
        pauseScreen = FindFirstObjectByType<PauseScreen>();

        Popped.ToPopHasPoppedOrIsPopping.Clear();

        moveAudio = this.GetComponent<AudioSource>();

        if (activePlayers.Count == 0)
        {
            Debug.LogError("Forgot to assign a player.");
        }

        allUnits = FindObjectsByType<ColouredUnit>(FindObjectsSortMode.None);

        WonLevelText = FindFirstObjectByType<WinComponentController>();
    }

    public void OnButtonUp()
    {
        ProcessMovement(Vector2.up);
    }

    public void OnButtonDown()
    {
        ProcessMovement(Vector2.down);
    }

    public void OnButtonLeft()
    {
        ProcessMovement(Vector2.left);
    }

    public void OnButtonRight()
    {
        ProcessMovement(Vector2.right);
    }

    private bool processingMove = false;

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }

        ProcessMovement(context.ReadValue<Vector2>());
    }

    private void ProcessMovement(Vector2 movementDir)
    {
        Console.WriteLine("process movement called.");

        if (processingMove || AnyToPopAreNotPopped())
            return;

        processingMove = true;

        foreach (var player in activePlayers)
        {
            if (Popped.ToPopHasPoppedOrIsPopping.Contains(player.colouredUnit))
            {
                continue;
            }

            var wasMoved = player.TryMove(movementDir);

            if (wasMoved)
            {
                this.moveAudio.Play();
            }
        }

        if (HaveWonLoadingNextScene)
        {
            return;
        }

        this.PopIfTouching();

        processingMove = false;
    }

    private void PopIfTouching()
    {
        foreach (var player in activePlayers)
        {
            if (player == null)
            {
                Debug.LogError("Grace did not assign players properly for the level.");
                return;
            }

            if (Popped.ToPopHasPoppedOrIsPopping.Contains(player.colouredUnit))
            {
                continue;
            }

            if (player.colouredUnit.IsTouchingAnotherOfSameColour)
            {
                player.colouredUnit.AddToPopChain();
            }
        }

        if (Popped.ToPopHasPoppedOrIsPopping.Count == 0)
        {
            return;
        }

        if (Popped.ToPopHasPoppedOrIsPopping.Any(x => x.gameObject.activeSelf))
        {
            waitForAllPops ??= StartCoroutine(PopAll());
        }
    }

    private bool AnyToPopAreNotPopped()
    {
        return !Popped.ToPopHasPoppedOrIsPopping.Where(x => x.gameObject.activeSelf).All(x => x.FinishedPopping);
    }

    public float DelayBetweenPops = 0.2f;

    IEnumerator PopAll()
    {
        foreach (var item in Popped.ToPopHasPoppedOrIsPopping.Where(x => x.gameObject.activeSelf))
        {
            item.Pop();
            yield return new WaitForSecondsRealtime(DelayBetweenPops);
        }

        while (AnyToPopAreNotPopped())
        {
            yield return null;
        }

        CheckLevelState();

        waitForAllPops = null;
    }

    private bool HaveWonLoadingNextScene = false;

    private ColouredUnit[] allUnits = Array.Empty<ColouredUnit>();

    [SerializeField] private float waitOnLevelCompleteScreen = 1f;

    private IEnumerator LevelCompleteTextThenLoad()
    {
        this.WonLevelText.TurnOn();
        while (!this.WonLevelText.IsFinishedAnimating)
        {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(waitOnLevelCompleteScreen);

        GameLevels.LevelCompleted(transition);
    }

    private void CheckLevelState()
    {
        // if All Coloured Units are popped. Have finished level.
        if (allUnits.All(x => Popped.ToPopHasPoppedOrIsPopping.Contains(x)))
        {
            StartCoroutine(LevelCompleteTextThenLoad());
            HaveWonLoadingNextScene = true;
            return;
        }

        // check fail conditions here:
        if (this.IsFailed())
        {
        }

        // deactivate all popped objects
        foreach (var square in Popped.ToPopHasPoppedOrIsPopping)
        {
            square.gameObject.SetActive(false);
        }

        // If no active players were popped this turn then return.
        //if (!activePlayers.Any(x => Popped.ToPopHasPoppedOrIsPopping.Contains(x.colouredUnit)))
        //{
        //    return;
        //}

        // if a player was popped. And we have not won. we need to try change controls or we failed the level - as fail check is not implemented yet.
        //if (allUnits.Any(x => !Popped.ToPopHasPoppedOrIsPopping.Contains(x) && x.TryGetComponent<PlayerMovement>(out var possiblePlayer)))
        //{
        //    // if All active players have NO next player throw an error
        //    if (activePlayers.All(x => x.NextPlayers.Count == 0))
        //    {
        //        Debug.LogWarning("There is another player in scene that is NOT assigned to any next player and is NOT an initial player.");
        //    }

        //    ChangeOverPlayers();
        //}
    }

    //private void ChangeOverPlayers()
    //{
    //    // change active players to the current active players next player.
    //    List<PlayerMovement> newActivePlayers = new();

    //    // Keep players that did not pop
    //    newActivePlayers.AddRange(activePlayers.Where(x => !Popped.ToPopHasPoppedOrIsPopping.Contains(x.colouredUnit)));

    //    // For all popped players change controls over or not if the list is empty anyways.
    //    var activePlayersThatPopped = activePlayers.Where(x => Popped.ToPopHasPoppedOrIsPopping.Contains(x.colouredUnit));
    //    foreach (var poppedPlayer in activePlayersThatPopped)
    //    {
    //        newActivePlayers.AddRange(poppedPlayer.NextPlayers);
    //    }

    //    activePlayers = newActivePlayers;
    //}

    private bool IsFailed()
    {
        // var unpopped = allUnits.Where(x => !Popped.ToPopHasPoppedOrIsPopping.Contains(x));
        Debug.Log("Check fail conditions.");

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
