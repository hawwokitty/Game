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

        //private Button button;
        private AnimationManager am;

        private KeyboardState prevKeyState;

        public StartScene(ContentManager contentManager, GraphicsDeviceManager graphics)
        {
            this.contentManager = contentManager;
            camera = new FollowCamera(graphics, new Vector2(0, 0), 1, 0);
            camera.SetLimit(new Rectangle(0, 0, 1920, 768));
        }

        public void Load()
        {
            texture = contentManager.Load<Texture2D>("Scenes/menu");
            //button = new Button(texture, new Vector2(100, 100), texture.Height*4, texture.Width*4);
            am = new(texture, 0, 0, new Vector2(texture.Width, texture.Height), 0, 0);
        }

        public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            //button.Update(Keyboard.GetState(), prevKeyState, gameTime);
            prevKeyState = Keyboard.GetState();

            //am.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //button.Draw(spriteBatch, am);
        }

        public FollowCamera GetCamera()
        {
            return camera;
        }
    }
}
