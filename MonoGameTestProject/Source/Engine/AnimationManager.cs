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
    /// Constructor - always keeps track of 1 animation at a time.
    /// </summary>
    public class AnimationManager
    {
        
        //private Animation _animation;

        // timer to know when to increment the currentFrame of the animation
        private float _timer;

        public Animation Animation;

        public Vector2 Position { get; set; }

        /// <summary>
        /// AnimationManager Constructor.
        /// By default, the constructor should be passed the 'idle' animation.
        /// </summary>
        /// <param name="animation"></param>
        public AnimationManager(Animation animation)
        {
            Animation = animation;
        }

        /// <summary>
        /// Play animation method.
        /// Set the animation and instantiate the animation values.
        /// </summary>
        /// <param name="animation"></param>
        public void Play(Animation animation)
        {
            // if we already in the current animation, then just break out of the method to not reinstantiate the values
            if (Animation == animation)
                return;

            Animation = animation;
            Animation.CurrentFrame = 0;
            _timer = 0f;
        }

        public void Stop()
        {
            _timer = 0f;
            Animation.CurrentFrame = 0;
        }

        public void Update(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer > Animation.FrameSpeed)
            {
                _timer = 0f;
                Animation.CurrentFrame++;

                if (Animation.CurrentFrame >= Animation.FrameCount)
                    Animation.CurrentFrame = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Animation.Texture,
                Position,
                new Rectangle(
                    Animation.FrameCol,
                    Animation.CurrentFrame * Animation.FrameHeight,
                    Animation.FrameWidth,
                    Animation.FrameHeight),
                Color.White);
        }




    }
}
