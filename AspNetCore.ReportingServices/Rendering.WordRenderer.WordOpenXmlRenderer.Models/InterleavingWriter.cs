using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class InterleavingWriter
	{
		public delegate void FinalWriteInterleaver(IInterleave interleaver, TextWriter output);

		public delegate T CreateInterleaver<T>(int index, long location) where T : IInterleave;

		private Stream _stream;

		private IList<IInterleave> _interleavingContent;

		private TextWriter _textWriter;

		private IInterleave _currentInterleaver;

		public TextWriter TextWriter
		{
			get
			{
				return this._textWriter;
			}
		}

		public long Location
		{
			get
			{
				this.TextWriter.Flush();
				return this._stream.Position;
			}
		}

		public InterleavingWriter(Stream stream, ScalabilityCache scalabilityCache)
			: this(stream)
		{
			this._interleavingContent = new ScalableList<IInterleave>(1, scalabilityCache, 113, 13);
		}

		public InterleavingWriter(Stream stream)
		{
			this._stream = stream;
			this._textWriter = new StreamWriter(this._stream);
			this._interleavingContent = new List<IInterleave>();
		}

		public T WriteInterleaver<T>(CreateInterleaver<T> createInterleaver) where T : IInterleave
		{
			this.TextWriter.Flush();
			this.StoreInterleaver();
			T val = createInterleaver(((ICollection<IInterleave>)this._interleavingContent).Count, this._stream.Position);
			this._currentInterleaver = (IInterleave)(object)val;
			return val;
		}

		private void StoreInterleaver()
		{
			if (this._currentInterleaver != null)
			{
				this._interleavingContent.Add(this._currentInterleaver);
				this._currentInterleaver = null;
			}
		}

		public void CommitInterleaver(IInterleave interleaver)
		{
			if (this._currentInterleaver == interleaver)
			{
				this.StoreInterleaver();
			}
			else
			{
				this._interleavingContent[interleaver.Index] = interleaver;
			}
		}

		public void Interleave(Stream output, FinalWriteInterleaver writeInterleaver)
		{
			this._textWriter.Flush();
			this.StoreInterleaver();
			this._stream.Seek(0L, SeekOrigin.Begin);
			long num = 0L;
			foreach (IInterleave item in this._interleavingContent)
			{
				WordOpenXmlUtils.CopyStream(this._stream, output, item.Location - num);
				num = item.Location;
				StringBuilder stringBuilder = new StringBuilder();
				using (StringWriter output2 = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
				{
					writeInterleaver(item, output2);
				}
				byte[] bytes = this._textWriter.Encoding.GetBytes(stringBuilder.ToString());
				output.Write(bytes, 0, bytes.Length);
			}
			WordOpenXmlUtils.CopyStream(this._stream, output, this._stream.Length - num);
		}

		public static InterleavingWriter CreateInterleavingWriterForTesting(Stream stream, Dictionary<string, string> namespaces)
		{
			return new InterleavingWriter(stream);
		}
	}
}
