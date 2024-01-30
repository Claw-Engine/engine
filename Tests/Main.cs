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
            Draw.SetCamera(new Camera());
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
            else if (Input.KeyPressed(Keys.Down)) Tilemap.Resize(Tilemap.Size - new Vector2(1, 0));
            else if (Input.KeyPressed(Keys.Up)) Tilemap.Resize(Tilemap.Size + new Vector2(1, 0));

            updateables.ForEach((u) => u.Step());
        }

        protected override void Render()
        {
            drawables.ForEach((d) => d.Render());
            Draw.FilledCircle(0, 4, Tilemap.PositionToGrid(Draw.GetCamera().ScreenToWorld(Input.MousePosition)), 0, Color.Red);
            //Draw.Text(Font, "Testando coisas...", Vector2.Zero, Color.White);
        }
    }
}