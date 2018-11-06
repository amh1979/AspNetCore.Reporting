using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal abstract class IDOwner : IInstancePath, IPersistable, IReferenceable, IGlobalIDOwner
	{
		protected int m_ID;

		[NonSerialized]
		protected bool m_isClone;

		[NonSerialized]
		private static readonly Declaration m_Declaration = IDOwner.GetDeclaration();

		[NonSerialized]
		protected string m_cachedDefinitionPath;

		[NonSerialized]
		private InstancePathItem m_instancePathItem;

		[NonSerialized]
		protected IDOwner m_parentIDOwner;

		[NonSerialized]
		protected List<InstancePathItem> m_cachedInstancePath;

		[NonSerialized]
		protected int m_globalID;

		[NonSerialized]
		protected string m_renderingModelID;

		public int ID
		{
			get
			{
				return this.m_ID;
			}
			set
			{
				this.m_ID = value;
			}
		}

		public int GlobalID
		{
			get
			{
				return this.m_globalID;
			}
			set
			{
				this.m_globalID = value;
			}
		}

		internal string RenderingModelID
		{
			get
			{
				if (this.m_renderingModelID == null)
				{
					this.m_renderingModelID = this.m_globalID.ToString(CultureInfo.InvariantCulture);
				}
				return this.m_renderingModelID;
			}
		}

		public InstancePathItem InstancePathItem
		{
			get
			{
				if (this.m_instancePathItem == null)
				{
					this.m_instancePathItem = this.CreateInstancePathItem();
				}
				return this.m_instancePathItem;
			}
		}

		internal string SubReportDefinitionPath
		{
			get
			{
				if (this.m_cachedDefinitionPath == null)
				{
					if (this.m_parentIDOwner != null)
					{
						this.m_cachedDefinitionPath = this.m_parentIDOwner.SubReportDefinitionPath;
					}
					else
					{
						this.m_cachedDefinitionPath = "";
					}
					if (this.InstancePathItem.Type == InstancePathItemType.SubReport)
					{
						this.m_cachedDefinitionPath = this.m_cachedDefinitionPath + 'x' + this.m_ID.ToString(CultureInfo.InvariantCulture);
					}
				}
				return this.m_cachedDefinitionPath;
			}
		}

		public virtual List<InstancePathItem> InstancePath
		{
			get
			{
				if (this.m_cachedInstancePath == null)
				{
					this.m_cachedInstancePath = new List<InstancePathItem>();
					if (this.ParentInstancePath != null)
					{
						List<InstancePathItem> instancePath = this.ParentInstancePath.InstancePath;
						this.m_cachedInstancePath.AddRange(instancePath);
					}
					if (!this.InstancePathItem.IsEmpty)
					{
						this.m_cachedInstancePath.Add(this.InstancePathItem);
					}
				}
				return this.m_cachedInstancePath;
			}
		}

		public IInstancePath ParentInstancePath
		{
			get
			{
				return this.m_parentIDOwner;
			}
			set
			{
				Global.Tracer.Assert(value == null || value is IDOwner, "((value != null) ? (value is IDOwner) : true)");
				this.m_parentIDOwner = (IDOwner)value;
			}
		}

		public virtual string UniqueName
		{
			get
			{
				return InstancePathItem.GenerateUniqueNameString(this.ID, this.InstancePath);
			}
		}

		internal bool IsClone
		{
			get
			{
				return this.m_isClone;
			}
		}

		protected IDOwner()
		{
		}

		protected IDOwner(int id)
		{
			this.m_ID = id;
		}

		protected virtual InstancePathItem CreateInstancePathItem()
		{
			return new InstancePathItem();
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context)
		{
			IDOwner iDOwner = (IDOwner)base.MemberwiseClone();
			iDOwner.m_ID = context.GenerateID();
			iDOwner.m_isClone = true;
			return iDOwner;
		}

		internal virtual void SetupCriRenderItemDef(ReportItem reportItem)
		{
			reportItem.m_parentIDOwner = this.m_parentIDOwner;
		}

		protected static IRIFReportDataScope FindReportDataScope(IInstancePath candidate)
		{
			IRIFReportDataScope iRIFReportDataScope = null;
			while (candidate != null && iRIFReportDataScope == null)
			{
				switch (candidate.InstancePathItem.Type)
				{
				case InstancePathItemType.DataRegion:
				case InstancePathItemType.Cell:
				case InstancePathItemType.ColumnMemberInstanceIndexTopMost:
				case InstancePathItemType.ColumnMemberInstanceIndex:
				case InstancePathItemType.RowMemberInstanceIndex:
					iRIFReportDataScope = (IRIFReportDataScope)candidate;
					break;
				}
				candidate = candidate.ParentInstancePath;
			}
			return iRIFReportDataScope;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(IDOwner.m_Declaration);
			while (writer.NextMember())
			{
				if (writer.CurrentMember.MemberName == MemberName.ID)
				{
					writer.Write(this.m_ID);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(IDOwner.m_Declaration);
			while (reader.NextMember())
			{
				if (reader.CurrentMember.MemberName == MemberName.ID)
				{
					this.m_ID = reader.ReadInt32();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public virtual AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner;
		}
	}
}
