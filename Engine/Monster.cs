using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    public class Monster : LivingCreature
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int MaximumDamage { get; set; }
        public int RewardExperiencePoints { get; set; }
        public int RewardGold { get; set; }

        public List<LootItem> LootTable { get; set; }


        public Monster(int id, string name, int maximumDamage, int rewardExperiencePoints, int rewardGold, int currentHitPoints, int maximumHitPoints) : base(currentHitPoints, maximumHitPoints)
        {
            this.ID = id;
            this.Name = name;
            this.MaximumDamage = maximumDamage;
            this.RewardExperiencePoints = rewardExperiencePoints;
            this.RewardGold = rewardGold;

            this.LootTable = new List<LootItem>();
        }

        //Create the loot table for the monster in question
        public List<InventoryItem> CreateLootTable(Monster monster)
        {
            //Create a list to hold all the loots
            List<InventoryItem> lootList = new List<InventoryItem>();

            //Compare the monster's loot percentage to a random number to see if it will be added to the loots.
            foreach(LootItem li in LootTable)
            {
                if(RandomNumberGenerator.NumberBetween(1, 100) <= li.DropPercentage)
                {
                    lootList.Add(new InventoryItem(li.Details, 1));
                }
            }

            //If no lootItems were added to the lootList, then add the default item to the list instead.
            if (lootList.Count == 0)
            {
                //Loop to find the default item in the lootList
                foreach(LootItem li in LootTable)
                {
                    if(li.IsDefaultItem)
                    {
                        lootList.Add(new InventoryItem(li.Details, 1));
                    }
                }
            }
            //Return the complete list with the items in it.
            return lootList;
        }



    }
}
