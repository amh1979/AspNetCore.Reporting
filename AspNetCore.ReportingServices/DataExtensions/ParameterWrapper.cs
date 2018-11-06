using AspNetCore.ReportingServices.DataProcessing;
using System.Data;

namespace AspNetCore.ReportingServices.DataExtensions
{
	internal class ParameterWrapper : BaseDataWrapper, AspNetCore.ReportingServices.DataProcessing.IDataParameter
	{
		public virtual string ParameterName
		{
			get
			{
				return this.UnderlyingParameter.ParameterName;
			}
			set
			{
				this.UnderlyingParameter.ParameterName = value;
			}
		}

		public virtual object Value
		{
			get
			{
				return this.UnderlyingParameter.Value;
			}
			set
			{
				this.UnderlyingParameter.Value = value;
			}
		}

		protected internal System.Data.IDataParameter UnderlyingParameter
		{
			get
			{
				return (System.Data.IDataParameter)base.UnderlyingObject;
			}
		}

		protected internal ParameterWrapper(System.Data.IDataParameter param)
			: base(param)
		{
		}
	}
}
