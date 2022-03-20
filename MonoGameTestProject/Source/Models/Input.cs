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
    }
}
