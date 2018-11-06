using AspNetCore.ReportingServices.RdlObjectModel;

namespace AspNetCore.ReportingServices.RdlObjectModel2005
{
	internal class ReportParameter2005 : ReportParameter
	{
		public new ReportExpression? Prompt
		{
			get
			{
				if (base.PropertyStore.ContainsObject(5))
				{
					return base.Prompt;
				}
				return null;
			}
			set
			{
				if (value.HasValue)
				{
					base.Prompt = value.Value;
				}
			}
		}
	}
}
