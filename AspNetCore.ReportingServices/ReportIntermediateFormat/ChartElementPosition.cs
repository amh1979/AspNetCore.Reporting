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
	internal sealed class ChartElementPosition : IPersistable
	{
		internal enum Position
		{
			Top,
			Left,
			Height,
			Width
		}

		[NonSerialized]
		private ChartElementPositionExprHost m_exprHost;

		[Reference]
		private Chart m_chart;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartElementPosition.GetDeclaration();

		private ExpressionInfo m_top;

		private ExpressionInfo m_left;

		private ExpressionInfo m_height;

		private ExpressionInfo m_width;

		internal ExpressionInfo Top
		{
			get
			{
				return this.m_top;
			}
			set
			{
				this.m_top = value;
			}
		}

		internal ExpressionInfo Left
		{
			get
			{
				return this.m_left;
			}
			set
			{
				this.m_left = value;
			}
		}

		internal ExpressionInfo Height
		{
			get
			{
				return this.m_height;
			}
			set
			{
				this.m_height = value;
			}
		}

		internal ExpressionInfo Width
		{
			get
			{
				return this.m_width;
			}
			set
			{
				this.m_width = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return this.m_chart.Name;
			}
		}

		internal ChartElementPositionExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal ChartElementPosition()
		{
		}

		internal ChartElementPosition(Chart chart)
		{
			this.m_chart = chart;
		}

		internal void Initialize(InitializationContext context)
		{
			this.Initialize(context, false);
		}

		internal void Initialize(InitializationContext context, bool innerPlot)
		{
			context.ExprHostBuilder.ChartElementPositionStart(innerPlot);
			if (this.m_top != null)
			{
				this.m_top.Initialize("Top", context);
				context.ExprHostBuilder.ChartElementPositionTop(this.m_top);
			}
			if (this.m_left != null)
			{
				this.m_left.Initialize("Left", context);
				context.ExprHostBuilder.ChartElementPositionLeft(this.m_left);
			}
			if (this.m_height != null)
			{
				this.m_height.Initialize("Height", context);
				context.ExprHostBuilder.ChartElementPositionHeight(this.m_height);
			}
			if (this.m_width != null)
			{
				this.m_width.Initialize("Width", context);
				context.ExprHostBuilder.ChartElementPositionWidth(this.m_width);
			}
			context.ExprHostBuilder.ChartElementPositionEnd(innerPlot);
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ChartElementPosition chartElementPosition = (ChartElementPosition)base.MemberwiseClone();
			chartElementPosition.m_chart = (Chart)context.CurrentDataRegionClone;
			if (this.m_top != null)
			{
				chartElementPosition.m_top = (ExpressionInfo)this.m_top.PublishClone(context);
			}
			if (this.m_left != null)
			{
				chartElementPosition.m_left = (ExpressionInfo)this.m_left.PublishClone(context);
			}
			if (this.m_height != null)
			{
				chartElementPosition.m_height = (ExpressionInfo)this.m_height.PublishClone(context);
			}
			if (this.m_width != null)
			{
				chartElementPosition.m_width = (ExpressionInfo)this.m_width.PublishClone(context);
			}
			return chartElementPosition;
		}

		internal void SetExprHost(ChartElementPositionExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Top, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Left, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Height, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Width, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Chart, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartElementPosition, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(ChartElementPosition.m_Declaration.ObjectType, out list))
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

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ChartElementPosition.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Top:
					writer.Write(this.m_top);
					break;
				case MemberName.Left:
					writer.Write(this.m_left);
					break;
				case MemberName.Height:
					writer.Write(this.m_height);
					break;
				case MemberName.Width:
					writer.Write(this.m_width);
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
			reader.RegisterDeclaration(ChartElementPosition.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Top:
					this.m_top = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Left:
					this.m_left = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Height:
					this.m_height = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Width:
					this.m_width = (ExpressionInfo)reader.ReadRIFObject();
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

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartElementPosition;
		}

		internal double EvaluateTop(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartElementPositionExpression(this.Top, "Top", this.ExprHost, Position.Top, this.m_chart.Name);
		}

		internal double EvaluateLeft(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartElementPositionExpression(this.Left, "Left", this.ExprHost, Position.Left, this.m_chart.Name);
		}

		internal double EvaluateHeight(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartElementPositionExpression(this.Height, "Height", this.ExprHost, Position.Height, this.m_chart.Name);
		}

		internal double EvaluateWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartElementPositionExpression(this.Width, "Width", this.ExprHost, Position.Width, this.m_chart.Name);
		}
	}
}
