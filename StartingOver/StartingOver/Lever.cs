using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace StartingOver
{
    internal class Lever : Sprite
    {
        public AnimationManager leverAnimation;
        public Lever(Dictionary<string, AnimationManager> animation, Vector2 position, int height, int width) : base(animation, position, height, width)
        {
            Texture = animation["lever-left"].Texture;
            Velocity = new();
        }
        public Lever(Dictionary<string, AnimationManager> animation, Vector2 position, int height, int width, AnimationManager leverAm) : base(animation, position, height, width)
        {
            Texture = animation["lever-left"].Texture;
            Velocity = new();
            leverAnimation = leverAm;
        }

        //public override void Update(KeyboardState keystate, KeyboardState prevKeyState, GameTime gameTime)
        //{
        //}
    }
}
