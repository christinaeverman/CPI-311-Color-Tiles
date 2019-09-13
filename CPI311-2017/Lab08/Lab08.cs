using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using System;
using System.Collections.Generic;

using CPI311.GameEngine;

namespace Lab08
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab08 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont Font;
        Effect effect;
        Texture2D texture;
        SoundEffect gunSound;
        SoundEffectInstance soundInstance;
        Model cube;
        List<Transform> transforms;
        List<Collider> colliders;
        List<Camera> cameras;
        Camera camera, topDownCamera;

        public Lab08()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            transforms = new List<Transform>();
            colliders = new List<Collider>();
            cameras = new List<Camera>();

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
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(graphics);

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

            effect = Content.Load<Effect>("SimpleShading");
            texture = Content.Load<Texture2D>("Square");
            Font = Content.Load<SpriteFont>("Font");
            gunSound = Content.Load<SoundEffect>("Gun");
            cube = Content.Load<Model>("Sphere");
            (cube.Meshes[0].Effects[0] as BasicEffect).EnableDefaultLighting();
            Transform transform = new Transform();
            SphereCollider collider = new SphereCollider();
            collider.Radius = 1f;
            collider.Transform = transform;
            transforms.Add(transform);
            colliders.Add(collider);

            // *** Lab 8 Item ***********************
            ScreenManager.Setup(true, 1920, 1080);
            //***************************************

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.Backward * 5;
            camera.Position = new Vector2(0f, 0f);
            camera.Size = new Vector2(0.5f, 1f);
            camera.AspectRatio = camera.Viewport.AspectRatio;

            topDownCamera = new Camera();
            topDownCamera.Transform = new Transform();
            topDownCamera.Transform.LocalPosition = Vector3.Up * 10;
            topDownCamera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
            topDownCamera.Position = new Vector2(0.5f, 0f);
            topDownCamera.Size = new Vector2(0.5f, 1f);
            topDownCamera.AspectRatio = topDownCamera.Viewport.AspectRatio;

            cameras.Add(topDownCamera);
            cameras.Add(camera);

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

            Time.Update(gameTime);
            InputManager.Update();

            // ********************** Lab08 Update *********************************
            Ray ray = camera.ScreenPointToWorldRay(InputManager.GetMousePosition());

            foreach (Collider collider in colliders)
            {
                // ***** Rotate the sphere
                collider.Transform.Rotate(Vector3.Up, Time.ElapsedGameTime);
                collider.Transform.Rotate(Vector3.Right, Time.ElapsedGameTime);
                collider.Transform.Rotate(Vector3.Forward, Time.ElapsedGameTime);

                if (collider.Intersects(ray) != null)
                {
                    effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                    (cube.Meshes[0].Effects[0] as BasicEffect).DiffuseColor =
                                                 Color.Blue.ToVector3();

                    if (InputManager.IsMousePressed(0))
                    {
                        soundInstance = gunSound.CreateInstance();
                        soundInstance.IsLooped = false;
                        soundInstance.Volume = 0.2f;
                        soundInstance.Pitch = 0.2f;
                        soundInstance.Play();
                    }
                }
                else
                {
                    effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
                    (cube.Meshes[0].Effects[0] as BasicEffect).DiffuseColor =
                                                 Color.Red.ToVector3();
                }
            }

            // ********************** Lab08 (Sound Test) *****************************
            if (InputManager.IsKeyPressed(Keys.Space))
            {
                soundInstance = gunSound.CreateInstance();
                soundInstance.IsLooped = false;
                soundInstance.Volume = 0.2f;
                soundInstance.Pitch = 0.2f;
                soundInstance.Play();
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

            foreach (Camera camera in cameras)
            {
                GraphicsDevice.DepthStencilState = new DepthStencilState();
                GraphicsDevice.Viewport = camera.Viewport;
                Matrix view = camera.View;
                Matrix projection = camera.Projection;

                effect.CurrentTechnique = effect.Techniques[1];
                effect.Parameters["View"].SetValue(view);
                effect.Parameters["Projection"].SetValue(projection);
                effect.Parameters["LightPosition"].SetValue(Vector3.Backward * 10 + Vector3.Right * 5);
                effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
                effect.Parameters["Shininess"].SetValue(20f);
                effect.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
                effect.Parameters["SpecularColor"].SetValue(new Vector3(0, 0, 0.5f));
                effect.Parameters["DiffuseTexture"].SetValue(texture);

                foreach (Transform transform in transforms)
                {
                    effect.Parameters["World"].SetValue(transform.World);
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        foreach (ModelMesh mesh in cube.Meshes)
                            foreach (ModelMeshPart part in mesh.MeshParts)
                            {
                                GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                                GraphicsDevice.Indices = part.IndexBuffer;
                                GraphicsDevice.DrawIndexedPrimitives(
                                    PrimitiveType.TriangleList, part.VertexOffset, 0,
                                    part.NumVertices, part.StartIndex, part.PrimitiveCount);
                            }
                    }
                }

            }

            base.Draw(gameTime);
        }
    }
}
