using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeDetailObj : RuntimeHierarchyObj
	{
		protected IReference<IScope> m_outerScope;

		[StaticReference]
		protected AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion m_dataRegionDef;

		protected ScalableList<DataFieldRow> m_dataRows;

		protected List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[]> m_rvValueList;

		protected List<string> m_runningValuesInGroup;

		protected List<string> m_previousValuesInGroup;

		protected Dictionary<string, IReference<RuntimeGroupRootObj>> m_groupCollection;

		protected DataActions m_outerDataAction;

		private static Declaration m_declaration = RuntimeDetailObj.GetDeclaration();

		public override int Size
		{
			get
			{
				return base.Size + ItemSizes.SizeOf(this.m_outerScope) + ItemSizes.ReferenceSize + ItemSizes.SizeOf(this.m_dataRows) + ItemSizes.SizeOf(this.m_rvValueList) + ItemSizes.SizeOf(this.m_runningValuesInGroup) + ItemSizes.SizeOf(this.m_previousValuesInGroup) + ItemSizes.SizeOf(this.m_groupCollection) + 4;
			}
		}

		protected RuntimeDetailObj(IReference<IScope> outerScope, AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, DataActions dataAction, OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(odpContext, objectType, (outerScope == null) ? dataRegionDef.InnerGroupingDynamicMemberCount : (outerScope.Value().Depth + 1))
		{
			base.m_hierarchyRoot = (RuntimeDetailObjReference)base.SelfReference;
			this.m_outerScope = outerScope;
			this.m_dataRegionDef = dataRegionDef;
			this.m_outerDataAction = dataAction;
		}

		internal RuntimeDetailObj(RuntimeDetailObj detailRoot, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(detailRoot.OdpContext, objectType, detailRoot.Depth)
		{
			base.m_hierarchyRoot = (RuntimeDetailObjReference)detailRoot.SelfReference;
			this.m_outerScope = detailRoot.m_outerScope;
			this.m_dataRegionDef = detailRoot.m_dataRegionDef;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RuntimeDetailObj.m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.OuterScope:
					writer.Write(this.m_outerScope);
					break;
				case MemberName.DataRegionDef:
				{
					int value = scalabilityCache.StoreStaticReference(this.m_dataRegionDef);
					writer.Write(value);
					break;
				}
				case MemberName.DataRows:
					writer.Write(this.m_dataRows);
					break;
				case MemberName.RunningValueValues:
					writer.Write(this.m_rvValueList);
					break;
				case MemberName.RunningValuesInGroup:
					writer.WriteListOfPrimitives(this.m_runningValuesInGroup);
					break;
				case MemberName.PreviousValuesInGroup:
					writer.WriteListOfPrimitives(this.m_previousValuesInGroup);
					break;
				case MemberName.GroupCollection:
					writer.Write(this.m_groupCollection);
					break;
				case MemberName.OuterDataAction:
					writer.Write(this.m_outerDataAction);
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
			reader.RegisterDeclaration(RuntimeDetailObj.m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.OuterScope:
					this.m_outerScope = (IReference<IScope>)reader.ReadRIFObject();
					break;
				case MemberName.DataRegionDef:
				{
					int id = reader.ReadInt32();
					this.m_dataRegionDef = (AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.DataRows:
					this.m_dataRows = reader.ReadRIFObject<ScalableList<DataFieldRow>>();
					break;
				case MemberName.RunningValueValues:
					this.m_rvValueList = reader.ReadListOfRIFObjectArrays<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult>();
					break;
				case MemberName.RunningValuesInGroup:
					this.m_runningValuesInGroup = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.PreviousValuesInGroup:
					this.m_previousValuesInGroup = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.GroupCollection:
					this.m_groupCollection = reader.ReadStringRIFObjectDictionary<IReference<RuntimeGroupRootObj>>();
					break;
				case MemberName.OuterDataAction:
					this.m_outerDataAction = (DataActions)reader.ReadEnum();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDetailObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (RuntimeDetailObj.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.OuterScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IScopeReference));
				list.Add(new MemberInfo(MemberName.DataRegionDef, Token.Int32));
				list.Add(new MemberInfo(MemberName.DataRows, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow));
				list.Add(new MemberInfo(MemberName.RunningValueValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray));
				list.Add(new MemberInfo(MemberName.RunningValuesInGroup, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
				list.Add(new MemberInfo(MemberName.PreviousValuesInGroup, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
				list.Add(new MemberInfo(MemberName.GroupCollection, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupRootObjReference));
				list.Add(new MemberInfo(MemberName.OuterDataAction, Token.Enum));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDetailObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObj, list);
			}
			return RuntimeDetailObj.m_declaration;
		}
	}
}
