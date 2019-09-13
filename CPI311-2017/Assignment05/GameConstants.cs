using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment05
{
    public class GameConstants
    {
        // camera constants
        public const float PlayfieldSizeX = 100f;
        public const float PlayfieldSizeY = 100f;

        // asteroid constants
        public const int NumAsteroids = 10;
        public const int AsteroidMinSpeed = 0;
        public const float AsteroidMaxSpeed = 2;

        // bullet constants
        public const int NumBullets = 10;
        public const float BulletSpeedAdjustment = 5f;

        // score constants
        public const int ShotPenalty = 10;
        public const int KillBonus = 50;
    }
}
