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
                _loadGameButton.interactable = false;
            _newGameButton.onClick.AddListener(NewGame);
            _loadGameButton.onClick.AddListener(LoadGame);
            _quitButton.onClick.AddListener(Quit);
        }

        private void NewGame()
        {
            GameParameters.LoadModus = LoadModus.NewGame;
            SceneManager.LoadScene("GameScene");
        }

        private void LoadGame()
        {
            GameParameters.LoadModus = LoadModus.ResumeGame;
            SceneManager.LoadScene("GameScene");
        }

        private void Quit()
        {
            Application.Quit();
        }
    }
}
