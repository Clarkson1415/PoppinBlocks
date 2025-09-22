using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private Image medalImage;

    public void Start()
    {
        var levelToLoad = GetComponent<LoadScene>().GetLevel();

        if (medalImage == null)
        {
            Debug.LogError($"No medal image on {this.name}");
            return;
        }

        medalImage.enabled = GameLevels.HasLevelBeenBeaten(levelToLoad);
    }
}
