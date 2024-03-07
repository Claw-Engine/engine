using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Claw.Modules;
using Claw.Physics;

namespace Claw
{
    /// <summary>
    /// Representa um colisor poligonal.
    /// </summary>
    public sealed class Polygon : IDisposable
    {
        public bool CanRotate = true, CanScale = true;
        public bool Enabled
        {
            get => _enabled && Module.Enabled;
            set => _enabled = value;
        }
        private bool _enabled = true;

        public Vector2 Offset = Vector2.Zero;
        public readonly BaseModule Module;
        public Line[] Lines = new Line[0];
        public Line[] LinesInWorld
        {
            get
            {
                Line[] lines = new Line[this.Lines.Length];

                for (int i = 0; i < lines.Length; i++) lines[i] = LineToWorld(this.Lines[i], this);

                return lines;
            }
        }
        
        public float Top
        {
            get
            {
                float topS = LinesInWorld.OrderBy(start => start.Start.Y).First().Start.Y;
                float topE = LinesInWorld.OrderBy(end => end.End.Y).First().End.Y;
                float top = topS < topE ? topS : topE;

                return top;
            }
        }
        public float Bottom
        {
            get
            {
                float bottomS = LinesInWorld.OrderBy(start => start.Start.Y).Last().Start.Y;
                float bottomE = LinesInWorld.OrderBy(end => end.End.Y).Last().End.Y;
                float bottom = bottomS > bottomE ? bottomS : bottomE;

                return bottom;
            }
        }
        public float Right
        {
            get
            {
                float rightS = LinesInWorld.OrderBy(start => start.Start.X).Last().Start.X;
                float rightE = LinesInWorld.OrderBy(end => end.End.X).Last().End.X;
                float right = rightS > rightE ? rightS : rightE;

                return right;
            }
        }
        public float Left
        {
            get
            {
                float leftS = LinesInWorld.OrderBy(start => start.Start.X).First().Start.X;
                float leftE = LinesInWorld.OrderBy(end => end.End.X).First().End.X;
                float left = leftS < leftE ? leftS : leftE;

                return left;
            }
        }
        public Vector2 Center
        {
            get
            {
                if (Lines.Length > 0)
                {
                    float r = Right, l = Left, t = Top, b = Bottom;
                    float xx = Math.Abs(r - l), yy = Math.Abs(b - t);

                    return new Vector2(l + xx * .5f, t + yy * .5f);
                }

                return Module.Transform.Position;
            }
        }

        public Polygon(BaseModule module)
        {
            Module = module;

            Module.Polygons.Add(this);
        }
        public Polygon(BaseModule module, params Line[] lines) : this(module) => Lines = lines;
        ~Polygon() => Dispose();

        public void Dispose() => Lines = null;

        private static Line LineToWorld(Line line, Polygon polygon)
        {
            Line result;

            if (polygon.CanRotate)
            {
                if (polygon.CanScale) result = Line.Rotate(line * polygon.Module.Transform.Scale + polygon.Module.Transform.Position + polygon.Offset, polygon.Module.Transform.Position, polygon.Module.Transform.Rotation);
                else result = Line.Rotate(line + polygon.Module.Transform.Position + polygon.Offset, polygon.Module.Transform.Position, polygon.Module.Transform.Rotation);
            }
            else
            {
                if (polygon.CanScale) result = line * polygon.Module.Transform.Scale + polygon.Module.Transform.Position + polygon.Offset;
                else result = line + polygon.Module.Transform.Position + polygon.Offset;
            }

            return result;
        }
        
        /// <summary>
         /// Cria um colisor quadrado.
         /// </summary>
        public static Polygon Box(BaseModule module, Rectangle rectangle) => new Polygon(module, Line.BoxGenerator(rectangle));
        /// <summary>
        /// Cria um colisor circular.
        /// </summary>
        public static Polygon Circle(BaseModule module, float radius, Vector2 center, int segments = 16) => new Polygon(module, Line.CircleGenerator(radius, center, segments));

