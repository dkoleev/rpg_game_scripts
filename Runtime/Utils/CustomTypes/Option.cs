namespace Darkness.Runtime.Utils.CustomTypes
{
    using System;
    using OneOf;
    using OneOf.Types;

    public class Option<T>
    {
        private readonly OneOf<T, None> _value;

        private Option(OneOf<T, None> value)
        {
            _value = value;
        }

        public static Option<T> Some(T val) => new (val);

        public static Option<T> None() => new (new None());

        public bool IsSome => _value.IsT0;

        public bool IsNone => _value.IsT1;

        public T Unwrap()
        {
            if (IsSome) return _value.AsT0;
            throw new InvalidOperationException("Option is None");
        }

        public T UnwrapOr(T defaultValue)
        {
            return IsSome ? _value.AsT0 : defaultValue;
        }

        public void Match(Action<T> someAction, Action noneAction)
        {
            _value.Switch(
                someAction,
                _ => noneAction()
            );
        }

        public TResult Match<TResult>(Func<T, TResult> someFunc, Func<None, TResult> noneFunc)
        {
            return _value.Match(
                someFunc,
                noneFunc
            );
        }
    }

}