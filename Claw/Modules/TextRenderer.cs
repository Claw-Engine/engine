using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Claw.Graphics;
using Claw.Extensions;

namespace Claw.Modules
{
    /// <summary>
    /// Realiza a renderização de textos com diferentes efeitos para cada bloco.
    /// </summary>
    public sealed class TextRenderer : BaseModule, IStep, IRender
	{
		public int StepOrder
		{
			get => _StepOrder;
			set
			{
				if (_StepOrder != value) StepOrderChanged?.Invoke(this);

				_StepOrder = value;
			}
		}
		public int RenderOrder
		{
			get => _RenderOrder;
			set
			{
				if (_RenderOrder != value) RenderOrderChanged?.Invoke(this);

				_RenderOrder = value;
			}
		}
		private int _StepOrder, _RenderOrder;

		public event Action<IStep> StepOrderChanged;
		public event Action<IRender> RenderOrderChanged;

		#region Configurações
		/// <summary>
		/// Dicionário para armazenar configurações prontas.
		/// </summary>
		public static Dictionary<string, TextConfig> TextConfigs = new Dictionary<string, TextConfig>();
        private static Regex CompilerRegex = new Regex("<[^>]*>|[^</]+");
        private static TextBlock LineBreak = new TextBlock("\n", new TextConfig());
        private static Dictionary<TextOrigin, Vector2> OriginAsVector = new Dictionary<TextOrigin, Vector2>()
        {
            { TextOrigin.TopLeft, Vector2.Zero },
            { TextOrigin.Top, new Vector2(.5f, 0) },
            { TextOrigin.TopRight, new Vector2(1, 0) },

            { TextOrigin.Left, new Vector2(0, .5f) },
            { TextOrigin.Center, new Vector2(.5f) },
            { TextOrigin.Right, new Vector2(1, .5f) },

            { TextOrigin.BottomLeft, new Vector2(0, 1) },
            { TextOrigin.Bottom, new Vector2(.5f, 1) },
            { TextOrigin.BottomRight, Vector2.One }
        };
        private static Random random = new Random();
        /// <summary>
        /// Configurações usadas no caso das do bloco serem nulas.
        /// </summary>
        public TextConfig DefaultConfig = new TextConfig(0, Color.Black, Vector2.One, TextOrigin.TopLeft, TextEffect.None, null, Flip.None);
		#endregion

		#region Texto
		/// <summary>
		/// Até que caractere ele vai desenhar (negativo para desenhar tudo).
		/// </summary>
		public int MaxChar = -1;
        /// <summary>
        /// Máximo de caracteres das linhas para o <see cref="TextWrap"/>.
        /// </summary>
        public int MaxLength
        {
            get => _maxLength;
            set
            {
                if (value != _maxLength)
                {
                    _maxLength = value;
                    needToUpdate = true;
                }
            }
        }
        public string Text
        {
            get => _text;
            set
            {
                if (value != _text)
                {
                    _text = value;
                    needToUpdate = true;
                }
            }
        }
        /// <summary>
        /// Armazena o seu texto, sem as tags.
        /// </summary>
        public string FilteredText { get; private set; } = string.Empty;
        public TextWrap TextWrap
        {
            get => _textWrap;
            set
            {
                if (value != _textWrap)
                {
                    _textWrap = value;
                    needToUpdate = true;
                }
            }
        }
        private bool needToUpdate = false;
        private int _maxLength = 0;
        private string _text = string.Empty;
        private TextWrap _textWrap = TextWrap.NoWrap;
        private List<TextBlock> textBlocks = new List<TextBlock>();
        #endregion

        #region Animação
        public bool UseScaledTime = true;
        public float RotationAmount = 3, PulsateAmount = .01f, WaveSpeed = .05f, WaveAmplitude = .5f;
        public Vector2 PulsateLimit = new Vector2(.5f, 1), ScreamOffset = new Vector2(2), MovingSpeed = new Vector2(.05f), MovingAmplitude = new Vector2(4);
        private float theta = 0;
        private float rotationEffect = 0, scaleEffect = 1;
		#endregion

		public TextRenderer(bool instantlyAdd = true) : base(instantlyAdd) { }

		public override void Initialize() { }

