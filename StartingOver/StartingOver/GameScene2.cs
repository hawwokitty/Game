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
    internal class GameScene2 : IScene
    {
        private ContentManager contentManager;
        private SceneManager sceneManager;
        private Texture2D texture;
        private GraphicsDevice graphicsDevice;
        private GraphicsDeviceManager graphics;

        private Sprite collisionSprite;

        private Player player;
        private Box box;
        private Key key;
        private Door door;
        private Button button;
        private Button button2;
        private Platform platform;
        private Platform platform2;
        private Platform exitCollision;
        private Platform exitCollisionV;
        private Platform enterCollision;
        private Platform enterCollisionV;
        private AnimationManager boxAm;
        private AnimationManager keyAm;
        private AnimationManager doorAm;
        private AnimationManager door2Am;
        private AnimationManager am;
        private AnimationManager buttonAm;
        private AnimationManager button2Am;
        private AnimationManager platformAm;
        private AnimationManager platform2Am;

        //private Dictionary<string, AnimationManager> buttonAnimation;

        private bool isJumping;

        private int movePlatform;
        private int movePlatform2;

        private bool boxIsCollide;
        private bool doorAndKeyIsCollide;

        private Dictionary<string, AnimationManager> animations;

        private Texture2D rectangleTexture;

        private StartScene startScene;
        private DeathScene deathScene;

        private KeyboardState prevKeyState;

        public FollowCamera camera;

        private List<Rectangle> intersections;

        private Dictionary<Vector2, int> tilemap;
        private Dictionary<Vector2, int> bgT;
        private Dictionary<Vector2, int> fg1T;
        private Dictionary<Vector2, int> fg2T;
        private Dictionary<Vector2, int> fg3T;

        private List<Rectangle> textureStore;
        private List<Rectangle> textureStoreTilemap;
        private Texture2D textureAtlas;
        private Texture2D bg;
        private Texture2D fg1;
        private Texture2D fg2;
        private Texture2D fg3;


        private int TILESIZE = 48;

        public GameScene2(ContentManager contentManager, SceneManager sceneManager, GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            this.graphicsDevice = graphicsDevice;
            this.graphics = graphics;
            intersections = new();
            tilemap = LoadMap("../../../Content/Tilemaps/LEVEL2_6_collisions.csv");
            bgT = LoadMap("../../../Content/Tilemaps/LEVEL2_6_background.csv");
            fg1T = LoadMap("../../../Content/Tilemaps/LEVEL2_6_foreground1.csv");
            fg2T = LoadMap("../../../Content/Tilemaps/LEVEL2_6_foreground2.csv");
            fg3T = LoadMap("../../../Content/Tilemaps/LEVEL2_6_foreground3.csv");

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

            textureStoreTilemap = new List<Rectangle>();

            int tileWidth = 16;
            int tileHeight = 16;
            int rows = 21;
            int columns = 27;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    textureStoreTilemap.Add(new Rectangle(col * tileWidth, row * tileHeight, tileWidth, tileHeight));
                }
            }



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
                {"WalkUp", new AnimationManager(contentManager.Load<Texture2D>("Character/character_s"),8, 8, new Vector2(16, 16), 0, 7)},
                {"WalkDown", new AnimationManager(contentManager.Load<Texture2D>("Character/character_s"),8, 8, new Vector2(16, 16), 0, 6)},
                {"WalkRight", new AnimationManager(contentManager.Load<Texture2D>("Character/character_s_color"),8, 8, new Vector2(16, 16), 0, 4)},
                {"WalkLeft", new AnimationManager(contentManager.Load<Texture2D>("Character/character_s_color"),8, 8, new Vector2(16, 16), 0, 5)},
                {"IdleUp", new AnimationManager(contentManager.Load<Texture2D>("Character/character_s"),4, 4, new Vector2(16, 16), 0, 3)},
                {"IdleDown", new AnimationManager(contentManager.Load<Texture2D>("Character/character_s"),4, 4, new Vector2(16, 16), 0, 2)},
                {"IdleRight", new AnimationManager(contentManager.Load<Texture2D>("Character/character_s_color"),4, 4, new Vector2(16, 16), 0, 0)},
                {"IdleLeft", new AnimationManager(contentManager.Load<Texture2D>("Character/character_s_color"),4, 4, new Vector2(16, 16), 0, 1)},
            };
            var boxAnimation = new Dictionary<string, AnimationManager>()
            {
                {
                    "box",
                    new AnimationManager(contentManager.Load<Texture2D>("box3"), 0, 0,
                        new Vector2(30, 30), 0, 0)
                }
            };
            var keyAnimation = new Dictionary<string, AnimationManager>()
            {
                {
                    "key",
                    new AnimationManager(contentManager.Load<Texture2D>("key_small2"), 0, 0,
                        new Vector2(16, 16), 0, 0)
                }
            };
            var doorAnimation = new Dictionary<string, AnimationManager>()
            {
                {
                    "door-closed",
                    new AnimationManager(contentManager.Load<Texture2D>("door-both2"), 2, 2,
                        new Vector2(14, 32), 0, 0)
                },
                {
                    "door-open",
                    new AnimationManager(contentManager.Load<Texture2D>("door-both2"), 2, 2,
                        new Vector2(14, 32), 1, 0)
                }
            };
            var buttonAnimation = new Dictionary<string, AnimationManager>()
            {
                {
                    "button-up",
                    new AnimationManager(contentManager.Load<Texture2D>("button-both"), 2, 2,
                        new Vector2(26, 9), 1, 0)
                },
                {
                    "button-down",
                    new AnimationManager(contentManager.Load<Texture2D>("button-both"), 2, 2,
                        new Vector2(26, 9), 0, 0)
                },
            };
            var platformAnimation = new Dictionary<string, AnimationManager>()
            {
                {
                    "platform",
                    new AnimationManager(contentManager.Load<Texture2D>("platform1"), 0, 0,
                        new Vector2(144, 8), 0, 0)
                }
            };
            var platform2Animation = new Dictionary<string, AnimationManager>()
            {
                {
                    "platform",
                    new AnimationManager(contentManager.Load<Texture2D>("platform2"), 0, 0,
                        new Vector2(8, 144), 0, 0)
                }
            };
            box = new Box(boxAnimation, new Vector2(100 * 3, 16 * 3), 30 * 3, 30 * 3);
            key = new Key(keyAnimation, new Vector2(324 * 3, 24 * 3), 16 * 3, 16 * 3);
            platform = new Platform(platformAnimation, new Vector2(169 * 3, 224 * 3), 8 * 3, 144 * 3);
            exitCollision = new Platform(platformAnimation, new Vector2(624 * 3, 176 * 3), 8 * 3, 144 * 3);
            enterCollision = new Platform(platformAnimation, new Vector2(-100 * 3, 224 * 3), 8 * 3, 144 * 3);
            platform2 = new Platform(platform2Animation, new Vector2(498 * 3, 32 * 3), 144 * 3, 8 * 3);
            exitCollisionV = new Platform(platform2Animation, new Vector2(654 * 3, 64 * 3), 144 * 3, 8 * 3);
            enterCollisionV = new Platform(platform2Animation, new Vector2(-16 * 3, 100 * 3), 144 * 3, 8 * 3);
            boxAm = new AnimationManager(boxAnimation["box"].Texture, 0, 0, new Vector2(30, 30), 0, 0);
            keyAm = new AnimationManager(keyAnimation["key"].Texture, 0, 0, new Vector2(16, 16), 0, 0);
            buttonAm = new AnimationManager(buttonAnimation["button-up"].Texture, 0, 0, new Vector2(26, 9), 0, 0);
            button2Am = new AnimationManager(buttonAnimation["button-down"].Texture, 0, 0, new Vector2(26, 9), 1, 0);
            platformAm = new AnimationManager(platformAnimation["platform"].Texture, 0, 0, new Vector2(144, 8), 0, 0);
            platform2Am = new AnimationManager(platform2Animation["platform"].Texture, 0, 0, new Vector2(8, 144), 0, 0);
            doorAm = new AnimationManager(doorAnimation["door-closed"].Texture, 0, 0, new Vector2(14, 32), 0, 0);
            door2Am = new AnimationManager(doorAnimation["door-open"].Texture, 0, 0, new Vector2(14, 32), 1, 0);
            button = new Button(buttonAnimation, new Vector2(212 * 3, 215 * 3), 9 * 3, 26 * 3, buttonAm);
            button2 = new Button(buttonAnimation, new Vector2(467 * 3, 231 * 3), 9 * 3, 26 * 3, buttonAm);
            door = new Door(doorAnimation, new Vector2(590 * 3, 144 * 3), 32 * 3, 14 * 3, doorAm);
            player = new Player(animations, new Vector2(64, 600), 16 * 3, 16 * 3);

            rectangleTexture = new Texture2D(graphicsDevice, 1, 1);
            rectangleTexture.SetData(new Color[] { new(255, 0, 0, 255) });

            textureAtlas = contentManager.Load<Texture2D>("collisions3");
            bg = contentManager.Load<Texture2D>("Tiles");
            fg1 = contentManager.Load<Texture2D>("Tiles");
            fg2 = contentManager.Load<Texture2D>("Tiles");
            fg3 = contentManager.Load<Texture2D>("Tiles");


            UpdatePlayerAnimation();

            startScene = new StartScene(contentManager, graphics, sceneManager);
            deathScene = new DeathScene(contentManager, graphics, sceneManager);
        }

        public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            KeyboardState currentKeyState = Keyboard.GetState();

            player.Update(currentKeyState, prevKeyState, gameTime);
            box.Update(currentKeyState, prevKeyState, gameTime);
            key.Update(currentKeyState, prevKeyState, gameTime);
            platform.Update(currentKeyState, prevKeyState, gameTime, movePlatform);
            platform2.Update(currentKeyState, prevKeyState, gameTime, movePlatform2);
            button.Update(currentKeyState, prevKeyState, gameTime);
            button2.Update(currentKeyState, prevKeyState, gameTime);
            door.Update(currentKeyState, prevKeyState, gameTime);


            HandleJumpInput(currentKeyState);

            UpdatePlayerAnimation();
            HandleCollisions();

            if (currentKeyState.IsKeyDown(Keys.Escape))
            {
                sceneManager.AddScene(startScene);
            }

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

            if (player.Rect.Intersects((platform.Rect)))
            {
                HandleEntityCollision(platform);
            }
            if (player.Rect.Intersects((platform2.Rect)))
            {
                HandleEntityCollision(platform2);
            }
            if (player.Rect.Intersects((exitCollision.Rect)))
            {
                HandleEntityCollision(exitCollision);
            }
            if (player.Rect.Intersects((enterCollision.Rect)))
            {
                HandleEntityCollision(enterCollision);
            }
            if (player.Rect.Intersects((enterCollisionV.Rect)))
            {
                HandleEntityCollision(enterCollisionV);
            }
            if (player.Rect.Intersects((exitCollisionV.Rect)))
            {
                player.Rect.X = -4;
                player.Rect.Y = 647;
            }
            if (box.Rect.Intersects((button.Rect)) || player.Rect.Intersects((button.Rect)))
            {
                movePlatform = 1;
                MovePlatform();
                button.ButtonAm = button2Am;
                //buttonAm = new AnimationManager(buttonAnimation["button-down"].Texture, 0, 0, new Vector2(26, 9), 0, 0);

            }
            else
            {
                movePlatform = 2;
                button.ButtonAm = buttonAm;
                //buttonAm = new AnimationManager(buttonAnimation["button-up"].Texture, 0, 0, new Vector2(26, 9), 0, 0);

            }
            if (box.Rect.Intersects((button2.Rect)) || player.Rect.Intersects((button2.Rect)))
            {
                movePlatform2 = 1;
                MovePlatform2();
                button2.ButtonAm = button2Am;
            }
            else
            {
                movePlatform2 = 3;
                MovePlatform2();
                button2.ButtonAm = buttonAm;
            }

            if (player.Rect.Intersects(key.Rect))
            {
                player.AttachKey(key);
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
                    door.DoorAm = door2Am;

                }
            }
        }


        private void MovePlatform()
        {
            if (movePlatform == 1)
            {
                if (platform.Rect.X >= 80 * 3)
                {

                    platform.ApplyVelocityX((int)platform.Velocity.X);
                }
            }
            else
            {
                platform.ApplyVelocityX((int)0);
            }
        }
        private void MovePlatform2()
        {
            if (movePlatform2 == 1)
            {
                if (platform2.Rect.Y >= -46 * 3)
                {
                    // platform goes up
                    platform2.ApplyVelocityY((int)platform2.Velocity.Y);
                }
            }
            else
            {
                platform2.ApplyVelocityY((int)0);
            }
        }


        private void ApplyGravity(Sprite entity)
        {
            // Track previous vertical position for checks
            float entityPastY = entity.Rect.Bottom;

            // Handle horizontal movement and collisions
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

                    collisionSprite = new Sprite(new Dictionary<string, AnimationManager>(),
                        new Vector2(
                        rect.X * TILESIZE,
                        rect.Y * TILESIZE),
                        TILESIZE,
                        TILESIZE);

                    if (!entity.Rect.Intersects(collision)) continue;

                    // Handle tile-specific logic
                    if (_val == 1) // Special tile type 1
                    {
                        // this has to be here
                    }
                    else if (_val == 5 && entity is Player) // Ladder tile
                    {
                        player.CollideWithLadder();
                    }
                    else // All other solid tiles
                    {
                        HandleEntityCollision(collisionSprite);
                        if (!(entity is Player))
                        {
                            if (entity.Rect.Center.X < collision.Center.X) // Moving right
                            {

                                //{
                                //    entity.Rect.X = collision.Left - entity.ColliderRect.Width - 10;
                                //}
                                //else
                                //{
                                entity.Rect.X = collision.Left - entity.Rect.Width - 1;
                                //}
                            }
                            else if (entity.Rect.Center.X > collision.Center.X) // Moving left
                            {
                                //if (entity is Player)
                                //{
                                //entity.Rect.X = collision.Right - 8;
                                //}
                                //else
                                //{
                                entity.Rect.X = collision.Right;
                                //}
                            }

                            entity.Velocity.X = 0.0f;
                        }
                    }
                }
            }

            // Handle vertical movement and collisions
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
                    collisionSprite = new Sprite(new Dictionary<string, AnimationManager>(),
                        new Vector2(
                            rect.X * TILESIZE,
                            rect.Y * TILESIZE),
                        TILESIZE,
                        TILESIZE);

                    if (!entity.Rect.Intersects(collision)) continue;

                    // Handle tile-specific logic
                    if (_val == 1) // Special tile type 1
                    {
                        bool movingDown = entity.Velocity.Y > 0.0f;
                        bool justCrossedTileTop = entityPastY < (collision.Top + 2);
                        if (movingDown && justCrossedTileTop)
                        {
                            entity.Rect.Y = collision.Top - entity.Rect.Height;
                            entity.Grounded = true;
                            FallDamage();
                            entity.Velocity.Y = 1.0f;
                        }

                    }
                    else if (_val == 5 && entity is Player) // Ladder tile
                    {
                        player.CollideWithLadder();

                        bool movingDown = entity.Velocity.Y > 0.0f;
                        bool justCrossedTileTop = entityPastY < (collision.Top + 2);
                        if (movingDown && justCrossedTileTop && !Keyboard.GetState().IsKeyDown(Keys.Down))
                        {
                            entity.Rect.Y = collision.Top - entity.ColliderRect.Height;
                            entity.Grounded = true;
                            FallDamage();
                            entity.Velocity.Y = 0.0f;
                        }
                    }
                    else // All other solid tiles
                    {
                        HandleEntityCollision(collisionSprite);
                        if (!(entity is Player))
                        {
                            if (entity.Rect.Center.Y < collision.Center.Y) // Moving down
                            {
                                entity.Rect.Y = collision.Top - entity.Rect.Height;
                                entity.Grounded = true;
                                FallDamage();
                                entity.Velocity.Y = 1.0f;
                            }
                            else if (entity.Rect.Center.Y > collision.Center.Y) // Moving up
                            {
                                entity.Rect.Y = collision.Bottom;
                                entity.Velocity.Y = -1.0f;
                            }
                        }

                    }
                }
            }

        }

        private void FallDamage()
        {
            if (player.Velocity.Y >= 21 && player.Grounded || player.Velocity.Y >= 30)
            {

                sceneManager.AddScene(deathScene);
            }
        }


        private void HandleEntityCollision(Sprite entity)
        {
            Rectangle playerRect = player.ColliderRect;
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

                // Determine which collision to resolve
                if (overlapX < overlapY)
                {
                    // Horizontal collision
                    ResolveHorizontalCollision(entityRect);
                }
                else
                {
                    // Vertical collision
                    ResolveVerticalCollision(entityRect);
                }
            }
        }

        private void ResolveHorizontalCollision(Rectangle entityRect)
        {
            // Resolve horizontal position
            if (player.ColliderRect.Center.X < entityRect.Center.X)
            {
                player.Rect.X = entityRect.Left - player.ColliderRect.Width - 9; // Adjust to the left
            }
            else
            {
                player.Rect.X = entityRect.Right - 7; // Adjust to the right
            }

            // Stop horizontal velocity
            player.Velocity.X = 0.0f;

            //// Prevent sticking to walls: Ensure the player continues to fall if not grounded
            //if (!player.Grounded)
            //{
            //    player.Velocity.Y = Math.Max(player.Velocity.Y, 0.1f); // Small downward force
            //}
        }

        private void ResolveVerticalCollision(Rectangle entityRect)
        {
            if (player.ColliderRect.Center.Y < entityRect.Center.Y)
            {
                // Player is above the entity
                player.Rect.Y = entityRect.Top - player.Rect.Height;
                player.Grounded = true; // Player is grounded
                FallDamage(); // Handle any fall damage
                player.Velocity.Y = 0.0f; // Stop vertical movement
            }
            else
            {
                // Player is below the entity
                player.Rect.Y = entityRect.Bottom - 8;
                player.Velocity.Y = Math.Max(player.Velocity.Y, 1.0f); // Push downward slightly
            }
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
            foreach (var item in bgT)
            {
                int value = item.Value;
                if (value >= 0 && value < textureStoreTilemap.Count)
                {
                    Rectangle dest = new((int)item.Key.X * TILESIZE, (int)item.Key.Y * TILESIZE, TILESIZE, TILESIZE);
                    Rectangle src = textureStoreTilemap[value];
                    spriteBatch.Draw(bg, dest, src, Color.White);
                }
            }
            platform.Draw(spriteBatch, platformAm);
            platform2.Draw(spriteBatch, platform2Am);
            foreach (var item in fg1T)
            {
                int value = item.Value;
                if (value >= 0 && value < textureStoreTilemap.Count)
                {
                    Rectangle dest = new((int)item.Key.X * TILESIZE, (int)item.Key.Y * TILESIZE, TILESIZE, TILESIZE);
                    Rectangle src = textureStoreTilemap[value];
                    spriteBatch.Draw(fg1, dest, src, Color.White);
                }
            }
            foreach (var item in fg2T)
            {
                int value = item.Value;
                if (value >= 0 && value < textureStoreTilemap.Count)
                {
                    Rectangle dest = new((int)item.Key.X * TILESIZE, (int)item.Key.Y * TILESIZE, TILESIZE, TILESIZE);
                    Rectangle src = textureStoreTilemap[value];
                    spriteBatch.Draw(fg2, dest, src, Color.White);
                }
            }
            foreach (var item in fg3T)
            {
                int value = item.Value;
                if (value >= 0 && value < textureStoreTilemap.Count)
                {
                    Rectangle dest = new((int)item.Key.X * TILESIZE, (int)item.Key.Y * TILESIZE, TILESIZE, TILESIZE);
                    Rectangle src = textureStoreTilemap[value];
                    spriteBatch.Draw(fg3, dest, src, Color.White);
                }
            }

            door.Draw(spriteBatch, door.DoorAm);
            player.Draw(spriteBatch, am);
            box.Draw(spriteBatch, boxAm);
            if (!doorAndKeyIsCollide)
            {
                key.Draw(spriteBatch, keyAm);
            }
            //exitCollision.Draw(spriteBatch, platformAm);
            //exitCollisionV.Draw(spriteBatch, platform2Am);
            //enterCollision.Draw(spriteBatch, platformAm);
            //enterCollisionV.Draw(spriteBatch, platform2Am);
            button.Draw(spriteBatch, button.ButtonAm);
            button2.Draw(spriteBatch, button2.ButtonAm);

            //DrawRectHollow(spriteBatch, player.ColliderRect, 4);
            //DrawRectHollow(spriteBatch, player.Rect, 4);

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
