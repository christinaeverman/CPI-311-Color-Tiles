using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CPI311.GameEngine
{
    public class Agent : GameObject
    {
        public AStarSearch search;
        List<Vector3> path;
        private float speed = 3f; //moving speed
        private int gridSize = 20; //grid size
        private TerrainRenderer Terrain;
        public SphereCollider sphereCollider;

        public Agent(TerrainRenderer terrain, ContentManager Content,
            Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            Terrain = terrain;
            path = null;
            sphereCollider = new SphereCollider();

            search = new AStarSearch(gridSize, gridSize);
            float gridW = Terrain.size.X / gridSize;
            float gridH = Terrain.size.Y / gridSize;

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Vector3 pos = new Vector3(gridW * j + gridW / 2 - terrain.size.X / 2,
                        0, gridH * i + gridH / 2 - terrain.size.Y / 2);
                    if (Terrain.GetAltitude(pos) > 1.0)
                        search.Nodes[i, j].Passable = false;
                }
            }
        }

        private Vector3 GetGridPosition(Vector3 gridPos)
        {
            float gridW = Terrain.size.X / search.Cols;
            float gridH = Terrain.size.Y / search.Rows;
            return new Vector3(gridW * gridPos.X + gridW / 2 - Terrain.size.X / 2,
                0, gridH * gridPos.Z + gridH / 2 - Terrain.size.Y / 2);
        }

        private void RandomPathFinding()
        {
            Random random = new Random();
            while (!(search.Start = search.Nodes[random.Next(search.Cols),
                random.Next(search.Rows)]).Passable) ;
            search.End = search.Nodes[search.Cols / 2, search.Rows / 2];
            search.Search();
            path = new List<Vector3>();
            AStarNode current = search.End;

            while (current != null)
            {
                path.Insert(0, current.Position);
                current = current.Parent;
            }
        }

        public override void Update()
        {
            if (path != null)
            {
                // Move to the destination along the path
                Vector3 distance = GetGridPosition(path[0]) - this.Transform.LocalPosition;
                if (distance != Vector3.Zero)
                    this.Transform.LocalPosition += distance * Time.ElapsedGameTime * speed;

                if (Vector3.Distance(this.Transform.LocalPosition, GetGridPosition(path[0])) < 1) // if it reaches a point, go to the next in path
                {
                    path.RemoveAt(0);

                    if (path.Count == 0) // if it reached the goal
                    {
                        path = null;
                        return;
                    }
                }
            }
            else
            {
                // Search again to make a new path.
                RandomPathFinding();
                this.Transform.Position = GetGridPosition(path[0]);
            }
            Transform.Update();
        }
    }
}
