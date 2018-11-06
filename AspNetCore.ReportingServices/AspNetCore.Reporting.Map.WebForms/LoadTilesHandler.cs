using System.Drawing;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal delegate Image[,] LoadTilesHandler(Layer layer, string[,] tileUrls);
}
