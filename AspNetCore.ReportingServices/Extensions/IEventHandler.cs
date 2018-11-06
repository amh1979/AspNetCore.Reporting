using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Interfaces;

namespace AspNetCore.ReportingServices.Extensions
{
	internal interface IEventHandler : IExtension
	{
		bool CanSubscribe(ICatalogQuery catalogQuery, string reportName);

		void ValidateSubscriptionData(Subscription subscription, string subscriptionData, UserContext userContext);

		void HandleEvent(ICatalogQuery catalogQuery, string eventType, string eventData);

		void CleanUp(Subscription subscription);
	}
}
