namespace Core.Structures
{
    using System;

    public class OnceAction
    {
        private readonly Action _action;
        private bool _done;

        public OnceAction(Action action, bool done = false)
        {
            _action = action;
            _done = done;
        }

        public void TryExecute()
        {
            if (_done)
                return;

            _action();
            _done = true;
        }

        public void Prime()
        {
            _done = false;
        }

        public void Cancel()
        {
            _done = true;
        }
    }
}