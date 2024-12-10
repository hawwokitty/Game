using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StartingOver
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

       

        private SceneManager sceneManager;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            //_graphics.PreferredBackBufferWidth = 1920; // Set width
            //_graphics.PreferredBackBufferHeight = 768; // Set height

            //// Apply the changes to the graphics device
            //_graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            sceneManager = new()
            {
                game1 = this,
            };
            

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            makeGameScene();
        }

        public void makeGameScene()
        {
            sceneManager.AddScene(new GameScene(Content, sceneManager, GraphicsDevice, _graphics));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            // TODO: Add your update logic here
            sceneManager.GetCurrentScene().Update(gameTime, _graphics);


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: sceneManager.GetCurrentScene().GetCamera().ViewMatrix);
            sceneManager.GetCurrentScene().Draw(_spriteBatch);
            


            _spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
