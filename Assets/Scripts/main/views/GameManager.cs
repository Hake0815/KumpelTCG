using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.common;
using gamecore.game;
using gamecore.game.action;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace gameview
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private HandView _handViewPrefab;

        [SerializeField]
        private DeckView _deckViewPrefab;

        [SerializeField]
        private PlayArea _playAreaPrefab;

        [SerializeField]
        private ActiveSpot _activeSpotPrefab;

        [SerializeField]
        private BenchView _benchViewPrefab;

        [SerializeField]
        private CardViewCreator _cardViewCreatorPrefab;

        [SerializeField]
        private InputHandler _inputHandlerPrefab;

        [SerializeField]
        private DiscardPileView _discardPileViewPrefab;

        [SerializeField]
        private MulliganView _mulliganViewPrefab;

        [SerializeField]
        private MulliganSelectorView _mulliganSelectorViewPrefab;
        private MulliganSelectorView _mulliganSelectorView;

        [SerializeField]
        private EndGameView _endGameViewPrefab;

        [SerializeField]
        private PrizeView _prizeViewPrefab;

        [SerializeField]
        private FloatingSelectionView _floatingSelectionViewPrefab;
        private FloatingSelectionView _floatingSelectionView;

        [SerializeField]
        private SearchView _searchViewPrefab;
        private SearchView _searchView;
        private readonly Dictionary<IPlayer, HandView> _playerHandViews = new();
        private readonly Dictionary<IPlayer, DeckView> _playerDeckViews = new();
        private readonly Dictionary<IPlayer, PrizeView> _playerPrizeViews = new();
        private readonly Dictionary<IPlayer, BenchView> _playerBenchViews = new();
        private readonly Dictionary<IPlayer, DiscardPileView> _playerDiscardPileViews = new();
        public Dictionary<IPlayer, ActiveSpot> PlayerActiveSpots { get; } = new();

        private Button _button;
        private TMP_Text _buttonText;

        async Task Start()
        {
            _button = GetComponentInChildren<Button>();
            _buttonText = _button.GetComponentInChildren<TMP_Text>();
            DisableButton();
            await InstantiateAsync(_cardViewCreatorPrefab);
            await InstantiateAsync(_inputHandlerPrefab);
            _floatingSelectionView = (await InstantiateAsync(_floatingSelectionViewPrefab))[0];
            _searchView = (await InstantiateAsync(_searchViewPrefab))[0];

            var remoteService = new GameRemoteService(this);
            await remoteService.InitializeGame();
        }

        public void SetUpPlayerViews(IPlayer player1, IPlayer player2)
        {
            SetUpPlayerViews(player1, new Quaternion(0f, 0f, 0f, 1f));
            SetUpPlayerViews(player2, new Quaternion(0f, 0f, 1f, 0f));
            DisablePlayerHandViews();
        }

        public void EnablePlayerHandViews()
        {
            foreach (var handView in _playerHandViews.Values)
            {
                handView.gameObject.SetActive(true);
            }
            foreach (var deckView in _playerDeckViews.Values)
            {
                deckView.gameObject.SetActive(true);
            }
        }

        private void DisablePlayerHandViews()
        {
            foreach (var handView in _playerHandViews.Values)
            {
                handView.gameObject.SetActive(false);
            }
            foreach (var deckView in _playerDeckViews.Values)
            {
                deckView.gameObject.SetActive(false);
            }
        }

        private void SetUpPlayerViews(IPlayer player, Quaternion rotation)
        {
            SetUpDiscardPileView(player, rotation);
            SetUpDeckView(player, rotation);
            SetUpHandView(player, rotation);
            SetUpPlayArea(player, rotation);
            SetUpActiveSpot(player, rotation);
            SetUpBenchView(player, rotation);
            SetUpPrizeView(player, rotation);
        }

        private void SetUpDiscardPileView(IPlayer player, Quaternion rotation)
        {
            var discardPileView = Instantiate(
                _discardPileViewPrefab,
                rotation * _discardPileViewPrefab.transform.position,
                rotation
            );
            discardPileView.SetUp(player.DiscardPile);
            _playerDiscardPileViews.Add(player, discardPileView);
        }

        private void SetUpDeckView(IPlayer player, Quaternion rotation)
        {
            var deckView = Instantiate(
                _deckViewPrefab,
                rotation * _deckViewPrefab.transform.position,
                rotation
            );
            deckView.SetUp(player);
            _playerDeckViews.Add(player, deckView);
        }

        private void SetUpHandView(IPlayer player, Quaternion rotation)
        {
            var handView = Instantiate(
                _handViewPrefab,
                _handViewPrefab.transform.position,
                rotation
            ); // Position is at 0,0,0
            handView.Register(player);
            _playerHandViews.Add(player, handView);
        }

        private void SetUpPlayArea(IPlayer player, Quaternion rotation)
        {
            var playArea = Instantiate(
                _playAreaPrefab,
                rotation * _playAreaPrefab.transform.position,
                rotation
            );
            playArea.SetUp(player);
        }

        private void SetUpActiveSpot(IPlayer player, Quaternion rotation)
        {
            var activeSpot = Instantiate(
                _activeSpotPrefab,
                rotation * _activeSpotPrefab.transform.position,
                rotation
            );
            activeSpot.SetUp(player);
            PlayerActiveSpots.Add(player, activeSpot);
        }

        private void SetUpBenchView(IPlayer player, Quaternion rotation)
        {
            var benchView = Instantiate(
                _benchViewPrefab,
                rotation * _benchViewPrefab.transform.position,
                rotation
            );
            benchView.SetUp(player);
            _playerBenchViews.Add(player, benchView);
        }

        private void SetUpPrizeView(IPlayer player, Quaternion rotation)
        {
            var prizeView = Instantiate(
                _prizeViewPrefab,
                rotation * _prizeViewPrefab.transform.position,
                rotation
            );
            prizeView.SetUp(player);
            _playerPrizeViews.Add(player, prizeView);
        }

        public async Task ShowMulligan(IPlayer player, List<List<ICard>> mulligans, Action onDone)
        {
            if (mulligans.Count == 0)
            {
                onDone();
                return;
            }
            await UIQueue.INSTANCE.Queue(() => CreateMulliganView(player, mulligans, onDone));
        }

        private Task CreateMulliganView(IPlayer player, List<List<ICard>> mulligans, Action onDone)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            var mulliganView = Instantiate(_mulliganViewPrefab);
            mulliganView.SetUp(player, mulligans);
            mulliganView.AddDoneListener(() =>
            {
                onDone();
                taskCompletionSource.SetResult(true);
            });
            return taskCompletionSource.Task;
        }

        public async Task ShowGameState()
        {
            foreach (var player in _playerHandViews.Keys)
            {
                await ShowHandCards(player);
                player.ActivePokemon?.Let(
                    async (activePokemon) => await ShowActivePokemon(player, activePokemon)
                );
                await ShowPrizeCards(player);
                await ShowBenchedPokemon(player);
                ShowDiscardPile(player);
                _playerDeckViews[player].UpdateView();
            }
        }

        private async Task ShowHandCards(IPlayer player)
        {
            await _playerDeckViews[player].CreateDrawnCards(player.Hand.Cards);
            await _playerHandViews[player].HandleCardCountChanged();
        }

        private async Task ShowActivePokemon(IPlayer player, IPokemonCard activePokemon)
        {
            await _playerDeckViews[player].CreateDrawnCards(new() { activePokemon });
            await PlayerActiveSpots[player].SetActivePokemon(activePokemon);
            if (activePokemon.AttachedEnergyCards.Count > 0)
                await ShowAttachedEnergyCards(activePokemon);
        }

        private async Task ShowBenchedPokemon(IPlayer player)
        {
            await _playerDeckViews[player].CreateDrawnCards(player.Bench.Cards);
            await _playerBenchViews[player].UpdateBenchedPokemonPositions();
            foreach (var pokemon in player.Bench.Cards)
            {
                if ((pokemon as IPokemonCard).AttachedEnergyCards.Count > 0)
                {
                    await ShowAttachedEnergyCards(pokemon as IPokemonCard);
                }
            }
        }

        private async Task ShowAttachedEnergyCards(IPokemonCard card)
        {
            var attachedEnergyCards = card.AttachedEnergyCards;
            await _playerDeckViews[card.Owner]
                .CreateDrawnCards(attachedEnergyCards.Cast<ICard>().ToList());
            await UIQueue.INSTANCE.Queue(async () =>
            {
                await CardViewRegistry.INSTANCE.Get(card).AttachEnergy(attachedEnergyCards);
            });
        }

        private async Task ShowPrizeCards(IPlayer player)
        {
            await _playerDeckViews[player].CreateDrawnCardsFaceDown(player.Prizes.Cards);
            await _playerPrizeViews[player].UpdateView();
        }

        private void ShowDiscardPile(IPlayer player)
        {
            _playerDiscardPileViews[player].UpdateView();
        }

        public void EnableEndTurnButton(Action gameControllerMethod, Action onInteract)
        {
            _button.gameObject.SetActive(true);
            _buttonText.text = "EndTurn";
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() =>
            {
                onInteract();
                gameControllerMethod();
            });
        }

        public void EnableButtonWithText(string text, Action onButtonClick)
        {
            _button.gameObject.SetActive(true);
            _buttonText.text = text;
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => onButtonClick());
        }

        public void DisableButton()
        {
            _button.onClick.RemoveAllListeners();
            _button.gameObject.SetActive(false);
        }

        internal void AddOptionToMulliganSelector(int mulliganOption, Action onConfirm)
        {
            GetMulliganSelector().AddOption(mulliganOption.ToString(), onConfirm);
        }

        private MulliganSelectorView GetMulliganSelector()
        {
            if (_mulliganSelectorView == null)
                _mulliganSelectorView = Instantiate(_mulliganSelectorViewPrefab);

            return _mulliganSelectorView;
        }

        internal Button EnableDoneButton(Action onClicked)
        {
            _button.gameObject.SetActive(true);
            _buttonText.text = "Done";
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => onClicked.Invoke());
            return _button;
        }

        internal void ShowGameOver(IPlayer winner)
        {
            var endGameView = Instantiate(_endGameViewPrefab);
            endGameView.ShowWinner(winner);
        }

        internal void ActivateFloatingSelectionView(List<ICard> possibleTargets)
        {
            _floatingSelectionView.DisplayCards(possibleTargets);
        }

        internal void DisableFloatingSelection()
        {
            _floatingSelectionView.Clear();
        }

        internal void ActivateSearchView(List<ICard> cards, List<ICard> possibleTargets)
        {
            _searchView.DisplayCards(cards, possibleTargets);
        }

        internal void DisableSearchView()
        {
            _searchView.Clear();
        }
    }
}
