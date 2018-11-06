using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class CriImageInstance : ImageInstance
	{
		private byte[] m_imageData;

		private string m_mimeType;

		private ActionInfoWithDynamicImageMapCollection m_actionInfoImageMapAreas;

		[NonSerialized]
		private string m_streamName;

		[NonSerialized]
		private bool m_mimeTypeEvaluated;

		[NonSerialized]
		private static readonly Declaration m_Declaration = CriImageInstance.GetDeclaration();

		public override byte[] ImageData
		{
			get
			{
				return this.m_imageData;
			}
			set
			{
				if (base.m_reportElementDef.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance)
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMDefinitionWriteback);
				}
				this.m_imageData = value;
			}
		}

		public override string StreamName
		{
			get
			{
				return this.m_streamName;
			}
			internal set
			{
				this.m_streamName = value;
			}
		}

		public override string MIMEType
		{
			get
			{
				if (!this.m_mimeTypeEvaluated)
				{
					this.m_mimeTypeEvaluated = true;
					if (base.ImageDef.ImageDef.MIMEType != null && !base.ImageDef.ImageDef.MIMEType.IsExpression)
					{
						this.m_mimeType = base.ImageDef.MIMEType.Value;
					}
				}
				return this.m_mimeType;
			}
			set
			{
				if (base.m_reportElementDef.CriGenerationPhase != 0 && (base.m_reportElementDef.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || base.ImageDef.MIMEType.IsExpression))
				{
					this.m_mimeTypeEvaluated = true;
					this.m_mimeType = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		public override TypeCode TagDataType
		{
			get
			{
				return TypeCode.Empty;
			}
		}

		public override object Tag
		{
			get
			{
				return null;
			}
		}

		internal override string ImageDataId
		{
			get
			{
				return this.StreamName;
			}
		}

		public override ActionInfoWithDynamicImageMapCollection ActionInfoWithDynamicImageMapAreas
		{
			get
			{
				if (this.m_actionInfoImageMapAreas == null)
				{
					this.m_actionInfoImageMapAreas = new ActionInfoWithDynamicImageMapCollection();
				}
				return this.m_actionInfoImageMapAreas;
			}
		}

		internal override bool IsNullImage
		{
			get
			{
				return false;
			}
		}

		internal CriImageInstance(Image reportItemDef)
			: base(reportItemDef)
		{
			Global.Tracer.Assert(base.m_reportElementDef.CriOwner != null, "Expected CRI Owner");
		}

		internal override List<string> GetFieldsUsedInValueExpression()
		{
			return null;
		}

		public override ActionInfoWithDynamicImageMap CreateActionInfoWithDynamicImageMap()
		{
			if (base.ReportElementDef.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance)
			{
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMDefinitionWriteback);
			}
			if (this.m_actionInfoImageMapAreas == null)
			{
				this.m_actionInfoImageMapAreas = new ActionInfoWithDynamicImageMapCollection();
			}
			return this.m_actionInfoImageMapAreas.Add(base.RenderingContext, base.ImageDef, base.ImageDef);
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_streamName = null;
			this.m_imageData = null;
			this.m_mimeTypeEvaluated = false;
			this.m_mimeType = null;
			this.m_actionInfoImageMapAreas = null;
		}

		internal override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(CriImageInstance.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ImageData:
					writer.Write(this.m_imageData);
					break;
				case MemberName.MIMEType:
				{
					string value = null;
					if (base.ImageDef.MIMEType != null && base.ImageDef.MIMEType.IsExpression)
					{
						value = this.m_mimeType;
					}
					writer.Write(value);
					break;
				}
				case MemberName.Actions:
				{
					ActionInstance[] array = null;
					if (base.ImageDef.ActionInfo != null)
					{
						array = new ActionInstance[base.ImageDef.ActionInfo.Actions.Count];
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = ((ReportElementCollectionBase<Action>)base.ImageDef.ActionInfo.Actions)[i].Instance;
						}
					}
					writer.Write((IPersistable[])array);
					break;
				}
				case MemberName.ImageMapAreas:
					writer.WriteRIFList(this.ActionInfoWithDynamicImageMapAreas.InternalList);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		internal override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(CriImageInstance.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ImageData:
					this.m_imageData = reader.ReadByteArray();
					break;
				case MemberName.MIMEType:
				{
					string text = reader.ReadString();
					if (base.ImageDef.MIMEType != null && base.ImageDef.MIMEType.IsExpression)
					{
						this.m_mimeTypeEvaluated = true;
						this.m_mimeType = text;
					}
					else
					{
						Global.Tracer.Assert(text == null, "(mimeType == null)");
					}
					break;
				}
				case MemberName.Actions:
					((ROMInstanceObjectCreator)reader.PersistenceHelper).StartActionInfoInstancesDeserialization(base.ImageDef.ActionInfo);
					reader.ReadArrayOfRIFObjects<ActionInstance>();
					((ROMInstanceObjectCreator)reader.PersistenceHelper).CompleteActionInfoInstancesDeserialization();
					break;
				case MemberName.ImageMapAreas:
					this.m_actionInfoImageMapAreas = new ActionInfoWithDynamicImageMapCollection();
					reader.ReadListOfRIFObjects(this.m_actionInfoImageMapAreas.InternalList);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		internal override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageInstance;
		}

		[SkipMemberStaticValidation(MemberName.Actions)]
		private static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ImageData, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.MIMEType, Token.String));
			list.Add(new MemberInfo(MemberName.Actions, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionInstance));
			list.Add(new MemberInfo(MemberName.ImageMapAreas, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionInfoWithDynamicImageMap));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemInstance, list);
		}
	}
}
