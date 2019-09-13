using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using CPI311.GameEngine;

namespace FinalGame
{
    public class Player : GameObject
    {
        public ContentManager Content;
        public Camera Camera;
        private Grid Grid;
        public GridNode currentNode;
        private GridNode nextNode;
        public bool IsMoving;
        private bool FirstMove;
        private SoundEffect sound;
        private SoundEffectInstance soundInstance;
        private float Timer;
        public int numMoves;

        public Player(ContentManager content, Camera camera, GraphicsDevice graphicsDevice, 
            Light light, Grid grid) : base()
        {
            Content = content;
            Camera = camera;
            Grid = grid;
            IsMoving = false;
            FirstMove = false;
            Timer = 0;
            numMoves = 0;

            // Sound
            sound = Content.Load<SoundEffect>("hyperspace_activate");

            // Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            Transform.Position += Vector3.Up * 5;

            // Renderer
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("box2"),
                Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            Add<Renderer>(renderer);

            // Collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius * 0.75f;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            Transform.Position = Grid.GetPosition(Grid.Start) + Vector3.Up * 4;
            currentNode = Grid.Start;
            nextNode = Grid.End;

            this.Renderer.Material.Diffuse = Color.Gray.ToVector3();
            this.Renderer.Material.Specular = Color.Black.ToVector3();
            this.Renderer.Material.Ambient = Color.Black.ToVector3();
        }

        public override void Update()
        {
            // Move player
            if (InputManager.IsKeyPressed(Keys.W) && currentNode.Forward != null
                && (!currentNode.Forward.HasBox || (currentNode.Forward.Forward != null 
                && !currentNode.Forward.Forward.HasBox)) && Rigidbody.AnimationSpeed == 0
                && !IsMoving && (Timer >= 0.29f || !FirstMove))
            {
                IsMoving = true;
                Timer = 0;
                FirstMove = true;
                Rigidbody.Velocity = Vector3.Forward;
                Rigidbody.AnimationSpeed = 10;
                nextNode = currentNode.Forward;
                numMoves++;
            }
            else if (InputManager.IsKeyPressed(Keys.S) && currentNode.Backward != null
                && (!currentNode.Backward.HasBox || (currentNode.Backward.Backward != null
                && !currentNode.Backward.Backward.HasBox)) && Rigidbody.AnimationSpeed == 0
                && !IsMoving && (Timer >= 0.29f || !FirstMove))
            {
                IsMoving = true;
                Timer = 0;
                FirstMove = true;
                Rigidbody.Velocity = Vector3.Backward;
                Rigidbody.AnimationSpeed = 10;
                nextNode = currentNode.Backward;
                numMoves++;
            }
            else if (InputManager.IsKeyPressed(Keys.D) && currentNode.Right != null
                && (!currentNode.Right.HasBox || (currentNode.Right.Right != null
                && !currentNode.Right.Right.HasBox)) && Rigidbody.AnimationSpeed == 0
                && !IsMoving && (Timer >= 0.29f || !FirstMove))
            {
                IsMoving = true;
                Timer = 0;
                FirstMove = true;
                Rigidbody.Velocity = Vector3.Right;
                Rigidbody.AnimationSpeed = 10;
                nextNode = currentNode.Right;
                numMoves++;
            }
            else if (InputManager.IsKeyPressed(Keys.A) && currentNode.Left != null
                && (!currentNode.Left.HasBox || (currentNode.Left.Left != null
                && !currentNode.Left.Left.HasBox)) && Rigidbody.AnimationSpeed == 0
                && !IsMoving && (Timer >= 0.29f || !FirstMove))
            {
                IsMoving = true;
                Timer = 0;
                FirstMove = true;
                Rigidbody.Velocity = Vector3.Left;
                Rigidbody.AnimationSpeed = 10;
                nextNode = currentNode.Left;
                numMoves++;
            }

            if (Vector2.Distance(new Vector2(Transform.Position.X, Transform.Position.Z), 
                new Vector2(nextNode.Transform.Position.X, nextNode.Transform.Position.Z)) <= 0.01f)
            {
                Rigidbody.Velocity = Vector3.Zero;
                Rigidbody.AnimationSpeed = 0;
                IsMoving = false;
                Timer += Time.ElapsedGameTime;
            }
            
            // Check for collision with new grid space
            Vector3 normal;
            foreach (GridNode node in Grid.Nodes)
            {
                if (this.Collider.Collides(node.Collider, out normal) && !node.ColorChange 
                    && node != currentNode)
                {
                    node.Renderer.Material.Diffuse = Color.Orange.ToVector3();
                    node.Renderer.Material.Specular = Color.Orange.ToVector3();
                    node.Renderer.Material.Ambient = Color.Orange.ToVector3();
                    node.ColorChange = true;
                    currentNode = node;
                    node.IsPressed = true;

                    // Play sound when moving to a new grid space
                    soundInstance = sound.CreateInstance();
                    soundInstance.Play();
                }
                if (this.Collider.Collides(node.Collider, out normal) && node.ColorChange 
                    && node != currentNode)
                {
                    node.Renderer.Material.Diffuse = Color.LightGray.ToVector3();
                    node.Renderer.Material.Specular = Color.LightGray.ToVector3();
                    node.Renderer.Material.Ambient = Color.LightGray.ToVector3();
                    node.ColorChange = false;
                    currentNode = node;
                    node.IsPressed = true;

                    // Play sound when moving to a new grid space
                    soundInstance = sound.CreateInstance();
                    soundInstance.Play();
                }
            }

            base.Update();
        }
    }
}
