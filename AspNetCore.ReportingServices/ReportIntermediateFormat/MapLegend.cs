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
	internal sealed class MapLegend : MapDockableSubItem, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapLegend.GetDeclaration();

		private ExpressionInfo m_layout;

		private MapLegendTitle m_mapLegendTitle;

		private ExpressionInfo m_autoFitTextDisabled;

		private ExpressionInfo m_minFontSize;

		private ExpressionInfo m_interlacedRows;

		private ExpressionInfo m_interlacedRowsColor;

		private ExpressionInfo m_equallySpacedItems;

		private ExpressionInfo m_textWrapThreshold;

		private string m_name;

		internal string Name
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

		internal ExpressionInfo Layout
		{
			get
			{
				return this.m_layout;
			}
			set
			{
				this.m_layout = value;
			}
		}

		internal MapLegendTitle MapLegendTitle
		{
			get
			{
				return this.m_mapLegendTitle;
			}
			set
			{
				this.m_mapLegendTitle = value;
			}
		}

		internal ExpressionInfo AutoFitTextDisabled
		{
			get
			{
				return this.m_autoFitTextDisabled;
			}
			set
			{
				this.m_autoFitTextDisabled = value;
			}
		}

		internal ExpressionInfo MinFontSize
		{
			get
			{
				return this.m_minFontSize;
			}
			set
			{
				this.m_minFontSize = value;
			}
		}

		internal ExpressionInfo InterlacedRows
		{
			get
			{
				return this.m_interlacedRows;
			}
			set
			{
				this.m_interlacedRows = value;
			}
		}

		internal ExpressionInfo InterlacedRowsColor
		{
			get
			{
				return this.m_interlacedRowsColor;
			}
			set
			{
				this.m_interlacedRowsColor = value;
			}
		}

		internal ExpressionInfo EquallySpacedItems
		{
			get
			{
				return this.m_equallySpacedItems;
			}
			set
			{
				this.m_equallySpacedItems = value;
			}
		}

		internal ExpressionInfo TextWrapThreshold
		{
			get
			{
				return this.m_textWrapThreshold;
			}
			set
			{
				this.m_textWrapThreshold = value;
			}
		}

		internal new MapLegendExprHost ExprHost
		{
			get
			{
				return (MapLegendExprHost)base.m_exprHost;
			}
		}

		internal MapLegend()
		{
		}

		internal MapLegend(Map map, int id)
			: base(map, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapLegendStart(this.m_name);
			base.Initialize(context);
			if (this.m_layout != null)
			{
				this.m_layout.Initialize("Layout", context);
				context.ExprHostBuilder.MapLegendLayout(this.m_layout);
			}
			if (this.m_mapLegendTitle != null)
			{
				this.m_mapLegendTitle.Initialize(context);
			}
			if (this.m_autoFitTextDisabled != null)
			{
				this.m_autoFitTextDisabled.Initialize("AutoFitTextDisabled", context);
				context.ExprHostBuilder.MapLegendAutoFitTextDisabled(this.m_autoFitTextDisabled);
			}
			if (this.m_minFontSize != null)
			{
				this.m_minFontSize.Initialize("MinFontSize", context);
				context.ExprHostBuilder.MapLegendMinFontSize(this.m_minFontSize);
			}
			if (this.m_interlacedRows != null)
			{
				this.m_interlacedRows.Initialize("InterlacedRows", context);
				context.ExprHostBuilder.MapLegendInterlacedRows(this.m_interlacedRows);
			}
			if (this.m_interlacedRowsColor != null)
			{
				this.m_interlacedRowsColor.Initialize("InterlacedRowsColor", context);
				context.ExprHostBuilder.MapLegendInterlacedRowsColor(this.m_interlacedRowsColor);
			}
			if (this.m_equallySpacedItems != null)
			{
				this.m_equallySpacedItems.Initialize("EquallySpacedItems", context);
				context.ExprHostBuilder.MapLegendEquallySpacedItems(this.m_equallySpacedItems);
			}
			if (this.m_textWrapThreshold != null)
			{
				this.m_textWrapThreshold.Initialize("TextWrapThreshold", context);
				context.ExprHostBuilder.MapLegendTextWrapThreshold(this.m_textWrapThreshold);
			}
			base.m_exprHostID = context.ExprHostBuilder.MapLegendEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapLegend mapLegend = (MapLegend)base.PublishClone(context);
			if (this.m_layout != null)
			{
				mapLegend.m_layout = (ExpressionInfo)this.m_layout.PublishClone(context);
			}
			if (this.m_mapLegendTitle != null)
			{
				mapLegend.m_mapLegendTitle = (MapLegendTitle)this.m_mapLegendTitle.PublishClone(context);
			}
			if (this.m_autoFitTextDisabled != null)
			{
				mapLegend.m_autoFitTextDisabled = (ExpressionInfo)this.m_autoFitTextDisabled.PublishClone(context);
			}
			if (this.m_minFontSize != null)
			{
				mapLegend.m_minFontSize = (ExpressionInfo)this.m_minFontSize.PublishClone(context);
			}
			if (this.m_interlacedRows != null)
			{
				mapLegend.m_interlacedRows = (ExpressionInfo)this.m_interlacedRows.PublishClone(context);
			}
			if (this.m_interlacedRowsColor != null)
			{
				mapLegend.m_interlacedRowsColor = (ExpressionInfo)this.m_interlacedRowsColor.PublishClone(context);
			}
			if (this.m_equallySpacedItems != null)
			{
				mapLegend.m_equallySpacedItems = (ExpressionInfo)this.m_equallySpacedItems.PublishClone(context);
			}
			if (this.m_textWrapThreshold != null)
			{
				mapLegend.m_textWrapThreshold = (ExpressionInfo)this.m_textWrapThreshold.PublishClone(context);
			}
			return mapLegend;
		}

		internal void SetExprHost(MapLegendExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			if (this.m_mapLegendTitle != null && this.ExprHost.MapLegendTitleHost != null)
			{
				this.m_mapLegendTitle.SetExprHost(this.ExprHost.MapLegendTitleHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Layout, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapLegendTitle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLegendTitle));
			list.Add(new MemberInfo(MemberName.AutoFitTextDisabled, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MinFontSize, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.InterlacedRows, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.InterlacedRowsColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EquallySpacedItems, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TextWrapThreshold, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLegend, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDockableSubItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapLegend.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.Layout:
					writer.Write(this.m_layout);
					break;
				case MemberName.MapLegendTitle:
					writer.Write(this.m_mapLegendTitle);
					break;
				case MemberName.AutoFitTextDisabled:
					writer.Write(this.m_autoFitTextDisabled);
					break;
				case MemberName.MinFontSize:
					writer.Write(this.m_minFontSize);
					break;
				case MemberName.InterlacedRows:
					writer.Write(this.m_interlacedRows);
					break;
				case MemberName.InterlacedRowsColor:
					writer.Write(this.m_interlacedRowsColor);
					break;
				case MemberName.EquallySpacedItems:
					writer.Write(this.m_equallySpacedItems);
					break;
				case MemberName.TextWrapThreshold:
					writer.Write(this.m_textWrapThreshold);
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
			reader.RegisterDeclaration(MapLegend.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.Layout:
					this.m_layout = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapLegendTitle:
					this.m_mapLegendTitle = (MapLegendTitle)reader.ReadRIFObject();
					break;
				case MemberName.AutoFitTextDisabled:
					this.m_autoFitTextDisabled = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MinFontSize:
					this.m_minFontSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.InterlacedRows:
					this.m_interlacedRows = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.InterlacedRowsColor:
					this.m_interlacedRowsColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EquallySpacedItems:
					this.m_equallySpacedItems = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TextWrapThreshold:
					this.m_textWrapThreshold = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLegend;
		}

		internal MapLegendLayout EvaluateLayout(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapLegendLayout(context.ReportRuntime.EvaluateMapLegendLayoutExpression(this, base.m_map.Name), context.ReportRuntime);
		}

		internal bool EvaluateAutoFitTextDisabled(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLegendAutoFitTextDisabledExpression(this, base.m_map.Name);
		}

		internal string EvaluateMinFontSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLegendMinFontSizeExpression(this, base.m_map.Name);
		}

		internal bool EvaluateInterlacedRows(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLegendInterlacedRowsExpression(this, base.m_map.Name);
		}

		internal string EvaluateInterlacedRowsColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLegendInterlacedRowsColorExpression(this, base.m_map.Name);
		}

		internal bool EvaluateEquallySpacedItems(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLegendEquallySpacedItemsExpression(this, base.m_map.Name);
		}

		internal int EvaluateTextWrapThreshold(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLegendTextWrapThresholdExpression(this, base.m_map.Name);
		}
	}
}
