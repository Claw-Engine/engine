using System;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Newtonsoft.Json;
using Claw.Graphics;
using Clawssets.Builder.Data;

namespace Clawssets.Builder
{
    /// <summary>
    /// Classe para a compilação de fontes.
    /// </summary>
    public static class FontBuilder
    {
        /// <summary>
        /// Compila fontes (reais ou baseadas em sprite) para a pasta do grupo.
        /// </summary>
        public static void Build(AssetGroup group, string basePath)
        {
            string directory = Path.Combine(AssetBuilder.BuildDirectory, group.Name);

            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            for (int i = 0; i < group.Files.Count; i++)
            {
                string path = Path.GetFullPath(Path.Combine(basePath, group.Files[i]));
                string json = File.ReadAllText(path);
                FontDescription description = JsonConvert.DeserializeObject<FontDescription>(json);
                Font font = GetFont(description.FontName, description.Size, (FontStyle)Enum.Parse(typeof(FontStyle), description.Style));

                if (font == null)
                {
                    Console.WriteLine("Erro: Ocorreu um erro ao tentar carregar a fonte {0}", group.Files[i]);

                    return;
                }
                else
                {
                    string text = description.RegionsToText();
                    Bitmap copy = Properties.Resources.BaseAtlas;
                    Bitmap image = copy.Clone(new RectangleF(Point.Empty, copy.Size), PixelFormat.Format32bppArgb);
                    Graphics graphics = Graphics.FromImage(image);
                    graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
                    Brush brush = new SolidBrush(Color.White);

                    graphics.FillRectangle(brush, new Rectangle(0, 0, 1, 1));

                    Dictionary<char, Glyph> glyphs = graphics.DrawFont(font, text, brush, description.Size, image.Size, out SizeF fontAtlasSize);

                    if (glyphs != null)
                    {
                        byte[] pixels = image.GetPixels(new Rectangle(Point.Empty, new Size((int)Math.Ceiling(fontAtlasSize.Width), (int)Math.Ceiling(fontAtlasSize.Height))));

                        string outputPath = Path.Combine(directory, Path.GetFileNameWithoutExtension(path) + AssetBuilder.AssetExtension);

                        SaveFont(outputPath, string.Format("{0}/{1}", group.Name, Path.GetFileNameWithoutExtension(group.Files[i])), pixels, fontAtlasSize, description, glyphs);
                    }
                    else return;

                    graphics.Dispose();
                    image.Dispose();
                }
            }

            Console.WriteLine("Grupo \"{0}\" compilado com sucesso!", group.Name);
        }

        /// <summary>
        /// Salva a fonte num arquivo binário.
        /// </summary>
        private static void SaveFont(string path, string fontName, byte[] pixels, SizeF size, FontDescription description, Dictionary<char, Glyph> glyphs)
        {
            StreamWriter writer = new StreamWriter(path);
            BinaryWriter binWriter = new BinaryWriter(writer.BaseStream);
            
            binWriter.WriteTexture(pixels, size);
            binWriter.Write(fontName);
            binWriter.Write(description.Spacing);
            binWriter.Write(glyphs);

            binWriter.Close();
        }

        /// <summary>
        /// Cria uma fonte, baseado nos parâmetros especificados.
        /// </summary>
        private static Font GetFont(string fontName, int size, FontStyle style)
        {
            InstalledFontCollection fonts = new InstalledFontCollection();

            for (int i = 0; i < fonts.Families.Length; i++)
            {
                if (fonts.Families[i].Name == fontName) return new Font(fonts.Families[i], size, style);
            }

            return null;
        }

