using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class SpiralMover
    {
        public Sprite Sprite { get; set; }
        public Vector2 Position { get; set; }
        public float Radius { get; set; }
        public float Phase { get; set; }
        public float Speed { get; set; }
        
        public SpiralMover(Texture2D texture, Vector2 position, float radius = 150)
        {
            Sprite = new Sprite(texture);
            Position = position;
            Radius = radius;
            Phase = 0;
            Speed = 0.5f;
            Sprite.Position = Position + new Vector2(Radius, 0);
        }

        public void Update()
        {
            Phase += Time.ElapsedGameTime * Speed;

            Sprite.Position = Position + new Vector2((float)((Radius + Math.Cos(Phase)) * (Math.Cos(Phase))), 
               (float)((Radius + Math.Sin(Phase)) * Math.Sin(Phase)));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Sprite.Draw(spriteBatch);
        }
    }
}
