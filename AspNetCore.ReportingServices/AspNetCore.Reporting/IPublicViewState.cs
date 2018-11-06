namespace AspNetCore.Reporting
{
	internal interface IPublicViewState
	{
		void LoadViewState(object viewState);

		object SaveViewState();
	}
}
