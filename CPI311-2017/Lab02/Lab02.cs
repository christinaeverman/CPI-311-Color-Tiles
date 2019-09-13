using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;

namespace Lab02
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab02 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpiralMover sprite;

        public Lab02()
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

            Texture2D temp = Content.Load<Texture2D>("Square");
            sprite = new SpiralMover(temp, new Vector2(300, 300), 50);
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
                sprite.Radius++;
            if (InputManager.IsKeyDown(Keys.Left))
                sprite.Radius--;
            if (InputManager.IsKeyDown(Keys.Up))
                sprite.Speed += 0.05f;
            if (InputManager.IsKeyDown(Keys.Down))
                sprite.Speed -= 0.05f;

            Time.Update(gameTime);
            InputManager.Update();
            sprite.Update();
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
            sprite.Draw(spriteBatch); // CPI311.GameEngine's Sprite
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
