using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class UnifiedObjectCreator : IRIFObjectCreator
	{
		private IScalabilityObjectCreator[] m_objectCreators;

		private IReferenceCreator[] m_referenceCreators;

		private IScalabilityCache m_scalabilityCache;

		internal IScalabilityCache ScalabilityCache
		{
			get
			{
				return this.m_scalabilityCache;
			}
			set
			{
				this.m_scalabilityCache = value;
			}
		}

		internal UnifiedObjectCreator(IScalabilityObjectCreator appObjectCreator, IReferenceCreator appReferenceCreator)
		{
			this.m_objectCreators = new IScalabilityObjectCreator[2];
			this.m_objectCreators[0] = CommonObjectCreator.Instance;
			this.m_objectCreators[1] = appObjectCreator;
			this.m_referenceCreators = new IReferenceCreator[2];
			this.m_referenceCreators[0] = CommonReferenceCreator.Instance;
			this.m_referenceCreators[1] = appReferenceCreator;
		}

		public IPersistable CreateRIFObject(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType objectType, ref IntermediateFormatReader context)
		{
			IPersistable persistable = null;
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < this.m_objectCreators.Length; i++)
			{
				if (flag)
				{
					break;
				}
				flag = this.m_objectCreators[i].TryCreateObject(objectType, out persistable);
			}
			if (!flag)
			{
				flag2 = true;
				BaseReference baseReference = null;
				for (int j = 0; j < this.m_referenceCreators.Length; j++)
				{
					if (flag)
					{
						break;
					}
					flag = this.m_referenceCreators[j].TryCreateReference(objectType, out baseReference);
				}
				persistable = baseReference;
			}
			if (flag)
			{
				persistable.Deserialize(context);
				if (flag2)
				{
					BaseReference baseReference2 = (BaseReference)persistable;
					persistable = baseReference2.ScalabilityCache.PoolReference(baseReference2);
				}
			}
			else
			{
				Global.Tracer.Assert(false, "Cannot create object of type: {0}", objectType);
			}
			return persistable;
		}

		internal List<Declaration> GetDeclarations()
		{
			List<Declaration> list = new List<Declaration>();
			for (int i = 0; i < this.m_objectCreators.Length; i++)
			{
				list.AddRange(this.m_objectCreators[i].GetDeclarations());
			}
			return list;
		}
	}
}
