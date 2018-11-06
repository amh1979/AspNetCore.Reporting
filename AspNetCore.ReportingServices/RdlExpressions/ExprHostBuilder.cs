using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Globalization;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace AspNetCore.ReportingServices.RdlExpressions
{
	internal sealed class ExprHostBuilder
	{
		internal enum ErrorSource
		{
			Expression,
			CodeModuleClassInstanceDecl,
			CustomCode,
			Unknown
		}

		internal enum DataRegionMode
		{
			Tablix,
			Chart,
			GaugePanel,
			CustomReportItem,
			MapDataRegion,
			DataShape
		}

		private static class Constants
		{
			internal const string ReportObjectModelNS = "AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel";

			internal const string ExprHostObjectModelNS = "AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel";

			internal const string ReportExprHost = "ReportExprHost";

			internal const string IndexedExprHost = "IndexedExprHost";

			internal const string ReportParamExprHost = "ReportParamExprHost";

			internal const string CalcFieldExprHost = "CalcFieldExprHost";

			internal const string DataSourceExprHost = "DataSourceExprHost";

			internal const string DataSetExprHost = "DataSetExprHost";

			internal const string ReportItemExprHost = "ReportItemExprHost";

			internal const string ActionExprHost = "ActionExprHost";

			internal const string ActionInfoExprHost = "ActionInfoExprHost";

			internal const string TextBoxExprHost = "TextBoxExprHost";

			internal const string ImageExprHost = "ImageExprHost";

			internal const string ParamExprHost = "ParamExprHost";

			internal const string SubreportExprHost = "SubreportExprHost";

			internal const string SortExprHost = "SortExprHost";

			internal const string FilterExprHost = "FilterExprHost";

			internal const string GroupExprHost = "GroupExprHost";

			internal const string StyleExprHost = "StyleExprHost";

			internal const string AggregateParamExprHost = "AggregateParamExprHost";

			internal const string LookupExprHost = "LookupExprHost";

			internal const string LookupDestExprHost = "LookupDestExprHost";

			internal const string ReportSectionExprHost = "ReportSectionExprHost";

			internal const string JoinConditionExprHost = "JoinConditionExprHost";

			internal const string IncludeParametersParam = "includeParameters";

			internal const string ParametersOnlyParam = "parametersOnly";

			internal const string CustomCodeProxy = "CustomCodeProxy";

			internal const string CustomCodeProxyBase = "CustomCodeProxyBase";

			internal const string ReportObjectModelParam = "reportObjectModel";

			internal const string SetReportObjectModel = "SetReportObjectModel";

			internal const string Code = "Code";

			internal const string CodeProxyBase = "m_codeProxyBase";

			internal const string CodeParam = "code";

			internal const string Report = "Report";

			internal const string RemoteArrayWrapper = "RemoteArrayWrapper";

			internal const string RemoteMemberArrayWrapper = "RemoteMemberArrayWrapper";

			internal const string LabelExpr = "LabelExpr";

			internal const string ValueExpr = "ValueExpr";

			internal const string NoRowsExpr = "NoRowsExpr";

			internal const string ParameterHosts = "m_parameterHostsRemotable";

			internal const string IndexParam = "index";

			internal const string FilterHosts = "m_filterHostsRemotable";

			internal const string SortHost = "m_sortHost";

			internal const string GroupHost = "m_groupHost";

			internal const string VisibilityHiddenExpr = "VisibilityHiddenExpr";

			internal const string SortDirectionHosts = "SortDirectionHosts";

			internal const string DataValueHosts = "m_dataValueHostsRemotable";

			internal const string CustomPropertyHosts = "m_customPropertyHostsRemotable";

			internal const string VariableValueHosts = "VariableValueHosts";

			internal const string ReportLanguageExpr = "ReportLanguageExpr";

			internal const string AutoRefreshExpr = "AutoRefreshExpr";

			internal const string AggregateParamHosts = "m_aggregateParamHostsRemotable";

			internal const string ReportParameterHosts = "m_reportParameterHostsRemotable";

			internal const string DataSourceHosts = "m_dataSourceHostsRemotable";

			internal const string DataSetHosts = "m_dataSetHostsRemotable";

			internal const string PageSectionHosts = "m_pageSectionHostsRemotable";

			internal const string PageHosts = "m_pageHostsRemotable";

			internal const string ReportSectionHosts = "m_reportSectionHostsRemotable";

			internal const string LineHosts = "m_lineHostsRemotable";

			internal const string RectangleHosts = "m_rectangleHostsRemotable";

			internal const string TextBoxHosts = "m_textBoxHostsRemotable";

			internal const string ImageHosts = "m_imageHostsRemotable";

			internal const string SubreportHosts = "m_subreportHostsRemotable";

			internal const string TablixHosts = "m_tablixHostsRemotable";

			internal const string ChartHosts = "m_chartHostsRemotable";

			internal const string GaugePanelHosts = "m_gaugePanelHostsRemotable";

			internal const string CustomReportItemHosts = "m_customReportItemHostsRemotable";

			internal const string LookupExprHosts = "m_lookupExprHostsRemotable";

			internal const string LookupDestExprHosts = "m_lookupDestExprHostsRemotable";

			internal const string ReportInitialPageName = "InitialPageNameExpr";

			internal const string ConnectStringExpr = "ConnectStringExpr";

			internal const string FieldHosts = "m_fieldHostsRemotable";

			internal const string QueryParametersHost = "QueryParametersHost";

			internal const string QueryCommandTextExpr = "QueryCommandTextExpr";

			internal const string JoinConditionHosts = "m_joinConditionExprHostsRemotable";

			internal const string PromptExpr = "PromptExpr";

			internal const string ValidValuesHost = "ValidValuesHost";

			internal const string ValidValueLabelsHost = "ValidValueLabelsHost";

			internal const string ValidationExpressionExpr = "ValidationExpressionExpr";

			internal const string ActionInfoHost = "ActionInfoHost";

			internal const string ActionHost = "ActionHost";

			internal const string ActionItemHosts = "m_actionItemHostsRemotable";

			internal const string BookmarkExpr = "BookmarkExpr";

			internal const string ToolTipExpr = "ToolTipExpr";

			internal const string ToggleImageInitialStateExpr = "ToggleImageInitialStateExpr";

			internal const string UserSortExpressionsHost = "UserSortExpressionsHost";

			internal const string MIMETypeExpr = "MIMETypeExpr";

			internal const string TagExpr = "TagExpr";

			internal const string OmitExpr = "OmitExpr";

			internal const string HyperlinkExpr = "HyperlinkExpr";

			internal const string DrillThroughReportNameExpr = "DrillThroughReportNameExpr";

			internal const string DrillThroughParameterHosts = "m_drillThroughParameterHostsRemotable";

			internal const string DrillThroughBookmakLinkExpr = "DrillThroughBookmarkLinkExpr";

			internal const string BookmarkLinkExpr = "BookmarkLinkExpr";

			internal const string FilterExpressionExpr = "FilterExpressionExpr";

			internal const string ParentExpressionsHost = "ParentExpressionsHost";

			internal const string ReGroupExpressionsHost = "ReGroupExpressionsHost";

			internal const string DataValueExprHost = "DataValueExprHost";

			internal const string DataValueNameExpr = "DataValueNameExpr";

			internal const string DataValueValueExpr = "DataValueValueExpr";

			internal const string TablixExprHost = "TablixExprHost";

			internal const string DataShapeExprHost = "DataShapeExprHost";

			internal const string ChartExprHost = "ChartExprHost";

			internal const string GaugePanelExprHost = "GaugePanelExprHost";

			internal const string CustomReportItemExprHost = "CustomReportItemExprHost";

			internal const string MapDataRegionExprHost = "MapDataRegionExprHost";

			internal const string TablixMemberExprHost = "TablixMemberExprHost";

			internal const string DataShapeMemberExprHost = "DataShapeMemberExprHost";

			internal const string ChartMemberExprHost = "ChartMemberExprHost";

			internal const string GaugeMemberExprHost = "GaugeMemberExprHost";

			internal const string DataGroupExprHost = "DataGroupExprHost";

			internal const string TablixCellExprHost = "TablixCellExprHost";

			internal const string DataShapeIntersectionExprHost = "DataShapeIntersectionExprHost";

			internal const string ChartDataPointExprHost = "ChartDataPointExprHost";

			internal const string GaugeCellExprHost = "GaugeCellExprHost";

			internal const string DataCellExprHost = "DataCellExprHost";

			internal const string MemberTreeHosts = "m_memberTreeHostsRemotable";

			internal const string DataCellHosts = "m_cellHostsRemotable";

			internal const string MapMemberExprHost = "MapMemberExprHost";

			internal const string TablixCornerCellHosts = "m_cornerCellHostsRemotable";

			internal const string ChartTitleExprHost = "ChartTitleExprHost";

			internal const string ChartAxisTitleExprHost = "ChartAxisTitleExprHost";

			internal const string ChartLegendTitleExprHost = "ChartLegendTitleExprHost";

			internal const string ChartLegendExprHost = "ChartLegendExprHost";

			internal const string ChartTitleHost = "TitleHost";

			internal const string ChartNoDataMessageHost = "NoDataMessageHost";

			internal const string ChartLegendTitleHost = "TitleExprHost";

			internal const string PaletteExpr = "PaletteExpr";

			internal const string PaletteHatchBehaviorExpr = "PaletteHatchBehaviorExpr";

			internal const string ChartAreaExprHost = "ChartAreaExprHost";

			internal const string ChartNoMoveDirectionsExprHost = "ChartNoMoveDirectionsExprHost";

			internal const string ChartNoMoveDirectionsHost = "NoMoveDirectionsHost";

			internal const string UpExpr = "UpExpr";

			internal const string DownExpr = "DownExpr";

			internal const string LeftExpr = "LeftExpr";

			internal const string RightExpr = "RightExpr";

			internal const string UpLeftExpr = "UpLeftExpr";

			internal const string UpRightExpr = "UpRightExpr";

			internal const string DownLeftExpr = "DownLeftExpr";

			internal const string DownRightExpr = "DownRightExpr";

			internal const string ChartSmartLabelExprHost = "ChartSmartLabelExprHost";

			internal const string ChartSmartLabelHost = "SmartLabelHost";

			internal const string AllowOutSidePlotAreaExpr = "AllowOutSidePlotAreaExpr";

			internal const string CalloutBackColorExpr = "CalloutBackColorExpr";

			internal const string CalloutLineAnchorExpr = "CalloutLineAnchorExpr";

			internal const string CalloutLineColorExpr = "CalloutLineColorExpr";

			internal const string CalloutLineStyleExpr = "CalloutLineStyleExpr";

			internal const string CalloutLineWidthExpr = "CalloutLineWidthExpr";

			internal const string CalloutStyleExpr = "CalloutStyleExpr";

			internal const string HideOverlappedExpr = "HideOverlappedExpr";

			internal const string MarkerOverlappingExpr = "MarkerOverlappingExpr";

			internal const string MaxMovingDistanceExpr = "MaxMovingDistanceExpr";

			internal const string MinMovingDistanceExpr = "MinMovingDistanceExpr";

			internal const string DisabledExpr = "DisabledExpr";

			internal const string ChartAxisScaleBreakExprHost = "ChartAxisScaleBreakExprHost";

			internal const string ChartAxisScaleBreakHost = "AxisScaleBreakHost";

			internal const string ChartBorderSkinExprHost = "ChartBorderSkinExprHost";

			internal const string ChartBorderSkinHost = "BorderSkinHost";

			internal const string TitleSeparatorExpr = "TitleSeparatorExpr";

			internal const string ColumnTypeExpr = "ColumnTypeExpr";

			internal const string MinimumWidthExpr = "MinimumWidthExpr";

			internal const string MaximumWidthExpr = "MaximumWidthExpr";

			internal const string SeriesSymbolWidthExpr = "SeriesSymbolWidthExpr";

			internal const string SeriesSymbolHeightExpr = "SeriesSymbolHeightExpr";

			internal const string CellTypeExpr = "CellTypeExpr";

			internal const string TextExpr = "TextExpr";

			internal const string CellSpanExpr = "CellSpanExpr";

			internal const string ImageWidthExpr = "ImageWidthExpr";

			internal const string ImageHeightExpr = "ImageHeightExpr";

			internal const string SymbolHeightExpr = "SymbolHeightExpr";

			internal const string SymbolWidthExpr = "SymbolWidthExpr";

			internal const string AlignmentExpr = "AlignmentExpr";

			internal const string TopMarginExpr = "TopMarginExpr";

			internal const string BottomMarginExpr = "BottomMarginExpr";

			internal const string LeftMarginExpr = "LeftMarginExpr";

			internal const string RightMarginExpr = "RightMarginExpr";

			internal const string VisibleExpr = "VisibleExpr";

			internal const string MarginExpr = "MarginExpr";

			internal const string IntervalExpr = "IntervalExpr";

			internal const string IntervalTypeExpr = "IntervalTypeExpr";

			internal const string IntervalOffsetExpr = "IntervalOffsetExpr";

			internal const string IntervalOffsetTypeExpr = "IntervalOffsetTypeExpr";

			internal const string MarksAlwaysAtPlotEdgeExpr = "MarksAlwaysAtPlotEdgeExpr";

			internal const string ReverseExpr = "ReverseExpr";

			internal const string LocationExpr = "LocationExpr";

			internal const string InterlacedExpr = "InterlacedExpr";

			internal const string InterlacedColorExpr = "InterlacedColorExpr";

			internal const string LogScaleExpr = "LogScaleExpr";

			internal const string LogBaseExpr = "LogBaseExpr";

			internal const string HideLabelsExpr = "HideLabelsExpr";

			internal const string AngleExpr = "AngleExpr";

			internal const string ArrowsExpr = "ArrowsExpr";

			internal const string PreventFontShrinkExpr = "PreventFontShrinkExpr";

			internal const string PreventFontGrowExpr = "PreventFontGrowExpr";

			internal const string PreventLabelOffsetExpr = "PreventLabelOffsetExpr";

			internal const string PreventWordWrapExpr = "PreventWordWrapExpr";

			internal const string AllowLabelRotationExpr = "AllowLabelRotationExpr";

			internal const string IncludeZeroExpr = "IncludeZeroExpr";

			internal const string LabelsAutoFitDisabledExpr = "LabelsAutoFitDisabledExpr";

			internal const string MinFontSizeExpr = "MinFontSizeExpr";

			internal const string MaxFontSizeExpr = "MaxFontSizeExpr";

			internal const string OffsetLabelsExpr = "OffsetLabelsExpr";

			internal const string HideEndLabelsExpr = "HideEndLabelsExpr";

			internal const string ChartTickMarksExprHost = "ChartTickMarksExprHost";

			internal const string ChartTickMarksHost = "TickMarksHost";

			internal const string ChartGridLinesExprHost = "ChartGridLinesExprHost";

			internal const string ChartGridLinesHost = "GridLinesHost";

			internal const string ChartDataPointInLegendExprHost = "ChartDataPointInLegendExprHost";

			internal const string ChartDataPointInLegendHost = "DataPointInLegendHost";

			internal const string ChartEmptyPointsExprHost = "ChartEmptyPointsExprHost";

			internal const string ChartEmptyPointsHost = "EmptyPointsHost";

			internal const string AxisLabelExpr = "AxisLabelExpr";

			internal const string LegendTextExpr = "LegendTextExpr";

			internal const string ChartLegendColumnHeaderExprHost = "ChartLegendColumnHeaderExprHost";

			internal const string ChartLegendColumnHeaderHost = "ChartLegendColumnHeaderHost";

			internal const string ChartCustomPaletteColorExprHost = "ChartCustomPaletteColorExprHost";

			internal const string ChartCustomPaletteColorHosts = "m_customPaletteColorHostsRemotable";

			internal const string ChartLegendCustomItemCellExprHost = "ChartLegendCustomItemCellExprHost";

			internal const string ChartLegendCustomItemCellsHosts = "m_legendCustomItemCellHostsRemotable";

			internal const string ChartDerivedSeriesExprHost = "ChartDerivedSeriesExprHost";

			internal const string ChartDerivedSeriesCollectionHosts = "m_derivedSeriesCollectionHostsRemotable";

			internal const string SourceChartSeriesNameExpr = "SourceChartSeriesNameExpr";

			internal const string DerivedSeriesFormulaExpr = "DerivedSeriesFormulaExpr";

			internal const string SizeExpr = "SizeExpr";

			internal const string TypeExpr = "TypeExpr";

			internal const string SubtypeExpr = "SubtypeExpr";

			internal const string LegendNameExpr = "LegendNameExpr";

			internal const string ChartAreaNameExpr = "ChartAreaNameExpr";

			internal const string ValueAxisNameExpr = "ValueAxisNameExpr";

			internal const string CategoryAxisNameExpr = "CategoryAxisNameExpr";

			internal const string ChartStripLineExprHost = "ChartStripLineExprHost";

			internal const string ChartStripLinesHosts = "m_stripLinesHostsRemotable";

			internal const string ChartSeriesExprHost = "ChartSeriesExprHost";

			internal const string ChartSeriesHost = "ChartSeriesHost";

			internal const string TitleExpr = "TitleExpr";

			internal const string TitleAngleExpr = "TitleAngleExpr";

			internal const string StripWidthExpr = "StripWidthExpr";

			internal const string StripWidthTypeExpr = "StripWidthTypeExpr";

			internal const string HiddenExpr = "HiddenExpr";

			internal const string ChartFormulaParameterExprHost = "ChartFormulaParameterExprHost";

			internal const string ChartFormulaParametersHosts = "m_formulaParametersHostsRemotable";

			internal const string ChartLegendColumnExprHost = "ChartLegendColumnExprHost";

			internal const string ChartLegendColumnsHosts = "m_legendColumnsHostsRemotable";

			internal const string ChartLegendCustomItemExprHost = "ChartLegendCustomItemExprHost";

			internal const string ChartLegendCustomItemsHosts = "m_legendCustomItemsHostsRemotable";

			internal const string SeparatorExpr = "SeparatorExpr";

			internal const string SeparatorColorExpr = "SeparatorColorExpr";

			internal const string ChartValueAxesHosts = "m_valueAxesHostsRemotable";

			internal const string ChartCategoryAxesHosts = "m_categoryAxesHostsRemotable";

			internal const string ChartTitlesHosts = "m_titlesHostsRemotable";

			internal const string ChartLegendsHosts = "m_legendsHostsRemotable";

			internal const string ChartAreasHosts = "m_chartAreasHostsRemotable";

			internal const string ChartAxisExprHost = "ChartAxisExprHost";

			internal const string MemberLabelExpr = "MemberLabelExpr";

			internal const string MemberStyleHost = "MemberStyleHost";

			internal const string DataLabelStyleHost = "DataLabelStyleHost";

			internal const string StyleHost = "StyleHost";

			internal const string MarkerStyleHost = "MarkerStyleHost";

			internal const string CaptionExpr = "CaptionExpr";

			internal const string CategoryAxisHost = "CategoryAxisHost";

			internal const string PlotAreaHost = "PlotAreaHost";

			internal const string AxisMinExpr = "AxisMinExpr";

			internal const string AxisMaxExpr = "AxisMaxExpr";

			internal const string AxisCrossAtExpr = "AxisCrossAtExpr";

			internal const string AxisMajorIntervalExpr = "AxisMajorIntervalExpr";

			internal const string AxisMinorIntervalExpr = "AxisMinorIntervalExpr";

			internal const string ChartDataPointValueXExpr = "DataPointValuesXExpr";

			internal const string ChartDataPointValueYExpr = "DataPointValuesYExpr";

			internal const string ChartDataPointValueSizeExpr = "DataPointValuesSizeExpr";

			internal const string ChartDataPointValueHighExpr = "DataPointValuesHighExpr";

			internal const string ChartDataPointValueLowExpr = "DataPointValuesLowExpr";

			internal const string ChartDataPointValueStartExpr = "DataPointValuesStartExpr";

			internal const string ChartDataPointValueEndExpr = "DataPointValuesEndExpr";

			internal const string ChartDataPointValueMeanExpr = "DataPointValuesMeanExpr";

			internal const string ChartDataPointValueMedianExpr = "DataPointValuesMedianExpr";

			internal const string ChartDataPointValueHighlightXExpr = "DataPointValuesHighlightXExpr";

			internal const string ChartDataPointValueHighlightYExpr = "DataPointValuesHighlightYExpr";

			internal const string ChartDataPointValueHighlightSizeExpr = "DataPointValuesHighlightSizeExpr";

			internal const string ChartDataPointValueFormatXExpr = "ChartDataPointValueFormatXExpr";

			internal const string ChartDataPointValueFormatYExpr = "ChartDataPointValueFormatYExpr";

			internal const string ChartDataPointValueFormatSizeExpr = "ChartDataPointValueFormatSizeExpr";

			internal const string ChartDataPointValueCurrencyLanguageXExpr = "ChartDataPointValueCurrencyLanguageXExpr";

			internal const string ChartDataPointValueCurrencyLanguageYExpr = "ChartDataPointValueCurrencyLanguageYExpr";

			internal const string ChartDataPointValueCurrencyLanguageSizeExpr = "ChartDataPointValueCurrencyLanguageSizeExpr";

			internal const string ColorExpr = "ColorExpr";

			internal const string BorderSkinTypeExpr = "BorderSkinTypeExpr";

			internal const string LengthExpr = "LengthExpr";

			internal const string EnabledExpr = "EnabledExpr";

			internal const string BreakLineTypeExpr = "BreakLineTypeExpr";

			internal const string CollapsibleSpaceThresholdExpr = "CollapsibleSpaceThresholdExpr";

			internal const string MaxNumberOfBreaksExpr = "MaxNumberOfBreaksExpr";

			internal const string SpacingExpr = "SpacingExpr";

			internal const string AxesViewExpr = "AxesViewExpr";

			internal const string CursorExpr = "CursorExpr";

			internal const string InnerPlotPositionExpr = "InnerPlotPositionExpr";

			internal const string ChartAlignTypePositionExpr = "ChartAlignTypePositionExpr";

			internal const string EquallySizedAxesFontExpr = "EquallySizedAxesFontExpr";

			internal const string AlignOrientationExpr = "AlignOrientationExpr";

			internal const string Chart3DPropertiesExprHost = "Chart3DPropertiesExprHost";

			internal const string Chart3DPropertiesHost = "Chart3DPropertiesHost";

			internal const string LayoutExpr = "LayoutExpr";

			internal const string DockOutsideChartAreaExpr = "DockOutsideChartAreaExpr";

			internal const string TitleExprHost = "TitleExprHost";

			internal const string AutoFitTextDisabledExpr = "AutoFitTextDisabledExpr";

			internal const string HeaderSeparatorExpr = "HeaderSeparatorExpr";

			internal const string HeaderSeparatorColorExpr = "HeaderSeparatorColorExpr";

			internal const string ColumnSeparatorExpr = "ColumnSeparatorExpr";

			internal const string ColumnSeparatorColorExpr = "ColumnSeparatorColorExpr";

			internal const string ColumnSpacingExpr = "ColumnSpacingExpr";

			internal const string InterlacedRowsExpr = "InterlacedRowsExpr";

			internal const string InterlacedRowsColorExpr = "InterlacedRowsColorExpr";

			internal const string EquallySpacedItemsExpr = "EquallySpacedItemsExpr";

			internal const string ReversedExpr = "ReversedExpr";

			internal const string MaxAutoSizeExpr = "MaxAutoSizeExpr";

			internal const string TextWrapThresholdExpr = "TextWrapThresholdExpr";

			internal const string DockingExpr = "DockingExpr";

			internal const string ChartTitlePositionExpr = "ChartTitlePositionExpr";

			internal const string DockingOffsetExpr = "DockingOffsetExpr";

			internal const string ChartLegendPositionExpr = "ChartLegendPositionExpr";

			internal const string DockOutsideChartArea = "DockOutsideChartArea";

			internal const string AutoFitTextDisabled = "AutoFitTextDisabled";

			internal const string MinFontSize = "MinFontSize";

			internal const string HeaderSeparator = "HeaderSeparator";

			internal const string HeaderSeparatorColor = "HeaderSeparatorColor";

			internal const string ColumnSeparator = "ColumnSeparator";

			internal const string ColumnSeparatorColor = "ColumnSeparatorColor";

			internal const string ColumnSpacing = "ColumnSpacing";

			internal const string InterlacedRows = "InterlacedRows";

			internal const string InterlacedRowsColor = "InterlacedRowsColor";

			internal const string EquallySpacedItems = "EquallySpacedItems";

			internal const string HideInLegendExpr = "HideInLegendExpr";

			internal const string ShowOverlappedExpr = "ShowOverlappedExpr";

			internal const string MajorChartTickMarksHost = "MajorTickMarksHost";

			internal const string MinorChartTickMarksHost = "MinorTickMarksHost";

			internal const string MajorChartGridLinesHost = "MajorGridLinesHost";

			internal const string MinorChartGridLinesHost = "MinorGridLinesHost";

			internal const string RotationExpr = "RotationExpr";

			internal const string ProjectionModeExpr = "ProjectionModeExpr";

			internal const string InclinationExpr = "InclinationExpr";

			internal const string PerspectiveExpr = "PerspectiveExpr";

			internal const string DepthRatioExpr = "DepthRatioExpr";

			internal const string ShadingExpr = "ShadingExpr";

			internal const string GapDepthExpr = "GapDepthExpr";

			internal const string WallThicknessExpr = "WallThicknessExpr";

			internal const string ClusteredExpr = "ClusteredExpr";

			internal const string ChartDataLabelExprHost = "ChartDataLabelExprHost";

			internal const string ChartDataLabelPositionExpr = "ChartDataLabelPositionExpr";

			internal const string UseValueAsLabelExpr = "UseValueAsLabelExpr";

			internal const string ChartDataLabelHost = "DataLabelHost";

			internal const string ChartMarkerExprHost = "ChartMarkerExprHost";

			internal const string ChartMarkerHost = "ChartMarkerHost";

			internal const string VariableAutoIntervalExpr = "VariableAutoIntervalExpr";

			internal const string LabelIntervalExpr = "LabelIntervalExpr";

			internal const string LabelIntervalTypeExpr = "LabelIntervalTypeExpr";

			internal const string LabelIntervalOffsetExpr = "LabelIntervalOffsetExpr";

			internal const string LabelIntervalOffsetTypeExpr = "LabelIntervalOffsetTypeExpr";

			internal const string DynamicWidthExpr = "DynamicWidthExpr";

			internal const string DynamicHeightExpr = "DynamicHeightExpr";

			internal const string TextOrientationExpr = "TextOrientationExpr";

			internal const string ChartElementPositionExprHost = "ChartElementPositionExprHost";

			internal const string ChartElementPositionHost = "ChartElementPositionHost";

			internal const string ChartInnerPlotPositionHost = "ChartInnerPlotPositionHost";

			internal const string BaseGaugeImageExprHost = "BaseGaugeImageExprHost";

			internal const string BaseGaugeImageHost = "BaseGaugeImageHost";

			internal const string SourceExpr = "SourceExpr";

			internal const string TransparentColorExpr = "TransparentColorExpr";

			internal const string CapImageExprHost = "CapImageExprHost";

			internal const string CapImageHost = "CapImageHost";

			internal const string TopImageHost = "TopImageHost";

			internal const string TopImageExprHost = "TopImageExprHost";

			internal const string HueColorExpr = "HueColorExpr";

			internal const string OffsetXExpr = "OffsetXExpr";

			internal const string OffsetYExpr = "OffsetYExpr";

			internal const string FrameImageExprHost = "FrameImageExprHost";

			internal const string FrameImageHost = "FrameImageHost";

			internal const string IndicatorImageExprHost = "IndicatorImageExprHost";

			internal const string IndicatorImageHost = "IndicatorImageHost";

			internal const string TransparencyExpr = "TransparencyExpr";

			internal const string ClipImageExpr = "ClipImageExpr";

			internal const string PointerImageExprHost = "PointerImageExprHost";

			internal const string PointerImageHost = "PointerImageHost";

			internal const string BackFrameExprHost = "BackFrameExprHost";

			internal const string BackFrameHost = "BackFrameHost";

			internal const string FrameStyleExpr = "FrameStyleExpr";

			internal const string FrameShapeExpr = "FrameShapeExpr";

			internal const string FrameWidthExpr = "FrameWidthExpr";

			internal const string GlassEffectExpr = "GlassEffectExpr";

			internal const string FrameBackgroundExprHost = "FrameBackgroundExprHost";

			internal const string FrameBackgroundHost = "FrameBackgroundHost";

			internal const string CustomLabelExprHost = "CustomLabelExprHost";

			internal const string FontAngleExpr = "FontAngleExpr";

			internal const string UseFontPercentExpr = "UseFontPercentExpr";

			internal const string GaugeExprHost = "GaugeExprHost";

			internal const string ClipContentExpr = "ClipContentExpr";

			internal const string GaugeImageExprHost = "GaugeImageExprHost";

			internal const string AspectRatioExpr = "AspectRatioExpr";

			internal const string GaugeInputValueExprHost = "GaugeInputValueExprHost";

			internal const string FormulaExpr = "FormulaExpr";

			internal const string MinPercentExpr = "MinPercentExpr";

			internal const string MaxPercentExpr = "MaxPercentExpr";

			internal const string MultiplierExpr = "MultiplierExpr";

			internal const string AddConstantExpr = "AddConstantExpr";

			internal const string GaugeLabelExprHost = "GaugeLabelExprHost";

			internal const string AntiAliasingExpr = "AntiAliasingExpr";

			internal const string AutoLayoutExpr = "AutoLayoutExpr";

			internal const string ShadowIntensityExpr = "ShadowIntensityExpr";

			internal const string TileLanguageExpr = "TileLanguageExpr";

			internal const string TextAntiAliasingQualityExpr = "TextAntiAliasingQualityExpr";

			internal const string GaugePanelItemExprHost = "GaugePanelItemExprHost";

			internal const string TopExpr = "TopExpr";

			internal const string HeightExpr = "HeightExpr";

			internal const string GaugePointerExprHost = "GaugePointerExprHost";

			internal const string BarStartExpr = "BarStartExpr";

			internal const string MarkerLengthExpr = "MarkerLengthExpr";

			internal const string MarkerStyleExpr = "MarkerStyleExpr";

			internal const string SnappingEnabledExpr = "SnappingEnabledExpr";

			internal const string SnappingIntervalExpr = "SnappingIntervalExpr";

			internal const string GaugeScaleExprHost = "GaugeScaleExprHost";

			internal const string LogarithmicExpr = "LogarithmicExpr";

			internal const string LogarithmicBaseExpr = "LogarithmicBaseExpr";

			internal const string TickMarksOnTopExpr = "TickMarksOnTopExpr";

			internal const string GaugeTickMarksExprHost = "GaugeTickMarksExprHost";

			internal const string LinearGaugeExprHost = "LinearGaugeExprHost";

			internal const string OrientationExpr = "OrientationExpr";

			internal const string LinearPointerExprHost = "LinearPointerExprHost";

			internal const string LinearScaleExprHost = "LinearScaleExprHost";

			internal const string StartMarginExpr = "StartMarginExpr";

			internal const string EndMarginExpr = "EndMarginExpr";

			internal const string NumericIndicatorExprHost = "NumericIndicatorExprHost";

			internal const string PinLabelExprHost = "PinLabelExprHost";

			internal const string AllowUpsideDownExpr = "AllowUpsideDownExpr";

			internal const string RotateLabelExpr = "RotateLabelExpr";

			internal const string PointerCapExprHost = "PointerCapExprHost";

			internal const string OnTopExpr = "OnTopExpr";

			internal const string ReflectionExpr = "ReflectionExpr";

			internal const string CapStyleExpr = "CapStyleExpr";

			internal const string RadialGaugeExprHost = "RadialGaugeExprHost";

			internal const string PivotXExpr = "PivotXExpr";

			internal const string PivotYExpr = "PivotYExpr";

			internal const string RadialPointerExprHost = "RadialPointerExprHost";

			internal const string NeedleStyleExpr = "NeedleStyleExpr";

			internal const string RadialScaleExprHost = "RadialScaleExprHost";

			internal const string RadiusExpr = "RadiusExpr";

			internal const string StartAngleExpr = "StartAngleExpr";

			internal const string SweepAngleExpr = "SweepAngleExpr";

			internal const string ScaleLabelsExprHost = "ScaleLabelsExprHost";

			internal const string RotateLabelsExpr = "RotateLabelsExpr";

			internal const string ShowEndLabelsExpr = "ShowEndLabelsExpr";

			internal const string ScalePinExprHost = "ScalePinExprHost";

			internal const string EnableExpr = "EnableExpr";

			internal const string ScaleRangeExprHost = "ScaleRangeExprHost";

			internal const string DistanceFromScaleExpr = "DistanceFromScaleExpr";

			internal const string StartWidthExpr = "StartWidthExpr";

			internal const string EndWidthExpr = "EndWidthExpr";

			internal const string InRangeBarPointerColorExpr = "InRangeBarPointerColorExpr";

			internal const string InRangeLabelColorExpr = "InRangeLabelColorExpr";

			internal const string InRangeTickMarksColorExpr = "InRangeTickMarksColorExpr";

			internal const string BackgroundGradientTypeExpr = "BackgroundGradientTypeExpr";

			internal const string PlacementExpr = "PlacementExpr";

			internal const string StateIndicatorExprHost = "StateIndicatorExprHost";

			internal const string ThermometerExprHost = "ThermometerExprHost";

			internal const string BulbOffsetExpr = "BulbOffsetExpr";

			internal const string BulbSizeExpr = "BulbSizeExpr";

			internal const string ThermometerStyleExpr = "ThermometerStyleExpr";

			internal const string TickMarkStyleExprHost = "TickMarkStyleExprHost";

			internal const string EnableGradientExpr = "EnableGradientExpr";

			internal const string GradientDensityExpr = "GradientDensityExpr";

			internal const string GaugeMajorTickMarksHost = "GaugeMajorTickMarksHost";

			internal const string GaugeMinorTickMarksHost = "GaugeMinorTickMarksHost";

			internal const string GaugeMaximumPinHost = "MaximumPinHost";

			internal const string GaugeMinimumPinHost = "MinimumPinHost";

			internal const string GaugeInputValueHost = "GaugeInputValueHost";

			internal const string WidthExpr = "WidthExpr";

			internal const string ZIndexExpr = "ZIndexExpr";

			internal const string PositionExpr = "PositionExpr";

			internal const string ShapeExpr = "ShapeExpr";

			internal const string ScaleLabelsHost = "ScaleLabelsHost";

			internal const string ScalePinHost = "ScalePinHost";

			internal const string PinLabelHost = "PinLabelHost";

			internal const string PointerCapHost = "PointerCapHost";

			internal const string ThermometerHost = "ThermometerHost";

			internal const string TickMarkStyleHost = "TickMarkStyleHost";

			internal const string ResizeModeExpr = "ResizeModeExpr";

			internal const string TextShadowOffsetExpr = "TextShadowOffsetExpr";

			internal const string CustomLabelsHosts = "m_customLabelsHostsRemotable";

			internal const string GaugeImagesHosts = "m_gaugeImagesHostsRemotable";

			internal const string GaugeLabelsHosts = "m_gaugeLabelsHostsRemotable";

			internal const string LinearGaugesHosts = "m_linearGaugesHostsRemotable";

			internal const string RadialGaugesHosts = "m_radialGaugesHostsRemotable";

			internal const string LinearPointersHosts = "m_linearPointersHostsRemotable";

			internal const string RadialPointersHosts = "m_radialPointersHostsRemotable";

			internal const string LinearScalesHosts = "m_linearScalesHostsRemotable";

			internal const string RadialScalesHosts = "m_radialScalesHostsRemotable";

			internal const string ScaleRangesHosts = "m_scaleRangesHostsRemotable";

			internal const string NumericIndicatorsHosts = "m_numericIndicatorsHostsRemotable";

			internal const string StateIndicatorsHosts = "m_stateIndicatorsHostsRemotable";

			internal const string GaugeInputValuesHosts = "m_gaugeInputValueHostsRemotable";

			internal const string NumericIndicatorRangesHosts = "m_numericIndicatorRangesHostsRemotable";

			internal const string IndicatorStatesHosts = "m_indicatorStatesHostsRemotable";

			internal const string NumericIndicatorHost = "NumericIndicatorHost";

			internal const string DecimalDigitColorExpr = "DecimalDigitColorExpr";

			internal const string DigitColorExpr = "DigitColorExpr";

			internal const string DecimalDigitsExpr = "DecimalDigitsExpr";

			internal const string DigitsExpr = "DigitsExpr";

			internal const string NonNumericStringExpr = "NonNumericStringExpr";

			internal const string OutOfRangeStringExpr = "OutOfRangeStringExpr";

			internal const string ShowDecimalPointExpr = "ShowDecimalPointExpr";

			internal const string ShowLeadingZerosExpr = "ShowLeadingZerosExpr";

			internal const string IndicatorStyleExpr = "IndicatorStyleExpr";

			internal const string ShowSignExpr = "ShowSignExpr";

			internal const string LedDimColorExpr = "LedDimColorExpr";

			internal const string SeparatorWidthExpr = "SeparatorWidthExpr";

			internal const string NumericIndicatorRangeExprHost = "NumericIndicatorRangeExprHost";

			internal const string NumericIndicatorRangeHost = "NumericIndicatorRangeHost";

			internal const string StateIndicatorHost = "StateIndicatorHost";

			internal const string IndicatorStateExprHost = "IndicatorStateExprHost";

			internal const string IndicatorStateHost = "IndicatorStateHost";

			internal const string TransformationTypeExpr = "TransformationTypeExpr";

			internal const string MapViewExprHost = "MapViewExprHost";

			internal const string MapViewHost = "MapViewHost";

			internal const string ZoomExpr = "ZoomExpr";

			internal const string MapElementViewExprHost = "MapElementViewExprHost";

			internal const string MapElementViewHost = "MapElementViewHost";

			internal const string LayerNameExpr = "LayerNameExpr";

			internal const string MapDataBoundViewExprHost = "MapDataBoundViewExprHost";

			internal const string MapDataBoundViewHost = "MapDataBoundViewHost";

			internal const string MapCustomViewExprHost = "MapCustomViewExprHost";

			internal const string MapCustomViewHost = "MapCustomViewHost";

			internal const string CenterXExpr = "CenterXExpr";

			internal const string CenterYExpr = "CenterYExpr";

			internal const string MapBorderSkinExprHost = "MapBorderSkinExprHost";

			internal const string MapBorderSkinHost = "MapBorderSkinHost";

			internal const string MapBorderSkinTypeExpr = "MapBorderSkinTypeExpr";

			internal const string MapDataRegionNameExpr = "MapDataRegionNameExpr";

			internal const string MapTileLayerExprHost = "MapTileLayerExprHost";

			internal const string MapTileLayerHost = "MapTileLayerHost";

			internal const string ServiceUrlExpr = "ServiceUrlExpr";

			internal const string TileStyleExpr = "TileStyleExpr";

			internal const string MapTileExprHost = "MapTileExprHost";

			internal const string MapTileHost = "MapTileHost";

			internal const string UseSecureConnectionExpr = "UseSecureConnectionExpr";

			internal const string MapPolygonLayerExprHost = "MapPolygonLayerExprHost";

			internal const string MapPointLayerExprHost = "MapPointLayerExprHost";

			internal const string MapLineLayerExprHost = "MapLineLayerExprHost";

			internal const string MapSpatialDataSetExprHost = "MapSpatialDataSetExprHost";

			internal const string DataSetNameExpr = "DataSetNameExpr";

			internal const string SpatialFieldExpr = "SpatialFieldExpr";

			internal const string MapSpatialDataRegionExprHost = "MapSpatialDataRegionExprHost";

			internal const string VectorDataExpr = "VectorDataExpr";

			internal const string MapSpatialDataExprHost = "MapSpatialDataExprHost";

			internal const string MapSpatialDataHost = "MapSpatialDataHost";

			internal const string SimplificationResolutionExpr = "SimplificationResolutionExpr";

			internal const string MapShapefileExprHost = "MapShapefileExprHost";

			internal const string MapLayerExprHost = "MapLayerExprHost";

			internal const string MapLayerHost = "MapLayerHost";

			internal const string VisibilityModeExpr = "VisibilityModeExpr";

			internal const string MapFieldNameExprHost = "MapFieldNameExprHost";

			internal const string MapFieldNameHost = "MapFieldNameHost";

			internal const string NameExpr = "NameExpr";

			internal const string MapFieldDefinitionExprHost = "MapFieldDefinitionExprHost";

			internal const string MapFieldDefinitionHost = "MapFieldDefinitionHost";

			internal const string MapPointExprHost = "MapPointExprHost";

			internal const string MapPointHost = "MapPointHost";

			internal const string MapSpatialElementExprHost = "MapSpatialElementExprHost";

			internal const string MapSpatialElementHost = "MapSpatialElementHost";

			internal const string MapPolygonExprHost = "MapPolygonExprHost";

			internal const string MapPolygonHost = "MapPolygonHost";

			internal const string UseCustomPolygonTemplateExpr = "UseCustomPolygonTemplateExpr";

			internal const string UseCustomPointTemplateExpr = "UseCustomPointTemplateExpr";

			internal const string MapLineExprHost = "MapLineExprHost";

			internal const string MapLineHost = "MapLineHost";

			internal const string UseCustomLineTemplateExpr = "UseCustomLineTemplateExpr";

			internal const string MapFieldExprHost = "MapFieldExprHost";

			internal const string MapFieldHost = "MapFieldHost";

			internal const string MapPointTemplateExprHost = "MapPointTemplateExprHost";

			internal const string MapPointTemplateHost = "MapPointTemplateHost";

			internal const string MapMarkerTemplateExprHost = "MapMarkerTemplateExprHost";

			internal const string MapMarkerTemplateHost = "MapMarkerTemplateHost";

			internal const string MapPolygonTemplateExprHost = "MapPolygonTemplateExprHost";

			internal const string MapPolygonTemplateHost = "MapPolygonTemplateHost";

			internal const string ScaleFactorExpr = "ScaleFactorExpr";

			internal const string CenterPointOffsetXExpr = "CenterPointOffsetXExpr";

			internal const string CenterPointOffsetYExpr = "CenterPointOffsetYExpr";

			internal const string ShowLabelExpr = "ShowLabelExpr";

			internal const string MapLineTemplateExprHost = "MapLineTemplateExprHost";

			internal const string MapLineTemplateHost = "MapLineTemplateHost";

			internal const string MapCustomColorRuleExprHost = "MapCustomColorRuleExprHost";

			internal const string MapCustomColorExprHost = "MapCustomColorExprHost";

			internal const string MapCustomColorHost = "MapCustomColorHost";

			internal const string MapPointRulesExprHost = "MapPointRulesExprHost";

			internal const string MapPointRulesHost = "MapPointRulesHost";

			internal const string MapMarkerRuleExprHost = "MapMarkerRuleExprHost";

			internal const string MapMarkerRuleHost = "MapMarkerRuleHost";

			internal const string MapMarkerExprHost = "MapMarkerExprHost";

			internal const string MapMarkerHost = "MapMarkerHost";

			internal const string MapMarkerStyleExpr = "MapMarkerStyleExpr";

			internal const string MapMarkerImageExprHost = "MapMarkerImageExprHost";

			internal const string MapMarkerImageHost = "MapMarkerImageHost";

			internal const string MapSizeRuleExprHost = "MapSizeRuleExprHost";

			internal const string MapSizeRuleHost = "MapSizeRuleHost";

			internal const string StartSizeExpr = "StartSizeExpr";

			internal const string EndSizeExpr = "EndSizeExpr";

			internal const string MapPolygonRulesExprHost = "MapPolygonRulesExprHost";

			internal const string MapPolygonRulesHost = "MapPolygonRulesHost";

			internal const string MapLineRulesExprHost = "MapLineRulesExprHost";

			internal const string MapLineRulesHost = "MapLineRulesHost";

			internal const string MapColorRuleExprHost = "MapColorRuleExprHost";

			internal const string MapColorRuleHost = "MapColorRuleHost";

			internal const string ShowInColorScaleExpr = "ShowInColorScaleExpr";

			internal const string MapColorRangeRuleExprHost = "MapColorRangeRuleExprHost";

			internal const string StartColorExpr = "StartColorExpr";

			internal const string MiddleColorExpr = "MiddleColorExpr";

			internal const string EndColorExpr = "EndColorExpr";

			internal const string MapColorPaletteRuleExprHost = "MapColorPaletteRuleExprHost";

			internal const string MapBucketExprHost = "MapBucketExprHost";

			internal const string MapBucketHost = "MapBucketHost";

			internal const string MapAppearanceRuleExprHost = "MapAppearanceRuleExprHost";

			internal const string MapAppearanceRuleHost = "MapAppearanceRuleHost";

			internal const string DataValueExpr = "DataValueExpr";

			internal const string DistributionTypeExpr = "DistributionTypeExpr";

			internal const string BucketCountExpr = "BucketCountExpr";

			internal const string StartValueExpr = "StartValueExpr";

			internal const string EndValueExpr = "EndValueExpr";

			internal const string MapLegendTitleExprHost = "MapLegendTitleExprHost";

			internal const string MapLegendTitleHost = "MapLegendTitleHost";

			internal const string TitleSeparatorColorExpr = "TitleSeparatorColorExpr";

			internal const string MapLegendExprHost = "MapLegendExprHost";

			internal const string MapLegendHost = "MapLegendHost";

			internal const string MapLocationExprHost = "MapLocationExprHost";

			internal const string MapLocationHost = "MapLocationHost";

			internal const string MapSizeExprHost = "MapSizeExprHost";

			internal const string MapSizeHost = "MapSizeHost";

			internal const string UnitExpr = "UnitExpr";

			internal const string MapGridLinesExprHost = "MapGridLinesExprHost";

			internal const string MapMeridiansHost = "MapMeridiansHost";

			internal const string MapParallelsHost = "MapParallelsHost";

			internal const string ShowLabelsExpr = "ShowLabelsExpr";

			internal const string LabelPositionExpr = "LabelPositionExpr";

			internal const string MapHosts = "m_mapHostsRemotable";

			internal const string MapDataRegionHosts = "m_mapDataRegionHostsRemotable";

			internal const string MapDockableSubItemExprHost = "MapDockableSubItemExprHost";

			internal const string MapDockableSubItemHost = "MapDockableSubItemHost";

			internal const string DockOutsideViewportExpr = "DockOutsideViewportExpr";

			internal const string MapSubItemExprHost = "MapSubItemExprHost";

			internal const string MapSubItemHost = "MapSubItemHost";

			internal const string MapBindingFieldPairExprHost = "MapBindingFieldPairExprHost";

			internal const string MapBindingFieldPairHost = "MapBindingFieldPairHost";

			internal const string FieldNameExpr = "FieldNameExpr";

			internal const string BindingExpressionExpr = "BindingExpressionExpr";

			internal const string ZoomEnabledExpr = "ZoomEnabledExpr";

			internal const string MapViewportExprHost = "MapViewportExprHost";

			internal const string MapViewportHost = "MapViewportHost";

			internal const string MapCoordinateSystemExpr = "MapCoordinateSystemExpr";

			internal const string MapProjectionExpr = "MapProjectionExpr";

			internal const string ProjectionCenterXExpr = "ProjectionCenterXExpr";

			internal const string ProjectionCenterYExpr = "ProjectionCenterYExpr";

			internal const string MaximumZoomExpr = "MaximumZoomExpr";

			internal const string MinimumZoomExpr = "MinimumZoomExpr";

			internal const string ContentMarginExpr = "ContentMarginExpr";

			internal const string GridUnderContentExpr = "GridUnderContentExpr";

			internal const string MapBindingFieldPairsHosts = "m_mapBindingFieldPairsHostsRemotable";

			internal const string MapLimitsExprHost = "MapLimitsExprHost";

			internal const string MapLimitsHost = "MapLimitsHost";

			internal const string MinimumXExpr = "MinimumXExpr";

			internal const string MinimumYExpr = "MinimumYExpr";

			internal const string MaximumXExpr = "MaximumXExpr";

			internal const string MaximumYExpr = "MaximumYExpr";

			internal const string LimitToDataExpr = "LimitToDataExpr";

			internal const string MapColorScaleExprHost = "MapColorScaleExprHost";

			internal const string MapColorScaleHost = "MapColorScaleHost";

			internal const string TickMarkLengthExpr = "TickMarkLengthExpr";

			internal const string ColorBarBorderColorExpr = "ColorBarBorderColorExpr";

			internal const string LabelFormatExpr = "LabelFormatExpr";

			internal const string LabelPlacementExpr = "LabelPlacementExpr";

			internal const string LabelBehaviorExpr = "LabelBehaviorExpr";

			internal const string RangeGapColorExpr = "RangeGapColorExpr";

			internal const string NoDataTextExpr = "NoDataTextExpr";

			internal const string MapColorScaleTitleExprHost = "MapColorScaleTitleExprHost";

			internal const string MapColorScaleTitleHost = "MapColorScaleTitleHost";

			internal const string MapDistanceScaleExprHost = "MapDistanceScaleExprHost";

			internal const string MapDistanceScaleHost = "MapDistanceScaleHost";

			internal const string ScaleColorExpr = "ScaleColorExpr";

			internal const string ScaleBorderColorExpr = "ScaleBorderColorExpr";

			internal const string MapTitleExprHost = "MapTitleExprHost";

			internal const string MapTitleHost = "MapTitleHost";

			internal const string MapLegendsHosts = "m_mapLegendsHostsRemotable";

			internal const string MapTitlesHosts = "m_mapTitlesHostsRemotable";

			internal const string MapMarkersHosts = "m_mapMarkersHostsRemotable";

			internal const string MapBucketsHosts = "m_mapBucketsHostsRemotable";

			internal const string MapCustomColorsHosts = "m_mapCustomColorsHostsRemotable";

			internal const string MapPointsHosts = "m_mapPointsHostsRemotable";

			internal const string MapPolygonsHosts = "m_mapPolygonsHostsRemotable";

			internal const string MapLinesHosts = "m_mapLinesHostsRemotable";

			internal const string MapTileLayersHosts = "m_mapTileLayersHostsRemotable";

			internal const string MapTilesHosts = "m_mapTilesHostsRemotable";

			internal const string MapPointLayersHosts = "m_mapPointLayersHostsRemotable";

			internal const string MapPolygonLayersHosts = "m_mapPolygonLayersHostsRemotable";

			internal const string MapLineLayersHosts = "m_mapLineLayersHostsRemotable";

			internal const string MapFieldNamesHosts = "m_mapFieldNamesHostsRemotable";

			internal const string MapExprHost = "MapExprHost";

			internal const string DataElementLabelExpr = "DataElementLabelExpr";

			internal const string ParagraphExprHost = "ParagraphExprHost";

			internal const string ParagraphHosts = "m_paragraphHostsRemotable";

			internal const string LeftIndentExpr = "LeftIndentExpr";

			internal const string RightIndentExpr = "RightIndentExpr";

			internal const string HangingIndentExpr = "HangingIndentExpr";

			internal const string SpaceBeforeExpr = "SpaceBeforeExpr";

			internal const string SpaceAfterExpr = "SpaceAfterExpr";

			internal const string ListStyleExpr = "ListStyleExpr";

			internal const string ListLevelExpr = "ListLevelExpr";

			internal const string TextRunExprHost = "TextRunExprHost";

			internal const string TextRunHosts = "m_textRunHostsRemotable";

			internal const string MarkupTypeExpr = "MarkupTypeExpr";

			internal const string LookupSourceExpr = "SourceExpr";

			internal const string LookupDestExpr = "DestExpr";

			internal const string LookupResultExpr = "ResultExpr";

			internal const string PageBreakExprHost = "PageBreakExprHost";

			internal const string PageBreakDisabledExpr = "DisabledExpr";

			internal const string PageBreakPageNameExpr = "PageNameExpr";

			internal const string PageBreakResetPageNumberExpr = "ResetPageNumberExpr";

			internal const string JoinConditionForeignKeyExpr = "ForeignKeyExpr";

			internal const string JoinConditionPrimaryKeyExpr = "PrimaryKeyExpr";
		}

		private abstract class TypeDecl
		{
			internal CodeTypeDeclaration Type;

			internal string BaseTypeName;

			internal TypeDecl Parent;

			internal CodeConstructor Constructor;

			internal bool HasExpressions;

			internal CodeExpressionCollection DataValues;

			protected readonly bool m_setCode;

			internal void NestedTypeAdd(string name, CodeTypeDeclaration nestedType)
			{
				this.ConstructorCreate();
				this.Type.Members.Add(nestedType);
				this.Constructor.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), name), this.CreateTypeCreateExpression(nestedType.Name)));
			}

			internal int NestedTypeColAdd(string name, string baseTypeName, ref CodeExpressionCollection initializers, CodeTypeDeclaration nestedType)
			{
				this.Type.Members.Add(nestedType);
				this.TypeColInit(name, baseTypeName, ref initializers);
				return initializers.Add(this.CreateTypeCreateExpression(nestedType.Name));
			}

			protected TypeDecl(string typeName, string baseTypeName, TypeDecl parent, bool setCode)
			{
				this.BaseTypeName = baseTypeName;
				this.Parent = parent;
				this.m_setCode = setCode;
				this.Type = this.CreateType(typeName, baseTypeName);
			}

			protected void ConstructorCreate()
			{
				if (this.Constructor == null)
				{
					this.Constructor = this.CreateConstructor();
					this.Type.Members.Add(this.Constructor);
				}
			}

			protected virtual CodeConstructor CreateConstructor()
			{
				CodeConstructor codeConstructor = new CodeConstructor();
				codeConstructor.Attributes = MemberAttributes.Public;
				return codeConstructor;
			}

			protected CodeAssignStatement CreateTypeColInitStatement(string name, string baseTypeName, ref CodeExpressionCollection initializers)
			{
				CodeObjectCreateExpression codeObjectCreateExpression = new CodeObjectCreateExpression();
				codeObjectCreateExpression.CreateType = new CodeTypeReference((name == "m_memberTreeHostsRemotable") ? "RemoteMemberArrayWrapper" : "RemoteArrayWrapper", new CodeTypeReference(baseTypeName));
				if (initializers != null)
				{
					codeObjectCreateExpression.Parameters.AddRange(initializers);
				}
				initializers = codeObjectCreateExpression.Parameters;
				return new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), name), codeObjectCreateExpression);
			}

			protected virtual CodeTypeDeclaration CreateType(string name, string baseType)
			{
				Global.Tracer.Assert(name != null, "(name != null)");
				CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(name);
				if (baseType != null)
				{
					codeTypeDeclaration.BaseTypes.Add(new CodeTypeReference(baseType));
				}
				codeTypeDeclaration.Attributes = (MemberAttributes)24578;
				return codeTypeDeclaration;
			}

			private void TypeColInit(string name, string baseTypeName, ref CodeExpressionCollection initializers)
			{
				this.ConstructorCreate();
				if (initializers == null)
				{
					this.Constructor.Statements.Add(this.CreateTypeColInitStatement(name, baseTypeName, ref initializers));
				}
			}

			private CodeObjectCreateExpression CreateTypeCreateExpression(string typeName)
			{
				if (this.m_setCode)
				{
					return new CodeObjectCreateExpression(typeName, new CodeArgumentReferenceExpression("Code"));
				}
				return new CodeObjectCreateExpression(typeName);
			}
		}

		private sealed class RootTypeDecl : TypeDecl
		{
			internal CodeExpressionCollection Aggregates;

			internal CodeExpressionCollection PageSections;

			internal CodeExpressionCollection ReportParameters;

			internal CodeExpressionCollection DataSources;

			internal CodeExpressionCollection DataSets;

			internal CodeExpressionCollection Lines;

			internal CodeExpressionCollection Rectangles;

			internal CodeExpressionCollection TextBoxes;

			internal CodeExpressionCollection Images;

			internal CodeExpressionCollection Subreports;

			internal CodeExpressionCollection Tablices;

			internal CodeExpressionCollection DataShapes;

			internal CodeExpressionCollection Charts;

			internal CodeExpressionCollection GaugePanels;

			internal CodeExpressionCollection CustomReportItems;

			internal CodeExpressionCollection Lookups;

			internal CodeExpressionCollection LookupDests;

			internal CodeExpressionCollection Pages;

			internal CodeExpressionCollection ReportSections;

			internal CodeExpressionCollection Maps;

			internal RootTypeDecl(bool setCode)
				: base("ReportExprHostImpl", "ReportExprHost", null, setCode)
			{
			}

			protected override CodeConstructor CreateConstructor()
			{
				CodeConstructor codeConstructor = base.CreateConstructor();
				codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "includeParameters"));
				codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "parametersOnly"));
				CodeParameterDeclarationExpression value = new CodeParameterDeclarationExpression(typeof(object), "reportObjectModel");
				codeConstructor.Parameters.Add(value);
				codeConstructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("reportObjectModel"));
				this.ReportParameters = new CodeExpressionCollection();
				this.DataSources = new CodeExpressionCollection();
				this.DataSets = new CodeExpressionCollection();
				return codeConstructor;
			}

			protected override CodeTypeDeclaration CreateType(string name, string baseType)
			{
				CodeTypeDeclaration codeTypeDeclaration = base.CreateType(name, baseType);
				if (base.m_setCode)
				{
					CodeMemberField codeMemberField = new CodeMemberField("CustomCodeProxy", "Code");
					codeMemberField.Attributes = (MemberAttributes)20482;
					codeTypeDeclaration.Members.Add(codeMemberField);
				}
				return codeTypeDeclaration;
			}

			internal void CompleteConstructorCreation()
			{
				if (base.HasExpressions)
				{
					if (base.Constructor == null)
					{
						base.ConstructorCreate();
					}
					else
					{
						CodeConditionStatement codeConditionStatement = new CodeConditionStatement();
						codeConditionStatement.Condition = new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("parametersOnly"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(true));
						codeConditionStatement.TrueStatements.Add(new CodeMethodReturnStatement());
						base.Constructor.Statements.Insert(0, codeConditionStatement);
						if (this.ReportParameters.Count > 0)
						{
							CodeConditionStatement codeConditionStatement2 = new CodeConditionStatement();
							codeConditionStatement2.Condition = new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("includeParameters"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(true));
							codeConditionStatement2.TrueStatements.Add(base.CreateTypeColInitStatement("m_reportParameterHostsRemotable", "ReportParamExprHost", ref this.ReportParameters));
							base.Constructor.Statements.Insert(0, codeConditionStatement2);
						}
						if (this.DataSources.Count > 0)
						{
							base.Constructor.Statements.Insert(0, base.CreateTypeColInitStatement("m_dataSourceHostsRemotable", "DataSourceExprHost", ref this.DataSources));
						}
						if (this.DataSets.Count > 0)
						{
							base.Constructor.Statements.Insert(0, base.CreateTypeColInitStatement("m_dataSetHostsRemotable", "DataSetExprHost", ref this.DataSets));
						}
					}
					Global.Tracer.Assert(null != base.Constructor, "Invalid EH constructor");
					this.CreateCustomCodeInitialization();
				}
			}

			private void CreateCustomCodeInitialization()
			{
				if (base.m_setCode)
				{
					base.Constructor.Statements.Insert(0, new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "m_codeProxyBase"), new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Code")));
					base.Constructor.Statements.Insert(0, new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Code"), new CodeObjectCreateExpression("CustomCodeProxy", new CodeThisReferenceExpression())));
				}
			}
		}

		private sealed class NonRootTypeDecl : TypeDecl
		{
			internal CodeExpressionCollection Parameters;

			internal CodeExpressionCollection Filters;

			internal CodeExpressionCollection Actions;

			internal CodeExpressionCollection Fields;

			internal CodeExpressionCollection ValueAxes;

			internal CodeExpressionCollection CategoryAxes;

			internal CodeExpressionCollection ChartTitles;

			internal CodeExpressionCollection ChartLegends;

			internal CodeExpressionCollection ChartAreas;

			internal CodeExpressionCollection TablixMembers;

			internal CodeExpressionCollection DataShapeMembers;

			internal CodeExpressionCollection ChartMembers;

			internal CodeExpressionCollection GaugeMembers;

			internal CodeExpressionCollection DataGroups;

			internal CodeExpressionCollection TablixCells;

			internal CodeExpressionCollection DataShapeIntersections;

			internal CodeExpressionCollection DataPoints;

			internal CodeExpressionCollection DataCells;

			internal CodeExpressionCollection ChartLegendCustomItemCells;

			internal CodeExpressionCollection ChartCustomPaletteColors;

			internal CodeExpressionCollection ChartStripLines;

			internal CodeExpressionCollection ChartSeriesCollection;

			internal CodeExpressionCollection ChartDerivedSeriesCollection;

			internal CodeExpressionCollection ChartFormulaParameters;

			internal CodeExpressionCollection ChartLegendColumns;

			internal CodeExpressionCollection ChartLegendCustomItems;

			internal CodeExpressionCollection Paragraphs;

			internal CodeExpressionCollection TextRuns;

			internal CodeExpressionCollection GaugeCells;

			internal CodeExpressionCollection CustomLabels;

			internal CodeExpressionCollection GaugeImages;

			internal CodeExpressionCollection GaugeLabels;

			internal CodeExpressionCollection LinearGauges;

			internal CodeExpressionCollection RadialGauges;

			internal CodeExpressionCollection RadialPointers;

			internal CodeExpressionCollection LinearPointers;

			internal CodeExpressionCollection LinearScales;

			internal CodeExpressionCollection RadialScales;

			internal CodeExpressionCollection ScaleRanges;

			internal CodeExpressionCollection NumericIndicators;

			internal CodeExpressionCollection StateIndicators;

			internal CodeExpressionCollection GaugeInputValues;

			internal CodeExpressionCollection NumericIndicatorRanges;

			internal CodeExpressionCollection IndicatorStates;

			internal CodeExpressionCollection MapMembers;

			internal CodeExpressionCollection MapBindingFieldPairs;

			internal CodeExpressionCollection MapLegends;

			internal CodeExpressionCollection MapTitles;

			internal CodeExpressionCollection MapMarkers;

			internal CodeExpressionCollection MapBuckets;

			internal CodeExpressionCollection MapCustomColors;

			internal CodeExpressionCollection MapPoints;

			internal CodeExpressionCollection MapPolygons;

			internal CodeExpressionCollection MapLines;

			internal CodeExpressionCollection MapTileLayers;

			internal CodeExpressionCollection MapTiles;

			internal CodeExpressionCollection MapPointLayers;

			internal CodeExpressionCollection MapPolygonLayers;

			internal CodeExpressionCollection MapLineLayers;

			internal CodeExpressionCollection MapFieldNames;

			internal CodeExpressionCollection JoinConditions;

			internal ReturnStatementList IndexedExpressions;

			internal NonRootTypeDecl(string typeName, string baseTypeName, TypeDecl parent, bool setCode)
				: base(typeName, baseTypeName, parent, setCode)
			{
				if (setCode)
				{
					base.ConstructorCreate();
				}
			}

			protected override CodeConstructor CreateConstructor()
			{
				CodeConstructor codeConstructor = base.CreateConstructor();
				if (base.m_setCode)
				{
					codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression("CustomCodeProxy", "code"));
					codeConstructor.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Code"), new CodeArgumentReferenceExpression("code")));
				}
				return codeConstructor;
			}

			protected override CodeTypeDeclaration CreateType(string name, string baseType)
			{
				CodeTypeDeclaration codeTypeDeclaration = base.CreateType(string.Format(CultureInfo.InvariantCulture, "{0}_{1}", name, baseType), baseType);
				if (base.m_setCode)
				{
					CodeMemberField codeMemberField = new CodeMemberField("CustomCodeProxy", "Code");
					codeMemberField.Attributes = (MemberAttributes)20482;
					codeTypeDeclaration.Members.Add(codeMemberField);
				}
				return codeTypeDeclaration;
			}
		}

		private sealed class CustomCodeProxyDecl : TypeDecl
		{
			internal CustomCodeProxyDecl(TypeDecl parent)
				: base("CustomCodeProxy", "CustomCodeProxyBase", parent, false)
			{
				base.ConstructorCreate();
			}

			protected override CodeConstructor CreateConstructor()
			{
				CodeConstructor codeConstructor = base.CreateConstructor();
				codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IReportObjectModelProxyForCustomCode), "reportObjectModel"));
				codeConstructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("reportObjectModel"));
				return codeConstructor;
			}

			internal void AddClassInstance(string className, string instanceName, int id)
			{
				string fileName = "CMCID" + id.ToString(CultureInfo.InvariantCulture) + "end";
				CodeMemberField codeMemberField = new CodeMemberField(className, "m_" + instanceName);
				codeMemberField.Attributes = (MemberAttributes)20482;
				codeMemberField.InitExpression = new CodeObjectCreateExpression(className);
				codeMemberField.LinePragma = new CodeLinePragma(fileName, 0);
				base.Type.Members.Add(codeMemberField);
				CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
				codeMemberProperty.Type = new CodeTypeReference(className);
				codeMemberProperty.Name = instanceName;
				codeMemberProperty.Attributes = (MemberAttributes)24578;
				codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), codeMemberField.Name)));
				codeMemberProperty.LinePragma = new CodeLinePragma(fileName, 2);
				base.Type.Members.Add(codeMemberProperty);
			}

			internal void AddCode(string code)
			{
				CodeTypeMember codeTypeMember = new CodeSnippetTypeMember(code);
				codeTypeMember.LinePragma = new CodeLinePragma("CustomCode", 0);
				base.Type.Members.Add(codeTypeMember);
			}
		}

		private sealed class ReturnStatementList
		{
			private ArrayList m_list = new ArrayList();

			internal CodeMethodReturnStatement this[int index]
			{
				get
				{
					return (CodeMethodReturnStatement)this.m_list[index];
				}
			}

			internal int Count
			{
				get
				{
					return this.m_list.Count;
				}
			}

			internal int Add(CodeMethodReturnStatement retStatement)
			{
                return this.m_list.Add(retStatement);
			}
		}

		internal const string RootType = "ReportExprHostImpl";

		internal const int InvalidExprHostId = -1;

		private const string EndSrcMarker = "end";

		private const string ExprSrcMarker = "Expr";

		private const string CustomCodeSrcMarker = "CustomCode";

		private const string CodeModuleClassInstanceDeclSrcMarker = "CMCID";

		private RootTypeDecl m_rootTypeDecl;

		private TypeDecl m_currentTypeDecl;

		private bool m_setCode;

		private static readonly Regex m_findExprNumber = new Regex("^Expr([0-9]+)end", RegexOptions.Compiled);

		private static readonly Regex m_findCodeModuleClassInstanceDeclNumber = new Regex("^CMCID([0-9]+)end", RegexOptions.Compiled);

		internal bool HasExpressions
		{
			get
			{
				if (this.m_rootTypeDecl != null)
				{
					return this.m_rootTypeDecl.HasExpressions;
				}
				return false;
			}
		}

		internal bool CustomCode
		{
			get
			{
				return this.m_setCode;
			}
		}

		internal ExprHostBuilder()
		{
		}

		internal void SetCustomCode()
		{
			this.m_setCode = true;
		}

		internal CodeCompileUnit GetExprHost(ProcessingIntermediateFormatVersion version, bool refusePermissions)
		{
			Global.Tracer.Assert(this.m_rootTypeDecl != null && this.m_currentTypeDecl.Parent == null, "(m_rootTypeDecl != null && m_currentTypeDecl.Parent == null)");
			CodeCompileUnit codeCompileUnit = null;
			if (this.HasExpressions)
			{
				codeCompileUnit = new CodeCompileUnit();
                //codeCompileUnit.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration("System.Runtime.CompilerServices.InternalsVisibleTo", new CodeAttributeArgument(new CodePrimitiveExpression(version.ToString()))));
				if (refusePermissions)
				{
                    //codeCompileUnit.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration("System.Security.Permissions.SecurityPermission", new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SecurityAction)), "RequestMinimum")), new CodeAttributeArgument("Execution", new CodePrimitiveExpression(true))));
                    //codeCompileUnit.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration("System.Security.Permissions.SecurityPermission", new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SecurityAction)), "RequestOptional")), new CodeAttributeArgument("Execution", new CodePrimitiveExpression(true))));
                }
				CodeNamespace codeNamespace = new CodeNamespace("ExpressionHost");
				codeCompileUnit.Namespaces.Add(codeNamespace);               
                codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
				codeNamespace.Imports.Add(new CodeNamespaceImport("System.Convert"));
				codeNamespace.Imports.Add(new CodeNamespaceImport("System.Math"));
                codeNamespace.Imports.Add(new CodeNamespaceImport("System.Runtime.CompilerServices"));
                codeNamespace.Imports.Add(new CodeNamespaceImport("Microsoft.VisualBasic"));
				//codeNamespace.Imports.Add(new CodeNamespaceImport("Microsoft.SqlServer.Types"));
				codeNamespace.Imports.Add(new CodeNamespaceImport("AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel"));
				codeNamespace.Imports.Add(new CodeNamespaceImport("AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel"));
				codeNamespace.Types.Add(this.m_rootTypeDecl.Type);
			}
            this.m_rootTypeDecl = null;
			return codeCompileUnit;
		}

		internal ErrorSource ParseErrorSource(CompilerError error, out int id)
		{
			Global.Tracer.Assert(error.FileName != null, "(error.FileName != null)");
			id = -1;
			if (error.FileName.StartsWith("CustomCode", StringComparison.Ordinal))
			{
				return ErrorSource.CustomCode;
			}
			try
			{
				Match match = ExprHostBuilder.m_findCodeModuleClassInstanceDeclNumber.Match(error.FileName);
				if (match.Success)
				{
					id = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
					return ErrorSource.CodeModuleClassInstanceDecl;
				}
				match = ExprHostBuilder.m_findExprNumber.Match(error.FileName);
				if (match.Success)
				{
					id = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
					return ErrorSource.Expression;
				}
			}
			catch (FormatException)
			{
			}
			return ErrorSource.Unknown;
		}

		internal void SharedDataSetStart()
		{
			this.m_currentTypeDecl = (this.m_rootTypeDecl = new RootTypeDecl(this.m_setCode));
		}

		internal void SharedDataSetEnd()
		{
			this.m_rootTypeDecl.CompleteConstructorCreation();
		}

		internal void ReportStart()
		{
			this.m_currentTypeDecl = (this.m_rootTypeDecl = new RootTypeDecl(this.m_setCode));
		}

		internal void ReportEnd()
		{
			this.m_rootTypeDecl.CompleteConstructorCreation();
		}

		internal void ReportLanguage(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ReportLanguageExpr", expression);
		}

		internal void ReportAutoRefresh(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AutoRefreshExpr", expression);
		}

		internal void ReportInitialPageName(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("InitialPageNameExpr", expression);
		}

		internal void GenericLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LabelExpr", expression);
		}

		internal void GenericValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ValueExpr", expression);
		}

		internal void GenericNoRows(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("NoRowsExpr", expression);
		}

		internal void GenericVisibilityHidden(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("VisibilityHiddenExpr", expression);
		}

		internal void AggregateParamExprAdd(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.AggregateStart();
			this.GenericValue(expression);
			expression.ExprHostID = this.AggregateEnd();
		}

		internal void CustomCodeProxyStart()
		{
			Global.Tracer.Assert(this.m_setCode, "(m_setCode)");
			this.m_currentTypeDecl = new CustomCodeProxyDecl(this.m_currentTypeDecl);
			this.m_currentTypeDecl.HasExpressions = true;
		}

		internal void CustomCodeProxyEnd()
		{
			this.m_rootTypeDecl.Type.Members.Add(this.m_currentTypeDecl.Type);
			this.TypeEnd(this.m_rootTypeDecl);
		}

		internal void CustomCodeClassInstance(string className, string instanceName, int id)
		{
			((CustomCodeProxyDecl)this.m_currentTypeDecl).AddClassInstance(className, instanceName, id);
		}

		internal void ReportCode(string code)
		{
			((CustomCodeProxyDecl)this.m_currentTypeDecl).AddCode(code);
		}

		internal void ReportParameterStart(string name)
		{
			this.TypeStart(name, "ReportParamExprHost");
		}

		internal int ReportParameterEnd()
		{
			this.ExprIndexerCreate();
			return this.TypeEnd((TypeDecl)this.m_rootTypeDecl, "m_reportParameterHostsRemotable", ref this.m_rootTypeDecl.ReportParameters);
		}

		internal void ReportParameterValidationExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ValidationExpressionExpr", expression);
		}

		internal void ReportParameterPromptExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PromptExpr", expression);
		}

		internal void ReportParameterDefaultValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void ReportParameterValidValuesStart()
		{
			this.TypeStart("ReportParameterValidValues", "IndexedExprHost");
		}

		internal void ReportParameterValidValuesEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "ValidValuesHost");
		}

		internal void ReportParameterValidValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void ReportParameterValidValueLabelsStart()
		{
			this.TypeStart("ReportParameterValidValueLabels", "IndexedExprHost");
		}

		internal void ReportParameterValidValueLabelsEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "ValidValueLabelsHost");
		}

		internal void ReportParameterValidValueLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void CalcFieldStart(string name)
		{
			this.TypeStart(name, "CalcFieldExprHost");
		}

		internal int CalcFieldEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_fieldHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).Fields);
		}

		internal void QueryParametersStart()
		{
			this.TypeStart("QueryParameters", "IndexedExprHost");
		}

		internal void QueryParametersEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "QueryParametersHost");
		}

		internal void QueryParameterValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void DataSourceStart(string name)
		{
			this.TypeStart(name, "DataSourceExprHost");
		}

		internal int DataSourceEnd()
		{
			return this.TypeEnd((TypeDecl)this.m_rootTypeDecl, "m_dataSourceHostsRemotable", ref this.m_rootTypeDecl.DataSources);
		}

		internal void DataSourceConnectString(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ConnectStringExpr", expression);
		}

		internal void DataSetStart(string name)
		{
			this.TypeStart(name, "DataSetExprHost");
		}

		internal int DataSetEnd()
		{
			return this.TypeEnd((TypeDecl)this.m_rootTypeDecl, "m_dataSetHostsRemotable", ref this.m_rootTypeDecl.DataSets);
		}

		internal void DataSetQueryCommandText(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("QueryCommandTextExpr", expression);
		}

		internal void PageSectionStart()
		{
			this.TypeStart(this.CreateTypeName("PageSection", this.m_rootTypeDecl.PageSections), "StyleExprHost");
		}

		internal int PageSectionEnd()
		{
			return this.TypeEnd((TypeDecl)this.m_rootTypeDecl, "m_pageSectionHostsRemotable", ref this.m_rootTypeDecl.PageSections);
		}

		internal void PageStart()
		{
			this.TypeStart(this.CreateTypeName("Page", this.m_rootTypeDecl.Pages), "StyleExprHost");
		}

		internal int PageEnd()
		{
			return this.TypeEnd((TypeDecl)this.m_rootTypeDecl, "m_pageHostsRemotable", ref this.m_rootTypeDecl.Pages);
		}

		internal void ReportSectionStart()
		{
			this.TypeStart(this.CreateTypeName("ReportSection", this.m_rootTypeDecl.ReportSections), "ReportSectionExprHost");
		}

		internal int ReportSectionEnd()
		{
			return this.TypeEnd((TypeDecl)this.m_rootTypeDecl, "m_reportSectionHostsRemotable", ref this.m_rootTypeDecl.ReportSections);
		}

		internal void ParameterOmit(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("OmitExpr", expression);
		}

		internal void StyleAttribute(string name, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd(name + "Expr", expression);
		}

		internal void ActionInfoStart()
		{
			this.TypeStart("ActionInfo", "ActionInfoExprHost");
		}

		internal void ActionInfoEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "ActionInfoHost");
		}

		internal void ActionStart()
		{
			this.TypeStart(this.CreateTypeName("Action", ((NonRootTypeDecl)this.m_currentTypeDecl).Actions), "ActionExprHost");
		}

		internal int ActionEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_actionItemHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).Actions);
		}

		internal void ActionHyperlink(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HyperlinkExpr", expression);
		}

		internal void ActionDrillThroughReportName(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DrillThroughReportNameExpr", expression);
		}

		internal void ActionDrillThroughBookmarkLink(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DrillThroughBookmarkLinkExpr", expression);
		}

		internal void ActionBookmarkLink(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("BookmarkLinkExpr", expression);
		}

		internal void ActionDrillThroughParameterStart()
		{
			this.ParameterStart();
		}

		internal int ActionDrillThroughParameterEnd()
		{
			return this.ParameterEnd("m_drillThroughParameterHostsRemotable");
		}

		internal void ReportItemBookmark(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("BookmarkExpr", expression);
		}

		internal void ReportItemToolTip(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ToolTipExpr", expression);
		}

		internal void LineStart(string name)
		{
			this.TypeStart(name, "ReportItemExprHost");
		}

		internal int LineEnd()
		{
			return this.ReportItemEnd("m_lineHostsRemotable", ref this.m_rootTypeDecl.Lines);
		}

		internal void RectangleStart(string name)
		{
			this.TypeStart(name, "ReportItemExprHost");
		}

		internal int RectangleEnd()
		{
			return this.ReportItemEnd("m_rectangleHostsRemotable", ref this.m_rootTypeDecl.Rectangles);
		}

		internal void TextBoxStart(string name)
		{
			this.TypeStart(name, "TextBoxExprHost");
		}

		internal int TextBoxEnd()
		{
			return this.ReportItemEnd("m_textBoxHostsRemotable", ref this.m_rootTypeDecl.TextBoxes);
		}

		internal void TextBoxToggleImageInitialState(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ToggleImageInitialStateExpr", expression);
		}

		internal void UserSortExpressionsStart()
		{
			this.TypeStart("UserSort", "IndexedExprHost");
		}

		internal void UserSortExpressionsEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "UserSortExpressionsHost");
		}

		internal void UserSortExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void ImageStart(string name)
		{
			this.TypeStart(name, "ImageExprHost");
		}

		internal int ImageEnd()
		{
			return this.ReportItemEnd("m_imageHostsRemotable", ref this.m_rootTypeDecl.Images);
		}

		internal void ImageMIMEType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MIMETypeExpr", expression);
		}

		internal void ImageTag(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TagExpr", expression);
		}

		internal void SubreportStart(string name)
		{
			this.TypeStart(name, "SubreportExprHost");
		}

		internal int SubreportEnd()
		{
			return this.ReportItemEnd("m_subreportHostsRemotable", ref this.m_rootTypeDecl.Subreports);
		}

		internal void SubreportParameterStart()
		{
			this.ParameterStart();
		}

		internal int SubreportParameterEnd()
		{
			return this.ParameterEnd("m_parameterHostsRemotable");
		}

		internal void SortStart()
		{
			this.TypeStart("Sort", "SortExprHost");
		}

		internal void SortEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "m_sortHost");
		}

		internal void SortExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void SortDirectionsStart()
		{
			this.TypeStart("SortDirections", "IndexedExprHost");
		}

		internal void SortDirectionsEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "SortDirectionHosts");
		}

		internal void SortDirection(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void FilterStart()
		{
			this.TypeStart(this.CreateTypeName("Filter", ((NonRootTypeDecl)this.m_currentTypeDecl).Filters), "FilterExprHost");
		}

		internal int FilterEnd()
		{
			this.ExprIndexerCreate();
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_filterHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).Filters);
		}

		internal void FilterExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("FilterExpressionExpr", expression);
		}

		internal void FilterValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void GroupStart(string typeName)
		{
			this.TypeStart(typeName, "GroupExprHost");
		}

		internal void GroupEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "m_groupHost");
		}

		internal void GroupExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void GroupParentExpressionsStart()
		{
			this.TypeStart("Parent", "IndexedExprHost");
		}

		internal void GroupParentExpressionsEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "ParentExpressionsHost");
		}

		internal void GroupParentExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void ReGroupExpressionsStart()
		{
			this.TypeStart("ReGroup", "IndexedExprHost");
		}

		internal void ReGroupExpressionsEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "ReGroupExpressionsHost");
		}

		internal void ReGroupExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void VariableValuesStart()
		{
			this.TypeStart("Variables", "IndexedExprHost");
		}

		internal void VariableValuesEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "VariableValueHosts");
		}

		internal void VariableValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void DataRegionStart(DataRegionMode mode, string dataregionName)
		{
			switch (mode)
			{
			case DataRegionMode.Tablix:
				this.TypeStart(dataregionName, "TablixExprHost");
				break;
			case DataRegionMode.DataShape:
				this.TypeStart(dataregionName, "DataShapeExprHost");
				break;
			case DataRegionMode.Chart:
				this.TypeStart(dataregionName, "ChartExprHost");
				break;
			case DataRegionMode.GaugePanel:
				this.TypeStart(dataregionName, "GaugePanelExprHost");
				break;
			case DataRegionMode.CustomReportItem:
				this.TypeStart(dataregionName, "CustomReportItemExprHost");
				break;
			case DataRegionMode.MapDataRegion:
				this.TypeStart(dataregionName, "MapDataRegionExprHost");
				break;
			}
		}

		internal int DataRegionEnd(DataRegionMode mode)
		{
			int result = -1;
			switch (mode)
			{
			case DataRegionMode.Tablix:
				result = this.ReportItemEnd("m_tablixHostsRemotable", ref this.m_rootTypeDecl.Tablices);
				break;
			case DataRegionMode.DataShape:
				result = this.ReportItemEnd("DataShapeExprHost", ref this.m_rootTypeDecl.DataShapes);
				break;
			case DataRegionMode.Chart:
				result = this.ReportItemEnd("m_chartHostsRemotable", ref this.m_rootTypeDecl.Charts);
				break;
			case DataRegionMode.GaugePanel:
				result = this.ReportItemEnd("m_gaugePanelHostsRemotable", ref this.m_rootTypeDecl.GaugePanels);
				break;
			case DataRegionMode.CustomReportItem:
				result = this.ReportItemEnd("m_customReportItemHostsRemotable", ref this.m_rootTypeDecl.CustomReportItems);
				break;
			case DataRegionMode.MapDataRegion:
				result = this.ReportItemEnd("m_mapDataRegionHostsRemotable", ref this.m_rootTypeDecl.CustomReportItems);
				break;
			}
			return result;
		}

		internal void DataGroupStart(DataRegionMode mode, bool column)
		{
			string str = column ? "Column" : "Row";
			switch (mode)
			{
			case DataRegionMode.Tablix:
				this.TypeStart(this.CreateTypeName("TablixMember" + str, ((NonRootTypeDecl)this.m_currentTypeDecl).TablixMembers), "TablixMemberExprHost");
				break;
			case DataRegionMode.DataShape:
				this.TypeStart(this.CreateTypeName("DataShapeMember" + str, ((NonRootTypeDecl)this.m_currentTypeDecl).DataShapeMembers), "DataShapeMemberExprHost");
				break;
			case DataRegionMode.Chart:
				this.TypeStart(this.CreateTypeName("ChartMember" + str, ((NonRootTypeDecl)this.m_currentTypeDecl).ChartMembers), "ChartMemberExprHost");
				break;
			case DataRegionMode.GaugePanel:
				this.TypeStart(this.CreateTypeName("GaugeMember" + str, ((NonRootTypeDecl)this.m_currentTypeDecl).GaugeMembers), "GaugeMemberExprHost");
				break;
			case DataRegionMode.CustomReportItem:
				this.TypeStart(this.CreateTypeName("DataGroup" + str, ((NonRootTypeDecl)this.m_currentTypeDecl).DataGroups), "DataGroupExprHost");
				break;
			case DataRegionMode.MapDataRegion:
				this.TypeStart(this.CreateTypeName("MapMember" + str, ((NonRootTypeDecl)this.m_currentTypeDecl).MapMembers), "MapMemberExprHost");
				break;
			}
		}

		internal int DataGroupEnd(DataRegionMode mode, bool column)
		{
			switch (mode)
			{
			case DataRegionMode.Tablix:
				return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_memberTreeHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).TablixMembers);
			case DataRegionMode.DataShape:
				return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_memberTreeHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).DataShapeMembers);
			case DataRegionMode.Chart:
				return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_memberTreeHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).ChartMembers);
			case DataRegionMode.GaugePanel:
				return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_memberTreeHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).GaugeMembers);
			case DataRegionMode.CustomReportItem:
				return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_memberTreeHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).DataGroups);
			case DataRegionMode.MapDataRegion:
				return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_memberTreeHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).MapMembers);
			default:
				return -1;
			}
		}

		internal void DataCellStart(DataRegionMode mode)
		{
			switch (mode)
			{
			case DataRegionMode.MapDataRegion:
				break;
			case DataRegionMode.Tablix:
				this.TypeStart(this.CreateTypeName("TablixCell", ((NonRootTypeDecl)this.m_currentTypeDecl).TablixCells), "TablixCellExprHost");
				break;
			case DataRegionMode.DataShape:
				this.TypeStart(this.CreateTypeName("DataShapeIntersection", ((NonRootTypeDecl)this.m_currentTypeDecl).DataShapeIntersections), "DataShapeIntersectionExprHost");
				break;
			case DataRegionMode.Chart:
				this.TypeStart(this.CreateTypeName("ChartDataPoint", ((NonRootTypeDecl)this.m_currentTypeDecl).DataPoints), "ChartDataPointExprHost");
				break;
			case DataRegionMode.GaugePanel:
				this.TypeStart(this.CreateTypeName("GaugeCell", ((NonRootTypeDecl)this.m_currentTypeDecl).GaugeCells), "GaugeCellExprHost");
				break;
			case DataRegionMode.CustomReportItem:
				this.TypeStart(this.CreateTypeName("DataCell", ((NonRootTypeDecl)this.m_currentTypeDecl).DataCells), "DataCellExprHost");
				break;
			}
		}

		internal int DataCellEnd(DataRegionMode mode)
		{
			switch (mode)
			{
			case DataRegionMode.Tablix:
				return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_cellHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).TablixCells);
			case DataRegionMode.DataShape:
				return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_cellHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).DataShapeIntersections);
			case DataRegionMode.Chart:
				return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_cellHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).DataPoints);
			case DataRegionMode.GaugePanel:
				return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_cellHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).GaugeCells);
			case DataRegionMode.CustomReportItem:
				return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_cellHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).DataCells);
			default:
				return -1;
			}
		}

		internal void MarginExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, string marginPosition)
		{
			this.ExpressionAdd(marginPosition + "Expr", expression);
		}

		internal void ChartTitleStart(string titleName)
		{
			this.TypeStart(this.CreateTypeName("ChartTitle" + titleName, ((NonRootTypeDecl)this.m_currentTypeDecl).ChartTitles), "ChartTitleExprHost");
		}

		internal void ChartTitlePosition(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ChartTitlePositionExpr", expression);
		}

		internal void ChartTitleHidden(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HiddenExpr", expression);
		}

		internal void ChartTitleDocking(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DockingExpr", expression);
		}

		internal void ChartTitleDockOffset(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DockingOffsetExpr", expression);
		}

		internal void ChartTitleDockOutsideChartArea(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DockOutsideChartAreaExpr", expression);
		}

		internal void ChartTitleToolTip(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ToolTipExpr", expression);
		}

		internal void ChartTitleTextOrientation(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TextOrientationExpr", expression);
		}

		internal int ChartTitleEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_titlesHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).ChartTitles);
		}

		internal void ChartNoDataMessageStart()
		{
			this.TypeStart("ChartTitle", "ChartTitleExprHost");
		}

		internal void ChartNoDataMessageEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "NoDataMessageHost");
		}

		internal void ChartCaption(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("CaptionExpr", expression);
		}

		internal void ChartAxisTitleStart()
		{
			this.TypeStart("ChartAxisTitle", "ChartAxisTitleExprHost");
		}

		internal void ChartAxisTitleTextOrientation(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TextOrientationExpr", expression);
		}

		internal void ChartAxisTitleEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "TitleHost");
		}

		internal void ChartLegendTitleStart()
		{
			this.TypeStart("ChartLegendTitle", "ChartLegendTitleExprHost");
		}

		internal void ChartLegendTitleSeparator(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TitleSeparatorExpr", expression);
		}

		internal void ChartLegendTitleEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "TitleExprHost");
		}

		internal void AxisMin(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AxisMinExpr", expression);
		}

		internal void AxisMax(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AxisMaxExpr", expression);
		}

		internal void AxisCrossAt(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AxisCrossAtExpr", expression);
		}

		internal void AxisMajorInterval(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AxisMajorIntervalExpr", expression);
		}

		internal void AxisMinorInterval(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AxisMinorIntervalExpr", expression);
		}

		internal void ChartPalette(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PaletteExpr", expression);
		}

		internal void ChartPaletteHatchBehavior(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PaletteHatchBehaviorExpr", expression);
		}

		internal void DynamicWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DynamicWidthExpr", expression);
		}

		internal void DynamicHeight(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DynamicHeightExpr", expression);
		}

		internal void ChartAxisStart(string axisName, bool isValueAxis)
		{
			if (isValueAxis)
			{
				this.TypeStart(this.CreateTypeName("ValueAxis" + axisName, ((NonRootTypeDecl)this.m_currentTypeDecl).ValueAxes), "ChartAxisExprHost");
			}
			else
			{
				this.TypeStart(this.CreateTypeName("CategoryAxis" + axisName, ((NonRootTypeDecl)this.m_currentTypeDecl).CategoryAxes), "ChartAxisExprHost");
			}
		}

		internal void ChartAxisVisible(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("VisibleExpr", expression);
		}

		internal void ChartAxisMargin(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MarginExpr", expression);
		}

		internal void ChartAxisInterval(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalExpr", expression);
		}

		internal void ChartAxisIntervalType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalTypeExpr", expression);
		}

		internal void ChartAxisIntervalOffset(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalOffsetExpr", expression);
		}

		internal void ChartAxisIntervalOffsetType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalOffsetTypeExpr", expression);
		}

		internal void ChartAxisMarksAlwaysAtPlotEdge(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MarksAlwaysAtPlotEdgeExpr", expression);
		}

		internal void ChartAxisReverse(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ReverseExpr", expression);
		}

		internal void ChartAxisLocation(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LocationExpr", expression);
		}

		internal void ChartAxisInterlaced(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("InterlacedExpr", expression);
		}

		internal void ChartAxisInterlacedColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("InterlacedColorExpr", expression);
		}

		internal void ChartAxisLogScale(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LogScaleExpr", expression);
		}

		internal void ChartAxisLogBase(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LogBaseExpr", expression);
		}

		internal void ChartAxisHideLabels(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HideLabelsExpr", expression);
		}

		internal void ChartAxisAngle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AngleExpr", expression);
		}

		internal void ChartAxisArrows(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ArrowsExpr", expression);
		}

		internal void ChartAxisPreventFontShrink(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PreventFontShrinkExpr", expression);
		}

		internal void ChartAxisPreventFontGrow(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PreventFontGrowExpr", expression);
		}

		internal void ChartAxisPreventLabelOffset(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PreventLabelOffsetExpr", expression);
		}

		internal void ChartAxisPreventWordWrap(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PreventWordWrapExpr", expression);
		}

		internal void ChartAxisAllowLabelRotation(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AllowLabelRotationExpr", expression);
		}

		internal void ChartAxisIncludeZero(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IncludeZeroExpr", expression);
		}

		internal void ChartAxisLabelsAutoFitDisabled(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LabelsAutoFitDisabledExpr", expression);
		}

		internal void ChartAxisMinFontSize(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MinFontSizeExpr", expression);
		}

		internal void ChartAxisMaxFontSize(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MaxFontSizeExpr", expression);
		}

		internal void ChartAxisOffsetLabels(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("OffsetLabelsExpr", expression);
		}

		internal void ChartAxisHideEndLabels(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HideEndLabelsExpr", expression);
		}

		internal void ChartAxisVariableAutoInterval(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("VariableAutoIntervalExpr", expression);
		}

		internal void ChartAxisLabelInterval(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LabelIntervalExpr", expression);
		}

		internal void ChartAxisLabelIntervalType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LabelIntervalTypeExpr", expression);
		}

		internal void ChartAxisLabelIntervalOffset(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LabelIntervalOffsetExpr", expression);
		}

		internal void ChartAxisLabelIntervalOffsetType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LabelIntervalOffsetTypeExpr", expression);
		}

		internal int ChartAxisEnd(bool isValueAxis)
		{
			if (isValueAxis)
			{
				return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_valueAxesHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).ValueAxes);
			}
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_categoryAxesHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).CategoryAxes);
		}

		internal void ChartGridLinesStart(bool isMajor)
		{
			this.TypeStart("ChartGridLines" + (isMajor ? "MajorGridLinesHost" : "MinorGridLinesHost"), "ChartGridLinesExprHost");
		}

		internal void ChartGridLinesEnd(bool isMajor)
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, isMajor ? "MajorGridLinesHost" : "MinorGridLinesHost");
		}

		internal void ChartGridLinesIntervalOffsetType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalOffsetTypeExpr", expression);
		}

		internal void ChartGridLinesIntervalOffset(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalOffsetExpr", expression);
		}

		internal void ChartGridLinesEnabledIntervalType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalTypeExpr", expression);
		}

		internal void ChartGridLinesInterval(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalExpr", expression);
		}

		internal void ChartGridLinesEnabled(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("EnabledExpr", expression);
		}

		internal void ChartLegendStart(string legendName)
		{
			this.TypeStart(this.CreateTypeName("ChartLegend" + legendName, ((NonRootTypeDecl)this.m_currentTypeDecl).ChartLegends), "ChartLegendExprHost");
		}

		internal void ChartLegendHidden(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HiddenExpr", expression);
		}

		internal void ChartLegendPosition(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ChartLegendPositionExpr", expression);
		}

		internal void ChartLegendLayout(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LayoutExpr", expression);
		}

		internal void ChartLegendDockOutsideChartArea(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DockOutsideChartAreaExpr", expression);
		}

		internal void ChartLegendAutoFitTextDisabled(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AutoFitTextDisabledExpr", expression);
		}

		internal void ChartLegendMinFontSize(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MinFontSizeExpr", expression);
		}

		internal void ChartLegendHeaderSeparator(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HeaderSeparatorExpr", expression);
		}

		internal void ChartLegendHeaderSeparatorColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HeaderSeparatorColorExpr", expression);
		}

		internal void ChartLegendColumnSeparator(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ColumnSeparatorExpr", expression);
		}

		internal void ChartLegendColumnSeparatorColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ColumnSeparatorColorExpr", expression);
		}

		internal void ChartLegendColumnSpacing(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ColumnSpacingExpr", expression);
		}

		internal void ChartLegendInterlacedRows(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("InterlacedRowsExpr", expression);
		}

		internal void ChartLegendInterlacedRowsColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("InterlacedRowsColorExpr", expression);
		}

		internal void ChartLegendEquallySpacedItems(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("EquallySpacedItemsExpr", expression);
		}

		internal void ChartLegendReversed(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ReversedExpr", expression);
		}

		internal void ChartLegendMaxAutoSize(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MaxAutoSizeExpr", expression);
		}

		internal void ChartLegendTextWrapThreshold(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TextWrapThresholdExpr", expression);
		}

		internal int ChartLegendEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_legendsHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).ChartLegends);
		}

		internal void ChartSeriesStart()
		{
			this.TypeStart("ChartSeries", "ChartSeriesExprHost");
		}

		internal void ChartSeriesType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TypeExpr", expression);
		}

		internal void ChartSeriesSubtype(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SubtypeExpr", expression);
		}

		internal void ChartSeriesLegendName(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LegendNameExpr", expression);
		}

		internal void ChartSeriesLegendText(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LegendTextExpr", expression);
		}

		internal void ChartSeriesChartAreaName(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ChartAreaNameExpr", expression);
		}

		internal void ChartSeriesValueAxisName(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ValueAxisNameExpr", expression);
		}

		internal void ChartSeriesCategoryAxisName(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("CategoryAxisNameExpr", expression);
		}

		internal void ChartSeriesHidden(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HiddenExpr", expression);
		}

		internal void ChartSeriesHideInLegend(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HideInLegendExpr", expression);
		}

		internal void ChartSeriesToolTip(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ToolTipExpr", expression);
		}

		internal void ChartSeriesEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "ChartSeriesHost");
		}

		internal void ChartNoMoveDirectionsStart()
		{
			this.TypeStart("ChartNoMoveDirections", "ChartNoMoveDirectionsExprHost");
		}

		internal void ChartNoMoveDirectionsUp(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("UpExpr", expression);
		}

		internal void ChartNoMoveDirectionsDown(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DownExpr", expression);
		}

		internal void ChartNoMoveDirectionsLeft(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LeftExpr", expression);
		}

		internal void ChartNoMoveDirectionsRight(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("RightExpr", expression);
		}

		internal void ChartNoMoveDirectionsUpLeft(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("UpLeftExpr", expression);
		}

		internal void ChartNoMoveDirectionsUpRight(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("UpRightExpr", expression);
		}

		internal void ChartNoMoveDirectionsDownLeft(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DownLeftExpr", expression);
		}

		internal void ChartNoMoveDirectionsDownRight(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DownRightExpr", expression);
		}

		internal void ChartNoMoveDirectionsEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "NoMoveDirectionsHost");
		}

		internal void ChartElementPositionStart(bool innerPlot)
		{
			this.TypeStart(innerPlot ? "ChartInnerPlotPosition" : "ChartElementPosition", "ChartElementPositionExprHost");
		}

		internal void ChartElementPositionEnd(bool innerPlot)
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, innerPlot ? "ChartInnerPlotPositionHost" : "ChartElementPositionHost");
		}

		internal void ChartElementPositionTop(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TopExpr", expression);
		}

		internal void ChartElementPositionLeft(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LeftExpr", expression);
		}

		internal void ChartElementPositionHeight(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HeightExpr", expression);
		}

		internal void ChartElementPositionWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("WidthExpr", expression);
		}

		internal void ChartSmartLabelStart()
		{
			this.TypeStart("ChartSmartLabel", "ChartSmartLabelExprHost");
		}

		internal void ChartSmartLabelAllowOutSidePlotArea(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AllowOutSidePlotAreaExpr", expression);
		}

		internal void ChartSmartLabelCalloutBackColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("CalloutBackColorExpr", expression);
		}

		internal void ChartSmartLabelCalloutLineAnchor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("CalloutLineAnchorExpr", expression);
		}

		internal void ChartSmartLabelCalloutLineColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("CalloutLineColorExpr", expression);
		}

		internal void ChartSmartLabelCalloutLineStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("CalloutLineStyleExpr", expression);
		}

		internal void ChartSmartLabelCalloutLineWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("CalloutLineWidthExpr", expression);
		}

		internal void ChartSmartLabelCalloutStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("CalloutStyleExpr", expression);
		}

		internal void ChartSmartLabelShowOverlapped(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ShowOverlappedExpr", expression);
		}

		internal void ChartSmartLabelMarkerOverlapping(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MarkerOverlappingExpr", expression);
		}

		internal void ChartSmartLabelDisabled(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DisabledExpr", expression);
		}

		internal void ChartSmartLabelMaxMovingDistance(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MaxMovingDistanceExpr", expression);
		}

		internal void ChartSmartLabelMinMovingDistance(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MinMovingDistanceExpr", expression);
		}

		internal void ChartSmartLabelEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "SmartLabelHost");
		}

		internal void ChartAxisScaleBreakStart()
		{
			this.TypeStart("ChartAxisScaleBreak", "ChartAxisScaleBreakExprHost");
		}

		internal void ChartAxisScaleBreakEnabled(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("EnabledExpr", expression);
		}

		internal void ChartAxisScaleBreakBreakLineType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("BreakLineTypeExpr", expression);
		}

		internal void ChartAxisScaleBreakCollapsibleSpaceThreshold(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("CollapsibleSpaceThresholdExpr", expression);
		}

		internal void ChartAxisScaleBreakMaxNumberOfBreaks(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MaxNumberOfBreaksExpr", expression);
		}

		internal void ChartAxisScaleBreakSpacing(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SpacingExpr", expression);
		}

		internal void ChartAxisScaleBreakIncludeZero(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IncludeZeroExpr", expression);
		}

		internal void ChartAxisScaleBreakEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "AxisScaleBreakHost");
		}

		internal void ChartBorderSkinStart()
		{
			this.TypeStart("ChartBorderSkin", "ChartBorderSkinExprHost");
		}

		internal void ChartBorderSkinBorderSkinType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("BorderSkinTypeExpr", expression);
		}

		internal void ChartBorderSkinEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "BorderSkinHost");
		}

		internal void ChartItemInLegendStart()
		{
			this.TypeStart("ChartItemInLegend", "ChartDataPointInLegendExprHost");
		}

		internal void ChartItemInLegendLegendText(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LegendTextExpr", expression);
		}

		internal void ChartItemInLegendToolTip(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ToolTipExpr", expression);
		}

		internal void ChartItemInLegendHidden(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HiddenExpr", expression);
		}

		internal void ChartItemInLegendEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "DataPointInLegendHost");
		}

		internal void ChartTickMarksStart(bool isMajor)
		{
			this.TypeStart("ChartTickMarks" + (isMajor ? "MajorTickMarksHost" : "MinorTickMarksHost"), "ChartTickMarksExprHost");
		}

		internal void ChartTickMarksEnd(bool isMajor)
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, isMajor ? "MajorTickMarksHost" : "MinorTickMarksHost");
		}

		internal void ChartTickMarksEnabled(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("EnabledExpr", expression);
		}

		internal void ChartTickMarksType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TypeExpr", expression);
		}

		internal void ChartTickMarksLength(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LengthExpr", expression);
		}

		internal void ChartTickMarksInterval(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalExpr", expression);
		}

		internal void ChartTickMarksIntervalType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalTypeExpr", expression);
		}

		internal void ChartTickMarksIntervalOffset(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalOffsetExpr", expression);
		}

		internal void ChartTickMarksIntervalOffsetType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalOffsetTypeExpr", expression);
		}

		internal void ChartEmptyPointsStart()
		{
			this.TypeStart("ChartEmptyPoints", "ChartEmptyPointsExprHost");
		}

		internal void ChartEmptyPointsAxisLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AxisLabelExpr", expression);
		}

		internal void ChartEmptyPointsToolTip(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ToolTipExpr", expression);
		}

		internal void ChartEmptyPointsEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "EmptyPointsHost");
		}

		internal void ChartLegendColumnHeaderStart()
		{
			this.TypeStart("ChartLegendColumnHeader", "ChartLegendColumnHeaderExprHost");
		}

		internal void ChartLegendColumnHeaderValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ValueExpr", expression);
		}

		internal void ChartLegendColumnHeaderEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "ChartLegendColumnHeaderHost");
		}

		internal void ChartCustomPaletteColorStart(int index)
		{
			this.TypeStart(this.CreateTypeName("ChartCustomPaletteColor" + index.ToString(CultureInfo.InvariantCulture), ((NonRootTypeDecl)this.m_currentTypeDecl).ChartCustomPaletteColors), "ChartCustomPaletteColorExprHost");
		}

		internal int ChartCustomPaletteColorEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_customPaletteColorHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).ChartCustomPaletteColors);
		}

		internal void ChartCustomPaletteColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ColorExpr", expression);
		}

		internal void ChartLegendCustomItemCellStart(string name)
		{
			this.TypeStart(this.CreateTypeName("ChartLegendCustomItemCell" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).ChartLegendCustomItemCells), "ChartLegendCustomItemCellExprHost");
		}

		internal void ChartLegendCustomItemCellCellType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("CellTypeExpr", expression);
		}

		internal void ChartLegendCustomItemCellText(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TextExpr", expression);
		}

		internal void ChartLegendCustomItemCellCellSpan(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("CellSpanExpr", expression);
		}

		internal void ChartLegendCustomItemCellToolTip(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ToolTipExpr", expression);
		}

		internal void ChartLegendCustomItemCellImageWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ImageWidthExpr", expression);
		}

		internal void ChartLegendCustomItemCellImageHeight(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ImageHeightExpr", expression);
		}

		internal void ChartLegendCustomItemCellSymbolHeight(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SymbolHeightExpr", expression);
		}

		internal void ChartLegendCustomItemCellSymbolWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SymbolWidthExpr", expression);
		}

		internal void ChartLegendCustomItemCellAlignment(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AlignmentExpr", expression);
		}

		internal void ChartLegendCustomItemCellTopMargin(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TopMarginExpr", expression);
		}

		internal void ChartLegendCustomItemCellBottomMargin(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("BottomMarginExpr", expression);
		}

		internal void ChartLegendCustomItemCellLeftMargin(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LeftMarginExpr", expression);
		}

		internal void ChartLegendCustomItemCellRightMargin(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("RightMarginExpr", expression);
		}

		internal int ChartLegendCustomItemCellEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_legendCustomItemCellHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).ChartLegendCustomItemCells);
		}

		internal void ChartDerivedSeriesStart(int index)
		{
			this.TypeStart(this.CreateTypeName("ChartDerivedSeries" + index.ToString(CultureInfo.InvariantCulture), ((NonRootTypeDecl)this.m_currentTypeDecl).ChartDerivedSeriesCollection), "ChartDerivedSeriesExprHost");
		}

		internal void ChartDerivedSeriesSourceChartSeriesName(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SourceChartSeriesNameExpr", expression);
		}

		internal void ChartDerivedSeriesDerivedSeriesFormula(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DerivedSeriesFormulaExpr", expression);
		}

		internal int ChartDerivedSeriesEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_derivedSeriesCollectionHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).ChartDerivedSeriesCollection);
		}

		internal void ChartStripLineStart(int index)
		{
			this.TypeStart(this.CreateTypeName("ChartStripLine" + index.ToString(CultureInfo.InvariantCulture), ((NonRootTypeDecl)this.m_currentTypeDecl).ChartStripLines), "ChartStripLineExprHost");
		}

		internal void ChartStripLineTitle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TitleExpr", expression);
		}

		internal void ChartStripLineTitleAngle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TitleAngleExpr", expression);
		}

		internal void ChartStripLineTextOrientation(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TextOrientationExpr", expression);
		}

		internal void ChartStripLineToolTip(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ToolTipExpr", expression);
		}

		internal void ChartStripLineInterval(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalExpr", expression);
		}

		internal void ChartStripLineIntervalType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalTypeExpr", expression);
		}

		internal void ChartStripLineIntervalOffset(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalOffsetExpr", expression);
		}

		internal void ChartStripLineIntervalOffsetType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalOffsetTypeExpr", expression);
		}

		internal void ChartStripLineStripWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("StripWidthExpr", expression);
		}

		internal void ChartStripLineStripWidthType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("StripWidthTypeExpr", expression);
		}

		internal int ChartStripLineEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_stripLinesHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).ChartStripLines);
		}

		internal void ChartFormulaParameterStart(string name)
		{
			this.TypeStart(this.CreateTypeName("ChartFormulaParameter" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).ChartFormulaParameters), "ChartFormulaParameterExprHost");
		}

		internal void ChartFormulaParameterValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ValueExpr", expression);
		}

		internal int ChartFormulaParameterEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_formulaParametersHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).ChartFormulaParameters);
		}

		internal void ChartLegendColumnStart(string name)
		{
			this.TypeStart(this.CreateTypeName("ChartLegendColumn" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).ChartLegendColumns), "ChartLegendColumnExprHost");
		}

		internal void ChartLegendColumnColumnType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ColumnTypeExpr", expression);
		}

		internal void ChartLegendColumnValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ValueExpr", expression);
		}

		internal void ChartLegendColumnToolTip(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ToolTipExpr", expression);
		}

		internal void ChartLegendColumnMinimumWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MinimumWidthExpr", expression);
		}

		internal void ChartLegendColumnMaximumWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MaximumWidthExpr", expression);
		}

		internal void ChartLegendColumnSeriesSymbolWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SeriesSymbolWidthExpr", expression);
		}

		internal void ChartLegendColumnSeriesSymbolHeight(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SeriesSymbolHeightExpr", expression);
		}

		internal int ChartLegendColumnEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_legendColumnsHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).ChartLegendColumns);
		}

		internal void ChartLegendCustomItemStart(string name)
		{
			this.TypeStart(this.CreateTypeName("ChartLegendCustomItem" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).ChartLegendCustomItems), "ChartLegendCustomItemExprHost");
		}

		internal void ChartLegendCustomItemSeparator(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SeparatorExpr", expression);
		}

		internal void ChartLegendCustomItemSeparatorColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SeparatorColorExpr", expression);
		}

		internal void ChartLegendCustomItemToolTip(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ToolTipExpr", expression);
		}

		internal int ChartLegendCustomItemEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_legendCustomItemsHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).ChartLegendCustomItems);
		}

		internal void ChartAreaStart(string chartAreaName)
		{
			this.TypeStart(this.CreateTypeName("ChartArea" + chartAreaName, ((NonRootTypeDecl)this.m_currentTypeDecl).ChartAreas), "ChartAreaExprHost");
		}

		internal void Chart3DPropertiesStart()
		{
			this.TypeStart("Chart3DProperties", "Chart3DPropertiesExprHost");
		}

		internal void Chart3DPropertiesEnabled(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("EnabledExpr", expression);
		}

		internal void Chart3DPropertiesRotation(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("RotationExpr", expression);
		}

		internal void Chart3DPropertiesProjectionMode(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ProjectionModeExpr", expression);
		}

		internal void Chart3DPropertiesInclination(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("InclinationExpr", expression);
		}

		internal void Chart3DPropertiesPerspective(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PerspectiveExpr", expression);
		}

		internal void Chart3DPropertiesDepthRatio(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DepthRatioExpr", expression);
		}

		internal void Chart3DPropertiesShading(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ShadingExpr", expression);
		}

		internal void Chart3DPropertiesGapDepth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("GapDepthExpr", expression);
		}

		internal void Chart3DPropertiesWallThickness(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("WallThicknessExpr", expression);
		}

		internal void Chart3DPropertiesClustered(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ClusteredExpr", expression);
		}

		internal void Chart3DPropertiesEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "Chart3DPropertiesHost");
		}

		internal void ChartAreaHidden(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HiddenExpr", expression);
		}

		internal void ChartAreaAlignOrientation(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AlignOrientationExpr", expression);
		}

		internal void ChartAreaEquallySizedAxesFont(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("EquallySizedAxesFontExpr", expression);
		}

		internal void ChartAlignTypePosition(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ChartAlignTypePositionExpr", expression);
		}

		internal void ChartAlignTypeInnerPlotPosition(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("InnerPlotPositionExpr", expression);
		}

		internal void ChartAlignTypCursor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("CursorExpr", expression);
		}

		internal void ChartAlignTypeAxesView(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AxesViewExpr", expression);
		}

		internal int ChartAreaEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_chartAreasHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).ChartAreas);
		}

		internal void ChartDataPointValueX(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DataPointValuesXExpr", expression);
		}

		internal void ChartDataPointValueY(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DataPointValuesYExpr", expression);
		}

		internal void ChartDataPointValueSize(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DataPointValuesSizeExpr", expression);
		}

		internal void ChartDataPointValueHigh(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DataPointValuesHighExpr", expression);
		}

		internal void ChartDataPointValueLow(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DataPointValuesLowExpr", expression);
		}

		internal void ChartDataPointValueStart(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DataPointValuesStartExpr", expression);
		}

		internal void ChartDataPointValueEnd(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DataPointValuesEndExpr", expression);
		}

		internal void ChartDataPointValueMean(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DataPointValuesMeanExpr", expression);
		}

		internal void ChartDataPointValueMedian(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DataPointValuesMedianExpr", expression);
		}

		internal void ChartDataPointValueHighlightX(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DataPointValuesHighlightXExpr", expression);
		}

		internal void ChartDataPointValueHighlightY(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DataPointValuesHighlightYExpr", expression);
		}

		internal void ChartDataPointValueHighlightSize(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DataPointValuesHighlightSizeExpr", expression);
		}

		internal void ChartDataPointValueFormatX(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ChartDataPointValueFormatXExpr", expression);
		}

		internal void ChartDataPointValueFormatY(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ChartDataPointValueFormatYExpr", expression);
		}

		internal void ChartDataPointValueFormatSize(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ChartDataPointValueFormatSizeExpr", expression);
		}

		internal void ChartDataPointValueCurrencyLanguageX(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ChartDataPointValueCurrencyLanguageXExpr", expression);
		}

		internal void ChartDataPointValueCurrencyLanguageY(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ChartDataPointValueCurrencyLanguageYExpr", expression);
		}

		internal void ChartDataPointValueCurrencyLanguageSize(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ChartDataPointValueCurrencyLanguageSizeExpr", expression);
		}

		internal void ChartDataPointAxisLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AxisLabelExpr", expression);
		}

		internal void ChartDataPointToolTip(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ToolTipExpr", expression);
		}

		internal void DataLabelStart()
		{
			this.TypeStart("DataLabel", "ChartDataLabelExprHost");
		}

		internal void DataLabelLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LabelExpr", expression);
		}

		internal void DataLabelVisible(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("VisibleExpr", expression);
		}

		internal void DataLabelPosition(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ChartDataLabelPositionExpr", expression);
		}

		internal void DataLabelRotation(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("RotationExpr", expression);
		}

		internal void DataLabelUseValueAsLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("UseValueAsLabelExpr", expression);
		}

		internal void ChartDataLabelToolTip(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ToolTipExpr", expression);
		}

		internal void DataLabelEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "DataLabelHost");
		}

		internal void DataPointStyleStart()
		{
			this.StyleStart("Style");
		}

		internal void DataPointStyleEnd()
		{
			this.StyleEnd("StyleHost");
		}

		internal void DataPointMarkerStart()
		{
			this.TypeStart("ChartMarker", "ChartMarkerExprHost");
		}

		internal void DataPointMarkerSize(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SizeExpr", expression);
		}

		internal void DataPointMarkerType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TypeExpr", expression);
		}

		internal void DataPointMarkerEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "ChartMarkerHost");
		}

		internal void ChartMemberLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MemberLabelExpr", expression);
		}

		internal void ChartMemberStyleStart()
		{
			this.StyleStart("MemberStyle");
		}

		internal void ChartMemberStyleEnd()
		{
			this.StyleEnd("MemberStyleHost");
		}

		internal void DataValueStart()
		{
			this.TypeStart(this.CreateTypeName("DataValue", this.m_currentTypeDecl.DataValues), "DataValueExprHost");
		}

		internal int DataValueEnd(bool isCustomProperty)
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, isCustomProperty ? "m_customPropertyHostsRemotable" : "m_dataValueHostsRemotable", ref this.m_currentTypeDecl.Parent.DataValues);
		}

		internal void DataValueName(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DataValueNameExpr", expression);
		}

		internal void DataValueValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DataValueValueExpr", expression);
		}

		internal void BaseGaugeImageSource(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SourceExpr", expression);
		}

		internal void BaseGaugeImageValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ValueExpr", expression);
		}

		internal void BaseGaugeImageMIMEType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MIMETypeExpr", expression);
		}

		internal void BaseGaugeImageTransparentColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TransparentColorExpr", expression);
		}

		internal void CapImageStart()
		{
			this.TypeStart("CapImage", "CapImageExprHost");
		}

		internal void CapImageEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "CapImageHost");
		}

		internal void CapImageHueColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HueColorExpr", expression);
		}

		internal void CapImageOffsetX(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("OffsetXExpr", expression);
		}

		internal void CapImageOffsetY(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("OffsetYExpr", expression);
		}

		internal void FrameImageStart()
		{
			this.TypeStart("FrameImage", "FrameImageExprHost");
		}

		internal void FrameImageEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "FrameImageHost");
		}

		internal void FrameImageHueColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HueColorExpr", expression);
		}

		internal void FrameImageTransparency(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TransparencyExpr", expression);
		}

		internal void FrameImageClipImage(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ClipImageExpr", expression);
		}

		internal void PointerImageStart()
		{
			this.TypeStart("PointerImage", "PointerImageExprHost");
		}

		internal void PointerImageEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "PointerImageHost");
		}

		internal void PointerImageHueColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HueColorExpr", expression);
		}

		internal void PointerImageTransparency(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TransparencyExpr", expression);
		}

		internal void PointerImageOffsetX(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("OffsetXExpr", expression);
		}

		internal void PointerImageOffsetY(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("OffsetYExpr", expression);
		}

		internal void TopImageStart()
		{
			this.TypeStart("TopImage", "TopImageExprHost");
		}

		internal void TopImageEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "TopImageHost");
		}

		internal void TopImageHueColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HueColorExpr", expression);
		}

		internal void BackFrameStart()
		{
			this.TypeStart("BackFrame", "BackFrameExprHost");
		}

		internal void BackFrameEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "BackFrameHost");
		}

		internal void BackFrameFrameStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("FrameStyleExpr", expression);
		}

		internal void BackFrameFrameShape(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("FrameShapeExpr", expression);
		}

		internal void BackFrameFrameWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("FrameWidthExpr", expression);
		}

		internal void BackFrameGlassEffect(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("GlassEffectExpr", expression);
		}

		internal void FrameBackgroundStart()
		{
			this.TypeStart("FrameBackground", "FrameBackgroundExprHost");
		}

		internal void FrameBackgroundEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "FrameBackgroundHost");
		}

		internal void CustomLabelStart(string name)
		{
			this.TypeStart(this.CreateTypeName("CustomLabel" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).CustomLabels), "CustomLabelExprHost");
		}

		internal int CustomLabelEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_customLabelsHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).CustomLabels);
		}

		internal void CustomLabelText(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TextExpr", expression);
		}

		internal void CustomLabelAllowUpsideDown(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AllowUpsideDownExpr", expression);
		}

		internal void CustomLabelDistanceFromScale(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DistanceFromScaleExpr", expression);
		}

		internal void CustomLabelFontAngle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("FontAngleExpr", expression);
		}

		internal void CustomLabelPlacement(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PlacementExpr", expression);
		}

		internal void CustomLabelRotateLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("RotateLabelExpr", expression);
		}

		internal void CustomLabelValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ValueExpr", expression);
		}

		internal void CustomLabelHidden(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HiddenExpr", expression);
		}

		internal void CustomLabelUseFontPercent(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("UseFontPercentExpr", expression);
		}

		internal void GaugeClipContent(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ClipContentExpr", expression);
		}

		internal void GaugeImageStart(string name)
		{
			this.TypeStart(this.CreateTypeName("GaugeImage" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).GaugeImages), "GaugeImageExprHost");
		}

		internal int GaugeImageEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_gaugeImagesHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).GaugeImages);
		}

		internal void GaugeAspectRatio(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AspectRatioExpr", expression);
		}

		internal void GaugeInputValueStart(int index)
		{
			this.TypeStart(this.CreateTypeName("GaugeInputValue" + index.ToString(CultureInfo.InvariantCulture), ((NonRootTypeDecl)this.m_currentTypeDecl).GaugeInputValues), "GaugeInputValueExprHost");
		}

		internal int GaugeInputValueEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_gaugeInputValueHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).GaugeInputValues);
		}

		internal void GaugeInputValueValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ValueExpr", expression);
		}

		internal void GaugeInputValueFormula(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("FormulaExpr", expression);
		}

		internal void GaugeInputValueMinPercent(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MinPercentExpr", expression);
		}

		internal void GaugeInputValueMaxPercent(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MaxPercentExpr", expression);
		}

		internal void GaugeInputValueMultiplier(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MultiplierExpr", expression);
		}

		internal void GaugeInputValueAddConstant(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AddConstantExpr", expression);
		}

		internal void GaugeLabelStart(string name)
		{
			this.TypeStart(this.CreateTypeName("GaugeLabel" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).GaugeLabels), "GaugeLabelExprHost");
		}

		internal int GaugeLabelEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_gaugeLabelsHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).GaugeLabels);
		}

		internal void GaugeLabelText(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TextExpr", expression);
		}

		internal void GaugeLabelAngle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AngleExpr", expression);
		}

		internal void GaugeLabelResizeMode(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ResizeModeExpr", expression);
		}

		internal void GaugeLabelTextShadowOffset(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TextShadowOffsetExpr", expression);
		}

		internal void GaugeLabelUseFontPercent(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("UseFontPercentExpr", expression);
		}

		internal void GaugePanelAntiAliasing(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AntiAliasingExpr", expression);
		}

		internal void GaugePanelAutoLayout(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AutoLayoutExpr", expression);
		}

		internal void GaugePanelShadowIntensity(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ShadowIntensityExpr", expression);
		}

		internal void GaugePanelTextAntiAliasingQuality(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TextAntiAliasingQualityExpr", expression);
		}

		internal void GaugePanelItemTop(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TopExpr", expression);
		}

		internal void GaugePanelItemLeft(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LeftExpr", expression);
		}

		internal void GaugePanelItemHeight(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HeightExpr", expression);
		}

		internal void GaugePanelItemWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("WidthExpr", expression);
		}

		internal void GaugePanelItemZIndex(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ZIndexExpr", expression);
		}

		internal void GaugePanelItemHidden(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HiddenExpr", expression);
		}

		internal void GaugePanelItemToolTip(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ToolTipExpr", expression);
		}

		internal void GaugePointerBarStart(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("BarStartExpr", expression);
		}

		internal void GaugePointerDistanceFromScale(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DistanceFromScaleExpr", expression);
		}

		internal void GaugePointerMarkerLength(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MarkerLengthExpr", expression);
		}

		internal void GaugePointerMarkerStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MarkerStyleExpr", expression);
		}

		internal void GaugePointerPlacement(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PlacementExpr", expression);
		}

		internal void GaugePointerSnappingEnabled(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SnappingEnabledExpr", expression);
		}

		internal void GaugePointerSnappingInterval(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SnappingIntervalExpr", expression);
		}

		internal void GaugePointerToolTip(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ToolTipExpr", expression);
		}

		internal void GaugePointerHidden(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HiddenExpr", expression);
		}

		internal void GaugePointerWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("WidthExpr", expression);
		}

		internal void GaugeScaleInterval(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalExpr", expression);
		}

		internal void GaugeScaleIntervalOffset(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalOffsetExpr", expression);
		}

		internal void GaugeScaleLogarithmic(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LogarithmicExpr", expression);
		}

		internal void GaugeScaleLogarithmicBase(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LogarithmicBaseExpr", expression);
		}

		internal void GaugeScaleMultiplier(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MultiplierExpr", expression);
		}

		internal void GaugeScaleReversed(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ReversedExpr", expression);
		}

		internal void GaugeScaleTickMarksOnTop(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TickMarksOnTopExpr", expression);
		}

		internal void GaugeScaleToolTip(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ToolTipExpr", expression);
		}

		internal void GaugeScaleHidden(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HiddenExpr", expression);
		}

		internal void GaugeScaleWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("WidthExpr", expression);
		}

		internal void GaugeTickMarksStart(bool isMajor)
		{
			this.TypeStart("GaugeTickMarks" + (isMajor ? "GaugeMajorTickMarksHost" : "GaugeMinorTickMarksHost"), "GaugeTickMarksExprHost");
		}

		internal void GaugeTickMarksEnd(bool isMajor)
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, isMajor ? "GaugeMajorTickMarksHost" : "GaugeMinorTickMarksHost");
		}

		internal void TickMarkStyleStart()
		{
			this.TypeStart("TickMarkStyle", "TickMarkStyleExprHost");
		}

		internal void TickMarkStyleEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "TickMarkStyleHost");
		}

		internal void GaugeTickMarksInterval(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalExpr", expression);
		}

		internal void GaugeTickMarksIntervalOffset(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalOffsetExpr", expression);
		}

		internal void LinearGaugeStart(string name)
		{
			this.TypeStart(this.CreateTypeName("LinearGauge" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).LinearGauges), "LinearGaugeExprHost");
		}

		internal int LinearGaugeEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_linearGaugesHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).LinearGauges);
		}

		internal void LinearGaugeOrientation(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("OrientationExpr", expression);
		}

		internal void LinearPointerStart(string name)
		{
			this.TypeStart(this.CreateTypeName("LinearPointer" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).LinearPointers), "LinearPointerExprHost");
		}

		internal int LinearPointerEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_linearPointersHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).LinearPointers);
		}

		internal void LinearPointerType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TypeExpr", expression);
		}

		internal void LinearScaleStart(string name)
		{
			this.TypeStart(this.CreateTypeName("LinearScale" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).LinearScales), "LinearScaleExprHost");
		}

		internal int LinearScaleEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_linearScalesHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).LinearScales);
		}

		internal void LinearScaleStartMargin(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("StartMarginExpr", expression);
		}

		internal void LinearScaleEndMargin(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("EndMarginExpr", expression);
		}

		internal void LinearScalePosition(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PositionExpr", expression);
		}

		internal void NumericIndicatorStart(string name)
		{
			this.TypeStart(this.CreateTypeName("NumericIndicator" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).NumericIndicators), "NumericIndicatorExprHost");
		}

		internal int NumericIndicatorEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_numericIndicatorsHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).NumericIndicators);
		}

		internal void NumericIndicatorDecimalDigitColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DecimalDigitColorExpr", expression);
		}

		internal void NumericIndicatorDigitColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DigitColorExpr", expression);
		}

		internal void NumericIndicatorUseFontPercent(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("UseFontPercentExpr", expression);
		}

		internal void NumericIndicatorDecimalDigits(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DecimalDigitsExpr", expression);
		}

		internal void NumericIndicatorDigits(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DigitsExpr", expression);
		}

		internal void NumericIndicatorMultiplier(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MultiplierExpr", expression);
		}

		internal void NumericIndicatorNonNumericString(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("NonNumericStringExpr", expression);
		}

		internal void NumericIndicatorOutOfRangeString(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("OutOfRangeStringExpr", expression);
		}

		internal void NumericIndicatorResizeMode(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ResizeModeExpr", expression);
		}

		internal void NumericIndicatorShowDecimalPoint(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ShowDecimalPointExpr", expression);
		}

		internal void NumericIndicatorShowLeadingZeros(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ShowLeadingZerosExpr", expression);
		}

		internal void NumericIndicatorIndicatorStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IndicatorStyleExpr", expression);
		}

		internal void NumericIndicatorShowSign(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ShowSignExpr", expression);
		}

		internal void NumericIndicatorSnappingEnabled(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SnappingEnabledExpr", expression);
		}

		internal void NumericIndicatorSnappingInterval(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SnappingIntervalExpr", expression);
		}

		internal void NumericIndicatorLedDimColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LedDimColorExpr", expression);
		}

		internal void NumericIndicatorSeparatorWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SeparatorWidthExpr", expression);
		}

		internal void NumericIndicatorSeparatorColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SeparatorColorExpr", expression);
		}

		internal void NumericIndicatorRangeStart(string name)
		{
			this.TypeStart(this.CreateTypeName("NumericIndicatorRange" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).NumericIndicatorRanges), "NumericIndicatorRangeExprHost");
		}

		internal int NumericIndicatorRangeEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_numericIndicatorRangesHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).NumericIndicatorRanges);
		}

		internal void NumericIndicatorRangeDecimalDigitColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DecimalDigitColorExpr", expression);
		}

		internal void NumericIndicatorRangeDigitColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DigitColorExpr", expression);
		}

		internal void PinLabelStart()
		{
			this.TypeStart("PinLabel", "PinLabelExprHost");
		}

		internal void PinLabelEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "PinLabelHost");
		}

		internal void PinLabelText(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TextExpr", expression);
		}

		internal void PinLabelAllowUpsideDown(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AllowUpsideDownExpr", expression);
		}

		internal void PinLabelDistanceFromScale(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DistanceFromScaleExpr", expression);
		}

		internal void PinLabelFontAngle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("FontAngleExpr", expression);
		}

		internal void PinLabelPlacement(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PlacementExpr", expression);
		}

		internal void PinLabelRotateLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("RotateLabelExpr", expression);
		}

		internal void PinLabelUseFontPercent(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("UseFontPercentExpr", expression);
		}

		internal void PointerCapStart()
		{
			this.TypeStart("PointerCap", "PointerCapExprHost");
		}

		internal void PointerCapEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "PointerCapHost");
		}

		internal void PointerCapOnTop(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("OnTopExpr", expression);
		}

		internal void PointerCapReflection(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ReflectionExpr", expression);
		}

		internal void PointerCapCapStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("CapStyleExpr", expression);
		}

		internal void PointerCapHidden(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HiddenExpr", expression);
		}

		internal void PointerCapWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("WidthExpr", expression);
		}

		internal void RadialGaugeStart(string name)
		{
			this.TypeStart(this.CreateTypeName("RadialGauge" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).RadialGauges), "RadialGaugeExprHost");
		}

		internal int RadialGaugeEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_radialGaugesHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).RadialGauges);
		}

		internal void RadialGaugePivotX(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PivotXExpr", expression);
		}

		internal void RadialGaugePivotY(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PivotYExpr", expression);
		}

		internal void RadialPointerStart(string name)
		{
			this.TypeStart(this.CreateTypeName("RadialPointer" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).RadialPointers), "RadialPointerExprHost");
		}

		internal int RadialPointerEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_radialPointersHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).RadialPointers);
		}

		internal void RadialPointerType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TypeExpr", expression);
		}

		internal void RadialPointerNeedleStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("NeedleStyleExpr", expression);
		}

		internal void RadialScaleStart(string name)
		{
			this.TypeStart(this.CreateTypeName("RadialScale" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).RadialScales), "RadialScaleExprHost");
		}

		internal int RadialScaleEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_radialScalesHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).RadialScales);
		}

		internal void RadialScaleRadius(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("RadiusExpr", expression);
		}

		internal void RadialScaleStartAngle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("StartAngleExpr", expression);
		}

		internal void RadialScaleSweepAngle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SweepAngleExpr", expression);
		}

		internal void ScaleLabelsStart()
		{
			this.TypeStart("ScaleLabels", "ScaleLabelsExprHost");
		}

		internal void ScaleLabelsEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "ScaleLabelsHost");
		}

		internal void ScaleLabelsInterval(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalExpr", expression);
		}

		internal void ScaleLabelsIntervalOffset(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalOffsetExpr", expression);
		}

		internal void ScaleLabelsAllowUpsideDown(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AllowUpsideDownExpr", expression);
		}

		internal void ScaleLabelsDistanceFromScale(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DistanceFromScaleExpr", expression);
		}

		internal void ScaleLabelsFontAngle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("FontAngleExpr", expression);
		}

		internal void ScaleLabelsPlacement(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PlacementExpr", expression);
		}

		internal void ScaleLabelsRotateLabels(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("RotateLabelsExpr", expression);
		}

		internal void ScaleLabelsShowEndLabels(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ShowEndLabelsExpr", expression);
		}

		internal void ScaleLabelsHidden(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HiddenExpr", expression);
		}

		internal void ScaleLabelsUseFontPercent(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("UseFontPercentExpr", expression);
		}

		internal void ScalePinStart(bool isMaximum)
		{
			this.TypeStart("ScalePin" + (isMaximum ? "MaximumPinHost" : "MinimumPinHost"), "ScalePinExprHost");
		}

		internal void ScalePinEnd(bool isMaximum)
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, isMaximum ? "MaximumPinHost" : "MinimumPinHost");
		}

		internal void ScalePinLocation(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LocationExpr", expression);
		}

		internal void ScalePinEnable(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("EnableExpr", expression);
		}

		internal void ScaleRangeStart(string name)
		{
			this.TypeStart(this.CreateTypeName("ScaleRange" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).ScaleRanges), "ScaleRangeExprHost");
		}

		internal int ScaleRangeEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_scaleRangesHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).ScaleRanges);
		}

		internal void ScaleRangeDistanceFromScale(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DistanceFromScaleExpr", expression);
		}

		internal void ScaleRangeStartWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("StartWidthExpr", expression);
		}

		internal void ScaleRangeEndWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("EndWidthExpr", expression);
		}

		internal void ScaleRangeInRangeBarPointerColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("InRangeBarPointerColorExpr", expression);
		}

		internal void ScaleRangeInRangeLabelColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("InRangeLabelColorExpr", expression);
		}

		internal void ScaleRangeInRangeTickMarksColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("InRangeTickMarksColorExpr", expression);
		}

		internal void ScaleRangeBackgroundGradientType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("BackgroundGradientTypeExpr", expression);
		}

		internal void ScaleRangePlacement(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PlacementExpr", expression);
		}

		internal void ScaleRangeToolTip(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ToolTipExpr", expression);
		}

		internal void ScaleRangeHidden(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HiddenExpr", expression);
		}

		internal void IndicatorImageStart()
		{
			this.TypeStart("IndicatorImage", "IndicatorImageExprHost");
		}

		internal void IndicatorImageEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "IndicatorImageHost");
		}

		internal void IndicatorImageHueColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HueColorExpr", expression);
		}

		internal void IndicatorImageTransparency(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TransparencyExpr", expression);
		}

		internal void StateIndicatorStart(string name)
		{
			this.TypeStart(this.CreateTypeName("StateIndicator" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).StateIndicators), "StateIndicatorExprHost");
		}

		internal int StateIndicatorEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_stateIndicatorsHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).StateIndicators);
		}

		internal void StateIndicatorIndicatorStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IndicatorStyleExpr", expression);
		}

		internal void StateIndicatorScaleFactor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ScaleFactorExpr", expression);
		}

		internal void StateIndicatorResizeMode(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ResizeModeExpr", expression);
		}

		internal void StateIndicatorAngle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AngleExpr", expression);
		}

		internal void StateIndicatorTransformationType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TransformationTypeExpr", expression);
		}

		internal void ThermometerStart()
		{
			this.TypeStart("Thermometer", "ThermometerExprHost");
		}

		internal void ThermometerEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "ThermometerHost");
		}

		internal void ThermometerBulbOffset(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("BulbOffsetExpr", expression);
		}

		internal void ThermometerBulbSize(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("BulbSizeExpr", expression);
		}

		internal void ThermometerThermometerStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ThermometerStyleExpr", expression);
		}

		internal void TickMarkStyleDistanceFromScale(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DistanceFromScaleExpr", expression);
		}

		internal void TickMarkStylePlacement(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PlacementExpr", expression);
		}

		internal void TickMarkStyleEnableGradient(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("EnableGradientExpr", expression);
		}

		internal void TickMarkStyleGradientDensity(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("GradientDensityExpr", expression);
		}

		internal void TickMarkStyleLength(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LengthExpr", expression);
		}

		internal void TickMarkStyleWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("WidthExpr", expression);
		}

		internal void TickMarkStyleShape(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ShapeExpr", expression);
		}

		internal void TickMarkStyleHidden(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HiddenExpr", expression);
		}

		internal void IndicatorStateStart(string name)
		{
			this.TypeStart(this.CreateTypeName("IndicatorState" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).IndicatorStates), "IndicatorStateExprHost");
		}

		internal int IndicatorStateEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_indicatorStatesHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).IndicatorStates);
		}

		internal void IndicatorStateColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ColorExpr", expression);
		}

		internal void IndicatorStateScaleFactor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ScaleFactorExpr", expression);
		}

		internal void IndicatorStateIndicatorStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IndicatorStyleExpr", expression);
		}

		internal void MapViewZoom(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ZoomExpr", expression);
		}

		internal void MapElementViewStart()
		{
			this.TypeStart("MapElementView", "MapElementViewExprHost");
		}

		internal void MapElementViewEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapViewHost");
		}

		internal void MapElementViewLayerName(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LayerNameExpr", expression);
		}

		internal void MapCustomViewStart()
		{
			this.TypeStart("MapCustomView", "MapCustomViewExprHost");
		}

		internal void MapCustomViewEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapViewHost");
		}

		internal void MapCustomViewCenterX(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("CenterXExpr", expression);
		}

		internal void MapCustomViewCenterY(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("CenterYExpr", expression);
		}

		internal void MapDataBoundViewStart()
		{
			this.TypeStart("MapDataBoundView", "MapDataBoundViewExprHost");
		}

		internal void MapDataBoundViewEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapViewHost");
		}

		internal void MapBorderSkinStart()
		{
			this.TypeStart("MapBorderSkin", "MapBorderSkinExprHost");
		}

		internal void MapBorderSkinEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapBorderSkinHost");
		}

		internal void MapBorderSkinMapBorderSkinType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MapBorderSkinTypeExpr", expression);
		}

		internal void MapAntiAliasing(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AntiAliasingExpr", expression);
		}

		internal void MapTextAntiAliasingQuality(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TextAntiAliasingQualityExpr", expression);
		}

		internal void MapShadowIntensity(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ShadowIntensityExpr", expression);
		}

		internal void MapTileLanguage(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TileLanguageExpr", expression);
		}

		internal void MapVectorLayerMapDataRegionName(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MapDataRegionNameExpr", expression);
		}

		internal void MapTileLayerStart(string name)
		{
			this.TypeStart(this.CreateTypeName("MapTileLayer" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).MapTileLayers), "MapTileLayerExprHost");
		}

		internal int MapTileLayerEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_mapTileLayersHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).MapTileLayers);
		}

		internal void MapTileLayerServiceUrl(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ServiceUrlExpr", expression);
		}

		internal void MapTileLayerTileStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TileStyleExpr", expression);
		}

		internal void MapTileLayerUseSecureConnection(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("UseSecureConnectionExpr", expression);
		}

		internal void MapTileStart(string name)
		{
			this.TypeStart(this.CreateTypeName("MapTile" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).MapTiles), "MapTileExprHost");
		}

		internal int MapTileEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_mapTilesHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).MapTiles);
		}

		internal void MapPointLayerStart(string name)
		{
			this.TypeStart(this.CreateTypeName("MapPointLayer" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).MapPointLayers), "MapPointLayerExprHost");
		}

		internal int MapPointLayerEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_mapPointLayersHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).MapPointLayers);
		}

		internal void MapSpatialDataSetStart()
		{
			this.TypeStart("MapSpatialDataSet", "MapSpatialDataSetExprHost");
		}

		internal void MapSpatialDataSetEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapSpatialDataHost");
		}

		internal void MapSpatialDataSetDataSetName(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DataSetNameExpr", expression);
		}

		internal void MapSpatialDataSetSpatialField(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SpatialFieldExpr", expression);
		}

		internal void MapSpatialDataRegionStart()
		{
			this.TypeStart("MapSpatialDataRegion", "MapSpatialDataRegionExprHost");
		}

		internal void MapSpatialDataRegionEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapSpatialDataHost");
		}

		internal void MapSpatialDataRegionVectorData(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("VectorDataExpr", expression);
		}

		internal void MapPolygonLayerStart(string name)
		{
			this.TypeStart(this.CreateTypeName("MapPolygonLayer" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).MapPolygonLayers), "MapPolygonLayerExprHost");
		}

		internal int MapPolygonLayerEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_mapPolygonLayersHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).MapPolygonLayers);
		}

		internal void MapShapefileStart()
		{
			this.TypeStart("MapShapefile", "MapShapefileExprHost");
		}

		internal void MapShapefileEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapSpatialDataHost");
		}

		internal void MapShapefileSource(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SourceExpr", expression);
		}

		internal void MapLineLayerStart(string name)
		{
			this.TypeStart(this.CreateTypeName("MapLineLayer" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).MapLineLayers), "MapLineLayerExprHost");
		}

		internal int MapLineLayerEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_mapLineLayersHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).MapLineLayers);
		}

		internal void MapLayerVisibilityMode(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("VisibilityModeExpr", expression);
		}

		internal void MapLayerMinimumZoom(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MinimumZoomExpr", expression);
		}

		internal void MapLayerMaximumZoom(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MaximumZoomExpr", expression);
		}

		internal void MapLayerTransparency(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TransparencyExpr", expression);
		}

		internal void MapFieldNameStart(string name)
		{
			this.TypeStart(this.CreateTypeName("MapFieldName" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).MapFieldNames), "MapFieldNameExprHost");
		}

		internal int MapFieldNameEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_mapFieldNamesHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).MapFieldNames);
		}

		internal void MapFieldNameName(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("NameExpr", expression);
		}

		internal void MapPointStart(string name)
		{
			this.TypeStart(this.CreateTypeName("MapPoint" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).MapPoints), "MapPointExprHost");
		}

		internal int MapPointEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_mapPointsHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).MapPoints);
		}

		internal void MapPointUseCustomPointTemplate(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("UseCustomPointTemplateExpr", expression);
		}

		internal void MapPolygonStart(string name)
		{
			this.TypeStart(this.CreateTypeName("MapPolygon" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).MapPolygons), "MapPolygonExprHost");
		}

		internal int MapPolygonEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_mapPolygonsHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).MapPolygons);
		}

		internal void MapPolygonUseCustomPolygonTemplate(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("UseCustomPolygonTemplateExpr", expression);
		}

		internal void MapPolygonUseCustomCenterPointTemplate(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("UseCustomPointTemplateExpr", expression);
		}

		internal void MapLineStart(string name)
		{
			this.TypeStart(this.CreateTypeName("MapLine" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).MapLines), "MapLineExprHost");
		}

		internal int MapLineEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_mapLinesHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).MapLines);
		}

		internal void MapLineUseCustomLineTemplate(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("UseCustomLineTemplateExpr", expression);
		}

		internal void MapSpatialElementTemplateHidden(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HiddenExpr", expression);
		}

		internal void MapSpatialElementTemplateOffsetX(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("OffsetXExpr", expression);
		}

		internal void MapSpatialElementTemplateOffsetY(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("OffsetYExpr", expression);
		}

		internal void MapSpatialElementTemplateLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LabelExpr", expression);
		}

		internal void MapSpatialElementTemplateDataElementLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DataElementLabelExpr", expression);
		}

		internal void MapSpatialElementTemplateToolTip(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ToolTipExpr", expression);
		}

		internal void MapPointTemplateSize(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SizeExpr", expression);
		}

		internal void MapPointTemplateLabelPlacement(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LabelPlacementExpr", expression);
		}

		internal void MapMarkerTemplateStart()
		{
			this.TypeStart("MapMarkerTemplate", "MapMarkerTemplateExprHost");
		}

		internal void MapMarkerTemplateEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapPointTemplateHost");
		}

		internal void MapPolygonTemplateStart()
		{
			this.TypeStart("MapPolygonTemplate", "MapPolygonTemplateExprHost");
		}

		internal void MapPolygonTemplateEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapPolygonTemplateHost");
		}

		internal void MapPolygonTemplateScaleFactor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ScaleFactorExpr", expression);
		}

		internal void MapPolygonTemplateCenterPointOffsetX(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("CenterPointOffsetXExpr", expression);
		}

		internal void MapPolygonTemplateCenterPointOffsetY(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("CenterPointOffsetYExpr", expression);
		}

		internal void MapPolygonTemplateShowLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ShowLabelExpr", expression);
		}

		internal void MapPolygonTemplateLabelPlacement(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LabelPlacementExpr", expression);
		}

		internal void MapLineTemplateStart()
		{
			this.TypeStart("MapLineTemplate", "MapLineTemplateExprHost");
		}

		internal void MapLineTemplateEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapLineTemplateHost");
		}

		internal void MapLineTemplateWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("WidthExpr", expression);
		}

		internal void MapLineTemplateLabelPlacement(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LabelPlacementExpr", expression);
		}

		internal void MapCustomColorRuleStart()
		{
			this.TypeStart("MapCustomColorRule", "MapCustomColorRuleExprHost");
		}

		internal void MapCustomColorRuleEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapColorRuleHost");
		}

		internal void MapCustomColorStart(string name)
		{
			this.TypeStart(this.CreateTypeName("MapCustomColor" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).MapCustomColors), "MapCustomColorExprHost");
		}

		internal int MapCustomColorEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_mapCustomColorsHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).MapCustomColors);
		}

		internal void MapCustomColorColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ColorExpr", expression);
		}

		internal void MapPointRulesStart()
		{
			this.TypeStart("MapPointRules", "MapPointRulesExprHost");
		}

		internal void MapPointRulesEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapPointRulesHost");
		}

		internal void MapMarkerRuleStart()
		{
			this.TypeStart("MapMarkerRule", "MapMarkerRuleExprHost");
		}

		internal void MapMarkerRuleEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapMarkerRuleHost");
		}

		internal void MapMarkerStart()
		{
			this.TypeStart("MapMarker", "MapMarkerExprHost");
		}

		internal void MapMarkerEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapMarkerHost");
		}

		internal void MapMarkerInCollectionStart(string name)
		{
			this.TypeStart(this.CreateTypeName("MapMarker" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).MapMarkers), "MapMarkerExprHost");
		}

		internal int MapMarkerInCollectionEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_mapMarkersHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).MapMarkers);
		}

		internal void MapMarkerMapMarkerStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MapMarkerStyleExpr", expression);
		}

		internal void MapMarkerImageStart()
		{
			this.TypeStart("MapMarkerImage", "MapMarkerImageExprHost");
		}

		internal void MapMarkerImageEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapMarkerImageHost");
		}

		internal void MapMarkerImageSource(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SourceExpr", expression);
		}

		internal void MapMarkerImageValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ValueExpr", expression);
		}

		internal void MapMarkerImageMIMEType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MIMETypeExpr", expression);
		}

		internal void MapMarkerImageTransparentColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TransparentColorExpr", expression);
		}

		internal void MapMarkerImageResizeMode(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ResizeModeExpr", expression);
		}

		internal void MapSizeRuleStart()
		{
			this.TypeStart("MapSizeRule", "MapSizeRuleExprHost");
		}

		internal void MapSizeRuleEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapSizeRuleHost");
		}

		internal void MapSizeRuleStartSize(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("StartSizeExpr", expression);
		}

		internal void MapSizeRuleEndSize(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("EndSizeExpr", expression);
		}

		internal void MapPolygonRulesStart()
		{
			this.TypeStart("MapPolygonRules", "MapPolygonRulesExprHost");
		}

		internal void MapPolygonRulesEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapPolygonRulesHost");
		}

		internal void MapLineRulesStart()
		{
			this.TypeStart("MapLineRules", "MapLineRulesExprHost");
		}

		internal void MapLineRulesEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapLineRulesHost");
		}

		internal void MapColorRuleShowInColorScale(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ShowInColorScaleExpr", expression);
		}

		internal void MapColorRangeRuleStart()
		{
			this.TypeStart("MapColorRangeRule", "MapColorRangeRuleExprHost");
		}

		internal void MapColorRangeRuleEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapColorRuleHost");
		}

		internal void MapColorRangeRuleStartColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("StartColorExpr", expression);
		}

		internal void MapColorRangeRuleMiddleColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MiddleColorExpr", expression);
		}

		internal void MapColorRangeRuleEndColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("EndColorExpr", expression);
		}

		internal void MapColorPaletteRuleStart()
		{
			this.TypeStart("MapColorPaletteRule", "MapColorPaletteRuleExprHost");
		}

		internal void MapColorPaletteRuleEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapColorRuleHost");
		}

		internal void MapColorPaletteRulePalette(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PaletteExpr", expression);
		}

		internal void MapBucketStart(string name)
		{
			this.TypeStart(this.CreateTypeName("MapBucket" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).MapBuckets), "MapBucketExprHost");
		}

		internal int MapBucketEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_mapBucketsHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).MapBuckets);
		}

		internal void MapBucketStartValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("StartValueExpr", expression);
		}

		internal void MapBucketEndValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("EndValueExpr", expression);
		}

		internal void MapAppearanceRuleDataValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DataValueExpr", expression);
		}

		internal void MapAppearanceRuleDistributionType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DistributionTypeExpr", expression);
		}

		internal void MapAppearanceRuleBucketCount(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("BucketCountExpr", expression);
		}

		internal void MapAppearanceRuleStartValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("StartValueExpr", expression);
		}

		internal void MapAppearanceRuleEndValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("EndValueExpr", expression);
		}

		internal void MapAppearanceRuleLegendText(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LegendTextExpr", expression);
		}

		internal void MapLegendTitleStart()
		{
			this.TypeStart("MapLegendTitle", "MapLegendTitleExprHost");
		}

		internal void MapLegendTitleEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapLegendTitleHost");
		}

		internal void MapLegendTitleCaption(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("CaptionExpr", expression);
		}

		internal void MapLegendTitleTitleSeparator(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TitleSeparatorExpr", expression);
		}

		internal void MapLegendTitleTitleSeparatorColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TitleSeparatorColorExpr", expression);
		}

		internal void MapLegendStart(string name)
		{
			this.TypeStart(this.CreateTypeName("MapLegend" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).MapLegends), "MapLegendExprHost");
		}

		internal int MapLegendEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_mapLegendsHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).MapLegends);
		}

		internal void MapLegendLayout(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LayoutExpr", expression);
		}

		internal void MapLegendAutoFitTextDisabled(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AutoFitTextDisabledExpr", expression);
		}

		internal void MapLegendMinFontSize(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MinFontSizeExpr", expression);
		}

		internal void MapLegendInterlacedRows(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("InterlacedRowsExpr", expression);
		}

		internal void MapLegendInterlacedRowsColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("InterlacedRowsColorExpr", expression);
		}

		internal void MapLegendEquallySpacedItems(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("EquallySpacedItemsExpr", expression);
		}

		internal void MapLegendTextWrapThreshold(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TextWrapThresholdExpr", expression);
		}

		internal void MapTitleStart(string name)
		{
			this.TypeStart(this.CreateTypeName("MapTitle" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).MapTitles), "MapTitleExprHost");
		}

		internal int MapTitleEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_mapTitlesHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).MapTitles);
		}

		internal void MapTitleText(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TextExpr", expression);
		}

		internal void MapTitleAngle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("AngleExpr", expression);
		}

		internal void MapTitleTextShadowOffset(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TextShadowOffsetExpr", expression);
		}

		internal void MapDistanceScaleStart()
		{
			this.TypeStart("MapDistanceScale", "MapDistanceScaleExprHost");
		}

		internal void MapDistanceScaleEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapDistanceScaleHost");
		}

		internal void MapDistanceScaleScaleColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ScaleColorExpr", expression);
		}

		internal void MapDistanceScaleScaleBorderColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ScaleBorderColorExpr", expression);
		}

		internal void MapColorScaleTitleStart()
		{
			this.TypeStart("MapColorScaleTitle", "MapColorScaleTitleExprHost");
		}

		internal void MapColorScaleTitleEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapColorScaleTitleHost");
		}

		internal void MapColorScaleTitleCaption(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("CaptionExpr", expression);
		}

		internal void MapColorScaleStart()
		{
			this.TypeStart("MapColorScale", "MapColorScaleExprHost");
		}

		internal void MapColorScaleEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapColorScaleHost");
		}

		internal void MapColorScaleTickMarkLength(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TickMarkLengthExpr", expression);
		}

		internal void MapColorScaleColorBarBorderColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ColorBarBorderColorExpr", expression);
		}

		internal void MapColorScaleLabelInterval(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LabelIntervalExpr", expression);
		}

		internal void MapColorScaleLabelFormat(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LabelFormatExpr", expression);
		}

		internal void MapColorScaleLabelPlacement(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LabelPlacementExpr", expression);
		}

		internal void MapColorScaleLabelBehavior(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LabelBehaviorExpr", expression);
		}

		internal void MapColorScaleHideEndLabels(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HideEndLabelsExpr", expression);
		}

		internal void MapColorScaleRangeGapColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("RangeGapColorExpr", expression);
		}

		internal void MapColorScaleNoDataText(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("NoDataTextExpr", expression);
		}

		internal void MapStart(string name)
		{
			this.TypeStart(name, "MapExprHost");
		}

		internal int MapEnd()
		{
			return this.ReportItemEnd("m_mapHostsRemotable", ref this.m_rootTypeDecl.Maps);
		}

		internal void MapLocationStart()
		{
			this.TypeStart("MapLocation", "MapLocationExprHost");
		}

		internal void MapLocationEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapLocationHost");
		}

		internal void MapLocationLeft(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LeftExpr", expression);
		}

		internal void MapLocationTop(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TopExpr", expression);
		}

		internal void MapLocationUnit(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("UnitExpr", expression);
		}

		internal void MapSizeStart()
		{
			this.TypeStart("MapSize", "MapSizeExprHost");
		}

		internal void MapSizeEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapSizeHost");
		}

		internal void MapSizeWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("WidthExpr", expression);
		}

		internal void MapSizeHeight(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HeightExpr", expression);
		}

		internal void MapSizeUnit(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("UnitExpr", expression);
		}

		internal void MapGridLinesStart(bool isMeridian)
		{
			this.TypeStart("MapGridLines" + (isMeridian ? "MapMeridiansHost" : "MapParallelsHost"), "MapGridLinesExprHost");
		}

		internal void MapGridLinesEnd(bool isMeridian)
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, isMeridian ? "MapMeridiansHost" : "MapParallelsHost");
		}

		internal void MapGridLinesHidden(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HiddenExpr", expression);
		}

		internal void MapGridLinesInterval(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("IntervalExpr", expression);
		}

		internal void MapGridLinesShowLabels(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ShowLabelsExpr", expression);
		}

		internal void MapGridLinesLabelPosition(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LabelPositionExpr", expression);
		}

		internal void MapDockableSubItemPosition(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PositionExpr", expression);
		}

		internal void MapDockableSubItemDockOutsideViewport(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DockOutsideViewportExpr", expression);
		}

		internal void MapDockableSubItemHidden(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HiddenExpr", expression);
		}

		internal void MapDockableSubItemToolTip(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ToolTipExpr", expression);
		}

		internal void MapSubItemLeftMargin(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LeftMarginExpr", expression);
		}

		internal void MapSubItemRightMargin(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("RightMarginExpr", expression);
		}

		internal void MapSubItemTopMargin(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("TopMarginExpr", expression);
		}

		internal void MapSubItemBottomMargin(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("BottomMarginExpr", expression);
		}

		internal void MapSubItemZIndex(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ZIndexExpr", expression);
		}

		internal void MapBindingFieldPairStart(string name)
		{
			this.TypeStart(this.CreateTypeName("MapBindingFieldPair" + name, ((NonRootTypeDecl)this.m_currentTypeDecl).MapBindingFieldPairs), "MapBindingFieldPairExprHost");
		}

		internal int MapBindingFieldPairEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_mapBindingFieldPairsHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).MapBindingFieldPairs);
		}

		internal void MapBindingFieldPairFieldName(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("FieldNameExpr", expression);
		}

		internal void MapBindingFieldPairBindingExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("BindingExpressionExpr", expression);
		}

		internal void MapViewportStart()
		{
			this.TypeStart("MapViewport", "MapViewportExprHost");
		}

		internal void MapViewportEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapViewportHost");
		}

		internal void MapViewportSimplificationResolution(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SimplificationResolutionExpr", expression);
		}

		internal void MapViewportMapCoordinateSystem(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MapCoordinateSystemExpr", expression);
		}

		internal void MapViewportMapProjection(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MapProjectionExpr", expression);
		}

		internal void MapViewportProjectionCenterX(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ProjectionCenterXExpr", expression);
		}

		internal void MapViewportProjectionCenterY(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ProjectionCenterYExpr", expression);
		}

		internal void MapViewportMaximumZoom(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MaximumZoomExpr", expression);
		}

		internal void MapViewportMinimumZoom(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MinimumZoomExpr", expression);
		}

		internal void MapViewportContentMargin(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ContentMarginExpr", expression);
		}

		internal void MapViewportGridUnderContent(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("GridUnderContentExpr", expression);
		}

		internal void MapLimitsStart()
		{
			this.TypeStart("MapLimits", "MapLimitsExprHost");
		}

		internal void MapLimitsEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MapLimitsHost");
		}

		internal void MapLimitsMinimumX(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MinimumXExpr", expression);
		}

		internal void MapLimitsMinimumY(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MinimumYExpr", expression);
		}

		internal void MapLimitsMaximumX(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MaximumXExpr", expression);
		}

		internal void MapLimitsMaximumY(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MaximumYExpr", expression);
		}

		internal void MapLimitsLimitToData(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LimitToDataExpr", expression);
		}

		internal void ParagraphStart(int index)
		{
			this.TypeStart(this.CreateTypeName("Paragraph" + index.ToString(CultureInfo.InvariantCulture), ((NonRootTypeDecl)this.m_currentTypeDecl).Paragraphs), "ParagraphExprHost");
		}

		internal int ParagraphEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_paragraphHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).Paragraphs);
		}

		internal void ParagraphLeftIndent(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("LeftIndentExpr", expression);
		}

		internal void ParagraphRightIndent(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("RightIndentExpr", expression);
		}

		internal void ParagraphHangingIndent(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("HangingIndentExpr", expression);
		}

		internal void ParagraphSpaceBefore(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SpaceBeforeExpr", expression);
		}

		internal void ParagraphSpaceAfter(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SpaceAfterExpr", expression);
		}

		internal void ParagraphListStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ListStyleExpr", expression);
		}

		internal void ParagraphListLevel(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ListLevelExpr", expression);
		}

		internal void TextRunStart(int index)
		{
			this.TypeStart(this.CreateTypeName("TextRun" + index.ToString(CultureInfo.InvariantCulture), ((NonRootTypeDecl)this.m_currentTypeDecl).TextRuns), "TextRunExprHost");
		}

		internal int TextRunEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_textRunHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).TextRuns);
		}

		internal void TextRunToolTip(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ToolTipExpr", expression);
		}

		internal void TextRunValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ValueExpr", expression);
		}

		internal void TextRunMarkupType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("MarkupTypeExpr", expression);
		}

		internal void LookupStart()
		{
			this.TypeStart(this.CreateTypeName("Lookup", this.m_rootTypeDecl.Lookups), "LookupExprHost");
		}

		internal void LookupSourceExpr(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("SourceExpr", expression);
		}

		internal void LookupResultExpr(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ResultExpr", expression);
		}

		internal int LookupEnd()
		{
			return this.TypeEnd((TypeDecl)this.m_rootTypeDecl, "m_lookupExprHostsRemotable", ref this.m_rootTypeDecl.Lookups);
		}

		internal void LookupDestStart()
		{
			this.TypeStart(this.CreateTypeName("LookupDest", this.m_rootTypeDecl.LookupDests), "LookupDestExprHost");
		}

		internal void LookupDestExpr(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DestExpr", expression);
		}

		internal int LookupDestEnd()
		{
			return this.TypeEnd((TypeDecl)this.m_rootTypeDecl, "m_lookupDestExprHostsRemotable", ref this.m_rootTypeDecl.LookupDests);
		}

		internal void PageBreakStart()
		{
			this.TypeStart("PageBreak", "PageBreakExprHost");
		}

		internal bool PageBreakEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "PageBreakExprHost");
		}

		internal void Disabled(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("DisabledExpr", expression);
		}

		internal void PageName(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PageNameExpr", expression);
		}

		internal void ResetPageNumber(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ResetPageNumberExpr", expression);
		}

		internal void JoinConditionStart()
		{
			this.TypeStart(this.CreateTypeName("JoinCondition", ((NonRootTypeDecl)this.m_currentTypeDecl).JoinConditions), "JoinConditionExprHost");
		}

		internal void JoinConditionForeignKeyExpr(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("ForeignKeyExpr", expression);
		}

		internal void JoinConditionPrimaryKeyExpr(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.ExpressionAdd("PrimaryKeyExpr", expression);
		}

		internal int JoinConditionEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_joinConditionExprHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).JoinConditions);
		}

		private void TypeStart(string typeName, string baseType)
		{
			this.m_currentTypeDecl = new NonRootTypeDecl(typeName, baseType, this.m_currentTypeDecl, this.m_setCode);
		}

		private int TypeEnd(TypeDecl container, string name, ref CodeExpressionCollection initializers)
		{
			int result = -1;
			if (this.m_currentTypeDecl.HasExpressions)
			{
				result = container.NestedTypeColAdd(name, this.m_currentTypeDecl.BaseTypeName, ref initializers, this.m_currentTypeDecl.Type);
			}
			this.TypeEnd(container);
			return result;
		}

		private bool TypeEnd(TypeDecl container, string name)
		{
			bool hasExpressions = this.m_currentTypeDecl.HasExpressions;
			if (hasExpressions)
			{
				container.NestedTypeAdd(name, this.m_currentTypeDecl.Type);
			}
			this.TypeEnd(container);
			return hasExpressions;
		}

		private void TypeEnd(TypeDecl container)
		{
			Global.Tracer.Assert(this.m_currentTypeDecl.Parent != null && container != null, "(m_currentTypeDecl.Parent != null && container != null)");
			container.HasExpressions |= this.m_currentTypeDecl.HasExpressions;
			this.m_currentTypeDecl = this.m_currentTypeDecl.Parent;
		}

		private int ReportItemEnd(string name, ref CodeExpressionCollection initializers)
		{
			return this.TypeEnd((TypeDecl)this.m_rootTypeDecl, name, ref initializers);
		}

		private void ParameterStart()
		{
			this.TypeStart(this.CreateTypeName("Parameter", ((NonRootTypeDecl)this.m_currentTypeDecl).Parameters), "ParamExprHost");
		}

		private int ParameterEnd(string propName)
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, propName, ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).Parameters);
		}

		private void StyleStart(string typeName)
		{
			this.TypeStart(typeName, "StyleExprHost");
		}

		private void StyleEnd(string propName)
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, propName);
		}

		private void AggregateStart()
		{
			this.TypeStart(this.CreateTypeName("Aggregate", this.m_rootTypeDecl.Aggregates), "AggregateParamExprHost");
		}

		private int AggregateEnd()
		{
			return this.TypeEnd((TypeDecl)this.m_rootTypeDecl, "m_aggregateParamHostsRemotable", ref this.m_rootTypeDecl.Aggregates);
		}

		private string CreateTypeName(string template, CodeExpressionCollection initializers)
		{
			return template + ((initializers == null) ? "0" : initializers.Count.ToString(CultureInfo.InvariantCulture));
		}

		private void ExprIndexerCreate()
		{
			NonRootTypeDecl nonRootTypeDecl = (NonRootTypeDecl)this.m_currentTypeDecl;
			if (nonRootTypeDecl.IndexedExpressions != null)
			{
				Global.Tracer.Assert(nonRootTypeDecl.IndexedExpressions.Count > 0, "(currentTypeDecl.IndexedExpressions.Count > 0)");
				CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
				codeMemberProperty.Name = "Item";
				codeMemberProperty.Attributes = (MemberAttributes)24580;
				codeMemberProperty.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "index"));
				codeMemberProperty.Type = new CodeTypeReference(typeof(object));
				nonRootTypeDecl.Type.Members.Add(codeMemberProperty);
				int count = nonRootTypeDecl.IndexedExpressions.Count;
				if (count == 1)
				{
					codeMemberProperty.GetStatements.Add(nonRootTypeDecl.IndexedExpressions[0]);
				}
				else
				{
					codeMemberProperty.GetStatements.Add(this.ExprIndexerTree(nonRootTypeDecl.IndexedExpressions, 0, count - 1));
				}
			}
		}

		private CodeStatement ExprIndexerTree(ReturnStatementList indexedExpressions, int leftIndex, int rightIndex)
		{
			if (leftIndex == rightIndex)
			{
				return indexedExpressions[leftIndex];
			}
			int num = rightIndex - leftIndex >> 1;
			CodeConditionStatement codeConditionStatement = new CodeConditionStatement();
			codeConditionStatement.Condition = new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("index"), CodeBinaryOperatorType.LessThanOrEqual, new CodePrimitiveExpression(leftIndex + num));
			codeConditionStatement.TrueStatements.Add(this.ExprIndexerTree(indexedExpressions, leftIndex, leftIndex + num));
			codeConditionStatement.FalseStatements.Add(this.ExprIndexerTree(indexedExpressions, leftIndex + num + 1, rightIndex));
			return codeConditionStatement;
		}

		private void IndexedExpressionAdd(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			if (expression.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression)
			{
				NonRootTypeDecl nonRootTypeDecl = (NonRootTypeDecl)this.m_currentTypeDecl;
				if (nonRootTypeDecl.IndexedExpressions == null)
				{
					nonRootTypeDecl.IndexedExpressions = new ReturnStatementList();
				}
				nonRootTypeDecl.HasExpressions = true;
				expression.ExprHostID = nonRootTypeDecl.IndexedExpressions.Add(this.CreateExprReturnStatement(expression));
			}
		}

		private void ExpressionAdd(string name, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			if (expression.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression)
			{
				CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
				codeMemberProperty.Name = name;
				codeMemberProperty.Type = new CodeTypeReference(typeof(object));
				codeMemberProperty.Attributes = (MemberAttributes)24580;
				codeMemberProperty.GetStatements.Add(this.CreateExprReturnStatement(expression));
				this.m_currentTypeDecl.Type.Members.Add(codeMemberProperty);
				this.m_currentTypeDecl.HasExpressions = true;
			}
		}

		private CodeMethodReturnStatement CreateExprReturnStatement(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			CodeMethodReturnStatement codeMethodReturnStatement = new CodeMethodReturnStatement(new CodeSnippetExpression(expression.TransformedExpression));
			codeMethodReturnStatement.LinePragma = new CodeLinePragma("Expr" + expression.CompileTimeID.ToString(CultureInfo.InvariantCulture) + "end", 0);
			return codeMethodReturnStatement;
		}
	}
}
