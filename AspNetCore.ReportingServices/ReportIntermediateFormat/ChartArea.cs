using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
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
	internal sealed class ChartArea : ChartStyleContainer, IPersistable
	{
		private string m_name;

		private List<ChartAxis> m_categoryAxes;

		private List<ChartAxis> m_valueAxes;

		private ChartThreeDProperties m_3dProperties;

		private ChartElementPosition m_chartElementPosition;

		private ChartElementPosition m_chartInnerPlotPosition;

		private int m_exprHostID;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_alignOrientation;

		private ChartAlignType m_chartAlignType;

		private string m_alignWithChartArea;

		private ExpressionInfo m_equallySizedAxesFont;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartArea.GetDeclaration();

		[NonSerialized]
		private ChartAreaExprHost m_exprHost;

		internal string ChartAreaName
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

		internal List<ChartAxis> CategoryAxes
		{
			get
			{
				return this.m_categoryAxes;
			}
			set
			{
				this.m_categoryAxes = value;
			}
		}

		internal List<ChartAxis> ValueAxes
		{
			get
			{
				return this.m_valueAxes;
			}
			set
			{
				this.m_valueAxes = value;
			}
		}

		internal ChartThreeDProperties ThreeDProperties
		{
			get
			{
				return this.m_3dProperties;
			}
			set
			{
				this.m_3dProperties = value;
			}
		}

		internal ChartAreaExprHost ExprHost
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

		internal ExpressionInfo Hidden
		{
			get
			{
				return this.m_hidden;
			}
			set
			{
				this.m_hidden = value;
			}
		}

		internal ExpressionInfo AlignOrientation
		{
			get
			{
				return this.m_alignOrientation;
			}
			set
			{
				this.m_alignOrientation = value;
			}
		}

		internal ChartAlignType ChartAlignType
		{
			get
			{
				return this.m_chartAlignType;
			}
			set
			{
				this.m_chartAlignType = value;
			}
		}

		internal string AlignWithChartArea
		{
			get
			{
				return this.m_alignWithChartArea;
			}
			set
			{
				this.m_alignWithChartArea = value;
			}
		}

		internal ExpressionInfo EquallySizedAxesFont
		{
			get
			{
				return this.m_equallySizedAxesFont;
			}
			set
			{
				this.m_equallySizedAxesFont = value;
			}
		}

		internal ChartElementPosition ChartElementPosition
		{
			get
			{
				return this.m_chartElementPosition;
			}
			set
			{
				this.m_chartElementPosition = value;
			}
		}

		internal ChartElementPosition ChartInnerPlotPosition
		{
			get
			{
				return this.m_chartInnerPlotPosition;
			}
			set
			{
				this.m_chartInnerPlotPosition = value;
			}
		}

		internal Chart Chart
		{
			get
			{
				return base.m_chart;
			}
			set
			{
				base.m_chart = value;
			}
		}

		internal ChartArea()
		{
		}

		internal ChartArea(Chart chart)
			: base(chart)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartAreaStart(this.m_name);
			base.Initialize(context);
			if (this.m_categoryAxes != null)
			{
				for (int i = 0; i < this.m_categoryAxes.Count; i++)
				{
					this.m_categoryAxes[i].Initialize(context, false);
				}
			}
			if (this.m_valueAxes != null)
			{
				for (int j = 0; j < this.m_valueAxes.Count; j++)
				{
					this.m_valueAxes[j].Initialize(context, true);
				}
			}
			if (this.m_3dProperties != null)
			{
				this.m_3dProperties.Initialize(context);
			}
			if (this.m_hidden != null)
			{
				this.m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.ChartAreaHidden(this.m_hidden);
			}
			if (this.m_alignOrientation != null)
			{
				this.m_alignOrientation.Initialize("AlignOrientation", context);
				context.ExprHostBuilder.ChartAreaAlignOrientation(this.m_alignOrientation);
			}
			if (this.m_chartAlignType != null)
			{
				this.m_chartAlignType.Initialize(context);
			}
			if (this.m_equallySizedAxesFont != null)
			{
				this.m_equallySizedAxesFont.Initialize("EquallySizedAxesFont", context);
				context.ExprHostBuilder.ChartAreaEquallySizedAxesFont(this.m_equallySizedAxesFont);
			}
			if (this.m_chartElementPosition != null)
			{
				this.m_chartElementPosition.Initialize(context);
			}
			if (this.m_chartInnerPlotPosition != null)
			{
				this.m_chartInnerPlotPosition.Initialize(context, true);
			}
			this.m_exprHostID = context.ExprHostBuilder.ChartAreaEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartArea chartArea = (ChartArea)base.PublishClone(context);
			if (this.m_categoryAxes != null)
			{
				chartArea.m_categoryAxes = new List<ChartAxis>(this.m_categoryAxes.Count);
				foreach (ChartAxis categoryAxis in this.m_categoryAxes)
				{
					chartArea.m_categoryAxes.Add((ChartAxis)categoryAxis.PublishClone(context));
				}
			}
			if (this.m_valueAxes != null)
			{
				chartArea.m_valueAxes = new List<ChartAxis>(this.m_valueAxes.Count);
				foreach (ChartAxis valueAxis in this.m_valueAxes)
				{
					chartArea.m_valueAxes.Add((ChartAxis)valueAxis.PublishClone(context));
				}
			}
			if (this.m_3dProperties != null)
			{
				chartArea.m_3dProperties = (ChartThreeDProperties)this.m_3dProperties.PublishClone(context);
			}
			if (this.m_hidden != null)
			{
				chartArea.m_hidden = (ExpressionInfo)this.m_hidden.PublishClone(context);
			}
			if (this.m_alignOrientation != null)
			{
				chartArea.m_alignOrientation = (ExpressionInfo)this.m_alignOrientation.PublishClone(context);
			}
			if (this.m_chartAlignType != null)
			{
				chartArea.m_chartAlignType = (ChartAlignType)this.m_chartAlignType.PublishClone(context);
			}
			if (this.m_equallySizedAxesFont != null)
			{
				chartArea.m_equallySizedAxesFont = (ExpressionInfo)this.m_equallySizedAxesFont.PublishClone(context);
			}
			if (this.m_chartElementPosition != null)
			{
				chartArea.m_chartElementPosition = (ChartElementPosition)this.m_chartElementPosition.PublishClone(context);
			}
			if (this.m_chartInnerPlotPosition != null)
			{
				chartArea.m_chartInnerPlotPosition = (ChartElementPosition)this.m_chartInnerPlotPosition.PublishClone(context);
			}
			return chartArea;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.CategoryAxes, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAxis));
			list.Add(new MemberInfo(MemberName.ValueAxes, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAxis));
			list.Add(new MemberInfo(MemberName.ThreeDProperties, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ThreeDProperties));
			list.Add(new MemberInfo(MemberName.Hidden, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.AlignOrientation, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartAlignType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAlignType));
			list.Add(new MemberInfo(MemberName.AlignWithChartArea, Token.String));
			list.Add(new MemberInfo(MemberName.EquallySizedAxesFont, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ChartElementPosition, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartElementPosition));
			list.Add(new MemberInfo(MemberName.ChartInnerPlotPosition, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartElementPosition));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartArea, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartArea.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.CategoryAxes:
					writer.Write(this.m_categoryAxes);
					break;
				case MemberName.ValueAxes:
					writer.Write(this.m_valueAxes);
					break;
				case MemberName.ThreeDProperties:
					writer.Write(this.m_3dProperties);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.Hidden:
					writer.Write(this.m_hidden);
					break;
				case MemberName.AlignOrientation:
					writer.Write(this.m_alignOrientation);
					break;
				case MemberName.ChartAlignType:
					writer.Write(this.m_chartAlignType);
					break;
				case MemberName.AlignWithChartArea:
					writer.Write(this.m_alignWithChartArea);
					break;
				case MemberName.EquallySizedAxesFont:
					writer.Write(this.m_equallySizedAxesFont);
					break;
				case MemberName.ChartElementPosition:
					writer.Write(this.m_chartElementPosition);
					break;
				case MemberName.ChartInnerPlotPosition:
					writer.Write(this.m_chartInnerPlotPosition);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(ChartArea.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.CategoryAxes:
					this.m_categoryAxes = reader.ReadGenericListOfRIFObjects<ChartAxis>();
					break;
				case MemberName.ValueAxes:
					this.m_valueAxes = reader.ReadGenericListOfRIFObjects<ChartAxis>();
					break;
				case MemberName.ThreeDProperties:
					this.m_3dProperties = (ChartThreeDProperties)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.Hidden:
					this.m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.AlignOrientation:
					this.m_alignOrientation = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartAlignType:
					this.m_chartAlignType = (ChartAlignType)reader.ReadRIFObject();
					break;
				case MemberName.AlignWithChartArea:
					this.m_alignWithChartArea = reader.ReadString();
					break;
				case MemberName.EquallySizedAxesFont:
					this.m_equallySizedAxesFont = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartElementPosition:
					this.m_chartElementPosition = (ChartElementPosition)reader.ReadRIFObject();
					break;
				case MemberName.ChartInnerPlotPosition:
					this.m_chartInnerPlotPosition = (ChartElementPosition)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartArea;
		}

		internal void SetExprHost(ChartAreaExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
			if (this.m_3dProperties != null && this.m_exprHost.Chart3DPropertiesHost != null)
			{
				this.m_3dProperties.SetExprHost(this.m_exprHost.Chart3DPropertiesHost, reportObjectModel);
			}
			if (this.m_chartAlignType != null)
			{
				this.m_chartAlignType.SetExprHost(this);
			}
			IList<ChartAxisExprHost> categoryAxesHostsRemotable = exprHost.CategoryAxesHostsRemotable;
			if (this.m_categoryAxes != null && categoryAxesHostsRemotable != null)
			{
				for (int i = 0; i < this.m_categoryAxes.Count; i++)
				{
					ChartAxis chartAxis = this.m_categoryAxes[i];
					if (chartAxis != null && chartAxis.ExpressionHostID > -1)
					{
						chartAxis.SetExprHost(categoryAxesHostsRemotable[chartAxis.ExpressionHostID], reportObjectModel);
					}
				}
			}
			IList<ChartAxisExprHost> valueAxesHostsRemotable = exprHost.ValueAxesHostsRemotable;
			if (this.m_valueAxes != null && valueAxesHostsRemotable != null)
			{
				for (int j = 0; j < this.m_valueAxes.Count; j++)
				{
					ChartAxis chartAxis2 = this.m_valueAxes[j];
					if (chartAxis2 != null && chartAxis2.ExpressionHostID > -1)
					{
						chartAxis2.SetExprHost(valueAxesHostsRemotable[chartAxis2.ExpressionHostID], reportObjectModel);
					}
				}
			}
			if (this.m_chartElementPosition != null && this.m_exprHost.ChartElementPositionHost != null)
			{
				this.m_chartElementPosition.SetExprHost(this.m_exprHost.ChartElementPositionHost, reportObjectModel);
			}
			if (this.m_chartInnerPlotPosition != null && this.m_exprHost.ChartInnerPlotPositionHost != null)
			{
				this.m_chartInnerPlotPosition.SetExprHost(this.m_exprHost.ChartInnerPlotPositionHost, reportObjectModel);
			}
		}

		internal bool EvaluateHidden(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, instance);
			return context.ReportRuntime.EvaluateChartAreaHiddenExpression(this, base.Name, "Hidden");
		}

		internal ChartAreaAlignOrientations EvaluateAlignOrientation(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, instance);
			return EnumTranslator.TranslateChartAreaAlignOrientation(context.ReportRuntime.EvaluateChartAreaAlignOrientationExpression(this, base.Name, "AlignOrientation"), context.ReportRuntime);
		}

		internal bool EvaluateEquallySizedAxesFont(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, instance);
			return context.ReportRuntime.EvaluateChartAreaEquallySizedAxesFontExpression(this, base.Name, "EquallySizedAxesFont");
		}
	}
}
