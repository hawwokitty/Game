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
    internal class Sprite
    {
        public Texture2D Texture;
        public Vector2 Position;
        public int Height;
        public int Width;
        public Rectangle Rect;
        public Rectangle ColliderRect;
        public Vector2 Velocity;
        public bool Grounded { get; set; }

        public Dictionary<string, AnimationManager> Animation;

        public Sprite(Dictionary<string, AnimationManager> animation, Vector2 position, int height, int width)
        {
            Animation = animation;
            this.Position = position;
            Height = height;
            Width = width;
            Rect = new((int)Position.X, (int)Position.Y, Width, Height);
            ColliderRect = Rect;
        }

        public virtual void ApplyVelocityX(int value)
        {
            Rect.X += value;
            ColliderRect = Rect;
        }
        public virtual void ApplyVelocityY(int value)
        {
            Rect.Y += value;
            ColliderRect = Rect;
        }

        public virtual void Update(KeyboardState keystate, KeyboardState prevKeyState, GameTime gameTime)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch, AnimationManager am)
        {
            spriteBatch.Draw(
                Texture,
                Rect,
                am.GetFrame(),
                Color.White
            );
        }
    }
}
