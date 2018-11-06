using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ScopeLookupTable : IStorable, IPersistable
	{
		private int m_lookupInt;

		private Hashtable m_lookupTable;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ScopeLookupTable.GetDeclaration();

		internal Hashtable LookupTable
		{
			get
			{
				return this.m_lookupTable;
			}
			set
			{
				this.m_lookupTable = value;
			}
		}

		internal int LookupInt
		{
			get
			{
				return this.m_lookupInt;
			}
			set
			{
				this.m_lookupInt = value;
			}
		}

		public int Size
		{
			get
			{
				return 4 + ItemSizes.SizeOf(this.m_lookupTable);
			}
		}

		internal void Clear()
		{
			Hashtable lookupTable = this.m_lookupTable;
			if (lookupTable != null)
			{
				lookupTable.Clear();
			}
		}

		internal void Add(GroupingList scopeDefs, List<object>[] scopeValues, int value)
		{
			if (scopeValues == null || scopeValues.Length == 0)
			{
				Global.Tracer.Assert(scopeDefs == null || 0 == scopeDefs.Count, "(null == scopeDefs || 0 == scopeDefs.Count)");
				this.m_lookupInt = value;
			}
			else
			{
				bool flag = true;
				if (this.m_lookupTable == null)
				{
					this.m_lookupTable = new Hashtable();
					flag = false;
				}
				Hashtable hashtable = this.m_lookupTable;
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < scopeValues.Length; i++)
				{
					List<object> list = scopeValues[i];
					if (list == null)
					{
						num2++;
					}
					else
					{
						num = list.Count;
						if (i == scopeValues.Length - 1)
						{
							num--;
						}
						this.GetNullScopeEntries(num2, ref hashtable, ref flag);
						for (int j = 0; j < num; j++)
						{
							Hashtable hashtable2 = (!flag) ? null : ((Hashtable)hashtable[list[j]]);
							if (hashtable2 == null)
							{
								hashtable2 = new Hashtable();
								hashtable.Add(list[j], hashtable2);
								flag = false;
							}
							hashtable = hashtable2;
						}
						num2 = 0;
					}
				}
				object key = 1;
				if (scopeValues[scopeValues.Length - 1] != null)
				{
					key = scopeValues[scopeValues.Length - 1][num];
				}
				else
				{
					this.GetNullScopeEntries(num2, ref hashtable, ref flag);
				}
				Global.Tracer.Assert(!hashtable.Contains(key), "(!hashEntries.Contains(lastKey))");
				hashtable.Add(key, value);
			}
		}

		private void GetNullScopeEntries(int nullScopes, ref Hashtable hashEntries, ref bool lookup)
		{
			Hashtable hashtable = null;
			if (lookup)
			{
				hashtable = (Hashtable)hashEntries[nullScopes];
			}
			if (hashtable == null)
			{
				hashtable = new Hashtable();
				hashEntries.Add(nullScopes, hashtable);
				lookup = false;
			}
			hashEntries = hashtable;
		}

		internal int Lookup(GroupingList scopeDefs, List<object>[] scopeValues)
		{
			object obj = null;
			if (scopeValues == null || scopeValues.Length == 0)
			{
				Global.Tracer.Assert(scopeDefs == null || 0 == scopeDefs.Count, "(null == scopeDefs || 0 == scopeDefs.Count)");
				obj = this.m_lookupInt;
			}
			else
			{
				Hashtable hashtable = this.m_lookupTable;
				int num = 0;
				for (int i = 0; i < scopeValues.Length; i++)
				{
					List<object> list = scopeValues[i];
					if (list == null)
					{
						num++;
					}
					else
					{
						hashtable = (Hashtable)hashtable[num];
						for (int j = 0; j < list.Count; j++)
						{
							obj = hashtable[list[j]];
							if (i < scopeValues.Length - 1 || j < list.Count - 1)
							{
								hashtable = (Hashtable)obj;
								Global.Tracer.Assert(null != hashtable, "(null != hashEntries)");
							}
						}
						num = 0;
					}
				}
				if (scopeValues[scopeValues.Length - 1] == null)
				{
					hashtable = (Hashtable)hashtable[num];
					obj = hashtable[1];
				}
			}
			Global.Tracer.Assert(obj is int, "(value is int)");
			return (int)obj;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.LookupInt, Token.Int32));
			list.Add(new MemberInfo(MemberName.LookupTable, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.NLevelVariantHashtable, Token.Object));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeLookupTable, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ScopeLookupTable.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.LookupInt:
					writer.Write(this.m_lookupInt);
					break;
				case MemberName.LookupTable:
					writer.WriteNLevelVariantHashtable(this.m_lookupTable);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ScopeLookupTable.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.LookupInt:
					this.m_lookupInt = reader.ReadInt32();
					break;
				case MemberName.LookupTable:
					this.m_lookupTable = reader.ReadNLevelVariantHashtable();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeLookupTable;
		}
	}
}
