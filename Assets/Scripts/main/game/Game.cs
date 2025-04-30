using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Threading.Tasks;
using gamecore.action;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using UnityEngine;
using UnityEngine.Android;

namespace gamecore.game
{
    public interface IGame
    {
        IPlayer Player1 { get; }
        IPlayer Player2 { get; }
        Dictionary<IPlayer, List<List<ICard>>> Mulligans { get; }
        int TurnCounter { get; }
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
        private List<IPlayerLogic> _players = new();

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
        public int TurnCounter { get; private set; } = 0;
        public event Action AwaitInteractionEvent;
        public event Action AwaitGeneralInteractionEvent;

        public Game(IPlayerLogic player1, IPlayerLogic player2)
        {
            Player1 = player1;
            Player2 = player2;
            ActionSystem.INSTANCE.AttachPerformer<EndTurnGA>(this);
            CardSystem.INSTANCE.Enable();
            DamageSystem.INSTANCE.Enable();
            GeneralMechnicSystem.INSTANCE.Enable(this);
            GameState = new CreatedState();
            _players.Add(player1);
            _players.Add(player2);
        }

        public void PerformSetup()
        {
            GameSetupBuilder = new GameSetupBuilder().WithPlayer1(Player1).WithPlayer2(Player2);
            GameSetupBuilder.Setup();
            AdvanceGameState();
        }

        public void StartGame()
        {
            TurnCounter++;
            Player1.IsActive = true;
            ActionSystem.INSTANCE.Perform(new DrawCardGA(1, Player1));
        }

        public void EndTurn()
        {
            var endTurn = new EndTurnGA();
            ActionSystem.INSTANCE.Perform(endTurn);
            AwaitInteraction();
        }

        public EndTurnGA Perform(EndTurnGA endTurnGA)
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
            TurnCounter++;
            return endTurnGA;
        }

        public void EndGame(IPlayerLogic winner)
        {
            ActionSystem.INSTANCE.DetachPerformer<EndTurnGA>();
            CardSystem.INSTANCE.Disable();
            DamageSystem.INSTANCE.Disable();
            GeneralMechnicSystem.INSTANCE.Disable();
            GameState = new GameOverState(winner);
            AwaitGeneralInteraction();
        }

        /* Returns if both active Pokemon are set */
        internal void SetActivePokemon(ICardLogic basicPokemon)
        {
            basicPokemon.Play();
            AdvanceGameState();
        }

        public void AdvanceGameState()
        {
            GameState = GameState.AdvanceSuccesfully();
            Debug.Log("Game state advanced to: " + GameState.GetType().Name);
            GameState.OnAdvanced(this);
        }

        internal void PlayCard(ICardLogic card)
        {
            card.Play();
            AwaitInteraction();
        }

        internal virtual void AwaitInteraction()
        {
            AwaitInteractionEvent?.Invoke();
        }

        internal virtual void AwaitGeneralInteraction()
        {
            AwaitGeneralInteractionEvent?.Invoke();
        }

        internal void DrawMulliganCards(int numberOfExtraCards, IPlayerLogic player)
        {
            ActionSystem.INSTANCE.Perform(new DrawCardGA(numberOfExtraCards, player));
            AdvanceGameState();
        }

        internal void SetPrizeCards()
        {
            foreach (var player in _players)
            {
                player.SetPrizeCards();
            }
        }

        internal void PlayCardWithTargets(ICardLogic card, List<ICardLogic> targets)
        {
            card.PlayWithTargets(targets);
            AwaitInteraction();
        }

        internal void PerformAttack(IAttackLogic attack, IPokemonCardLogic attacker)
        {
            ActionSystem.INSTANCE.Perform(new AttackGA(attack, attacker));
            AwaitInteraction();
        }
    }
}
