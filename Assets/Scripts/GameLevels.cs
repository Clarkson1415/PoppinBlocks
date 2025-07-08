using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public static class GameLevels
    {
        public static List<string> LevelNames = new() { "Level1", "Level2", "Level3", "End" };

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

        public static void Reload()
        {
            Debug.Log($"loading level {CurrentLevel}");
            SceneManager.LoadScene(CurrentLevel);
        }
    }
}
