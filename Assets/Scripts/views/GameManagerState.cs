using System;

namespace gameview
{
    public abstract class GameManagerState
    {
        private protected GameManager _gameManager;

        protected GameManagerState(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public abstract GameManagerState AdvanceSuccessfully();
        public abstract void OnEnter();
    }

    public class MulliganStatePlayer1 : GameManagerState
    {
        public MulliganStatePlayer1(GameManager gameManager)
            : base(gameManager) { }

        public override GameManagerState AdvanceSuccessfully()
        {
            return new MulliganStatePlayer2(_gameManager);
        }

        public override void OnEnter()
        {
            _gameManager.ShowMulliganPlayer1();
        }
    }

    public class MulliganStatePlayer2 : GameManagerState
    {
        public MulliganStatePlayer2(GameManager gameManager)
            : base(gameManager) { }

        public override GameManagerState AdvanceSuccessfully()
        {
            return new ShowGameState(_gameManager);
        }

        public override void OnEnter()
        {
            _gameManager.ShowMulliganPlayer2();
        }
    }

    public class ShowGameState : GameManagerState
    {
        public ShowGameState(GameManager gameManager)
            : base(gameManager) { }

        public override GameManagerState AdvanceSuccessfully()
        {
            return new StartGameState(_gameManager);
        }

        public override void OnEnter()
        {
            _gameManager.ShowGameState();
        }
    }

    public class StartGameState : GameManagerState
    {
        public StartGameState(GameManager gameManager)
            : base(gameManager) { }

        public override GameManagerState AdvanceSuccessfully()
        {
            return new IdleState(_gameManager);
        }

        public override void OnEnter()
        {
            _gameManager.StartGame();
        }
    }

    public class IdleState : GameManagerState
    {
        public IdleState(GameManager gameManager)
            : base(gameManager) { }

        public override GameManagerState AdvanceSuccessfully()
        {
            return this;
        }

        public override void OnEnter() { }
    }

    // public class GameManagerStateFactory
    // {
    //     public static GameManagerState CreateMulliganStatePlayer1(GameManager gameManager)
    //     {
    //         return SetState(new MulliganStatePlayer1(gameManager));
    //     }

    //     public static GameManagerState CreateMulliganStatePlayer2(GameManager gameManager)
    //     {
    //         return SetState(new MulliganStatePlayer2(gameManager));
    //     }

    //     public static GameManagerState CreateShowGameState(GameManager gameManager)
    //     {
    //         return SetState(new ShowGameState(gameManager));
    //     }

    //     public static GameManagerState CreateStartGameState(GameManager gameManager)
    //     {
    //         return SetState(new StartGameState(gameManager));
    //     }

    //     public static GameManagerState CreateIdleState(GameManager gameManager)
    //     {
    //         return SetState(new IdleState(gameManager));
    //     }

    //     private static GameManagerState SetState(GameManagerState state)
    //     {
    //         state.OnEnter();
    //         return state;
    //     }
    // }
}
