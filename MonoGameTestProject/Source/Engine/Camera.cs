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
    /// <summary>
    /// Camera class engine.
    /// Camera is used to move the game screen around; depends on player position.
    /// </summary>
    public class Camera
    {
        // Zoom level of game screen
        private float _zoom = 3f;
                
        // Transform matrix to determine position of Camera
        public Matrix Transform { get; private set; }

        /// <summary>
        /// Follow method to make camera follow target sprite.
        /// Target sprite should be player.
        /// </summary>
        /// <param name="target"></param>
        public void Follow(Sprite target)
        {
            var position = Matrix.CreateTranslation(
                -target.Position.X - (target.Rectangle.Width / 2),
                -target.Position.Y - (target.Rectangle.Height / 2),
                0);

            var offset = Matrix.CreateTranslation(
                    Game1.screenWidth / 2,
                    Game1.screenHeight / 2,
                    0);

            var zoom = Matrix.CreateScale(new Vector3(_zoom, _zoom, 1));

            Transform = position * zoom * offset;
        }


    }
}
