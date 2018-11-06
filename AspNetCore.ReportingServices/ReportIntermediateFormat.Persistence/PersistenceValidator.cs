using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal class PersistenceValidator
	{
		[Conditional("DEBUG")]
		internal static void VerifyReadOrWrite(MemberInfo CurrentMember, PersistMethod persistMethod)
		{
		}

		[Conditional("DEBUG")]
		internal static void VerifyReadOrWrite(MemberInfo currentMember, PersistMethod persistMethod, Token primitiveType, ObjectType containedType)
		{
			switch (persistMethod)
			{
			case PersistMethod.PrimitiveGenericList:
			case PersistMethod.PrimitiveList:
				Global.Tracer.Assert(currentMember.ObjectType == ObjectType.PrimitiveList);
				Global.Tracer.Assert(currentMember.Token == primitiveType);
				break;
			case PersistMethod.PrimitiveTypedArray:
				Global.Tracer.Assert(currentMember.ObjectType == ObjectType.PrimitiveTypedArray);
				Global.Tracer.Assert(currentMember.Token == primitiveType);
				break;
			case PersistMethod.PrimitiveArray:
				Global.Tracer.Assert(currentMember.ObjectType == ObjectType.PrimitiveArray);
				Global.Tracer.Assert(currentMember.Token == primitiveType || currentMember.ContainedType == containedType);
				break;
			case PersistMethod.GenericListOfReferences:
			case PersistMethod.ListOfReferences:
				Global.Tracer.Assert(currentMember.ObjectType == ObjectType.RIFObjectList);
				Global.Tracer.Assert(currentMember.Token == Token.Reference);
				break;
			case PersistMethod.GenericListOfGlobalReferences:
			case PersistMethod.ListOfGlobalReferences:
				Global.Tracer.Assert(currentMember.ObjectType == ObjectType.RIFObjectList);
				Global.Tracer.Assert(currentMember.Token == Token.GlobalReference);
				break;
			case PersistMethod.SerializableArray:
				Global.Tracer.Assert(currentMember.ObjectType == ObjectType.SerializableArray);
				Global.Tracer.Assert(currentMember.Token == Token.Serializable);
				break;
			default:
				Global.Tracer.Assert(false);
				break;
			}
		}

		[Conditional("DEBUG")]
		internal static void VerifyDeclaredType(MemberInfo currentMember, ObjectType persistedType, Dictionary<ObjectType, Declaration> declarations, bool verify)
		{
		}

		[Conditional("DEBUG")]
		internal static void VerifyDeclaredType(MemberInfo currentMember, ObjectType persistedType, Dictionary<ObjectType, Declaration> declarations)
		{
			if (declarations != null && currentMember.ContainedType != ObjectType.RIFObjectArray && currentMember.ContainedType != ObjectType.RIFObjectList && !PersistenceValidator.VerifyDeclaredType(currentMember.ObjectType, persistedType, declarations) && !PersistenceValidator.VerifyDeclaredType(currentMember.ContainedType, persistedType, declarations) && !PersistenceValidator.CheckSpecialCase(currentMember, persistedType))
			{
				Global.Tracer.Assert(false);
			}
		}

		private static bool VerifyDeclaredType(ObjectType declaredType, ObjectType persistedType, Dictionary<ObjectType, Declaration> declarations)
		{
			if (persistedType != declaredType && declaredType != ObjectType.RIFObject)
			{
				Declaration declaration = default(Declaration);
				if (declarations.TryGetValue(persistedType, out declaration))
				{
					while (declaration.BaseObjectType != ObjectType.None)
					{
						if (declaration.BaseObjectType == declaredType)
						{
							return true;
						}
						declaration = declarations[declaration.BaseObjectType];
					}
					return false;
				}
				return true;
			}
			return true;
		}

		internal static bool CheckSpecialCase(MemberInfo currentMember, ObjectType persistedType)
		{
			for (int i = 0; i < 2; i++)
			{
				switch ((i != 0) ? currentMember.ContainedType : currentMember.ObjectType)
				{
				case ObjectType.ISortFilterScope:
					switch (persistedType)
					{
					case ObjectType.DataRegion:
					case ObjectType.Grouping:
					case ObjectType.DataSet:
					case ObjectType.CustomReportItem:
					case ObjectType.Tablix:
					case ObjectType.Chart:
					case ObjectType.GaugePanel:
					case ObjectType.MapDataRegion:
						return true;
					}
					break;
				case ObjectType.IHierarchyObj:
					switch (persistedType)
					{
					case ObjectType.RuntimeSortHierarchyObj:
					case ObjectType.SortFilterExpressionScopeObj:
					case ObjectType.SortExpressionScopeInstanceHolder:
					case ObjectType.RuntimeHierarchyObj:
					case ObjectType.RuntimeDataTablixGroupRootObj:
					case ObjectType.RuntimeDataTablixObj:
					case ObjectType.RuntimeTablixObj:
					case ObjectType.RuntimeChartObj:
					case ObjectType.RuntimeCriObj:
					case ObjectType.RuntimeTablixGroupLeafObj:
					case ObjectType.RuntimeChartCriGroupLeafObj:
					case ObjectType.RuntimeOnDemandDataSetObj:
					case ObjectType.IHierarchyObj:
					case ObjectType.RuntimeRDLDataRegionObj:
					case ObjectType.RuntimeGroupLeafObj:
					case ObjectType.RuntimeGroupObj:
					case ObjectType.RuntimeDetailObj:
					case ObjectType.RuntimeGroupRootObj:
					case ObjectType.RuntimeChartCriObj:
					case ObjectType.RuntimeDataTablixGroupLeafObj:
					case ObjectType.RuntimeGaugePanelObj:
					case ObjectType.RuntimeMapDataRegionObj:
					case ObjectType.RuntimeDataRowSortHierarchyObj:
						return true;
					}
					break;
				case ObjectType.IInScopeEventSource:
					if (persistedType != ObjectType.TextBox)
					{
						break;
					}
					return true;
				case ObjectType.IVisibilityOwner:
					switch (persistedType)
					{
					case ObjectType.ReportItem:
					case ObjectType.Line:
					case ObjectType.Rectangle:
					case ObjectType.Image:
					case ObjectType.TextBox:
					case ObjectType.SubReport:
					case ObjectType.CustomReportItem:
					case ObjectType.Tablix:
					case ObjectType.TablixMember:
					case ObjectType.Chart:
					case ObjectType.GaugePanel:
					case ObjectType.Map:
						return true;
					}
					break;
				case ObjectType.DataAggregate:
					switch (persistedType)
					{
					case ObjectType.Aggregate:
					case ObjectType.First:
					case ObjectType.Last:
					case ObjectType.Sum:
					case ObjectType.Avg:
					case ObjectType.Max:
					case ObjectType.Min:
					case ObjectType.Count:
					case ObjectType.CountDistinct:
					case ObjectType.CountRows:
					case ObjectType.Var:
					case ObjectType.StDev:
					case ObjectType.VarP:
					case ObjectType.StDevP:
					case ObjectType.Previous:
					case ObjectType.Union:
						return true;
					}
					break;
				case ObjectType.IScalableDictionaryEntry:
					switch (persistedType)
					{
					case ObjectType.ScalableDictionaryNodeReference:
					case ObjectType.ScalableDictionaryValues:
					case ObjectType.ScalableDictionaryNode:
						return true;
					}
					break;
				case ObjectType.ISortDataHolder:
					switch (persistedType)
					{
					case ObjectType.RuntimeTablixGroupLeafObj:
					case ObjectType.RuntimeChartCriGroupLeafObj:
					case ObjectType.RuntimeSortDataHolder:
						return true;
					}
					break;
				case ObjectType.ISortDataHolderReference:
					switch (persistedType)
					{
					case ObjectType.RuntimeTablixGroupLeafObjReference:
					case ObjectType.RuntimeChartCriGroupLeafObjReference:
						return true;
					}
					break;
				case ObjectType.IRowItemStruct:
					switch (persistedType)
					{
					case ObjectType.RowItemStruct:
					case ObjectType.TablixItemStruct:
					case ObjectType.TablixStruct:
					case ObjectType.TablixMemberStruct:
						return true;
					}
					break;
				case ObjectType.TablixItemStruct:
					switch (persistedType)
					{
					case ObjectType.TablixStruct:
					case ObjectType.TablixMemberStruct:
						return true;
					}
					break;
				}
			}
			return false;
		}
	}
}
