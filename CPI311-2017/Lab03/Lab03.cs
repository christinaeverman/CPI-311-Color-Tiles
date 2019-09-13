using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;

namespace Lab03
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab03 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        Model torusModel;
        Matrix torusWorld;
        Matrix view;
        Matrix projection;

        Vector3 torusPosition;
        float torusScale;
        Vector3 torusRotation;
        Vector3 cameraPosition;
        bool isPerspective;
        bool isSRT;
        Vector2 cameraCenter;
        Vector2 cameraSize;

        public Lab03()
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
            torusModel = Content.Load<Model>("Torus");
            font = Content.Load<SpriteFont>("Font");

            torusPosition = Vector3.Zero;
            torusScale = 1.0f;
            torusRotation = Vector3.Zero;
            cameraPosition = Vector3.Backward * 5;
            isPerspective = true;
            isSRT = true;
            cameraSize = Vector2.One;
            cameraCenter = Vector2.Zero;
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            // toggle between Orthographic and Perspective modes
            if (InputManager.IsKeyPressed(Keys.Tab))
                isPerspective = !isPerspective;

            // toggle between SRT and TRS modes
            if (InputManager.IsKeyPressed(Keys.Space))
                isSRT = !isSRT;

            // Camera Transformation
            if (!InputManager.IsKeyDown(Keys.LeftControl) && !InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.D))
                cameraPosition += Vector3.Right * Time.ElapsedGameTime;
            if (!InputManager.IsKeyDown(Keys.LeftControl) && !InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.A))
                cameraPosition += Vector3.Left * Time.ElapsedGameTime;
            if (!InputManager.IsKeyDown(Keys.LeftControl) && !InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.W))
                cameraPosition += Vector3.Up * Time.ElapsedGameTime;
            if (!InputManager.IsKeyDown(Keys.LeftControl) && !InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.S))
                cameraPosition += Vector3.Down * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.D))
                cameraCenter += new Vector2(0.01f, 0);
            if (InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.A))
                cameraCenter -= new Vector2(0.01f, 0);
            if (InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.W))
                cameraCenter += new Vector2(0, 0.01f);
            if (InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.S))
                cameraCenter -= new Vector2(0, 0.01f);
            if (InputManager.IsKeyDown(Keys.LeftControl) && InputManager.IsKeyDown(Keys.D))
                cameraSize += new Vector2(0.01f, 0);
            if (InputManager.IsKeyDown(Keys.LeftControl) && InputManager.IsKeyDown(Keys.A))
                cameraSize -= new Vector2(0.01f, 0);
            if (InputManager.IsKeyDown(Keys.LeftControl) && InputManager.IsKeyDown(Keys.W))
                cameraSize += new Vector2(0, 0.01f);
            if (InputManager.IsKeyDown(Keys.LeftControl) && InputManager.IsKeyDown(Keys.S))
                cameraSize -= new Vector2(0, 0.01f);

            // Torus Transformation
            if (InputManager.IsKeyDown(Keys.Right))
                torusPosition += Vector3.Right * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Left))
                torusPosition += Vector3.Left * Time.ElapsedGameTime;
            if (!InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.Up))
                torusPosition += Vector3.Forward * Time.ElapsedGameTime;
            if (!InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.Down))
                torusPosition += Vector3.Backward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.Up))
                torusScale += 0.1f * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.Down))
                torusScale -= 0.1f * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Insert))
                torusRotation += new Vector3(1, 0, 0) * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Delete))
                torusRotation -= new Vector3(1, 0, 0) * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Home))
                torusRotation += new Vector3(0, 1, 0) * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.End))
                torusRotation -= new Vector3(0, 1, 0) * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.PageUp))
                torusRotation += new Vector3(0, 0, 1) * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.PageDown))
                torusRotation -= new Vector3(0, 0, 1) * Time.ElapsedGameTime;

            if (isSRT)
                torusWorld = Matrix.CreateScale(torusScale) *
                    Matrix.CreateFromYawPitchRoll(torusRotation.X, torusRotation.Y, torusRotation.Z) *
                    Matrix.CreateTranslation(torusPosition);
            else
                torusWorld = Matrix.CreateTranslation(torusPosition) *
                    Matrix.CreateFromYawPitchRoll(torusRotation.X, torusRotation.Y, torusRotation.Z) *
                    Matrix.CreateScale(torusScale);

            view = Matrix.CreateLookAt(cameraPosition, 
                cameraPosition + Vector3.Forward, 
                Vector3.Up);

            Vector2 topLeft = cameraCenter - cameraSize;
            Vector2 bottomRight = cameraCenter + cameraSize;

            if (isPerspective)
                projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2,
                    GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000f);
            else
                projection = Matrix.CreateOrthographicOffCenter(topLeft.X * 10, bottomRight.X * 10, topLeft.Y * 10, bottomRight.Y * 10, 1, 10f);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            torusModel.Draw(torusWorld, view, projection);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Position: (" + 
                torusPosition.X.ToString("0.00") + ", " + 
                torusPosition.Y.ToString("0.00") + ", " + 
                torusPosition.Z.ToString("0.00") + ")", 
                new Vector2(0, 0), Color.Black);
            spriteBatch.DrawString(font, "Rotation: (" +
                torusRotation.X.ToString("0.00") + ", " +
                torusRotation.Y.ToString("0.00") + ", " +
                torusRotation.Z.ToString("0.00") + ")",
                new Vector2(0, 25), Color.Black);
            spriteBatch.DrawString(font, "Scale:" +
                torusScale.ToString("0.00"),
                new Vector2(0, 50), Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}