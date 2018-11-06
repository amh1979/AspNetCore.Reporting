using AspNetCore.ReportingServices.ReportProcessing;
using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class CustomPropertyInstance
	{
		private CustomProperty m_customPropertyDef;

		private string m_name;

		private object m_value;

		private TypeCode m_typeCode;

		public string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				ReportElement reportElementOwner = this.m_customPropertyDef.ReportElementOwner;
				if (reportElementOwner != null && reportElementOwner.CriGenerationPhase != 0 && (reportElementOwner.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_customPropertyDef.Name.IsExpression))
				{
					this.m_name = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal TypeCode TypeCode
		{
			get
			{
				return this.m_typeCode;
			}
		}

		public object Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				ReportElement reportElementOwner = this.m_customPropertyDef.ReportElementOwner;
				if (reportElementOwner != null && reportElementOwner.CriGenerationPhase != 0 && (reportElementOwner.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_customPropertyDef.Value.IsExpression))
				{
					if (value != null)
					{
						if (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Definition)
						{
							if (!(value is string))
							{
								throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWritebackStringExpected);
							}
						}
						else if (!ReportRuntime.IsVariant(value))
						{
							throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
						}
					}
					this.m_value = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal CustomPropertyInstance(CustomProperty customPropertyDef, string name, object value, TypeCode typeCode)
		{
			this.m_customPropertyDef = customPropertyDef;
			this.m_name = name;
			this.m_value = value;
			this.m_typeCode = typeCode;
		}

		internal void Update(string name, object value, TypeCode typeCode)
		{
			this.m_name = name;
			this.m_value = value;
			this.m_typeCode = typeCode;
		}
	}
}
