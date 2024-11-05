using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StartingOver
{
    public class FollowCamera
    {
        public float Zoom { get; private set; }
        public float Rotation { get; private set; }
        private GraphicsDeviceManager graphics; 

        public Matrix ViewMatrix
        {
            get
            {
                return
                    Matrix.Identity *
                    //Position matrix
                    Matrix.CreateTranslation(new Vector3(-new Vector2((int)Math.Floor(Position.X), (int)Math.Floor(Position.Y)), 0)) *
                    //Rotation Matrix
                    Matrix.CreateRotationZ(Rotation) *
                    //Zoom Matrix
                    Matrix.CreateScale((int)Zoom, (int)Zoom, 1) *
                    //World Origin matix
                    Matrix.CreateTranslation(new Vector3(new Vector2((int)Math.Floor(Origin.X), (int)Math.Floor(Origin.Y)), 0));
            }
        }

        public Vector2 Position { get; private set; }
        private Vector2 oldPosition;
        public Vector2 Origin { get; private set; }

        public Viewport Viewport { get; private set; }
        public Rectangle? Limits { get; set; }

        public FollowCamera(GraphicsDeviceManager graphics, Vector2 pos, float zoom = 1f, float rotation = 0)
        {
            this.graphics = graphics; 
            this.Zoom = zoom;
            this.Rotation = rotation;

            Viewport = new Viewport
            {
                Width = graphics.PreferredBackBufferWidth,
                Height = graphics.PreferredBackBufferHeight 
            };

            Origin = new Vector2(Viewport.Width / 2, Viewport.Height / 2);
            Position = Origin;

            ValidatePosition();
        }

        public void Update()
        {
            oldPosition = Position;
        }

        public void Move(int x = 0, int y = 0)
        {
            if (x == 0 && y == 0)
                return;

            Vector2 newPos = new Vector2(Position.X + x, Position.Y + y);
            SetPosition(newPos, false);
        }

        public void LookAt(Vector2 position)
        {
            var rounded = position.ToPoint();
            Position = rounded.ToVector2();
        }

        public bool HasMoved()
        {
            return Position != oldPosition;
        }

        public void Approach(Vector2 position, float ease)
        {
            Position += (position - Position) * ease;
            ValidatePosition();
        }

        public void SetPosition(Vector2 position, bool validate = true)
        {
            Position = position;

            if (validate)
            {
                ValidatePosition();
            }
        }

        public void SetLimit(Rectangle? rect)
        {
            Limits = rect;
            ValidatePosition();
        }


        public void ValidatePosition()
        {
            // Calculate the camera's view size based on the zoom level
            Vector2 cameraSize = new Vector2(Viewport.Width, Viewport.Height) / Zoom;

            // Calculate the map boundaries
            if (Limits.HasValue)
            {
                // Calculate the half size of the camera view
                Vector2 halfCameraSize = cameraSize / 2;

                // Get the map boundaries
                float leftLimit = Limits.Value.Left + halfCameraSize.X;
                float rightLimit = Limits.Value.Right - halfCameraSize.X;
                float topLimit = Limits.Value.Top + halfCameraSize.Y;
                float bottomLimit = Limits.Value.Bottom - halfCameraSize.Y;

                // Create a new Vector2 with the clamped values
                Position = new Vector2(
                    MathHelper.Clamp(Position.X, leftLimit, rightLimit),
                    MathHelper.Clamp(Position.Y, topLimit, bottomLimit)
                );
            }
        }



        public void SetOrigin(Vector2 point)
        {
            Origin = point;
        }

        public void SetRotation(float angle)
        {
            Rotation = angle;
        }
    }

}
