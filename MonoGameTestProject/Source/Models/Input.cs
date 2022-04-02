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
    /// Input class for Keyboard input bindings.
    /// </summary>
    public class Input
    {
        public Keys Down { get; set; }

        public Keys Left { get; set; }

        public Keys Right { get; set; }

        public Keys Up { get; set; }

        public Keys Attack { get; set; }

        public Keys Jump { get; set; }

        public Keys Special1 { get; set; }

        public Keys Special2 { get; set; }

        public Keys Interact { get; set; }

        public Keys Item { get; set; }

        public Keys SwitchWeapon { get; set; } 
        
        public Keys SwitchItem { get; set; }
    }
}
