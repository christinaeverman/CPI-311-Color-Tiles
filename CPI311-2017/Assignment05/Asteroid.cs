using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;

namespace Assignment05
{
    public class Asteroid : GameObject
    {
        public float speed { get; set; }
        public Boolean isActive;

        public Asteroid (ContentManager Content, Camera camera,
            GraphicsDevice graphicsDevice, Light light) : base()
        {
            // Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            // Renderer
            Texture2D texture = Content.Load<Texture2D>("Content/Content/Textures/asteroid1");
            Renderer renderer = new Renderer(Content.Load<Model>("Content/Content/Models/asteroid"),
                Transform, camera, Content, graphicsDevice, light, 1, null, 20f, texture);
            Add<Renderer>(renderer);

            // Collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            isActive = true;

            speed = 5.0f;
            Transform.LocalScale *= Vector3.One * 8;
        }

        public override void Update()
        {
            if (Transform.Position.X > GameConstants.PlayfieldSizeX ||
            Transform.Position.X < -GameConstants.PlayfieldSizeX ||
            Transform.Position.Z > GameConstants.PlayfieldSizeY ||
            Transform.Position.Z < -GameConstants.PlayfieldSizeY)
            {
                isActive = false;
                Rigidbody.Velocity = Vector3.Zero; // stop moving
            }

            Transform.Position += Rigidbody.Velocity * speed * Time.ElapsedGameTime;
            base.Update();
        }

        public override void Draw()
        {
            if (isActive)
                base.Draw();
        }
    }
}