        /// <summary>
        /// Checa se um polígono está colidindo com outro (formado apenas por linhas).
        /// </summary>
        public static CollisionResult OverlapPolygon(Polygon polygon, IEnumerable<Line> lines)
        {
            if(polygon.Enabled)
            {
                foreach (Line line in lines)
                {
                    CollisionResult collisionResult = IntersectRay(line, polygon);

                    if (collisionResult.Intersect) return collisionResult;
                }
            }

            return new CollisionResult(false, null, null, null);
        }

        /// <summary>
        /// Checa se um ponto está colidindo com um <see cref="Polygon"/>.
        /// </summary>
        public static CollisionResult Intersect(Vector2 point, Polygon polygon)
        {
            bool result = false;

            if (polygon.Enabled)
            {
                foreach (Line nline in polygon.Lines)
                {
                    Line line = LineToWorld(nline, polygon);

                    if (point.Y > Math.Min(line.Start.Y, line.End.Y) && point.Y <= Math.Max(line.Start.Y, line.End.Y) && point.X <= Math.Max(line.Start.X, line.End.X) && line.Start.Y != line.End.Y)
                    {
                        float xInter = (point.Y - line.Start.Y) * (line.End.X - line.Start.X) / (line.End.Y - line.Start.Y) + line.Start.X;

                        if (line.Start.X == line.End.X || point.X <= xInter) result = !result;
                    }
                }
            }

            return new CollisionResult(result, point, result ? polygon.Module : null, result ? polygon : null);
        }
        /// <summary>
        /// Checa se um ponto está colidindo com um <see cref="Polygon"/>.
        /// </summary>
        public static CollisionResult Intersect(Vector2 point, string tag, float maxDistance = 0, bool first = false) => Intersect(point, TagManager.GetModules(tag, true), maxDistance, first);
        /// <summary>
        /// Checa se um ponto está colidindo com um <see cref="Polygon"/>.
        /// </summary>
        public static CollisionResult Intersect<T>(Vector2 point, float maxDistance = 0, bool first = false) where T : GameObject => Intersect(point, SceneManager.FindModulesOfType<T>(), maxDistance, first);

        /// <summary>
        /// Checa se dois polígonos estão colidindo.
        /// </summary>
        public static CollisionResult Intersect(Polygon polygon, Polygon other)
        {
            if (polygon.Enabled && other.Enabled && polygon != other)
            {
                foreach (Line nline in polygon.Lines)
                {
                    Line line = LineToWorld(nline, polygon);

                    CollisionResult collisionResult = IntersectRay(line, other);

                    if (collisionResult.Intersect) return collisionResult;
                }
            }

            return new CollisionResult(false, null, null, null);
        }
        /// <summary>
        /// Checa se dois polígonos estão colidindo.
        /// </summary>
        public static CollisionResult Intersect(Polygon polygon, string tag, float maxDistance = 0, bool first = false, bool invert = false) => Intersect(polygon, TagManager.GetModules(tag, true), maxDistance, first, invert);
        /// <summary>
        /// Checa se dois polígonos estão colidindo.
        /// </summary>
        public static CollisionResult Intersect<T>(Polygon polygon, float maxDistance = 0, bool first = false, bool invert = false) where T : GameObject => Intersect(polygon, SceneManager.FindModulesOfType<T>(), maxDistance, first, invert);

        /// <summary>
        /// Checa se uma linha está colidindo com um <see cref="Polygon"/>.
        /// </summary>
        public static CollisionResult IntersectRay(Line ray, Polygon polygon, float cellSize = 1)
        {
            CollisionResult result = new CollisionResult(false, null, null, null);

            RayCaster.Cast(ray, (Vector2 check) =>
            {
                result = Intersect(check, polygon);

                return result;
            }, out Vector2? hitPoint, new Vector2(cellSize));

            return result;
        }
        /// <summary>
        /// Checa se uma linha está colidindo com um <see cref="Polygon"/>.
        /// </summary>
        public static CollisionResult IntersectRay(Line ray, string tag, float maxDistance = 0, bool first = false, float cellSize = 1) => IntersectRay(ray, TagManager.GetModules(tag, true), maxDistance, first, cellSize);
        /// <summary>
        /// Checa se uma linha está colidindo com um <see cref="Polygon"/>.
        /// </summary>
        public static CollisionResult IntersectRay<T>(Line ray, float maxDistance = 0, bool first = false, float cellSize = 1) where T : GameObject => IntersectRay(ray, SceneManager.FindModulesOfType<T>(), maxDistance, first, cellSize);

