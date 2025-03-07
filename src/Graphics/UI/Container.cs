namespace Claw.Graphics.UI;

/// <summary>
/// Contêiner base para carregar outros elementos.
/// </summary>
public abstract class Container : Element
{
	/// <summary>
	/// Define se o conteúdo deste elemento pode ultrapassar as bordas (true por padrão).
	/// </summary>
	/// <remarks>Ao marcar como false, o elemento usará o <see cref="RenderTarget"/> como método de corte.</remarks>
	public bool AllowOverflow = true;
	public sealed override Vector2 Size => _size;
	public Vector2 Gap;
	public Vector2 Padding
	{
		get => _padding;
		set => _padding = value;
	}
	public Vector2 MinSize
	{
		get => _minSize;
		set
		{
			_minSize = value;
			needUpdate = true;
		}
	}
	public Vector2? MaxSize
	{
		get => _maxSize;
		set
		{
			_maxSize = value;
			needUpdate = true;
		}
	}
	public Vector2 ScrollOffset
	{
		get => _scrollOffset;
		set
		{
			Vector2 previous = _scrollOffset;
			_scrollOffset = Vector2.Clamp(value, Vector2.Zero, MaxScrollOffset);
			addedScroll += _scrollOffset - previous;
		}
	}
	public Vector2 MaxScrollOffset { get; private set; } = Vector2.Zero;
	public LayoutAlignment Alignment
	{
		get => _alignment;
		set
		{
			_alignment = value;
			needUpdate = true;
		}
	}

	private bool needUpdate = true;
	private Vector2 _scrollOffset, _size, _minSize, _padding, addedScroll = Vector2.Zero;
	private Vector2? _maxSize;
	private Rectangle sourceRectangle;
	private LayoutAlignment _alignment = LayoutAlignment.Left;
	private RenderTarget surface;

	public int Count => elements.Count;
	public Element this[int index]
	{
		get => elements[index];
		set
		{
			elements[index] = value;
			needUpdate = true;
		}
	}
	private List<Element> elements = new List<Element>();

	/// <summary>
	/// Insere um elemento no Contêiner.
	/// </summary>
	public void Add(Element element)
	{
		needUpdate = true;

		elements.Add(element);
	}
	/// <summary>
	/// Remove um elemento do Contêiner.
	/// </summary>
	public void Remove(Element element)
	{
		needUpdate = true;

		elements.Remove(element);
	}

