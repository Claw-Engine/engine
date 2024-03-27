using System.IO;

namespace Claw.Tiled
{
    /// <summary>
    /// Representa um vértice do Tiled.
    /// </summary>
    public sealed class Vertice
    {
        public float x = 0, y = 0;

        internal static Vertice ReadVertice(BinaryReader reader)
        {
            Vertice vertice = new Vertice();
            vertice.x = reader.ReadSingle();
            vertice.y = reader.ReadSingle();

            return vertice;
        } 
		internal static Vertice[] ReadPolygon(BinaryReader reader)
		{
            int verticeCount = reader.ReadInt32();
            Vertice[] vertices = new Vertice[verticeCount];

            for (int i = 0; i < verticeCount; i++) vertices[i] = ReadVertice(reader);

            return vertices;
		}
	}
}