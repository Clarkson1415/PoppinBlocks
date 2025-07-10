using EasyTransition;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public static class GameLevels
    {
        public static List<string> LevelNames = new() { "TitleScreen", "Level 1", "Level 2", "Level 3", "Level 4", "Level 5", "Level 6", "Level 7", "Level 8", "Level 9", "End" };

        public static string GetNextLevel()
        {
            var currentLevelIndex = LevelNames.IndexOf(CurrentLevel);
            return LevelNames[currentLevelIndex + 1];
        }

        private static string CurrentLevel = SceneManager.GetActiveScene().name;

        public static void LevelCompleted(TransitionSettings setting)
        {
            CurrentLevel = GetNextLevel();
            LoadLevel(setting, CurrentLevel);
        }

        public static void LoadLevel(TransitionSettings setting, string levelName)
        {
            CurrentLevel = levelName;
            TransitionManager.Instance().Transition(CurrentLevel, setting, 0f);
        }

        public static void Reload(TransitionSettings setting)
        {
            LoadLevel(setting, CurrentLevel);
        }
    }
}
