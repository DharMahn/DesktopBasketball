using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DrawBehindDesktopIcons;
namespace DesktopBasketball
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        SpriteFont sf1;
        int pts = 0;

        Random r = new Random();
        Texture2D ballTexture;

        MouseState lastMouseState = new MouseState();

        Bullet ball;
        Bullet[] kosar = new Bullet[2];

        Rectangle checker1, checker2;
        bool checker1checked, checker2checked;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Application.EnableVisualStyles();
            Form gameForm = (Form)Control.FromHandle(Window.Handle);
            gameForm.Location = new System.Drawing.Point(0, 0);
            gameForm.FormBorderStyle = FormBorderStyle.None;
            
            graphics.PreferredBackBufferWidth = Screen.PrimaryScreen.Bounds.Width;
            graphics.PreferredBackBufferHeight = Screen.PrimaryScreen.Bounds.Height;
            graphics.ApplyChanges();
            IntPtr progman = W32.FindWindow("Progman", null);

            IntPtr result = IntPtr.Zero;

            W32.SendMessageTimeout(progman, 0x052C, new IntPtr(0), IntPtr.Zero, W32.SendMessageTimeoutFlags.SMTO_NORMAL, 1000, out result);
            IntPtr workerw = IntPtr.Zero;

            W32.EnumWindows(new W32.EnumWindowsProc((tophandle, topparamhandle) =>
            {
                IntPtr p = W32.FindWindowEx(tophandle, IntPtr.Zero, "SHELLDLL_DefView", IntPtr.Zero);
                if (p != IntPtr.Zero)
                {
                    // Gets the WorkerW Window after the current one.
                    workerw = W32.FindWindowEx(IntPtr.Zero, tophandle, "WorkerW", IntPtr.Zero);
                }
                return true;
            }), IntPtr.Zero);

            W32.SetParent(Window.Handle, workerw);

            ball = new Bullet(new Vector2(40, Screen.PrimaryScreen.Bounds.Height - 10), Vector2.Zero, new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height), 20, Content.Load<Texture2D>("circle"));

            ball.c = Color.Black;
            kosar[0] = new Bullet(new Vector2(Screen.PrimaryScreen.Bounds.Width - 5 - 150, Screen.PrimaryScreen.Bounds.Height / 2.5f), 10);
            kosar[1] = new Bullet(new Vector2(Screen.PrimaryScreen.Bounds.Width - 5, Screen.PrimaryScreen.Bounds.Height / 2.5f), 10);
            checker1 = new Rectangle((int)kosar[0].location.X, -512, (int)kosar[0].radius * 2, (int)kosar[0].location.Y + 512);
            checker2 = new Rectangle((int)kosar[0].Center.X + (int)kosar[0].radius, (int)kosar[0].location.Y, 230, 40);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sf1 = Content.Load<SpriteFont>("Score");
            ballTexture = Content.Load<Texture2D>("circle");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();
            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (currentMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && lastMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && ball.onGround) //Will be true only if the user is currently clicking, but wasn't on the previous call.
            {
                checker1checked = false;
                checker2checked = false;
                Vector2 vector = new Vector2(ball.Center.X - currentMouseState.X, ball.Center.Y - currentMouseState.Y);
                vector.Normalize();
                vector *= -35f;
                ball.velocity = vector;
            }
            if (currentMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                ball.location = new Vector2(Cursor.Position.X - ball.radius, Cursor.Position.Y - ball.radius);
                ball.velocity = Vector2.Zero;
            }

            for (int i = 0; i < 2; i++)
            {
                if (doCirclesOverlap(ball.Center.X, ball.Center.Y, ball.radius, kosar[i].Center.X, kosar[i].Center.Y, kosar[i].radius))
                {
                    float fDistance = (float)Math.Sqrt((ball.Center.X - kosar[i].Center.X) * (ball.Center.X - kosar[i].Center.X) + (ball.Center.Y - kosar[i].Center.Y) * (ball.Center.Y - kosar[i].Center.Y));
                    float fOverlap = 0.5f * (fDistance - ball.radius - kosar[i].radius);
                    //static
                    ball.location.X -= fOverlap * (ball.Center.X - kosar[i].Center.X) / fDistance;
                    ball.location.Y -= fOverlap * (ball.Center.Y - kosar[i].Center.Y) / fDistance;


                    //dynamic
                    fDistance = (float)Math.Sqrt((ball.Center.X - kosar[i].Center.X) * (ball.Center.X - kosar[i].Center.X) + (ball.Center.Y - kosar[i].Center.Y) * (ball.Center.Y - kosar[i].Center.Y));

                    float nx = (kosar[i].Center.X - ball.Center.X) / fDistance;
                    float ny = (kosar[i].Center.Y - ball.Center.Y) / fDistance;

                    float tx = -ny;
                    float ty = nx;

                    float dpTan1 = ball.velocity.X * tx + ball.velocity.Y * ty;


                    float dpNorm1 = ball.velocity.X * nx + ball.velocity.Y * ny;
                    float dpNorm2 = kosar[i].velocity.X * nx + kosar[i].velocity.Y * ny;

                    float m1 = (dpNorm1 * (ball.mass - kosar[i].mass) + 2.0f * kosar[i].mass * dpNorm2) / (ball.mass + kosar[i].mass);


                    ball.velocity.X = tx * dpTan1 + nx * m1 * 0.75f;
                    ball.velocity.Y = ty * dpTan1 + ny * m1 * 0.75f;
                }
            }
            ball.update();
            if (!checker1checked)
            {
                if (new Rectangle(checker1.X, checker1.Y, checker1.Width, checker1.Height).Contains(new Point((int)ball.Center.X, (int)ball.Center.Y)))
                {
                    if (!checker2checked)
                    {
                        checker1checked = true;
                    }
                }
            }
            if (!checker2checked)
            {
                if (new Rectangle(checker2.X, checker2.Y, checker2.Width, checker2.Height).Contains(new Point((int)ball.Center.X, (int)ball.Center.Y)))
                {
                    checker2checked = true;
                    if (checker1checked)
                    {
                        pts++;
                        checker1checked = false;
                    }
                }
            }
            lastMouseState = currentMouseState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);
            Rectangle ballRekt = new Rectangle((int)ball.location.X, (int)ball.location.Y, (int)ball.radius * 2, (int)ball.radius * 2);

            spriteBatch.Draw(ballTexture, ballRekt, Color.Black);
            foreach (var item in kosar)
            {
                spriteBatch.Draw(ballTexture, new Rectangle((int)item.location.X, (int)item.location.Y, (int)(item.radius) * 2, (int)(item.radius) * 2), Color.Black);
            }

            //Primitives2D.DrawCircle(spriteBatch, ball.Center.X, ball.Center.Y, ball.radius, 64, Color.Black);
            //Primitives2D.DrawCircle(spriteBatch, kosar[0].Center.X, kosar[0].Center.Y, kosar[0].radius, 64, Color.Black);
            //Primitives2D.DrawCircle(spriteBatch, kosar[1].Center.X, kosar[1].Center.Y, kosar[1].radius, 64, Color.Black);
            //Rectangle asd = new Rectangle((int)kosar[0].location.X, 0, (int)kosar[1].Center.X + (int)kosar[1].radius-(int)kosar[0].Center.X, (int)kosar[1].Center.Y + 53 + (int)kosar[1].radius);
            //Primitives2D.DrawRectangle(spriteBatch, asd, Color.Blue);
            //spriteBatch.Draw(palank, asd,Color.White);
            //spriteBatch.Draw(palank, new Rectangle(0,0,400,400), Color.White);
            //Primitives2D.DrawRectangle(spriteBatch, checker2, Color.Black);
            spriteBatch.DrawString(sf1, "Score: " + pts, new Vector2(0, 0), Color.Black);
            spriteBatch.DrawString(sf1, checker1checked.ToString(), new Vector2(0, 20), Color.Black);
            spriteBatch.DrawString(sf1, checker2checked.ToString(), new Vector2(0, 40), Color.Black);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        bool doCirclesOverlap(float x1, float y1, float r1, float x2, float y2, float r2)
        {
            return Math.Abs((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)) <= (r1 + r2) * (r1 + r2);
        }
    }
}
