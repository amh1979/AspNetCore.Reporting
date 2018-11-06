namespace AspNetCore.ReportingServices.DataProcessing
{
	internal interface IDbCommandRewriter
	{
		string RewrittenCommandText
		{
			get;
		}
	}
}
