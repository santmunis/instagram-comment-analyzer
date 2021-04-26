using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Serilog;

namespace MashupWebApi.Extesions
{
    public class RestResult
    {
        public bool Success { get; protected set; }

        public IList<string> Errors { get; protected set; }

        [JsonIgnore]
        protected int StatusCode { get; set; }

        protected RestResult()
        {
        }

        public static RestResult Ok() => new RestResult()
        {
            Success = true,
            Errors = (IList<string>)new List<string>(),
            StatusCode = 200
        };

        public static RestResult Fail(string error)
        {
            Log.Error(error);
            return new RestResult()
            {
                Success = false,
                Errors = (IList<string>)new List<string>()
        {
          error
        },
                StatusCode = 400
            };
        }

        public static RestResult Create(Exception ex)
        {
            Log.Error(ex, ex.Message);
            return new RestResult()
            {
                Success = false,
                Errors = (IList<string>)new List<string>()
        {
          ex.Message
        },
                StatusCode = 500
            };
        }

        public static RestResult Create(ModelStateDictionary dict)
        {
            Log.Error<ModelStateDictionary>("Erro validando Model State {@dict}", dict);
            return new RestResult()
            {
                Success = dict.IsValid,
                Errors = dict.IsValid ? (IList<string>)new List<string>() : (IList<string>)dict.Values.SelectMany<ModelStateEntry, ModelError>((Func<ModelStateEntry, IEnumerable<ModelError>>)(x => (IEnumerable<ModelError>)x.Errors)).Select<ModelError, string>((Func<ModelError, string>)(x => x.ErrorMessage)).ToList<string>(),
                StatusCode = dict.IsValid ? 200 : 422
            };
        }

        public static RestResult Create<T>(T result)
        {
            RestResult<T> restResult = new RestResult<T>(result);
            restResult.Success = true;
            restResult.Errors = (IList<string>)new List<string>();
            restResult.StatusCode = 200;
            return (RestResult)restResult;
        }

        public static RestResult Create<T>(Result<T> result)
        {
            if (result.IsFailure)
                Log.Error(result.Error);
            RestResult<T> restResult = new RestResult<T>(result.IsSuccess ? result.Data : default(T));
            restResult.Success = result.IsSuccess;
            restResult.Errors = result.IsSuccess ? (IList<string>)new List<string>() : (IList<string>)((IEnumerable<string>)result.Error.Split(';')).ToList<string>();
            restResult.StatusCode = result.IsSuccess ? 200 : 400;
            return (RestResult)restResult;
        }

        public static RestResult Create(Result result)
        {
            if (result.IsFailure)
                Log.Error(result.Error);
            return new RestResult()
            {
                Success = result.IsSuccess,
                Errors = result.IsSuccess ? (IList<string>)new List<string>() : (IList<string>)((IEnumerable<string>)result.Error.Split(';')).ToList<string>(),
                StatusCode = result.IsSuccess ? 200 : 400
            };
        }

        public static IActionResult CreateHttpResponse(ModelStateDictionary dict) => RestResult.Create(dict).ToHttp();

        public static IActionResult CreateHttpResponse(Exception ex) => RestResult.Create(ex).ToHttp();

        public static IActionResult CreateHttpResponse<T>(T data) => RestResult.Create<T>(data).ToHttp();

        public static IActionResult CreateHttpResponse<T>(Result<T> data) => RestResult.Create<T>(data).ToHttp();

        public static IActionResult CreateHttpResponse(Result data) => RestResult.Create(data).ToHttp();

        public static IActionResult CreateOkHttpResponse() => RestResult.Ok().ToHttp();

        public static IActionResult CreateFailHttpResponse(string error) => RestResult.Fail(error).ToHttp();

        private IActionResult ToHttp() => (IActionResult)new ObjectResult((object)this)
        {
            StatusCode = new int?(this.StatusCode)
        };
    }

    public class RestResult<T> : RestResult
    {
        public T Data { get; }

        public RestResult(T data) => this.Data = data;
    }
}
