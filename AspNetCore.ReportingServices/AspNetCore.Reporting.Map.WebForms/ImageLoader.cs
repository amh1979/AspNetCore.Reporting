using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.Net;

namespace AspNetCore.Reporting.Map.WebForms
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
				throw new ArgumentNullException("ImageLoader - Valid Service Container object must be provided");
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
			throw new ArgumentException("ImageLoader - Image loader do not provide service of type: " + serviceType.ToString());
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
				MapCore mapCore = (MapCore)this.serviceContainer.GetService(typeof(MapCore));
				if (mapCore != null)
				{
					foreach (NamedImage namedImage in mapCore.NamedImages)
					{
						if (namedImage.Name == imageURL)
						{
							return namedImage.Image;
						}
					}
				}
			}
			if (this.imageData == null)
			{
				this.imageData = new Hashtable(new CaseInsensitiveHashCodeProvider(CultureInfo.InvariantCulture), StringComparer.OrdinalIgnoreCase);
			}
			if (this.imageData.Contains(imageURL))
			{
				image = (Image)this.imageData[imageURL];
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
				if (uri == (Uri)null)
				{
					MapControl mapControl = (MapControl)this.serviceContainer.GetService(typeof(MapControl));
					if (mapControl != null)
					{
						if (imageURL.StartsWith("~", StringComparison.Ordinal) && !string.IsNullOrEmpty(mapControl.applicationDocumentURL))
						{
							try
							{
								uri = new Uri(mapControl.applicationDocumentURL + imageURL.Substring(1));
							}
							catch (Exception)
							{
							}
						}
						else
						{
							string text = mapControl.webFormDocumentURL;
							int num = text.LastIndexOf('/');
							if (num != -1)
							{
								text = text.Substring(0, num + 1);
							}
							try
							{
								uri = new Uri(new Uri(text), imageURL);
							}
							catch (Exception)
							{
							}
						}
					}
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
				throw new ArgumentException(SR.ExceptionCannotLoadImageFromLocation(imageURL));
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
	}
}
