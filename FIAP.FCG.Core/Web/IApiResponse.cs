using System.Net;

namespace FIAP.FCG.Core.Web
{
	public interface IApiResponse<TResult>
	{
		TResult? ResultValue { get; set; }
		HttpStatusCode StatusCode { get; set; }
		string Message { get; set; }
		bool IsSuccess { get; set; }
	}
}
