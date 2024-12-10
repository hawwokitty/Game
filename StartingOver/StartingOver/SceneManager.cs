using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace StartingOver
{
    internal class SceneManager
    {
        public Game1 game1;

        private readonly Stack<IScene> _sceneStack;

        public SceneManager()
        {
            _sceneStack = new();
        }

        public void AddScene(IScene scene)
        {
            scene.Load();
            _sceneStack.Push(scene);
        }

        public void RemoveScene(IScene scene)
        {
            if (_sceneStack.Peek() == scene)
            {
                _sceneStack.Pop();
            }
        }

        public IScene GetCurrentScene()
        {
            //Debug.WriteLine("Current Scene Stack:");
            //foreach (var scene in _sceneStack)
            //{
            //    Debug.WriteLine(scene.ToString()); 
            //}
            return _sceneStack.Peek();
        }

        public void ExitGame()
        {
            game1.Exit();
        }

        public void OpenOptions()
        {
            Debug.WriteLine("open options");
        }

        public void RestartGame()
        {
            Debug.WriteLine("restarting");
            _sceneStack.Clear();
            game1.makeGameScene();
        }

        public void ResumeGame(IScene scene)
        {
            RemoveScene(scene);
        }
    }
}
