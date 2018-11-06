using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class HeaderFooterReferences : BaseInterleaver, OpenXmlSectionPropertiesModel.IHeaderFooterReferences
	{
		[NonSerialized]
		private static Declaration _declaration;

		public string Header
		{
			get;
			set;
		}

		public string Footer
		{
			get;
			set;
		}

		public string FirstPageHeader
		{
			get;
			set;
		}

		public string FirstPageFooter
		{
			get;
			set;
		}

		public override int Size
		{
			get
			{
				return base.Size + ItemSizes.SizeOf(this.Header) + ItemSizes.SizeOf(this.Footer) + ItemSizes.SizeOf(this.FirstPageHeader) + ItemSizes.SizeOf(this.FirstPageFooter);
			}
		}

		public HeaderFooterReferences(int index, long location, string footer, string header, string firstPageHeader, string firstPageFooter)
			: base(index, location)
		{
			this.Footer = footer;
			this.Header = header;
			this.FirstPageHeader = firstPageHeader;
			this.FirstPageFooter = firstPageFooter;
		}

		public HeaderFooterReferences()
		{
		}

		static HeaderFooterReferences()
		{
			HeaderFooterReferences._declaration = new Declaration(ObjectType.WordOpenXmlHeaderFooterReferences, ObjectType.WordOpenXmlBaseInterleaver, new List<MemberInfo>
			{
				new MemberInfo(MemberName.Header, Token.String),
				new MemberInfo(MemberName.Footer, Token.String),
				new MemberInfo(MemberName.FirstPageHeader, Token.String),
				new MemberInfo(MemberName.FirstPageFooter, Token.String)
			});
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(HeaderFooterReferences.GetDeclaration());
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Header:
					writer.Write(this.Header);
					break;
				case MemberName.Footer:
					writer.Write(this.Footer);
					break;
				case MemberName.FirstPageHeader:
					writer.Write(this.FirstPageHeader);
					break;
				case MemberName.FirstPageFooter:
					writer.Write(this.FirstPageFooter);
					break;
				default:
					WordOpenXmlUtils.FailSerializable();
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(HeaderFooterReferences.GetDeclaration());
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Header:
					this.Header = reader.ReadString();
					break;
				case MemberName.Footer:
					this.Footer = reader.ReadString();
					break;
				case MemberName.FirstPageHeader:
					this.FirstPageHeader = reader.ReadString();
					break;
				case MemberName.FirstPageFooter:
					this.FirstPageFooter = reader.ReadString();
					break;
				default:
					WordOpenXmlUtils.FailSerializable();
					break;
				}
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.WordOpenXmlHeaderFooterReferences;
		}

		internal new static Declaration GetDeclaration()
		{
			return HeaderFooterReferences._declaration;
		}

		public override void Write(TextWriter output)
		{
		}
	}
}
