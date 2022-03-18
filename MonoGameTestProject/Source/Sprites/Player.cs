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
    public class Player : Sprite
    {
        public Player(Texture2D texture) : base(texture)
        {

        }

        public override void Update(GameTime gameTime)
        {
            var velocity = new Vector2();

            var speed = 3f;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                velocity.Y = -speed;
            else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                velocity.Y = speed;

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                velocity.X = -speed;
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
                velocity.X = speed;

            Position += velocity;
        }
    }
}
