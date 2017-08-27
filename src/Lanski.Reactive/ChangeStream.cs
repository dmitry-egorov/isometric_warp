using System;
using System.Collections.Generic;
using Lanski.Structures;

namespace Lanski.Reactive
{
    public class ChangeStream<T> : IStream<TheVoid>
    {
        private readonly Stream<TheVoid> _stream = new Stream<TheVoid>();

        private Parameters? _last;

        public void Update(T p0)
        {
            var p = new Parameters(p0);
            
            if(_last.HasValue && _last.Value.Equals(p))
                return;

            _last = p;
            _stream.Next(TheVoid.Instance);
        }

        public Action Subscribe(Action<TheVoid> action)
        {
            return _stream.Subscribe(action);
        }
        
        private struct Parameters
        {
            private readonly T _p;

            public Parameters(T p)
            {
                _p = p;
            }

            public bool Equals(Parameters other)
            {
                return EqualityComparer<T>.Default.Equals(_p, other._p);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is Parameters && Equals((Parameters) obj);
            }

            public override int GetHashCode()
            {
                return EqualityComparer<T>.Default.GetHashCode(_p);
            }
        }
    }
    
    public class ChangeStream<T0, T1> : IStream<TheVoid>
    {
        private readonly Stream<TheVoid> _stream = new Stream<TheVoid>();

        private Parameters? _last;

        public void Update(T0 p0, T1 p1)
        {
            var p = new Parameters(p0, p1);
            
            if(_last.HasValue && _last.Value.Equals(p))
                return;

            _last = p;
            _stream.Next(TheVoid.Instance);
        }

        public Action Subscribe(Action<TheVoid> action)
        {
            return _stream.Subscribe(action);
        }
        
        private struct Parameters
        {
            private readonly T0 _p0;
            private readonly T1 _p1;

            public Parameters(T0 p0, T1 p1)
            {
                _p0 = p0;
                _p1 = p1;
            }

            public bool Equals(Parameters other)
            {
                return EqualityComparer<T0>.Default.Equals(_p0, other._p0) && EqualityComparer<T1>.Default.Equals(_p1, other._p1);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is Parameters && Equals((Parameters) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (EqualityComparer<T0>.Default.GetHashCode(_p0) * 397) ^ EqualityComparer<T1>.Default.GetHashCode(_p1);
                }
            }
        }
    }
    
    public class ChangeStream<T0, T1, T2> : IStream<TheVoid>
    {
        private readonly Stream<TheVoid> _stream = new Stream<TheVoid>();

        private Parameters? _last;

        public void Update(T0 p0, T1 p1, T2 p2)
        {
            var p = new Parameters(p0, p1, p2);
            
            if(_last.HasValue && _last.Value.Equals(p))
                return;

            _last = p;
            _stream.Next(TheVoid.Instance);
        }

        public Action Subscribe(Action<TheVoid> action)
        {
            return _stream.Subscribe(action);
        }
        
        private struct Parameters
        {
            private readonly T0 _p0;
            private readonly T1 _p1;
            private readonly T2 _p2;

            public Parameters(T0 p0, T1 p1, T2 p2)
            {
                _p0 = p0;
                _p1 = p1;
                _p2 = p2;
            }

            public bool Equals(Parameters other)
            {
                return EqualityComparer<T0>.Default.Equals(_p0, other._p0) && EqualityComparer<T1>.Default.Equals(_p1, other._p1) && EqualityComparer<T2>.Default.Equals(_p2, other._p2);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is Parameters && Equals((Parameters) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = EqualityComparer<T0>.Default.GetHashCode(_p0);
                    hashCode = (hashCode * 397) ^ EqualityComparer<T1>.Default.GetHashCode(_p1);
                    hashCode = (hashCode * 397) ^ EqualityComparer<T2>.Default.GetHashCode(_p2);
                    return hashCode;
                }
            }
        }
    }
    
    public class ChangeStream<T0, T1, T2, T3> : IStream<TheVoid>
    {
        private readonly Stream<TheVoid> _stream = new Stream<TheVoid>();

        private Parameters? _last;

        public void Update(T0 p0, T1 p1, T2 p2, T3 p3)
        {
            var p = new Parameters(p0, p1, p2, p3);
            
            if(_last.HasValue && _last.Value.Equals(p))
                return;

            _last = p;
            _stream.Next(TheVoid.Instance);
        }

        public Action Subscribe(Action<TheVoid> action)
        {
            return _stream.Subscribe(action);
        }
        
        private struct Parameters
        {
            private readonly T0 _p0;
            private readonly T1 _p1;
            private readonly T2 _p2;
            private readonly T3 _p3;

            public Parameters(T0 p0, T1 p1, T2 p2, T3 p3)
            {
                _p0 = p0;
                _p1 = p1;
                _p2 = p2;
                _p3 = p3;
            }

            public bool Equals(Parameters other)
            {
                return EqualityComparer<T0>.Default.Equals(_p0, other._p0) && EqualityComparer<T1>.Default.Equals(_p1, other._p1) && EqualityComparer<T2>.Default.Equals(_p2, other._p2) && EqualityComparer<T3>.Default.Equals(_p3, other._p3);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is Parameters && Equals((Parameters) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = EqualityComparer<T0>.Default.GetHashCode(_p0);
                    hashCode = (hashCode * 397) ^ EqualityComparer<T1>.Default.GetHashCode(_p1);
                    hashCode = (hashCode * 397) ^ EqualityComparer<T2>.Default.GetHashCode(_p2);
                    hashCode = (hashCode * 397) ^ EqualityComparer<T3>.Default.GetHashCode(_p3);
                    return hashCode;
                }
            }
        }
    }
}