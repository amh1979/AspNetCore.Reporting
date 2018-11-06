using AspNetCore.Reporting.Chart.Helpers;
using AspNetCore.Reporting.Chart.WebForms;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class ChartMapper : MapperBase, IChartMapper, IDVMappingLayer, IDisposable
	{
		private class ChartAreaInfo
		{
			public List<SeriesInfo> SeriesInfoList = new List<SeriesInfo>();

			public List<string> CategoryAxesScalar;

			public List<AspNetCore.Reporting.Chart.WebForms.Axis> CategoryAxesAutoMargin;

			public bool PrimaryAxisSet;

			public bool SecondaryAxisSet;
		}

		private class SeriesInfo
		{
			public ChartAreaInfo ChartAreaInfo;

			public ChartAxis ChartCategoryAxis;

			public Series Series;

			public List<Series> DerivedSeries;

			public ChartSeries ChartSeries;

			public ChartMember SeriesGrouping;

			public bool XValueSet;

			public bool XValueSetFailed;

			public List<DataPoint> NullXValuePoints = new List<DataPoint>();

			public bool IsLine;

			public bool IsExploded;

			public bool IsRange;

			public bool IsBubble;

			public bool IsAttachedToScalarAxis;

			public bool? IsDataPointColorEmpty = null;

			public bool IsDataPointHatchDefined;

			public DataPointBackgroundImageInfoCollection DataPointBackgroundImageInfoCollection = new DataPointBackgroundImageInfoCollection();

			public DataPoint DefaultDataPointAppearance;

			public bool IsGradientPerDataPointSupported = true;

			public bool IsGradientSupported = true;

			public GradientType? BackGradientType = null;

			public Color? Color = null;

			public Color? BackGradientEndColor = null;
		}

		private class DataPointBackgroundImageInfoCollection : List<DataPointBackgroundImageInfo>
		{
			public void Initialize(ChartSeries chartSeries)
			{
				base.Clear();
				for (int i = 0; i < chartSeries.Count; i++)
				{
					DataPointBackgroundImageInfo dataPointBackgroundImageInfo = new DataPointBackgroundImageInfo();
					dataPointBackgroundImageInfo.Initialize(((ReportElementCollectionBase<ChartDataPoint>)chartSeries)[i]);
					base.Add(dataPointBackgroundImageInfo);
				}
			}
		}

		private class DataPointBackgroundImageInfo
		{
			public BackgroundImageInfo DataPointBackgroundImage = new BackgroundImageInfo();

			public BackgroundImageInfo MarkerBackgroundImage = new BackgroundImageInfo();

			public void Initialize(ChartDataPoint chartDataPoint)
			{
				this.DataPointBackgroundImage.Initialize(chartDataPoint.Style);
				if (chartDataPoint.Marker != null)
				{
					this.MarkerBackgroundImage.Initialize(chartDataPoint.Marker.Style);
				}
			}
		}

		private class BackgroundImageInfo
		{
			public bool CanShareBackgroundImage;

			public string SharedBackgroundImageName;

			public void Initialize(Style style)
			{
				if (style != null && style.BackgroundImage != null && style.BackgroundImage.MIMEType != null && style.BackgroundImage.Value != null)
				{
					this.CanShareBackgroundImage = (!style.BackgroundImage.MIMEType.IsExpression && !style.BackgroundImage.Value.IsExpression);
				}
			}
		}

		private class Hatcher
		{
			private int m_current = -1;

			private static ChartHatchStyle[] m_hatchArray = new ChartHatchStyle[54]
			{
				ChartHatchStyle.BackwardDiagonal,
				ChartHatchStyle.Cross,
				ChartHatchStyle.DarkDownwardDiagonal,
				ChartHatchStyle.DarkHorizontal,
				ChartHatchStyle.DarkUpwardDiagonal,
				ChartHatchStyle.DarkVertical,
				ChartHatchStyle.DashedDownwardDiagonal,
				ChartHatchStyle.DashedHorizontal,
				ChartHatchStyle.DashedUpwardDiagonal,
				ChartHatchStyle.DashedVertical,
				ChartHatchStyle.DiagonalBrick,
				ChartHatchStyle.DiagonalCross,
				ChartHatchStyle.Divot,
				ChartHatchStyle.DottedDiamond,
				ChartHatchStyle.DottedGrid,
				ChartHatchStyle.ForwardDiagonal,
				ChartHatchStyle.Horizontal,
				ChartHatchStyle.HorizontalBrick,
				ChartHatchStyle.LargeCheckerBoard,
				ChartHatchStyle.LargeConfetti,
				ChartHatchStyle.LargeGrid,
				ChartHatchStyle.LightDownwardDiagonal,
				ChartHatchStyle.LightHorizontal,
				ChartHatchStyle.LightUpwardDiagonal,
				ChartHatchStyle.LightVertical,
				ChartHatchStyle.NarrowHorizontal,
				ChartHatchStyle.NarrowVertical,
				ChartHatchStyle.OutlinedDiamond,
				ChartHatchStyle.Percent05,
				ChartHatchStyle.Percent10,
				ChartHatchStyle.Percent20,
				ChartHatchStyle.Percent25,
				ChartHatchStyle.Percent30,
				ChartHatchStyle.Percent40,
				ChartHatchStyle.Percent50,
				ChartHatchStyle.Percent60,
				ChartHatchStyle.Percent70,
				ChartHatchStyle.Percent75,
				ChartHatchStyle.Percent80,
				ChartHatchStyle.Percent90,
				ChartHatchStyle.Plaid,
				ChartHatchStyle.Shingle,
				ChartHatchStyle.SmallCheckerBoard,
				ChartHatchStyle.SmallConfetti,
				ChartHatchStyle.SmallGrid,
				ChartHatchStyle.SolidDiamond,
				ChartHatchStyle.Sphere,
				ChartHatchStyle.Trellis,
				ChartHatchStyle.Vertical,
				ChartHatchStyle.Wave,
				ChartHatchStyle.Weave,
				ChartHatchStyle.WideDownwardDiagonal,
				ChartHatchStyle.WideUpwardDiagonal,
				ChartHatchStyle.ZigZag
			};

			internal ChartHatchStyle Current
			{
				get
				{
					this.m_current = (this.m_current + 1) % Hatcher.m_hatchArray.Length;
					return Hatcher.m_hatchArray[this.m_current];
				}
			}
		}

		private class AutoMarker
		{
			private int m_current;

			private bool m_currentUsed;

			private static MarkerStyle[] m_markerArray = new MarkerStyle[9]
			{
				MarkerStyle.Square,
				MarkerStyle.Circle,
				MarkerStyle.Diamond,
				MarkerStyle.Triangle,
				MarkerStyle.Cross,
				MarkerStyle.Star4,
				MarkerStyle.Star5,
				MarkerStyle.Star6,
				MarkerStyle.Star10
			};

			internal MarkerStyle Current
			{
				get
				{
					this.m_currentUsed = true;
					return AutoMarker.m_markerArray[this.m_current];
				}
			}

			internal void MoveNext()
			{
				if (this.m_currentUsed)
				{
					this.m_currentUsed = false;
					this.m_current = (this.m_current + 1) % AutoMarker.m_markerArray.Length;
				}
			}
		}

		private static class FormulaHelper
		{
			public const string PARAM_NAME_START_FROM_FIRST = "StartFromFirst";

			public const string PARAM_NAME_OUTPUT = "Output";

			public const string PARAM_NAME_INPUT = "Input";

			public const string PARAM_NAME_PERIOD = "Period";

			public const string PARAM_DEFAULT_PERIOD = "2";

			public const string PARAM_NAME_SHORT_PERIOD = "ShortPeriod";

			public const string PARAM_DEFAULT_SHORT_PERIOD = "12";

			public const string PARAM_NAME_LONG_PERIOD = "LongPeriod";

			public const string PARAM_DEFAULT_LONG_PERIOD = "26";

			public const string PARAM_NAME_DEVIATION = "Deviation";

			public const string PARAM_DEFAULT_DEVIATION = "2";

			public const string PARAM_NAME_SHIFT = "Shift";

			public const string PARAM_DEFAULT_SHIFT = "7";

			public const string OUTPUT_SERIES_KEYWORD = "#OUTPUTSERIES";

			public const string DERIVED_SERIES_NAME_SUFFIX = "_Formula";

			public const string NEW_CHART_AREA_NAME = "#NewChartArea";

			internal static void RenderFormulaParameters(ChartFormulaParameterCollection chartFormulaParameters, ChartSeriesFormula formula, string sourceSeriesName, string derivedSeriesName, out string formulaParameters, out string inputValues, out string outputValues, out bool startFromFirst)
			{
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				FormulaHelper.GetParameters(chartFormulaParameters, parameters);
				formulaParameters = FormulaHelper.ConstructFormulaParameters(parameters, formula);
				outputValues = FormulaHelper.GetOutputValues(parameters, formula, derivedSeriesName);
				inputValues = FormulaHelper.GetInputValues(parameters, sourceSeriesName);
				string parameter = FormulaHelper.GetParameter(parameters, "StartFromFirst");
				if (parameter.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase))
				{
					startFromFirst = true;
				}
				else
				{
					startFromFirst = false;
				}
			}

			internal static string GetInputValues(Dictionary<string, string> parameters, string sourceSeriesName)
			{
				string text = FormulaHelper.GetParameter(parameters, "Input");
				if (text == "")
				{
					text = sourceSeriesName;
				}
				return text;
			}

			internal static string GetOutputValues(Dictionary<string, string> parameters, ChartSeriesFormula formula, string derivedSeriesName)
			{
				string parameter = FormulaHelper.GetParameter(parameters, "Output");
				if (parameter != "")
				{
					return parameter.Replace("#OUTPUTSERIES", derivedSeriesName);
				}
				switch (formula)
				{
				case ChartSeriesFormula.BollingerBands:
				case ChartSeriesFormula.DetrendedPriceOscillator:
				case ChartSeriesFormula.Envelopes:
					return derivedSeriesName + ":Y, " + derivedSeriesName + ":Y2";
				default:
					return derivedSeriesName;
				}
			}

			internal static string ConstructFormulaParameters(Dictionary<string, string> parameters, ChartSeriesFormula formula)
			{
				string result = "";
				switch (formula)
				{
				case ChartSeriesFormula.BollingerBands:
					FormulaHelper.AppendParameter(parameters, "Period", "2", ref result);
					FormulaHelper.AppendParameter(parameters, "Deviation", "2", ref result);
					break;
				case ChartSeriesFormula.MovingAverage:
					FormulaHelper.AppendParameter(parameters, "Period", "2", ref result);
					break;
				case ChartSeriesFormula.ExponentialMovingAverage:
					FormulaHelper.AppendParameter(parameters, "Period", "2", ref result);
					break;
				case ChartSeriesFormula.TriangularMovingAverage:
					FormulaHelper.AppendParameter(parameters, "Period", "2", ref result);
					break;
				case ChartSeriesFormula.WeightedMovingAverage:
					FormulaHelper.AppendParameter(parameters, "Period", "2", ref result);
					break;
				case ChartSeriesFormula.Envelopes:
					FormulaHelper.AppendParameter(parameters, "Period", "2", ref result);
					FormulaHelper.AppendParameter(parameters, "Shift", "7", ref result);
					break;
				case ChartSeriesFormula.MACD:
					FormulaHelper.AppendParameter(parameters, "ShortPeriod", "12", ref result);
					FormulaHelper.AppendParameter(parameters, "LongPeriod", "26", ref result);
					break;
				case ChartSeriesFormula.DetrendedPriceOscillator:
					FormulaHelper.AppendParameter(parameters, "Period", "2", ref result);
					break;
				case ChartSeriesFormula.RateOfChange:
					FormulaHelper.AppendParameter(parameters, "Period", "10", ref result);
					break;
				case ChartSeriesFormula.RelativeStrengthIndex:
					FormulaHelper.AppendParameter(parameters, "Period", "10", ref result);
					break;
				case ChartSeriesFormula.StandardDeviation:
					FormulaHelper.AppendParameter(parameters, "Period", "2", ref result);
					break;
				case ChartSeriesFormula.TRIX:
					FormulaHelper.AppendParameter(parameters, "Period", "2", ref result);
					break;
				}
				return result;
			}

			private static void GetParameters(ChartFormulaParameterCollection chartFormulaParameters, Dictionary<string, string> parameters)
			{
				if (chartFormulaParameters != null)
				{
					foreach (ChartFormulaParameter chartFormulaParameter in chartFormulaParameters)
					{
						string key = default(string);
						string value = default(string);
						FormulaHelper.GetParameter(chartFormulaParameter, out key, out value);
						parameters.Add(key, value);
					}
				}
			}

			private static void GetParameter(ChartFormulaParameter chartFormulaParameter, out string name, out string value)
			{
				name = "";
				value = "";
				if (chartFormulaParameter.Name != null)
				{
					name = chartFormulaParameter.Name;
				}
				if (chartFormulaParameter.Value != null)
				{
					object value2 = chartFormulaParameter.Value.IsExpression ? chartFormulaParameter.Instance.Value : chartFormulaParameter.Value.Value;
					value = Convert.ToString(value2, CultureInfo.InvariantCulture);
				}
			}

			private static void AppendParameter(Dictionary<string, string> parameters, string parameterName, string defaultValue, ref string formulaParameters)
			{
				if (formulaParameters != string.Empty)
				{
					formulaParameters += ",";
				}
				string text = null;
				if (parameters.ContainsKey(parameterName))
				{
					text = parameters[parameterName];
				}
				if (text != null)
				{
					formulaParameters += text;
				}
				else
				{
					formulaParameters += defaultValue;
				}
			}

			private static string GetParameter(Dictionary<string, string> parameters, string parameterName)
			{
				string text = null;
				if (parameters.ContainsKey(parameterName))
				{
					text = parameters[parameterName];
				}
				if (text == null)
				{
					return "";
				}
				return text;
			}

			internal static string GetDerivedSeriesName(string sourceSeriesName)
			{
				return sourceSeriesName + "_Formula";
			}

			internal static bool IsNewAreaRequired(ChartSeriesFormula formula)
			{
				switch (formula)
				{
				case ChartSeriesFormula.MACD:
				case ChartSeriesFormula.Performance:
				case ChartSeriesFormula.RateOfChange:
				case ChartSeriesFormula.RelativeStrengthIndex:
				case ChartSeriesFormula.StandardDeviation:
				case ChartSeriesFormula.TRIX:
					return true;
				default:
					return false;
				}
			}

			internal static bool ShouldSendDerivedSeriesBack(SeriesChartType type)
			{
				if (type != SeriesChartType.Line)
				{
					return type != SeriesChartType.Spline;
				}
				return false;
			}
		}

		private class TraceContext : ITraceContext
		{
			private DateTime m_startTime;

			private DateTime m_lastOperation;

			public bool TraceEnabled
			{
				get
				{
					return true;
				}
			}

			public TraceContext()
			{
				this.m_startTime = (this.m_lastOperation = DateTime.Now);
			}

			public void Write(string category, string message)
			{
				RSTrace.ProcessingTracer.Trace(category + "; " + message + "; " + (DateTime.Now - this.m_startTime).TotalMilliseconds + "; " + (DateTime.Now - this.m_lastOperation).TotalMilliseconds);
				this.m_lastOperation = DateTime.Now;
			}
		}

		private Chart m_chart;

		private ActionInfoWithDynamicImageMapCollection m_actions = new ActionInfoWithDynamicImageMapCollection();

		private AspNetCore.Reporting.Chart.WebForms.Chart m_coreChart;

		private bool m_multiColumn;

		private bool m_multiRow;

		private Formatter m_formatter;

		private Dictionary<string, ChartAreaInfo> m_chartAreaInfoDictionary = new Dictionary<string, ChartAreaInfo>();

		private Hatcher m_hatcher;

		private AutoMarker m_autoMarker;

		private static string m_legendTextSeparator = " - ";

		private static string m_defaulChartAreaName = "Default";

		private static string m_imagePrefix = "KatmaiChartMapperImage";

		private static string m_pieAutoAxisLabelsName = "AutoAxisLabels";

		private static string m_defaultMarkerSizeString = "3.75pt";

		private static string m_defaultCalloutLineWidthString = "0.75pt";

		private static string m_defaultMaxMovingDistanceString = "23pt";

		private static ReportSize m_defaultMarkerSize = new ReportSize(ChartMapper.m_defaultMarkerSizeString);

		private static ReportSize m_defaultCalloutLineWidth = new ReportSize(ChartMapper.m_defaultCalloutLineWidthString);

		private static ReportSize m_defaultMaxMovingDistance = new ReportSize(ChartMapper.m_defaultMaxMovingDistanceString);

		private static LabelsAutoFitStyles m_defaultLabelsAngleStep = LabelsAutoFitStyles.LabelsAngleStep90;

		private static ChartDashStyle m_defaultCoreDataPointBorderStyle = ChartDashStyle.Solid;

		private static int m_defaultCoreDataPointBorderWidth = 1;

		public ChartMapper(Chart chart, string defaultFontFamily)
			: base(defaultFontFamily)
		{
			this.m_chart = chart;
		}

		public void RenderChart()
		{
			try
			{
				if (this.m_chart != null)
				{
					this.InitializeChart();
					this.SetChartProperties();
					this.RenderChartStyle();
					this.RenderBorderSkin();
					this.RenderPalettes();
					this.RenderChartAreas();
					this.RenderLegends();
					this.RenderTitles();
					this.RenderAnnotations();
					this.RenderData();
					if (this.IsChartEmpty())
					{
						this.m_coreChart.Series.Clear();
						this.RenderNoDataMessage();
					}
					this.m_coreChart.SuppressExceptions = true;
				}
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex2))
				{
					throw;
				}
				throw new RenderingObjectModelException(ex2);
			}
		}

		public Stream GetCoreXml()
		{
			try
			{
				this.m_coreChart.Serializer.Content = SerializationContents.All;
				this.m_coreChart.Serializer.NonSerializableContent = "";
				MemoryStream memoryStream = new MemoryStream();
				this.m_coreChart.Serializer.Save(memoryStream);
				memoryStream.Position = 0L;
				return memoryStream;
			}
			catch (Exception ex)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex))
				{
					throw;
				}
				Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
				return null;
			}
		}

		public Stream GetImage(DynamicImageInstance.ImageType imageType)
		{
			try
			{
				if (this.m_coreChart == null)
				{
					return null;
				}
				ChartImageFormat format = ChartImageFormat.Png;
				Stream stream = null;
				switch (imageType)
				{
				case DynamicImageInstance.ImageType.EMF:
					format = ChartImageFormat.EmfPlus;
					stream = this.m_chart.RenderingContext.OdpContext.CreateStreamCallback(this.m_chart.Name, "emf", null, "image/emf", true, StreamOper.CreateOnly);
					break;
				case DynamicImageInstance.ImageType.PNG:
					format = ChartImageFormat.Png;
					stream = new MemoryStream();
					break;
				}
				AspNetCore.Reporting.Chart.WebForms.Chart coreChart = this.m_coreChart;
				coreChart.FormatNumberHandler = (FormatNumberHandler)Delegate.Combine(coreChart.FormatNumberHandler, new FormatNumberHandler(this.FormatNumber));
				this.m_coreChart.CustomizeLegend += this.AdjustSeriesInLegend;
				this.m_coreChart.ImageResolution = base.DpiX;
				this.m_coreChart.Save(stream, format);
				stream.Position = 0L;
				return stream;
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex2))
				{
					throw;
				}
				throw new RenderingObjectModelException(ex2);
			}
		}

		private string FormatNumber(object sender, double value, string format, ChartValueTypes valueType, int elementId, ChartElementType elementType)
		{
			if (this.m_formatter == null)
			{
				this.m_formatter = new Formatter(this.m_chart.ChartDef.StyleClass, this.m_chart.RenderingContext.OdpContext, ObjectType.Chart, this.m_chart.Name);
			}
			bool addDateTimeOffsetSuffix = false;
			if (format.Length == 0)
			{
				switch (valueType)
				{
				case ChartValueTypes.DateTime:
				case ChartValueTypes.Date:
				case ChartValueTypes.DateTimeOffset:
					format = "d";
					break;
				case ChartValueTypes.Time:
					format = "t";
					break;
				}
				addDateTimeOffsetSuffix = (valueType == ChartValueTypes.DateTimeOffset);
			}
			TypeCode typeCode = this.GetTypeCode(valueType);
			object value2 = (typeCode != TypeCode.DateTime) ? ((object)value) : ((object)DateTime.FromOADate(value));
			return this.m_formatter.FormatValue(value2, format, typeCode, addDateTimeOffsetSuffix);
		}

		private TypeCode GetTypeCode(ChartValueTypes chartValueType)
		{
			switch (chartValueType)
			{
			case ChartValueTypes.DateTime:
			case ChartValueTypes.Date:
			case ChartValueTypes.Time:
			case ChartValueTypes.DateTimeOffset:
				return TypeCode.DateTime;
			default:
				return TypeCode.Double;
			}
		}

		private ImageMapArea.ImageMapAreaShape GetMapAreaShape(MapAreaShape shape)
		{
			if (shape == MapAreaShape.Rectangle)
			{
				return ImageMapArea.ImageMapAreaShape.Rectangle;
			}
			if (MapAreaShape.Circle == shape)
			{
				return ImageMapArea.ImageMapAreaShape.Circle;
			}
			return ImageMapArea.ImageMapAreaShape.Polygon;
		}

		public ActionInfoWithDynamicImageMapCollection GetImageMaps()
		{
			return MappingHelper.GetImageMaps(this.GetMapAreaInfoList(), this.m_actions, this.m_chart);
		}

		internal IEnumerable<MappingHelper.MapAreaInfo> GetMapAreaInfoList()
		{
			foreach (MapArea mapArea in this.m_coreChart.MapAreas)
			{
				yield return new MappingHelper.MapAreaInfo(mapArea.ToolTip, ((IMapAreaAttributes)mapArea).Tag, this.GetMapAreaShape(mapArea.Shape), mapArea.Coordinates);
			}
		}

		private void InitializeChart()
		{
			this.m_coreChart = new AspNetCore.Reporting.Chart.WebForms.Chart();
			if (RSTrace.ProcessingTracer.TraceVerbose)
			{
				TraceManager traceManager = (TraceManager)this.m_coreChart.GetService(typeof(TraceManager));
				traceManager.TraceContext = new TraceContext();
			}
			this.m_coreChart.ChartAreas.Clear();
			this.m_coreChart.Series.Clear();
			this.m_coreChart.Titles.Clear();
			this.m_coreChart.Legends.Clear();
			this.m_coreChart.Annotations.Clear();
			this.OnPostInitialize();
		}

		private void SetChartProperties()
		{
			int width = 300;
			int height = 300;
			if (base.WidthOverrideInPixels.HasValue)
			{
				width = base.WidthOverrideInPixels.Value;
			}
			else if (this.m_chart.DynamicWidth != null)
			{
				if (!this.m_chart.DynamicWidth.IsExpression)
				{
					if (this.m_chart.DynamicWidth.Value != null)
					{
						width = MappingHelper.ToIntPixels(this.m_chart.DynamicWidth.Value, base.DpiX);
					}
				}
				else if (((ChartInstance)this.m_chart.Instance).DynamicWidth != null)
				{
					width = MappingHelper.ToIntPixels(((ChartInstance)this.m_chart.Instance).DynamicWidth, base.DpiX);
				}
			}
			this.m_coreChart.Width = width;
			if (base.HeightOverrideInPixels.HasValue)
			{
				height = base.HeightOverrideInPixels.Value;
			}
			else if (this.m_chart.DynamicHeight != null)
			{
				if (!this.m_chart.DynamicHeight.IsExpression)
				{
					if (this.m_chart.DynamicHeight.Value != null)
					{
						height = MappingHelper.ToIntPixels(this.m_chart.DynamicHeight.Value, base.DpiY);
					}
				}
				else if (((ChartInstance)this.m_chart.Instance).DynamicHeight != null)
				{
					height = MappingHelper.ToIntPixels(((ChartInstance)this.m_chart.Instance).DynamicHeight, base.DpiY);
				}
			}
			this.m_coreChart.Height = height;
		}

		private void RenderNoDataMessage()
		{
			if (this.m_chart.NoDataMessage != null)
			{
				Title title = new Title();
				this.m_coreChart.Titles.Add(title);
				this.RenderTitle(this.m_chart.NoDataMessage, title);
			}
		}

		private void RenderPalettes()
		{
			this.RenderStandardPalettes();
			this.RenderCustomPalette();
			this.RenderPaletteHatchBehavior();
		}

		private void RenderStandardPalettes()
		{
			if (this.m_chart.Palette == null)
			{
				this.m_coreChart.Palette = ChartColorPalette.Default;
			}
			else
			{
				ChartPalette chartPalette = ChartPalette.BrightPastel;
				switch (this.m_chart.Palette.IsExpression ? ((ChartInstance)this.m_chart.Instance).Palette : this.m_chart.Palette.Value)
				{
				case ChartPalette.Default:
					this.m_coreChart.Palette = ChartColorPalette.Default;
					break;
				case ChartPalette.EarthTones:
					this.m_coreChart.Palette = ChartColorPalette.EarthTones;
					break;
				case ChartPalette.Excel:
					this.m_coreChart.Palette = ChartColorPalette.Excel;
					break;
				case ChartPalette.GrayScale:
					this.m_coreChart.Palette = ChartColorPalette.Grayscale;
					break;
				case ChartPalette.Light:
					this.m_coreChart.Palette = ChartColorPalette.Light;
					break;
				case ChartPalette.Pastel:
					this.m_coreChart.Palette = ChartColorPalette.Pastel;
					break;
				case ChartPalette.SemiTransparent:
					this.m_coreChart.Palette = ChartColorPalette.Semitransparent;
					break;
				case ChartPalette.Berry:
					this.m_coreChart.Palette = ChartColorPalette.Berry;
					break;
				case ChartPalette.BrightPastel:
					this.m_coreChart.Palette = ChartColorPalette.BrightPastel;
					break;
				case ChartPalette.Chocolate:
					this.m_coreChart.Palette = ChartColorPalette.Chocolate;
					break;
				case ChartPalette.Custom:
					this.m_coreChart.Palette = ChartColorPalette.None;
					break;
				case ChartPalette.Fire:
					this.m_coreChart.Palette = ChartColorPalette.Fire;
					break;
				case ChartPalette.SeaGreen:
					this.m_coreChart.Palette = ChartColorPalette.SeaGreen;
					break;
				case ChartPalette.Pacific:
					this.m_coreChart.Palette = ChartColorPalette.Pacific;
					break;
				case ChartPalette.PacificLight:
					this.m_coreChart.Palette = ChartColorPalette.PacificLight;
					break;
				case ChartPalette.PacificSemiTransparent:
					this.m_coreChart.Palette = ChartColorPalette.PacificSemiTransparent;
					break;
				}
			}
		}

		private void RenderPaletteHatchBehavior()
		{
			if (this.m_chart.PaletteHatchBehavior != null)
			{
				ReportEnumProperty<PaletteHatchBehavior> paletteHatchBehavior = this.m_chart.PaletteHatchBehavior;
				PaletteHatchBehavior paletteHatchBehavior2 = PaletteHatchBehavior.None;
				paletteHatchBehavior2 = (paletteHatchBehavior.IsExpression ? ((ChartInstance)this.m_chart.Instance).PaletteHatchBehavior : paletteHatchBehavior.Value);
				if (paletteHatchBehavior2 == PaletteHatchBehavior.Always)
				{
					this.m_hatcher = new Hatcher();
				}
			}
		}

		private void RenderCustomPalette()
		{
			if (this.m_chart.CustomPaletteColors != null && this.m_chart.CustomPaletteColors.Count != 0 && this.m_coreChart.Palette == ChartColorPalette.None)
			{
				Color[] array = new Color[this.m_chart.CustomPaletteColors.Count];
				Color empty = Color.Empty;
				for (int i = 0; i < this.m_chart.CustomPaletteColors.Count; i++)
				{
					ChartCustomPaletteColor chartCustomPaletteColor = ((ChartObjectCollectionBase<ChartCustomPaletteColor, ChartCustomPaletteColorInstance>)this.m_chart.CustomPaletteColors)[i];
					ReportColorProperty color = chartCustomPaletteColor.Color;
					if (!color.IsExpression)
					{
						if (MappingHelper.GetColorFromReportColorProperty(color, ref empty))
						{
							array[i] = empty;
						}
					}
					else
					{
						array[i] = chartCustomPaletteColor.Instance.Color.ToColor();
					}
				}
				this.m_coreChart.PaletteCustomColors = array;
			}
		}

		private void RenderChartStyle()
		{
			Border border = null;
			this.m_coreChart.BackColor = Color.Transparent;
			Style style = this.m_chart.Style;
			if (style != null)
			{
				StyleInstance style2 = this.m_chart.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(this.m_chart.Style.BackgroundColor))
				{
					this.m_coreChart.BackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(this.m_chart.Style.BackgroundGradientEndColor))
				{
					this.m_coreChart.BackGradientEndColor = MappingHelper.GetStyleBackGradientEndColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(this.m_chart.Style.BackgroundGradientType))
				{
					this.m_coreChart.BackGradientType = this.GetGradientType(MappingHelper.GetStyleBackGradientType(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(this.m_chart.Style.BackgroundHatchType))
				{
					this.m_coreChart.BackHatchStyle = this.GetHatchType(MappingHelper.GetStyleBackgroundHatchType(style, style2));
				}
				this.m_coreChart.RightToLeft = MappingHelper.GetStyleDirection(this.m_chart.Style, this.m_chart.Instance.Style);
				this.RenderChartBackgroundImage(this.m_chart.Style.BackgroundImage);
				border = this.m_chart.Style.Border;
			}
			if (this.m_coreChart.BackColor.A != 255)
			{
				this.m_coreChart.AntiAlias = AntiAlias.Off;
			}
			if (this.m_chart.SpecialBorderHandling)
			{
				this.RenderChartBorder(border);
			}
		}

		private void RenderBorderSkin()
		{
			if (this.m_chart.BorderSkin != null)
			{
				this.RenderBorderSkinStyle(this.m_chart.BorderSkin);
				this.RenderBorderSkinType(this.m_chart.BorderSkin);
			}
		}

		private void RenderBorderSkinType(ChartBorderSkin borderSkin)
		{
			if (borderSkin.BorderSkinType != null)
			{
				if (!borderSkin.BorderSkinType.IsExpression)
				{
					ChartBorderSkinType value = borderSkin.BorderSkinType.Value;
				}
				else
				{
					ChartBorderSkinType borderSkinType = borderSkin.Instance.BorderSkinType;
				}
				BorderSkinStyle skinStyle = BorderSkinStyle.None;
				switch (borderSkin.Instance.BorderSkinType)
				{
				case ChartBorderSkinType.Emboss:
					skinStyle = BorderSkinStyle.Emboss;
					break;
				case ChartBorderSkinType.FrameThin1:
					skinStyle = BorderSkinStyle.FrameThin1;
					break;
				case ChartBorderSkinType.FrameThin2:
					skinStyle = BorderSkinStyle.FrameThin2;
					break;
				case ChartBorderSkinType.FrameThin3:
					skinStyle = BorderSkinStyle.FrameThin3;
					break;
				case ChartBorderSkinType.FrameThin4:
					skinStyle = BorderSkinStyle.FrameThin4;
					break;
				case ChartBorderSkinType.FrameThin5:
					skinStyle = BorderSkinStyle.FrameThin5;
					break;
				case ChartBorderSkinType.FrameThin6:
					skinStyle = BorderSkinStyle.FrameThin6;
					break;
				case ChartBorderSkinType.FrameTitle1:
					skinStyle = BorderSkinStyle.FrameTitle1;
					break;
				case ChartBorderSkinType.FrameTitle2:
					skinStyle = BorderSkinStyle.FrameTitle2;
					break;
				case ChartBorderSkinType.FrameTitle3:
					skinStyle = BorderSkinStyle.FrameTitle3;
					break;
				case ChartBorderSkinType.FrameTitle4:
					skinStyle = BorderSkinStyle.FrameTitle4;
					break;
				case ChartBorderSkinType.FrameTitle5:
					skinStyle = BorderSkinStyle.FrameTitle5;
					break;
				case ChartBorderSkinType.FrameTitle6:
					skinStyle = BorderSkinStyle.FrameTitle6;
					break;
				case ChartBorderSkinType.FrameTitle7:
					skinStyle = BorderSkinStyle.FrameTitle7;
					break;
				case ChartBorderSkinType.FrameTitle8:
					skinStyle = BorderSkinStyle.FrameTitle8;
					break;
				case ChartBorderSkinType.None:
					skinStyle = BorderSkinStyle.None;
					break;
				case ChartBorderSkinType.Raised:
					skinStyle = BorderSkinStyle.Raised;
					break;
				case ChartBorderSkinType.Sunken:
					skinStyle = BorderSkinStyle.Sunken;
					break;
				}
				this.m_coreChart.BorderSkin.SkinStyle = skinStyle;
			}
		}

		private void RenderBorderSkinStyle(ChartBorderSkin chartBorderSkin)
		{
			Style style = chartBorderSkin.Style;
			if (style != null)
			{
				StyleInstance style2 = chartBorderSkin.Instance.Style;
				BorderSkinAttributes borderSkin = this.m_coreChart.BorderSkin;
				if (MappingHelper.IsStylePropertyDefined(chartBorderSkin.Style.Color))
				{
					borderSkin.PageColor = MappingHelper.GetStyleColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartBorderSkin.Style.BackgroundColor))
				{
					borderSkin.FrameBackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartBorderSkin.Style.BackgroundGradientEndColor))
				{
					borderSkin.FrameBackGradientEndColor = MappingHelper.GetStyleBackGradientEndColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartBorderSkin.Style.BackgroundGradientType))
				{
					borderSkin.FrameBackGradientType = this.GetGradientType(MappingHelper.GetStyleBackGradientType(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(chartBorderSkin.Style.BackgroundHatchType))
				{
					borderSkin.FrameBackHatchStyle = this.GetHatchType(MappingHelper.GetStyleBackgroundHatchType(style, style2));
				}
				this.RenderBorderSkinBorder(chartBorderSkin.Style.Border, borderSkin);
				if (style.BackgroundImage != null)
				{
					this.RenderBorderSkinBackgroundImage(style.BackgroundImage, borderSkin);
				}
			}
		}

		private void RenderChartAreas()
		{
			if (this.m_chart.ChartAreas == null || this.m_chart.ChartAreas.Count == 0)
			{
				this.m_coreChart.ChartAreas.Add(ChartMapper.m_defaulChartAreaName);
			}
			else
			{
				foreach (ChartArea chartArea in this.m_chart.ChartAreas)
				{
					this.RenderChartArea(chartArea);
				}
			}
		}

		private void RenderChartArea(ChartArea chartArea)
		{
			AspNetCore.Reporting.Chart.WebForms.ChartArea chartArea2 = new AspNetCore.Reporting.Chart.WebForms.ChartArea();
			this.m_coreChart.ChartAreas.Add(chartArea2);
			this.SetChartAreaProperties(chartArea, chartArea2);
			this.RenderElementPosition(chartArea.ChartElementPosition, chartArea2.Position);
			this.RenderElementPosition(chartArea.ChartInnerPlotPosition, chartArea2.InnerPlotPosition);
			this.RenderChartAreaStyle(chartArea, chartArea2);
			if (!this.m_chartAreaInfoDictionary.ContainsKey(chartArea2.Name))
			{
				this.m_chartAreaInfoDictionary.Add(chartArea2.Name, new ChartAreaInfo());
			}
			this.RenderAxes(chartArea, chartArea2);
			this.Render3DProperties(chartArea.ThreeDProperties, chartArea2.Area3DStyle);
		}

		private void SetChartAreaProperties(ChartArea chartArea, AspNetCore.Reporting.Chart.WebForms.ChartArea area)
		{
			this.RenderAlignType(chartArea.ChartAlignType, area);
			if (chartArea.Name != null)
			{
				area.Name = chartArea.Name;
			}
			else
			{
				area.Name = ChartMapper.m_defaulChartAreaName;
			}
			if (chartArea.AlignOrientation != null)
			{
				if (!chartArea.AlignOrientation.IsExpression)
				{
					area.AlignOrientation = this.GetAreaAlignOrientation(chartArea.AlignOrientation.Value);
				}
				else
				{
					area.AlignOrientation = this.GetAreaAlignOrientation(chartArea.Instance.AlignOrientation);
				}
			}
			else
			{
				area.AlignOrientation = AreaAlignOrientations.None;
			}
			if (chartArea.AlignWithChartArea != null)
			{
				area.AlignWithChartArea = chartArea.AlignWithChartArea;
			}
			if (chartArea.EquallySizedAxesFont != null)
			{
				if (!chartArea.EquallySizedAxesFont.IsExpression)
				{
					area.EquallySizedAxesFont = chartArea.EquallySizedAxesFont.Value;
				}
				else
				{
					area.EquallySizedAxesFont = chartArea.Instance.EquallySizedAxesFont;
				}
			}
			if (chartArea.Hidden != null)
			{
				if (!chartArea.Hidden.IsExpression)
				{
					area.Visible = !chartArea.Hidden.Value;
				}
				else
				{
					area.Visible = !chartArea.Instance.Hidden;
				}
			}
		}

		private AreaAlignOrientations GetAreaAlignOrientation(ChartAreaAlignOrientations chartAreaOrientation)
		{
			switch (chartAreaOrientation)
			{
			case ChartAreaAlignOrientations.All:
				return AreaAlignOrientations.All;
			case ChartAreaAlignOrientations.Horizontal:
				return AreaAlignOrientations.Horizontal;
			case ChartAreaAlignOrientations.Vertical:
				return AreaAlignOrientations.Vertical;
			default:
				return AreaAlignOrientations.None;
			}
		}

		private void RenderAlignType(ChartAlignType chartAlignType, AspNetCore.Reporting.Chart.WebForms.ChartArea area)
		{
			area.AlignType = AreaAlignTypes.None;
			if (chartAlignType != null)
			{
				if (chartAlignType.AxesView != null)
				{
					if (!chartAlignType.AxesView.IsExpression)
					{
						if (chartAlignType.AxesView.Value)
						{
							area.AlignType |= AreaAlignTypes.AxesView;
						}
					}
					else if (chartAlignType.Instance.AxesView)
					{
						area.AlignType |= AreaAlignTypes.AxesView;
					}
				}
				if (chartAlignType.Cursor != null)
				{
					if (!chartAlignType.Cursor.IsExpression)
					{
						if (chartAlignType.Cursor.Value)
						{
							area.AlignType |= AreaAlignTypes.Cursor;
						}
					}
					else if (chartAlignType.Instance.Cursor)
					{
						area.AlignType |= AreaAlignTypes.Cursor;
					}
				}
				if (chartAlignType.InnerPlotPosition != null)
				{
					if (!chartAlignType.InnerPlotPosition.IsExpression)
					{
						if (chartAlignType.InnerPlotPosition.Value)
						{
							area.AlignType |= AreaAlignTypes.PlotPosition;
						}
					}
					else if (chartAlignType.Instance.InnerPlotPosition)
					{
						area.AlignType |= AreaAlignTypes.PlotPosition;
					}
				}
				if (chartAlignType.Position != null)
				{
					if (!chartAlignType.Position.IsExpression)
					{
						if (chartAlignType.Position.Value)
						{
							area.AlignType |= AreaAlignTypes.Position;
						}
					}
					else if (chartAlignType.Instance.Position)
					{
						area.AlignType |= AreaAlignTypes.Position;
					}
				}
			}
		}

		private void RenderChartAreaStyle(ChartArea chartArea, AspNetCore.Reporting.Chart.WebForms.ChartArea area)
		{
			Style style = chartArea.Style;
			if (style != null)
			{
				StyleInstance style2 = chartArea.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(chartArea.Style.BackgroundColor))
				{
					area.BackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartArea.Style.BackgroundGradientEndColor))
				{
					area.BackGradientEndColor = MappingHelper.GetStyleBackGradientEndColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartArea.Style.BackgroundGradientType))
				{
					area.BackGradientType = this.GetGradientType(MappingHelper.GetStyleBackGradientType(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(chartArea.Style.BackgroundHatchType))
				{
					area.BackHatchStyle = this.GetHatchType(MappingHelper.GetStyleBackgroundHatchType(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(chartArea.Style.ShadowColor))
				{
					area.ShadowColor = MappingHelper.GetStyleShadowColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartArea.Style.ShadowOffset))
				{
					area.ShadowOffset = MappingHelper.GetStyleShadowOffset(style, style2, base.DpiX);
				}
				this.RenderChartAreaBorder(chartArea.Style.Border, area);
				this.RenderChartAreaBackgroundImage(chartArea.Style.BackgroundImage, area);
			}
		}

		private void RenderAxes(ChartArea chartArea, AspNetCore.Reporting.Chart.WebForms.ChartArea area)
		{
			if (chartArea.CategoryAxes != null)
			{
				this.RenderCategoryAxes(chartArea.CategoryAxes, area);
			}
			if (chartArea.ValueAxes != null)
			{
				this.RenderValueAxes(chartArea.ValueAxes, area);
			}
		}

		private void RenderCategoryAxes(ChartAxisCollection categoryAxes, AspNetCore.Reporting.Chart.WebForms.ChartArea area)
		{
			bool flag = false;
			foreach (ChartAxis categoryAxis in categoryAxes)
			{
				if (categoryAxis.Location != null)
				{
					if (!categoryAxis.Location.IsExpression)
					{
						if (categoryAxis.Location.Value == ChartAxisLocation.Default && !flag)
						{
							this.RenderAxis(categoryAxis, area.AxisX, area, true);
							flag = true;
						}
						else
						{
							this.RenderAxis(categoryAxis, area.AxisX2, area, true);
						}
					}
					else if (categoryAxis.Instance.Location == ChartAxisLocation.Default && !flag)
					{
						this.RenderAxis(categoryAxis, area.AxisX, area, true);
						flag = true;
					}
					else
					{
						this.RenderAxis(categoryAxis, area.AxisX2, area, true);
					}
				}
				else if (!flag)
				{
					this.RenderAxis(categoryAxis, area.AxisX, area, true);
					flag = true;
				}
				else
				{
					this.RenderAxis(categoryAxis, area.AxisX2, area, true);
				}
			}
		}

		private void RenderValueAxes(ChartAxisCollection valueAxes, AspNetCore.Reporting.Chart.WebForms.ChartArea area)
		{
			bool flag = false;
			foreach (ChartAxis valueAxis in valueAxes)
			{
				if (valueAxis.Location != null)
				{
					if (!valueAxis.Location.IsExpression)
					{
						if (valueAxis.Location.Value == ChartAxisLocation.Default && !flag)
						{
							this.RenderAxis(valueAxis, area.AxisY, area, false);
							flag = true;
						}
						else
						{
							this.RenderAxis(valueAxis, area.AxisY2, area, false);
						}
					}
					else if (valueAxis.Instance.Location == ChartAxisLocation.Default && !flag)
					{
						this.RenderAxis(valueAxis, area.AxisY, area, false);
						flag = true;
					}
					else
					{
						this.RenderAxis(valueAxis, area.AxisY2, area, false);
					}
				}
				else if (!flag)
				{
					this.RenderAxis(valueAxis, area.AxisY, area, false);
					flag = true;
				}
				else
				{
					this.RenderAxis(valueAxis, area.AxisY2, area, false);
				}
			}
		}

		private void Render3DProperties(ChartThreeDProperties chartThreeDProperties, ChartArea3DStyle threeDProperties)
		{
			if (chartThreeDProperties != null)
			{
				if (chartThreeDProperties.Clustered != null)
				{
					if (!chartThreeDProperties.Clustered.IsExpression)
					{
						threeDProperties.Clustered = chartThreeDProperties.Clustered.Value;
					}
					else
					{
						threeDProperties.Clustered = chartThreeDProperties.Instance.Clustered;
					}
				}
				if (chartThreeDProperties.DepthRatio != null)
				{
					if (!chartThreeDProperties.DepthRatio.IsExpression)
					{
						threeDProperties.PointDepth = chartThreeDProperties.DepthRatio.Value;
					}
					else
					{
						threeDProperties.PointDepth = chartThreeDProperties.Instance.DepthRatio;
					}
				}
				if (chartThreeDProperties.Enabled != null)
				{
					if (!chartThreeDProperties.Enabled.IsExpression)
					{
						threeDProperties.Enable3D = chartThreeDProperties.Enabled.Value;
					}
					else
					{
						threeDProperties.Enable3D = chartThreeDProperties.Instance.Enabled;
					}
				}
				if (chartThreeDProperties.GapDepth != null)
				{
					if (!chartThreeDProperties.GapDepth.IsExpression)
					{
						threeDProperties.PointGapDepth = chartThreeDProperties.GapDepth.Value;
					}
					else
					{
						threeDProperties.PointGapDepth = chartThreeDProperties.Instance.GapDepth;
					}
				}
				if (chartThreeDProperties.Inclination != null)
				{
					if (!chartThreeDProperties.Inclination.IsExpression)
					{
						threeDProperties.XAngle = chartThreeDProperties.Inclination.Value;
					}
					else
					{
						threeDProperties.XAngle = chartThreeDProperties.Instance.Inclination;
					}
				}
				if (chartThreeDProperties.Perspective != null)
				{
					if (!chartThreeDProperties.Perspective.IsExpression)
					{
						threeDProperties.Perspective = chartThreeDProperties.Perspective.Value;
					}
					else
					{
						threeDProperties.Perspective = chartThreeDProperties.Instance.Perspective;
					}
				}
				if (chartThreeDProperties.ProjectionMode != null)
				{
					ChartThreeDProjectionModes chartThreeDProjectionModes = ChartThreeDProjectionModes.Perspective;
					chartThreeDProjectionModes = (chartThreeDProperties.ProjectionMode.IsExpression ? chartThreeDProperties.Instance.ProjectionMode : chartThreeDProperties.ProjectionMode.Value);
					threeDProperties.RightAngleAxes = (chartThreeDProjectionModes == ChartThreeDProjectionModes.Oblique);
				}
				else
				{
					threeDProperties.RightAngleAxes = true;
				}
				if (chartThreeDProperties.Rotation != null)
				{
					if (!chartThreeDProperties.Rotation.IsExpression)
					{
						threeDProperties.YAngle = chartThreeDProperties.Rotation.Value;
					}
					else
					{
						threeDProperties.YAngle = chartThreeDProperties.Instance.Rotation;
					}
				}
				if (chartThreeDProperties.Shading != null)
				{
					if (!chartThreeDProperties.Shading.IsExpression)
					{
						threeDProperties.Light = this.GetThreeDLight(chartThreeDProperties.Shading.Value);
					}
					else
					{
						threeDProperties.Light = this.GetThreeDLight(chartThreeDProperties.Instance.Shading);
					}
				}
				else
				{
					threeDProperties.Light = LightStyle.Realistic;
				}
				if (chartThreeDProperties.WallThickness != null)
				{
					if (!chartThreeDProperties.WallThickness.IsExpression)
					{
						threeDProperties.WallWidth = chartThreeDProperties.WallThickness.Value;
					}
					else
					{
						threeDProperties.WallWidth = chartThreeDProperties.Instance.WallThickness;
					}
				}
			}
		}

		private LightStyle GetThreeDLight(ChartThreeDShadingTypes shading)
		{
			switch (shading)
			{
			case ChartThreeDShadingTypes.Real:
				return LightStyle.Realistic;
			case ChartThreeDShadingTypes.Simple:
				return LightStyle.Simplistic;
			default:
				return LightStyle.None;
			}
		}

		private void RenderAxis(ChartAxis chartAxis, AspNetCore.Reporting.Chart.WebForms.Axis axis, AspNetCore.Reporting.Chart.WebForms.ChartArea area, bool isCategory)
		{
			this.RenderAxisStyle(chartAxis, axis);
			this.RenderAxisTitle(chartAxis.Title, axis);
			this.RenderAxisStripLines(chartAxis, axis);
			this.RenderAxisGridLines(chartAxis.MajorGridLines, axis.MajorGrid, true);
			this.RenderAxisGridLines(chartAxis.MinorGridLines, axis.MinorGrid, false);
			this.RenderAxisTickMarks(chartAxis.MajorTickMarks, axis.MajorTickMark, true);
			this.RenderAxisTickMarks(chartAxis.MinorTickMarks, axis.MinorTickMark, false);
			this.RenderAxisScaleBreak(chartAxis.AxisScaleBreak, axis.ScaleBreakStyle);
			this.RenderCustomProperties(chartAxis.CustomProperties, axis);
			this.SetAxisProperties(chartAxis, axis, area, isCategory);
		}

		private void SetAxisProperties(ChartAxis chartAxis, AspNetCore.Reporting.Chart.WebForms.Axis axis, AspNetCore.Reporting.Chart.WebForms.ChartArea area, bool isCategory)
		{
			this.SetAxisArrow(chartAxis, axis);
			this.SetAxisCrossing(chartAxis, axis);
			this.SetAxisLabelsProperties(chartAxis, axis);
			if (chartAxis.IncludeZero != null)
			{
				if (!chartAxis.IncludeZero.IsExpression)
				{
					axis.StartFromZero = chartAxis.IncludeZero.Value;
				}
				else
				{
					axis.StartFromZero = chartAxis.Instance.IncludeZero;
				}
			}
			if (chartAxis.Interlaced != null)
			{
				if (!chartAxis.Interlaced.IsExpression)
				{
					axis.Interlaced = chartAxis.Interlaced.Value;
				}
				else
				{
					axis.Interlaced = chartAxis.Instance.Interlaced;
				}
			}
			if (chartAxis.InterlacedColor != null)
			{
				Color empty = Color.Empty;
				if (MappingHelper.GetColorFromReportColorProperty(chartAxis.InterlacedColor, ref empty))
				{
					axis.InterlacedColor = empty;
				}
				else if (chartAxis.Instance.InterlacedColor != null)
				{
					axis.InterlacedColor = chartAxis.Instance.InterlacedColor.ToColor();
				}
			}
			if (chartAxis.VariableAutoInterval != null)
			{
				if (!chartAxis.VariableAutoInterval.IsExpression)
				{
					axis.IntervalAutoMode = this.GetIntervalAutoMode(chartAxis.VariableAutoInterval.Value);
				}
				else
				{
					axis.IntervalAutoMode = this.GetIntervalAutoMode(chartAxis.Instance.VariableAutoInterval);
				}
			}
			if (chartAxis.Interval != null)
			{
				double num = chartAxis.Interval.IsExpression ? chartAxis.Instance.Interval : chartAxis.Interval.Value;
				if (num == 0.0)
				{
					num = double.NaN;
				}
				axis.Interval = num;
			}
			else
			{
				axis.Interval = double.NaN;
			}
			if (chartAxis.IntervalType != null)
			{
				if (!chartAxis.IntervalType.IsExpression)
				{
					axis.IntervalType = this.GetDateTimeIntervalType(chartAxis.IntervalType.Value);
				}
				else
				{
					axis.IntervalType = this.GetDateTimeIntervalType(chartAxis.Instance.IntervalType);
				}
			}
			if (chartAxis.IntervalOffset != null)
			{
				if (!chartAxis.IntervalOffset.IsExpression)
				{
					axis.IntervalOffset = chartAxis.IntervalOffset.Value;
				}
				else
				{
					axis.IntervalOffset = chartAxis.Instance.IntervalOffset;
				}
			}
			if (chartAxis.IntervalOffsetType != null)
			{
				if (!chartAxis.IntervalOffsetType.IsExpression)
				{
					axis.IntervalOffsetType = this.GetDateTimeIntervalType(chartAxis.IntervalOffsetType.Value);
				}
				else
				{
					axis.IntervalOffsetType = this.GetDateTimeIntervalType(chartAxis.Instance.IntervalOffsetType);
				}
			}
			if (chartAxis.LabelInterval != null)
			{
				double num2 = chartAxis.LabelInterval.IsExpression ? chartAxis.Instance.LabelInterval : chartAxis.LabelInterval.Value;
				if (num2 == 0.0)
				{
					num2 = double.NaN;
				}
				axis.LabelStyle.Interval = num2;
			}
			else
			{
				axis.LabelStyle.Interval = double.NaN;
			}
			if (chartAxis.LabelIntervalType != null)
			{
				if (!chartAxis.LabelIntervalType.IsExpression)
				{
					axis.LabelStyle.IntervalType = this.GetDateTimeIntervalType(chartAxis.LabelIntervalType.Value);
				}
				else
				{
					axis.LabelStyle.IntervalType = this.GetDateTimeIntervalType(chartAxis.Instance.LabelIntervalType);
				}
			}
			if (chartAxis.LabelIntervalOffset != null)
			{
				if (!chartAxis.LabelIntervalOffset.IsExpression)
				{
					axis.LabelStyle.IntervalOffset = chartAxis.LabelIntervalOffset.Value;
				}
				else
				{
					axis.LabelStyle.IntervalOffset = chartAxis.Instance.LabelIntervalOffset;
				}
			}
			if (chartAxis.LabelIntervalOffsetType != null)
			{
				if (!chartAxis.LabelIntervalOffsetType.IsExpression)
				{
					axis.LabelStyle.IntervalOffsetType = this.GetDateTimeIntervalType(chartAxis.LabelIntervalOffsetType.Value);
				}
				else
				{
					axis.LabelStyle.IntervalOffsetType = this.GetDateTimeIntervalType(chartAxis.Instance.LabelIntervalOffsetType);
				}
			}
			if (chartAxis.LogBase != null)
			{
				if (!chartAxis.LogBase.IsExpression)
				{
					axis.LogarithmBase = chartAxis.LogBase.Value;
				}
				else
				{
					axis.LogarithmBase = chartAxis.Instance.LogBase;
				}
			}
			if (chartAxis.LogScale != null)
			{
				if (!chartAxis.LogScale.IsExpression)
				{
					axis.Logarithmic = chartAxis.LogScale.Value;
				}
				else
				{
					axis.Logarithmic = chartAxis.Instance.LogScale;
				}
			}
			ChartAutoBool chartAutoBool = (chartAxis.Margin != null) ? (chartAxis.Margin.IsExpression ? chartAxis.Instance.Margin : chartAxis.Margin.Value) : ChartAutoBool.Auto;
			if (chartAutoBool == ChartAutoBool.Auto && isCategory)
			{
				List<AspNetCore.Reporting.Chart.WebForms.Axis> list = this.m_chartAreaInfoDictionary[area.Name].CategoryAxesAutoMargin;
				if (list == null)
				{
					list = new List<AspNetCore.Reporting.Chart.WebForms.Axis>();
					this.m_chartAreaInfoDictionary[area.Name].CategoryAxesAutoMargin = list;
				}
				list.Add(axis);
				axis.Margin = false;
			}
			else
			{
				axis.Margin = this.GetMargin(chartAutoBool);
			}
			if (chartAxis.MarksAlwaysAtPlotEdge != null)
			{
				if (!chartAxis.MarksAlwaysAtPlotEdge.IsExpression)
				{
					axis.MarksNextToAxis = !chartAxis.MarksAlwaysAtPlotEdge.Value;
				}
				else
				{
					axis.MarksNextToAxis = !chartAxis.Instance.MarksAlwaysAtPlotEdge;
				}
			}
			if (chartAxis.Maximum != null)
			{
				if (!chartAxis.Maximum.IsExpression)
				{
					axis.Maximum = this.ConvertToDouble(chartAxis.Maximum.Value);
				}
				else
				{
					axis.Maximum = this.ConvertToDouble(chartAxis.Instance.Maximum);
				}
			}
			if (chartAxis.Minimum != null)
			{
				if (!chartAxis.Minimum.IsExpression)
				{
					axis.Minimum = this.ConvertToDouble(chartAxis.Minimum.Value);
				}
				else
				{
					axis.Minimum = this.ConvertToDouble(chartAxis.Instance.Minimum);
				}
			}
			if (chartAxis.Name != null)
			{
				axis.Name = chartAxis.Name;
			}
			if (chartAxis.Reverse != null)
			{
				if (!chartAxis.Reverse.IsExpression)
				{
					axis.Reverse = chartAxis.Reverse.Value;
				}
				else
				{
					axis.Reverse = chartAxis.Instance.Reverse;
				}
			}
			if (chartAxis.Scalar && isCategory && this.m_chartAreaInfoDictionary.ContainsKey(area.Name))
			{
				ChartAreaInfo chartAreaInfo = this.m_chartAreaInfoDictionary[area.Name];
				if (chartAreaInfo.CategoryAxesScalar == null)
				{
					chartAreaInfo.CategoryAxesScalar = new List<string>();
				}
				chartAreaInfo.CategoryAxesScalar.Add(chartAxis.Name);
			}
			if (chartAxis.Visible != null)
			{
				if (!chartAxis.Visible.IsExpression)
				{
					axis.Enabled = this.GetAxisEnabled(chartAxis.Visible.Value);
				}
				else
				{
					axis.Enabled = this.GetAxisEnabled(chartAxis.Instance.Visible);
				}
			}
			this.SetAxisLabelAutoFitStyle(chartAxis, axis);
		}

		private void SetAxisLabelsProperties(ChartAxis chartAxis, AspNetCore.Reporting.Chart.WebForms.Axis axis)
		{
			if (chartAxis.HideLabels != null)
			{
				if (!chartAxis.HideLabels.IsExpression)
				{
					axis.LabelStyle.Enabled = !chartAxis.HideLabels.Value;
				}
				else
				{
					axis.LabelStyle.Enabled = !chartAxis.Instance.HideLabels;
				}
			}
			if (chartAxis.OffsetLabels != null)
			{
				if (!chartAxis.OffsetLabels.IsExpression)
				{
					axis.LabelStyle.OffsetLabels = chartAxis.OffsetLabels.Value;
				}
				else
				{
					axis.LabelStyle.OffsetLabels = chartAxis.Instance.OffsetLabels;
				}
			}
			if (chartAxis.HideEndLabels != null)
			{
				if (!chartAxis.HideEndLabels.IsExpression)
				{
					axis.LabelStyle.ShowEndLabels = !chartAxis.HideEndLabels.Value;
				}
				else
				{
					axis.LabelStyle.ShowEndLabels = !chartAxis.Instance.HideEndLabels;
				}
			}
			if (chartAxis.Angle != null)
			{
				if (!chartAxis.Angle.IsExpression)
				{
					axis.LabelStyle.FontAngle = (int)Math.Round(chartAxis.Angle.Value);
				}
				else
				{
					axis.LabelStyle.FontAngle = (int)Math.Round(chartAxis.Instance.Angle);
				}
			}
		}

		private void RenderAxisLabelFont(ChartAxis chartAxis, AspNetCore.Reporting.Chart.WebForms.Axis axis)
		{
			Style style = chartAxis.Style;
			if (style == null)
			{
				axis.LabelStyle.Font = base.GetDefaultFont();
			}
			else
			{
				axis.LabelStyle.Font = base.GetFont(style, chartAxis.Instance.Style);
			}
		}

		private void SetAxisCrossing(ChartAxis chartAxis, AspNetCore.Reporting.Chart.WebForms.Axis axis)
		{
			if (chartAxis.CrossAt != null)
			{
				if (!chartAxis.CrossAt.IsExpression)
				{
					if (chartAxis.CrossAt.Value != null)
					{
						double num = this.ConvertToDouble(chartAxis.CrossAt.Value, true);
						if (!double.IsNaN(num))
						{
							axis.Crossing = num;
						}
					}
				}
				else if (chartAxis.Instance.CrossAt != null)
				{
					double num = this.ConvertToDouble(chartAxis.Instance.CrossAt, true);
					if (!double.IsNaN(num))
					{
						axis.Crossing = num;
					}
				}
			}
		}

		private void SetAxisLabelAutoFitStyle(ChartAxis chartAxis, AspNetCore.Reporting.Chart.WebForms.Axis axis)
		{
			if (chartAxis.MaxFontSize != null)
			{
				if (!chartAxis.MaxFontSize.IsExpression)
				{
					axis.LabelsAutoFitMaxFontSize = (int)Math.Round(chartAxis.MaxFontSize.Value.ToPoints());
				}
				else
				{
					axis.LabelsAutoFitMaxFontSize = (int)Math.Round(chartAxis.Instance.MaxFontSize.ToPoints());
				}
			}
			if (chartAxis.MinFontSize != null)
			{
				if (!chartAxis.MinFontSize.IsExpression)
				{
					axis.LabelsAutoFitMinFontSize = (int)Math.Round(chartAxis.MinFontSize.Value.ToPoints());
				}
				else
				{
					axis.LabelsAutoFitMinFontSize = (int)Math.Round(chartAxis.Instance.MinFontSize.ToPoints());
				}
			}
			axis.LabelsAutoFitStyle = LabelsAutoFitStyles.None;
			if (chartAxis.PreventFontGrow != null)
			{
				if (!(chartAxis.PreventFontGrow.IsExpression ? chartAxis.Instance.PreventFontGrow : chartAxis.PreventFontGrow.Value))
				{
					axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.IncreaseFont;
				}
			}
			else
			{
				axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.IncreaseFont;
			}
			if (chartAxis.PreventFontShrink != null)
			{
				if (!(chartAxis.PreventFontShrink.IsExpression ? chartAxis.Instance.PreventFontShrink : chartAxis.PreventFontShrink.Value))
				{
					axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.DecreaseFont;
				}
			}
			else
			{
				axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.DecreaseFont;
			}
			if (chartAxis.PreventLabelOffset != null)
			{
				if (!(chartAxis.PreventLabelOffset.IsExpression ? chartAxis.Instance.PreventLabelOffset : chartAxis.PreventLabelOffset.Value))
				{
					axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.OffsetLabels;
				}
			}
			else
			{
				axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.OffsetLabels;
			}
			if (chartAxis.AllowLabelRotation != null)
			{
				switch (chartAxis.AllowLabelRotation.IsExpression ? chartAxis.Instance.AllowLabelRotation : chartAxis.AllowLabelRotation.Value)
				{
				case ChartAxisLabelRotation.Rotate30:
					axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.LabelsAngleStep30;
					break;
				case ChartAxisLabelRotation.Rotate45:
					axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.LabelsAngleStep45;
					break;
				case ChartAxisLabelRotation.Rotate90:
					axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.LabelsAngleStep90;
					break;
				}
			}
			else
			{
				axis.LabelsAutoFitStyle |= ChartMapper.m_defaultLabelsAngleStep;
			}
			if (chartAxis.PreventWordWrap != null)
			{
				if (!(chartAxis.PreventWordWrap.IsExpression ? chartAxis.Instance.PreventWordWrap : chartAxis.PreventWordWrap.Value))
				{
					axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.WordWrap;
				}
			}
			else
			{
				axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.WordWrap;
			}
			if (chartAxis.LabelsAutoFitDisabled != null)
			{
				if (!chartAxis.LabelsAutoFitDisabled.IsExpression)
				{
					axis.LabelsAutoFit = !chartAxis.LabelsAutoFitDisabled.Value;
				}
				else
				{
					axis.LabelsAutoFit = !chartAxis.Instance.LabelsAutoFitDisabled;
				}
			}
			else
			{
				axis.LabelsAutoFit = true;
			}
		}

		private void SetAxisArrow(ChartAxis chartAxis, AspNetCore.Reporting.Chart.WebForms.Axis axis)
		{
			ChartAxisArrow chartAxisArrow = ChartAxisArrow.None;
			if (chartAxis.Arrows != null)
			{
				switch (chartAxis.Arrows.IsExpression ? chartAxis.Instance.Arrows : chartAxis.Arrows.Value)
				{
				case ChartAxisArrow.None:
					axis.Arrows = ArrowsType.None;
					break;
				case ChartAxisArrow.Lines:
					axis.Arrows = ArrowsType.Lines;
					break;
				case ChartAxisArrow.SharpTriangle:
					axis.Arrows = ArrowsType.SharpTriangle;
					break;
				case ChartAxisArrow.Triangle:
					axis.Arrows = ArrowsType.Triangle;
					break;
				}
			}
		}

		private void RenderAxisStripLines(ChartAxis chartAxis, AspNetCore.Reporting.Chart.WebForms.Axis axis)
		{
			if (chartAxis.StripLines != null)
			{
				foreach (ChartStripLine stripLine2 in chartAxis.StripLines)
				{
					StripLine stripLine = new StripLine();
					this.RenderStripLine(stripLine2, stripLine);
					axis.StripLines.Add(stripLine);
				}
			}
		}

		private void RenderStripLine(ChartStripLine chartStripLine, StripLine stripLine)
		{
			this.SetStripLineProperties(chartStripLine, stripLine);
			this.RenderStripLineStyle(chartStripLine, stripLine);
			this.RenderActionInfo(chartStripLine.ActionInfo, stripLine.ToolTip, stripLine);
		}

		private void SetStripLineProperties(ChartStripLine chartStripLine, StripLine stripLine)
		{
			if (chartStripLine.Interval != null)
			{
				if (!chartStripLine.Interval.IsExpression)
				{
					stripLine.Interval = chartStripLine.Interval.Value;
				}
				else
				{
					stripLine.Interval = chartStripLine.Instance.Interval;
				}
			}
			if (chartStripLine.IntervalType != null)
			{
				if (!chartStripLine.IntervalType.IsExpression)
				{
					stripLine.IntervalType = this.GetDateTimeIntervalType(chartStripLine.IntervalType.Value);
				}
				else
				{
					stripLine.IntervalType = this.GetDateTimeIntervalType(chartStripLine.Instance.IntervalType);
				}
			}
			if (chartStripLine.IntervalOffset != null)
			{
				if (!chartStripLine.IntervalOffset.IsExpression)
				{
					stripLine.IntervalOffset = chartStripLine.IntervalOffset.Value;
				}
				else
				{
					stripLine.IntervalOffset = chartStripLine.Instance.IntervalOffset;
				}
			}
			if (chartStripLine.IntervalOffsetType != null)
			{
				if (!chartStripLine.IntervalOffsetType.IsExpression)
				{
					stripLine.IntervalOffsetType = this.GetDateTimeIntervalType(chartStripLine.IntervalOffsetType.Value);
				}
				else
				{
					stripLine.IntervalOffsetType = this.GetDateTimeIntervalType(chartStripLine.Instance.IntervalOffsetType);
				}
			}
			if (chartStripLine.StripWidth != null)
			{
				if (!chartStripLine.StripWidth.IsExpression)
				{
					stripLine.StripWidth = chartStripLine.StripWidth.Value;
				}
				else
				{
					stripLine.StripWidth = chartStripLine.Instance.StripWidth;
				}
			}
			if (chartStripLine.StripWidthType != null)
			{
				if (!chartStripLine.StripWidthType.IsExpression)
				{
					stripLine.StripWidthType = this.GetDateTimeIntervalType(chartStripLine.StripWidthType.Value);
				}
				else
				{
					stripLine.StripWidthType = this.GetDateTimeIntervalType(chartStripLine.Instance.StripWidthType);
				}
			}
			if (chartStripLine.Title != null)
			{
				if (!chartStripLine.Title.IsExpression)
				{
					if (chartStripLine.Title.Value != null)
					{
						stripLine.Title = chartStripLine.Title.Value;
					}
				}
				else if (chartStripLine.Instance.Title != null)
				{
					stripLine.Title = chartStripLine.Instance.Title;
				}
			}
			if (chartStripLine.ToolTip != null)
			{
				if (!chartStripLine.ToolTip.IsExpression)
				{
					if (chartStripLine.ToolTip.Value != null)
					{
						stripLine.ToolTip = chartStripLine.ToolTip.Value;
					}
				}
				else if (chartStripLine.Instance.ToolTip != null)
				{
					stripLine.ToolTip = chartStripLine.Instance.ToolTip;
				}
			}
			if (chartStripLine.TextOrientation != null)
			{
				if (!chartStripLine.TextOrientation.IsExpression)
				{
					stripLine.TextOrientation = this.GetTextOrientation(chartStripLine.TextOrientation.Value);
				}
				else
				{
					stripLine.TextOrientation = this.GetTextOrientation(chartStripLine.Instance.TextOrientation);
				}
			}
		}

		private void RenderStripLineStyle(ChartStripLine chartStripLine, StripLine stripLine)
		{
			stripLine.TitleAlignment = StringAlignment.Near;
			Style style = chartStripLine.Style;
			Border border = null;
			if (style != null)
			{
				StyleInstance style2 = chartStripLine.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(chartStripLine.Style.Color))
				{
					stripLine.TitleColor = MappingHelper.GetStyleColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartStripLine.Style.BackgroundColor))
				{
					stripLine.BackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartStripLine.Style.BackgroundGradientEndColor))
				{
					stripLine.BackGradientEndColor = MappingHelper.GetStyleBackGradientEndColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartStripLine.Style.BackgroundGradientType))
				{
					stripLine.BackGradientType = this.GetGradientType(MappingHelper.GetStyleBackGradientType(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(chartStripLine.Style.BackgroundHatchType))
				{
					stripLine.BackHatchStyle = this.GetHatchType(MappingHelper.GetStyleBackgroundHatchType(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(chartStripLine.Style.TextAlign))
				{
					stripLine.TitleAlignment = this.GetStringAlignmentFromTextAlignments(MappingHelper.GetStyleTextAlign(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(chartStripLine.Style.VerticalAlign))
				{
					stripLine.TitleLineAlignment = this.GetStringAlignmentFromVericalAlignments(MappingHelper.GetStyleVerticalAlignment(style, style2));
				}
				this.RenderStripLineBackgroundImage(chartStripLine.Style.BackgroundImage, stripLine);
				border = style.Border;
			}
			this.RenderStripLineBorder(border, stripLine);
			this.RenderStripLineFont(chartStripLine, stripLine);
		}

		private void RenderAxisTitle(ChartAxisTitle chartAxisTitle, AspNetCore.Reporting.Chart.WebForms.Axis axis)
		{
			if (chartAxisTitle != null)
			{
				this.SetAxisTitleProperties(chartAxisTitle, axis);
				this.RenderAxisTitleStyle(chartAxisTitle, axis);
			}
		}

		private void SetAxisTitleProperties(ChartAxisTitle chartAxisTitle, AspNetCore.Reporting.Chart.WebForms.Axis axis)
		{
			if (chartAxisTitle.Caption != null)
			{
				if (chartAxisTitle.Caption.Value != null)
				{
					axis.Title = chartAxisTitle.Caption.Value;
				}
				else if (chartAxisTitle.Instance.Caption != null)
				{
					axis.Title = chartAxisTitle.Instance.Caption;
				}
			}
			if (chartAxisTitle.Position != null)
			{
				if (!chartAxisTitle.Position.IsExpression)
				{
					axis.TitleAlignment = this.GetAlignment(chartAxisTitle.Position.Value);
				}
				else
				{
					axis.TitleAlignment = this.GetAlignment(chartAxisTitle.Instance.Position);
				}
			}
			if (chartAxisTitle.TextOrientation != null)
			{
				if (!chartAxisTitle.TextOrientation.IsExpression)
				{
					axis.TextOrientation = this.GetTextOrientation(chartAxisTitle.TextOrientation.Value);
				}
				else
				{
					axis.TextOrientation = this.GetTextOrientation(chartAxisTitle.Instance.TextOrientation);
				}
			}
		}

		private StringAlignment GetAlignment(ChartAxisTitlePositions position)
		{
			switch (position)
			{
			case ChartAxisTitlePositions.Center:
				return StringAlignment.Center;
			case ChartAxisTitlePositions.Far:
				return StringAlignment.Far;
			default:
				return StringAlignment.Near;
			}
		}

		private void RenderAxisTitleStyle(ChartAxisTitle axisTitle, AspNetCore.Reporting.Chart.WebForms.Axis axis)
		{
			Style style = axisTitle.Style;
			if (style != null)
			{
				StyleInstance style2 = axisTitle.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(axisTitle.Style.Color))
				{
					axis.TitleColor = MappingHelper.GetStyleColor(style, style2);
				}
			}
			this.RenderAxisTitleFont(axisTitle, axis);
		}

		private void RenderAxisTitleFont(ChartAxisTitle axisTitle, AspNetCore.Reporting.Chart.WebForms.Axis axis)
		{
			Style style = axisTitle.Style;
			if (style == null)
			{
				axis.TitleFont = base.GetDefaultFont();
			}
			else
			{
				axis.TitleFont = base.GetFont(style, axisTitle.Instance.Style);
			}
		}

		private void RenderAxisStyle(ChartAxis chartAxis, AspNetCore.Reporting.Chart.WebForms.Axis axis)
		{
			Style style = chartAxis.Style;
			if (style != null)
			{
				StyleInstance style2 = chartAxis.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(chartAxis.Style.Format))
				{
					axis.LabelStyle.Format = MappingHelper.GetStyleFormat(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartAxis.Style.Color))
				{
					axis.LabelStyle.FontColor = MappingHelper.GetStyleColor(style, style2);
				}
				this.RenderAxisBorder(chartAxis.Style.Border, axis);
			}
			this.RenderAxisLabelFont(chartAxis, axis);
		}

		private void RenderAxisBorder(Border border, AspNetCore.Reporting.Chart.WebForms.Axis axis)
		{
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					axis.LineColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					axis.LineStyle = this.GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), false);
				}
				axis.LineWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderAxisGridLines(ChartGridLines chartGridLines, Grid gridLines, bool isMajor)
		{
			if (chartGridLines != null)
			{
				this.SetAxisGridLinesProperties(chartGridLines, gridLines, isMajor);
				if (chartGridLines.Style != null)
				{
					this.RenderAxisGridLinesBorder(chartGridLines.Style.Border, gridLines);
				}
			}
		}

		private void RenderAxisGridLinesBorder(Border border, Grid gridLines)
		{
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					gridLines.LineColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					gridLines.LineStyle = this.GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), false);
				}
				gridLines.LineWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void SetAxisGridLinesProperties(ChartGridLines chartGridLines, Grid gridLines, bool isMajor)
		{
			if (chartGridLines.Enabled != null)
			{
				ChartAutoBool chartAutoBool = ChartAutoBool.Auto;
				switch (chartGridLines.Enabled.IsExpression ? chartGridLines.Instance.Enabled : chartGridLines.Enabled.Value)
				{
				case ChartAutoBool.Auto:
					gridLines.Enabled = ((byte)(isMajor ? 1 : 0) != 0);
					break;
				case ChartAutoBool.True:
					gridLines.Enabled = true;
					break;
				case ChartAutoBool.False:
					gridLines.Enabled = false;
					break;
				}
			}
			if (chartGridLines.Interval != null)
			{
				double num = chartGridLines.Interval.IsExpression ? chartGridLines.Instance.Interval : chartGridLines.Interval.Value;
				if (num == 0.0)
				{
					num = double.NaN;
				}
				gridLines.Interval = num;
			}
			else
			{
				gridLines.Interval = double.NaN;
			}
			if (chartGridLines.IntervalType != null)
			{
				if (!chartGridLines.IntervalType.IsExpression)
				{
					gridLines.IntervalType = this.GetDateTimeIntervalType(chartGridLines.IntervalType.Value);
				}
				else
				{
					gridLines.IntervalType = this.GetDateTimeIntervalType(chartGridLines.Instance.IntervalType);
				}
			}
			if (chartGridLines.IntervalOffset != null)
			{
				if (!chartGridLines.IntervalOffset.IsExpression)
				{
					gridLines.IntervalOffset = chartGridLines.IntervalOffset.Value;
				}
				else
				{
					gridLines.IntervalOffset = chartGridLines.Instance.IntervalOffset;
				}
			}
			if (chartGridLines.IntervalOffsetType != null)
			{
				if (!chartGridLines.IntervalOffsetType.IsExpression)
				{
					gridLines.IntervalOffsetType = this.GetDateTimeIntervalType(chartGridLines.IntervalOffsetType.Value);
				}
				else
				{
					gridLines.IntervalOffsetType = this.GetDateTimeIntervalType(chartGridLines.Instance.IntervalOffsetType);
				}
			}
		}

		private void RenderAxisTickMarks(ChartTickMarks chartTickMarks, TickMark tickMarks, bool isMajor)
		{
			if (chartTickMarks != null)
			{
				this.SetAxisTickMarkProperties(chartTickMarks, tickMarks, isMajor);
				this.RenderTickMarkStyle(chartTickMarks, tickMarks);
			}
		}

		private void SetAxisTickMarkProperties(ChartTickMarks chartTickMarks, TickMark tickMarks, bool isMajor)
		{
			if (chartTickMarks.Enabled != null)
			{
				if (!chartTickMarks.Enabled.IsExpression)
				{
					tickMarks.Enabled = this.GetChartTickMarksEnabled(chartTickMarks.Enabled.Value, isMajor);
				}
				else
				{
					tickMarks.Enabled = this.GetChartTickMarksEnabled(chartTickMarks.Instance.Enabled, isMajor);
				}
			}
			if (chartTickMarks.Interval != null)
			{
				double num = chartTickMarks.Interval.IsExpression ? chartTickMarks.Instance.Interval : chartTickMarks.Interval.Value;
				if (num == 0.0)
				{
					num = double.NaN;
				}
				tickMarks.Interval = num;
			}
			else
			{
				tickMarks.Interval = double.NaN;
			}
			if (chartTickMarks.IntervalOffset != null)
			{
				if (!chartTickMarks.IntervalOffset.IsExpression)
				{
					tickMarks.IntervalOffset = chartTickMarks.IntervalOffset.Value;
				}
				else
				{
					tickMarks.IntervalOffset = chartTickMarks.Instance.IntervalOffset;
				}
			}
			if (chartTickMarks.IntervalOffsetType != null)
			{
				if (!chartTickMarks.IntervalOffsetType.IsExpression)
				{
					tickMarks.IntervalOffsetType = this.GetDateTimeIntervalType(chartTickMarks.IntervalOffsetType.Value);
				}
				else
				{
					tickMarks.IntervalOffsetType = this.GetDateTimeIntervalType(chartTickMarks.Instance.IntervalOffsetType);
				}
			}
			if (chartTickMarks.IntervalType != null)
			{
				if (!chartTickMarks.IntervalType.IsExpression)
				{
					tickMarks.IntervalType = this.GetDateTimeIntervalType(chartTickMarks.IntervalType.Value);
				}
				else
				{
					tickMarks.IntervalType = this.GetDateTimeIntervalType(chartTickMarks.Instance.IntervalType);
				}
			}
			if (chartTickMarks.Length != null)
			{
				if (!chartTickMarks.Length.IsExpression)
				{
					tickMarks.Size = (float)chartTickMarks.Length.Value;
				}
				else
				{
					tickMarks.Size = (float)chartTickMarks.Instance.Length;
				}
			}
			if (chartTickMarks.Type != null)
			{
				if (!chartTickMarks.Type.IsExpression)
				{
					tickMarks.Style = this.GetTickMarkStyle(chartTickMarks.Type.Value);
				}
				else
				{
					tickMarks.Style = this.GetTickMarkStyle(chartTickMarks.Instance.Type);
				}
			}
		}

		private bool GetChartTickMarksEnabled(ChartAutoBool enabled, bool isMajor)
		{
			switch (enabled)
			{
			case ChartAutoBool.False:
				return false;
			case ChartAutoBool.True:
				return true;
			default:
				return isMajor;
			}
		}

		private void RenderTickMarkStyle(ChartTickMarks chartTickMarks, TickMark tickMarks)
		{
			if (chartTickMarks.Style != null)
			{
				this.RenderTickMarkBorder(chartTickMarks.Style.Border, tickMarks);
			}
		}

		private void RenderTickMarkBorder(Border border, TickMark tickMark)
		{
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					tickMark.LineColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					tickMark.LineStyle = this.GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), false);
				}
				tickMark.LineWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderAxisScaleBreak(ChartAxisScaleBreak chartAxisScaleBreak, AxisScaleBreakStyle axisScaleBreak)
		{
			if (chartAxisScaleBreak != null)
			{
				if (chartAxisScaleBreak.BreakLineType != null)
				{
					if (!chartAxisScaleBreak.BreakLineType.IsExpression)
					{
						axisScaleBreak.BreakLineType = this.GetScaleBreakLineType(chartAxisScaleBreak.BreakLineType.Value);
					}
					else
					{
						axisScaleBreak.BreakLineType = this.GetScaleBreakLineType(chartAxisScaleBreak.Instance.BreakLineType);
					}
				}
				if (chartAxisScaleBreak.CollapsibleSpaceThreshold != null)
				{
					if (!chartAxisScaleBreak.CollapsibleSpaceThreshold.IsExpression)
					{
						axisScaleBreak.CollapsibleSpaceThreshold = chartAxisScaleBreak.CollapsibleSpaceThreshold.Value;
					}
					else
					{
						axisScaleBreak.CollapsibleSpaceThreshold = chartAxisScaleBreak.Instance.CollapsibleSpaceThreshold;
					}
				}
				if (chartAxisScaleBreak.Enabled != null)
				{
					if (!chartAxisScaleBreak.Enabled.IsExpression)
					{
						axisScaleBreak.Enabled = chartAxisScaleBreak.Enabled.Value;
					}
					else
					{
						axisScaleBreak.Enabled = chartAxisScaleBreak.Instance.Enabled;
					}
				}
				if (chartAxisScaleBreak.IncludeZero != null)
				{
					if (!chartAxisScaleBreak.IncludeZero.IsExpression)
					{
						axisScaleBreak.StartFromZero = this.GetAutoBool(chartAxisScaleBreak.IncludeZero.Value);
					}
					else
					{
						axisScaleBreak.StartFromZero = this.GetAutoBool(chartAxisScaleBreak.Instance.IncludeZero);
					}
				}
				if (chartAxisScaleBreak.MaxNumberOfBreaks != null)
				{
					if (!chartAxisScaleBreak.MaxNumberOfBreaks.IsExpression)
					{
						axisScaleBreak.MaxNumberOfBreaks = chartAxisScaleBreak.MaxNumberOfBreaks.Value;
					}
					else
					{
						axisScaleBreak.MaxNumberOfBreaks = chartAxisScaleBreak.Instance.MaxNumberOfBreaks;
					}
				}
				if (chartAxisScaleBreak.Spacing != null)
				{
					if (!chartAxisScaleBreak.Spacing.IsExpression)
					{
						axisScaleBreak.Spacing = chartAxisScaleBreak.Spacing.Value;
					}
					else
					{
						axisScaleBreak.Spacing = chartAxisScaleBreak.Instance.Spacing;
					}
				}
				if (chartAxisScaleBreak.Style != null)
				{
					this.RenderAxisScaleBreakBorder(chartAxisScaleBreak.Style.Border, axisScaleBreak);
				}
			}
		}

		private void RenderCustomProperties(CustomPropertyCollection customProperties, AspNetCore.Reporting.Chart.WebForms.Axis axis)
		{
		}

		private void RenderLegends()
		{
			if (this.m_chart.Legends != null)
			{
				foreach (ChartLegend legend2 in this.m_chart.Legends)
				{
					AspNetCore.Reporting.Chart.WebForms.Legend legend = new AspNetCore.Reporting.Chart.WebForms.Legend();
					this.m_coreChart.Legends.Add(legend);
					this.RenderLegend(legend2, legend);
				}
			}
		}

		private void RenderLegend(ChartLegend chartLegend, AspNetCore.Reporting.Chart.WebForms.Legend legend)
		{
			if (chartLegend.Name != null)
			{
				legend.Name = chartLegend.Name;
			}
			this.RenderLegendStyle(chartLegend, legend);
			this.SetLegendProperties(chartLegend, legend);
			this.RenderElementPosition(chartLegend.ChartElementPosition, legend.Position);
			this.RenderLegendTitle(chartLegend.LegendTitle, legend);
			this.RenderLegendColumns(chartLegend.LegendColumns, legend.CellColumns);
			this.RenderLegendCustomItems(chartLegend.LegendCustomItems, legend.CustomItems);
		}

		private void SetLegendProperties(ChartLegend chartLegend, AspNetCore.Reporting.Chart.WebForms.Legend legend)
		{
			if (chartLegend.Hidden != null)
			{
				if (!chartLegend.Hidden.IsExpression)
				{
					legend.Enabled = !chartLegend.Hidden.Value;
				}
				else
				{
					legend.Enabled = !chartLegend.Instance.Hidden;
				}
			}
			if (chartLegend.DockOutsideChartArea != null)
			{
				if (!chartLegend.DockOutsideChartArea.IsExpression)
				{
					legend.DockInsideChartArea = !chartLegend.DockOutsideChartArea.Value;
				}
				else
				{
					legend.DockInsideChartArea = !chartLegend.Instance.DockOutsideChartArea;
				}
			}
			if (chartLegend.DockToChartArea != null)
			{
				legend.DockToChartArea = chartLegend.DockToChartArea;
			}
			if (chartLegend.Position != null)
			{
				if (!chartLegend.Position.IsExpression)
				{
					legend.Alignment = this.GetLegendAlignment(chartLegend.Position.Value);
					legend.Docking = this.GetLegendDocking(chartLegend.Position.Value);
				}
				else
				{
					legend.Alignment = this.GetLegendAlignment(chartLegend.Instance.Position);
					legend.Docking = this.GetLegendDocking(chartLegend.Instance.Position);
				}
			}
			if (chartLegend.Layout != null)
			{
				if (!chartLegend.Layout.IsExpression)
				{
					this.SetLegendLayout(chartLegend.Layout.Value, legend);
				}
				else
				{
					this.SetLegendLayout(chartLegend.Instance.Layout, legend);
				}
			}
			if (chartLegend.AutoFitTextDisabled != null)
			{
				if (!chartLegend.AutoFitTextDisabled.IsExpression)
				{
					legend.AutoFitText = !chartLegend.AutoFitTextDisabled.Value;
				}
				else
				{
					legend.AutoFitText = !chartLegend.Instance.AutoFitTextDisabled;
				}
			}
			else
			{
				legend.AutoFitText = true;
			}
			ReportBoolProperty equallySpacedItem = chartLegend.EquallySpacedItems;
			if (chartLegend.InterlacedRows != null)
			{
				if (!chartLegend.InterlacedRows.IsExpression)
				{
					legend.InterlacedRows = chartLegend.InterlacedRows.Value;
				}
				else
				{
					legend.InterlacedRows = chartLegend.Instance.InterlacedRows;
				}
			}
			if (chartLegend.InterlacedRowsColor != null)
			{
				Color empty = Color.Empty;
				if (MappingHelper.GetColorFromReportColorProperty(chartLegend.InterlacedRowsColor, ref empty))
				{
					legend.InterlacedRowsColor = empty;
				}
				else if (chartLegend.Instance.InterlacedRowsColor != null)
				{
					legend.InterlacedRowsColor = chartLegend.Instance.InterlacedRowsColor.ToColor();
				}
			}
			if (chartLegend.MaxAutoSize != null)
			{
				if (!chartLegend.MaxAutoSize.IsExpression)
				{
					legend.MaxAutoSize = (float)chartLegend.MaxAutoSize.Value;
				}
				else
				{
					legend.MaxAutoSize = (float)chartLegend.Instance.MaxAutoSize;
				}
			}
			if (chartLegend.MinFontSize != null)
			{
				if (!chartLegend.MinFontSize.IsExpression)
				{
					legend.AutoFitMinFontSize = (int)Math.Round(chartLegend.MinFontSize.Value.ToPoints());
				}
				else
				{
					legend.AutoFitMinFontSize = (int)Math.Round(chartLegend.Instance.MinFontSize.ToPoints());
				}
			}
			if (chartLegend.Reversed != null)
			{
				ChartAutoBool chartAutoBool = ChartAutoBool.Auto;
				switch (chartLegend.Reversed.IsExpression ? chartLegend.Instance.Reversed : chartLegend.Reversed.Value)
				{
				case ChartAutoBool.Auto:
					legend.Reversed = AutoBool.Auto;
					break;
				case ChartAutoBool.False:
					legend.Reversed = AutoBool.False;
					break;
				case ChartAutoBool.True:
					legend.Reversed = AutoBool.True;
					break;
				}
			}
			if (chartLegend.TextWrapThreshold != null)
			{
				if (!chartLegend.TextWrapThreshold.IsExpression)
				{
					legend.TextWrapThreshold = chartLegend.TextWrapThreshold.Value;
				}
				else
				{
					legend.TextWrapThreshold = chartLegend.Instance.TextWrapThreshold;
				}
			}
		}

		private void RenderLegendTitle(ChartLegendTitle chartLegendTitle, AspNetCore.Reporting.Chart.WebForms.Legend legend)
		{
			if (chartLegendTitle != null)
			{
				if (chartLegendTitle.Caption != null)
				{
					if (!chartLegendTitle.Caption.IsExpression)
					{
						if (chartLegendTitle.Caption.Value != null)
						{
							legend.Title = chartLegendTitle.Caption.Value;
						}
					}
					else if (chartLegendTitle.Instance.Caption != null)
					{
						legend.Title = chartLegendTitle.Instance.Caption;
					}
				}
				if (chartLegendTitle.TitleSeparator != null)
				{
					if (!chartLegendTitle.TitleSeparator.IsExpression)
					{
						legend.TitleSeparator = this.GetLegendSeparatorStyle(chartLegendTitle.TitleSeparator.Value);
					}
					else
					{
						legend.TitleSeparator = this.GetLegendSeparatorStyle(chartLegendTitle.Instance.TitleSeparator);
					}
				}
				this.RenderLegendTitleStyle(chartLegendTitle, legend);
			}
		}

		private LegendSeparatorType GetLegendSeparatorStyle(ChartSeparators chartLegendSeparator)
		{
			switch (chartLegendSeparator)
			{
			case ChartSeparators.DashLine:
				return LegendSeparatorType.DashLine;
			case ChartSeparators.DotLine:
				return LegendSeparatorType.DotLine;
			case ChartSeparators.DoubleLine:
				return LegendSeparatorType.DoubleLine;
			case ChartSeparators.GradientLine:
				return LegendSeparatorType.GradientLine;
			case ChartSeparators.Line:
				return LegendSeparatorType.Line;
			case ChartSeparators.ThickGradientLine:
				return LegendSeparatorType.ThickGradientLine;
			case ChartSeparators.ThickLine:
				return LegendSeparatorType.ThickLine;
			default:
				return LegendSeparatorType.None;
			}
		}

		private void RenderLegendTitleStyle(ChartLegendTitle chartLegendTitle, AspNetCore.Reporting.Chart.WebForms.Legend legend)
		{
			Style style = chartLegendTitle.Style;
			if (style != null)
			{
				StyleInstance style2 = chartLegendTitle.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(style.Color))
				{
					legend.TitleColor = MappingHelper.GetStyleColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(style.BackgroundColor))
				{
					legend.TitleBackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				}
				this.RenderLegendTitleBorder(style.Border, legend);
				legend.TitleAlignment = this.GetLegendTitleAlign(MappingHelper.GetStyleTextAlign(style, style2));
			}
			this.RenderLegendTitleFont(chartLegendTitle, legend);
		}

		private void RenderLegendTitleBorder(Border border, AspNetCore.Reporting.Chart.WebForms.Legend legend)
		{
			if (border != null && MappingHelper.IsStylePropertyDefined(border.Color))
			{
				legend.TitleSeparatorColor = MappingHelper.GetStyleBorderColor(border);
			}
		}

		private void SetLegendLayout(ChartLegendLayouts layout, AspNetCore.Reporting.Chart.WebForms.Legend legend)
		{
			switch (layout)
			{
			case ChartLegendLayouts.Row:
				legend.LegendStyle = LegendStyle.Row;
				break;
			case ChartLegendLayouts.Column:
				legend.LegendStyle = LegendStyle.Column;
				break;
			case ChartLegendLayouts.AutoTable:
				legend.LegendStyle = LegendStyle.Table;
				legend.TableStyle = LegendTableStyle.Auto;
				break;
			case ChartLegendLayouts.TallTable:
				legend.LegendStyle = LegendStyle.Table;
				legend.TableStyle = LegendTableStyle.Tall;
				break;
			case ChartLegendLayouts.WideTable:
				legend.LegendStyle = LegendStyle.Table;
				legend.TableStyle = LegendTableStyle.Wide;
				break;
			}
		}

		private void GetChartTitlePosition(ChartTitlePositions position, out ContentAlignment alignment, out Docking docking)
		{
			docking = Docking.Top;
			alignment = ContentAlignment.MiddleLeft;
			switch (position)
			{
			case ChartTitlePositions.TopLeft:
				docking = Docking.Top;
				alignment = ContentAlignment.MiddleLeft;
				break;
			case ChartTitlePositions.TopCenter:
				docking = Docking.Top;
				alignment = ContentAlignment.MiddleCenter;
				break;
			case ChartTitlePositions.TopRight:
				docking = Docking.Top;
				alignment = ContentAlignment.MiddleRight;
				break;
			case ChartTitlePositions.LeftTop:
				docking = Docking.Left;
				alignment = ContentAlignment.MiddleRight;
				break;
			case ChartTitlePositions.LeftCenter:
				docking = Docking.Left;
				alignment = ContentAlignment.MiddleCenter;
				break;
			case ChartTitlePositions.LeftBottom:
				docking = Docking.Left;
				alignment = ContentAlignment.MiddleLeft;
				break;
			case ChartTitlePositions.RightTop:
				docking = Docking.Right;
				alignment = ContentAlignment.MiddleLeft;
				break;
			case ChartTitlePositions.RightCenter:
				docking = Docking.Right;
				alignment = ContentAlignment.MiddleCenter;
				break;
			case ChartTitlePositions.RightBottom:
				docking = Docking.Right;
				alignment = ContentAlignment.MiddleRight;
				break;
			case ChartTitlePositions.BottomRight:
				docking = Docking.Bottom;
				alignment = ContentAlignment.MiddleRight;
				break;
			case ChartTitlePositions.BottomCenter:
				docking = Docking.Bottom;
				alignment = ContentAlignment.MiddleCenter;
				break;
			case ChartTitlePositions.BottomLeft:
				docking = Docking.Bottom;
				alignment = ContentAlignment.MiddleLeft;
				break;
			}
		}

		private TextOrientation GetTextOrientation(TextOrientations textOrientations)
		{
			switch (textOrientations)
			{
			case TextOrientations.Horizontal:
				return TextOrientation.Horizontal;
			case TextOrientations.Rotated270:
				return TextOrientation.Rotated270;
			case TextOrientations.Rotated90:
				return TextOrientation.Rotated90;
			case TextOrientations.Stacked:
				return TextOrientation.Stacked;
			default:
				return TextOrientation.Auto;
			}
		}

		private StringAlignment GetLegendAlignment(ChartLegendPositions position)
		{
			switch (position)
			{
			case ChartLegendPositions.TopCenter:
			case ChartLegendPositions.LeftCenter:
			case ChartLegendPositions.RightCenter:
			case ChartLegendPositions.BottomCenter:
				return StringAlignment.Center;
			case ChartLegendPositions.TopRight:
			case ChartLegendPositions.LeftBottom:
			case ChartLegendPositions.RightBottom:
			case ChartLegendPositions.BottomRight:
				return StringAlignment.Far;
			default:
				return StringAlignment.Near;
			}
		}

		private StringAlignment GetLegendTitleAlign(TextAlignments textAlignment)
		{
			switch (textAlignment)
			{
			case TextAlignments.Left:
				return StringAlignment.Near;
			case TextAlignments.Right:
				return StringAlignment.Far;
			default:
				return StringAlignment.Center;
			}
		}

		private LegendDocking GetLegendDocking(ChartLegendPositions position)
		{
			switch (position)
			{
			case ChartLegendPositions.BottomLeft:
			case ChartLegendPositions.BottomCenter:
			case ChartLegendPositions.BottomRight:
				return LegendDocking.Bottom;
			case ChartLegendPositions.TopLeft:
			case ChartLegendPositions.TopCenter:
			case ChartLegendPositions.TopRight:
				return LegendDocking.Top;
			case ChartLegendPositions.LeftTop:
			case ChartLegendPositions.LeftCenter:
			case ChartLegendPositions.LeftBottom:
				return LegendDocking.Left;
			default:
				return LegendDocking.Right;
			}
		}

		private void RenderLegendStyle(ChartLegend chartLegend, AspNetCore.Reporting.Chart.WebForms.Legend legend)
		{
			Border border = null;
			Style style = chartLegend.Style;
			if (style != null)
			{
				StyleInstance style2 = chartLegend.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(chartLegend.Style.Color))
				{
					legend.FontColor = MappingHelper.GetStyleColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartLegend.Style.BackgroundColor))
				{
					legend.BackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartLegend.Style.BackgroundGradientEndColor))
				{
					legend.BackGradientEndColor = MappingHelper.GetStyleBackGradientEndColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartLegend.Style.BackgroundGradientType))
				{
					legend.BackGradientType = this.GetGradientType(MappingHelper.GetStyleBackGradientType(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(chartLegend.Style.BackgroundHatchType))
				{
					legend.BackHatchStyle = this.GetHatchType(MappingHelper.GetStyleBackgroundHatchType(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(chartLegend.Style.ShadowColor))
				{
					legend.ShadowColor = MappingHelper.GetStyleShadowColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartLegend.Style.ShadowOffset))
				{
					legend.ShadowOffset = MappingHelper.GetStyleShadowOffset(style, style2, base.DpiX);
				}
				this.RenderLegendBackgroundImage(chartLegend.Style.BackgroundImage, legend);
				border = chartLegend.Style.Border;
			}
			this.RenderLegendBorder(border, legend);
			this.RenderLegendFont(chartLegend, legend);
		}

		private void RenderLegendColumns(ChartLegendColumnCollection chartLegendColumns, LegendCellColumnCollection legendColumns)
		{
		}

		private void RenderLegendCustomItems(ChartLegendCustomItemCollection chartLegendCustomItems, LegendItemsCollection legendCustomItems)
		{
		}

		private void RenderElementPosition(ChartElementPosition chartElementPosition, ElementPosition elementPosition)
		{
			if (chartElementPosition != null)
			{
				ReportDoubleProperty left = chartElementPosition.Left;
				if (left != null)
				{
					if (!left.IsExpression)
					{
						elementPosition.X = (float)left.Value;
					}
					else
					{
						elementPosition.X = (float)chartElementPosition.Instance.Left;
					}
				}
				else
				{
					elementPosition.X = 0f;
				}
				left = chartElementPosition.Top;
				if (left != null)
				{
					if (!left.IsExpression)
					{
						elementPosition.Y = (float)left.Value;
					}
					else
					{
						elementPosition.Y = (float)chartElementPosition.Instance.Top;
					}
				}
				else
				{
					elementPosition.Y = 0f;
				}
				left = chartElementPosition.Width;
				if (left != null)
				{
					if (!left.IsExpression)
					{
						elementPosition.Width = (float)left.Value;
					}
					else
					{
						elementPosition.Width = (float)chartElementPosition.Instance.Width;
					}
				}
				else
				{
					elementPosition.Width = (float)(100.0 - elementPosition.X);
				}
				left = chartElementPosition.Height;
				if (left != null)
				{
					if (!left.IsExpression)
					{
						elementPosition.Height = (float)left.Value;
					}
					else
					{
						elementPosition.Height = (float)chartElementPosition.Instance.Height;
					}
				}
				else
				{
					elementPosition.Height = (float)(100.0 - elementPosition.Y);
				}
			}
		}

		private void RenderTitles()
		{
			if (this.m_chart.Titles != null)
			{
				foreach (ChartTitle title2 in this.m_chart.Titles)
				{
					Title title = new Title();
					this.m_coreChart.Titles.Add(title);
					this.RenderTitle(title2, title);
				}
			}
		}

		private void RenderTitle(ChartTitle chartTitle, Title title)
		{
			this.SetTitleProperties(chartTitle, title);
			this.RenderElementPosition(chartTitle.ChartElementPosition, title.Position);
			this.RenderActionInfo(chartTitle.ActionInfo, title.ToolTip, title);
			this.RenderTitleStyle(chartTitle, title);
		}

		private void SetTitleProperties(ChartTitle chartTitle, Title title)
		{
			if (chartTitle.Name != null)
			{
				title.Name = chartTitle.Name;
			}
			if (chartTitle.Caption != null)
			{
				if (!chartTitle.Caption.IsExpression)
				{
					if (chartTitle.Caption.Value != null)
					{
						title.Text = chartTitle.Caption.Value;
					}
				}
				else if (chartTitle.Instance.Caption != null)
				{
					title.Text = chartTitle.Instance.Caption;
				}
			}
			if (chartTitle.Position != null)
			{
				ChartTitlePositions chartTitlePositions = ChartTitlePositions.TopCenter;
				chartTitlePositions = (chartTitle.Position.IsExpression ? chartTitle.Instance.Position : chartTitle.Position.Value);
				ContentAlignment alignment = default(ContentAlignment);
				Docking docking = default(Docking);
				this.GetChartTitlePosition(chartTitlePositions, out alignment, out docking);
				title.Docking = docking;
				title.Alignment = alignment;
			}
			if (chartTitle.DockOffset != null)
			{
				if (!chartTitle.DockOffset.IsExpression)
				{
					title.DockOffset = chartTitle.DockOffset.Value;
				}
				else
				{
					title.DockOffset = chartTitle.Instance.DockOffset;
				}
			}
			if (chartTitle.DockOutsideChartArea != null)
			{
				if (!chartTitle.DockOutsideChartArea.IsExpression)
				{
					title.DockInsideChartArea = !chartTitle.DockOutsideChartArea.Value;
				}
				else
				{
					title.DockInsideChartArea = !chartTitle.Instance.DockOutsideChartArea;
				}
			}
			if (chartTitle.DockToChartArea != null)
			{
				title.DockToChartArea = chartTitle.DockToChartArea;
			}
			if (chartTitle.Hidden != null)
			{
				if (!chartTitle.Hidden.IsExpression)
				{
					title.Visible = !chartTitle.Hidden.Value;
				}
				else
				{
					title.Visible = !chartTitle.Instance.Hidden;
				}
			}
			ReportStringProperty toolTip = chartTitle.ToolTip;
			if (toolTip != null)
			{
				if (!toolTip.IsExpression)
				{
					if (toolTip.Value != null)
					{
						title.ToolTip = toolTip.Value;
					}
				}
				else
				{
					string toolTip2 = chartTitle.Instance.ToolTip;
					if (toolTip2 != null)
					{
						title.ToolTip = chartTitle.Instance.ToolTip;
					}
				}
			}
			if (chartTitle.TextOrientation != null)
			{
				if (!chartTitle.TextOrientation.IsExpression)
				{
					title.TextOrientation = this.GetTextOrientation(chartTitle.TextOrientation.Value);
				}
				else
				{
					title.TextOrientation = this.GetTextOrientation(chartTitle.Instance.TextOrientation);
				}
			}
		}

		private void RenderTitleStyle(ChartTitle chartTitle, Title title)
		{
			Border border = null;
			Style style = chartTitle.Style;
			if (style != null)
			{
				StyleInstance style2 = chartTitle.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(chartTitle.Style.Color))
				{
					title.Color = MappingHelper.GetStyleColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartTitle.Style.BackgroundColor))
				{
					title.BackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				}
				else
				{
					title.BackColor = Color.Transparent;
				}
				if (MappingHelper.IsStylePropertyDefined(chartTitle.Style.BackgroundGradientEndColor))
				{
					title.BackGradientEndColor = MappingHelper.GetStyleBackGradientEndColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartTitle.Style.BackgroundGradientType))
				{
					title.BackGradientType = this.GetGradientType(MappingHelper.GetStyleBackGradientType(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(chartTitle.Style.BackgroundHatchType))
				{
					title.BackHatchStyle = this.GetHatchType(MappingHelper.GetStyleBackgroundHatchType(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(chartTitle.Style.ShadowColor))
				{
					title.ShadowColor = MappingHelper.GetStyleShadowColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartTitle.Style.ShadowOffset))
				{
					title.ShadowOffset = MappingHelper.GetStyleShadowOffset(style, style2, base.DpiX);
				}
				if (MappingHelper.IsStylePropertyDefined(chartTitle.Style.TextEffect))
				{
					title.Style = this.GetTextStyle(MappingHelper.GetStyleTextEffect(style, style2));
				}
				this.RenderTitleBackgroundImage(chartTitle.Style.BackgroundImage, title);
				border = chartTitle.Style.Border;
			}
			this.RenderTitleBorder(border, title);
			this.RenderTitleFont(chartTitle, title);
		}

		private TextStyle GetTextStyle(TextEffects textEffects)
		{
			switch (textEffects)
			{
			case TextEffects.Embed:
				return TextStyle.Embed;
			case TextEffects.Emboss:
				return TextStyle.Emboss;
			case TextEffects.Frame:
				return TextStyle.Frame;
			case TextEffects.Shadow:
				return TextStyle.Shadow;
			default:
				return TextStyle.Default;
			}
		}

		private void RenderDataLabel(ChartDataLabel chartDataLabel, DataPointAttributes dataPointAttributes, bool isDataPoint)
		{
			if (chartDataLabel != null)
			{
				this.SetDataLabelProperties(chartDataLabel, dataPointAttributes, isDataPoint);
				this.RenderDataLabelStyle(chartDataLabel, dataPointAttributes, isDataPoint);
				this.RenderDataLabelActionInfo(chartDataLabel.ActionInfo, dataPointAttributes.LabelToolTip, dataPointAttributes);
			}
		}

		private void SetDataLabelProperties(ChartDataLabel chartDataLabel, DataPointAttributes dataPointAttributes, bool isDataPoint)
		{
			if (chartDataLabel != null)
			{
				if (chartDataLabel.Position != null)
				{
					ChartDataLabelPositions dataLabelPositionValue = this.GetDataLabelPositionValue(chartDataLabel);
					dataPointAttributes.SetAttribute("LabelStyle", this.GetDataLabelPosition(dataLabelPositionValue));
				}
				if (chartDataLabel.Rotation != null)
				{
					if (!chartDataLabel.Rotation.IsExpression)
					{
						dataPointAttributes.FontAngle = chartDataLabel.Rotation.Value;
					}
					else
					{
						dataPointAttributes.FontAngle = chartDataLabel.Instance.Rotation;
					}
				}
				if (chartDataLabel.UseValueAsLabel != null)
				{
					if (!chartDataLabel.UseValueAsLabel.IsExpression)
					{
						dataPointAttributes.ShowLabelAsValue = chartDataLabel.UseValueAsLabel.Value;
					}
					else
					{
						dataPointAttributes.ShowLabelAsValue = chartDataLabel.Instance.UseValueAsLabel;
					}
				}
				if (!dataPointAttributes.ShowLabelAsValue && chartDataLabel.Label != null)
				{
					if (!chartDataLabel.Label.IsExpression)
					{
						if (chartDataLabel.Label != null)
						{
							dataPointAttributes.Label = chartDataLabel.Label.Value;
						}
					}
					else if (chartDataLabel.Instance.Label != null)
					{
						dataPointAttributes.Label = chartDataLabel.Instance.Label;
					}
				}
				if (chartDataLabel.Visible != null)
				{
					bool flag = false;
					if (!(chartDataLabel.Visible.IsExpression ? chartDataLabel.Instance.Visible : chartDataLabel.Visible.Value))
					{
						if (isDataPoint)
						{
							dataPointAttributes.DeleteAttribute(CommonAttributes.Label);
							dataPointAttributes.DeleteAttribute(CommonAttributes.ShowLabelAsValue);
							ChartMapper.HideDataPointLabels(dataPointAttributes);
						}
						else
						{
							dataPointAttributes.Label = "";
							dataPointAttributes.ShowLabelAsValue = false;
						}
					}
				}
				else if (isDataPoint)
				{
					dataPointAttributes.DeleteAttribute(CommonAttributes.Label);
					dataPointAttributes.DeleteAttribute(CommonAttributes.ShowLabelAsValue);
					ChartMapper.HideDataPointLabels(dataPointAttributes);
				}
				else
				{
					dataPointAttributes.Label = "";
					dataPointAttributes.ShowLabelAsValue = false;
				}
				ReportStringProperty toolTip = chartDataLabel.ToolTip;
				if (toolTip != null)
				{
					if (!toolTip.IsExpression)
					{
						if (toolTip.Value != null)
						{
							dataPointAttributes.LabelToolTip = toolTip.Value;
						}
					}
					else
					{
						string toolTip2 = chartDataLabel.Instance.ToolTip;
						if (toolTip2 != null)
						{
							dataPointAttributes.LabelToolTip = toolTip2;
						}
					}
				}
			}
		}

		private static void HideDataPointLabels(DataPointAttributes dataPointAttributes)
		{
			dataPointAttributes.SetAttribute("LabelsVisible", "false");
		}

		private ChartDataLabelPositions GetDataLabelPositionValue(ChartDataLabel chartDataLabel)
		{
			ChartDataLabelPositions chartDataLabelPositions = ChartDataLabelPositions.Auto;
			if (!chartDataLabel.Position.IsExpression)
			{
				return chartDataLabel.Position.Value;
			}
			return chartDataLabel.Instance.Position;
		}

		private string GetDataLabelPosition(ChartDataLabelPositions position)
		{
			switch (position)
			{
			case ChartDataLabelPositions.Bottom:
				return "Bottom";
			case ChartDataLabelPositions.BottomLeft:
				return "BottomLeft";
			case ChartDataLabelPositions.BottomRight:
				return "BottomRight";
			case ChartDataLabelPositions.Center:
				return "Center";
			case ChartDataLabelPositions.Left:
				return "Left";
			case ChartDataLabelPositions.Outside:
				return "Outside";
			case ChartDataLabelPositions.Right:
				return "Right";
			case ChartDataLabelPositions.Top:
				return "Top";
			case ChartDataLabelPositions.TopLeft:
				return "TopLeft";
			case ChartDataLabelPositions.TopRight:
				return "TopRight";
			default:
				return "Auto";
			}
		}

		private void RenderDataLabelStyle(ChartDataLabel chartDataLabel, DataPointAttributes dataPointAttributes, bool isDataPoint)
		{
			Border border = null;
			Style style = chartDataLabel.Style;
			if (style != null)
			{
				StyleInstance style2 = chartDataLabel.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(chartDataLabel.Style.BackgroundColor))
				{
					dataPointAttributes.LabelBackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartDataLabel.Style.Color))
				{
					dataPointAttributes.FontColor = MappingHelper.GetStyleColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartDataLabel.Style.Format))
				{
					dataPointAttributes.LabelFormat = MappingHelper.GetStyleFormat(style, style2);
				}
				border = chartDataLabel.Style.Border;
			}
			this.RenderDataLabelBorder(border, dataPointAttributes);
			this.RenderDataLabelFont(chartDataLabel, dataPointAttributes, isDataPoint);
		}

		private void RenderDataLabelBorder(Border border, DataPointAttributes dataPointAttributes)
		{
			dataPointAttributes.LabelBorderColor = Color.Black;
			dataPointAttributes.LabelBorderStyle = ChartDashStyle.NotSet;
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					dataPointAttributes.LabelBorderColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					dataPointAttributes.LabelBorderStyle = this.GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), false);
				}
				dataPointAttributes.LabelBorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderDataLabelActionInfo(ActionInfo actionInfo, string toolTip, DataPointAttributes dataPointAttributes)
		{
			if (actionInfo == null && string.IsNullOrEmpty(toolTip))
			{
				return;
			}
			string text = default(string);
			ActionInfoWithDynamicImageMap actionInfoWithDynamicImageMap = MappingHelper.CreateActionInfoDynamic((ReportItem)this.m_chart, actionInfo, toolTip, out text);
			if (actionInfoWithDynamicImageMap != null)
			{
				if (text != null)
				{
					dataPointAttributes.LabelHref = text;
				}
				int count = this.m_actions.Count;
				this.m_actions.InternalList.Add(actionInfoWithDynamicImageMap);
				dataPointAttributes.LabelTag = count;
			}
		}

		private void RenderDataPointMarker(ChartMarker chartMarker, DataPoint dataPoint, BackgroundImageInfo backgroundImageInfo)
		{
			if (chartMarker != null)
			{
				this.SetMarkerProperties(chartMarker, dataPoint);
				this.RenderDataPointMarkerStyle(chartMarker, dataPoint, backgroundImageInfo);
			}
		}

		private void RenderSeriesMarker(ChartMarker chartMarker, Series series)
		{
			if (chartMarker != null)
			{
				this.SetMarkerProperties(chartMarker, series);
				this.RenderSeriesMarkerStyle(chartMarker, series);
			}
		}

		private void RenderEmptyPointMarker(ChartMarker chartMarker, DataPointAttributes dataPointAttributes)
		{
			if (chartMarker != null)
			{
				this.SetMarkerProperties(chartMarker, dataPointAttributes);
				this.RenderEmptyPointMarkerStyle(chartMarker, dataPointAttributes);
			}
		}

		private void SetMarkerProperties(ChartMarker chartMarker, DataPointAttributes dataPointAttributes)
		{
			if (chartMarker.Size != null)
			{
				if (!chartMarker.Size.IsExpression)
				{
					if (chartMarker.Size.Value != null)
					{
						dataPointAttributes.MarkerSize = MappingHelper.ToIntPixels(chartMarker.Size.Value, base.DpiX);
					}
				}
				else if (chartMarker.Instance.Size != null)
				{
					dataPointAttributes.MarkerSize = MappingHelper.ToIntPixels(chartMarker.Instance.Size, base.DpiX);
				}
			}
			else
			{
				dataPointAttributes.MarkerSize = MappingHelper.ToIntPixels(ChartMapper.m_defaultMarkerSize, base.DpiX);
			}
			if (chartMarker.Type != null)
			{
				if (!chartMarker.Type.IsExpression)
				{
					dataPointAttributes.MarkerStyle = this.GetMarkerStyle(chartMarker.Type.Value);
				}
				else
				{
					dataPointAttributes.MarkerStyle = this.GetMarkerStyle(chartMarker.Instance.Type);
				}
			}
		}

		private MarkerStyle GetMarkerStyle(ChartMarkerTypes chartMarkerType)
		{
			switch (chartMarkerType)
			{
			case ChartMarkerTypes.Auto:
				if (this.m_autoMarker == null)
				{
					this.m_autoMarker = new AutoMarker();
				}
				return this.m_autoMarker.Current;
			case ChartMarkerTypes.Circle:
				return MarkerStyle.Circle;
			case ChartMarkerTypes.Cross:
				return MarkerStyle.Cross;
			case ChartMarkerTypes.Diamond:
				return MarkerStyle.Diamond;
			case ChartMarkerTypes.Square:
				return MarkerStyle.Square;
			case ChartMarkerTypes.Star10:
				return MarkerStyle.Star10;
			case ChartMarkerTypes.Star4:
				return MarkerStyle.Star4;
			case ChartMarkerTypes.Star5:
				return MarkerStyle.Star5;
			case ChartMarkerTypes.Star6:
				return MarkerStyle.Star6;
			case ChartMarkerTypes.Triangle:
				return MarkerStyle.Triangle;
			default:
				return MarkerStyle.None;
			}
		}

		private void RenderDataPointMarkerStyle(ChartMarker chartMarker, DataPoint dataPoint, BackgroundImageInfo backgroundImageInfo)
		{
			this.RenderMarkerStyle(chartMarker, dataPoint);
			if (chartMarker.Style != null)
			{
				this.RenderMarkerBackgroundImage(chartMarker.Style.BackgroundImage, dataPoint, backgroundImageInfo);
			}
		}

		private void RenderSeriesMarkerStyle(ChartMarker chartMarker, Series series)
		{
			this.RenderMarkerStyle(chartMarker, series);
			if (chartMarker.Style != null)
			{
				this.RenderMarkerBackgroundImage(chartMarker.Style.BackgroundImage, series, null);
			}
		}

		private void RenderEmptyPointMarkerStyle(ChartMarker chartMarker, DataPointAttributes emptyPoint)
		{
			this.RenderMarkerStyle(chartMarker, emptyPoint);
			if (chartMarker.Style != null)
			{
				this.RenderMarkerBackgroundImage(chartMarker.Style.BackgroundImage, emptyPoint, null);
			}
		}

		private void RenderMarkerStyle(ChartMarker chartMarker, DataPointAttributes dataPointAttributes)
		{
			Style style = chartMarker.Style;
			if (style != null)
			{
				StyleInstance style2 = chartMarker.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(chartMarker.Style.Color))
				{
					dataPointAttributes.MarkerColor = MappingHelper.GetStyleColor(style, style2);
				}
				this.RenderMarkerBorder(chartMarker.Style.Border, dataPointAttributes);
			}
		}

		private void RenderMarkerBorder(Border border, DataPointAttributes dataPointAttributes)
		{
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					dataPointAttributes.MarkerBorderColor = MappingHelper.GetStyleBorderColor(border);
				}
				dataPointAttributes.MarkerBorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderMarkerBackgroundImage(BackgroundImage backgroundImage, DataPointAttributes dataPointAttributes, BackgroundImageInfo backgroundImageInfo)
		{
			dataPointAttributes.MarkerImage = this.GetImageName(backgroundImage, backgroundImageInfo);
			ChartImageWrapMode chartImageWrapMode = default(ChartImageWrapMode);
			ChartImageAlign chartImageAlign = default(ChartImageAlign);
			Color markerImageTransparentColor = default(Color);
			this.GetBackgroundImageProperties(backgroundImage, out chartImageWrapMode, out chartImageAlign, out markerImageTransparentColor);
			dataPointAttributes.MarkerImageTransparentColor = markerImageTransparentColor;
		}

		private void RenderSmartLabels(ChartSmartLabel chartSmartLabels, SmartLabelsStyle smartLabels)
		{
			smartLabels.Enabled = true;
			smartLabels.CalloutLineWidth = MappingHelper.ToIntPixels(ChartMapper.m_defaultCalloutLineWidth, base.DpiX);
			smartLabels.MaxMovingDistance = MappingHelper.ToPixels(ChartMapper.m_defaultMaxMovingDistance, base.DpiX);
			if (chartSmartLabels != null)
			{
				this.SetSmartLabelsProperties(chartSmartLabels, smartLabels);
				this.RenderNoMoveDirections(chartSmartLabels.NoMoveDirections, smartLabels);
			}
		}

		private void SetSmartLabelsProperties(ChartSmartLabel chartSmartLabels, SmartLabelsStyle smartLabels)
		{
			if (chartSmartLabels.Disabled != null)
			{
				if (!chartSmartLabels.Disabled.IsExpression)
				{
					smartLabels.Enabled = !chartSmartLabels.Disabled.Value;
				}
				else
				{
					smartLabels.Enabled = !chartSmartLabels.Instance.Disabled;
				}
			}
			if (chartSmartLabels.AllowOutSidePlotArea != null)
			{
				if (!chartSmartLabels.AllowOutSidePlotArea.IsExpression)
				{
					smartLabels.AllowOutsidePlotArea = this.GetLabelOutsidePlotAreaStyle(chartSmartLabels.AllowOutSidePlotArea.Value);
				}
				else
				{
					smartLabels.AllowOutsidePlotArea = this.GetLabelOutsidePlotAreaStyle(chartSmartLabels.Instance.AllowOutSidePlotArea);
				}
			}
			Color empty = Color.Empty;
			if (chartSmartLabels.CalloutBackColor != null)
			{
				if (MappingHelper.GetColorFromReportColorProperty(chartSmartLabels.CalloutBackColor, ref empty))
				{
					smartLabels.CalloutBackColor = empty;
				}
				else if (chartSmartLabels.Instance.CalloutBackColor != null)
				{
					smartLabels.CalloutBackColor = chartSmartLabels.Instance.CalloutBackColor.ToColor();
				}
			}
			if (chartSmartLabels.CalloutLineAnchor != null)
			{
				if (!chartSmartLabels.CalloutLineAnchor.IsExpression)
				{
					smartLabels.CalloutLineAnchorCap = this.GetCalloutLineAnchor(chartSmartLabels.CalloutLineAnchor.Value);
				}
				else
				{
					smartLabels.CalloutLineAnchorCap = this.GetCalloutLineAnchor(chartSmartLabels.Instance.CalloutLineAnchor);
				}
			}
			if (chartSmartLabels.CalloutLineColor != null)
			{
				if (MappingHelper.GetColorFromReportColorProperty(chartSmartLabels.CalloutLineColor, ref empty))
				{
					smartLabels.CalloutLineColor = empty;
				}
				else if (chartSmartLabels.Instance.CalloutLineColor != null)
				{
					smartLabels.CalloutLineColor = chartSmartLabels.Instance.CalloutLineColor.ToColor();
				}
			}
			if (chartSmartLabels.CalloutLineStyle != null)
			{
				if (!chartSmartLabels.CalloutLineStyle.IsExpression)
				{
					smartLabels.CalloutLineStyle = this.GetCalloutLineStyle(chartSmartLabels.CalloutLineStyle.Value);
				}
				else
				{
					smartLabels.CalloutLineStyle = this.GetCalloutLineStyle(chartSmartLabels.Instance.CalloutLineStyle);
				}
			}
			if (chartSmartLabels.CalloutLineWidth != null)
			{
				if (!chartSmartLabels.CalloutLineWidth.IsExpression)
				{
					if (chartSmartLabels.CalloutLineWidth.Value != null)
					{
						smartLabels.CalloutLineWidth = MappingHelper.ToIntPixels(chartSmartLabels.CalloutLineWidth.Value, base.DpiX);
					}
				}
				else if (chartSmartLabels.Instance.CalloutLineWidth != null)
				{
					smartLabels.CalloutLineWidth = MappingHelper.ToIntPixels(chartSmartLabels.Instance.CalloutLineWidth, base.DpiX);
				}
			}
			if (chartSmartLabels.CalloutStyle != null)
			{
				if (!chartSmartLabels.CalloutStyle.IsExpression)
				{
					smartLabels.CalloutStyle = this.GetCalloutStyle(chartSmartLabels.CalloutStyle.Value);
				}
				else
				{
					smartLabels.CalloutStyle = this.GetCalloutStyle(chartSmartLabels.Instance.CalloutStyle);
				}
			}
			if (chartSmartLabels.ShowOverlapped != null)
			{
				if (!chartSmartLabels.ShowOverlapped.IsExpression)
				{
					smartLabels.HideOverlapped = !chartSmartLabels.ShowOverlapped.Value;
				}
				else
				{
					smartLabels.HideOverlapped = !chartSmartLabels.Instance.ShowOverlapped;
				}
			}
			if (chartSmartLabels.MarkerOverlapping != null)
			{
				if (!chartSmartLabels.MarkerOverlapping.IsExpression)
				{
					smartLabels.MarkerOverlapping = chartSmartLabels.MarkerOverlapping.Value;
				}
				else
				{
					smartLabels.MarkerOverlapping = chartSmartLabels.Instance.MarkerOverlapping;
				}
			}
			if (chartSmartLabels.MaxMovingDistance != null)
			{
				if (!chartSmartLabels.MaxMovingDistance.IsExpression)
				{
					if (chartSmartLabels.MaxMovingDistance.Value != null)
					{
						smartLabels.MaxMovingDistance = MappingHelper.ToPixels(chartSmartLabels.MaxMovingDistance.Value, base.DpiX);
					}
				}
				else if (chartSmartLabels.Instance.MaxMovingDistance != null)
				{
					smartLabels.MaxMovingDistance = MappingHelper.ToPixels(chartSmartLabels.Instance.MaxMovingDistance, base.DpiX);
				}
			}
			if (chartSmartLabels.MinMovingDistance != null)
			{
				if (!chartSmartLabels.MinMovingDistance.IsExpression)
				{
					if (chartSmartLabels.MinMovingDistance.Value != null)
					{
						smartLabels.MinMovingDistance = MappingHelper.ToPixels(chartSmartLabels.MinMovingDistance.Value, base.DpiX);
					}
				}
				else if (chartSmartLabels.Instance.MinMovingDistance != null)
				{
					smartLabels.MinMovingDistance = MappingHelper.ToPixels(chartSmartLabels.Instance.MinMovingDistance, base.DpiX);
				}
			}
		}

		private LineAnchorCap GetCalloutLineAnchor(ChartCalloutLineAnchor chartCalloutLineAnchor)
		{
			switch (chartCalloutLineAnchor)
			{
			case ChartCalloutLineAnchor.Arrow:
				return LineAnchorCap.Arrow;
			case ChartCalloutLineAnchor.Diamond:
				return LineAnchorCap.Diamond;
			case ChartCalloutLineAnchor.Round:
				return LineAnchorCap.Round;
			case ChartCalloutLineAnchor.Square:
				return LineAnchorCap.Square;
			default:
				return LineAnchorCap.None;
			}
		}

		private ChartDashStyle GetCalloutLineStyle(ChartCalloutLineStyle chartCalloutLineStyle)
		{
			switch (chartCalloutLineStyle)
			{
			case ChartCalloutLineStyle.DashDot:
				return ChartDashStyle.DashDot;
			case ChartCalloutLineStyle.DashDotDot:
				return ChartDashStyle.DashDotDot;
			case ChartCalloutLineStyle.Dashed:
				return ChartDashStyle.Dash;
			case ChartCalloutLineStyle.Dotted:
				return ChartDashStyle.Dot;
			case ChartCalloutLineStyle.Solid:
			case ChartCalloutLineStyle.Double:
				return ChartDashStyle.Solid;
			default:
				return ChartDashStyle.NotSet;
			}
		}

		private LabelCalloutStyle GetCalloutStyle(ChartCalloutStyle chartCalloutStyle)
		{
			switch (chartCalloutStyle)
			{
			case ChartCalloutStyle.Box:
				return LabelCalloutStyle.Box;
			case ChartCalloutStyle.Underline:
				return LabelCalloutStyle.Underlined;
			default:
				return LabelCalloutStyle.None;
			}
		}

		private void RenderNoMoveDirections(ChartNoMoveDirections chartNoMoveDirections, SmartLabelsStyle smartLabelsStyle)
		{
			if (chartNoMoveDirections != null)
			{
				LabelAlignmentTypes labelAlignmentTypes = (LabelAlignmentTypes)0;
				if (chartNoMoveDirections.Down != null)
				{
					if (!chartNoMoveDirections.Down.IsExpression)
					{
						if (!chartNoMoveDirections.Down.Value)
						{
							labelAlignmentTypes |= LabelAlignmentTypes.Bottom;
						}
					}
					else if (!chartNoMoveDirections.Instance.Down)
					{
						labelAlignmentTypes |= LabelAlignmentTypes.Bottom;
					}
				}
				else
				{
					labelAlignmentTypes |= LabelAlignmentTypes.Bottom;
				}
				if (chartNoMoveDirections.DownLeft != null)
				{
					if (!chartNoMoveDirections.DownLeft.IsExpression)
					{
						if (!chartNoMoveDirections.DownLeft.Value)
						{
							labelAlignmentTypes |= LabelAlignmentTypes.BottomLeft;
						}
					}
					else if (!chartNoMoveDirections.Instance.DownLeft)
					{
						labelAlignmentTypes |= LabelAlignmentTypes.BottomLeft;
					}
				}
				else
				{
					labelAlignmentTypes |= LabelAlignmentTypes.BottomLeft;
				}
				if (chartNoMoveDirections.DownRight != null)
				{
					if (!chartNoMoveDirections.DownRight.IsExpression)
					{
						if (!chartNoMoveDirections.DownRight.Value)
						{
							labelAlignmentTypes |= LabelAlignmentTypes.BottomRight;
						}
					}
					else if (!chartNoMoveDirections.Instance.DownRight)
					{
						labelAlignmentTypes |= LabelAlignmentTypes.BottomRight;
					}
				}
				else
				{
					labelAlignmentTypes |= LabelAlignmentTypes.BottomRight;
				}
				if (chartNoMoveDirections.Left != null)
				{
					if (!chartNoMoveDirections.Left.IsExpression)
					{
						if (!chartNoMoveDirections.Left.Value)
						{
							labelAlignmentTypes |= LabelAlignmentTypes.Left;
						}
					}
					else if (!chartNoMoveDirections.Instance.Left)
					{
						labelAlignmentTypes |= LabelAlignmentTypes.Left;
					}
				}
				else
				{
					labelAlignmentTypes |= LabelAlignmentTypes.Left;
				}
				if (chartNoMoveDirections.Right != null)
				{
					if (!chartNoMoveDirections.Right.IsExpression)
					{
						if (!chartNoMoveDirections.Right.Value)
						{
							labelAlignmentTypes |= LabelAlignmentTypes.Right;
						}
					}
					else if (!chartNoMoveDirections.Instance.Right)
					{
						labelAlignmentTypes |= LabelAlignmentTypes.Right;
					}
				}
				else
				{
					labelAlignmentTypes |= LabelAlignmentTypes.Right;
				}
				if (chartNoMoveDirections.Up != null)
				{
					if (!chartNoMoveDirections.Up.IsExpression)
					{
						if (!chartNoMoveDirections.Up.Value)
						{
							labelAlignmentTypes |= LabelAlignmentTypes.Top;
						}
					}
					else if (!chartNoMoveDirections.Instance.Up)
					{
						labelAlignmentTypes |= LabelAlignmentTypes.Top;
					}
				}
				else
				{
					labelAlignmentTypes |= LabelAlignmentTypes.Top;
				}
				if (chartNoMoveDirections.UpLeft != null)
				{
					if (!chartNoMoveDirections.UpLeft.IsExpression)
					{
						if (!chartNoMoveDirections.UpLeft.Value)
						{
							labelAlignmentTypes |= LabelAlignmentTypes.TopLeft;
						}
					}
					else if (!chartNoMoveDirections.Instance.UpLeft)
					{
						labelAlignmentTypes |= LabelAlignmentTypes.TopLeft;
					}
				}
				else
				{
					labelAlignmentTypes |= LabelAlignmentTypes.TopLeft;
				}
				if (chartNoMoveDirections.UpRight != null)
				{
					if (!chartNoMoveDirections.UpRight.IsExpression)
					{
						if (!chartNoMoveDirections.UpRight.Value)
						{
							labelAlignmentTypes |= LabelAlignmentTypes.TopRight;
						}
					}
					else if (!chartNoMoveDirections.Instance.UpRight)
					{
						labelAlignmentTypes |= LabelAlignmentTypes.TopRight;
					}
				}
				else
				{
					labelAlignmentTypes |= LabelAlignmentTypes.TopRight;
				}
				smartLabelsStyle.MovingDirection = labelAlignmentTypes;
			}
		}

		private void RenderAnnotations()
		{
		}

		private void RenderData()
		{
			this.RenderSeriesGroupings();
			this.PostProcessData();
			this.RenderCategoryGrouping();
			this.RenderSpecialChartTypes();
			this.OnPostApplyData();
			this.RenderDerivedSeriesCollecion();
		}

		private void RenderSeriesGroupings()
		{
			this.RenderSeriesGroupingCollection(this.m_chart.SeriesHierarchy.MemberCollection);
		}

		private void RenderSeriesGroupingCollection(ChartMemberCollection seriesGroupingCollection)
		{
			if (!this.m_multiRow)
			{
				this.m_multiRow = (seriesGroupingCollection.Count > 1);
			}
			foreach (ChartMember item in seriesGroupingCollection)
			{
				this.RenderSeriesGrouping(item);
			}
		}

		private void RenderSeriesGrouping(ChartMember seriesGrouping)
		{
			if (!seriesGrouping.IsStatic)
			{
				ChartDynamicMemberInstance chartDynamicMemberInstance = (ChartDynamicMemberInstance)seriesGrouping.Instance;
				chartDynamicMemberInstance.ResetContext();
				this.m_multiRow = true;
				while (chartDynamicMemberInstance.MoveNext())
				{
					if (seriesGrouping.Children != null)
					{
						this.RenderSeriesGroupingCollection(seriesGrouping.Children);
					}
					else
					{
						this.RenderSeries(seriesGrouping);
					}
				}
			}
			else if (seriesGrouping.Children != null)
			{
				this.RenderSeriesGroupingCollection(seriesGrouping.Children);
			}
			else
			{
				this.RenderSeries(seriesGrouping);
			}
		}

		private void RenderCategoryGroupings(ChartSeries chartSeries, ChartMember seriesGrouping, SeriesInfo seriesInfo)
		{
			this.RenderCategoryGroupingCollection(chartSeries, seriesGrouping, this.m_chart.CategoryHierarchy.MemberCollection, seriesInfo);
		}

		private void RenderCategoryGrouping(ChartSeries chartSeries, ChartMember seriesGrouping, ChartMember categoryGrouping, SeriesInfo seriesInfo)
		{
			if (!categoryGrouping.IsStatic)
			{
				ChartDynamicMemberInstance chartDynamicMemberInstance = (ChartDynamicMemberInstance)categoryGrouping.Instance;
				chartDynamicMemberInstance.ResetContext();
				this.m_multiColumn = true;
				while (chartDynamicMemberInstance.MoveNext())
				{
					if (categoryGrouping.Children != null)
					{
						this.RenderCategoryGroupingCollection(chartSeries, seriesGrouping, categoryGrouping.Children, seriesInfo);
					}
					else
					{
						this.RenderDataPoint(chartSeries, seriesGrouping, categoryGrouping, seriesInfo, this.DataPointShowsInLegend(chartSeries));
					}
				}
			}
			else if (categoryGrouping.Children != null)
			{
				this.RenderCategoryGroupingCollection(chartSeries, seriesGrouping, categoryGrouping.Children, seriesInfo);
			}
			else
			{
				this.RenderDataPoint(chartSeries, seriesGrouping, categoryGrouping, seriesInfo, this.DataPointShowsInLegend(chartSeries));
			}
		}

		private void RenderCategoryGroupingCollection(ChartSeries chartSeries, ChartMember seriesGrouping, ChartMemberCollection categoryGroupingCollection, SeriesInfo seriesInfo)
		{
			int count = seriesInfo.Series.Points.Count;
			if (!this.m_multiColumn)
			{
				this.m_multiColumn = (categoryGroupingCollection.Count > 1);
			}
			foreach (ChartMember item in categoryGroupingCollection)
			{
				this.RenderCategoryGrouping(chartSeries, seriesGrouping, item, seriesInfo);
			}
		}

		private void RenderCategoryGrouping()
		{
			foreach (KeyValuePair<string, ChartAreaInfo> item in this.m_chartAreaInfoDictionary)
			{
				bool flag = this.CanSetCategoryGroupingLabels(item.Value);
				bool flag2 = this.VisualizeCategoryGrouping(item.Value);
				CategoryNodeCollection categoryNodes = (!flag2) ? null : this.GetCategoryNodes(item.Key);
				if (flag || flag2)
				{
					this.RenderChartAreaCategoryGroupings(item, flag, categoryNodes);
				}
			}
		}

		private bool VisualizeCategoryGrouping(ChartAreaInfo chartAreaInfo)
		{
			foreach (SeriesInfo seriesInfo in chartAreaInfo.SeriesInfoList)
			{
				if (seriesInfo.Series.ChartType == SeriesChartType.Sunburst)
				{
					return true;
				}
			}
			return false;
		}

		private void RenderChartAreaCategoryGroupings(KeyValuePair<string, ChartAreaInfo> seriesInfoList, bool setCategoryGroupingLabels, CategoryNodeCollection categoryNodes)
		{
			int num = 0;
			foreach (ChartMember item in this.m_chart.CategoryHierarchy.MemberCollection)
			{
				this.RenderCategoryGrouping(item, seriesInfoList, ref num, setCategoryGroupingLabels, categoryNodes);
			}
		}

		private void RenderCategoryGrouping(ChartMember categoryGrouping, KeyValuePair<string, ChartAreaInfo> seriesInfoList, ref int numberOfPoints, bool setCategoryGroupingLabels, CategoryNodeCollection categoryNodes)
		{
			if (!categoryGrouping.IsStatic)
			{
				ChartDynamicMemberInstance chartDynamicMemberInstance = (ChartDynamicMemberInstance)categoryGrouping.Instance;
				chartDynamicMemberInstance.ResetContext();
				while (chartDynamicMemberInstance.MoveNext())
				{
					CategoryNode categoryNode = this.GetCategoryNode(categoryGrouping, categoryNodes);
					if (categoryGrouping.Children != null)
					{
						this.RenderCategoryGroupingChildren(categoryGrouping, seriesInfoList, ref numberOfPoints, setCategoryGroupingLabels, categoryNode);
					}
					else
					{
						this.SetDataPointsCategoryGrouping(categoryGrouping, seriesInfoList, numberOfPoints, setCategoryGroupingLabels, categoryNode);
						numberOfPoints++;
					}
				}
			}
			else
			{
				CategoryNode categoryNode2 = this.GetCategoryNode(categoryGrouping, categoryNodes);
				if (categoryGrouping.Children != null)
				{
					this.RenderCategoryGroupingChildren(categoryGrouping, seriesInfoList, ref numberOfPoints, setCategoryGroupingLabels, categoryNode2);
				}
				else
				{
					this.SetDataPointsCategoryGrouping(categoryGrouping, seriesInfoList, numberOfPoints, setCategoryGroupingLabels, categoryNode2);
					numberOfPoints++;
				}
			}
		}

		private CategoryNodeCollection GetCategoryNodes(string chartAreaName)
		{
			AspNetCore.Reporting.Chart.WebForms.ChartArea chartArea = this.GetChartArea(chartAreaName);
			if (chartArea != null)
			{
				return chartArea.CategoryNodes = new CategoryNodeCollection(null);
			}
			return null;
		}

		private CategoryNode GetCategoryNode(ChartMember categoryGrouping, CategoryNodeCollection categoryNodes)
		{
			if (categoryNodes != null)
			{
				CategoryNode categoryNode = new CategoryNode(categoryNodes, this.IsCategoryEmpty(categoryGrouping), this.GetGroupingLabel(categoryGrouping));
				categoryNodes.Add(categoryNode);
				return categoryNode;
			}
			return null;
		}

		private static CategoryNodeCollection GetCategoryChildren(CategoryNode categoryNode)
		{
			if (categoryNode != null)
			{
				categoryNode.Children = new CategoryNodeCollection(categoryNode);
				return categoryNode.Children;
			}
			return null;
		}

		private bool IsCategoryEmpty(ChartMember categoryGrouping)
		{
			if (categoryGrouping.Group != null)
			{
				GroupExpressionValueCollection groupExpressions = categoryGrouping.Group.Instance.GroupExpressions;
				if (groupExpressions != null)
				{
					for (int i = 0; i < groupExpressions.Count; i++)
					{
						if (categoryGrouping.Group.Instance.GroupExpressions[i] != null)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		private bool IsDynamicOrHasDynamicParentMember(ChartMember member)
		{
			if (member == null)
			{
				return false;
			}
			if (!member.IsStatic)
			{
				return true;
			}
			if (this.IsDynamicOrHasDynamicParentMember(member.Parent))
			{
				return true;
			}
			return false;
		}

		private bool HasDynamicMember(ChartMemberCollection members)
		{
			if (members == null)
			{
				return false;
			}
			foreach (ChartMember member in members)
			{
				if (!member.IsStatic)
				{
					return true;
				}
				if (this.HasDynamicMember(member.Children))
				{
					return true;
				}
			}
			return false;
		}

		private void RenderCategoryGroupingChildren(ChartMember categoryGrouping, KeyValuePair<string, ChartAreaInfo> seriesInfoList, ref int numberOfPoints, bool setCategoryGroupingLabels, CategoryNode categoryNode)
		{
			int num = numberOfPoints;
			CategoryNodeCollection categoryChildren = ChartMapper.GetCategoryChildren(categoryNode);
			foreach (ChartMember child in categoryGrouping.Children)
			{
				this.RenderCategoryGrouping(child, seriesInfoList, ref numberOfPoints, setCategoryGroupingLabels, categoryChildren);
			}
			if (setCategoryGroupingLabels)
			{
				this.AddAxisGroupingLabel(categoryGrouping, seriesInfoList, (double)(num + 1), (double)numberOfPoints);
			}
		}

		private void SetDataPointsCategoryGrouping(ChartMember categoryGrouping, KeyValuePair<string, ChartAreaInfo> seriesInfoList, int index, bool setCategoryGroupingLabels, CategoryNode categoryNode)
		{
			foreach (SeriesInfo seriesInfo in seriesInfoList.Value.SeriesInfoList)
			{
				DataPoint dataPoint;
				try
				{
					dataPoint = seriesInfo.Series.Points[index];
				}
				catch (Exception ex)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(ex))
					{
						throw;
					}
					Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
					continue;
				}
				this.SetDataPointGrouping(categoryGrouping, seriesInfoList, dataPoint, seriesInfo, setCategoryGroupingLabels, categoryNode);
			}
		}

		private void SetDataPointGrouping(ChartMember categoryGrouping, KeyValuePair<string, ChartAreaInfo> seriesInfoList, DataPoint dataPoint, SeriesInfo seriesInfo, bool setCategoryGroupingLabels, CategoryNode categoryNode)
		{
			if (categoryNode == null)
			{
				if (setCategoryGroupingLabels && this.CanSetDataPointAxisLabel(seriesInfo.Series, dataPoint))
				{
					dataPoint.AxisLabel = this.GetFormattedGroupingLabel(categoryGrouping, seriesInfoList.Key, seriesInfo.ChartCategoryAxis);
				}
			}
			else
			{
				categoryNode.AddDataPoint(dataPoint);
			}
		}

		private void AddAxisGroupingLabel(ChartMember categoryGrouping, KeyValuePair<string, ChartAreaInfo> seriesInfoList, double startPointIndex, double endPointIndex)
		{
			int groupingLevel = this.GetGroupingLevel(categoryGrouping);
			LabelMark mark = LabelMark.LineSideMark;
			double num = 0.4;
			bool flag = false;
			bool flag2 = false;
			foreach (SeriesInfo seriesInfo in seriesInfoList.Value.SeriesInfoList)
			{
				if (seriesInfo.Series.XAxisType == AxisType.Primary)
				{
					flag = true;
				}
				else if (seriesInfo.Series.XAxisType == AxisType.Secondary)
				{
					flag2 = true;
				}
			}
			AspNetCore.Reporting.Chart.WebForms.ChartArea chartArea = this.GetChartArea(seriesInfoList.Key);
			if (chartArea != null)
			{
				if (flag)
				{
					if (!chartArea.AxisX.Margin)
					{
						chartArea.AxisX.LabelStyle.ShowEndLabels = true;
					}
					chartArea.AxisX.CustomLabels.Add(startPointIndex - num, endPointIndex + num, this.GetGroupingLabel(categoryGrouping), groupingLevel, mark);
				}
				if (flag2 && !flag)
				{
					if (!chartArea.AxisX2.Margin)
					{
						chartArea.AxisX2.LabelStyle.ShowEndLabels = true;
					}
					chartArea.AxisX2.CustomLabels.Add(startPointIndex - num, endPointIndex + num, this.GetGroupingLabel(categoryGrouping), groupingLevel, mark);
				}
			}
		}

		private void RenderSeries(ChartMember seriesGrouping)
		{
			ChartSeries chartSeries = ((ReportElementCollectionBase<ChartSeries>)this.m_chart.ChartData.SeriesCollection)[seriesGrouping.MemberCellIndex];
			SeriesInfo seriesInfo = null;
			bool flag = this.DataPointShowsInLegend(chartSeries);
			if (flag)
			{
				seriesInfo = this.GetShapeSeriesOnSameChartArea(chartSeries);
				if (seriesInfo == null)
				{
					seriesInfo = this.CreateSeries(seriesGrouping, chartSeries);
					seriesInfo.IsExploded = this.IsSeriesExploded(chartSeries);
					seriesInfo.IsAttachedToScalarAxis = this.IsSeriesAttachedToScalarAxis(seriesInfo);
					seriesInfo.Series.SetAttribute(ChartMapper.m_pieAutoAxisLabelsName, "False");
					this.RenderSeries(seriesGrouping, seriesInfo.ChartSeries, seriesInfo.Series, seriesInfo.IsLine);
				}
			}
			else
			{
				seriesInfo = this.CreateSeries(seriesGrouping, chartSeries);
				seriesInfo.IsLine = this.IsSeriesLine(chartSeries);
				seriesInfo.IsRange = this.IsSeriesRange(chartSeries);
				seriesInfo.IsBubble = this.IsSeriesBubble(chartSeries);
				seriesInfo.IsAttachedToScalarAxis = this.IsSeriesAttachedToScalarAxis(seriesInfo);
				seriesInfo.IsGradientPerDataPointSupported = this.IsGradientPerDataPointSupported(chartSeries);
				this.RenderSeries(seriesGrouping, seriesInfo.ChartSeries, seriesInfo.Series, seriesInfo.IsLine);
			}
			seriesInfo.DataPointBackgroundImageInfoCollection.Initialize(chartSeries);
			this.RenderCategoryGroupings(chartSeries, seriesGrouping, seriesInfo);
			if (!flag)
			{
				this.AdjustNonShapeSeriesAppearance(seriesInfo);
			}
			this.OnPostApplySeriesData(seriesInfo.Series);
		}

		private void AdjustNonShapeSeriesAppearance(SeriesInfo seriesInfo)
		{
			bool flag = false;
			if (seriesInfo.IsDataPointColorEmpty.HasValue)
			{
				flag = seriesInfo.IsDataPointColorEmpty.Value;
			}
			else if (seriesInfo.DefaultDataPointAppearance != null)
			{
				flag = (seriesInfo.DefaultDataPointAppearance.Color == Color.Empty);
			}
			if (!flag)
			{
				DataPoint dataPoint = this.GetFirstNonEmptyDataPoint(seriesInfo.Series);
				if (dataPoint == null)
				{
					dataPoint = seriesInfo.DefaultDataPointAppearance;
				}
				if (dataPoint != null)
				{
					seriesInfo.Series.Color = dataPoint.Color;
				}
				else
				{
					seriesInfo.Series.Color = Color.White;
				}
			}
			if (seriesInfo.IsDataPointHatchDefined && this.m_hatcher != null)
			{
				seriesInfo.Series.BackHatchStyle = ChartHatchStyle.None;
			}
			if (!seriesInfo.IsGradientPerDataPointSupported && seriesInfo.IsGradientSupported)
			{
				if (seriesInfo.Color.HasValue)
				{
					seriesInfo.Series.Color = seriesInfo.Color.Value;
				}
				if (seriesInfo.BackGradientEndColor.HasValue)
				{
					seriesInfo.Series.BackGradientEndColor = seriesInfo.BackGradientEndColor.Value;
				}
				if (seriesInfo.BackGradientType.HasValue)
				{
					seriesInfo.Series.BackGradientType = seriesInfo.BackGradientType.Value;
				}
			}
		}

		private void RenderSpecialChartTypes()
		{
			foreach (ChartAreaInfo value in this.m_chartAreaInfoDictionary.Values)
			{
				foreach (SeriesInfo seriesInfo in value.SeriesInfoList)
				{
					if (this.IsSeriesCollectedPie(seriesInfo.Series))
					{
						this.ShowPieAsCollected(seriesInfo.Series);
					}
					else
					{
						bool flag = this.IsSeriesPareto(seriesInfo.Series);
						bool flag2 = this.IsSeriesHistogram(seriesInfo.Series);
						if ((flag || flag2) && this.ChartAreaHasMultipleSeries(seriesInfo.Series) && this.IsCategoryHierarchyValidForHistogramAndPareto(seriesInfo) && !this.IsDynamicOrHasDynamicParentMember(seriesInfo.SeriesGrouping))
						{
							if (flag)
							{
								this.MakeParetoChart(seriesInfo.Series);
							}
							else if (flag2)
							{
								this.MakeHistogramChart(seriesInfo.Series);
							}
						}
					}
				}
			}
		}

		private bool ChartAreaHasMultipleSeries(Series series)
		{
			bool result = true;
			foreach (Series item in this.m_coreChart.Series)
			{
				if (!object.ReferenceEquals(series, item) && series.ChartArea == item.ChartArea)
				{
					return false;
				}
			}
			return result;
		}

		private bool IsCategoryHierarchyValidForHistogramAndPareto(SeriesInfo seriesInfo)
		{
			if (this.m_chart.CategoryHierarchy.MemberCollection.Count == 0)
			{
				return true;
			}
			if (!this.HasDynamicMember(((ReportElementCollectionBase<ChartMember>)this.m_chart.CategoryHierarchy.MemberCollection)[0].Children) && seriesInfo.ChartSeries.Count <= 1)
			{
				return true;
			}
			return false;
		}

		private void MakeParetoChart(Series series)
		{
			try
			{
				ParetoHelper paretoHelper = new ParetoHelper();
				paretoHelper.MakeParetoChart(this.m_coreChart, series.Name, series.LegendText + " Pareto Line");
			}
			catch (Exception ex)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex))
				{
					throw;
				}
				Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
			}
		}

		private void MakeHistogramChart(Series series)
		{
			try
			{
				HistogramHelper histogramHelper = new HistogramHelper();
				try
				{
					histogramHelper.SegmentIntervalNumber = int.Parse(((DataPointAttributes)series)["HistogramSegmentIntervalNumber"], CultureInfo.InvariantCulture);
				}
				catch (Exception ex)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(ex))
					{
						throw;
					}
					Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
				}
				try
				{
					histogramHelper.SegmentIntervalWidth = double.Parse(((DataPointAttributes)series)["HistogramSegmentIntervalWidth"], CultureInfo.InvariantCulture);
				}
				catch (Exception ex2)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(ex2))
					{
						throw;
					}
					Global.Tracer.Trace(TraceLevel.Verbose, ex2.Message);
				}
				try
				{
					histogramHelper.ShowPercentOnSecondaryYAxis = bool.Parse(((DataPointAttributes)series)["HistogramShowPercentOnSecondaryYAxis"]);
				}
				catch (Exception ex3)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(ex3))
					{
						throw;
					}
					Global.Tracer.Trace(TraceLevel.Verbose, ex3.Message);
				}
				histogramHelper.CreateHistogram(this.m_coreChart, series.Name, series.Name + "_Histogram", series.LegendText + " Histogram");
			}
			catch (Exception ex4)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex4))
				{
					throw;
				}
				Global.Tracer.Trace(TraceLevel.Verbose, ex4.Message);
			}
		}

		private void ShowPieAsCollected(Series series)
		{
			try
			{
				CollectedPieHelper collectedPieHelper = new CollectedPieHelper(this.m_coreChart);
				bool showCollectedLegend = false;
				bool.TryParse(((DataPointAttributes)series)["CollectedChartShowLegend"], out showCollectedLegend);
				collectedPieHelper.ShowCollectedLegend = showCollectedLegend;
				bool showCollectedPointLabels = false;
				bool.TryParse(((DataPointAttributes)series)["CollectedChartShowLabels"], out showCollectedPointLabels);
				collectedPieHelper.ShowCollectedPointLabels = showCollectedPointLabels;
				string text = ((DataPointAttributes)series)["CollectedLabel"];
				if (text != null)
				{
					collectedPieHelper.CollectedLabel = text;
				}
				string text2 = ((DataPointAttributes)series)["CollectedColor"];
				if (text2 != null && text2 != "")
				{
					Color empty = Color.Empty;
					try
					{
						ColorConverter colorConverter = new ColorConverter();
						empty = ((string.Compare(text2, "Empty", StringComparison.OrdinalIgnoreCase) != 0) ? ((Color)colorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, text2)) : Color.Empty);
					}
					catch (Exception e)
					{
						if (AsynchronousExceptionDetection.IsStoppingException(e))
						{
							throw;
						}
						empty = Color.Empty;
					}
					collectedPieHelper.SliceColor = empty;
				}
				double num = 5.0;
				try
				{
					num = double.Parse(((DataPointAttributes)series)["CollectedThreshold"], CultureInfo.InvariantCulture);
				}
				catch
				{
					num = 5.0;
				}
				collectedPieHelper.CollectedPercentage = num;
				collectedPieHelper.ShowCollectedDataAsOneSlice = true;
				collectedPieHelper.SupplementedAreaSizeRatio = 0.9f;
				collectedPieHelper.ShowSmallSegmentsAsSupplementalPie(series);
			}
			catch (Exception ex)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex))
				{
					throw;
				}
				Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
			}
		}

		private SeriesInfo GetShapeSeriesOnSameChartArea(ChartSeries chartSeries)
		{
			foreach (KeyValuePair<string, ChartAreaInfo> item in this.m_chartAreaInfoDictionary)
			{
				if (item.Key == this.GetSeriesChartAreaName(chartSeries))
				{
					foreach (SeriesInfo seriesInfo in item.Value.SeriesInfoList)
					{
						if (this.DataPointShowsInLegend(seriesInfo.ChartSeries))
						{
							return seriesInfo;
						}
					}
				}
			}
			return null;
		}

		private SeriesInfo CreateSeries(ChartMember seriesGrouping, ChartSeries chartSeries)
		{
			Series series = new Series();
			SeriesInfo seriesInfo = new SeriesInfo();
			seriesInfo.Series = series;
			seriesInfo.ChartSeries = chartSeries;
			seriesInfo.SeriesGrouping = seriesGrouping;
			series.ChartArea = this.GetSeriesChartAreaName(chartSeries);
			this.m_coreChart.Series.Add(series);
			this.AddSeriesToDictionary(seriesInfo);
			return seriesInfo;
		}

		private void RenderSeries(ChartMember seriesGrouping, ChartSeries chartSeries, Series series, bool isLine)
		{
			this.SetSeriesProperties(chartSeries, seriesGrouping, series);
			if (!this.DataPointShowsInLegend(chartSeries) && this.m_hatcher != null)
			{
				series.BackHatchStyle = this.m_hatcher.Current;
			}
			if (this.m_autoMarker != null)
			{
				this.m_autoMarker.MoveNext();
			}
			if (chartSeries.Style != null)
			{
				this.RenderSeriesStyle(chartSeries.Style, chartSeries.Instance.Style, series, isLine);
			}
			this.RenderItemInLegendActionInfo(chartSeries.ActionInfo, series.LegendToolTip, series);
			this.RenderItemInLegend(chartSeries.ChartItemInLegend, series, this.DataPointShowsInLegend(chartSeries));
			this.RenderCustomProperties(chartSeries.CustomProperties, series);
			this.RenderEmptyPoint(chartSeries.EmptyPoints, series.EmptyPointStyle, isLine);
			this.RenderSmartLabels(chartSeries.SmartLabel, series.SmartLabels);
			this.RenderDataLabel(chartSeries.DataLabel, series, false);
			this.RenderSeriesMarker(chartSeries.Marker, series);
			if (this.m_chartAreaInfoDictionary.ContainsKey(series.ChartArea))
			{
				ChartAreaInfo chartAreaInfo = this.m_chartAreaInfoDictionary[series.ChartArea];
				if (chartAreaInfo != null)
				{
					AspNetCore.Reporting.Chart.WebForms.ChartArea chartArea = this.GetChartArea(series.ChartArea);
					if (chartArea != null)
					{
						AspNetCore.Reporting.Chart.WebForms.Axis categoryAxis = this.GetCategoryAxis(chartArea, series.XAxisType);
						if (categoryAxis != null && this.IsAxisAutoMargin(chartAreaInfo, categoryAxis) && this.DoesSeriesRequireMargin(chartSeries))
						{
							categoryAxis.Margin = true;
							chartAreaInfo.CategoryAxesAutoMargin.Remove(categoryAxis);
						}
					}
				}
			}
		}

		private AspNetCore.Reporting.Chart.WebForms.Axis GetCategoryAxis(AspNetCore.Reporting.Chart.WebForms.ChartArea area, AxisType axisType)
		{
			for (int i = 0; i < area.Axes.Length; i++)
			{
				AspNetCore.Reporting.Chart.WebForms.Axis axis = area.Axes[i];
				if (axis.Type != AxisName.Y && axis.Type != AxisName.Y2)
				{
					if (axis.Type == AxisName.X && axisType == AxisType.Primary)
					{
						goto IL_0037;
					}
					if (axis.Type == AxisName.X2 && axisType == AxisType.Secondary)
					{
						goto IL_0037;
					}
				}
				continue;
				IL_0037:
				return axis;
			}
			return null;
		}

		private string GetSeriesChartAreaName(ChartSeries chartSeries)
		{
			if (chartSeries.ChartAreaName != null)
			{
				if (!chartSeries.ChartAreaName.IsExpression)
				{
					if (chartSeries.ChartAreaName.Value != null)
					{
						return chartSeries.ChartAreaName.Value;
					}
				}
				else if (chartSeries.Instance.ChartAreaName != null)
				{
					return chartSeries.Instance.ChartAreaName;
				}
			}
			return ChartMapper.m_defaulChartAreaName;
		}

		private string GetSeriesCategoryAxisName(ChartSeries chartSeries)
		{
			if (chartSeries.CategoryAxisName != null)
			{
				if (!chartSeries.CategoryAxisName.IsExpression)
				{
					if (chartSeries.CategoryAxisName.Value != null)
					{
						return chartSeries.CategoryAxisName.Value;
					}
				}
				else if (chartSeries.Instance.CategoryAxisName != null)
				{
					return chartSeries.Instance.CategoryAxisName;
				}
			}
			return "Primary";
		}

		private void SetSeriesProperties(ChartSeries chartSeries, ChartMember seriesGrouping, Series series)
		{
			this.SetSeriesType(chartSeries, series);
			if (seriesGrouping == null || !this.IsDynamicOrHasDynamicParentMember(seriesGrouping))
			{
				series.Name = chartSeries.Name;
			}
			if (chartSeries.LegendName != null)
			{
				if (!chartSeries.LegendName.IsExpression)
				{
					if (chartSeries.LegendName.Value != null)
					{
						series.Legend = chartSeries.LegendName.Value;
					}
				}
				else if (chartSeries.Instance.LegendName != null)
				{
					series.Legend = chartSeries.Instance.LegendName;
				}
			}
			if (chartSeries.LegendText != null)
			{
				if (!chartSeries.LegendText.IsExpression)
				{
					if (chartSeries.LegendText.Value != null)
					{
						series.LegendText = chartSeries.LegendText.Value;
					}
				}
				else if (chartSeries.Instance.LegendText != null)
				{
					series.LegendText = chartSeries.Instance.LegendText;
				}
			}
			if (chartSeries.HideInLegend != null)
			{
				if (!chartSeries.HideInLegend.IsExpression)
				{
					series.ShowInLegend = !chartSeries.HideInLegend.Value;
				}
				else
				{
					series.ShowInLegend = !chartSeries.Instance.HideInLegend;
				}
			}
			string seriesCategoryAxisName = this.GetSeriesCategoryAxisName(chartSeries);
			if (seriesCategoryAxisName == "Secondary")
			{
				series.XAxisType = AxisType.Secondary;
			}
			else
			{
				series.XAxisType = AxisType.Primary;
			}
			if (chartSeries.ValueAxisName != null)
			{
				if (!chartSeries.ValueAxisName.IsExpression)
				{
					if (chartSeries.ValueAxisName.Value != null && chartSeries.ValueAxisName.Value == "Secondary")
					{
						series.YAxisType = AxisType.Secondary;
					}
				}
				else if (chartSeries.Instance.ValueAxisName == "Secondary")
				{
					series.YAxisType = AxisType.Secondary;
				}
			}
			ReportStringProperty toolTip = chartSeries.ToolTip;
			if (toolTip != null)
			{
				if (!toolTip.IsExpression)
				{
					if (toolTip.Value != null)
					{
						series.LegendToolTip = toolTip.Value;
					}
				}
				else
				{
					string toolTip2 = chartSeries.Instance.ToolTip;
					if (toolTip2 != null)
					{
						series.LegendToolTip = toolTip2;
					}
				}
			}
			ReportBoolProperty hidden = chartSeries.Hidden;
			if (hidden != null)
			{
				if (!hidden.IsExpression)
				{
					series.Enabled = !hidden.Value;
				}
				else
				{
					series.Enabled = !chartSeries.Instance.Hidden;
				}
			}
			if (seriesGrouping != null && !this.DataPointShowsInLegend(chartSeries) && series.LegendText == "")
			{
				series.LegendText = this.GetSeriesLegendText(seriesGrouping);
			}
		}

		private void RenderDataPointStyle(Style style, StyleInstance styleInstance, DataPoint dataPoint, SeriesInfo seriesInfo, int cellIndex)
		{
			this.RenderDataPointAttributesStyle(style, styleInstance, dataPoint, seriesInfo.IsLine);
			if (seriesInfo.IsGradientPerDataPointSupported)
			{
				this.RenderDataPointAttributesGradient(style, styleInstance, dataPoint);
			}
			else if (seriesInfo.IsGradientSupported)
			{
				seriesInfo.IsGradientSupported = this.CheckGradientSupport(style, styleInstance, dataPoint, seriesInfo);
			}
			this.RenderDataPointBackgroundImage(style.BackgroundImage, dataPoint, ((List<DataPointBackgroundImageInfo>)seriesInfo.DataPointBackgroundImageInfoCollection)[cellIndex].DataPointBackgroundImage);
		}

		private bool CheckGradientSupport(Style style, StyleInstance styleInstance, DataPoint dataPoint, SeriesInfo seriesInfo)
		{
			if (!seriesInfo.BackGradientType.HasValue)
			{
				seriesInfo.BackGradientType = this.GetGradientType(MappingHelper.GetStyleBackGradientType(style, styleInstance));
				GradientType? backGradientType = seriesInfo.BackGradientType;
				if (backGradientType.GetValueOrDefault() == GradientType.None && backGradientType.HasValue)
				{
					return false;
				}
			}
			else if (seriesInfo.BackGradientType != this.GetGradientType(MappingHelper.GetStyleBackGradientType(style, styleInstance)))
			{
				return false;
			}
			if (!seriesInfo.Color.HasValue)
			{
				seriesInfo.Color = dataPoint.Color;
			}
			else if (seriesInfo.Color != (Color?)dataPoint.Color)
			{
				return false;
			}
			if (!seriesInfo.BackGradientEndColor.HasValue)
			{
				seriesInfo.BackGradientEndColor = MappingHelper.GetStyleBackGradientEndColor(style, styleInstance);
			}
			else if (seriesInfo.BackGradientEndColor != (Color?)MappingHelper.GetStyleBackGradientEndColor(style, styleInstance))
			{
				return false;
			}
			return true;
		}

		private string GetImageName(BackgroundImage backgroundImage, BackgroundImageInfo backgroundImageInfo)
		{
			string text = "";
			if (backgroundImageInfo == null || !backgroundImageInfo.CanShareBackgroundImage || backgroundImageInfo.SharedBackgroundImageName == null)
			{
				text = this.CreateImage(backgroundImage);
				if (backgroundImageInfo != null && backgroundImageInfo.CanShareBackgroundImage)
				{
					backgroundImageInfo.SharedBackgroundImageName = text;
				}
			}
			else
			{
				text = backgroundImageInfo.SharedBackgroundImageName;
			}
			return text;
		}

		private void RenderDataPointBackgroundImage(BackgroundImage backgroundImage, DataPoint dataPoint, BackgroundImageInfo backgroundImageInfo)
		{
			dataPoint.BackImage = this.GetImageName(backgroundImage, backgroundImageInfo);
			ChartImageWrapMode backImageMode = default(ChartImageWrapMode);
			ChartImageAlign backImageAlign = default(ChartImageAlign);
			Color backImageTransparentColor = default(Color);
			this.GetBackgroundImageProperties(backgroundImage, out backImageMode, out backImageAlign, out backImageTransparentColor);
			dataPoint.BackImageMode = backImageMode;
			dataPoint.BackImageAlign = backImageAlign;
			dataPoint.BackImageTransparentColor = backImageTransparentColor;
		}

		private void RenderSeriesStyle(Style style, StyleInstance styleInstance, Series series, bool isSeriesLine)
		{
			this.RenderDataPointAttributesStyle(style, styleInstance, series, isSeriesLine);
			this.RenderDataPointAttributesGradient(style, styleInstance, series);
			if (MappingHelper.IsStylePropertyDefined(style.ShadowColor))
			{
				series.ShadowColor = MappingHelper.GetStyleShadowColor(style, styleInstance);
			}
			if (MappingHelper.IsStylePropertyDefined(style.ShadowOffset))
			{
				series.ShadowOffset = MappingHelper.GetStyleShadowOffset(style, styleInstance, base.DpiX);
			}
			this.RenderDataPointAttributesBackgroundImage(style.BackgroundImage, series);
		}

		private void RenderEmptyPointStyle(Style style, StyleInstance styleInstance, DataPointAttributes dataPointAttributes, bool isSeriesLine)
		{
			this.RenderDataPointAttributesStyle(style, styleInstance, dataPointAttributes, isSeriesLine);
			this.RenderDataPointAttributesGradient(style, styleInstance, dataPointAttributes);
			this.RenderDataPointAttributesBackgroundImage(style.BackgroundImage, dataPointAttributes);
		}

		private void RenderDataPointAttributesGradient(Style style, StyleInstance styleInstance, DataPointAttributes dataPointAttributes)
		{
			if (MappingHelper.IsStylePropertyDefined(style.BackgroundGradientEndColor))
			{
				dataPointAttributes.BackGradientEndColor = MappingHelper.GetStyleBackGradientEndColor(style, styleInstance);
			}
			if (MappingHelper.IsStylePropertyDefined(style.BackgroundGradientType))
			{
				dataPointAttributes.BackGradientType = this.GetGradientType(MappingHelper.GetStyleBackGradientType(style, styleInstance));
			}
		}

		private void RenderDataPointAttributesStyle(Style style, StyleInstance styleInstance, DataPointAttributes dataPointAttributes, bool isSeriesLine)
		{
			if (MappingHelper.IsStylePropertyDefined(style.Color) && (!style.Color.IsExpression || (styleInstance.Color != null && styleInstance.Color.ToString() != "#00000000")))
			{
				dataPointAttributes.Color = MappingHelper.GetStyleColor(style, styleInstance);
			}
			if (MappingHelper.IsStylePropertyDefined(style.BackgroundHatchType))
			{
				dataPointAttributes.BackHatchStyle = this.GetHatchType(MappingHelper.GetStyleBackgroundHatchType(style, styleInstance));
			}
			this.RenderDataPointAttributesBorder(style.Border, dataPointAttributes, isSeriesLine);
		}

		private void RenderDataLabelFont(ChartDataLabel chartDataLabel, DataPointAttributes dataPointAttributes, bool dataPoint)
		{
			Style style = chartDataLabel.Style;
			if (style == null)
			{
				if (dataPoint)
				{
					dataPointAttributes.Font = base.GetDefaultFontFromCache(this.m_coreChart.Series.Count);
				}
				else
				{
					dataPointAttributes.Font = base.GetDefaultFont();
				}
			}
			else if (dataPoint)
			{
				dataPointAttributes.Font = base.GetFontFromCache(this.m_coreChart.Series.Count, style, chartDataLabel.Instance.Style);
			}
			else
			{
				dataPointAttributes.Font = base.GetFont(style, chartDataLabel.Instance.Style);
			}
		}

		private void RenderTitleFont(ChartTitle chartTitle, Title title)
		{
			Style style = chartTitle.Style;
			if (style == null)
			{
				title.Font = base.GetDefaultFont();
			}
			else
			{
				title.Font = base.GetFont(style, chartTitle.Instance.Style);
			}
		}

		private void RenderLegendTitleFont(ChartLegendTitle chartLegendTitle, AspNetCore.Reporting.Chart.WebForms.Legend legend)
		{
			Style style = chartLegendTitle.Style;
			if (style == null)
			{
				legend.TitleFont = base.GetDefaultFont();
			}
			else
			{
				legend.TitleFont = base.GetFont(style, chartLegendTitle.Instance.Style);
			}
		}

		private void RenderLegendFont(ChartLegend chartLegend, AspNetCore.Reporting.Chart.WebForms.Legend legend)
		{
			Style style = chartLegend.Style;
			if (style == null)
			{
				legend.Font = base.GetDefaultFont();
			}
			else
			{
				legend.Font = base.GetFont(style, chartLegend.Instance.Style);
			}
		}

		private void RenderStripLineFont(ChartStripLine chartStripLine, StripLine stripLine)
		{
			Style style = chartStripLine.Style;
			if (style == null)
			{
				stripLine.TitleFont = base.GetDefaultFont();
			}
			else
			{
				stripLine.TitleFont = base.GetFont(style, chartStripLine.Instance.Style);
			}
		}

		private void RenderAxisScaleBreakBorder(Border border, AxisScaleBreakStyle axisScaleBreak)
		{
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					axisScaleBreak.LineColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					axisScaleBreak.LineStyle = this.GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), true);
				}
				axisScaleBreak.LineWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderChartBorder(Border border)
		{
			this.m_coreChart.BorderColor = Color.Black;
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					this.m_coreChart.BorderColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					this.m_coreChart.BorderStyle = this.GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), false);
				}
				this.m_coreChart.BorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderDataPointAttributesBorder(Border border, DataPointAttributes dataPointAttributes, bool isLine)
		{
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					dataPointAttributes.BorderColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					dataPointAttributes.BorderStyle = this.GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), isLine);
				}
				dataPointAttributes.BorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderTitleBorder(Border border, Title title)
		{
			title.BorderColor = Color.Black;
			title.BorderStyle = ChartDashStyle.NotSet;
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					title.BorderColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					title.BorderStyle = this.GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), false);
				}
				title.BorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderChartAreaBorder(Border border, AspNetCore.Reporting.Chart.WebForms.ChartArea area)
		{
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					area.BorderColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					area.BorderStyle = this.GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), false);
				}
				area.BorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderStripLineBorder(Border border, StripLine stripLine)
		{
			stripLine.BorderStyle = ChartDashStyle.NotSet;
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					stripLine.BorderColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					stripLine.BorderStyle = this.GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), false);
				}
				stripLine.BorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderBorderSkinBorder(Border border, BorderSkinAttributes borderSkin)
		{
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					borderSkin.FrameBorderColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					borderSkin.FrameBorderStyle = this.GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), false);
				}
				borderSkin.FrameBorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderLegendBorder(Border border, AspNetCore.Reporting.Chart.WebForms.Legend legend)
		{
			legend.BorderStyle = ChartDashStyle.NotSet;
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					legend.BorderColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					legend.BorderStyle = this.GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), false);
				}
				legend.BorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderChartBackgroundImage(BackgroundImage backgroundImage)
		{
			string backImage = default(string);
			ChartImageWrapMode backImageMode = default(ChartImageWrapMode);
			ChartImageAlign backImageAlign = default(ChartImageAlign);
			Color backImageTransparentColor = default(Color);
			this.RenderBackgroundImage(backgroundImage, out backImage, out backImageMode, out backImageAlign, out backImageTransparentColor);
			this.m_coreChart.BackImage = backImage;
			this.m_coreChart.BackImageMode = backImageMode;
			this.m_coreChart.BackImageAlign = backImageAlign;
			this.m_coreChart.BackImageTransparentColor = backImageTransparentColor;
		}

		private void RenderBorderSkinBackgroundImage(BackgroundImage backgroundImage, BorderSkinAttributes borderSkin)
		{
			string frameBackImage = default(string);
			ChartImageWrapMode frameBackImageMode = default(ChartImageWrapMode);
			ChartImageAlign frameBackImageAlign = default(ChartImageAlign);
			Color frameBackImageTransparentColor = default(Color);
			this.RenderBackgroundImage(backgroundImage, out frameBackImage, out frameBackImageMode, out frameBackImageAlign, out frameBackImageTransparentColor);
			borderSkin.FrameBackImage = frameBackImage;
			borderSkin.FrameBackImageMode = frameBackImageMode;
			borderSkin.FrameBackImageAlign = frameBackImageAlign;
			borderSkin.FrameBackImageTransparentColor = frameBackImageTransparentColor;
		}

		private void RenderDataPointAttributesBackgroundImage(BackgroundImage backgroundImage, DataPointAttributes dataPointAttributes)
		{
			string backImage = default(string);
			ChartImageWrapMode backImageMode = default(ChartImageWrapMode);
			ChartImageAlign backImageAlign = default(ChartImageAlign);
			Color backImageTransparentColor = default(Color);
			this.RenderBackgroundImage(backgroundImage, out backImage, out backImageMode, out backImageAlign, out backImageTransparentColor);
			dataPointAttributes.BackImage = backImage;
			dataPointAttributes.BackImageMode = backImageMode;
			dataPointAttributes.BackImageAlign = backImageAlign;
			dataPointAttributes.BackImageTransparentColor = backImageTransparentColor;
		}

		private void RenderTitleBackgroundImage(BackgroundImage backgroundImage, Title title)
		{
			string backImage = default(string);
			ChartImageWrapMode backImageMode = default(ChartImageWrapMode);
			ChartImageAlign backImageAlign = default(ChartImageAlign);
			Color backImageTransparentColor = default(Color);
			this.RenderBackgroundImage(backgroundImage, out backImage, out backImageMode, out backImageAlign, out backImageTransparentColor);
			title.BackImage = backImage;
			title.BackImageMode = backImageMode;
			title.BackImageAlign = backImageAlign;
			title.BackImageTransparentColor = backImageTransparentColor;
		}

		private void RenderChartAreaBackgroundImage(BackgroundImage backgroundImage, AspNetCore.Reporting.Chart.WebForms.ChartArea chartArea)
		{
			string backImage = default(string);
			ChartImageWrapMode backImageMode = default(ChartImageWrapMode);
			ChartImageAlign backImageAlign = default(ChartImageAlign);
			Color backImageTransparentColor = default(Color);
			this.RenderBackgroundImage(backgroundImage, out backImage, out backImageMode, out backImageAlign, out backImageTransparentColor);
			chartArea.BackImage = backImage;
			chartArea.BackImageMode = backImageMode;
			chartArea.BackImageAlign = backImageAlign;
			chartArea.BackImageTransparentColor = backImageTransparentColor;
		}

		private void RenderLegendBackgroundImage(BackgroundImage backgroundImage, AspNetCore.Reporting.Chart.WebForms.Legend legend)
		{
			string backImage = default(string);
			ChartImageWrapMode backImageMode = default(ChartImageWrapMode);
			ChartImageAlign backImageAlign = default(ChartImageAlign);
			Color backImageTransparentColor = default(Color);
			this.RenderBackgroundImage(backgroundImage, out backImage, out backImageMode, out backImageAlign, out backImageTransparentColor);
			legend.BackImage = backImage;
			legend.BackImageMode = backImageMode;
			legend.BackImageAlign = backImageAlign;
			legend.BackImageTransparentColor = backImageTransparentColor;
		}

		private void RenderBackgroundImage(BackgroundImage backgroundImage, out string backImage, out ChartImageWrapMode backImageMode, out ChartImageAlign backImageAlign, out Color backImageTransparentColor)
		{
			this.GetBackgroundImageProperties(backgroundImage, out backImageMode, out backImageAlign, out backImageTransparentColor);
			backImage = this.CreateImage(backgroundImage);
		}

		private void GetBackgroundImageProperties(BackgroundImage backgroundImage, out ChartImageWrapMode backImageMode, out ChartImageAlign backImageAlign, out Color backImageTransparentColor)
		{
			backImageMode = ChartImageWrapMode.Scaled;
			backImageAlign = ChartImageAlign.Center;
			backImageTransparentColor = Color.Empty;
			if (backgroundImage != null)
			{
				if (MappingHelper.IsStylePropertyDefined(backgroundImage.BackgroundRepeat))
				{
					if (!backgroundImage.BackgroundRepeat.IsExpression)
					{
						backImageMode = this.GetBackImageMode(backgroundImage.BackgroundRepeat.Value);
					}
					else
					{
						backImageMode = this.GetBackImageMode(backgroundImage.Instance.BackgroundRepeat);
					}
				}
				if (MappingHelper.IsStylePropertyDefined(backgroundImage.Position))
				{
					if (!backgroundImage.Position.IsExpression)
					{
						backImageAlign = this.GetBackImageAlign(backgroundImage.Position.Value);
					}
					else
					{
						backImageAlign = this.GetBackImageAlign(backgroundImage.Instance.Position);
					}
				}
				if (MappingHelper.IsStylePropertyDefined(backgroundImage.TransparentColor))
				{
					Color empty = Color.Empty;
					if (MappingHelper.GetColorFromReportColorProperty(backgroundImage.TransparentColor, ref empty))
					{
						backImageTransparentColor = empty;
					}
					else if (backgroundImage.Instance.TransparentColor != null)
					{
						backImageTransparentColor = backgroundImage.Instance.TransparentColor.ToColor();
					}
				}
			}
		}

		private string CreateImage(BackgroundImage backgroundImage)
		{
			string text = "";
			if (backgroundImage != null)
			{
				System.Drawing.Image imageFromStream = this.GetImageFromStream(backgroundImage);
				if (imageFromStream != null)
				{
					text = ChartMapper.m_imagePrefix + this.m_coreChart.Images.Count;
					this.m_coreChart.Images.Add(text, imageFromStream);
				}
			}
			return text;
		}

		private void RenderStripLineBackgroundImage(BackgroundImage backgroundImage, StripLine stripLine)
		{
			string backImage = default(string);
			ChartImageWrapMode backImageMode = default(ChartImageWrapMode);
			ChartImageAlign backImageAlign = default(ChartImageAlign);
			Color backImageTransparentColor = default(Color);
			this.RenderBackgroundImage(backgroundImage, out backImage, out backImageMode, out backImageAlign, out backImageTransparentColor);
			stripLine.BackImage = backImage;
			stripLine.BackImageMode = backImageMode;
			stripLine.BackImageAlign = backImageAlign;
			stripLine.BackImageTransparentColor = backImageTransparentColor;
		}

		private System.Drawing.Image GetImageFromStream(BackgroundImage backgroundImage)
		{
			if (backgroundImage.Instance.ImageData == null)
			{
				return null;
			}
			MemoryStream stream = new MemoryStream(backgroundImage.Instance.ImageData, false);
			return System.Drawing.Image.FromStream(stream);
		}

		private void RenderActionInfo(ActionInfo actionInfo, string toolTip, IMapAreaAttributes mapAreaAttributes)
		{
			if (actionInfo == null && string.IsNullOrEmpty(toolTip))
			{
				return;
			}
			string text = default(string);
			ActionInfoWithDynamicImageMap actionInfoWithDynamicImageMap = MappingHelper.CreateActionInfoDynamic((ReportItem)this.m_chart, actionInfo, toolTip, out text);
			if (actionInfoWithDynamicImageMap != null)
			{
				if (text != null)
				{
					mapAreaAttributes.Href = text;
				}
				int count = this.m_actions.Count;
				this.m_actions.InternalList.Add(actionInfoWithDynamicImageMap);
				mapAreaAttributes.Tag = count;
			}
		}

		private void RenderEmptyPoint(ChartEmptyPoints chartEmptyPoint, DataPointAttributes emptyPoint, bool isSeriesLine)
		{
			if (chartEmptyPoint != null)
			{
				this.SetEmptyPointProperties(chartEmptyPoint, emptyPoint);
				if (chartEmptyPoint.Style != null)
				{
					this.RenderEmptyPointStyle(chartEmptyPoint.Style, chartEmptyPoint.Instance.Style, emptyPoint, isSeriesLine);
				}
				this.RenderActionInfo(chartEmptyPoint.ActionInfo, emptyPoint.ToolTip, emptyPoint);
				this.RenderEmptyPointMarker(chartEmptyPoint.Marker, emptyPoint);
				this.RenderDataLabel(chartEmptyPoint.DataLabel, emptyPoint, true);
				this.RenderCustomProperties(chartEmptyPoint.CustomProperties, emptyPoint);
			}
		}

		private void SetEmptyPointProperties(ChartEmptyPoints chartEmptyPoint, DataPointAttributes emptyPoint)
		{
			if (chartEmptyPoint.AxisLabel != null)
			{
				object obj = null;
				obj = (chartEmptyPoint.AxisLabel.IsExpression ? chartEmptyPoint.Instance.AxisLabel : chartEmptyPoint.AxisLabel.Value);
				if (obj != null)
				{
					emptyPoint.AxisLabel = this.GetFormattedValue(obj, "");
				}
			}
			ReportStringProperty toolTip = chartEmptyPoint.ToolTip;
			if (toolTip != null)
			{
				if (!toolTip.IsExpression)
				{
					if (toolTip.Value != null)
					{
						emptyPoint.ToolTip = toolTip.Value;
					}
				}
				else
				{
					string toolTip2 = chartEmptyPoint.Instance.ToolTip;
					if (toolTip2 != null)
					{
						emptyPoint.ToolTip = toolTip2;
					}
				}
			}
		}

		private void RenderItemInLegend(ChartItemInLegend chartItemInLegend, DataPointAttributes dataPointAttributes, bool dataPointShowsInLegend)
		{
			if (!dataPointShowsInLegend && chartItemInLegend != null)
			{
				if (chartItemInLegend.LegendText != null)
				{
					if (!chartItemInLegend.LegendText.IsExpression)
					{
						if (chartItemInLegend.LegendText.Value != null)
						{
							dataPointAttributes.LegendText = chartItemInLegend.LegendText.Value;
						}
					}
					else if (chartItemInLegend.Instance.LegendText != null)
					{
						dataPointAttributes.LegendText = chartItemInLegend.Instance.LegendText;
					}
				}
				ReportStringProperty toolTip = chartItemInLegend.ToolTip;
				if (toolTip != null)
				{
					if (!toolTip.IsExpression)
					{
						if (toolTip.Value != null)
						{
							dataPointAttributes.LegendToolTip = toolTip.Value;
						}
					}
					else
					{
						string toolTip2 = chartItemInLegend.Instance.ToolTip;
						if (toolTip2 != null)
						{
							dataPointAttributes.LegendToolTip = toolTip2;
						}
					}
				}
				ReportBoolProperty hidden = chartItemInLegend.Hidden;
				if (hidden != null)
				{
					if (!hidden.IsExpression)
					{
						dataPointAttributes.ShowInLegend = !hidden.Value;
					}
					else
					{
						dataPointAttributes.ShowInLegend = !chartItemInLegend.Instance.Hidden;
					}
				}
				this.RenderItemInLegendActionInfo(chartItemInLegend.ActionInfo, dataPointAttributes.LegendToolTip, dataPointAttributes);
			}
		}

		private void RenderItemInLegendActionInfo(ActionInfo actionInfo, string toolTip, DataPointAttributes dataPointAttributes)
		{
			if (actionInfo == null && string.IsNullOrEmpty(toolTip))
			{
				return;
			}
			string text = default(string);
			ActionInfoWithDynamicImageMap actionInfoWithDynamicImageMap = MappingHelper.CreateActionInfoDynamic((ReportItem)this.m_chart, actionInfo, toolTip, out text);
			if (actionInfoWithDynamicImageMap != null)
			{
				if (text != null)
				{
					dataPointAttributes.LegendHref = text;
				}
				int count = this.m_actions.Count;
				this.m_actions.InternalList.Add(actionInfoWithDynamicImageMap);
				dataPointAttributes.LegendTag = count;
			}
		}

		private bool GetCustomProperty(CustomProperty customProperty, ref string name, ref string value)
		{
			if (customProperty.Name == null)
			{
				return false;
			}
			if (customProperty.Value == null)
			{
				return false;
			}
			if (!customProperty.Name.IsExpression)
			{
				if (customProperty.Name.Value != null)
				{
					name = customProperty.Name.Value;
					goto IL_005d;
				}
				return false;
			}
			if (customProperty.Instance.Name != null)
			{
				name = customProperty.Instance.Name;
				goto IL_005d;
			}
			return false;
			IL_00ba:
			return true;
			IL_005d:
			if (!customProperty.Value.IsExpression)
			{
				if (customProperty.Value.Value != null)
				{
					value = Convert.ToString(customProperty.Value.Value, CultureInfo.InvariantCulture);
					goto IL_00ba;
				}
				return false;
			}
			if (customProperty.Instance.Value != null)
			{
				value = Convert.ToString(customProperty.Instance.Value, CultureInfo.InvariantCulture);
				goto IL_00ba;
			}
			return false;
		}

		private void RenderCustomProperties(CustomPropertyCollection customProperties, DataPointAttributes dataPointAttributes)
		{
			if (customProperties != null)
			{
				string name = null;
				string attributeValue = null;
				foreach (CustomProperty customProperty in customProperties)
				{
					if (this.GetCustomProperty(customProperty, ref name, ref attributeValue))
					{
						dataPointAttributes.SetAttribute(name, attributeValue);
					}
				}
			}
		}

		private void SetSeriesType(ChartSeries chartSeries, Series series)
		{
			ChartSeriesType chartSeriesType = ChartSeriesType.Column;
			ChartSeriesSubtype chartSeriesSubtype = ChartSeriesSubtype.Plain;
			chartSeriesType = this.GetSeriesType(chartSeries);
			chartSeriesSubtype = this.GetValidSeriesSubType(chartSeriesType, this.GetSeriesSubType(chartSeries));
			switch (chartSeriesType)
			{
			case ChartSeriesType.Area:
				this.SetSeriesTypeArea(series, chartSeriesSubtype);
				break;
			case ChartSeriesType.Bar:
				this.SetSeriesTypeBar(series, chartSeriesSubtype);
				break;
			case ChartSeriesType.Column:
				this.SetSeriesTypeColumn(series, chartSeriesSubtype);
				break;
			case ChartSeriesType.Line:
				this.SetSeriesTypeLine(series, chartSeriesSubtype);
				break;
			case ChartSeriesType.Polar:
				this.SetSeriesTypePolar(series, chartSeriesSubtype);
				break;
			case ChartSeriesType.Range:
				this.SetSeriesTypeRange(series, chartSeriesSubtype);
				break;
			case ChartSeriesType.Scatter:
				this.SetSeriesTypeScatter(series, chartSeriesSubtype);
				break;
			case ChartSeriesType.Shape:
				this.SetSeriesTypeShape(series, chartSeriesSubtype);
				break;
			}
		}

		private ChartSeriesType GetSeriesType(ChartSeries chartSeries)
		{
			if (chartSeries.Type != null)
			{
				if (!chartSeries.Type.IsExpression)
				{
					return chartSeries.Type.Value;
				}
				if (chartSeries.Instance != null)
				{
					return chartSeries.Instance.Type;
				}
			}
			return ChartSeriesType.Column;
		}

		private ChartSeriesSubtype GetSeriesSubType(ChartSeries chartSeries)
		{
			if (chartSeries.Subtype != null)
			{
				if (!chartSeries.Subtype.IsExpression)
				{
					return chartSeries.Subtype.Value;
				}
				if (chartSeries.Instance != null)
				{
					return chartSeries.Instance.Subtype;
				}
			}
			return ChartSeriesSubtype.Plain;
		}

		private ChartSeriesSubtype GetValidSeriesSubType(ChartSeriesType type, ChartSeriesSubtype subtype)
		{
			switch (type)
			{
			case ChartSeriesType.Area:
				if (subtype != ChartSeriesSubtype.Smooth && subtype != ChartSeriesSubtype.Stacked && subtype != ChartSeriesSubtype.PercentStacked)
				{
					break;
				}
				return subtype;
			case ChartSeriesType.Column:
			case ChartSeriesType.Bar:
				if (subtype != ChartSeriesSubtype.Stacked && subtype != ChartSeriesSubtype.PercentStacked)
				{
					break;
				}
				return subtype;
			case ChartSeriesType.Line:
				if (subtype != ChartSeriesSubtype.Smooth && subtype != ChartSeriesSubtype.Stepped)
				{
					break;
				}
				return subtype;
			case ChartSeriesType.Polar:
				if (subtype != ChartSeriesSubtype.Radar)
				{
					break;
				}
				return subtype;
			case ChartSeriesType.Range:
				switch (subtype)
				{
				case ChartSeriesSubtype.Smooth:
				case ChartSeriesSubtype.Candlestick:
				case ChartSeriesSubtype.Stock:
				case ChartSeriesSubtype.Bar:
				case ChartSeriesSubtype.Column:
				case ChartSeriesSubtype.BoxPlot:
				case ChartSeriesSubtype.ErrorBar:
					return subtype;
				}
				break;
			case ChartSeriesType.Scatter:
				if (subtype != ChartSeriesSubtype.Bubble)
				{
					break;
				}
				return subtype;
			case ChartSeriesType.Shape:
				switch (subtype)
				{
				case ChartSeriesSubtype.ExplodedPie:
				case ChartSeriesSubtype.Doughnut:
				case ChartSeriesSubtype.ExplodedDoughnut:
				case ChartSeriesSubtype.Funnel:
				case ChartSeriesSubtype.Pyramid:
				case ChartSeriesSubtype.TreeMap:
				case ChartSeriesSubtype.Sunburst:
					return subtype;
				default:
					return ChartSeriesSubtype.Pie;
				}
			}
			return ChartSeriesSubtype.Plain;
		}

		private void SetSeriesTypeArea(Series series, ChartSeriesSubtype subtype)
		{
			switch (subtype)
			{
			case ChartSeriesSubtype.Smooth:
				series.ChartType = SeriesChartType.SplineArea;
				break;
			case ChartSeriesSubtype.Stacked:
				series.ChartType = SeriesChartType.StackedArea;
				break;
			case ChartSeriesSubtype.PercentStacked:
				series.ChartType = SeriesChartType.StackedArea100;
				break;
			default:
				series.ChartType = SeriesChartType.Area;
				break;
			}
		}

		private void SetSeriesTypeBar(Series series, ChartSeriesSubtype subtype)
		{
			switch (subtype)
			{
			case ChartSeriesSubtype.Stacked:
				series.ChartType = SeriesChartType.StackedBar;
				break;
			case ChartSeriesSubtype.PercentStacked:
				series.ChartType = SeriesChartType.StackedBar100;
				break;
			default:
				series.ChartType = SeriesChartType.Bar;
				break;
			}
		}

		private void SetSeriesTypeColumn(Series series, ChartSeriesSubtype subtype)
		{
			switch (subtype)
			{
			case ChartSeriesSubtype.Stacked:
				series.ChartType = SeriesChartType.StackedColumn;
				break;
			case ChartSeriesSubtype.PercentStacked:
				series.ChartType = SeriesChartType.StackedColumn100;
				break;
			default:
				series.ChartType = SeriesChartType.Column;
				break;
			}
		}

		private void SetSeriesTypeLine(Series series, ChartSeriesSubtype subtype)
		{
			switch (subtype)
			{
			case ChartSeriesSubtype.Smooth:
				series.ChartType = SeriesChartType.Spline;
				break;
			case ChartSeriesSubtype.Stepped:
				series.ChartType = SeriesChartType.StepLine;
				break;
			default:
				series.ChartType = SeriesChartType.Line;
				break;
			}
		}

		private void SetSeriesTypePolar(Series series, ChartSeriesSubtype subtype)
		{
			if (subtype == ChartSeriesSubtype.Radar)
			{
				series.ChartType = SeriesChartType.Radar;
			}
			else
			{
				series.ChartType = SeriesChartType.Polar;
			}
		}

		private void SetSeriesTypeRange(Series series, ChartSeriesSubtype subtype)
		{
			switch (subtype)
			{
			case ChartSeriesSubtype.Candlestick:
				series.ChartType = SeriesChartType.Candlestick;
				break;
			case ChartSeriesSubtype.Stock:
				series.ChartType = SeriesChartType.Stock;
				break;
			case ChartSeriesSubtype.Smooth:
				series.ChartType = SeriesChartType.SplineRange;
				break;
			case ChartSeriesSubtype.Column:
				series.ChartType = SeriesChartType.RangeColumn;
				break;
			case ChartSeriesSubtype.Bar:
				series.ChartType = SeriesChartType.Gantt;
				break;
			case ChartSeriesSubtype.BoxPlot:
				series.ChartType = SeriesChartType.BoxPlot;
				break;
			case ChartSeriesSubtype.ErrorBar:
				series.ChartType = SeriesChartType.ErrorBar;
				break;
			default:
				series.ChartType = SeriesChartType.Range;
				break;
			}
		}

		private void SetSeriesTypeScatter(Series series, ChartSeriesSubtype subtype)
		{
			if (subtype == ChartSeriesSubtype.Bubble)
			{
				series.ChartType = SeriesChartType.Bubble;
			}
			else
			{
				series.ChartType = SeriesChartType.Point;
			}
		}

		private void SetSeriesTypeShape(Series series, ChartSeriesSubtype subtype)
		{
			switch (subtype)
			{
			case ChartSeriesSubtype.Doughnut:
			case ChartSeriesSubtype.ExplodedDoughnut:
				series.ChartType = SeriesChartType.Doughnut;
				break;
			case ChartSeriesSubtype.Funnel:
				series.ChartType = SeriesChartType.Funnel;
				break;
			case ChartSeriesSubtype.Pyramid:
				series.ChartType = SeriesChartType.Pyramid;
				break;
			case ChartSeriesSubtype.TreeMap:
				series.ChartType = SeriesChartType.TreeMap;
				break;
			case ChartSeriesSubtype.Sunburst:
				series.ChartType = SeriesChartType.Sunburst;
				break;
			default:
				series.ChartType = SeriesChartType.Pie;
				break;
			}
		}

		private void RenderDataPoint(ChartSeries chartSeries, ChartMember seriesGrouping, ChartMember categoryGrouping, SeriesInfo seriesInfo, bool dataPointShowsInLegend)
		{
			ChartDataPoint chartDataPoint = ((ReportElementCollectionBase<ChartDataPoint>)chartSeries)[categoryGrouping.MemberCellIndex];
			if (chartDataPoint != null)
			{
				DataPoint dataPoint = new DataPoint();
				int yValuesCount = ChartMapper.GetYValuesCount(seriesInfo.Series.ChartType);
				if (yValuesCount != 1)
				{
					dataPoint.YValues = new double[yValuesCount];
				}
				this.RenderDataPointValues(chartDataPoint, dataPoint, seriesInfo, categoryGrouping);
				Style style = chartDataPoint.Style;
				if (!dataPoint.Empty)
				{
					this.SetDataPointProperties(chartDataPoint, dataPoint);
					if (dataPointShowsInLegend && this.m_hatcher != null)
					{
						dataPoint.BackHatchStyle = this.m_hatcher.Current;
					}
					if (style != null)
					{
						this.RenderDataPointStyle(style, chartDataPoint.Instance.Style, dataPoint, seriesInfo, categoryGrouping.MemberCellIndex);
						if (!seriesInfo.IsDataPointHatchDefined)
						{
							seriesInfo.IsDataPointHatchDefined = MappingHelper.IsStylePropertyDefined(style.BackgroundHatchType);
						}
					}
					if (!seriesInfo.IsDataPointColorEmpty.HasValue || !seriesInfo.IsDataPointColorEmpty.Value)
					{
						seriesInfo.IsDataPointColorEmpty = (dataPoint.Color == Color.Empty);
					}
					this.RenderActionInfo(chartDataPoint.ActionInfo, dataPoint.ToolTip, dataPoint);
					this.RenderDataLabel(chartDataPoint.DataLabel, dataPoint, true);
					this.RenderDataPointMarker(chartDataPoint.Marker, dataPoint, ((List<DataPointBackgroundImageInfo>)seriesInfo.DataPointBackgroundImageInfoCollection)[categoryGrouping.MemberCellIndex].MarkerBackgroundImage);
					this.RenderItemInLegend(chartDataPoint.ItemInLegend, dataPoint, false);
					this.RenderCustomProperties(chartDataPoint.CustomProperties, dataPoint);
				}
				else if (!dataPointShowsInLegend && seriesInfo.DefaultDataPointAppearance == null)
				{
					seriesInfo.DefaultDataPointAppearance = new DataPoint();
					if (style != null)
					{
						this.RenderDefaultDataPointStyle(style, chartDataPoint.Instance.Style, seriesInfo.DefaultDataPointAppearance, seriesInfo, categoryGrouping.MemberCellIndex);
					}
					this.RenderDataPointMarker(chartDataPoint.Marker, seriesInfo.DefaultDataPointAppearance, ((List<DataPointBackgroundImageInfo>)seriesInfo.DataPointBackgroundImageInfoCollection)[categoryGrouping.MemberCellIndex].MarkerBackgroundImage);
					this.RenderItemInLegend(chartDataPoint.ItemInLegend, seriesInfo.DefaultDataPointAppearance, false);
				}
				seriesInfo.Series.Points.Add(dataPoint);
				if (dataPointShowsInLegend)
				{
					if (seriesInfo.IsExploded)
					{
						dataPoint.SetAttribute("Exploded", "true");
					}
					if (this.CanSetPieDataPointLegendText(seriesInfo.Series, dataPoint))
					{
						dataPoint.LegendText = this.GetDataPointLegendText(categoryGrouping, seriesGrouping);
					}
				}
				this.OnPostApplySeriesPointData(seriesInfo.Series, seriesInfo.Series.Points.Count - 1);
			}
		}

		private void RenderDefaultDataPointStyle(Style style, StyleInstance styleInstance, DataPoint dataPoint, SeriesInfo seriesInfo, int cellIndex)
		{
			this.RenderDataPointAttributesStyle(style, styleInstance, dataPoint, seriesInfo.IsLine);
			this.RenderDataPointAttributesGradient(style, styleInstance, dataPoint);
			this.RenderDataPointBackgroundImage(style.BackgroundImage, dataPoint, ((List<DataPointBackgroundImageInfo>)seriesInfo.DataPointBackgroundImageInfoCollection)[cellIndex].DataPointBackgroundImage);
		}

		public static int GetYValuesCount(SeriesChartType seriesType)
		{
			switch (seriesType)
			{
			default:
				return 1;
			case SeriesChartType.Bubble:
			case SeriesChartType.Range:
			case SeriesChartType.SplineRange:
			case SeriesChartType.Gantt:
			case SeriesChartType.RangeColumn:
			case SeriesChartType.PointAndFigure:
				return 2;
			case SeriesChartType.ErrorBar:
				return 3;
			case SeriesChartType.Stock:
			case SeriesChartType.Candlestick:
				return 4;
			case SeriesChartType.BoxPlot:
				return 6;
			}
		}

		private void SetDataPointProperties(ChartDataPoint chartDataPoint, DataPoint dataPoint)
		{
			if (chartDataPoint.AxisLabel != null)
			{
				object obj = null;
				obj = (chartDataPoint.AxisLabel.IsExpression ? chartDataPoint.Instance.AxisLabel : chartDataPoint.AxisLabel.Value);
				if (obj != null)
				{
					dataPoint.AxisLabel = this.GetFormattedValue(obj, "");
				}
			}
			ReportStringProperty toolTip = chartDataPoint.ToolTip;
			if (toolTip != null)
			{
				if (!toolTip.IsExpression)
				{
					if (toolTip.Value != null)
					{
						dataPoint.ToolTip = toolTip.Value;
					}
				}
				else
				{
					string toolTip2 = chartDataPoint.Instance.ToolTip;
					if (toolTip2 != null)
					{
						dataPoint.ToolTip = toolTip2;
					}
				}
			}
		}

		private void RenderDataPointValues(ChartDataPoint chartDataPoint, DataPoint dataPoint, SeriesInfo seriesInfo, ChartMember categoryGrouping)
		{
			if (chartDataPoint.DataPointValues != null)
			{
				this.SetDataPointXValue(chartDataPoint, dataPoint, seriesInfo, categoryGrouping);
				this.SetDataPointYValues(chartDataPoint, dataPoint, seriesInfo);
			}
		}

		private void SetDataPointYValue(DataPoint dataPoint, int index, object value, ref ChartValueTypes? dateTimeType)
		{
			if (index < dataPoint.YValues.Length)
			{
				dataPoint.YValues[index] = this.ConvertToDouble(value, ref dateTimeType);
			}
		}

		private void SetDataPointYValues(ChartDataPoint chartDataPoint, DataPoint dataPoint, SeriesInfo seriesInfo)
		{
			ChartValueTypes? nullable = null;
			if (chartDataPoint.DataPointValues.Y != null)
			{
				this.SetDataPointYValue(dataPoint, 0, chartDataPoint.DataPointValues.Instance.Y, ref nullable);
			}
			if (seriesInfo.IsBubble && chartDataPoint.DataPointValues.Size != null)
			{
				this.SetDataPointYValue(dataPoint, 1, chartDataPoint.DataPointValues.Instance.Size, ref nullable);
			}
			if (seriesInfo.IsRange)
			{
				if (chartDataPoint.DataPointValues.High != null)
				{
					this.SetDataPointYValue(dataPoint, this.GetHighYValueIndex(seriesInfo), chartDataPoint.DataPointValues.Instance.High, ref nullable);
				}
				if (chartDataPoint.DataPointValues.Low != null)
				{
					this.SetDataPointYValue(dataPoint, this.GetLowYValueIndex(seriesInfo), chartDataPoint.DataPointValues.Instance.Low, ref nullable);
				}
				if (chartDataPoint.DataPointValues.Start != null)
				{
					this.SetDataPointYValue(dataPoint, this.GetStartYValueIndex(seriesInfo), chartDataPoint.DataPointValues.Instance.Start, ref nullable);
				}
				if (chartDataPoint.DataPointValues.End != null)
				{
					this.SetDataPointYValue(dataPoint, this.GetEndYValueIndex(seriesInfo), chartDataPoint.DataPointValues.Instance.End, ref nullable);
				}
				if (chartDataPoint.DataPointValues.Mean != null)
				{
					this.SetDataPointYValue(dataPoint, 4, chartDataPoint.DataPointValues.Instance.Mean, ref nullable);
				}
				if (chartDataPoint.DataPointValues.Median != null)
				{
					this.SetDataPointYValue(dataPoint, 5, chartDataPoint.DataPointValues.Instance.Median, ref nullable);
				}
			}
			if (nullable.HasValue)
			{
				seriesInfo.Series.YValueType = nullable.Value;
			}
			int num = 0;
			while (true)
			{
				if (num < dataPoint.YValues.Length)
				{
					if (!double.IsNaN(dataPoint.YValues[num]))
					{
						num++;
						continue;
					}
					break;
				}
				return;
			}
			dataPoint.Empty = true;
			dataPoint.ShowLabelAsValue = false;
		}

		private int GetStartYValueIndex(SeriesInfo seriesInfo)
		{
			SeriesChartType chartType = seriesInfo.Series.ChartType;
			if (chartType == SeriesChartType.Gantt)
			{
				return 0;
			}
			return 2;
		}

		private int GetEndYValueIndex(SeriesInfo seriesInfo)
		{
			SeriesChartType chartType = seriesInfo.Series.ChartType;
			if (chartType == SeriesChartType.Gantt)
			{
				return 1;
			}
			return 3;
		}

		private int GetHighYValueIndex(SeriesInfo seriesInfo)
		{
			switch (seriesInfo.Series.ChartType)
			{
			case SeriesChartType.BoxPlot:
				return 1;
			case SeriesChartType.ErrorBar:
				return 2;
			default:
				return 0;
			}
		}

		private int GetLowYValueIndex(SeriesInfo seriesInfo)
		{
			SeriesChartType chartType = seriesInfo.Series.ChartType;
			if (chartType == SeriesChartType.BoxPlot)
			{
				return 0;
			}
			return 1;
		}

		private bool IsSeriesAttachedToScalarAxis(SeriesInfo seriesInfo)
		{
			if (seriesInfo.ChartAreaInfo.CategoryAxesScalar == null)
			{
				return false;
			}
			return seriesInfo.ChartAreaInfo.CategoryAxesScalar.Contains(this.GetSeriesCategoryAxisName(seriesInfo.ChartSeries));
		}

		private bool IsAxisAutoMargin(ChartAreaInfo chartAreaInfo, AspNetCore.Reporting.Chart.WebForms.Axis axis)
		{
			if (chartAreaInfo.CategoryAxesAutoMargin == null)
			{
				return false;
			}
			return chartAreaInfo.CategoryAxesAutoMargin.Contains(axis);
		}

		private void SetDataPointXValue(ChartDataPoint chartDataPoint, DataPoint dataPoint, SeriesInfo seriesInfo, ChartMember categoryGrouping)
		{
			if (chartDataPoint.DataPointValues.X == null && !seriesInfo.IsAttachedToScalarAxis)
			{
				return;
			}
			object obj = null;
			ChartValueTypes? nullable = null;
			if (chartDataPoint.DataPointValues.X != null)
			{
				obj = chartDataPoint.DataPointValues.Instance.X;
			}
			else if (categoryGrouping.Group != null && categoryGrouping.Group.Instance.GroupExpressions != null && categoryGrouping.Group.Instance.GroupExpressions.Count > 0)
			{
				obj = categoryGrouping.Group.Instance.GroupExpressions[0];
			}
			if (obj == null)
			{
				seriesInfo.NullXValuePoints.Add(dataPoint);
			}
			else
			{
				double num = this.ConvertToDouble(obj, ref nullable);
				if (!double.IsNaN(num))
				{
					dataPoint.XValue = num;
					seriesInfo.XValueSet = true;
				}
				else
				{
					seriesInfo.XValueSetFailed = true;
					if (this.DataPointShowsInLegend(seriesInfo.ChartSeries))
					{
						if (this.CanSetPieDataPointLegendText(seriesInfo.Series, dataPoint))
						{
							dataPoint.LegendText = this.GetFormattedValue(obj, "");
						}
					}
					else if (this.CanSetDataPointAxisLabel(seriesInfo.Series, dataPoint))
					{
						AspNetCore.Reporting.Chart.WebForms.ChartArea chartArea = null;
						try
						{
							chartArea = this.m_coreChart.ChartAreas[seriesInfo.Series.ChartArea];
						}
						catch (Exception ex)
						{
							if (AsynchronousExceptionDetection.IsStoppingException(ex))
							{
								throw;
							}
							Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
						}
						string format = (chartArea == null || seriesInfo.ChartCategoryAxis == null || seriesInfo.ChartCategoryAxis.Style == null || !MappingHelper.IsStylePropertyDefined(seriesInfo.ChartCategoryAxis.Style.Format)) ? "" : MappingHelper.GetStyleFormat(seriesInfo.ChartCategoryAxis.Style, seriesInfo.ChartCategoryAxis.Instance.Style);
						dataPoint.AxisLabel = this.GetFormattedValue(obj, format);
					}
				}
			}
			if (nullable.HasValue)
			{
				seriesInfo.Series.XValueType = nullable.Value;
			}
		}

		private void RenderDerivedSeriesCollecion()
		{
			if (this.m_chart.ChartData.DerivedSeriesCollection != null)
			{
				foreach (ChartDerivedSeries item in this.m_chart.ChartData.DerivedSeriesCollection)
				{
					this.RenderDerivedSeries(item);
				}
			}
		}

		private void RenderDerivedSeries(ChartDerivedSeries chartDerivedSeries)
		{
			ChartSeriesFormula derivedSeriesFormula = chartDerivedSeries.DerivedSeriesFormula;
			string sourceChartSeriesName = chartDerivedSeries.SourceChartSeriesName;
			if (sourceChartSeriesName != null && !(sourceChartSeriesName == ""))
			{
				this.GetSeriesInfo(sourceChartSeriesName);
				string text = "";
				if (chartDerivedSeries.Series != null)
				{
					text = chartDerivedSeries.Series.Name;
				}
				if (text == "")
				{
					text = FormulaHelper.GetDerivedSeriesName(sourceChartSeriesName);
				}
				string formulaParameters = default(string);
				string inputValues = default(string);
				string outputValues = default(string);
				bool startFromFirst = default(bool);
				FormulaHelper.RenderFormulaParameters(chartDerivedSeries.FormulaParameters, derivedSeriesFormula, sourceChartSeriesName, text, out formulaParameters, out inputValues, out outputValues, out startFromFirst);
				this.RenderDerivedSeriesProperties(chartDerivedSeries, text, sourceChartSeriesName, derivedSeriesFormula);
				this.ApplyFormula(derivedSeriesFormula, formulaParameters, inputValues, outputValues, startFromFirst);
			}
		}

		private void RenderDerivedSeriesProperties(ChartDerivedSeries chartDerivedSeries, string derivedSeriesName, string sourceSeriesName, ChartSeriesFormula formula)
		{
			if (chartDerivedSeries.Series != null)
			{
				Series series = new Series();
				series.Name = derivedSeriesName;
				SeriesInfo seriesInfo = this.GetSeriesInfo(sourceSeriesName);
				if (seriesInfo.DerivedSeries == null)
				{
					seriesInfo.DerivedSeries = new List<Series>();
				}
				seriesInfo.DerivedSeries.Add(series);
				this.RenderSeries(null, chartDerivedSeries.Series, series, this.IsSeriesLine(chartDerivedSeries.Series));
				if (FormulaHelper.ShouldSendDerivedSeriesBack(series.ChartType))
				{
					this.m_coreChart.Series.Insert(0, series);
				}
				else
				{
					this.m_coreChart.Series.Add(series);
				}
				series.ChartArea = this.GetSeriesChartAreaName(chartDerivedSeries.Series);
				if (series.ChartArea == "#NewChartArea")
				{
					Series series2 = seriesInfo.Series;
					if (series2 != null)
					{
						AspNetCore.Reporting.Chart.WebForms.ChartArea chartArea = this.GetChartArea(series2.ChartArea);
						if (chartArea != null)
						{
							AspNetCore.Reporting.Chart.WebForms.ChartArea chartArea2 = this.CreateNewChartArea(chartArea, !FormulaHelper.IsNewAreaRequired(formula));
							chartArea2.AlignWithChartArea = chartArea.Name;
							chartArea2.AlignType = AreaAlignTypes.All;
							series.ChartArea = chartArea2.Name;
						}
						else
						{
							series.ChartArea = series2.ChartArea;
						}
					}
				}
			}
		}

		private void AdjustSeriesInLegend(object sender, CustomizeLegendEventArgs e)
		{
			foreach (ChartAreaInfo value in this.m_chartAreaInfoDictionary.Values)
			{
				foreach (SeriesInfo seriesInfo in value.SeriesInfoList)
				{
					LegendItem seriesLegendItem = this.GetSeriesLegendItem(seriesInfo.Series, e.LegendItems);
					if (seriesLegendItem != null)
					{
						if (!this.DataPointShowsInLegend(seriesInfo.ChartSeries))
						{
							this.AdjustSeriesAppearanceInLegend(seriesInfo, seriesLegendItem);
						}
						if (seriesInfo.DerivedSeries != null)
						{
							this.AdjustDerivedSeriesInLegend(seriesInfo, seriesLegendItem, e.LegendItems);
						}
					}
				}
			}
		}

		private void AdjustSeriesAppearanceInLegend(SeriesInfo seriesInfo, LegendItem seriesLegendItem)
		{
			DataPoint dataPoint = this.GetFirstNonEmptyDataPoint(seriesInfo.Series);
			if (dataPoint == null)
			{
				if (seriesInfo.DefaultDataPointAppearance == null)
				{
					return;
				}
				dataPoint = seriesInfo.DefaultDataPointAppearance;
			}
			if (dataPoint.Color != Color.Empty)
			{
				seriesLegendItem.Color = dataPoint.Color;
			}
			if (dataPoint.BackGradientEndColor != Color.Empty)
			{
				seriesLegendItem.BackGradientEndColor = dataPoint.BackGradientEndColor;
			}
			if (dataPoint.BackGradientType != 0)
			{
				seriesLegendItem.BackGradientType = dataPoint.BackGradientType;
			}
			if (dataPoint.BackHatchStyle != 0)
			{
				seriesLegendItem.BackHatchStyle = dataPoint.BackHatchStyle;
			}
			if (dataPoint.BorderColor != Color.Empty)
			{
				seriesLegendItem.BorderColor = dataPoint.BorderColor;
			}
			if (dataPoint.BorderStyle != ChartMapper.m_defaultCoreDataPointBorderStyle)
			{
				seriesLegendItem.BorderStyle = dataPoint.BorderStyle;
			}
			if (dataPoint.BorderWidth != ChartMapper.m_defaultCoreDataPointBorderWidth)
			{
				seriesLegendItem.BorderWidth = dataPoint.BorderWidth;
			}
			if (dataPoint.BackImage != "")
			{
				seriesLegendItem.Image = dataPoint.BackImage;
			}
			if (dataPoint.BackImageTransparentColor != Color.Empty)
			{
				seriesLegendItem.BackImageTransparentColor = dataPoint.BackImageTransparentColor;
			}
			if (dataPoint.MarkerColor != Color.Empty)
			{
				seriesLegendItem.MarkerColor = dataPoint.MarkerColor;
			}
			if (dataPoint.MarkerBorderColor != Color.Empty)
			{
				seriesLegendItem.MarkerBorderColor = dataPoint.MarkerBorderColor;
			}
			if (dataPoint.MarkerBorderWidth != ChartMapper.m_defaultCoreDataPointBorderWidth)
			{
				seriesLegendItem.MarkerBorderWidth = dataPoint.MarkerBorderWidth;
			}
			seriesLegendItem.MarkerSize = dataPoint.MarkerSize;
			if (dataPoint.MarkerStyle != 0)
			{
				seriesLegendItem.MarkerStyle = dataPoint.MarkerStyle;
			}
			if (dataPoint.MarkerImage != "")
			{
				seriesLegendItem.MarkerImage = dataPoint.MarkerImage;
			}
			if (dataPoint.MarkerImageTransparentColor != Color.Empty)
			{
				seriesLegendItem.MarkerImageTransparentColor = dataPoint.MarkerImageTransparentColor;
			}
		}

		private DataPoint GetFirstNonEmptyDataPoint(Series series)
		{
			foreach (DataPoint point in series.Points)
			{
				if (!point.Empty)
				{
					return point;
				}
			}
			return null;
		}

		private void AdjustDerivedSeriesInLegend(SeriesInfo seriesInfo, LegendItem seriesLegendItem, LegendItemsCollection legendItems)
		{
			List<LegendItem> list = new List<LegendItem>();
			foreach (Series item in seriesInfo.DerivedSeries)
			{
				LegendItem seriesLegendItem2 = this.GetSeriesLegendItem(item, legendItems);
				if (seriesLegendItem2 != null)
				{
					list.Add(seriesLegendItem2);
					legendItems.Remove(seriesLegendItem2);
				}
			}
			int num = legendItems.IndexOf(seriesLegendItem);
			for (int i = 0; i < list.Count; i++)
			{
				num++;
				legendItems.Insert(num, list[i]);
			}
		}

		private LegendItem GetSeriesLegendItem(Series series, LegendItemsCollection legendItemCollection)
		{
			foreach (LegendItem item in legendItemCollection)
			{
				if (series.Name == item.SeriesName)
				{
					return item;
				}
			}
			return null;
		}

		private Series GetSeries(string seriesName)
		{
			try
			{
				return this.m_coreChart.Series[seriesName];
			}
			catch (Exception ex)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex))
				{
					throw;
				}
				Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
				return null;
			}
		}

		private SeriesInfo GetSeriesInfo(string seriesName)
		{
			foreach (ChartAreaInfo value in this.m_chartAreaInfoDictionary.Values)
			{
				foreach (SeriesInfo seriesInfo in value.SeriesInfoList)
				{
					if (seriesInfo.ChartSeries.Name == seriesName)
					{
						return seriesInfo;
					}
				}
			}
			return null;
		}

		private AspNetCore.Reporting.Chart.WebForms.ChartArea GetChartArea(string chartAreaName)
		{
			try
			{
				return this.m_coreChart.ChartAreas[chartAreaName];
			}
			catch
			{
				return null;
			}
		}

		private AspNetCore.Reporting.Chart.WebForms.ChartArea CreateNewChartArea(AspNetCore.Reporting.Chart.WebForms.ChartArea originalChartArea, bool copyYAxisProperties)
		{
			AspNetCore.Reporting.Chart.WebForms.ChartArea chartArea = new AspNetCore.Reporting.Chart.WebForms.ChartArea();
			this.m_coreChart.ChartAreas.Add(chartArea);
			if (originalChartArea != null)
			{
				chartArea.BackColor = originalChartArea.BackColor;
				chartArea.BackGradientEndColor = originalChartArea.BackGradientEndColor;
				chartArea.BackGradientType = originalChartArea.BackGradientType;
				chartArea.BackHatchStyle = originalChartArea.BackHatchStyle;
				chartArea.BorderColor = originalChartArea.BorderColor;
				chartArea.BorderStyle = originalChartArea.BorderStyle;
				chartArea.BorderWidth = originalChartArea.BorderWidth;
				chartArea.ShadowColor = originalChartArea.ShadowColor;
				chartArea.ShadowOffset = originalChartArea.ShadowOffset;
				for (int i = 0; i < originalChartArea.Axes.Length; i++)
				{
					AspNetCore.Reporting.Chart.WebForms.Axis axis = originalChartArea.Axes[i];
					AspNetCore.Reporting.Chart.WebForms.Axis axis2 = chartArea.Axes[i];
					Grid majorGrid = axis2.MajorGrid;
					Grid majorGrid2 = axis.MajorGrid;
					Grid minorGrid = axis2.MinorGrid;
					Grid minorGrid2 = axis.MinorGrid;
					TickMark majorTickMark = axis2.MajorTickMark;
					TickMark majorTickMark2 = axis.MajorTickMark;
					TickMark minorTickMark = axis2.MinorTickMark;
					TickMark minorTickMark2 = axis.MinorTickMark;
					majorGrid.LineColor = majorGrid2.LineColor;
					minorGrid.LineColor = minorGrid2.LineColor;
					majorTickMark.LineColor = majorTickMark2.LineColor;
					minorTickMark.LineColor = minorTickMark2.LineColor;
					majorGrid.LineStyle = majorGrid2.LineStyle;
					minorGrid.LineStyle = minorGrid2.LineStyle;
					majorTickMark.LineStyle = majorTickMark2.LineStyle;
					minorTickMark.LineStyle = minorTickMark2.LineStyle;
					majorGrid.LineWidth = majorGrid2.LineWidth;
					minorGrid.LineWidth = minorGrid2.LineWidth;
					majorTickMark.LineWidth = majorTickMark2.LineWidth;
					minorTickMark.LineWidth = minorTickMark2.LineWidth;
					majorGrid.Enabled = majorGrid2.Enabled;
					minorGrid.Enabled = minorGrid2.Enabled;
					majorTickMark.Enabled = majorTickMark2.Enabled;
					minorTickMark.Enabled = minorTickMark2.Enabled;
					axis2.StartFromZero = axis.StartFromZero;
					axis2.Margin = axis.Margin;
					axis2.Enabled = axis.Enabled;
				}
				this.CopyAxisRootProperties(originalChartArea.AxisX, chartArea.AxisX);
				if (copyYAxisProperties)
				{
					this.CopyAxisRootProperties(originalChartArea.AxisY, chartArea.AxisY);
				}
			}
			return chartArea;
		}

		private void CopyAxisRootProperties(AspNetCore.Reporting.Chart.WebForms.Axis source, AspNetCore.Reporting.Chart.WebForms.Axis target)
		{
			Label labelStyle = target.LabelStyle;
			Label labelStyle2 = source.LabelStyle;
			labelStyle.Font = labelStyle2.Font;
			labelStyle.FontAngle = labelStyle2.FontAngle;
			labelStyle.FontColor = labelStyle2.FontColor;
			labelStyle.Format = labelStyle2.Format;
			labelStyle.Interval = labelStyle2.Interval;
			labelStyle.IntervalOffset = labelStyle2.IntervalOffset;
			labelStyle.IntervalOffsetType = labelStyle2.IntervalOffsetType;
			labelStyle.IntervalType = labelStyle2.IntervalType;
			labelStyle.OffsetLabels = labelStyle2.OffsetLabels;
			labelStyle.ShowEndLabels = labelStyle2.ShowEndLabels;
			labelStyle.TruncatedLabels = labelStyle2.TruncatedLabels;
			Grid majorGrid = source.MajorGrid;
			Grid majorGrid2 = target.MajorGrid;
			if (majorGrid.Enabled)
			{
				majorGrid2.Interval = majorGrid.Interval;
				majorGrid2.IntervalOffset = majorGrid.IntervalOffset;
				majorGrid2.IntervalOffsetType = majorGrid.IntervalOffsetType;
				majorGrid2.IntervalType = majorGrid.IntervalType;
			}
			else
			{
				majorGrid2.Enabled = majorGrid.Enabled;
			}
			Grid minorGrid = source.MinorGrid;
			Grid minorGrid2 = target.MinorGrid;
			if (minorGrid.Enabled)
			{
				minorGrid2.Interval = minorGrid.Interval;
				minorGrid2.IntervalOffset = minorGrid.IntervalOffset;
				minorGrid2.IntervalOffsetType = minorGrid.IntervalOffsetType;
				minorGrid2.IntervalType = minorGrid.IntervalType;
			}
			else
			{
				minorGrid2.Enabled = minorGrid.Enabled;
			}
			TickMark majorTickMark = source.MajorTickMark;
			TickMark majorTickMark2 = target.MajorTickMark;
			if (majorTickMark.Enabled)
			{
				majorTickMark2.Interval = majorTickMark.Interval;
				majorTickMark2.IntervalOffset = majorTickMark.IntervalOffset;
				majorTickMark2.IntervalOffsetType = majorTickMark.IntervalOffsetType;
				majorTickMark2.IntervalType = majorTickMark.IntervalType;
				majorTickMark2.Size = majorTickMark.Size;
				majorTickMark2.Style = majorTickMark.Style;
			}
			else
			{
				majorTickMark2.Enabled = majorTickMark.Enabled;
			}
			TickMark minorTickMark = source.MinorTickMark;
			TickMark minorTickMark2 = target.MinorTickMark;
			if (minorTickMark.Enabled)
			{
				minorTickMark2.Interval = minorTickMark.Interval;
				minorTickMark2.IntervalOffset = minorTickMark.IntervalOffset;
				minorTickMark2.IntervalOffsetType = minorTickMark.IntervalOffsetType;
				minorTickMark2.IntervalType = minorTickMark.IntervalType;
				minorTickMark2.Size = minorTickMark.Size;
				minorTickMark2.Style = minorTickMark.Style;
			}
			else
			{
				minorTickMark2.Enabled = minorTickMark.Enabled;
			}
			target.Arrows = source.Arrows;
			target.Crossing = source.Crossing;
			target.Interlaced = source.Interlaced;
			target.InterlacedColor = source.InterlacedColor;
			target.Interval = source.Interval;
			target.IntervalAutoMode = source.IntervalAutoMode;
			target.IntervalOffset = source.IntervalOffset;
			target.IntervalOffsetType = source.IntervalOffsetType;
			target.IntervalType = source.IntervalType;
			target.LabelsAutoFitMaxFontSize = source.LabelsAutoFitMaxFontSize;
			target.LabelsAutoFitMinFontSize = source.LabelsAutoFitMinFontSize;
			target.LabelsAutoFitStyle = source.LabelsAutoFitStyle;
			target.LabelsAutoFit = source.LabelsAutoFit;
			target.LineColor = source.LineColor;
			target.LineStyle = source.LineStyle;
			target.LineWidth = source.LineWidth;
			target.LogarithmBase = source.LogarithmBase;
			target.Logarithmic = source.Logarithmic;
			target.MarksNextToAxis = source.MarksNextToAxis;
			target.Reverse = source.Reverse;
			target.ScaleBreakStyle = source.ScaleBreakStyle;
			target.ValueType = source.ValueType;
		}

		private void ApplyFormula(ChartSeriesFormula formula, string formulaParameters, string inputValues, string outputValues, bool startFromFirst)
		{
			if (formula == ChartSeriesFormula.Mean || formula == ChartSeriesFormula.Median)
			{
				double num = 0.0;
				num = ((formula != ChartSeriesFormula.Mean) ? this.m_coreChart.DataManipulator.Statistics.Median(inputValues) : this.m_coreChart.DataManipulator.Statistics.Mean(inputValues));
				Series series = this.GetSeries(inputValues);
				Series series2 = this.GetSeries(outputValues);
				if (series != null && series2 != null)
				{
					foreach (DataPoint point in series.Points)
					{
						DataPoint dataPoint2 = new DataPoint();
						dataPoint2.XValue = point.XValue;
						dataPoint2.YValues = new double[point.YValues.Length];
						point.YValues.CopyTo(dataPoint2.YValues, 0);
						dataPoint2.AxisLabel = point.AxisLabel;
						if (dataPoint2.YValues.Length > 0)
						{
							dataPoint2.YValues[0] = num;
							series2.Points.Add(dataPoint2);
						}
					}
				}
			}
			else
			{
				this.m_coreChart.DataManipulator.StartFromFirst = startFromFirst;
				this.m_coreChart.DataManipulator.FormulaFinancial(this.GetFinancialFormula(formula), formulaParameters, inputValues, outputValues);
			}
		}

		private FinancialFormula GetFinancialFormula(ChartSeriesFormula formula)
		{
			switch (formula)
			{
			case ChartSeriesFormula.BollingerBands:
				return FinancialFormula.BollingerBands;
			case ChartSeriesFormula.DetrendedPriceOscillator:
				return FinancialFormula.DetrendedPriceOscillator;
			case ChartSeriesFormula.Envelopes:
				return FinancialFormula.Envelopes;
			case ChartSeriesFormula.ExponentialMovingAverage:
				return FinancialFormula.ExponentialMovingAverage;
			case ChartSeriesFormula.MACD:
				return FinancialFormula.MACD;
			case ChartSeriesFormula.MovingAverage:
				return FinancialFormula.MovingAverage;
			case ChartSeriesFormula.Performance:
				return FinancialFormula.Performance;
			case ChartSeriesFormula.RateOfChange:
				return FinancialFormula.RateOfChange;
			case ChartSeriesFormula.RelativeStrengthIndex:
				return FinancialFormula.RelativeStrengthIndex;
			case ChartSeriesFormula.StandardDeviation:
				return FinancialFormula.StandardDeviation;
			case ChartSeriesFormula.TriangularMovingAverage:
				return FinancialFormula.TriangularMovingAverage;
			case ChartSeriesFormula.TRIX:
				return FinancialFormula.TRIX;
			default:
				return FinancialFormula.WeightedMovingAverage;
			}
		}

		private void PostProcessData()
		{
			foreach (KeyValuePair<string, ChartAreaInfo> item in this.m_chartAreaInfoDictionary)
			{
				this.AdjustChartAreaData(item);
				this.AdjustAxesMargin(item);
			}
		}

		private void AdjustAxesMargin(KeyValuePair<string, ChartAreaInfo> chartAreaInfoKeyPair)
		{
			AspNetCore.Reporting.Chart.WebForms.ChartArea chartArea = this.GetChartArea(chartAreaInfoKeyPair.Key);
			ChartAreaInfo value = chartAreaInfoKeyPair.Value;
			if (chartArea != null)
			{
				List<AspNetCore.Reporting.Chart.WebForms.Axis> categoryAxesAutoMargin = value.CategoryAxesAutoMargin;
				if (categoryAxesAutoMargin != null)
				{
					foreach (AspNetCore.Reporting.Chart.WebForms.Axis item in categoryAxesAutoMargin)
					{
						if (item.Enabled == AxisEnabled.True)
						{
							if (item == chartArea.AxisX2)
							{
								item.Margin = chartArea.AxisX.Margin;
							}
							else if (item == chartArea.AxisX)
							{
								item.Margin = chartArea.AxisX2.Margin;
							}
						}
					}
				}
			}
		}

		private void AdjustChartAreaData(KeyValuePair<string, ChartAreaInfo> chartAreaInfo)
		{
			ChartAreaInfo value = chartAreaInfo.Value;
			bool flag = this.IsXValueSet(value);
			bool flag2 = this.IsXValueSetFailed(value);
			if (flag && flag2)
			{
				foreach (SeriesInfo seriesInfo in value.SeriesInfoList)
				{
					this.ClearSeriesXValues(seriesInfo.Series);
				}
			}
			else if (flag && !this.HasStackedSeries(value))
			{
				foreach (SeriesInfo seriesInfo2 in value.SeriesInfoList)
				{
					this.ClearNullXValues(seriesInfo2);
				}
			}
			if ((!flag || !flag2) && flag)
			{
				return;
			}
			try
			{
				AspNetCore.Reporting.Chart.WebForms.ChartArea chartArea = this.m_coreChart.ChartAreas[chartAreaInfo.Key];
				chartArea.AxisX.Logarithmic = false;
				chartArea.AxisX2.Logarithmic = false;
			}
			catch (Exception ex)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex))
				{
					throw;
				}
				Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
			}
		}

		private void AddSeriesToDictionary(SeriesInfo seriesInfo)
		{
			string chartArea = seriesInfo.Series.ChartArea;
			if (!this.m_chartAreaInfoDictionary.ContainsKey(chartArea))
			{
				this.m_chartAreaInfoDictionary.Add(chartArea, new ChartAreaInfo());
			}
			seriesInfo.ChartAreaInfo = this.m_chartAreaInfoDictionary[chartArea];
			if (this.m_chart.ChartAreas != null)
			{
				ChartArea byName = this.m_chart.ChartAreas.GetByName(chartArea);
				if (byName != null && byName.CategoryAxes != null)
				{
					seriesInfo.ChartCategoryAxis = byName.CategoryAxes.GetByName(this.GetSeriesCategoryAxisName(seriesInfo.ChartSeries));
				}
			}
			this.m_chartAreaInfoDictionary[chartArea].SeriesInfoList.Add(seriesInfo);
		}

		private void ClearNullXValues(SeriesInfo seriesInfo)
		{
			foreach (DataPoint nullXValuePoint in seriesInfo.NullXValuePoints)
			{
				seriesInfo.Series.Points.Remove(nullXValuePoint);
			}
			seriesInfo.NullXValuePoints.Clear();
		}

		private void ClearSeriesXValues(Series series)
		{
			foreach (DataPoint point in series.Points)
			{
				point.AxisLabel = "";
				point.XValue = 0.0;
			}
		}

		public override void Dispose()
		{
			if (this.m_coreChart != null)
			{
				this.m_coreChart.Dispose();
			}
			this.m_coreChart = null;
			base.Dispose();
		}

		private void OnPostInitialize()
		{
		}

		private void OnPostApplySeriesPointData(Series series, int index)
		{
		}

		private void OnPostApplySeriesData(Series series)
		{
		}

		private void OnPostApplyData()
		{
		}

		private double ConvertToDouble(object value)
		{
			ChartValueTypes? nullable = null;
			return this.ConvertToDouble(value, false, ref nullable);
		}

		private double ConvertToDouble(object value, bool checkForMaxMinValue)
		{
			ChartValueTypes? nullable = null;
			return this.ConvertToDouble(value, checkForMaxMinValue, ref nullable);
		}

		private double ConvertToDouble(object value, ref ChartValueTypes? dateTimeType)
		{
			return this.ConvertToDouble(value, false, ref dateTimeType);
		}

		private double ConvertToDouble(object value, bool checkForMaxMinValue, ref ChartValueTypes? dateTimeType)
		{
			if (value == null)
			{
				return double.NaN;
			}
			switch (Type.GetTypeCode(value.GetType()))
			{
			case TypeCode.Byte:
				return (double)(int)(byte)value;
			case TypeCode.Char:
				return (double)(int)(char)value;
			case TypeCode.Decimal:
				return decimal.ToDouble((decimal)value);
			case TypeCode.Double:
				return (double)value;
			case TypeCode.Int16:
				return (double)(short)value;
			case TypeCode.Int32:
				return (double)(int)value;
			case TypeCode.Int64:
				return (double)(long)value;
			case TypeCode.SByte:
				return (double)(sbyte)value;
			case TypeCode.Single:
				return (double)(float)value;
			case TypeCode.UInt16:
				return (double)(int)(ushort)value;
			case TypeCode.UInt32:
				return (double)(uint)value;
			case TypeCode.UInt64:
				return (double)(ulong)value;
			case TypeCode.DateTime:
				dateTimeType = ChartValueTypes.DateTime;
				return ChartMapper.ConvertDateTimeToDouble((DateTime)value);
			case TypeCode.String:
			{
				string text = value.ToString().Trim();
				double result = default(double);
				if (double.TryParse(text, out result))
				{
					return result;
				}
				if (checkForMaxMinValue)
				{
					if (text == "MaxValue")
					{
						return 1.7976931348623157E+308;
					}
					if (text == "MinValue")
					{
						return -1.7976931348623157E+308;
					}
				}
				DateTimeOffset dateTimeOffset = default(DateTimeOffset);
				bool flag = default(bool);
				if (!DateTimeUtil.TryParseDateTime(text, (CultureInfo)null, out dateTimeOffset, out flag))
				{
					break;
				}
				if (flag)
				{
					return this.ConvertToDouble((object)dateTimeOffset, checkForMaxMinValue, ref dateTimeType);
				}
				return this.ConvertToDouble((object)dateTimeOffset.DateTime, checkForMaxMinValue, ref dateTimeType);
			}
			}
			if (value is DateTimeOffset)
			{
				dateTimeType = ChartValueTypes.DateTimeOffset;
				return ChartMapper.ConvertDateTimeToDouble(((DateTimeOffset)value).UtcDateTime);
			}
			if (value is TimeSpan)
			{
				dateTimeType = ChartValueTypes.Time;
				return ChartMapper.ConvertDateTimeToDouble(DateTime.MinValue + (TimeSpan)value);
			}
			return double.NaN;
		}

		private static double ConvertDateTimeToDouble(DateTime dateTime)
		{
			return dateTime.ToOADate();
		}

		private string GetFormattedValue(object value, string format)
		{
			if (this.m_formatter == null)
			{
				this.m_formatter = new Formatter(this.m_chart.ChartDef.StyleClass, this.m_chart.RenderingContext.OdpContext, ObjectType.Chart, this.m_chart.Name);
			}
			Type type = value.GetType();
			TypeCode typeCode = Type.GetTypeCode(type);
			if (typeCode == TypeCode.Object && value is DateTimeOffset)
			{
				typeCode = TypeCode.DateTime;
			}
			return this.m_formatter.FormatValue(value, format, typeCode);
		}

		private BreakLineType GetScaleBreakLineType(ChartBreakLineType chartBreakLineType)
		{
			switch (chartBreakLineType)
			{
			case ChartBreakLineType.None:
				return BreakLineType.None;
			case ChartBreakLineType.Straight:
				return BreakLineType.Straight;
			case ChartBreakLineType.Wave:
				return BreakLineType.Wave;
			default:
				return BreakLineType.Ragged;
			}
		}

		private AutoBool GetAutoBool(ChartAutoBool autoBool)
		{
			switch (autoBool)
			{
			case ChartAutoBool.True:
				return AutoBool.True;
			case ChartAutoBool.False:
				return AutoBool.False;
			default:
				return AutoBool.Auto;
			}
		}

		private AxisEnabled GetAxisEnabled(ChartAutoBool autoBool)
		{
			switch (autoBool)
			{
			case ChartAutoBool.True:
				return AxisEnabled.True;
			case ChartAutoBool.False:
				return AxisEnabled.False;
			default:
				return AxisEnabled.Auto;
			}
		}

		private bool GetMargin(ChartAutoBool autoBool)
		{
			if (autoBool == ChartAutoBool.False)
			{
				return false;
			}
			return true;
		}

		private bool DoesSeriesRequireMargin(ChartSeries chartSeries)
		{
			ChartSeriesType seriesType = this.GetSeriesType(chartSeries);
			ChartSeriesSubtype seriesSubType = this.GetSeriesSubType(chartSeries);
			switch (seriesType)
			{
			case ChartSeriesType.Range:
				if (seriesSubType != 0 && seriesSubType != ChartSeriesSubtype.Smooth)
				{
					break;
				}
				goto case ChartSeriesType.Area;
			case ChartSeriesType.Area:
				return false;
			}
			return true;
		}

		private StringAlignment GetStringAlignmentFromTextAlignments(TextAlignments value)
		{
			switch (value)
			{
			case TextAlignments.Center:
				return StringAlignment.Center;
			case TextAlignments.Right:
				return StringAlignment.Far;
			default:
				return StringAlignment.Near;
			}
		}

		private StringAlignment GetStringAlignmentFromVericalAlignments(VerticalAlignments value)
		{
			switch (value)
			{
			case VerticalAlignments.Middle:
				return StringAlignment.Center;
			case VerticalAlignments.Bottom:
				return StringAlignment.Far;
			default:
				return StringAlignment.Near;
			}
		}

		private string GetSeriesLegendText(ChartMember seriesGrouping)
		{
			return this.GetGroupingLegendText(seriesGrouping);
		}

		private string GetDataPointLegendText(ChartMember categoryGrouping, ChartMember seriesGrouping)
		{
			string text = "";
			if (this.m_multiColumn)
			{
				text = this.GetGroupingLegendText(categoryGrouping);
			}
			if (this.m_multiRow)
			{
				if (text != "")
				{
					text += ChartMapper.m_legendTextSeparator;
				}
				text += this.GetGroupingLegendText(seriesGrouping);
			}
			return text;
		}

		private string GetGroupingLegendText(ChartMember grouping)
		{
			ChartMember chartMember = grouping;
			string text = "";
			do
			{
				string text2 = this.GetGroupingLabel(chartMember);
				if (chartMember.Children != null && text2 != "" && text != "")
				{
					text2 += ChartMapper.m_legendTextSeparator;
				}
				text = text.Insert(0, text2);
				chartMember = chartMember.Parent;
			}
			while (chartMember != null);
			return text;
		}

		private string GetGroupingLabel(ChartMember grouping)
		{
			if (grouping.Instance.Label == null)
			{
				return "";
			}
			return grouping.Instance.Label;
		}

		private string GetFormattedGroupingLabel(ChartMember categoryGrouping, string chartAreaName, ChartAxis categoryAxis)
		{
			object labelObject = categoryGrouping.Instance.LabelObject;
			if (labelObject == null)
			{
				return " ";
			}
			AspNetCore.Reporting.Chart.WebForms.ChartArea chartArea = this.GetChartArea(chartAreaName);
			string format = (chartArea == null || categoryAxis == null || categoryAxis.Style == null || !MappingHelper.IsStylePropertyDefined(categoryAxis.Style.Format)) ? "" : MappingHelper.GetStyleFormat(categoryAxis.Style, categoryAxis.Instance.Style);
			return this.GetFormattedValue(labelObject, format);
		}

		private int GetGroupingLevel(ChartMember grouping)
		{
			int num = -1;
			if (grouping.Children != null)
			{
				foreach (ChartMember child in grouping.Children)
				{
					int groupingLevel = this.GetGroupingLevel(child);
					if (num < groupingLevel)
					{
						num = groupingLevel;
					}
				}
			}
			return num + 1;
		}

		private bool IsChartEmpty()
		{
			foreach (Series item in this.m_coreChart.Series)
			{
				if (item.Points.Count > 0)
				{
					return false;
				}
			}
			return true;
		}

		private bool DataPointShowsInLegend(ChartSeries chartSeries)
		{
			if (this.GetSeriesType(chartSeries) != ChartSeriesType.Shape)
			{
				return false;
			}
			ChartSeriesSubtype seriesSubType = this.GetSeriesSubType(chartSeries);
			if (seriesSubType != ChartSeriesSubtype.TreeMap && seriesSubType != ChartSeriesSubtype.Sunburst)
			{
				return true;
			}
			return false;
		}

		private bool IsSeriesCollectedPie(Series series)
		{
			if (series.ChartType != SeriesChartType.Pie && series.ChartType != SeriesChartType.Doughnut)
			{
				return false;
			}
			if (((DataPointAttributes)series)["CollectedStyle"] == null)
			{
				return false;
			}
			return AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(((DataPointAttributes)series)["CollectedStyle"], "CollectedPie");
		}

		private bool IsSeriesPareto(Series series)
		{
			if (series.ChartType != SeriesChartType.Column)
			{
				return false;
			}
			if (((DataPointAttributes)series)["ShowColumnAs"] == null)
			{
				return false;
			}
			return AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(((DataPointAttributes)series)["ShowColumnAs"], "pareto", true) == 0;
		}

		private bool IsSeriesHistogram(Series series)
		{
			if (series.ChartType != SeriesChartType.Column)
			{
				return false;
			}
			if (((DataPointAttributes)series)["ShowColumnAs"] == null)
			{
				return false;
			}
			return AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(((DataPointAttributes)series)["ShowColumnAs"], "histogram", true) == 0;
		}

		private bool IsGradientPerDataPointSupported(ChartSeries chartSeries)
		{
			ChartSeriesType seriesType = this.GetSeriesType(chartSeries);
			switch (seriesType)
			{
			case ChartSeriesType.Area:
				return false;
			case ChartSeriesType.Range:
			{
				ChartSeriesSubtype validSeriesSubType = this.GetValidSeriesSubType(seriesType, this.GetSeriesSubType(chartSeries));
				if (validSeriesSubType != 0 && validSeriesSubType != ChartSeriesSubtype.Smooth)
				{
					break;
				}
				return false;
			}
			}
			return true;
		}

		private bool IsSeriesStacked(ChartSeries chartSeries)
		{
			ChartSeriesSubtype validSeriesSubType = this.GetValidSeriesSubType(this.GetSeriesType(chartSeries), this.GetSeriesSubType(chartSeries));
			if (validSeriesSubType != ChartSeriesSubtype.Stacked)
			{
				return validSeriesSubType == ChartSeriesSubtype.PercentStacked;
			}
			return true;
		}

		private bool IsSeriesLine(ChartSeries chartSeries)
		{
			return this.GetSeriesType(chartSeries) == ChartSeriesType.Line;
		}

		private bool IsSeriesRange(ChartSeries chartSeries)
		{
			return this.GetSeriesType(chartSeries) == ChartSeriesType.Range;
		}

		private bool IsSeriesBubble(ChartSeries chartSeries)
		{
			return this.GetValidSeriesSubType(this.GetSeriesType(chartSeries), this.GetSeriesSubType(chartSeries)) == ChartSeriesSubtype.Bubble;
		}

		private bool IsSeriesExploded(ChartSeries chartSeries)
		{
			ChartSeriesSubtype validSeriesSubType = this.GetValidSeriesSubType(this.GetSeriesType(chartSeries), this.GetSeriesSubType(chartSeries));
			if (validSeriesSubType != ChartSeriesSubtype.ExplodedDoughnut)
			{
				return validSeriesSubType == ChartSeriesSubtype.ExplodedPie;
			}
			return true;
		}

		private bool CanSetCategoryGroupingLabels(ChartAreaInfo seriesInfoList)
		{
			bool flag = this.IsXValueSet(seriesInfoList);
			if (flag && this.IsXValueSetFailed(seriesInfoList))
			{
				return true;
			}
			return !flag;
		}

		private bool CanSetPieDataPointLegendText(Series series, DataPoint dataPoint)
		{
			if (!dataPoint.Empty)
			{
				return dataPoint.LegendText == string.Empty;
			}
			return series.EmptyPointStyle.LegendText == string.Empty;
		}

		private bool CanSetDataPointAxisLabel(Series series, DataPoint dataPoint)
		{
			if (!dataPoint.Empty)
			{
				return dataPoint.AxisLabel == string.Empty;
			}
			return series.EmptyPointStyle.AxisLabel == string.Empty;
		}

		private bool IsXValueSet(ChartAreaInfo seriesInfoList)
		{
			foreach (SeriesInfo seriesInfo in seriesInfoList.SeriesInfoList)
			{
				if (seriesInfo.XValueSet)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsXValueSetFailed(ChartAreaInfo seriesInfoList)
		{
			foreach (SeriesInfo seriesInfo in seriesInfoList.SeriesInfoList)
			{
				if (seriesInfo.XValueSetFailed)
				{
					return true;
				}
			}
			return false;
		}

		private bool HasStackedSeries(ChartAreaInfo seriesInfoList)
		{
			foreach (SeriesInfo seriesInfo in seriesInfoList.SeriesInfoList)
			{
				if (this.IsSeriesStacked(seriesInfo.ChartSeries))
				{
					return true;
				}
			}
			return false;
		}

		private GradientType GetGradientType(BackgroundGradients chartGradientType)
		{
			switch (chartGradientType)
			{
			case BackgroundGradients.Center:
				return GradientType.Center;
			case BackgroundGradients.DiagonalLeft:
				return GradientType.DiagonalLeft;
			case BackgroundGradients.DiagonalRight:
				return GradientType.DiagonalRight;
			case BackgroundGradients.HorizontalCenter:
				return GradientType.HorizontalCenter;
			case BackgroundGradients.LeftRight:
				return GradientType.LeftRight;
			case BackgroundGradients.TopBottom:
				return GradientType.TopBottom;
			case BackgroundGradients.VerticalCenter:
				return GradientType.VerticalCenter;
			default:
				return GradientType.None;
			}
		}

		private ChartHatchStyle GetHatchType(BackgroundHatchTypes chartHatchType)
		{
			switch (chartHatchType)
			{
			case BackgroundHatchTypes.BackwardDiagonal:
				return ChartHatchStyle.BackwardDiagonal;
			case BackgroundHatchTypes.Cross:
				return ChartHatchStyle.Cross;
			case BackgroundHatchTypes.DarkDownwardDiagonal:
				return ChartHatchStyle.DarkDownwardDiagonal;
			case BackgroundHatchTypes.DarkHorizontal:
				return ChartHatchStyle.DarkHorizontal;
			case BackgroundHatchTypes.DarkUpwardDiagonal:
				return ChartHatchStyle.DarkUpwardDiagonal;
			case BackgroundHatchTypes.DarkVertical:
				return ChartHatchStyle.DarkVertical;
			case BackgroundHatchTypes.DashedDownwardDiagonal:
				return ChartHatchStyle.DashedDownwardDiagonal;
			case BackgroundHatchTypes.DashedHorizontal:
				return ChartHatchStyle.DashedHorizontal;
			case BackgroundHatchTypes.DashedUpwardDiagonal:
				return ChartHatchStyle.DashedUpwardDiagonal;
			case BackgroundHatchTypes.DashedVertical:
				return ChartHatchStyle.DashedVertical;
			case BackgroundHatchTypes.DiagonalBrick:
				return ChartHatchStyle.DiagonalBrick;
			case BackgroundHatchTypes.DiagonalCross:
				return ChartHatchStyle.DiagonalCross;
			case BackgroundHatchTypes.Divot:
				return ChartHatchStyle.Divot;
			case BackgroundHatchTypes.DottedDiamond:
				return ChartHatchStyle.DottedDiamond;
			case BackgroundHatchTypes.DottedGrid:
				return ChartHatchStyle.DottedGrid;
			case BackgroundHatchTypes.ForwardDiagonal:
				return ChartHatchStyle.ForwardDiagonal;
			case BackgroundHatchTypes.Horizontal:
				return ChartHatchStyle.Horizontal;
			case BackgroundHatchTypes.HorizontalBrick:
				return ChartHatchStyle.HorizontalBrick;
			case BackgroundHatchTypes.LargeCheckerBoard:
				return ChartHatchStyle.LargeCheckerBoard;
			case BackgroundHatchTypes.LargeConfetti:
				return ChartHatchStyle.LargeConfetti;
			case BackgroundHatchTypes.LargeGrid:
				return ChartHatchStyle.LargeGrid;
			case BackgroundHatchTypes.LightDownwardDiagonal:
				return ChartHatchStyle.LightDownwardDiagonal;
			case BackgroundHatchTypes.LightHorizontal:
				return ChartHatchStyle.LightHorizontal;
			case BackgroundHatchTypes.LightUpwardDiagonal:
				return ChartHatchStyle.LightUpwardDiagonal;
			case BackgroundHatchTypes.LightVertical:
				return ChartHatchStyle.LightVertical;
			case BackgroundHatchTypes.NarrowHorizontal:
				return ChartHatchStyle.NarrowHorizontal;
			case BackgroundHatchTypes.OutlinedDiamond:
				return ChartHatchStyle.OutlinedDiamond;
			case BackgroundHatchTypes.Percent05:
				return ChartHatchStyle.Percent05;
			case BackgroundHatchTypes.Percent10:
				return ChartHatchStyle.Percent10;
			case BackgroundHatchTypes.Percent20:
				return ChartHatchStyle.Percent20;
			case BackgroundHatchTypes.Percent25:
				return ChartHatchStyle.Percent25;
			case BackgroundHatchTypes.Percent30:
				return ChartHatchStyle.Percent30;
			case BackgroundHatchTypes.Percent40:
				return ChartHatchStyle.Percent40;
			case BackgroundHatchTypes.Percent50:
				return ChartHatchStyle.Percent50;
			case BackgroundHatchTypes.Percent60:
				return ChartHatchStyle.Percent60;
			case BackgroundHatchTypes.Percent70:
				return ChartHatchStyle.Percent70;
			case BackgroundHatchTypes.Percent75:
				return ChartHatchStyle.Percent75;
			case BackgroundHatchTypes.Percent80:
				return ChartHatchStyle.Percent80;
			case BackgroundHatchTypes.Percent90:
				return ChartHatchStyle.Percent90;
			case BackgroundHatchTypes.Plaid:
				return ChartHatchStyle.Plaid;
			case BackgroundHatchTypes.Shingle:
				return ChartHatchStyle.Shingle;
			case BackgroundHatchTypes.SmallCheckerBoard:
				return ChartHatchStyle.SmallCheckerBoard;
			case BackgroundHatchTypes.SmallConfetti:
				return ChartHatchStyle.SmallConfetti;
			case BackgroundHatchTypes.SmallGrid:
				return ChartHatchStyle.SmallGrid;
			case BackgroundHatchTypes.SolidDiamond:
				return ChartHatchStyle.SolidDiamond;
			case BackgroundHatchTypes.Sphere:
				return ChartHatchStyle.Sphere;
			case BackgroundHatchTypes.Trellis:
				return ChartHatchStyle.Trellis;
			case BackgroundHatchTypes.Vertical:
				return ChartHatchStyle.Vertical;
			case BackgroundHatchTypes.Wave:
				return ChartHatchStyle.Wave;
			case BackgroundHatchTypes.Weave:
				return ChartHatchStyle.Weave;
			case BackgroundHatchTypes.WideDownwardDiagonal:
				return ChartHatchStyle.WideDownwardDiagonal;
			case BackgroundHatchTypes.WideUpwardDiagonal:
				return ChartHatchStyle.WideUpwardDiagonal;
			case BackgroundHatchTypes.ZigZag:
				return ChartHatchStyle.ZigZag;
			default:
				return ChartHatchStyle.None;
			}
		}

		private ChartDashStyle GetBorderStyle(BorderStyles chartBorderStyle, bool isLine)
		{
			switch (chartBorderStyle)
			{
			case BorderStyles.DashDot:
				return ChartDashStyle.DashDot;
			case BorderStyles.DashDotDot:
				return ChartDashStyle.DashDotDot;
			case BorderStyles.Dashed:
				return ChartDashStyle.Dash;
			case BorderStyles.Dotted:
				return ChartDashStyle.Dot;
			case BorderStyles.None:
				return ChartDashStyle.NotSet;
			case BorderStyles.Solid:
				return ChartDashStyle.Solid;
			default:
				if (isLine)
				{
					return ChartDashStyle.Solid;
				}
				return ChartDashStyle.NotSet;
			}
		}

		private ChartImageWrapMode GetBackImageMode(BackgroundRepeatTypes backgroundImageRepeatType)
		{
			switch (backgroundImageRepeatType)
			{
			case BackgroundRepeatTypes.Repeat:
				return ChartImageWrapMode.Tile;
			case BackgroundRepeatTypes.Clip:
				return ChartImageWrapMode.Unscaled;
			default:
				return ChartImageWrapMode.Scaled;
			}
		}

		private ChartImageAlign GetBackImageAlign(Positions position)
		{
			switch (position)
			{
			case Positions.Bottom:
				return ChartImageAlign.Bottom;
			case Positions.BottomLeft:
				return ChartImageAlign.BottomLeft;
			case Positions.BottomRight:
				return ChartImageAlign.BottomRight;
			case Positions.Center:
				return ChartImageAlign.Center;
			case Positions.Left:
				return ChartImageAlign.Left;
			case Positions.Right:
				return ChartImageAlign.Right;
			case Positions.Top:
				return ChartImageAlign.Top;
			case Positions.TopLeft:
				return ChartImageAlign.TopLeft;
			case Positions.TopRight:
				return ChartImageAlign.TopRight;
			default:
				return ChartImageAlign.TopLeft;
			}
		}

		private LabelOutsidePlotAreaStyle GetLabelOutsidePlotAreaStyle(ChartAllowOutsideChartArea chartAllowOutsideChartArea)
		{
			switch (chartAllowOutsideChartArea)
			{
			case ChartAllowOutsideChartArea.False:
				return LabelOutsidePlotAreaStyle.No;
			case ChartAllowOutsideChartArea.True:
				return LabelOutsidePlotAreaStyle.Yes;
			default:
				return LabelOutsidePlotAreaStyle.Partial;
			}
		}

		private DateTimeIntervalType GetDateTimeIntervalType(ChartIntervalType chartIntervalType)
		{
			switch (chartIntervalType)
			{
			case ChartIntervalType.Days:
				return DateTimeIntervalType.Days;
			case ChartIntervalType.Hours:
				return DateTimeIntervalType.Hours;
			case ChartIntervalType.Milliseconds:
				return DateTimeIntervalType.Milliseconds;
			case ChartIntervalType.Minutes:
				return DateTimeIntervalType.Minutes;
			case ChartIntervalType.Months:
				return DateTimeIntervalType.Months;
			case ChartIntervalType.Number:
				return DateTimeIntervalType.Number;
			case ChartIntervalType.Seconds:
				return DateTimeIntervalType.Seconds;
			case ChartIntervalType.Weeks:
				return DateTimeIntervalType.Weeks;
			case ChartIntervalType.Years:
				return DateTimeIntervalType.Years;
			case ChartIntervalType.Default:
				return DateTimeIntervalType.Auto;
			default:
				return DateTimeIntervalType.Auto;
			}
		}

		private IntervalAutoMode GetIntervalAutoMode(bool variableAutoInterval)
		{
			if (!variableAutoInterval)
			{
				return IntervalAutoMode.FixedCount;
			}
			return IntervalAutoMode.VariableCount;
		}

		private AspNetCore.Reporting.Chart.WebForms.TickMarkStyle GetTickMarkStyle(ChartTickMarksType chartTickMarksType)
		{
			switch (chartTickMarksType)
			{
			case ChartTickMarksType.Cross:
				return AspNetCore.Reporting.Chart.WebForms.TickMarkStyle.Cross;
			case ChartTickMarksType.Inside:
				return AspNetCore.Reporting.Chart.WebForms.TickMarkStyle.Inside;
			case ChartTickMarksType.Outside:
				return AspNetCore.Reporting.Chart.WebForms.TickMarkStyle.Outside;
			default:
				return AspNetCore.Reporting.Chart.WebForms.TickMarkStyle.None;
			}
		}
	}
}
