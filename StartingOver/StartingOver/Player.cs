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

    public enum PlayerState
    {
        WalkUp, WalkDown, WalkLeft, WalkRight,
        IdleUp, IdleDown, IdleLeft, IdleRight
    }
    internal class Player : Sprite
    {
        
        public Vector2 Velocity;
        public bool Grounded { get; set; }
        public PlayerState State { get; private set; } = PlayerState.IdleRight;

        public Player(Dictionary<string, AnimationManager> _animation, Vector2 position, int height, int width) : base(_animation, position, height, width)
        {
            Velocity = new();
            Texture = _animation["IdleDown"].Texture;
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
                State = PlayerState.WalkRight;
                Texture = _animation["WalkRight"].Texture;
            }
            else if (keystate.IsKeyDown(Keys.Left))
            {
                Velocity.X += -30 * dt;
                State = PlayerState.WalkLeft;
                Texture = _animation["WalkLeft"].Texture;
            }
            else
            {
                // If no movement keys are pressed, set to idle based on last direction
                State = State switch
                {
                    PlayerState.WalkUp => PlayerState.IdleUp,
                    PlayerState.WalkDown => PlayerState.IdleDown,
                    PlayerState.WalkLeft => PlayerState.IdleLeft,
                    PlayerState.WalkRight => PlayerState.IdleRight,
                    _ => State // maintain the last idle direction
                };
                Texture = State switch
                {
                    PlayerState.IdleUp => _animation["IdleUp"].Texture,
                    PlayerState.IdleDown => _animation["IdleDown"].Texture,
                    PlayerState.IdleLeft => _animation["IdleLeft"].Texture,
                    PlayerState.IdleRight => _animation["IdleRight"].Texture,
                    _ => Texture
                };
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
