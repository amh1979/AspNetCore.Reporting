using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ChartDataPointValues : IPersistable
	{
		private ExpressionInfo m_x;

		private ExpressionInfo m_y;

		private ExpressionInfo m_size;

		private ExpressionInfo m_high;

		private ExpressionInfo m_low;

		private ExpressionInfo m_start;

		private ExpressionInfo m_end;

		private ExpressionInfo m_mean;

		private ExpressionInfo m_median;

		private ExpressionInfo m_highlightX;

		private ExpressionInfo m_highlightY;

		private ExpressionInfo m_highlightSize;

		private ExpressionInfo m_formatX;

		private ExpressionInfo m_formatY;

		private ExpressionInfo m_formatSize;

		private ExpressionInfo m_currencyLanguageX;

		private ExpressionInfo m_currencyLanguageY;

		private ExpressionInfo m_currencyLanguageSize;

		[Reference]
		private ChartDataPoint m_dataPoint;

		[Reference]
		private Chart m_chart;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartDataPointValues.GetDeclaration();

		internal ExpressionInfo X
		{
			get
			{
				return this.m_x;
			}
			set
			{
				this.m_x = value;
			}
		}

		internal ExpressionInfo Y
		{
			get
			{
				return this.m_y;
			}
			set
			{
				this.m_y = value;
			}
		}

		internal ExpressionInfo Size
		{
			get
			{
				return this.m_size;
			}
			set
			{
				this.m_size = value;
			}
		}

		internal ExpressionInfo High
		{
			get
			{
				return this.m_high;
			}
			set
			{
				this.m_high = value;
			}
		}

		internal ExpressionInfo Low
		{
			get
			{
				return this.m_low;
			}
			set
			{
				this.m_low = value;
			}
		}

		internal ExpressionInfo Start
		{
			get
			{
				return this.m_start;
			}
			set
			{
				this.m_start = value;
			}
		}

		internal ExpressionInfo End
		{
			get
			{
				return this.m_end;
			}
			set
			{
				this.m_end = value;
			}
		}

		internal ExpressionInfo Mean
		{
			get
			{
				return this.m_mean;
			}
			set
			{
				this.m_mean = value;
			}
		}

		internal ExpressionInfo Median
		{
			get
			{
				return this.m_median;
			}
			set
			{
				this.m_median = value;
			}
		}

		internal ChartDataPoint DataPoint
		{
			set
			{
				this.m_dataPoint = value;
			}
		}

		internal ExpressionInfo HighlightX
		{
			get
			{
				return this.m_highlightX;
			}
			set
			{
				this.m_highlightX = value;
			}
		}

		internal ExpressionInfo HighlightY
		{
			get
			{
				return this.m_highlightY;
			}
			set
			{
				this.m_highlightY = value;
			}
		}

		internal ExpressionInfo HighlightSize
		{
			get
			{
				return this.m_highlightSize;
			}
			set
			{
				this.m_highlightSize = value;
			}
		}

		internal ExpressionInfo FormatX
		{
			get
			{
				return this.m_formatX;
			}
			set
			{
				this.m_formatX = value;
			}
		}

		internal ExpressionInfo FormatY
		{
			get
			{
				return this.m_formatY;
			}
			set
			{
				this.m_formatY = value;
			}
		}

		internal ExpressionInfo FormatSize
		{
			get
			{
				return this.m_formatSize;
			}
			set
			{
				this.m_formatSize = value;
			}
		}

		internal ExpressionInfo CurrencyLanguageX
		{
			get
			{
				return this.m_currencyLanguageX;
			}
			set
			{
				this.m_currencyLanguageX = value;
			}
		}

		internal ExpressionInfo CurrencyLanguageY
		{
			get
			{
				return this.m_currencyLanguageY;
			}
			set
			{
				this.m_currencyLanguageY = value;
			}
		}

		internal ExpressionInfo CurrencyLanguageSize
		{
			get
			{
				return this.m_currencyLanguageSize;
			}
			set
			{
				this.m_currencyLanguageSize = value;
			}
		}

		internal ChartDataPointValues()
		{
		}

		internal ChartDataPointValues(Chart chart, ChartDataPoint dataPoint)
		{
			this.m_dataPoint = dataPoint;
			this.m_chart = chart;
		}

		internal void Initialize(InitializationContext context)
		{
			if (this.m_x != null)
			{
				this.m_x.Initialize("X", context);
				context.ExprHostBuilder.ChartDataPointValueX(this.m_x);
			}
			if (this.m_y != null)
			{
				this.m_y.Initialize("Y", context);
				context.ExprHostBuilder.ChartDataPointValueY(this.m_y);
			}
			if (this.m_size != null)
			{
				this.m_size.Initialize("Size", context);
				context.ExprHostBuilder.ChartDataPointValueSize(this.m_size);
			}
			if (this.m_high != null)
			{
				this.m_high.Initialize("High", context);
				context.ExprHostBuilder.ChartDataPointValueHigh(this.m_high);
			}
			if (this.m_low != null)
			{
				this.m_low.Initialize("Low", context);
				context.ExprHostBuilder.ChartDataPointValueLow(this.m_low);
			}
			if (this.m_start != null)
			{
				this.m_start.Initialize("Start", context);
				context.ExprHostBuilder.ChartDataPointValueStart(this.m_start);
			}
			if (this.m_end != null)
			{
				this.m_end.Initialize("End", context);
				context.ExprHostBuilder.ChartDataPointValueEnd(this.m_end);
			}
			if (this.m_mean != null)
			{
				this.m_mean.Initialize("Mean", context);
				context.ExprHostBuilder.ChartDataPointValueMean(this.m_mean);
			}
			if (this.m_median != null)
			{
				this.m_median.Initialize("Median", context);
				context.ExprHostBuilder.ChartDataPointValueMedian(this.m_median);
			}
			if (this.m_highlightX != null)
			{
				this.m_highlightX.Initialize("HighlightX", context);
				context.ExprHostBuilder.ChartDataPointValueHighlightX(this.m_highlightX);
			}
			if (this.m_highlightY != null)
			{
				this.m_highlightY.Initialize("HighlightY", context);
				context.ExprHostBuilder.ChartDataPointValueHighlightY(this.m_highlightY);
			}
			if (this.m_highlightSize != null)
			{
				this.m_highlightSize.Initialize("HighlightSize", context);
				context.ExprHostBuilder.ChartDataPointValueHighlightSize(this.m_highlightSize);
			}
			if (this.m_formatX != null)
			{
				this.m_formatX.Initialize("FormatX", context);
				context.ExprHostBuilder.ChartDataPointValueFormatX(this.m_formatX);
			}
			if (this.m_formatY != null)
			{
				this.m_formatY.Initialize("FormatY", context);
				context.ExprHostBuilder.ChartDataPointValueFormatY(this.m_formatY);
			}
			if (this.m_formatSize != null)
			{
				this.m_formatSize.Initialize("FormatSize", context);
				context.ExprHostBuilder.ChartDataPointValueFormatSize(this.m_formatSize);
			}
			if (this.m_currencyLanguageX != null)
			{
				this.m_currencyLanguageX.Initialize("CurrencyLanguageX", context);
				context.ExprHostBuilder.ChartDataPointValueCurrencyLanguageX(this.m_currencyLanguageX);
			}
			if (this.m_currencyLanguageY != null)
			{
				this.m_currencyLanguageY.Initialize("CurrencyLanguageY", context);
				context.ExprHostBuilder.ChartDataPointValueCurrencyLanguageY(this.m_currencyLanguageY);
			}
			if (this.m_currencyLanguageSize != null)
			{
				this.m_currencyLanguageSize.Initialize("CurrencyLanguageSize", context);
				context.ExprHostBuilder.ChartDataPointValueCurrencyLanguageSize(this.m_currencyLanguageSize);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.X, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Y, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Size, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.High, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Low, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Start, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.End, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Mean, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Median, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartDataPoint, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataPoint, Token.Reference));
			list.Add(new MemberInfo(MemberName.Chart, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			list.Add(new MemberInfo(MemberName.HighlightX, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.HighlightY, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.HighlightSize, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.FormatX, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo, Lifetime.AddedIn(200)));
			list.Add(new MemberInfo(MemberName.FormatY, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo, Lifetime.AddedIn(200)));
			list.Add(new MemberInfo(MemberName.FormatSize, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo, Lifetime.AddedIn(200)));
			list.Add(new MemberInfo(MemberName.CurrencyLanguageX, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo, Lifetime.AddedIn(200)));
			list.Add(new MemberInfo(MemberName.CurrencyLanguageY, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo, Lifetime.AddedIn(200)));
			list.Add(new MemberInfo(MemberName.CurrencyLanguageSize, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo, Lifetime.AddedIn(200)));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataPointValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ChartDataPointValues.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.X:
					writer.Write(this.m_x);
					break;
				case MemberName.Y:
					writer.Write(this.m_y);
					break;
				case MemberName.Size:
					writer.Write(this.m_size);
					break;
				case MemberName.High:
					writer.Write(this.m_high);
					break;
				case MemberName.Low:
					writer.Write(this.m_low);
					break;
				case MemberName.Start:
					writer.Write(this.m_start);
					break;
				case MemberName.End:
					writer.Write(this.m_end);
					break;
				case MemberName.Mean:
					writer.Write(this.m_mean);
					break;
				case MemberName.Median:
					writer.Write(this.m_median);
					break;
				case MemberName.HighlightX:
					writer.Write(this.m_highlightX);
					break;
				case MemberName.HighlightY:
					writer.Write(this.m_highlightY);
					break;
				case MemberName.HighlightSize:
					writer.Write(this.m_highlightSize);
					break;
				case MemberName.FormatX:
					writer.Write(this.m_formatX);
					break;
				case MemberName.FormatY:
					writer.Write(this.m_formatY);
					break;
				case MemberName.FormatSize:
					writer.Write(this.m_formatSize);
					break;
				case MemberName.CurrencyLanguageX:
					writer.Write(this.m_currencyLanguageX);
					break;
				case MemberName.CurrencyLanguageY:
					writer.Write(this.m_currencyLanguageY);
					break;
				case MemberName.CurrencyLanguageSize:
					writer.Write(this.m_currencyLanguageSize);
					break;
				case MemberName.ChartDataPoint:
					writer.WriteReference(this.m_dataPoint);
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
			reader.RegisterDeclaration(ChartDataPointValues.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.X:
					this.m_x = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Y:
					this.m_y = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Size:
					this.m_size = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.High:
					this.m_high = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Low:
					this.m_low = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Start:
					this.m_start = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.End:
					this.m_end = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Mean:
					this.m_mean = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Median:
					this.m_median = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HighlightX:
					this.m_highlightX = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HighlightY:
					this.m_highlightY = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HighlightSize:
					this.m_highlightSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.FormatX:
					this.m_formatX = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.FormatY:
					this.m_formatY = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.FormatSize:
					this.m_formatSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CurrencyLanguageX:
					this.m_currencyLanguageX = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CurrencyLanguageY:
					this.m_currencyLanguageY = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CurrencyLanguageSize:
					this.m_currencyLanguageSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartDataPoint:
					this.m_dataPoint = reader.ReadReference<ChartDataPoint>(this);
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
			if (memberReferencesCollection.TryGetValue(ChartDataPointValues.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					switch (item.MemberName)
					{
					case MemberName.ChartDataPoint:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_dataPoint = (ChartDataPoint)referenceableItems[item.RefID];
						break;
					case MemberName.Chart:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_chart = (Chart)referenceableItems[item.RefID];
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataPointValues;
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ChartDataPointValues chartDataPointValues = (ChartDataPointValues)base.MemberwiseClone();
			chartDataPointValues.m_chart = (Chart)context.CurrentDataRegionClone;
			if (this.m_x != null)
			{
				chartDataPointValues.m_x = (ExpressionInfo)this.m_x.PublishClone(context);
			}
			if (this.m_y != null)
			{
				chartDataPointValues.m_y = (ExpressionInfo)this.m_y.PublishClone(context);
			}
			if (this.m_size != null)
			{
				chartDataPointValues.m_size = (ExpressionInfo)this.m_size.PublishClone(context);
			}
			if (this.m_high != null)
			{
				chartDataPointValues.m_high = (ExpressionInfo)this.m_high.PublishClone(context);
			}
			if (this.m_low != null)
			{
				chartDataPointValues.m_low = (ExpressionInfo)this.m_low.PublishClone(context);
			}
			if (this.m_start != null)
			{
				chartDataPointValues.m_start = (ExpressionInfo)this.m_start.PublishClone(context);
			}
			if (this.m_end != null)
			{
				chartDataPointValues.m_end = (ExpressionInfo)this.m_end.PublishClone(context);
			}
			if (this.m_mean != null)
			{
				chartDataPointValues.m_mean = (ExpressionInfo)this.m_mean.PublishClone(context);
			}
			if (this.m_median != null)
			{
				chartDataPointValues.m_median = (ExpressionInfo)this.m_median.PublishClone(context);
			}
			return chartDataPointValues;
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesXExpression(this.m_dataPoint, this.m_chart.Name);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesYExpression(this.m_dataPoint, this.m_chart.Name);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValueSizesExpression(this.m_dataPoint, this.m_chart.Name);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateHigh(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesHighExpression(this.m_dataPoint, this.m_chart.Name);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateLow(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesLowExpression(this.m_dataPoint, this.m_chart.Name);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateStart(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesStartExpression(this.m_dataPoint, this.m_chart.Name);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateEnd(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesEndExpression(this.m_dataPoint, this.m_chart.Name);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateMean(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesMeanExpression(this.m_dataPoint, this.m_chart.Name);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateMedian(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesMedianExpression(this.m_dataPoint, this.m_chart.Name);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateHighlightX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesHighlightXExpression(this.m_dataPoint, this.m_chart.Name);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateHighlightY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesHighlightYExpression(this.m_dataPoint, this.m_chart.Name);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateHighlightSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesHighlightSizeExpression(this.m_dataPoint, this.m_chart.Name);
		}

		internal string EvaluateFormatX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesFormatXExpression(this.m_dataPoint, this.m_chart.Name);
		}

		internal string EvaluateFormatY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesFormatYExpression(this.m_dataPoint, this.m_chart.Name);
		}

		internal string EvaluateFormatSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesFormatSizeExpression(this.m_dataPoint, this.m_chart.Name);
		}

		internal string EvaluateCurrencyLanguageX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesCurrencyLanguageXExpression(this.m_dataPoint, this.m_chart.Name);
		}

		internal string EvaluateCurrencyLanguageY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesCurrencyLanguageYExpression(this.m_dataPoint, this.m_chart.Name);
		}

		internal string EvaluateCurrencyLanguageSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesCurrencyLanguageSizeExpression(this.m_dataPoint, this.m_chart.Name);
		}
	}
}
