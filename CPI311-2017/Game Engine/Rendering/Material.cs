using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System;
using System.Linq;

namespace CPI311.GameEngine
{
    public class Material
    {

        public Vector3 Diffuse { get; set; }
        public Vector3 Ambient { get; set; }
        public Vector3 Specular { get; set; }
        public float Shininess { get; set; }

        public Matrix World { get; set; }
        public Camera Camera { get; set; }

        public Light Light { get; set; }
        public int CurrentTechnique { get; set; }
        public int Passes { get { return effect.CurrentTechnique.Passes.Count; } }

        public Texture2D Texture { get; set; }

        public Effect effect;


        // Assignment3
        //public Vector2 Tiling;
        //public Vector2 Offset;

        public Material(Matrix world, Camera camera, Light light, ContentManager content, String FileLoc,
            int currentTechnique, float shininess, Texture2D texture)
        {

            effect = content.Load<Effect>(FileLoc);

            World = world;
            Texture = texture;
            Camera = camera;
            Light = light;
            Shininess = shininess;
            CurrentTechnique = currentTechnique;
            Ambient = Color.LightGray.ToVector3();
            Specular = Color.LightGray.ToVector3();
            Diffuse = Color.LightGray.ToVector3();

            // Assignment3 for tiling and offset

        }

        public virtual void Apply(int currentPass)
        {
            effect.CurrentTechnique = effect.Techniques[CurrentTechnique];
            effect.Parameters["World"].SetValue(World);
            effect.Parameters["View"].SetValue(Camera.View);
            effect.Parameters["Projection"].SetValue(Camera.Projection);
            effect.Parameters["LightPosition"].SetValue(Light.Transform.LocalPosition);
            effect.Parameters["CameraPosition"].SetValue(Camera.Transform.Position);
            effect.Parameters["Shininess"].SetValue(Shininess);
            effect.Parameters["AmbientColor"].SetValue(Ambient); // ToVector3() from Color to Vector3
            effect.Parameters["DiffuseColor"].SetValue(Diffuse);
            effect.Parameters["SpecularColor"].SetValue(Specular);
            effect.Parameters["DiffuseTexture"].SetValue(Texture);
            // Assignment3 for tiling and offset 
            effect.CurrentTechnique.Passes[currentPass].Apply();
        }
    }
}
