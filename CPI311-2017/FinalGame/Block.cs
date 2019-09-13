using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using CPI311.GameEngine;

namespace FinalGame
{
    public class Block : GameObject
    {
        Camera camera;
        Texture2D texture;
        Effect effect;
        GridNode CurrentNode;
        GridNode NextNode;
        private Player Player;

        public Block(ContentManager Content, Camera camera,
            GraphicsDevice graphicsDevice, Light light, Player player, GridNode currentNode)
            : base()
        {
            effect = Content.Load<Effect>("SimpleShading");
            this.camera = camera;
            Player = player;
            CurrentNode = currentNode;
            CurrentNode.HasBox = true;
            NextNode = CurrentNode;

            // Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            Transform.LocalScale *= Vector3.One * 2.5f;

            // Renderer
            texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("box2"),
                Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            Add<Renderer>(renderer);

            // Collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            this.Renderer.Material.Diffuse = Color.LightBlue.ToVector3();
            this.Renderer.Material.Specular = Color.Blue.ToVector3();
            this.Renderer.Material.Ambient = Color.Blue.ToVector3();
        }

        public override void Update()
        {
            // Player collides with block
            Vector3 normal;
            if (Player.Collider.Collides(this.Collider, out normal))
            {
                if (Player.Rigidbody.Velocity.Z < 0 && CurrentNode.Forward != null
                    && !CurrentNode.Forward.HasBox)
                {
                    Rigidbody.Velocity = Vector3.Forward;
                    Rigidbody.AnimationSpeed = 10;
                    NextNode = CurrentNode.Forward;
                    CurrentNode.HasBox = false;
                }
                if (Player.Rigidbody.Velocity.Z > 0 && CurrentNode.Backward != null
                    && !CurrentNode.Backward.HasBox)
                {
                    Rigidbody.Velocity = Vector3.Backward;
                    Rigidbody.AnimationSpeed = 10;
                    NextNode = CurrentNode.Backward;
                    CurrentNode.HasBox = false;
                }
                if (Player.Rigidbody.Velocity.X < 0 && CurrentNode.Left != null
                    && !CurrentNode.Left.HasBox)
                {
                    Rigidbody.Velocity = Vector3.Left;
                    Rigidbody.AnimationSpeed = 10;
                    NextNode = CurrentNode.Left;
                    CurrentNode.HasBox = false;
                }
                if (Player.Rigidbody.Velocity.X > 0 && CurrentNode.Right != null
                    && !CurrentNode.Right.HasBox)
                {
                    Rigidbody.Velocity = Vector3.Right;
                    Rigidbody.AnimationSpeed = 10;
                    NextNode = CurrentNode.Right;
                    CurrentNode.HasBox = false;
                }
            }
            
            if (Vector2.Distance(new Vector2(Transform.Position.X, Transform.Position.Z), 
                new Vector2(NextNode.Transform.Position.X, NextNode.Transform.Position.Z)) <= 0.01f)
            {
                Rigidbody.Velocity = Vector3.Zero;
                Rigidbody.AnimationSpeed = 0;
                CurrentNode = NextNode;
                CurrentNode.HasBox = true;
            }

            base.Update();

        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
