using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace StartingOver
{
    internal class GameScene : IScene
    {
        private ContentManager contentManager;
        private SceneManager sceneManager;
        private Texture2D texture;
        private GraphicsDevice graphicsDevice;
        private GraphicsDeviceManager graphics;

        private Player player;
        private Box box;
        private Key key;
        private Door door;
        //private List<Rope> rope;
        private Rope rope;
        private Lever lever1;
        private Lever lever2;
        private AnimationManager boxAm;
        private AnimationManager keyAm;
        private AnimationManager doorAm;
        private AnimationManager am;
        private AnimationManager ropeAm;
        private AnimationManager leverAm1;
        private AnimationManager leverAm2;

        private bool isJumping;

        private int moveRope;

        private bool boxIsCollide;
        private bool doorAndKeyIsCollide;

        private Dictionary<string, AnimationManager> animations;

        private Texture2D rectangleTexture;

        //private StartScene startScene;
        private bool spacePressed;

        private KeyboardState prevKeyState;

        //private FollowCamera camera1;
        public FollowCamera camera;

        private List<Rectangle> intersections;

        private Dictionary<Vector2, int> tilemap;
        private List<Rectangle> textureStore;
        private Texture2D textureAtlas;

        private int TILESIZE = 48;
        public GameScene(ContentManager contentManager, SceneManager sceneManager, GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            this.graphicsDevice = graphicsDevice;
            this.graphics = graphics;
            intersections = new();
            tilemap = LoadMap("../../../Content/Tilemaps/LEVEL1.csv");
            textureStore = new()
            {
                new Rectangle(0, 0, 16, 16),
                new Rectangle(16,0, 16,16),
                new Rectangle(32, 0, 16,16),
                new Rectangle(48, 0, 16,16),
                new Rectangle(64, 0, 16,16),
                new Rectangle(80, 0, 16,16),
                new Rectangle(96, 0, 16,16),

                new Rectangle(0, 16, 16, 16),
                new Rectangle(16,16, 16,16),
                new Rectangle(32, 16, 16,16),
                new Rectangle(48, 16, 16,16),
                new Rectangle(64, 16, 16,16),
                new Rectangle(80, 16, 16,16),
                new Rectangle(96, 16, 16,16),

                new Rectangle(0, 32, 16, 16),
                new Rectangle(16,32, 16,16),
                new Rectangle(32, 32, 16,16),
                new Rectangle(48, 32, 16,16),
                new Rectangle(64, 32, 16,16),
                new Rectangle(80, 32, 16,16),
                new Rectangle(96, 32, 16,16),

                new Rectangle(0, 48, 16, 16),
                new Rectangle(16,48, 16,16),
                new Rectangle(32, 48, 16,16),
                new Rectangle(48, 48, 16,16),
                new Rectangle(64, 48, 16,16),
                new Rectangle(80, 48, 16,16),
                new Rectangle(96, 48, 16,16),

            };

            camera = new FollowCamera(graphics, new Vector2(0, 0), 1, 0);
            camera.SetLimit(new Rectangle(0, 0, 1920, 768));

        }

        private Dictionary<Vector2, int> LoadMap(string filepath)
        {
            Dictionary<Vector2, int> result = new();

            StreamReader reader = new StreamReader(filepath);
            int y = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split(',');
                for (int x = 0; x < items.Length; x++)
                {
                    if (int.TryParse(items[x], out int value))
                    {
                        if (value > -1)
                        {
                            result[new Vector2(x, y)] = value;
                        }
                    }
                }

                y++;
            }

            return result;
        }
        public void Load()
        {
            animations = new Dictionary<string, AnimationManager>()
            {
                {"WalkUp", new AnimationManager(contentManager.Load<Texture2D>("Character/Unarmed_Walk_full2"),6, 6, new Vector2(15, 28), 0, 3)},
                {"WalkDown", new AnimationManager(contentManager.Load<Texture2D>("Character/Unarmed_Walk_full2"),6, 6, new Vector2(15, 28), 0, 0)},
                {"WalkRight", new AnimationManager(contentManager.Load<Texture2D>("Character/Unarmed_Walk_full2"),6, 6, new Vector2(15, 28), 0, 2)},
                {"WalkLeft", new AnimationManager(contentManager.Load<Texture2D>("Character/Unarmed_Walk_full2"),6, 6, new Vector2(15, 28), 0, 1)},
                {"IdleUp", new AnimationManager(contentManager.Load<Texture2D>("Character/Unarmed_Idle_full2"),4, 4, new Vector2(15, 28), 0, 3)},
                {"IdleDown", new AnimationManager(contentManager.Load<Texture2D>("Character/Unarmed_Idle_full2"),12, 12, new Vector2(15, 28), 0, 0)},
                {"IdleRight", new AnimationManager(contentManager.Load<Texture2D>("Character/Unarmed_Idle_full2"),12, 12, new Vector2(15, 28), 0, 2)},
                {"IdleLeft", new AnimationManager(contentManager.Load<Texture2D>("Character/Unarmed_Idle_full2"),12, 12, new Vector2(15, 28), 0, 1)},
            };
            var boxAnimation = new Dictionary<string, AnimationManager>()
            {
                {
                    "box",
                    new AnimationManager(contentManager.Load<Texture2D>("box"), 0, 0,
                        new Vector2(32, 32), 0, 0)
                }
            };
            var keyAnimation = new Dictionary<string, AnimationManager>()
            {
                {
                    "key",
                    new AnimationManager(contentManager.Load<Texture2D>("key"), 0, 0,
                        new Vector2(32, 32), 0, 0)
                }
            };
            var doorAnimation = new Dictionary<string, AnimationManager>()
            {
                {
                    "door1",
                    new AnimationManager(contentManager.Load<Texture2D>("door1"), 0, 0,
                        new Vector2(5, 32), 0, 0)
                }
            };
            var ropeAnimation = new Dictionary<string, AnimationManager>()
            {
                {
                    "rope",
                    new AnimationManager(contentManager.Load<Texture2D>("rope"), 0, 0,
                        new Vector2(3, 80), 0, 0)
                }
            };
            var leverAnimation = new Dictionary<string, AnimationManager>()
            {
                {
                    "lever-left",
                    new AnimationManager(contentManager.Load<Texture2D>("lever"), 2, 2,
                        new Vector2(14, 9), 1, 0)
                },{
                    "lever-right",
                    new AnimationManager(contentManager.Load<Texture2D>("lever"), 2, 2,
                        new Vector2(14, 9), 0, 0)
                },
            };
            box = new Box(boxAnimation, new Vector2(144 * 3, 48 * 3), 32 * 3, 32 * 3);
            key = new Key(keyAnimation, new Vector2(230 * 3, 48 * 3), 32 * 3, 32 * 3);
            door = new Door(doorAnimation, new Vector2(363 * 3, 80 * 3), 32 * 3, 5 * 3);
            //rope = new List<Rope>
            //{
            //    new (ropeAnimation, new Vector2(471 * 3, 48 * 3), 8 * 3, 3 * 3),
            //    new (ropeAnimation, new Vector2(471 * 3, 54 * 3), 8 * 3, 3 * 3),
            //    new (ropeAnimation, new Vector2(471 * 3, 62 * 3), 8 * 3, 3 * 3),
            //    new (ropeAnimation, new Vector2(471 * 3, 70 * 3), 8 * 3, 3 * 3),
            //    new (ropeAnimation, new Vector2(471 * 3, 78 * 3), 8 * 3, 3 * 3),
            //    new (ropeAnimation, new Vector2(471 * 3, 84 * 3), 8 * 3, 3 * 3),
            //    new (ropeAnimation, new Vector2(471 * 3, 92 * 3), 8 * 3, 3 * 3),
            //    new (ropeAnimation, new Vector2(471 * 3, 100 * 3), 8 * 3, 3 * 3),
            //};
            rope = new Rope(ropeAnimation, new Vector2(470 * 3, 48 * 3), 80 * 3, 3 * 3);
            leverAm1 = new AnimationManager(leverAnimation["lever-left"].Texture, 0, 0, new Vector2(7, 9), 1, 0);
            leverAm2 = new AnimationManager(leverAnimation["lever-right"].Texture, 0, 0, new Vector2(7, 9), 0, 0);
            lever1 = new Lever(leverAnimation, new Vector2(548 * 3, 103 * 3), 9 * 3, 7 * 3, leverAm2);
            lever2 = new Lever(leverAnimation, new Vector2(374 * 3, 103 * 3), 9 * 3, 7 * 3, leverAm2);
            boxAm = new AnimationManager(boxAnimation["box"].Texture, 0, 0, new Vector2(32, 32), 0, 0);
            keyAm = new AnimationManager(keyAnimation["key"].Texture, 0, 0, new Vector2(32, 32), 0, 0);
            doorAm = new AnimationManager(doorAnimation["door1"].Texture, 0, 0, new Vector2(5, 32), 0, 0);
            ropeAm = new AnimationManager(ropeAnimation["rope"].Texture, 0, 0, new Vector2(3, 80), 0, 0);
            texture = contentManager.Load<Texture2D>("Character/Unarmed_Idle_full2");
            player = new Player(animations, new Vector2(80, 578), 96, 48);
            //am = animations["IdleDown"];

            rectangleTexture = new Texture2D(graphicsDevice, 1, 1);
            rectangleTexture.SetData(new Color[] { new(255, 0, 0, 255) });

            textureAtlas = contentManager.Load<Texture2D>("collisions3");

            //camera1 = new FollowCamera(graphics, player.Position);

            //startScene = new StartScene(contentManager);
        }

        public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            KeyboardState currentKeyState = Keyboard.GetState();

            player.Update(currentKeyState, prevKeyState, gameTime);
            box.Update(currentKeyState, prevKeyState, gameTime);
            key.Update(currentKeyState, prevKeyState, gameTime);
            rope.Update(currentKeyState, prevKeyState, gameTime, moveRope);
            lever1.Update(currentKeyState, prevKeyState, gameTime);
            lever2.Update(currentKeyState, prevKeyState, gameTime);
            MoveRope();

            HandleJumpInput(currentKeyState);

            UpdatePlayerAnimation();
            HandleCollisions();

            camera.Approach(player.Rect.Location.ToVector2() + new Vector2(0, player.Rect.Height), 0.2f);

            prevKeyState = currentKeyState;

        }

        private void HandleJumpInput(KeyboardState currentKeyState)
        {
            if (currentKeyState.IsKeyDown(Keys.Space) && !prevKeyState.IsKeyDown(Keys.Space) && player.Grounded)
            {
                isJumping = true;
                player.Grounded = false;
            }
        }

        private void UpdatePlayerAnimation()
        {
            am = player.State switch
            {
                PlayerState.WalkUp => animations["WalkUp"],
                PlayerState.WalkDown => animations["WalkDown"],
                PlayerState.WalkLeft => animations["WalkLeft"],
                PlayerState.WalkRight => animations["WalkRight"],
                PlayerState.IdleUp => animations["IdleUp"],
                PlayerState.IdleDown => animations["IdleDown"],
                PlayerState.IdleLeft => animations["IdleLeft"],
                PlayerState.IdleRight => animations["IdleRight"],
                _ => am // Keep the current animation if no match
            };

            am.Update();
        }

        private void HandleCollisions()
        {
            player.Grounded = false;
            ApplyGravity(player);
            ApplyGravity(box);

            if (Keyboard.GetState().IsKeyUp(Keys.X))
            {
                //Debug.WriteLine("x is not pressed");
                player.DetachBox();
                player.DetachRope();
            }

            if (player.Rect.Intersects(box.Rect))
            {
                boxIsCollide = true;
                HandleEntityCollision(box);
            }
            else
            {
                boxIsCollide = false;
            }

            if (player.Rect.Intersects(key.Rect))
            {
                player.AttachKey(key);
            }
            if (player.Rect.Intersects(rope.Rect))
            {
                HandleRopeCollision(rope);
            }
            if (player.Rect.Intersects(lever1.Rect))
            {
                HandleLever1Collision(lever1);
            }
            if (player.Rect.Intersects(lever2.Rect))
            {
                HandleLever1Collision(lever2);
            }
            if (player.Rect.Intersects(door.Rect))
            {
                if (player.HeldKey == null)
                {
                    HandleEntityCollision(door);
                }
                else
                {
                    doorAndKeyIsCollide = true;
                    //Debug.WriteLine("door and key is collide");
                    //player.DetachKey();
                }
            }
        }

        private void HandleLever1Collision(Lever lever)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.X) && !prevKeyState.IsKeyDown(Keys.X))
            {
                if (lever.leverAnimation == leverAm1)
                {
                    //Debug.WriteLine("rope move right");
                    lever.leverAnimation = leverAm2;
                    moveRope = 2;
                }
                else
                {
                    //Debug.WriteLine("rope move left");
                    lever.leverAnimation = leverAm1;
                    moveRope = 1;
                    //MoveRope(1);
                }
                //Debug.WriteLine("flick lever");
            }
        }

        private void MoveRope()
        {
            if (moveRope == 1)
            {
                if (rope.Rect.X > 390 * 3)
                {
                    rope.ApplyVelocityX((int)rope.Velocity.X);
                    //Debug.WriteLine("velocity negative");
                }
            }
            else if (moveRope == 2)
            {
                if (rope.Rect.X < 534 * 3)
                {
                    rope.ApplyVelocityX((int)rope.Velocity.X);
                    //Debug.WriteLine("velocity positive");

                }
            }
            else
            {
                rope.ApplyVelocityX((int)0);
                //Debug.WriteLine("no velocity");
            }
        }

        private void ApplyGravity(Sprite entity)
        {
            // Handle horizontal movement and collisions
            //entity.Rect.X += (int)entity.Velocity.X;
            entity.ApplyVelocityX((int)entity.Velocity.X);
            intersections = GetIntersectingTiles(entity.ColliderRect, horizontal: true);

            foreach (var rect in intersections)
            {
                if (tilemap.TryGetValue(new Vector2(rect.X, rect.Y), out int _val))
                {
                    Rectangle collision = new Rectangle(
                        rect.X * TILESIZE,
                        rect.Y * TILESIZE,
                        TILESIZE,
                        TILESIZE
                    );

                    if (!entity.Rect.Intersects(collision)) continue;

                    if (_val == 1)
                    {
                        // this needs to be here!
                    }

                    else if (_val == 5)
                    {
                        player.CollideWithLadder();
                    }
                    // handle collisions based on the direction the player is moving
                    else if (entity.Velocity.X > 0.0f)
                    {
                        //entity.Velocity.X = 0.0f;
                        entity.Rect.X = collision.Left - entity.Rect.Width;
                        if (entity == player)
                        {
                            if (player.HeldBox != null)
                                player.HeldBox.Velocity.X = 0.0f;
                        }

                        //Debug.WriteLine(entity + "moving right");
                    }
                    else if (entity.Velocity.X < 0.0f)
                    {
                        //entity.Velocity.X = 0.0f;
                        entity.Rect.X = collision.Right;
                        if (entity == player)
                        {
                            if (player.HeldBox != null)
                                player.HeldBox.Velocity.X = 0.0f;
                        }

                        //Debug.WriteLine(entity + "moving left");
                    }

                }


            }

            // Handle vertical movement and collisions
            float entityPastY = entity.Rect.Bottom;
            //entity.Rect.Y += (int)entity.Velocity.Y;
            entity.ApplyVelocityY((int)entity.Velocity.Y);
            intersections = GetIntersectingTiles(entity.ColliderRect, horizontal: false);

            foreach (var rect in intersections)
            {

                if (tilemap.TryGetValue(new Vector2(rect.X, rect.Y), out int _val))
                {

                    Rectangle collision = new Rectangle(
                        rect.X * TILESIZE,
                        rect.Y * TILESIZE,
                        TILESIZE,
                        TILESIZE
                    );

                    if (!entity.Rect.Intersects(collision))
                    {
                        continue;
                    }


                    if (_val == 1)
                    {
                        bool movingDown = entity.Velocity.Y > 0.0f;
                        bool justCrossedTileTop = entityPastY < (collision.Top + 2);
                        if (movingDown && justCrossedTileTop)
                        {
                            entity.Rect.Y = collision.Top - entity.Rect.Height;
                            entity.Velocity.Y = 1.0f;
                            entity.Grounded = true;
                        }
                    }

                    else if (_val == 5)
                    {
                        player.CollideWithLadder();
                    }
                    // handle collisions based on the direction the player is moving
                    else if (entity.Velocity.Y > 0.0f)
                    {
                        entity.Rect.Y = collision.Top - entity.Rect.Height;
                        entity.Velocity.Y = 1.0f;
                        entity.Grounded = true;
                        //if (entity == player && boxIsCollide)
                        //{
                        //    Debug.WriteLine("colliding with box");
                        //}
                        //Debug.WriteLine(entity + " is moving down and is at " + entity.Rect.Y + " pos and " + entity.Velocity.X + " x velocity");
                    }
                    else if (entity.Velocity.Y < 0.0f)
                    {


                        entity.Rect.Y = collision.Bottom;
                        entity.Velocity.Y = -1.0f;
                        //Debug.WriteLine(entity + " is moving up");
                    }



                }
                // Reset the jump flag once upward motion stops
                //if (entity.Velocity.Y >= 0.0f)
                //{
                //    isJumping = false;
                //}
            }
        }

        private void HandleEntityCollision(Sprite entity)
        {
            Rectangle playerRect = player.Rect;
            Rectangle entityRect = entity.Rect;

            if (playerRect.Intersects(entityRect))
            {
                // Calculate overlap amounts
                int overlapX = Math.Min(playerRect.Right - entityRect.Left, entityRect.Right - playerRect.Left);
                int overlapY = Math.Min(playerRect.Bottom - entityRect.Top, entityRect.Bottom - playerRect.Top);

                if (Keyboard.GetState().IsKeyDown(Keys.X))
                {
                    if (entity is Box box)
                    {
                        player.AttachBox(box);
                    }
                }

                // Determine which side to resolve based on the player's position
                if (overlapX < overlapY || (overlapX == overlapY && Math.Abs(player.Velocity.X) > Math.Abs(player.Velocity.Y)))
                {
                    // Horizontal collision
                    if (playerRect.Center.X < entityRect.Center.X) // Player is to the left of the box
                    {
                        player.Rect.X = entity.Rect.Left - player.Rect.Width;
                        player.Velocity.X = 0.0f; // Stop horizontal movement
                    }
                    else if (playerRect.Center.X > entityRect.Center.X) // Player is to the right of the box
                    {
                        player.Rect.X = entity.Rect.Right;
                        player.Velocity.X = 0.0f; // Stop horizontal movement
                    }
                }
                else
                {
                    // Vertical collision
                    if (playerRect.Center.Y < entityRect.Center.Y) // Player is above the box
                    {
                        player.Rect.Y = entity.Rect.Top - player.Rect.Height;
                        player.Velocity.Y = 0.0f;
                        player.Grounded = true; // Set grounded to true
                    }
                    else if (playerRect.Center.Y > entityRect.Center.Y) // Player is below the box
                    {
                        // Stop the box's downward velocity if it is falling onto the player's head
                        if (entity.Velocity.Y > 0)
                        {
                            entity.Velocity.Y = 0.0f; // Stop the box's downward movement
                            entity.Rect.Y = player.Rect.Top - entity.Rect.Height; // Align the box just above the player
                        }

                        // Keep the original player behavior
                        player.Rect.Y = entity.Rect.Bottom;
                        player.Velocity.Y = 1.0f; // Reset vertical velocity
                    }
                }
            }
        }
        private void HandleRopeCollision(Rope rope)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.X))
            {

                //Debug.WriteLine("x before: " + player.HeldRopePos.X);
                //Debug.WriteLine("y before: " + player.HeldRopePos.Y);
                player.AttachRope(rope);
                //Debug.WriteLine("y after: " + player.HeldRopePos.Y);

                //player.Rect.X = (int)player.HeldRopePos.X;
                //player.Rect.Y = (int)player.HeldRopePos.Y;

            }
            //Debug.WriteLine("x after: " + player.HeldRopePos.X);
        }

        private List<Rectangle> GetIntersectingTiles(Rectangle entityRect, bool horizontal)
        {
            List<Rectangle> intersections = new();
            int startX = horizontal ? entityRect.Left / TILESIZE : entityRect.Left / TILESIZE;
            int endX = horizontal ? entityRect.Right / TILESIZE : entityRect.Right / TILESIZE;
            int startY = horizontal ? entityRect.Top / TILESIZE : entityRect.Top / TILESIZE;
            int endY = horizontal ? entityRect.Bottom / TILESIZE : entityRect.Bottom / TILESIZE;

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    intersections.Add(new Rectangle(x, y, TILESIZE, TILESIZE));
                }
            }
            return intersections;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            player.Draw(spriteBatch, am);
            box.Draw(spriteBatch, boxAm);
            if (!doorAndKeyIsCollide)
            {
                key.Draw(spriteBatch, keyAm);
            }
            door.Draw(spriteBatch, doorAm);
            rope.Draw(spriteBatch, ropeAm);
            lever1.Draw(spriteBatch, lever1.leverAnimation);
            lever2.Draw(spriteBatch, lever2.leverAnimation
            );
            //foreach (var item in rope)
            //{
            //    item.Draw(spriteBatch, ropeAm);
            //}
            //DrawRectHollow(spriteBatch, player.ColliderRect, 4);
            foreach (var item in tilemap)
            {
                int value = item.Value;
                if (value >= 0 && value < textureStore.Count)
                {
                    Rectangle dest = new((int)item.Key.X * TILESIZE, (int)item.Key.Y * TILESIZE, TILESIZE, TILESIZE);
                    Rectangle src = textureStore[value];
                    spriteBatch.Draw(textureAtlas, dest, src, Color.White);
                }
            }

            //foreach (var rect in intersections)
            //{

            //    DrawRectHollow(
            //        spriteBatch,
            //        new Rectangle(
            //            rect.X * TILESIZE,
            //            rect.Y * TILESIZE,
            //            TILESIZE,
            //            TILESIZE
            //        ),
            //        4
            //    );

            //}
        }

        public void DrawRectHollow(SpriteBatch spriteBatch, Rectangle rect, int thickness)
        {
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Y,
                    rect.Width,
                    thickness
                ),
                Color.White
            );
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Bottom - thickness,
                    rect.Width,
                    thickness
                ),
                Color.White
            );
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Y,
                    thickness,
                    rect.Height
                ),
                Color.White
            );
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.Right - thickness,
                    rect.Y,
                    thickness,
                    rect.Height
                ),
                Color.White
            );
        }

        public FollowCamera GetCamera()
        {
            return camera;
        }
    }
}
