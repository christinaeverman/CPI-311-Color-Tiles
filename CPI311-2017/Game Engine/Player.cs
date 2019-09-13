using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace CPI311.GameEngine
{
    public class Player : GameObject
    {
        public TerrainRenderer Terrain { get; set; }
        public Rigidbody rigidbody { get; set; }
        public int aliensCaught { get; set; }
        public float timeSpent { get; set; }
        public BoxCollider boxCollider { get; set; }

        public Player(TerrainRenderer terrain, ContentManager Content, Camera camera,
        GraphicsDevice graphicsDevice, Light light) : base()
        {
            Terrain = terrain;

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            aliensCaught = 0;
            timeSpent = 0.0f;
            boxCollider = new BoxCollider();
        }

        public override void Update()
        {
            // Control the player
            if (InputManager.IsKeyDown(Keys.W) && Terrain.GetAltitude(this.Transform.LocalPosition + Vector3.Forward * 2) < 1) // move forward
                this.Transform.LocalPosition += Vector3.Forward * Time.ElapsedGameTime * 10;
            if (InputManager.IsKeyDown(Keys.S) && Terrain.GetAltitude(this.Transform.LocalPosition + Vector3.Backward * 2) < 1) // move backwards
                this.Transform.LocalPosition += Vector3.Backward * Time.ElapsedGameTime * 10;
            if (InputManager.IsKeyDown(Keys.A) && Terrain.GetAltitude(this.Transform.LocalPosition + Vector3.Left * 2) < 1)
                this.Transform.LocalPosition += Vector3.Left * Time.ElapsedGameTime * 10;
            if (InputManager.IsKeyDown(Keys.D) && Terrain.GetAltitude(this.Transform.LocalPosition + Vector3.Right * 2) < 1)
                this.Transform.LocalPosition += Vector3.Right * Time.ElapsedGameTime * 10;

            // change the Y position corresponding to the terrain (maze)
            this.Transform.LocalPosition = new Vector3(
            this.Transform.LocalPosition.X,
            Terrain.GetAltitude(this.Transform.LocalPosition),
            this.Transform.LocalPosition.Z) + Vector3.Up;

            timeSpent += Time.ElapsedGameTime;
            base.Update();
        }

    }
}
