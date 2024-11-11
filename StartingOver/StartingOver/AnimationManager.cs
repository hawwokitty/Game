using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace StartingOver
{
    internal class AnimationManager
    {
        int numFrames;
        int numColumns;
        Vector2 size;

        public int counter;
        public int activeFrame;
        int interval;

        int rowPos;
        int colPos;

        int originalRowPos;

        public Texture2D Texture { get; private set; }

        public AnimationManager(Texture2D texture, int numFrames, int numColumns, Vector2 size, int colPos, int rowPos)
        {
            Texture = texture;
            this.numFrames = numFrames;
            this.numColumns = numColumns;
            this.size = size;

            counter = 0;
            activeFrame = 0;
            interval = 10;

            this.rowPos = rowPos;
            originalRowPos = rowPos;
            this.colPos = colPos;

        }

        public void Update()
        {
            counter++;
            if (counter > interval)
            {
                counter = 0;
                NextFrame();
            }
        }

        private void NextFrame()
        {
            activeFrame++;
            colPos++;
            if (activeFrame >= numFrames)
            {
                activeFrame = 0;
                rowPos = originalRowPos;
                colPos = 0;
            }
            if (colPos >= numColumns)
            {
                colPos = 0;
                rowPos++;
            }
        }

        public Rectangle GetFrame()
        {
            return new Rectangle(
                colPos * (int)size.X, 
                rowPos * (int)size.Y, 
                (int)size.X, 
                (int)size.Y);
        }
    }
}