        /// <summary>
        /// Desenha a fonte no atlas e retorna um dicionário com os <see cref="Glyph"/>s.
        /// </summary>
        private static Dictionary<char, Glyph> DrawFont(this Graphics graphics, Font font, string characters, Brush brush, int emSize, Size imageSize, out SizeF measure)
        {
            PointF basePos = new PointF(AtlasBuilder.HorizontalOffset, 0);
            Dictionary<char, Glyph> result = new Dictionary<char, Glyph>();
            List<Kerning.Pair> pairs = font.GetKerningPairs(characters);
            float charHeight = 0;
            measure = SizeF.Empty;

            for (int i = 0; i < characters.Length; i++)
            {
                char glyphChar = characters[i];

                switch (glyphChar)
                {
                    case '\n': continue;
                    default:
                        SizeF charMeasure = font.MeasureString(glyphChar.ToString());

                        if (basePos.X + charMeasure.Width >= imageSize.Width)
                        {
                            basePos.X = 0;
                            basePos.Y += charHeight;
                            measure.Height = basePos.Y;
                            charHeight = 0;
                        }

                        charHeight = Math.Max(charHeight, (int)charMeasure.Height);
                        
                        graphics.DrawString(glyphChar.ToString(), font, brush, new PointF(basePos.X, basePos.Y));

                        Dictionary<char, float> glyphKerningPairs = new Dictionary<char, float>();

                        foreach (Kerning.Pair pair in pairs.Where((p) => p.wSecond == glyphChar)) glyphKerningPairs.Add((char)pair.wFirst, pair.iKernAmount);

                        /*
                         Como o tamanho dos caracteres não é exato, fazemos um crop apenas no source do caractere.
                         Usar o tamanho exato para gerar o atlas acaba gerando uma imprecisão muito alta.
                         */
                        SizeF sizeDiff = charMeasure - font.MeasureString(glyphChar.ToString(), emSize);
                        sizeDiff.Width *= .5f;
                        sizeDiff.Height *= .5f;
                        Claw.Rectangle rect = new Claw.Rectangle(basePos.X + sizeDiff.Width, basePos.Y + sizeDiff.Height, charMeasure.Width - sizeDiff.Width, charMeasure.Height - sizeDiff.Height);

                        result.Add(glyphChar, new Glyph(rect, glyphKerningPairs));

                        basePos.X += (int)charMeasure.Width;
                        measure.Width = Math.Max(measure.Width, basePos.X);

                        if (basePos.X >= imageSize.Width)
                        {
                            basePos.X = 0;
                            basePos.Y += charHeight;
                            measure.Height = basePos.Y;
                            charHeight = 0;
                        }
                        break;
                }

                if (basePos.Y >= imageSize.Height)
                {
                    Console.WriteLine("Erro: A fonte {0} ultrapassou o atlas!", font.Name);

                    return null;
                }
            }

            return result;
        }

        /// <summary>
        /// Retorna o tamanho de uma string, baseado na fonte.
        /// </summary>
        private static SizeF MeasureString(this Font font, string text, int? width = null)
        {
            Image image = new Bitmap(1, 1);
            Graphics graphics = Graphics.FromImage(image);
            SizeF measure = SizeF.Empty, lineSize = SizeF.Empty;
            
            for (int i = 0; i < text.Length; i++)
            {
                char glyphChar = text[i];

                switch (glyphChar)
                {
                    case '\n':
                        measure.Height += lineSize.Height;
                        measure.Width = Math.Max(measure.Width, lineSize.Width);
                        lineSize = SizeF.Empty;
                        break;
                    default:
                        SizeF charMeasure = SizeF.Empty;

                        if (width.HasValue) charMeasure = graphics.MeasureString(glyphChar.ToString(), font, width.Value, StringFormat.GenericTypographic);
                        else charMeasure = graphics.MeasureString(glyphChar.ToString(), font);

                        lineSize.Height = Math.Max(lineSize.Height, charMeasure.Height);
                        lineSize.Width += charMeasure.Width;
                        break;
                }
            }

            measure.Height += lineSize.Height;
            measure.Width = Math.Max(measure.Width, lineSize.Width);

            graphics.Dispose();
            image.Dispose();

            return measure;
        }

        /// <summary>
        /// Escreve um array de glyphs num arquivo binário.
        /// </summary>
        private static void Write(this BinaryWriter writer, Dictionary<char, Glyph> glyphs)
        {
            writer.Write(glyphs.Count);

            foreach (KeyValuePair<char, Glyph> glyph in glyphs)
            {
                writer.Write(glyph.Key);
                writer.Write((int)glyph.Value.Area.X);
                writer.Write((int)glyph.Value.Area.Y);
                writer.Write((int)glyph.Value.Area.Width);
                writer.Write((int)glyph.Value.Area.Height);

                writer.Write(glyph.Value.KerningPair.Count);

                foreach (KeyValuePair<char, float> kerning in glyph.Value.KerningPair)
                {
                    writer.Write(kerning.Key);
                    writer.Write(kerning.Value);
                }
            }
        }

        /// <summary>
        /// Descreve uma fonte.
        /// </summary>
        private class FontDescription
        {
            public int Size;
            public bool UseKerning;
            public float Spacing;
            public string FontName, Style;
            public CharacterRegion[] CharacterRegions;

            /// <summary>
            /// Converte os <see cref="CharacterRegion"/> em um único texto, com quebra de linha para cada grupo.
            /// </summary>
            public string RegionsToText()
            {
                string text = string.Empty;

                for (int i = 0; i < CharacterRegions.Length; i++)
                {
                    for (int j = CharacterRegions[i].Start; j <= CharacterRegions[i].End; j++) text += (char)j;

                    if (i != CharacterRegions.Length - 1) text += '\n';
                }
                
                return text;
            }
        }
        /// <summary>
        /// Descreve um grupo de caracteres, de um ponto A ao B.
        /// </summary>
        private class CharacterRegion
        {
            public int Start, End;
        }
    }
}