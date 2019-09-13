using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class DistanceBar : ProgressBar
    {
        public float DistanceWalked { get; set; }
        private int TotalValue;
        public DistanceBar(Texture2D texture, Color fillColor, float value, float speed) : 
            base(texture, fillColor, value, speed)
        {
            DistanceWalked = 0;
            TotalValue = 16;
        }

        public override void Update()
        {
            if (DistanceWalked >= 128)
            {
                Value++;
                DistanceWalked = 0;
            }

            if (Value == 0)
                ProgressSource = new Rectangle(0, 0, 0, Texture.Height);
            if (Value >= TotalValue)
                ProgressSource = new Rectangle(0, 0, Texture.Width, Texture.Height);
            if (Value > 0 && Value < TotalValue)
                ProgressSource = new Rectangle(0, 0,
                    Convert.ToInt32(Texture.Width * Value / TotalValue), Texture.Height);

            base.Update();
        }
    }
}
