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
    /// AnimationManager Constructor - always keeps track of 1 animation at a time.
    /// </summary>
    public class AnimationManager
    {
        #region Fields and Properties

        // Timer to know when to increment the currentFrame of the animation
        private float _timer;

        // Source Rectangle to locate animation frame.
        private Rectangle _sourceRectangle;

        // The Animation model to manage
        public Animation Animation;

        // Position of where to draw texture
        public Vector2 Position { get; set; }

        #endregion

        #region Methods

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
            // if we are already in the current animation, then just break out of the method to not reinstantiate the values
            if (Animation == animation)
                return;

            Animation = animation;
            Animation.CurrentFrame = 0;
            _timer = 0f;
        }

        /// <summary>
        /// Stop animation method.
        /// </summary>
        public void Stop()
        {
            _timer = 0f;
            Animation.CurrentFrame = 0;
        }

        /// <summary>
        /// Update method.
        /// The actual 'animate' method where animation frames are incremented.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // animate by incrementing animation frames
            if (_timer > Animation.FrameSpeed)
            {
                _timer = 0f;
                Animation.CurrentFrame++;

                // restart animation if current frame is the last frame.
                if (Animation.IsLooping && Animation.CurrentFrame >= Animation.FrameCount)
                    Animation.CurrentFrame = 0;
            }

            // update sourceRectangle
            if (Animation.IsSpriteSheetDirectionVertical)
                _sourceRectangle = new Rectangle(
                    Animation.FrameLocation,
                    Animation.CurrentFrame * Animation.FrameHeight,
                    Animation.FrameWidth,
                    Animation.FrameHeight);
            else
                _sourceRectangle = new Rectangle(
                    Animation.CurrentFrame * Animation.FrameWidth,
                    0,
                    Animation.FrameWidth,
                    Animation.FrameHeight);
        }

        /// <summary>
        /// Draw method.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture: Animation.Texture,
                position: Position,
                sourceRectangle: _sourceRectangle,
                color: Color.White);
        }

        #endregion



    }
}
