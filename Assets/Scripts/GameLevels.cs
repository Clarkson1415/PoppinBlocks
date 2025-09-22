using EasyTransition;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.VisualScripting;

namespace Assets.Scripts
{
    public static class GameLevels
    {
        public static Dictionary<string, string> SceneNameSpecialName = new() { { "TitleScreen", "TitleScreen" }, { "Level 1", "Something new" }, { "Level 2", "s" } };

        public static List<string> LevelNames = new() { "TitleScreen", "Level 1", "Level 2", "Level 3", "Level 4", "Level 5", "Level 6", "Level 7", "Level 8", "Level 9", "Level 10", "Level 11", "Level 12", "Level 13", "Level 14", "Level 15", "Level 16", "Level 17", "Level 18", "End" };

        public static string GetNextLevel()
        {
            var currentLevelIndex = LevelNames.IndexOf(CurrentLevel);

            if (currentLevelIndex + 1 > LevelNames.Count)
            {
                Debug.LogError($"failed to get next level from. {CurrentLevel}");
            }

            return LevelNames[currentLevelIndex + 1];
        }

        public static bool HasLevelBeenBeaten(string levelName)
        {
            var completedLevels = PlayerPrefs.GetString(CompletedLevelsString);
            return completedLevels == null ? false : completedLevels.Contains(levelName);
        }

        private static string CompletedLevelsString = "CompletedLevels";

        private static void AddToCompletedLevels(string levelName)
        {
            var completedLevels = PlayerPrefs.GetString(CompletedLevelsString);
            if (!completedLevels.Contains(levelName))
            {
                completedLevels += levelName + ";";
                PlayerPrefs.SetString(CompletedLevelsString, completedLevels);
            }
        }

        private static string CurrentLevel = SceneManager.GetActiveScene().name;

        public static void LevelCompleted(TransitionSettings setting)
        {
            AddToCompletedLevels(SceneManager.GetActiveScene().name);
            CurrentLevel = GetNextLevel();
            LoadLevel(setting, CurrentLevel);
        }

        public static void LoadLevel(TransitionSettings setting, string levelName)
        {
            CurrentLevel = levelName;
            TransitionManager.Instance().Transition(CurrentLevel, setting, 0f);
        }

        public static void Reload()
        {
            SceneManager.LoadScene(CurrentLevel);
        }
    }
}
