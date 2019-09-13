using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    public class Rigidbody : Component, IUpdateable
    {
        public Vector3 Velocity { get; set; }
        public float Mass { get; set; }
        public Vector3 Acceleration { get; set; }
        public Vector3 Impulse { get; set; }

        public float AnimationSpeed { get; set; }

        public void Update()
        {
            Velocity += Acceleration * Time.ElapsedGameTime + Impulse / Mass;
            Transform.LocalPosition += Velocity * Time.ElapsedGameTime * AnimationSpeed;
            Impulse = Vector3.Zero;
        }
    }
}
