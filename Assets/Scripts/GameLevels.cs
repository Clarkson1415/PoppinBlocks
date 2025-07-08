using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public static class GameLevels
    {
        public static List<string> LevelNames = new() { "Level1", "Level2", "End" };

        public static string GetNextLevel()
        {
            var currentLevelIndex = LevelNames.IndexOf(CurrentLevel);
            return LevelNames[currentLevelIndex + 1];
        }

        private static string CurrentLevel = string.Empty;

        public static void LevelCompleted()
        {
            // Transition manager .load 
            var next = GetNextLevel();
            Debug.Log($"loading level {next}");

            SceneManager.LoadScene(next);
        }

        public static void Initialise()
        {
            if (string.IsNullOrEmpty(CurrentLevel))
            {
                CurrentLevel = LevelNames[0];
            }

        }

        public static void Reload()
        {
            Debug.Log($"loading level {CurrentLevel}");
            SceneManager.LoadScene(CurrentLevel);
        }
    }
}
