using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MashupWebApi.Extesions
{
    public struct Result : ISerializable
    {
        private static readonly Result OkResult = new Result(false, (string)null);
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ResultCommonLogic _logic;

        void ISerializable.GetObjectData(
          SerializationInfo oInfo,
          StreamingContext oContext)
        {
            this._logic.GetObjectData(oInfo, oContext);
        }

        public bool IsFailure => this._logic.IsFailure;

        public bool IsSuccess => this._logic.IsSuccess;

        public string Error => this._logic.Error;

        [DebuggerStepThrough]
        private Result(bool isFailure, string error) => this._logic = ResultCommonLogic.Create(isFailure, error);

        [DebuggerStepThrough]
        public static Result Ok() => Result.OkResult;

        [DebuggerStepThrough]
        public static Result Fail(string error) => new Result(true, error);

        [DebuggerStepThrough]
        public static Result<T> Ok<T>(T value) => new Result<T>(false, value, (string)null);

        [DebuggerStepThrough]
        public static Result<T> Fail<T>(string error) => new Result<T>(true, default(T), error);

        [DebuggerStepThrough]
        public static Result<TValue, TError> Ok<TValue, TError>(TValue value) where TError : class => new Result<TValue, TError>(false, value, default(TError));

        [DebuggerStepThrough]
        public static Result<TValue, TError> Fail<TValue, TError>(TError error) where TError : class => new Result<TValue, TError>(true, default(TValue), error);

        [DebuggerStepThrough]
        public static Result FirstFailureOrSuccess(params Result[] results)
        {
            foreach (Result result in results)
            {
                if (result.IsFailure)
                    return Result.Fail(result.Error);
            }
            return Result.Ok();
        }

        [DebuggerStepThrough]
        public static Result Combine(string errorMessagesSeparator, params Result[] results)
        {
            List<Result> list = ((IEnumerable<Result>)results).Where<Result>((Func<Result, bool>)(x => x.IsFailure)).ToList<Result>();
            return !list.Any<Result>() ? Result.Ok() : Result.Fail(string.Join(errorMessagesSeparator, list.Select<Result, string>((Func<Result, string>)(x => x.Error)).ToArray<string>()));
        }

        [DebuggerStepThrough]
        public static Result Combine(params Result[] results) => Result.Combine(", ", results);

        [DebuggerStepThrough]
        public static Result Combine<T>(params Result<T>[] results) => Result.Combine<T>(", ", results);

        [DebuggerStepThrough]
        public static Result Combine<T>(string errorMessagesSeparator, params Result<T>[] results)
        {
            Result[] array = ((IEnumerable<Result<T>>)results).Select<Result<T>, Result>((Func<Result<T>, Result>)(result => (Result)result)).ToArray<Result>();
            return Result.Combine(errorMessagesSeparator, array);
        }
    }

    public struct Result<TValue, TError> : ISerializable
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ResultCommonLogic<TError> _logic;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly TValue _data;

        public bool IsFailure => this._logic.IsFailure;

        public bool IsSuccess => this._logic.IsSuccess;

        public TError Error => this._logic.Error;

        void ISerializable.GetObjectData(
            SerializationInfo oInfo,
            StreamingContext oContext)
        {
            this._logic.GetObjectData(oInfo, oContext);
            if (!this.IsSuccess)
                return;
            oInfo.AddValue("Data", (object)this.Data);
        }

        public TValue Data
        {
            [DebuggerStepThrough]
            get
            {
                if (!this.IsSuccess)
                    throw new InvalidOperationException("There is no value for failure.");
                return this._data;
            }
        }

        [DebuggerStepThrough]
        internal Result(bool isFailure, TValue data, TError error)
        {
            this._logic = isFailure || (object)data != null ? new ResultCommonLogic<TError>(isFailure, error) : throw new ArgumentNullException(nameof(data));
            this._data = data;
        }

        public static implicit operator Result(Result<TValue, TError> result) => result.IsSuccess ? Result.Ok() : Result.Fail(result.Error.ToString());

        public static implicit operator Result<TValue>(Result<TValue, TError> result) => result.IsSuccess ? Result.Ok<TValue>(result.Data) : Result.Fail<TValue>(result.Error.ToString());
    }

    public struct Result<T> : ISerializable
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ResultCommonLogic _logic;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly T _data;

        public bool IsFailure => this._logic.IsFailure;

        public bool IsSuccess => this._logic.IsSuccess;

        public string Error => this._logic.Error;

        void ISerializable.GetObjectData(
            SerializationInfo oInfo,
            StreamingContext oContext)
        {
            this._logic.GetObjectData(oInfo, oContext);
            if (!this.IsSuccess)
                return;
            oInfo.AddValue("Data", (object)this.Data);
        }

        public T Data
        {
            [DebuggerStepThrough]
            get
            {
                if (!this.IsSuccess)
                    throw new InvalidOperationException("There is no value for failure.");
                return this._data;
            }
        }

        [DebuggerStepThrough]
        internal Result(bool isFailure, T data, string error)
        {
            this._logic = isFailure || (object)data != null ? ResultCommonLogic.Create(isFailure, error) : throw new ArgumentNullException(nameof(data));
            this._data = data;
        }

        public static implicit operator Result(Result<T> result) => result.IsSuccess ? Result.Ok() : Result.Fail(result.Error);
    }
}
