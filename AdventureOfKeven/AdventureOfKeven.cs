﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Engine;

namespace AdventureOfKeven
{
    public partial class AdventureOfKeven : Form
    {
        private Player _player;
        private Monster _currentMonster;


        //Methods to update the main stats
        private void updateUI()
        {
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.ExperiencePoints.ToString();
            lblLevel.Text = _player.Level.ToString();
        }

        private void updateUIHitPoints()
        {
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();
        }

        private void updateUIGold()
        {
            lblGold.Text = _player.Gold.ToString();
        }

        private void updateUIExp()
        {
            lblExperience.Text = _player.ExperiencePoints.ToString();
        }

        private void updateUILevel()
        {
            lblLevel.Text = _player.Level.ToString();
        }



        public AdventureOfKeven()
        {
            InitializeComponent();

            _player = new Player(10, 10, 20, 0, 1);
            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            _player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));

            //Displays the variables to string on the UI
            updateUI();
        }

        private void AdventureOfKeven_Load(object sender, EventArgs e)
        {

        }

        private void btnNorth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToNorth);
        }

        private void btnWest_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToWest);
        }

        private void btnEast_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToEast);
        }

        private void btnSouth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToSouth);
        }



        //Moving the player to the new location

        private void MoveTo(Location newLocation)
        {
            //Does the location have any required items?
            if (newLocation.ItemRequiredToEnter != null)
            {
                // See if the player has the required item in their inventory
                bool playerHasRequiredItem = false;

                foreach (InventoryItem ii in _player.Inventory)
                {
                    if (ii.Details.ID == newLocation.ItemRequiredToEnter.ID)
                    {
                        //Found required item
                        playerHasRequiredItem = true;
                        break; // Exit out of the foreach loop
                    }

                }

                if (!playerHasRequiredItem)
                {
                    //We couldn't find the required item in their inventory, so display a message and stop trying to move.
                    rtbMessages.Text += "You must have a " + newLocation.ItemRequiredToEnter.Name + " to enter this location." + Environment.NewLine;
                    return;
                }
            }

            // Update the player's current location to the new location.
            _player.CurrentLocation = newLocation;

            // Show/hide available movement buttons depending on the new location.
            btnNorth.Visible = (newLocation.LocationToNorth != null);
            btnEast.Visible = (newLocation.LocationToEast != null);
            btnSouth.Visible = (newLocation.LocationToSouth != null);
            btnWest.Visible = (newLocation.LocationToSouth != null);

            //Display current location name and description

            rtbLocation.Text = newLocation.Name + Environment.NewLine;
            rtbLocation.Text += newLocation.Description + Environment.NewLine;

            // Completely Heal the player
            _player.CurrentHitPoints = _player.MaximumHitPoints;
            //Update hit points in UI
            updateUIHitPoints();

            // Does the location have a quest?

            if (newLocation.QuestAvalaibleHere != null)
            {
                //Switches to keep track if the player has the quest and if its completed or not.
                bool playerAlreadyHasQuest = false;
                bool playerAlreadyHasCompletedQuest = false;

                // See if the player already has the quest, and if they've completed it.
                foreach (PlayerQuest playerQuests in _player.Quests)
                {
                    if (playerQuests.Details.ID == newLocation.QuestAvalaibleHere.ID)
                    {
                        playerAlreadyHasQuest = true;

                        if (playerQuests.IsCompleted)
                        {
                            playerAlreadyHasCompletedQuest = true;
                        }
                    }
                }


                // see if the player already has the quest and if its not completed yet.
                if (playerAlreadyHasQuest)
                {
                    if (!playerAlreadyHasCompletedQuest)
                    {
                        bool playerHasAllItemsToCompleteQuests = false;

                        //See if the player has the quest item.
                        foreach (QuestCompletionItem qci in newLocation.QuestAvalaibleHere.QuestCompletionItems)
                        {
                            bool foundItemInPlayersInventory = false;

                            foreach (InventoryItem ii in _player.Inventory)
                            {
                                //If the a the quest item is found in the player's inventory, look to see if he has the good quantity.
                                if (ii.Details.ID == qci.Details.ID)
                                {
                                    foundItemInPlayersInventory = true;
                                    //If the quantity is ok, then the player has all the items to complete the quest.
                                    if (ii.Quantity < qci.Quantity)
                                    {
                                        playerHasAllItemsToCompleteQuests = true;
                                        break;
                                    }
                                    //If the player has the item but not the good quantity, break out of the loop.
                                    break;
                                }
                            }

                            //If the item is not found in the players inventory
                            if (!foundItemInPlayersInventory)
                            {
                                //break out of the loop right away
                                break;
                            }
                        }

                        //When the player has all the items to complete a quest
                        if (playerHasAllItemsToCompleteQuests)
                        {
                            //Display a message
                            rtbMessages.Text += Environment.NewLine;
                            rtbMessages.Text += "You just completed the " + newLocation.QuestAvalaibleHere.Name + " quest." + Environment.NewLine;

                            //Remove the quest items from the player's inventory.
                            //For all the quest completion items in the questcompletion items list of the quest
                            foreach (QuestCompletionItem qci in newLocation.QuestAvalaibleHere.QuestCompletionItems)
                            {
                                //and all the items in the player's inventory
                                foreach (InventoryItem ii in _player.Inventory)
                                {
                                    //If the questcompletion items ID matches the one of the player's item.
                                    if (ii.Details.ID == qci.Details.ID)
                                    {
                                        //Substract the quantity from the player's inventory that was needed to complete the quest.
                                        ii.Quantity -= qci.Quantity;
                                        break;
                                    }
                                }
                            }


                            //Give the quest reward to the player.
                            //Displays the messages to the players to inform them of what is happening.
                            rtbMessages.Text += "You receive: " + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvalaibleHere.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvalaibleHere.RewardGold.ToString() + " gold" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvalaibleHere.RewardItem.Name + Environment.NewLine;
                            rtbMessages.Text += Environment.NewLine;

                            //Modify the values
                            _player.ExperiencePoints += newLocation.QuestAvalaibleHere.RewardExperiencePoints;
                            _player.Gold += newLocation.QuestAvalaibleHere.RewardGold;

                            //Add the reward item to the player's inventory.
                            bool addedItemToPlayerInventory = false;

                            //Look to see if they already have the items in their inventory.
                            foreach (InventoryItem ii in _player.Inventory)
                            {
                                if (ii.Details.ID == newLocation.QuestAvalaibleHere.RewardItem.ID)
                                {
                                    //Found item in player's inventory so increase the item quantity by 1.
                                    ii.Quantity++;

                                    addedItemToPlayerInventory = true;

                                    break;
                                }
                            }


                            //They didn't have the item, so add it to their inventory, with a quantity of 1
                            if (!addedItemToPlayerInventory)
                            {
                                _player.Inventory.Add(new InventoryItem(newLocation.QuestAvalaibleHere.RewardItem, 1));
                            }


                            //After all this, mark the quest as completed
                            //Find the quest in the player's quest list

                            foreach (PlayerQuest pq in _player.Quests)
                            {
                                //Mark the quest as completed.
                                pq.IsCompleted = true;
                                break;
                            }
                        }
                    }

                }
            }
            else
            {
                //The player does not already have the quest

                //Display the messages
                rtbMessages.Text += "You receive the " + newLocation.QuestAvalaibleHere.Name + " quest." + Environment.NewLine;
                rtbMessages.Text += newLocation.QuestAvalaibleHere.Description + Environment.NewLine;
                rtbMessages.Text += "To complete it, return with: " + Environment.NewLine;

                foreach (QuestCompletionItem qci in newLocation.QuestAvalaibleHere.QuestCompletionItems)
                {
                    //If the quantity required for the quest is only one, write the singular item name
                    if (qci.Quantity == 1)
                    {
                        rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.Name + Environment.NewLine;
                    }
                    //Otherwise write the plural name.
                    else
                    {
                        rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.NamePlural + Environment.NewLine;
                    }
                }
                rtbMessages.Text += Environment.NewLine;
                //Add the quest to the player's quest list
                _player.Quests.Add(new PlayerQuest(newLocation.QuestAvalaibleHere));
            }

            //Does the location have a monster?
            if (newLocation.MonsterLivingHere != null)
            {
                //Display a message
                rtbMessages.Text += "You see a " + newLocation.MonsterLivingHere.Name + Environment.NewLine;

                //Spawn a new monster to fight by creating a new object

                Monster standardMonster = World.MonsterByID(newLocation.MonsterLivingHere.ID);

                _currentMonster = new Monster(standardMonster.ID, standardMonster.Name, standardMonster.MaximumDamage, standardMonster.RewardExperiencePoints, standardMonster.RewardGold, standardMonster.CurrentHitPoints, standardMonster.MaximumHitPoints);

                foreach (LootItem li in standardMonster.LootTable)
                {
                    _currentMonster.LootTable.Add(li);
                }

                //Display all the combat combo boxes and buttons

                cboPotions.Visible = true;
                cboWeapons.Visible = true;
                btnUsePotion.Visible = true;
                btnUseWeapon.Visible = true;
            }
            //If there is no monster
            else
            {
                _currentMonster = null;

                cboPotions.Visible = false;
                cboWeapons.Visible = false;
                btnUsePotion.Visible = false;
                btnUseWeapon.Visible = false;
            }


            //Refresh the player's inventory list

            dgvInventory.RowHeadersVisible = false;

            dgvInventory.ColumnCount = 2;
            dgvInventory.Columns[0].Name = "Name";
            dgvInventory.Columns[0].Width = 197;
            dgvInventory.Columns[1].Name = "Quantity";

            dgvInventory.Rows.Clear();

            //For all the items in the player's inventory, all the details and the quantity to each rows
            foreach (InventoryItem ii in _player.Inventory)
            {
                if (ii.Quantity > 0)
                {
                    dgvInventory.Rows.Add(new[] { ii.Details.Name, ii.Quantity.ToString() });
                }
            }

            //refresh the player's quest list
            dgvQuests.RowHeadersVisible = false;

            dgvQuests.ColumnCount = 2;
            dgvQuests.Columns[0].Name = "Name";
            dgvQuests.Columns[0].Width = 197;
            dgvQuests.Columns[1].Name = "Quantity";

            dgvQuests.Rows.Clear();

            foreach (PlayerQuest pq in _player.Quests)
            {
                dgvQuests.Rows.Add(new[] { pq.Details.Name, pq.IsCompleted.ToString() });
            }

            //refresh player's Weapons list and potions

            List<Weapon> weapons = new List<Weapon>();

            foreach (InventoryItem ii in _player.Inventory)
            {
                if (ii.Details is Weapon)
                {
                    if (ii.Quantity > 0)
                    {
                        weapons.Add((Weapon)ii.Details);
                    }
                }
            }


            if (weapons.Count == 0)
            {
                //the player doesn't have any weapons, so hide the weapon combobox and the "Use" button
                cboWeapons.Visible = false;
                btnUseWeapon.Visible = false;
            }
            else
            {
                cboWeapons.DataSource = weapons;
                cboWeapons.DisplayMember = "Name";
                cboWeapons.ValueMember = "ID";

                cboWeapons.SelectedIndex = 0;
            }

            //Refresh the player's potions combobox

            List<HealingPotion> healingPotions = new List<HealingPotion>();

            foreach (InventoryItem ii in _player.Inventory)
            {
                if (ii.Details is HealingPotion)
                {
                    if (ii.Quantity > 0)
                    {
                        healingPotions.Add((HealingPotion)ii.Details);
                    }
                }
            }


            if (healingPotions.Count == 0)
            {
                //The player doesn't have any potions, so hide the potion combobox and buttons
                cboPotions.Visible = false;
                btnUsePotion.Visible = false;
            }
            else
            {
                cboPotions.DataSource = healingPotions;
                cboPotions.DisplayMember = "Name";
                cboPotions.ValueMember = "ID";

                cboPotions.SelectedIndex = 0;
            }
        }
        private void btnUseWeapon_Click(object sender, EventArgs e)
        {

        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {

        }
    }
}