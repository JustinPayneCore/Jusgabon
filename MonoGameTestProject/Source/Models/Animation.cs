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
    public class Animation
    {
        // the current frame to view
        public int CurrentFrame { get; set; }

        // how many frames there are in the texture
        public int FrameCount { get; private set; }

        // how tall is each frame
        public int FrameHeight { get { return Texture.Height / FrameCount; } }

        // which column to take the animation frames from
        public int FrameCol { get; set; } = 0;
        
        // how fast to animate through the frames
        public float FrameSpeed { get; set; }

        // how wide the texture is
        public int FrameWidth { get { return Texture.Width / 4; } }

        // is the aninmation something that should be looping i.e. walking
        // vs one where it doesn't loop and stops at the last frame i.e. crouching
        public bool IsLooping { get; set; }

        // the texture to animate
        public Texture2D Texture { get; private set; }

        /// <summary>
        /// Constructor for Animation model.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="frameCount"></param>
        public Animation(Texture2D texture, int frameCount)
        {
            Texture = texture;

            FrameCount = frameCount;

            IsLooping = true;

            FrameSpeed = 0.15f;
        }
    }
}
