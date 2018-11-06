using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapTile : IPersistable
	{
		private int m_exprHostID = -1;

		[NonSerialized]
		private MapTileExprHost m_exprHost;

		[Reference]
		private Map m_map;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapTile.GetDeclaration();

		private string m_name;

		private string m_tileData;

		private string m_mIMEType;

		internal string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		internal string TileData
		{
			get
			{
				return this.m_tileData;
			}
			set
			{
				this.m_tileData = value;
			}
		}

		internal string MIMEType
		{
			get
			{
				return this.m_mIMEType;
			}
			set
			{
				this.m_mIMEType = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return this.m_map.Name;
			}
		}

		internal MapTileExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal int ExpressionHostID
		{
			get
			{
				return this.m_exprHostID;
			}
		}

		internal MapTile()
		{
		}

		internal MapTile(Map map)
		{
			this.m_map = map;
		}

		internal void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.MapTileStart(index.ToString(CultureInfo.InvariantCulture.NumberFormat));
			this.m_exprHostID = context.ExprHostBuilder.MapTileEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			MapTile mapTile = (MapTile)base.MemberwiseClone();
			mapTile.m_map = context.CurrentMapClone;
			return mapTile;
		}

		internal void SetExprHost(MapTileExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.TileData, Token.String));
			list.Add(new MemberInfo(MemberName.MIMEType, Token.String));
			list.Add(new MemberInfo(MemberName.Map, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapTile, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(MapTile.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Map:
					writer.WriteReference(this.m_map);
					break;
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.TileData:
					writer.Write(this.m_tileData);
					break;
				case MemberName.MIMEType:
					writer.Write(this.m_mIMEType);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(MapTile.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Map:
					this.m_map = reader.ReadReference<Map>(this);
					break;
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.TileData:
					this.m_tileData = reader.ReadString();
					break;
				case MemberName.MIMEType:
					this.m_mIMEType = reader.ReadString();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
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
			if (memberReferencesCollection.TryGetValue(MapTile.m_Declaration.ObjectType, out list))
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapTile;
		}
	}
}
