using Assets.Scripts;
using EasyTransition;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] TransitionSettings transitionSettings;

    public void StartLevel1()
    {
        GameLevels.LoadLevel(transitionSettings, "Level 1");
    }
}
