using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

using CPI311.GameEngine;

namespace Assignment03
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Assignment03 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Model model;
        Random random;
        Transform cameraTransform;

        Camera camera;
        Light light;
        BoxCollider boxCollider;
        List<GameObject> gameObjects;
        
        int numCollisions = 0;
        int lastCollisions = 0;
        float animationSpeed = 1.0f;
        bool showText = true;
        bool haveThreadRunning = false;
        float time = 0;
        int framesPerSecond = 0;
        int frames = 0;
        System.Timers.Timer timer;

        public Assignment03()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

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

            random = new Random();
            gameObjects = new List<GameObject>();

            timer = new System.Timers.Timer();
            timer.Interval = 10000;
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;

            haveThreadRunning = true;
            
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

            model = Content.Load<Model>("Sphere");
            font = Content.Load<SpriteFont>("Font");

            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 20;
            camera = new Camera();
            camera.Transform = cameraTransform;

            light = new Light();
            Transform lightTransform = new Transform();
            lightTransform.LocalPosition = Vector3.Backward * 10 + Vector3.Right * 5;
            light.Transform = lightTransform;

            boxCollider = new BoxCollider();
            boxCollider.Size = 10;

            AddGameObject();
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
            Time.Update(gameTime);
            InputManager.Update();

            frames++;

            if (InputManager.IsKeyDown(Keys.Escape))
                Exit();
            if (InputManager.IsKeyPressed(Keys.Up))
                AddGameObject();
            if (InputManager.IsKeyPressed(Keys.Down) && gameObjects.Count > 0)
                gameObjects.RemoveAt(gameObjects.Count - 1);
            if (InputManager.IsKeyDown(Keys.Right))
            {
                foreach (GameObject gameObject in gameObjects)
                    gameObject.Rigidbody.AnimationSpeed += 0.01f;
                animationSpeed += 0.01f;
            }
            if (InputManager.IsKeyDown(Keys.Left))
            {
                foreach (GameObject gameObject in gameObjects)
                    gameObject.Rigidbody.AnimationSpeed -= 0.01f;
                animationSpeed -= 0.01f;
            }
            if (InputManager.IsKeyPressed(Keys.LeftShift))
                showText = !showText;
            if (InputManager.IsKeyPressed(Keys.T)) // press T to toggle multi-threading
                haveThreadRunning = !haveThreadRunning;
            
            ThreadPool.QueueUserWorkItem(new WaitCallback(CollisionReset));

            if (time >= 1.0f)
            {
                time = 0;

                if (!haveThreadRunning)
                {
                    lastCollisions = numCollisions;
                    numCollisions = 0;
                }
            }
            else
                time += Time.ElapsedGameTime;

            foreach (GameObject gameObject in gameObjects)
                gameObject.Update();

            Vector3 normal;
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (boxCollider.Collides(gameObjects[i].Collider, out normal))
                {
                    numCollisions++;
                    if (Vector3.Dot(normal, gameObjects[i].Rigidbody.Velocity) < 0)
                        gameObjects[i].Rigidbody.Impulse += Vector3.Dot(normal, gameObjects[i].Rigidbody.Velocity) * -2 * normal;
                }
                for (int j = i + 1; j < gameObjects.Count; j++)
                {
                    if (gameObjects[i].Collider.Collides(gameObjects[j].Collider, out normal))
                    {
                        numCollisions++;
                        if (Vector3.Dot(normal, gameObjects[i].Rigidbody.Velocity) > 0 && Vector3.Dot(normal, gameObjects[j].Rigidbody.Velocity) < 0)
                            return;

                        Vector3 velocityNormal = Vector3.Dot(normal, gameObjects[i].Rigidbody.Velocity - gameObjects[j].Rigidbody.Velocity)
                                        * -2 * normal * gameObjects[i].Rigidbody.Mass * gameObjects[j].Rigidbody.Mass;
                        gameObjects[i].Rigidbody.Impulse += velocityNormal / 2;
                        gameObjects[j].Rigidbody.Impulse += -velocityNormal / 2;
                    }
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            foreach (GameObject gameObject in gameObjects)
                gameObject.Draw();

            if (showText)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(font, "Animation Speed: " + animationSpeed, Vector2.Zero, Color.Black);
                spriteBatch.DrawString(font, "Number of Spheres: " + gameObjects.Count, new Vector2(0, 20), Color.Black);
                spriteBatch.DrawString(font, "Collisions / Second: " + lastCollisions, new Vector2(0, 40), Color.Black);
                spriteBatch.DrawString(font, "Frames / Second: " + framesPerSecond, new Vector2(0, 60), Color.Black);
                spriteBatch.End();
            }
            
            base.Draw(gameTime);
        }

        private void AddGameObject()
        {
            GameObject gameObject = new GameObject();
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Mass = 0.5f + (float)random.NextDouble();
            rigidbody.Acceleration = Vector3.Down * 9.81f;
            Vector3 direction = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            direction.Normalize();
            rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 5);
            rigidbody.AnimationSpeed = animationSpeed;
            gameObject.Add<Rigidbody>(rigidbody);

            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = gameObject.Rigidbody.Transform.LocalScale.Y;
            sphereCollider.Transform = gameObject.Rigidbody.Transform;
            gameObject.Add<Collider>(sphereCollider);

            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(model, gameObject.Rigidbody.Transform, camera, Content, GraphicsDevice, light, 1, "SimpleShading", 20f, texture);
            gameObject.Add<Renderer>(renderer);

            gameObjects.Add(gameObject);
        }
        
        private void CollisionReset(Object obj)
        {
            while (haveThreadRunning)
            {
                lastCollisions = numCollisions;
                numCollisions = 0;
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            framesPerSecond = frames / 10;
            frames = 0;
        }
    }
}
