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
    /// AnimationManager Constructor - always keeps track of 1 animation frame at a time.
    /// The Animation Frame is a splice of a Texture from a SpriteSheet.
    /// Source (tutorial): https://www.youtube.com/watch?v=OLsiWxgONeM
    /// </summary>
    public class AnimationManager
    {
        #region Members

        // Timer for incrementing Animation's CurrentFrame
        private float _timer;

        // The Animation model & Frame to manage
        public Animation Animation;

        // Colour of Animation Frame
        public Color Colour { get; set; }
        
        // The default origin (centre) of the Animation Frame
        public Vector2 Origin { get; set; }

        // Game-Position of where to draw Animation
        public Vector2 Position { get; set; }

        // Rotation of Animation Frame
        public float Rotation { get; set; }

        // Source Rectangle to locate Animation Frame.
        public Rectangle SourceRectangle
        {
            get
            {
                if (Animation.IsSpriteSheetDirectionVertical)
                    return new Rectangle(
                        Animation.FrameLocation,
                        Animation.CurrentFrame * Animation.FrameHeight,
                        Animation.FrameWidth,
                        Animation.FrameHeight);
                else // SpriteSheet is in a horizontal orientation (w/ 1 row of animation frames)
                    return new Rectangle(
                        Animation.CurrentFrame * Animation.FrameWidth,
                        0,
                        Animation.FrameWidth,
                        Animation.FrameHeight);
            }
        }

        #endregion Members

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
        public void Update(GameTime gameTime, List<Sprite> sprites)
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
        }

        /// <summary>
        /// Draw method.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture: Animation.Texture,
                position: Position,
                sourceRectangle: SourceRectangle,
                color: Colour,
                rotation: Rotation,
                origin: Origin,
                scale: 1,
                effects: SpriteEffects.None,
                layerDepth: 0);
        }

        #endregion Methods



    }
}