		/// <summary>
		/// Gera os blocos de texto para renderização.
		/// </summary>
		private void BuildText()
        {
            textBlocks.Clear();

            MatchCollection matches = CompilerRegex.Matches(Text);
            TextConfig current = new TextConfig();
            int charCount = 0;

            for (int i = 0; i < matches.Count; i++)
            {
                string match = matches[i].Value;

                if (match.StartsWith("<") && match.EndsWith(">"))
                {
                    current = new TextConfig();

                    if (match != "<>") BuildConfig(match, ref current);
                }
                else
                {
                    textBlocks.Add(new TextBlock(match, current));

                    FilteredText += match;
                }
            }
            
            switch (TextWrap)
            {
                case TextWrap.Anywhere: WrapAnywhere(); break;
                case TextWrap.Word: WrapByWord(); break;
            }
        }
        /// <summary>
        /// Altera os valores de um <see cref="TextConfig"/> com base nos argumentos de uma tag.
        /// </summary>
        private void BuildConfig(string tag, ref TextConfig config)
        {
            string[] arguments = tag.Substring(1, tag.Length - 2).Split('|');

            for (int i = 0; i < arguments.Length; i++)
            {
                if (!arguments[i].Contains('=')) continue;

                string[] arg = arguments[i].Split('=');

                switch (arg[0].ToLower())
                {
                    case "config": config.Copy(TextConfigs[arg[1]]); break;
                    case "color":
                        if (arg[1][0] == '#') config.Color = new Color(arg[1]);
                        else
                        {
                            string colorParsing = arg[1].Replace("{", "").Replace("}", "").Replace("R:", "").Replace("G:", "").Replace("B:", "").Replace("A:", "");
                            string[] colorParse = colorParsing.Split(' ');

                            config.Color = new Color(int.Parse(colorParse[0]), int.Parse(colorParse[1]), int.Parse(colorParse[2]), int.Parse(colorParse[3]));
                        }
                        break;
                    case "font": config.Font = Asset.Load<SpriteFont>(arg[1]); break;
                    case "origin": config.Origin = (TextOrigin)Enum.Parse(typeof(TextOrigin), arg[1]);  break;
                    case "scale":
                        string vector2Parsing = arg[1].Replace("{", "").Replace("}", "").Replace("X:", "").Replace("Y:", "");
                        string[] vector2Parse = vector2Parsing.Split(' ');

                        config.Scale = new Vector2(float.Parse(vector2Parse[0]), float.Parse(vector2Parse[1]));
                        break;
                    case "rotation": config.Rotation = float.Parse(arg[1]); break;
                    case "effect": config.Effect = (TextEffect)Enum.Parse(typeof(TextEffect), arg[1]); break;
                    case "flip": config.Flip = (Flip)Enum.Parse(typeof(Flip), arg[1]); break;
                }
            }
        }
        /// <summary>
        /// Realiza a quebra de um texto dentro de um limite.
        /// </summary>
        private void WrapAnywhere()
        {
            if (MaxLength <= 0) return;

            int charCount = 0;

            for (int i = 0; i < textBlocks.Count; i++)
            {
                for (int j = 0; j < textBlocks[i].text.Length; j++)
                {
                    if (textBlocks[i].text[j] == '\n') charCount = 0;
                    else if (charCount == MaxLength)
                    {
                        string previous = textBlocks[i].text.Substring(0, j),
                            post = textBlocks[i].text.Substring(j);
                        textBlocks[i].text = previous;

                        textBlocks.Insert(i + 1, LineBreak);
                        textBlocks.Insert(i + 2, new TextBlock(post, textBlocks[i].config));

                        i++;

                        charCount = 0;
                    }
                    else charCount++;
                }
            }
        }
        /// <summary>
        /// Realiza a quebra de um texto por palavra dentro de um limite.
        /// </summary>
        private void WrapByWord()
        {
            if (MaxLength <= 0) return;

            int charCount = 0;

            for (int i = 0; i < textBlocks.Count; i++)
            {
                for (int j = 0; j < textBlocks[i].text.Length; j++)
                {
                    bool breakLine = false;

                    if (textBlocks[i].text[j] == '\n') charCount = 0;
                    else if ((j > 0 && textBlocks[i].text[j - 1] == ' ' && IsBreakNeeded(i, j, charCount)) || charCount == MaxLength) breakLine = true;
                    else charCount++;

                    if (breakLine)
                    {
                        string previous = textBlocks[i].text.Substring(0, j),
                            post = textBlocks[i].text.Substring(j);
                        textBlocks[i].text = previous;

                        textBlocks.Insert(i + 1, LineBreak);
                        textBlocks.Insert(i + 2, new TextBlock(post, textBlocks[i].config));

                        i++;

                        charCount = 0;
                    }
                }
            }
        }
        /// <summary>
        /// Conta a largura da próxima palavra e diz se precisa quebrar de linha.
        /// </summary>
        private bool IsBreakNeeded(int blockIndex, int charIndex, int charCount)
        {
            int count = 0;

            for (; blockIndex < textBlocks.Count; blockIndex++)
            {
                for (; charIndex < textBlocks[blockIndex].text.Length; charIndex++)
                {
                    if (textBlocks[blockIndex].text[charIndex] == ' ' || textBlocks[blockIndex].text[charIndex] == '\n') return charCount + count >= MaxLength;
                    else count++;
                }

                charIndex = 0;

                if (count + charCount >= MaxLength) return true;
            }

            return true;
        }

