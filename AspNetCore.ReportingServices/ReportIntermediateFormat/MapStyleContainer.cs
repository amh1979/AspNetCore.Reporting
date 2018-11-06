using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal abstract class MapStyleContainer : IStyleContainer, IPersistable
	{
		[Reference]
		protected Map m_map;

		protected Style m_styleClass;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapStyleContainer.GetDeclaration();

		public Style StyleClass
		{
			get
			{
				return this.m_styleClass;
			}
			set
			{
				this.m_styleClass = value;
			}
		}

		IInstancePath IStyleContainer.InstancePath
		{
			get
			{
				return this.m_map;
			}
		}

		AspNetCore.ReportingServices.ReportProcessing.ObjectType IStyleContainer.ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map;
			}
		}

		string IStyleContainer.Name
		{
			get
			{
				return this.m_map.Name;
			}
		}

		internal MapStyleContainer()
		{
		}

		internal MapStyleContainer(Map map)
		{
			this.m_map = map;
		}

		internal virtual void Initialize(InitializationContext context)
		{
			if (this.m_styleClass != null)
			{
				this.m_styleClass.Initialize(context);
			}
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context)
		{
			MapStyleContainer mapStyleContainer = (MapStyleContainer)base.MemberwiseClone();
			mapStyleContainer.m_map = context.CurrentMapClone;
			if (this.m_styleClass != null)
			{
				mapStyleContainer.m_styleClass = (Style)this.m_styleClass.PublishClone(context);
			}
			return mapStyleContainer;
		}

		internal virtual void SetExprHost(StyleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && null != reportObjectModel);
			exprHost.SetReportObjectModel(reportObjectModel);
			if (this.m_styleClass != null)
			{
				this.m_styleClass.SetStyleExprHost(exprHost);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Map, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			list.Add(new MemberInfo(MemberName.StyleClass, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapStyleContainer, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(MapStyleContainer.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Map:
					writer.WriteReference(this.m_map);
					break;
				case MemberName.StyleClass:
					writer.Write(this.m_styleClass);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(MapStyleContainer.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Map:
					this.m_map = reader.ReadReference<Map>(this);
					break;
				case MemberName.StyleClass:
					this.m_styleClass = (Style)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(MapStyleContainer.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.Map)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_map = (Map)referenceableItems[item.RefID];
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}
		}

		public virtual AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapStyleContainer;
		}
	}
}
