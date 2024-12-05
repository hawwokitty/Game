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

        //public override void Update(KeyboardState keystate, KeyboardState prevKeyState, GameTime gameTime)
        //{
        //    float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        //    //Velocity.X = -300.0f * dt;
        //    //Debug.WriteLine("velocity x is " + Velocity.X);

        //}
    }
}
