using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Filter : IPersistable
	{
		internal enum Operators
		{
			Equal,
			Like,
			GreaterThan,
			GreaterThanOrEqual,
			LessThan,
			LessThanOrEqual,
			TopN,
			BottomN,
			TopPercent,
			BottomPercent,
			In,
			Between,
			NotEqual
		}

		private ExpressionInfo m_expression;

		private Operators m_operator;

		private List<ExpressionInfoTypeValuePair> m_values;

		private int m_exprHostID = -1;

		[NonSerialized]
		private FilterExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = Filter.GetDeclaration();

		internal ExpressionInfo Expression
		{
			get
			{
				return this.m_expression;
			}
			set
			{
				this.m_expression = value;
			}
		}

		internal Operators Operator
		{
			get
			{
				return this.m_operator;
			}
			set
			{
				this.m_operator = value;
			}
		}

		internal List<ExpressionInfoTypeValuePair> Values
		{
			get
			{
				return this.m_values;
			}
			set
			{
				this.m_values = value;
			}
		}

		internal int ExprHostID
		{
			get
			{
				return this.m_exprHostID;
			}
			set
			{
				this.m_exprHostID = value;
			}
		}

		internal FilterExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.FilterStart();
			if (this.m_expression != null)
			{
				this.m_expression.Initialize("FilterExpression", context);
				context.ExprHostBuilder.FilterExpression(this.m_expression);
			}
			if (this.m_values != null)
			{
				for (int i = 0; i < this.m_values.Count; i++)
				{
					ExpressionInfo value = this.m_values[i].Value;
					Global.Tracer.Assert(value != null, "(expression != null)");
					value.Initialize("FilterValue", context);
					context.ExprHostBuilder.FilterValue(value);
				}
			}
			this.m_exprHostID = context.ExprHostBuilder.FilterEnd();
		}

		internal void SetExprHost(IList<FilterExprHost> filterHosts, ObjectModelImpl reportObjectModel)
		{
			if (this.ExprHostID >= 0)
			{
				Global.Tracer.Assert(filterHosts != null && reportObjectModel != null, "(filterHosts != null && reportObjectModel != null)");
				this.m_exprHost = filterHosts[this.ExprHostID];
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			Filter filter = (Filter)base.MemberwiseClone();
			if (this.m_expression != null)
			{
				filter.m_expression = (ExpressionInfo)this.m_expression.PublishClone(context);
			}
			if (this.m_values != null)
			{
				filter.m_values = new List<ExpressionInfoTypeValuePair>(this.m_values.Count);
				{
					foreach (ExpressionInfoTypeValuePair value in this.m_values)
					{
						filter.m_values.Add(new ExpressionInfoTypeValuePair(value.DataType, value.HadExplicitDataType, (ExpressionInfo)value.Value.PublishClone(context)));
					}
					return filter;
				}
			}
			return filter;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Expression, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Operator, Token.Enum));
			list.Add(new MemberInfo(MemberName.Values, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfoTypeValuePair));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Filter, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(Filter.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Expression:
					writer.Write(this.m_expression);
					break;
				case MemberName.Operator:
					writer.WriteEnum((int)this.m_operator);
					break;
				case MemberName.Values:
					writer.Write(this.m_values);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(Filter.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Expression:
					this.m_expression = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Operator:
					this.m_operator = (Operators)reader.ReadEnum();
					break;
				case MemberName.Values:
					this.m_values = reader.ReadGenericListOfRIFObjects<ExpressionInfoTypeValuePair>();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Filter;
		}
	}
}
