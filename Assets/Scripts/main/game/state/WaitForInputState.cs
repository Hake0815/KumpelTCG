using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gamecore.card;
using gamecore.game.interaction;
using gamecore.serialization;

namespace gamecore.game.state
{
    class WaitForInputState : IGameState
    {
        public WaitForInputState(
            TaskCompletionSource<List<ICardLogic>> selectTask,
            IPlayerLogic player,
            List<ICardLogic> options,
            ConditionalTargetQuery conditionalTargetQuery,
            SelectFrom selectFrom,
            ActionOnSelection targetAction,
            ActionOnSelection remainderAction,
            bool isQuickSelection
        )
        {
            _selectTask = selectTask;
            _player = player;
            _options = options;
            _selectFrom = selectFrom;
            _conditionalTargetQuery = conditionalTargetQuery;
            _isQuickSelection = isQuickSelection;
            _targetAction = targetAction;
            _remainderAction = remainderAction;
        }

        private readonly TaskCompletionSource<List<ICardLogic>> _selectTask;
        private readonly IPlayerLogic _player;
        private readonly List<ICardLogic> _options;
        private readonly ConditionalTargetQuery _conditionalTargetQuery;
        private readonly SelectFrom _selectFrom;
        private readonly bool _isQuickSelection;
        private readonly ActionOnSelection _targetAction;
        private readonly ActionOnSelection _remainderAction;

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
            var selectFromData = _selectFrom switch
            {
                SelectFrom.Deck => new SelectFromData(
                    _selectFrom,
                    _player.Deck.Cards.Cast<ICard>().ToList()
                ),
                SelectFrom.DiscardPile => new SelectFromData(
                    _selectFrom,
                    _player.DiscardPile.Cards.Cast<ICard>().ToList()
                ),
                _ => new SelectFromData(_selectFrom),
            };
            return new()
            {
                new GameInteraction(
                    targets => _selectTask.SetResult(targets.Cast<ICardLogic>().ToList()),
                    GameInteractionType.SelectCards,
                    new()
                    {
                        new ConditionalTargetData(
                            conditionalTargetQuery: _conditionalTargetQuery,
                            possibleTargets: _options.Cast<ICard>().ToList(),
                            targetAction: _targetAction,
                            remainderAction: _remainderAction,
                            isQuickSelection: _isQuickSelection
                        ),
                        selectFromData,
                    }
                ),
            };
        }

        public void OnAdvanced(Game game) { }

        public ProtoBufTechnicalGameState ToProtoBuf()
        {
            return ProtoBufTechnicalGameState.GameStateWaitForInput;
        }
    }
}
