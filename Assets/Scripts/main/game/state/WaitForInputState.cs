using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gamecore.card;

namespace gamecore.game.state
{
    class WaitForInputState : IGameState
    {
        public WaitForInputState(
            TaskCompletionSource<List<ICardLogic>> selectTask,
            IPlayerLogic player,
            List<ICardLogic> options,
            Predicate<List<ICard>> selectionCondition,
            SelectFrom selectFrom,
            bool isQuickSelection
        )
        {
            _selectTask = selectTask;
            _player = player;
            _options = options;
            _selectFrom = selectFrom;
            _selectionCondition = selectionCondition;
            _isQuickSelection = isQuickSelection;
        }

        private readonly TaskCompletionSource<List<ICardLogic>> _selectTask;
        private readonly IPlayerLogic _player;
        private readonly List<ICardLogic> _options;
        private readonly Predicate<List<ICard>> _selectionCondition;
        private readonly SelectFrom _selectFrom;
        private readonly bool _isQuickSelection;

        public IGameState AdvanceSuccesfully()
        {
            return new IdlePlayerTurnState();
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            if (player != _player)
                return new();
            return CreateSelectCardsInteraction();
        }

        private List<GameInteraction> CreateSelectCardsInteraction()
        {
            return new()
            {
                new GameInteraction(
                    targets => _selectTask.SetResult(targets.Cast<ICardLogic>().ToList()),
                    GameInteractionType.SelectCards,
                    new()
                    {
                        new ConditionalTargetData(
                            _selectionCondition,
                            _options.Cast<ICard>().ToList(),
                            _isQuickSelection
                        ),
                        new SelectFromData(_selectFrom),
                    }
                ),
            };
        }

        public void OnAdvanced(Game game) { }
    }
}
