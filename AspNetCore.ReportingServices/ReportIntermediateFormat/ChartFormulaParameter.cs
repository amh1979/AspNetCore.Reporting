using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions;
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
	internal sealed class ChartFormulaParameter : IPersistable
	{
		private string m_name;

		private int m_exprHostID;

		[Reference]
		private Chart m_chart;

		[Reference]
		private ChartSeries m_sourceSeries;

		[NonSerialized]
		private ChartDerivedSeries m_parentDerivedSeries;

		private ExpressionInfo m_value;

		private string m_source;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartFormulaParameter.GetDeclaration();

		[NonSerialized]
		private ChartFormulaParameterExprHost m_exprHost;

		internal string FormulaParameterName
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		internal ChartFormulaParameterExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal int ExpressionHostID
		{
			get
			{
				return this.m_exprHostID;
			}
		}

		internal ExpressionInfo Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
			}
		}

		internal string Source
		{
			get
			{
				return this.m_source;
			}
			set
			{
				this.m_source = value;
			}
		}

		private ChartSeries SourceSeries
		{
			get
			{
				if (this.m_sourceSeries == null && this.m_parentDerivedSeries != null)
				{
					this.m_sourceSeries = this.m_parentDerivedSeries.SourceSeries;
				}
				return this.m_sourceSeries;
			}
		}

		internal ChartFormulaParameter()
		{
		}

		internal ChartFormulaParameter(Chart chart, ChartDerivedSeries parentDerivedSeries)
		{
			this.m_chart = chart;
			this.m_parentDerivedSeries = parentDerivedSeries;
		}

		internal void SetExprHost(ChartFormulaParameterExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartFormulaParameterStart(this.m_name);
			if (this.m_value != null)
			{
				this.m_value.Initialize("Value", context);
				context.ExprHostBuilder.ChartFormulaParameterValue(this.m_value);
			}
			this.m_exprHostID = context.ExprHostBuilder.ChartFormulaParameterEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ChartFormulaParameter chartFormulaParameter = (ChartFormulaParameter)base.MemberwiseClone();
			chartFormulaParameter.m_chart = (Chart)context.CurrentDataRegionClone;
			if (this.m_value != null)
			{
				chartFormulaParameter.m_value = (ExpressionInfo)this.m_value.PublishClone(context);
			}
			return chartFormulaParameter;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Source, Token.String));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Chart, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			list.Add(new MemberInfo(MemberName.SourceSeries, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartFormulaParameter, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.SourceSeries, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartFormulaParameterValueExpression(this, this.m_chart.Name);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ChartFormulaParameter.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.Value:
					writer.Write(this.m_value);
					break;
				case MemberName.Source:
					writer.Write(this.m_source);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.Chart:
					writer.WriteReference(this.m_chart);
					break;
				case MemberName.SourceSeries:
					writer.WriteReference(this.SourceSeries);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ChartFormulaParameter.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.Value:
					this.m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Source:
					this.m_source = reader.ReadString();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.Chart:
					this.m_chart = reader.ReadReference<Chart>(this);
					break;
				case MemberName.SourceSeries:
					this.m_sourceSeries = reader.ReadReference<ChartSeries>(this);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(ChartFormulaParameter.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					switch (item.MemberName)
					{
					case MemberName.Chart:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_chart = (Chart)referenceableItems[item.RefID];
						break;
					case MemberName.SourceSeries:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_sourceSeries = (ChartSeries)referenceableItems[item.RefID];
						break;
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartFormulaParameter;
		}
	}
}
