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
			_scrollOffset = Vector2.Clamp(value, Vector2.Zero, ScrollMaxOffset);
			addedScroll += _scrollOffset - previous;
		}
	}
	public Vector2 ScrollMaxOffset { get; private set; } = Vector2.Zero;
	public LayoutAlignment Alignment = LayoutAlignment.Right;
	private bool needUpdate = true;
	private Vector2 _scrollOffset, _size, _minSize, _padding, addedScroll = Vector2.Zero;
	private Vector2? _maxSize;
	private Rectangle sourceRectangle;
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
		ScrollMaxOffset = Vector2.Zero;

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

			ScrollMaxOffset = new(Math.Max(Math.Abs(current.Position.X + current.Size.X - _padding.X), ScrollMaxOffset.X), Math.Max(Math.Abs(current.Position.Y + current.Size.Y - _padding.Y), ScrollMaxOffset.Y));
		}

		content.Y += addY;
		content += _padding;

		_size.X = Math.Max(content.X, _minSize.X);
		_size.Y = Math.Max(content.Y, _minSize.Y);

		if (_maxSize.HasValue)
		{
			_size.X = Math.Min(content.X, _maxSize.Value.X);
			_size.Y = Math.Min(content.Y, _maxSize.Value.Y);
		}

		sourceRectangle = new(_padding, _size - _padding * 2);
		addedScroll = Vector2.Zero;

		return _size != previousSize;
	}

	public override bool Step(Vector2 relativeCursor)
	{
		bool result = needUpdate;
		needUpdate = false;

		for (int i = 0; i < elements.Count; i++)
		{
			elements[i].Position -= addedScroll;
			result = elements[i].Step(relativeCursor - elements[i].Position) || result;
		}

		addedScroll = Vector2.Zero;

		if (result)
		{
			result = DoUpdate();

			if (result && !AllowOverflow)
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
