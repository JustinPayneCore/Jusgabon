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
        #region Fields

        protected AnimationManager _animationManager;

        protected Dictionary<string, Animation> _animations;

        protected Vector2 _position;

        protected Texture2D _texture;

        #endregion

        #region Properties

        public Input Input;

        public Vector2 Position 
        {
            get { return _position; }
            set
            {
                // for a static image
                _position = value;

                // for an animation
                if (_animationManager != null)
                    _animationManager.Position = _position;
            }
        }

        public float Speed = 1.5f;

        public Vector2 Velocity;

        public Rectangle Rectangle
        {
            get 
            {
                if (_texture != null)
                    return new Rectangle(
                        (int)Position.X,
                        (int)Position.Y,
                        _texture.Width,
                        _texture.Height);
                else // _animationManager != null
                    return new Rectangle(
                        (int)Position.X,
                        (int)Position.Y,
                        _animationManager.Animation.FrameWidth,
                        _animationManager.Animation.FrameHeight);
            }
        }

        #endregion


        #region Methods

        /// <summary>
        /// Sprite Constructor for a sprite with a dictionary(ex. spritesheet) of animations
        /// </summary>
        /// <param name="animations"></param>
        public Sprite(Dictionary<string, Animation> animations)
        {
            _animations = animations;
            _animationManager = new AnimationManager(_animations.First().Value);
        }
        
        /// <summary>
        /// Sprite Constructor for a sprite with a static texture i.e. an idle texture.
        /// </summary>
        /// <param name="texture"></param>
        public Sprite(Texture2D texture)
        {
            _texture = texture;
        }

        /// <summary>
        /// SetAnimations Method.
        /// If sprite has animations, then set logic on which animation to use here.
        /// </summary>
        protected virtual void SetAnimations()
        {
            if (Velocity.X > 0)
                _animationManager.Play(_animations["WalkRight"]);
            else if (Velocity.X < 0)
                _animationManager.Play(_animations["WalkLeft"]);
            else if (Velocity.Y > 0)
                _animationManager.Play(_animations["WalkDown"]);
            else if (Velocity.Y < 0)
                _animationManager.Play(_animations["WalkUp"]);
            else
                _animationManager.Stop();
        }

        /// <summary>
        /// Update method.
        /// Only constantly update if sprite has animations.
        /// If sprite only has a texture, then no update needed, or override this method in child class.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {

            if (_animationManager != null)
            {
                SetAnimations();

                _animationManager.Update(gameTime);
            }

        }

        /// <summary>
        /// Draw method.
        /// Draw texture (as a static image) or draw animation (moving sprite).
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_texture != null)
                spriteBatch.Draw(
                    texture:_texture, 
                    position: Position, 
                    color: Color.White);
            else if (_animationManager != null)
                _animationManager.Draw(spriteBatch);
            else throw new Exception("Error: No texture/animations found for Sprite.");
        }

        #endregion
    }
}
