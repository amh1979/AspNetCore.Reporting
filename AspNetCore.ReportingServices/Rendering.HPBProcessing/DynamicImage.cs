using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal abstract class DynamicImage : PageItem, IStorable, IPersistable
	{
		private static Declaration m_declaration = DynamicImage.GetDeclaration();

		protected abstract byte ElementToken
		{
			get;
		}

		protected abstract bool SpecialBorderHandling
		{
			get;
		}

		internal DynamicImage()
		{
		}

		internal DynamicImage(ReportItem source)
			: base(source)
		{
			base.m_itemPageSizes = new ItemSizes(source);
			bool unresolvedPBS = base.UnresolvedPBE = true;
			base.UnresolvedPBS = unresolvedPBS;
		}

		protected abstract RPLItem CreateRPLItem();

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(DynamicImage.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(false, string.Empty);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(DynamicImage.m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(false, string.Empty);
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.DynamicImage;
		}

		internal new static Declaration GetDeclaration()
		{
			if (DynamicImage.m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ObjectType.DynamicImage, ObjectType.PageItem, memberInfoList);
			}
			return DynamicImage.m_declaration;
		}

		private Stream LoadDynamicImage(out ActionInfoWithDynamicImageMapCollection actionImageMaps, PageContext pageContext)
		{
			IDynamicImageInstance dynamicImageInstance = (IDynamicImageInstance)base.m_source.Instance;
			dynamicImageInstance.SetDpi(pageContext.DynamicImageDpiX, pageContext.DynamicImageDpiY);
			if (pageContext.IsInSelectiveRendering)
			{
				dynamicImageInstance.SetSize(pageContext.Common.Pagination.PhysicalPageWidth, pageContext.Common.Pagination.PhysicalPageHeight);
			}
			return dynamicImageInstance.GetImage((DynamicImageInstance.ImageType)(pageContext.EMFDynamicImages ? 1 : 0), out actionImageMaps);
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
					binaryWriter.Write(this.ElementToken);
					base.WriteElementProps(binaryWriter, rplWriter, pageContext, position + 1);
					base.m_offset = baseStream.Position;
					binaryWriter.Write((byte)254);
					binaryWriter.Write(position);
					binaryWriter.Write((byte)255);
				}
				else if (base.m_rplElement == null)
				{
					base.m_rplElement = this.CreateRPLItem();
					base.WriteElementProps(base.m_rplElement.ElementProps, pageContext);
				}
			}
		}

		internal override void WriteCustomNonSharedItemProps(BinaryWriter spbifWriter, PageContext pageContext)
		{
			ActionInfoWithDynamicImageMapCollection actionInfoWithDynamicImageMapCollection = null;
			Stream stream = this.LoadDynamicImage(out actionInfoWithDynamicImageMapCollection, pageContext);
			if (stream != null)
			{
				spbifWriter.Write((byte)39);
				spbifWriter.Write((int)stream.Length);
				byte[] array = new byte[4096];
				stream.Position = 0L;
				for (int num = stream.Read(array, 0, array.Length); num != 0; num = stream.Read(array, 0, array.Length))
				{
					spbifWriter.Write(array, 0, num);
				}
			}
		}

		internal override void WriteCustomNonSharedItemProps(RPLElementProps nonSharedProps, PageContext pageContext)
		{
			ActionInfoWithDynamicImageMapCollection actionInfoWithDynamicImageMapCollection = null;
			RPLDynamicImageProps rPLDynamicImageProps = (RPLDynamicImageProps)nonSharedProps;
			rPLDynamicImageProps.DynamicImageContent = this.LoadDynamicImage(out actionInfoWithDynamicImageMapCollection, pageContext);
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps styleProps, Style style, PageContext pageContext)
		{
			PageItem.WriteStyleProp(style, styleProps, StyleAttributeNames.BackgroundColor, 34);
		}

		internal override void WriteItemNonSharedStyleProps(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			if (styleAtt == StyleAttributeNames.BackgroundColor)
			{
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
			}
		}

		internal override void WriteItemNonSharedStyleProps(RPLStyleProps styleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			if (styleAtt == StyleAttributeNames.BackgroundColor)
			{
				base.WriteStyleProp(styleDef, style, styleProps, StyleAttributeNames.BackgroundColor, 34);
			}
		}

		internal override void WriteBorderProps(BinaryWriter spbifWriter, Style style)
		{
			if (!this.SpecialBorderHandling)
			{
				base.WriteBorderProps(spbifWriter, style);
			}
		}

		internal override void WriteBorderProps(RPLStyleProps rplStyleProps, Style style)
		{
			if (!this.SpecialBorderHandling)
			{
				base.WriteBorderProps(rplStyleProps, style);
			}
		}

		internal override void WriteNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			if (!this.SpecialBorderHandling)
			{
				base.WriteNonSharedStyleProp(spbifWriter, styleDef, style, styleAttribute, pageContext);
			}
			else
			{
				this.WriteItemNonSharedStyleProps(spbifWriter, styleDef, style, styleAttribute, pageContext);
			}
		}

		internal override void WriteNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			if (!this.SpecialBorderHandling)
			{
				base.WriteNonSharedStyleProp(rplStyleProps, styleDef, style, styleAttribute, pageContext);
			}
			else
			{
				this.WriteItemNonSharedStyleProps(rplStyleProps, styleDef, style, styleAttribute, pageContext);
			}
		}
	}
}
