using Assets.Scripts;
using EasyTransition;
using UnityEngine;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private string levelToLoad;

    [SerializeField] private TransitionSettings transition;

    public string GetLevel() { return levelToLoad; }

    public void LoadTheLevel()
    {
        GameLevels.LoadLevel(transition, levelToLoad);
    }
}
