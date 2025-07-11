using Assets.Scripts;
using EasyTransition;
using UnityEngine;

public class LoadLevel : MonoBehaviour
{
    [SerializeField] private string levelToLoad;

    [SerializeField] private TransitionSettings transition;

    public void LoadTheLevel()
    {
        GameLevels.LoadLevel(transition, levelToLoad);
    }
}
