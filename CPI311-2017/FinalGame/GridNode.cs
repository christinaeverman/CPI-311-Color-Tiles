using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using CPI311.GameEngine;

namespace FinalGame
{
    public class GridNode : GameObject
    {
        public Vector3 GridPosition { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public bool ColorChange { get; set; }
        public bool HasBox { get; set; }
        public bool IsPressed { get; set; }
        public GridNode Forward { get; set; }
        public GridNode Backward { get; set; }
        public GridNode Right { get; set; }
        public GridNode Left { get; set; }
        public Vector3 OriginalPosition { get; set; }
        public ContentManager Content;

        public GridNode(ContentManager content, Camera camera, GraphicsDevice graphicsDevice, 
            Light light, int col, int row, Vector3 pos) : base()
        {
            Col = col;
            Row = row;
            GridPosition = pos;
            ColorChange = false;
            HasBox = false;
            IsPressed = false;
            Forward = null;
            Backward = null;
            Right = null;
            Left = null;

            Content = content;

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("box2"),
                Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            Add<Renderer>(renderer);

            // Collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius * 1.5f;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            this.Transform.LocalScale = Vector3.One * 3;
        }
    }
}
