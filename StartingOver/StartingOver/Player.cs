using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public override void Update(KeyboardState keystate, KeyboardState prevKeyState, GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Debug.WriteLine(dt);

            Velocity.Y += 30.0f * dt;
            Velocity.Y = Math.Min(30.0f, Velocity.Y);

            if (keystate.IsKeyDown(Keys.Right))
            {
                Velocity.X += 30 * dt;
            }
            if (keystate.IsKeyDown(Keys.Left))
            {
                Velocity.X += -30 * dt;
            }

            Velocity.X = Math.Max(-300, Math.Min(300, Velocity.X));
            Velocity.X *= 0.91f;

            //jumping
            if (Grounded && keystate.IsKeyDown(Keys.Space) && !prevKeyState.IsKeyDown(Keys.Space))
            {
                Velocity.Y = -650 * dt;
            }
            
        }

        public override void Draw(SpriteBatch spriteBatch, AnimationManager am)
        {
            base.Draw(spriteBatch, am);
        }
    }
}
