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
	internal sealed class MapCustomColorRule : MapColorRule, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapCustomColorRule.GetDeclaration();

		private List<MapCustomColor> m_mapCustomColors;

		internal List<MapCustomColor> MapCustomColors
		{
			get
			{
				return this.m_mapCustomColors;
			}
			set
			{
				this.m_mapCustomColors = value;
			}
		}

		internal new MapCustomColorRuleExprHost ExprHost
		{
			get
			{
				return (MapCustomColorRuleExprHost)base.m_exprHost;
			}
		}

		internal MapCustomColorRule()
		{
		}

		internal MapCustomColorRule(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapCustomColorRuleStart();
			base.Initialize(context);
			if (this.m_mapCustomColors != null)
			{
				for (int i = 0; i < this.m_mapCustomColors.Count; i++)
				{
					this.m_mapCustomColors[i].Initialize(context, i);
				}
			}
			context.ExprHostBuilder.MapCustomColorRuleEnd();
		}

		internal override void InitializeMapMember(InitializationContext context)
		{
			context.ExprHostBuilder.MapCustomColorRuleStart();
			base.InitializeMapMember(context);
			context.ExprHostBuilder.MapCustomColorRuleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapCustomColorRule mapCustomColorRule = (MapCustomColorRule)base.PublishClone(context);
			if (this.m_mapCustomColors != null)
			{
				mapCustomColorRule.m_mapCustomColors = new List<MapCustomColor>(this.m_mapCustomColors.Count);
				{
					foreach (MapCustomColor mapCustomColor in this.m_mapCustomColors)
					{
						mapCustomColorRule.m_mapCustomColors.Add((MapCustomColor)mapCustomColor.PublishClone(context));
					}
					return mapCustomColorRule;
				}
			}
			return mapCustomColorRule;
		}

		internal override void SetExprHost(MapAppearanceRuleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			IList<MapCustomColorExprHost> mapCustomColorsHostsRemotable = this.ExprHost.MapCustomColorsHostsRemotable;
			if (this.m_mapCustomColors != null && mapCustomColorsHostsRemotable != null)
			{
				for (int i = 0; i < this.m_mapCustomColors.Count; i++)
				{
					MapCustomColor mapCustomColor = this.m_mapCustomColors[i];
					if (mapCustomColor != null && mapCustomColor.ExpressionHostID > -1)
					{
						mapCustomColor.SetExprHost(mapCustomColorsHostsRemotable[mapCustomColor.ExpressionHostID], reportObjectModel);
					}
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapCustomColors, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapCustomColor));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapCustomColorRule, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorRule, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapCustomColorRule.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.MapCustomColors)
				{
					writer.Write(this.m_mapCustomColors);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(MapCustomColorRule.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.MapCustomColors)
				{
					this.m_mapCustomColors = reader.ReadGenericListOfRIFObjects<MapCustomColor>();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapCustomColorRule;
		}
	}
}
