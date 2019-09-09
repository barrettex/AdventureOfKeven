using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    public class LivingCreature
    {
        public int CurrentHitPoints { get; set; }
        public int MaximumHitPoints { get; set; }
        public bool isAlive { get; set; }

        public LivingCreature(int currentHitPoints, int maximumHitPoints)
        {
            this.CurrentHitPoints = currentHitPoints;
            this.MaximumHitPoints = maximumHitPoints;
            this.isAlive = this.CurrentHitPoints > 0 ? true : false; //TO VERIFY
        }
    }
}
