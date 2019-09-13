using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class TimerBar : ProgressBar
    {
        public TimerBar(Texture2D texture, Color fillColor, float value, float speed) : 
            base(texture, fillColor, value, speed)
        {

        }

        public override void Update()
        {
            if (!isStart)
            {
                totalValue = Value;
                isStart = true;
            }

            if (Value > totalValue)
                Value = totalValue;

            if (Value > 0)
            {
                ProgressSource = new Rectangle(0, 0, 
                    Convert.ToInt32(Texture.Width * Value / totalValue), Texture.Height);
                Value -= Speed;
            }

            base.Update();
        }
    }
}
