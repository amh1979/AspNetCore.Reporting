using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class ROMInstanceObjectCreator : PersistenceHelper, IRIFObjectCreator
	{
		private ReportItemInstance m_reportItemInstance;

		private ProcessingRIFObjectCreator __processingRIFObjectCreator;

		private ActionInfo m_currentActionInfo;

		private int m_currentActionIndex;

		private ParameterCollection m_currentParameterCollection;

		private int m_currentParameterIndex;

		private IRIFObjectCreator ProcessingRIFObjectCreator
		{
			get
			{
				if (this.__processingRIFObjectCreator == null)
				{
					this.__processingRIFObjectCreator = new ProcessingRIFObjectCreator(null, null);
				}
				return this.__processingRIFObjectCreator;
			}
		}

		internal ROMInstanceObjectCreator(ReportItemInstance reportItemInstance)
		{
			this.m_reportItemInstance = reportItemInstance;
		}

		internal void StartActionInfoInstancesDeserialization(ActionInfo actionInfo)
		{
			this.m_currentActionInfo = actionInfo;
			this.m_currentActionIndex = 0;
		}

		internal void CompleteActionInfoInstancesDeserialization()
		{
			this.m_currentActionInfo = null;
			this.m_currentActionIndex = 0;
		}

		internal void StartParameterInstancesDeserialization(ParameterCollection paramCollection)
		{
			this.m_currentParameterCollection = paramCollection;
			this.m_currentParameterIndex = 0;
		}

		internal void CompleteParameterInstancesDeserialization()
		{
			this.m_currentParameterCollection = null;
			this.m_currentParameterIndex = 0;
		}

		public IPersistable CreateRIFObject(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType objectType, ref IntermediateFormatReader context)
		{
			IPersistable persistable;
			switch (objectType)
			{
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Null:
				return null;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageInstance:
				persistable = (ImageInstance)this.m_reportItemInstance;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StyleInstance:
				persistable = this.m_reportItemInstance.Style;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionInstance:
				Global.Tracer.Assert(this.m_currentActionInfo != null && this.m_currentActionInfo.Actions.Count > this.m_currentActionIndex, "Ensure m_currentActionInfo is setup properly");
				persistable = ((ReportElementCollectionBase<Action>)this.m_currentActionInfo.Actions)[this.m_currentActionIndex].Instance;
				this.m_currentActionIndex++;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInstance:
				Global.Tracer.Assert(this.m_currentParameterCollection != null && this.m_currentParameterCollection.Count > this.m_currentParameterIndex, "Ensure m_currentParameterCollection is setup properly");
				persistable = ((ReportElementCollectionBase<Parameter>)this.m_currentParameterCollection)[this.m_currentParameterIndex].Instance;
				this.m_currentParameterIndex++;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionInfoWithDynamicImageMap:
				persistable = new ActionInfoWithDynamicImageMap(this.m_reportItemInstance.RenderingContext, null, (ReportItem)this.m_reportItemInstance.ReportElementDef, (IROMActionOwner)this.m_reportItemInstance.ReportElementDef);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageMapAreaInstance:
				persistable = new ImageMapAreaInstance();
				break;
			default:
				return this.ProcessingRIFObjectCreator.CreateRIFObject(objectType, ref context);
			}
			persistable.Deserialize(context);
			return persistable;
		}
	}
}
