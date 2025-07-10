using UnityEngine;

public class GameMusic : MonoBehaviour
{
    public static GameMusic Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
