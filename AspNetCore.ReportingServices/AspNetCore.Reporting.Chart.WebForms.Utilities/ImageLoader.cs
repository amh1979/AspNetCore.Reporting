using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Resources;

namespace AspNetCore.Reporting.Chart.WebForms.Utilities
{
	internal class ImageLoader : IDisposable, IServiceProvider
	{
		private Hashtable imageData;

		private IServiceContainer serviceContainer;

		private ImageLoader()
		{
		}

		public ImageLoader(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(SR.ExceptionImageLoaderInvalidServiceContainer);
			}
			this.serviceContainer = container;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(ImageLoader))
			{
				return this;
			}
			throw new ArgumentException(SR.ExceptionImageLoaderUnsupportedType(serviceType.ToString()));
		}

		public void Dispose()
		{
			if (this.imageData != null)
			{
				IDictionaryEnumerator enumerator = this.imageData.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						((Image)((DictionaryEntry)enumerator.Current).Value).Dispose();
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				this.imageData = null;
				GC.SuppressFinalize(this);
			}
		}

		public Image LoadImage(string imageURL)
		{
			return this.LoadImage(imageURL, true);
		}

		public Image LoadImage(string imageURL, bool saveImage)
		{
			Image image = null;
			if (this.serviceContainer != null)
			{
				Chart chart = (Chart)this.serviceContainer.GetService(typeof(Chart));
				if (chart != null)
				{
					foreach (NamedImage image2 in chart.Images)
					{
						if (image2.Name == imageURL)
						{
							return image2.Image;
						}
					}
				}
			}
			if (this.imageData == null)
			{
				this.imageData = new Hashtable(StringComparer.OrdinalIgnoreCase);
			}
			if (this.imageData.Contains(imageURL))
			{
				image = (Image)this.imageData[imageURL];
			}
			if (image == null)
			{
				try
				{
					int num = imageURL.IndexOf("::", StringComparison.Ordinal);
					if (num > 0)
					{
						string baseName = imageURL.Substring(0, num);
						string name = imageURL.Substring(num + 2);
						ResourceManager resourceManager = new ResourceManager(baseName, Assembly.GetExecutingAssembly());
						image = (Image)resourceManager.GetObject(name);
					}
				}
				catch (Exception)
				{
				}
			}
			if (image == null)
			{
				Uri uri = null;
				try
				{
					uri = new Uri(imageURL);
				}
				catch (Exception)
				{
				}
				if (uri != (Uri)null)
				{
					try
					{
						WebRequest webRequest = WebRequest.Create(uri);
						image = Image.FromStream(webRequest.GetResponse().GetResponseStream());
					}
					catch (Exception)
					{
					}
				}
			}
			if (image == null)
			{
				image = this.LoadFromFile(imageURL);
			}
			if (image == null)
			{
				throw new ArgumentException(SR.ExceptionImageLoaderIncorrectImageUrl(imageURL));
			}
			if (saveImage)
			{
				this.imageData[imageURL] = image;
			}
			return image;
		}

		private Image LoadFromFile(string fileName)
		{
			try
			{
				return Image.FromFile(fileName);
			}
			catch (Exception)
			{
				return null;
			}
		}

		internal bool GetAdjustedImageSize(string name, Graphics graphics, ref SizeF size)
		{
			Image image = this.LoadImage(name);
			if (image == null)
			{
				return false;
			}
			ImageLoader.GetAdjustedImageSize(image, graphics, ref size);
			return true;
		}

		internal static void GetAdjustedImageSize(Image image, Graphics graphics, ref SizeF size)
		{
			if (graphics != null)
			{
				size.Width = (float)image.Width * graphics.DpiX / image.HorizontalResolution;
				size.Height = (float)image.Height * graphics.DpiY / image.VerticalResolution;
			}
			else
			{
				size.Width = (float)image.Width;
				size.Height = (float)image.Height;
			}
		}

		internal static bool DoDpisMatch(Image image, Graphics graphics)
		{
			if (graphics.DpiX == image.HorizontalResolution)
			{
				return graphics.DpiY == image.VerticalResolution;
			}
			return false;
		}

		internal static Image GetScaledImage(Image image, Graphics graphics)
		{
			return new Bitmap(image, new Size((int)((float)image.Width * graphics.DpiX / image.HorizontalResolution), (int)((float)image.Height * graphics.DpiY / image.VerticalResolution)));
		}
	}
}
