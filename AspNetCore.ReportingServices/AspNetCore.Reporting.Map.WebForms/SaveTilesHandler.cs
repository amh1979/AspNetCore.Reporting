using System.Drawing;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal delegate void SaveTilesHandler(Layer layer, string[,] tileUrls, Image[,] tileImages);
}
