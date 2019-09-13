using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CPI311.GameEngine
{
    public class Ship : GameObject
    {
        public Boolean isActive;

        public Ship(ContentManager Content, Camera camera, GraphicsDevice
            graphicsDevice, Light light) : base()
        {
            // *** Add Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            // *** Add Renderer
            Texture2D texture = Content.Load<Texture2D>("Content/Content/Textures/Square");
            Renderer renderer = new Renderer(Content.Load<Model>("Content/Content/Models/p1_wedge"),
            Transform, camera, Content, graphicsDevice, light, 1, null, 20f, texture);
            Add<Renderer>(renderer);

            // *** Add collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius * 0.01f;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            isActive = true;
            this.Transform.LocalScale = new Vector3(0.01f, 0.01f, 0.01f);
        }

        public override void Update()
        {
            if (isActive)
            {
                if (InputManager.IsKeyDown(Keys.W))
                    Transform.Position += Vector3.Up * Time.ElapsedGameTime * 10;
                if (InputManager.IsKeyDown(Keys.A))
                    Transform.Position -= Vector3.Right * Time.ElapsedGameTime * 10;
                if (InputManager.IsKeyDown(Keys.S))
                    Transform.Position -= Vector3.Up * Time.ElapsedGameTime * 10;
                if (InputManager.IsKeyDown(Keys.D))
                    Transform.Position += Vector3.Right * Time.ElapsedGameTime * 10;

                base.Update();
            }
        }

        public override void Draw()
        {
            if (isActive)
                base.Draw();
        }
    }
}