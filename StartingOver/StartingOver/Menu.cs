using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace StartingOver
{
    internal class Menu : Sprite
    {
        // Define the menu states and their corresponding textures.
        Dictionary<int, string> menuStates = new Dictionary<int, string>
        {
            { 0, "menu_resume" },
            { 1, "menu_restart" },
            { 2, "menu_options" },
            { 3, "menu_exit" }
        };
        int currentState = 0; // The index of the currently selected menu state.
        KeyboardState previousKeystate;
        SceneManager sceneManager;
        public Menu(Dictionary<string, AnimationManager> animation, Vector2 position, int height, int width, SceneManager sceneManager) : base(animation, position, height, width)
        {
            Texture = animation["menu_resume"].Texture;
            Velocity = new();
            this.sceneManager = sceneManager;
        }

        public override void Update(KeyboardState keystate, KeyboardState prevKeyState, GameTime gameTime)
        {
            // Navigate down
            if (keystate.IsKeyDown(Keys.Down) && !previousKeystate.IsKeyDown(Keys.Down))
            {
                currentState = (currentState + 1) % menuStates.Count; // Wrap around to the top
            }

            // Navigate up
            if (keystate.IsKeyDown(Keys.Up) && !previousKeystate.IsKeyDown(Keys.Up))
            {
                currentState = (currentState - 1 + menuStates.Count) % menuStates.Count; // Wrap around to the bottom
            }

            // Set the current texture to the selected menu state's texture.
            Texture = Animation[menuStates[currentState]].Texture;

            // Handle Enter key
            if (keystate.IsKeyDown(Keys.Enter) && !previousKeystate.IsKeyDown(Keys.Enter))
            {
                switch (currentState)
                {
                    case 0:
                        sceneManager.ResumeGame(sceneManager.GetCurrentScene());
                        break;

                    case 1:
                        sceneManager.RestartGame(sceneManager.GetCurrentScene());
                        break;

                    case 2:
                        sceneManager.OpenOptions();
                        break;

                    case 3:
                        sceneManager.ExitGame();
                        break;
                }
            }

            previousKeystate = keystate;
        }

        
    }
}
