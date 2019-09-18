using System;
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
        private void UpdateUI()
        {
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.ExperiencePoints.ToString();
            lblLevel.Text = _player.Level.ToString();
            UpdateInventoryListInUI();
            UpdateHealingPotionsListInUI();
            UpdateQuestListInUI();
            UpdateWeaponListInUI();
        }

        private void UpdateUIHitPoints()
        {
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();
        }

        private void UpdateUIGold()
        {
            lblGold.Text = _player.Gold.ToString();
        }

        private void UpdateUIExp()
        {
            lblExperience.Text = _player.ExperiencePoints.ToString();
        }

        private void UpdateUILevel()
        {
            lblLevel.Text = _player.Level.ToString();
        }



        public AdventureOfKeven()
        {
            InitializeComponent();

            _player = new Player(10, 10, 20, 0, 1);
            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            _player.AddItemToInventory(World.ItemByID(World.ITEM_ID_RUSTY_SWORD));
            _player.AddItemToInventory(World.ItemByID(World.ITEM_ID_HEALING_POTION), 2);

            //Displays the variables to string on the UI
            UpdateUI();
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
            if (!_player.HasRequiredItemToEnterThisLocation(newLocation))
            {
                rtbMessages.Text += "You must have a " + newLocation.ItemRequiredToEnter.Name + " to enter this location." + Environment.NewLine;
                return;
            }

            // Update the player's current location to the new location.
            _player.CurrentLocation = newLocation;

            // Show/hide available movement buttons depending on the new location.
            btnNorth.Visible = (newLocation.LocationToNorth != null);
            btnEast.Visible = (newLocation.LocationToEast != null);
            btnSouth.Visible = (newLocation.LocationToSouth != null);
            btnWest.Visible = (newLocation.LocationToWest != null);

            //Display current location name and description

            rtbLocation.Text = newLocation.Name + Environment.NewLine;
            rtbLocation.Text += newLocation.Description + Environment.NewLine;

            // Completely Heal the player
            _player.CurrentHitPoints = _player.MaximumHitPoints;
            //Update hit points in UI
            UpdateUIHitPoints();

            // Does the location have a quest?

            if (newLocation.QuestAvalaibleHere != null)
            {
                //Switches to keep track if the player has the quest and if its completed or not.
                bool playerAlreadyHasQuest = _player.HasThisQuest(newLocation.QuestAvalaibleHere);
                bool playerAlreadyHasCompletedQuest = _player.CompletedThisQuest(newLocation.QuestAvalaibleHere);

                // See if the player already has the quest, and if they've completed it.


                // see if the player already has the quest and if its not completed yet.
                if (playerAlreadyHasQuest)
                {
                    if (!playerAlreadyHasCompletedQuest)
                    {
                        bool playerHasAllItemsToCompleteQuest = _player.HasAllQuestCompletionItems(newLocation.QuestAvalaibleHere);

                        //When the player has all the items to complete a quest
                        if (playerHasAllItemsToCompleteQuest)
                        {
                            //Display a message
                            rtbMessages.Text += Environment.NewLine;
                            rtbMessages.Text += "You just completed the " + newLocation.QuestAvalaibleHere.Name + " quest." + Environment.NewLine;

                            //Remove the quest items from the player's inventory.
                            _player.RemoveQuestCompletionItems(newLocation.QuestAvalaibleHere);

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
                            _player.AddItemToInventory(newLocation.QuestAvalaibleHere.RewardItem);


                            //After all this, mark the quest as completed
                            _player.MarkQuestCompleted(newLocation.QuestAvalaibleHere);
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

            UpdateInventoryListInUI();
            UpdateQuestListInUI();
            UpdateWeaponListInUI();
            UpdateHealingPotionsListInUI();

        }

        private void UpdateInventoryListInUI()
        {
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
        }

        private void UpdateQuestListInUI()
        {
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
        }

        private void UpdateWeaponListInUI()
        {
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
        }

        private void UpdateHealingPotionsListInUI()
        {
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
            // get the currently selected weapon from the cboWeapons ComboBox
            Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;

            //Determine the amount of damage to do to the monster
            int damageToMonster = RandomNumberGenerator.NumberBetween(currentWeapon.MinimumDamage, currentWeapon.MaximumDamage);

            //Apply the damage to the monster's CurrentHitPoint
            _currentMonster.CurrentHitPoints -= damageToMonster;

            //Display a message to the player
            if(damageToMonster != 0)
            {
                rtbMessages.Text += "You hit the " + _currentMonster.Name + " for " + damageToMonster.ToString() + " points." + Environment.NewLine;                
            }
            else
            {
                rtbMessages.Text += "You miss the " + _currentMonster.Name + "." + Environment.NewLine;
            }

            //Check if the monster is dead
            if(!_currentMonster.isAlive())
            {
                //Monster is dead
                rtbMessages.Text += Environment.NewLine;
                rtbMessages.Text += "You defeated the " + _currentMonster.Name + "." + Environment.NewLine;

                //give the player the rewards
                _player.ObtainExperiencePoints(_currentMonster.RewardExperiencePoints);
                rtbMessages.Text += "You receive " + _currentMonster.RewardExperiencePoints.ToString() + " experience points." + Environment.NewLine;

                //Give the player gold for killing an enemy
                _player.ObtainGold(_currentMonster.RewardGold);
                rtbMessages.Text += "You receive " + _currentMonster.RewardGold.ToString() + " gold piece." + Environment.NewLine;

                //Get random loot items from the monster's loottable
                List<InventoryItem> lootTable = _currentMonster.CreateLootTable(_currentMonster);
                //Add the looted items to the player's inventory
                _player.ObtainMonsterLoots(lootTable);
                //Display the messages
                foreach(InventoryItem ii in lootTable)
                {
                    if(ii.Quantity == 1)
                    {
                        rtbMessages.Text += "You loot " + ii.Quantity.ToString() + " " + ii.Details.Name + Environment.NewLine;
                    }
                    else
                    {
                        rtbMessages.Text += "You loot " + ii.Quantity.ToString() + " " + ii.Details.NamePlural + Environment.NewLine;
                    }
                }

                //Update all the UI
                UpdateUI();

                // Add a blank line to the messages box, just for appearance.

                rtbMessages.Text += Environment.NewLine;

                // Move the the player to the current location (to heal player and create a new monster to fight)
                MoveTo(_player.CurrentLocation);
            }
            else
            {
                //Monster is still alive

                //Determine the amount of damage the monster does to the player
                int damageToPlayer = RandomNumberGenerator.NumberBetween(0, _currentMonster.MaximumDamage);

                if(damageToPlayer == 0)
                {
                    rtbMessages.Text += "The " + _currentMonster.Name + " misses you!" + Environment.NewLine;
                }
                else
                {
                    rtbMessages.Text += "The " + _currentMonster.Name + " did " + damageToPlayer.ToString() + " points of damage!" + Environment.NewLine;
                }

                //Substract the damage from the player's health.
                _player.CurrentHitPoints -= damageToPlayer;

                //Refresh Data in UI
                UpdateUIHitPoints();

                if(!_player.isAlive())
                {
                    //display message
                    rtbMessages.Text += "The " + _currentMonster.Name + " killed you " + Environment.NewLine;

                    // Move player to "home"
                    MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
                }
            }
        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {

            //Get the currently selected potion from the combobox
            HealingPotion potion = (HealingPotion)cboPotions.SelectedItem;

            //Randomize Healing amount
            int amountToHeal = RandomNumberGenerator.NumberBetween(potion.AmountToHeal, potion.AmountToHeal + 3);
            
            //Add healing amount to the player's current hitpoint
            _player.CurrentHitPoints = (_player.CurrentHitPoints + amountToHeal);

            //CurrentHitPoints cannot exceed player's MaximumHitPoints
            if(_player.CurrentHitPoints < _player.MaximumHitPoints)
            {
                _player.CurrentHitPoints = _player.CurrentHitPoints;
            }

            //remove the potion from the player's inventory
            foreach(InventoryItem ii in _player.Inventory)
            {
                if(ii.Details.ID == potion.ID)
                {
                    ii.Quantity--;
                    break;
                }
            }

            //Display message
            rtbMessages.Text += "You drink a " + potion.Name + " and regain " + amountToHeal + " Hit Points." + Environment.NewLine;

            //Monster gets their turn to attack

            //Determine the amount of damage the monster does to the player
            int damageToPlayer = _currentMonster.CalculateDamage(_currentMonster.MaximumDamage);

            //If monster misses
            if(damageToPlayer == 0)
            {
                rtbMessages.Text += "The " + _currentMonster.Name + " misses you." + Environment.NewLine;
            }

            //display DAMAGE message
            rtbMessages.Text += "The " + _currentMonster.Name + " hits you for " + damageToPlayer + " point of damage." + Environment.NewLine;

            //substract the damage from the player's current hitpoint
            _player.CurrentHitPoints -= damageToPlayer;

            //See if player is dead
            if(!_player.isAlive())
            {
                //display message
                rtbMessages.Text += "The " + _currentMonster.Name + " killed you." + Environment.NewLine;

                //Move player to "Home"
                MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            }

            //Refresh the player's data in UI
            UpdateUI();

        }
    }
}
