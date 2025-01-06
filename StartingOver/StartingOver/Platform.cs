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
    internal class Platform : Sprite
    {
        public Platform(Dictionary<string, AnimationManager> animation, Vector2 position, int height, int width) : base(animation, position, height, width)
        {
            Texture = animation["platform"].Texture;
            Velocity = new();
        }

        public void Update(KeyboardState keystate, KeyboardState prevKeyState, GameTime gameTime, int movePlatform)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (movePlatform == 1)
            {
                Velocity.X = -160.0f * dt;
                Velocity.Y = -160.0f * dt;
            }
            else if (movePlatform == 2)
            {
                Velocity.X = 160.0f * dt;
                if (Rect.X <= 170 * 3)
                {
                    ApplyVelocityX((int)Velocity.X);

                }
            }
            else if (movePlatform == 3)
            {
                Debug.WriteLine("off button " + Rect.Y);
                Velocity.Y = 160.0f * dt;
                if (Rect.Y <= 100 * 3)
                {
                    ApplyVelocityY((int)Velocity.Y);

                }
            }


        }
    }
}
