using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace gameview
{
    public class UIQueue
    {
        private static readonly Lazy<UIQueue> lazy = new(() => new UIQueue());
        public static UIQueue INSTANCE => lazy.Value;

        private UIQueue() { }

        private readonly Queue<Func<Task>> _actionQueue = new();

        private bool _idle = true;

        public async Task Queue(Func<Task> action)
        {
            _actionQueue.Enqueue(action);
            if (_idle)
                await PerfromNextAction();
        }

        private async Task PerfromNextAction()
        {
            if (_idle && _actionQueue.TryDequeue(out Func<Task> action))
            {
                _idle = false;
                await action.Invoke();
                _idle = true;
                await PerfromNextAction();
            }
        }
    }
}
