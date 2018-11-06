using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class PlayAxis : Navigation
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = PlayAxis.GetDeclaration();

		private Slider m_slider;

		private DockingOption m_dockingOption;

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

		internal DockingOption DockingOption
		{
			get
			{
				return this.m_dockingOption;
			}
			set
			{
				this.m_dockingOption = value;
			}
		}

		internal override void Initialize(Tablix tablix, InitializationContext context)
		{
			if (this.m_slider != null)
			{
				this.m_slider.Initialize(tablix, context);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Slider, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Slider));
			list.Add(new MemberInfo(MemberName.DockingOption, Token.Enum));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PlayAxis, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Navigation, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(PlayAxis.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Slider:
					writer.Write(this.m_slider);
					break;
				case MemberName.DockingOption:
					writer.WriteEnum((int)this.m_dockingOption);
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
			reader.RegisterDeclaration(PlayAxis.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Slider:
					this.m_slider = reader.ReadRIFObject<Slider>();
					break;
				case MemberName.DockingOption:
					this.m_dockingOption = (DockingOption)reader.ReadEnum();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PlayAxis;
		}
	}
}
