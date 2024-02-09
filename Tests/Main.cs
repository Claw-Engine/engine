using System;
using Claw;
using Claw.Graphics;
using Claw.Graphics.UI;
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
            Asset.Load<Sprite[]>("MainAtlas");
            NineSlice.AddTexture("base", TextureAtlas.Sprites["MainAtlas/slice"]);

            Font = Asset.Load<SpriteFont>("Fonts/font");
            UI = new UI();
            UI.Body = new Container() { Style = new Style() { Gap = new Vector2(4), MaxSize = new Vector2(150, 0), TopLeftPadding = new Vector2(8), BottomRightPadding = new Vector2(8), NineSlice = "base" } };

            UI.Body.Elements.Add(new Container() { Style = new Style() { Size = new Vector2(24), Color = Color.Red, NineSlice = "base" } });
            UI.Body.Elements.Add(new Container() { Style = new Style() { Size = new Vector2(24), Color = Color.Red, NineSlice = "base" } });
            UI.Body.Elements.Add(new Container() { Style = new Style() { Size = new Vector2(32), Color = Color.Blue, NineSlice = "base" } });
            UI.Body.Elements.Add(new Container() { Style = new Style() { Size = new Vector2(16), Color = Color.Green, NineSlice = "base" } });
            UI.Body.Elements.Add(new Container() { Style = new Style() { Size = new Vector2(24), Color = Color.Red, NineSlice = "base" } });
            UI.Body.Elements.Add(new Container() { Style = new Style() { Size = new Vector2(24), Color = Color.Red, NineSlice = "base" } });
            UI.Body.Elements.Add(new Container() { Style = new Style() { Size = new Vector2(24, 36), Color = Color.Black, NineSlice = "base" } });
            UI.Body.Elements.Add(new Container() { Style = new Style() { Size = new Vector2(24), Color = Color.Red, NineSlice = "base" } });
            UI.Body.Elements.Add(new Container() { Style = new Style() { Size = new Vector2(24), Color = Color.Red, NineSlice = "base" } });
            UI.Body.Elements.Add(new Container() { Style = new Style() { Size = new Vector2(24), Color = Color.Red, NineSlice = "base" } });
            UI.Body.Elements.Add(new Container() { Style = new Style() { Size = new Vector2(24), Color = Color.Red, NineSlice = "base" } });
            UI.Body.Elements.Add(new Image() { Sprite = TextureAtlas.Sprites["MainAtlas/collision_tiles"], Style = new Style() { Color = Color.Yellow } });
            UI.Body.Elements.Add(new Label() { Text = "MEU TEXTO", Style = new Style() { Font = Font, FontScale = .5f, TextColor = Color.Black } });
            UI.Recalculate();
        }
        
        protected override void Step() => updateables.ForEach((u) => u.Step());

        protected override void Render() => drawables.ForEach((d) => d.Render());
    }
}