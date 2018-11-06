using AspNetCore.ReportingServices.Rendering.HPBProcessing;
using AspNetCore.ReportingServices.Rendering.RichText;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Resources;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal class Renderer : IDisposable
	{
		internal WriterBase Writer;

		internal RPLReport RplReport;

		internal int SharedItemsCount;

		internal string CurrentLanguage;

		internal bool PhysicalPagination;

		private int m_pageNumber;

		internal Dictionary<string, int> SharedItems = new Dictionary<string, int>(50);

		private Dictionary<string, float> m_cachedReportSizes = new Dictionary<string, float>();

		private Dictionary<string, float> m_cachedFontSizes = new Dictionary<string, float>();

		private Dictionary<string, Color> m_cachedReportColors = new Dictionary<string, Color>();

		private FontCache m_fontCache;

		private SectionItemizedData m_sectionItemizedData;

		private Dictionary<string, List<TextRunItemizedData>> m_pageParagraphsItemizedData;

		private bool m_beginPage;

		internal static Dictionary<string, Bitmap> ImageResources;

		private static ResourceManager ImageResourceManager;

		static Renderer()
		{
			Renderer.ImageResources = new Dictionary<string, Bitmap>(10);
            var assembly = Assembly.GetExecutingAssembly();
            try
            {
                Renderer.ImageResourceManager = new ResourceManager("AspNetCore.ReportingServices.Rendering.ImageRenderer.Images", assembly);
                Renderer.ImageResources.Add("toggleMinus", (Bitmap)Renderer.ImageResourceManager.GetObject("toggleMinus"));
                Renderer.ImageResources.Add("togglePlus", (Bitmap)Renderer.ImageResourceManager.GetObject("togglePlus"));
                Renderer.ImageResources.Add("unsorted", (Bitmap)Renderer.ImageResourceManager.GetObject("unsorted"));
                Renderer.ImageResources.Add("sortAsc", (Bitmap)Renderer.ImageResourceManager.GetObject("sortAsc"));
                Renderer.ImageResources.Add("sortDesc", (Bitmap)Renderer.ImageResourceManager.GetObject("sortDesc"));
                Renderer.ImageResources.Add("InvalidImage", (Bitmap)Renderer.ImageResourceManager.GetObject("InvalidImage"));
            }
            catch
            {
                AddImageSource(assembly, "toggleMinus");
                AddImageSource(assembly, "togglePlus");
                AddImageSource(assembly, "unsorted");
                AddImageSource(assembly, "sortAsc");
                AddImageSource(assembly, "sortDesc");
                AddImageSource(assembly, "InvalidImage", "bmp");
            }
        }
        const string ResourceNameFormat = "AspNetCore.Reporting.Resources.{0}.{1}";
        static void AddImageSource(Assembly assembly,string name,string ext="gif")
        {
            try
            {
                var image = System.Drawing.Image.FromStream(assembly.GetManifestResourceStream(string.Format(ResourceNameFormat, name, ext)));
                Renderer.ImageResources.Add(name, (Bitmap)image);
            }
            catch {
                }
        }
		internal Renderer(bool physicalPagination)
		{
			this.PhysicalPagination = physicalPagination;
		}

		internal Renderer(bool physicalPagination, FontCache fontCache)
			: this(physicalPagination)
		{
			this.m_fontCache = fontCache;
		}

		public void Dispose()
		{
			this.m_fontCache = null;
			if (this.Writer != null)
			{
				this.Writer.Dispose();
				this.Writer = null;
			}
			GC.SuppressFinalize(this);
		}

		private void CalculateUsableReportItemRectangle(RPLElementProps properties, ref RectangleF position)
		{
			float num = default(float);
			float num2 = default(float);
			float num3 = default(float);
			float num4 = default(float);
			this.GetReportItemPaddingStyleMM(properties, out num, out num2, out num3, out num4);
			position.X += num;
			position.Width -= num;
			position.Y += num2;
			position.Height -= num2;
			position.Width -= num3;
			position.Height -= num4;
		}

		private Color GetCachedReportColorStyle(RPLElementProps properties, byte style)
		{
			bool flag = false;
			string stylePropertyValueString = SharedRenderer.GetStylePropertyValueString(properties, style, ref flag);
			if (!string.IsNullOrEmpty(stylePropertyValueString) && string.Compare(stylePropertyValueString, "TRANSPARENT", StringComparison.OrdinalIgnoreCase) != 0)
			{
				Color color = default(Color);
				if (!this.m_cachedReportColors.TryGetValue(stylePropertyValueString, out color))
				{
					color = new RPLReportColor(stylePropertyValueString).ToColor();
					this.m_cachedReportColors.Add(stylePropertyValueString, color);
				}
				return color;
			}
			return Color.Empty;
		}

		private float GetCachedReportSizeStyleMM(RPLElementProps properties, byte style)
		{
			bool flag = false;
			string stylePropertyValueString = SharedRenderer.GetStylePropertyValueString(properties, style, ref flag);
			if (string.IsNullOrEmpty(stylePropertyValueString))
			{
				return float.NaN;
			}
			float num = default(float);
			if (!this.m_cachedReportSizes.TryGetValue(stylePropertyValueString, out num))
			{
				num = (float)new RPLReportSize(stylePropertyValueString).ToMillimeters();
				this.m_cachedReportSizes.Add(stylePropertyValueString, num);
			}
			return num;
		}

		private void GetReportItemPaddingStyleMM(RPLElementProps instanceProperties, out float paddingLeft, out float paddingTop, out float paddingRight, out float paddingBottom)
		{
			paddingLeft = this.GetCachedReportSizeStyleMM(instanceProperties, 15);
			paddingTop = this.GetCachedReportSizeStyleMM(instanceProperties, 17);
			paddingRight = this.GetCachedReportSizeStyleMM(instanceProperties, 16);
			paddingBottom = this.GetCachedReportSizeStyleMM(instanceProperties, 18);
		}

		private void ProcessBackgroundColorAndImage(RPLElementProps properties, RectangleF position, RectangleF bounds)
		{
			Color cachedReportColorStyle = this.GetCachedReportColorStyle(properties, 34);
			if (cachedReportColorStyle != Color.Empty)
			{
				this.Writer.FillRectangle(cachedReportColorStyle, bounds);
			}
			object stylePropertyValueObject = SharedRenderer.GetStylePropertyValueObject(properties, 33);
			if (stylePropertyValueObject != null)
			{
				object stylePropertyValueObject2 = SharedRenderer.GetStylePropertyValueObject(properties, 35);
				RPLFormat.BackgroundRepeatTypes repeat = (stylePropertyValueObject2 != null) ? ((RPLFormat.BackgroundRepeatTypes)stylePropertyValueObject2) : RPLFormat.BackgroundRepeatTypes.Repeat;
				this.Writer.DrawBackgroundImage((RPLImageData)stylePropertyValueObject, repeat, PointF.Empty, position);
			}
		}

		private List<Operation> ProcessPageBorders(RPLElementStyle style, RectangleF position, RectangleF bounds, ref bool inPageSection, RectangleF fullPageBounds)
		{
			RPLFormat.BorderStyles stylePropertyValueBorderStyle = SharedRenderer.GetStylePropertyValueBorderStyle(style, 5, RPLFormat.BorderStyles.None);
			RPLFormat.BorderStyles stylePropertyValueBorderStyle2 = SharedRenderer.GetStylePropertyValueBorderStyle(style, 6, stylePropertyValueBorderStyle);
			RPLFormat.BorderStyles stylePropertyValueBorderStyle3 = SharedRenderer.GetStylePropertyValueBorderStyle(style, 8, stylePropertyValueBorderStyle);
			RPLFormat.BorderStyles stylePropertyValueBorderStyle4 = SharedRenderer.GetStylePropertyValueBorderStyle(style, 7, stylePropertyValueBorderStyle);
			RPLFormat.BorderStyles stylePropertyValueBorderStyle5 = SharedRenderer.GetStylePropertyValueBorderStyle(style, 9, stylePropertyValueBorderStyle);
			if (stylePropertyValueBorderStyle2 == RPLFormat.BorderStyles.None && stylePropertyValueBorderStyle3 == RPLFormat.BorderStyles.None && stylePropertyValueBorderStyle4 == RPLFormat.BorderStyles.None && stylePropertyValueBorderStyle5 == RPLFormat.BorderStyles.None)
			{
				return null;
			}
			float reportSizeStyleMM = SharedRenderer.GetReportSizeStyleMM(style, 10);
			float num = SharedRenderer.GetReportSizeStyleMM(style, 11);
			if (float.IsNaN(num) && !float.IsNaN(reportSizeStyleMM))
			{
				num = reportSizeStyleMM;
			}
			float num2 = SharedRenderer.GetReportSizeStyleMM(style, 13);
			if (float.IsNaN(num2) && !float.IsNaN(reportSizeStyleMM))
			{
				num2 = reportSizeStyleMM;
			}
			float num3 = SharedRenderer.GetReportSizeStyleMM(style, 12);
			if (float.IsNaN(num3) && !float.IsNaN(reportSizeStyleMM))
			{
				num3 = reportSizeStyleMM;
			}
			float num4 = SharedRenderer.GetReportSizeStyleMM(style, 14);
			if (float.IsNaN(num4) && !float.IsNaN(reportSizeStyleMM))
			{
				num4 = reportSizeStyleMM;
			}
			if (float.IsNaN(num) && float.IsNaN(num2) && float.IsNaN(num3) && float.IsNaN(num4))
			{
				return null;
			}
			Color reportColorStyle = SharedRenderer.GetReportColorStyle(style, 0);
			Color color = SharedRenderer.GetReportColorStyle(style, 1);
			if (color == Color.Empty && reportColorStyle != Color.Empty)
			{
				color = reportColorStyle;
			}
			Color color2 = SharedRenderer.GetReportColorStyle(style, 3);
			if (color2 == Color.Empty && reportColorStyle != Color.Empty)
			{
				color2 = reportColorStyle;
			}
			Color color3 = SharedRenderer.GetReportColorStyle(style, 2);
			if (color3 == Color.Empty && reportColorStyle != Color.Empty)
			{
				color3 = reportColorStyle;
			}
			Color color4 = SharedRenderer.GetReportColorStyle(style, 4);
			if (color4 == Color.Empty && reportColorStyle != Color.Empty)
			{
				color4 = reportColorStyle;
			}
			if (color == Color.Empty && color2 == Color.Empty && color3 == Color.Empty && color4 == Color.Empty)
			{
				return null;
			}
			if (!inPageSection)
			{
				this.Writer.BeginPageSection(fullPageBounds);
				inPageSection = true;
			}
			bounds.X = fullPageBounds.X;
			bounds.Y = fullPageBounds.Y;
			position.X = fullPageBounds.X;
			position.Y = fullPageBounds.Y;
			return this.ProcessBorders(stylePropertyValueBorderStyle2, stylePropertyValueBorderStyle3, stylePropertyValueBorderStyle4, stylePropertyValueBorderStyle5, num, num2, num3, num4, color, color2, color3, color4, position, bounds, false, 0);
		}

		private List<Operation> ProcessBorders(RPLElementProps properties, RectangleF position, RectangleF bounds, bool renderBorders, byte state)
		{
			RPLFormat.BorderStyles stylePropertyValueBorderStyle = SharedRenderer.GetStylePropertyValueBorderStyle(properties, 5, RPLFormat.BorderStyles.None);
			RPLFormat.BorderStyles stylePropertyValueBorderStyle2 = SharedRenderer.GetStylePropertyValueBorderStyle(properties, 6, stylePropertyValueBorderStyle);
			RPLFormat.BorderStyles stylePropertyValueBorderStyle3 = SharedRenderer.GetStylePropertyValueBorderStyle(properties, 8, stylePropertyValueBorderStyle);
			RPLFormat.BorderStyles stylePropertyValueBorderStyle4 = SharedRenderer.GetStylePropertyValueBorderStyle(properties, 7, stylePropertyValueBorderStyle);
			RPLFormat.BorderStyles stylePropertyValueBorderStyle5 = SharedRenderer.GetStylePropertyValueBorderStyle(properties, 9, stylePropertyValueBorderStyle);
			if (stylePropertyValueBorderStyle2 == RPLFormat.BorderStyles.None && stylePropertyValueBorderStyle3 == RPLFormat.BorderStyles.None && stylePropertyValueBorderStyle4 == RPLFormat.BorderStyles.None && stylePropertyValueBorderStyle5 == RPLFormat.BorderStyles.None)
			{
				return null;
			}
			float cachedReportSizeStyleMM = this.GetCachedReportSizeStyleMM(properties, 10);
			float num = this.GetCachedReportSizeStyleMM(properties, 11);
			if (float.IsNaN(num) && !float.IsNaN(cachedReportSizeStyleMM))
			{
				num = cachedReportSizeStyleMM;
			}
			float num2 = this.GetCachedReportSizeStyleMM(properties, 13);
			if (float.IsNaN(num2) && !float.IsNaN(cachedReportSizeStyleMM))
			{
				num2 = cachedReportSizeStyleMM;
			}
			float num3 = this.GetCachedReportSizeStyleMM(properties, 12);
			if (float.IsNaN(num3) && !float.IsNaN(cachedReportSizeStyleMM))
			{
				num3 = cachedReportSizeStyleMM;
			}
			float num4 = this.GetCachedReportSizeStyleMM(properties, 14);
			if (float.IsNaN(num4) && !float.IsNaN(cachedReportSizeStyleMM))
			{
				num4 = cachedReportSizeStyleMM;
			}
			if (float.IsNaN(num) && float.IsNaN(num2) && float.IsNaN(num3) && float.IsNaN(num4))
			{
				return null;
			}
			Color cachedReportColorStyle = this.GetCachedReportColorStyle(properties, 0);
			Color color = this.GetCachedReportColorStyle(properties, 1);
			if (color == Color.Empty && cachedReportColorStyle != Color.Empty)
			{
				color = cachedReportColorStyle;
			}
			Color color2 = this.GetCachedReportColorStyle(properties, 3);
			if (color2 == Color.Empty && cachedReportColorStyle != Color.Empty)
			{
				color2 = cachedReportColorStyle;
			}
			Color color3 = this.GetCachedReportColorStyle(properties, 2);
			if (color3 == Color.Empty && cachedReportColorStyle != Color.Empty)
			{
				color3 = cachedReportColorStyle;
			}
			Color color4 = this.GetCachedReportColorStyle(properties, 4);
			if (color4 == Color.Empty && cachedReportColorStyle != Color.Empty)
			{
				color4 = cachedReportColorStyle;
			}
			if (color == Color.Empty && color2 == Color.Empty && color3 == Color.Empty && color4 == Color.Empty)
			{
				return null;
			}
			return this.ProcessBorders(stylePropertyValueBorderStyle2, stylePropertyValueBorderStyle3, stylePropertyValueBorderStyle4, stylePropertyValueBorderStyle5, num, num2, num3, num4, color, color2, color3, color4, position, bounds, renderBorders, state);
		}

		private List<Operation> ProcessBorders(RPLFormat.BorderStyles borderStyleLeft, RPLFormat.BorderStyles borderStyleTop, RPLFormat.BorderStyles borderStyleRight, RPLFormat.BorderStyles borderStyleBottom, float borderWidthLeft, float borderWidthTop, float borderWidthRight, float borderWidthBottom, Color borderColorLeft, Color borderColorTop, Color borderColorRight, Color borderColorBottom, RectangleF position, RectangleF bounds, bool renderBorders, byte state)
		{
			float num = position.Top;
			float num2 = num;
			float borderTopEdgeUnclipped = 0f;
			float num3 = position.Left;
			float num4 = num3;
			float borderLeftEdgeUnclipped = 0f;
			float num5 = position.Bottom;
			float num6 = num5;
			float borderBottomEdgeUnclipped = 0f;
			float num7 = position.Right;
			float num8 = num7;
			float borderRightEdgeUnclipped = 0f;
			float borderWidthTopUnclipped = borderWidthTop;
			float borderWidthLeftUnclipped = borderWidthLeft;
			float borderWidthBottomUnclipped = borderWidthBottom;
			float borderWidthRightUnclipped = borderWidthRight;
			if (borderStyleLeft == RPLFormat.BorderStyles.Double && borderWidthLeft < 0.52920001745224)
			{
				borderStyleLeft = RPLFormat.BorderStyles.Solid;
			}
			if (borderStyleRight == RPLFormat.BorderStyles.Double && borderWidthRight < 0.52920001745224)
			{
				borderStyleRight = RPLFormat.BorderStyles.Solid;
			}
			if (borderStyleTop == RPLFormat.BorderStyles.Double && borderWidthTop < 0.52920001745224)
			{
				borderStyleTop = RPLFormat.BorderStyles.Solid;
			}
			if (borderStyleBottom == RPLFormat.BorderStyles.Double && borderWidthBottom < 0.52920001745224)
			{
				borderStyleBottom = RPLFormat.BorderStyles.Solid;
			}
			if (borderStyleTop != 0)
			{
				num2 = (float)(num2 - borderWidthTop / 2.0);
				borderTopEdgeUnclipped = num2;
				if (num2 < bounds.Top)
				{
					float num9 = bounds.Top - num2;
					borderWidthTop -= num9;
					num = (float)(num + num9 / 2.0);
					num2 = bounds.Top;
					if (borderWidthTop <= 0.0)
					{
						borderStyleTop = RPLFormat.BorderStyles.None;
					}
				}
			}
			else
			{
				borderWidthTop = 0f;
			}
			if (borderStyleLeft != 0)
			{
				num4 = (float)(num4 - borderWidthLeft / 2.0);
				borderLeftEdgeUnclipped = num4;
				if (num4 < bounds.Left)
				{
					float num10 = bounds.Left - num4;
					borderWidthLeft -= num10;
					num3 = (float)(num3 + num10 / 2.0);
					num4 = bounds.Left;
					if (borderWidthLeft <= 0.0)
					{
						borderStyleLeft = RPLFormat.BorderStyles.None;
					}
				}
			}
			else
			{
				borderWidthLeft = 0f;
			}
			if (borderStyleBottom != 0)
			{
				num6 = (float)(num6 + borderWidthBottom / 2.0);
				borderBottomEdgeUnclipped = num6;
				if (num6 > bounds.Bottom)
				{
					float num11 = num6 - bounds.Bottom;
					borderWidthBottom -= num11;
					num5 = (float)(num5 - num11 / 2.0);
					num6 = bounds.Bottom;
					if (borderWidthBottom <= 0.0)
					{
						borderStyleBottom = RPLFormat.BorderStyles.None;
					}
				}
			}
			else
			{
				borderWidthBottom = 0f;
			}
			if (borderStyleRight != 0)
			{
				num8 = (float)(num8 + borderWidthRight / 2.0);
				borderRightEdgeUnclipped = num8;
				if (num8 > bounds.Right)
				{
					float num12 = num8 - bounds.Right;
					borderWidthRight -= num12;
					num7 = (float)(num7 - num12 / 2.0);
					num8 = bounds.Right;
					if (borderWidthRight <= 0.0)
					{
						borderStyleRight = RPLFormat.BorderStyles.None;
					}
				}
			}
			else
			{
				borderWidthRight = 0f;
			}
			num4 = Math.Max(bounds.Left, num4);
			num8 = Math.Min(bounds.Right, num8);
			num2 = Math.Max(bounds.Top, num2);
			num6 = Math.Min(bounds.Bottom, num6);
			if (borderStyleTop != RPLFormat.BorderStyles.Double && state == 0 && borderStyleTop == borderStyleLeft && borderStyleTop == borderStyleBottom && borderStyleTop == borderStyleRight && borderWidthTop == borderWidthLeft && borderWidthTop == borderWidthBottom && borderWidthTop == borderWidthRight && borderColorTop == borderColorLeft && borderColorTop == borderColorBottom && borderColorTop == borderColorRight)
			{
				RectangleF rectangle = new RectangleF(num3, num, num7 - num3, num5 - num);
				if (renderBorders)
				{
					this.Writer.DrawRectangle(borderColorTop, borderWidthTop, borderStyleTop, rectangle);
					return null;
				}
				List<Operation> list = new List<Operation>(1);
				list.Add(new DrawRectangleOp(borderColorTop, borderWidthTop, borderStyleTop, rectangle));
				return list;
			}
			List<Operation> list2 = null;
			if (!renderBorders)
			{
				list2 = new List<Operation>(8);
			}
			float halfPixelWidthX = this.Writer.HalfPixelWidthX;
			float halfPixelWidthY = this.Writer.HalfPixelWidthY;
			float num13 = Math.Min(halfPixelWidthY, (float)(borderWidthTop / 2.0));
			float num14 = Math.Min(halfPixelWidthX, (float)(borderWidthLeft / 2.0));
			float num15 = Math.Min(halfPixelWidthY, (float)(borderWidthBottom / 2.0));
			float num16 = Math.Min(halfPixelWidthX, (float)(borderWidthRight / 2.0));
			if (borderStyleTop != 0 && (state & 1) == 0)
			{
				SharedRenderer.ProcessTopBorder(this.Writer, list2, borderWidthTop, borderStyleTop, borderColorTop, borderColorLeft, borderColorRight, num, num2, num4 + num14, num8 - num16, borderTopEdgeUnclipped, borderLeftEdgeUnclipped, borderRightEdgeUnclipped, borderWidthLeft, borderWidthRight, borderWidthTopUnclipped, borderWidthLeftUnclipped, borderWidthRightUnclipped);
			}
			if (borderStyleLeft != 0 && (state & 4) == 0)
			{
				SharedRenderer.ProcessLeftBorder(this.Writer, list2, borderWidthLeft, borderStyleLeft, borderColorLeft, borderColorTop, borderColorBottom, num3, num4, num2 + num13, num6 - num15, borderLeftEdgeUnclipped, borderTopEdgeUnclipped, borderBottomEdgeUnclipped, borderWidthTop, borderWidthBottom, borderWidthLeftUnclipped, borderWidthTopUnclipped, borderWidthBottomUnclipped);
			}
			if (borderStyleBottom != 0 && (state & 2) == 0)
			{
				SharedRenderer.ProcessBottomBorder(this.Writer, list2, borderWidthBottom, borderStyleBottom, borderColorBottom, borderColorLeft, borderColorRight, num5, num6, num4 + num14, num8 - num16, borderBottomEdgeUnclipped, borderLeftEdgeUnclipped, borderRightEdgeUnclipped, borderWidthLeft, borderWidthRight, borderWidthBottomUnclipped, borderWidthLeftUnclipped, borderWidthRightUnclipped);
			}
			if (borderStyleRight != 0 && (state & 8) == 0)
			{
				SharedRenderer.ProcessRightBorder(this.Writer, list2, borderWidthRight, borderStyleRight, borderColorRight, borderColorTop, borderColorBottom, num7, num8, num2 + num13, num6 - num15, borderRightEdgeUnclipped, borderTopEdgeUnclipped, borderBottomEdgeUnclipped, borderWidthTop, borderWidthBottom, borderWidthRightUnclipped, borderWidthTopUnclipped, borderWidthBottomUnclipped);
			}
			return list2;
		}

		private void ProcessDynamicImage(RPLItemMeasurement measurement, RectangleF position)
		{
			RPLDynamicImageProps rPLDynamicImageProps = (RPLDynamicImageProps)measurement.Element.ElementProps;
			if (rPLDynamicImageProps.DynamicImageContent == null && rPLDynamicImageProps.DynamicImageContentOffset == -1)
			{
				return;
			}
			this.Writer.DrawDynamicImage(rPLDynamicImageProps.UniqueName, rPLDynamicImageProps.DynamicImageContent, rPLDynamicImageProps.DynamicImageContentOffset, position);
			this.ProcessImageMapActions(rPLDynamicImageProps.ActionImageMapAreas, rPLDynamicImageProps.UniqueName, position);
		}

		private void ProcessImage(RPLItemMeasurement measurement, RectangleF position)
		{
			RPLImage rPLImage = (RPLImage)measurement.Element;
			RPLImageProps rPLImageProps = (RPLImageProps)rPLImage.ElementProps;
			RPLImagePropsDef definitionProperties = (RPLImagePropsDef)rPLImageProps.Definition;
			this.CalculateUsableReportItemRectangle(rPLImageProps, ref position);
			if (!(position.Width <= 0.0) && !(position.Height <= 0.0))
			{
				this.Writer.DrawImage(position, rPLImage, rPLImageProps, definitionProperties);
				if (rPLImageProps.ActionInfo != null && rPLImageProps.ActionInfo.Actions.Length > 0)
				{
					this.Writer.ProcessAction(rPLImageProps.UniqueName, rPLImageProps.ActionInfo, position);
				}
				this.ProcessImageMapActions(rPLImageProps.ActionImageMapAreas, rPLImageProps.UniqueName, position);
			}
		}

		private void ProcessImageMapActions(RPLActionInfoWithImageMap[] imageMap, string uniqueName, RectangleF position)
		{
			if (imageMap != null)
			{
				for (int i = 0; i < imageMap.Length; i++)
				{
					this.Writer.ProcessAction(uniqueName, imageMap[i], position);
				}
			}
		}

		private bool ProcessLabelAndBookmark(RPLElementProps properties, RectangleF position)
		{
			RPLItemProps rPLItemProps = properties as RPLItemProps;
			RPLItemPropsDef rPLItemPropsDef = rPLItemProps.Definition as RPLItemPropsDef;
			bool result = false;
			string label = rPLItemProps.Label;
			if (string.IsNullOrEmpty(label))
			{
				label = rPLItemPropsDef.Label;
			}
			if (!string.IsNullOrEmpty(label))
			{
				this.Writer.ProcessLabel(rPLItemProps.UniqueName, label, position.Location);
				result = true;
			}
			string bookmark = rPLItemProps.Bookmark;
			if (string.IsNullOrEmpty(bookmark))
			{
				bookmark = rPLItemPropsDef.Bookmark;
			}
			if (!string.IsNullOrEmpty(bookmark))
			{
				this.Writer.ProcessBookmark(rPLItemProps.UniqueName, position.Location);
				result = true;
			}
			return result;
		}

		private void ProcessLine(RPLItemMeasurement measurement, RectangleF position)
		{
			RPLLine rPLLine = (RPLLine)measurement.Element;
			RPLLineProps properties = (RPLLineProps)rPLLine.ElementProps;
			RPLLinePropsDef rPLLinePropsDef = (RPLLinePropsDef)rPLLine.ElementProps.Definition;
			Color cachedReportColorStyle = this.GetCachedReportColorStyle(properties, 0);
			float cachedReportSizeStyleMM = this.GetCachedReportSizeStyleMM(properties, 10);
			RPLFormat.BorderStyles stylePropertyValueBorderStyle = SharedRenderer.GetStylePropertyValueBorderStyle(properties, 5, RPLFormat.BorderStyles.None);
			if (stylePropertyValueBorderStyle != 0)
			{
				if (!rPLLinePropsDef.Slant)
				{
					this.Writer.DrawLine(cachedReportColorStyle, cachedReportSizeStyleMM, stylePropertyValueBorderStyle, position.Left, position.Top, position.Right, position.Bottom);
				}
				else
				{
					this.Writer.DrawLine(cachedReportColorStyle, cachedReportSizeStyleMM, stylePropertyValueBorderStyle, position.Left, position.Bottom, position.Right, position.Top);
				}
			}
		}

		private void ProcessNonTablixContainerReportItems(RPLContainer container, RectangleF bounds)
		{
			if (container.Children != null && container.Children.Length != 0)
			{
				if (container.Children.Length == 1)
				{
					this.ProcessReportItem(container.Children[0], bounds, true, RectangleF.Empty, false);
					container.Children = null;
				}
				else
				{
					List<RPLItemMeasurement> list = new List<RPLItemMeasurement>(container.Children.Length);
					for (int i = 0; i < container.Children.Length; i++)
					{
						list.Add(container.Children[i]);
					}
					container.Children = null;
					list.Sort(new ZIndexComparer());
					for (int j = 0; j < list.Count; j++)
					{
						this.ProcessReportItem(list[j], bounds, true, RectangleF.Empty, false);
						list[j] = null;
					}
				}
			}
		}

		internal void ProcessPage(RPLReport rplReport, int pageNumber, FontCache sharedFontCache, List<SectionItemizedData> glyphCache)
		{
			this.RplReport = rplReport;
			this.CurrentLanguage = this.RplReport.Language;
			this.m_pageNumber = pageNumber;
			this.m_fontCache = sharedFontCache;
			this.m_beginPage = true;
			if (this.RplReport.RPLPaginatedPages.Length != 0 && this.RplReport.RPLPaginatedPages[0] != null)
			{
				RPLPageContent rPLPageContent = this.RplReport.RPLPaginatedPages[0];
				RPLPageLayout pageLayout = rPLPageContent.PageLayout;
				RPLReportSection nextReportSection = rPLPageContent.GetNextReportSection();
				if (nextReportSection != null)
				{
					float num = default(float);
					float num2 = default(float);
					RectangleF rectangleF = this.Writer.CalculatePageBounds(rPLPageContent, out num, out num2);
					this.Writer.BeginPage(num, num2);
					RectangleF rectangleF2 = new RectangleF(0f, 0f, num, num2);
					this.Writer.PreProcessPage(nextReportSection.ID, rectangleF2);
					Color backgroundColor = Color.Empty;
					object obj = pageLayout.Style[34];
					if (obj != null)
					{
						backgroundColor = new RPLReportColor((string)obj).ToColor();
					}
					RPLImageData backgroundImage = null;
					RPLFormat.BackgroundRepeatTypes backgroundRepeat = RPLFormat.BackgroundRepeatTypes.Clip;
					object obj2 = pageLayout.Style[33];
					if (obj2 != null)
					{
						backgroundImage = (RPLImageData)obj2;
						object obj3 = pageLayout.Style[35];
						backgroundRepeat = ((obj3 != null) ? ((RPLFormat.BackgroundRepeatTypes)obj3) : RPLFormat.BackgroundRepeatTypes.Repeat);
					}
					List<Operation> list = this.ProcessPageStyle(backgroundColor, backgroundImage, backgroundRepeat, pageLayout.Style, rectangleF, rectangleF2);
					float num3 = rectangleF.Top;
					int num4 = 0;
					float width = rectangleF.Width;
					while (nextReportSection != null)
					{
						if (glyphCache != null)
						{
							this.m_sectionItemizedData = glyphCache[num4];
						}
						float num5 = 0f;
						RectangleF rectangleF3 = this.Writer.CalculateHeaderBounds(nextReportSection, pageLayout, num3, width);
						num5 = nextReportSection.BodyArea.Top;
						if (!this.PhysicalPagination)
						{
							num3 += rectangleF3.Height;
							num5 = 0f;
						}
						for (int i = 0; i < nextReportSection.Columns.Length; i++)
						{
							RPLItemMeasurement rPLItemMeasurement = nextReportSection.Columns[i];
							RectangleF rectangleF4 = this.Writer.CalculateColumnBounds(nextReportSection, pageLayout, rPLItemMeasurement, i, num3 + num5, nextReportSection.BodyArea.Height, width);
							this.Writer.BeginPageSection(rectangleF4);
							if (this.PhysicalPagination)
							{
								if (this.m_sectionItemizedData != null && this.m_sectionItemizedData.Columns.Count > i)
								{
									this.m_pageParagraphsItemizedData = this.m_sectionItemizedData.Columns[i];
								}
								else
								{
									this.m_pageParagraphsItemizedData = null;
								}
								rectangleF4.Location = PointF.Empty;
							}
							RectangleF styleBounds = rectangleF4;
							if (this.PhysicalPagination)
							{
								float num6 = rPLItemMeasurement.Left + rPLItemMeasurement.Width;
								if (rectangleF4.Width > num6)
								{
									rectangleF4.Width = num6;
								}
								float num7 = rPLItemMeasurement.Top + rPLItemMeasurement.Height;
								if (rectangleF4.Height > num7)
								{
									rectangleF4.Height = num7;
								}
							}
							this.ProcessReportItem(rPLItemMeasurement, rectangleF4, true, styleBounds, true);
							this.Writer.EndPageSection();
						}
						num3 += nextReportSection.BodyArea.Height + num5;
						if (nextReportSection.Footer != null)
						{
							if (this.m_sectionItemizedData != null)
							{
								this.m_pageParagraphsItemizedData = this.m_sectionItemizedData.HeaderFooter;
							}
							RectangleF rectangleF5 = this.Writer.CalculateFooterBounds(nextReportSection, pageLayout, num3, width);
							this.Writer.BeginPageSection(rectangleF5);
							if (this.PhysicalPagination)
							{
								rectangleF5.Location = PointF.Empty;
							}
							this.ProcessReportItem(nextReportSection.Footer, rectangleF5, true, rectangleF5, true);
							this.Writer.EndPageSection();
							num3 += rectangleF5.Height;
						}
						if (nextReportSection.Header != null)
						{
							if (this.m_sectionItemizedData != null)
							{
								this.m_pageParagraphsItemizedData = this.m_sectionItemizedData.HeaderFooter;
							}
							this.Writer.BeginPageSection(rectangleF3);
							if (this.PhysicalPagination)
							{
								rectangleF3.Location = PointF.Empty;
							}
							this.ProcessReportItem(nextReportSection.Header, rectangleF3, true, rectangleF3, true);
							this.Writer.EndPageSection();
						}
						this.m_pageParagraphsItemizedData = null;
						nextReportSection = rPLPageContent.GetNextReportSection();
						num4++;
					}
					this.Writer.BeginPageSection(rectangleF);
					if (list != null)
					{
						for (int j = 0; j < list.Count; j++)
						{
							list[j].Perform(this.Writer);
						}
					}
					this.Writer.EndPageSection();
					this.Writer.EndPage();
					this.Writer.PostProcessPage();
					this.m_sectionItemizedData = null;
				}
			}
		}

		private List<Operation> ProcessPageStyle(Color backgroundColor, RPLImageData backgroundImage, RPLFormat.BackgroundRepeatTypes backgroundRepeat, RPLElementStyle style, RectangleF pageBounds, RectangleF fullPageBounds)
		{
			bool flag = false;
			if (backgroundColor != Color.Empty && backgroundColor != Color.Transparent)
			{
				goto IL_001f;
			}
			if (backgroundImage != null)
			{
				goto IL_001f;
			}
			goto IL_002e;
			IL_002e:
			if (backgroundColor != Color.Empty && backgroundColor != Color.Transparent)
			{
				this.Writer.FillRectangle(backgroundColor, pageBounds);
			}
			if (backgroundImage != null)
			{
				this.Writer.DrawBackgroundImage(backgroundImage, backgroundRepeat, PointF.Empty, pageBounds);
			}
			List<Operation> result = this.ProcessPageBorders(style, pageBounds, pageBounds, ref flag, fullPageBounds);
			if (flag)
			{
				this.Writer.EndPageSection();
			}
			return result;
			IL_001f:
			this.Writer.BeginPageSection(fullPageBounds);
			flag = true;
			goto IL_002e;
		}

		internal List<Operation> ProcessReportItem(RPLItemMeasurement measurement, RectangleF bounds, bool renderBorders, RectangleF styleBounds, bool renderStylesOnBounds)
		{
			return this.ProcessReportItem(measurement, bounds, renderBorders, styleBounds, renderStylesOnBounds, false);
		}

		private List<Operation> ProcessReportItem(RPLItemMeasurement measurement, RectangleF bounds, bool renderBorders, RectangleF styleBounds, bool renderStylesOnBounds, bool hasTablixCellParent)
		{
			List<Operation> result = null;
			RPLElement element = measurement.Element;
			if (element == null)
			{
				return null;
			}
			bool flag = element is RPLBody;
			bool flag2 = element is RPLLine;
			bool flag3 = element is RPLTablix;
			if (!flag2 && (measurement.Width <= 0.0 || measurement.Height <= 0.0))
			{
				return result;
			}
			RPLElementProps elementProps = element.ElementProps;
			RectangleF measurementRectangle = SharedRenderer.GetMeasurementRectangle(measurement, bounds);
			bool hasLabel = this.ProcessLabelAndBookmark(elementProps, measurementRectangle);
			object state = this.Writer.PreProcessReportItem(element, elementProps, measurementRectangle, hasLabel);
			if (flag3)
			{
				RPLTablix rPLTablix = (RPLTablix)measurement.Element;
				if (rPLTablix.ColumnWidths != null && rPLTablix.ColumnWidths.Length != 0 && rPLTablix.RowHeights != null && rPLTablix.RowHeights.Length != 0)
				{
					RectangleF position = default(RectangleF);
					position.X = measurementRectangle.X + rPLTablix.ContentLeft;
					position.Y = measurementRectangle.Y + rPLTablix.ContentTop;
					float[] array = new float[rPLTablix.RowHeights.Length];
					float[] array2 = new float[rPLTablix.ColumnWidths.Length];
					for (int i = 1; i < rPLTablix.RowHeights.Length; i++)
					{
						array[i] = array[i - 1] + rPLTablix.RowHeights[i - 1];
					}
					for (int j = 1; j < rPLTablix.ColumnWidths.Length; j++)
					{
						array2[j] = array2[j - 1] + rPLTablix.ColumnWidths[j - 1];
					}
					position.Height = array[array.Length - 1] + rPLTablix.RowHeights[rPLTablix.RowHeights.Length - 1];
					position.Width = array2[array2.Length - 1] + rPLTablix.ColumnWidths[rPLTablix.ColumnWidths.Length - 1];
					if (!hasTablixCellParent)
					{
						measurementRectangle.X += rPLTablix.ContentLeft;
						measurementRectangle.Y += rPLTablix.ContentTop;
						measurementRectangle.Height = position.Height;
						measurementRectangle.Width = position.Width;
					}
					this.ProcessBackgroundColorAndImage(elementProps, measurementRectangle, measurementRectangle);
					this.ProcessTablixContainer(rPLTablix, position, array, array2);
				}
				else
				{
					measurementRectangle.X += rPLTablix.ContentLeft;
					measurementRectangle.Y += rPLTablix.ContentTop;
					this.ProcessBackgroundColorAndImage(elementProps, measurementRectangle, measurementRectangle);
				}
			}
			else
			{
				bool flag4 = element is RPLSubReport;
				if (!flag4)
				{
					if (renderStylesOnBounds && styleBounds != RectangleF.Empty)
					{
						this.ProcessBackgroundColorAndImage(elementProps, styleBounds, styleBounds);
					}
					else
					{
						this.ProcessBackgroundColorAndImage(elementProps, measurementRectangle, measurementRectangle);
					}
				}
				if (element is RPLTextBox)
				{
					this.ProcessTextBox(measurement, measurementRectangle);
				}
				else if (element is RPLRectangle || element is RPLHeaderFooter)
				{
					this.ProcessNonTablixContainerReportItems((RPLContainer)element, measurementRectangle);
				}
				else if (flag)
				{
					this.ProcessNonTablixContainerReportItems((RPLContainer)element, measurementRectangle);
				}
				else if (flag4)
				{
					string currentLanguage = this.CurrentLanguage;
					string stylePropertyValueString = SharedRenderer.GetStylePropertyValueString(element.ElementProps, 32);
					if (!string.IsNullOrEmpty(stylePropertyValueString))
					{
						this.CurrentLanguage = stylePropertyValueString;
					}
					RPLContainer rPLContainer = (RPLContainer)element;
					if (rPLContainer.Children.Length == 1)
					{
						this.ProcessReportItem(rPLContainer.Children[0], measurementRectangle, true, RectangleF.Empty, false);
					}
					else
					{
						float num = 0f;
						for (int k = 0; k < rPLContainer.Children.Length; k++)
						{
							if (rPLContainer.Children[k].Top == 0.0)
							{
								rPLContainer.Children[k].Top = num;
							}
							rPLContainer.Children[k].Width = measurement.Width;
							this.ProcessReportItem(rPLContainer.Children[k], measurementRectangle, true, RectangleF.Empty, false);
							num += rPLContainer.Children[k].Height;
							rPLContainer.Children[k] = null;
						}
					}
					this.CurrentLanguage = currentLanguage;
				}
				else if (element is RPLImage)
				{
					this.ProcessImage(measurement, measurementRectangle);
				}
				else if (element is RPLChart || element is RPLGaugePanel || element is RPLMap)
				{
					this.ProcessDynamicImage(measurement, measurementRectangle);
				}
				else if (flag2)
				{
					this.ProcessLine(measurement, measurementRectangle);
				}
			}
			if (!flag2)
			{
				if (flag)
				{
					switch (renderStylesOnBounds)
					{
					case false:
						break;
					default:
						goto IL_0458;
					}
				}
				result = this.ProcessBorders(elementProps, measurementRectangle, bounds, renderBorders, measurement.State);
			}
			goto IL_047a;
			IL_0458:
			if (styleBounds != RectangleF.Empty)
			{
				result = this.ProcessBorders(elementProps, styleBounds, styleBounds, renderBorders, measurement.State);
			}
			goto IL_047a;
			IL_047a:
			this.Writer.PostProcessReportItem(state);
			return result;
		}

		private void ProcessTablixContainer(RPLTablix tablix, RectangleF position, float[] rowStarts, float[] columnStarts)
		{
			int[] array = new int[tablix.ColumnWidths.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = 2147483647;
			}
			this.Writer.ProcessFixedHeaders(tablix, position, rowStarts, columnStarts);
			List<Border> list = null;
			List<Border> list2 = null;
			List<Border> list3 = null;
			List<Border> list4 = null;
			int num = 0;
			int num2 = -1;
			RPLTablixRow nextRow;
			while ((nextRow = tablix.GetNextRow()) != null)
			{
				SharedRenderer.CalculateColumnZIndexes(tablix, nextRow, num, array);
				RPLTablixOmittedRow rPLTablixOmittedRow = nextRow as RPLTablixOmittedRow;
				if (rPLTablixOmittedRow != null)
				{
					for (int j = 0; j < rPLTablixOmittedRow.NumCells; j++)
					{
						RPLTablixMemberCell rPLTablixMemberCell = rPLTablixOmittedRow.OmittedHeaders[j];
						array[rPLTablixMemberCell.ColIndex] = Math.Min(array[rPLTablixMemberCell.ColIndex], SharedRenderer.CalculateZIndex(rPLTablixMemberCell));
						if (!string.IsNullOrEmpty(rPLTablixMemberCell.GroupLabel))
						{
							PointF point = new PointF(position.X, position.Y);
							if (rPLTablixMemberCell.ColIndex < columnStarts.Length)
							{
								point.X = columnStarts[rPLTablixMemberCell.ColIndex];
							}
							if (num < rowStarts.Length)
							{
								point.Y = rowStarts[num];
							}
							this.Writer.ProcessLabel(rPLTablixMemberCell.UniqueName, rPLTablixMemberCell.GroupLabel, point);
						}
					}
				}
				else
				{
					if (nextRow.OmittedHeaders != null)
					{
						for (int k = 0; k < nextRow.OmittedHeaders.Count; k++)
						{
							RPLTablixMemberCell rPLTablixMemberCell2 = nextRow.OmittedHeaders[k];
							if (!string.IsNullOrEmpty(rPLTablixMemberCell2.GroupLabel))
							{
								PointF point2 = new PointF(position.X, position.Y);
								if (rPLTablixMemberCell2.ColIndex < columnStarts.Length)
								{
									point2.X = columnStarts[rPLTablixMemberCell2.ColIndex];
								}
								if (num < rowStarts.Length)
								{
									point2.Y = rowStarts[num];
								}
								this.Writer.ProcessLabel(rPLTablixMemberCell2.UniqueName, rPLTablixMemberCell2.GroupLabel, point2);
							}
						}
					}
					int num3 = 2147483647;
					for (int l = 0; l < nextRow.NumCells; l++)
					{
						RPLTablixCell rPLTablixCell = nextRow[l];
						RPLItemMeasurement rPLItemMeasurement = new RPLItemMeasurement();
						rPLItemMeasurement.Element = rPLTablixCell.Element;
						rPLItemMeasurement.Left = columnStarts[rPLTablixCell.ColIndex];
						rPLItemMeasurement.Top = rowStarts[rPLTablixCell.RowIndex];
						rPLItemMeasurement.Width = tablix.GetColumnWidth(rPLTablixCell.ColIndex, rPLTablixCell.ColSpan);
						rPLItemMeasurement.Height = tablix.GetRowHeight(rPLTablixCell.RowIndex, rPLTablixCell.RowSpan);
						rPLItemMeasurement.State = rPLTablixCell.ElementState;
						if (rPLTablixCell.ContentSizes != null)
						{
							rPLItemMeasurement.Top += rPLTablixCell.ContentSizes.Top;
							rPLItemMeasurement.Height -= rPLTablixCell.ContentSizes.Top;
							rPLItemMeasurement.Left += rPLTablixCell.ContentSizes.Left;
							if (rPLTablixCell.ContentSizes.Width != 0.0)
							{
								rPLItemMeasurement.Width = rPLTablixCell.ContentSizes.Width;
							}
							else
							{
								rPLItemMeasurement.Width -= rPLTablixCell.ContentSizes.Left;
							}
						}
						RPLTablixMemberCell rPLTablixMemberCell3 = rPLTablixCell as RPLTablixMemberCell;
						if (rPLTablixMemberCell3 != null && !string.IsNullOrEmpty(rPLTablixMemberCell3.GroupLabel))
						{
							this.Writer.ProcessLabel(rPLTablixMemberCell3.UniqueName, rPLTablixMemberCell3.GroupLabel, new PointF(rPLItemMeasurement.Left + position.Left, rPLItemMeasurement.Top + position.Top));
						}
						List<Operation> list5 = this.ProcessReportItem(rPLItemMeasurement, position, false, position, false, true);
						if (list5 != null)
						{
							Border border = new Border();
							border.RowIndex = rPLTablixCell.RowIndex;
							border.ColumnIndex = rPLTablixCell.ColIndex;
							border.Operations = list5;
							if (num < tablix.ColumnHeaderRows && rPLTablixCell is RPLTablixCornerCell)
							{
								if (list == null)
								{
									list = new List<Border>();
								}
								list.Add(border);
							}
							else
							{
								if (num3 == 2147483647)
								{
									num3 = SharedRenderer.CalculateRowZIndex(nextRow);
								}
								if (num3 == 2147483647)
								{
									num3 = num2;
								}
								border.RowZIndex = num3;
								border.ColumnZIndex = array[rPLTablixCell.ColIndex];
								if (rPLTablixMemberCell3 != null)
								{
									if (num < tablix.ColumnHeaderRows)
									{
										if (list2 == null)
										{
											list2 = new List<Border>();
										}
										list2.Add(border);
									}
									else
									{
										border.CompareRowFirst = false;
										if (list3 == null)
										{
											list3 = new List<Border>();
										}
										list3.Add(border);
									}
								}
								else
								{
									if (list4 == null)
									{
										list4 = new List<Border>();
									}
									list4.Add(border);
								}
							}
						}
						nextRow[l] = null;
					}
					num++;
					num2 = num3;
				}
			}
			this.RenderBorders(list4);
			this.RenderBorders(list2);
			this.RenderBorders(list3);
			this.RenderBorders(list);
		}

		private void RenderBorders(List<Border> borders)
		{
			if (borders != null && borders.Count > 0)
			{
				borders.Sort(new ZIndexComparer());
				for (int i = 0; i < borders.Count; i++)
				{
					Border border = borders[i];
					for (int j = 0; j < border.Operations.Count; j++)
					{
						border.Operations[j].Perform(this.Writer);
					}
				}
			}
		}

		private void ProcessTextBox(RPLItemMeasurement measurement, RectangleF position)
		{
			RPLTextBox rPLTextBox = (RPLTextBox)measurement.Element;
			RectangleF rectangleF = new RectangleF(position.Location, position.Size);
			RPLTextBoxProps rPLTextBoxProps = (RPLTextBoxProps)rPLTextBox.ElementProps;
			RPLTextBoxPropsDef rPLTextBoxPropsDef = (RPLTextBoxPropsDef)rPLTextBoxProps.Definition;
			string uniqueName = rPLTextBoxProps.UniqueName;
			this.CalculateUsableReportItemRectangle(rPLTextBoxProps, ref rectangleF);
			if (!(rectangleF.Width <= 0.0) && !(rectangleF.Height <= 0.0))
			{
				if (rPLTextBoxProps.IsToggleParent)
				{
					this.Writer.ProcessToggle(uniqueName, rPLTextBoxProps.ToggleState, ref rectangleF);
				}
				if (rPLTextBoxPropsDef.CanSort)
				{
					this.Writer.ProcessSort(uniqueName, rPLTextBoxProps.SortState, ref rectangleF);
				}
				if (rPLTextBoxProps.ActionInfo != null && rPLTextBoxProps.ActionInfo.Actions.Length > 0)
				{
					this.Writer.ProcessAction(uniqueName, rPLTextBoxProps.ActionInfo, rectangleF);
				}
				PointF offset = new PointF(0f, rPLTextBoxProps.ContentOffset);
				ReportTextBox rptTextBox = new ReportTextBox(rPLTextBoxProps, this.Writer);
				if (rPLTextBoxPropsDef.IsSimple)
				{
					string value = rPLTextBoxProps.Value;
					if ((measurement.State & 0x40) == 0 && string.IsNullOrEmpty(value))
					{
						value = rPLTextBoxPropsDef.Value;
					}
					if (!string.IsNullOrEmpty(value))
					{
						ReportTextRun reportTextRun = new ReportTextRun(rPLTextBoxProps.Style, this.m_cachedFontSizes, this.m_cachedReportColors);
						ReportParagraph reportParagraph = new ReportParagraph(rPLTextBoxProps.Style, rPLTextBoxProps.UniqueName);
						this.ProcessSimpleTextBox(value, rectangleF, rptTextBox, reportParagraph, reportTextRun, offset);
					}
				}
				else
				{
					this.ProcessRichTextBox(rectangleF, rPLTextBox, rptTextBox, offset);
				}
			}
		}

		protected virtual void ProcessSimpleTextBox(string value, RectangleF textPosition, ReportTextBox rptTextBox, ReportParagraph reportParagraph, ReportTextRun reportTextRun, PointF offset)
		{
			AspNetCore.ReportingServices.Rendering.RichText.Paragraph paragraph = new AspNetCore.ReportingServices.Rendering.RichText.Paragraph(reportParagraph, 1);
			AspNetCore.ReportingServices.Rendering.RichText.TextBox richTextBox = new AspNetCore.ReportingServices.Rendering.RichText.TextBox(rptTextBox);
			bool flag = true;
			TextRunItemizedData textRunItemizedData = null;
			if (this.m_pageParagraphsItemizedData != null)
			{
				List<TextRunItemizedData> list = null;
				this.m_pageParagraphsItemizedData.TryGetValue(reportParagraph.UniqueName, out list);
				if (list != null)
				{
					flag = false;
					textRunItemizedData = list[0];
				}
			}
			if (textRunItemizedData != null)
			{
				this.CreateParagraphRuns(value, paragraph, reportTextRun, textRunItemizedData);
			}
			else
			{
				AspNetCore.ReportingServices.Rendering.RichText.TextRun item = new AspNetCore.ReportingServices.Rendering.RichText.TextRun(value, reportTextRun);
				paragraph.Runs.Add(item);
			}
			richTextBox.Paragraphs = new List<AspNetCore.ReportingServices.Rendering.RichText.Paragraph>(1);
			richTextBox.Paragraphs.Add(paragraph);
			if (flag)
			{
				richTextBox.ScriptItemize();
			}
			this.Writer.CommonGraphics.ExecuteSync(delegate
			{
				Win32DCSafeHandle zero = Win32DCSafeHandle.Zero;
				try
				{
					float dpiX = default(float);
					this.Writer.GetHdc(this.m_beginPage, out zero, out dpiX);
					this.m_beginPage = false;
					FlowContext flowContext = new FlowContext(textPosition.Width, textPosition.Height, true, false);
					if (richTextBox.VerticalText)
					{
						if (textPosition.Y <= 0.0)
						{
							flowContext.Height = textPosition.Bottom;
							textPosition.Height = flowContext.Height;
							textPosition.Y = 0f;
							rptTextBox.SpanPages = true;
						}
						if (textPosition.Bottom >= this.Writer.PageSectionBounds.Height)
						{
							flowContext.Height = this.Writer.PageSectionBounds.Height - textPosition.Y;
							textPosition.Height = flowContext.Height;
							rptTextBox.SpanPages = true;
						}
					}
					flowContext.ContentOffset = offset.Y;
					flowContext.Updatable = true;
					float num = default(float);
					List<AspNetCore.ReportingServices.Rendering.RichText.Paragraph> paragraphs = LineBreaker.Flow(richTextBox, zero, dpiX, this.m_fontCache, flowContext, true, out num);
					this.Writer.ClipTextboxRectangle(zero, textPosition);
					AspNetCore.ReportingServices.Rendering.RichText.TextBox.Render(richTextBox, paragraphs, zero, this.m_fontCache, offset, textPosition, dpiX);
					this.Writer.UnClipTextboxRectangle(zero);
				}
				finally
				{
					this.Writer.ReleaseHdc(false);
				}
			});
		}

		protected virtual void ProcessRichTextBox(RectangleF textPosition, RPLTextBox textbox, ReportTextBox rptTextBox, PointF offset)
		{
			List<AspNetCore.ReportingServices.Rendering.RichText.Paragraph> list = new List<AspNetCore.ReportingServices.Rendering.RichText.Paragraph>();
			RPLParagraph rPLParagraph = null;
			List<TextRunItemizedData> list2 = null;
			bool flag = true;
			while ((rPLParagraph = textbox.GetNextParagraph()) != null)
			{
				RPLParagraphProps rPLParagraphProps = (RPLParagraphProps)rPLParagraph.ElementProps;
				ReportParagraph reportParagraph = new ReportParagraph(rPLParagraphProps);
				AspNetCore.ReportingServices.Rendering.RichText.Paragraph paragraph = new AspNetCore.ReportingServices.Rendering.RichText.Paragraph(reportParagraph, 1);
				if (!rPLParagraphProps.FirstLine)
				{
					paragraph.Updated = true;
				}
				if (this.m_pageParagraphsItemizedData != null)
				{
					this.m_pageParagraphsItemizedData.TryGetValue(reportParagraph.UniqueName, out list2);
					if (list2 != null)
					{
						flag = false;
					}
				}
				RPLTextRun rPLTextRun = null;
				int num = 0;
				while ((rPLTextRun = rPLParagraph.GetNextTextRun()) != null)
				{
					RPLTextRunProps rPLTextRunProps = (RPLTextRunProps)rPLTextRun.ElementProps;
					string value = rPLTextRunProps.Value;
					if (string.IsNullOrEmpty(value))
					{
						value = ((RPLTextRunPropsDef)rPLTextRunProps.Definition).Value;
					}
					ReportTextRun reportTextRun = new ReportTextRun(rPLTextRunProps.Style, rPLTextRunProps.UniqueName, rPLTextRunProps.ActionInfo, this.m_cachedFontSizes, this.m_cachedReportColors);
					TextRunItemizedData textRunItemizedData = null;
					if (list2 != null)
					{
						textRunItemizedData = list2[num];
					}
					if (textRunItemizedData != null)
					{
						this.CreateParagraphRuns(value, paragraph, reportTextRun, textRunItemizedData);
					}
					else
					{
						AspNetCore.ReportingServices.Rendering.RichText.TextRun item = new AspNetCore.ReportingServices.Rendering.RichText.TextRun(value, reportTextRun);
						paragraph.Runs.Add(item);
					}
					num++;
				}
				list.Add(paragraph);
			}
			if (list.Count != 0)
			{
				AspNetCore.ReportingServices.Rendering.RichText.TextBox richTextBox = new AspNetCore.ReportingServices.Rendering.RichText.TextBox(rptTextBox);
				richTextBox.Paragraphs = list;
				if (flag)
				{
					richTextBox.ScriptItemize();
				}
				this.Writer.CommonGraphics.ExecuteSync(delegate
				{
					Win32DCSafeHandle zero = Win32DCSafeHandle.Zero;
					try
					{
						float dpiX = default(float);
						this.Writer.GetHdc(this.m_beginPage, out zero, out dpiX);
						this.m_beginPage = false;
						FlowContext flowContext = new FlowContext(textPosition.Width, textPosition.Height, true, false);
						if (richTextBox.VerticalText)
						{
							if (textPosition.Y <= 0.0)
							{
								flowContext.Height = textPosition.Bottom;
								textPosition.Height = flowContext.Height;
								textPosition.Y = 0f;
								rptTextBox.SpanPages = true;
							}
							if (textPosition.Bottom >= this.Writer.PageSectionBounds.Height)
							{
								flowContext.Height = this.Writer.PageSectionBounds.Height - textPosition.Y;
								textPosition.Height = flowContext.Height;
								rptTextBox.SpanPages = true;
							}
						}
						flowContext.ContentOffset = offset.Y;
						flowContext.Updatable = true;
						float num2 = default(float);
						List<AspNetCore.ReportingServices.Rendering.RichText.Paragraph> list3 = LineBreaker.Flow(richTextBox, zero, dpiX, this.m_fontCache, flowContext, true, out num2);
						if (list3 != null && list3.Count > 0)
						{
							this.Writer.ClipTextboxRectangle(zero, textPosition);
							AspNetCore.ReportingServices.Rendering.RichText.TextBox.Render(richTextBox, list3, zero, this.m_fontCache, offset, textPosition, dpiX);
							this.Writer.UnClipTextboxRectangle(zero);
						}
					}
					finally
					{
						this.Writer.ReleaseHdc(false);
					}
				});
			}
		}

		private void CreateParagraphRuns(string value, AspNetCore.ReportingServices.Rendering.RichText.Paragraph richTextParagraph, ReportTextRun reportTextRun, TextRunItemizedData textRunItemizedData)
		{
			AspNetCore.ReportingServices.Rendering.RichText.TextRun textRun = null;
			if (textRunItemizedData.SplitIndexes != null && textRunItemizedData.SplitIndexes.Count > 0)
			{
				string text = null;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				for (int i = 0; i < textRunItemizedData.SplitIndexes.Count; i++)
				{
					num3 = textRunItemizedData.SplitIndexes[i];
					text = value.Substring(num2, num3 - num2);
					textRun = ((textRunItemizedData.GlyphData == null || textRunItemizedData.GlyphData.Count <= 0) ? new AspNetCore.ReportingServices.Rendering.RichText.TextRun(text, reportTextRun) : new AspNetCore.ReportingServices.Rendering.RichText.TextRun(text, reportTextRun, textRunItemizedData.GlyphData[num]));
					num2 = num3;
					num++;
					richTextParagraph.Runs.Add(textRun);
				}
				if (num2 < value.Length)
				{
					num3 = value.Length;
					text = value.Substring(num2, num3 - num2);
					textRun = ((textRunItemizedData.GlyphData == null || textRunItemizedData.GlyphData.Count <= 0) ? new AspNetCore.ReportingServices.Rendering.RichText.TextRun(text, reportTextRun) : new AspNetCore.ReportingServices.Rendering.RichText.TextRun(text, reportTextRun, textRunItemizedData.GlyphData[num]));
					richTextParagraph.Runs.Add(textRun);
				}
			}
			else
			{
				textRun = ((textRunItemizedData.GlyphData == null || textRunItemizedData.GlyphData.Count <= 0) ? new AspNetCore.ReportingServices.Rendering.RichText.TextRun(value, reportTextRun) : new AspNetCore.ReportingServices.Rendering.RichText.TextRun(value, reportTextRun, textRunItemizedData.GlyphData[0]));
				richTextParagraph.Runs.Add(textRun);
			}
		}
	}
}
