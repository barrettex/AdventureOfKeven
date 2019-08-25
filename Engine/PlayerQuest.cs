using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    public class PlayerQuest
    {

        public Quest Details { get; set; }
        public bool IsCompleted { get; set; }

        public PlayerQuest(Quest details, bool isCompleted = false)
        {
            this.Details = details;
            this.IsCompleted = isCompleted;
        }


    }
}
