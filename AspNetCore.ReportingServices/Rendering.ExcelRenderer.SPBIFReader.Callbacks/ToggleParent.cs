using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.SPBIFReader.Callbacks
{
	internal class ToggleParent : IStorable, IPersistable
	{
		internal int m_top;

		internal int m_left;

		internal int m_width;

		internal int m_height;

		[NonSerialized]
		private static Declaration m_declaration = ToggleParent.GetDeclaration();

		internal int Top
		{
			get
			{
				return this.m_top;
			}
		}

		internal int Left
		{
			get
			{
				return this.m_left;
			}
		}

		internal int Width
		{
			get
			{
				return this.m_width;
			}
		}

		internal int Height
		{
			get
			{
				return this.m_height;
			}
		}

		public int Size
		{
			get
			{
				return 16;
			}
		}

		internal ToggleParent()
		{
		}

		internal ToggleParent(int top, int left, int width, int height)
		{
			this.m_top = top;
			this.m_left = left;
			this.m_width = width;
			this.m_height = height;
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ToggleParent.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Top:
					writer.Write(this.m_top);
					break;
				case MemberName.Left:
					writer.Write(this.m_left);
					break;
				case MemberName.Width:
					writer.Write(this.m_width);
					break;
				case MemberName.Height:
					writer.Write(this.m_height);
					break;
				default:
					RSTrace.ExcelRendererTracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ToggleParent.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Top:
					this.m_top = reader.ReadInt32();
					break;
				case MemberName.Left:
					this.m_left = reader.ReadInt32();
					break;
				case MemberName.Width:
					this.m_width = reader.ReadInt32();
					break;
				case MemberName.Height:
					this.m_height = reader.ReadInt32();
					break;
				default:
					RSTrace.ExcelRendererTracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public ObjectType GetObjectType()
		{
			return ObjectType.ToggleParent;
		}

		internal static Declaration GetDeclaration()
		{
			if (ToggleParent.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Top, Token.Int32));
				list.Add(new MemberInfo(MemberName.Left, Token.Int32));
				list.Add(new MemberInfo(MemberName.Width, Token.Int32));
				list.Add(new MemberInfo(MemberName.Height, Token.Int32));
				return new Declaration(ObjectType.ToggleParent, ObjectType.None, list);
			}
			return ToggleParent.m_declaration;
		}
	}
}
