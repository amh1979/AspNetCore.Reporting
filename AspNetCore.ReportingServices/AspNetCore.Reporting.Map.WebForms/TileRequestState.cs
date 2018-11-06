using System.Drawing;
using System.Net;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class TileRequestState
	{
		public HttpWebRequest Request;

		public Image[,] TileImages;

		public int X;

		public int Y;

		public Rectangle Rectangle = Rectangle.Empty;

		public string Url = string.Empty;

		public Layer Layer;

		public MapCore MapCore;

		public bool Timeout;

		public TileRequestState(HttpWebRequest request, string url, Image[,] tileImages, int x, int y, Rectangle rect, Layer layer, MapCore mapCore)
		{
			this.Request = request;
			this.Url = url;
			this.TileImages = tileImages;
			this.X = x;
			this.Y = y;
			this.Rectangle = rect;
			this.Layer = layer;
			this.MapCore = mapCore;
		}
	}
}
