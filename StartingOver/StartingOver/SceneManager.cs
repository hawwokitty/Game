using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingOver
{
    internal class SceneManager
    {
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
    }
}
