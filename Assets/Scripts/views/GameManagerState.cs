using System;

namespace gameview
{
    public abstract class GameManagerState
    {
        private protected GameManager _gameManager;

        private protected GameManagerState(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public abstract GameManagerState AdvanceSuccessfully();
        internal abstract GameManagerState OnEnter();
    }

    public class MulliganStatePlayer1 : GameManagerState
    {
        internal MulliganStatePlayer1(GameManager gameManager)
            : base(gameManager) { }

        public override GameManagerState AdvanceSuccessfully()
        {
            return GameManagerStateFactory.CreateMulliganStatePlayer2(_gameManager);
        }

        internal override GameManagerState OnEnter()
        {
            if (_gameManager.ShowMulliganPlayer1())
                return this;
            else
                return AdvanceSuccessfully();
        }
    }

    public class MulliganStatePlayer2 : GameManagerState
    {
        internal MulliganStatePlayer2(GameManager gameManager)
            : base(gameManager) { }

        public override GameManagerState AdvanceSuccessfully()
        {
            return GameManagerStateFactory.CreateShowGameState(_gameManager);
        }

        internal override GameManagerState OnEnter()
        {
            if (_gameManager.ShowMulliganPlayer2())
                return this;
            else
                return AdvanceSuccessfully();
        }
    }

    public class ShowGameState : GameManagerState
    {
        internal ShowGameState(GameManager gameManager)
            : base(gameManager) { }

        public override GameManagerState AdvanceSuccessfully()
        {
            return GameManagerStateFactory.CreateStartGameState(_gameManager);
        }

        internal override GameManagerState OnEnter()
        {
            _gameManager.ShowGameState();
            return AdvanceSuccessfully();
        }
    }

    public class StartGameState : GameManagerState
    {
        internal StartGameState(GameManager gameManager)
            : base(gameManager) { }

        public override GameManagerState AdvanceSuccessfully()
        {
            return GameManagerStateFactory.CreateIdleState(_gameManager);
        }

        internal override GameManagerState OnEnter()
        {
            _gameManager.StartGame();
            return AdvanceSuccessfully();
        }
    }

    public class IdleState : GameManagerState
    {
        internal IdleState(GameManager gameManager)
            : base(gameManager) { }

        public override GameManagerState AdvanceSuccessfully()
        {
            return this;
        }

        internal override GameManagerState OnEnter()
        {
            return this;
        }
    }

    public class GameManagerStateFactory
    {
        public static GameManagerState CreateMulliganStatePlayer1(GameManager gameManager)
        {
            return EnterState(new MulliganStatePlayer1(gameManager));
        }

        public static GameManagerState CreateMulliganStatePlayer2(GameManager gameManager)
        {
            return EnterState(new MulliganStatePlayer2(gameManager));
        }

        public static GameManagerState CreateShowGameState(GameManager gameManager)
        {
            return EnterState(new ShowGameState(gameManager));
        }

        public static GameManagerState CreateStartGameState(GameManager gameManager)
        {
            return EnterState(new StartGameState(gameManager));
        }

        public static GameManagerState CreateIdleState(GameManager gameManager)
        {
            return EnterState(new IdleState(gameManager));
        }

        private static GameManagerState EnterState(GameManagerState state)
        {
            return state.OnEnter();
        }
    }
}
