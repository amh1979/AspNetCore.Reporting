using AspNetCore.Reporting.Map.WebForms.BingMaps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(LayerConverter))]
	internal class Layer : NamedElement, ISelectable
	{
		private LayerVisibility visibility;

		private float visibleFromZoom = 50f;

		private float visibleToZoom = 200f;

		private float labelVisibleFromZoom = 50f;

		private float transparency;

		private TileSystem tileSystem;

		private bool useSecureConnectionForTiles;

		private ImageryProvider[] tileImageryProviders;

		private string tileImageUriFormat = string.Empty;

		private string[] tileImageUriSubdomains;

		private string tileError = string.Empty;

		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				if (value != null && (value == "(none)" || value == "(all)"))
				{
					throw new ArgumentException("bad_layer_name");
				}
				if (base.Name != value)
				{
					string name = base.Name;
					base.Name = value;
					this.UpdateLayerElements(name, value);
				}
			}
		}

		[SRCategory("CategoryAttribute_GeneralVisibility")]
		[DefaultValue(LayerVisibility.Shown)]
		[RefreshProperties(RefreshProperties.All)]
		[SRDescription("DescriptionAttributeLayer_Visibility")]
		public LayerVisibility Visibility
		{
			get
			{
				return this.visibility;
			}
			set
			{
				if (this.visibility != value)
				{
					this.visibility = value;
					this.InvalidateViewport();
				}
			}
		}

		[SRDescription("DescriptionAttributeLayer_VisibleFromZoom")]
		[DefaultValue(50)]
		[SRCategory("CategoryAttribute_GeneralVisibility")]
		public float VisibleFromZoom
		{
			get
			{
				return this.visibleFromZoom;
			}
			set
			{
				if (this.visibleFromZoom != value)
				{
					this.visibleFromZoom = value;
					if (this.Visibility == LayerVisibility.ZoomBased)
					{
						this.InvalidateViewport();
					}
				}
			}
		}

		[SRCategory("CategoryAttribute_GeneralVisibility")]
		[DefaultValue(200)]
		[SRDescription("DescriptionAttributeLayer_VisibleToZoom")]
		public float VisibleToZoom
		{
			get
			{
				return this.visibleToZoom;
			}
			set
			{
				if (this.visibleToZoom != value)
				{
					this.visibleToZoom = value;
					if (this.Visibility == LayerVisibility.ZoomBased)
					{
						this.InvalidateViewport();
					}
				}
			}
		}

		[DefaultValue(50)]
		[SRCategory("CategoryAttribute_LabelVisibility")]
		[SRDescription("DescriptionAttributeLayer_LabelVisibleFromZoom")]
		public float LabelVisibleFromZoom
		{
			get
			{
				return this.labelVisibleFromZoom;
			}
			set
			{
				if (this.labelVisibleFromZoom != value)
				{
					this.labelVisibleFromZoom = value;
					if (this.Visibility == LayerVisibility.ZoomBased)
					{
						this.InvalidateViewport();
					}
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Visible
		{
			get
			{
				if (this.Transparency == 100.0)
				{
					return false;
				}
				if (this.Visibility == LayerVisibility.ZoomBased)
				{
					if (this.Common != null && this.Common.MapCore != null)
					{
						float zoom = this.Common.MapCore.Viewport.Zoom;
						if (zoom >= this.VisibleFromZoom)
						{
							return zoom <= this.VisibleToZoom;
						}
						return false;
					}
					return false;
				}
				return this.Visibility == LayerVisibility.Shown;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public bool LabelVisible
		{
			get
			{
				if (this.Visibility == LayerVisibility.ZoomBased)
				{
					if (this.Common != null && this.Common.MapCore != null)
					{
						MapCore mapCore = this.Common.MapCore;
						float zoom = mapCore.Viewport.Zoom;
						if (zoom >= this.LabelVisibleFromZoom)
						{
							return zoom <= this.VisibleToZoom;
						}
						return false;
					}
					return false;
				}
				return this.Visibility == LayerVisibility.Shown;
			}
		}

		[SRCategory("CategoryAttribute_GeneralVisibility")]
		[SRDescription("DescriptionAttributeLayer_Transparency")]
		[DefaultValue(0f)]
		public float Transparency
		{
			get
			{
				return this.transparency;
			}
			set
			{
				if (!(value > 100.0) && !(value < 0.0))
				{
					this.transparency = value;
					this.InvalidateViewport();
					return;
				}
				throw new ArgumentOutOfRangeException(SR.out_of_range(0.0, 100.0));
			}
		}

		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeLayer_TileSystem")]
		[DefaultValue(TileSystem.None)]
		public TileSystem TileSystem
		{
			get
			{
				return this.tileSystem;
			}
			set
			{
				if (this.tileSystem != value)
				{
					this.tileSystem = value;
					this.ResetStoredVirtualEarthParameters();
					this.InvalidateViewport();
				}
			}
		}

		[DefaultValue(false)]
		[SRCategory("DescriptionAttributeLayer_TileSystem")]
		[SRDescription("DescriptionAttributeLayer_UseSecureConnectionForTiles")]
		[RefreshProperties(RefreshProperties.All)]
		public bool UseSecureConnectionForTiles
		{
			get
			{
				return this.useSecureConnectionForTiles;
			}
			set
			{
				if (this.useSecureConnectionForTiles != value)
				{
					this.useSecureConnectionForTiles = value;
					this.ResetStoredVirtualEarthParameters();
					this.InvalidateViewport();
				}
			}
		}

		internal ImageryProvider[] TileImageryProviders
		{
			get
			{
				return this.tileImageryProviders;
			}
			set
			{
				this.tileImageryProviders = value;
			}
		}

		internal string TileImageUriFormat
		{
			get
			{
				return this.tileImageUriFormat;
			}
			set
			{
				this.tileImageUriFormat = value;
			}
		}

		internal string[] TileImageUriSubdomains
		{
			get
			{
				return this.tileImageUriSubdomains;
			}
			set
			{
				this.tileImageUriSubdomains = value;
			}
		}

		internal string TileError
		{
			get
			{
				return this.tileError;
			}
			set
			{
				this.tileError = value;
			}
		}

		internal override void OnRemove()
		{
			base.OnRemove();
			this.UpdateLayerElements(this.Name, string.Empty);
		}

		internal string GetAttributionStrings()
		{
			if (this.TileImageryProviders == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			RectangleF rectangleF = new RectangleF(this.Common.MapCore.Viewport.GetAbsoluteLocation(), this.Common.MapCore.Viewport.GetAbsoluteSize());
			MapPoint minimumPoint = this.Common.MapCore.PixelsToGeographic(new PointF(rectangleF.Left, rectangleF.Bottom));
			MapPoint maximumPoint = this.Common.MapCore.PixelsToGeographic(new PointF(rectangleF.Right, rectangleF.Top));
			MapBounds a = new MapBounds(minimumPoint, maximumPoint);
			int num = Math.Max((int)VirtualEarthTileSystem.LevelOfDetail(this.Common.MapCore.Viewport.GetGroundResolutionAtEquator()), 1);
			ImageryProvider[] array = this.TileImageryProviders;
			foreach (ImageryProvider imageryProvider in array)
			{
				CoverageArea[] coverageAreas = imageryProvider.CoverageAreas;
				foreach (CoverageArea coverageArea in coverageAreas)
				{
					if (num >= coverageArea.ZoomMin && num <= coverageArea.ZoomMax)
					{
						MapBounds b = new MapBounds(new MapPoint(coverageArea.BoundingBox[1], coverageArea.BoundingBox[1]), new MapPoint(coverageArea.BoundingBox[3], coverageArea.BoundingBox[3]));
						if (MapBounds.Intersect(a, b))
						{
							if (stringBuilder.Length > 0)
							{
								stringBuilder.Append("|");
							}
							stringBuilder.Append(imageryProvider.Attribution);
							break;
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		internal List<ILayerElement> GetLayerElements()
		{
			List<ILayerElement> list = new List<ILayerElement>();
			if (this.Common != null && this.Common.MapCore != null)
			{
				MapCore mapCore = this.Common.MapCore;
				foreach (ILayerElement group in mapCore.Groups)
				{
					if (group.Layer == this.Name)
					{
						list.Add(group);
					}
				}
				foreach (ILayerElement shape in mapCore.Shapes)
				{
					if (shape.Layer == this.Name)
					{
						list.Add(shape);
					}
				}
				foreach (ILayerElement path in mapCore.Paths)
				{
					if (path.Layer == this.Name)
					{
						list.Add(path);
					}
				}
				{
					foreach (ILayerElement symbol in mapCore.Symbols)
					{
						if (symbol.Layer == this.Name)
						{
							list.Add(symbol);
						}
					}
					return list;
				}
			}
			return list;
		}

		private void UpdateLayerElements(string oldLayerName, string newLayerName)
		{
			List<ILayerElement> layerElements = this.GetLayerElements();
			foreach (ILayerElement item in layerElements)
			{
				if (item.Layer == oldLayerName)
				{
					item.Layer = newLayerName;
				}
			}
		}

		void ISelectable.DrawSelection(MapGraphics g, RectangleF clipRect, bool designTimeSelection)
		{
			RectangleF selectionRectangle = ((ISelectable)this).GetSelectionRectangle(g, clipRect);
			if (!selectionRectangle.IsEmpty)
			{
				g.DrawSelection(selectionRectangle, designTimeSelection, this.Common.MapCore.SelectionBorderColor, this.Common.MapCore.SelectionMarkerColor);
			}
		}

		bool ISelectable.IsSelected()
		{
			return false;
		}

		bool ISelectable.IsVisible()
		{
			return this.Visible;
		}

		RectangleF ISelectable.GetSelectionRectangle(MapGraphics g, RectangleF clipRect)
		{
			RectangleF rectangleF = RectangleF.Empty;
			List<ILayerElement> layerElements = this.GetLayerElements();
			foreach (ISelectable item in layerElements)
			{
				bool flag = false;
				IContentElement contentElement = item as IContentElement;
				if ((contentElement == null) ? item.IsVisible() : contentElement.IsVisible(g, this, false, clipRect))
				{
					rectangleF = ((!rectangleF.IsEmpty) ? RectangleF.Union(rectangleF, item.GetSelectionRectangle(g, clipRect)) : item.GetSelectionRectangle(g, clipRect));
				}
			}
			return rectangleF;
		}

		internal bool IsVirtualEarthServiceQueried()
		{
			lock (this.TileImageUriFormat)
			{
				return !string.IsNullOrEmpty(this.TileImageUriFormat);
			}
		}

		internal void ResetStoredVirtualEarthParameters()
		{
			lock (this.TileImageUriFormat)
			{
				this.TileImageUriFormat = string.Empty;
				this.TileImageUriSubdomains = null;
				this.TileImageryProviders = null;
			}
			lock (this.TileError)
			{
				this.TileError = string.Empty;
			}
		}

		internal bool QueryVirtualEarthService(bool asyncQuery)
		{
			if (!string.IsNullOrEmpty(this.Common.MapCore.TileServerAppId) && !(this.Common.MapCore.TileServerAppId.ToUpper(CultureInfo.InvariantCulture) == "(DEFAULT)"))
			{
				try
				{
					ImageryMetadataRequest imageryMetadataRequest = new ImageryMetadataRequest();
					imageryMetadataRequest.BingMapsKey = base.common.MapCore.TileServerAppId;
					imageryMetadataRequest.ImagerySet = VirtualEarthTileSystem.TileSystemToMapStyle(this.TileSystem);
					imageryMetadataRequest.IncludeImageryProviders = true;
					imageryMetadataRequest.UseHTTPS = true;
					ImageryMetadataRequest imageryRequest = imageryMetadataRequest;
					if (asyncQuery)
					{
						BingMapsService.GetImageryMetadataAsync(imageryRequest, this.ProcessImageryMetadataResponse, delegate(Exception ex)
						{
							lock (this.TileError)
							{
								this.TileError = ex.Message;
							}
						});
					}
					else
					{
						Response imageryMetadata = BingMapsService.GetImageryMetadata(imageryRequest);
						this.ProcessImageryMetadataResponse(imageryMetadata);
					}
				}
				catch (Exception ex2)
				{
					lock (this.TileError)
					{
						this.TileError = ex2.Message;
					}
					return false;
				}
				return true;
			}
			lock (this.TileError)
			{
				this.TileError = SR.ProvideBingMapsAppID;
			}
			return false;
		}

		private void ProcessImageryMetadataResponse(Response response)
		{
			try
			{
				this.ResetStoredVirtualEarthParameters();
				lock (this.TileImageUriFormat)
				{
					if (response != null && response.ResourceSets.Any())
					{
						ImageryMetadata imageryMetadata = response.ResourceSets.First().Resources.OfType<ImageryMetadata>().FirstOrDefault();
						if (imageryMetadata != null)
						{
							this.TileImageUriFormat = imageryMetadata.ImageUrl;
							this.TileImageUriSubdomains = imageryMetadata.ImageUrlSubdomains;
							this.TileImageryProviders = imageryMetadata.ImageryProviders;
						}
					}
				}
			}
			catch (Exception ex)
			{
				lock (this.TileError)
				{
					this.TileError = ex.InnerException.Message;
				}
			}
		}
	}
}
