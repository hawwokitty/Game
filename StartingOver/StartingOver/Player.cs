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

        public Player(Texture2D texture, Vector2 position, int height, int width) : base(texture, position, height, width)
        {
            Velocity = new();
        }

        public override void Update(KeyboardState keystate)
        {
            Velocity = Vector2.Zero;
            Velocity.Y = 5.0f;

            if (keystate.IsKeyDown(Keys.Right))
            {
                Velocity.X = 5;
            }
            if (keystate.IsKeyDown(Keys.Left))
            {
                Velocity.X = -5;
            }
            if (keystate.IsKeyDown(Keys.Up))
            {
                Velocity.Y = -5;
            }
            if (keystate.IsKeyDown(Keys.Down))
            {
                Velocity.Y = 5;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, AnimationManager am)
        {
            base.Draw(spriteBatch, am);
        }
    }
}