        public void Step()
        {
            float delta = UseScaledTime ? Time.DeltaTime : Time.UnscaledDeltaTime;

            if (scaleEffect <= PulsateLimit.X) PulsateAmount = Math.Abs(PulsateAmount);
            else if (scaleEffect >= PulsateLimit.Y) PulsateAmount = -Math.Abs(PulsateAmount);

            theta += Time.TargetFPS * delta;
            scaleEffect += Time.TargetFPS * PulsateAmount * delta;
            rotationEffect += Time.TargetFPS * RotationAmount * delta;

            if (rotationEffect >= 360) rotationEffect -= 360;

            if (needToUpdate)
            {
                needToUpdate = false;

                BuildText();
            }
        }
        public void Render()
        {
            if (FilteredText.Length == 0) return;

            int charCount = 0;
            float charHeight = 0;
            Vector2 basePos = Vector2.Zero;

            for (int i = 0; i < textBlocks.Count; i++)
            {
                string text = textBlocks[i].text;
                float rotation = textBlocks[i].config.Rotation ?? DefaultConfig.Rotation.Value;
                Color color = textBlocks[i].config.Color ?? DefaultConfig.Color.Value;
                Vector2 scale = textBlocks[i].config.Scale ?? DefaultConfig.Scale.Value;
                TextEffect effect = textBlocks[i].config.Effect ?? DefaultConfig.Effect.Value;
                SpriteFont font = textBlocks[i].config.Font ?? DefaultConfig.Font;
                Vector2 origin = OriginAsVector[textBlocks[i].config.Origin ?? DefaultConfig.Origin.Value];
                Flip flip = textBlocks[i].config.Flip ?? DefaultConfig.Flip.Value;

                switch (flip)
                {
                    case Flip.Horizontal: scale.X *= -1; break;
                    case Flip.Vertical: scale.Y *= -1; break;
                    case Flip.Both: scale *= -1; break;
                }

                if (font != null && text.Length > 0)
                {
                    Vector2 measure = font.MeasureString(text) * scale,
                        center = basePos + measure * origin;

                    for (int j = 0; j < text.Length; j++)
                    {
                        if (MaxChar >= 0 && charCount > MaxChar) return;

                        char glyphChar = text[j];

                        switch (glyphChar)
                        {
                            case '\r': continue;
                            case '\n':
                                basePos.X = 0;
                                basePos.Y += charHeight + font.Spacing.Y * scale.Y;
                                charHeight = 0;
                                break;
                            case ' ':
                                if (font.Glyphs.ContainsKey(glyphChar)) goto default;

                                basePos.X += font.Spacing.X * scale.X;
                                break;
                            default:
                                Glyph glyph = font.Glyphs[glyphChar];
                                Vector2 charMeasure = font.MeasureChar(glyphChar);

                                float charAngle = rotation;
                                Vector2 charPos = basePos;
                                Vector2 addToPos = Vector2.Zero, charScale = scale;

                                switch (effect)
                                {
                                    case TextEffect.Rotation: charAngle = rotationEffect; break;
                                    case TextEffect.Pulsate:
                                        charScale *= scaleEffect;
                                        charPos.Y += measure.Y * (1 - charScale.Y) * origin.Y;
                                        break;
                                    case TextEffect.Wave:
                                        float so = theta + j;
                                        addToPos.Y = (float)Math.Sin(so * Math.PI * WaveSpeed) * (charMeasure.Y * WaveAmplitude);
                                        break;
                                    case TextEffect.Scream: addToPos = new Vector2(random.Next(-(int)ScreamOffset.X, (int)ScreamOffset.X), random.Next(-(int)ScreamOffset.Y, (int)ScreamOffset.Y)); break;
                                    case TextEffect.MovingHorizontal: charPos += new Vector2((float)Math.Sin(theta * Math.PI * MovingSpeed.X) * measure.X / MovingAmplitude.X, 0); break;
                                    case TextEffect.MovingVertical: charPos += new Vector2(0, (float)Math.Sin(theta * Math.PI * MovingSpeed.Y) * measure.Y / MovingAmplitude.Y); break;
                                }

                                charPos = Vector2.Rotate(charPos, center, charAngle) + addToPos;
                                charPos = Vector2.Rotate(charPos * Transform.Scale + Transform.Position, Transform.Position, Transform.Rotation);

								Draw.Sprite(font.Sprite, charPos, glyph.Area, color, charAngle + Transform.Rotation, Vector2.Zero, charScale * Transform.Scale, 0);
                                
                                charHeight = Math.Max(charHeight, charMeasure.Y * charScale.Y);

                                if (j > 0) basePos.X += glyph.KerningPair.Get(text[j - 1], 0) * charScale.X;

                                basePos.X += glyph.Area.Width * charScale.X;

                                if (j != text.Length - 1) basePos.X += font.Spacing.X * charScale.X;
                                break;
                        }

                        charCount++;
                    }
                }
            }
        }

        private class TextBlock
        {
            public string text;
            public TextConfig config;

            public TextBlock(string text, TextConfig config)
            {
                this.text = text;
                this.config = config;
            }
        }
    }
}