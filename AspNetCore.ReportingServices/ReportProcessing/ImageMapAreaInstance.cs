using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportRendering;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ImageMapAreaInstance
	{
		private string m_id;

		private ImageMapArea.ImageMapAreaShape m_shape;

		private float[] m_coordinates;

		private Action m_actionDef;

		private ActionInstance m_actionInstance;

		private int m_uniqueName;

		public string ID
		{
			get
			{
				return this.m_id;
			}
			set
			{
				this.m_id = value;
			}
		}

		public ImageMapArea.ImageMapAreaShape Shape
		{
			get
			{
				return this.m_shape;
			}
			set
			{
				this.m_shape = value;
			}
		}

		public float[] Coordinates
		{
			get
			{
				return this.m_coordinates;
			}
			set
			{
				this.m_coordinates = value;
			}
		}

		public Action Action
		{
			get
			{
				return this.m_actionDef;
			}
			set
			{
				this.m_actionDef = value;
			}
		}

		public ActionInstance ActionInstance
		{
			get
			{
				return this.m_actionInstance;
			}
			set
			{
				this.m_actionInstance = value;
			}
		}

		public int UniqueName
		{
			get
			{
				return this.m_uniqueName;
			}
			set
			{
				this.m_uniqueName = value;
			}
		}

		internal ImageMapAreaInstance()
		{
		}

		internal ImageMapAreaInstance(ReportProcessing.ProcessingContext processingContext)
		{
			this.m_uniqueName = processingContext.CreateUniqueName();
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Id, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Shape, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Coordinates, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Single));
			memberInfoList.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Action));
			memberInfoList.Add(new MemberInfo(MemberName.ActionInstance, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ActionInstance));
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
