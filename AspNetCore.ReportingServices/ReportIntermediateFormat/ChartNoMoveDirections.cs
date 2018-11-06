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
	internal sealed class ChartNoMoveDirections : IPersistable
	{
		[Reference]
		private Chart m_chart;

		[Reference]
		private ChartSeries m_chartSeries;

		private ExpressionInfo m_up;

		private ExpressionInfo m_down;

		private ExpressionInfo m_left;

		private ExpressionInfo m_right;

		private ExpressionInfo m_upLeft;

		private ExpressionInfo m_upRight;

		private ExpressionInfo m_downLeft;

		private ExpressionInfo m_downRight;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartNoMoveDirections.GetDeclaration();

		[NonSerialized]
		private ChartNoMoveDirectionsExprHost m_exprHost;

		internal ChartNoMoveDirectionsExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal ExpressionInfo Up
		{
			get
			{
				return this.m_up;
			}
			set
			{
				this.m_up = value;
			}
		}

		internal ExpressionInfo Down
		{
			get
			{
				return this.m_down;
			}
			set
			{
				this.m_down = value;
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

		internal ExpressionInfo Right
		{
			get
			{
				return this.m_right;
			}
			set
			{
				this.m_right = value;
			}
		}

		internal ExpressionInfo UpLeft
		{
			get
			{
				return this.m_upLeft;
			}
			set
			{
				this.m_upLeft = value;
			}
		}

		internal ExpressionInfo UpRight
		{
			get
			{
				return this.m_upRight;
			}
			set
			{
				this.m_upRight = value;
			}
		}

		internal ExpressionInfo DownLeft
		{
			get
			{
				return this.m_downLeft;
			}
			set
			{
				this.m_downLeft = value;
			}
		}

		internal ExpressionInfo DownRight
		{
			get
			{
				return this.m_downRight;
			}
			set
			{
				this.m_downRight = value;
			}
		}

		internal ChartNoMoveDirections()
		{
		}

		internal ChartNoMoveDirections(Chart chart, ChartSeries chartSeries)
		{
			this.m_chart = chart;
			this.m_chartSeries = chartSeries;
		}

		internal void SetExprHost(ChartNoMoveDirectionsExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartNoMoveDirectionsStart();
			if (this.m_up != null)
			{
				this.m_up.Initialize("Up", context);
				context.ExprHostBuilder.ChartNoMoveDirectionsUp(this.m_up);
			}
			if (this.m_down != null)
			{
				this.m_down.Initialize("Down", context);
				context.ExprHostBuilder.ChartNoMoveDirectionsDown(this.m_down);
			}
			if (this.m_left != null)
			{
				this.m_left.Initialize("Left", context);
				context.ExprHostBuilder.ChartNoMoveDirectionsLeft(this.m_left);
			}
			if (this.m_right != null)
			{
				this.m_right.Initialize("Right", context);
				context.ExprHostBuilder.ChartNoMoveDirectionsRight(this.m_right);
			}
			if (this.m_upLeft != null)
			{
				this.m_upLeft.Initialize("UpLeft", context);
				context.ExprHostBuilder.ChartNoMoveDirectionsUpLeft(this.m_upLeft);
			}
			if (this.m_upRight != null)
			{
				this.m_upRight.Initialize("UpRight", context);
				context.ExprHostBuilder.ChartNoMoveDirectionsUpRight(this.m_upRight);
			}
			if (this.m_downLeft != null)
			{
				this.m_downLeft.Initialize("DownLeft", context);
				context.ExprHostBuilder.ChartNoMoveDirectionsDownLeft(this.m_downLeft);
			}
			if (this.m_downRight != null)
			{
				this.m_downRight.Initialize("DownRight", context);
				context.ExprHostBuilder.ChartNoMoveDirectionsDownRight(this.m_downRight);
			}
			context.ExprHostBuilder.ChartNoMoveDirectionsEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ChartNoMoveDirections chartNoMoveDirections = (ChartNoMoveDirections)base.MemberwiseClone();
			chartNoMoveDirections.m_chart = (Chart)context.CurrentDataRegionClone;
			if (this.m_up != null)
			{
				chartNoMoveDirections.m_up = (ExpressionInfo)this.m_up.PublishClone(context);
			}
			if (this.m_down != null)
			{
				chartNoMoveDirections.m_down = (ExpressionInfo)this.m_down.PublishClone(context);
			}
			if (this.m_left != null)
			{
				chartNoMoveDirections.m_left = (ExpressionInfo)this.m_left.PublishClone(context);
			}
			if (this.m_right != null)
			{
				chartNoMoveDirections.m_right = (ExpressionInfo)this.m_right.PublishClone(context);
			}
			if (this.m_upLeft != null)
			{
				chartNoMoveDirections.m_upLeft = (ExpressionInfo)this.m_upLeft.PublishClone(context);
			}
			if (this.m_upRight != null)
			{
				chartNoMoveDirections.m_upRight = (ExpressionInfo)this.m_upRight.PublishClone(context);
			}
			if (this.m_downLeft != null)
			{
				chartNoMoveDirections.m_downLeft = (ExpressionInfo)this.m_downLeft.PublishClone(context);
			}
			if (this.m_downRight != null)
			{
				chartNoMoveDirections.m_downRight = (ExpressionInfo)this.m_downRight.PublishClone(context);
			}
			return chartNoMoveDirections;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Up, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Down, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Left, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Right, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.UpLeft, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.UpRight, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DownLeft, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DownRight, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Chart, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			list.Add(new MemberInfo(MemberName.ChartSeries, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartNoMoveDirections, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		internal bool EvaluateUp(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chartSeries, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartNoMoveDirectionsUpExpression(this, this.m_chart.Name);
		}

		internal bool EvaluateDown(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chartSeries, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartNoMoveDirectionsDownExpression(this, this.m_chart.Name);
		}

		internal bool EvaluateLeft(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chartSeries, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartNoMoveDirectionsLeftExpression(this, this.m_chart.Name);
		}

		internal bool EvaluateRight(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chartSeries, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartNoMoveDirectionsRightExpression(this, this.m_chart.Name);
		}

		internal bool EvaluateUpLeft(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chartSeries, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartNoMoveDirectionsUpLeftExpression(this, this.m_chart.Name);
		}

		internal bool EvaluateUpRight(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chartSeries, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartNoMoveDirectionsUpRightExpression(this, this.m_chart.Name);
		}

		internal bool EvaluateDownLeft(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chartSeries, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartNoMoveDirectionsDownLeftExpression(this, this.m_chart.Name);
		}

		internal bool EvaluateDownRight(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chartSeries, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartNoMoveDirectionsDownRightExpression(this, this.m_chart.Name);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ChartNoMoveDirections.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Chart:
					writer.WriteReference(this.m_chart);
					break;
				case MemberName.ChartSeries:
					writer.WriteReference(this.m_chartSeries);
					break;
				case MemberName.Up:
					writer.Write(this.m_up);
					break;
				case MemberName.Down:
					writer.Write(this.m_down);
					break;
				case MemberName.Left:
					writer.Write(this.m_left);
					break;
				case MemberName.Right:
					writer.Write(this.m_right);
					break;
				case MemberName.UpLeft:
					writer.Write(this.m_upLeft);
					break;
				case MemberName.UpRight:
					writer.Write(this.m_upRight);
					break;
				case MemberName.DownLeft:
					writer.Write(this.m_downLeft);
					break;
				case MemberName.DownRight:
					writer.Write(this.m_downRight);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ChartNoMoveDirections.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Chart:
					this.m_chart = reader.ReadReference<Chart>(this);
					break;
				case MemberName.ChartSeries:
					this.m_chartSeries = reader.ReadReference<ChartSeries>(this);
					break;
				case MemberName.Up:
					this.m_up = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Down:
					this.m_down = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Left:
					this.m_left = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Right:
					this.m_right = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.UpLeft:
					this.m_upLeft = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.UpRight:
					this.m_upRight = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DownLeft:
					this.m_downLeft = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DownRight:
					this.m_downRight = (ExpressionInfo)reader.ReadRIFObject();
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
			if (memberReferencesCollection.TryGetValue(ChartNoMoveDirections.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					switch (item.MemberName)
					{
					case MemberName.Chart:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_chart = (Chart)referenceableItems[item.RefID];
						break;
					case MemberName.ChartSeries:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_chartSeries = (ChartSeries)referenceableItems[item.RefID];
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartNoMoveDirections;
		}
	}
}
