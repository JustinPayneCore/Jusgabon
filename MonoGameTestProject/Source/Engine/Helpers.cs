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
    /// Static Helper class for Attributes.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Sum helper method.
        /// Takes a list of Attributes and returns an Attribute with the sum of all its values.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static Attributes Sum(this IEnumerable<Attributes> attributes)
        {
            var finalAttributes = new Attributes();

            foreach (var attribute in attributes)
                finalAttributes += attribute;

            return finalAttributes;
        }

        /// <summary>
        /// RotateAboutOrigin helper method.
        /// Returns Rotated Vector from a Vector that's been rotated on the passed origin value.
        /// Source: https://stackoverflow.com/questions/8148651/rotation-of-an-object-around-a-central-vector2-point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="origin"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Vector2 RotateAboutOrigin(Vector2 point, Vector2 origin, float rotation)
        {
            return Vector2.Transform(point - origin, Matrix.CreateRotationZ(rotation)) + origin;
        }

        // Texture2D var for DrawRectangle helper method
        private static Texture2D rect;

        /// <summary>
        /// DrawRectangle helper method.
        /// Draws a simple rectangle of the passed Rectangle coords and Color.
        /// Note: Mostly used for testing/debugging.
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="color"></param>
        public static void DrawRectangle(Rectangle coords, Color color)
        {
            if (rect == null)
            {
                rect = new Texture2D(Globals.spriteBatch.GraphicsDevice, 1, 1);
                rect.SetData(new[] { Color.White });
            }
            Globals.spriteBatch.Draw(rect, coords, color);
        }

    }
}
