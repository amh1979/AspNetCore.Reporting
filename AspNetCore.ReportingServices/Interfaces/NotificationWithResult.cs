namespace AspNetCore.ReportingServices.Interfaces
{
	internal abstract class NotificationWithResult : Notification
	{
		public abstract string SubscriptionResult
		{
			get;
			set;
		}
	}
}
