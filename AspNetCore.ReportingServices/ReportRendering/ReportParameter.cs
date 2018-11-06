using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Text;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ReportParameter
	{
		private ParameterInfo m_underlyingParam;

		public string Name
		{
			get
			{
				return this.m_underlyingParam.Name;
			}
		}

		public TypeCode DataType
		{
			get
			{
				return (TypeCode)this.m_underlyingParam.DataType;
			}
		}

		public bool Nullable
		{
			get
			{
				return this.m_underlyingParam.Nullable;
			}
		}

		public bool MultiValue
		{
			get
			{
				return this.m_underlyingParam.MultiValue;
			}
		}

		public bool AllowBlank
		{
			get
			{
				return this.m_underlyingParam.AllowBlank;
			}
		}

		public string Prompt
		{
			get
			{
				return this.m_underlyingParam.Prompt;
			}
		}

		public bool UsedInQuery
		{
			get
			{
				return this.m_underlyingParam.UsedInQuery;
			}
		}

		public object Value
		{
			get
			{
				if (this.m_underlyingParam.Values != null && this.m_underlyingParam.Values.Length != 0)
				{
					return this.m_underlyingParam.Values[0];
				}
				return null;
			}
		}

		public object[] Values
		{
			get
			{
				if (this.m_underlyingParam.Values != null && this.m_underlyingParam.Values.Length != 0)
				{
					return this.m_underlyingParam.Values;
				}
				return null;
			}
		}

		internal string StringValues
		{
			get
			{
				if (this.m_underlyingParam.Values != null && this.m_underlyingParam.Values.Length != 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 0; i < this.m_underlyingParam.Values.Length; i++)
					{
						if (i != 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(this.m_underlyingParam.CastToString(this.m_underlyingParam.Values[i], Localization.ClientPrimaryCulture));
					}
					return stringBuilder.ToString();
				}
				return null;
			}
		}

		internal ParameterInfo UnderlyingParam
		{
			get
			{
				return this.m_underlyingParam;
			}
		}

		internal ReportParameter(ParameterInfo param)
		{
			this.m_underlyingParam = param;
		}
	}
}
