using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class Image : PageItem, IStorable, IPersistable
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

		private bool m_invalidImage;

		[StaticReference]
		private GDIImageProps m_imageProps;

		private string m_streamName;

		private double m_padHorizontal;

		private double m_padVertical;

		private static Declaration m_declaration = Image.GetDeclaration();

		public override int Size
		{
			get
			{
				return base.Size + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_streamName) + 1 + 16;
			}
		}

		internal Image()
		{
		}

		internal Image(AspNetCore.ReportingServices.OnDemandReportRendering.Image source)
			: base(source)
		{
			base.m_itemPageSizes = new ItemSizes(source);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			writer.RegisterDeclaration(Image.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.InvalidImage:
					writer.Write(this.m_invalidImage);
					break;
				case MemberName.ImageProps:
				{
					int value = scalabilityCache.StoreStaticReference(this.m_imageProps);
					writer.Write(value);
					break;
				}
				case MemberName.StreamName:
					writer.Write(this.m_streamName);
					break;
				case MemberName.HorizontalPadding:
					writer.Write(this.m_padHorizontal);
					break;
				case MemberName.VerticalPadding:
					writer.Write(this.m_padVertical);
					break;
				default:
					RSTrace.RenderingTracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			reader.RegisterDeclaration(Image.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.InvalidImage:
					this.m_invalidImage = reader.ReadBoolean();
					break;
				case MemberName.ImageProps:
				{
					int id = reader.ReadInt32();
					this.m_imageProps = (GDIImageProps)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.StreamName:
					this.m_streamName = reader.ReadString();
					break;
				case MemberName.HorizontalPadding:
					this.m_padHorizontal = reader.ReadDouble();
					break;
				case MemberName.VerticalPadding:
					this.m_padVertical = reader.ReadDouble();
					break;
				default:
					RSTrace.RenderingTracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.Image;
		}

		internal new static Declaration GetDeclaration()
		{
			if (Image.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.InvalidImage, Token.Boolean));
				list.Add(new MemberInfo(MemberName.ImageProps, Token.Int32));
				list.Add(new MemberInfo(MemberName.StreamName, Token.String));
				list.Add(new MemberInfo(MemberName.HorizontalPadding, Token.Double));
				list.Add(new MemberInfo(MemberName.VerticalPadding, Token.Double));
				return new Declaration(ObjectType.Image, ObjectType.PageItem, list);
			}
			return Image.m_declaration;
		}

		protected override void DetermineHorizontalSize(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, List<PageItem> ancestors, bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			this.DetermineSize(pageContext);
		}

		protected override void DetermineVerticalSize(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			this.DetermineSize(pageContext);
		}

		private void DetermineSize(PageContext pageContext)
		{
			if (!this.m_invalidImage && this.m_streamName == null && this.m_imageProps == null)
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.Image image = (AspNetCore.ReportingServices.OnDemandReportRendering.Image)base.m_source;
				this.CheckAutoSize(image, pageContext);
			}
			if (this.m_imageProps != null)
			{
				this.ResizeImage(pageContext, this.m_imageProps.Width, this.m_imageProps.Height);
			}
			else if (this.m_streamName != null)
			{
				Hashtable autoSizeSharedImages = pageContext.AutoSizeSharedImages;
				if (autoSizeSharedImages != null)
				{
					AutosizeImageProps autosizeImageProps = (AutosizeImageProps)autoSizeSharedImages[this.m_streamName];
					if (autosizeImageProps != null)
					{
						this.ResizeImage(pageContext, autosizeImageProps.Width, autosizeImageProps.Height);
					}
				}
			}
		}

		internal override void CacheNonSharedProperties(PageContext pageContext)
		{
			if (pageContext.CacheNonSharedProps)
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.Image image = (AspNetCore.ReportingServices.OnDemandReportRendering.Image)base.m_source;
				this.CheckAutoSize(image, pageContext);
				base.CacheNonSharedProperties(pageContext);
			}
		}

		private void CheckAutoSize(AspNetCore.ReportingServices.OnDemandReportRendering.Image image, PageContext pageContext)
		{
			if (image.Sizing == AspNetCore.ReportingServices.OnDemandReportRendering.Image.Sizings.AutoSize)
			{
				ImageInstance imageInstance = (ImageInstance)image.Instance;
				System.Drawing.Image image2 = null;
				this.m_invalidImage = this.AutoSizeImage(pageContext, imageInstance, out image2);
				if (image2 != null)
				{
					this.m_imageProps = new GDIImageProps(image2);
					image2.Dispose();
				}
			}
		}

		private bool AutoSizeImage(PageContext pageContext, ImageInstance imageInstance, out System.Drawing.Image gdiImage)
		{
			gdiImage = null;
			bool result = false;
			AutosizeImageProps autosizeImageProps = null;
			this.m_streamName = imageInstance.StreamName;
			if (this.m_streamName != null)
			{
				Hashtable hashtable = pageContext.AutoSizeSharedImages;
				if (hashtable != null)
				{
					autosizeImageProps = (AutosizeImageProps)hashtable[this.m_streamName];
					if (autosizeImageProps != null)
					{
						this.GetPaddings(pageContext);
						return autosizeImageProps.InvalidImage;
					}
				}
				autosizeImageProps = new AutosizeImageProps();
				if (hashtable == null)
				{
					hashtable = (pageContext.AutoSizeSharedImages = new Hashtable());
				}
				hashtable.Add(this.m_streamName, autosizeImageProps);
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
						((Bitmap)gdiImage).SetResolution((float)pageContext.DpiX, (float)pageContext.DpiY);
					}
				}
				catch
				{
					array = null;
				}
			}
			if (array == null)
			{
				gdiImage = (Bitmap)HPBProcessing.HPBResourceManager.GetObject("InvalidImage");
				result = true;
				autosizeImageProps.InvalidImage = true;
			}
			if (gdiImage != null)
			{
				this.GetPaddings(pageContext);
				autosizeImageProps.Width = gdiImage.Width;
				autosizeImageProps.Height = gdiImage.Height;
			}
			return result;
		}

		private void GetPaddings(PageContext pageContext)
		{
			PaddingsStyle paddingsStyle = null;
			if (pageContext.ItemPaddingsStyle != null)
			{
				paddingsStyle = (PaddingsStyle)pageContext.ItemPaddingsStyle[base.m_source.ID];
			}
			double num = 0.0;
			if (paddingsStyle != null)
			{
				paddingsStyle.GetPaddingValues(base.m_source, out this.m_padVertical, out this.m_padHorizontal, out num);
			}
			else
			{
				PaddingsStyle.CreatePaddingsStyle(pageContext, base.m_source, out this.m_padVertical, out this.m_padHorizontal, out num);
			}
		}

		private void ResizeImage(PageContext pageContext, int width, int height)
		{
			double num = pageContext.ConvertToMillimeters(height, (float)pageContext.Common.Pagination.MeasureImageDpiY);
			base.m_itemPageSizes.AdjustHeightTo(num + this.m_padVertical);
			double num2 = pageContext.ConvertToMillimeters(width, (float)pageContext.Common.Pagination.MeasureImageDpiX);
			base.m_itemPageSizes.AdjustWidthTo(num2 + this.m_padHorizontal);
		}

		internal override void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					long position = baseStream.Position;
					binaryWriter.Write((byte)9);
					base.WriteElementProps(binaryWriter, rplWriter, pageContext, position + 1);
					base.m_offset = baseStream.Position;
					binaryWriter.Write((byte)254);
					binaryWriter.Write(position);
					binaryWriter.Write((byte)255);
				}
				else if (base.m_rplElement == null)
				{
					base.m_rplElement = new RPLImage();
					base.WriteElementProps(base.m_rplElement.ElementProps, pageContext);
				}
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

		internal override void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, PageContext pageContext)
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

		internal override void WriteCustomNonSharedItemProps(BinaryWriter spbifWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Image image = (AspNetCore.ReportingServices.OnDemandReportRendering.Image)base.m_source;
			ImageInstance imageInstance = (ImageInstance)image.Instance;
			if (this.m_invalidImage)
			{
				if (base.m_nonSharedOffset < 0)
				{
					base.WriteInvalidImage(spbifWriter, pageContext, this.m_imageProps);
				}
			}
			else
			{
				base.WriteImage(imageInstance, null, spbifWriter, pageContext, this.m_imageProps, false);
			}
			base.WriteActionInfo(image.ActionInfo, spbifWriter);
		}

		internal override void WriteCustomNonSharedItemProps(RPLElementProps nonSharedProps, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Image image = (AspNetCore.ReportingServices.OnDemandReportRendering.Image)base.m_source;
			ImageInstance imageInstance = (ImageInstance)image.Instance;
			RPLImageProps rPLImageProps = (RPLImageProps)nonSharedProps;
			if (this.m_invalidImage)
			{
				base.WriteInvalidImage(rPLImageProps, pageContext, this.m_imageProps);
			}
			else
			{
				base.WriteImage(imageInstance, null, rPLImageProps, pageContext, this.m_imageProps);
			}
			rPLImageProps.ActionInfo = PageItem.WriteActionInfo(image.ActionInfo);
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
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingBottom, 18);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingLeft, 15);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingRight, 16);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingTop, 17);
		}

		internal override void WriteItemNonSharedStyleProps(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
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

		internal override void WriteItemNonSharedStyleProps(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
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
	}
}
