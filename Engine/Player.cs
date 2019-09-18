using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    public class Player : LivingCreature
    {
        public int Gold { get; set; }
        public int ExperiencePoints { get; set; }
        public int Level { get; set; }
        public Location CurrentLocation { get; set; }
        public List<InventoryItem> Inventory { get; set; }
        public List<PlayerQuest> Quests { get; set; }


        public Player(int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints, int level) : base(currentHitPoints, maximumHitPoints)
        {
            this.Gold = gold;
            this.ExperiencePoints = experiencePoints;
            this.Level = level;

            Inventory = new List<InventoryItem>();
            Quests = new List<PlayerQuest>();
        }

        public bool HasRequiredItemToEnterThisLocation(Location location)
        {
            if (location.ItemRequiredToEnter == null)
            {
                //There is no required item for this location, so return "true"
                return true;
            }
            //see if the player has the required item in their inventory
            foreach (InventoryItem ii in Inventory)
            {
                if (ii.Details.ID == location.ItemRequiredToEnter.ID)
                {
                    //We found the required item, so return "true"
                    return true;
                }
            }
            //Didn't find the item in the player's inventory so return false.
            return false;
        }

        public bool HasThisQuest(Quest quest)
        {

                    foreach (PlayerQuest pq in Quests)
                    {
                        if (pq.Details.ID == quest.ID)
                        {
                            return true;
                        }
                    }
                    return false;

        }

        public bool CompletedThisQuest(Quest quest)
        {
            foreach (PlayerQuest pq in Quests)
            {
                if (pq.Details.ID == quest.ID)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasAllQuestCompletionItems(Quest quest)
        {
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                //See if the player has all the items needed to complete the quest here
                bool foundItemInPlayersInventory = false;

                //Check each item in the player's inventory to see if they have it, and enough of it.
                foreach (InventoryItem ii in Inventory)
                {
                    //If the item is found in the player's inventory
                    if (ii.Details.ID == qci.Details.ID)
                    {
                        foundItemInPlayersInventory = true;
                        //the player does not have enough of this item to complete the quest
                        if (ii.Quantity < qci.Quantity)
                        {
                            return false;
                        }
                    }
                }

                //If the player doesn't have the item at all
                if (!foundItemInPlayersInventory)
                {
                    return false;
                }
            }
            //If all the checks failed, then player has the item and has enough of it.
            return true;
        }

        public void RemoveQuestCompletionItems(Quest quest)
        {
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                foreach (InventoryItem ii in Inventory)
                {
                    if (ii.Details.ID == qci.Details.ID)
                    {
                        //substract the quantity of items needed to complete the quest from the player's inventory.
                        ii.Quantity -= qci.Quantity;
                        break;
                    }
                }
            }
        }

        public void AddItemToInventory(Item itemToAdd)
        {
            foreach (InventoryItem ii in Inventory)
            {
                if (ii.Details.ID == itemToAdd.ID)
                {
                    //If they have the item in their inventory, add 1 to its quantity
                    ii.Quantity++;
                    return;
                }
            }
            //If they didn't have the item in their inventory add it to the inventory, with a quantity of 1
            Inventory.Add(new InventoryItem(itemToAdd, 1));
        }

        public void MarkQuestCompleted(Quest quest)
        {
            //Find the quest in the player's quest list
            foreach (PlayerQuest pq in Quests)
            {

                if (pq.Details.ID == quest.ID)
                {
                    //Mark it as completed
                    pq.IsCompleted = true;
                    //We found the quest and marked it complete so get out of the function.
                    return;
                }
            }
        }


        public void ObtainExperiencePoints(int amount)
        {
            this.ExperiencePoints += amount;
        }

        public void ObtainGold(int amount)
        {
            this.Gold += amount;
        }

        public void ObtainMonsterLoots(List<InventoryItem> loots)
        {
            foreach(InventoryItem ii in loots)
            {
                AddItemToInventory(ii.Details);
            }
                
        }
    }
}
