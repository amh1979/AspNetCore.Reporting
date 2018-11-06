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
	internal sealed class ChartCustomPaletteColor : IPersistable
	{
		private int m_exprHostID;

		[Reference]
		private Chart m_chart;

		private ExpressionInfo m_color;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartCustomPaletteColor.GetDeclaration();

		[NonSerialized]
		private ChartCustomPaletteColorExprHost m_exprHost;

		internal ChartCustomPaletteColorExprHost ExprHost
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

		internal ExpressionInfo Color
		{
			get
			{
				return this.m_color;
			}
			set
			{
				this.m_color = value;
			}
		}

		internal ChartCustomPaletteColor()
		{
		}

		internal ChartCustomPaletteColor(Chart chart)
		{
			this.m_chart = chart;
		}

		internal void SetExprHost(ChartCustomPaletteColorExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.ChartCustomPaletteColorStart(index);
			if (this.m_color != null)
			{
				this.m_color.Initialize("Color", context);
				context.ExprHostBuilder.ChartCustomPaletteColor(this.m_color);
			}
			this.m_exprHostID = context.ExprHostBuilder.ChartCustomPaletteColorEnd();
		}

		internal string EvaluateColor(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chart, instance);
			return context.ReportRuntime.EvaluateChartCustomPaletteColorExpression(this, this.m_chart.Name);
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ChartCustomPaletteColor chartCustomPaletteColor = (ChartCustomPaletteColor)base.MemberwiseClone();
			chartCustomPaletteColor.m_chart = (Chart)context.CurrentDataRegionClone;
			if (this.m_color != null)
			{
				chartCustomPaletteColor.m_color = (ExpressionInfo)this.m_color.PublishClone(context);
			}
			return chartCustomPaletteColor;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Color, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Chart, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartCustomPaletteColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ChartCustomPaletteColor.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Color:
					writer.Write(this.m_color);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
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
			reader.RegisterDeclaration(ChartCustomPaletteColor.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Color:
					this.m_color = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
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
			if (memberReferencesCollection.TryGetValue(ChartCustomPaletteColor.m_Declaration.ObjectType, out list))
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartCustomPaletteColor;
		}
	}
}
