using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CPI311.GameEngine
{
    public class AnimatedSprite : Sprite
    {
        public int Frames { get; set; }
        public float Frame { get; set; }
        public float Speed { get; set; }

        private bool isStarted = false;

        public AnimatedSprite(Texture2D texture, int frames = 1) : base (texture)
        {
            Frames = frames;
            Frame = 0;
            Speed = 10;
        }

        public override void Update()
        {
            if (!isStarted)
            {
                Source = new Rectangle((Convert.ToInt32(Frame)) * 32, 32, 32, 32);
                isStarted = true;
            }

            if (Frame > 7)
                Frame = 0;

            if (InputManager.IsKeyDown(Keys.Up))
            {
                Frame += Speed * Time.ElapsedGameTime;
                Source = new Rectangle((Convert.ToInt32(Frame)) * 32, 0, 32, 32);
            }
            if (InputManager.IsKeyDown(Keys.Down))
            {
                Frame += Speed * Time.ElapsedGameTime;
                Source = new Rectangle((Convert.ToInt32(Frame)) * 32, 32, 32, 32);
            }
            if (InputManager.IsKeyDown(Keys.Left))
            {
                Frame += Speed * Time.ElapsedGameTime;
                Source = new Rectangle((Convert.ToInt32(Frame)) * 32, 64, 32, 32);
            }
            if (InputManager.IsKeyDown(Keys.Right))
            {
                Frame += Speed * Time.ElapsedGameTime;
                Source = new Rectangle((Convert.ToInt32(Frame)) * 32, 96, 32, 32);
            }

            base.Update();
        }
    }
}