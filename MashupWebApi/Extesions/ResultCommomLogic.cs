using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MashupWebApi.Extesions
{
    internal class ResultCommonLogic<TError>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly TError _error;

        public bool IsFailure { get; }

        public bool IsSuccess => !this.IsFailure;

        public TError Error
        {
            [DebuggerStepThrough]
            get
            {
                if (this.IsSuccess)
                    throw new InvalidOperationException("There is no error message for success.");
                return this._error;
            }
        }

        [DebuggerStepThrough]
        public ResultCommonLogic(bool isFailure, TError error)
        {
            if (isFailure)
            {
                if ((object)error == null)
                    throw new ArgumentNullException(nameof(error), ResultMessages.ErrorObjectIsNotProvidedForFailure);
            }
            else if ((object)error != null)
                throw new ArgumentException(ResultMessages.ErrorObjectIsProvidedForSuccess, nameof(error));
            this.IsFailure = isFailure;
            this._error = error;
        }

        public void GetObjectData(SerializationInfo oInfo, StreamingContext oContext)
        {
            oInfo.AddValue("IsFailure", this.IsFailure);
            oInfo.AddValue("IsSuccess", this.IsSuccess);
            if (!this.IsFailure)
                return;
            oInfo.AddValue("Error", (object)this.Error);
        }
    }

    internal static class ResultMessages
    {
        public static readonly string ErrorObjectIsNotProvidedForFailure = "You have tried to create a failure result, but error object appeared to be null, please review the code, generating error object.";
        public static readonly string ErrorObjectIsProvidedForSuccess = "You have tried to create a success result, but error object was also passed to the constructor, please try to review the code, creating a success result.";
        public static readonly string ErrorMessageIsNotProvidedForFailure = "There must be error message for failure.";
        public static readonly string ErrorMessageIsProvidedForSuccess = "There should be no error message for success.";
    }

    internal sealed class ResultCommonLogic : ResultCommonLogic<string>
    {
        [DebuggerStepThrough]
        public static ResultCommonLogic Create(bool isFailure, string error)
        {
            if (isFailure)
            {
                if (string.IsNullOrEmpty(error))
                    throw new ArgumentNullException(nameof(error), ResultMessages.ErrorMessageIsNotProvidedForFailure);
            }
            else if (error != null)
                throw new ArgumentException(ResultMessages.ErrorMessageIsProvidedForSuccess, nameof(error));
            return new ResultCommonLogic(isFailure, error);
        }

        public ResultCommonLogic(bool isFailure, string error)
            : base(isFailure, error)
        {
        }
    }
}
