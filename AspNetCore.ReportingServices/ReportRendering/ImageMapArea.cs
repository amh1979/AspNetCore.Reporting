using AspNetCore.ReportingServices.ReportProcessing;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ImageMapArea
	{
		internal enum ImageMapAreaShape
		{
			Rectangle,
			Polygon,
			Circle
		}

		private string m_id;

		private ImageMapAreaShape m_shape = ImageMapAreaShape.Polygon;

		private float[] m_coordinates;

		private ActionInfo m_actionInfo;

		private MemberBase m_members;

		public string ID
		{
			get
			{
				return this.m_id;
			}
			set
			{
				if (!this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.m_id = value;
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				ActionInfo actionInfo = this.m_actionInfo;
				if (!this.IsCustomControl && this.Rendering.m_mapAreaInstance != null)
				{
					actionInfo = new ActionInfo(this.Rendering.m_mapAreaInstance.Action, this.Rendering.m_mapAreaInstance.ActionInstance, this.Rendering.m_mapAreaInstance.UniqueName.ToString(CultureInfo.InvariantCulture), this.Rendering.m_renderingContext);
					if (this.Rendering.m_renderingContext.CacheState)
					{
						this.m_actionInfo = actionInfo;
					}
				}
				return actionInfo;
			}
			set
			{
				if (!this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.m_actionInfo = value;
			}
		}

		public ImageMapAreaShape Shape
		{
			get
			{
				return this.m_shape;
			}
		}

		public float[] Coordinates
		{
			get
			{
				return this.m_coordinates;
			}
		}

		private bool IsCustomControl
		{
			get
			{
				return this.m_members.IsCustomControl;
			}
		}

		private ImageMapAreaRendering Rendering
		{
			get
			{
				Global.Tracer.Assert(!this.m_members.IsCustomControl);
				ImageMapAreaRendering imageMapAreaRendering = this.m_members as ImageMapAreaRendering;
				if (imageMapAreaRendering == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return imageMapAreaRendering;
			}
		}

		public ImageMapArea()
		{
			this.m_members = new ImageMapAreaProcessing();
		}

		internal ImageMapArea(ImageMapAreaInstance mapAreaInstance, RenderingContext renderingContext)
		{
			this.m_members = new ImageMapAreaRendering();
			this.Rendering.m_mapAreaInstance = mapAreaInstance;
			this.Rendering.m_renderingContext = renderingContext;
			if (mapAreaInstance != null)
			{
				this.m_id = mapAreaInstance.ID;
				this.m_shape = mapAreaInstance.Shape;
				this.m_coordinates = mapAreaInstance.Coordinates;
			}
		}

		public void SetCoordinates(ImageMapAreaShape shape, float[] coordinates)
		{
			if (!this.IsCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			if (coordinates == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "coordinates");
			}
			this.m_shape = shape;
			this.m_coordinates = coordinates;
		}

		internal ImageMapArea DeepClone()
		{
			Global.Tracer.Assert(this.IsCustomControl);
			ImageMapArea imageMapArea = new ImageMapArea();
			imageMapArea.m_members = null;
			imageMapArea.m_shape = this.m_shape;
			if (this.m_id != null)
			{
				imageMapArea.m_id = string.Copy(this.m_id);
			}
			if (this.m_coordinates != null)
			{
				imageMapArea.m_coordinates = new float[this.m_coordinates.Length];
				this.m_coordinates.CopyTo(imageMapArea.m_coordinates, 0);
			}
			if (this.m_actionInfo != null)
			{
				imageMapArea.m_actionInfo = this.m_actionInfo.DeepClone();
			}
			return imageMapArea;
		}

		internal ImageMapAreaInstance Deconstruct(AspNetCore.ReportingServices.ReportProcessing.CustomReportItem context)
		{
			Global.Tracer.Assert(null != context);
			ImageMapAreaInstance imageMapAreaInstance = new ImageMapAreaInstance(context.ProcessingContext);
			imageMapAreaInstance.ID = this.m_id;
			imageMapAreaInstance.Shape = this.m_shape;
			imageMapAreaInstance.Coordinates = this.m_coordinates;
			if (this.m_actionInfo != null)
			{
				AspNetCore.ReportingServices.ReportProcessing.Action action = null;
				ActionInstance actionInstance = default(ActionInstance);
				this.m_actionInfo.Deconstruct(imageMapAreaInstance.UniqueName, ref action, out actionInstance, context);
				imageMapAreaInstance.Action = action;
				imageMapAreaInstance.ActionInstance = actionInstance;
			}
			return imageMapAreaInstance;
		}
	}
}