        /// <summary>
        /// Retorna uma lista de colisões com o ponto especificado.
        /// </summary>
        public static CollisionResult[] IntersectList(Vector2 point, Func<BaseModule, bool> predicate)
        {
            BaseModule[] modules = Game.Instance.Modules.Where(predicate).ToArray();
            List<CollisionResult> results = new List<CollisionResult>();

            foreach (BaseModule module in modules)
            {
                List<Polygon> polygons = module.Polygons;

                if (polygons != null)
                {
                    foreach (Polygon polygon in polygons)
                    {
                        var data = Intersect(point, polygon);

                        if (data.Intersect) results.Add(data);
                    }
                }
            }

            return results.ToArray();
        }
        /// <summary>
        /// Retorna uma lista de colisões com o polígono especificado.
        /// </summary>
        public static CollisionResult[] IntersectList(Polygon polygon, Func<BaseModule, bool> predicate)
        {
            if (polygon.Enabled)
            {
                BaseModule[] modules = Game.Instance.Modules.Where(predicate).ToArray();
                List<CollisionResult> results = new List<CollisionResult>();

                foreach (BaseModule module in modules)
                {
                    if (module != polygon.Module && module.Enabled)
                    {
                        var polygons = module.Polygons;

                        foreach (Polygon other in polygons)
                        {
                            var data = Intersect(polygon, other);

                            if (data.Intersect) results.Add(data);
                        }
                    }
                }

                return results.ToArray();
            }

            return new CollisionResult[0];
        }

        /// <summary>
        /// Checa se um ponto está colidindo com um dos <see cref="Polygon"/>s de uma lista.
        /// </summary>
        public static CollisionResult IntersectWith(Vector2 point, IEnumerable<BaseModule> modules, float maxDistance = 0, bool first = false) => Intersect(point, modules, maxDistance, first);
        /// <summary>
        /// Checa se um <see cref="Polygon"/> está colidindo com um dos <see cref="Polygon"/>s de uma lista.
        /// </summary>
        public static CollisionResult IntersectWith(Polygon polygon, IEnumerable<BaseModule> modules, float maxDistance = 0, bool first = false, bool invert = false) => Intersect(polygon, modules, maxDistance, first, invert);

        private static CollisionResult Intersect(Vector2 point, IEnumerable<BaseModule> modules, float maxDistance = 0, bool first = false)
        {
            modules = modules.OrderBy(m => Vector2.Distance(point, m.Transform.Position)).ToArray();
            
            if (maxDistance != 0) modules = modules.Where(m => Vector2.Distance(point, m.Transform.Position) <= maxDistance);

            if (first && modules.Count() > 0) modules = new BaseModule[] { modules.FirstOrDefault() };

			foreach (BaseModule module in modules)
            {
                List<Polygon> polygons = module.Polygons;

                foreach (Polygon other in polygons)
                {
                    CollisionResult intersects = Intersect(point, other);

                    if (intersects) return intersects;
                }
            }

            return new CollisionResult(false, null, null, null);
        }
        private static CollisionResult Intersect(Polygon polygon, IEnumerable<BaseModule> modules, float maxDistance = 0, bool first = false, bool invert = false)
        {
            if (polygon.Enabled)
            {
				modules = modules.OrderBy(m => Vector2.Distance(polygon.Module.Transform.Position, m.Transform.Position));
                
                if (maxDistance != 0) modules = modules.Where(m => Vector2.Distance(polygon.Module.Transform.Position, m.Transform.Position) <= maxDistance);

                if (first && modules.Count() > 0) modules = new BaseModule[] { modules.FirstOrDefault() };

				foreach (BaseModule module in modules)
                {
                    if (module != polygon.Module)
                    {
                        List<Polygon> polygons = module.Polygons;

                        foreach (Polygon other in polygons)
                        {
                            CollisionResult intersects;

                            if (invert)
                            {
                                intersects = Intersect(other, polygon);
                                intersects = new CollisionResult(intersects.Intersect, intersects.CollisionPoint, module, other);
                            }
                            else intersects = Intersect(polygon, other);

                            if (intersects) return intersects;
                        }
                    }
                }
            }

            return new CollisionResult(false, null, null, null);
        }
        private static CollisionResult IntersectRay(Line ray, IEnumerable<BaseModule> modules, float maxDistance = 0, bool first = false, float cellSize = 1)
		{
			modules = modules.OrderBy(m => Vector2.Distance(ray.End, m.Transform.Position));

			if (maxDistance != 0) modules = modules.Where(m => Vector2.Distance(ray.Start, m.Transform.Position) <= maxDistance || Vector2.Distance(ray.End, m.Transform.Position) <= maxDistance);

			if (first && modules.Count() > 0) modules = new BaseModule[] { modules.FirstOrDefault() };

			foreach (BaseModule module in modules)
            {
                foreach (Polygon polygon in module.Polygons)
                {
                    if (polygon.Enabled)
                    {
                        var intersects = IntersectRay(ray, polygon, cellSize);

                        if (intersects.Intersect) return intersects;
                    }
                }
            }

            return new CollisionResult(false, null, null, null);
        }
    }
    /// <summary>
    /// Dados do resultado de uma colisão.
    /// </summary>
    public struct CollisionResult
    {
        public bool Intersect { get; private set; }
        public Vector2? CollisionPoint { get; private set; }
        public BaseModule Module { get; private set; }
        public Polygon Polygon { get; private set; }

