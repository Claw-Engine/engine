using System;
using Claw;
using Claw.Graphics;
using Claw.Input;
using Claw.Save;
using Claw.Tiled;
using Claw.Utils;

namespace Tests
{
    public class Main : Game
    {
        public static SpriteFont Font;
        private ComponentSortingFilteringCollection<IUpdateable> updateables;
        private ComponentSortingFilteringCollection<IDrawable> drawables;

        protected override void Initialize()
        {
            Window.Title = "Teste";
            Window.CanUserResize = true;

            updateables = Components.CreateForUpdate();
            drawables = Components.CreateForDraw();

            LoadContent();
            Draw.SetCamera(new Camera() { Position = new Vector2(0, -64) });
        }
        private void LoadContent()
        {
            Asset.Load<Sprite[]>("MainAtlas");

            Font = Asset.Load<SpriteFont>("Fonts/font");
            Tiled.Config = new Config("Tests");
            
            Tiled.Config.AddPalettes(("iso", TextureAtlas.Sprites["MainAtlas/isometric_tiles"]), ("ort", TextureAtlas.Sprites["MainAtlas/collision_tiles"]));
            Tiled.Load(Asset.Load<Map>("Maps/sisotest"));
        }
        
        protected override void Step()
        {
            if (Input.KeyDown(Keys.Right)) Draw.GetCamera().Position.X += 4;
            else if (Input.KeyDown(Keys.Left)) Draw.GetCamera().Position.X -= 4;
            else if (Input.KeyDown(Keys.Down)) Draw.GetCamera().Position.Y += 4;
            else if (Input.KeyDown(Keys.Up)) Draw.GetCamera().Position.Y -= 4;
            else if (Input.MouseButtonPressed(MouseButtons.Left)) Tilemap[Tilemap.LayerCount - 1][Tilemap.PositionToCell(Draw.GetCamera().ScreenToWorld(Input.MousePosition))] = 1;
            else if (Input.KeyPressed(Keys.NumPad0)) Tiled.Load(Asset.Load<Map>("Maps/ortest"));
            else if (Input.KeyPressed(Keys.NumPad1)) Tiled.Load(Asset.Load<Map>("Maps/sisotest"));
            else if (Input.KeyPressed(Keys.NumPad2)) Tiled.Load(Asset.Load<Map>("Maps/isotest"));

            updateables.ForEach((u) => u.Step());
        }

        protected override void Render()
        {
            Draw.Rectangle(1, new Rectangle(Vector2.Zero, Tilemap.PixelSize), Color.Red);
            drawables.ForEach((d) => d.Render());
            Draw.FilledCircle(0, 4, Tilemap.PositionToGrid(Draw.GetCamera().ScreenToWorld(Input.MousePosition)), 0, Color.Red);
            Draw.Text(Font, Tilemap.PositionToCell(Draw.GetCamera().ScreenToWorld(Input.MousePosition)).ToString(), new Vector2(0, 300), Color.Red);
        }
    }
}