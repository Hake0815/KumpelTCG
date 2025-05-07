using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gamecore.card;

namespace gamecore.game.state
{
    internal class WaitForInputState : IGameState
    {
        public WaitForInputState(
            TaskCompletionSource<List<ICardLogic>> selectTask,
            IPlayerLogic player,
            List<ICardLogic> options,
            int amount
        )
        {
            _selectTask = selectTask;
            _player = player;
            _options = options;
            _amount = amount;
        }

        private readonly TaskCompletionSource<List<ICardLogic>> _selectTask;
        private readonly IPlayerLogic _player;
        private readonly List<ICardLogic> _options;
        private readonly int _amount;

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
                    new() { new TargetData(_amount, _options.Cast<ICard>().ToList()) }
                ),
            };
        }

        public Task OnAdvanced(Game game)
        {
            return Task.CompletedTask;
        }
    }
}