        public CollisionResult(bool intersect, Vector2? collisionPoint, BaseModule module, Polygon polygon)
        {
            Intersect = intersect;
            CollisionPoint = collisionPoint;
            Module = module;
            Polygon = polygon;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CollisionResult)) return false;

            var result = (CollisionResult)obj;

            return Intersect == result.Intersect && EqualityComparer<Vector2?>.Default.Equals(CollisionPoint, result.CollisionPoint) &&
                   EqualityComparer<BaseModule>.Default.Equals(Module, result.Module) && EqualityComparer<Polygon>.Default.Equals(Polygon, result.Polygon);
        }
        public override int GetHashCode()
        {
            var hashCode = -741360006;
            hashCode = hashCode * -1521134295 + Intersect.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2?>.Default.GetHashCode(CollisionPoint);
            hashCode = hashCode * -1521134295 + EqualityComparer<BaseModule>.Default.GetHashCode(Module);
            hashCode = hashCode * -1521134295 + EqualityComparer<Polygon>.Default.GetHashCode(Polygon);

            return hashCode;
        }

        public static bool operator ==(CollisionResult a, CollisionResult b) => a.Intersect == b.Intersect && a.CollisionPoint == b.CollisionPoint && a.Module == b.Module && a.Polygon == b.Polygon;
        public static bool operator !=(CollisionResult a, CollisionResult b) => a.Intersect != b.Intersect && a.CollisionPoint != b.CollisionPoint && a.Module != b.Module && a.Polygon != b.Polygon;
        public static bool operator ==(CollisionResult a, bool b) => a.Intersect == b;
        public static bool operator !=(CollisionResult a, bool b) => a.Intersect != b;
        public static bool operator ==(CollisionResult a, Vector2 b) => a.CollisionPoint == b;
        public static bool operator !=(CollisionResult a, Vector2 b) => a.CollisionPoint != b;
        public static bool operator ==(CollisionResult a, BaseModule b) => a.Module == b;
        public static bool operator !=(CollisionResult a, BaseModule b) => a.Module != b;
        public static bool operator ==(CollisionResult a, Polygon b) => a.Polygon == b;
        public static bool operator !=(CollisionResult a, Polygon b) => a.Polygon != b;

        public static implicit operator bool(CollisionResult a) => a.Intersect;
        public static implicit operator Vector2?(CollisionResult a) => a.CollisionPoint;
        public static implicit operator BaseModule(CollisionResult a) => a.Module;
        public static implicit operator Polygon(CollisionResult a) => a.Polygon;
    }
}