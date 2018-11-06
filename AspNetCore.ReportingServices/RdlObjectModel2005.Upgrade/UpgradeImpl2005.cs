using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.RdlObjectModel;
using AspNetCore.ReportingServices.RdlObjectModel.Serialization;
using AspNetCore.ReportingServices.RdlObjectModel2008.Upgrade;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetCore.ReportingServices.RdlObjectModel2005.Upgrade
{
	internal class UpgradeImpl2005 : UpgradeImpl2008
	{
		private class Cloner
		{
			private UpgradeImpl2005 m_upgrader;

			private Dictionary<string, string> m_nameTable;

			private ArrayList m_clonedObjects;

			private Dictionary<string, string> m_textboxNameValueExprTable;

			public Dictionary<string, string> TextboxNameValueExprTable
			{
				get
				{
					return this.m_textboxNameValueExprTable;
				}
			}

			public Cloner(UpgradeImpl2005 upgrader)
			{
				this.m_upgrader = upgrader;
				this.m_nameTable = new Dictionary<string, string>();
				this.m_clonedObjects = new ArrayList();
				this.m_textboxNameValueExprTable = new Dictionary<string, string>();
			}

			public object Clone(object obj)
			{
				if (obj is ReportObject)
				{
					Type substituteType = SerializerHost2005.GetSubstituteType(obj.GetType(), true);
					StructMapping mapping = (StructMapping)TypeMapper.GetTypeMapping(substituteType);
					object obj2 = this.CloneStructure(obj, mapping);
					this.m_clonedObjects.Add(obj2);
					return obj2;
				}
				if (obj is IList)
				{
					object obj2 = this.CloneList((IList)obj);
					this.m_clonedObjects.Add(obj2);
					return obj2;
				}
				return obj;
			}

			private object CloneStructure(object obj, StructMapping mapping)
			{
				Type type = mapping.Type;
				object obj2 = Activator.CreateInstance(type);
				foreach (MemberMapping member in mapping.Members)
				{
					object value = member.GetValue(obj);
					member.SetValue(obj2, this.Clone(value));
				}
				if (obj2 is IGlobalNamedObject)
				{
					string baseName;
					if (obj2 is AspNetCore.ReportingServices.RdlObjectModel.Group)
					{
						baseName = this.m_upgrader.GetParentReportItemName((IContainedObject)obj) + "_Group";
					}
					else
					{
						baseName = type.Name;
						baseName = char.ToLower(baseName[0], CultureInfo.InvariantCulture) + baseName.Substring(1);
					}
					string text = this.m_upgrader.UniqueName(baseName, obj2, false);
					this.m_nameTable.Add(((IGlobalNamedObject)obj2).Name, text);
					((IGlobalNamedObject)obj2).Name = text;
				}
				return obj2;
			}

			private object CloneList(IList obj)
			{
				IList list = (IList)Activator.CreateInstance(obj.GetType());
				foreach (object item in obj)
				{
					list.Add(this.Clone(item));
				}
				return list;
			}

			public void FixReferences()
			{
				foreach (object clonedObject in this.m_clonedObjects)
				{
					this.FixReferences(clonedObject);
				}
			}

			public void FixReferences(object obj)
			{
				if (obj is IList)
				{
					foreach (object item in (IList)obj)
					{
						this.FixReferences(item);
					}
				}
				else if (obj is ReportObject)
				{
					Type type = obj.GetType();
					StructMapping structMapping = (StructMapping)TypeMapper.GetTypeMapping(type);
					if (typeof(AspNetCore.ReportingServices.RdlObjectModel.ReportItem).IsAssignableFrom(type))
					{
						AspNetCore.ReportingServices.RdlObjectModel.ReportItem reportItem = (AspNetCore.ReportingServices.RdlObjectModel.ReportItem)obj;
						reportItem.RepeatWith = this.FixReference(reportItem.RepeatWith);
						if (type == typeof(AspNetCore.ReportingServices.RdlObjectModel.Rectangle))
						{
							((AspNetCore.ReportingServices.RdlObjectModel.Rectangle)reportItem).LinkToChild = this.FixReference(((AspNetCore.ReportingServices.RdlObjectModel.Rectangle)reportItem).LinkToChild);
						}
						else if (type == typeof(Textbox))
						{
							((Textbox)obj).HideDuplicates = this.FixReference(((Textbox)obj).HideDuplicates);
						}
					}
					else if (type == typeof(AspNetCore.ReportingServices.RdlObjectModel.Visibility))
					{
						((AspNetCore.ReportingServices.RdlObjectModel.Visibility)obj).ToggleItem = this.FixReference(((AspNetCore.ReportingServices.RdlObjectModel.Visibility)obj).ToggleItem);
					}
					else if (type == typeof(UserSort))
					{
						((UserSort)obj).SortExpressionScope = this.FixReference(((UserSort)obj).SortExpressionScope);
					}
					foreach (MemberMapping member in structMapping.Members)
					{
						object value = member.GetValue(obj);
						if (typeof(IExpression).IsAssignableFrom(member.Type))
						{
							member.SetValue(obj, this.FixReference((IExpression)value));
						}
						else
						{
							this.FixReferences(value);
						}
					}
				}
			}

			private string FixReference(string value)
			{
				if (value != null && this.m_nameTable.ContainsKey(value))
				{
					return this.m_nameTable[value];
				}
				return value;
			}

			private IExpression FixReference(IExpression value)
			{
				if (value != null && value.IsExpression)
				{
					string expression = value.Expression;
					foreach (KeyValuePair<string, string> item in this.m_nameTable)
					{
						expression = this.m_upgrader.ReplaceReference(expression, item.Key, item.Value);
					}
					expression = this.m_upgrader.ReplaceReportItemReferenceWithValue(expression, this.m_textboxNameValueExprTable);
					value = (IExpression)Activator.CreateInstance(value.GetType());
					value.Expression = expression;
				}
				return value;
			}

			public void AddNameMapping(string oldName, string newName)
			{
				this.m_nameTable.Add(oldName, newName);
			}

			public void ApplySubTotalStyleOverrides(AspNetCore.ReportingServices.RdlObjectModel.ReportItem item, SubtotalStyle2005 style)
			{
				if (item != null && style != null)
				{
					if (item is Textbox)
					{
						if (item.Style == null)
						{
							item.Style = new AspNetCore.ReportingServices.RdlObjectModel.Style();
						}
						Textbox textbox = (Textbox)item;
						AspNetCore.ReportingServices.RdlObjectModel.Style style2 = textbox.Style;
						Paragraph paragraph = null;
						AspNetCore.ReportingServices.RdlObjectModel.Style style3 = null;
						AspNetCore.ReportingServices.RdlObjectModel.Style style4 = null;
						if (textbox.Paragraphs.Count > 0)
						{
							paragraph = textbox.Paragraphs[0];
							if (paragraph.Style == null)
							{
								Paragraph paragraph2 = paragraph;
								AspNetCore.ReportingServices.RdlObjectModel.Style style6 = paragraph2.Style = new AspNetCore.ReportingServices.RdlObjectModel.Style();
								style3 = style6;
							}
							else
							{
								style3 = paragraph.Style;
							}
							if (paragraph.TextRuns.Count > 0)
							{
								TextRun textRun = paragraph.TextRuns[0];
								if (textRun.Style == null)
								{
									TextRun textRun2 = textRun;
									AspNetCore.ReportingServices.RdlObjectModel.Style style8 = textRun2.Style = new AspNetCore.ReportingServices.RdlObjectModel.Style();
									style4 = style8;
								}
								else
								{
									style4 = textRun.Style;
								}
							}
						}
						this.ApplySubTotalStyleOverrides(style, style2, style3, style4);
					}
					else if (item.Style != null)
					{
						this.ApplySubTotalStyleOverrides(style, item.Style, item.Style, item.Style);
					}
				}
			}

			private void ApplySubTotalStyleOverrides(SubtotalStyle2005 subTotalStyle, AspNetCore.ReportingServices.RdlObjectModel.Style style1, AspNetCore.ReportingServices.RdlObjectModel.Style style2, AspNetCore.ReportingServices.RdlObjectModel.Style style3)
			{
				StructMapping structMapping = (StructMapping)TypeMapper.GetTypeMapping(typeof(AspNetCore.ReportingServices.RdlObjectModel.Style));
				foreach (MemberMapping member in structMapping.Members)
				{
					if (member.HasValue(subTotalStyle) && !subTotalStyle.IsPropertyDefinedOnInitialize(member.Name))
					{
						switch (member.Name)
						{
						case "TextAlign":
						case "LineHeight":
							if (style2 != null)
							{
								member.SetValue(style2, member.GetValue(subTotalStyle));
							}
							break;
						case "FontStyle":
						case "FontFamily":
						case "FontSize":
						case "FontWeight":
						case "Format":
						case "TextDecoration":
						case "Color":
						case "Language":
						case "Calendar":
						case "NumeralLanguage":
						case "NumeralVariant":
							if (style3 != null)
							{
								member.SetValue(style3, member.GetValue(subTotalStyle));
							}
							break;
						default:
							member.SetValue(style1, member.GetValue(subTotalStyle));
							break;
						}
					}
				}
			}
		}

		internal delegate AspNetCore.ReportingServices.RdlObjectModel.Group GroupAccessor(object member);

		internal delegate IList<CustomProperty> CustomPropertiesAccessor(object member);

		private delegate string AggregateFunctionFixup(string expression, int currentOffset, string specialFunctionName, int specialFunctionPos, int argumentsPos, int scopePos, int scopeLength, ref int offset);

		private const string DundasChartControl = "DundasChartControl";

		private const string DundasGaugeControl = "DundasGaugeControl";

		private const string UpgradedYukonChart = "__Upgraded2005__";

		private const double YukonDefaultPointWidth = 0.8;

		private const double YukonDefaultBarAndColumnPointWidth = 0.6;

		private const double YukonDefaultLineWidthInPoints = 2.25;

		private const double YukonDefaultBorderWidthInPoints = 0.75;

		private const double YukonBorderWidthFactor = 0.75;

		private const double KatmaiMinimumVisibleBorderWidth = 0.376;

		private const double KatmaiMinimumBorderWidth = 0.25;

		private const double PointsPerPixel = 0.75;

		private const string DundasCRIExpressionPrefixLowerCase = "expression:";

		private const string DundasCRIDefaultFont = "Microsoft Sans Serif, 8pt";

		private const string DundasCRIDefaultBoldFont = "Microsoft Sans Serif, 8pt, style=Bold";

		private const string DundasCRIDefaultCollectedPieStyle = "CollectedPie";

		private const string DundasCRISizeExpressionWrapper = "=CStr(({0})*{1})&\"pt\"";

		private const string EmptySeriesName = "emptySeriesName";

		private const string EmptyNamePrefix = "chart";

		private const string ChartElementDefaultName = "Default";

		private const string ChartPrimaryAxisName = "Primary";

		private const string ChartSecondaryAxisName = "Secondary";

		private const string NewChartAreaName = "ChartArea";

		private const string NewChartSeriesName = "Series";

		private const string NewChartTitleName = "Title";

		private const string ChartNoDataMessageTitleName = "NoDataMessageTitle";

		private const string NewChartLegendName = "Legend";

		private const string ChartFormulaNamePostfix = "_Formula";

		private const string NewChartAreaNameForFormulaSeries = "#NewChartArea";

		private const string PointWidthAttributeName = "PointWidth";

		private const string DrawingStyleAttributeName = "DrawingStyle";

		private const string PieLabelStyleAttributeName = "PieLabelStyle";

		private const string PieLabelStyleAttributeDefaultValueForYukon = "Outside";

		private const double DefaultSmartLabelMaxMovingDistance = 30.0;

		private const double DefaultBorderLineWidthInPixels = 1.0;

		private const string GaugeElementDefaultName = "Default";

		private const string DefaultRadialGaugeCollectionPrefix = "RadialGauges.";

		private const string DefaultLinearGaugeCollectionPrefix = "LinearGauges.";

		private const string DefaultGaugeLabelCollectionPrefix = "GaugeLabels.";

		private const string GaugeFontUnitPercentValue = "Percent";

		private const string GaugeFontUnitDefaultValue = "Default";

		private const string DefaultDundasCircularGaugeCollectionPrefix = "CircularGauges.";

		private const string DefaultDundasLinearGaugeCollectionPrefix = "LinearGauges.";

		private const string DefaultDundasGaugeLabelCollectionPrefix = "GaugeLabels.";

		private const string DefaultGaugeScaleLabelFont = "Microsoft Sans Serif, 14pt";

		private const string DefaultGaugeScalePinFont = "Microsoft Sans Serif, 12pt";

		private const string DefaultGaugeLabelFont = "Microsoft Sans Serif, 8.25pt";

		private const string DefaultGaugeScaleName = "Default";

		private const string DefaultGaugeTextPropertyValue = "Text";

		private const double DefaultLinearScaleRangeStartWidth = 10.0;

		private const double DefaultRadialScaleRangeStartWidth = 15.0;

		private const double DefaultLinearScaleRangeEndWidth = 10.0;

		private const double DefaultRadialScaleRangeEndWidth = 30.0;

		private const double DefaultLinearScaleRangeDistanceFromScale = 10.0;

		private const double DefaultRadialScaleRangeDistanceFromScale = 30.0;

		private const double DefaultGaugeTickMarkWidth = 3.0;

		private const double DefaultLinearScaleMajorTickMarkWidth = 4.0;

		private const double DefaultLinearScaleMinorTickMarkWidth = 3.0;

		private const double DefaultRadialScaleMajorTickMarkWidth = 8.0;

		private const double DefaultRadialScaleMinorTickMarkWidth = 3.0;

		private const double DefaultLinearScaleMajorTickMarkLength = 15.0;

		private const double DefaultLinearScaleMinorTickMarkLength = 9.0;

		private const double DefaultRadialScaleMajorTickMarkLength = 14.0;

		private const double DefaultRadialScaleMinorTickMarkLength = 8.0;

		private const double DefaultLinearGaugePointerWidth = 20.0;

		private const double DefaultRadialGaugePointerWidth = 15.0;

		private const double DefaultLinearGaugePointerMarkerLength = 20.0;

		private const double DefaultRadialGaugePointerMarkerLength = 10.0;

		private const double DefaultDefaultScalePinWidth = 6.0;

		private const double DefaultDefaultScalePinLength = 6.0;

		private const int DefaultGaugeScaleShadowOffset = 1;

		private const int DefaultGaugePointerShadowOffset = 2;

		private Hashtable m_nameTable;

		private Hashtable m_dataSourceNameTable;

		private Hashtable m_dataSourceCaseSensitiveNameTable;

		private List<DataSource2005> m_dataSources;

		private List<IUpgradeable> m_upgradeable;

		private ReportRegularExpressions m_regexes;

		private bool m_throwUpgradeException = true;

		private bool m_upgradeDundasCRIToNative;

		private bool m_renameInvalidDataSources = true;

		private static string[] m_scatterChartDataPointNames = new string[2]
		{
			"X",
			"Y"
		};

		private static string[] m_bubbleChartDataPointNames = new string[3]
		{
			"X",
			"Y",
			"Size"
		};

		private static string[] m_highLowCloseDataPointNames = new string[3]
		{
			"High",
			"Low",
			"Close"
		};

		private static string[] m_openHighLowCloseDataPointNames = new string[4]
		{
			"High",
			"Low",
			"Open",
			"Close"
		};

		private static AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties[] m_ParagraphAvailableStyles = new AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties[2]
		{
			AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties.TextAlign,
			AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties.LineHeight
		};

		private static AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties[] m_TextRunAvailableStyles = new AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties[11]
		{
			AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties.FontStyle,
			AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties.FontFamily,
			AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties.FontSize,
			AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties.FontWeight,
			AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties.Format,
			AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties.TextDecoration,
			AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties.Color,
			AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties.Language,
			AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties.Calendar,
			AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties.NumeralLanguage,
			AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties.NumeralVariant
		};

		internal void UpgradeMatrix(Matrix2005 matrix)
		{
			this.UpgradeReportItem(matrix);
			this.UpgradePageBreak(matrix);
			TablixBody tablixBody = matrix.TablixBody;
			matrix.RepeatRowHeaders = true;
			matrix.RepeatColumnHeaders = true;
			int count = matrix.ColumnGroupings.Count;
			int count2 = matrix.RowGroupings.Count;
			int i;
			if (matrix.Corner != null)
			{
				TablixCorner tablixCorner2 = matrix.TablixCorner = new TablixCorner();
				for (i = 0; i < count; i++)
				{
					TablixCornerRow tablixCornerRow = new TablixCornerRow();
					tablixCorner2.TablixCornerRows.Add(tablixCornerRow);
					for (int j = 0; j < count2; j++)
					{
						tablixCornerRow.Add(new TablixCornerCell());
					}
				}
				TablixCornerCell tablixCornerCell = tablixCorner2.TablixCornerRows[0][0];
				tablixCornerCell.CellContents = new CellContents();
				tablixCornerCell.CellContents.RowSpan = count;
				tablixCornerCell.CellContents.ColSpan = count2;
				if (matrix.Corner.ReportItems.Count > 0)
				{
					tablixCornerCell.CellContents.ReportItem = matrix.Corner.ReportItems[0];
				}
			}
			IList<TablixMember> list = null;
			TablixMember tablixMember = null;
			TablixMember tablixMember2 = null;
			TablixMember keepTogether = null;
			ColumnGrouping2005 columnGrouping = null;
			RowGrouping2005 rowGrouping = null;
			int num = 1;
			int num2 = 1;
			foreach (ColumnGrouping2005 columnGrouping3 in matrix.ColumnGroupings)
			{
				if (list == null)
				{
					matrix.TablixColumnHierarchy = new TablixHierarchy();
					list = matrix.TablixColumnHierarchy.TablixMembers;
				}
				if (columnGrouping3.FixedHeader)
				{
					matrix.FixedColumnHeaders = true;
				}
				DynamicColumns2005 dynamicColumns = columnGrouping3.DynamicColumns;
				if (dynamicColumns != null)
				{
					TablixMember tablixMember3 = new TablixMember();
					list.Add(tablixMember3);
					list = tablixMember3.TablixMembers;
					tablixMember = tablixMember3;
					keepTogether = tablixMember3;
					tablixMember3.Group = dynamicColumns.Grouping;
					tablixMember3.SortExpressions = dynamicColumns.Sorting;
					tablixMember3.Visibility = dynamicColumns.Visibility;
					tablixMember3.DataElementName = dynamicColumns.Grouping.DataCollectionName;
					tablixMember3.DataElementOutput = dynamicColumns.Grouping.DataElementOutput;
					this.TransferGroupingCustomProperties(tablixMember3, UpgradeImpl2005.TablixMemberGroupAccessor, UpgradeImpl2005.TablixMemberCustomPropertiesAccessor);
					TablixHeader tablixHeader2 = tablixMember3.TablixHeader = new TablixHeader();
					tablixHeader2.Size = columnGrouping3.Height;
					tablixHeader2.CellContents = new CellContents();
					if (dynamicColumns.ReportItems.Count > 0)
					{
						tablixHeader2.CellContents.ReportItem = dynamicColumns.ReportItems[0];
					}
					continue;
				}
				if (columnGrouping3.StaticColumns.Count > 0)
				{
					if (columnGrouping != null)
					{
						throw new ArgumentException("More than one ColumnGrouping with StaticColumns.");
					}
					columnGrouping = columnGrouping3;
					num = columnGrouping3.StaticColumns.Count;
					for (int j = 0; j < num; j++)
					{
						TablixMember tablixMember3 = new TablixMember();
						list.Add(tablixMember3);
						TablixHeader tablixHeader4 = tablixMember3.TablixHeader = new TablixHeader();
						tablixHeader4.Size = columnGrouping3.Height;
						tablixHeader4.CellContents = new CellContents();
						if (columnGrouping3.StaticColumns[j].ReportItems.Count > 0)
						{
							tablixHeader4.CellContents.ReportItem = columnGrouping3.StaticColumns[j].ReportItems[0];
						}
						int k;
						for (k = 0; k < matrix.MatrixRows.Count; k++)
						{
							MatrixRow2005 matrixRow = matrix.MatrixRows[k];
							if (matrixRow.MatrixCells.Count > j && matrixRow.MatrixCells[j].ReportItems.Count > 0 && matrixRow.MatrixCells[j].ReportItems[0].DataElementOutput != DataElementOutputTypes.NoOutput)
							{
								break;
							}
						}
						if (k == matrix.MatrixRows.Count)
						{
							tablixMember3.DataElementOutput = DataElementOutputTypes.NoOutput;
						}
					}
					tablixMember = list[0];
					list = tablixMember.TablixMembers;
					continue;
				}
				throw new ArgumentException("No DynamicColumns or StaticColumns.");
			}
			this.SetKeepTogether(keepTogether);
			list = null;
			keepTogether = null;
			foreach (RowGrouping2005 rowGrouping3 in matrix.RowGroupings)
			{
				if (list == null)
				{
					matrix.TablixRowHierarchy = new TablixHierarchy();
					list = matrix.TablixRowHierarchy.TablixMembers;
				}
				if (rowGrouping3.FixedHeader)
				{
					matrix.FixedRowHeaders = true;
				}
				DynamicRows2005 dynamicRows = rowGrouping3.DynamicRows;
				if (dynamicRows != null)
				{
					TablixMember tablixMember4 = new TablixMember();
					list.Add(tablixMember4);
					list = tablixMember4.TablixMembers;
					tablixMember2 = tablixMember4;
					keepTogether = tablixMember4;
					tablixMember4.Group = dynamicRows.Grouping;
					tablixMember4.SortExpressions = dynamicRows.Sorting;
					tablixMember4.Visibility = dynamicRows.Visibility;
					tablixMember4.DataElementName = dynamicRows.Grouping.DataCollectionName;
					tablixMember4.DataElementOutput = dynamicRows.Grouping.DataElementOutput;
					this.TransferGroupingCustomProperties(tablixMember4, UpgradeImpl2005.TablixMemberGroupAccessor, UpgradeImpl2005.TablixMemberCustomPropertiesAccessor);
					TablixHeader tablixHeader6 = tablixMember4.TablixHeader = new TablixHeader();
					tablixHeader6.Size = rowGrouping3.Width;
					tablixHeader6.CellContents = new CellContents();
					if (dynamicRows.ReportItems.Count > 0)
					{
						tablixHeader6.CellContents.ReportItem = dynamicRows.ReportItems[0];
					}
					continue;
				}
				if (rowGrouping3.StaticRows.Count > 0)
				{
					if (rowGrouping != null)
					{
						throw new ArgumentException("More than one RowGrouping with StaticRows.");
					}
					rowGrouping = rowGrouping3;
					num2 = rowGrouping3.StaticRows.Count;
					for (int j = 0; j < num2; j++)
					{
						TablixMember tablixMember4 = new TablixMember();
						list.Add(tablixMember4);
						TablixHeader tablixHeader8 = tablixMember4.TablixHeader = new TablixHeader();
						tablixHeader8.Size = rowGrouping3.Width;
						tablixHeader8.CellContents = new CellContents();
						if (rowGrouping3.StaticRows[j].ReportItems.Count > 0)
						{
							tablixHeader8.CellContents.ReportItem = rowGrouping3.StaticRows[j].ReportItems[0];
						}
						if (matrix.MatrixRows.Count > j)
						{
							MatrixRow2005 matrixRow2 = matrix.MatrixRows[j];
							int k;
							for (k = 0; k < matrixRow2.MatrixCells.Count && (matrixRow2.MatrixCells[k].ReportItems.Count <= 0 || matrixRow2.MatrixCells[k].ReportItems[0].DataElementOutput == DataElementOutputTypes.NoOutput); k++)
							{
							}
							if (k == matrixRow2.MatrixCells.Count)
							{
								tablixMember4.DataElementOutput = DataElementOutputTypes.NoOutput;
							}
						}
					}
					tablixMember2 = list[0];
					list = tablixMember2.TablixMembers;
					continue;
				}
				throw new ArgumentException("No DynamicRows or StaticRows.");
			}
			this.SetKeepTogether(keepTogether);
			this.UpgradePageBreaks(matrix, false);
			if (matrix.MatrixColumns.Count != num)
			{
				throw new ArgumentException("Wrong number of MatrixColumns.");
			}
			if (matrix.MatrixRows.Count != num2)
			{
				throw new ArgumentException("Wrong number of MatrixRows.");
			}
			foreach (MatrixRow2005 matrixRow3 in matrix.MatrixRows)
			{
				TablixRow tablixRow = new TablixRow();
				tablixBody.TablixRows.Add(tablixRow);
				tablixRow.Height = matrixRow3.Height;
				if (matrixRow3.MatrixCells.Count != num)
				{
					throw new ArgumentException("Wrong number of MatrixCells.");
				}
				foreach (MatrixCell2005 matrixCell in matrixRow3.MatrixCells)
				{
					TablixCell tablixCell = new TablixCell();
					tablixRow.TablixCells.Add(tablixCell);
					tablixCell.DataElementName = matrix.CellDataElementName;
					tablixCell.DataElementOutput = matrix.CellDataElementOutput;
					tablixCell.CellContents = new CellContents();
					if (matrixCell.ReportItems.Count > 0)
					{
						tablixCell.CellContents.ReportItem = matrixCell.ReportItems[0];
					}
				}
			}
			List<int> subTotalRows = new List<int>();
			int outerStaticMembers = num2;
			i = matrix.RowGroupings.Count;
			while (--i >= 0)
			{
				RowGrouping2005 rowGrouping2 = matrix.RowGroupings[i];
				if (rowGrouping2 == rowGrouping)
				{
					outerStaticMembers = 1;
					if (i < matrix.RowGroupings.Count - 1)
					{
						this.CloneTablixHierarchy(matrix, tablixMember2, true);
					}
				}
				else if (rowGrouping2.DynamicRows != null && rowGrouping2.DynamicRows.Subtotal != null)
				{
					this.CloneTablixSubtotal(matrix, tablixMember2, rowGrouping2.DynamicRows.Subtotal, outerStaticMembers, num2, true, subTotalRows);
				}
				if (i > 0)
				{
					tablixMember2 = (TablixMember)tablixMember2.Parent;
				}
			}
			outerStaticMembers = num;
			i = matrix.ColumnGroupings.Count;
			while (--i >= 0)
			{
				ColumnGrouping2005 columnGrouping2 = matrix.ColumnGroupings[i];
				if (columnGrouping2.StaticColumns.Count > 0)
				{
					outerStaticMembers = 1;
					if (i < matrix.ColumnGroupings.Count - 1)
					{
						this.CloneTablixHierarchy(matrix, tablixMember, false);
					}
				}
				else if (columnGrouping2.DynamicColumns != null && columnGrouping2.DynamicColumns.Subtotal != null)
				{
					this.CloneTablixSubtotal(matrix, tablixMember, columnGrouping2.DynamicColumns.Subtotal, outerStaticMembers, num, false, subTotalRows);
				}
				if (i > 0)
				{
					tablixMember = (TablixMember)tablixMember.Parent;
				}
			}
		}

		private void SetKeepTogether(TablixMember innerMostDynamicMember)
		{
			if (innerMostDynamicMember != null)
			{
				innerMostDynamicMember.KeepTogether = true;
				if (innerMostDynamicMember.TablixMembers != null)
				{
					foreach (TablixMember tablixMember in innerMostDynamicMember.TablixMembers)
					{
						tablixMember.KeepTogether = true;
					}
				}
			}
		}

		private void CloneTablixHierarchy(AspNetCore.ReportingServices.RdlObjectModel.Tablix tablix, TablixMember staticMember, bool cloneRows)
		{
			if (staticMember.TablixMembers.Count != 0)
			{
				TablixBody tablixBody = tablix.TablixBody;
				IList<TablixMember> siblingTablixMembers = this.GetSiblingTablixMembers(staticMember);
				int count = siblingTablixMembers.Count;
				for (int i = 1; i < count; i++)
				{
					TablixMember tablixMember = siblingTablixMembers[i];
					Cloner cloner = new Cloner(this);
					tablixMember.TablixMembers = (IList<TablixMember>)cloner.Clone(staticMember.TablixMembers);
					cloner.FixReferences();
					if (!cloneRows)
					{
						int num = tablixBody.TablixColumns.Count / count;
						foreach (TablixRow tablixRow in tablixBody.TablixRows)
						{
							int num2 = num;
							int num3 = i * num;
							while (num2-- > 0)
							{
								cloner.FixReferences(tablixRow.TablixCells[num3].CellContents.ReportItem);
								num3++;
							}
						}
					}
					else
					{
						int num = tablixBody.TablixRows.Count / count;
						int num2 = num;
						int num3 = i * num;
						while (num2-- > 0)
						{
							cloner.FixReferences(tablixBody.TablixRows[num3].TablixCells);
							num3++;
						}
					}
				}
			}
		}

		private IList<TablixMember> GetSiblingTablixMembers(TablixMember tablixMember)
		{
			if (!(tablixMember.Parent is TablixHierarchy))
			{
				return ((TablixMember)tablixMember.Parent).TablixMembers;
			}
			return ((TablixHierarchy)tablixMember.Parent).TablixMembers;
		}

		private void CloneTablixSubtotal(AspNetCore.ReportingServices.RdlObjectModel.Tablix tablix, TablixMember dynamicMember, Subtotal2005 subtotal, int outerStaticMembers, int originalCount, bool rowSubtotal, List<int> subTotalRows)
		{
			string name = tablix.Name;
			bool flag = true;
			TablixMember tablixMember = dynamicMember.Parent as TablixMember;
			while (tablixMember != null)
			{
				if (tablixMember.Group == null)
				{
					tablixMember = (tablixMember.Parent as TablixMember);
					continue;
				}
				flag = false;
				name = tablixMember.Group.Name;
				break;
			}
			Cloner cloner = new Cloner(this);
			this.ProcessClonedDynamicTablixMember(dynamicMember, cloner, name);
			TablixMember tablixMember2 = new TablixMember();
			if (flag)
			{
				tablixMember2.HideIfNoRows = true;
			}
			IList<TablixMember> siblingTablixMembers = this.GetSiblingTablixMembers(dynamicMember);
			siblingTablixMembers.Insert((subtotal.Position != SubtotalPositions.Before) ? 1 : 0, tablixMember2);
			tablixMember2.DataElementName = subtotal.DataElementName;
			tablixMember2.DataElementOutput = subtotal.DataElementOutput;
			TablixHeader tablixHeader2 = tablixMember2.TablixHeader = new TablixHeader();
			tablixHeader2.Size = dynamicMember.TablixHeader.Size;
			tablixHeader2.CellContents = new CellContents();
			tablixHeader2.CellContents.ReportItem = subtotal.ReportItems[0];
			this.CloneSubtotalTablixMembers(cloner, tablixMember2, dynamicMember.TablixMembers, name);
			this.FixupMutualReferences(cloner.TextboxNameValueExprTable);
			TablixBody tablixBody = tablix.TablixBody;
			if (!rowSubtotal)
			{
				int num = originalCount / outerStaticMembers;
				int num2 = tablixBody.TablixColumns.Count / outerStaticMembers;
				for (int i = 0; i < outerStaticMembers; i++)
				{
					int num3 = i * (num + num2);
					int num4 = (subtotal.Position == SubtotalPositions.Before) ? num3 : (num3 + num2);
					int num5 = 0;
					while (num5 < num)
					{
						for (int j = 0; j < tablixBody.TablixRows.Count; j++)
						{
							TablixRow tablixRow = tablixBody.TablixRows[j];
							TablixCell tablixCell = (TablixCell)cloner.Clone(tablixRow.TablixCells[num3]);
							if (!subTotalRows.Contains(j))
							{
								cloner.ApplySubTotalStyleOverrides(tablixCell.CellContents.ReportItem, subtotal.Style);
							}
							tablixRow.TablixCells.Insert(num4, tablixCell);
						}
						TablixColumn item = (TablixColumn)cloner.Clone(tablixBody.TablixColumns[num3]);
						tablixBody.TablixColumns.Insert(num4, item);
						if (num3 >= num4)
						{
							num3++;
						}
						num5++;
						num3++;
						num4++;
					}
				}
			}
			else
			{
				int num = originalCount / outerStaticMembers;
				int num2 = tablixBody.TablixRows.Count / outerStaticMembers;
				for (int i = 0; i < outerStaticMembers; i++)
				{
					int num3 = i * (num + num2);
					int num4 = (subtotal.Position == SubtotalPositions.Before) ? num3 : (num3 + num2);
					int num5 = 0;
					while (num5 < num)
					{
						TablixRow tablixRow2 = (TablixRow)cloner.Clone(tablixBody.TablixRows[num3]);
						foreach (TablixCell tablixCell2 in tablixRow2.TablixCells)
						{
							cloner.ApplySubTotalStyleOverrides(tablixCell2.CellContents.ReportItem, subtotal.Style);
						}
						tablixBody.TablixRows.Insert(num4, tablixRow2);
						subTotalRows.Add(num4);
						if (num3 >= num4)
						{
							num3++;
						}
						num5++;
						num3++;
						num4++;
					}
				}
			}
			cloner.FixReferences();
		}

		private void CloneSubtotalTablixMembers(Cloner cloner, TablixMember tablixMember, IList<TablixMember> tablixMembers, string parentScope)
		{
			if (tablixMembers.Count > 0)
			{
				TablixMember tablixMember2 = null;
				if (tablixMembers[0].Group != null)
				{
					tablixMember2 = tablixMembers[0];
				}
				else if (tablixMembers.Count > 1 && tablixMembers[1].Group != null)
				{
					tablixMember2 = tablixMembers[1];
				}
				if (tablixMember2 != null)
				{
					ReportSize size = tablixMember2.TablixHeader.Size;
					TablixHeader tablixHeader = tablixMember.TablixHeader;
					tablixHeader.Size += size;
					this.ProcessClonedDynamicTablixMember(tablixMember2, cloner, parentScope);
					this.CloneSubtotalTablixMembers(cloner, tablixMember, tablixMember2.TablixMembers, parentScope);
				}
				else
				{
					foreach (TablixMember tablixMember4 in tablixMembers)
					{
						TablixMember tablixMember3 = new TablixMember();
						tablixMember.TablixMembers.Add(tablixMember3);
						tablixMember3.Visibility = (AspNetCore.ReportingServices.RdlObjectModel.Visibility)cloner.Clone(tablixMember4.Visibility);
						tablixMember3.TablixHeader = (TablixHeader)cloner.Clone(tablixMember4.TablixHeader);
						tablixMember3.DataElementName = tablixMember4.DataElementName;
						tablixMember3.DataElementOutput = tablixMember4.DataElementOutput;
						this.CloneSubtotalTablixMembers(cloner, tablixMember3, tablixMember4.TablixMembers, parentScope);
					}
				}
			}
		}

		private void ProcessClonedDynamicTablixMember(TablixMember dynamicMember, Cloner cloner, string parentScope)
		{
			cloner.AddNameMapping(dynamicMember.Group.Name, parentScope);
			if (dynamicMember.TablixHeader.CellContents != null)
			{
				this.CollectInScopeTextboxValues(dynamicMember.TablixHeader.CellContents.ReportItem, cloner.TextboxNameValueExprTable);
			}
		}

		private void CollectInScopeTextboxValues(AspNetCore.ReportingServices.RdlObjectModel.ReportItem reportItem, Dictionary<string, string> nameValueExprTable)
		{
			if (reportItem != null)
			{
				if (reportItem is Textbox)
				{
					Textbox textbox = (Textbox)reportItem;
					string text = textbox.Paragraphs[0].TextRuns[0].Value.Value;
					if (ReportExpression.IsExpressionString(text))
					{
						text = text.Substring(1);
					}
					this.ReplaceReportItemReferenceWithValue(text, nameValueExprTable);
					nameValueExprTable[textbox.Name] = text;
				}
				else if (reportItem is AspNetCore.ReportingServices.RdlObjectModel.Rectangle)
				{
					AspNetCore.ReportingServices.RdlObjectModel.Rectangle rectangle = (AspNetCore.ReportingServices.RdlObjectModel.Rectangle)reportItem;
					this.CollectInScopeTextboxValues(rectangle.ReportItems, nameValueExprTable);
				}
				else if (reportItem is AspNetCore.ReportingServices.RdlObjectModel.Tablix)
				{
					AspNetCore.ReportingServices.RdlObjectModel.Tablix tablix = (AspNetCore.ReportingServices.RdlObjectModel.Tablix)reportItem;
					if (tablix.TablixCorner != null)
					{
						IList<IList<TablixCornerCell>> tablixCornerRows = tablix.TablixCorner.TablixCornerRows;
						if (tablixCornerRows != null)
						{
							foreach (IList<TablixCornerCell> item in tablixCornerRows)
							{
								if (item != null)
								{
									foreach (TablixCornerCell item2 in item)
									{
										if (item2 != null && item2.CellContents != null)
										{
											this.CollectInScopeTextboxValues(item2.CellContents.ReportItem, nameValueExprTable);
										}
									}
								}
							}
						}
					}
					this.CollectInScopeTextboxValues(tablix.TablixColumnHierarchy, nameValueExprTable);
					this.CollectInScopeTextboxValues(tablix.TablixRowHierarchy, nameValueExprTable);
				}
			}
		}

		private void CollectInScopeTextboxValues(TablixHierarchy hierarchy, Dictionary<string, string> nameValueExprTable)
		{
			if (hierarchy != null && hierarchy.TablixMembers != null)
			{
				this.CollectInScopeTextboxValues(hierarchy.TablixMembers, nameValueExprTable);
			}
		}

		private void CollectInScopeTextboxValues(IList<TablixMember> tablixMembers, Dictionary<string, string> nameValueExprTable)
		{
			foreach (TablixMember tablixMember in tablixMembers)
			{
				if (tablixMember != null && tablixMember.Group == null)
				{
					if (tablixMember.TablixHeader != null && tablixMember.TablixHeader.CellContents != null)
					{
						this.CollectInScopeTextboxValues(tablixMember.TablixHeader.CellContents.ReportItem, nameValueExprTable);
					}
					this.CollectInScopeTextboxValues(tablixMember.TablixMembers, nameValueExprTable);
				}
			}
		}

		private void CollectInScopeTextboxValues(IList<AspNetCore.ReportingServices.RdlObjectModel.ReportItem> reportItems, Dictionary<string, string> nameValueExprTable)
		{
			if (reportItems != null)
			{
				foreach (AspNetCore.ReportingServices.RdlObjectModel.ReportItem reportItem in reportItems)
				{
					this.CollectInScopeTextboxValues(reportItem, nameValueExprTable);
				}
			}
		}

		private string ReplaceReference(string expression, string oldValue, string newValue)
		{
			MatchCollection matchCollection = this.m_regexes.ReportItemName.Matches(expression);
			int num = 0;
			int newLength = newValue.Length;
			int oldLength = oldValue.Length;
			foreach (Match item in matchCollection)
			{
				System.Text.RegularExpressions.Group group = item.Groups["reportitemname"];
				if (group != null && group.Value.Equals(oldValue, StringComparison.OrdinalIgnoreCase))
				{
					expression = expression.Substring(0, num + group.Index) + newValue + expression.Substring(num + group.Index + oldLength);
					num += newLength - oldLength;
				}
			}
			expression = this.FixAggregateFunctions(expression, delegate(string expr, int currentOffset, string specialFunctionName, int specialFunctionPos, int argumentsPos, int scopePos, int scopeLength, ref int offset)
			{
				if (scopeLength != 0)
				{
					Match match2 = this.m_regexes.StringLiteralOnly.Match(expr, scopePos, scopeLength);
					if (match2.Success && match2.Groups["string"].Value.Equals(oldValue, StringComparison.OrdinalIgnoreCase))
					{
						scopePos = match2.Groups["string"].Index;
						expr = expr.Substring(0, scopePos) + newValue + expr.Substring(scopePos + oldLength);
						offset += newLength - oldLength;
					}
				}
				return expr;
			});
			return expression;
		}

		private string ReplaceReportItemReferenceWithValue(string expression, Dictionary<string, string> nameValueExprTable)
		{
			if (nameValueExprTable.Count == 0)
			{
				return expression;
			}
			MatchCollection matchCollection = this.m_regexes.ReportItemValueReference.Matches(expression);
			int num = 0;
			foreach (Match item in matchCollection)
			{
				System.Text.RegularExpressions.Group group = item.Groups["reportitemname"];
				string text = default(string);
				if (group != null && nameValueExprTable.TryGetValue(group.Value, out text))
				{
					text = "(" + text + ")";
					int length = text.Length;
					int length2 = item.Value.Length;
					expression = expression.Substring(0, num + item.Index) + text + expression.Substring(num + item.Index + length2);
					num += length - length2;
				}
			}
			return expression;
		}

		private void FixupMutualReferences(Dictionary<string, string> nameValueExprTable)
		{
			if (nameValueExprTable.Count != 0)
			{
				string[] array = new string[nameValueExprTable.Count];
				nameValueExprTable.Keys.CopyTo(array, 0);
				for (int i = 0; i < array.Length - 1; i++)
				{
					foreach (string key in array)
					{
						string text2 = nameValueExprTable[key] = this.ReplaceReportItemReferenceWithValue(nameValueExprTable[key], nameValueExprTable);
					}
				}
			}
		}

		private int GetScopeArgumentIndex(string function)
		{
			switch (function.ToUpperInvariant())
			{
			case "RUNNINGVALUE":
				return 2;
			case "ROWNUMBER":
			case "COUNTROWS":
				return 0;
			default:
				return 1;
			}
		}

		private bool FindArgument(int currentPos, string expression, out int newPos, int argumentIndex, out int argumentPos, out int argumentLength)
		{
			int num = 1;
			int num2 = 0;
			argumentPos = currentPos;
			argumentLength = 0;
			while (0 < num && currentPos < expression.Length)
			{
				Match match = this.m_regexes.Arguments.Match(expression, currentPos);
				if (!match.Success)
				{
					currentPos = expression.Length;
				}
				else
				{
					string text = match.Result("${openParen}");
					string text2 = match.Result("${closeParen}");
					string text3 = match.Result("${comma}");
					if (text != null && text.Length != 0)
					{
						num++;
					}
					else if (text2 != null && text2.Length != 0)
					{
						num--;
						if (num == 0)
						{
							if (num2 == argumentIndex)
							{
								argumentLength = match.Index - argumentPos;
							}
							num2++;
						}
					}
					else if (text3 != null && text3.Length != 0 && 1 == num)
					{
						if (num2 == argumentIndex)
						{
							argumentLength = match.Index - argumentPos;
						}
						num2++;
						if (num2 == argumentIndex)
						{
							argumentPos = match.Index + 1;
						}
					}
					currentPos = match.Index + match.Length;
				}
			}
			newPos = currentPos;
			return argumentLength != 0;
		}

		public UpgradeImpl2005(bool throwUpgradeException)
			: this(throwUpgradeException, true, true)
		{
		}

		public UpgradeImpl2005(bool throwUpgradeException, bool upgradeDundasCRIToNative, bool renameInvalidDataSources)
		{
			this.m_throwUpgradeException = throwUpgradeException;
			this.m_upgradeDundasCRIToNative = upgradeDundasCRIToNative;
			this.m_renameInvalidDataSources = renameInvalidDataSources;
			this.m_regexes = ReportRegularExpressions.Value;
		}

		internal override Type GetReportType()
		{
			return typeof(Report2005);
		}

		protected override void InitUpgrade()
		{
			this.m_dataSourceNameTable = new Hashtable();
			this.m_dataSourceCaseSensitiveNameTable = new Hashtable();
			this.m_upgradeable = new List<IUpgradeable>();
			this.m_dataSources = new List<DataSource2005>();
			this.m_nameTable = new Hashtable();
			base.InitUpgrade();
		}

		protected override void Upgrade(AspNetCore.ReportingServices.RdlObjectModel.Report report)
		{
			if (this.m_dataSources != null)
			{
				foreach (DataSource2005 dataSource in this.m_dataSources)
				{
					dataSource.Upgrade(this);
				}
			}
			foreach (IUpgradeable item in this.m_upgradeable)
			{
				item.Upgrade(this);
				if (item is CustomReportItem2005)
				{
					CustomReportItem2005 customReportItem = (CustomReportItem2005)item;
					if (customReportItem.Type == "DundasChartControl" && this.m_upgradeDundasCRIToNative)
					{
						AspNetCore.ReportingServices.RdlObjectModel.Chart chart = new AspNetCore.ReportingServices.RdlObjectModel.Chart();
						this.UpgradeDundasCRIChart(customReportItem, chart);
						this.ChangeReportItem(customReportItem.Parent, customReportItem, chart);
					}
					else if (customReportItem.Type == "DundasGaugeControl" && this.m_upgradeDundasCRIToNative)
					{
						GaugePanel gaugePanel = new GaugePanel();
						this.UpgradeDundasCRIGaugePanel(customReportItem, gaugePanel);
						this.ChangeReportItem(customReportItem.Parent, customReportItem, gaugePanel);
					}
					else if (this.m_throwUpgradeException)
					{
						throw new CRI2005UpgradeException();
					}
				}
			}
			UpgradeImpl2005.AdjustBodyWhitespace((Report2005)report);
			base.Upgrade(report);
		}

		protected override RdlSerializerSettings CreateReaderSettings()
		{
			return UpgradeSerializerSettings2005.CreateReaderSettings();
		}

		protected override RdlSerializerSettings CreateWriterSettings()
		{
			return UpgradeSerializerSettings2005.CreateWriterSettings();
		}

		protected override void SetupReaderSettings(RdlSerializerSettings settings)
		{
			SerializerHost2005 serializerHost = (SerializerHost2005)settings.Host;
			serializerHost.Upgradeable = this.m_upgradeable;
			serializerHost.DataSources = this.m_dataSources;
			serializerHost.NameTable = this.m_nameTable;
			base.SetupReaderSettings(settings);
		}

		private void ChangeReportItem(object parentObject, object oldReportItem, object newReportItem)
		{
			TypeMapping typeMapping = TypeMapper.GetTypeMapping(parentObject.GetType());
			if (typeMapping is StructMapping)
			{
				foreach (MemberMapping member in ((StructMapping)typeMapping).Members)
				{
					object value = member.GetValue(parentObject);
					if (member.Type == typeof(RdlCollection<AspNetCore.ReportingServices.RdlObjectModel.ReportItem>))
					{
						RdlCollection<AspNetCore.ReportingServices.RdlObjectModel.ReportItem> rdlCollection = (RdlCollection<AspNetCore.ReportingServices.RdlObjectModel.ReportItem>)value;
						int num = rdlCollection.IndexOf((AspNetCore.ReportingServices.RdlObjectModel.ReportItem)oldReportItem);
						if (num != -1)
						{
							((Collection<AspNetCore.ReportingServices.RdlObjectModel.ReportItem>)rdlCollection)[num] = (AspNetCore.ReportingServices.RdlObjectModel.ReportItem)newReportItem;
							break;
						}
					}
					else if (value == oldReportItem)
					{
						member.SetValue(parentObject, newReportItem);
						break;
					}
				}
			}
		}

		private static void AdjustBodyWhitespace(Report2005 report)
		{
			if (report.Body.ReportItems != null && report.Body.ReportItems.Count != 0)
			{
				double num = 0.0;
				double num2 = 0.0;
				double num3 = report.Width.ToPixels();
				double num4 = report.Body.Height.ToPixels();
				foreach (AspNetCore.ReportingServices.RdlObjectModel.ReportItem reportItem in report.Body.ReportItems)
				{
					num = ((!reportItem.Width.IsEmpty) ? Math.Max(num, reportItem.Left.ToPixels() + reportItem.Width.ToPixels()) : num3);
					num2 = ((!reportItem.Height.IsEmpty) ? Math.Max(num2, reportItem.Top.ToPixels() + reportItem.Height.ToPixels()) : num4);
				}
				num4 = Math.Min(num4, num2);
				report.Body.Height = ReportSize.FromPixels(num4, report.Body.Height.Type);
				double num5 = Math.Max(1.0, report.Page.PageWidth.ToPixels() - report.Page.LeftMargin.ToPixels() - report.Page.RightMargin.ToPixels());
				if (report.Page.Columns > 1)
				{
					num5 -= (double)(report.Page.Columns - 1) * report.Page.ColumnSpacing.ToPixels();
					num5 = Math.Max(1.0, num5 / (double)report.Page.Columns);
				}
				num3 = Math.Min(num3, num5 * Math.Ceiling(num / num5));
				report.Width = ReportSize.FromPixels(num3, report.Width.Type);
			}
		}

		internal static AspNetCore.ReportingServices.RdlObjectModel.Group TablixMemberGroupAccessor(object member)
		{
			return ((TablixMember)member).Group;
		}

		internal static IList<CustomProperty> TablixMemberCustomPropertiesAccessor(object member)
		{
			return ((TablixMember)member).CustomProperties;
		}

		internal static AspNetCore.ReportingServices.RdlObjectModel.Group ChartMemberGroupAccessor(object member)
		{
			return ((ChartMember)member).Group;
		}

		internal static IList<CustomProperty> ChartMemberCustomPropertiesAccessor(object member)
		{
			return ((ChartMember)member).CustomProperties;
		}

		internal static AspNetCore.ReportingServices.RdlObjectModel.Group DataMemberGroupAccessor(object member)
		{
			return ((DataMember)member).Group;
		}

		internal static IList<CustomProperty> DataMemberCustomPropertiesAccessor(object member)
		{
			return ((DataMember)member).CustomProperties;
		}

		internal static string SplitName(string name)
		{
			return Regex.Replace(name, "(\\p{Ll})(\\p{Lu})|_+", "$1 $2");
		}

		internal void UpgradeReport(Report2005 report)
		{
			report.ConsumeContainerWhitespace = true;
			Body2005 body = report.Body as Body2005;
			if (body != null)
			{
				report.Page.Columns = body.Columns;
				report.Page.ColumnSpacing = body.ColumnSpacing;
			}
			AspNetCore.ReportingServices.RdlObjectModel.Style style = body.Style;
			if (style != null && (style.Border == null || style.Border.Style == BorderStyles.None) && (style.TopBorder == null || style.TopBorder.Style == BorderStyles.None) && (style.BottomBorder == null || style.BottomBorder.Style == BorderStyles.None) && (style.LeftBorder == null || style.LeftBorder.Style == BorderStyles.None) && (style.RightBorder == null || style.RightBorder.Style == BorderStyles.None))
			{
				report.Page.Style = style;
				report.Body.Style = null;
				style = null;
			}
			foreach (ReportParameter2005 reportParameter in report.ReportParameters)
			{
				if (reportParameter.Nullable && (reportParameter.DefaultValue == null || (reportParameter.DefaultValue.Values.Count == 0 && reportParameter.DefaultValue.DataSetReference == null)))
				{
					if (reportParameter.DefaultValue == null)
					{
						reportParameter.DefaultValue = new DefaultValue();
					}
					reportParameter.DefaultValue.Values.Add(null);
				}
				if (reportParameter.Prompt.HasValue && reportParameter.Prompt.Value.Value == "")
				{
					reportParameter.Hidden = true;
					reportParameter.Prompt = reportParameter.Name;
				}
			}
			if (report.Page.InteractiveHeight == report.Page.PageHeight)
			{
				report.Page.InteractiveHeight = ReportSize.Empty;
			}
			if (report.Page.InteractiveWidth == report.Page.PageWidth)
			{
				report.Page.InteractiveWidth = ReportSize.Empty;
			}
		}

		internal void UpgradeReportItem(AspNetCore.ReportingServices.RdlObjectModel.ReportItem item)
		{
			IReportItem2005 reportItem = (IReportItem2005)item;
			if (reportItem.Action != null)
			{
				item.ActionInfo = new ActionInfo();
				item.ActionInfo.Actions.Add(reportItem.Action);
			}
			this.UpgradeDataElementOutput(item);
		}

		internal void UpgradeDataElementOutput(AspNetCore.ReportingServices.RdlObjectModel.ReportItem reportItem)
		{
			if (reportItem.DataElementOutput == DataElementOutputTypes.Auto && reportItem.Visibility != null && reportItem.Visibility.Hidden.IsExpression)
			{
				reportItem.DataElementOutput = DataElementOutputTypes.NoOutput;
			}
		}

		internal void UpgradePageBreak(IPageBreakLocation2005 item)
		{
			if (item.PageBreak == null)
			{
				if (!item.PageBreakAtStart && !item.PageBreakAtEnd)
				{
					return;
				}
				item.PageBreak = new PageBreak();
				item.PageBreak.BreakLocation = (BreakLocations)((!item.PageBreakAtStart) ? 2 : ((!item.PageBreakAtEnd) ? 1 : 3));
			}
		}

		internal void UpgradeRectangle(Rectangle2005 rectangle)
		{
			if (rectangle.DataElementOutput == DataElementOutputTypes.Auto)
			{
				rectangle.DataElementOutput = DataElementOutputTypes.ContentsOnly;
			}
			this.UpgradeReportItem(rectangle);
			this.UpgradePageBreak(rectangle);
		}

		internal void UpgradeCustomReportItem(CustomReportItem2005 cri)
		{
			this.UpgradeReportItem(cri);
		}

		internal void UpgradeDataGrouping(DataGrouping2005 dataGrouping)
		{
			if (!dataGrouping.Static && dataGrouping.Group == null)
			{
				AspNetCore.ReportingServices.RdlObjectModel.Group group = new AspNetCore.ReportingServices.RdlObjectModel.Group();
				string parentReportItemName = this.GetParentReportItemName(dataGrouping);
				group.Name = this.UniqueName(parentReportItemName + "_Group", group);
				dataGrouping.Group = group;
			}
			else
			{
				this.TransferGroupingCustomProperties(dataGrouping, UpgradeImpl2005.DataMemberGroupAccessor, UpgradeImpl2005.DataMemberCustomPropertiesAccessor);
			}
		}

		internal void UpgradeList(List2005 list)
		{
			this.UpgradeReportItem(list);
			this.UpgradePageBreak(list);
			list.TablixColumnHierarchy = new TablixHierarchy();
			TablixMember item = new TablixMember();
			list.TablixColumnHierarchy.TablixMembers.Add(item);
			TablixMember tablixMember = new TablixMember();
			list.TablixRowHierarchy = new TablixHierarchy();
			list.TablixRowHierarchy.TablixMembers.Add(tablixMember);
			if (list.Grouping == null)
			{
				AspNetCore.ReportingServices.RdlObjectModel.Group group = new AspNetCore.ReportingServices.RdlObjectModel.Group();
				group = new AspNetCore.ReportingServices.RdlObjectModel.Group();
				group.Name = this.UniqueName(list.Name + "_Details_Group", group);
				tablixMember.Group = group;
				if (list.DataInstanceName == null)
				{
					tablixMember.Group.DataElementName = "Item";
					tablixMember.DataElementName = "Item_Collection";
				}
				else
				{
					tablixMember.Group.DataElementName = list.DataInstanceName;
					tablixMember.DataElementName = list.DataInstanceName + "_Collection";
				}
			}
			else
			{
				tablixMember.Group = list.Grouping;
				Grouping2005 grouping = (Grouping2005)list.Grouping;
				this.UpgradePageBreaks(list, false);
			}
			tablixMember.DataElementOutput = list.DataInstanceElementOutput;
			tablixMember.KeepTogether = true;
			this.TransferGroupingCustomProperties(tablixMember, UpgradeImpl2005.TablixMemberGroupAccessor, UpgradeImpl2005.TablixMemberCustomPropertiesAccessor);
			TablixColumn tablixColumn = new TablixColumn();
			list.TablixBody.TablixColumns.Add(tablixColumn);
			tablixColumn.Width = this.GetReportItemWidth(list);
			TablixRow tablixRow = new TablixRow();
			list.TablixBody.TablixRows.Add(tablixRow);
			tablixRow.Height = this.GetReportItemHeight(list);
			TablixCell tablixCell = new TablixCell();
			tablixRow.TablixCells.Add(tablixCell);
			AspNetCore.ReportingServices.RdlObjectModel.Rectangle rectangle = new AspNetCore.ReportingServices.RdlObjectModel.Rectangle();
			tablixCell.CellContents = new CellContents();
			tablixCell.CellContents.ReportItem = rectangle;
			rectangle.KeepTogether = true;
			rectangle.Name = this.UniqueName(list.Name + "_Contents", rectangle);
			rectangle.ReportItems = list.ReportItems;
			bool flag = false;
			if (this.IsUpgradedListDetailMember(tablixMember))
			{
				this.FixAggregateFunction(rectangle.ReportItems, list.Name, list.Name, false, ref flag);
			}
			else
			{
				this.FixAggregateFunction(rectangle.ReportItems);
			}
			if (list.Visibility != null)
			{
				string toggleItem = list.Visibility.ToggleItem;
				bool flag2 = false;
				if (toggleItem != null && this.m_nameTable.ContainsKey(toggleItem))
				{
					flag2 = true;
					if (tablixMember.Group.Parent != (string)null && this.TextBoxExistsInCollection(rectangle.ReportItems, toggleItem))
					{
						tablixMember.Visibility = list.Visibility;
						list.Visibility = null;
					}
				}
				if (!flag2 && tablixMember.Visibility == null)
				{
					tablixMember.Visibility = new AspNetCore.ReportingServices.RdlObjectModel.Visibility();
					tablixMember.Visibility.Hidden = list.Visibility.Hidden;
					list.Visibility.Hidden = null;
				}
				if (this.IsUpgradedListDetailMember(tablixMember))
				{
					this.FixAggregateFunction(tablixMember.Visibility, list.Name, list.Name, false, ref flag);
				}
				else
				{
					this.FixAggregateFunction(tablixMember.Visibility);
				}
			}
			if (list.Sorting != null && list.Sorting.Count != 0)
			{
				if (!flag || list.Grouping != null || this.SortingContainsAggregate(list.Sorting))
				{
					tablixMember.SortExpressions = list.Sorting;
				}
				else
				{
					list.SortExpressions = list.Sorting;
				}
			}
		}

		private bool IsUpgradedListDetailMember(TablixMember rowMember)
		{
			if (rowMember.Group.GroupExpressions != null)
			{
				return rowMember.Group.GroupExpressions.Count == 0;
			}
			return true;
		}

		private bool TextBoxExistsInCollection(IList<AspNetCore.ReportingServices.RdlObjectModel.ReportItem> reportItems, string name)
		{
			if (reportItems != null)
			{
				foreach (AspNetCore.ReportingServices.RdlObjectModel.ReportItem reportItem in reportItems)
				{
					if (reportItem is AspNetCore.ReportingServices.RdlObjectModel.Rectangle)
					{
						if (this.TextBoxExistsInCollection(((AspNetCore.ReportingServices.RdlObjectModel.Rectangle)reportItem).ReportItems, name))
						{
							return true;
						}
					}
					else if (reportItem is Matrix2005)
					{
						if (((Matrix2005)reportItem).Corner != null && this.TextBoxExistsInCollection(((Matrix2005)reportItem).Corner.ReportItems, name))
						{
							return true;
						}
					}
					else if (reportItem is Table2005)
					{
						Table2005 table = (Table2005)reportItem;
						if (table.Header != null && this.TextBoxExistsInCollection(table.Header.TableRows, name))
						{
							return true;
						}
						if (table.Footer != null && this.TextBoxExistsInCollection(table.Footer.TableRows, name))
						{
							return true;
						}
					}
					else if (reportItem is Textbox && reportItem.Name == name)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool TextBoxExistsInCollection(IList<TableRow2005> rows, string name)
		{
			if (rows != null && rows.Count > 0)
			{
				foreach (TableRow2005 row in rows)
				{
					IList<TableCell2005> tableCells = row.TableCells;
					if (tableCells != null && tableCells.Count > 0)
					{
						foreach (TableCell2005 item in tableCells)
						{
							if (this.TextBoxExistsInCollection(item.ReportItems, name))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		internal void UpgradeTable(Table2005 table)
		{
			this.UpgradeReportItem(table);
			this.UpgradePageBreak(table);
			int count = table.TableColumns.Count;
			table.TablixColumnHierarchy = new TablixHierarchy();
			for (int i = 0; i < count; i++)
			{
				TableColumn2005 tableColumn = table.TableColumns[i];
				TablixMember tablixMember = new TablixMember();
				tablixMember.Visibility = tableColumn.Visibility;
				tablixMember.FixedData = tableColumn.FixedHeader;
				table.TablixColumnHierarchy.TablixMembers.Add(tablixMember);
				TablixColumn tablixColumn = new TablixColumn();
				tablixColumn.Width = tableColumn.Width;
				table.TablixBody.TablixColumns.Add(tablixColumn);
			}
			table.TablixRowHierarchy = new TablixHierarchy();
			IList<TablixMember> tablixMembers = table.TablixRowHierarchy.TablixMembers;
			int index = 0;
			int num = 0;
			bool flag = false;
			if (table.Header != null)
			{
				int i;
				for (i = 0; i < table.Header.TableRows.Count; i++)
				{
					TablixMember tablixMember = new TablixMember();
					tablixMembers.Add(tablixMember);
					tablixMember.FixedData = table.Header.FixedHeader;
					tablixMember.KeepTogether = true;
					tablixMember.KeepWithGroup = KeepWithGroupTypes.After;
					if (table.Header.RepeatOnNewPage)
					{
						tablixMember.RepeatOnNewPage = true;
					}
					TablixRow obj = this.UpgradeTableRow(table.Header.TableRows[i], table, i, tablixMember);
					this.FixAggregateFunction(obj, ref flag);
				}
				index = (num = i);
			}
			if (table.Footer != null)
			{
				for (int i = 0; i < table.Footer.TableRows.Count; i++)
				{
					TablixMember tablixMember = new TablixMember();
					tablixMembers.Add(tablixMember);
					tablixMember.KeepTogether = true;
					tablixMember.KeepWithGroup = KeepWithGroupTypes.Before;
					if (table.Footer.RepeatOnNewPage)
					{
						tablixMember.RepeatOnNewPage = true;
					}
					TablixRow obj2 = this.UpgradeTableRow(table.Footer.TableRows[i], table, num + i, tablixMember);
					this.FixAggregateFunction(obj2, ref flag);
				}
			}
			for (int i = 0; i < table.TableGroups.Count; i++)
			{
				TableGroup2005 tableGroup = table.TableGroups[i];
				TablixMember tablixMember = new TablixMember();
				tablixMembers.Insert(index, tablixMember);
				tablixMember.Visibility = tableGroup.Visibility;
				tablixMember.Group = tableGroup.Grouping;
				tablixMember.SortExpressions = tableGroup.Sorting;
				this.TransferGroupingCustomProperties(tablixMember, UpgradeImpl2005.TablixMemberGroupAccessor, UpgradeImpl2005.TablixMemberCustomPropertiesAccessor);
				tablixMembers = tablixMember.TablixMembers;
				index = 0;
				if (tableGroup.Header != null)
				{
					int j;
					for (j = 0; j < tableGroup.Header.TableRows.Count; j++)
					{
						tablixMember = new TablixMember();
						tablixMembers.Add(tablixMember);
						tablixMember.KeepTogether = true;
						tablixMember.KeepWithGroup = KeepWithGroupTypes.After;
						if (tableGroup.Header.RepeatOnNewPage)
						{
							tablixMember.RepeatOnNewPage = true;
						}
						TablixRow obj3 = this.UpgradeTableRow(tableGroup.Header.TableRows[j], table, num + j, tablixMember);
						this.FixAggregateFunction(obj3);
					}
					index = j;
					num += j;
				}
				if (tableGroup.Footer != null)
				{
					for (int j = 0; j < tableGroup.Footer.TableRows.Count; j++)
					{
						tablixMember = new TablixMember();
						tablixMembers.Add(tablixMember);
						tablixMember.KeepTogether = true;
						tablixMember.KeepWithGroup = KeepWithGroupTypes.Before;
						if (tableGroup.Footer.RepeatOnNewPage)
						{
							tablixMember.RepeatOnNewPage = true;
						}
						TablixRow obj4 = this.UpgradeTableRow(tableGroup.Footer.TableRows[j], table, num + j, tablixMember);
						this.FixAggregateFunction(obj4);
					}
				}
				if (i == table.TableGroups.Count - 1 && tableGroup.Header == null && tableGroup.Footer == null && table.Details == null)
				{
					tablixMember = new TablixMember();
					tablixMembers.Add(tablixMember);
					tablixMember.Visibility = new AspNetCore.ReportingServices.RdlObjectModel.Visibility();
					tablixMember.Visibility.Hidden = true;
					TablixRow tablixRow = new TablixRow();
					table.TablixBody.TablixRows.Insert(num, tablixRow);
					for (int j = 0; j < count; j++)
					{
						TablixCell tablixCell = new TablixCell();
						tablixCell.CellContents = new CellContents();
						tablixRow.TablixCells.Add(tablixCell);
					}
					num++;
				}
			}
			Details2005 details = table.Details;
			if (details != null)
			{
				TablixMember tablixMember = new TablixMember();
				tablixMembers.Insert(index, tablixMember);
				tablixMember.Visibility = details.Visibility;
				AspNetCore.ReportingServices.RdlObjectModel.Group group = details.Grouping;
				if (group == null)
				{
					group = new AspNetCore.ReportingServices.RdlObjectModel.Group();
					group.Name = this.UniqueName(table.Name + "_Details_Group", group);
				}
				tablixMember.Group = group;
				tablixMember.DataElementOutput = table.DetailDataElementOutput;
				if (table.DetailDataElementName == null)
				{
					tablixMember.Group.DataElementName = "Detail";
				}
				else
				{
					tablixMember.Group.DataElementName = table.DetailDataElementName;
				}
				if (table.DetailDataCollectionName == null)
				{
					tablixMember.DataElementName = tablixMember.Group.DataElementName + "_Collection";
				}
				else
				{
					tablixMember.DataElementName = table.DetailDataCollectionName;
				}
				this.TransferGroupingCustomProperties(tablixMember, UpgradeImpl2005.TablixMemberGroupAccessor, UpgradeImpl2005.TablixMemberCustomPropertiesAccessor);
				for (int i = 0; i < details.TableRows.Count; i++)
				{
					TablixMember tablixMember2 = new TablixMember();
					tablixMember.TablixMembers.Add(tablixMember2);
					tablixMember.KeepTogether = true;
					TablixRow obj5 = this.UpgradeTableRow(details.TableRows[i], table, num + i, tablixMember2);
					if (group.GroupExpressions.Count == 0)
					{
						string name = table.Name;
						if (table.TableGroups.Count > 0)
						{
							name = table.TableGroups[table.TableGroups.Count - 1].Grouping.Name;
						}
						this.FixAggregateFunction(obj5, name, table.Name, false, ref flag);
						this.FixAggregateFunction(tablixMember2, name, table.Name, false, ref flag);
					}
				}
				if (details.Sorting != null && details.Sorting.Count != 0)
				{
					if (!flag || table.TableGroups.Count != 0 || this.SortingContainsAggregate(details.Sorting))
					{
						tablixMember.SortExpressions = details.Sorting;
					}
					else
					{
						table.SortExpressions = details.Sorting;
					}
				}
			}
			if (table.TablixBody.TablixRows.Count == 0)
			{
				if (table.TablixBody.TablixColumns.Count > 0)
				{
					TablixMember tablixMember = new TablixMember();
					table.TablixRowHierarchy.TablixMembers.Add(tablixMember);
					tablixMember.Visibility = new AspNetCore.ReportingServices.RdlObjectModel.Visibility();
					tablixMember.Visibility.Hidden = true;
					TablixRow tablixRow2 = new TablixRow();
					table.TablixBody.TablixRows.Insert(num, tablixRow2);
					for (int j = 0; j < count; j++)
					{
						TablixCell tablixCell2 = new TablixCell();
						tablixCell2.CellContents = new CellContents();
						tablixRow2.TablixCells.Add(tablixCell2);
					}
				}
				else
				{
					table.TablixBody = null;
				}
			}
			this.UpgradePageBreaks(table, true);
		}

		private bool SortingContainsAggregate(IList<SortExpression> sortExpressions)
		{
			if (sortExpressions != null && sortExpressions.Count != 0)
			{
				for (int i = 0; i < sortExpressions.Count; i++)
				{
					if (this.SortExpressionContainsAggregate(sortExpressions[i]))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private bool SortExpressionContainsAggregate(SortExpression sortExpression)
		{
			if (sortExpression != null && !((string)null == sortExpression.Value) && sortExpression.Value.Value != null && sortExpression.Value.IsExpression)
			{
				return this.ContainsRegexMatch(sortExpression.Value.Value, this.m_regexes.SpecialFunction, "sfname");
			}
			return false;
		}

		private bool ContainsRegexMatch(string expression, Regex regex, string pattern)
		{
			if (string.IsNullOrEmpty(expression))
			{
				return false;
			}
			Match match;
			for (int num = 0; num < expression.Length; num = match.Index + match.Length)
			{
				match = regex.Match(expression, num);
				if (!match.Success)
				{
					return false;
				}
				System.Text.RegularExpressions.Group group = match.Groups[pattern];
				string value = group.Value;
				if (value.Length != 0)
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsToggleable(AspNetCore.ReportingServices.RdlObjectModel.Visibility visibility)
		{
			if (visibility != null)
			{
				if (visibility.ToggleItem == null)
				{
					return visibility.Hidden.IsExpression;
				}
				return true;
			}
			return false;
		}

		private void MergePageBreakLocation(BreakLocations breakLocation, PageBreak pageBreak)
		{
			switch (breakLocation)
			{
			case BreakLocations.Start:
				if (pageBreak.BreakLocation != BreakLocations.End && pageBreak.BreakLocation != BreakLocations.StartAndEnd)
				{
					break;
				}
				pageBreak.BreakLocation = BreakLocations.StartAndEnd;
				break;
			case BreakLocations.End:
				if (pageBreak.BreakLocation != BreakLocations.Start && pageBreak.BreakLocation != BreakLocations.StartAndEnd)
				{
					break;
				}
				pageBreak.BreakLocation = BreakLocations.StartAndEnd;
				break;
			}
		}

		private void UpgradePageBreaks(AspNetCore.ReportingServices.RdlObjectModel.Tablix tablix, bool isTable)
		{
			if (tablix.TablixRowHierarchy != null)
			{
				IList<TablixMember> tablixMembers = tablix.TablixRowHierarchy.TablixMembers;
				if (tablixMembers != null && tablixMembers.Count > 0)
				{
					BreakLocations? nullable = this.UpgradePageBreaks(tablixMembers, UpgradeImpl2005.IsToggleable(tablix.Visibility), isTable);
					if (nullable.HasValue)
					{
						if (tablix.PageBreak == null)
						{
							tablix.PageBreak = new PageBreak();
							tablix.PageBreak.BreakLocation = nullable.Value;
						}
						else
						{
							this.MergePageBreakLocation(nullable.Value, tablix.PageBreak);
						}
					}
				}
			}
		}

		private BreakLocations? UpgradePageBreaks(IList<TablixMember> members, bool thisOrAnscestorHasToggle, bool isTable)
		{
			BreakLocations? result = null;
			bool flag = false;
			int num = (!isTable) ? 1 : members.Count;
			TablixMember tablixMember = null;
			int num2 = 0;
			while (num2 < num)
			{
				TablixMember tablixMember2 = members[num2];
				if (tablixMember2.Group == null)
				{
					if (isTable)
					{
						if (tablixMember2.RepeatOnNewPage)
						{
							flag = true;
						}
					}
					else
					{
						IList<TablixMember> tablixMembers = tablixMember2.TablixMembers;
						if (tablixMembers != null && tablixMembers.Count > 0)
						{
							result = this.UpgradePageBreaks(tablixMembers, thisOrAnscestorHasToggle, isTable);
						}
					}
					num2++;
					continue;
				}
				tablixMember = tablixMember2;
				break;
			}
			if (tablixMember != null)
			{
				thisOrAnscestorHasToggle |= UpgradeImpl2005.IsToggleable(tablixMember.Visibility);
				IList<TablixMember> tablixMembers2 = tablixMember.TablixMembers;
				AspNetCore.ReportingServices.RdlObjectModel.Group group = tablixMember.Group;
				PageBreak pageBreak = group.PageBreak;
				if (tablixMembers2 != null && tablixMembers2.Count > 0)
				{
					result = this.UpgradePageBreaks(tablixMembers2, thisOrAnscestorHasToggle, isTable);
					if (result.HasValue)
					{
						if (pageBreak == null)
						{
							pageBreak = new PageBreak();
							pageBreak.BreakLocation = result.Value;
							group.PageBreak = pageBreak;
						}
						else
						{
							this.MergePageBreakLocation(result.Value, pageBreak);
						}
					}
				}
				if ((!isTable || flag) && pageBreak != null)
				{
					if (!thisOrAnscestorHasToggle)
					{
						result = pageBreak.BreakLocation;
					}
					pageBreak.BreakLocation = BreakLocations.Between;
				}
			}
			return result;
		}

		private ReportSize GetReportItemWidth(AspNetCore.ReportingServices.RdlObjectModel.ReportItem reportItem)
		{
			ReportSize reportSize = reportItem.Width;
			ReportSize reportSize2 = default(ReportSize);
			IContainedObject containedObject = reportItem;
			while (reportSize.IsEmpty && containedObject != null)
			{
				if (containedObject is AspNetCore.ReportingServices.RdlObjectModel.ReportItem)
				{
					reportSize = ((AspNetCore.ReportingServices.RdlObjectModel.ReportItem)containedObject).Width;
					reportSize2 += ((AspNetCore.ReportingServices.RdlObjectModel.ReportItem)containedObject).Left;
				}
				else
				{
					if (containedObject is AspNetCore.ReportingServices.RdlObjectModel.Report)
					{
						reportSize = ((AspNetCore.ReportingServices.RdlObjectModel.Report)containedObject).Width;
						break;
					}
					if (containedObject is TableCell2005)
					{
						TableCell2005 tableCell = (TableCell2005)containedObject;
						TableRow2005 tableRow = (TableRow2005)containedObject.Parent;
						Table2005 parentTable = this.GetParentTable(tableRow);
						int num = tableRow.TableCells.IndexOf((TableCell2005)containedObject);
						if (num < parentTable.TableColumns.Count)
						{
							reportSize = parentTable.TableColumns[num].Width;
						}
						int num2 = tableCell.ColSpan;
						while (--num2 > 0 && ++num < parentTable.TableColumns.Count)
						{
							reportSize += parentTable.TableColumns[num].Width;
						}
						break;
					}
					if (containedObject is MatrixCell2005)
					{
						MatrixRow2005 matrixRow = (MatrixRow2005)containedObject.Parent;
						Matrix2005 matrix = (Matrix2005)matrixRow.Parent;
						int num3 = matrixRow.MatrixCells.IndexOf((MatrixCell2005)containedObject);
						if (num3 < matrix.MatrixColumns.Count)
						{
							reportSize = matrix.MatrixColumns[num3].Width;
						}
						break;
					}
				}
				containedObject = containedObject.Parent;
			}
			reportSize = ((!reportSize.IsEmpty) ? (reportSize - reportSize2) : new ReportSize(0.0));
			return reportSize;
		}

		private ReportSize GetReportItemHeight(AspNetCore.ReportingServices.RdlObjectModel.ReportItem reportItem)
		{
			ReportSize reportSize = reportItem.Height;
			ReportSize reportSize2 = default(ReportSize);
			IContainedObject containedObject = reportItem;
			while (reportSize.IsEmpty && containedObject != null)
			{
				if (containedObject is AspNetCore.ReportingServices.RdlObjectModel.ReportItem)
				{
					reportSize = ((AspNetCore.ReportingServices.RdlObjectModel.ReportItem)containedObject).Height;
					if (reportSize.IsEmpty)
					{
						reportSize2 += ((AspNetCore.ReportingServices.RdlObjectModel.ReportItem)containedObject).Top;
					}
				}
				else
				{
					if (containedObject is Body)
					{
						reportSize = ((Body)containedObject).Height;
						break;
					}
					if (containedObject is TableCell2005)
					{
						TableRow2005 tableRow = (TableRow2005)containedObject.Parent;
						reportSize = tableRow.Height;
						break;
					}
					if (containedObject is MatrixCell2005)
					{
						MatrixRow2005 matrixRow = (MatrixRow2005)containedObject.Parent;
						reportSize = matrixRow.Height;
						break;
					}
				}
				containedObject = containedObject.Parent;
			}
			reportSize = ((!reportSize.IsEmpty) ? (reportSize - reportSize2) : new ReportSize(0.0));
			return reportSize;
		}

		private Table2005 GetParentTable(TableRow2005 row)
		{
			for (IContainedObject parent = row.Parent; parent != null; parent = parent.Parent)
			{
				if (parent is Table2005)
				{
					return (Table2005)parent;
				}
			}
			return null;
		}

		private void FixAggregateFunction(object obj)
		{
			bool flag = false;
			this.FixAggregateFunction(obj, null, null, true, ref flag);
		}

		private void FixAggregateFunction(object obj, ref bool containsPostSortAggregate)
		{
			this.FixAggregateFunction(obj, (string)null, (string)null, true, ref containsPostSortAggregate);
		}

		private void FixAggregateFunction(object obj, string defaultScope, string dataRegion, bool fixPreviousAggregate, ref bool containsPostSortAggregate)
		{
			if (obj is IList)
			{
				foreach (object item in (IList)obj)
				{
					this.FixAggregateFunction(item, defaultScope, dataRegion, fixPreviousAggregate, ref containsPostSortAggregate);
				}
			}
			else if (obj is ReportObject)
			{
				Type type = obj.GetType();
				StructMapping structMapping = (StructMapping)TypeMapper.GetTypeMapping(type);
				foreach (MemberMapping member in structMapping.Members)
				{
					object value = member.GetValue(obj);
					if (value != null)
					{
						if (typeof(IExpression).IsAssignableFrom(value.GetType()))
						{
							member.SetValue(obj, this.FixAggregateFunction((IExpression)value, defaultScope, dataRegion, fixPreviousAggregate, ref containsPostSortAggregate));
						}
						else
						{
							this.FixAggregateFunction(value, defaultScope, dataRegion, fixPreviousAggregate, ref containsPostSortAggregate);
						}
					}
				}
			}
		}

		private IExpression FixAggregateFunction(IExpression value, string defaultScope, string dataRegion, bool fixPreviousAggregates, ref bool containsPostSortAggregate)
		{
			if (value != null && value.IsExpression && !string.IsNullOrEmpty(value.Expression))
			{
				string expression = value.Expression;
				if (this.ContainsRegexMatch(expression, this.m_regexes.PSAFunction, "psaname"))
				{
					containsPostSortAggregate = true;
				}
				if (this.m_regexes.SpecialFunction.IsMatch(expression))
				{
					expression = this.FixAggregateFunctions(expression, delegate(string expr, int currentOffset, string specialFunctionName, int specialFunctionPos, int argumentsPos, int scopePos, int scopeLength, ref int offset)
					{
						if (!specialFunctionName.Equals("Previous", StringComparison.OrdinalIgnoreCase))
						{
							if (scopeLength == 0 && defaultScope != null)
							{
								string text = (this.GetScopeArgumentIndex(specialFunctionName) > 0) ? ", " : "";
								expr = expr.Substring(0, offset - 1) + text + "\"" + defaultScope + "\")" + expr.Substring(offset);
								offset += defaultScope.Length + 4;
							}
						}
						else if (fixPreviousAggregates)
						{
							expr = this.FixPreviousAggregate(expr, currentOffset, specialFunctionPos, argumentsPos, ref offset);
						}
						return expr;
					});
					value = (IExpression)Activator.CreateInstance(value.GetType());
					value.Expression = expression;
				}
			}
			return value;
		}

		private string FixPreviousAggregate(string expr, int currentOffset, int specialFunctionPos, int argumentsPos, ref int offset)
		{
			Match match = this.m_regexes.SpecialFunction.Match(expr, argumentsPos);
			if (match.Success)
			{
				System.Text.RegularExpressions.Group group = match.Groups["sfname"];
				if (group.Length > 0 && group.Index <= offset)
				{
					return expr;
				}
			}
			int num = default(int);
			int num2 = default(int);
			int num3 = default(int);
			if (this.FindArgument(currentOffset, expr, out num, 0, out num2, out num3))
			{
				Match match2 = this.m_regexes.FieldDetection.Match(expr, num2 - 1);
				if (match2.Success)
				{
					int num4 = 0;
					int num5 = num2;
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(expr.Substring(0, num5));
					while (match2.Success && num5 < num2 + num3)
					{
						System.Text.RegularExpressions.Group group2 = match2.Groups["detected"];
						if (group2.Length > 0)
						{
							stringBuilder.Append(expr.Substring(num5, group2.Index - num5));
							int num6 = 0;
							int i;
							for (i = group2.Index; i < num2 + num3 && !this.IsNotPartOfReference(expr[i]); i++)
							{
								if (expr[i] == '(')
								{
									num6++;
								}
								else if (expr[i] == ')')
								{
									if (num6 == 0)
									{
										break;
									}
									num6--;
								}
							}
							stringBuilder.Append("Last(");
							stringBuilder.Append(expr.Substring(group2.Index, i - group2.Index));
							stringBuilder.Append(")");
							num5 = i;
							num4++;
						}
						match2 = match2.NextMatch();
					}
					stringBuilder.Append(expr.Substring(num5));
					expr = stringBuilder.ToString();
					offset += num4 * 6;
				}
			}
			return expr;
		}

		private bool IsNotPartOfReference(char c)
		{
			switch (c)
			{
			case '\t':
			case '\n':
			case '\r':
			case ' ':
			case '&':
			case '*':
			case '+':
			case '-':
			case '/':
			case '<':
			case '=':
			case '>':
			case '\\':
			case '^':
				return true;
			default:
				return false;
			}
		}

		private string FixAggregateFunctions(string expression, AggregateFunctionFixup fixup)
		{
			int num = 0;
			while (num < expression.Length)
			{
				Match match = this.m_regexes.SpecialFunction.Match(expression, num);
				if (!match.Success)
				{
					break;
				}
				System.Text.RegularExpressions.Group group = match.Groups["sfname"];
				string value = group.Value;
				if (value.Length == 0)
				{
					num = match.Index + match.Length;
				}
				else
				{
					int index = group.Index;
					int argumentsPos = index + group.Length;
					num = match.Index + match.Length;
					int scopeArgumentIndex = this.GetScopeArgumentIndex(value);
					int scopePos = default(int);
					int scopeLength = default(int);
					this.FindArgument(num, expression, out num, scopeArgumentIndex, out scopePos, out scopeLength);
					expression = fixup(expression, match.Index + match.Length, value, index, argumentsPos, scopePos, scopeLength, ref num);
				}
			}
			return expression;
		}

		private TablixRow UpgradeTableRow(TableRow2005 tableRow, AspNetCore.ReportingServices.RdlObjectModel.Tablix tablix, int rowIndex, TablixMember tablixMember)
		{
			tablixMember.Visibility = tableRow.Visibility;
			TablixRow tablixRow = new TablixRow();
			tablix.TablixBody.TablixRows.Insert(rowIndex, tablixRow);
			tablixRow.Height = tableRow.Height;
			IList<TablixMember> tablixMembers = tablix.TablixColumnHierarchy.TablixMembers;
			int num = 0;
			foreach (TableCell2005 tableCell in tableRow.TableCells)
			{
				TablixCell tablixCell = new TablixCell();
				tablixRow.TablixCells.Add(tablixCell);
				tablixCell.CellContents = new CellContents();
				if (tableCell.ReportItems.Count > 0)
				{
					tablixCell.CellContents.ReportItem = tableCell.ReportItems[0];
				}
				if (tableCell.ColSpan > 1)
				{
					int index = num + tableCell.ColSpan - 1;
					bool flag = tablixMembers[num].FixedData || tablixMembers[index].FixedData;
					tablixCell.CellContents.ColSpan = tableCell.ColSpan;
					for (int i = 0; i < tableCell.ColSpan; i++)
					{
						if (i > 0)
						{
							tablixRow.TablixCells.Add(new TablixCell());
						}
						if (flag)
						{
							tablixMembers[num].FixedData = true;
						}
						num++;
					}
				}
				else
				{
					num++;
				}
			}
			return tablixRow;
		}

		internal void UpgradeChart(Chart2005 chart2005)
		{
			this.UpgradeReportItem(chart2005);
			this.UpgradePageBreak(chart2005);
			if (chart2005.CustomProperties == null)
			{
				chart2005.CustomProperties = new List<CustomProperty>();
			}
			CustomProperty customProperty = new CustomProperty();
			customProperty.Name = "__Upgraded2005__";
			customProperty.Value = "__Upgraded2005__";
			chart2005.CustomProperties.Add(customProperty);
			IList<ChartMember> list = null;
			ChartMember chartMember = null;
			foreach (SeriesGrouping2005 seriesGrouping2 in chart2005.SeriesGroupings)
			{
				if (list == null)
				{
					chart2005.ChartSeriesHierarchy = new ChartSeriesHierarchy();
					list = chart2005.ChartSeriesHierarchy.ChartMembers;
				}
				DynamicSeries2005 dynamicSeries = seriesGrouping2.DynamicSeries;
				if (dynamicSeries != null)
				{
					ChartMember chartMember2 = new ChartMember();
					list.Add(chartMember2);
					list = chartMember2.ChartMembers;
					chartMember = chartMember2;
					chartMember2.Group = dynamicSeries.Grouping;
					chartMember2.SortExpressions = dynamicSeries.Sorting;
					chartMember2.Label = dynamicSeries.Label;
					chartMember2.PropertyStore.SetObject(4, dynamicSeries.LabelLocID);
					chartMember2.DataElementName = dynamicSeries.Grouping.DataCollectionName;
					chartMember2.DataElementOutput = dynamicSeries.Grouping.DataElementOutput;
					this.TransferGroupingCustomProperties(chartMember2, UpgradeImpl2005.ChartMemberGroupAccessor, UpgradeImpl2005.ChartMemberCustomPropertiesAccessor);
				}
				else
				{
					foreach (StaticMember2005 item in seriesGrouping2.StaticSeries)
					{
						ChartMember chartMember2 = new ChartMember();
						list.Add(chartMember2);
						chartMember2.Label = item.Label;
						chartMember2.PropertyStore.SetObject(4, item.LabelLocID);
					}
					if (list.Count > 0)
					{
						chartMember = list[0];
						list = chartMember.ChartMembers;
					}
				}
			}
			list = null;
			foreach (CategoryGrouping2005 categoryGrouping in chart2005.CategoryGroupings)
			{
				if (list == null)
				{
					chart2005.ChartCategoryHierarchy = new ChartCategoryHierarchy();
					list = chart2005.ChartCategoryHierarchy.ChartMembers;
				}
				DynamicSeries2005 dynamicCategories = categoryGrouping.DynamicCategories;
				if (dynamicCategories != null)
				{
					ChartMember chartMember3 = new ChartMember();
					list.Add(chartMember3);
					list = chartMember3.ChartMembers;
					chartMember3.Group = dynamicCategories.Grouping;
					chartMember3.SortExpressions = dynamicCategories.Sorting;
					chartMember3.Label = dynamicCategories.Label;
					chartMember3.PropertyStore.SetObject(4, dynamicCategories.LabelLocID);
					chartMember3.DataElementName = dynamicCategories.Grouping.DataCollectionName;
					chartMember3.DataElementOutput = dynamicCategories.Grouping.DataElementOutput;
					this.TransferGroupingCustomProperties(chartMember3, UpgradeImpl2005.ChartMemberGroupAccessor, UpgradeImpl2005.ChartMemberCustomPropertiesAccessor);
					continue;
				}
				foreach (StaticMember2005 staticCategory in categoryGrouping.StaticCategories)
				{
					ChartMember chartMember3 = new ChartMember();
					list.Add(chartMember3);
					chartMember3.Label = staticCategory.Label;
					chartMember3.PropertyStore.SetObject(4, staticCategory.LabelLocID);
				}
				break;
			}
			if (chart2005.Palette.Value == ChartPalettes.GrayScale)
			{
				chart2005.PaletteHatchBehavior = ChartPaletteHatchBehaviorTypes.Always;
			}
			if (chart2005.Action != null)
			{
				chart2005.ActionInfo = new ActionInfo();
				chart2005.ActionInfo.Actions.Add(chart2005.Action);
			}
			if (chart2005.NoRows != (string)null)
			{
				chart2005.ChartNoDataMessage = new AspNetCore.ReportingServices.RdlObjectModel.ChartTitle();
				chart2005.ChartNoDataMessage.Name = "NoDataMessageTitle";
				chart2005.ChartNoDataMessage.Caption = chart2005.NoRows.ToString();
			}
			ChartArea chartArea = new ChartArea();
			chart2005.ChartAreas.Add(chartArea);
			chartArea.Name = "Default";
			if (chart2005.ThreeDProperties != null)
			{
				chartArea.ChartThreeDProperties = new ChartThreeDProperties();
				chartArea.ChartThreeDProperties.Clustered = !chart2005.ThreeDProperties.Clustered.Value;
				chartArea.ChartThreeDProperties.DepthRatio = chart2005.ThreeDProperties.DepthRatio;
				chartArea.ChartThreeDProperties.Enabled = chart2005.ThreeDProperties.Enabled;
				chartArea.ChartThreeDProperties.GapDepth = chart2005.ThreeDProperties.GapDepth;
				chartArea.ChartThreeDProperties.Inclination = chart2005.ThreeDProperties.Rotation;
				chartArea.ChartThreeDProperties.Rotation = chart2005.ThreeDProperties.Inclination;
				chartArea.ChartThreeDProperties.Shading = chart2005.ThreeDProperties.Shading;
				ChartProjectionModes projectionMode = (ChartProjectionModes)chart2005.ThreeDProperties.ProjectionMode;
				chartArea.ChartThreeDProperties.ProjectionMode = projectionMode;
				if (projectionMode == ChartProjectionModes.Perspective)
				{
					chartArea.ChartThreeDProperties.Perspective = chart2005.ThreeDProperties.Perspective;
				}
				int num = (int)(30.0 * ((double)chart2005.ThreeDProperties.WallThickness / 100.0));
				chartArea.ChartThreeDProperties.WallThickness = ((num > 30 || num < 0) ? 7 : num);
			}
			if (chart2005.PlotArea != null)
			{
				chartArea.Style = chart2005.PlotArea.Style;
				this.FixYukonChartBorderWidth(chartArea.Style, false);
			}
			if (((ReportElement)chart2005).Style != null && ((ReportElement)chart2005).Style.BackgroundImage != null)
			{
				if (chartArea.Style == null)
				{
					chartArea.Style = new AspNetCore.ReportingServices.RdlObjectModel.Style();
				}
				chartArea.Style.BackgroundImage = ((ReportElement)chart2005).Style.BackgroundImage;
				((ReportElement)chart2005).Style.BackgroundImage = null;
			}
			if (chart2005.Title != null && chart2005.Title.Caption.Value.Length > 0)
			{
				AspNetCore.ReportingServices.RdlObjectModel.ChartTitle chartTitle = new AspNetCore.ReportingServices.RdlObjectModel.ChartTitle();
				chartTitle.Name = "Default";
				chart2005.ChartTitles.Add(chartTitle);
				chartTitle.Caption = chart2005.Title.Caption;
				chartTitle.PropertyStore.SetObject(2, chart2005.Title.PropertyStore.GetObject(2));
				chartTitle.Style = chart2005.Title.Style;
			}
			ChartLegend chartLegend = new ChartLegend();
			chartLegend.AutoFitTextDisabled = true;
			chartLegend.Name = "Default";
			chart2005.ChartLegends.Add(chartLegend);
			chartLegend.Hidden = !chart2005.Legend.Visible;
			chartLegend.Style = this.FixYukonEmptyBorderStyle(chart2005.Legend.Style);
			this.FixYukonChartBorderWidth(chartLegend.Style, false);
			chartLegend.Position = chart2005.Legend.Position;
			chartLegend.Layout = new ReportExpression<ChartLegendLayouts>((ChartLegendLayouts)chart2005.Legend.Layout);
			if (chart2005.Legend.InsidePlotArea)
			{
				chartLegend.DockOutsideChartArea = !chart2005.Legend.InsidePlotArea;
				chartLegend.DockToChartArea = chartArea.Name;
			}
			if (chart2005.CategoryAxis != null)
			{
				chartArea.ChartCategoryAxes.Add(this.UpgradeChartAxis(chart2005.CategoryAxis.Axis, true, chart2005.Type));
				chartArea.ChartCategoryAxes[0].Name = "Primary";
			}
			if (chart2005.ValueAxis != null)
			{
				chartArea.ChartValueAxes.Add(this.UpgradeChartAxis(chart2005.ValueAxis.Axis, false, chart2005.Type));
				chartArea.ChartValueAxes[0].Name = "Primary";
			}
			if (chart2005.ChartData != null && chart2005.ChartData.Count > 0)
			{
				((AspNetCore.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData = new ChartData();
				foreach (ChartSeries2005 chartDatum in chart2005.ChartData)
				{
					ChartSeries chartSeries = new ChartSeries();
					chartSeries.CategoryAxisName = "Primary";
					chartSeries.ValueAxisName = "Primary";
					this.SetChartTypes(chart2005, chartDatum.PlotType, chartSeries);
					double num2 = 0.8;
					if (chart2005.PointWidth > 0)
					{
						num2 = Math.Min((double)chart2005.PointWidth / 100.0, 2.0);
					}
					else if (chart2005.Type == ChartTypes2005.Bar || chart2005.Type == ChartTypes2005.Column)
					{
						num2 = 0.6;
					}
					if (num2 != 0.8)
					{
						CustomProperty customProperty2 = new CustomProperty();
						customProperty2.Name = "PointWidth";
						customProperty2.Value = num2.ToString(CultureInfo.InvariantCulture.NumberFormat);
						chartSeries.CustomProperties.Add(customProperty2);
					}
					if (chart2005.Type == ChartTypes2005.Bar || chart2005.Type == ChartTypes2005.Column)
					{
						ThreeDProperties2005 threeDProperties = chart2005.ThreeDProperties;
						if (threeDProperties != null && chart2005.ThreeDProperties.Enabled == true && chart2005.ThreeDProperties.DrawingStyle != 0)
						{
							CustomProperty customProperty3 = new CustomProperty();
							customProperty3.Name = "DrawingStyle";
							customProperty3.Value = chart2005.ThreeDProperties.DrawingStyle.ToString();
							chartSeries.CustomProperties.Add(customProperty3);
						}
					}
					((AspNetCore.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData.ChartSeriesCollection.Add(chartSeries);
					chartSeries.Name = "Series" + ((AspNetCore.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData.ChartSeriesCollection.Count.ToString(CultureInfo.InvariantCulture.NumberFormat);
					using (IEnumerator<DataPoint2005> enumerator6 = chartDatum.DataPoints.GetEnumerator())
					{
						AspNetCore.ReportingServices.RdlObjectModel.ChartDataPoint chartDataPoint;
						for (; enumerator6.MoveNext(); chartSeries.ChartDataPoints.Add(chartDataPoint))
						{
							DataPoint2005 current6 = enumerator6.Current;
							chartDataPoint = current6;
							chartDataPoint.DataElementName = current6.DataElementName;
							chartDataPoint.DataElementOutput = ((current6.DataElementOutput == DataElementOutputTypes.Output) ? DataElementOutputTypes.ContentsOnly : current6.DataElementOutput);
							if (current6.Style != null)
							{
								chartDataPoint.Style = new EmptyColorStyle(current6.Style.PropertyStore);
								if (chartDataPoint.Style.Border != null && chartDataPoint.Style.Border.Style == BorderStyles.None)
								{
									chartDataPoint.Style.Border.Style = BorderStyles.Solid;
								}
							}
							ReportExpression<ReportColor> color;
							ReportColor value;
							if (chartSeries.Type == ChartTypes.Line)
							{
								if (chartDataPoint.Style == null)
								{
									chartDataPoint.Style = new EmptyColorStyle();
								}
								if (chartDataPoint.Style.Border == null)
								{
									chartDataPoint.Style.Border = new EmptyBorder();
								}
								if (!chartDataPoint.Style.Border.Width.IsExpression && chartDataPoint.Style.Border.Width.Value.IsEmpty)
								{
									chartDataPoint.Style.Border.Width = new ReportSize(2.25, SizeTypes.Point);
								}
								else
								{
									this.FixYukonChartBorderWidth(chartDataPoint.Style, false);
								}
								color = chartDataPoint.Style.Border.Color;
								value = color.Value;
								if (!value.IsEmpty || chartDataPoint.Style.Border.Color.IsExpression)
								{
									chartDataPoint.Style.Color = chartDataPoint.Style.Border.Color;
									chartDataPoint.Style.Border.Color = ReportColor.Empty;
								}
							}
							else
							{
								if (chartDataPoint.Style != null && (!chartDataPoint.Style.BackgroundColor.Value.IsEmpty || chartDataPoint.Style.BackgroundColor.IsExpression))
								{
									chartDataPoint.Style.Color = chartDataPoint.Style.BackgroundColor;
									chartDataPoint.Style.BackgroundColor = ReportColor.Empty;
								}
								this.FixYukonChartBorderWidth(chartDataPoint.Style, false);
							}
							if (chart2005.Type == ChartTypes2005.Pie || chart2005.Type == ChartTypes2005.Doughnut)
							{
								if (chartDataPoint.Style == null)
								{
									chartDataPoint.Style = new EmptyColorStyle();
								}
								if (chartDataPoint.Style.Border == null)
								{
									chartDataPoint.Style.Border = new EmptyBorder();
								}
								if (!chartDataPoint.Style.Border.Color.IsExpression && chartDataPoint.Style.Border.Color.Value.IsEmpty)
								{
									chartDataPoint.Style.Border.Color = new ReportColor(Color.Black);
								}
							}
							if (current6.DataValues != null && current6.DataValues.Count > 0)
							{
								chartDataPoint.ChartDataPointValues = new ChartDataPointValues();
								switch (chart2005.Type)
								{
								case ChartTypes2005.Scatter:
									this.SetChartDataPointNames(current6, UpgradeImpl2005.m_scatterChartDataPointNames);
									break;
								case ChartTypes2005.Bubble:
									this.SetChartDataPointNames(current6, UpgradeImpl2005.m_bubbleChartDataPointNames);
									break;
								case ChartTypes2005.Stock:
								{
									string[] names = (chart2005.Subtype != ChartSubtypes2005.HighLowClose) ? UpgradeImpl2005.m_openHighLowCloseDataPointNames : UpgradeImpl2005.m_highLowCloseDataPointNames;
									this.SetChartDataPointNames(current6, names);
									break;
								}
								}
								foreach (DataValue2005 dataValue in current6.DataValues)
								{
									switch (dataValue.Name)
									{
									case "X":
										chartDataPoint.ChartDataPointValues.X = dataValue.Value;
										break;
									case "High":
										chartDataPoint.ChartDataPointValues.High = dataValue.Value;
										break;
									case "Low":
										chartDataPoint.ChartDataPointValues.Low = dataValue.Value;
										break;
									case "Open":
										chartDataPoint.ChartDataPointValues.Start = dataValue.Value;
										break;
									case "Close":
										chartDataPoint.ChartDataPointValues.End = dataValue.Value;
										break;
									case "Size":
										chartDataPoint.ChartDataPointValues.Size = dataValue.Value;
										break;
									default:
										chartDataPoint.ChartDataPointValues.Y = dataValue.Value;
										break;
									}
								}
							}
							if (current6.Action != null)
							{
								current6.ActionInfo = new ActionInfo();
								current6.ActionInfo.Actions.Add(current6.Action);
							}
							if (current6.DataLabel != null)
							{
								AspNetCore.ReportingServices.RdlObjectModel.ChartDataLabel chartDataLabel2 = chartDataPoint.ChartDataLabel = new AspNetCore.ReportingServices.RdlObjectModel.ChartDataLabel();
								chartDataLabel2.Visible = current6.DataLabel.Visible;
								chartDataLabel2.UseValueAsLabel = (current6.DataLabel.Visible && current6.DataLabel.Value == (string)null);
								chartDataLabel2.Style = current6.DataLabel.Style;
								chartDataLabel2.Label = current6.DataLabel.Value;
								chartDataLabel2.PropertyStore.SetObject(2, current6.DataLabel.ValueLocID);
								chartDataLabel2.Rotation = current6.DataLabel.Rotation;
								if (current6.DataLabel.Position != ChartDataLabelPositions.Auto)
								{
									if (chartSeries.ChartSmartLabel == null)
									{
										chartSeries.ChartSmartLabel = new ChartSmartLabel();
									}
									chartSeries.ChartSmartLabel.Disabled = true;
								}
								if ((chart2005.Type == ChartTypes2005.Pie || chart2005.Type == ChartTypes2005.Doughnut) && current6.DataLabel.Position != ChartDataLabelPositions.Auto && current6.DataLabel.Position != ChartDataLabelPositions.Center)
								{
									CustomProperty customProperty4 = new CustomProperty();
									customProperty4.Name = "PieLabelStyle";
									customProperty4.Value = "Outside";
									current6.CustomProperties.Add(customProperty4);
									goto IL_1047;
								}
								chartDataLabel2.Position = current6.DataLabel.Position;
							}
							goto IL_1047;
							IL_11f2:
							chartDataPoint.Style.BackgroundHatchType = BackgroundHatchTypes.None;
							goto IL_1204;
							IL_1047:
							if (current6.Marker != null)
							{
								chartDataPoint.ChartMarker = new ChartMarker();
								chartDataPoint.ChartMarker.Type = current6.Marker.Type;
								chartDataPoint.ChartMarker.Size = current6.Marker.Size;
								if (current6.Marker.Style != null)
								{
									chartDataPoint.ChartMarker.Style = new EmptyColorStyle(current6.Marker.Style.PropertyStore);
									chartDataPoint.ChartMarker.Style.Color = chartDataPoint.ChartMarker.Style.BackgroundColor;
									chartDataPoint.ChartMarker.Style.BackgroundColor = ReportColor.Empty;
								}
							}
							if (chart2005.Type == ChartTypes2005.Bubble)
							{
								if (chartDataPoint.ChartMarker == null)
								{
									chartDataPoint.ChartMarker = new ChartMarker();
								}
								if (chartDataPoint.ChartMarker.Type == ChartMarkerTypes.None)
								{
									chartDataPoint.ChartMarker.Type = ChartMarkerTypes.Circle;
								}
							}
							if (chart2005.Palette.Value == ChartPalettes.GrayScale)
							{
								if (chartDataPoint.Style == null)
								{
									chartDataPoint.Style = new EmptyColorStyle();
									goto IL_1204;
								}
								if (!chartDataPoint.Style.Color.IsExpression)
								{
									color = chartDataPoint.Style.Color;
									value = color.Value;
									if (value.IsEmpty)
									{
										ReportExpression<BackgroundGradients> backgroundGradientType = chartDataPoint.Style.BackgroundGradientType;
										if (!backgroundGradientType.IsExpression)
										{
											backgroundGradientType = chartDataPoint.Style.BackgroundGradientType;
											if (backgroundGradientType.Value != 0)
											{
												backgroundGradientType = chartDataPoint.Style.BackgroundGradientType;
												if (backgroundGradientType.Value != BackgroundGradients.None)
												{
													goto IL_11f2;
												}
											}
											goto IL_1204;
										}
									}
								}
								goto IL_11f2;
							}
							continue;
							IL_1204:
							if (chartDataPoint.Style.Border == null)
							{
								chartDataPoint.Style.Border = new EmptyBorder();
								chartDataPoint.Style.Border.Color = new ReportColor(Color.Black);
								chartDataPoint.Style.Border.Width = new ReportSize(0.75, SizeTypes.Point);
								chartDataPoint.Style.Border.Style = BorderStyles.Solid;
							}
							else
							{
								color = chartDataPoint.Style.Border.Color;
								if (!color.IsExpression)
								{
									color = chartDataPoint.Style.Border.Color;
									value = color.Value;
									if (value.IsEmpty)
									{
										chartDataPoint.Style.Border.Color = new ReportColor(Color.Black);
									}
								}
							}
						}
					}
				}
			}
			if (chart2005.ChartCategoryHierarchy == null || chart2005.ChartCategoryHierarchy.ChartMembers == null || chart2005.ChartCategoryHierarchy.ChartMembers.Count == 0)
			{
				if (chart2005.ChartCategoryHierarchy == null)
				{
					chart2005.ChartCategoryHierarchy = new ChartCategoryHierarchy();
				}
				if (chart2005.ChartCategoryHierarchy.ChartMembers == null)
				{
					chart2005.ChartCategoryHierarchy.ChartMembers = new RdlCollection<ChartMember>();
				}
				if (((AspNetCore.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData != null && ((AspNetCore.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData.ChartSeriesCollection != null && ((AspNetCore.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData.ChartSeriesCollection.Count > 0)
				{
					foreach (AspNetCore.ReportingServices.RdlObjectModel.ChartDataPoint chartDataPoint2 in ((AspNetCore.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData.ChartSeriesCollection[0].ChartDataPoints)
					{
						AspNetCore.ReportingServices.RdlObjectModel.ChartDataPoint chartDataPoint3 = chartDataPoint2;
						chart2005.ChartCategoryHierarchy.ChartMembers.Add(new ChartMember());
					}
				}
				else
				{
					chart2005.ChartCategoryHierarchy.ChartMembers.Add(new ChartMember());
				}
			}
			if (chart2005.ChartSeriesHierarchy == null || chart2005.ChartSeriesHierarchy.ChartMembers == null || chart2005.ChartSeriesHierarchy.ChartMembers.Count == 0)
			{
				if (chart2005.ChartSeriesHierarchy == null)
				{
					chart2005.ChartSeriesHierarchy = new ChartSeriesHierarchy();
				}
				if (chart2005.ChartSeriesHierarchy.ChartMembers == null)
				{
					chart2005.ChartSeriesHierarchy.ChartMembers = new RdlCollection<ChartMember>();
				}
				chart2005.ChartSeriesHierarchy.ChartMembers.Add(new ChartMember());
			}
			if (((AspNetCore.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData == null || ((AspNetCore.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData.ChartSeriesCollection == null || ((AspNetCore.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData.ChartSeriesCollection.Count == 0)
			{
				if (((AspNetCore.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData == null)
				{
					((AspNetCore.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData = new ChartData();
				}
				if (((AspNetCore.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData.ChartSeriesCollection == null)
				{
					((AspNetCore.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData.ChartSeriesCollection = new RdlCollection<ChartSeries>();
				}
				ChartSeries chartSeries2 = new ChartSeries();
				chartSeries2.Name = "emptySeriesName";
				chartSeries2.ChartDataPoints.Add(new AspNetCore.ReportingServices.RdlObjectModel.ChartDataPoint());
				((AspNetCore.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData.ChartSeriesCollection.Add(chartSeries2);
			}
			int num3 = chart2005.SeriesGroupings.Count;
			while (--num3 >= 0)
			{
				SeriesGrouping2005 seriesGrouping = chart2005.SeriesGroupings[num3];
				if (seriesGrouping.StaticSeries.Count > 0 && num3 < chart2005.SeriesGroupings.Count - 1)
				{
					this.CloneChartSeriesHierarchy(chart2005, chartMember);
				}
				if (num3 > 0)
				{
					chartMember = (ChartMember)chartMember.Parent;
				}
			}
		}

		private void CloneChartSeriesHierarchy(AspNetCore.ReportingServices.RdlObjectModel.Chart chart, ChartMember staticMember)
		{
			if (staticMember.ChartMembers.Count != 0)
			{
				ChartData chartData = chart.ChartData;
				IList<ChartMember> siblingChartMembers = this.GetSiblingChartMembers(staticMember);
				int count = siblingChartMembers.Count;
				for (int i = 1; i < count; i++)
				{
					ChartMember chartMember = siblingChartMembers[i];
					Cloner cloner = new Cloner(this);
					chartMember.ChartMembers = (IList<ChartMember>)cloner.Clone(staticMember.ChartMembers);
					cloner.FixReferences();
					int num = chartData.ChartSeriesCollection.Count / count;
					int num2 = num;
					int num3 = i * num;
					while (num2-- > 0)
					{
						cloner.FixReferences(chartData.ChartSeriesCollection[num3].ChartDataPoints);
						num3++;
					}
				}
			}
		}

		private IList<ChartMember> GetSiblingChartMembers(ChartMember chartMember)
		{
			if (chartMember.Parent is ChartSeriesHierarchy)
			{
				return ((ChartSeriesHierarchy)chartMember.Parent).ChartMembers;
			}
			if (chartMember.Parent is ChartCategoryHierarchy)
			{
				return ((ChartCategoryHierarchy)chartMember.Parent).ChartMembers;
			}
			return ((ChartMember)chartMember.Parent).ChartMembers;
		}

		private ChartAxis UpgradeChartAxis(Axis2005 axis2005, bool categoryAxis, ChartTypes2005 charType)
		{
			ChartAxis chartAxis = new ChartAxis();
			chartAxis.HideLabels = !axis2005.Visible;
			chartAxis.Margin = new ReportExpression<ChartAxisMarginVisibleTypes>(axis2005.Margin.ToString(), CultureInfo.InvariantCulture);
			chartAxis.Reverse = axis2005.Reverse;
			chartAxis.CrossAt = axis2005.CrossAt;
			chartAxis.Interlaced = axis2005.Interlaced;
			chartAxis.Scalar = axis2005.Scalar;
			chartAxis.LogScale = axis2005.LogScale;
			chartAxis.PreventFontShrink = true;
			chartAxis.PreventFontGrow = true;
			chartAxis.Style = this.FixYukonEmptyBorderStyle(axis2005.Style);
			this.FixYukonChartBorderWidth(chartAxis.Style, true);
			if (!categoryAxis || chartAxis.Scalar)
			{
				chartAxis.Minimum = axis2005.Min;
				chartAxis.Maximum = axis2005.Max;
			}
			chartAxis.IncludeZero = false;
			double num = double.NaN;
			double num2 = double.NaN;
			if (axis2005.MajorGridLines != null)
			{
				chartAxis.ChartMajorGridLines = new ChartGridLines();
				chartAxis.ChartMajorGridLines.Enabled = new ReportExpression<ChartGridLinesEnabledTypes>(axis2005.MajorGridLines.ShowGridLines.ToString(), CultureInfo.InvariantCulture);
				chartAxis.ChartMajorGridLines.Style = this.FixYukonEmptyBorderStyle(axis2005.MajorGridLines.Style);
				this.FixYukonChartBorderWidth(chartAxis.ChartMajorGridLines.Style, true);
				if (axis2005.MajorInterval.IsExpression)
				{
					chartAxis.ChartMajorGridLines.Interval = new ReportExpression<double>(axis2005.MajorInterval.ToString(), CultureInfo.InvariantCulture);
				}
				else
				{
					num = this.ConvertToDouble(axis2005.MajorInterval.Value);
					if (num < 0.0)
					{
						num = double.NaN;
					}
					chartAxis.ChartMajorGridLines.Interval = num;
					if (!chartAxis.Scalar && chartAxis.Margin == ChartAxisMarginVisibleTypes.True)
					{
						chartAxis.ChartMajorGridLines.IntervalOffset = 1.0;
					}
				}
			}
			if (axis2005.MinorGridLines != null)
			{
				chartAxis.ChartMinorGridLines = new ChartGridLines();
				chartAxis.ChartMinorGridLines.Enabled = new ReportExpression<ChartGridLinesEnabledTypes>(axis2005.MinorGridLines.ShowGridLines.ToString(), CultureInfo.InvariantCulture);
				chartAxis.ChartMinorGridLines.Style = this.FixYukonEmptyBorderStyle(axis2005.MinorGridLines.Style);
				this.FixYukonChartBorderWidth(chartAxis.ChartMinorGridLines.Style, true);
				if (axis2005.MinorInterval.IsExpression)
				{
					chartAxis.ChartMinorGridLines.Interval = new ReportExpression<double>(axis2005.MinorInterval.ToString(), CultureInfo.InvariantCulture);
				}
				else
				{
					num2 = this.ConvertToDouble(axis2005.MinorInterval.Value);
					if (num2 < 0.0)
					{
						num2 = double.NaN;
					}
					chartAxis.ChartMinorGridLines.Interval = num2;
					if (!chartAxis.Scalar && chartAxis.Margin == ChartAxisMarginVisibleTypes.False)
					{
						chartAxis.ChartMinorGridLines.IntervalOffset = -1.0;
					}
				}
			}
			chartAxis.ChartMajorTickMarks = new ChartTickMarks();
			chartAxis.ChartMajorTickMarks.Type = new ReportExpression<ChartTickMarkTypes>(axis2005.MajorTickMarks.ToString(), CultureInfo.InvariantCulture);
			if (axis2005.MajorTickMarks != TickMarks2005.None)
			{
				if (chartAxis.ChartMajorGridLines != null)
				{
					chartAxis.ChartMajorTickMarks.Style = chartAxis.ChartMajorGridLines.Style;
					chartAxis.ChartMajorTickMarks.Interval = chartAxis.ChartMajorGridLines.Interval;
					chartAxis.ChartMajorTickMarks.IntervalOffset = chartAxis.ChartMajorGridLines.IntervalOffset;
				}
				chartAxis.ChartMajorTickMarks.Enabled = ChartTickMarksEnabledTypes.True;
			}
			chartAxis.ChartMinorTickMarks = new ChartTickMarks();
			chartAxis.ChartMinorTickMarks.Type = new ReportExpression<ChartTickMarkTypes>(axis2005.MinorTickMarks.ToString(), CultureInfo.InvariantCulture);
			if (axis2005.MinorTickMarks != TickMarks2005.None)
			{
				if (chartAxis.ChartMinorGridLines != null)
				{
					chartAxis.ChartMinorTickMarks.Style = chartAxis.ChartMinorGridLines.Style;
					chartAxis.ChartMinorTickMarks.Interval = chartAxis.ChartMinorGridLines.Interval;
					chartAxis.ChartMinorTickMarks.IntervalOffset = chartAxis.ChartMinorGridLines.IntervalOffset;
				}
				chartAxis.ChartMinorTickMarks.Enabled = ChartTickMarksEnabledTypes.True;
			}
			if (axis2005.Title != null && axis2005.Title.Caption != (string)null)
			{
				ChartAxisTitle chartAxisTitle2 = chartAxis.ChartAxisTitle = new ChartAxisTitle();
				chartAxisTitle2.Caption = axis2005.Title.Caption;
				chartAxisTitle2.PropertyStore.SetObject(1, axis2005.Title.PropertyStore.GetObject(2));
				chartAxisTitle2.Position = new ReportExpression<ChartAxisTitlePositions>(axis2005.Title.Position.ToString(), CultureInfo.InvariantCulture);
				chartAxisTitle2.Style = axis2005.Title.Style;
			}
			if (categoryAxis)
			{
				if (!chartAxis.Scalar)
				{
					if (!double.IsNaN(num))
					{
						chartAxis.Interval = (double.IsNaN(num2) ? num : Math.Min(num, num2));
					}
					else if (!double.IsNaN(num2))
					{
						chartAxis.Interval = num2;
					}
					else if (charType != ChartTypes2005.Bar)
					{
						chartAxis.Interval = 1.0;
					}
				}
				else
				{
					chartAxis.Interval = num;
				}
			}
			return chartAxis;
		}

		private double ConvertToDouble(string value)
		{
			double result = double.NaN;
			if (double.TryParse(value, out result))
			{
				return result;
			}
			return double.NaN;
		}

		private void SetChartDataPointNames(DataPoint2005 dataPoint, string[] names)
		{
			int num = Math.Min(dataPoint.DataValues.Count, names.Length);
			for (int i = 0; i < num; i++)
			{
				dataPoint.DataValues[i].Name = names[i];
			}
		}

		private void SetChartTypes(Chart2005 oldChart, PlotTypes2005 plotType, ChartSeries newSeries)
		{
			if (plotType == PlotTypes2005.Line && oldChart.Type != ChartTypes2005.Line)
			{
				newSeries.Type = ChartTypes.Line;
				newSeries.Subtype = ChartSubtypes.Plain;
			}
			else
			{
				switch (oldChart.Type)
				{
				case (ChartTypes2005)3:
					break;
				case ChartTypes2005.Column:
				case ChartTypes2005.Bar:
				case ChartTypes2005.Line:
				case ChartTypes2005.Area:
					newSeries.Type = new ReportExpression<ChartTypes>(oldChart.Type.ToString(), CultureInfo.InvariantCulture);
					newSeries.Subtype = new ReportExpression<ChartSubtypes>(oldChart.Subtype.ToString(), CultureInfo.InvariantCulture);
					break;
				case ChartTypes2005.Pie:
					newSeries.Type = ChartTypes.Shape;
					if (oldChart.Subtype == ChartSubtypes2005.OpenHighLowClose)
					{
						newSeries.Subtype = ChartSubtypes.ExplodedPie;
					}
					else
					{
						newSeries.Subtype = ChartSubtypes.Pie;
					}
					break;
				case ChartTypes2005.Scatter:
					if (oldChart.Subtype == ChartSubtypes2005.Plain)
					{
						newSeries.Type = ChartTypes.Scatter;
						newSeries.Subtype = ChartSubtypes.Plain;
					}
					else
					{
						newSeries.Type = ChartTypes.Line;
						if (oldChart.Subtype == ChartSubtypes2005.Line)
						{
							newSeries.Subtype = ChartSubtypes.Plain;
						}
						else
						{
							newSeries.Subtype = ChartSubtypes.Smooth;
						}
					}
					break;
				case ChartTypes2005.Bubble:
					newSeries.Type = ChartTypes.Scatter;
					newSeries.Subtype = ChartSubtypes.Bubble;
					break;
				case ChartTypes2005.Doughnut:
					newSeries.Type = ChartTypes.Shape;
					if (oldChart.Subtype == ChartSubtypes2005.OpenHighLowClose)
					{
						newSeries.Subtype = ChartSubtypes.ExplodedDoughnut;
					}
					else
					{
						newSeries.Subtype = ChartSubtypes.Doughnut;
					}
					break;
				case ChartTypes2005.Stock:
					newSeries.Type = ChartTypes.Range;
					if (oldChart.Subtype == ChartSubtypes2005.Candlestick)
					{
						newSeries.Subtype = ChartSubtypes.Candlestick;
					}
					else
					{
						newSeries.Subtype = ChartSubtypes.Stock;
					}
					break;
				}
			}
		}

		private void FixYukonChartBorderWidth(AspNetCore.ReportingServices.RdlObjectModel.Style style, bool roundValue)
		{
			if (style != null)
			{
				if (style.Border == null)
				{
					if (style is EmptyColorStyle)
					{
						style.Border = new EmptyBorder();
					}
					else
					{
						style.Border = new Border();
					}
				}
				if (!style.Border.Width.IsExpression)
				{
					double value = roundValue ? Math.Max(Math.Round(style.Border.Width.Value.Value) * 0.75, 0.25) : Math.Max(style.Border.Width.Value.Value * 0.75, 0.376);
					style.Border.Width = new ReportSize(value, SizeTypes.Point);
				}
			}
		}

		private AspNetCore.ReportingServices.RdlObjectModel.Style FixYukonEmptyBorderStyle(Style2005 style2005)
		{
			AspNetCore.ReportingServices.RdlObjectModel.Style style2006 = style2005;
			if (style2006 == null)
			{
				style2006 = new AspNetCore.ReportingServices.RdlObjectModel.Style();
			}
			if (style2006.Border == null)
			{
				style2006.Border = new Border();
			}
			if (style2006.Border.Style == BorderStyles.Default)
			{
				style2006.Border.Style = BorderStyles.None;
			}
			return style2006;
		}

		internal void UpgradeTextbox(Textbox2005 textbox)
		{
			this.UpgradeReportItem(textbox);
			textbox.KeepTogether = true;
			RdlCollection<Paragraph> rdlCollection = new RdlCollection<Paragraph>();
			Paragraph paragraph = new Paragraph();
			rdlCollection.Add(paragraph);
			textbox.Paragraphs = rdlCollection;
			RdlCollection<TextRun> rdlCollection2 = new RdlCollection<TextRun>();
			TextRun textRun = new TextRun();
			rdlCollection2.Add(textRun);
			paragraph.TextRuns = rdlCollection2;
			textRun.Value = textbox.Value;
			textRun.PropertyStore.SetObject(3, textbox.ValueLocID);
			AspNetCore.ReportingServices.RdlObjectModel.Style style = textbox.Style;
			if (style != null)
			{
				AspNetCore.ReportingServices.RdlObjectModel.Style style2 = this.CreateAndMoveStyleProperties(style, UpgradeImpl2005.m_TextRunAvailableStyles);
				if (style2 != null)
				{
					textRun.Style = style2;
				}
				style2 = this.CreateAndMoveStyleProperties(style, UpgradeImpl2005.m_ParagraphAvailableStyles, true, textbox.Name);
				if (style2 != null)
				{
					paragraph.Style = style2;
				}
			}
			textbox.Value = null;
			textbox.ValueLocID = null;
		}

		private AspNetCore.ReportingServices.RdlObjectModel.Style CreateAndMoveStyleProperties(AspNetCore.ReportingServices.RdlObjectModel.Style srcStyle, AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties[] availableStyles)
		{
			return this.CreateAndMoveStyleProperties(srcStyle, availableStyles, false, null);
		}

		private AspNetCore.ReportingServices.RdlObjectModel.Style CreateAndMoveStyleProperties(AspNetCore.ReportingServices.RdlObjectModel.Style srcStyle, AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties[] availableStyles, bool convertMeDotValue, string textboxName)
		{
			AspNetCore.ReportingServices.RdlObjectModel.Style style = null;
			for (int i = 0; i < availableStyles.Length; i++)
			{
				int propertyIndex = (int)availableStyles[i];
				if (srcStyle.PropertyStore.ContainsObject(propertyIndex))
				{
					if (style == null)
					{
						style = new AspNetCore.ReportingServices.RdlObjectModel.Style();
					}
					IExpression expression = (IExpression)srcStyle.PropertyStore.GetObject(propertyIndex);
					if (convertMeDotValue && expression.IsExpression)
					{
						expression.Expression = this.ConvertMeDotValue(expression.ToString(), textboxName);
					}
					style.PropertyStore.SetObject(propertyIndex, expression);
					switch (availableStyles[i])
					{
					case AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties.FontFamily:
						srcStyle.FontFamily = "Arial";
						break;
					case AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties.FontSize:
						srcStyle.FontSize = AspNetCore.ReportingServices.RdlObjectModel.Constants.DefaultFontSize;
						break;
					case AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties.Color:
						srcStyle.Color = AspNetCore.ReportingServices.RdlObjectModel.Constants.DefaultColor;
						break;
					case AspNetCore.ReportingServices.RdlObjectModel.Style.Definition.Properties.NumeralVariant:
						srcStyle.NumeralVariant = 1;
						break;
					default:
						srcStyle.PropertyStore.RemoveObject(propertyIndex);
						break;
					}
				}
			}
			return style;
		}

		private string ConvertMeDotValue(string expression, string textboxName)
		{
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			MatchCollection matchCollection = this.m_regexes.MeDotValueExpression.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				System.Text.RegularExpressions.Group group = matchCollection[i].Groups["medotvalue"];
				if (group.Value != null && group.Value.Length > 0)
				{
					stringBuilder.Append(expression.Substring(num, group.Index - num));
					stringBuilder.Append("ReportItems!");
					stringBuilder.Append(textboxName);
					stringBuilder.Append(".Value");
					num = group.Index + group.Length;
				}
			}
			if (num == 0)
			{
				return expression;
			}
			if (num < expression.Length)
			{
				stringBuilder.Append(expression.Substring(num));
			}
			return stringBuilder.ToString();
		}

		internal void UpgradeQuery(Query2005 query)
		{
			query.DataSourceName = this.GetDataSourceName(query.DataSourceName);
		}

		internal void UpgradeDataSource(DataSource2005 dataSource)
		{
			if (this.m_renameInvalidDataSources)
			{
				dataSource.Name = this.CreateUniqueDataSourceName(dataSource.Name);
			}
		}

		internal void UpgradeSubreport(Subreport2005 subreport)
		{
			this.UpgradeReportItem(subreport);
			subreport.KeepTogether = true;
		}

		internal void UpgradeStyle(Style2005 style2005)
		{
			((AspNetCore.ReportingServices.RdlObjectModel.Style)style2005).FontWeight = this.UpgradeStyleEnum<FontWeights, FontWeight2005>(style2005.FontWeight);
			if (!style2005.FontWeight.IsExpression)
			{
				switch (style2005.FontWeight.Value.Value)
				{
				case FontWeight2005.Normal:
					((AspNetCore.ReportingServices.RdlObjectModel.Style)style2005).FontWeight = FontWeights.Normal;
					break;
				case FontWeight2005.Lighter:
					((AspNetCore.ReportingServices.RdlObjectModel.Style)style2005).FontWeight = FontWeights.Light;
					break;
				case FontWeight2005.Bold:
					((AspNetCore.ReportingServices.RdlObjectModel.Style)style2005).FontWeight = FontWeights.Bold;
					break;
				case FontWeight2005.Bolder:
					((AspNetCore.ReportingServices.RdlObjectModel.Style)style2005).FontWeight = FontWeights.Bold;
					break;
				}
			}
			((AspNetCore.ReportingServices.RdlObjectModel.Style)style2005).WritingMode = this.UpgradeStyleEnum<WritingModes, WritingMode2005>(style2005.WritingMode);
			((AspNetCore.ReportingServices.RdlObjectModel.Style)style2005).Calendar = this.UpgradeStyleEnum<Calendars, Calendar2005>(style2005.Calendar);
			((AspNetCore.ReportingServices.RdlObjectModel.Style)style2005).UnicodeBiDi = this.UpgradeStyleEnum<UnicodeBiDiTypes, UnicodeBiDi2005>(style2005.UnicodeBiDi);
			Border border = null;
			Border border2 = null;
			Border border3 = null;
			Border border4 = null;
			Border border5 = null;
			BorderColor2005 borderColor = style2005.BorderColor;
			if (borderColor != null)
			{
				this.SetBorderColor(borderColor.Default, ref border, AspNetCore.ReportingServices.RdlObjectModel.Constants.DefaultBorderColor);
				this.SetBorderColor(borderColor.Top, ref border2, ReportColor.Empty);
				this.SetBorderColor(borderColor.Bottom, ref border3, ReportColor.Empty);
				this.SetBorderColor(borderColor.Left, ref border4, ReportColor.Empty);
				this.SetBorderColor(borderColor.Right, ref border5, ReportColor.Empty);
			}
			BorderStyle2005 borderStyle = style2005.BorderStyle;
			if (borderStyle != null)
			{
				this.SetBorderStyle(borderStyle.Default, ref border, BorderStyles2005.None);
				this.SetBorderStyle(borderStyle.Top, ref border2, BorderStyles2005.Default);
				this.SetBorderStyle(borderStyle.Bottom, ref border3, BorderStyles2005.Default);
				this.SetBorderStyle(borderStyle.Left, ref border4, BorderStyles2005.Default);
				this.SetBorderStyle(borderStyle.Right, ref border5, BorderStyles2005.Default);
			}
			BorderWidth2005 borderWidth = style2005.BorderWidth;
			if (borderWidth != null)
			{
				this.SetBorderWidth(borderWidth.Default, ref border, AspNetCore.ReportingServices.RdlObjectModel.Constants.DefaultBorderWidth);
				this.SetBorderWidth(borderWidth.Top, ref border2, ReportSize.Empty);
				this.SetBorderWidth(borderWidth.Bottom, ref border3, ReportSize.Empty);
				this.SetBorderWidth(borderWidth.Left, ref border4, ReportSize.Empty);
				this.SetBorderWidth(borderWidth.Right, ref border5, ReportSize.Empty);
			}
			BackgroundImage2005 backgroundImage = style2005.BackgroundImage;
			if (backgroundImage != null)
			{
				BackgroundImage backgroundImage3 = ((AspNetCore.ReportingServices.RdlObjectModel.Style)style2005).BackgroundImage = backgroundImage;
				style2005.BackgroundImage = null;
				if (!backgroundImage.BackgroundRepeat.IsExpression)
				{
					if (style2005.Parent is Chart2005)
					{
						if (backgroundImage.BackgroundRepeat.Value != BackgroundRepeatTypes2005.NoRepeat)
						{
							backgroundImage3.BackgroundRepeat = BackgroundRepeatTypes.Repeat;
						}
					}
					else if (backgroundImage.BackgroundRepeat.Value == BackgroundRepeatTypes2005.NoRepeat)
					{
						backgroundImage3.BackgroundRepeat = BackgroundRepeatTypes.Clip;
					}
					else
					{
						backgroundImage3.BackgroundRepeat = (BackgroundRepeatTypes)backgroundImage.BackgroundRepeat.Value;
					}
				}
				else
				{
					backgroundImage3.BackgroundRepeat = new ReportExpression<BackgroundRepeatTypes>(backgroundImage.BackgroundRepeat.Expression, CultureInfo.InvariantCulture);
				}
			}
			if (border != null)
			{
				style2005.Border = border;
			}
			if (border2 != null)
			{
				style2005.TopBorder = border2;
			}
			if (border3 != null)
			{
				style2005.BottomBorder = border3;
			}
			if (border4 != null)
			{
				style2005.LeftBorder = border4;
			}
			if (border5 != null)
			{
				style2005.RightBorder = border5;
			}
		}

		internal void UpgradeEmptyColorStyle(EmptyColorStyle2005 emptyColorStyle2005)
		{
			if (emptyColorStyle2005.BorderColor == null)
			{
				emptyColorStyle2005.BorderColor = new EmptyBorderColor2005();
			}
			if (emptyColorStyle2005.BorderStyle == null)
			{
				emptyColorStyle2005.BorderStyle = new BorderStyle2005();
			}
			((AspNetCore.ReportingServices.RdlObjectModel.Style)emptyColorStyle2005).FontWeight = this.UpgradeStyleEnum<FontWeights, FontWeight2005>(emptyColorStyle2005.FontWeight);
			if (!emptyColorStyle2005.FontWeight.IsExpression)
			{
				switch (emptyColorStyle2005.FontWeight.Value.Value)
				{
				case FontWeight2005.Normal:
					((AspNetCore.ReportingServices.RdlObjectModel.Style)emptyColorStyle2005).FontWeight = FontWeights.Normal;
					break;
				case FontWeight2005.Lighter:
					((AspNetCore.ReportingServices.RdlObjectModel.Style)emptyColorStyle2005).FontWeight = FontWeights.Light;
					break;
				case FontWeight2005.Bold:
					((AspNetCore.ReportingServices.RdlObjectModel.Style)emptyColorStyle2005).FontWeight = FontWeights.Bold;
					break;
				case FontWeight2005.Bolder:
					((AspNetCore.ReportingServices.RdlObjectModel.Style)emptyColorStyle2005).FontWeight = FontWeights.Bold;
					break;
				}
			}
			((AspNetCore.ReportingServices.RdlObjectModel.Style)emptyColorStyle2005).WritingMode = this.UpgradeStyleEnum<WritingModes, WritingMode2005>(emptyColorStyle2005.WritingMode);
			((AspNetCore.ReportingServices.RdlObjectModel.Style)emptyColorStyle2005).Calendar = this.UpgradeStyleEnum<Calendars, Calendar2005>(emptyColorStyle2005.Calendar);
			((AspNetCore.ReportingServices.RdlObjectModel.Style)emptyColorStyle2005).UnicodeBiDi = this.UpgradeStyleEnum<UnicodeBiDiTypes, UnicodeBiDi2005>(emptyColorStyle2005.UnicodeBiDi);
			EmptyBorder border = null;
			EmptyBorder topBorder = null;
			EmptyBorder bottomBorder = null;
			EmptyBorder leftBorder = null;
			EmptyBorder rightBorder = null;
			EmptyBorderColor2005 borderColor = emptyColorStyle2005.BorderColor;
			if (borderColor != null)
			{
				this.SetEmptyBorderColor(borderColor.Default, ref border, AspNetCore.ReportingServices.RdlObjectModel.Constants.DefaultEmptyColor);
				this.SetEmptyBorderColor(borderColor.Top, ref topBorder, ReportColor.Empty);
				this.SetEmptyBorderColor(borderColor.Bottom, ref bottomBorder, ReportColor.Empty);
				this.SetEmptyBorderColor(borderColor.Left, ref leftBorder, ReportColor.Empty);
				this.SetEmptyBorderColor(borderColor.Right, ref rightBorder, ReportColor.Empty);
			}
			BorderStyle2005 borderStyle = emptyColorStyle2005.BorderStyle;
			if (borderStyle != null)
			{
				this.SetEmptyBorderStyle(borderStyle.Default, ref border, BorderStyles2005.Solid);
				this.SetEmptyBorderStyle(borderStyle.Top, ref topBorder, BorderStyles2005.Default);
				this.SetEmptyBorderStyle(borderStyle.Bottom, ref bottomBorder, BorderStyles2005.Default);
				this.SetEmptyBorderStyle(borderStyle.Left, ref leftBorder, BorderStyles2005.Default);
				this.SetEmptyBorderStyle(borderStyle.Right, ref rightBorder, BorderStyles2005.Default);
			}
			BorderWidth2005 borderWidth = emptyColorStyle2005.BorderWidth;
			if (borderWidth != null)
			{
				this.SetEmptyBorderWidth(borderWidth.Default, ref border, AspNetCore.ReportingServices.RdlObjectModel.Constants.DefaultBorderWidth);
				this.SetEmptyBorderWidth(borderWidth.Top, ref topBorder, ReportSize.Empty);
				this.SetEmptyBorderWidth(borderWidth.Bottom, ref bottomBorder, ReportSize.Empty);
				this.SetEmptyBorderWidth(borderWidth.Left, ref leftBorder, ReportSize.Empty);
				this.SetEmptyBorderWidth(borderWidth.Right, ref rightBorder, ReportSize.Empty);
			}
			emptyColorStyle2005.Border = border;
			emptyColorStyle2005.TopBorder = topBorder;
			emptyColorStyle2005.BottomBorder = bottomBorder;
			emptyColorStyle2005.LeftBorder = leftBorder;
			emptyColorStyle2005.RightBorder = rightBorder;
		}

		private ReportExpression<T> UpgradeStyleEnum<T, T2005>(ReportExpression<ReportEnum<T2005>> value2005) where T : struct where T2005 : struct, IConvertible
		{
			ReportExpression<T> result = default(ReportExpression<T>);
			if (value2005.IsExpression)
			{
				result.Expression = value2005.Expression;
			}
			else
			{
				result.Value = (T)Enum.ToObject(typeof(T), value2005.Value.Value.ToInt32(null));
			}
			return result;
		}

		private ReportExpression<T> UpgradeStyleEnum<T, T2005>(ReportExpression<T2005> value2005) where T : struct where T2005 : struct, IConvertible
		{
			ReportExpression<T> result = default(ReportExpression<T>);
			if (value2005.IsExpression)
			{
				result.Expression = value2005.Expression;
			}
			else
			{
				result.Value = (T)Enum.ToObject(typeof(T), value2005.Value.ToInt32(null));
			}
			return result;
		}

		private void SetBorderColor(ReportExpression<ReportColor> color, ref Border border, ReportColor defaultColor)
		{
			if (color != defaultColor)
			{
				if (border == null)
				{
					border = new Border();
				}
				border.Color = color;
			}
		}

		private void SetEmptyBorderColor(ReportExpression<ReportColor> color, ref EmptyBorder border, ReportColor defaultColor)
		{
			if (color != defaultColor)
			{
				if (border == null)
				{
					border = new EmptyBorder();
				}
				border.Color = color;
			}
		}

		private void SetBorderStyle(ReportExpression<BorderStyles2005> style, ref Border border, BorderStyles2005 defaultStyle)
		{
			if (style != defaultStyle)
			{
				if (border == null)
				{
					border = new Border();
				}
				border.Style = this.UpgradeStyleEnum<BorderStyles, BorderStyles2005>(style);
				if (!border.Style.IsExpression)
				{
					switch (style.Value)
					{
					case BorderStyles2005.Groove:
					case BorderStyles2005.Ridge:
					case BorderStyles2005.Inset:
					case BorderStyles2005.WindowInset:
					case BorderStyles2005.Outset:
						border.Style = BorderStyles.Solid;
						break;
					}
				}
			}
		}

		private void SetEmptyBorderStyle(ReportExpression<BorderStyles2005> style, ref EmptyBorder border, BorderStyles2005 defaultStyle)
		{
			if (style != defaultStyle)
			{
				if (border == null)
				{
					border = new EmptyBorder();
				}
				border.Style = this.UpgradeStyleEnum<BorderStyles, BorderStyles2005>(style);
				if (!border.Style.IsExpression)
				{
					switch (style.Value)
					{
					case BorderStyles2005.Groove:
					case BorderStyles2005.Ridge:
					case BorderStyles2005.Inset:
					case BorderStyles2005.WindowInset:
					case BorderStyles2005.Outset:
						border.Style = BorderStyles.Solid;
						break;
					}
				}
			}
		}

		private void SetBorderWidth(ReportExpression<ReportSize> width, ref Border border, ReportSize defaultWidth)
		{
			if (width != defaultWidth)
			{
				if (border == null)
				{
					border = new Border();
				}
				border.Width = width;
			}
		}

		private void SetEmptyBorderWidth(ReportExpression<ReportSize> width, ref EmptyBorder border, ReportSize defaultWidth)
		{
			if (width != defaultWidth)
			{
				if (border == null)
				{
					border = new EmptyBorder();
				}
				border.Width = width;
			}
		}

		private void TransferGroupingCustomProperties(object member, GroupAccessor groupAccessor, CustomPropertiesAccessor propertiesAccessor)
		{
			Grouping2005 grouping = groupAccessor(member) as Grouping2005;
			if (grouping != null)
			{
				IList<CustomProperty> list = propertiesAccessor(member);
				foreach (CustomProperty customProperty in grouping.CustomProperties)
				{
					list.Add(customProperty);
				}
			}
		}

		private string UniqueName(string baseName, object obj)
		{
			return this.UniqueName(baseName, obj, true);
		}

		private string UniqueName(string baseName, object obj, bool allowBaseName)
		{
			string text = baseName;
			int num = (!allowBaseName) ? 1 : 0;
			while (true)
			{
				if (num > 0)
				{
					text = baseName + num;
				}
				if (!this.m_nameTable.ContainsKey(text))
				{
					break;
				}
				num++;
			}
			this.m_nameTable.Add(text, obj);
			return text;
		}

		private string CreateUniqueDataSourceName(string oldName)
		{
			string text = oldName;
			Match match = ReportRegularExpressions.Value.ClsIdentifierRegex.Match(oldName);
			if (!match.Success)
			{
				text = Regex.Replace(oldName, "[^\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Mn}\\p{Mc}\\p{Nd}\\p{Pc}\\p{Cf}]", "_");
				match = ReportRegularExpressions.Value.ClsIdentifierRegex.Match(text);
				if (!match.Success)
				{
					text = "AutoGen_" + text;
				}
			}
			string text2 = text.ToUpperInvariant();
			string text3 = text;
			string key = text2;
			int num = 0;
			while (true)
			{
				if (num > 0)
				{
					text3 = text + num;
					key = text2 + num;
				}
				if (!this.m_dataSourceNameTable.ContainsKey(key))
				{
					break;
				}
				num++;
			}
			this.m_dataSourceNameTable.Add(key, text3);
			if (!this.m_dataSourceCaseSensitiveNameTable.ContainsKey(oldName))
			{
				this.m_dataSourceCaseSensitiveNameTable.Add(oldName, text3);
			}
			return text3;
		}

		private string GetDataSourceName(string dataSourceName)
		{
			if (this.m_dataSourceCaseSensitiveNameTable.ContainsKey(dataSourceName))
			{
				return (string)this.m_dataSourceCaseSensitiveNameTable[dataSourceName];
			}
			string key = dataSourceName.ToUpperInvariant();
			if (this.m_dataSourceNameTable.ContainsKey(key))
			{
				return (string)this.m_dataSourceNameTable[key];
			}
			return dataSourceName;
		}

		private string GetParentReportItemName(IContainedObject obj)
		{
			IContainedObject containedObject = null;
			for (containedObject = obj.Parent; containedObject != null; containedObject = containedObject.Parent)
			{
				if (containedObject is AspNetCore.ReportingServices.RdlObjectModel.ReportItem)
				{
					return ((AspNetCore.ReportingServices.RdlObjectModel.ReportItem)containedObject).Name;
				}
			}
			return "";
		}

		private void UpgradeDundasCRIChart(AspNetCore.ReportingServices.RdlObjectModel.CustomReportItem cri, AspNetCore.ReportingServices.RdlObjectModel.Chart chart)
		{
			OrderedDictionary orderedDictionary = new OrderedDictionary();
			OrderedDictionary orderedDictionary2 = new OrderedDictionary();
			chart.Name = cri.Name;
			chart.ActionInfo = cri.ActionInfo;
			chart.Bookmark = cri.Bookmark;
			chart.DataElementName = cri.DataElementName;
			chart.DataElementOutput = cri.DataElementOutput;
			chart.DocumentMapLabel = cri.DocumentMapLabel;
			chart.PropertyStore.SetObject(12, cri.PropertyStore.GetObject(12));
			chart.Height = cri.Height;
			chart.Left = cri.Left;
			chart.Parent = cri.Parent;
			chart.RepeatWith = cri.RepeatWith;
			chart.ToolTip = cri.ToolTip;
			chart.PropertyStore.SetObject(10, cri.PropertyStore.GetObject(10));
			chart.Top = cri.Top;
			chart.Visibility = cri.Visibility;
			chart.Width = cri.Width;
			chart.ZIndex = cri.ZIndex;
			if (cri.CustomData != null)
			{
				chart.DataSetName = cri.CustomData.DataSetName;
				chart.Filters = cri.CustomData.Filters;
			}
			Hashtable hashtable = new Hashtable();
			List<Hashtable> list = new List<Hashtable>();
			List<Hashtable> list2 = new List<Hashtable>();
			List<Hashtable> list3 = new List<Hashtable>();
			ReportExpression reportExpression;
			foreach (CustomProperty customProperty in cri.CustomProperties)
			{
				reportExpression = customProperty.Name;
				string text = reportExpression.Value;
				if (text.StartsWith("expression:", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Substring("expression:".Length);
				}
				if (!this.AddToPropertyList(list, "Chart.Titles.", text, customProperty.Value) && !this.AddToPropertyList(list2, "Chart.Legends.", text, customProperty.Value) && !this.AddToPropertyList(list3, "Chart.ChartAreas.", text, customProperty.Value))
				{
					hashtable.Add(text, customProperty.Value);
				}
				if (text.StartsWith("CHART.ANNOTATIONS.", StringComparison.OrdinalIgnoreCase))
				{
					base.UpgradeResults.HasUnsupportedDundasChartFeatures = true;
				}
			}
			if (hashtable["CUSTOM_CODE_CS"] != null || hashtable["CUSTOM_CODE_VB"] != null || hashtable["CUSTOM_CODE_COMPILED_ASSEMBLY"] != null)
			{
				if (this.m_throwUpgradeException)
				{
					throw new CRI2005UpgradeException();
				}
				base.UpgradeResults.HasUnsupportedDundasChartFeatures = true;
			}
			StringCollection stringCollection = new StringCollection();
			List<Hashtable>.Enumerator enumerator2 = list3.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					Hashtable current2 = enumerator2.Current;
					ChartArea chartArea = new ChartArea();
					chart.ChartAreas.Add(chartArea);
					string text2 = this.ConvertDundasCRIStringProperty(current2["ChartArea.Name"]);
					string text3 = this.CreateNewName(stringCollection, text2, "ChartArea");
					stringCollection.Add(text3);
					if (!orderedDictionary.Contains(text2))
					{
						orderedDictionary.Add(text2, text3);
					}
					chartArea.Name = text3;
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
			int num = 0;
			enumerator2 = list3.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					Hashtable current3 = enumerator2.Current;
					ChartArea chartArea2 = chart.ChartAreas[num];
					num++;
					chartArea2.AlignWithChartArea = this.GetNewName(orderedDictionary, this.ConvertDundasCRIStringProperty(current3["ChartArea.AlignWithChartArea"]));
					chartArea2.AlignOrientation = new ReportExpression<ChartAlignOrientations>(this.ConvertDundasCRIStringProperty(ChartAlignOrientations.Vertical.ToString(), current3["ChartArea.AlignOrientation"]), CultureInfo.InvariantCulture);
					bool? nullable = this.ConvertDundasCRIBoolProperty(current3["EquallySizedAxesFont"]);
					if (nullable.HasValue)
					{
						chartArea2.EquallySizedAxesFont = nullable.Value;
					}
					bool? nullable2 = this.ConvertDundasCRIBoolProperty(current3["ChartArea.Visible"]);
					if (nullable2.HasValue)
					{
						chartArea2.Hidden = !nullable2.Value;
					}
					string text4 = this.ConvertDundasCRIStringProperty(current3["ChartArea.AlignType"]);
					if (text4 != string.Empty)
					{
						ChartAlignType chartAlignType = new ChartAlignType();
						text4 = " " + text4.Replace(',', ' ') + " ";
						chartAlignType.AxesView = text4.Contains("AxesView");
						chartAlignType.Cursor = text4.Contains("Cursor");
						chartAlignType.InnerPlotPosition = text4.Contains("PlotPosition");
						chartAlignType.Position = text4.Contains("Position");
					}
					chartArea2.Style = this.ConvertDundasCRIStyleProperty(null, current3["ChartArea.BackColor"], current3["ChartArea.BackGradientType"], current3["ChartArea.BackGradientEndColor"], current3["ChartArea.BackHatchStyle"], current3["ChartArea.ShadowColor"], current3["ChartArea.ShadowOffset"], current3["ChartArea.BorderColor"], current3["ChartArea.BorderStyle"], current3["ChartArea.BorderWidth"], current3["ChartArea.BackImage"], current3["ChartArea.BackImageTranspColor"], current3["ChartArea.BackImageAlign"], current3["ChartArea.BackImageMode"], null, null, null, null, null);
					chartArea2.ChartElementPosition = this.ConvertDundasCRIChartElementPosition(current3["ChartArea.Position.Y"], current3["ChartArea.Position.X"], current3["ChartArea.Position.Height"], current3["ChartArea.Position.Width"]);
					chartArea2.ChartInnerPlotPosition = this.ConvertDundasCRIChartElementPosition(current3["ChartArea.InnerPlotPosition.Y"], current3["ChartArea.InnerPlotPosition.X"], current3["ChartArea.InnerPlotPosition.Height"], current3["ChartArea.InnerPlotPosition.Width"]);
					int num2 = 0;
					ChartThreeDProperties chartThreeDProperties = new ChartThreeDProperties();
					chartThreeDProperties.Perspective = this.ConvertDundasCRIIntegerReportExpressionProperty(current3["ChartArea.Area3DStyle.Perspective"], ref num2);
					chartThreeDProperties.Rotation = this.ConvertDundasCRIIntegerReportExpressionProperty(chartThreeDProperties.Rotation, current3["ChartArea.Area3DStyle.YAngle"], ref num2);
					chartThreeDProperties.Inclination = this.ConvertDundasCRIIntegerReportExpressionProperty(chartThreeDProperties.Inclination, current3["ChartArea.Area3DStyle.XAngle"], ref num2);
					chartThreeDProperties.DepthRatio = this.ConvertDundasCRIIntegerReportExpressionProperty(chartThreeDProperties.DepthRatio, current3["ChartArea.Area3DStyle.PointDepth"], ref num2);
					chartThreeDProperties.GapDepth = this.ConvertDundasCRIIntegerReportExpressionProperty(chartThreeDProperties.GapDepth, current3["ChartArea.Area3DStyle.PointGapDepth"], ref num2);
					chartThreeDProperties.WallThickness = this.ConvertDundasCRIIntegerReportExpressionProperty(chartThreeDProperties.WallThickness, current3["ChartArea.Area3DStyle.WallWidth"], ref num2);
					bool? nullable3 = this.ConvertDundasCRIBoolProperty(current3["ChartArea.Area3DStyle.Clustered"], ref num2);
					if (nullable3.HasValue)
					{
						chartThreeDProperties.Clustered = !nullable3.Value;
					}
					else
					{
						chartThreeDProperties.Clustered = true;
					}
					bool? nullable4 = this.ConvertDundasCRIBoolProperty(current3["ChartArea.Area3DStyle.Enable3D"], ref num2);
					if (nullable4.HasValue)
					{
						chartThreeDProperties.Enabled = nullable4.Value;
					}
					bool? nullable5 = this.ConvertDundasCRIBoolProperty(current3["ChartArea.Area3DStyle.RightAngleAxes"], ref num2);
					if (!nullable5.HasValue || nullable5.Value)
					{
						chartThreeDProperties.ProjectionMode = ChartProjectionModes.Oblique;
					}
					else
					{
						chartThreeDProperties.ProjectionMode = ChartProjectionModes.Perspective;
					}
					string a = this.ConvertDundasCRIStringProperty(current3["ChartArea.Area3DStyle.Light"], ref num2);
					if (a == "None")
					{
						chartThreeDProperties.Shading = ChartShadings.None;
					}
					else if (a == "Realistic")
					{
						chartThreeDProperties.Shading = ChartShadings.Real;
					}
					else
					{
						chartThreeDProperties.Shading = ChartShadings.Simple;
					}
					if (num2 > 0)
					{
						chartArea2.ChartThreeDProperties = chartThreeDProperties;
					}
					ChartAxis chartAxis = new ChartAxis();
					ChartAxis chartAxis2 = new ChartAxis();
					ChartAxis chartAxis3 = chartAxis;
					ChartAxis chartAxis4 = chartAxis2;
					string text7 = chartAxis3.Name = (chartAxis4.Name = "Primary");
					ChartAxis chartAxis5 = chartAxis;
					ChartAxis chartAxis6 = chartAxis2;
					ReportExpression<ChartAxisLocations> reportExpression4 = chartAxis5.Location = (chartAxis6.Location = ChartAxisLocations.Default);
					ChartAxis chartAxis7 = new ChartAxis();
					ChartAxis chartAxis8 = new ChartAxis();
					ChartAxis chartAxis9 = chartAxis7;
					ChartAxis chartAxis10 = chartAxis8;
					text7 = (chartAxis9.Name = (chartAxis10.Name = "Secondary"));
					ChartAxis chartAxis11 = chartAxis7;
					ChartAxis chartAxis12 = chartAxis8;
					reportExpression4 = (chartAxis11.Location = (chartAxis12.Location = ChartAxisLocations.Opposite));
					chartArea2.ChartCategoryAxes.Add(chartAxis);
					chartArea2.ChartCategoryAxes.Add(chartAxis7);
					chartArea2.ChartValueAxes.Add(chartAxis2);
					chartArea2.ChartValueAxes.Add(chartAxis8);
					this.UpgradeDundasCRIChartAxis(chartAxis, current3, "ChartArea.AxisX.");
					this.UpgradeDundasCRIChartAxis(chartAxis7, current3, "ChartArea.AxisX2.");
					this.UpgradeDundasCRIChartAxis(chartAxis2, current3, "ChartArea.AxisY.");
					this.UpgradeDundasCRIChartAxis(chartAxis8, current3, "ChartArea.AxisY2.");
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
			chart.ToolTip = this.ConvertDundasCRIStringProperty(hashtable["Chart.ToolTip"]);
			Color color = Color.White;
			chart.Style = this.ConvertDundasCRIStyleProperty(null, this.ConvertDundasCRIColorProperty(color.Name, hashtable["Chart.BackColor"]), hashtable["Chart.BackGradientType"], hashtable["Chart.BackGradientEndColor"], hashtable["Chart.BackHatchStyle"], null, null, hashtable["Chart.BorderLineColor"], hashtable["Chart.BorderLineStyle"] ?? ((object)BorderStyles.None), hashtable["Chart.BorderLineWidth"], hashtable["Chart.BackImage"], null, null, null, null, null, null, null, null);
			if (cri.Style != null && cri.Style.Language != (string)null)
			{
				if (chart.Style == null)
				{
					chart.Style = new AspNetCore.ReportingServices.RdlObjectModel.Style();
				}
				chart.Style.Language = cri.Style.Language;
			}
			string text10 = this.ConvertDundasCRIStringProperty(hashtable["Chart.Palette"]);
			if (text10 == string.Empty || text10 == "Dundas")
			{
				chart.Palette = ChartPalettes.BrightPastel;
			}
			else if (text10 != "None")
			{
				chart.Palette = new ReportExpression<ChartPalettes>(text10, CultureInfo.InvariantCulture);
			}
			string text11 = this.ConvertDundasCRIStringProperty(hashtable["Chart.PaletteCustomColors"]);
			if (text11 != string.Empty)
			{
				string[] array = text11.Split(';');
				string[] array2 = array;
				foreach (string text12 in array2)
				{
					string text13 = text12.Trim();
					IList<ReportExpression<ReportColor>> chartCustomPaletteColors = chart.ChartCustomPaletteColors;
					color = Color.Transparent;
					chartCustomPaletteColors.Add(this.ConvertDundasCRIColorProperty(color.Name, string.IsNullOrEmpty(text13) ? null : text13.Trim()));
				}
				if (array.Length > 0)
				{
					chart.Palette = new ReportExpression<ChartPalettes>(ChartPalettes.Custom);
				}
			}
			int num3 = 0;
			ChartBorderSkin chartBorderSkin = new ChartBorderSkin();
			chartBorderSkin.ChartBorderSkinType = new ReportExpression<ChartBorderSkinTypes>(this.ConvertDundasCRIStringProperty(hashtable["Chart.BorderSkin.SkinStyle"], ref num3), CultureInfo.InvariantCulture);
			ChartBorderSkin chartBorderSkin2 = chartBorderSkin;
			color = Color.White;
			chartBorderSkin2.Style = this.ConvertDundasCRIStyleProperty(this.ConvertDundasCRIColorProperty(color.Name, hashtable["Chart.BorderSkin.PageColor"]), hashtable["Chart.BorderSkin.FrameBackColor"], hashtable["Chart.BorderSkin.FrameBackGradientType"], hashtable["Chart.BorderSkin.FrameBackGradientEndColor"], hashtable["Chart.BorderSkin.FrameBackHatchStyle"], null, null, hashtable["Chart.BorderSkin.FrameBorderColor"], hashtable["Chart.BorderSkin.FrameBorderStyle"], hashtable["Chart.BorderSkin.FrameBorderWidth"], null, null, null, null, null, null, null, null, null, ref num3);
			if (num3 > 0)
			{
				chart.ChartBorderSkin = chartBorderSkin;
			}
			StringCollection stringCollection2 = new StringCollection();
			enumerator2 = list.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					Hashtable current4 = enumerator2.Current;
					AspNetCore.ReportingServices.RdlObjectModel.ChartTitle chartTitle = new AspNetCore.ReportingServices.RdlObjectModel.ChartTitle();
					chart.ChartTitles.Add(chartTitle);
					string text14 = this.CreateNewName(stringCollection2, this.ConvertDundasCRIStringProperty(current4["Title.Name"]), "Title");
					stringCollection2.Add(text14);
					chartTitle.DockToChartArea = this.GetNewName(orderedDictionary, this.ConvertDundasCRIStringProperty(current4["Title.DockToChartArea"]));
					chartTitle.Name = text14;
					this.UpgradeDundasCRIChartTitle(chartTitle, current4, "Title.");
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
			AspNetCore.ReportingServices.RdlObjectModel.ChartTitle chartTitle2 = new AspNetCore.ReportingServices.RdlObjectModel.ChartTitle();
			if (this.UpgradeDundasCRIChartTitle(chartTitle2, hashtable, "Chart.NoDataMessage."))
			{
				chartTitle2.Name = "NoDataMessageTitle";
				chart.ChartNoDataMessage = chartTitle2;
			}
			StringCollection stringCollection3 = new StringCollection();
			enumerator2 = list2.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					Hashtable current5 = enumerator2.Current;
					ChartLegend chartLegend = new ChartLegend();
					chart.ChartLegends.Add(chartLegend);
					string text15 = this.ConvertDundasCRIStringProperty(current5["Legend.Name"]);
					string text16 = this.CreateNewName(stringCollection3, text15, "Legend");
					stringCollection3.Add(text16);
					if (!orderedDictionary2.Contains(text15))
					{
						orderedDictionary2.Add(text15, text16);
					}
					chartLegend.DockToChartArea = this.GetNewName(orderedDictionary, this.ConvertDundasCRIStringProperty(current5["Legend.DockToChartArea"]));
					chartLegend.Name = text16;
					this.UpgradeDundasCRIChartLegend(chartLegend, current5, "Legend.");
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
			if (cri.CustomData != null)
			{
				List<Hashtable> list4 = new List<Hashtable>();
				List<Hashtable> list5 = new List<Hashtable>();
				if (cri.CustomData.DataColumnHierarchy != null)
				{
					IList<DataMember> dataMembers = cri.CustomData.DataColumnHierarchy.DataMembers;
					IList<ChartMember> chartMembers = chart.ChartCategoryHierarchy.ChartMembers;
					while (dataMembers != null && dataMembers.Count > 0)
					{
						foreach (DataMember item in dataMembers)
						{
							ChartMember chartMember = new ChartMember();
							chartMembers.Add(chartMember);
							chartMember.Group = item.Group;
							chartMember.SortExpressions = item.SortExpressions;
							foreach (CustomProperty customProperty2 in item.CustomProperties)
							{
								if (customProperty2.Name == "GroupLabel")
								{
									ChartMember chartMember2 = chartMember;
									reportExpression = customProperty2.Value;
									chartMember2.Label = reportExpression.ToString();
									break;
								}
							}
							dataMembers = item.DataMembers;
							chartMembers = chartMember.ChartMembers;
						}
					}
				}
				if (cri.CustomData.DataRowHierarchy != null)
				{
					IList<DataMember> dataMembers = cri.CustomData.DataRowHierarchy.DataMembers;
					IList<ChartMember> chartMembers = chart.ChartSeriesHierarchy.ChartMembers;
					while (dataMembers != null)
					{
						bool flag = false;
						foreach (DataMember item2 in dataMembers)
						{
							if (item2.DataMembers != null && item2.DataMembers.Count > 0)
							{
								ChartMember chartMember3 = new ChartMember();
								chartMembers.Add(chartMember3);
								chartMember3.Group = item2.Group;
								chartMember3.SortExpressions = item2.SortExpressions;
								foreach (CustomProperty customProperty3 in item2.CustomProperties)
								{
									if (customProperty3.Name == "GroupLabel")
									{
										ChartMember chartMember4 = chartMember3;
										reportExpression = customProperty3.Value;
										chartMember4.Label = reportExpression.ToString();
										break;
									}
								}
								dataMembers = item2.DataMembers;
								chartMembers = chartMember3.ChartMembers;
							}
							else
							{
								flag = true;
								Hashtable hashtable2 = new Hashtable(item2.CustomProperties.Count);
								if (item2.CustomProperties != null)
								{
									foreach (CustomProperty customProperty4 in item2.CustomProperties)
									{
										hashtable2.Add(customProperty4.Name, customProperty4.Value);
									}
									list4.Add(hashtable2);
								}
								ChartMember chartMember5 = new ChartMember();
								chartMember5.Label = this.ConvertDundasCRIStringProperty(hashtable2["SeriesLabel"]);
								chartMembers.Add(chartMember5);
							}
						}
						if (flag)
						{
							dataMembers = null;
						}
					}
					if (cri.CustomData.DataRows != null)
					{
						foreach (IList<IList<AspNetCore.ReportingServices.RdlObjectModel.DataValue>> dataRow in cri.CustomData.DataRows)
						{
							foreach (IList<AspNetCore.ReportingServices.RdlObjectModel.DataValue> item3 in dataRow)
							{
								Hashtable hashtable3 = new Hashtable(item3.Count);
								foreach (AspNetCore.ReportingServices.RdlObjectModel.DataValue item4 in item3)
								{
									reportExpression = item4.Name;
									if (reportExpression.Value.StartsWith("CUSTOMVALUE:", StringComparison.OrdinalIgnoreCase))
									{
										if (this.m_throwUpgradeException)
										{
											throw new CRI2005UpgradeException();
										}
										base.UpgradeResults.HasUnsupportedDundasChartFeatures = true;
									}
									hashtable3.Add(item4.Name, item4.Value);
								}
								list5.Add(hashtable3);
							}
						}
					}
					if (chart.ChartData == null)
					{
						chart.ChartData = new ChartData();
					}
					enumerator2 = list4.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							Hashtable current14 = enumerator2.Current;
							ChartSeries chartSeries = new ChartSeries();
							chart.ChartData.ChartSeriesCollection.Add(chartSeries);
							chartSeries.Name = "Series" + chart.ChartData.ChartSeriesCollection.Count.ToString(CultureInfo.InvariantCulture.NumberFormat);
							chartSeries.ChartAreaName = this.GetNewName(orderedDictionary, this.ConvertDundasCRIStringProperty(current14["ChartArea"]));
							chartSeries.LegendName = this.GetNewName(orderedDictionary2, this.ConvertDundasCRIStringProperty(current14["Legend"]));
							this.UpgradeDundasCRIChartSeries(chartSeries, chart.ChartData.ChartDerivedSeriesCollection, current14, list5);
						}
					}
					finally
					{
						((IDisposable)enumerator2).Dispose();
					}
				}
			}
			else
			{
				chart.ChartCategoryHierarchy.ChartMembers.Add(new ChartMember());
				chart.ChartSeriesHierarchy.ChartMembers.Add(new ChartMember());
				chart.ChartData = new ChartData();
				ChartSeries chartSeries2 = new ChartSeries();
				chartSeries2.Name = "emptySeriesName";
				chartSeries2.ChartDataPoints.Add(new AspNetCore.ReportingServices.RdlObjectModel.ChartDataPoint());
				chart.ChartData.ChartSeriesCollection.Add(chartSeries2);
			}
			this.FixChartAxisStriplineTitleAngle(chart);
		}

		private void UpgradeDundasCRIChartAxis(ChartAxis axis, Hashtable axisProperties, string propertyPrefix)
		{
			axis.Visible = new ReportExpression<ChartVisibleTypes>(this.ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "Enabled"]), CultureInfo.InvariantCulture);
			axis.Interval = this.ConvertDundasCRIDoubleReportExpressionProperty(axisProperties[propertyPrefix + "Interval"]);
			axis.IntervalType = new ReportExpression<ChartIntervalTypes>(this.ConvertDundasCRIStringProperty(ChartIntervalTypes.Auto.ToString(), axisProperties[propertyPrefix + "IntervalType"]), CultureInfo.InvariantCulture);
			axis.IntervalOffset = this.ConvertDundasCRIDoubleReportExpressionProperty(axisProperties[propertyPrefix + "IntervalOffset"]);
			axis.IntervalOffsetType = new ReportExpression<ChartIntervalOffsetTypes>(this.ConvertDundasCRIStringProperty(ChartIntervalOffsetTypes.Auto.ToString(), axisProperties[propertyPrefix + "IntervalOffsetType"]), CultureInfo.InvariantCulture);
			axis.CrossAt = this.ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "Crossing"]);
			axis.Arrows = new ReportExpression<ChartArrowsTypes>(this.ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "Arrows"]), CultureInfo.InvariantCulture);
			axis.Minimum = this.ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "Minimum"]);
			axis.Maximum = this.ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "Maximum"]);
			axis.LogBase = this.ConvertDundasCRIDoubleReportExpressionProperty(axis.LogBase, axisProperties[propertyPrefix + "LogarithmBase"]);
			axis.Angle = this.ConvertDundasCRIDoubleReportExpressionProperty(axisProperties[propertyPrefix + "LabelStyle.FontAngle"]);
			axis.LabelInterval = this.ConvertDundasCRIDoubleReportExpressionProperty(axisProperties[propertyPrefix + "LabelStyle.Interval"]);
			axis.LabelIntervalType = new ReportExpression<ChartIntervalTypes>(this.ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "LabelStyle.IntervalType"]), CultureInfo.InvariantCulture);
			axis.LabelIntervalOffset = this.ConvertDundasCRIDoubleReportExpressionProperty(axisProperties[propertyPrefix + "LabelStyle.IntervalOffset"]);
			axis.LabelIntervalOffsetType = new ReportExpression<ChartIntervalOffsetTypes>(this.ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "LabelStyle.IntervalOffsetType"]), CultureInfo.InvariantCulture);
			axis.MinFontSize = this.ConvertDundasCRIPointReportSizeProperty(axis.MinFontSize, axisProperties[propertyPrefix + "LabelsAutoFitMinFontSize"]);
			axis.MaxFontSize = this.ConvertDundasCRIPointReportSizeProperty(axis.MaxFontSize, axisProperties[propertyPrefix + "LabelsAutoFitMaxFontSize"]);
			axis.InterlacedColor = this.ConvertDundasCRIColorProperty(axis.InterlacedColor, axisProperties[propertyPrefix + "InterlacedColor"]);
			bool? nullable = this.ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "Reverse"]);
			if (nullable.HasValue)
			{
				axis.Reverse = nullable.Value;
			}
			bool? nullable2 = this.ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "Interlaced"]);
			if (nullable2.HasValue)
			{
				axis.Interlaced = nullable2.Value;
			}
			bool? nullable3 = this.ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "Logarithmic"]);
			if (nullable3.HasValue)
			{
				axis.LogScale = nullable3.Value;
			}
			bool? nullable4 = this.ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "LabelStyle.Enabled"]);
			if (nullable4.HasValue)
			{
				axis.HideLabels = !nullable4.Value;
			}
			bool? nullable5 = this.ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "StartFromZero"]);
			if (nullable5.HasValue)
			{
				axis.IncludeZero = nullable5.Value;
			}
			bool? nullable6 = this.ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "LabelsAutoFit"]);
			if (nullable6.HasValue)
			{
				axis.LabelsAutoFitDisabled = !nullable6.Value;
			}
			bool? nullable7 = this.ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "LabelStyle.OffsetLabels"]);
			if (nullable7.HasValue)
			{
				axis.OffsetLabels = nullable7.Value;
			}
			bool? nullable8 = this.ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "LabelStyle.ShowEndLabels"]);
			if (nullable8.HasValue)
			{
				axis.HideEndLabels = !nullable8.Value;
			}
			string text = this.ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "LabelsAutoFitStyle"]);
			axis.PreventFontGrow = (!(text == string.Empty) && !text.Contains("IncreaseFont"));
			axis.PreventFontShrink = (!(text == string.Empty) && !text.Contains("DecreaseFont"));
			axis.PreventLabelOffset = (!(text == string.Empty) && !text.Contains("OffsetLabels"));
			axis.PreventWordWrap = (!(text == string.Empty) && !text.Contains("WordWrap"));
			if (text == string.Empty || text.Contains("LabelsAngleStep30"))
			{
				axis.AllowLabelRotation = ChartLabelRotationTypes.Rotate30;
			}
			else if (text.Contains("LabelsAngleStep45"))
			{
				axis.AllowLabelRotation = ChartLabelRotationTypes.Rotate45;
			}
			else if (text.Contains("LabelsAngleStep90"))
			{
				axis.AllowLabelRotation = ChartLabelRotationTypes.Rotate90;
			}
			bool? nullable9 = this.ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "MarksNextToAxis"]);
			if (nullable9.HasValue)
			{
				axis.MarksAlwaysAtPlotEdge = !nullable9.Value;
			}
			bool? nullable10 = this.ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "Margin"]);
			if (nullable10.HasValue && !nullable10.Value)
			{
				axis.Margin = ChartAxisMarginVisibleTypes.False;
			}
			else
			{
				axis.Margin = ChartAxisMarginVisibleTypes.True;
			}
			axis.Style = this.ConvertDundasCRIStyleProperty(axisProperties[propertyPrefix + "LabelStyle.FontColor"], null, null, null, null, null, null, axisProperties[propertyPrefix + "LineColor"], axisProperties[propertyPrefix + "LineStyle"], axisProperties[propertyPrefix + "LineWidth"], null, null, null, null, this.ConvertDundasCRIStringProperty("Microsoft Sans Serif, 8pt", axisProperties[propertyPrefix + "LabelStyle.Font"]), axisProperties[propertyPrefix + "LabelStyle.Format"], null, null, null);
			int num = 0;
			ChartAxisTitle chartAxisTitle = new ChartAxisTitle();
			chartAxisTitle.Caption = this.ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "Title"], ref num);
			chartAxisTitle.Position = new ReportExpression<ChartAxisTitlePositions>(this.ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "TitleAlignment"], ref num), CultureInfo.InvariantCulture);
			chartAxisTitle.Style = this.ConvertDundasCRIStyleProperty(axisProperties[propertyPrefix + "TitleColor"], null, null, null, null, null, null, null, null, null, null, null, null, null, this.ConvertDundasCRIStringProperty("Microsoft Sans Serif, 8pt", axisProperties[propertyPrefix + "TitleFont"]), null, null, null, null, ref num);
			if (num > 0)
			{
				axis.ChartAxisTitle = chartAxisTitle;
			}
			num = 0;
			ChartAxisScaleBreak chartAxisScaleBreak = new ChartAxisScaleBreak();
			bool? nullable11 = this.ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "ScaleBreakStyle.Enabled"], ref num);
			if (nullable11.HasValue)
			{
				chartAxisScaleBreak.Enabled = nullable11.Value;
			}
			chartAxisScaleBreak.BreakLineType = new ReportExpression<ChartBreakLineTypes>(this.ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "ScaleBreakStyle.BreakLineType"], ref num), CultureInfo.InvariantCulture);
			chartAxisScaleBreak.CollapsibleSpaceThreshold = this.ConvertDundasCRIIntegerReportExpressionProperty(chartAxisScaleBreak.CollapsibleSpaceThreshold, axisProperties[propertyPrefix + "ScaleBreakStyle.CollapsibleSpaceThreshold"], ref num);
			chartAxisScaleBreak.MaxNumberOfBreaks = this.ConvertDundasCRIIntegerReportExpressionProperty(chartAxisScaleBreak.MaxNumberOfBreaks, axisProperties[propertyPrefix + "ScaleBreakStyle.MaxNumberOfBreaks"], ref num);
			chartAxisScaleBreak.Spacing = this.ConvertDundasCRIDoubleReportExpressionProperty(chartAxisScaleBreak.Spacing, axisProperties[propertyPrefix + "ScaleBreakStyle.Spacing"], ref num);
			chartAxisScaleBreak.IncludeZero = new ReportExpression<ChartIncludeZeroTypes>(this.ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "ScaleBreakStyle.StartFromZero"], ref num), CultureInfo.InvariantCulture);
			chartAxisScaleBreak.Style = this.ConvertDundasCRIStyleProperty(null, null, null, null, null, null, null, axisProperties[propertyPrefix + "ScaleBreakStyle.LineColor"], axisProperties[propertyPrefix + "ScaleBreakStyle.LineStyle"], axisProperties[propertyPrefix + "ScaleBreakStyle.LineWidth"], null, null, null, null, null, null, null, null, null, ref num);
			if (num > 0)
			{
				axis.ChartAxisScaleBreak = chartAxisScaleBreak;
			}
			ChartTickMarks chartTickMarks = new ChartTickMarks();
			if (this.UpgradeDundasCRIChartTickMarks(chartTickMarks, axisProperties, propertyPrefix + "MajorTickMark.", true))
			{
				axis.ChartMajorTickMarks = chartTickMarks;
			}
			ChartTickMarks chartTickMarks2 = new ChartTickMarks();
			if (this.UpgradeDundasCRIChartTickMarks(chartTickMarks2, axisProperties, propertyPrefix + "MinorTickMark.", false))
			{
				axis.ChartMinorTickMarks = chartTickMarks2;
			}
			ChartGridLines chartGridLines = new ChartGridLines();
			if (this.UpgradeDundasCRIChartGridLines(chartGridLines, axisProperties, propertyPrefix + "MajorGrid.", true))
			{
				axis.ChartMajorGridLines = chartGridLines;
			}
			ChartGridLines chartGridLines2 = new ChartGridLines();
			if (this.UpgradeDundasCRIChartGridLines(chartGridLines2, axisProperties, propertyPrefix + "MinorGrid.", false))
			{
				axis.ChartMinorGridLines = chartGridLines2;
			}
			List<Hashtable> list = new List<Hashtable>();
			IDictionaryEnumerator enumerator = axisProperties.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
					this.AddToPropertyList(list, propertyPrefix + "StripLines.", dictionaryEntry.Key.ToString(), dictionaryEntry.Value.ToString());
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			foreach (Hashtable item in list)
			{
				ChartStripLine chartStripLine = new ChartStripLine();
				chartStripLine.Title = this.ConvertDundasCRIStringProperty(item["StripLine.Title"]);
				chartStripLine.Interval = this.ConvertDundasCRIDoubleReportExpressionProperty(item["StripLine.Interval"]);
				chartStripLine.IntervalType = new ReportExpression<ChartIntervalTypes>(this.ConvertDundasCRIStringProperty(ChartIntervalTypes.Auto.ToString(), item["StripLine.IntervalType"]), CultureInfo.InvariantCulture);
				chartStripLine.IntervalOffset = this.ConvertDundasCRIDoubleReportExpressionProperty(item["StripLine.IntervalOffset"]);
				chartStripLine.IntervalOffsetType = new ReportExpression<ChartIntervalOffsetTypes>(this.ConvertDundasCRIStringProperty(ChartIntervalTypes.Auto.ToString(), item["StripLine.IntervalOffsetType"]), CultureInfo.InvariantCulture);
				chartStripLine.StripWidth = this.ConvertDundasCRIDoubleReportExpressionProperty(item["StripLine.StripWidth"]);
				chartStripLine.StripWidthType = new ReportExpression<ChartStripWidthTypes>(this.ConvertDundasCRIStringProperty(ChartIntervalTypes.Auto.ToString(), item["StripLine.StripWidthType"]), CultureInfo.InvariantCulture);
				switch (this.ConvertDundasCRIStringProperty(item["StripLine.TitleAngle"]))
				{
				case "90":
					chartStripLine.TextOrientation = TextOrientations.Rotated90;
					break;
				case "180":
					chartStripLine.TextOrientation = TextOrientations.Stacked;
					break;
				case "270":
					chartStripLine.TextOrientation = TextOrientations.Rotated270;
					break;
				default:
					chartStripLine.TextOrientation = TextOrientations.Horizontal;
					break;
				}
				string text2 = this.ConvertDundasCRIStringProperty(item["StripLine.Href"]);
				if (text2 != string.Empty)
				{
					chartStripLine.ActionInfo = new ActionInfo();
					AspNetCore.ReportingServices.RdlObjectModel.Action action = new AspNetCore.ReportingServices.RdlObjectModel.Action();
					action.Hyperlink = text2;
					chartStripLine.ActionInfo.Actions.Add(action);
				}
				AspNetCore.ReportingServices.RdlObjectModel.Style style = new AspNetCore.ReportingServices.RdlObjectModel.Style();
				int num2 = 0;
				string a = this.ConvertDundasCRIStringProperty(item["StripLine.TitleAlignment"], ref num2);
				int num3 = 0;
				string a2 = this.ConvertDundasCRIStringProperty(item["StripLine.TitleLineAlignment"], ref num3);
				object color = item["StripLine.TitleColor"];
				object backgroundColor = item["StripLine.BackColor"];
				object backgroundGradientType = item["StripLine.BackGradientType"];
				object backgroundGradientEndColor = item["StripLine.BackGradientEndColor"];
				object backgroundHatchType = item["StripLine.BackHatchStyle"];
				object borderColor = item["StripLine.BorderColor"];
				object borderStyle = item["StripLine.BorderStyle"];
				object borderWidth = item["StripLine.BorderWidth"];
				object imageReference = item["StripLine.BackImage"];
				object imageTransColor = item["StripLine.BackImageTranspColor"];
				object imageAlign = item["StripLine.BackImageAlign"];
				object imageMode = item["StripLine.BackImageMode"];
				object font = item["StripLine.TitleFont"] ?? "Microsoft Sans Serif, 8pt";
				string textAlign = (a == "Near") ? TextAlignments.Left.ToString() : ((a == "Center") ? TextAlignments.Center.ToString() : TextAlignments.Right.ToString());
				string textVerticalAlign = (num3 == 0) ? null : ((a2 == "Center") ? VerticalAlignments.Middle.ToString() : ((a2 == "Far") ? VerticalAlignments.Bottom.ToString() : VerticalAlignments.Top.ToString()));
				style = (chartStripLine.Style = this.ConvertDundasCRIStyleProperty(color, backgroundColor, backgroundGradientType, backgroundGradientEndColor, backgroundHatchType, null, null, borderColor, borderStyle, borderWidth, imageReference, imageTransColor, imageAlign, imageMode, font, null, null, textAlign, textVerticalAlign));
				axis.ChartStripLines.Add(chartStripLine);
			}
		}

		private void UpgradeDundasCRIChartSeries(ChartSeries series, IList<ChartDerivedSeries> derivedSeriesCollection, Hashtable seriesProperties, List<Hashtable> dataPointCustomProperties)
		{
			string text = this.ConvertDundasCRIStringProperty(seriesProperties["Type"]);
			this.SetChartSeriesType(series, text);
			this.ConvertDundasCRICustomProperties(series.CustomProperties, seriesProperties["CustomAttributes"]);
			ReportExpression reportExpression = null;
			foreach (CustomProperty customProperty2 in series.CustomProperties)
			{
				switch (customProperty2.Name.Value)
				{
				case "ShowPieAsCollected":
					customProperty2.Name = "CollectedStyle";
					customProperty2.Value = "CollectedPie";
					break;
				case "CollectedPercentage":
					customProperty2.Name = "CollectedThreshold";
					break;
				case "CollectedSliceLabel":
					customProperty2.Name = "CollectedLabel";
					reportExpression = customProperty2.Value;
					break;
				case "CollectedSliceColor":
					customProperty2.Name = "CollectedColor";
					break;
				case "ShowCollectedLegend":
					customProperty2.Name = "CollectedChartShowLegend";
					break;
				case "ShowCollectedPointLabels":
					customProperty2.Name = "CollectedChartShowLabels";
					break;
				}
			}
			if (reportExpression != (string)null)
			{
				CustomProperty customProperty = new CustomProperty();
				customProperty.Name = "CollectedLegendText";
				customProperty.Value = reportExpression;
				series.CustomProperties.Add(customProperty);
			}
			series.ValueAxisName = this.ConvertDundasCRIStringProperty("Primary", seriesProperties["YAxisType"]);
			series.CategoryAxisName = this.ConvertDundasCRIStringProperty("Primary", seriesProperties["XAxisType"]);
			series.Style.ShadowOffset = this.ConvertDundasCRIPixelReportSizeProperty(seriesProperties["ShadowOffset"]);
			int num = 0;
			ChartItemInLegend chartItemInLegend = new ChartItemInLegend();
			chartItemInLegend.LegendText = this.ConvertDundasCRIStringProperty(seriesProperties["LegendText"], ref num);
			bool? nullable = this.ConvertDundasCRIBoolProperty(seriesProperties["ShowInLegend"], ref num);
			if (nullable.HasValue)
			{
				chartItemInLegend.Hidden = !nullable.Value;
			}
			if (num > 0)
			{
				series.ChartItemInLegend = chartItemInLegend;
			}
			num = 0;
			ChartSmartLabel chartSmartLabel = new ChartSmartLabel();
			chartSmartLabel.CalloutBackColor = this.ConvertDundasCRIColorProperty(chartSmartLabel.CalloutBackColor, seriesProperties["SmartLabels.CalloutBackColor"], ref num);
			chartSmartLabel.CalloutLineAnchor = new ReportExpression<ChartCalloutLineAnchorTypes>(this.ConvertDundasCRIStringProperty(seriesProperties["SmartLabels.CalloutLineAnchorCap"], ref num), CultureInfo.InvariantCulture);
			chartSmartLabel.CalloutLineColor = this.ConvertDundasCRIColorProperty(chartSmartLabel.CalloutLineColor, seriesProperties["SmartLabels.CalloutLineColor"], ref num);
			chartSmartLabel.CalloutLineStyle = new ReportExpression<ChartCalloutLineStyles>(this.ConvertDundasCRIStringProperty(seriesProperties["SmartLabels.CalloutLineStyle"], ref num), CultureInfo.InvariantCulture);
			chartSmartLabel.CalloutLineWidth = this.ConvertDundasCRIPixelReportSizeProperty(seriesProperties["SmartLabels.CalloutLineWidth"], ref num);
			chartSmartLabel.MaxMovingDistance = this.ConvertDundasCRIPixelReportSizeProperty(30.0, seriesProperties["SmartLabels.MaxMovingDistance"], ref num);
			chartSmartLabel.MinMovingDistance = this.ConvertDundasCRIPixelReportSizeProperty(seriesProperties["SmartLabels.MinMovingDistance"], ref num);
			string a = this.ConvertDundasCRIStringProperty(seriesProperties["SmartLabels.AllowOutsidePlotArea"], ref num);
			if (a == "Yes")
			{
				chartSmartLabel.AllowOutSidePlotArea = ChartAllowOutSidePlotAreaTypes.True;
			}
			else if (a == "No")
			{
				chartSmartLabel.AllowOutSidePlotArea = ChartAllowOutSidePlotAreaTypes.False;
			}
			else
			{
				chartSmartLabel.AllowOutSidePlotArea = ChartAllowOutSidePlotAreaTypes.Partial;
			}
			string text2 = this.ConvertDundasCRIStringProperty(seriesProperties["SmartLabels.CalloutStyle"], ref num);
			chartSmartLabel.CalloutStyle = new ReportExpression<ChartCalloutStyles>((text2 == "Underlined") ? (text2 = ChartCalloutStyles.Underline.ToString()) : text2, CultureInfo.InvariantCulture);
			bool? nullable2 = this.ConvertDundasCRIBoolProperty(seriesProperties["SmartLabels.Enabled"], ref num);
			if (nullable2.HasValue)
			{
				chartSmartLabel.Disabled = !nullable2.Value;
			}
			bool? nullable3 = this.ConvertDundasCRIBoolProperty(seriesProperties["SmartLabels.HideOverlapped"], ref num);
			if (nullable3.HasValue)
			{
				chartSmartLabel.ShowOverlapped = !nullable3.Value;
			}
			bool? nullable4 = this.ConvertDundasCRIBoolProperty(seriesProperties["SmartLabels.MarkerOverlapping"], ref num);
			if (nullable4.HasValue)
			{
				chartSmartLabel.MarkerOverlapping = nullable4.Value;
			}
			string text3 = this.ConvertDundasCRIStringProperty(seriesProperties["SmartLabels.MovingDirection"], ref num);
			if (text3 != string.Empty)
			{
				ChartNoMoveDirections chartNoMoveDirections2 = chartSmartLabel.ChartNoMoveDirections = new ChartNoMoveDirections();
				text3 = " " + text3.Replace(',', ' ') + " ";
				chartNoMoveDirections2.Down = !text3.Contains(" Bottom ");
				chartNoMoveDirections2.DownLeft = !text3.Contains(" BottomLeft ");
				chartNoMoveDirections2.DownRight = !text3.Contains(" BottomRight ");
				chartNoMoveDirections2.Left = !text3.Contains(" Left ");
				chartNoMoveDirections2.Right = !text3.Contains(" Right ");
				chartNoMoveDirections2.Up = !text3.Contains(" Top ");
				chartNoMoveDirections2.UpLeft = !text3.Contains(" TopLeft ");
				chartNoMoveDirections2.UpRight = !text3.Contains(" TopRight ");
			}
			if (num > 0)
			{
				series.ChartSmartLabel = chartSmartLabel;
			}
			num = 0;
			ChartEmptyPoints chartEmptyPoints = new ChartEmptyPoints();
			this.ConvertDundasCRICustomProperties(chartEmptyPoints.CustomProperties, seriesProperties["EmptyPointStyle.CustomAttributes"], ref num);
			chartEmptyPoints.AxisLabel = this.ConvertDundasCRIStringProperty(seriesProperties["EmptyPointStyle.AxisLabel"], ref num);
			chartEmptyPoints.ActionInfo = this.ConvertDundasCRIActionInfoProperty(seriesProperties["EmptyPointStyle.Href"], ref num);
			chartEmptyPoints.Style = this.ConvertDundasCRIEmptyColorStyleProperty(seriesProperties["EmptyPointStyle.Color"], null, seriesProperties["EmptyPointStyle.BackGradientType"], seriesProperties["EmptyPointStyle.BackGradientEndColor"], seriesProperties["EmptyPointStyle.BackHatchStyle"], seriesProperties["EmptyPointStyle.BorderColor"], seriesProperties["EmptyPointStyle.BorderStyle"], seriesProperties["EmptyPointStyle.BorderWidth"], seriesProperties["EmptyPointStyle.BackImage"], seriesProperties["EmptyPointStyle.BackImageTranspColor"], seriesProperties["EmptyPointStyle.BackImageAlign"], seriesProperties["EmptyPointStyle.BackImageMode"], null, null, ref num);
			int num2 = 0;
			ChartMarker chartMarker = new ChartMarker();
			chartMarker.Style = this.ConvertDundasCRIEmptyColorStyleProperty(seriesProperties["EmptyPointStyle.MarkerColor"], null, null, null, null, seriesProperties["EmptyPointStyle.MarkerBorderColor"], null, seriesProperties["EmptyPointStyle.MarkerBorderWidth"], seriesProperties["EmptyPointStyle.MarkerImage"], seriesProperties["EmptyPointStyle.MarkerImageTranspColor"], seriesProperties["EmptyPointStyle.MarkerImageAlign"], seriesProperties["EmptyPointStyle.MarkerImageMode"], null, null, ref num2);
			chartMarker.Type = new ReportExpression<ChartMarkerTypes>(this.ConvertDundasCRIStringProperty(seriesProperties["EmptyPointStyle.MarkerStyle"], ref num2), CultureInfo.InvariantCulture);
			chartMarker.Size = this.ConvertDundasCRIPixelReportSizeProperty(seriesProperties["EmptyPointStyle.MarkerSize"], ref num2);
			if (num2 > 0)
			{
				chartEmptyPoints.ChartMarker = chartMarker;
				num++;
			}
			int num3 = 0;
			AspNetCore.ReportingServices.RdlObjectModel.ChartDataLabel chartDataLabel = new AspNetCore.ReportingServices.RdlObjectModel.ChartDataLabel();
			string value = this.ConvertDundasCRIStringProperty(seriesProperties["EmptyPointStyle.Label"], ref num3);
			chartDataLabel.Label = value;
			chartDataLabel.Visible = !string.IsNullOrEmpty(value);
			chartDataLabel.Rotation = this.ConvertDundasCRIIntegerReportExpressionProperty(seriesProperties["EmptyPointStyle.FontAngle"], ref num3);
			chartDataLabel.ActionInfo = this.ConvertDundasCRIActionInfoProperty(seriesProperties["ChartEmptyPointstyle.LabelHref"], ref num3);
			chartDataLabel.Style = this.ConvertDundasCRIEmptyColorStyleProperty(seriesProperties["EmptyPointStyle.FontColor"], seriesProperties["EmptyPointStyle.LabelBackColor"], null, null, null, seriesProperties["EmptyPointStyle.LabelBorderColor"], seriesProperties["EmptyPointStyle.LabelBorderStyle"], seriesProperties["EmptyPointStyle.LabelBorderWidth"], null, null, null, null, seriesProperties["EmptyPointStyle.Font"] ?? "Microsoft Sans Serif, 8pt", null, ref num2);
			if (num3 > 0)
			{
				chartEmptyPoints.ChartDataLabel = chartDataLabel;
				num++;
			}
			if (num > 0)
			{
				series.ChartEmptyPoints = chartEmptyPoints;
			}
			IDictionaryEnumerator enumerator2 = seriesProperties.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator2.Current;
					string text4 = dictionaryEntry.Key.ToString();
					if (text4.Equals("ERRORFORMULA:BOXPLOT", StringComparison.OrdinalIgnoreCase) || text4.Equals("FINANCIALFORMULA:FORECASTING", StringComparison.OrdinalIgnoreCase))
					{
						if (this.m_throwUpgradeException)
						{
							throw new CRI2005UpgradeException();
						}
						base.UpgradeResults.HasUnsupportedDundasChartFeatures = true;
					}
					if (text4.StartsWith("FINANCIALFORMULA:", StringComparison.OrdinalIgnoreCase) || text4.StartsWith("STATISTICALFORMULA:", StringComparison.OrdinalIgnoreCase))
					{
						ChartDerivedSeries chartDerivedSeries = new ChartDerivedSeries();
						chartDerivedSeries.SourceChartSeriesName = series.Name;
						ChartSeries chartSeries = chartDerivedSeries.ChartSeries;
						string text6 = chartSeries.Name = series.Name + "_Formula";
						string str = text6;
						int num4 = 1;
						bool flag = false;
						do
						{
							flag = false;
							foreach (ChartDerivedSeries item in derivedSeriesCollection)
							{
								if (item.ChartSeries.Name == str + ((num4 > 1) ? num4.ToString(CultureInfo.InvariantCulture) : string.Empty))
								{
									flag = true;
									num4++;
									break;
								}
							}
						}
						while (flag);
						if (num4 > 1)
						{
							ChartSeries chartSeries2 = chartDerivedSeries.ChartSeries;
							chartSeries2.Name += num4.ToString(CultureInfo.InvariantCulture);
						}
						string value2 = text4.Substring(text4.IndexOf(':') + 1);
						try
						{
							chartDerivedSeries.DerivedSeriesFormula = (ChartFormulas)Enum.Parse(typeof(ChartFormulas), value2);
						}
						catch
						{
							goto IL_0cf9;
						}
						string[] array = dictionaryEntry.Value.ToString().Split(';');
						string[] array2 = array;
						foreach (string text7 in array2)
						{
							string[] array3 = text7.Split('=');
							string a2 = (array3.Length > 0) ? array3[0].ToUpperInvariant().Trim() : string.Empty;
							string text8 = (array3.Length > 1) ? array3[1] : string.Empty;
							if (a2 == "SERIESTYPE")
							{
								this.SetChartSeriesType(chartDerivedSeries.ChartSeries, text8);
							}
							else if (a2 == "SHOWLEGEND")
							{
								bool flag2 = default(bool);
								if (bool.TryParse(text8, out flag2))
								{
									if (chartDerivedSeries.ChartSeries.ChartItemInLegend == null)
									{
										chartDerivedSeries.ChartSeries.ChartItemInLegend = new ChartItemInLegend();
									}
									chartDerivedSeries.ChartSeries.ChartItemInLegend.Hidden = !flag2;
								}
							}
							else if (a2 == "LEGENDTEXT")
							{
								if (chartDerivedSeries.ChartSeries.ChartItemInLegend == null)
								{
									chartDerivedSeries.ChartSeries.ChartItemInLegend = new ChartItemInLegend();
								}
								chartDerivedSeries.ChartSeries.ChartItemInLegend.LegendText = text8.Replace("_x003B_", ";").Replace("_x003D_", "=");
							}
							else if (a2 == "FORMULAPARAMETERS")
							{
								ChartFormulaParameter chartFormulaParameter = new ChartFormulaParameter();
								chartFormulaParameter.Name = "FormulaParameters";
								chartFormulaParameter.Value = text8;
								chartDerivedSeries.ChartFormulaParameters.Add(chartFormulaParameter);
							}
							else if (a2 == "NEWAREA")
							{
								bool flag3 = default(bool);
								if (bool.TryParse(text8, out flag3) && flag3)
								{
									chartDerivedSeries.ChartSeries.ChartAreaName = "#NewChartArea";
								}
							}
							else if (a2 == "STARTFROMFIRST")
							{
								ChartFormulaParameter chartFormulaParameter2 = new ChartFormulaParameter();
								chartFormulaParameter2.Name = "StartFromFirst";
								chartFormulaParameter2.Value = text8;
								chartDerivedSeries.ChartFormulaParameters.Add(chartFormulaParameter2);
							}
							else if (a2 == "OUTPUT")
							{
								ChartFormulaParameter chartFormulaParameter3 = new ChartFormulaParameter();
								chartFormulaParameter3.Name = "Output";
								chartFormulaParameter3.Value = text8.Replace("#OUTPUTSERIES", chartDerivedSeries.ChartSeries.Name);
								chartDerivedSeries.ChartFormulaParameters.Add(chartFormulaParameter3);
							}
							else if (a2 == "INPUT")
							{
								ChartFormulaParameter chartFormulaParameter4 = new ChartFormulaParameter();
								chartFormulaParameter4.Name = "Input";
								chartFormulaParameter4.Value = text8;
								chartDerivedSeries.ChartFormulaParameters.Add(chartFormulaParameter4);
							}
							else if (a2 == "SECONDARYAXIS")
							{
								if (this.m_throwUpgradeException)
								{
									throw new CRI2005UpgradeException();
								}
								base.UpgradeResults.HasUnsupportedDundasChartFeatures = true;
							}
						}
						derivedSeriesCollection.Add(chartDerivedSeries);
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator2 as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			goto IL_0cf9;
			IL_0cf9:
			string text9 = this.ConvertDundasCRIStringProperty(seriesProperties["ID"]);
			if (text9 != null)
			{
				foreach (Hashtable dataPointCustomProperty in dataPointCustomProperties)
				{
					if (this.ConvertDundasCRIStringProperty(dataPointCustomProperty["ID"]) == text9)
					{
						AspNetCore.ReportingServices.RdlObjectModel.ChartDataPoint chartDataPoint = new AspNetCore.ReportingServices.RdlObjectModel.ChartDataPoint();
						chartDataPoint.ChartDataPointValues = new ChartDataPointValues();
						series.ChartDataPoints.Add(chartDataPoint);
						this.ConvertDundasCRICustomProperties(chartDataPoint.CustomProperties, dataPointCustomProperty["CustomAttributes"]);
						chartDataPoint.AxisLabel = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["AxisLabel"]);
						chartDataPoint.ChartDataPointValues.X = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["XValue"]);
						ReportExpression<ChartSubtypes> subtype;
						if (series.Type.Value == ChartTypes.Range)
						{
							subtype = series.Subtype;
							switch (subtype.Value)
							{
							case ChartSubtypes.BoxPlot:
								chartDataPoint.ChartDataPointValues.Median = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["Y5"]);
								chartDataPoint.ChartDataPointValues.Mean = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["Y4"]);
								chartDataPoint.ChartDataPointValues.End = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["Y3"]);
								chartDataPoint.ChartDataPointValues.Start = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["Y2"]);
								chartDataPoint.ChartDataPointValues.High = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["Y1"]);
								chartDataPoint.ChartDataPointValues.Low = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["Y0"]);
								break;
							case ChartSubtypes.Candlestick:
							case ChartSubtypes.Stock:
								chartDataPoint.ChartDataPointValues.End = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["Y3"]);
								chartDataPoint.ChartDataPointValues.Start = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["Y2"]);
								chartDataPoint.ChartDataPointValues.Low = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["Y1"]);
								chartDataPoint.ChartDataPointValues.High = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["Y0"]);
								break;
							case ChartSubtypes.ErrorBar:
								chartDataPoint.ChartDataPointValues.High = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["Y2"]);
								chartDataPoint.ChartDataPointValues.Low = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["Y1"]);
								chartDataPoint.ChartDataPointValues.Y = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["Y0"]);
								break;
							case ChartSubtypes.Bar:
								chartDataPoint.ChartDataPointValues.End = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["Y1"]);
								chartDataPoint.ChartDataPointValues.Start = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["Y0"]);
								break;
							default:
								chartDataPoint.ChartDataPointValues.Low = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["Y1"]);
								chartDataPoint.ChartDataPointValues.High = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["Y0"]);
								break;
							}
						}
						else
						{
							subtype = series.Subtype;
							if (subtype.Value == ChartSubtypes.Bubble)
							{
								chartDataPoint.ChartDataPointValues.Size = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["Y1"]);
							}
							chartDataPoint.ChartDataPointValues.Y = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["Y0"]);
						}
						if (!text.StartsWith("fast", StringComparison.OrdinalIgnoreCase))
						{
							chartDataPoint.Style = this.ConvertDundasCRIEmptyColorStyleProperty(dataPointCustomProperty["Color"], null, dataPointCustomProperty["BackGradientType"], dataPointCustomProperty["BackGradientEndColor"], dataPointCustomProperty["BackHatchStyle"], dataPointCustomProperty["BorderColor"] ?? seriesProperties["BorderColor"], dataPointCustomProperty["BorderStyle"] ?? seriesProperties["BorderStyle"], dataPointCustomProperty["BorderWidth"] ?? seriesProperties["BorderWidth"], dataPointCustomProperty["BackImage"], dataPointCustomProperty["BackImageTranspColor"], dataPointCustomProperty["BackImageAlign"], dataPointCustomProperty["BackImageMode"], null, null, ref num);
						}
						else
						{
							chartDataPoint.Style = this.ConvertDundasCRIEmptyColorStyleProperty(dataPointCustomProperty["Color"], null, dataPointCustomProperty["BackGradientType"], dataPointCustomProperty["BackGradientEndColor"], dataPointCustomProperty["BackHatchStyle"], null, null, null, null, null, null, null, null, null, ref num);
						}
						num = 0;
						ChartMarker chartMarker2 = new ChartMarker();
						chartMarker2.Type = new ReportExpression<ChartMarkerTypes>(this.ConvertDundasCRIStringProperty(dataPointCustomProperty["MarkerStyle"], ref num), CultureInfo.InvariantCulture);
						chartMarker2.Size = this.ConvertDundasCRIPixelReportSizeProperty(dataPointCustomProperty["MarkerSize"] ?? seriesProperties["MarkerSize"], ref num);
						chartMarker2.Style = this.ConvertDundasCRIEmptyColorStyleProperty(dataPointCustomProperty["MarkerColor"], null, null, null, null, (!this.IsZero(dataPointCustomProperty["MarkerBorderWidth"])) ? dataPointCustomProperty["MarkerBorderColor"] : ((object)ReportColor.Empty), null, (!this.IsZero(dataPointCustomProperty["MarkerBorderWidth"])) ? dataPointCustomProperty["MarkerBorderWidth"] : null, dataPointCustomProperty["MarkerImage"], dataPointCustomProperty["MarkerImageTranspColor"], null, null, null, null, ref num);
						if (num > 0)
						{
							chartDataPoint.ChartMarker = chartMarker2;
						}
						num = 0;
						AspNetCore.ReportingServices.RdlObjectModel.ChartDataLabel chartDataLabel2 = new AspNetCore.ReportingServices.RdlObjectModel.ChartDataLabel();
						bool? nullable5 = this.ConvertDundasCRIBoolProperty(seriesProperties["ShowLabelAsValue"], ref num);
						if (nullable5.HasValue)
						{
							chartDataLabel2.UseValueAsLabel = nullable5.Value;
						}
						string value3 = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["Label"], ref num);
						chartDataLabel2.Label = value3;
						chartDataLabel2.Visible = (!string.IsNullOrEmpty(value3) || chartDataLabel2.UseValueAsLabel.Value);
						chartDataLabel2.Rotation = this.ConvertDundasCRIIntegerReportExpressionProperty(dataPointCustomProperty["FontAngle"], ref num);
						chartDataLabel2.ActionInfo = this.ConvertDundasCRIActionInfoProperty(dataPointCustomProperty["LabelHref"], ref num);
						chartDataLabel2.Style = this.ConvertDundasCRIStyleProperty(dataPointCustomProperty["FontColor"], dataPointCustomProperty["LabelBackColor"], null, null, null, null, null, dataPointCustomProperty["LabelBorderColor"], dataPointCustomProperty["LabelBorderStyle"], dataPointCustomProperty["LabelBorderWidth"], null, null, null, null, dataPointCustomProperty["Font"] ?? "Microsoft Sans Serif, 8pt", dataPointCustomProperty["LabelFormat"], null, null, null, ref num);
						if (num > 0)
						{
							chartDataPoint.ChartDataLabel = chartDataLabel2;
						}
						chartDataPoint.ActionInfo = (this.UpgradeDundasCRIChartActionInfo(dataPointCustomProperty) ?? this.ConvertDundasCRIActionInfoProperty(dataPointCustomProperty["Href"]));
						num = 0;
						ChartItemInLegend chartItemInLegend2 = new ChartItemInLegend();
						chartItemInLegend2.ActionInfo = this.ConvertDundasCRIActionInfoProperty(dataPointCustomProperty["LegendHref"], ref num);
						chartItemInLegend2.LegendText = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["LegendText"], ref num);
						if (num > 0)
						{
							chartDataPoint.ChartItemInLegend = chartItemInLegend2;
						}
						string text10 = this.ConvertDundasCRIStringProperty(dataPointCustomProperty["MarkerBorderWidth"]);
						if (series.ChartEmptyPoints != null && series.ChartEmptyPoints.ChartMarker != null && text10 != string.Empty)
						{
							if (series.ChartEmptyPoints.ChartMarker.Style == null)
							{
								series.ChartEmptyPoints.ChartMarker.Style = new EmptyColorStyle();
							}
							if (series.ChartEmptyPoints.ChartMarker.Style.Border == null)
							{
								series.ChartEmptyPoints.ChartMarker.Style.Border = new EmptyBorder();
							}
							if (this.IsZero(text10))
							{
								series.ChartEmptyPoints.ChartMarker.Style.Border.Color = ReportColor.Empty;
							}
							else
							{
								series.ChartEmptyPoints.ChartMarker.Style.Border.Width = this.ConvertDundasCRIPixelReportSizeProperty(text10);
							}
						}
					}
				}
			}
		}

		private bool UpgradeDundasCRIChartTitle(AspNetCore.ReportingServices.RdlObjectModel.ChartTitle title, Hashtable titleProperties, string propertyPrefix)
		{
			int num = 0;
			title.Caption = this.ConvertDundasCRIStringProperty(titleProperties[propertyPrefix + "Text"], ref num);
			title.DockOffset = this.ConvertDundasCRIIntegerReportExpressionProperty(titleProperties[propertyPrefix + "DockOffset"], ref num);
			string docking = this.ConvertDundasCRIStringProperty(titleProperties[propertyPrefix + "Docking"], ref num);
			string alignment = this.ConvertDundasCRIStringProperty(titleProperties[propertyPrefix + "Alignment"], ref num);
			title.Position = this.ConvertDundasCRIPosition(docking, alignment);
			bool? nullable = this.ConvertDundasCRIBoolProperty(titleProperties[propertyPrefix + "DockInsideChartArea"], ref num);
			if (nullable.HasValue)
			{
				title.DockOutsideChartArea = !nullable.Value;
			}
			bool? nullable2 = this.ConvertDundasCRIBoolProperty(titleProperties[propertyPrefix + "Visible"], ref num);
			if (nullable2.HasValue)
			{
				title.Hidden = !nullable2.Value;
			}
			title.Style = this.ConvertDundasCRIStyleProperty(titleProperties[propertyPrefix + "Color"], titleProperties[propertyPrefix + "BackColor"], titleProperties[propertyPrefix + "BackGradientType"], titleProperties[propertyPrefix + "BackGradientEndColor"], titleProperties[propertyPrefix + "BackHatchStyle"], titleProperties[propertyPrefix + "ShadowColor"], titleProperties[propertyPrefix + "ShadowOffset"], titleProperties[propertyPrefix + "BorderColor"] ?? ((object)Color.Transparent), titleProperties[propertyPrefix + "BorderStyle"], titleProperties[propertyPrefix + "BorderWidth"], titleProperties[propertyPrefix + "BackImage"], titleProperties[propertyPrefix + "BackImageTranspColor"], titleProperties[propertyPrefix + "BackImageAlign"], titleProperties[propertyPrefix + "BackImageMode"], this.ConvertDundasCRIStringProperty("Microsoft Sans Serif, 8pt", titleProperties[propertyPrefix + "Font"]), null, titleProperties[propertyPrefix + "Style"], null, null, ref num);
			title.ChartElementPosition = this.ConvertDundasCRIChartElementPosition(titleProperties[propertyPrefix + "Position.Y"], titleProperties[propertyPrefix + "Position.X"], titleProperties[propertyPrefix + "Position.Height"], titleProperties[propertyPrefix + "Position.Width"], ref num);
			return num > 0;
		}

		private void UpgradeDundasCRIChartLegend(ChartLegend legend, Hashtable legendProperties, string propertyPrefix)
		{
			legend.HeaderSeparator = new ReportExpression<ChartHeaderSeparatorTypes>(this.ConvertDundasCRIStringProperty(legendProperties[propertyPrefix + "HeaderSeparator"]), CultureInfo.InvariantCulture);
			legend.InterlacedRowsColor = this.ConvertDundasCRIColorProperty(legend.InterlacedRowsColor, legendProperties[propertyPrefix + "InterlacedRowsColor"]);
			legend.Reversed = new ReportExpression<ChartLegendReversedTypes>(this.ConvertDundasCRIStringProperty(legendProperties[propertyPrefix + "Reversed"]), CultureInfo.InvariantCulture);
			legend.ColumnSeparator = new ReportExpression<ChartColumnSeparatorTypes>(this.ConvertDundasCRIStringProperty(legendProperties[propertyPrefix + "ItemColumnSeparator"]), CultureInfo.InvariantCulture);
			legend.ColumnSeparatorColor = this.ConvertDundasCRIColorProperty(legend.ColumnSeparatorColor, legendProperties[propertyPrefix + "ItemColumnSeparatorColor"]);
			legend.ColumnSpacing = this.ConvertDundasCRIIntegerReportExpressionProperty(legend.ColumnSpacing, legendProperties[propertyPrefix + "ItemColumnSpacing"]);
			legend.MaxAutoSize = this.ConvertDundasCRIIntegerReportExpressionProperty(legend.MaxAutoSize, legendProperties[propertyPrefix + "MaxAutoSize"]);
			legend.TextWrapThreshold = this.ConvertDundasCRIIntegerReportExpressionProperty(legend.TextWrapThreshold, legendProperties[propertyPrefix + "TextWrapThreshold"]);
			legend.MinFontSize = this.ConvertDundasCRIPointReportSizeProperty(legend.MinFontSize, legendProperties[propertyPrefix + "AutoFitMinFontSize"]);
			bool? nullable = this.ConvertDundasCRIBoolProperty(legendProperties[propertyPrefix + "AutoFitText"]);
			if (nullable.HasValue)
			{
				legend.AutoFitTextDisabled = !nullable.Value;
			}
			bool? nullable2 = this.ConvertDundasCRIBoolProperty(legendProperties[propertyPrefix + "Enabled"]);
			if (nullable2.HasValue)
			{
				legend.Hidden = !nullable2.Value;
			}
			bool? nullable3 = this.ConvertDundasCRIBoolProperty(legendProperties[propertyPrefix + "DockInsideChartArea"]);
			if (nullable3.HasValue)
			{
				legend.DockOutsideChartArea = !nullable3.Value;
			}
			bool? nullable4 = this.ConvertDundasCRIBoolProperty(legendProperties[propertyPrefix + "InterlacedRows"]);
			if (nullable4.HasValue)
			{
				legend.InterlacedRows = nullable4.Value;
			}
			bool? nullable5 = this.ConvertDundasCRIBoolProperty(legendProperties[propertyPrefix + "EquallySpacedItems"]);
			if (nullable5.HasValue)
			{
				legend.EquallySpacedItems = nullable5.Value;
			}
			legend.Style = this.ConvertDundasCRIStyleProperty(legendProperties[propertyPrefix + "FontColor"], legendProperties[propertyPrefix + "BackColor"], legendProperties[propertyPrefix + "BackGradientType"], legendProperties[propertyPrefix + "BackGradientEndColor"], legendProperties[propertyPrefix + "BackHatchStyle"], legendProperties[propertyPrefix + "ShadowColor"], legendProperties[propertyPrefix + "ShadowOffset"], legendProperties[propertyPrefix + "BorderColor"], legendProperties[propertyPrefix + "BorderStyle"] ?? ((object)BorderStyles.Solid), legendProperties[propertyPrefix + "BorderWidth"], legendProperties[propertyPrefix + "BackImage"], legendProperties[propertyPrefix + "BackImageTranspColor"], legendProperties[propertyPrefix + "BackImageAlign"], legendProperties[propertyPrefix + "BackImageMode"], this.ConvertDundasCRIStringProperty("Microsoft Sans Serif, 8pt", legendProperties[propertyPrefix + "Font"]), null, null, null, null);
			string text = this.ConvertDundasCRIStringProperty("Right", legendProperties[propertyPrefix + "Docking"]);
			string text2 = this.ConvertDundasCRIStringProperty("Near", legendProperties[propertyPrefix + "Alignment"]);
			text2 = ((!(text == "Top") && !(text == "Bottom")) ? text2.Replace("Near", "Top").Replace("Far", "Bottom") : text2.Replace("Near", "Left").Replace("Far", "Right"));
			legend.Position = new ReportExpression<ChartPositions>(text + text2, CultureInfo.InvariantCulture);
			string text3 = this.ConvertDundasCRIStringProperty(legendProperties[propertyPrefix + "LegendStyle"]);
			string a = this.ConvertDundasCRIStringProperty(legendProperties[propertyPrefix + "TableStyle"]);
			if (text3 != "" && text3 != "Table")
			{
				legend.Layout = new ReportExpression<ChartLegendLayouts>(text3, CultureInfo.InvariantCulture);
			}
			else if (a == "Wide")
			{
				legend.Layout = ChartLegendLayouts.WideTable;
			}
			else if (a == "Tall")
			{
				legend.Layout = ChartLegendLayouts.TallTable;
			}
			else
			{
				legend.Layout = ChartLegendLayouts.AutoTable;
			}
			legend.ChartElementPosition = this.ConvertDundasCRIChartElementPosition(legendProperties[propertyPrefix + "Position.Y"], legendProperties[propertyPrefix + "Position.X"], legendProperties[propertyPrefix + "Position.Height"], legendProperties[propertyPrefix + "Position.Width"]);
			int num = 0;
			ChartLegendTitle chartLegendTitle = new ChartLegendTitle();
			chartLegendTitle.Caption = this.ConvertDundasCRIStringProperty(legendProperties[propertyPrefix + "Title"], ref num);
			int num2 = 0;
			string a2 = this.ConvertDundasCRIStringProperty(legendProperties[propertyPrefix + "TitleAlignment"], ref num2);
			chartLegendTitle.Style = this.ConvertDundasCRIStyleProperty(legendProperties[propertyPrefix + "TitleColor"], legendProperties[propertyPrefix + "TitleBackColor"], null, null, null, null, null, legendProperties[propertyPrefix + "TitleSeparatorColor"], null, null, null, null, null, null, this.ConvertDundasCRIStringProperty("Microsoft Sans Serif, 8pt, style=Bold", legendProperties[propertyPrefix + "TitleFont"]), null, null, (num2 == 0) ? null : ((a2 == "Near") ? TextAlignments.Left.ToString() : ((a2 == "Far") ? TextAlignments.Right.ToString() : TextAlignments.Center.ToString())), null, ref num);
			try
			{
				chartLegendTitle.TitleSeparator = new ReportExpression<ChartTitleSeparatorTypes>(this.ConvertDundasCRIStringProperty(legendProperties[propertyPrefix + "TitleSeparator"], ref num), CultureInfo.InvariantCulture);
			}
			catch
			{
			}
			if (num > 0)
			{
				legend.ChartLegendTitle = chartLegendTitle;
			}
			IDictionaryEnumerator enumerator = legendProperties.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string text4 = ((DictionaryEntry)enumerator.Current).Key.ToString();
					if (text4.StartsWith("LEGEND.CUSTOMITEMS.", StringComparison.OrdinalIgnoreCase) || text4.StartsWith("LEGEND.CELLCOLUMNS.", StringComparison.OrdinalIgnoreCase))
					{
						base.UpgradeResults.HasUnsupportedDundasChartFeatures = true;
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		private bool UpgradeDundasCRIChartGridLines(ChartGridLines gridLines, Hashtable properties, string propertyPrefix, bool isMajor)
		{
			int num = 0;
			gridLines.Interval = this.ConvertDundasCRIDoubleReportExpressionProperty(properties[propertyPrefix + "Interval"], ref num);
			gridLines.IntervalType = new ReportExpression<ChartIntervalTypes>(this.ConvertDundasCRIStringProperty(properties[propertyPrefix + "IntervalType"], ref num), CultureInfo.InvariantCulture);
			gridLines.IntervalOffset = this.ConvertDundasCRIDoubleReportExpressionProperty(double.NaN, properties[propertyPrefix + "IntervalOffset"], ref num);
			gridLines.IntervalOffsetType = new ReportExpression<ChartIntervalOffsetTypes>(this.ConvertDundasCRIStringProperty(properties[propertyPrefix + "IntervalOffsetType"], ref num), CultureInfo.InvariantCulture);
			if (isMajor)
			{
				bool? nullable = this.ConvertDundasCRIBoolProperty(properties[propertyPrefix + "Enabled"], ref num);
				if (nullable.HasValue && !nullable.Value)
				{
					gridLines.Enabled = ChartGridLinesEnabledTypes.False;
				}
				else
				{
					gridLines.Enabled = ChartGridLinesEnabledTypes.True;
				}
			}
			else
			{
				bool? nullable2 = this.ConvertDundasCRIBoolProperty(properties[propertyPrefix + "Disabled"], ref num);
				if (nullable2.HasValue && !nullable2.Value)
				{
					gridLines.Enabled = ChartGridLinesEnabledTypes.True;
				}
				else
				{
					gridLines.Enabled = ChartGridLinesEnabledTypes.False;
				}
			}
			gridLines.Style = this.ConvertDundasCRIStyleProperty(null, null, null, null, null, null, null, properties[propertyPrefix + "LineColor"], properties[propertyPrefix + "LineStyle"], properties[propertyPrefix + "LineWidth"], null, null, null, null, null, null, null, null, null, ref num);
			return num > 0;
		}

		private bool UpgradeDundasCRIChartTickMarks(ChartTickMarks tickMarks, Hashtable properties, string propertyPrefix, bool isMajor)
		{
			int num = 0;
			tickMarks.Type = new ReportExpression<ChartTickMarkTypes>(this.ConvertDundasCRIStringProperty(properties[propertyPrefix + "Style"], ref num), CultureInfo.InvariantCulture);
			tickMarks.Length = this.ConvertDundasCRIDoubleReportExpressionProperty(tickMarks.Length, properties[propertyPrefix + "Size"], ref num);
			tickMarks.Interval = this.ConvertDundasCRIDoubleReportExpressionProperty(properties[propertyPrefix + "Interval"], ref num);
			tickMarks.IntervalType = new ReportExpression<ChartIntervalTypes>(this.ConvertDundasCRIStringProperty(properties[propertyPrefix + "IntervalType"], ref num), CultureInfo.InvariantCulture);
			tickMarks.IntervalOffset = this.ConvertDundasCRIDoubleReportExpressionProperty(properties[propertyPrefix + "IntervalOffset"], ref num);
			tickMarks.IntervalOffsetType = new ReportExpression<ChartIntervalOffsetTypes>(this.ConvertDundasCRIStringProperty(properties[propertyPrefix + "IntervalOffsetType"], ref num), CultureInfo.InvariantCulture);
			if (isMajor)
			{
				bool? nullable = this.ConvertDundasCRIBoolProperty(properties[propertyPrefix + "Enabled"], ref num);
				if (nullable.HasValue && !nullable.Value)
				{
					tickMarks.Enabled = ChartTickMarksEnabledTypes.False;
				}
				else
				{
					tickMarks.Enabled = ChartTickMarksEnabledTypes.True;
				}
			}
			else
			{
				bool? nullable2 = this.ConvertDundasCRIBoolProperty(properties[propertyPrefix + "Disabled"], ref num);
				if (nullable2.HasValue && !nullable2.Value)
				{
					tickMarks.Enabled = ChartTickMarksEnabledTypes.True;
				}
				else
				{
					tickMarks.Enabled = ChartTickMarksEnabledTypes.False;
				}
			}
			tickMarks.Style = this.ConvertDundasCRIStyleProperty(null, null, null, null, null, null, null, properties[propertyPrefix + "LineColor"], properties[propertyPrefix + "LineStyle"], properties[propertyPrefix + "LineWidth"], null, null, null, null, null, null, null, null, null, ref num);
			return num > 0;
		}

		private ActionInfo UpgradeDundasCRIChartActionInfo(Hashtable properties)
		{
			return this.UpgradeDundasCRIActionInfo(properties, string.Empty, "Hyperlink");
		}

		private ChartPositions ConvertDundasCRIPosition(string docking, string alignment)
		{
			switch (docking)
			{
			case "Left":
				if (alignment.EndsWith("Right", StringComparison.Ordinal))
				{
					return ChartPositions.LeftTop;
				}
				if (alignment.EndsWith("Left", StringComparison.Ordinal))
				{
					return ChartPositions.LeftBottom;
				}
				return ChartPositions.LeftCenter;
			case "Right":
				if (alignment.EndsWith("Left", StringComparison.Ordinal))
				{
					return ChartPositions.RightTop;
				}
				if (alignment.EndsWith("Right", StringComparison.Ordinal))
				{
					return ChartPositions.RightBottom;
				}
				return ChartPositions.RightCenter;
			case "Bottom":
				if (alignment.EndsWith("Left", StringComparison.Ordinal))
				{
					return ChartPositions.BottomLeft;
				}
				if (alignment.EndsWith("Right", StringComparison.Ordinal))
				{
					return ChartPositions.BottomRight;
				}
				return ChartPositions.BottomCenter;
			default:
				if (alignment.EndsWith("Left", StringComparison.Ordinal))
				{
					return ChartPositions.TopLeft;
				}
				if (alignment.EndsWith("Right", StringComparison.Ordinal))
				{
					return ChartPositions.TopRight;
				}
				return ChartPositions.TopCenter;
			}
		}

		private void SetChartSeriesType(ChartSeries series, string dundasSeriesType)
		{
            switch (dundasSeriesType)
            {
                case "Column":
                    series.Type = ChartTypes.Column;
                    series.Subtype = ChartSubtypes.Plain;
                    return;
                case "StackedColumn":
                    series.Type = ChartTypes.Column;
                    series.Subtype = ChartSubtypes.Stacked;
                    return;
                case "StackedColumn100":
                    series.Type = ChartTypes.Column;
                    series.Subtype = ChartSubtypes.PercentStacked;
                    return;
                case "Bar":
                    series.Type = ChartTypes.Bar;
                    series.Subtype = ChartSubtypes.Plain;
                    return;
                case "StackedBar":
                    series.Type = ChartTypes.Bar;
                    series.Subtype = ChartSubtypes.Stacked;
                    return;
                case "StackedBar100":
                    series.Type = ChartTypes.Bar;
                    series.Subtype = ChartSubtypes.PercentStacked;
                    return;
                case "Area":
                    series.Type = ChartTypes.Area;
                    series.Subtype = ChartSubtypes.Plain;
                    return;
                case "SplineArea":
                    series.Type = ChartTypes.Area;
                    series.Subtype = ChartSubtypes.Smooth;
                    return;
                case "StackedArea":
                    series.Type = ChartTypes.Area;
                    series.Subtype = ChartSubtypes.Stacked;
                    return;
                case "StackedArea100":
                    series.Type = ChartTypes.Area;
                    series.Subtype = ChartSubtypes.PercentStacked;
                    return;
                case "Line":
                case "FastLine":
                    series.Type = ChartTypes.Line;
                    series.Subtype = ChartSubtypes.Plain;
                    return;
                case "Spline":
                    series.Type = ChartTypes.Line;
                    series.Subtype = ChartSubtypes.Smooth;
                    return;
                case "StepLine":
                    series.Type = ChartTypes.Line;
                    series.Subtype = ChartSubtypes.Stepped;
                    return;
                case "Point":
                case "FastPoint":
                    series.Type = ChartTypes.Scatter;
                    series.Subtype = ChartSubtypes.Plain;
                    return;
                case "Bubble":
                    series.Type = ChartTypes.Scatter;
                    series.Subtype = ChartSubtypes.Bubble;
                    return;
                case "Stock":
                    series.Type = ChartTypes.Range;
                    series.Subtype = ChartSubtypes.Stock;
                    return;
                case "CandleStick":
                    series.Type = ChartTypes.Range;
                    series.Subtype = ChartSubtypes.Candlestick;
                    return;
                case "RangeColumn":
                    series.Type = ChartTypes.Range;
                    series.Subtype = ChartSubtypes.Column;
                    return;
                case "Range":
                    series.Type = ChartTypes.Range;
                    series.Subtype = ChartSubtypes.Plain;
                    return;
                case "SplineRange":
                    series.Type = ChartTypes.Range;
                    series.Subtype = ChartSubtypes.Smooth;
                    return;
                case "ErrorBar":
                    series.Type = ChartTypes.Range;
                    series.Subtype = ChartSubtypes.ErrorBar;
                    return;
                case "BoxPlot":
                    series.Type = ChartTypes.Range;
                    series.Subtype = ChartSubtypes.BoxPlot;
                    return;
                case "Gantt":
                    series.Type = ChartTypes.Range;
                    series.Subtype = ChartSubtypes.Bar;
                    return;
                case "Pie":
                    series.Type = ChartTypes.Shape;
                    series.Subtype = ChartSubtypes.Pie;
                    return;
                case "Doughnut":
                    series.Type = ChartTypes.Shape;
                    series.Subtype = ChartSubtypes.Doughnut;
                    return;
                case "Funnel":
                    series.Type = ChartTypes.Shape;
                    series.Subtype = ChartSubtypes.Funnel;
                    return;
                case "Pyramid":
                    series.Type = ChartTypes.Shape;
                    series.Subtype = ChartSubtypes.Pyramid;
                    return;
                case "Polar":
                    series.Type = ChartTypes.Polar;
                    series.Subtype = ChartSubtypes.Plain;
                    return;
                case "Radar":
                    series.Type = ChartTypes.Polar;
                    series.Subtype = ChartSubtypes.Radar;
                    return;
            }
        }

		private BackgroundImage ConvertDundasCRIChartBackgroundImageProperty(object imageReference, object transparentColor, object align, object mode, ref int counter)
		{
			int num = 0;
			BackgroundImage backgroundImage = new BackgroundImage();
			backgroundImage.Source = SourceType.External;
			backgroundImage.Value = this.ConvertDundasCRIStringProperty(imageReference, ref num);
			backgroundImage.TransparentColor = this.ConvertDundasCRIColorProperty(backgroundImage.TransparentColor, transparentColor, ref num);
			backgroundImage.Position = new ReportExpression<BackgroundPositions>(this.ConvertDundasCRIStringProperty(BackgroundPositions.TopLeft.ToString(), align, ref num), CultureInfo.InvariantCulture);
			string text = this.ConvertDundasCRIStringProperty(mode, ref num);
			switch (text)
			{
			case "Tile":
			case "TileFlipX":
			case "TileFlipY":
			case "TileFlipXY":
				backgroundImage.BackgroundRepeat = BackgroundRepeatTypes.Repeat;
				break;
			case "Scaled":
				backgroundImage.BackgroundRepeat = BackgroundRepeatTypes.Fit;
				break;
			case "Unscaled":
				backgroundImage.BackgroundRepeat = BackgroundRepeatTypes.Clip;
				break;
			default:
				if (text != string.Empty)
				{
					backgroundImage.BackgroundRepeat = new ReportExpression<BackgroundRepeatTypes>(text, CultureInfo.InvariantCulture);
				}
				break;
			}
			if (num > 0)
			{
				counter++;
				return backgroundImage;
			}
			return null;
		}

		private void FixChartAxisStriplineTitleAngle(AspNetCore.ReportingServices.RdlObjectModel.Chart chart)
		{
			foreach (ChartArea chartArea in chart.ChartAreas)
			{
				bool flag = false;
				if (chart.ChartData != null && chart.ChartData.ChartSeriesCollection != null)
				{
					foreach (ChartSeries item in chart.ChartData.ChartSeriesCollection)
					{
						if (item.ChartAreaName == chartArea.Name && item.Type.Value == ChartTypes.Bar)
						{
							flag = true;
							break;
						}
					}
				}
				IList<ChartAxis> list = flag ? chartArea.ChartValueAxes : chartArea.ChartCategoryAxes;
				IList<ChartAxis> list2 = flag ? chartArea.ChartCategoryAxes : chartArea.ChartValueAxes;
				foreach (ChartAxis item2 in list)
				{
					if (item2.ChartStripLines != null)
					{
						foreach (ChartStripLine chartStripLine in item2.ChartStripLines)
						{
							if (!chartStripLine.TextOrientation.IsExpression)
							{
								switch (chartStripLine.TextOrientation.Value)
								{
								case TextOrientations.Rotated90:
								case TextOrientations.Rotated270:
									chartStripLine.TextOrientation = TextOrientations.Horizontal;
									break;
								case TextOrientations.Stacked:
									chartStripLine.TextOrientation = TextOrientations.Rotated90;
									break;
								default:
									chartStripLine.TextOrientation = TextOrientations.Auto;
									break;
								}
							}
						}
					}
				}
				foreach (ChartAxis item3 in list2)
				{
					if (item3.ChartStripLines != null)
					{
						foreach (ChartStripLine chartStripLine2 in item3.ChartStripLines)
						{
							if (!chartStripLine2.TextOrientation.IsExpression)
							{
								TextOrientations value = chartStripLine2.TextOrientation.Value;
								TextOrientations textOrientations = value;
								if (textOrientations == TextOrientations.Horizontal || textOrientations == TextOrientations.Stacked)
								{
									chartStripLine2.TextOrientation = TextOrientations.Auto;
								}
							}
						}
					}
				}
			}
		}

		private void UpgradeDundasCRIGaugePanel(AspNetCore.ReportingServices.RdlObjectModel.CustomReportItem cri, GaugePanel gaugePanel)
		{
			gaugePanel.Name = cri.Name;
			gaugePanel.ActionInfo = cri.ActionInfo;
			gaugePanel.Bookmark = cri.Bookmark;
			gaugePanel.DataElementName = cri.DataElementName;
			gaugePanel.DataElementOutput = cri.DataElementOutput;
			gaugePanel.DocumentMapLabel = cri.DocumentMapLabel;
			gaugePanel.PropertyStore.SetObject(12, cri.PropertyStore.GetObject(12));
			gaugePanel.Height = cri.Height;
			gaugePanel.Left = cri.Left;
			gaugePanel.Parent = cri.Parent;
			gaugePanel.RepeatWith = cri.RepeatWith;
			gaugePanel.Style = cri.Style;
			gaugePanel.ToolTip = cri.ToolTip;
			gaugePanel.PropertyStore.SetObject(10, cri.PropertyStore.GetObject(10));
			gaugePanel.Top = cri.Top;
			gaugePanel.Visibility = cri.Visibility;
			gaugePanel.Width = cri.Width;
			gaugePanel.ZIndex = cri.ZIndex;
			if (cri.CustomData != null)
			{
				gaugePanel.DataSetName = cri.CustomData.DataSetName;
				gaugePanel.Filters = cri.CustomData.Filters;
			}
			Hashtable hashtable = new Hashtable();
			Hashtable hashtable2 = new Hashtable();
			Hashtable hashtable3 = new Hashtable();
			List<Hashtable> list = new List<Hashtable>();
			List<Hashtable> list2 = new List<Hashtable>();
			List<Hashtable> list3 = new List<Hashtable>();
			foreach (CustomProperty customProperty in cri.CustomProperties)
			{
				string text = customProperty.Name.Value;
				if (text.StartsWith("expression:", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Substring("expression:".Length);
				}
				if (!this.AddToPropertyList(list, "GaugeCore.Labels.", text, customProperty.Value) && !this.AddToPropertyList(list2, "GaugeCore.CircularGauges.", text, customProperty.Value) && !this.AddToPropertyList(list3, "GaugeCore.LinearGauges.", text, customProperty.Value))
				{
					hashtable.Add(text, customProperty.Value);
				}
				if (text.StartsWith("GAUGECORE.STATEINDICATORS.", StringComparison.OrdinalIgnoreCase) || text.StartsWith("GAUGECORE.NUMERICINDICATORS.", StringComparison.OrdinalIgnoreCase) || text.StartsWith("GAUGECORE.NAMEDIMAGES.", StringComparison.OrdinalIgnoreCase) || text.StartsWith("GAUGECORE.IMAGES.", StringComparison.OrdinalIgnoreCase))
				{
					base.UpgradeResults.HasUnsupportedDundasGaugeFeatures = true;
				}
			}
			if (hashtable["CUSTOM_CODE_CS"] != null || hashtable["CUSTOM_CODE_VB"] != null || hashtable["CUSTOM_CODE_COMPILED_ASSEMBLY"] != null)
			{
				if (this.m_throwUpgradeException)
				{
					throw new CRI2005UpgradeException();
				}
				base.UpgradeResults.HasUnsupportedDundasGaugeFeatures = true;
			}
			if (cri.CustomData != null && cri.CustomData.DataRowHierarchy != null && cri.CustomData.DataRowHierarchy.DataMembers != null && cri.CustomData.DataRowHierarchy.DataMembers.Count > 0)
			{
				foreach (CustomProperty customProperty2 in cri.CustomData.DataRowHierarchy.DataMembers[0].CustomProperties)
				{
					hashtable3.Add(customProperty2.Name, customProperty2.Value);
				}
			}
			if (cri.CustomData != null && cri.CustomData.DataRows != null && cri.CustomData.DataRows.Count > 0 && cri.CustomData.DataRows[0].Count > 0)
			{
				foreach (AspNetCore.ReportingServices.RdlObjectModel.DataValue item in cri.CustomData.DataRows[0][0])
				{
					hashtable2.Add(item.Name, item.Value);
				}
			}
			gaugePanel.ToolTip = this.ConvertDundasCRIStringProperty(hashtable["GaugeCore.ToolTip"]);
			gaugePanel.AntiAliasing = new ReportExpression<AntiAliasingTypes>(this.ConvertDundasCRIStringProperty(hashtable["GaugeCore.AntiAliasing"]), CultureInfo.InvariantCulture);
			gaugePanel.TextAntiAliasingQuality = new ReportExpression<TextAntiAliasingQualityTypes>(this.ConvertDundasCRIStringProperty(hashtable["GaugeCore.TextAntiAliasingQuality"]), CultureInfo.InvariantCulture);
			gaugePanel.ShadowIntensity = this.ConvertDundasCRIDoubleReportExpressionProperty(gaugePanel.ShadowIntensity, hashtable["GaugeCore.ShadowIntensity"]);
			bool? nullable = this.ConvertDundasCRIBoolProperty(hashtable["GaugeCore.AutoLayout"]);
			if (nullable.HasValue)
			{
				gaugePanel.AutoLayout = nullable.Value;
			}
			else
			{
				gaugePanel.AutoLayout = true;
			}
			gaugePanel.Style = this.ConvertDundasCRIStyleProperty(null, hashtable["GaugeCore.BackColor"] ?? ((object)Color.White), null, null, null, null, null, null, null, null, null, null, null);
			BackFrame backFrame = new BackFrame();
			if (this.UpgradeDundasCRIGaugeBackFrame(backFrame, hashtable, "GaugeCore.BackFrame."))
			{
				gaugePanel.BackFrame = backFrame;
			}
			foreach (Hashtable item2 in list)
			{
				GaugeLabel gaugeLabel = new GaugeLabel();
				gaugePanel.GaugeLabels.Add(gaugeLabel);
				this.UpgradeDundasCRIGaugeLabel(gaugeLabel, item2, "GaugeLabel.");
			}
			foreach (Hashtable item3 in list2)
			{
				RadialGauge radialGauge = new RadialGauge();
				gaugePanel.RadialGauges.Add(radialGauge);
				this.UpgradeDundasCRIGaugeRadial(radialGauge, item3, "CircularGauge.", hashtable3, hashtable2);
			}
			foreach (Hashtable item4 in list3)
			{
				LinearGauge linearGauge = new LinearGauge();
				gaugePanel.LinearGauges.Add(linearGauge);
				this.UpgradeDundasCRIGaugeLinear(linearGauge, item4, "LinearGauge.", hashtable3, hashtable2);
			}
			if (cri.CustomData != null && cri.CustomData.DataColumnHierarchy != null)
			{
				IList<DataMember> dataMembers = cri.CustomData.DataColumnHierarchy.DataMembers;
				GaugeMember gaugeMember = null;
				while (dataMembers != null && dataMembers.Count > 0)
				{
					DataMember dataMember = dataMembers[0];
					if (((DataGrouping2005)dataMember).Static)
					{
						break;
					}
					if (gaugeMember == null)
					{
						GaugeMember gaugeMember3 = gaugePanel.GaugeMember = new GaugeMember();
						gaugeMember = gaugeMember3;
					}
					else
					{
						GaugeMember gaugeMember4 = gaugeMember;
						GaugeMember gaugeMember6 = gaugeMember4.ChildGaugeMember = new GaugeMember();
						gaugeMember = gaugeMember6;
					}
					gaugeMember.SortExpressions = dataMember.SortExpressions;
					if (dataMember.Group != null)
					{
						gaugeMember.Group = dataMember.Group;
					}
					dataMembers = dataMember.DataMembers;
				}
			}
			this.FixGaugeElementNames(gaugePanel);
		}

		private bool UpgradeDundasCRIGaugeBackFrame(BackFrame backFrame, Hashtable backFrameProperties, string propertyPrefix)
		{
			int num = 0;
			string value = this.ConvertDundasCRIStringProperty(backFrameProperties[propertyPrefix + "FrameStyle"], ref num);
			if (string.IsNullOrEmpty(value))
			{
				backFrame.FrameStyle = new ReportExpression<FrameStyles>(this.ConvertDundasCRIStringProperty(backFrameProperties[propertyPrefix + "Style"], ref num), CultureInfo.InvariantCulture);
			}
			else
			{
				backFrame.FrameStyle = new ReportExpression<FrameStyles>(value, CultureInfo.InvariantCulture);
			}
			string value2 = this.ConvertDundasCRIStringProperty(backFrameProperties[propertyPrefix + "FrameShape"], ref num);
			if (string.IsNullOrEmpty(value2))
			{
				backFrame.FrameShape = new ReportExpression<FrameShapes>(this.ConvertDundasCRIStringProperty(backFrameProperties[propertyPrefix + "Shape"], ref num), CultureInfo.InvariantCulture);
			}
			else
			{
				backFrame.FrameShape = new ReportExpression<FrameShapes>(value2, CultureInfo.InvariantCulture);
			}
			backFrame.FrameWidth = this.ConvertDundasCRIDoubleReportExpressionProperty(backFrame.FrameWidth, backFrameProperties[propertyPrefix + "FrameWidth"], ref num);
			backFrame.GlassEffect = new ReportExpression<GlassEffects>(this.ConvertDundasCRIStringProperty(backFrameProperties[propertyPrefix + "GlassEffect"], ref num), CultureInfo.InvariantCulture);
			backFrame.Style = this.ConvertDundasCRIStyleProperty(null, backFrameProperties[propertyPrefix + "FrameColor"] ?? ((object)Color.Gainsboro), backFrameProperties[propertyPrefix + "FrameGradientType"] ?? ((object)BackgroundGradients.DiagonalLeft), backFrameProperties[propertyPrefix + "FrameGradientEndColor"] ?? ((object)Color.Gray), backFrameProperties[propertyPrefix + "FrameHatchStyle"], backFrameProperties[propertyPrefix + "ShadowOffset"], backFrameProperties[propertyPrefix + "BorderColor"], backFrameProperties[propertyPrefix + "BorderStyle"], backFrameProperties[propertyPrefix + "BorderWidth"], null, null, null, null, ref num);
			int num2 = 0;
			AspNetCore.ReportingServices.RdlObjectModel.Style style = this.ConvertDundasCRIStyleProperty(null, backFrameProperties[propertyPrefix + "BackColor"] ?? ((object)Color.Silver), backFrameProperties[propertyPrefix + "BackGradientType"] ?? ((object)BackgroundGradients.DiagonalLeft), backFrameProperties[propertyPrefix + "BackGradientEndColor"] ?? ((object)Color.Gray), backFrameProperties[propertyPrefix + "BackHatchStyle"], null, null, null, null, null, null, null, null, ref num2);
			if (num2 > 0)
			{
				backFrame.FrameBackground = new FrameBackground();
				backFrame.FrameBackground.Style = style;
				num++;
			}
			return num > 0;
		}

		private void UpgradeDundasCRIGaugeLabel(GaugeLabel label, Hashtable labelProperties, string propertyPrefix)
		{
			label.Name = this.ConvertDundasCRIStringProperty(labelProperties[propertyPrefix + "Name"]);
			label.ParentItem = this.ConvertDundasCRIStringProperty(labelProperties[propertyPrefix + "Parent"]);
			label.ZIndex = this.ConvertDundasCRIIntegerReportExpressionProperty(labelProperties[propertyPrefix + "ZOrder"]);
			label.Left = this.ConvertDundasCRIDoubleReportExpressionProperty(labelProperties[propertyPrefix + "Location.X"]);
			label.Top = this.ConvertDundasCRIDoubleReportExpressionProperty(labelProperties[propertyPrefix + "Location.Y"]);
			label.Width = this.ConvertDundasCRIDoubleReportExpressionProperty(labelProperties[propertyPrefix + "Size.Width"]);
			label.Height = this.ConvertDundasCRIDoubleReportExpressionProperty(labelProperties[propertyPrefix + "Size.Height"]);
			label.ResizeMode = new ReportExpression<ResizeModes>(this.ConvertDundasCRIStringProperty(labelProperties[propertyPrefix + "ResizeMode"]), CultureInfo.InvariantCulture);
			label.Text = this.ConvertDundasCRIStringProperty("Text", labelProperties[propertyPrefix + "Text"]);
			label.TextShadowOffset = this.ConvertDundasCRIPixelReportSizeProperty(labelProperties[propertyPrefix + "TextShadowOffset"]);
			label.Angle = this.ConvertDundasCRIDoubleReportExpressionProperty(labelProperties[propertyPrefix + "Angle"]);
			bool? nullable = this.ConvertDundasCRIBoolProperty(labelProperties[propertyPrefix + "Visible"]);
			if (nullable.HasValue)
			{
				label.Hidden = !nullable.Value;
			}
			if (this.ConvertDundasCRIStringProperty("Default", labelProperties[propertyPrefix + "FontUnit"]) == "Percent")
			{
				label.UseFontPercent = true;
			}
			string text = this.ConvertDundasCRIStringProperty(labelProperties[propertyPrefix + "TextAlignment"]);
			TextAlignments textAlignments = (TextAlignments)((string.IsNullOrEmpty(text) || text.EndsWith("LEFT", StringComparison.OrdinalIgnoreCase)) ? 2 : (text.EndsWith("CENTER", StringComparison.OrdinalIgnoreCase) ? 3 : 4));
			VerticalAlignments verticalAlignments = (VerticalAlignments)((string.IsNullOrEmpty(text) || text.StartsWith("TOP", StringComparison.OrdinalIgnoreCase)) ? 1 : (text.StartsWith("MIDDLE", StringComparison.OrdinalIgnoreCase) ? 2 : 3));
			label.Style = this.ConvertDundasCRIStyleProperty(labelProperties[propertyPrefix + "TextColor"], labelProperties[propertyPrefix + "BackColor"], labelProperties[propertyPrefix + "BackGradientType"], labelProperties[propertyPrefix + "BackGradientEndColor"], labelProperties[propertyPrefix + "BackHatchStyle"], labelProperties[propertyPrefix + "BackShadowOffset"], labelProperties[propertyPrefix + "BorderColor"], labelProperties[propertyPrefix + "BorderStyle"], labelProperties[propertyPrefix + "BorderWidth"], labelProperties.ContainsKey(propertyPrefix + "Font") ? labelProperties[propertyPrefix + "Font"] : "Microsoft Sans Serif, 8.25pt", null, textAlignments, verticalAlignments);
		}

		private void UpgradeDundasCRIGauge(Gauge gauge, Hashtable gaugeProperties, string propertyPrefix, Hashtable formulaProperties, Hashtable dataValueProperties)
		{
			string str = gauge.Name = this.ConvertDundasCRIStringProperty(gaugeProperties[propertyPrefix + "Name"]);
			Hashtable hashtable = new Hashtable();
			Hashtable hashtable2 = new Hashtable();
			string text2 = ((gauge is LinearGauge) ? "LinearGauge" : "CircularGauge") + ":" + str;
			IDictionaryEnumerator enumerator = formulaProperties.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
					if (dictionaryEntry.Key.ToString().StartsWith(text2, StringComparison.Ordinal))
					{
						hashtable.Add(dictionaryEntry.Key.ToString().Remove(0, text2.Length + 1), dictionaryEntry.Value);
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			IDictionaryEnumerator enumerator2 = dataValueProperties.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					DictionaryEntry dictionaryEntry2 = (DictionaryEntry)enumerator2.Current;
					if (dictionaryEntry2.Key.ToString().StartsWith(text2, StringComparison.Ordinal))
					{
						hashtable2.Add(dictionaryEntry2.Key.ToString().Remove(0, text2.Length + 1), dictionaryEntry2.Value);
					}
				}
			}
			finally
			{
				IDisposable disposable2 = enumerator2 as IDisposable;
				if (disposable2 != null)
				{
					disposable2.Dispose();
				}
			}
			gauge.ActionInfo = this.UpgradeDundasCRIGaugeActionInfo(formulaProperties, text2 + ":");
			gauge.ParentItem = this.ConvertDundasCRIStringProperty(gaugeProperties[propertyPrefix + "Parent"]);
			gauge.ZIndex = this.ConvertDundasCRIIntegerReportExpressionProperty(gaugeProperties[propertyPrefix + "ZOrder"]);
			gauge.Left = this.ConvertDundasCRIDoubleReportExpressionProperty(gaugeProperties[propertyPrefix + "Location.X"]);
			gauge.Top = this.ConvertDundasCRIDoubleReportExpressionProperty(gaugeProperties[propertyPrefix + "Location.Y"]);
			gauge.Width = this.ConvertDundasCRIDoubleReportExpressionProperty(gaugeProperties[propertyPrefix + "Size.Width"]);
			gauge.Height = this.ConvertDundasCRIDoubleReportExpressionProperty(gaugeProperties[propertyPrefix + "Size.Height"]);
			bool? nullable = this.ConvertDundasCRIBoolProperty(gaugeProperties[propertyPrefix + "Visible"]);
			if (nullable.HasValue)
			{
				gauge.Hidden = !nullable.Value;
			}
			bool? nullable2 = this.ConvertDundasCRIBoolProperty(gaugeProperties[propertyPrefix + "ClipContent"]);
			if (nullable2.HasValue)
			{
				gauge.ClipContent = nullable2.Value;
			}
			else
			{
				gauge.ClipContent = true;
			}
			BackFrame backFrame = new BackFrame();
			if (this.UpgradeDundasCRIGaugeBackFrame(backFrame, gaugeProperties, propertyPrefix + "BackFrame."))
			{
				gauge.BackFrame = backFrame;
			}
			List<Hashtable> list = new List<Hashtable>();
			List<Hashtable> list2 = new List<Hashtable>();
			List<Hashtable> list3 = new List<Hashtable>();
			IDictionaryEnumerator enumerator3 = gaugeProperties.GetEnumerator();
			try
			{
				while (enumerator3.MoveNext())
				{
					DictionaryEntry dictionaryEntry3 = (DictionaryEntry)enumerator3.Current;
					string key = dictionaryEntry3.Key.ToString();
					string value = dictionaryEntry3.Value.ToString();
					this.AddToPropertyList(list, propertyPrefix + "Scales.", key, value);
					this.AddToPropertyList(list2, propertyPrefix + "Ranges.", key, value);
					this.AddToPropertyList(list3, propertyPrefix + "Pointers.", key, value);
				}
			}
			finally
			{
				IDisposable disposable3 = enumerator3 as IDisposable;
				if (disposable3 != null)
				{
					disposable3.Dispose();
				}
			}
			Hashtable hashtable3 = new Hashtable();
			foreach (Hashtable item in list)
			{
				if (gauge is LinearGauge)
				{
					LinearScale linearScale = new LinearScale();
					this.UpgradeDundasCRIGaugeScaleLinear(linearScale, item, "LinearScale.", hashtable, hashtable2);
					gauge.GaugeScales.Add(linearScale);
					hashtable3.Add(linearScale.Name, linearScale);
				}
				else
				{
					RadialScale radialScale = new RadialScale();
					this.UpgradeDundasCRIGaugeScaleRadial(radialScale, item, "CircularScale.", hashtable, hashtable2);
					gauge.GaugeScales.Add(radialScale);
					hashtable3.Add(radialScale.Name, radialScale);
				}
			}
			string text3 = (gauge.GaugeScales.Count > 0) ? gauge.GaugeScales[0].Name : "Default";
			foreach (Hashtable item2 in list2)
			{
				ScaleRange scaleRange = new ScaleRange();
				string text4 = text3;
				if (gauge is LinearGauge)
				{
					this.UpgradeDundasCRIGaugeScaleRange(scaleRange, item2, "LinearRange.", true, hashtable, hashtable2);
					text4 = this.ConvertDundasCRIStringProperty(text3, item2["LinearRange.ScaleName"]);
				}
				else
				{
					this.UpgradeDundasCRIGaugeScaleRange(scaleRange, item2, "CircularRange.", false, hashtable, hashtable2);
					text4 = this.ConvertDundasCRIStringProperty(text3, item2["CircularRange.ScaleName"]);
				}
				if (hashtable3.Contains(text4))
				{
					((GaugeScale)hashtable3[text4]).ScaleRanges.Add(scaleRange);
				}
			}
			foreach (Hashtable item3 in list3)
			{
				GaugePointer gaugePointer = null;
				string text5 = text3;
				if (gauge is LinearGauge)
				{
					gaugePointer = new LinearPointer();
					this.UpgradeDundasCRIGaugePointerLinear((LinearPointer)gaugePointer, item3, "LinearPointer.", hashtable, hashtable2);
					text5 = this.ConvertDundasCRIStringProperty(text3, item3["LinearPointer.ScaleName"]);
				}
				else
				{
					gaugePointer = new RadialPointer();
					this.UpgradeDundasCRIGaugePointerRadial((RadialPointer)gaugePointer, item3, "CircularPointer.", hashtable, hashtable2);
					text5 = this.ConvertDundasCRIStringProperty(text3, item3["CircularPointer.ScaleName"]);
				}
				if (hashtable3.Contains(text5))
				{
					((GaugeScale)hashtable3[text5]).GaugePointers.Add(gaugePointer);
				}
			}
		}

		private void UpgradeDundasCRIGaugeRadial(RadialGauge gauge, Hashtable gaugeProperties, string propertyPrefix, Hashtable formulaProperties, Hashtable dataValueProperties)
		{
			this.UpgradeDundasCRIGauge(gauge, gaugeProperties, propertyPrefix, formulaProperties, dataValueProperties);
			gauge.PivotX = this.ConvertDundasCRIDoubleReportExpressionProperty(gauge.PivotX, gaugeProperties[propertyPrefix + "PivotPoint.X"]);
			gauge.PivotY = this.ConvertDundasCRIDoubleReportExpressionProperty(gauge.PivotY, gaugeProperties[propertyPrefix + "PivotPoint.Y"]);
		}

		private void UpgradeDundasCRIGaugeLinear(LinearGauge gauge, Hashtable gaugeProperties, string propertyPrefix, Hashtable formulaCustomProperties, Hashtable dataValueCustomProperties)
		{
			this.UpgradeDundasCRIGauge(gauge, gaugeProperties, propertyPrefix, formulaCustomProperties, dataValueCustomProperties);
			gauge.Orientation = new ReportExpression<Orientations>(this.ConvertDundasCRIStringProperty(gaugeProperties[propertyPrefix + "Orientation"]), CultureInfo.InvariantCulture);
		}

		private void UpgradeDundasCRIGaugeScale(GaugeScale scale, Hashtable scaleProperties, string propertyPrefix, Hashtable formulaProperties, Hashtable dataValueProperties)
		{
			string str = scale.Name = this.ConvertDundasCRIStringProperty(scaleProperties[propertyPrefix + "Name"]);
			string str2 = ((scale is LinearScale) ? "LinearScale" : "CircularScale") + ":" + str + ":";
			scale.MinimumValue = this.UpgradeDundasCRIGaugeInputValue(formulaProperties, dataValueProperties, str2 + "Minimum", scaleProperties, propertyPrefix + "Minimum");
			scale.MaximumValue = this.UpgradeDundasCRIGaugeInputValue(formulaProperties, dataValueProperties, str2 + "Maximum", scaleProperties, propertyPrefix + "Maximum");
			scale.Multiplier = this.ConvertDundasCRIDoubleReportExpressionProperty(scale.Multiplier, scaleProperties[propertyPrefix + "Multiplier"]);
			scale.Interval = this.ConvertDundasCRIDoubleReportExpressionProperty(scaleProperties[propertyPrefix + "Interval"]);
			scale.IntervalOffset = this.ConvertDundasCRIDoubleReportExpressionProperty(double.NaN, scaleProperties[propertyPrefix + "IntervalOffset"]);
			scale.LogarithmicBase = this.ConvertDundasCRIDoubleReportExpressionProperty(scale.LogarithmicBase, scaleProperties[propertyPrefix + "LogarithmicBase"]);
			scale.Width = this.ConvertDundasCRIDoubleReportExpressionProperty(scale.Width, scaleProperties[propertyPrefix + "Width"]);
			scale.Style = this.ConvertDundasCRIStyleProperty(null, scaleProperties[propertyPrefix + "FillColor"] ?? ((object)Color.CornflowerBlue), scaleProperties[propertyPrefix + "FillGradientType"], scaleProperties[propertyPrefix + "FillGradientEndColor"] ?? ((object)Color.White), scaleProperties[propertyPrefix + "FillHatchStyle"], scaleProperties[propertyPrefix + "ShadowOffset"] ?? ((object)1), scaleProperties[propertyPrefix + "BorderColor"], scaleProperties[propertyPrefix + "BorderStyle"], scaleProperties[propertyPrefix + "BorderWidth"], null, null, null, null);
			bool? nullable = this.ConvertDundasCRIBoolProperty(scaleProperties[propertyPrefix + "Visible"]);
			if (nullable.HasValue)
			{
				scale.Hidden = !nullable.Value;
			}
			bool? nullable2 = this.ConvertDundasCRIBoolProperty(scaleProperties[propertyPrefix + "TickMarksOnTop"]);
			if (nullable2.HasValue)
			{
				scale.TickMarksOnTop = nullable2.Value;
			}
			bool? nullable3 = this.ConvertDundasCRIBoolProperty(scaleProperties[propertyPrefix + "Reversed"]);
			if (nullable3.HasValue)
			{
				scale.Reversed = nullable3.Value;
			}
			bool? nullable4 = this.ConvertDundasCRIBoolProperty(scaleProperties[propertyPrefix + "Logarithmic"]);
			if (nullable4.HasValue)
			{
				scale.Logarithmic = nullable4.Value;
			}
			ScalePin scalePin = new ScalePin();
			if (this.UpgradeDundasCRIGaugeScalePin(scalePin, scaleProperties, propertyPrefix + "MinimumPin.", 6.0, 6.0))
			{
				scale.MinimumPin = scalePin;
			}
			ScalePin scalePin2 = new ScalePin();
			if (this.UpgradeDundasCRIGaugeScalePin(scalePin2, scaleProperties, propertyPrefix + "MaximumPin.", 6.0, 6.0))
			{
				scale.MaximumPin = scalePin2;
			}
			int num = 0;
			ScaleLabels scaleLabels = new ScaleLabels();
			scaleLabels.Placement = new ReportExpression<Placements>(this.ConvertDundasCRIStringProperty(Placements.Inside.ToString(), scaleProperties[propertyPrefix + "LabelStyle.Placement"], ref num), CultureInfo.InvariantCulture);
			scaleLabels.Interval = this.ConvertDundasCRIDoubleReportExpressionProperty(scaleProperties[propertyPrefix + "LabelStyle.Interval"], ref num);
			scaleLabels.IntervalOffset = this.ConvertDundasCRIDoubleReportExpressionProperty(double.NaN, scaleProperties[propertyPrefix + "LabelStyle.IntervalOffset"], ref num);
			scaleLabels.FontAngle = this.ConvertDundasCRIDoubleReportExpressionProperty(scaleProperties[propertyPrefix + "LabelStyle.FontAngle"], ref num);
			scaleLabels.DistanceFromScale = this.ConvertDundasCRIDoubleReportExpressionProperty(scaleLabels.DistanceFromScale, scaleProperties[propertyPrefix + "LabelStyle.DistanceFromScale"], ref num);
			bool? nullable5 = this.ConvertDundasCRIBoolProperty(scaleProperties[propertyPrefix + "LabelStyle.Visible"], ref num);
			if (nullable5.HasValue)
			{
				scaleLabels.Hidden = !nullable5.Value;
			}
			bool? nullable6 = this.ConvertDundasCRIBoolProperty(scaleProperties[propertyPrefix + "LabelStyle.AllowUpsideDown"], ref num);
			if (nullable6.HasValue)
			{
				scaleLabels.AllowUpsideDown = nullable6.Value;
			}
			bool? nullable7 = this.ConvertDundasCRIBoolProperty(scaleProperties[propertyPrefix + "LabelStyle.ShowEndLabels"], ref num);
			if (nullable7.HasValue)
			{
				scaleLabels.ShowEndLabels = nullable7.Value;
			}
			else
			{
				scaleLabels.ShowEndLabels = true;
			}
			if (this.ConvertDundasCRIStringProperty("Percent", scaleProperties[propertyPrefix + "LabelStyle.FontUnit"], ref num) == "Percent")
			{
				scaleLabels.UseFontPercent = true;
			}
			scaleLabels.Style = this.ConvertDundasCRIStyleProperty(scaleProperties[propertyPrefix + "LabelStyle.TextColor"], null, null, null, null, null, null, null, null, scaleProperties.ContainsKey(propertyPrefix + "LabelStyle.Font") ? scaleProperties[propertyPrefix + "LabelStyle.Font"] : "Microsoft Sans Serif, 14pt", scaleProperties[propertyPrefix + "LabelStyle.FormatString"], null, null, ref num);
			if (num > 0)
			{
				scale.ScaleLabels = scaleLabels;
			}
			List<Hashtable> list = new List<Hashtable>();
			IDictionaryEnumerator enumerator = scaleProperties.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
					this.AddToPropertyList(list, propertyPrefix + "CustomLabels.", dictionaryEntry.Key.ToString(), dictionaryEntry.Value.ToString());
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			foreach (Hashtable item in list)
			{
				CustomLabel customLabel = new CustomLabel();
				this.UpgradeDundasCRIGaugeCustomLabel(customLabel, item, "CustomLabel.");
				scale.CustomLabels.Add(customLabel);
			}
		}

		private void UpgradeDundasCRIGaugeScaleRadial(RadialScale scale, Hashtable scaleProperties, string propertyPrefix, Hashtable formulaProperties, Hashtable dataValueProperties)
		{
			this.UpgradeDundasCRIGaugeScale(scale, scaleProperties, propertyPrefix, formulaProperties, dataValueProperties);
			scale.Radius = this.ConvertDundasCRIDoubleReportExpressionProperty(scale.Radius, scaleProperties[propertyPrefix + "Radius"]);
			scale.StartAngle = this.ConvertDundasCRIDoubleReportExpressionProperty(scale.StartAngle, scaleProperties[propertyPrefix + "StartAngle"]);
			scale.SweepAngle = this.ConvertDundasCRIDoubleReportExpressionProperty(scale.SweepAngle, scaleProperties[propertyPrefix + "SweepAngle"]);
			bool? nullable = this.ConvertDundasCRIBoolProperty(scaleProperties[propertyPrefix + "LabelStyle.RotateLabels"]);
			if (!nullable.HasValue)
			{
				nullable = true;
			}
			if (nullable.Value)
			{
				if (scale.ScaleLabels == null)
				{
					scale.ScaleLabels = new ScaleLabels();
				}
				scale.ScaleLabels.RotateLabels = true;
			}
			GaugeTickMarks gaugeTickMarks = new GaugeTickMarks();
			if (this.UpgradeDundasCRIGaugeTickMarks(gaugeTickMarks, scaleProperties, propertyPrefix + "MajorTickMark.", 8.0, 14.0, MarkerStyles.Trapezoid))
			{
				scale.GaugeMajorTickMarks = gaugeTickMarks;
			}
			GaugeTickMarks gaugeTickMarks2 = new GaugeTickMarks();
			if (this.UpgradeDundasCRIGaugeTickMarks(gaugeTickMarks2, scaleProperties, propertyPrefix + "MinorTickMark.", 3.0, 8.0, MarkerStyles.Rectangle))
			{
				scale.GaugeMinorTickMarks = gaugeTickMarks2;
			}
		}

		private void UpgradeDundasCRIGaugeScaleLinear(LinearScale scale, Hashtable scaleProperties, string propertyPrefix, Hashtable formulaProperties, Hashtable dataValueProperties)
		{
			this.UpgradeDundasCRIGaugeScale(scale, scaleProperties, propertyPrefix, formulaProperties, dataValueProperties);
			scale.StartMargin = this.ConvertDundasCRIDoubleReportExpressionProperty(scale.StartMargin, scaleProperties[propertyPrefix + "StartMargin"]);
			scale.EndMargin = this.ConvertDundasCRIDoubleReportExpressionProperty(scale.EndMargin, scaleProperties[propertyPrefix + "EndMargin"]);
			scale.Position = this.ConvertDundasCRIDoubleReportExpressionProperty(scale.Position, scaleProperties[propertyPrefix + "Position"]);
			GaugeTickMarks gaugeTickMarks = new GaugeTickMarks();
			if (this.UpgradeDundasCRIGaugeTickMarks(gaugeTickMarks, scaleProperties, propertyPrefix + "MajorTickMark.", 4.0, 15.0, MarkerStyles.Rectangle))
			{
				scale.GaugeMajorTickMarks = gaugeTickMarks;
			}
			GaugeTickMarks gaugeTickMarks2 = new GaugeTickMarks();
			if (this.UpgradeDundasCRIGaugeTickMarks(gaugeTickMarks2, scaleProperties, propertyPrefix + "MinorTickMark.", 3.0, 9.0, MarkerStyles.Rectangle))
			{
				scale.GaugeMinorTickMarks = gaugeTickMarks2;
			}
		}

		private void UpgradeDundasCRIGaugeScaleRange(ScaleRange range, Hashtable rangeProperties, string propertyPrefix, bool isLinear, Hashtable formulaProperties, Hashtable dataValueProperties)
		{
			string str = range.Name = this.ConvertDundasCRIStringProperty(rangeProperties[propertyPrefix + "Name"]);
			string str2 = (isLinear ? "LinearRange" : "CircularRange") + ":" + str + ":";
			range.StartValue = this.UpgradeDundasCRIGaugeInputValue(formulaProperties, dataValueProperties, str2 + "StartValue", rangeProperties, propertyPrefix + "StartValue");
			range.EndValue = this.UpgradeDundasCRIGaugeInputValue(formulaProperties, dataValueProperties, str2 + "EndValue", rangeProperties, propertyPrefix + "EndValue");
			range.StartWidth = this.ConvertDundasCRIDoubleReportExpressionProperty(isLinear ? 10.0 : 15.0, rangeProperties[propertyPrefix + "StartWidth"]);
			range.EndWidth = this.ConvertDundasCRIDoubleReportExpressionProperty(isLinear ? 10.0 : 30.0, rangeProperties[propertyPrefix + "EndWidth"]);
			range.DistanceFromScale = this.ConvertDundasCRIDoubleReportExpressionProperty(isLinear ? 10.0 : 30.0, rangeProperties[propertyPrefix + "DistanceFromScale"]);
			range.Placement = new ReportExpression<Placements>(this.ConvertDundasCRIStringProperty(isLinear ? Placements.Outside.ToString() : Placements.Inside.ToString(), rangeProperties[propertyPrefix + "Placement"]), CultureInfo.InvariantCulture);
			range.InRangeTickMarksColor = this.ConvertDundasCRIColorProperty(range.InRangeTickMarksColor, rangeProperties[propertyPrefix + "InRangeTickMarkColor"]);
			range.InRangeLabelColor = this.ConvertDundasCRIColorProperty(range.InRangeLabelColor, rangeProperties[propertyPrefix + "InRangeLabelColor"]);
			range.InRangeBarPointerColor = this.ConvertDundasCRIColorProperty(range.InRangeBarPointerColor, rangeProperties[propertyPrefix + "InRangeBarPointerColor"]);
			range.BackgroundGradientType = new ReportExpression<GaugeBackgroundGradients>(this.ConvertDundasCRIStringProperty(rangeProperties[propertyPrefix + "FillGradientType"]), CultureInfo.InvariantCulture);
			bool? nullable = this.ConvertDundasCRIBoolProperty(rangeProperties[propertyPrefix + "Visible"]);
			if (nullable.HasValue)
			{
				range.Hidden = !nullable.Value;
			}
			range.ActionInfo = this.ConvertDundasCRIActionInfoProperty(rangeProperties[propertyPrefix + "Href"]);
			range.Style = this.ConvertDundasCRIStyleProperty(null, rangeProperties[propertyPrefix + "FillColor"] ?? ((object)Color.Lime), null, rangeProperties[propertyPrefix + "FillGradientEndColor"] ?? ((object)Color.Red), rangeProperties[propertyPrefix + "FillHatchStyle"], rangeProperties[propertyPrefix + "ShadowOffset"], rangeProperties[propertyPrefix + "BorderColor"], rangeProperties[propertyPrefix + "BorderStyle"], rangeProperties[propertyPrefix + "BorderWidth"], null, null, null, null);
		}

		private bool UpgradeDundasCRIGaugeTickMarkStyle(TickMarkStyle tickMarkStyle, Hashtable properties, string propertyPrefix, ReportExpression<double> defaultWidth, ReportExpression<double> defaultLength, MarkerStyles? defaultShape)
		{
			int num = 0;
			tickMarkStyle.Shape = new ReportExpression<MarkerStyles>(this.ConvertDundasCRIStringProperty(defaultShape.HasValue ? defaultShape.Value.ToString() : string.Empty, properties[propertyPrefix + "Shape"], ref num), CultureInfo.InvariantCulture);
			tickMarkStyle.Placement = new ReportExpression<Placements>(this.ConvertDundasCRIStringProperty(Placements.Cross.ToString(), properties[propertyPrefix + "Placement"], ref num), CultureInfo.InvariantCulture);
			tickMarkStyle.GradientDensity = this.ConvertDundasCRIDoubleReportExpressionProperty(tickMarkStyle.GradientDensity, properties[propertyPrefix + "GradientDensity"], ref num);
			tickMarkStyle.DistanceFromScale = this.ConvertDundasCRIDoubleReportExpressionProperty(properties[propertyPrefix + "DistanceFromScale"], ref num);
			tickMarkStyle.Width = this.ConvertDundasCRIDoubleReportExpressionProperty(defaultWidth, properties[propertyPrefix + "Width"], ref num);
			tickMarkStyle.Length = this.ConvertDundasCRIDoubleReportExpressionProperty(defaultLength, properties[propertyPrefix + "Length"], ref num);
			bool? nullable = this.ConvertDundasCRIBoolProperty(properties[propertyPrefix + "Visible"], ref num);
			if (nullable.HasValue)
			{
				tickMarkStyle.Hidden = !nullable.Value;
			}
			bool? nullable2 = this.ConvertDundasCRIBoolProperty(properties[propertyPrefix + "EnableGradient"], ref num);
			if (nullable2.HasValue)
			{
				tickMarkStyle.EnableGradient = nullable2.Value;
			}
			else
			{
				tickMarkStyle.EnableGradient = true;
			}
			tickMarkStyle.Style = this.ConvertDundasCRIStyleProperty(null, properties[propertyPrefix + "FillColor"] ?? ((object)Color.WhiteSmoke), null, null, null, null, properties[propertyPrefix + "BorderColor"] ?? ((object)Color.DimGray), null, properties[propertyPrefix + "BorderWidth"], null, null, null, null, ref num);
			return num > 0;
		}

		private bool UpgradeDundasCRIGaugeTickMarks(GaugeTickMarks tickMarks, Hashtable properties, string propertyPrefix, ReportExpression<double> defaultWidth, ReportExpression<double> defaultLength, MarkerStyles defaultShape)
		{
			int num = 0;
			if (this.UpgradeDundasCRIGaugeTickMarkStyle(tickMarks, properties, propertyPrefix, defaultWidth, defaultLength, defaultShape))
			{
				num++;
			}
			tickMarks.Interval = this.ConvertDundasCRIDoubleReportExpressionProperty(properties[propertyPrefix + "Interval"], ref num);
			tickMarks.IntervalOffset = this.ConvertDundasCRIDoubleReportExpressionProperty(double.NaN, properties[propertyPrefix + "IntervalOffset"], ref num);
			return num > 0;
		}

		private bool UpgradeDundasCRIGaugeScalePin(ScalePin pin, Hashtable properties, string propertyPrefix, ReportExpression<double> defaultWidth, ReportExpression<double> defaultLength)
		{
			int num = 0;
			if (this.UpgradeDundasCRIGaugeTickMarkStyle(pin, properties, propertyPrefix, defaultWidth, defaultLength, MarkerStyles.Circle))
			{
				num++;
			}
			pin.Location = this.ConvertDundasCRIDoubleReportExpressionProperty(pin.Location, properties[propertyPrefix + "Location"], ref num);
			bool? nullable = this.ConvertDundasCRIBoolProperty(properties[propertyPrefix + "Enable"], ref num);
			if (nullable.HasValue)
			{
				pin.Enable = nullable.Value;
			}
			int num2 = 0;
			PinLabel pinLabel = new PinLabel();
			pinLabel.Placement = new ReportExpression<Placements>(this.ConvertDundasCRIStringProperty(Placements.Inside.ToString(), properties[propertyPrefix + "LabelStyle.Placement"], ref num2), CultureInfo.InvariantCulture);
			pinLabel.Text = this.ConvertDundasCRIStringProperty(properties[propertyPrefix + "LabelStyle.Text"], ref num2);
			pinLabel.FontAngle = this.ConvertDundasCRIDoubleReportExpressionProperty(properties[propertyPrefix + "LabelStyle.FontAngle"], ref num2);
			pinLabel.DistanceFromScale = this.ConvertDundasCRIDoubleReportExpressionProperty(pinLabel.DistanceFromScale, properties[propertyPrefix + "LabelStyle.DistanceFromScale"], ref num2);
			bool? nullable2 = this.ConvertDundasCRIBoolProperty(properties[propertyPrefix + "LabelStyle.RotateLabel"], ref num2);
			if (nullable2.HasValue)
			{
				pinLabel.RotateLabel = nullable2.Value;
			}
			bool? nullable3 = this.ConvertDundasCRIBoolProperty(properties[propertyPrefix + "LabelStyle.AllowUpsideDown"], ref num2);
			if (nullable3.HasValue)
			{
				pinLabel.AllowUpsideDown = nullable3.Value;
			}
			if (this.ConvertDundasCRIStringProperty("Percent", properties[propertyPrefix + "LabelStyle.FontUnit"], ref num2) == "Percent")
			{
				pinLabel.UseFontPercent = true;
			}
			pinLabel.Style = this.ConvertDundasCRIStyleProperty(properties[propertyPrefix + "LabelStyle.TextColor"], null, null, null, null, null, null, null, null, properties.ContainsKey(propertyPrefix + "LabelStyle.Font") ? properties[propertyPrefix + "LabelStyle.Font"] : "Microsoft Sans Serif, 12pt", null, null, null, ref num2);
			if (num2 > 0)
			{
				pin.PinLabel = pinLabel;
				num++;
			}
			return num > 0;
		}

		private void UpgradeDundasCRIGaugeCustomLabel(CustomLabel customLabel, Hashtable properties, string propertyPrefix)
		{
			customLabel.Name = this.ConvertDundasCRIStringProperty(properties[propertyPrefix + "Name"]);
			customLabel.Text = this.ConvertDundasCRIStringProperty(properties[propertyPrefix + "Text"]);
			customLabel.Value = this.ConvertDundasCRIStringProperty(properties[propertyPrefix + "Value"]);
			customLabel.Placement = new ReportExpression<Placements>(this.ConvertDundasCRIStringProperty(Placements.Inside.ToString(), properties[propertyPrefix + "Placement"]), CultureInfo.InvariantCulture);
			customLabel.FontAngle = this.ConvertDundasCRIDoubleReportExpressionProperty(properties[propertyPrefix + "FontAngle"]);
			customLabel.DistanceFromScale = this.ConvertDundasCRIDoubleReportExpressionProperty(properties[propertyPrefix + "DistanceFromScale"]);
			bool? nullable = this.ConvertDundasCRIBoolProperty(properties[propertyPrefix + "Visible"]);
			if (nullable.HasValue)
			{
				customLabel.Hidden = !nullable.Value;
			}
			bool? nullable2 = this.ConvertDundasCRIBoolProperty(properties[propertyPrefix + "RotateLabel"]);
			if (nullable2.HasValue)
			{
				customLabel.RotateLabel = nullable2.Value;
			}
			bool? nullable3 = this.ConvertDundasCRIBoolProperty(properties[propertyPrefix + "AllowUpsideDown"]);
			if (nullable3.HasValue)
			{
				customLabel.AllowUpsideDown = nullable3.Value;
			}
			if (this.ConvertDundasCRIStringProperty("Percent", properties[propertyPrefix + "FontUnit"]) == "Percent")
			{
				customLabel.UseFontPercent = true;
			}
			customLabel.Style = this.ConvertDundasCRIStyleProperty(properties[propertyPrefix + "TextColor"], null, null, null, null, null, null, null, null, properties.ContainsKey(propertyPrefix + "Font") ? properties[propertyPrefix + "Font"] : "Microsoft Sans Serif, 14pt", null, null, null);
			TickMarkStyle tickMarkStyle = new TickMarkStyle();
			if (this.UpgradeDundasCRIGaugeTickMarkStyle(tickMarkStyle, properties, propertyPrefix + "TickMarkStyle.", 3.0, null, null))
			{
				customLabel.TickMarkStyle = tickMarkStyle;
			}
		}

		private void UpgradeDundasCRIGaugePointer(GaugePointer pointer, Hashtable properties, string propertyPrefix, ReportExpression<double> defaultWidth, ReportExpression<double> defaultMarkerLength, Hashtable formulaProperties, Hashtable dataValueProperties)
		{
			string str = pointer.Name = this.ConvertDundasCRIStringProperty(properties[propertyPrefix + "Name"]);
			string dataValuePropertyKey = ((pointer is LinearPointer) ? "LinearPointer" : "CircularPointer") + ":" + str;
			pointer.GaugeInputValue = this.UpgradeDundasCRIGaugeInputValue(formulaProperties, dataValueProperties, dataValuePropertyKey, properties, propertyPrefix + "Value");
			pointer.BarStart = new ReportExpression<BarStartTypes>(this.ConvertDundasCRIStringProperty(properties[propertyPrefix + "BarStart"]), CultureInfo.InvariantCulture);
			pointer.Width = this.ConvertDundasCRIDoubleReportExpressionProperty(defaultWidth, properties[propertyPrefix + "Width"]);
			pointer.MarkerLength = this.ConvertDundasCRIDoubleReportExpressionProperty(defaultMarkerLength, properties[propertyPrefix + "MarkerLength"]);
			pointer.DistanceFromScale = this.ConvertDundasCRIDoubleReportExpressionProperty(properties[propertyPrefix + "DistanceFromScale"]);
			bool? nullable = this.ConvertDundasCRIBoolProperty(properties[propertyPrefix + "Visible"]);
			if (nullable.HasValue)
			{
				pointer.Hidden = !nullable.Value;
			}
		}

		private void UpgradeDundasCRIGaugePointerLinear(LinearPointer pointer, Hashtable properties, string propertyPrefix, Hashtable formulaProperties, Hashtable dataValueProperties)
		{
			this.UpgradeDundasCRIGaugePointer(pointer, properties, propertyPrefix, 20.0, 20.0, formulaProperties, dataValueProperties);
			pointer.MarkerStyle = new ReportExpression<MarkerStyles>(this.ConvertDundasCRIStringProperty(MarkerStyles.Triangle.ToString(), properties[propertyPrefix + "MarkerStyle"]), CultureInfo.InvariantCulture);
			pointer.Type = new ReportExpression<LinearPointerTypes>(this.ConvertDundasCRIStringProperty(properties[propertyPrefix + "Type"]), CultureInfo.InvariantCulture);
			pointer.Placement = new ReportExpression<Placements>(this.ConvertDundasCRIStringProperty(Placements.Outside.ToString(), properties[propertyPrefix + "Placement"]), CultureInfo.InvariantCulture);
			int num = 0;
			Thermometer thermometer = new Thermometer();
			thermometer.ThermometerStyle = new ReportExpression<ThermometerStyles>(this.ConvertDundasCRIStringProperty(properties[propertyPrefix + "ThermometerStyle"], ref num), CultureInfo.InvariantCulture);
			thermometer.BulbOffset = this.ConvertDundasCRIDoubleReportExpressionProperty(thermometer.BulbOffset, properties[propertyPrefix + "ThermometerBulbOffset"], ref num);
			thermometer.BulbSize = this.ConvertDundasCRIDoubleReportExpressionProperty(thermometer.BulbSize, properties[propertyPrefix + "ThermometerBulbSize"], ref num);
			thermometer.Style = this.ConvertDundasCRIStyleProperty(null, properties[propertyPrefix + "ThermometerBackColor"], properties[propertyPrefix + "ThermometerBackGradientType"], properties[propertyPrefix + "ThermometerBackGradientEndColor"], properties[propertyPrefix + "ThermometerBackHatchStyle"], null, null, null, null, null, null, null, null, ref num);
			pointer.Style = this.ConvertDundasCRIStyleProperty(null, properties[propertyPrefix + "FillColor"] ?? ((object)Color.White), properties[propertyPrefix + "FillGradientType"] ?? ((object)BackgroundGradients.DiagonalLeft), properties[propertyPrefix + "FillGradientEndColor"] ?? ((object)Color.Red), properties[propertyPrefix + "FillHatchStyle"], properties[propertyPrefix + "ShadowOffset"] ?? ((object)2), properties[propertyPrefix + "BorderColor"], properties[propertyPrefix + "BorderStyle"] ?? ((object)BorderStyles.Solid), properties[propertyPrefix + "BorderWidth"], null, null, null, null);
			if (num > 0)
			{
				pointer.Thermometer = thermometer;
			}
		}

		private void UpgradeDundasCRIGaugePointerRadial(RadialPointer pointer, Hashtable properties, string propertyPrefix, Hashtable formulaProperties, Hashtable dataValueProperties)
		{
			this.UpgradeDundasCRIGaugePointer(pointer, properties, propertyPrefix, 15.0, 10.0, formulaProperties, dataValueProperties);
			pointer.MarkerStyle = new ReportExpression<MarkerStyles>(this.ConvertDundasCRIStringProperty(MarkerStyles.Diamond.ToString(), properties[propertyPrefix + "MarkerStyle"]), CultureInfo.InvariantCulture);
			pointer.Type = new ReportExpression<RadialPointerTypes>(this.ConvertDundasCRIStringProperty(properties[propertyPrefix + "Type"]), CultureInfo.InvariantCulture);
			pointer.Placement = new ReportExpression<Placements>(this.ConvertDundasCRIStringProperty(Placements.Cross.ToString(), properties[propertyPrefix + "Placement"]), CultureInfo.InvariantCulture);
			pointer.NeedleStyle = this.ConvertDundasCRIGaugeNeedleStyles(this.ConvertDundasCRIStringProperty(properties[propertyPrefix + "NeedleStyle"]));
			int num = 0;
			PointerCap pointerCap = new PointerCap();
			pointerCap.Width = this.ConvertDundasCRIDoubleReportExpressionProperty(pointerCap.Width, properties[propertyPrefix + "CapWidth"], ref num);
			pointerCap.CapStyle = this.ConvertDundasCRIGaugeCapStyle(this.ConvertDundasCRIStringProperty(properties[propertyPrefix + "CapStyle"], ref num));
			bool? nullable = this.ConvertDundasCRIBoolProperty(properties[propertyPrefix + "CapVisible"], ref num);
			if (nullable.HasValue)
			{
				pointerCap.Hidden = !nullable.Value;
			}
			bool? nullable2 = this.ConvertDundasCRIBoolProperty(properties[propertyPrefix + "CapOnTop"], ref num);
			if (nullable2.HasValue)
			{
				pointerCap.OnTop = nullable2.Value;
			}
			else
			{
				pointerCap.OnTop = true;
			}
			bool? nullable3 = this.ConvertDundasCRIBoolProperty(properties[propertyPrefix + "CapReflection"], ref num);
			if (nullable3.HasValue)
			{
				pointerCap.Reflection = nullable3.Value;
			}
			pointerCap.Style = this.ConvertDundasCRIStyleProperty(null, properties[propertyPrefix + "CapFillColor"] ?? ((object)Color.Gainsboro), properties[propertyPrefix + "CapFillGradientType"] ?? ((object)BackgroundGradients.DiagonalLeft), properties[propertyPrefix + "CapFillGradientEndColor"] ?? ((object)Color.DimGray), properties[propertyPrefix + "CapFillHatchStyle"], null, null, null, null, null, null, null, null, ref num);
			pointer.Style = this.ConvertDundasCRIStyleProperty(null, properties[propertyPrefix + "FillColor"] ?? ((object)Color.White), properties[propertyPrefix + "FillGradientType"] ?? ((object)BackgroundGradients.LeftRight), properties[propertyPrefix + "FillGradientEndColor"] ?? ((object)Color.Red), properties[propertyPrefix + "FillHatchStyle"], properties[propertyPrefix + "ShadowOffset"] ?? ((object)2), properties[propertyPrefix + "BorderColor"], properties[propertyPrefix + "BorderStyle"] ?? ((object)BorderStyles.Solid), properties[propertyPrefix + "BorderWidth"], null, null, null, null);
			if (num > 0)
			{
				pointer.PointerCap = pointerCap;
			}
		}

		private GaugeInputValue UpgradeDundasCRIGaugeInputValue(Hashtable formulaProperties, Hashtable dataValueProperties, string dataValuePropertyKey, Hashtable customProperties, string customPropertyKey)
		{
			GaugeInputValue gaugeInputValue = null;
			if (formulaProperties.Contains(dataValuePropertyKey))
			{
				if (gaugeInputValue == null)
				{
					gaugeInputValue = new GaugeInputValue();
				}
				string text = this.ConvertDundasCRIStringProperty(formulaProperties[dataValuePropertyKey]);
				int num = text.IndexOf('(');
				int num2 = text.LastIndexOf(')');
				string text2 = (num > -1 && num < num2) ? text.Substring(num + 1, num2 - num - 1) : string.Empty;
				string text3 = (num > -1) ? text.Remove(num) : text;
				switch (text3.ToUpperInvariant())
				{
				case "CALCULATEDVALUEAVERAGE":
					gaugeInputValue.Formula = FormulaTypes.Average;
					break;
				case "CALCULATEDVALUELINEAR":
					gaugeInputValue.Formula = FormulaTypes.Linear;
					break;
				case "CALCULATEDVALUEMAX":
					gaugeInputValue.Formula = FormulaTypes.Max;
					break;
				case "CALCULATEDVALUEMIN":
					gaugeInputValue.Formula = FormulaTypes.Min;
					break;
				case "MEDIAN":
					gaugeInputValue.Formula = FormulaTypes.Median;
					break;
				case "OPENCLOSE":
					gaugeInputValue.Formula = FormulaTypes.OpenClose;
					break;
				case "PERCENTILE":
					gaugeInputValue.Formula = FormulaTypes.Percentile;
					break;
				case "VARIANCE":
					gaugeInputValue.Formula = FormulaTypes.Variance;
					break;
				case "CALCULATEDVALUERATEOFCHANGE":
					gaugeInputValue.Formula = FormulaTypes.RateOfChange;
					break;
				case "CALCULATEDVALUEINTEGRAL":
					gaugeInputValue.Formula = FormulaTypes.Integral;
					break;
				}
				if (!string.IsNullOrEmpty(text2))
				{
					string[] array = text2.Split(',');
					if (gaugeInputValue.Formula == FormulaTypes.Percentile)
					{
						if (array.Length > 0)
						{
							gaugeInputValue.MinPercent = new ReportExpression<double>(array[0], CultureInfo.InvariantCulture);
						}
						if (array.Length > 1)
						{
							gaugeInputValue.MaxPercent = new ReportExpression<double>(array[1], CultureInfo.InvariantCulture);
						}
					}
					else if (gaugeInputValue.Formula == FormulaTypes.Linear)
					{
						if (array.Length > 0)
						{
							gaugeInputValue.Multiplier = new ReportExpression<double>(array[0], CultureInfo.InvariantCulture);
						}
						if (array.Length > 1)
						{
							gaugeInputValue.AddConstant = new ReportExpression<double>(array[1], CultureInfo.InvariantCulture);
						}
					}
				}
			}
			if (dataValueProperties.Contains(dataValuePropertyKey))
			{
				if (gaugeInputValue == null)
				{
					gaugeInputValue = new GaugeInputValue();
				}
				gaugeInputValue.Value = this.ConvertDundasCRIStringProperty(dataValueProperties[dataValuePropertyKey]);
			}
			else if (customProperties.Contains(customPropertyKey))
			{
				if (gaugeInputValue == null)
				{
					gaugeInputValue = new GaugeInputValue();
				}
				gaugeInputValue.Value = this.ConvertDundasCRIStringProperty(customProperties[customPropertyKey]);
			}
			return gaugeInputValue;
		}

		private ActionInfo UpgradeDundasCRIGaugeActionInfo(Hashtable formulaProperties, string propertyPrefix)
		{
			return this.UpgradeDundasCRIActionInfo(formulaProperties, propertyPrefix, "Href");
		}

		private ActionInfo UpgradeDundasCRIActionInfo(Hashtable properties, string propertyPrefix, string hyperLinkKey)
		{
			ActionInfo actionInfo = null;
			object obj = properties[propertyPrefix + hyperLinkKey];
			if (obj != null)
			{
				string text = this.ConvertDundasCRIStringProperty(properties[propertyPrefix + "MapAreaType"]);
				actionInfo = new ActionInfo();
				AspNetCore.ReportingServices.RdlObjectModel.Action action = new AspNetCore.ReportingServices.RdlObjectModel.Action();
				actionInfo.Actions.Add(action);
				switch (text)
				{
				case "Url":
					action.Hyperlink = this.ConvertDundasCRIStringProperty(obj);
					break;
				case "Bookmark":
					action.BookmarkLink = this.ConvertDundasCRIStringProperty(obj);
					break;
				case "Report":
				{
					action.Drillthrough = new Drillthrough();
					action.Drillthrough.ReportName = this.ConvertDundasCRIStringProperty(obj);
					string text2 = propertyPrefix + "REPORTPARAM:";
					IDictionaryEnumerator enumerator = properties.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
							if (dictionaryEntry.Key.ToString().StartsWith(text2, StringComparison.Ordinal))
							{
								Parameter parameter = new Parameter();
								parameter.Name = dictionaryEntry.Key.ToString().Remove(0, text2.Length);
								parameter.Value = dictionaryEntry.Value.ToString();
								action.Drillthrough.Parameters.Add(parameter);
							}
						}
						return actionInfo;
					}
					finally
					{
						IDisposable disposable = enumerator as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
				}
				}
			}
			return actionInfo;
		}

		private void FixGaugeElementNames(GaugePanel gaugePanel)
		{
			StringCollection stringCollection = new StringCollection();
			OrderedDictionary orderedDictionary = new OrderedDictionary(gaugePanel.RadialGauges.Count);
			OrderedDictionary orderedDictionary2 = new OrderedDictionary(gaugePanel.LinearGauges.Count);
			OrderedDictionary orderedDictionary3 = new OrderedDictionary(gaugePanel.GaugeLabels.Count);
			foreach (RadialGauge radialGauge in gaugePanel.RadialGauges)
			{
				string text = this.CreateNewName(stringCollection, radialGauge.Name, "Default");
				stringCollection.Add(text);
				if (!orderedDictionary.Contains(radialGauge.Name))
				{
					orderedDictionary.Add(radialGauge.Name, text);
				}
				radialGauge.Name = text;
				this.FixGaugeSubElementNames(radialGauge);
			}
			stringCollection.Clear();
			foreach (LinearGauge linearGauge in gaugePanel.LinearGauges)
			{
				string text2 = this.CreateNewName(stringCollection, linearGauge.Name, "Default");
				stringCollection.Add(text2);
				if (!orderedDictionary2.Contains(linearGauge.Name))
				{
					orderedDictionary2.Add(linearGauge.Name, text2);
				}
				linearGauge.Name = text2;
				this.FixGaugeSubElementNames(linearGauge);
			}
			stringCollection.Clear();
			foreach (GaugeLabel gaugeLabel in gaugePanel.GaugeLabels)
			{
				string text3 = this.CreateNewName(stringCollection, gaugeLabel.Name, "Default");
				stringCollection.Add(text3);
				if (!orderedDictionary3.Contains(gaugeLabel.Name))
				{
					orderedDictionary3.Add(gaugeLabel.Name, text3);
				}
				gaugeLabel.Name = text3;
			}
			foreach (RadialGauge radialGauge2 in gaugePanel.RadialGauges)
			{
				this.FixGaugeElementParentItemNames(radialGauge2, orderedDictionary, orderedDictionary2, orderedDictionary3);
			}
			foreach (LinearGauge linearGauge2 in gaugePanel.LinearGauges)
			{
				this.FixGaugeElementParentItemNames(linearGauge2, orderedDictionary, orderedDictionary2, orderedDictionary3);
			}
			foreach (GaugeLabel gaugeLabel2 in gaugePanel.GaugeLabels)
			{
				this.FixGaugeElementParentItemNames(gaugeLabel2, orderedDictionary, orderedDictionary2, orderedDictionary3);
			}
		}

		private void FixGaugeSubElementNames(Gauge gauge)
		{
			StringCollection stringCollection = new StringCollection();
			foreach (GaugeScale gaugeScale in gauge.GaugeScales)
			{
				gaugeScale.Name = this.CreateNewName(stringCollection, gaugeScale.Name, "Default");
				stringCollection.Add(gaugeScale.Name);
				StringCollection stringCollection2 = new StringCollection();
				foreach (ScaleRange scaleRange in gaugeScale.ScaleRanges)
				{
					scaleRange.Name = this.CreateNewName(stringCollection2, scaleRange.Name, "Default");
					stringCollection2.Add(scaleRange.Name);
				}
				stringCollection2.Clear();
				foreach (GaugePointer gaugePointer in gaugeScale.GaugePointers)
				{
					gaugePointer.Name = this.CreateNewName(stringCollection2, gaugePointer.Name, "Default");
					stringCollection2.Add(gaugePointer.Name);
				}
				stringCollection2.Clear();
				foreach (CustomLabel customLabel in gaugeScale.CustomLabels)
				{
					customLabel.Name = this.CreateNewName(stringCollection2, customLabel.Name, "Default");
					stringCollection2.Add(customLabel.Name);
				}
			}
		}

		private void FixGaugeElementParentItemNames(GaugePanelItem gaugeElement, OrderedDictionary radialGaugeNameMapping, OrderedDictionary linearGaugeNameMapping, OrderedDictionary gaugeLabelNameMapping)
		{
			string text = string.Empty;
			if (gaugeElement.ParentItem.StartsWith("CircularGauges.", StringComparison.Ordinal))
			{
				text = this.GetNewName(radialGaugeNameMapping, gaugeElement.ParentItem.Substring("CircularGauges.".Length));
				if (!string.IsNullOrEmpty(text))
				{
					text = "RadialGauges." + text;
				}
			}
			else if (gaugeElement.ParentItem.StartsWith("LinearGauges.", StringComparison.Ordinal))
			{
				text = this.GetNewName(linearGaugeNameMapping, gaugeElement.ParentItem.Substring("LinearGauges.".Length));
				if (!string.IsNullOrEmpty(text))
				{
					text = "LinearGauges." + text;
				}
			}
			else if (gaugeElement is GaugeLabel && gaugeElement.ParentItem.StartsWith("GaugeLabels.", StringComparison.Ordinal))
			{
				text = this.GetNewName(gaugeLabelNameMapping, gaugeElement.ParentItem.Substring("GaugeLabels.".Length));
				if (!string.IsNullOrEmpty(text))
				{
					text = "GaugeLabels." + text;
				}
			}
			gaugeElement.ParentItem = text;
		}

		private CapStyles ConvertDundasCRIGaugeCapStyle(string capStyle)
		{
			if (capStyle == "CustomCap1")
			{
				return CapStyles.Rounded;
			}
			if (capStyle == "CustomCap2")
			{
				return CapStyles.RoundedLight;
			}
			if (capStyle == "CustomCap3")
			{
				return CapStyles.RoundedWithAdditionalTop;
			}
			if (capStyle == "CustomCap4")
			{
				return CapStyles.RoundedWithWideIndentation;
			}
			if (capStyle == "CustomCap5")
			{
				return CapStyles.FlattenedWithIndentation;
			}
			if (capStyle == "CustomCap6")
			{
				return CapStyles.FlattenedWithWideIndentation;
			}
			if (capStyle == "CustomCap7")
			{
				return CapStyles.RoundedGlossyWithIndentation;
			}
			if (capStyle == "CustomCap8")
			{
				return CapStyles.RoundedWithIndentation;
			}
			return CapStyles.RoundedDark;
		}

		private NeedleStyles ConvertDundasCRIGaugeNeedleStyles(string needleStyle)
		{
			if (needleStyle == "NeedleStyle2")
			{
				return NeedleStyles.Rectangular;
			}
			if (needleStyle == "NeedleStyle3")
			{
				return NeedleStyles.TaperedWithTail;
			}
			if (needleStyle == "NeedleStyle4")
			{
				return NeedleStyles.Tapered;
			}
			if (needleStyle == "NeedleStyle5")
			{
				return NeedleStyles.ArrowWithTail;
			}
			if (needleStyle == "NeedleStyle6")
			{
				return NeedleStyles.Arrow;
			}
			if (needleStyle == "NeedleStyle7")
			{
				return NeedleStyles.StealthArrowWithTail;
			}
			if (needleStyle == "NeedleStyle8")
			{
				return NeedleStyles.StealthArrow;
			}
			if (needleStyle == "NeedleStyle9")
			{
				return NeedleStyles.TaperedWithStealthArrow;
			}
			if (needleStyle == "NeedleStyle10")
			{
				return NeedleStyles.StealthArrowWithWideTail;
			}
			if (needleStyle == "NeedleStyle11")
			{
				return NeedleStyles.TaperedWithRoundedPoint;
			}
			return NeedleStyles.Triangular;
		}

		private string GetNewName(OrderedDictionary oldAndNewNameMapping, string oldName)
		{
			if (oldAndNewNameMapping.Contains(oldName))
			{
				return oldAndNewNameMapping[oldName].ToString();
			}
			return string.Empty;
		}

		private string CreateNewName(StringCollection newNamesCollection, string oldName, string defaultNewName)
		{
			int num = 1;
			string text = (oldName.Trim() == string.Empty) ? (defaultNewName + num.ToString(CultureInfo.InvariantCulture)) : StringUtil.GetClsCompliantIdentifier(oldName, "chart");
			if (newNamesCollection.Contains(text))
			{
				while (newNamesCollection.Contains(text + "_" + num.ToString(CultureInfo.InvariantCulture)))
				{
					num++;
				}
				text = text + "_" + num.ToString(CultureInfo.InvariantCulture);
			}
			return text;
		}

		private Font FontFromString(string fontString)
		{
			string text = fontString;
			byte b = 1;
			bool flag = false;
			int num = fontString.IndexOf(", GdiCharSet=", StringComparison.Ordinal);
			if (num >= 0)
			{
				string text2 = fontString.Substring(num + 13);
				int num2 = text2.IndexOf(',');
				if (num2 >= 0)
				{
					text2 = text2.Substring(0, num2);
				}
				b = (byte)int.Parse(text2, CultureInfo.InvariantCulture);
				if (text.Length > num)
				{
					text = text.Substring(0, num);
				}
			}
			num = fontString.IndexOf(", GdiVerticalFont", StringComparison.Ordinal);
			if (num >= 0)
			{
				flag = true;
				if (text.Length > num)
				{
					text = text.Substring(0, num);
				}
			}
			FontConverter fontConverter = new FontConverter();
			Font font = (Font)fontConverter.ConvertFromInvariantString(text);
			float sizeInPoints = font.SizeInPoints;
			sizeInPoints = Math.Min(Math.Max(font.SizeInPoints, (float)AspNetCore.ReportingServices.RdlObjectModel.Constants.MinimumFontSize.ToPoints()), (float)AspNetCore.ReportingServices.RdlObjectModel.Constants.MaximumFontSize.ToPoints());
			if (!flag && b == 1 && sizeInPoints == font.SizeInPoints)
			{
				return font;
			}
			Font result = new Font(font.Name, sizeInPoints, font.Style, GraphicsUnit.Point, b, flag);
			font.Dispose();
			return result;
		}

		private bool AddToPropertyList(List<Hashtable> propertyList, string counterPrefix, string key, ReportExpression value)
		{
			if (!key.StartsWith(counterPrefix, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (!counterPrefix.EndsWith(".", StringComparison.Ordinal))
			{
				counterPrefix += ".";
			}
			key = key.Substring(counterPrefix.Length);
			int num = key.IndexOf('.');
			int num2 = 0;
			if (!int.TryParse(key.Substring(0, num), out num2))
			{
				return false;
			}
			key = key.Substring(num + 1);
			while (num2 >= propertyList.Count)
			{
				propertyList.Add(new Hashtable());
			}
			propertyList[num2].Add(key, value);
			return true;
		}

		private bool IsZero(object value)
		{
			if (value == null)
			{
				return false;
			}
			double num = default(double);
			if (double.TryParse(value.ToString(), out num) && num == 0.0)
			{
				return true;
			}
			return false;
		}

		private void ConvertDundasCRICustomProperties(IList<CustomProperty> customProperties, object property)
		{
			int num = 0;
			this.ConvertDundasCRICustomProperties(customProperties, property, ref num);
		}

		private void ConvertDundasCRICustomProperties(IList<CustomProperty> customProperties, object property, ref int counter)
		{
			if (property != null)
			{
				counter++;
				if (customProperties != null)
				{
					customProperties.Clear();
				}
				else
				{
					customProperties = new List<CustomProperty>();
				}
				string[] array = property.ToString().Replace("\\,", "\\x45").Replace("\\=", "\\x46")
					.Split(new char[1]
					{
						','
					}, StringSplitOptions.RemoveEmptyEntries);
				string[] array2 = array;
				foreach (string text in array2)
				{
					int num = text.IndexOf('=');
					CustomProperty customProperty = new CustomProperty();
					customProperty.Name = text.Substring(0, num).Trim();
					customProperty.Value = text.Substring(num + 1).Replace("\\x45", ",").Replace("\\x46", "=")
						.Trim();
					customProperties.Add(customProperty);
				}
			}
		}

		private ReportExpression<ReportColor> ConvertDundasCRIColorProperty(string defaultValue, object color)
		{
			return this.ConvertDundasCRIColorProperty(new ReportExpression<ReportColor>(defaultValue, CultureInfo.InvariantCulture), color);
		}

		private ReportExpression<ReportColor> ConvertDundasCRIColorProperty(ReportExpression<ReportColor> defaultValue, object color)
		{
			int num = 0;
			return this.ConvertDundasCRIColorProperty(defaultValue, color, ref num);
		}

		private ReportExpression<ReportColor> ConvertDundasCRIColorProperty(ReportExpression<ReportColor> defaultValue, object color, ref int counter)
		{
			if (color != null)
			{
				counter++;
				if (color is ReportExpression<ReportColor>)
				{
					return (ReportExpression<ReportColor>)color;
				}
				ColorConverter colorConverter = new ColorConverter();
				try
				{
					Color color2 = (color is Color) ? ((Color)color) : ((Color)colorConverter.ConvertFromInvariantString(color.ToString()));
					/*if (color2.IsSystemColor)
					{
						return new ReportExpression<ReportColor>(new ReportColor(Color.FromArgb(color2.ToArgb())));
					}
                    */
					return new ReportExpression<ReportColor>(new ReportColor(color2));
				}
				catch
				{
					try
					{
						return new ReportExpression<ReportColor>(color.ToString(), CultureInfo.InvariantCulture);
					}
					catch
					{
						return defaultValue;
					}
				}
			}
			return defaultValue;
		}

		private bool? ConvertDundasCRIBoolProperty(object property)
		{
			int num = 0;
			return this.ConvertDundasCRIBoolProperty(property, ref num);
		}

		private bool? ConvertDundasCRIBoolProperty(object property, ref int counter)
		{
			bool value = default(bool);
			if (property != null && bool.TryParse(property.ToString(), out value))
			{
				counter++;
				return value;
			}
			return null;
		}

		private string ConvertDundasCRIStringProperty(object property)
		{
			int num = 0;
			return this.ConvertDundasCRIStringProperty(string.Empty, property, ref num);
		}

		private string ConvertDundasCRIStringProperty(string defaultValue, object property)
		{
			int num = 0;
			return this.ConvertDundasCRIStringProperty(defaultValue, property, ref num);
		}

		private string ConvertDundasCRIStringProperty(object property, ref int counter)
		{
			return this.ConvertDundasCRIStringProperty(string.Empty, property, ref counter);
		}

		private string ConvertDundasCRIStringProperty(string defaultValue, object property, ref int counter)
		{
			if (property != null)
			{
				counter++;
				return property.ToString();
			}
			return defaultValue;
		}

		private ReportExpression<int> ConvertDundasCRIIntegerReportExpressionProperty(object property)
		{
			int num = 0;
			return this.ConvertDundasCRIIntegerReportExpressionProperty(null, property, ref num);
		}

		private ReportExpression<int> ConvertDundasCRIIntegerReportExpressionProperty(object property, ref int counter)
		{
			return this.ConvertDundasCRIIntegerReportExpressionProperty((ReportExpression<int>)null, property, ref counter);
		}

		private ReportExpression<int> ConvertDundasCRIIntegerReportExpressionProperty(ReportExpression<int> defaultValue, object property)
		{
			int num = 0;
			return this.ConvertDundasCRIIntegerReportExpressionProperty(defaultValue, property, ref num);
		}

		private ReportExpression<int> ConvertDundasCRIIntegerReportExpressionProperty(ReportExpression<int> defaultValue, object property, ref int counter)
		{
			if (property != null)
			{
				counter++;
				return new ReportExpression<int>(property.ToString(), CultureInfo.InvariantCulture.NumberFormat);
			}
			return defaultValue;
		}

		private ReportExpression<double> ConvertDundasCRIDoubleReportExpressionProperty(object property)
		{
			int num = 0;
			return this.ConvertDundasCRIDoubleReportExpressionProperty(null, property, ref num);
		}

		private ReportExpression<double> ConvertDundasCRIDoubleReportExpressionProperty(ReportExpression<double> defaultValue, object property)
		{
			int num = 0;
			return this.ConvertDundasCRIDoubleReportExpressionProperty(defaultValue, property, ref num);
		}

		private ReportExpression<double> ConvertDundasCRIDoubleReportExpressionProperty(object property, ref int counter)
		{
			return this.ConvertDundasCRIDoubleReportExpressionProperty((ReportExpression<double>)null, property, ref counter);
		}

		private ReportExpression<double> ConvertDundasCRIDoubleReportExpressionProperty(ReportExpression<double> defaultValue, object property, ref int counter)
		{
			if (property != null)
			{
				string a = property.ToString();
				counter++;
				if (a == "Auto")
				{
					return new ReportExpression<double>(double.NaN);
				}
				return new ReportExpression<double>(property.ToString(), CultureInfo.InvariantCulture.NumberFormat);
			}
			return defaultValue;
		}

		private ReportExpression<ReportSize> ConvertDundasCRIPointReportSizeProperty(ReportExpression<ReportSize> defaultValue, object property, ref int counter)
		{
			if (property != null)
			{
				string text = property.ToString();
				counter++;
				if (property is ReportExpression && ((ReportExpression)property).IsExpression)
				{
					return new ReportExpression<ReportSize>(text, CultureInfo.InvariantCulture);
				}
				double value = 0.0;
				if (!double.TryParse(text, out value))
				{
					return defaultValue;
				}
				return new ReportExpression<ReportSize>(new ReportSize(value, SizeTypes.Point));
			}
			return defaultValue;
		}

		private ReportExpression<ReportSize> ConvertDundasCRIPointReportSizeProperty(ReportExpression<ReportSize> defaultValue, object property)
		{
			int num = 0;
			return this.ConvertDundasCRIPointReportSizeProperty(defaultValue, property, ref num);
		}

		private ReportExpression<ReportSize> ConvertDundasCRIPixelReportSizeProperty(double? defaultValue, object property, ref int counter)
		{
			if (property != null)
			{
				string text = property.ToString();
				counter++;
				if (property is ReportExpression && ((ReportExpression)property).IsExpression)
				{
					text = text.Substring(text.IndexOf('=') + 1);
					string value = "=CStr(({0})*{1})&\"pt\"".Replace("{1}", 0.75.ToString(CultureInfo.InvariantCulture.NumberFormat)).Replace("{0}", text);
					return new ReportExpression<ReportSize>(value, CultureInfo.InvariantCulture);
				}
				double num = 0.0;
				if (double.TryParse(text, out num))
				{
					return new ReportExpression<ReportSize>(new ReportSize(num * 0.75, SizeTypes.Point));
				}
			}
			if (!defaultValue.HasValue)
			{
				return default(ReportExpression<ReportSize>);
			}
			return new ReportExpression<ReportSize>(new ReportSize(defaultValue.Value * 0.75, SizeTypes.Point));
		}

		private ReportExpression<ReportSize> ConvertDundasCRIPixelReportSizeProperty(object property, ref int counter)
		{
			return this.ConvertDundasCRIPixelReportSizeProperty((double?)null, property, ref counter);
		}

		private ReportExpression<ReportSize> ConvertDundasCRIPixelReportSizeProperty(object property)
		{
			int num = 0;
			return this.ConvertDundasCRIPixelReportSizeProperty(null, property, ref num);
		}

		private ActionInfo ConvertDundasCRIActionInfoProperty(object hyperlink)
		{
			int num = 0;
			return this.ConvertDundasCRIActionInfoProperty(hyperlink, ref num);
		}

		private ActionInfo ConvertDundasCRIActionInfoProperty(object hyperlink, ref int counter)
		{
			if (hyperlink != null && hyperlink.ToString() != string.Empty)
			{
				AspNetCore.ReportingServices.RdlObjectModel.Action action = new AspNetCore.ReportingServices.RdlObjectModel.Action();
				action.Hyperlink = hyperlink.ToString();
				ActionInfo actionInfo = new ActionInfo();
				actionInfo.Actions.Add(action);
				counter++;
				return actionInfo;
			}
			return null;
		}

		private ChartElementPosition ConvertDundasCRIChartElementPosition(object top, object left, object height, object width)
		{
			int num = 0;
			return this.ConvertDundasCRIChartElementPosition(top, left, height, width, ref num);
		}

		private ChartElementPosition ConvertDundasCRIChartElementPosition(object top, object left, object height, object width, ref int counter)
		{
			int num = 0;
			ChartElementPosition chartElementPosition = new ChartElementPosition();
			chartElementPosition.Top = this.ConvertDundasCRIDoubleReportExpressionProperty(top, ref num);
			chartElementPosition.Left = this.ConvertDundasCRIDoubleReportExpressionProperty(left, ref num);
			chartElementPosition.Height = this.ConvertDundasCRIDoubleReportExpressionProperty(height, ref num);
			chartElementPosition.Width = this.ConvertDundasCRIDoubleReportExpressionProperty(width, ref num);
			if (num > 0)
			{
				counter++;
				return chartElementPosition;
			}
			return null;
		}

		private AspNetCore.ReportingServices.RdlObjectModel.Style ConvertDundasCRIStyleProperty(object color, object backgroundColor, object backgroundGradientType, object backgroundGradientEndColor, object backgroundHatchType, object shadowOffset, object borderColor, object borderStyle, object borderWidth, object font, object format, object textAlign, object textVerticalAlign)
		{
			int num = 0;
			return (AspNetCore.ReportingServices.RdlObjectModel.Style)this.ConvertDundasCRIStyleProperty(color, backgroundColor, backgroundGradientType, backgroundGradientEndColor, backgroundHatchType, null, shadowOffset, borderColor, borderStyle, borderWidth, null, null, null, null, font, format, null, textAlign, textVerticalAlign, ref num, new AspNetCore.ReportingServices.RdlObjectModel.Style(), new Border());
		}

		private AspNetCore.ReportingServices.RdlObjectModel.Style ConvertDundasCRIStyleProperty(object color, object backgroundColor, object backgroundGradientType, object backgroundGradientEndColor, object backgroundHatchType, object shadowOffset, object borderColor, object borderStyle, object borderWidth, object font, object format, object textAlign, object textVerticalAlign, ref int counter)
		{
			return (AspNetCore.ReportingServices.RdlObjectModel.Style)this.ConvertDundasCRIStyleProperty(color, backgroundColor, backgroundGradientType, backgroundGradientEndColor, backgroundHatchType, (object)null, shadowOffset, borderColor, borderStyle, borderWidth, (object)null, (object)null, (object)null, (object)null, font, format, (object)null, textAlign, textVerticalAlign, ref counter, new AspNetCore.ReportingServices.RdlObjectModel.Style(), new Border());
		}

		private AspNetCore.ReportingServices.RdlObjectModel.Style ConvertDundasCRIStyleProperty(object color, object backgroundColor, object backgroundGradientType, object backgroundGradientEndColor, object backgroundHatchType, object shadowColor, object shadowOffset, object borderColor, object borderStyle, object borderWidth, object imageReference, object imageTransColor, object imageAlign, object imageMode, object font, object format, object textEffect, object textAlign, object textVerticalAlign)
		{
			int num = 0;
			return (AspNetCore.ReportingServices.RdlObjectModel.Style)this.ConvertDundasCRIStyleProperty(color, backgroundColor, backgroundGradientType, backgroundGradientEndColor, backgroundHatchType, shadowColor, shadowOffset, borderColor, borderStyle, borderWidth, imageReference, imageTransColor, imageAlign, imageMode, font, format, textEffect, textAlign, textVerticalAlign, ref num, new AspNetCore.ReportingServices.RdlObjectModel.Style(), new Border());
		}

		private AspNetCore.ReportingServices.RdlObjectModel.Style ConvertDundasCRIStyleProperty(object color, object backgroundColor, object backgroundGradientType, object backgroundGradientEndColor, object backgroundHatchType, object shadowColor, object shadowOffset, object borderColor, object borderStyle, object borderWidth, object imageReference, object imageTransColor, object imageAlign, object imageMode, object font, object format, object textEffect, object textAlign, object textVerticalAlign, ref int counter)
		{
			return (AspNetCore.ReportingServices.RdlObjectModel.Style)this.ConvertDundasCRIStyleProperty(color, backgroundColor, backgroundGradientType, backgroundGradientEndColor, backgroundHatchType, shadowColor, shadowOffset, borderColor, borderStyle, borderWidth, imageReference, imageTransColor, imageAlign, imageMode, font, format, textEffect, textAlign, textVerticalAlign, ref counter, new AspNetCore.ReportingServices.RdlObjectModel.Style(), new Border());
		}

		private EmptyColorStyle ConvertDundasCRIEmptyColorStyleProperty(object color, object backgroundColor, object backgroundGradientType, object backgroundGradientEndColor, object backgroundHatchType, object borderColor, object borderStyle, object borderWidth, object imageReference, object imageTransColor, object imageAlign, object imageMode, object font, object format, ref int counter)
		{
			return (EmptyColorStyle)this.ConvertDundasCRIStyleProperty(color, backgroundColor, backgroundGradientType, backgroundGradientEndColor, backgroundHatchType, (object)null, (object)null, borderColor, borderStyle, borderWidth, imageReference, imageTransColor, imageAlign, imageMode, font, format, (object)null, (object)null, (object)null, ref counter, (AspNetCore.ReportingServices.RdlObjectModel.Style)new EmptyColorStyle(), (Border)new EmptyBorder());
		}

		private object ConvertDundasCRIStyleProperty(object color, object backgroundColor, object backgroundGradientType, object backgroundGradientEndColor, object backgroundHatchType, object shadowColor, object shadowOffset, object borderColor, object borderStyle, object borderWidth, object imageReference, object imageTransColor, object imageAlign, object imageMode, object font, object format, object textEffect, object textAlign, object textVerticalAlign, ref int counter, AspNetCore.ReportingServices.RdlObjectModel.Style style, Border border)
		{
			int num = 0;
			style.Color = this.ConvertDundasCRIColorProperty(style.Color, color, ref num);
			style.BackgroundColor = this.ConvertDundasCRIColorProperty(style.BackgroundColor, backgroundColor, ref num);
			style.BackgroundGradientEndColor = this.ConvertDundasCRIColorProperty(style.BackgroundGradientEndColor, backgroundGradientEndColor, ref num);
			style.ShadowColor = this.ConvertDundasCRIColorProperty(style.ShadowColor, shadowColor, ref num);
			style.ShadowOffset = this.ConvertDundasCRIPixelReportSizeProperty(shadowOffset, ref num);
			style.Format = this.ConvertDundasCRIStringProperty(format, ref num);
			style.TextEffect = new ReportExpression<TextEffects>(this.ConvertDundasCRIStringProperty(textEffect, ref num), CultureInfo.InvariantCulture);
			try
			{
				style.BackgroundGradientType = new ReportExpression<BackgroundGradients>(this.ConvertDundasCRIStringProperty(backgroundGradientType, ref num), CultureInfo.InvariantCulture);
			}
			catch
			{
			}
			try
			{
				style.BackgroundHatchType = new ReportExpression<BackgroundHatchTypes>(this.ConvertDundasCRIStringProperty(backgroundHatchType, ref num), CultureInfo.InvariantCulture);
			}
			catch
			{
			}
			style.TextAlign = new ReportExpression<TextAlignments>(this.ConvertDundasCRIStringProperty(textAlign, ref num), CultureInfo.InvariantCulture);
			style.VerticalAlign = new ReportExpression<VerticalAlignments>(this.ConvertDundasCRIStringProperty(textVerticalAlign, ref num), CultureInfo.InvariantCulture);
			int num2 = 0;
			border.Color = this.ConvertDundasCRIColorProperty(border.Color, borderColor, ref num2);
			border.Width = this.ConvertDundasCRIPixelReportSizeProperty(1.0, borderWidth, ref num2);
			string text = this.ConvertDundasCRIStringProperty(BorderStyles.Solid.ToString(), borderStyle, ref num2);
			switch (text)
			{
			case "NotSet":
				border.Style = BorderStyles.None;
				break;
			case "Dash":
				border.Style = BorderStyles.Dashed;
				break;
			case "Dot":
				border.Style = BorderStyles.Dotted;
				break;
			default:
				try
				{
					border.Style = new ReportExpression<BorderStyles>(text, CultureInfo.InvariantCulture);
				}
				catch
				{
					border.Style = BorderStyles.Solid;
				}
				break;
			}
			if (num2 > 0)
			{
				if (borderWidth != null && !border.Width.IsExpression)
				{
					if (border.Width.Value < AspNetCore.ReportingServices.RdlObjectModel.Constants.MinimumBorderWidth)
					{
						border.Width = AspNetCore.ReportingServices.RdlObjectModel.Constants.DefaultBorderWidth;
						border.Style = BorderStyles.None;
					}
					else if (border.Width.Value > AspNetCore.ReportingServices.RdlObjectModel.Constants.MaximumBorderWidth)
					{
						border.Width = AspNetCore.ReportingServices.RdlObjectModel.Constants.MaximumBorderWidth;
					}
				}
				if (style is EmptyColorStyle)
				{
					((EmptyColorStyle)style).Border = (EmptyBorder)border;
				}
				else
				{
					style.Border = border;
				}
				num++;
			}
			style.BackgroundImage = this.ConvertDundasCRIChartBackgroundImageProperty(imageReference, imageTransColor, imageAlign, imageMode, ref num);
			string text2 = this.ConvertDundasCRIStringProperty(font, ref num);
			if (text2 != string.Empty)
			{
				Font font2 = this.FontFromString(text2);
				style.FontFamily = font2.FontFamily.Name;
				style.FontSize = new ReportSize((double)font2.Size, SizeTypes.Point);
				if (font2.Bold)
				{
					style.FontWeight = FontWeights.Bold;
				}
				if (font2.Italic)
				{
					style.FontStyle = FontStyles.Italic;
				}
				if (font2.Strikeout)
				{
					style.TextDecoration = TextDecorations.LineThrough;
				}
				else if (font2.Underline)
				{
					style.TextDecoration = TextDecorations.Underline;
				}
			}
			if (num > 0)
			{
				counter++;
				return style;
			}
			return null;
		}
	}
}
