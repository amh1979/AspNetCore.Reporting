using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ActionInfoWithDynamicImageMap : ActionInfo, IPersistable
	{
		private ImageMapAreaInstanceCollection m_imageMapAreas;

		private static readonly Declaration m_Declaration = ActionInfoWithDynamicImageMap.GetDeclaration();

		public ImageMapAreaInstanceCollection ImageMapAreaInstances
		{
			get
			{
				if (this.m_imageMapAreas == null)
				{
					this.m_imageMapAreas = new ImageMapAreaInstanceCollection();
				}
				return this.m_imageMapAreas;
			}
		}

		internal ActionInfoWithDynamicImageMap(RenderingContext renderingContext, ReportItem owner, IROMActionOwner romActionOwner)
			: this(renderingContext, new AspNetCore.ReportingServices.ReportIntermediateFormat.Action(), owner, romActionOwner)
		{
		}

		internal ActionInfoWithDynamicImageMap(RenderingContext renderingContext, ReportItem owner, IReportScope reportScope, IInstancePath instancePath, IROMActionOwner romActionOwner, bool chartConstructor)
			: this(renderingContext, reportScope, new AspNetCore.ReportingServices.ReportIntermediateFormat.Action(), instancePath, owner, romActionOwner)
		{
			base.m_chartConstruction = chartConstructor;
		}

		internal ActionInfoWithDynamicImageMap(RenderingContext renderingContext, AspNetCore.ReportingServices.ReportIntermediateFormat.Action actionDef, ReportItem owner, IROMActionOwner romActionOwner)
			: this(renderingContext, owner.ReportScope, actionDef, owner.ReportItemDef, owner, romActionOwner)
		{
		}

		internal ActionInfoWithDynamicImageMap(RenderingContext renderingContext, IReportScope reportScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Action actionDef, IInstancePath instancePath, ReportItem owner, IROMActionOwner romActionOwner)
			: base(renderingContext, reportScope, actionDef, instancePath, owner, owner.ReportItemDef.ObjectType, owner.ReportItemDef.Name, romActionOwner)
		{
			base.IsDynamic = true;
		}

		internal ActionInfoWithDynamicImageMap(RenderingContext renderingContext, AspNetCore.ReportingServices.ReportRendering.ActionInfo renderAction, ImageMapAreasCollection renderImageMap)
			: base(renderingContext, renderAction)
		{
			base.IsDynamic = true;
			this.m_imageMapAreas = new ImageMapAreaInstanceCollection(renderImageMap);
		}

		public ImageMapAreaInstance CreateImageMapAreaInstance(ImageMapArea.ImageMapAreaShape shape, float[] coordinates)
		{
			return this.CreateImageMapAreaInstance(shape, coordinates, null);
		}

		public ImageMapAreaInstance CreateImageMapAreaInstance(ImageMapArea.ImageMapAreaShape shape, float[] coordinates, string toolTip)
		{
			if (!base.m_chartConstruction && base.ReportElementOwner.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance)
			{
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMDefinitionWriteback);
			}
			if (coordinates != null && coordinates.Length >= 1)
			{
				if (this.m_imageMapAreas == null)
				{
					this.m_imageMapAreas = new ImageMapAreaInstanceCollection();
				}
				return this.m_imageMapAreas.Add(shape, coordinates, toolTip);
			}
			throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "coordinates");
		}

		[Obsolete("ActionInfoWithDynamicImageMap objects are completely volatile, so there is no reason to reuse the same instance of this class. Hence there is no need to support Update and SetNewContext methods.")]
		internal new void Update(AspNetCore.ReportingServices.ReportRendering.ActionInfo newActionInfo)
		{
			Global.Tracer.Assert(false, "Update(...) should not be called on ActionInfoWithDynamicImageMap");
		}

		[Obsolete("ActionInfoWithDynamicImageMap objects are completely volatile, so there is no reason to reuse the same instance of this class. Hence there is no need to support Update and SetNewContext methods.")]
		internal override void SetNewContext()
		{
			Global.Tracer.Assert(false, "SetNewContext() should not be called on ActionInfoWithDynamicImageMap");
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ActionInfoWithDynamicImageMap.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ActionDefinition:
					writer.Write(base.ActionDef);
					break;
				case MemberName.Actions:
				{
					ActionInstance[] array = new ActionInstance[base.Actions.Count];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = ((ReportElementCollectionBase<Action>)base.Actions)[i].Instance;
					}
					writer.Write((IPersistable[])array);
					break;
				}
				case MemberName.ImageMapAreas:
					writer.WriteRIFList(this.ImageMapAreaInstances.InternalList);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ActionInfoWithDynamicImageMap.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ActionDefinition:
					base.ActionDef = (AspNetCore.ReportingServices.ReportIntermediateFormat.Action)reader.ReadRIFObject();
					break;
				case MemberName.Actions:
					((ROMInstanceObjectCreator)reader.PersistenceHelper).StartActionInfoInstancesDeserialization(this);
					reader.ReadArrayOfRIFObjects<ActionInstance>();
					((ROMInstanceObjectCreator)reader.PersistenceHelper).CompleteActionInfoInstancesDeserialization();
					break;
				case MemberName.ImageMapAreas:
					this.m_imageMapAreas = new ImageMapAreaInstanceCollection();
					reader.ReadListOfRIFObjects(this.m_imageMapAreas.InternalList);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionInfoWithDynamicImageMap;
		}

		[SkipMemberStaticValidation(MemberName.Actions)]
		[SkipMemberStaticValidation(MemberName.ActionDefinition)]
		private static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ActionDefinition, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.Actions, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionInstance));
			list.Add(new MemberInfo(MemberName.ImageMapAreas, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageMapAreaInstance));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionInfoWithDynamicImageMap, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}
	}
}
