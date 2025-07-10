using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TMP_Text))]
public class LevelText : MonoBehaviour
{
    private TMP_Text text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = this.GetComponent<TMP_Text>();

        var thisLevel = SceneManager.GetActiveScene().name;
        text.text = thisLevel;
    }

}
