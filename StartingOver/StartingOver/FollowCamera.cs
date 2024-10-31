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
        public Rectangle ViewArea { get; private set; }

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

            ValidateZoom();
            ValidatePosition();
        }

        public void Update()
        {
            ViewArea = new Rectangle((Position - Origin).ToPoint(), new Point((int)(Viewport.Width), (int)(Viewport.Height)));
            oldPosition = Position;
        }

        public void Move(int x = 0, int y = 0)
        {
            if (x == 0 && y == 0)
                return;

            Vector2 newPos = new Vector2(Position.X + x, Position.Y + y);
            SetPosition(newPos, false);
        }

        public void SetZoom(int zoom)
        {
            this.Zoom = zoom;
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
            ValidateZoom();
            ValidatePosition();
        }

        public void SetPosition(Vector2 position, bool validate = true)
        {
            Position = position;

            if (validate)
            {
                ValidateZoom();
                ValidatePosition();
            }
        }

        public void SetLimit(Rectangle? rect)
        {
            Limits = rect;
            ValidateZoom();
            ValidatePosition();
        }

        public void ValidateZoom()
        {
            if (Limits.HasValue)
            {
                float minZoomX = (float)Viewport.Width / Limits.Value.Width;
                float minZoomY = (float)Viewport.Height / Limits.Value.Height;
                Zoom = MathHelper.Max(Zoom, MathHelper.Max(minZoomX, minZoomY));
            }
        }

        public void ValidatePosition()
        {
            Vector2 cameraWorldMin = Vector2.Transform(Vector2.Zero, Matrix.Invert(ViewMatrix)); // Removed Engine.ScreenMatrix for simplicity
            Vector2 cameraSize = new Vector2(Viewport.Width, Viewport.Height) / Zoom;
            Vector2 positionOffset = Position - cameraWorldMin;

            if (Limits.HasValue)
            {
                Origin = new Vector2(Limits.Value.Width / 2, Limits.Value.Height / 2);
                Vector2 limitWorldMin = new Vector2(Limits.Value.Left, Limits.Value.Top);
                Vector2 limitWorldMax = new Vector2(Limits.Value.Right, Limits.Value.Bottom);
                Position = Vector2.Clamp(cameraWorldMin, limitWorldMin, limitWorldMax - cameraSize) + positionOffset;
            }
            else
            {
                Position = Vector2.Clamp(cameraWorldMin, Vector2.Zero, new Vector2(Viewport.Width, Viewport.Height) - cameraSize) + positionOffset;
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
