using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StartingOver
{
    internal class Button : Sprite
    {
        public AnimationManager ButtonAm;
        public Button(Dictionary<string, AnimationManager> animation, Vector2 position, int height, int width) : base(animation, position, height, width)
        {
            Texture = animation["button-up"].Texture;
            Velocity = new();
        }

        public Button(Dictionary<string, AnimationManager> animation, Vector2 position, int height, int width, AnimationManager buttonAm) : base(animation, position, height, width)
        {
            Texture = animation["button-up"].Texture;
            Velocity = new();
            ButtonAm = buttonAm;
        }
    }
}
