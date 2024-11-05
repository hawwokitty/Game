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
    internal class Player : Sprite
    {
        
        public Vector2 Velocity;
        public bool Grounded { get; set; }

        public Player(Texture2D texture, Vector2 position, int height, int width) : base(texture, position, height, width)
        {
            Velocity = new();
        }

        public override void Update(KeyboardState keystate, KeyboardState prevKeyState)
        {
            Velocity.X = 0;
            Velocity.Y += 0.5f;
            Velocity.Y = Math.Min(10.0f, Velocity.Y);

            if (keystate.IsKeyDown(Keys.Right))
            {
                Velocity.X = 5;
            }
            if (keystate.IsKeyDown(Keys.Left))
            {
                Velocity.X = -5;
            }
            //if (keystate.IsKeyDown(Keys.Up))
            //{
            //    Velocity.Y = -5;
            //}
            //if (keystate.IsKeyDown(Keys.Down))
            //{
            //    Velocity.Y = 5;
            //}

            //jumping
            if (Grounded && keystate.IsKeyDown(Keys.Space) && !prevKeyState.IsKeyDown(Keys.Space))
            {
                Velocity.Y = -10;
            }
            
        }

        public override void Draw(SpriteBatch spriteBatch, AnimationManager am)
        {
            base.Draw(spriteBatch, am);
        }
    }
}
