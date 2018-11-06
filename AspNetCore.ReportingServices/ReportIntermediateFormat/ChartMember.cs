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
	internal sealed class ChartMember : ReportHierarchyNode, IPersistable
	{
		private ChartMemberList m_chartMembers;

		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput = DataElementOutputTypes.Auto;

		private ExpressionInfo m_labelExpression;

		[NonSerialized]
		private bool m_chartGroupExpression;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartMember.GetDeclaration();

		[NonSerialized]
		private Formatter m_formatter;

		[NonSerialized]
		private ChartMemberExprHost m_exprHost;

		internal override string RdlElementName
		{
			get
			{
				return "ChartMember";
			}
		}

		internal override HierarchyNodeList InnerHierarchy
		{
			get
			{
				return this.m_chartMembers;
			}
		}

		internal ChartMemberList ChartMembers
		{
			get
			{
				return this.m_chartMembers;
			}
			set
			{
				this.m_chartMembers = value;
			}
		}

		internal string DataElementName
		{
			get
			{
				return this.m_dataElementName;
			}
			set
			{
				this.m_dataElementName = value;
			}
		}

		internal DataElementOutputTypes DataElementOutput
		{
			get
			{
				return this.m_dataElementOutput;
			}
			set
			{
				this.m_dataElementOutput = value;
			}
		}

		internal ExpressionInfo Label
		{
			get
			{
				return this.m_labelExpression;
			}
			set
			{
				this.m_labelExpression = value;
			}
		}

		internal bool ChartGroupExpression
		{
			get
			{
				return this.m_chartGroupExpression;
			}
			set
			{
				this.m_chartGroupExpression = value;
			}
		}

		internal ChartMemberExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal ChartMember()
		{
		}

		internal ChartMember(int id, Chart crItem)
			: base(id, crItem)
		{
		}

		internal void SetIsCategoryMember(bool value)
		{
			base.m_isColumn = value;
			if (this.m_chartMembers != null)
			{
				foreach (ChartMember chartMember in this.m_chartMembers)
				{
					chartMember.SetIsCategoryMember(value);
				}
			}
		}

		protected override void DataGroupStart(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder builder)
		{
			builder.DataGroupStart(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.Chart, base.m_isColumn);
		}

		protected override int DataGroupEnd(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder builder)
		{
			return builder.DataGroupEnd(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.Chart, base.m_isColumn);
		}

		internal override bool InnerInitialize(InitializationContext context, bool restrictive)
		{
			if (this.m_labelExpression != null)
			{
				this.m_labelExpression.Initialize("Label", context);
				context.ExprHostBuilder.ChartMemberLabel(this.m_labelExpression);
			}
			ChartSeries chartSeries = this.GetChartSeries();
			if (chartSeries != null)
			{
				chartSeries.Initialize(context, chartSeries.Name);
			}
			return base.InnerInitialize(context, restrictive);
		}

		internal override bool Initialize(InitializationContext context, bool restrictive)
		{
			this.DataRendererInitialize(context);
			return base.Initialize(context, restrictive);
		}

		internal void DataRendererInitialize(InitializationContext context)
		{
			if (this.m_dataElementOutput == DataElementOutputTypes.Auto)
			{
				if (base.m_grouping != null)
				{
					this.m_dataElementOutput = DataElementOutputTypes.Output;
				}
				else
				{
					this.m_dataElementOutput = DataElementOutputTypes.ContentsOnly;
				}
			}
			string defaultName = string.Empty;
			if (base.m_grouping != null)
			{
				defaultName = base.m_grouping.Name + "_Collection";
			}
			AspNetCore.ReportingServices.ReportPublishing.CLSNameValidator.ValidateDataElementName(ref this.m_dataElementName, defaultName, context.ObjectType, context.ObjectName, "DataElementName", context.ErrorContext);
		}

		internal override object PublishClone(AutomaticSubtotalContext context, DataRegion newContainingRegion)
		{
			ChartMember chartMember = (ChartMember)base.PublishClone(context, newContainingRegion);
			if (this.m_chartMembers != null)
			{
				chartMember.m_chartMembers = new ChartMemberList(this.m_chartMembers.Count);
				foreach (ChartMember chartMember2 in this.m_chartMembers)
				{
					chartMember.m_chartMembers.Add(chartMember2.PublishClone(context, newContainingRegion));
				}
			}
			if (this.m_labelExpression != null)
			{
				chartMember.m_labelExpression = (ExpressionInfo)this.m_labelExpression.PublishClone(context);
			}
			return chartMember;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ChartMembers, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMember));
			list.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			list.Add(new MemberInfo(MemberName.Label, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMember, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportHierarchyNode, list);
		}

		private ChartSeries GetChartSeries()
		{
			if (!base.IsColumn && this.ChartMembers == null)
			{
				ChartSeriesList chartSeriesCollection = ((Chart)base.m_dataRegionDef).ChartSeriesCollection;
				if (chartSeriesCollection.Count <= base.MemberCellIndex)
				{
					return null;
				}
				return chartSeriesCollection[base.MemberCellIndex];
			}
			return null;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartMember.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ChartMembers:
					writer.Write(this.m_chartMembers);
					break;
				case MemberName.DataElementName:
					writer.Write(this.m_dataElementName);
					break;
				case MemberName.DataElementOutput:
					writer.WriteEnum((int)this.m_dataElementOutput);
					break;
				case MemberName.Label:
					writer.Write(this.m_labelExpression);
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
			reader.RegisterDeclaration(ChartMember.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ChartMembers:
					this.m_chartMembers = reader.ReadListOfRIFObjects<ChartMemberList>();
					break;
				case MemberName.DataElementName:
					this.m_dataElementName = reader.ReadString();
					break;
				case MemberName.DataElementOutput:
					this.m_dataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
					break;
				case MemberName.Label:
					this.m_labelExpression = (ExpressionInfo)reader.ReadRIFObject();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMember;
		}

		internal override void SetExprHost(IMemberNode memberExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(memberExprHost != null && reportObjectModel != null);
				this.m_exprHost = (ChartMemberExprHost)memberExprHost;
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
				base.MemberNodeSetExprHost(this.m_exprHost, reportObjectModel);
			}
			if (this.m_exprHost != null && this.m_exprHost.ChartSeriesHost != null)
			{
				ChartSeries chartSeries = this.GetChartSeries();
				if (chartSeries != null)
				{
					chartSeries.SetExprHost(this.m_exprHost.ChartSeriesHost, reportObjectModel);
				}
			}
		}

		internal override void MemberContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions)
		{
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateLabel(ChartMemberInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, instance.ReportScopeInstance);
			return context.ReportRuntime.EvaluateChartDynamicMemberLabelExpression(this, this.m_labelExpression, base.m_dataRegionDef.Name);
		}

		internal string GetFormattedLabelValue(AspNetCore.ReportingServices.RdlExpressions.VariantResult labelObject, OnDemandProcessingContext context)
		{
			string result = null;
			if (labelObject.ErrorOccurred)
			{
				result = RPRes.rsExpressionErrorValue;
			}
			else if (labelObject.Value != null)
			{
				Type type = labelObject.Value.GetType();
				TypeCode typeCode = Type.GetTypeCode(type);
				if (this.m_formatter == null)
				{
					this.m_formatter = new Formatter(base.DataRegionDef.StyleClass, context, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, base.DataRegionDef.Name);
				}
				result = this.m_formatter.FormatValue(labelObject.Value, typeCode);
			}
			return result;
		}
	}
}
