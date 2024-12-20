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
    internal class Box : Sprite
    {
        public Box(Dictionary<string, AnimationManager> animation, Vector2 position, int height, int width) : base(animation, position, height, width)
        {
            Texture = animation["box"].Texture;
            Velocity = new();
        }

        public override void Update(KeyboardState keystate, KeyboardState prevKeyState, GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Velocity.Y += 30.0f * dt;
            Velocity.Y = Math.Min(30.0f, Velocity.Y);
            //Debug.WriteLine(Velocity.Y);

           

            //Velocity.X = Math.Max(-300, Math.Min(300, Velocity.X));
            //Velocity.X *= 0.91f;

           

        }

     

       
    }
}
