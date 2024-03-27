using System;
using Claw;
using Claw.Input;
using Claw.Modules;
using Claw.Physics;
using Claw.Tiled;

namespace Tests
{
	internal class Main : Game
	{
		protected override void Initialize()
		{
			Tiled.Config = new Config("Tests");
			PhysicsManager.Gravity = Vector2.Zero;

			Tiled.Load(Asset.Load<Map>("Maps/l0"));
		}

		protected override void Step()
		{
			for (int i = 0; i < Modules.Count; i++) ((IStep)Modules[i]).Step();
		}

		protected override void Render()
		{
			for (int i = 0; i < Modules.Count; i++) ((IRender)Modules[i]).Render();
		}
	}
	public class DrawBody : RigidBody, IRender
	{
		public int RenderOrder => 0;
		public event Action<IRender> RenderOrderChanged;

		public DrawBody(bool instantlyAdd = false) : base(instantlyAdd) { }

		public override void Initialize()
		{
			Console.WriteLine(Shape);

			if (Shape is CircleShape c) Console.WriteLine(c.Radius);
			else if (Shape is PolygonShape p) Console.WriteLine(p.VerticeCount);
		}

		public void Render()
		{
			Claw.Graphics.Draw.DebugBody(1, this, Color.Red, 32);
		}
	}
}