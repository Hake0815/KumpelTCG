using System;
using System.IO;
using System.Linq;
using gamecore.common;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace gameview
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private Button _newGameButton;

        [SerializeField]
        private Button _loadGameButton;

        [SerializeField]
        private Button _recreateStateButton;

        [SerializeField]
        private Button _replayGameButton;

        [SerializeField]
        private Button _quitButton;

        void Start()
        {
            if (
                !(
                    File.Exists(GameParameters.GAME_LOG_FILE)
                    && File.ReadAllText(GameParameters.GAME_LOG_FILE).Length > 0
                )
            )
            {
                _loadGameButton.interactable = false;
                _replayGameButton.interactable = false;
            }
            _newGameButton.onClick.AddListener(NewGame);
            _loadGameButton.onClick.AddListener(LoadGame);
            _replayGameButton.onClick.AddListener(ReplayGame);
            _recreateStateButton.onClick.AddListener(RecreateState);
            _quitButton.onClick.AddListener(Quit);
        }

        private void RecreateState()
        {
            var paths = StandaloneFileBrowser.OpenFilePanel("Open game state file", "", "", false);
            var recreatableGameState = File.ReadLines(paths[0])
                .Where(line => line.StartsWith("{\"Recreatable\":true"))
                .Last();
            GameParameters.GameState = recreatableGameState;
            GameParameters.LoadModus = LoadModus.RecreateState;
            SceneManager.LoadScene("GameScene");

            _newGameButton.gameObject.SetActive(false);
            _loadGameButton.gameObject.SetActive(false);
            _replayGameButton.gameObject.SetActive(false);
            _recreateStateButton.gameObject.SetActive(false);
            _quitButton.gameObject.SetActive(false);
        }

        private static void NewGame()
        {
            GameParameters.LoadModus = LoadModus.NewGame;
            SceneManager.LoadScene("GameScene");
        }

        private static void LoadGame()
        {
            GameParameters.LoadModus = LoadModus.ResumeGame;
            SceneManager.LoadScene("GameScene");
        }

        private static void ReplayGame()
        {
            GameParameters.LoadModus = LoadModus.ReplayGame;
            SceneManager.LoadScene("GameScene");
        }

        private static void Quit()
        {
            Application.Quit();
        }
    }
}
