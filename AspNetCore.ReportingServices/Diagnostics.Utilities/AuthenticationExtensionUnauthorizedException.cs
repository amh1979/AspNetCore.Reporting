using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class AuthenticationExtensionUnauthorizedException : RSException
	{
		public KeyValuePair<string, string> HttpResponseHeader
		{
			get;
			private set;
		}

		public AuthenticationExtensionUnauthorizedException(string authorizationUri, string resourceId, string nativeClientId, string oauthLogoutUrl)
			: base(ErrorCode.rsAuthorizationHeaderNotFound, ErrorStrings.rsAuthorizationTokenInvalidOrExpired, null, RSTrace.IsTraceInitialized ? RSTrace.CatalogTrace : null, null)
		{
			this.HttpResponseHeader = new KeyValuePair<string, string>("WWW-authenticate", this.GetAuthenticateHeader(authorizationUri, resourceId, nativeClientId, oauthLogoutUrl));
		}

		public AuthenticationExtensionUnauthorizedException(Exception innerException, string authorizationUri, string resourceId, string nativeClientId, string oauthLogoutUrl)
			: base(ErrorCode.rsAuthorizationTokenInvalidOrExpired, ErrorStrings.rsAuthorizationTokenInvalidOrExpired, innerException, RSTrace.IsTraceInitialized ? RSTrace.CatalogTrace : null, null)
		{
			this.HttpResponseHeader = new KeyValuePair<string, string>("WWW-authenticate", this.GetAuthenticateHeader(authorizationUri, resourceId, nativeClientId, oauthLogoutUrl));
		}

		private AuthenticationExtensionUnauthorizedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private string GetAuthenticateHeader(string authorizationUri, string resourceId, string nativeClientId, string oauthLogoutUrl)
		{
			return string.Format("Bearer authorization_uri={0},resource_id={1},nativeclient_id={2},oauthLogoutUrl_uri={3}", authorizationUri, resourceId, nativeClientId, oauthLogoutUrl);
		}
	}
}
