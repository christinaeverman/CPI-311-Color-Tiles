using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class ProgressBar : Sprite
    {
        public Color FillColor { get; set; }
        public float Value { get; set; }
        public float Speed { get; set; }
        public Rectangle ProgressSource { get; set; }
        protected bool isStart = false;
        public float totalValue = 1;

        public ProgressBar(Texture2D texture, Color fillColor, float value, float speed) : base(texture)
        {
            FillColor = fillColor;

            if (value <= 0)
                value = 10;
            else
                Value = value;

            Speed = speed;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.Draw(Texture, Position, ProgressSource, FillColor, Rotation,
               Origin, Scale, Effect, Layer);
        }
    }
}
