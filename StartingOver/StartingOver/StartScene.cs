using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingOver
{
    internal class StartScene : IScene
    {
        private ContentManager contentManager;
        private Texture2D texture;
        public FollowCamera camera;
        public SceneManager sceneManager;

        //private Button button;
        private Menu menu;
        private AnimationManager am;

        private KeyboardState prevKeyState;

        public StartScene(ContentManager contentManager, GraphicsDeviceManager graphics, SceneManager sceneManager)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            camera = new FollowCamera(graphics, new Vector2(0, 0), 1, 0);
            camera.SetLimit(new Rectangle(0, 0, 1920, 768));
        }

        public void Load()
        {
            //texture = contentManager.Load<Texture2D>("Scenes/menu");
            var menuAnimation = new Dictionary<string, AnimationManager>()
            {
                {
                    "menu",
                    new AnimationManager(contentManager.Load<Texture2D>("Scenes/menu"), 0, 0,
                        new Vector2(272, 160), 0, 0)
                },
                {
                    "menu_exit",
                    new AnimationManager(contentManager.Load<Texture2D>("Scenes/menu_exit"), 0, 0,
                        new Vector2(272, 160), 0, 0)
                },
                {
                    "menu_options",
                    new AnimationManager(contentManager.Load<Texture2D>("Scenes/menu_options"), 0, 0,
                        new Vector2(272, 160), 0, 0)
                },
                {
                    "menu_restart",
                    new AnimationManager(contentManager.Load<Texture2D>("Scenes/menu_restart"), 0, 0,
                        new Vector2(272, 160), 0, 0)
                },
                {
                    "menu_resume",
                    new AnimationManager(contentManager.Load<Texture2D>("Scenes/menu_resume"), 0, 0,
                        new Vector2(272, 160), 0, 0)
                },
            };
            //button = new Button(texture, new Vector2(100, 100), texture.Height*4, texture.Width*4);
            menu = new Menu(menuAnimation, new Vector2(0, 0), 160 * 3, 272 * 3, sceneManager);
            am = new AnimationManager(menuAnimation["menu"].Texture, 0, 0, new Vector2(272, 160), 0, 0);

            //am = new(texture, 0, 0, new Vector2(texture.Width, texture.Height), 0, 0);
        }

        public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            //button.Update(Keyboard.GetState(), prevKeyState, gameTime);
            menu.Update(Keyboard.GetState(), prevKeyState, gameTime);
            prevKeyState = Keyboard.GetState();

            //am.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //button.Draw(spriteBatch, am);
            menu.Draw(spriteBatch, am);
        }

        public FollowCamera GetCamera()
        {
            return camera;
        }
    }
}
