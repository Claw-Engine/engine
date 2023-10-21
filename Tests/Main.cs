using System;
using Claw;
using Claw.Graphics;
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
        }
        private void LoadContent()
        {
            TextureAtlas.AddSprites(Asset.Load<Sprite[]>("MainAtlas"));

            Font = Asset.Load<SpriteFont>("Fonts/font");
        }
        
        protected override void Step()
        {
            updateables.ForEach((u) => u.Step());
        }

        protected override void Render()
        {
            drawables.ForEach((d) => d.Render());
            Draw.Text(Font, "Testando coisas...", Vector2.Zero, Color.White);
        }
    }
}