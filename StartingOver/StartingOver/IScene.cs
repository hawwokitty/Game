using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StartingOver
{
    internal interface IScene
    {
        public void Load();
        public void Update(GameTime gameTime, GraphicsDeviceManager graphics);
        public void Draw(SpriteBatch spriteBatch);
        public FollowCamera GetCamera();
    }
}
