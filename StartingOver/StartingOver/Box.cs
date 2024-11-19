using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace StartingOver
{
    internal class Box : Sprite
    {
        public bool isGrabbed { get; set; }
        public Box(Dictionary<string, AnimationManager> animation, Vector2 position, int height, int width) : base(animation, position, height, width)
        {
            Texture = animation["box"].Texture;
            isGrabbed = false;
        }
    }
}
