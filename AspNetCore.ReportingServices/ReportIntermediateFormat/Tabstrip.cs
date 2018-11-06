using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Tabstrip : Navigation
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = Tabstrip.GetDeclaration();

		private NavigationItem m_navigationItem;

		private Slider m_slider;

		internal Slider Slider
		{
			get
			{
				return this.m_slider;
			}
			set
			{
				this.m_slider = value;
			}
		}

		internal NavigationItem NavigationItem
		{
			get
			{
				return this.m_navigationItem;
			}
			set
			{
				this.m_navigationItem = value;
			}
		}

		internal override void Initialize(Tablix tablix, InitializationContext context)
		{
			if (this.m_slider != null)
			{
				this.m_slider.Initialize(tablix, context);
			}
			if (this.m_navigationItem != null)
			{
				this.m_navigationItem.Initialize(tablix, context, "Tabstrip");
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.NavigationItem, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.NavigationItem));
			list.Add(new MemberInfo(MemberName.Slider, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Slider));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Tabstrip, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Navigation, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(Tabstrip.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.NavigationItem:
					writer.Write(this.m_navigationItem);
					break;
				case MemberName.Slider:
					writer.Write(this.m_slider);
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
			reader.RegisterDeclaration(Tabstrip.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.NavigationItem:
					this.m_navigationItem = reader.ReadRIFObject<NavigationItem>();
					break;
				case MemberName.Slider:
					this.m_slider = reader.ReadRIFObject<Slider>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Tabstrip;
		}
	}
}
