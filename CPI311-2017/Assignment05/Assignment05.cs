using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

using CPI311.GameEngine;

namespace Assignment05
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Assignment05 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera camera;
        Light light;

        // Audio Components
        SoundEffect gunSound;
        SoundEffectInstance soundInstance;

        // Visual Components
        Ship ship;
        Bullet[] bulletList = new Bullet[GameConstants.NumBullets];
        Asteroid[] asteroidList = new Asteroid[GameConstants.NumAsteroids];
        Random random = new Random();

        // Score & background
        int score;
        Texture2D stars;
        SpriteFont Font;
        Vector2 scorePosition = new Vector2(100, 50);

        // Particles
        ParticleManager particleManager;
        Texture2D particleTex;
        Effect particleEffect;

        int hitCount;

        // **************** Scenes inner-class ********************
        class Scene
        {
            public delegate void CallMethod(GameTime gameTime);
            public CallMethod Update;
            public CallMethod Draw;
            public Scene(CallMethod update, CallMethod draw)
            {
                Update = update;
                Draw = draw;
            }
        }
        Dictionary<String, Scene> scenes;
        Scene currentScene;
        List<GUIElement> guiElements;

        public Assignment05()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            guiElements = new List<GUIElement>();
            scenes = new Dictionary<String, Scene>();

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(graphics);

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.Position += Vector3.Backward * 50;
            light = new Light();
            //light.Transform = new CPI311.GameEngine.Transform();

            score = 0;
            hitCount = 0;

            scenes.Add("Menu", new Scene(MainMenuUpdate, MainMenuDraw));
            scenes.Add("Play", new Scene(PlayUpdate, PlayDraw));
            scenes.Add("GameOver", new Scene(GameOverUpdate, GameOverDraw));

            currentScene = scenes["Menu"];
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

            ship = new Ship(Content, camera, GraphicsDevice, light);
            ship.Transform.Position += Vector3.Backward * 15;
            for (int i = 0; i < GameConstants.NumBullets; i++)
                bulletList[i] = new Bullet(Content, camera, GraphicsDevice, light);
            ResetAsteroids();

            // *** Particle
            particleManager = new ParticleManager(GraphicsDevice, 100);
            particleEffect = Content.Load<Effect>("Content/Content/ParticleShader-complete");
            particleTex = Content.Load<Texture2D>("Content/Content/Textures/fire");
            
            gunSound = Content.Load<SoundEffect>("Gun");
            stars = Content.Load<Texture2D>("Content/Content/Textures/B1_stars");
            Font = Content.Load<SpriteFont>("Font");

            Button playButton = new Button();
            playButton.Texture = Content.Load<Texture2D>("Square");
            playButton.Text = "   Play";
            playButton.Bounds = new Rectangle(50, 50, 300, 20);
            playButton.Action += PlayGame;
            guiElements.Add(playButton);
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
        void PlayUpdate(GameTime gameTime)
        {
            if (InputManager.IsKeyPressed(Keys.Escape))
                currentScene = scenes["Menu"];

            Time.Update(gameTime);
            InputManager.Update();
            ship.Update();
            for (int i = 0; i < GameConstants.NumBullets; i++)
                bulletList[i].Update();
            for (int i = 0; i < GameConstants.NumAsteroids; i++)
                asteroidList[i].Update();

            // shoot bullet with mouse click
            if (/*InputManager.IsKeyDown(Keys.Space)*/InputManager.IsMousePressed(0))
            {
                for (int i = 0; i < GameConstants.NumBullets; i++)
                {
                    if (!bulletList[i].isActive)
                    {
                        bulletList[i].Rigidbody.Velocity =
                        (ship.Transform.Forward) * GameConstants.BulletSpeedAdjustment;
                        bulletList[i].Transform.LocalPosition = ship.Transform.Position;
                        bulletList[i].isActive = true;
                        score -= GameConstants.ShotPenalty;
                        // sound
                        soundInstance = gunSound.CreateInstance();
                        soundInstance.Play();
                        break; //exit the loop
                    }
                }
            }

            // check for collision between bullet and asteroid
            Vector3 normal;
            for (int i = 0; i < asteroidList.Length; i++)
                if (asteroidList[i].isActive)
                    for (int j = 0; j < bulletList.Length; j++)
                        if (bulletList[j].isActive)
                            if (asteroidList[i].Collider.Collides(bulletList[j].Collider, out normal))
                            {
                                hitCount++;
                                // Particles
                                Particle particle = particleManager.getNext();
                                particle.Position = asteroidList[i].Transform.Position;
                                particle.Velocity = new Vector3(
                                    random.Next(-5, 5), 2, random.Next(-50, 50));
                                particle.Acceleration = new Vector3(0, 3, 0);
                                particle.MaxAge = random.Next(1, 6);
                                particle.Init();
                                asteroidList[i].isActive = false;
                                bulletList[j].isActive = false;
                                score += GameConstants.KillBonus;
                                break; //no need to check other bullets
                            }

            // particles update
            particleManager.Update();

            // check for collision between ship and asteroid
            for (int i = 0; i < asteroidList.Length; i++)
            {
                if (asteroidList[i].isActive)
                {
                    if (asteroidList[i].Collider.Collides(ship.Collider, out normal))
                    {
                        // Particles
                        Particle particle = particleManager.getNext();
                        particle.Position = asteroidList[i].Transform.Position;
                        particle.Velocity = new Vector3(
                            random.Next(-5, 5), 2, random.Next(-50, 50));
                        particle.Acceleration = new Vector3(0, 3, 0);
                        particle.MaxAge = random.Next(1, 6);
                        particle.Init();
                        ship.isActive = false;
                        asteroidList[i].isActive = false;
                        break; //no need to check other bullets
                    }
                }
            }

            if (hitCount >= 3)
                currentScene = scenes["GameOver"];

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        void PlayDraw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            spriteBatch.Begin();
            spriteBatch.Draw(stars, new Rectangle(0, 0, 800, 600), Color.White);
            spriteBatch.DrawString(Font, "Score: " + score, scorePosition, Color.White);
            spriteBatch.End();
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // ship, bullets, and asteroids
            ship.Draw();
            for (int i = 0; i < GameConstants.NumBullets; i++) bulletList[i].Draw();
            for (int i = 0; i < GameConstants.NumAsteroids; i++) asteroidList[i].Draw();
            //particle draw
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            particleEffect.CurrentTechnique = particleEffect.Techniques["particle"];
            particleEffect.CurrentTechnique.Passes[0].Apply();
            particleEffect.Parameters["ViewProj"].SetValue(camera.View * camera.Projection);
            particleEffect.Parameters["World"].SetValue(Matrix.Identity);            particleEffect.Parameters["CamIRot"].SetValue(
                Matrix.Invert(Matrix.CreateFromQuaternion(camera.Transform.Rotation)));
            particleEffect.Parameters["Texture"].SetValue(particleTex);
            particleManager.Draw(GraphicsDevice);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            base.Draw(gameTime);
        }

        private void ResetAsteroids()
        {
            float xStart;
            float yStart;
            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                if (random.Next(2) == 0)
                    xStart = (float)-GameConstants.PlayfieldSizeX;
                else
                    xStart = (float)GameConstants.PlayfieldSizeX;
                yStart = (float)random.NextDouble() * GameConstants.PlayfieldSizeY;
                asteroidList[i] = new Asteroid(Content, camera, GraphicsDevice, light);

                asteroidList[i].Transform.Position = new Vector3(random.Next(-50, 50),
                    random.Next(-40, 40), -5);
                asteroidList[i].Rigidbody.Velocity = new Vector3(
                random.Next((int)(-GameConstants.AsteroidMaxSpeed), (int)GameConstants.AsteroidMaxSpeed) * 0.5f,
                    random.Next((int)(-GameConstants.AsteroidMaxSpeed), (int)GameConstants.AsteroidMaxSpeed) * 0.5f,
                    random.Next((int)(-GameConstants.AsteroidMaxSpeed), (int)GameConstants.AsteroidMaxSpeed) * 0.5f);
                asteroidList[i].isActive = true;
            }
        }

        void MainMenuUpdate(GameTime gameTime)
        {
            foreach (GUIElement element in guiElements)
                element.Update();
        }

        void MainMenuDraw(GameTime gameTime)
        {
            spriteBatch.Begin();
            //spriteBatch.Draw(menuPage, new Rectangle(0, 0, 800, 600), Color.White);
            spriteBatch.DrawString(Font, "Menu Page", Vector2.Zero, Color.Black);
            foreach (GUIElement element in guiElements)
                element.Draw(spriteBatch, Font);
            spriteBatch.End();
        }

        void GameOverUpdate(GameTime gameTime)
        {
            if (InputManager.IsKeyPressed(Keys.Escape))
                currentScene = scenes["Menu"];
        }

        void GameOverDraw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(Font, "Game Over Page", Vector2.Zero, Color.Black);
            foreach (GUIElement element in guiElements)
                element.Draw(spriteBatch, Font);
            spriteBatch.End();
        }

        void PlayGame(GUIElement element)
        {
            currentScene = scenes["Play"];
        }

        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            //    Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();
            InputManager.Update();
            Time.Update(gameTime);
            currentScene.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = new DepthStencilState();
            currentScene.Draw(gameTime);
            base.Draw(gameTime);

        }
    }
}