	/// <summary>
	/// Atualiza a posição e tamanho dos elementos internos.
	/// </summary>
	/// <returns>Se houve mudança no tamanho deste contêiner.</returns>
	private bool DoUpdate()
	{
		Vector2 previousSize = _size;
		_scrollOffset = Vector2.Zero;
		MaxScrollOffset = Vector2.Zero;

		if (elements.Count == 0)
		{
			_size = _minSize;

			return _size != previousSize;
		}

		Element current = null, previous;
		float addY = 0;
		Vector2 elementPos = _padding;
		Vector2 content = _padding;
		Vector2? maxContent = _maxSize.HasValue ? _maxSize - _padding * 2 : null;

		for (int i = 0; i < elements.Count; i++)
		{
			previous = current;
			current = elements[i];

			if (previous == null)
			{
				current.Position = elementPos;
				content.X += current.Size.X;
				addY = current.Size.Y;
			}
			else
			{
				switch (current.Layout)
				{
					case LayoutMode.Block:
						addY += Gap.Y;
						content.Y += addY;
						addY = current.Size.Y;
						elementPos.X = _padding.X;
						elementPos.Y = content.Y;
						current.Position = elementPos;
						break;
					case LayoutMode.Inline:
						elementPos.X += previous.Size.X + Gap.X;
						content.X = Math.Max(content.X, elementPos.X + current.Size.X);
						addY = Math.Max(addY, current.Size.Y);
						current.Position = elementPos;
						break;
					case LayoutMode.InlineBlock:
						elementPos.X += previous.Size.X + Gap.X;

						if (maxContent.HasValue && elementPos.X + current.Size.X > maxContent.Value.X)
						{
							addY += Gap.Y;
							content.Y += addY;
							addY = current.Size.Y;
							elementPos.X = _padding.X;
							elementPos.Y = content.Y;
						}
						else
						{
							content.X = Math.Max(content.X, elementPos.X + current.Size.X);
							addY = Math.Max(addY, current.Size.Y);
						}

						current.Position = elementPos;
						break;
				}
			}
			
			MaxScrollOffset = Vector2.Max(Vector2.Abs(current.Position + current.Size - _padding), MaxScrollOffset);
		}

		content.Y += addY;
		content += _padding;
		_size = Vector2.Max(content, _minSize);

		if (_maxSize.HasValue) _size = Vector2.Min(content, _maxSize.Value);

		int rowCount = 0;
		float rowWidth = 0, currentY = elements[0].Position.Y;

		if (Alignment != LayoutAlignment.Left)
		{
			for (int i = 0; i < elements.Count; i++)
			{
				if (currentY != elements[i].Position.Y)
				{
					for (int j = i - rowCount; j < i; j++) elements[j].Position.X = Align(elements[j].Position.X, rowWidth);

					rowCount = 0;
					rowWidth = 0;
					currentY = elements[i].Position.Y;
				}

				if (rowCount > 0) rowWidth += Gap.X;

				rowWidth += elements[i].Size.X;
				rowCount++;
			}

			elements[elements.Count - 1].Position.X = Align(elements[elements.Count - 1].Position.X, rowWidth);
		}

		sourceRectangle = new(_padding, _size - _padding * 2);
		MaxScrollOffset = Vector2.Max(MaxScrollOffset - _size + _padding * 2, Vector2.Zero);
		addedScroll = Vector2.Zero;

		return _size != previousSize;
	}
	private float Align(float x, float rowWidth)
	{
		if (Alignment == LayoutAlignment.Right) return MaxScrollOffset.X - rowWidth + x;
		
		return MaxScrollOffset.X * .5f - rowWidth * .5f + x;
	}

	public override bool Step(Vector2 relativeCursor)
	{
		bool result = needUpdate;
		Vector2 childRelative;
		needUpdate = false;

		for (int i = 0; i < elements.Count; i++)
		{
			elements[i].Position -= addedScroll;
			childRelative = relativeCursor - elements[i].Position;
			
			if (!AllowOverflow && elements[i].Contains(childRelative))
			{
				if (relativeCursor.X < _padding.X) childRelative.X = -Math.Sign(_padding.X - relativeCursor.X);
				else if (relativeCursor.X > _size.X - _padding.X) childRelative.X = elements[i].Size.X + relativeCursor.X - (_size.X - _padding.X);

				if (relativeCursor.Y < _padding.Y) childRelative.Y = -Math.Sign(_padding.Y - relativeCursor.Y);
				else if (relativeCursor.Y > _size.Y - _padding.Y) childRelative.Y = elements[i].Size.Y + relativeCursor.Y - (_size.Y - _padding.Y);
			}

			result = elements[i].Step(childRelative) || result;
		}

		addedScroll = Vector2.Zero;

		if (result)
		{
			result = DoUpdate();

			if (!AllowOverflow && result)
			{
				if (surface != null) surface.Destroy();

				surface = new RenderTarget((int)Size.X, (int)Size.Y);
			}
		}

		return result;
	}

	public override void Render()
	{
		if (!AllowOverflow && surface == null) return;

		RenderTarget previousTarget = Game.Instance.Renderer.GetRenderTarget();
		Color previousColor = Game.Instance.Renderer.ClearColor;

		if (!AllowOverflow)
		{
			Game.Instance.Renderer.ClearColor = Color.Transparent;

			Game.Instance.Renderer.SetRenderTarget(surface);
			Game.Instance.Renderer.Clear();
		}

		for (int i = 0; i < elements.Count; i++) elements[i].Render();

		if (!AllowOverflow)
		{
			Game.Instance.Renderer.ClearColor = previousColor;

			Game.Instance.Renderer.SetRenderTarget(previousTarget);
			Draw.Sprite(surface, Position + _padding, sourceRectangle, Color.White);
		}
	}
}
