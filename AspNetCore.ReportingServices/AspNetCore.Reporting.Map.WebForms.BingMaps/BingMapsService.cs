using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;

namespace AspNetCore.Reporting.Map.WebForms.BingMaps
{
	internal sealed class BingMapsService
	{
		private class InternalAsyncRequestState
		{
			public HttpWebRequest Request
			{
				get;
				set;
			}

			public Action<Response> ResponseCallBack
			{
				get;
				set;
			}

			public Action<Exception> ErrorCallBack
			{
				get;
				set;
			}
		}

		public static Response GetImageryMetadata(ImageryMetadataRequest imageryRequest)
		{
			HttpWebRequest httpWebRequest = WebRequest.Create(imageryRequest.GetRequestUrl()) as HttpWebRequest;
			return BingMapsService.ReadResponse((HttpWebResponse)httpWebRequest.GetResponse());
		}

		public static IAsyncResult GetImageryMetadataAsync(ImageryMetadataRequest imageryRequest, Action<Response> clientCallback, Action<Exception> clientErrorCallback)
		{
			HttpWebRequest httpWebRequest = WebRequest.Create(imageryRequest.GetRequestUrl()) as HttpWebRequest;
			InternalAsyncRequestState internalAsyncRequestState = new InternalAsyncRequestState();
			internalAsyncRequestState.Request = httpWebRequest;
			internalAsyncRequestState.ResponseCallBack = clientCallback;
			internalAsyncRequestState.ErrorCallBack = clientErrorCallback;
			InternalAsyncRequestState state = internalAsyncRequestState;
			return httpWebRequest.BeginGetResponse(BingMapsService.RespCallback, state);
		}

		private static void RespCallback(IAsyncResult asynchronousResult)
		{
			InternalAsyncRequestState internalAsyncRequestState = (InternalAsyncRequestState)asynchronousResult.AsyncState;
			try
			{
				HttpWebRequest request = internalAsyncRequestState.Request;
				HttpWebResponse httpResponse = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
				Response response = BingMapsService.ReadResponse(httpResponse);
				if (response != null)
				{
					internalAsyncRequestState.ResponseCallBack(response);
				}
				else
				{
					internalAsyncRequestState.ErrorCallBack(new Exception("Error parsing Bing Maps Response"));
				}
			}
			catch (WebException obj)
			{
				internalAsyncRequestState.ErrorCallBack(obj);
			}
		}

		private static Response ReadResponse(HttpWebResponse httpResponse)
		{
			if (httpResponse.StatusCode == HttpStatusCode.OK)
			{
				DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(Response));
				using (Stream stream = httpResponse.GetResponseStream())
				{
					return dataContractJsonSerializer.ReadObject(stream) as Response;
				}
			}
			return null;
		}
	}
}
