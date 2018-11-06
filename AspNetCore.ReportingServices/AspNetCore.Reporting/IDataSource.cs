namespace AspNetCore.Reporting
{
	internal interface IDataSource
	{
		string Name
		{
			get;
		}

		object Value
		{
			get;
		}
	}
}
