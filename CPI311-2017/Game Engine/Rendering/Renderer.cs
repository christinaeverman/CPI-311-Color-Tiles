using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CPI311.GameEngine
{
    public class Renderer : Component, IRenderable
    {
        public Model ObjectModel;
        public Material Material { get; set; }
        public Transform ObjectTransform;

        public Camera Camera;
        public int CurrentTechnique;
        public GraphicsDevice g;
        public Light Light;

        public Renderer(Model objectModel, Transform objectTransform, Camera camera, ContentManager content,
            GraphicsDevice graphicsDevice, Light light, int currentTechnique,
            String effectFileLoc, float shininess, Texture2D diffuseTexture)
        {
            if (effectFileLoc != null)
                Material = new Material(objectTransform.World, camera, light, content, effectFileLoc, currentTechnique, shininess, diffuseTexture);
            else
                Material = null;

            ObjectModel = objectModel;
            ObjectTransform = objectTransform;
            Camera = camera;
            Light = light;
            CurrentTechnique = currentTechnique;
            g = graphicsDevice;
        }
        public virtual void Draw()
        {
            if (Material != null)
            {
                Material.Camera = Camera;
                Material.World = ObjectTransform.World;
                Material.Light = Light;
                Material.CurrentTechnique = CurrentTechnique;

                for (int i = 0; i < Material.Passes; i++)
                {
                    Material.Apply(i);
                    foreach (ModelMesh mesh in ObjectModel.Meshes)
                    {
                        //foreach (BasicEffect basicEffect in mesh.Effects)
                        //  basicEffect.EnableDefaultLighting();
                        foreach (ModelMeshPart part in mesh.MeshParts)
                        {
                            g.SetVertexBuffer(part.VertexBuffer);
                            g.Indices = part.IndexBuffer;
                            g.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, 0,
                                part.NumVertices, part.StartIndex, part.PrimitiveCount);
                        }
                    }
                }
            }
            else
            {
                foreach (ModelMesh mesh in ObjectModel.Meshes)
                    foreach (BasicEffect effect in mesh.Effects)
                        effect.EnableDefaultLighting();

                ObjectModel.Draw(ObjectTransform.World, Camera.View, Camera.Projection);
            }

        }
    }
}
