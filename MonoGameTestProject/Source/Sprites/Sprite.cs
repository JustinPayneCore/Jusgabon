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
#endregion

namespace Jusgabon
{
    public class Sprite : Component
    {
        protected Texture2D _texture;

        public Vector2 Position { get; set; }



        public Rectangle Rectangle 
        { 
            get { return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height); } 
        }

        public Sprite(Texture2D texture)
        {
            _texture = texture;
        }
        
        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Globals.spriteBatch.Draw(_texture, Position, Color.White);
        }
    }
}
