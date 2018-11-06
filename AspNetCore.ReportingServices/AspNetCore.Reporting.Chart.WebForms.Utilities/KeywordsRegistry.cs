using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace AspNetCore.Reporting.Chart.WebForms.Utilities
{
	internal class KeywordsRegistry : IServiceProvider
	{
		private IServiceContainer serviceContainer;

		internal ArrayList registeredKeywords = new ArrayList();

		private KeywordsRegistry()
		{
		}

		public KeywordsRegistry(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(SR.ExceptionInvalidServiceContainer);
			}
			this.serviceContainer = container;
			this.RegisterKeywords();
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(KeywordsRegistry))
			{
				return this;
			}
			throw new ArgumentException(SR.ExceptionKeywordsRegistryUnsupportedType(serviceType.ToString()));
		}

		private void RegisterKeywords()
		{
			string appliesToProperties = "Text,Label,ToolTip,Href,LabelToolTip,MapAreaAttributes,AxisLabel,LegendToolTip,LegendMapAreaAttributes,LegendHref,LegendText";
			this.Register(SR.DescriptionKeyWordNameIndexDataPoint, "#INDEX", string.Empty, SR.DescriptionKeyWordIndexDataPoint2, "DataPoint", appliesToProperties, false, false);
			this.Register(SR.DescriptionKeyWordNameXValue, "#VALX", string.Empty, SR.DescriptionKeyWordXValue, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, true, false);
			this.Register(SR.DescriptionKeyWordNameYValue, "#VAL", string.Empty, SR.DescriptionKeyWordYValue, "Series,DataPoint,Annotation,LegendCellColumn,LegendCellColumn", appliesToProperties, true, true);
			this.Register(SR.DescriptionKeyWordNameTotalYValues, "#TOTAL", string.Empty, SR.DescriptionKeyWordTotalYValues, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, true, false);
			this.Register(SR.DescriptionKeyWordNameYValuePercentTotal, "#PERCENT", string.Empty, SR.DescriptionKeyWordYValuePercentTotal, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, true, true);
			this.Register(SR.DescriptionKeyWordNameIndexTheDataPoint, "#INDEX", string.Empty, SR.DescriptionKeyWordIndexDataPoint, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, false, false);
			this.Register(SR.DescriptionKeyWordNameLabelDataPoint, "#LABEL", string.Empty, SR.DescriptionKeyWordLabelDataPoint, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, false, false);
			this.Register(SR.DescriptionKeyWordNameAxisLabelDataPoint, "#AXISLABEL", string.Empty, SR.DescriptionKeyWordAxisLabelDataPoint, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, false, false);
			this.Register(SR.DescriptionKeyWordNameLegendText, "#LEGENDTEXT", string.Empty, SR.DescriptionKeyWordLegendText, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, false, false);
			this.Register(SR.DescriptionKeyWordNameSeriesName, "#SERIESNAME", "#SER", SR.DescriptionKeyWordSeriesName, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, false, false);
			this.Register(SR.DescriptionKeyWordNameAverageYValues, "#AVG", string.Empty, SR.DescriptionKeyWordAverageYValues, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, true, true);
			this.Register(SR.DescriptionKeyWordNameMaximumYValues, "#MAX", string.Empty, SR.DescriptionKeyWordMaximumYValues, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, true, true);
			this.Register(SR.DescriptionKeyWordNameMinimumYValues, "#MIN", string.Empty, SR.DescriptionKeyWordMinimumYValues, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, true, true);
			this.Register(SR.DescriptionKeyWordNameLastPointYValue, "#LAST", string.Empty, SR.DescriptionKeyWordLastPointYValue, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, true, true);
			this.Register(SR.DescriptionKeyWordNameFirstPointYValue, "#FIRST", string.Empty, SR.DescriptionKeyWordFirstPointYValue, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, true, true);
		}

		public void Register(string name, string keyword, string keywordAliases, string description, string appliesToTypes, string appliesToProperties, bool supportsFormatting, bool supportsValueIndex)
		{
			KeywordInfo value = new KeywordInfo(name, keyword, keywordAliases, description, appliesToTypes, appliesToProperties, supportsFormatting, supportsValueIndex);
			this.registeredKeywords.Add(value);
		}
	}
}
