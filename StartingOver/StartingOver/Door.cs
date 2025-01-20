using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace StartingOver
{
    internal class Door : Sprite
    {
        public AnimationManager DoorAm;
        public Door(Dictionary<string, AnimationManager> animation, Vector2 position, int height, int width) : base(animation, position, height, width)
        {
            Texture = animation["door1"].Texture;
            Velocity = new();
        }

        public Door(Dictionary<string, AnimationManager> animation, Vector2 position, int height, int width, AnimationManager doorAm) : base(animation, position, height, width)
        {
            Texture = animation["door-closed"].Texture;
            Velocity = new();
            DoorAm = doorAm;
        }
    }
}
