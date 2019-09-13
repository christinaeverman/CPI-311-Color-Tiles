using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Windows.Input;
using System.Diagnostics;

using CPI311.GameEngine;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Assignment02 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model sunModel;
        Model mercuryModel;
        Model earthModel;
        Model moonModel;
        Model thirdPersonModel;
        Model planeModel;
        Transform sunTransform;
        Transform mercuryTransform;
        Transform earthTransform;
        Transform moonTransform;
        Transform thirdPersonTransform;
        Camera camera;
        Transform cameraTransform;
        Transform planeTransform;
        bool isFirstPerson;
        float speed;

        public Assignment02()
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

            isFirstPerson = true;
            speed = 1.0f;

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

            sunModel = Content.Load<Model>("Sphere");
            mercuryModel = Content.Load<Model>("Sphere");
            earthModel = Content.Load<Model>("Sphere");
            moonModel = Content.Load<Model>("Sphere");
            thirdPersonModel = Content.Load<Model>("Sphere");
            planeModel = Content.Load<Model>("Plane");
            camera = new Camera();
            sunTransform = new Transform();
            mercuryTransform = new Transform();
            earthTransform = new Transform();
            moonTransform = new Transform();
            thirdPersonTransform = new Transform();
            cameraTransform = new Transform();
            planeTransform = new Transform();
            
            cameraTransform.LocalPosition = Vector3.Backward * 50;
            camera.Transform = cameraTransform;
            sunTransform.LocalScale *= 5;
            mercuryTransform.LocalPosition = Vector3.Right * 2;
            mercuryTransform.LocalScale *= 0.4f;
            earthTransform.LocalPosition = Vector3.Forward * 5;
            earthTransform.LocalScale *= 0.6f;
            moonTransform.LocalPosition = Vector3.Right * 3;
            moonTransform.LocalScale /= 3;
            thirdPersonTransform.LocalPosition = new Vector3(0, -5, 20);
            planeTransform.LocalPosition = new Vector3(0, -6, 0);
            planeTransform.LocalScale *= 10;
            
            // Parents
            mercuryTransform.Parent = sunTransform;
            earthTransform.Parent = sunTransform;
            moonTransform.Parent = earthTransform;

            // Lighting
            foreach (ModelMesh mesh in sunModel.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();
            foreach (ModelMesh mesh in mercuryModel.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();
            foreach (ModelMesh mesh in earthModel.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();
            foreach (ModelMesh mesh in moonModel.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();
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
            
            sunTransform.Rotate(Vector3.Up, speed * Time.ElapsedGameTime);
            earthTransform.Rotate(Vector3.Up, speed * Time.ElapsedGameTime);
            moonTransform.Rotate(Vector3.Up, 2 * speed * Time.ElapsedGameTime);

            // Animation Speed
            if (InputManager.IsKeyDown(Keys.Z))
                speed += Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.X))
                speed -= Time.ElapsedGameTime;

            // Camera Zoom
            if (InputManager.IsKeyDown(Keys.C))
                camera.FieldOfView += 0.01f;
            if (InputManager.IsKeyDown(Keys.V))
                camera.FieldOfView -= 0.01f;

            // check for change in perspective
            if (InputManager.IsKeyPressed(Keys.Tab))
                isFirstPerson = !isFirstPerson;

            // First Person Camera
            if (isFirstPerson)
            {
                // Camera Movement
                if (InputManager.IsKeyDown(Keys.W))
                    cameraTransform.LocalPosition += cameraTransform.Forward * 10 * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.S))
                    cameraTransform.LocalPosition += cameraTransform.Backward * 10 * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.A))
                    cameraTransform.LocalPosition += cameraTransform.Left * 10 * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.D))
                    cameraTransform.LocalPosition += cameraTransform.Right * 10 * Time.ElapsedGameTime;
                if (InputManager.IsMousePressed(0))
                    cameraTransform.LocalPosition += cameraTransform.Forward * 10 * Time.ElapsedGameTime;

                // Camera Rotation
                if (InputManager.IsKeyDown(Keys.Left))
                    cameraTransform.Rotate(Vector3.Up, Time.ElapsedGameTime);
                if (InputManager.IsKeyDown(Keys.Right))
                    cameraTransform.Rotate(Vector3.Up, -Time.ElapsedGameTime);
                if (InputManager.IsCursorMovedLeft())
                    cameraTransform.Rotate(Vector3.Up, Time.ElapsedGameTime);
                if (InputManager.IsCursorMovedRight())
                    cameraTransform.Rotate(Vector3.Up, -Time.ElapsedGameTime);
            }
            // Third Person Camera
            if (!isFirstPerson)
            {
                // Third Person Movement
                if (InputManager.IsKeyDown(Keys.W))
                    thirdPersonTransform.LocalPosition += thirdPersonTransform.Forward * 10 * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.S))
                    thirdPersonTransform.LocalPosition += thirdPersonTransform.Backward * 10 * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.A))
                    thirdPersonTransform.LocalPosition += thirdPersonTransform.Left * 10 * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.D))
                    thirdPersonTransform.LocalPosition += thirdPersonTransform.Right * 10 * Time.ElapsedGameTime;
                if (InputManager.IsMousePressed(0))
                    thirdPersonTransform.LocalPosition += thirdPersonTransform.Forward * 10 * Time.ElapsedGameTime;

                // Third Person Rotation
                if (InputManager.IsKeyDown(Keys.Left))
                    thirdPersonTransform.Rotate(Vector3.Up, Time.ElapsedGameTime);
                if (InputManager.IsKeyDown(Keys.Right))
                    thirdPersonTransform.Rotate(Vector3.Up, -Time.ElapsedGameTime);

                // Camera Rotation
                if (InputManager.IsCursorMovedLeft())
                    cameraTransform.Rotate(Vector3.Up, Time.ElapsedGameTime);
                if (InputManager.IsCursorMovedRight())
                    cameraTransform.Rotate(Vector3.Up, -Time.ElapsedGameTime);
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

            if (isFirstPerson)
            {
                sunModel.Draw(sunTransform.World, camera.View, camera.Projection);
                mercuryModel.Draw(mercuryTransform.World, camera.View, camera.Projection);
                earthModel.Draw(earthTransform.World, camera.View, camera.Projection);
                moonModel.Draw(moonTransform.World, camera.View, camera.Projection);
                planeModel.Draw(planeTransform.World, camera.View, camera.Projection);
            }
            else
            {
                
                sunModel.Draw(sunTransform.World, camera.ThirdPersonView, camera.Projection);
                mercuryModel.Draw(mercuryTransform.World, camera.ThirdPersonView, camera.Projection);
                earthModel.Draw(earthTransform.World, camera.ThirdPersonView, camera.Projection);
                moonModel.Draw(moonTransform.World, camera.ThirdPersonView, camera.Projection);
                thirdPersonModel.Draw(thirdPersonTransform.World, camera.ThirdPersonView, camera.Projection);
                planeModel.Draw(planeTransform.World, camera.ThirdPersonView, camera.Projection);
            }

            base.Draw(gameTime);
        }
    }
}
