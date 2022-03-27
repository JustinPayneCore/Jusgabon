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
    /// Boss Class - Inherits Enemy Class.
    /// Inheritance Order: Boss -> Enemy -> NPC -> Sprite -> Component
    /// </summary>
    public class Boss : Enemy
    {
        #region Members


        #endregion Members


        #region Methods

        /// <summary>
        /// Boss Constructor.
        /// </summary>
        /// <param name="animations"></param>
        public Boss(Dictionary<string, Animation> animations) : base(animations)
        {
            Speed = 0.5f; // slower than default enemy
            AggroModifier = 2f; // default enemy modifier = 1.5f; increase if boss should be "stickier" to player
        }

        #endregion

    }
}
