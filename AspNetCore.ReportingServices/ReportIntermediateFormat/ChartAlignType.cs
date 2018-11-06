using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ChartAlignType : IPersistable
	{
		private ExpressionInfo m_position;

		private ExpressionInfo m_axesView;

		private ExpressionInfo m_cursor;

		private ExpressionInfo m_innerPlotPosition;

		[Reference]
		private Chart m_chart;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartAlignType.GetDeclaration();

		[NonSerialized]
		private ChartArea m_chartArea;

		internal ExpressionInfo Cursor
		{
			get
			{
				return this.m_cursor;
			}
			set
			{
				this.m_cursor = value;
			}
		}

		internal ExpressionInfo AxesView
		{
			get
			{
				return this.m_axesView;
			}
			set
			{
				this.m_axesView = value;
			}
		}

		internal ExpressionInfo Position
		{
			get
			{
				return this.m_position;
			}
			set
			{
				this.m_position = value;
			}
		}

		internal ExpressionInfo InnerPlotPosition
		{
			get
			{
				return this.m_innerPlotPosition;
			}
			set
			{
				this.m_innerPlotPosition = value;
			}
		}

		internal ChartAreaExprHost ExprHost
		{
			get
			{
				return this.m_chartArea.ExprHost;
			}
		}

		internal ChartAlignType()
		{
		}

		internal ChartAlignType(Chart chart)
		{
			this.m_chart = chart;
		}

		internal void Initialize(InitializationContext context)
		{
			if (this.m_position != null)
			{
				this.m_position.Initialize("Position", context);
				context.ExprHostBuilder.ChartAlignTypePosition(this.m_position);
			}
			if (this.m_innerPlotPosition != null)
			{
				this.m_innerPlotPosition.Initialize("InnerPlotPosition", context);
				context.ExprHostBuilder.ChartAlignTypeInnerPlotPosition(this.m_innerPlotPosition);
			}
			if (this.m_cursor != null)
			{
				this.m_cursor.Initialize("Cursor", context);
				context.ExprHostBuilder.ChartAlignTypCursor(this.m_cursor);
			}
			if (this.m_axesView != null)
			{
				this.m_axesView.Initialize("AxesView", context);
				context.ExprHostBuilder.ChartAlignTypeAxesView(this.m_axesView);
			}
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ChartAlignType chartAlignType = (ChartAlignType)base.MemberwiseClone();
			chartAlignType.m_chart = (Chart)context.CurrentDataRegionClone;
			if (this.m_position != null)
			{
				chartAlignType.m_position = (ExpressionInfo)this.m_position.PublishClone(context);
			}
			if (this.m_innerPlotPosition != null)
			{
				chartAlignType.m_innerPlotPosition = (ExpressionInfo)this.m_innerPlotPosition.PublishClone(context);
			}
			if (this.m_cursor != null)
			{
				chartAlignType.m_cursor = (ExpressionInfo)this.m_cursor.PublishClone(context);
			}
			if (this.m_axesView != null)
			{
				chartAlignType.m_axesView = (ExpressionInfo)this.m_axesView.PublishClone(context);
			}
			return chartAlignType;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Cursor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.AxesView, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Position, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.InnerPlotPosition, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Chart, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAlignType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ChartAlignType.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Position:
					writer.Write(this.m_position);
					break;
				case MemberName.InnerPlotPosition:
					writer.Write(this.m_innerPlotPosition);
					break;
				case MemberName.AxesView:
					writer.Write(this.m_axesView);
					break;
				case MemberName.Cursor:
					writer.Write(this.m_cursor);
					break;
				case MemberName.Chart:
					writer.WriteReference(this.m_chart);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ChartAlignType.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Position:
					this.m_position = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.InnerPlotPosition:
					this.m_innerPlotPosition = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.AxesView:
					this.m_axesView = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Cursor:
					this.m_cursor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Chart:
					this.m_chart = reader.ReadReference<Chart>(this);
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
			if (memberReferencesCollection.TryGetValue(ChartAlignType.m_Declaration.ObjectType, out list))
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAlignType;
		}

		internal void SetExprHost(ChartArea chartArea)
		{
			this.m_chartArea = chartArea;
		}

		internal bool EvaluateAxesView(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chart, instance);
			return context.ReportRuntime.EvaluateChartAlignTypeAxesViewExpression(this, this.m_chart.Name, "AxesView");
		}

		internal bool EvaluateCursor(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chart, instance);
			return context.ReportRuntime.EvaluateChartAlignTypeCursorExpression(this, this.m_chart.Name, "Cursor");
		}

		internal bool EvaluatePosition(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chart, instance);
			return context.ReportRuntime.EvaluateChartAlignTypePositionExpression(this, this.m_chart.Name, "Position");
		}

		internal bool EvaluateInnerPlotPosition(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chart, instance);
			return context.ReportRuntime.EvaluateChartAlignTypeInnerPlotPositionExpression(this, this.m_chart.Name, "InnerPlotPosition");
		}
	}
}
