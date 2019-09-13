using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

using CPI311.GameEngine;

namespace Lab09
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab09 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        AStarSearch search;
        List<Vector3> path; // Path will be created after searching

        Random random = new Random();

        Camera camera;

        Model cube;
        Model sphere;

        int size = 10; //100 is original

        public Lab09()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(graphics);

            search = new AStarSearch(size, size);

            foreach (AStarNode node in search.Nodes)
                if (random.NextDouble() < 0.2)
                    search.Nodes[random.Next(size), random.Next(size)].Passable = false;

            search.Start = search.Nodes[0, 0]; search.Start.Passable = true;
            search.End = search.Nodes[size - 1, size - 1]; search.End.Passable = true;

            search.Search();
            path = new List<Vector3>();
            AStarNode current = search.End;
            while (current != null)
            {
                path.Insert(0, current.Position);
                current = current.Parent;
            }
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            cube = Content.Load<Model>("Box2");
            sphere = Content.Load<Model>("Sphere");

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.One * 7;
            camera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
        }

        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            InputManager.Update();
            if (InputManager.IsKeyDown(Keys.Escape))
                Exit();

            if (InputManager.IsKeyPressed(Keys.Space))
            {
                while (!(search.Start = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)]).Passable) ;
                while (!(search.End = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)]).Passable) ;
                search.Search();
                path.Clear();
                AStarNode current = search.End;
                while (current != null)
                {
                    path.Insert(0, current.Position);
                    current = current.Parent;
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix view = camera.View;
            Matrix projection = camera.Projection;

            (cube.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.DarkBlue.ToVector3();
            cube.Draw(Matrix.CreateScale(55, 0.1f, 55) * Matrix.CreateTranslation(50, -5, 50), view, projection);
            (cube.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.DarkRed.ToVector3();
            foreach (AStarNode node in search.Nodes)
                if (!node.Passable)
                    cube.Draw(Matrix.CreateScale(0.5f, 0.05f, 0.5f) * Matrix.CreateTranslation(node.Position), view, projection);
            (sphere.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.WhiteSmoke.ToVector3();
            foreach (Vector3 position in path)
                sphere.Draw(Matrix.CreateScale(0.1f, 0.1f, 0.1f) * Matrix.CreateTranslation(position), view, projection);
            //spriteBatch.Begin();

            //spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
