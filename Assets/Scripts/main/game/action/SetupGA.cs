using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    class SetupGA : GameAction
    {
        public Dictionary<string, List<List<ICardLogic>>> Mulligans { get; set; }
        public Dictionary<string, List<ICardLogic>> PlayerHands { get; set; }
    }
}
