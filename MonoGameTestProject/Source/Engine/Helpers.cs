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

        public static Vector2 RotateAboutOrigin(Vector2 point, Vector2 origin, float rotation)
        {
            return Vector2.Transform(point - origin, Matrix.CreateRotationZ(rotation)) + origin;
        }


        private static Texture2D rect;

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
