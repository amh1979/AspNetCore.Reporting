using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Slider : IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = Slider.GetDeclaration();

		private bool m_hidden;

		private LabelData m_labelData;

		internal bool Hidden
		{
			get
			{
				return this.m_hidden;
			}
			set
			{
				this.m_hidden = value;
			}
		}

		internal LabelData LabelData
		{
			get
			{
				return this.m_labelData;
			}
			set
			{
				this.m_labelData = value;
			}
		}

		internal void Initialize(Tablix tablix, InitializationContext context)
		{
			if (this.m_labelData != null)
			{
				this.m_labelData.Initialize(tablix, context);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Hidden, Token.Boolean));
			list.Add(new MemberInfo(MemberName.LabelData, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LabelData));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Slider, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(Slider.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Hidden:
					writer.Write(this.m_hidden);
					break;
				case MemberName.LabelData:
					writer.Write(this.m_labelData);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(Slider.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Hidden:
					this.m_hidden = reader.ReadBoolean();
					break;
				case MemberName.LabelData:
					this.m_labelData = reader.ReadRIFObject<LabelData>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Slider;
		}
	}
}
