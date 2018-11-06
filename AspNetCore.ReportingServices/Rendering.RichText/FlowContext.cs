namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal class FlowContext
	{
		internal float Width;

		internal float Height;

		internal float ContentOffset;

		internal bool WordTrim = true;

		internal bool LineLimit = true;

		internal float OmittedLineHeight;

		internal bool AtEndOfTextBox;

		internal TextBoxContext Context = new TextBoxContext();

		internal TextBoxContext ClipContext;

		internal bool Updatable;

		internal bool VerticalCanGrow;

		internal bool ForcedCharTrim;

		internal bool CharTrimLastLine = true;

		internal int CharTrimmedRunWidth;

		private FlowContext()
		{
		}

		internal FlowContext(float width, float height)
		{
			this.Width = width;
			this.Height = height;
		}

		internal FlowContext(float width, float height, int paragraphIndex, int runIndex, int runCharIndex)
		{
			this.Width = width;
			this.Height = height;
			this.Context.ParagraphIndex = paragraphIndex;
			this.Context.TextRunIndex = runIndex;
			this.Context.TextRunCharacterIndex = runCharIndex;
		}

		internal FlowContext(float width, float height, bool wordTrim, bool lineLimit)
			: this(width, height)
		{
			this.WordTrim = wordTrim;
			this.LineLimit = lineLimit;
		}

		internal FlowContext(float width, float height, TextBoxContext context)
			: this(width, height)
		{
			this.Context = context;
		}

		internal FlowContext Clone()
		{
			FlowContext flowContext = (FlowContext)base.MemberwiseClone();
			flowContext.Context = this.Context.Clone();
			if (this.ClipContext != null)
			{
				flowContext.ClipContext = this.ClipContext.Clone();
			}
			return flowContext;
		}

		internal void Reset()
		{
			this.Context.Reset();
			this.ClipContext = null;
			this.ContentOffset = 0f;
			this.AtEndOfTextBox = false;
			this.OmittedLineHeight = 0f;
			this.CharTrimmedRunWidth = 0;
			this.ForcedCharTrim = false;
		}
	}
}
