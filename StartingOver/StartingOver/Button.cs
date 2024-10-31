using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StartingOver
{
    internal class Button : Sprite
    {
        public Button(Texture2D texture, Vector2 position, int height, int width) : base(texture, position, height, width)
        {
        }

        public override void Update(KeyboardState keystate)
        {
            base.Update(keystate);
        }

        public override void Draw(SpriteBatch spriteBatch, AnimationManager am)
        {
            base.Draw(spriteBatch, am);
        }
    }
}
