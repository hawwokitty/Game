using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace StartingOver
{
    internal class Rope : Sprite
    {
        public Rope(Dictionary<string, AnimationManager> animation, Vector2 position, int height, int width) : base(animation, position, height, width)
        {
            Texture = animation["rope"].Texture;
            Velocity = new();
        }

        public void Update(KeyboardState keystate, KeyboardState prevKeyState, GameTime gameTime, int moveRope)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (moveRope == 1)
            {
                Velocity.X = -160.0f * dt;
            } else if (moveRope == 2)
            {
                Velocity.X = 160.0f * dt;
            }


        }
    }
}
