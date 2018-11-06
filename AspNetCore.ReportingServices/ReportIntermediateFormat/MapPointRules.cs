using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapPointRules : IPersistable
	{
		[NonSerialized]
		private MapPointRulesExprHost m_exprHost;

		[NonSerialized]
		private MapPointRulesExprHost m_exprHostMapMember;

		[Reference]
		private Map m_map;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapPointRules.GetDeclaration();

		private MapSizeRule m_mapSizeRule;

		private MapColorRule m_mapColorRule;

		private MapMarkerRule m_mapMarkerRule;

		internal MapSizeRule MapSizeRule
		{
			get
			{
				return this.m_mapSizeRule;
			}
			set
			{
				this.m_mapSizeRule = value;
			}
		}

		internal MapColorRule MapColorRule
		{
			get
			{
				return this.m_mapColorRule;
			}
			set
			{
				this.m_mapColorRule = value;
			}
		}

		internal MapMarkerRule MapMarkerRule
		{
			get
			{
				return this.m_mapMarkerRule;
			}
			set
			{
				this.m_mapMarkerRule = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return this.m_map.Name;
			}
		}

		internal MapPointRulesExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal MapPointRules()
		{
		}

		internal MapPointRules(Map map)
		{
			this.m_map = map;
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapPointRulesStart();
			if (this.m_mapSizeRule != null)
			{
				this.m_mapSizeRule.Initialize(context);
			}
			if (this.m_mapColorRule != null)
			{
				this.m_mapColorRule.Initialize(context);
			}
			if (this.m_mapMarkerRule != null)
			{
				this.m_mapMarkerRule.Initialize(context);
			}
			context.ExprHostBuilder.MapPointRulesEnd();
		}

		internal void InitializeMapMember(InitializationContext context)
		{
			context.ExprHostBuilder.MapPointRulesStart();
			if (this.m_mapSizeRule != null)
			{
				this.m_mapSizeRule.InitializeMapMember(context);
			}
			if (this.m_mapColorRule != null)
			{
				this.m_mapColorRule.InitializeMapMember(context);
			}
			if (this.m_mapMarkerRule != null)
			{
				this.m_mapMarkerRule.InitializeMapMember(context);
			}
			context.ExprHostBuilder.MapPointRulesEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			MapPointRules mapPointRules = (MapPointRules)base.MemberwiseClone();
			mapPointRules.m_map = context.CurrentMapClone;
			if (this.m_mapSizeRule != null)
			{
				mapPointRules.m_mapSizeRule = (MapSizeRule)this.m_mapSizeRule.PublishClone(context);
			}
			if (this.m_mapColorRule != null)
			{
				mapPointRules.m_mapColorRule = (MapColorRule)this.m_mapColorRule.PublishClone(context);
			}
			if (this.m_mapMarkerRule != null)
			{
				mapPointRules.m_mapMarkerRule = (MapMarkerRule)this.m_mapMarkerRule.PublishClone(context);
			}
			return mapPointRules;
		}

		internal void SetExprHost(MapPointRulesExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			if (this.m_mapSizeRule != null && this.ExprHost.MapSizeRuleHost != null)
			{
				this.m_mapSizeRule.SetExprHost(this.ExprHost.MapSizeRuleHost, reportObjectModel);
			}
			if (this.m_mapColorRule != null && this.ExprHost.MapColorRuleHost != null)
			{
				this.m_mapColorRule.SetExprHost(this.ExprHost.MapColorRuleHost, reportObjectModel);
			}
			if (this.m_mapMarkerRule != null && this.ExprHost.MapMarkerRuleHost != null)
			{
				this.m_mapMarkerRule.SetExprHost(this.ExprHost.MapMarkerRuleHost, reportObjectModel);
			}
		}

		internal void SetExprHostMapMember(MapPointRulesExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHostMapMember = exprHost;
			this.m_exprHostMapMember.SetReportObjectModel(reportObjectModel);
			if (this.m_mapSizeRule != null && this.m_exprHostMapMember.MapSizeRuleHost != null)
			{
				this.m_mapSizeRule.SetExprHostMapMember(this.m_exprHostMapMember.MapSizeRuleHost, reportObjectModel);
			}
			if (this.m_mapColorRule != null && this.m_exprHostMapMember.MapColorRuleHost != null)
			{
				this.m_mapColorRule.SetExprHostMapMember(this.m_exprHostMapMember.MapColorRuleHost, reportObjectModel);
			}
			if (this.m_mapMarkerRule != null && this.m_exprHostMapMember.MapMarkerRuleHost != null)
			{
				this.m_mapMarkerRule.SetExprHostMapMember(this.m_exprHostMapMember.MapMarkerRuleHost, reportObjectModel);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapSizeRule, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSizeRule));
			list.Add(new MemberInfo(MemberName.MapColorRule, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorRule));
			list.Add(new MemberInfo(MemberName.MapMarkerRule, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMarkerRule));
			list.Add(new MemberInfo(MemberName.Map, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointRules, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(MapPointRules.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Map:
					writer.WriteReference(this.m_map);
					break;
				case MemberName.MapSizeRule:
					writer.Write(this.m_mapSizeRule);
					break;
				case MemberName.MapColorRule:
					writer.Write(this.m_mapColorRule);
					break;
				case MemberName.MapMarkerRule:
					writer.Write(this.m_mapMarkerRule);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(MapPointRules.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Map:
					this.m_map = reader.ReadReference<Map>(this);
					break;
				case MemberName.MapSizeRule:
					this.m_mapSizeRule = (MapSizeRule)reader.ReadRIFObject();
					break;
				case MemberName.MapColorRule:
					this.m_mapColorRule = (MapColorRule)reader.ReadRIFObject();
					break;
				case MemberName.MapMarkerRule:
					this.m_mapMarkerRule = (MapMarkerRule)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(MapPointRules.m_Declaration.ObjectType, out list))
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

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointRules;
		}
	}
}
