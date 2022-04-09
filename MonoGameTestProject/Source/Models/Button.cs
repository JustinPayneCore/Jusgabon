#region Includes
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TiledSharp;
#endregion

namespace Jusgabon
{
    /// <summary>
    /// Button class for Menu UI.
    /// Sources (referenced from this tutorial): https://www.youtube.com/watch?v=76Mz7ClJLoE
    /// </summary>
    public class Button : Component
    {
        #region Members

        // mouse state to determine if there's any clicking
        private MouseState _currentMouse;

        // font to use for buttons
        private SpriteFont _font;

        // check if button is being hovered over
        private bool _isHovering;

        // previous mouse state
        private MouseState _previousMouse;

        // button texture
        private Texture2D _texture;

        // click event handler
        public event EventHandler Click;

        // check if button was clicked
        public bool Clicked { get; private set; }

        // colour of text
        public Color PenColour { get; set; }

        // position of button
        public Vector2 Position { get; set; }

        // rectangle of button
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            }
        }

        // text inside button
        public string Text { get; set; }

        #endregion Members

        #region Methods

        /// <summary>
        /// Button constructor - creates a button with given texture and font, default font color is black.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="font"></param>
        public Button(Texture2D texture, SpriteFont font)
        {
            _texture = texture;

            _font = font;

            PenColour = Color.Black;
        }

        /// <summary>
        /// Draw method for a button.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var colour = Color.White;

            if (_isHovering)
                colour = Color.Gray;

            spriteBatch.Draw(_texture, Rectangle, colour);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

                spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColour);
            }
        }

        /// <summary>
        /// Update method for a button.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="sprites"></param>
        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            _isHovering = false;

            if (mouseRectangle.Intersects(Rectangle))
            {
                _isHovering = true;

                if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());
                }
            }
        }

        #endregion
    }
}
