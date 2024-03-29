﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    public class LivingCreature
    {
        public int CurrentHitPoints { get; set; }
        public int MaximumHitPoints { get; set; }

        public LivingCreature(int currentHitPoints, int maximumHitPoints)
        {
            this.CurrentHitPoints = currentHitPoints;
            this.MaximumHitPoints = maximumHitPoints;
        }

        public bool isAlive()
        {
            if(this.CurrentHitPoints <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
