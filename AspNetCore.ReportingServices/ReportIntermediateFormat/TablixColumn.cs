using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class TablixColumn : IDOwner, IPersistable
	{
		private string m_width;

		private double m_widthValue;

		[NonSerialized]
		private bool m_forAutoSubtotal;

		[NonSerialized]
		private static readonly Declaration m_Declaration = TablixColumn.GetDeclaration();

		internal string Width
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

		internal double WidthValue
		{
			get
			{
				return this.m_widthValue;
			}
			set
			{
				this.m_widthValue = value;
			}
		}

		internal bool ForAutoSubtotal
		{
			get
			{
				return this.m_forAutoSubtotal;
			}
			set
			{
				this.m_forAutoSubtotal = value;
			}
		}

		internal TablixColumn()
		{
		}

		internal TablixColumn(int id)
			: base(id)
		{
		}

		internal void Initialize(InitializationContext context)
		{
			this.m_widthValue = context.ValidateSize(this.m_width, "Width");
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			TablixColumn tablixColumn = (TablixColumn)base.PublishClone(context);
			if (this.m_width != null)
			{
				tablixColumn.m_width = (string)this.m_width.Clone();
			}
			return tablixColumn;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Width, Token.String));
			list.Add(new MemberInfo(MemberName.WidthValue, Token.Double));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixColumn, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(TablixColumn.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Width:
					writer.Write(this.m_width);
					break;
				case MemberName.WidthValue:
					writer.Write(this.m_widthValue);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(TablixColumn.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Width:
					this.m_width = reader.ReadString();
					break;
				case MemberName.WidthValue:
					this.m_widthValue = reader.ReadDouble();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixColumn;
		}
	}
}
