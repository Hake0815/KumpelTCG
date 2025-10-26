using System.IO;
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
        private Button _replayGameButton;

        [SerializeField]
        private Button _quitButton;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            _quitButton.onClick.AddListener(Quit);
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
