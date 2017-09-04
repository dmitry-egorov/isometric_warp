using System;
using Lanski.Structures;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    internal struct ChangeSignal<T>: IDisposable
    {
        private readonly Curry<T> _curry;

        public ChangeSignal(Curry<T> curry)
        {
            _curry = curry;
        }

        public void Invoke() => _curry.Invoke();
        
        public static implicit operator ChangeSignal<T>(Curry<T> curry) => new ChangeSignal<T>(curry);

        public void Dispose()
        {
            Invoke();
        }
    }
}