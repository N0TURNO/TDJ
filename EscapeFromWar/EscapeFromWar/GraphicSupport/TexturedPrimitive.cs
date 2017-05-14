using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BookExample
{
    /// <summary>
    /// TexturedPrimitive class
    /// </summary>
    public partial class TexturedPrimitive
    {
        // Support for drawing the image
        protected Texture2D mImage;     // The UWB-JPG.jpg image to be loaded
        protected String mImageName;
        protected Vector2 mPosition;    // Center position of image
        protected Vector2 mSize;        // Size of the image to be drawn
        protected float mRotateAngle;   // In Radians, clockwise rotation
        protected String mLabelString;  // String to draw
        protected Color mLabelColor = Color.Black;

        protected void InitPrimitive(String imageName, Vector2 position, Vector2 size, String label = null)
        {
            mImage = Game1.sContent.Load<Texture2D>(imageName);
            mImageName = imageName;
            mPosition = position;
            mSize = size;
            mRotateAngle = 0f;
            mLabelString = label;
            ReadColorData();    // For Pixel-level collision support
        }

        /// <summary>
        /// Constructor of TexturePrimitive
        /// </summary>
        /// <param name="imageName">name of the image to be loaded as texture.</param>
        /// <param name="position">center position of the texture to be drawn</param>
        /// <param name="size">width/height of the texture to be drawn</param>
        public TexturedPrimitive(String imageName, Vector2 position, Vector2 size, String label = null)
        {
            InitPrimitive(imageName, position, size, label);
        }

        public TexturedPrimitive(String imageName)
        {
            InitPrimitive(imageName, Vector2.Zero, new Vector2(1f, 1f));
        }

        // Accessors
        public Vector2 Position { get { return mPosition; } set { mPosition = value; } }
        public float PositionX { get { return mPosition.X; } set { mPosition.X = value; } }
        public float PositionY { get { return mPosition.Y; } set { mPosition.Y = value; } }
        public Vector2 Size { get { return mSize; } set { mSize = value; } }
        public float Width { get { return mSize.X; } set { mSize.X = value; } }
        public float Height { get { return mSize.Y; } set { mSize.Y = value; } }
        public Vector2 MinBound { get { return mPosition - (0.5f * mSize); } }
        public Vector2 MaxBound { get { return mPosition + (0.5f * mSize); } }
        public float RotateAngleInRadian { get { return mRotateAngle; } set { mRotateAngle = value; } }
        public String Label { get { return mLabelString; } set { mLabelString = value; } }
        public Color LabelColor { get { return mLabelColor; } set { mLabelColor = value; } }


        // To support per-pixel collision for sprite
        protected virtual int SpriteTopPixel { get { return 0; } }
        protected virtual int SpriteLeftPixel { get { return 0; } }
        protected virtual int SpriteImageWidth { get { return mImage.Width; } }
        protected virtual int SpriteImageHeight { get { return mImage.Height; } }

        /// <summary>
        /// Allows the primitive object to update its state
        /// </summary>
        /// <param name="deltaTranslate">Amount to change the position of the primitive. 
        ///                              Value of 0 says position is not changed.</param>
        /// <param name="deltaScale">Amount to change of the scale the primitive. 
        ///                          Value of 0 says size is not changed.</param>
        /// <param name="deltaAngleInRadian">Amount to rotate in clock-wise (in Radian) </param>
        public void Update(Vector2 deltaTranslate, Vector2 deltaScale, float deltaAngleInRadian)
        {
            mPosition += deltaTranslate;
            mSize += deltaScale;
            mRotateAngle += deltaAngleInRadian;
        }

        /// <summary>
        /// Check for bound overlaps
        /// </summary>
        /// <param name="otherPrim">Primitive testing for collision</param>
        /// <returns>True: collides</returns>
        public bool PrimitivesTouches(TexturedPrimitive otherPrim)
        {
            if ((Math.Abs(RotateAngleInRadian) < float.Epsilon) &&
                (Math.Abs(otherPrim.RotateAngleInRadian) < float.Epsilon))
            {
                // No rotations involved ...: check for bound overlaps
                Vector2 myMin = MinBound;
                Vector2 otherMin = otherPrim.MinBound;

                Vector2 myMax = MaxBound;
                Vector2 otherMax = otherPrim.MaxBound;

                return
                    ((myMin.X < otherMax.X) && (myMax.X > otherMin.X) &&
                     (myMin.Y < otherMax.Y) && (myMax.Y > otherMin.Y));
            }
            else
            {
                // One of both are rotated ... use radius ... be conservative
                // Use the larger of the Width/Height and approx radius
                // Sqrt(1/2)*x Approx = 0.71f * x;
                float r1 = 0.71f * MathHelper.Max(Size.X, Size.Y);
                float r2 = 0.71f * MathHelper.Max(otherPrim.Size.X, otherPrim.Size.Y);
                return ((otherPrim.Position - Position).Length() < (r1 + r2));
            }
        }

        /// <summary>
        /// Draw the primitive
        /// </summary>
        virtual public void Draw()
        {
            // Define location and size of the texture
            Rectangle destRect = Camera.ComputePixelRectangle(Position, Size);

            // Define the rotation origin
            Vector2 org = new Vector2(mImage.Width / 2, mImage.Height / 2);

            // Draw the texture
            Game1.sSpriteBatch.Draw(mImage, 
                            destRect,           // Area to be drawn in pixel space
                            null,               // 
                            Color.White,        // 
                            mRotateAngle,       // Angle to rotate (clockwise)
                            org,                // Image reference position
                            SpriteEffects.None, 0f);

            if (null != Label)
                FontSupport.PrintStatusAt(Position, Label, LabelColor);
        }
    }
}
