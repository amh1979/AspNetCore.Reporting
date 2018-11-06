using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections;
using System.Drawing;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class Image : PageItem
	{
		internal sealed class AutosizeImageProps
		{
			private bool m_invalidImage;

			private int m_width;

			private int m_height;

			internal bool InvalidImage
			{
				get
				{
					return this.m_invalidImage;
				}
				set
				{
					this.m_invalidImage = value;
				}
			}

			internal int Width
			{
				get
				{
					return this.m_width;
				}
				set
				{
					this.m_width = value;
				}
			}

			internal int Height
			{
				get
				{
					return this.m_height;
				}
				set
				{
					this.m_height = value;
				}
			}
		}

		internal Image(AspNetCore.ReportingServices.OnDemandReportRendering.Image source, PageContext pageContext, bool createForRepeat)
			: base(source)
		{
			if (pageContext != null)
			{
				if (createForRepeat)
				{
					base.m_itemPageSizes = pageContext.GetSharedFromRepeatItemSizesElement(source, false);
				}
				else
				{
					base.m_itemPageSizes = pageContext.GetSharedItemSizesElement(source, false);
				}
			}
			else
			{
				base.m_itemPageSizes = new ItemSizes(source);
			}
			if (source.Sizing == AspNetCore.ReportingServices.OnDemandReportRendering.Image.Sizings.AutoSize)
			{
				if (base.m_itemPageSizes.Width == 0.0)
				{
					base.m_itemPageSizes.Width = 3.8;
				}
				if (base.m_itemPageSizes.Height == 0.0)
				{
					base.m_itemPageSizes.Height = 4.0;
				}
			}
		}

		internal override bool CalculatePage(RPLWriter rplWriter, PageItemHelper lastPageInfo, PageContext pageContext, PageItem[] siblings, RepeatWithItem[] repeatWithItems, double parentTopInPage, ref double parentPageHeight, Interactivity interactivity)
		{
			base.AdjustOriginFromItemsAbove(siblings, repeatWithItems);
			if (!this.HitsCurrentPage(pageContext, parentTopInPage))
			{
				return false;
			}
			ItemSizes contentSize = null;
			bool flag = base.ResolveItemHiddenState(rplWriter, interactivity, pageContext, false, ref contentSize);
			parentPageHeight = Math.Max(parentPageHeight, base.m_itemPageSizes.Bottom);
			if (rplWriter != null)
			{
				if (base.m_itemRenderSizes == null)
				{
					this.CreateItemRenderSizes(contentSize, pageContext, false);
				}
				if (!flag)
				{
					this.WriteItemToStream(rplWriter, pageContext);
				}
			}
			return true;
		}

		internal override void CalculateRepeatWithPage(RPLWriter rplWriter, PageContext pageContext, PageItem[] siblings)
		{
			base.AdjustOriginFromItemsAbove(siblings, null);
			ItemSizes contentSize = null;
			base.ResolveItemHiddenState(rplWriter, null, pageContext, true, ref contentSize);
			if (base.m_itemRenderSizes == null)
			{
				this.CreateItemRenderSizes(contentSize, pageContext, true);
			}
		}

		internal override int WriteRepeatWithToPage(RPLWriter rplWriter, PageContext pageContext)
		{
			if (base.ItemState == State.OnPageHidden)
			{
				return 0;
			}
			this.WriteItemToStream(rplWriter, pageContext);
			return 1;
		}

		private ReportSize GetStyleValue(StyleAttributeNames styleName)
		{
			object obj = null;
			ReportProperty reportProperty = ((StyleBase)base.m_source.Style)[styleName];
			if (reportProperty != null)
			{
				if (reportProperty.IsExpression)
				{
					StyleInstance styleInstance = base.GetStyleInstance(base.m_source, null);
					obj = ((StyleBaseInstance)styleInstance)[styleName];
				}
				if (obj == null)
				{
					obj = ((ReportSizeProperty)reportProperty).Value;
				}
			}
			return obj as ReportSize;
		}

		private double GetPaddings(StyleAttributeNames padd1, StyleAttributeNames padd2)
		{
			double num = 0.0;
			ReportSize styleValue = this.GetStyleValue(padd1);
			if (styleValue != null)
			{
				num += styleValue.ToMillimeters();
			}
			styleValue = this.GetStyleValue(padd2);
			if (styleValue != null)
			{
				num += styleValue.ToMillimeters();
			}
			return num;
		}

		private bool AutoSizeImage(PageContext pageContext, ImageInstance imageInstance, out System.Drawing.Image gdiImage)
		{
			gdiImage = null;
			if (!pageContext.MeasureItems)
			{
				return false;
			}
			AspNetCore.ReportingServices.OnDemandReportRendering.Image image = (AspNetCore.ReportingServices.OnDemandReportRendering.Image)base.m_source;
			if (image.Sizing != 0)
			{
				return false;
			}
			bool result = false;
			AutosizeImageProps autosizeImageProps = null;
			string streamName = imageInstance.StreamName;
			if (streamName != null)
			{
				Hashtable hashtable = pageContext.AutoSizeSharedImages;
				if (hashtable != null)
				{
					autosizeImageProps = (AutosizeImageProps)hashtable[streamName];
					if (autosizeImageProps != null)
					{
						this.ResizeImage(pageContext, autosizeImageProps.Width, autosizeImageProps.Height);
						return autosizeImageProps.InvalidImage;
					}
				}
				autosizeImageProps = new AutosizeImageProps();
				if (hashtable == null)
				{
					hashtable = (pageContext.AutoSizeSharedImages = new Hashtable());
				}
				hashtable.Add(streamName, autosizeImageProps);
			}
			else
			{
				autosizeImageProps = new AutosizeImageProps();
			}
			byte[] array = imageInstance.ImageData;
			if (array != null)
			{
				try
				{
					MemoryStream stream = new MemoryStream(array, false);
					gdiImage = System.Drawing.Image.FromStream(stream);
					if (gdiImage != null)
					{
						((Bitmap)gdiImage).SetResolution(pageContext.DpiX, pageContext.DpiY);
					}
				}
				catch
				{
					array = null;
					if (gdiImage != null)
					{
						gdiImage.Dispose();
						gdiImage = null;
					}
				}
			}
			if (array == null)
			{
				gdiImage = (Bitmap)SPBProcessing.SPBResourceManager.GetObject("InvalidImage");
				result = true;
				autosizeImageProps.InvalidImage = true;
			}
			if (gdiImage != null)
			{
				this.ResizeImage(pageContext, gdiImage.Width, gdiImage.Height);
				autosizeImageProps.Width = gdiImage.Width;
				autosizeImageProps.Height = gdiImage.Height;
			}
			return result;
		}

		private void ResizeImage(PageContext pageContext, int width, int height)
		{
			double paddings = this.GetPaddings(StyleAttributeNames.PaddingLeft, StyleAttributeNames.PaddingRight);
			double paddings2 = this.GetPaddings(StyleAttributeNames.PaddingTop, StyleAttributeNames.PaddingBottom);
			double num = pageContext.ConvertToMillimeters(height, pageContext.DpiY);
			base.m_itemRenderSizes.AdjustHeightTo(num + paddings2);
			double num2 = pageContext.ConvertToMillimeters(width, pageContext.DpiX);
			base.m_itemRenderSizes.AdjustWidthTo(num2 + paddings);
		}

		internal void WriteItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				Stream baseStream = binaryWriter.BaseStream;
				long position = baseStream.Position;
				binaryWriter.Write((byte)9);
				this.WriteElementProps(binaryWriter, rplWriter, pageContext, position + 1);
				base.m_offset = baseStream.Position;
				binaryWriter.Write((byte)254);
				binaryWriter.Write(position);
				binaryWriter.Write((byte)255);
			}
			else
			{
				base.m_rplElement = new RPLImage();
				this.WriteElementProps(base.m_rplElement.ElementProps, rplWriter, pageContext);
			}
		}

		internal override void WriteCustomSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Image image = (AspNetCore.ReportingServices.OnDemandReportRendering.Image)base.m_source;
			switch (image.Sizing)
			{
			case AspNetCore.ReportingServices.OnDemandReportRendering.Image.Sizings.Clip:
				spbifWriter.Write((byte)41);
				spbifWriter.Write((byte)3);
				break;
			case AspNetCore.ReportingServices.OnDemandReportRendering.Image.Sizings.Fit:
				spbifWriter.Write((byte)41);
				spbifWriter.Write((byte)1);
				break;
			case AspNetCore.ReportingServices.OnDemandReportRendering.Image.Sizings.FitProportional:
				spbifWriter.Write((byte)41);
				spbifWriter.Write((byte)2);
				break;
			}
		}

		internal override void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, RPLWriter rplWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Image image = (AspNetCore.ReportingServices.OnDemandReportRendering.Image)base.m_source;
			RPLImagePropsDef rPLImagePropsDef = (RPLImagePropsDef)sharedProps;
			switch (image.Sizing)
			{
			case AspNetCore.ReportingServices.OnDemandReportRendering.Image.Sizings.AutoSize:
				rPLImagePropsDef.Sizing = RPLFormat.Sizings.AutoSize;
				break;
			case AspNetCore.ReportingServices.OnDemandReportRendering.Image.Sizings.Clip:
				rPLImagePropsDef.Sizing = RPLFormat.Sizings.Clip;
				break;
			case AspNetCore.ReportingServices.OnDemandReportRendering.Image.Sizings.Fit:
				rPLImagePropsDef.Sizing = RPLFormat.Sizings.Fit;
				break;
			case AspNetCore.ReportingServices.OnDemandReportRendering.Image.Sizings.FitProportional:
				rPLImagePropsDef.Sizing = RPLFormat.Sizings.FitProportional;
				break;
			}
		}

		internal override void WriteCustomNonSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Image image = (AspNetCore.ReportingServices.OnDemandReportRendering.Image)base.m_source;
			ImageInstance imageInstance = (ImageInstance)image.Instance;
			System.Drawing.Image image2 = null;
			bool flag = this.AutoSizeImage(pageContext, imageInstance, out image2);
			try
			{
				if (flag)
				{
					base.WriteImage(null, "InvalidImage", spbifWriter, pageContext, image2);
				}
				else
				{
					base.WriteImage(imageInstance, null, spbifWriter, pageContext, image2);
				}
			}
			finally
			{
				if (image2 != null)
				{
					image2.Dispose();
					image2 = null;
				}
			}
			base.WriteActionInfo(image.ActionInfo, spbifWriter, pageContext, 7);
			base.WriteImageMapAreaInstanceCollection(imageInstance.ActionInfoWithDynamicImageMapAreas, spbifWriter, pageContext);
		}

		internal static string SharedImageStreamName(AspNetCore.ReportingServices.OnDemandReportRendering.Image image, PageContext pageContext)
		{
			ImageInstance imageInstance = (ImageInstance)image.Instance;
			System.Drawing.Image image2 = null;
			Image image3 = new Image(image, pageContext, false);
			bool flag = image3.AutoSizeImage(pageContext, imageInstance, out image2);
			if (image2 != null)
			{
				image2.Dispose();
				image2 = null;
			}
			if (flag)
			{
				return "InvalidImage";
			}
			if (imageInstance != null)
			{
				return imageInstance.StreamName;
			}
			return null;
		}

		internal override void WriteCustomNonSharedItemProps(RPLElementProps nonSharedProps, RPLWriter rplWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Image image = (AspNetCore.ReportingServices.OnDemandReportRendering.Image)base.m_source;
			ImageInstance imageInstance = (ImageInstance)image.Instance;
			System.Drawing.Image gdiImage = null;
			RPLImageProps rPLImageProps = (RPLImageProps)nonSharedProps;
			if (this.AutoSizeImage(pageContext, imageInstance, out gdiImage))
			{
				base.WriteImage(null, "InvalidImage", rPLImageProps, pageContext, gdiImage);
			}
			else
			{
				base.WriteImage(imageInstance, null, rPLImageProps, pageContext, gdiImage);
			}
			rPLImageProps.ActionInfo = base.WriteActionInfo(image.ActionInfo, pageContext);
			rPLImageProps.ActionImageMapAreas = base.WriteImageMapAreaInstanceCollection(imageInstance.ActionInfoWithDynamicImageMapAreas, pageContext);
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingBottom, 18);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingLeft, 15);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingRight, 16);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingTop, 17);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingBottom, 18);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingLeft, 15);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingRight, 16);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingTop, 17);
		}

		internal override void WriteItemNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.PaddingBottom:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.PaddingBottom, 18);
				break;
			case StyleAttributeNames.PaddingLeft:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.PaddingLeft, 15);
				break;
			case StyleAttributeNames.PaddingRight:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.PaddingRight, 16);
				break;
			case StyleAttributeNames.PaddingTop:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.PaddingTop, 17);
				break;
			}
		}

		internal override void WriteItemNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.PaddingBottom:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.PaddingBottom, 18);
				break;
			case StyleAttributeNames.PaddingLeft:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.PaddingLeft, 15);
				break;
			case StyleAttributeNames.PaddingRight:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.PaddingRight, 16);
				break;
			case StyleAttributeNames.PaddingTop:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.PaddingTop, 17);
				break;
			}
		}

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)9);
				base.WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write((byte)255);
			}
		}

		internal override PageItemHelper WritePaginationInfo()
		{
			PageItemHelper pageItemHelper = new PageItemHelper(9);
			base.WritePaginationInfoProperties(pageItemHelper);
			return pageItemHelper;
		}
	}
}
