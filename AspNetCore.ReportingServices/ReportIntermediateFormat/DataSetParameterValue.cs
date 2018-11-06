using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class DataSetParameterValue : ParameterValue, IParameterDef
	{
		private bool m_readOnly;

		private bool m_nullable;

		private bool m_omitFromQuery;

		[NonSerialized]
		private static readonly Declaration m_Declaration = DataSetParameterValue.GetDeclaration();

		internal bool ReadOnly
		{
			get
			{
				return this.m_readOnly;
			}
			set
			{
				this.m_readOnly = value;
			}
		}

		internal bool Nullable
		{
			get
			{
				return this.m_nullable;
			}
			set
			{
				this.m_nullable = value;
			}
		}

		internal bool OmitFromQuery
		{
			get
			{
				return this.m_omitFromQuery;
			}
			set
			{
				this.m_omitFromQuery = value;
			}
		}

		string IParameterDef.Name
		{
			get
			{
				return base.Name;
			}
		}

		AspNetCore.ReportingServices.ReportProcessing.ObjectType IParameterDef.ParameterObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.QueryParameter;
			}
		}

		DataType IParameterDef.DataType
		{
			get
			{
				return DataType.Object;
			}
		}

		public bool MultiValue
		{
			get
			{
				return true;
			}
		}

		public int DefaultValuesExpressionCount
		{
			get
			{
				if (base.Value == null)
				{
					return 0;
				}
				return 1;
			}
		}

		public int ValidValuesValueExpressionCount
		{
			get
			{
				return 0;
			}
		}

		public int ValidValuesLabelExpressionCount
		{
			get
			{
				return 0;
			}
		}

		public IParameterDataSource DefaultDataSource
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public IParameterDataSource ValidValuesDataSource
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ReadOnly, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Nullable, Token.Boolean));
			list.Add(new MemberInfo(MemberName.OmitFromQuery, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSetParameterValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterValue, list);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSetParameterValue;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(DataSetParameterValue.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ReadOnly:
					writer.Write(this.m_readOnly);
					break;
				case MemberName.Nullable:
					writer.Write(this.m_nullable);
					break;
				case MemberName.OmitFromQuery:
					writer.Write(this.m_omitFromQuery);
					break;
				default:
					Global.Tracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(DataSetParameterValue.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ReadOnly:
					this.m_readOnly = reader.ReadBoolean();
					break;
				case MemberName.Nullable:
					this.m_nullable = reader.ReadBoolean();
					break;
				case MemberName.OmitFromQuery:
					this.m_omitFromQuery = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public bool ValidateValueForNull(object newValue, ErrorContext errorContext, string parameterValueProperty)
		{
			return ParameterBase.ValidateValueForNull(newValue, this.Nullable, errorContext, AspNetCore.ReportingServices.ReportProcessing.ObjectType.QueryParameter, base.Name, parameterValueProperty);
		}

		public bool ValidateValueForBlank(object newValue, ErrorContext errorContext, string parameterValueProperty)
		{
			return true;
		}

		public bool HasDefaultValuesExpressions()
		{
			if (base.Value != null)
			{
				return base.Value.IsExpression;
			}
			return false;
		}

		public bool HasDefaultValuesDataSource()
		{
			return false;
		}

		public bool HasValidValuesValueExpressions()
		{
			return false;
		}

		public bool HasValidValuesLabelExpressions()
		{
			return false;
		}

		public bool HasValidValuesDataSource()
		{
			return false;
		}
	}
}
