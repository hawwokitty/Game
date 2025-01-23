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

        private bool Loop;
        public bool IsAnimationFinished { get; private set; } // To track if the animation has finished

        public Texture2D Texture { get; private set; }

        public AnimationManager(Texture2D texture, int numFrames, int numColumns, Vector2 size, int colPos, int rowPos)
        {
            Texture = texture;
            this.numFrames = numFrames;
            this.numColumns = numColumns;
            this.size = size;

            counter = 0;
            activeFrame = 0;
            interval = 6;

            this.rowPos = rowPos;
            originalRowPos = rowPos;
            this.colPos = colPos;
            Loop = true;
            IsAnimationFinished = false;

        }
        public AnimationManager(Texture2D texture, int numFrames, int numColumns, Vector2 size, int colPos, int rowPos, bool loop)
        {
            Texture = texture;
            this.numFrames = numFrames;
            this.numColumns = numColumns;
            this.size = size;

            counter = 0;
            activeFrame = 0;
            interval = 6;

            this.rowPos = rowPos;
            originalRowPos = rowPos;
            this.colPos = colPos;
            Loop = loop;
            IsAnimationFinished = false;

        }

        public void Update()
        {
            if (IsAnimationFinished) // Stop updating if the animation is finished
                return;

            counter++;
            if (counter > interval)
            {
                counter = 0;
                NextFrame();
            }
        }

        private void NextFrame()
        {
            if (activeFrame < numFrames - 1)
            {
                // Advance the frame
                activeFrame++;
                colPos++;

                if (colPos >= numColumns)
                {
                    colPos = 0;
                    rowPos++;
                }
            }
            else
            {
                // Stop on the last frame
                if (!Loop)
                {
                    // Ensure the animation stops at the last frame
                    activeFrame = numFrames - 1;
                    colPos = (numFrames - 1) % numColumns; // Calculate column for the last frame
                    rowPos = originalRowPos + (numFrames - 1) / numColumns; // Calculate row for the last frame
                }
                else
                {
                    // Looping behavior: Reset to the first frame
                    activeFrame = 0;
                    colPos = 0;
                    rowPos = originalRowPos;
                }
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
