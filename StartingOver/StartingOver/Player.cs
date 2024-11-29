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
        
        public PlayerState State { get; private set; } = PlayerState.IdleRight;

        private bool isJumping;
        private float jumpTime;
        private const float MaxJumpTime = 0.3f; // Maximum time the jump can be sustained
        private const float InitialJumpVelocity = -440.0f;
        private const float HoldJumpForce = -30.0f;

        private float coyoteTime = 0.15f;
        private float coyoteTimeCounter;

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

            // Jump logic
            if (Grounded)
            {
                coyoteTimeCounter = coyoteTime;
            }
            else
            {
                coyoteTimeCounter -= dt;
            }

            if (coyoteTimeCounter > 0f && keystate.IsKeyDown(Keys.Space) && !prevKeyState.IsKeyDown(Keys.Space))
            {
                isJumping = true;
                jumpTime = 0f;
                Velocity.Y = InitialJumpVelocity * dt;
                coyoteTimeCounter = 0;
            }

            if (isJumping)
            {
                jumpTime += dt;

                if (keystate.IsKeyDown(Keys.Space) && jumpTime < MaxJumpTime)
                {
                    Velocity.Y += HoldJumpForce * dt;
                }
                else
                {
                    isJumping = false;
                }
            }

            if (HeldBox != null)
            {
                //Debug.WriteLine("box should move");
                //do thingies here
                HeldBox.Velocity.X = Velocity.X;
                //HeldBox.Rect.X += (int)Velocity.X;
                int overlapY = Math.Min(Rect.Bottom - HeldBox.Rect.Top, HeldBox.Rect.Bottom - Rect.Top);

                if (overlapY > 96 || overlapY < 32)
                {
                    DetachBox();
                }
            }

            if (HeldKey != null)
            {
                HeldKey.Rect.X = Rect.X;
                HeldKey.Rect.Y = Rect.Y;

                //Debug.WriteLine("key should move");
            }

            
        }

        public void AttachBox(Box box)
        {
            HeldBox = box;
            //boxOffset = box.Position;
            //Debug.WriteLine("attach box");
            //record it's inital offset
        }

        public void DetachBox()
        {
            if (HeldBox != null)
            {
                HeldBox.Velocity.X = 0;
            }

            HeldBox = null;
            //Debug.WriteLine("detached box");
        }

        public void AttachKey(Key key)
        {
            HeldKey = key;
        }
        public void DetachKey()
        {
            HeldKey = null;
            //Debug.WriteLine("key should byebye");
        }

        public Box HeldBox;
        public Key HeldKey;

        public override void Draw(SpriteBatch spriteBatch, AnimationManager am)
        {
            base.Draw(spriteBatch, am);
        }
    }
}
