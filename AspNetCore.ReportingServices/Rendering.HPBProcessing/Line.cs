using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class Line : PageItem, IStorable, IPersistable
	{
		private static Declaration m_declaration = Line.GetDeclaration();

		internal Line()
		{
		}

		internal Line(AspNetCore.ReportingServices.OnDemandReportRendering.Line source)
			: base(source)
		{
			base.m_itemPageSizes = new ItemSizes(source);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(Line.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(false, string.Empty);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(Line.m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(false, string.Empty);
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.Line;
		}

		internal new static Declaration GetDeclaration()
		{
			if (Line.m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ObjectType.Line, ObjectType.PageItem, memberInfoList);
			}
			return Line.m_declaration;
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
					binaryWriter.Write((byte)8);
					base.WriteElementProps(binaryWriter, rplWriter, pageContext, position + 1);
					base.m_offset = binaryWriter.BaseStream.Position;
					binaryWriter.Write((byte)254);
					binaryWriter.Write(position);
					binaryWriter.Write((byte)255);
				}
				else if (base.m_rplElement == null)
				{
					base.m_rplElement = new RPLLine();
					base.WriteElementProps(base.m_rplElement.ElementProps, pageContext);
				}
			}
		}

		internal override void WriteCustomSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			if (((AspNetCore.ReportingServices.OnDemandReportRendering.Line)base.m_source).Slant)
			{
				spbifWriter.Write((byte)24);
				spbifWriter.Write(true);
			}
		}

		internal override void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, PageContext pageContext)
		{
			((RPLLinePropsDef)sharedProps).Slant = ((AspNetCore.ReportingServices.OnDemandReportRendering.Line)base.m_source).Slant;
		}
	}
}
