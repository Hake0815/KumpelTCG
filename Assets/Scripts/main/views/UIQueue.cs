using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace gameview
{
    public class UIQueue
    {
        private static readonly Lazy<UIQueue> lazy = new(() => new UIQueue());
        public static UIQueue INSTANCE => lazy.Value;
        private UIQueue() { }

        private readonly Queue<Action<Action>> _actionQueue = new();

        private bool _idle = true;

        public void Queue(Action<Action> action)
        {
            _actionQueue.Enqueue(action);
            if (_idle) PerfromNextAction();
        }

        private void PerfromNextAction()
        {
            if (_idle && _actionQueue.TryDequeue(out Action<Action> action))
            {
                _idle = false;
                action.Invoke(() => { _idle = true; PerfromNextAction(); });
            }
        }

    }
}
