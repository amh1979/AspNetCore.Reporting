using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ChartDerivedSeries : IPersistable
	{
		private int m_exprHostID;

		[Reference]
		private Chart m_chart;

		[NonSerialized]
		private ChartSeries m_sourceSeries;

		private ChartSeries m_series;

		private ExpressionInfo m_sourceChartSeriesName;

		private ExpressionInfo m_derivedSeriesFormula;

		private List<ChartFormulaParameter> m_chartFormulaParameters;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartDerivedSeries.GetDeclaration();

		[NonSerialized]
		private ChartDerivedSeriesExprHost m_exprHost;

		internal ChartSeries SourceSeries
		{
			get
			{
				if (this.m_sourceSeries == null)
				{
					this.m_sourceSeries = this.m_chart.ChartSeriesCollection.GetByName(this.SourceChartSeriesName);
				}
				return this.m_sourceSeries;
			}
		}

		internal ChartDerivedSeriesExprHost ExprHost
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

		internal ChartSeries Series
		{
			get
			{
				return this.m_series;
			}
			set
			{
				this.m_series = value;
			}
		}

		internal List<ChartFormulaParameter> FormulaParameters
		{
			get
			{
				return this.m_chartFormulaParameters;
			}
			set
			{
				this.m_chartFormulaParameters = value;
			}
		}

		internal string SourceChartSeriesName
		{
			get
			{
				if (this.m_sourceChartSeriesName != null)
				{
					return this.m_sourceChartSeriesName.StringValue;
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.m_sourceChartSeriesName = ExpressionInfo.CreateConstExpression(value);
				}
				else
				{
					this.m_sourceChartSeriesName = null;
				}
			}
		}

		internal ChartSeriesFormula DerivedSeriesFormula
		{
			get
			{
				if (this.m_derivedSeriesFormula != null)
				{
					return EnumTranslator.TranslateChartSeriesFormula(this.m_derivedSeriesFormula.StringValue);
				}
				return ChartSeriesFormula.BollingerBands;
			}
			set
			{
				this.m_derivedSeriesFormula = ExpressionInfo.CreateConstExpression(value.ToString());
			}
		}

		internal ChartDerivedSeries()
		{
		}

		internal ChartDerivedSeries(Chart chart)
		{
			this.m_chart = chart;
		}

		internal void SetExprHost(ChartDerivedSeriesExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			if (this.m_series != null && this.m_exprHost.ChartSeriesHost != null)
			{
				this.m_series.SetExprHost(this.m_exprHost.ChartSeriesHost, reportObjectModel);
			}
			IList<ChartFormulaParameterExprHost> chartFormulaParametersHostsRemotable = this.m_exprHost.ChartFormulaParametersHostsRemotable;
			if (this.m_chartFormulaParameters != null && chartFormulaParametersHostsRemotable != null)
			{
				for (int i = 0; i < this.m_chartFormulaParameters.Count; i++)
				{
					ChartFormulaParameter chartFormulaParameter = this.m_chartFormulaParameters[i];
					if (chartFormulaParameter != null && chartFormulaParameter.ExpressionHostID > -1)
					{
						chartFormulaParameter.SetExprHost(chartFormulaParametersHostsRemotable[chartFormulaParameter.ExpressionHostID], reportObjectModel);
					}
				}
			}
		}

		internal void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.ChartDerivedSeriesStart(index);
			if (this.m_series != null)
			{
				this.m_series.Initialize(context, index.ToString(CultureInfo.InvariantCulture));
			}
			if (this.m_chartFormulaParameters != null)
			{
				for (int i = 0; i < this.m_chartFormulaParameters.Count; i++)
				{
					this.m_chartFormulaParameters[i].Initialize(context);
				}
			}
			this.m_exprHostID = context.ExprHostBuilder.ChartDerivedSeriesEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ChartDerivedSeries chartDerivedSeries = (ChartDerivedSeries)base.MemberwiseClone();
			chartDerivedSeries.m_chart = (Chart)context.CurrentDataRegionClone;
			if (this.m_series != null)
			{
				chartDerivedSeries.m_series = (ChartSeries)this.m_series.PublishClone(context);
			}
			if (this.m_sourceChartSeriesName != null)
			{
				chartDerivedSeries.m_sourceChartSeriesName = (ExpressionInfo)this.m_sourceChartSeriesName.PublishClone(context);
			}
			if (this.m_derivedSeriesFormula != null)
			{
				chartDerivedSeries.m_derivedSeriesFormula = (ExpressionInfo)this.m_derivedSeriesFormula.PublishClone(context);
			}
			if (this.m_chartFormulaParameters != null)
			{
				chartDerivedSeries.m_chartFormulaParameters = new List<ChartFormulaParameter>(this.m_chartFormulaParameters.Count);
				{
					foreach (ChartFormulaParameter chartFormulaParameter in this.m_chartFormulaParameters)
					{
						chartDerivedSeries.m_chartFormulaParameters.Add((ChartFormulaParameter)chartFormulaParameter.PublishClone(context));
					}
					return chartDerivedSeries;
				}
			}
			return chartDerivedSeries;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Series, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries));
			list.Add(new MemberInfo(MemberName.SourceChartSeriesName, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DerivedSeriesFormula, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartFormulaParameters, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartFormulaParameter));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Chart, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDerivedSeries, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ChartDerivedSeries.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.Chart:
					writer.WriteReference(this.m_chart);
					break;
				case MemberName.Series:
					writer.Write(this.m_series);
					break;
				case MemberName.ChartFormulaParameters:
					writer.Write(this.m_chartFormulaParameters);
					break;
				case MemberName.SourceChartSeriesName:
					writer.Write(this.m_sourceChartSeriesName);
					break;
				case MemberName.DerivedSeriesFormula:
					writer.Write(this.m_derivedSeriesFormula);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ChartDerivedSeries.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.Chart:
					this.m_chart = reader.ReadReference<Chart>(this);
					break;
				case MemberName.Series:
					this.m_series = (ChartSeries)reader.ReadRIFObject();
					break;
				case MemberName.ChartFormulaParameters:
					this.m_chartFormulaParameters = reader.ReadGenericListOfRIFObjects<ChartFormulaParameter>();
					break;
				case MemberName.SourceChartSeriesName:
					this.m_sourceChartSeriesName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DerivedSeriesFormula:
					this.m_derivedSeriesFormula = (ExpressionInfo)reader.ReadRIFObject();
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
			if (memberReferencesCollection.TryGetValue(ChartDerivedSeries.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.Chart)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_chart = (Chart)referenceableItems[item.RefID];
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDerivedSeries;
		}
	}
}
