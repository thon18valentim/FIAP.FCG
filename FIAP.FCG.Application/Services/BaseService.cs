using FIAP.FCG.Core.Web;
using System.Net;

namespace FIAP.FCG.Application.Services
{
	public abstract class BaseService
	{
		public static IApiResponse<T> Success<T>(T resultValue, string message = "")
		{
			return RequestSuccess(resultValue, true, message);
		}

		public static IApiResponse<bool> Success(string message = "")
		{
			return RequestSuccess(true, true, message);
		}

		public static IApiResponse<T> Fail<T>(string message = "")
		{
			return RequestSuccess(default(T), false, message);
		}

		public static IApiResponse<T> BadRequest<T>(string message = "")
		{
			return RequestError<T>(HttpStatusCode.BadRequest, message);
		}

		public static IApiResponse<T> NotFound<T>(string message = "")
		{
			return RequestError<T>(HttpStatusCode.NotFound, message);
		}

		public static IApiResponse<T> Unauthorized<T>(string message = "")
		{
			return RequestError<T>(HttpStatusCode.Unauthorized, message);
		}

		public static IApiResponse<T> InternalServerError<T>(string message = "")
		{
			return RequestError<T>(HttpStatusCode.InternalServerError, message);
		}

		public static IApiResponse<T> RequestTimeout<T>(string message = "")
		{
			return RequestError<T>(HttpStatusCode.RequestTimeout, message);
		}

		public static IApiResponse<T> GenericError<T>(HttpStatusCode statusCode, string message = "")
		{
			return RequestError<T>(statusCode, message);
		}

		#region private ::

		private static ApiResponse<T> RequestSuccess<T>(T? value, bool isSuccess, string message)
		{
			return new ApiResponse<T>
			{
				ResultValue = value,
				Message = message,
				StatusCode = HttpStatusCode.OK,
				IsSuccess = isSuccess
			};
		}

		private static ApiResponse<T> RequestError<T>(HttpStatusCode statusCode, string message)
		{
			return new ApiResponse<T>
			{
				ResultValue = default,
				Message = message,
				StatusCode = statusCode,
				IsSuccess = false
			};
		}

		#endregion
	}
}
