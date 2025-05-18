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
            int amount,
            SelectFrom selectFrom
        )
        {
            _selectTask = selectTask;
            _player = player;
            _options = options;
            _amount = amount;
            _selectFrom = selectFrom;
        }

        private readonly TaskCompletionSource<List<ICardLogic>> _selectTask;
        private readonly IPlayerLogic _player;
        private readonly List<ICardLogic> _options;
        private readonly int _amount;
        private readonly SelectFrom _selectFrom;

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
            return new()
            {
                new GameInteraction(
                    targets => _selectTask.SetResult(targets.Cast<ICardLogic>().ToList()),
                    GameInteractionType.SelectCards,
                    new()
                    {
                        new TargetData(_amount, _options.Cast<ICard>().ToList()),
                        new SelectFromData(_selectFrom),
                    }
                ),
            };
        }

        public Task OnAdvanced(Game game)
        {
            return Task.CompletedTask;
        }
    }
}
