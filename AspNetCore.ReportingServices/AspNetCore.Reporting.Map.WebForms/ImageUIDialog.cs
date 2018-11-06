using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class ImageUIDialog : Form
	{
		private ListView listView1;

		private ImageList imageList1;

		private Button btnDelete;

		private Button btnAdd;

		private Button btnOk;

		private Button btnCancel;

		private IContainer components;

		private MapCore map;

		private string selectedValue;

		internal string SelectedImage
		{
			get
			{
				if (this.listView1.SelectedItems.Count > 0)
				{
					return this.listView1.SelectedItems[0].Text;
				}
				return "";
			}
		}

		public ImageUIDialog()
		{
			this.InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new Container();
			this.listView1 = new ListView();
			this.imageList1 = new ImageList(this.components);
			this.btnDelete = new Button();
			this.btnAdd = new Button();
			this.btnOk = new Button();
			this.btnCancel = new Button();
			base.SuspendLayout();
			this.listView1.LargeImageList = this.imageList1;
			this.listView1.Location = new Point(8, 6);
			this.listView1.Name = "listView1";
			this.listView1.Size = new Size(294, 280);
			this.listView1.TabIndex = 0;
			this.listView1.DoubleClick += this.listView1_DoubleClick;
			this.listView1.SelectedIndexChanged += this.listView1_SelectedIndexChanged;
			this.imageList1.ColorDepth = ColorDepth.Depth8Bit;
			this.imageList1.ImageSize = new Size(100, 100);
			this.imageList1.TransparentColor = Color.Transparent;
			this.btnDelete.Enabled = false;
			this.btnDelete.Location = new Point(8, 296);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new Size(70, 24);
			this.btnDelete.TabIndex = 1;
			this.btnDelete.Text = "Delete";
			this.btnDelete.Click += this.btnDelete_Click;
			this.btnAdd.Location = new Point(82, 296);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new Size(70, 24);
			this.btnAdd.TabIndex = 2;
			this.btnAdd.Text = "Add";
			this.btnAdd.Click += this.btnAdd_Click;
			this.btnOk.DialogResult = DialogResult.OK;
			this.btnOk.Enabled = false;
			this.btnOk.Location = new Point(156, 296);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new Size(70, 24);
			this.btnOk.TabIndex = 3;
			this.btnOk.Text = "Ok";
			this.btnOk.Click += this.btnOk_Click;
			this.btnCancel.DialogResult = DialogResult.Cancel;
			this.btnCancel.Location = new Point(230, 296);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new Size(70, 24);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			base.AcceptButton = this.btnOk;
			this.AutoScaleBaseSize = new Size(5, 13);
			base.CancelButton = this.btnCancel;
			base.ClientSize = new Size(310, 327);
			base.Controls.AddRange(new Control[5]
			{
				this.btnCancel,
				this.btnOk,
				this.btnAdd,
				this.btnDelete,
				this.listView1
			});
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ImageUIDialog";
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Select image";
			base.ResumeLayout(false);
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = "All Image Files|*.jpg;*.jpeg;*.jpe;*.jif;*.bmp;*.png;*.gif;*.ico;*.emf;*.wmf|Bitmap Files (*.bmp)|*.bmp|GIF Files (*.gif)|*.gif|JPEG Files (*.jpg; *.jpeg; *.jpe; *.jif )|*.jpg;*.jpeg;*.jpe;*.jfif|Meta Files (*.emf; *.wmf)|*.emf,*.wmf|PNG Files (*.png)|*.png|All Files (*.*)|*.*";
				openFileDialog.RestoreDirectory = true;
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					NamedImage namedImage = new NamedImage();
					this.map.NamedImages.Add(namedImage);
					string fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
					if (this.map.NamedImages.GetIndex(fileName) == -1)
					{
						namedImage.Name = fileName;
					}
					namedImage.Image = Image.FromFile(openFileDialog.FileName);
					//this.imageList1.Images.Add(this.GetResizedImage(namedImage.Image));
					ListViewItem listViewItem = new ListViewItem(namedImage.Name, this.imageList1.Images.Count - 1);
					this.listView1.Select();
					this.listView1.Items.Add(listViewItem);
					this.listView1.SelectedItems.Clear();
					listViewItem.Focused = true;
					listViewItem.Selected = true;
				}
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			int index = this.map.NamedImages.GetIndex(this.SelectedImage);
			if (index != -1)
			{
				this.map.NamedImages.RemoveAt(index);
				this.InitImages();
			}
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.btnOk.Enabled = (this.listView1.SelectedItems.Count > 0);
			this.btnDelete.Enabled = this.btnOk.Enabled;
		}

		private void listView1_DoubleClick(object sender, EventArgs e)
		{
			this.btnOk.Enabled = (this.listView1.SelectedItems.Count > 0);
			if (this.btnOk.Enabled)
			{
				this.btnOk.PerformClick();
			}
		}

		internal DialogResult Execute(MapCore map, string selectedValue)
		{
			this.map = map;
			this.selectedValue = selectedValue;
			this.InitImages();
			return base.ShowDialog();
		}

		public bool ThumbnailCallback()
		{
			return false;
		}

		private Image GetResizedImage(Image image)
		{
			float num = (float)image.Size.Width / (float)image.Size.Height;
			Bitmap bitmap = new Bitmap(100, 100);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.FillRectangle(Brushes.White, 0, 0, 100, 100);
				Rectangle destRect = new Rectangle(0, 0, 100, 100);
				if (image.Size.Width < 100 && image.Size.Height < 100)
				{
					destRect = new Rectangle(0, 0, image.Size.Width, image.Size.Height);
					destRect.X = 50 - destRect.Width / 2;
					destRect.Y = 50 - destRect.Height / 2;
					graphics.DrawImage(image, destRect, 0, 0, image.Size.Width, image.Size.Height, GraphicsUnit.Pixel);
					return bitmap;
				}
				if (image.Size.Width > image.Size.Height)
				{
					destRect.Height = (int)(100.0 / num);
					destRect.Y = 50 - destRect.Height / 2;
					graphics.DrawImage(image, destRect, 0, 0, image.Size.Width, image.Size.Height, GraphicsUnit.Pixel);
					return bitmap;
				}
				destRect.Width = (int)(100.0 * num);
				destRect.X = 50 - destRect.Width / 2;
				graphics.DrawImage(image, destRect, 0, 0, image.Size.Width, image.Size.Height, GraphicsUnit.Pixel);
				return bitmap;
			}
		}

		private void InitImages()
		{
			this.imageList1.Images.Clear();
			this.listView1.Items.Clear();
			foreach (NamedImage namedImage in this.map.NamedImages)
			{
				if (namedImage.Image != null)
				{
					//this.imageList1.Images.Add(this.GetResizedImage(namedImage.Image));
					this.listView1.Items.Add(namedImage.Name, this.imageList1.Images.Count - 1);
				}
			}
			int index = this.map.NamedImages.GetIndex(this.selectedValue);
			if (index != -1)
			{
				this.listView1.Items[index].Selected = true;
			}
		}
	}
}
