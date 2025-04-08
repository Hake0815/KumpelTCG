using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using NUnit.Framework.Constraints;
using UnityEngine.Android;

namespace gamecore.game
{
    public interface IGame
    {
        IPlayer Player1 { get; }
        IPlayer Player2 { get; }
        Dictionary<IPlayer, List<List<ICard>>> Mulligans { get; }
    }

    internal class Game : IGame, IActionPerformer<EndTurnGA>
    {
        public IPlayerLogic Player1 { get; private set; }
        public IPlayerLogic Player2 { get; private set; }
        IPlayer IGame.Player1
        {
            get => Player1;
        }
        IPlayer IGame.Player2
        {
            get => Player2;
        }

        public GameSetupBuilder GameSetupBuilder { get; private set; }
        public IGameState GameState { get; set; }
        public Dictionary<IPlayer, List<List<ICard>>> Mulligans
        {
            get
            {
                var result = new Dictionary<IPlayer, List<List<ICard>>>();
                foreach (var pair in GameSetupBuilder.Mulligans)
                {
                    var outerList = new List<List<ICard>>();
                    foreach (var innerList in pair.Value)
                    {
                        outerList.Add(innerList.Cast<ICard>().ToList());
                    }
                    result[pair.Key] = outerList;
                }
                return result;
            }
        }
        public event Action AwaitInteractionEvent;

        public Game(IPlayerLogic player1, IPlayerLogic player2)
        {
            Player1 = player1;
            Player2 = player2;
            ActionSystem.INSTANCE.AttachPerformer<EndTurnGA>(this);
            CardSystem.INSTANCE.Enable();
            GameState = new CreatedState();
        }

        public void PerformSetup()
        {
            GameSetupBuilder = new GameSetupBuilder().WithPlayer1(Player1).WithPlayer2(Player2);
            GameSetupBuilder.Setup();
            AdvanceGameState();
        }

        public void StartGame()
        {
            Player1.IsActive = true;
            ActionSystem.INSTANCE.Perform(new DrawCardGA(1, Player1));
        }

        public EndTurnGA Perform(EndTurnGA action)
        {
            return EndTurn(action);
        }

        public EndTurnGA EndTurn(EndTurnGA endTurnGA)
        {
            if (Player1.IsActive)
            {
                Player1.IsActive = false;
                Player2.IsActive = true;
                endTurnGA.NextPlayer = Player2;
            }
            else
            {
                Player2.IsActive = false;
                Player1.IsActive = true;
                endTurnGA.NextPlayer = Player1;
            }
            return endTurnGA;
        }

        public void EndGame()
        {
            ActionSystem.INSTANCE.DetachPerformer<EndTurnGA>();
            CardSystem.INSTANCE.Disable();
        }

        /* Returns if both active Pokemon are set */
        internal bool SetActivePokemon(ICardLogic basicPokemon)
        {
            basicPokemon.Play();
            if (Player1.ActivePokemon != null && Player2.ActivePokemon != null)
            {
                AdvanceGameState();
                return true;
            }
            return false;
        }

        public void AdvanceGameState()
        {
            GameState = GameState.AdvanceSuccesfully();
            GameState.OnEnter(this);
        }

        internal void PlayCard(ICardLogic card)
        {
            card.Play();
        }

        internal void AwaitInteraction()
        {
            AwaitInteractionEvent?.Invoke();
        }
    }
}
