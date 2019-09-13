using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

using CPI311.GameEngine;

namespace Assignment01
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Assignment01 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Texture2D explorerTexture;
        Texture2D meterBackgroundTexture;
        Texture2D clockTexture;
        Sprite clockSprite;
        AnimatedSprite explorerSprite;
        TimerBar healthBar;
        DistanceBar distanceBar;
        Random random;

        public Assignment01()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            InputManager.Initialize();
            Time.Initialize();

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

            font = Content.Load<SpriteFont>("font");

            explorerTexture = Content.Load<Texture2D>("explorer");
            explorerSprite = new AnimatedSprite(explorerTexture, 32);
            explorerSprite.Position = new Vector2(300, 250);

            meterBackgroundTexture = Content.Load<Texture2D>("meterBackground");
            healthBar = new TimerBar(meterBackgroundTexture, Color.Red, 10, 0.01f);
            healthBar.Position = new Vector2(160, 24);
            distanceBar = new DistanceBar(meterBackgroundTexture, Color.Green, 0, 0.01f);
            distanceBar.Position = new Vector2(368, 24);

            random = new Random();
            clockTexture = Content.Load<Texture2D>("clock");
            clockSprite = new Sprite(clockTexture);
            clockSprite.Position = new Vector2(random.Next(16, 780), random.Next(64, 400));
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (InputManager.IsKeyDown(Keys.Right))
            {
                explorerSprite.Position += new Vector2(50 * Time.ElapsedGameTime, 0);
                distanceBar.DistanceWalked += new Vector2(50 * Time.ElapsedGameTime, 0).Length();
            }
            if (InputManager.IsKeyDown(Keys.Left))
            {
                explorerSprite.Position -= new Vector2(50 * Time.ElapsedGameTime, 0);
                distanceBar.DistanceWalked += new Vector2(50 * Time.ElapsedGameTime, 0).Length();
            }
            if (InputManager.IsKeyDown(Keys.Down))
            {
                explorerSprite.Position += new Vector2(0, 50 * Time.ElapsedGameTime);
                distanceBar.DistanceWalked += new Vector2(0, 50 * Time.ElapsedGameTime).Length();
            }
            if (InputManager.IsKeyDown(Keys.Up))
            {
                explorerSprite.Position -= new Vector2(0, 50 * Time.ElapsedGameTime);
                distanceBar.DistanceWalked += new Vector2(0, 50 * Time.ElapsedGameTime).Length();
            }

            if ((explorerSprite.Position - new Vector2(116, 65) - clockSprite.Position).Length() < 32)
            {
                healthBar.Value += 5;
                clockSprite.Position = new Vector2(random.Next(16, 780), random.Next(64, 400));
            }

            Time.Update(gameTime);
            InputManager.Update();
            explorerSprite.Update();
            healthBar.Update();
            distanceBar.Update();
            clockSprite.Update();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            clockSprite.Draw(spriteBatch);
            explorerSprite.Draw(spriteBatch);
            spriteBatch.DrawString(font, "Time Remaining: ", new Vector2(8, 8), Color.DarkRed);
            spriteBatch.DrawString(font, "Distance Walked: ", new Vector2(208, 8), Color.DarkGreen); 
            healthBar.Draw(spriteBatch);
            distanceBar.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
