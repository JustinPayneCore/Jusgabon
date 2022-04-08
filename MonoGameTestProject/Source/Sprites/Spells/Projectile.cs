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
    /// Projectile Spell - uses the default implementation of Spell.
    /// </summary>
    public class Projectile : Spell
    {

        /// <summary>
        /// Projectile class constructor.
        /// </summary>
        /// <param name="animations"></param>
        public Projectile(Dictionary<string, Animation> animations) : base(animations) { }

    }
}
