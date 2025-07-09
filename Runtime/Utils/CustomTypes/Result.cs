using System;

namespace Darkness.Runtime.Utils.CustomTypes
{
    public struct Result<T>
    {
        public readonly T Value { get; }
        public readonly string Error { get; }
        public readonly bool IsSuccess { get; }

        private Result(T value, string error, bool isSuccess)
        {
            Value = value;
            Error = error;
            IsSuccess = isSuccess;
        }

        public static Result<T> Success(T value) => new Result<T>(value, null, true);
        public static Result<T> Fail(string error) => new Result<T>(default, error, false);

        public void Match(Action<T> onSuccess, Action<string> onFailure)
        {
            if (IsSuccess)
                onSuccess?.Invoke(Value);
            else
                onFailure?.Invoke(Error);
        }

        public Result<TOut> Then<TOut>(Func<T, Result<TOut>> func)
        {
            return IsSuccess ? func(Value) : Result<TOut>.Fail(Error);
        }

        public T ValueOr(T defaultValue) => IsSuccess ? Value : defaultValue;
    }}