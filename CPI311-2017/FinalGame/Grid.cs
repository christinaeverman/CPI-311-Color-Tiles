using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using CPI311.GameEngine;

namespace FinalGame
{
    public class Grid
    {
        public int Cols { get; set; }
        public int Rows { get; set; }
        public GridNode[,] Nodes {get; set; }
        public GridNode Start { get; set; }
        public GridNode End { get; set; }
        private float GridWidth { get; set; }
        private float GridHeight { get; set; }
        private float NodeSize;

        public Grid(ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, 
            Light light, int Cols, int Rows)
        {
            this.Cols = Cols;
            this.Rows = Rows;
            Nodes = new GridNode[Rows, Cols];
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    Nodes[r, c] = new GridNode(Content, camera, graphicsDevice, light, c, r, 
                        new Vector3(c, 0, r));

            Start = Nodes[0, 0];
            Start.Renderer.Material.Diffuse = Color.Orange.ToVector3();
            Start.Renderer.Material.Specular = Color.Orange.ToVector3();
            Start.Renderer.Material.Ambient = Color.Orange.ToVector3();
            Start.ColorChange = true;
            End = Nodes[Rows - 1, Cols - 1];

            NodeSize = 6.5f;
            GridWidth = Cols * NodeSize;
            GridHeight = Rows * NodeSize;

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    Nodes[i, j].Transform.LocalPosition = GetPosition(Nodes[i,j]);
                    Nodes[i, j].OriginalPosition = Nodes[i, j].Transform.LocalPosition;

                    if (i > 0)
                        Nodes[i, j].Forward = Nodes[i - 1, j];
                    if (i < Rows - 1)
                        Nodes[i, j].Backward = Nodes[i + 1, j];
                    if (j > 0)
                        Nodes[i, j].Left = Nodes[i, j - 1];
                    if (j < Cols - 1)
                        Nodes[i, j].Right = Nodes[i, j + 1];
                }
            }
        }

        public Vector3 GetPosition(GridNode node)
        {
            return new Vector3(node.Col * NodeSize - GridWidth / 2, -2, node.Row * NodeSize
                - GridHeight / 2);
        }

        public bool Equals(Grid grid)
        {
            if (this.Cols != grid.Cols || this.Rows != grid.Rows)
                return false;

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (this.Nodes[i, j].HasBox != grid.Nodes[i, j].HasBox)
                        return false;

                    if (this.Nodes[i, j].ColorChange != grid.Nodes[i, j].ColorChange)
                        return false;
                }
            }

            return true;
        }

        public void Draw()
        {
            foreach (GridNode node in Nodes)
                node.Draw();
        }
    }
}
