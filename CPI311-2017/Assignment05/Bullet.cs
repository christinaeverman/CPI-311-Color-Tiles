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
using Assignment05;

namespace Assignment05
{
    public class Bullet : GameObject
    {
        public Boolean isActive;

        public Bullet(ContentManager Content, Camera camera, GraphicsDevice
            graphicsDevice, Light light) : base()
        {
            // *** Add Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            // *** Add Renderer
            Texture2D texture = Content.Load<Texture2D>("Content/Content/Textures/Square");
            Renderer renderer = new Renderer(Content.Load<Model>("Content/Content/Models/bullet"),
            Transform, camera, Content, graphicsDevice, light, 1, null, 20f, texture);
            Add<Renderer>(renderer);

            // *** Add collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            //*** Additional Property (for Asteroid, isActive = true)
            isActive = false;
        }
        public override void Update()
        {
            if (!isActive) return;

            if (isActive)
                Transform.Position += Transform.Forward * GameConstants.BulletSpeedAdjustment;

            if (Transform.Position.X > GameConstants.PlayfieldSizeX ||
            Transform.Position.X < -GameConstants.PlayfieldSizeX ||
            Transform.Position.Z > GameConstants.PlayfieldSizeY ||
            Transform.Position.Z < -GameConstants.PlayfieldSizeY)
            {
                isActive = false;
                Rigidbody.Velocity = Vector3.Zero; // stop moving
            }

            base.Update();
        }

        public override void Draw()
        {
            if (isActive)
                base.Draw();
        }
    }
}
