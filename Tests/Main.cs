using System;
using System.Collections.Generic;
using Claw;
using Claw.Input;
using Claw.Graphics;
using Claw.Utils;
using Claw.Save;

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

            Dictionary<string, int> dict = new Dictionary<string, int> { { "Jõao", 10 }, { "Pedro", -2 } };

            Save.Open(".sv");
            Save.Write("Test", "Sub", 10.5f);
            Save.Write("Test2", "Sub", "João");
            Save.Write("Other", "Pai", null);
            Save.Write("Other", "Posição", new Vector2(10, 15));
            Save.Write("Other2", "Array", new int[] { 10, 15, 3 });
            Save.Write("Other2", "List", new List<int> { 13, 2, -2 });
            Save.Write("Other2", "Dict", dict);
            Save.Write("Other2", "Tuple", (10, 2));
            Save.Write("Ref", "Ref", dict);
            Save.Close();
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