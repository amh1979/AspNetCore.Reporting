using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class Previous : DataAggregate
	{
		private class ListCloner
		{
			private ListCloner()
			{
			}

			internal static List<object> CloneList(List<object> key, int startIndex)
			{
				if (key == null)
				{
					return null;
				}
				int num = key.Count - startIndex;
				List<object> list = new List<object>(Math.Max(0, num));
				for (int i = 0; i < num; i++)
				{
					list.Add(key[i + startIndex]);
				}
				return list;
			}
		}

		internal class ListOfObjectsEqualityComparer : IEqualityComparer<List<object>>
		{
			internal static readonly ListOfObjectsEqualityComparer Instance = new ListOfObjectsEqualityComparer();

			private ListOfObjectsEqualityComparer()
			{
			}

			public bool Equals(List<object> x, List<object> y)
			{
				if (x == null && y == null)
				{
					return true;
				}
				if (x == null == (y == null) && x.Count == y.Count)
				{
					for (int num = x.Count - 1; num >= 0; num--)
					{
						if (!x[num].Equals(y[num]))
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}

			public int GetHashCode(List<object> obj)
			{
				int count = obj.Count;
				int num = count << 24;
				if (count > 0)
				{
					num ^= obj[count - 1].GetHashCode();
				}
				return num;
			}
		}

		[StaticReference]
		private OnDemandProcessingContext m_odpContext;

		private Dictionary<List<object>, object> m_previousValues;

		private Dictionary<List<object>, object> m_values;

		private int m_startIndex;

		private bool m_isScopedInEvaluationScope;

		private object m_previous;

		private bool m_previousEnabled;

		private bool m_hasNoExplicitScope;

		private object m_value;

		private static Declaration m_declaration = Previous.GetDeclaration();

		internal override DataAggregateInfo.AggregateTypes AggregateType
		{
			get
			{
				return DataAggregateInfo.AggregateTypes.Previous;
			}
		}

		public override int Size
		{
			get
			{
				return ItemSizes.ReferenceSize + ItemSizes.SizeOf(this.m_previousValues) + ItemSizes.SizeOf(this.m_values) + 4 + 1 + ItemSizes.SizeOf(this.m_previous) + 1 + 1 + ItemSizes.SizeOf(this.m_value);
			}
		}

		internal Previous()
		{
		}

		internal Previous(OnDemandProcessingContext odpContext, int startIndex, bool isScopedInEvaluationScope, bool hasNoExplicitScope)
		{
			this.m_odpContext = odpContext;
			this.m_isScopedInEvaluationScope = isScopedInEvaluationScope;
			this.m_hasNoExplicitScope = hasNoExplicitScope;
			this.m_startIndex = startIndex;
		}

		internal override void Init()
		{
			if (!this.m_isScopedInEvaluationScope && !this.m_previousEnabled)
			{
				this.m_previousValues = this.m_values;
				this.m_values = new Dictionary<List<object>, object>(ListOfObjectsEqualityComparer.Instance);
			}
			this.m_previousEnabled = true;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			if (this.m_isScopedInEvaluationScope)
			{
				if (this.m_previousEnabled || this.m_hasNoExplicitScope)
				{
					this.m_previous = this.m_value;
				}
				this.m_value = expressions[0];
			}
			else
			{
				List<object> key = ListCloner.CloneList(this.m_odpContext.GroupExpressionValues, this.m_startIndex);
				if (this.m_previousValues != null)
				{
					this.m_previousValues.TryGetValue(key, out this.m_previous);
				}
				this.m_values[key] = expressions[0];
			}
			this.m_previousEnabled = false;
		}

		internal override object Result()
		{
			return this.m_previous;
		}

		internal override DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			RunningValueInfo runningValueInfo = (RunningValueInfo)aggregateDef;
			return new Previous(odpContext, runningValueInfo.TotalGroupingExpressionCount, runningValueInfo.IsScopedInEvaluationScope, string.IsNullOrEmpty(runningValueInfo.Scope));
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(Previous.m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.OdpContext:
				{
					int value = scalabilityCache.StoreStaticReference(this.m_odpContext);
					writer.Write(value);
					break;
				}
				case MemberName.PreviousValues:
					writer.WriteVariantListVariantDictionary(this.m_previousValues);
					break;
				case MemberName.Values:
					writer.WriteVariantListVariantDictionary(this.m_values);
					break;
				case MemberName.StartHidden:
					writer.Write(this.m_startIndex);
					break;
				case MemberName.IsScopedInEvaluationScope:
					writer.Write(this.m_isScopedInEvaluationScope);
					break;
				case MemberName.Previous:
					writer.Write(this.m_previous);
					break;
				case MemberName.PreviousEnabled:
					writer.Write(this.m_previousEnabled);
					break;
				case MemberName.HasNoExplicitScope:
					writer.Write(this.m_hasNoExplicitScope);
					break;
				case MemberName.Value:
					writer.Write(this.m_value);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(Previous.m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.OdpContext:
				{
					int id = reader.ReadInt32();
					this.m_odpContext = (OnDemandProcessingContext)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.PreviousValues:
					this.m_previousValues = reader.ReadVariantListVariantDictionary();
					break;
				case MemberName.Values:
					this.m_values = reader.ReadVariantListVariantDictionary();
					break;
				case MemberName.StartHidden:
					this.m_startIndex = reader.ReadInt32();
					break;
				case MemberName.IsScopedInEvaluationScope:
					this.m_isScopedInEvaluationScope = reader.ReadBoolean();
					break;
				case MemberName.Previous:
					this.m_previous = reader.ReadVariant();
					break;
				case MemberName.PreviousEnabled:
					this.m_previousEnabled = reader.ReadBoolean();
					break;
				case MemberName.HasNoExplicitScope:
					this.m_hasNoExplicitScope = reader.ReadBoolean();
					break;
				case MemberName.Value:
					this.m_value = reader.ReadVariant();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Previous;
		}

		public static Declaration GetDeclaration()
		{
			if (Previous.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.OdpContext, Token.Int32));
				list.Add(new MemberInfo(MemberName.PreviousValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VariantListVariantDictionary));
				list.Add(new MemberInfo(MemberName.Values, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VariantListVariantDictionary));
				list.Add(new MemberInfo(MemberName.StartIndex, Token.Int32));
				list.Add(new MemberInfo(MemberName.IsScopedInEvaluationScope, Token.Boolean));
				list.Add(new MemberInfo(MemberName.Previous, Token.Object));
				list.Add(new MemberInfo(MemberName.PreviousEnabled, Token.Boolean));
				list.Add(new MemberInfo(MemberName.HasNoExplicitScope, Token.Boolean));
				list.Add(new MemberInfo(MemberName.Value, Token.Object));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Previous, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return Previous.m_declaration;
		}
	}
}
