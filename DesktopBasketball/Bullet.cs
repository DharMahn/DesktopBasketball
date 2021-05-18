using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace DesktopBasketball
{
    class Bullet
    {
        public Rectangle map;

        public Vector2 Center;
        public Vector2 location;
        public Vector2 velocity;
        public float radius;
        public Texture2D texture;

        public bool physics = true;

        public float mass;

        public Vector2 gravity;

        public Color c;

        public bool doShrink;

        public bool isKosar = false;

        public bool onGround = false;

        Random r1 = new Random();

        public Bullet(Vector2 _location, Vector2 _velocity, Rectangle _map, int _radius, Texture2D _texture)
        {
            location = _location;
            velocity = _velocity;
            map = _map;
            radius = _radius;
            texture = _texture;
            gravity = new Vector2(0f, 0.5f);
            c = new Color(r1.Next(256), r1.Next(256), r1.Next(256));
            doShrink = false;
            mass = radius * radius * (float)Math.PI;
        }
        public Bullet(Vector2 _location, int _radius)
        {
            location = _location;
            radius = _radius;
            doShrink = false;
            mass = 100000000;
            isKosar = true;
            Center.X = location.X + radius;
            Center.Y = location.Y + radius;
        }

        public void update()
        {
            
            if (!isKosar)
            {
                velocity += gravity;
                location += velocity;
                Center.X = location.X + radius;
                Center.Y = location.Y + radius;

                //BOUNCEEEEE
                if (location.Y > map.Height - (radius * 2))
                {
                    velocity.Y *= -.80f;
                    location.Y = map.Height - (radius * 2);
                    
                }
                if (location.X > map.Width - (radius * 2))
                {
                    velocity.X *= -.85f;

                    location.X = map.Width - (radius * 2);

                }
                else if (location.X < map.X)
                {
                    velocity.X *= -.85f;
                    location.X = map.X;
                }
                velocity.X *= .99f;

                if (doShrink)
                {
                    if (radius > 0)
                    {
                        radius -= 0.5f;
                        mass = radius * radius * (float)Math.PI;
                    }
                }
                float temp = Math.Abs(velocity.X * velocity.X + velocity.Y * velocity.Y);
                Console.WriteLine(temp.ToString());
                if (temp < 3f && location.Y > map.Height - (radius * 2) - (radius * 2))
                {
                    
                    onGround = true;
                }
                else
                {
                    onGround = false;
                }
                if (temp < 0.2)
                {
                    velocity = Vector2.Zero;
                }
                Console.WriteLine(onGround.ToString());
            }
        }
    }
}
