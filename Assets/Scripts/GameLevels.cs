using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public static class GameLevels
    {
        public static List<string> LevelNames = new() { "Level 1", "Level 2", "Level 3", "Level 4", "Level 5", "Level 6", "Level 7", "Level 8", "Level 9", "End" };

        public static string GetNextLevel()
        {
            var currentLevelIndex = LevelNames.IndexOf(CurrentLevel);
            return LevelNames[currentLevelIndex + 1];
        }

        private static string CurrentLevel = SceneManager.GetActiveScene().name;

        public static void LevelCompleted()
        {
            // Transition manager .load 
            CurrentLevel = GetNextLevel();
            Debug.Log($"loading level {CurrentLevel}");

            SceneManager.LoadScene(CurrentLevel);
        }

        public static void LoadLevel(string levelName)
        {
            if (!levelName.Contains(levelName))
            {
                Debug.LogError($"no level to match {levelName}");
            }

            CurrentLevel = levelName;
            SceneManager.LoadScene(levelName);
        }

        public static void Reload()
        {
            Debug.Log($"loading level {CurrentLevel}");
            SceneManager.LoadScene(CurrentLevel);
        }
    }
}
