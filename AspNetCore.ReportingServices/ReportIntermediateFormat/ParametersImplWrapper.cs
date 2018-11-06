using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[SkipStaticValidation]
	internal class ParametersImplWrapper : IPersistable
	{
		private ParametersImpl m_opdParameters;

		[NonSerialized]
		private int m_hash;

		private static readonly Declaration m_Declaration = ParametersImplWrapper.GetDeclaration();

		internal ParametersImpl WrappedParametersImpl
		{
			get
			{
				return this.m_opdParameters;
			}
		}

		internal ParametersImplWrapper()
		{
			this.m_opdParameters = new ParametersImpl();
		}

		internal ParametersImplWrapper(ParametersImpl odpParameters)
		{
			this.m_opdParameters = odpParameters;
		}

		internal bool ValuesAreEqual(ParametersImplWrapper obj)
		{
			ParameterImpl[] collection = this.m_opdParameters.Collection;
			ParameterImpl[] collection2 = obj.WrappedParametersImpl.Collection;
			if (collection == null)
			{
				if (collection2 == null)
				{
					return true;
				}
				return false;
			}
			if (collection2 == null)
			{
				return false;
			}
			if (collection.Length != collection2.Length)
			{
				return false;
			}
			for (int i = 0; i < collection.Length; i++)
			{
				if (!collection[i].ValuesAreEqual(collection2[i]))
				{
					return false;
				}
			}
			return true;
		}

		internal int GetValuesHashCode()
		{
			ParameterImpl[] collection = this.m_opdParameters.Collection;
			if (this.m_hash == 0)
			{
				this.m_hash = 4051;
				if (collection != null)
				{
					this.m_hash |= collection.Length << 16;
					for (int i = 0; i < collection.Length; i++)
					{
						this.m_hash ^= collection[i].GetValuesHashCode();
					}
				}
			}
			return this.m_hash;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Parameters, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Parameter));
			list.Add(new MemberInfo(MemberName.Names, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringInt32Hashtable, Token.Int32));
			list.Add(new MemberInfo(MemberName.Count, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Parameters, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ParametersImplWrapper.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Parameters:
				{
					ParameterImplWrapper[] array = null;
					if (this.m_opdParameters.Collection != null)
					{
						array = new ParameterImplWrapper[this.m_opdParameters.Collection.Length];
						for (int i = 0; i < array.Length; i++)
						{
							if (this.m_opdParameters.Collection[i] != null)
							{
								array[i] = new ParameterImplWrapper(this.m_opdParameters.Collection[i]);
							}
						}
					}
					writer.Write(array);
					break;
				}
				case MemberName.Names:
					writer.WriteStringInt32Hashtable(this.m_opdParameters.NameMap);
					break;
				case MemberName.Count:
					writer.Write(this.m_opdParameters.Count);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ParametersImplWrapper.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Parameters:
				{
					ParameterImplWrapper[] array = reader.ReadArrayOfRIFObjects<ParameterImplWrapper>();
					if (array != null)
					{
						this.m_opdParameters.Collection = new ParameterImpl[array.Length];
						for (int i = 0; i < array.Length; i++)
						{
							if (array[i] != null)
							{
								this.m_opdParameters.Collection[i] = array[i].WrappedParameterImpl;
							}
						}
					}
					break;
				}
				case MemberName.Names:
					this.m_opdParameters.NameMap = reader.ReadStringInt32Hashtable<Hashtable>();
					break;
				case MemberName.Count:
					this.m_opdParameters.Count = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Parameters;
		}
	}
}
