using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Component interface for custom draw and update methods.
/// Author: Gabriel Baluyut
/// Version: 1.0
/// Date: April 5, 2022
/// Source: https://github.com/Oyyou/MonoGame_Tutorials/blob/master/MonoGame_Tutorials/Tutorial012/Component.cs
/// </summary>
namespace Jusgabon
{
    /// <summary>
    /// Class containing abstract methods.
    /// </summary>
    public abstract class ComponentMonogame
    {

        /// <summary>
        /// Abstract method for drawing button.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        /// <summary>
        /// Abstract method for updating button display.
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void Update(GameTime gameTime);
    }
}
