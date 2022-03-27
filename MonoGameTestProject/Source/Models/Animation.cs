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
    /// Animation class to depict an animation model.
    /// Each animation model depict what each frame's properties are.
    /// Source (tutorial): https://www.youtube.com/watch?v=OLsiWxgONeM
    /// </summary>
    public class Animation
    {
        // the current frame to view
        public int CurrentFrame { get; set; }

        // where to take the animation frame from (x-axis in pixels)
        // note: this is only relevant if animation direction of spritesheet goes downwards
        public int FrameLocation { get; set; } = 0;

        // how many frames there are in the texture
        public int FrameCount { get; private set; }

        // how tall is each frame
        //public int FrameHeight { get { return Texture.Height / FrameCount; } }
        public int FrameHeight { get; set; }
        
        // how fast to animate through the frames
        public float FrameSpeed { get; set; }

        // how wide the texture is
        // public int FrameWidth { get { return Texture.Width} }
        public int FrameWidth { get; set; }

        // is the animation direction of spritesheet going downwards (top to bot)
        // If false, the direction of animations reads left to right.
        public bool IsSpriteSheetDirectionVertical { get; set; } = true;

        // is the animation something that should be looping i.e. walking
        // vs one where it doesn't loop and stops at the last frame i.e. crouching
        public bool IsLooping { get; set; }

        // the texture to animate
        public Texture2D Texture { get; private set; }


        /// <summary>
        /// Constructor for Animation model.
        /// Animation for a vertical spritesheet.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="frameCount"></param>
        /// <param name="spritesheetColumns">spritesheet usually has 4 columns</param>
        public Animation(Texture2D texture, int frameCount, int spritesheetColumns, int frameLocation)
        {
            Texture = texture;

            FrameCount = frameCount;

            IsLooping = true;

            FrameSpeed = 0.15f;

            FrameHeight = Texture.Height / FrameCount;
            FrameWidth = Texture.Width / spritesheetColumns; // spritesheet usually has 4 columns

            FrameLocation = frameLocation;
        }

        /// <summary>
        /// Constructor for Animation model.
        /// Animation for a horizontal spritesheet.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="frameCount"></param>
        public Animation(Texture2D texture, int frameCount)
        {
            Texture = texture;

            FrameCount = frameCount;

            IsLooping = true;

            FrameSpeed = 0.15f;

            IsSpriteSheetDirectionVertical = false;

            if (!IsSpriteSheetDirectionVertical)
            {
                // spritesheet is a single row of animations
                FrameWidth = Texture.Width / FrameCount;
                FrameHeight = Texture.Height;
            }
            else
            {
                // spritesheet has 4 columns (usually)
                FrameHeight = Texture.Height / FrameCount;
                FrameWidth = Texture.Width / 4;
            }
        }
    }
}
