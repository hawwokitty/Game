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
        private AnimationManager am;

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
            tilemap = LoadMap("../../../Content/Tilemaps/pleasework.csv");
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
            texture = contentManager.Load<Texture2D>("Character/Unarmed_Idle_full2");
            player = new Player(animations, new Vector2(100, 50), 96, 48);
            //am = animations["IdleDown"];

            rectangleTexture = new Texture2D(graphicsDevice, 1, 1);
            rectangleTexture.SetData(new Color[] { new(255, 0, 0, 255) });

            textureAtlas = contentManager.Load<Texture2D>("collisions3");

            //camera1 = new FollowCamera(graphics, player.Position);

            //startScene = new StartScene(contentManager);
        }

        public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            player.Update(Keyboard.GetState(), prevKeyState, gameTime);

            prevKeyState = Keyboard.GetState();

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

            // add player's velocity and grab the intersecting tiles
            player.Rect.X += (int)player.Velocity.X;
            intersections = getIntersectingTilesHorizontal(player.Rect);

            player.Grounded = false;

            foreach (var rect in intersections)
            {

                // handle collisions if the tile position exists in the tile map layer.
                if (tilemap.TryGetValue(new Vector2(rect.X, rect.Y), out int _val))
                {

                    // create temp rect to handle collisions (not necessary, you can optimize!)
                    Rectangle collision = new(
                        rect.X * TILESIZE,
                        rect.Y * TILESIZE,
                        TILESIZE,
                        TILESIZE
                    );

                    if (!player.Rect.Intersects(collision))
                    {
                        continue;
                    }

                    if (_val == 1)
                    {
                        // Only handle top-face collision if the player is moving down
                        if (player.Velocity.Y > 0.0f)
                        {
                            player.Rect.Y = collision.Top - player.Rect.Height;
                            player.Velocity.Y = 1.0f;  
                            player.Grounded = true;     
                        }
                    }
                    // handle collisions based on the direction the player is moving
                    else if (player.Velocity.X > 0.0f)
                    {
                        player.Rect.X = collision.Left - player.Rect.Width;
                    }
                    else if (player.Velocity.X < 0.0f)
                    {
                        player.Rect.X = collision.Right;
                    }

                }

            }

            // same as horizontal collisions

            player.Rect.Y += (int)player.Velocity.Y;
            intersections = getIntersectingTilesVertical(player.Rect);

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

                    if (!player.Rect.Intersects(collision))
                    {
                        continue;
                    }

                    if (_val == 1)
                    {
                        // Only handle top-face collision if the player is moving down
                        if (player.Velocity.Y > 0.0f)
                        {
                            player.Rect.Y = collision.Top - player.Rect.Height;
                            player.Velocity.Y = 1.0f;  
                            player.Grounded = true;  
                        }
                    }
                    // handle collisions based on the direction the player is moving
                    else if (player.Velocity.Y > 0.0f)
                    {
                        player.Rect.Y = collision.Top - player.Rect.Height;
                        player.Velocity.Y = 1.0f;
                        player.Grounded = true;
                    }
                    else if (player.Velocity.Y < 0.0f)
                    {
                        player.Rect.Y = collision.Bottom;
                    }

                }
            }

            //camera.Follow(player.Rect, new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));

            //if (!spacePressed && Keyboard.GetState().IsKeyDown(Keys.G))
            //{
            //    if (sceneManager.GetCurrentScene() != startScene)
            //    {
            //        sceneManager.AddScene(startScene);
            //    }
            //    spacePressed = true;
            //}

            //if (spacePressed && Keyboard.GetState().IsKeyUp(Keys.G))
            //{
            //    if (sceneManager.GetCurrentScene() == startScene)
            //    {
            //        sceneManager.RemoveScene(startScene);
            //    }
            //    spacePressed = false;
            //}

            camera.Approach(player.Rect.Location.ToVector2() + new Vector2(0,player.Rect.Height),0.2f);

        }

        public List<Rectangle> getIntersectingTilesHorizontal(Rectangle target)
        {

            List<Rectangle> intersections = new();

            int widthInTiles = (target.Width - (target.Width % TILESIZE)) / TILESIZE;
            int heightInTiles = (target.Height - (target.Height % TILESIZE)) / TILESIZE;

            for (int x = 0; x <= widthInTiles; x++)
            {
                for (int y = 0; y <= heightInTiles; y++)
                {

                    intersections.Add(new Rectangle(

                        (target.X + x * TILESIZE) / TILESIZE,
                        (target.Y + y * (TILESIZE - 1)) / TILESIZE,
                        TILESIZE,
                        TILESIZE

                    ));

                }
            }

            return intersections;
        }
        public List<Rectangle> getIntersectingTilesVertical(Rectangle target)
        {

            List<Rectangle> intersections = new();

            int widthInTiles = (target.Width - (target.Width % TILESIZE)) / TILESIZE;
            int heightInTiles = (target.Height - (target.Height % TILESIZE)) / TILESIZE;

            for (int x = 0; x <= widthInTiles; x++)
            {
                for (int y = 0; y <= heightInTiles; y++)
                {

                    intersections.Add(new Rectangle(

                        (target.X + x * (TILESIZE - 1)) / TILESIZE,
                        (target.Y + y * TILESIZE) / TILESIZE,
                        TILESIZE,
                        TILESIZE

                    ));

                }
            }

            return intersections;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            player.Draw(spriteBatch, am);
            //DrawRectHollow(spriteBatch, player.Rect, 4);
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
