using System;
using Microsoft.Xna.Framework;

namespace BookExample
{
    /// <summary>
    /// 
    /// </summary>
    public class Platform : TexturedPrimitive
    {
        private float mFriction = 0.98f;    // slows down by 2% at each update
        private float mElasticity = 0.7f;   // retains 70% of velocity at each bounce

        /// <summary>
        /// Constructor 
        /// </summary>
        public Platform(String image, Vector2 center, Vector2 size)
                : base(image, center, size)
        {
        }

        public float Friction { get { return mFriction; } set { mFriction = value; } }
        public float Elasticity { get { return mElasticity; } set { mElasticity = value; } }

        virtual public void BounceObject(GameObject obj)
        {
            Vector2 collidePoint;
            if (obj.PixelTouches(this, out collidePoint))
            {
                #region Step 2a.
                // limitation: only collide from top/bottom not from the sides
                Vector2 v = obj.Velocity;
                v.Y *= -0.25f * mElasticity;
                v.X *= mFriction;
                obj.Velocity = v;
                #endregion

                #region Step 2b.
                // make sure object is not "stuck" inside the platform
                Vector2 p = obj.Position;
                if (p.Y > Position.Y)
                    p.Y = Position.Y + Size.Y * 0.5f + obj.Size.Y * 0.5f;
                else
                    p.Y = Position.Y - Size.Y * 0.5f - obj.Size.Y * 0.5f;
                obj.Position = p;
                #endregion
            }
        }
    }
}
