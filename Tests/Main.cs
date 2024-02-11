using System;
using Claw;
using Claw.Graphics;
using Claw.Graphics.UI;
using Claw.Input;
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
            UI.Body = new Container() { Style = new Style() { Size = new Vector2(128), Gap = new Vector2(8), TopLeftPadding = new Vector2(8), BottomRightPadding = new Vector2(8), NineSlice = "base" } };
            UI.Body.Scrollable = true;
            UI.Cursor = new UICursor();

            Container other = new Container() { Scrollable = true, Style = new Style() { NineSlice = "base", Color = Color.Yellow, Display = Display.Block, TopLeftPadding = new Vector2(4), BottomRightPadding = new Vector2(4), Gap = new Vector2(4), Size = new Vector2(128 - 16, 64) } };
            other.Elements.Add(new Container() { Style = new Style() { NineSlice = "base", Size = new Vector2(24), Color = Color.Magenta, Display = Display.Inline } });
            other.Elements.Add(new Container() { Style = new Style() { NineSlice = "base", Size = new Vector2(24), Color = Color.Magenta, Display = Display.Inline } });
            other.Elements.Add(new Container() { Style = new Style() { NineSlice = "base", Size = new Vector2(24), Color = Color.Blue, Display = Display.Inline } });
            other.Elements.Add(new Container() { Style = new Style() { NineSlice = "base", Size = new Vector2(24), Color = Color.Magenta, Display = Display.Inline } });
            other.Elements.Add(new Container() { Style = new Style() { NineSlice = "base", Size = new Vector2(24), Color = Color.Magenta, Display = Display.Inline } });
            other.Elements.Add(new Container() { Style = new Style() { NineSlice = "base", Size = new Vector2(24), Color = Color.Blue, Display = Display.Inline } });
            other.Elements.Add(new Container() { Style = new Style() { NineSlice = "base", Size = new Vector2(24), Color = Color.Magenta, Display = Display.Inline } });
            other.Elements.Add(new Container() { Style = new Style() { NineSlice = "base", Size = new Vector2(24), Color = Color.Magenta, Display = Display.Inline } });
            other.Elements.Add(new Container() { Style = new Style() { NineSlice = "base", Size = new Vector2(24), Color = Color.Blue, Display = Display.Inline } });
            other.Elements.Add(new Container() { Style = new Style() { NineSlice = "base", Size = new Vector2(24), Color = Color.Magenta, Display = Display.Inline } });
            other.Elements.Add(new Container() { Style = new Style() { NineSlice = "base", Size = new Vector2(24), Color = Color.Magenta, Display = Display.Inline } });
            other.Elements.Add(new Container() { Style = new Style() { NineSlice = "base", Size = new Vector2(24), Color = Color.Blue, Display = Display.Inline } });
            UI.Body.Elements.Add(new Slider() { Style = new Style() { Color = Color.Black }, Sync = (value) => other.ScrollOffset = new Vector2(other.ScrollMaxOffset.X * value, 0) });
            UI.Body.Elements.Add(other);
            UI.Recalculate();
        }
        
        protected override void Step()
        {
            UI.Cursor.Position = Input.MousePosition;

            updateables.ForEach((u) => u.Step());

            if (Input.KeyPressed(Keys.Right)) ((Slider)UI.Body.Elements[0]).Value += .1f;
            else if (Input.KeyPressed(Keys.Left)) ((Slider)UI.Body.Elements[0]).Value -= .1f;

            if (Input.MouseButtonDown(MouseButtons.Left) && UI.Cursor.Hover is Slider slider) slider.SetValue(UI.Cursor.HoverDifference);
        }

        protected override void Render() => drawables.ForEach((d) => d.Render());
    }
}