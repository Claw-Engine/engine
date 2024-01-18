using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Clawssets.Builder
{
    /// <summary>
    /// Classe para compilação de texturas num texture atlas.
    /// </summary>
    public static class AtlasBuilder
    {
        /// <summary>
        /// Offset horizontal inicial das texturas.
        /// </summary>
        public const int HorizontalOffset = 2;
        /// <summary>
        /// Base para a criação dos texture atlas (o primeiro pixel é branco).
        /// </summary>
        private static Image baseAtlas;
        private static Graphics canvas;

        /// <summary>
        /// Cria o canvas para a criação do texture atlas, com a imagem base.
        /// </summary>
        private static void CreateCanvas()
        {
            if (baseAtlas != null)
            {
                canvas.Dispose();
                baseAtlas.Dispose();
            }

            Bitmap copy = Properties.Resources.BaseAtlas;
            baseAtlas = copy.Clone(new Rectangle(0, 0, copy.Width, copy.Height), PixelFormat.Format32bppArgb);
            canvas = Graphics.FromImage(baseAtlas);
            canvas.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            canvas.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
        }

        /// <summary>
        /// Compila as texturas num único texture atlas.
        /// </summary>
        public static void Build(AssetGroup group, string basePath)
        {
            CreateCanvas();

            string outputPath = Path.Combine(AssetBuilder.BuildDirectory, group.Name + AssetBuilder.AssetExtension);
            string directory = Path.GetDirectoryName(outputPath);

            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            Dictionary<Image, Texture> images = new Dictionary<Image, Texture>(); // Usamos um dicionário para não precisar reordenar duas listas.
            
            for (int i = 0; i < group.Files.Count; i++)
            {
                string path = Path.GetFullPath(Path.Combine(basePath, group.Files[i]));
                Image image = Image.FromFile(path);

                images.Add(image, new Texture(group.Name + "/" + Path.GetFileNameWithoutExtension(group.Files[i]), new Rectangle(Point.Empty, image.Size)));
            }
        
            IOrderedEnumerable<KeyValuePair<Image, Texture>> orderedImages = images.OrderByDescending((pair) => pair.Key.Height);

            Point location = new Point(HorizontalOffset, 0);
            int addToY = 0;
            Size size = new Size(1, 1);
            
            foreach (KeyValuePair<Image, Texture> image in orderedImages)
            {
                if (location.X + image.Key.Width > baseAtlas.Width)
                {
                    location.X = 0;
                    location.Y += addToY + 1;
                    addToY = 0;
                }

                if (location.Y + image.Key.Height > baseAtlas.Height)
                {
                    Console.WriteLine("Erro: O texture atlas passou do limite!");

                    return;
                }

                image.Value.Area.Location = location;
                canvas.DrawImage(image.Key, location.X, location.Y, image.Key.Width, image.Key.Height);

                addToY = Math.Max(addToY, image.Key.Height);
                location.X += image.Key.Width + 1;
                size.Width = Math.Max(location.X, size.Width);
                size.Height = Math.Max(location.Y + image.Key.Height + 1, size.Height);
            }
            
            byte[] pixels = GetPixels(baseAtlas, new Rectangle(Point.Empty, size));

            SaveAtlas(outputPath, orderedImages, size, pixels, group.Files.Count);
        }

        /// <summary>
        /// Salva o texture atlas num arquivo binário.
        /// </summary>
        private static void SaveAtlas(string path, IOrderedEnumerable<KeyValuePair<Image, Texture>> atlas, Size size, byte[] pixels, int imageCount)
        {
            StreamWriter writer = new StreamWriter(path);
            BinaryWriter binWriter = new BinaryWriter(writer.BaseStream);

            binWriter.WriteTexture(pixels, size);
            binWriter.Write(imageCount);

            foreach (KeyValuePair<Image, Texture> image in atlas)
            {
                binWriter.Write(image.Value.Name);
                binWriter.Write(image.Value.Area.X);
                binWriter.Write(image.Value.Area.Y);
                binWriter.Write(image.Value.Area.Width);
                binWriter.Write(image.Value.Area.Height);
            }

            binWriter.Close();
            Console.WriteLine("{0} compilado com sucesso!", Path.GetFileName(path));
        }

        /// <summary>
        /// Retorna os pixels de uma <see cref="Image"/>.
        /// </summary>
        public static byte[] GetPixels(this Image image, Rectangle crop)
        {
            Bitmap bitmap = new Bitmap(image).Clone(crop, image.PixelFormat);
            BitmapData data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] pixels = new byte[Math.Abs(data.Stride) * data.Height];
            Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);
            bitmap.UnlockBits(data);

            bitmap.Dispose();

            return pixels;
        }

        /// <summary>
        /// Escreve uma textura numa <see cref="Stream"/>.
        /// </summary>
        public static void WriteTexture(this BinaryWriter writer, byte[] pixels, SizeF size)
        {
            writer.Write(BitConverter.IsLittleEndian); //A Claw precisa saber se a imagem foi exportada como ARGB ou BGRA.
            writer.Write((int)size.Width);
            writer.Write((int)size.Height);
            writer.Write(pixels);
        }

        /// <summary>
        /// Descreve uma sprite dentro do texture atlas.
        /// </summary>
        private class Texture
        {
            public string Name;
            public Rectangle Area;

            public Texture(string name, Rectangle area)
            {
                Name = name;
                Area = area;
            }
        }
    }
}