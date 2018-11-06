using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ChartAxisTitle : ChartTitleBase, IPersistable
	{
		private ExpressionInfo m_position;

		private ExpressionInfo m_textOrientation;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartAxisTitle.GetDeclaration();

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

		internal ExpressionInfo TextOrientation
		{
			get
			{
				return this.m_textOrientation;
			}
			set
			{
				this.m_textOrientation = value;
			}
		}

		internal ChartAxisTitle()
		{
		}

		internal ChartAxisTitle(Chart chart)
			: base(chart)
		{
			base.m_chart = chart;
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartAxisTitleStart();
			base.Initialize(context);
			if (this.m_position != null)
			{
				this.m_position.Initialize("Position", context);
				context.ExprHostBuilder.ChartTitlePosition(this.m_position);
			}
			if (this.m_textOrientation != null)
			{
				this.m_textOrientation.Initialize("TextOrientation", context);
				context.ExprHostBuilder.ChartAxisTitleTextOrientation(this.m_textOrientation);
			}
			context.ExprHostBuilder.ChartAxisTitleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartAxisTitle chartAxisTitle = (ChartAxisTitle)base.PublishClone(context);
			if (this.m_position != null)
			{
				chartAxisTitle.m_position = (ExpressionInfo)this.m_position.PublishClone(context);
			}
			if (this.m_textOrientation != null)
			{
				chartAxisTitle.m_textOrientation = (ExpressionInfo)this.m_textOrientation.PublishClone(context);
			}
			return chartAxisTitle;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Position, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TextOrientation, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAxisTitle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTitle, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartAxisTitle.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Position:
					writer.Write(this.m_position);
					break;
				case MemberName.TextOrientation:
					writer.Write(this.m_textOrientation);
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
			reader.RegisterDeclaration(ChartAxisTitle.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Position:
					this.m_position = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TextOrientation:
					this.m_textOrientation = (ExpressionInfo)reader.ReadRIFObject();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAxisTitle;
		}

		internal ChartAxisTitlePositions EvaluatePosition(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, instance);
			return EnumTranslator.TranslateChartAxisTitlePosition(context.ReportRuntime.EvaluateChartAxisTitlePositionExpression(this, base.Name, "Position"), context.ReportRuntime);
		}

		internal TextOrientations EvaluateTextOrientation(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateTextOrientations(context.ReportRuntime.EvaluateChartAxisTitleTextOrientationExpression(this, base.m_chart.Name), context.ReportRuntime);
		}
	}
}
