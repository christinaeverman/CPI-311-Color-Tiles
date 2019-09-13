using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Storage;

using CPI311.GameEngine;

namespace Lab06
{
    public class Lab07 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;
        Model model;
        Random random;

        Transform cameraTransform;
        Camera camera;
       
        List<Rigidbody> rigidbodies;
        List<Collider> colliders;
        List<Transform> transforms;

        BoxCollider boxCollider;

        int lastSecondCollisions = 0;
        int numberCollisions = 0;
        bool haveThreadRunning = false;

        // *** Lab7 : Need Light.cs ***********
        Light light;
        List<Renderer> renderers;
        //**************************************

        public Lab07()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = "Content";
            // ***** From MonoGame3.6 Need this statement
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            random = new Random();
            transforms = new List<Transform>();
            rigidbodies = new List<Rigidbody>();
            colliders = new List<Collider>();

            // *** Lab7 Need Renderer.cs
            renderers = new List<Renderer>(); 
            // *** Lab 7 for a thread
            haveThreadRunning = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(CollisionReset));
            //**************************************************************

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("font");
            model = Content.Load<Model>("Sphere");

            foreach (ModelMesh mesh in model.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();

            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 20;
            camera = new Camera();
            camera.Transform = cameraTransform;

            // *** Lab7 : Need Light.cs
            light = new Light();
            Transform lightTransform = new Transform();
            lightTransform.LocalPosition = Vector3.Backward * 10 + Vector3.Right * 5;
            light.Transform = lightTransform;
            // *** Lab7 : Use the AddSphere() method
            AddSphere(); // first shpere

            boxCollider = new BoxCollider();
            boxCollider.Size = 10;

        }

        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            InputManager.Update();
            if (InputManager.IsKeyDown(Keys.Escape))
                Exit();
            //if (objectTransform.LocalPosition.Y < 0 && rigidbody.Velocity.Y < 0)
            //   rigidbody.Impulse = -new Vector3(0,rigidbody.Velocity.Y,0) * 2.1f * rigidbody.Mass;

            // *** Lab7: Need AddSphere() method
            if (InputManager.IsKeyPressed(Keys.Space))
                AddSphere();

            foreach (Rigidbody rigidbody in rigidbodies)
                rigidbody.Update();

            // On a new thread --
            // To fix: Assignment 4
            // 1. Binary spheres (fixed (see (A))
            // 2. Enery in the system increases (how to fix?)
            Vector3 normal;
            for (int i = 0; i < transforms.Count; i++)
            {
                if (boxCollider.Collides(colliders[i], out normal))
                {
                    numberCollisions++;
                    // Lab 7: include mass in equation
                    if (Vector3.Dot(normal, rigidbodies[i].Velocity) < 0)
                        rigidbodies[i].Impulse += Vector3.Dot(normal, rigidbodies[i].Velocity) * -2 * normal; // reflection vector
                }
                for (int j = i + 1; j < transforms.Count; j++)
                {
                    if (colliders[i].Collides(colliders[j], out normal))
                    {
                        // Lab 7: include mass in equation
                        numberCollisions++;
                        // do resolution ONLY if they are colliding into one another
                        // if normal is from i to j
                        //dot(normal, vi) > 0 & dot(normal, vj) < 0) (A)
                        if (Vector3.Dot(normal, rigidbodies[i].Velocity) > 0 && Vector3.Dot(normal, rigidbodies[j].Velocity) < 0)
                            return;

                        Vector3 velocityNormal = Vector3.Dot(normal, rigidbodies[i].Velocity - rigidbodies[j].Velocity)
                                        * -2 * normal * rigidbodies[i].Mass * rigidbodies[j].Mass;
                        rigidbodies[i].Impulse += velocityNormal / 2;
                        rigidbodies[j].Impulse += -velocityNormal / 2;
                    }
                }
            }
            // on a new thread --

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            for (int i = 0; i < transforms.Count; i++)
            {


                // *** Lab7: Update the draw method 
                // *** OLD Lab6 
                // (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = new Vector3(1, 1, 1); // Old Lab6 method
                // model.Draw(transforms[i].World, camera.View, camera.Projection); // Old Lab6 method
                renderers[i].Draw();
                
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Collision: " + lastSecondCollisions, Vector2.Zero, Color.Black);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        // Lab 7: add shpere method
        private void AddSphere()
        {
            Transform transform = new Transform();
            // Lab 7: random scale
            //transform.LocalScale *= random.Next(1, 10) *0.25f;

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = transform;

            // Lab 7: random mass
            rigidbody.Mass = 0.5f + (float)random.NextDouble();
            // Lab 7: Gravity
            rigidbody.Acceleration = Vector3.Down * 9.81f;
            // Lab 7: random velocity
            Vector3 direction = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            direction.Normalize();
            rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 5);

            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = transform.LocalScale.Y;
            sphereCollider.Transform = transform;

            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(model, transform, camera, Content, GraphicsDevice, light, 1, "SimpleShading", 20f, texture);
            //Renderer renderer = new Renderer(model, transform, camera, Content, GraphicsDevice, light, 1, null, 20f, texture);

            transforms.Add(transform);
            colliders.Add(sphereCollider);
            rigidbodies.Add(rigidbody);
            renderers.Add(renderer);
        }

        // Simple example of multi threading 
        // Lab 7
        private void CollisionReset(Object obj)
        {
            while (haveThreadRunning)
            {
                lastSecondCollisions = numberCollisions;
                numberCollisions = 0;
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
    
    /*** Lab7 Pre *****************************************
    public class Lab07 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Model model;
        Random random;

        Transform cameraTransform;
        Camera camera;

        List<Rigidbody> rigidbodies;
        List<Collider> colliders;
        List<Transform> transforms;

        BoxCollider boxCollider;

        int lastSecondCollisions = 0;
        int numberCollisions = 0;
        bool haveThreadRunning = false;

        public Lab07()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            //ScreenManager.Initialize(graphics);

            random = new Random();
            transforms = new List<Transform>();
            rigidbodies = new List<Rigidbody>();
            colliders = new List<Collider>();

            haveThreadRunning = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(CollisionReset));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            model = Content.Load<Model>("Sphere");
            foreach (ModelMesh mesh in model.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();

            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 20;
            camera = new Camera();
            camera.Transform = cameraTransform;

            AddSphere();
            transforms[0].LocalPosition = new Vector3(0, -9.5f, 0);
            rigidbodies[0].Velocity = new Vector3(1, 1, 0);

            boxCollider = new BoxCollider();
            boxCollider.Size = 10;
        }

        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            InputManager.Update();
            if (InputManager.IsKeyDown(Keys.Escape))
                Exit();
            //if (objectTransform.LocalPosition.Y < 0 && rigidbody.Velocity.Y < 0)
            //    rigidbody.Impulse = -new Vector3(0,rigidbody.Velocity.Y,0) * 2.1f * rigidbody.Mass;

            if (InputManager.IsKeyPressed(Keys.Space))
            {
                AddSphere();
            }

            foreach (Rigidbody rigidbody in rigidbodies)
                rigidbody.Update();

            // On a new thread --
            // To fix: Assignment 4
            // 1. Binary spheres (fixed (see (A))
            // 2. Enery in the system increases (how to fix?)
            Vector3 normal;
            for (int i = 0; i < transforms.Count; i++)
            {
                if (boxCollider.Collides(colliders[i], out normal))
                {
                    numberCollisions++;
                    // Lab 7: include mass in equation
                    if (Vector3.Dot(normal, rigidbodies[i].Velocity) < 0)
                        rigidbodies[i].Impulse += Vector3.Dot(normal, rigidbodies[i].Velocity) * -2 * normal;
                }
                for (int j = i + 1; j < transforms.Count; j++)
                {
                    if (colliders[i].Collides(colliders[j], out normal))
                    {
                        // Lab 7: include mass in equation
                        numberCollisions++;
                        // do resolution ONLY if they are colliding into one another
                        // if normal is from i to j
                        //dot(normal, vi) > 0 & dot(normal, vj) < 0) (A)
                        if (Vector3.Dot(normal, rigidbodies[i].Velocity) > 0 && Vector3.Dot(normal, rigidbodies[j].Velocity) < 0) return;
                        Vector3 velocityNormal = Vector3.Dot(normal, rigidbodies[i].Velocity - rigidbodies[j].Velocity) * -2 * normal * rigidbodies[i].Mass * rigidbodies[j].Mass;
                        rigidbodies[i].Impulse += velocityNormal / 2;
                        rigidbodies[j].Impulse += -velocityNormal / 2;
                    }
                }
            }
            // on a new thread --

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();


            //foreach (Transform transform in transforms)
            for (int i = 0; i < transforms.Count; i++)
            {
                Transform transform = transforms[i];
                float speed = rigidbodies[i].Velocity.Length();
                float speedValue = MathHelper.Clamp(speed / 20f, 0, 1);
                (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = new Vector3(speedValue, speedValue, 1);
                model.Draw(transform.World, camera.View, camera.Projection);
            }


            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Collision: " + lastSecondCollisions, Vector2.Zero, Color.Black);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        // *** Lab 7: add shpere method
        private void AddSphere()
        {
            Transform transform = new Transform();
            // Lab 7: random scale
            //transform.LocalScale *= random.Next(1, 10) *0.25f;

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = transform;

            // Lab 7: random mass
            rigidbody.Mass = 1; // (float)random.NextDouble() * 5;
            // Lab 7: Gravity
            rigidbody.Acceleration = Vector3.Down * 9.81f;
            // Lab 7: random velocity
            Vector3 direction = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            direction.Normalize();
            rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 5);

            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = transform.LocalScale.Y; ; // 2.5f * transform.LocalScale.Y;
            sphereCollider.Transform = transform;

            transforms.Add(transform);
            colliders.Add(sphereCollider);
            rigidbodies.Add(rigidbody);
        }
        //**************************************************************

        // *** Lab7 Update :Simple example of multi threading 
        private void CollisionReset(Object obj)
        {
            while (haveThreadRunning)
            {
                lastSecondCollisions = numberCollisions;
                numberCollisions = 0;
                System.Threading.Thread.Sleep(1000);
            }
        }
        //******************************************************
     }*/


}
