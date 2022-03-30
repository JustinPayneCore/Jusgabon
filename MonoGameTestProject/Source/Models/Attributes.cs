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
using TiledSharp;
#endregion

namespace Jusgabon
{
    /// <summary>
    /// Attribute class to help track Player attributes.
    /// These attributes are modifiable by using the static Attributes +/- operator.
    /// These can be modifiable by items, enemy attacks, power-ups, etc...
    /// Source (tutorial): https://www.youtube.com/watch?v=vzDK15gmEXs
    /// </summary>
    public class Attributes
    {
        #region Attribute Property Members

        // Speed of sprite
        public float Speed { get; set; }

        // HP of sprite
        public int Health { get; set; }

        // Mana of sprite; cannot cast magic without mana
        public int Mana { get; set; }

        // Stamina of sprite; cannot dash without stamina
        public int Stamina { get; set; }

        // Attack power of sprite
        public int Attack { get; set; }

        // Magic power of sprite; how much power a magic spell does
        public int Magic { get; set; }

        #endregion Attribute Property Members

        #region Operations

        /// <summary>
        /// + operation to add more attributes to this Attribute
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Attributes operator +(Attributes a, Attributes b)
        {
            return new Attributes()
            {
                // for every Attribute Property in the class members, add here a + b
                Speed = a.Speed + b.Speed,
                Health = a.Health + b.Health,
                Mana = a.Mana + b.Mana,
                Stamina = a.Stamina + b.Stamina,
                Attack = a.Attack + b.Attack,
                Magic = a.Magic + b.Magic,
            };
        }

        /// <summary>
        /// - operation to subtract more attributes from this Attribute
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Attributes operator -(Attributes a, Attributes b)
        {
            return new Attributes()
            {
                // for every Attribute Property in the class members, add here with a - b
                Speed = a.Speed - b.Speed,
                Health = a.Health - b.Health,
                Mana = a.Mana - b.Mana,
                Stamina = a.Stamina - b.Stamina,
                Attack = a.Attack - b.Attack,
                Magic = a.Magic - b.Magic,
            };
        }

        #endregion Operations



    }
}
