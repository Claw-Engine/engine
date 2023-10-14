using System;
using Claw;
using Claw.Input;
using Claw.Graphics;
using Claw.Audio;
using Claw.Utils;

namespace Tests
{
    public class Main : Game
    {
        public static SpriteFont Font;
        public static SoundEffect SFX;
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
            SFX = Asset.Load<SoundEffect>("Sounds/honk");
        }
        
        protected override void Step()
        {
            if (Input.KeyPressed(Keys.Space)) Audio.Play(SFX.CreateInstance(SoundEffectGroup.SoundEffect));

            updateables.ForEach((u) => u.Step());
        }

        protected override void Render()
        {
            drawables.ForEach((d) => d.Render());
            Draw.Text(Font, "Testando coisas...", Vector2.Zero, Color.White);
        }
    }
}