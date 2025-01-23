using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace StartingOver
{

    public enum PlayerState
    {
        WalkUp, WalkDown, WalkLeft, WalkRight,
        IdleUp, IdleDown, IdleLeft, IdleRight,
        DeadRight, DeadLeft,
        PushRight, PushLeft, PushRightIdle, PushLeftIdle
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

        private float jumpBufferTime = 0.2f;
        private float jumpBufferCounter;

        public bool Dead;

        public Player(Dictionary<string, AnimationManager> _animation, Vector2 position, int height, int width) : base(_animation, position, height, width)
        {
            Velocity = new();
            Texture = _animation["IdleDown"].Texture;
        }

        public override void Update(KeyboardState keystate, KeyboardState prevKeyState, GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Debug.WriteLine(dt);

            if (Dead)
            {
                // Set the death animation based on the last direction
                if (State == PlayerState.WalkRight || State == PlayerState.IdleRight)
                {
                    State = PlayerState.DeadRight;
                    Texture = Animation["DeathRight"].Texture;
                }
                else if (State == PlayerState.WalkLeft || State == PlayerState.IdleLeft)
                {
                    State = PlayerState.DeadLeft;
                    Texture = Animation["DeathLeft"].Texture;
                }

                // Exit early to maintain the Dead state
                return;
            }

            if (!overLadder)
            {
                // gravity
                Velocity.Y += 30.0f * dt;
                Velocity.Y = Math.Min(30.0f, Velocity.Y);
            }
            else
            {
                Grounded = true; // makes so u can jump from side of ladder if far enough out

                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    Velocity.Y = -200 * dt;
                    State = PlayerState.WalkUp;
                    Texture = Animation["WalkUp"].Texture;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    Velocity.Y = 200 * dt;
                    State = PlayerState.WalkUp;
                    Texture = Animation["WalkUp"].Texture;
                }
                else
                {
                    Velocity.Y = 0.0f;
                }
            }
            
            Velocity.X = Math.Max(-300, Math.Min(300, Velocity.X));
            Velocity.X *= 0.91f;

            if (keystate.IsKeyDown(Keys.Right))
            {
                Velocity.X += 30 * dt;
                State = PlayerState.WalkRight;
                Texture = Animation["WalkRight"].Texture;
            }
            else if (keystate.IsKeyDown(Keys.Left))
            {
                Velocity.X += -30 * dt;
                State = PlayerState.WalkLeft;
                Texture = Animation["WalkLeft"].Texture;
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
                    PlayerState.IdleUp => Animation["IdleUp"].Texture,
                    PlayerState.IdleDown => Animation["IdleDown"].Texture,
                    PlayerState.IdleLeft => Animation["IdleLeft"].Texture,
                    PlayerState.IdleRight => Animation["IdleRight"].Texture,
                    _ => Texture
                };
                if (HeldBox != null)
                {
                    Velocity.X = 0;
                }
            }





            // Jump logic
            if (Grounded)
            {
                coyoteTimeCounter = coyoteTime;
            }
            else
            {
                coyoteTimeCounter -= dt;
            }

            //if (keystate.IsKeyDown(Keys.Space))
            //{
            //    jumpBufferCounter = jumpBufferTime;
            //}
            //else
            //{
            //    jumpBufferCounter -= dt;
            //}

            if (coyoteTimeCounter > 0f /* && jumpBufferCounter > 0 */ && keystate.IsKeyDown(Keys.Space) && !prevKeyState.IsKeyDown(Keys.Space))
            {
                isJumping = true;
                jumpTime = 0f;
                Velocity.Y = InitialJumpVelocity * dt;
                coyoteTimeCounter = 0;
                //jumpBufferCounter = 0;
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
                int overlapX = Math.Min(HeldBox.Rect.Right - Rect.Left, Rect.Right - HeldBox.Rect.Left);


                // Check if the player is idle or moving while pushing the box
                if (Rect.Center.X < HeldBox.Rect.Center.X)
                {
                    // Player is to the left of the box
                    if (Velocity.X != 0)
                    {
                        // Player is moving, pushing right
                        State = PlayerState.PushRight;
                        Texture = Animation["PushRight"].Texture;
                    }
                    else
                    {
                        // Player is idle, still holding the box
                        State = PlayerState.PushRightIdle;
                        Texture = Animation["PushRightIdle"].Texture;
                    }
                }
                else
                {
                    // Player is to the right of the box
                    if (Velocity.X != 0)
                    {
                        // Player is moving, pushing left
                        State = PlayerState.PushLeft;
                        Texture = Animation["PushLeft"].Texture;
                    }
                    else
                    {
                        // Player is idle, still holding the box
                        State = PlayerState.PushLeftIdle;
                        Texture = Animation["PushLeftIdle"].Texture;
                    }
                }

                if (overlapY > 96 || overlapY < 32 || overlapX < 0)
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
            if (HeldRope != null && HeldRopeOnce == 1)
            {
                //Debug.WriteLine("player x is: " + Rect.X);
                //HeldRopePos.X = Rect.X;
                HeldRopePos.Y = Rect.Y;
                HeldRopeOnce++;
                //Debug.WriteLine(HeldRopeOnce);
                //Velocity.Y = 0.0f;

            }
            if (HeldRope != null)
            {
                //Debug.WriteLine("rope is held");
                Velocity.Y = 0.0f;
                Velocity.X = 0.0f;
                Rect.X = HeldRope.Rect.X - 20;

                if (keystate.IsKeyDown(Keys.Up))
                {
                    if (Rect.Y > HeldRope.Rect.Y)
                    {
                        Velocity.Y = -200 * dt;
                    }
                }
                if (keystate.IsKeyDown(Keys.Down))
                {
                    if (Rect.Y < HeldRope.Rect.Bottom - 64)
                    {
                        Velocity.Y = 200 * dt;
                    }
                }
            }



            overLadder = false;

        }

        public void AttachBox(Box box)
        {
            HeldBox = box;
        }

        public void DetachBox()
        {
            if (HeldBox != null)
            {
                HeldBox.Velocity.X = 0;
            }

            HeldBox = null;
        }

        public void AttachKey(Key key)
        {
            HeldKey = key;
        }
        public void DetachKey()
        {
            HeldKey = null;
        }
        public void DetachRope()
        {
            HeldRope = null;
            HeldRopeOnce = 0;
        }

        public void AttachRope(Rope rope)
        {
            HeldRope = rope;
            HeldRopeOnce = 1;
        }

        public void CollideWithLadder()
        {
            overLadder = true;
        }

        public override void ApplyVelocityX(int value)
        {
            Rect.X += value;
            ColliderRect.X = Rect.X + 8;
            ColliderRect.Width = Width - 16;
        }

        public override void ApplyVelocityY(int value)
        {
            Rect.Y += value;
            ColliderRect.Y = Rect.Y + 10;
            ColliderRect.Height = Height - 10;
        }

        public Box HeldBox;
        public Key HeldKey;
        public Rope HeldRope;
        public Vector2 HeldRopePos;
        private int HeldRopeOnce;
        private bool overLadder;

        public override void Draw(SpriteBatch spriteBatch, AnimationManager am)
        {
            base.Draw(spriteBatch, am);
        }

    }
}
