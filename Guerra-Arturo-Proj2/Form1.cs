/*
	Developer: Guerra,Arturo
	Course: MIS 4321 – Spring 2017
	Assignment: Project #2 - Nutrition Planner  
	Description: A diet planner to track meals and nutrients. It also suggests a new item if a goal is met.
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Guerra_Arturo_Proj2
{

    public partial class Form1 : Form
    {

        //Variables
        const int TEST_CALORIES = 1500;
        const int TEST_CARBS = 180;
        const int TEST_PROTEIN = 60;
        const int TEST_FAT = 70;

        #region Arrays 
        public string[] MenuItems = new string[] 
            {
            "Quarter Lb Burger",
            "Third Lb Burger",
            "Half Lb Burger",
            "Club Sandwich",
            "Grilled Chicken",
            "Philly Steak Sandwich",
            "Chicken Wrap",
            "Small Fries",
            "Medium Fries",
            "Large Fries",
            "Egg Roll",
            "Fruit Cup",
            "Cookies",
            "Cola",
            "Diet Cola",
            "Ice Cream Shake",
            "Apple Juice"
            };

        public int[,] NutritionalValues = new int[,] 
            {
            { 300, 33, 20, 9 },
            { 510, 40, 29, 26 },
            { 790, 63, 45, 39 },
            { 420, 51, 32, 9 },
            { 400, 39, 24, 16 },
            { 410, 27, 20, 24 },
            { 160, 31, 4, 2 },
            { 230, 30, 2, 11 },
            { 340, 45, 4, 16 },
            { 510, 67, 6, 24 },
            { 260, 43, 4, 8 },
            { 70, 15, 0, 1 },
            { 380, 48, 4, 19 },
            { 220, 55, 0, 0 },
            { 1, 0, 0, 0 },
            { 210, 52, 0, 0 },
            { 100, 25, 0, 0 }
            };
        #endregion

        public int intCaloriesGoal;
        public int intCarbsGoal;
        public int intFatGoal;
        public int intProteinGoal;
        public int intTotalCalories;
        public int intTotalCarbs;
        public int intTotalProtein;
        public int intTotalFat;
        public int FlaggedGoals;




        public Form1()
        {
            InitializeComponent();
        }

        //This sets the values to 0 when the form loads
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadMenuButtons();
            txtTotalCalories.Text = "0";
            txtTotalCarbs.Text = "0";
            txtTotalProtein.Text = "0";
            txtTotalFat.Text = "0";
            FlaggedGoals = 0;
        }

        //Clears the table only
        private void btnClearSelection_Click(object sender, EventArgs e)
        {
            dgvFoodTable.Rows.Clear();
            intTotalCalories = 0;
            intTotalCarbs = 0;
            intTotalProtein = 0;
            intTotalFat = 0;

            txtTotalCalories.Text = "0";
            txtTotalCalories.BackColor = Color.White;

            txtTotalCarbs.Text = "0";
            txtTotalCarbs.BackColor = Color.White;

            txtTotalProtein.Text = "0";
            txtTotalProtein.BackColor = Color.White;

            txtTotalFat.Text = "0";
            txtTotalFat.BackColor = Color.White;
            btnSuggestItem.Enabled = false;


        }

        //Clears the table AND the goals, it also disables the buttons
        private void btnClearAll_Click(object sender, EventArgs e)
        {

            intTotalCalories = 0;
            intTotalCarbs = 0;
            intTotalProtein = 0;
            intTotalFat = 0;

            intCaloriesGoal = 0;
            intCarbsGoal = 0;
            intProteinGoal = 0;
            intFatGoal = 0;


            dgvFoodTable.Rows.Clear();

            txtTotalCalories.Text = "0";
            txtTotalCalories.BackColor = Color.White;

            txtTotalCarbs.Text = "0";
            txtTotalCarbs.BackColor = Color.White;

            txtTotalProtein.Text = "0";
            txtTotalProtein.BackColor = Color.White;

            txtTotalFat.Text = "0";
            txtTotalFat.BackColor = Color.White;


            lblCalories.Text = "";
            lblCalories.Visible = false;

            lblCarbs.Text = "";
            lblCarbs.Visible = false;

            lblProtein.Text = "";
            lblProtein.Visible = false;

            lblFat.Text = "";
            lblFat.Visible = false;
            btnSuggestItem.Enabled = false;

           
        }

        //Deletes the selected row, sometimes you have to click a few times since it has trouble registering the click
        //This button is only enabled if a row is highlighted.
        private void btnDeleteItem_Click(object sender, EventArgs e)
        {
            
                int rowSelected;

                rowSelected = dgvFoodTable.CurrentCellAddress.Y;

                intTotalCalories -= Convert.ToInt32(dgvFoodTable.Rows[rowSelected].Cells["calories"].Value);
                txtTotalCalories.Text = intTotalCalories.ToString();
                Flag(txtTotalCalories, intCaloriesGoal);

                intTotalCarbs -= Convert.ToInt32(dgvFoodTable.Rows[rowSelected].Cells["carbs"].Value);
                txtTotalCarbs.Text = intTotalCarbs.ToString();
                Flag(txtTotalCarbs, intCarbsGoal);

                intTotalProtein -= Convert.ToInt32(dgvFoodTable.Rows[rowSelected].Cells["protein"].Value);
                txtTotalProtein.Text = intTotalProtein.ToString();
                Flag(txtTotalProtein, intProteinGoal);

                intTotalFat -= Convert.ToInt32(dgvFoodTable.Rows[rowSelected].Cells["fat"].Value);
                txtTotalFat.Text = intTotalFat.ToString();
                Flag(txtTotalFat, intFatGoal);


                if (rowSelected > -1)
                {
                    dgvFoodTable.Rows.RemoveAt(rowSelected);
                }

            dgvFoodTable.CurrentCell = null;

            btnDeleteItem.Enabled = false;
        }

        //Suggestion button
        //Works by containing the original nutrient values, then it checks every value to ensure that the goal is not exceeded.
        //Every successful check adds 1 to the value, if it reaches 4 then the item is valid. 
        //if the goal is not exceeded then it randomly generates a meal to replace, the totals are recalculated.

        private void btnSuggestItem_Click(object sender, EventArgs e)
        {

            if (dgvFoodTable.CurrentCell == null)
            {
                    MessageBox.Show("Please click on an item to replace!");
            }
            else
            {
                int rowSelected;
                int calories;
                int carbs;
                int protein;
                int fat;
                int value;
           
                List<int> validItems = new List<int>();

                rowSelected = dgvFoodTable.CurrentCellAddress.Y;

                calories = Convert.ToInt32(dgvFoodTable.Rows[rowSelected].Cells["calories"].Value);
                carbs = Convert.ToInt32(dgvFoodTable.Rows[rowSelected].Cells["carbs"].Value);
                protein = Convert.ToInt32(dgvFoodTable.Rows[rowSelected].Cells["protein"].Value);
                fat = Convert.ToInt32(dgvFoodTable.Rows[rowSelected].Cells["fat"].Value);

                //Loop to check if the goal is not overstated.
                for (int x = 0; x < NutritionalValues.GetLength(0); x++)
                {

                     value = 0;
                    if ((intTotalCalories - calories + NutritionalValues[x, 0]) < intCaloriesGoal)
                    {
                        value++;
                    }
                    else
                    {
                        value--;
                    }

                    if ((intTotalCarbs - carbs + NutritionalValues[x, 1]) < intCarbsGoal)
                    {
                        value++;
                    }
                    else
                    {
                        value--;
                    }

                    if ((intTotalProtein - protein + NutritionalValues[x, 2]) < intProteinGoal)
                    {
                        value++;
                    }
                    else
                    {
                        value--;
                    }

                    if ((intTotalFat - fat + NutritionalValues[x, 3]) < intFatGoal)
                    {
                        value++;
                    }
                    else
                    {
                        value--;
                    }

                    if(value == 4)
                    {
                        validItems.Add(x);
                    }
 

                }
                // if no goals can be met then you have to remove some items from the list
                if (validItems.Count == 0)
                {
                    MessageBox.Show("No valid items, please remove an item or adjust your goals");
                }
                else
                {
                    // lots of code that basically changes the row value to the new values
                    if (rowSelected >= 0)
                    {
                        Random rand = new Random();
                        int randomNum = rand.Next(1, validItems.Count);
                        int randomItem = validItems[randomNum];

                        string name = dgvFoodTable.Rows[rowSelected].Cells[0].Value.ToString();
                        string newName = MenuItems[randomItem];

                        
                        string msg = "Do you want to change the item \n from:  " + name + " to: " + newName + "?";

                        DialogResult dr = MessageBox.Show(msg, "Suggestion", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);

                        if (dr == DialogResult.Yes)
                        {
                            dgvFoodTable.Rows[rowSelected].Cells[0].Value = newName;
                            dgvFoodTable.Rows[rowSelected].Cells["calories"].Value = NutritionalValues[randomItem, 0];
                            dgvFoodTable.Rows[rowSelected].Cells["carbs"].Value = NutritionalValues[randomItem, 1];
                            dgvFoodTable.Rows[rowSelected].Cells["protein"].Value = NutritionalValues[randomItem, 2];
                            dgvFoodTable.Rows[rowSelected].Cells["fat"].Value = NutritionalValues[randomItem, 3];

                            intTotalCalories = 0;
                            intTotalCarbs = 0;
                            intTotalProtein = 0;
                            intTotalFat = 0;

                            for (int i = 0; i < dgvFoodTable.Rows.Count; i++)
                            {
                                intTotalCalories += Convert.ToInt32(dgvFoodTable.Rows[i].Cells["calories"].Value);
                                intTotalCarbs += Convert.ToInt32(dgvFoodTable.Rows[i].Cells["carbs"].Value);
                                intTotalProtein += Convert.ToInt32(dgvFoodTable.Rows[i].Cells["protein"].Value);
                                intTotalFat += Convert.ToInt32(dgvFoodTable.Rows[i].Cells["fat"].Value);
                            }

                            txtTotalCalories.Text = intTotalCalories.ToString();
                            txtTotalCarbs.Text = intTotalCarbs.ToString();
                            txtTotalProtein.Text = intTotalProtein.ToString();
                            txtTotalFat.Text = intTotalFat.ToString();

                            Flag(txtTotalCalories, intCaloriesGoal);
                            Flag(txtTotalCarbs, intCarbsGoal);
                            Flag(txtTotalProtein, intProteinGoal);
                            Flag(txtTotalFat, intFatGoal);

                        }
                    }
                }
            }
        }

        //Loads the goals from the user input
        //If the user leaves it blank or sets it to 0 then its considered not set and will not be taken into consideration
        private void btnLoadGoals_Click(object sender, EventArgs e)
        {
            DataValidation(txtCaloriesGoal, "Calories");
            DataValidation(txtCarbsGoal, "Carbs");
            DataValidation(txtProteinGoal, "Protein");
            DataValidation(txtFatGoal, "Fat");

            intCaloriesGoal = Convert.ToInt32(txtCaloriesGoal.Text);
            if (intCaloriesGoal== 0)
            {
                lblCalories.Visible = true;
                lblCalories.Text = "Calories: Not set";

            }
            else
            {
                lblCalories.Text = "Calories: " + intCaloriesGoal.ToString();
                lblCalories.Visible = true;
                Flag(txtTotalCalories, intCaloriesGoal);
            }


            intCarbsGoal = Convert.ToInt32(txtCarbsGoal.Text);
            if (intCarbsGoal == 0)
            {
                lblCarbs.Visible = true;
                lblCarbs.Text = "Carbs: Not set";

            }
            else
            {
                lblCarbs.Text = "Carbs: " + intCarbsGoal.ToString();
                lblCarbs.Visible = true;
                Flag(txtTotalCarbs, intCarbsGoal);
            }


            intProteinGoal = Convert.ToInt32(txtProteinGoal.Text);
            if (intProteinGoal == 0)
            {
                lblProtein.Visible = true;            
                lblProtein.Text = "Protein: Not set";

            }
            else
            {
                lblProtein.Text = "Protein: " + intProteinGoal.ToString();
                lblProtein.Visible = true;
                Flag(txtTotalProtein, intProteinGoal);
            }

            intFatGoal = Convert.ToInt32(txtFatGoal.Text);
            if (intFatGoal == 0)
            {
                lblFat.Visible = true;
                lblFat.Text = "Fat: Not set";

            }
            else
            {
                lblFat.Text = "Fat: " + intFatGoal.ToString();
                lblFat.Visible = true;
                Flag(txtTotalFat, intFatGoal);
            }

            txtCaloriesGoal.Text = "";
            txtCarbsGoal.Text = "";
            txtProteinGoal.Text = "";
            txtFatGoal.Text = "";

            txtCaloriesGoal.Focus();

            Flag(txtTotalCalories, intCaloriesGoal);
            Flag(txtTotalCarbs, intCarbsGoal);
            Flag(txtTotalProtein, intProteinGoal);
            Flag(txtTotalFat, intFatGoal);
        }

        //Dynamically loads the buttons with the names and data
        private void LoadMenuButtons()
        {
            foreach (Control c in pnlMenu.Controls)
            {
                int pos = 0;
                if (c is Button)
                {
                    pos = Convert.ToInt32(c.Tag) - 1;
                    c.Text = MenuItems[pos];
                }
            }
        }

        //When a button is pressed it adds the data to the table
        private void AddMenuSelection(object sender, EventArgs e)
        {
            int newRowNum;
            Button btn = (Button)sender;
            newRowNum = dgvFoodTable.Rows.Add();
            dgvFoodTable.Rows[newRowNum].Cells["item"].Value = MenuItems[Convert.ToInt32(btn.Tag)-1];

            dgvFoodTable.Rows[newRowNum].Cells["calories"].Value = NutritionalValues[Convert.ToInt32(btn.Tag) - 1,0];
            intTotalCalories += NutritionalValues[Convert.ToInt32(btn.Tag) - 1, 0];
            txtTotalCalories.Text = intTotalCalories.ToString();
            Flag(txtTotalCalories, intCaloriesGoal);

            dgvFoodTable.Rows[newRowNum].Cells["carbs"].Value = NutritionalValues[Convert.ToInt32(btn.Tag) - 1, 1];
            intTotalCarbs += NutritionalValues[Convert.ToInt32(btn.Tag) - 1, 1];
            txtTotalCarbs.Text = intTotalCarbs.ToString();
            Flag(txtTotalCarbs, intCarbsGoal);

            dgvFoodTable.Rows[newRowNum].Cells["protein"].Value = NutritionalValues[Convert.ToInt32(btn.Tag) - 1, 2];
            intTotalProtein += NutritionalValues[Convert.ToInt32(btn.Tag) - 1, 2];
            txtTotalProtein.Text = intTotalProtein.ToString();
            Flag(txtTotalProtein, intProteinGoal);

            dgvFoodTable.Rows[newRowNum].Cells["fat"].Value = NutritionalValues[Convert.ToInt32(btn.Tag) - 1, 3];
            intTotalFat += NutritionalValues[Convert.ToInt32(btn.Tag) - 1, 3];
            txtTotalFat.Text = intTotalFat.ToString();
            Flag(txtTotalFat, intFatGoal);

            dgvFoodTable.CurrentCell = null;

        }

        //Loads the test data with the constants, self-explanatory
        private void btnLoadTestData_Click(object sender, EventArgs e)
        {
            intCaloriesGoal = TEST_CALORIES;
            lblCalories.Text = "Calories: " + TEST_CALORIES.ToString();        
            lblCalories.Visible = true;

            intCarbsGoal = TEST_CARBS;
            lblCarbs.Text = "Carbs: " + TEST_CARBS.ToString();
            lblCarbs.Visible = true;

            intProteinGoal = TEST_PROTEIN;
            lblProtein.Text = "Protein: " + TEST_PROTEIN.ToString();        
            lblProtein.Visible = true;

            intFatGoal = TEST_FAT;
            lblFat.Text = "Fat: " + TEST_FAT.ToString();      
            lblFat.Visible = true;

            txtCaloriesGoal.Focus();

            Flag(txtTotalCalories, intCaloriesGoal);
            Flag(txtTotalCarbs, intCarbsGoal);
            Flag(txtTotalProtein, intProteinGoal);
            Flag(txtTotalFat, intFatGoal);

        }
        /// <summary>  
        ///This ensures that the user enters only numbers in the textbox, it also makes a "blank" into a 0 just in case 
        /// </summary>  
        /// <param name="txtBox"> The textbox to be checked</param>
        /// <param name="name">Name of the value being checked</param>
        private void DataValidation(TextBox txtBox, string name)
        {
            
            double val;

            if(txtBox.Text == "")
            {
                txtBox.Text = "0";
            }
            else if (!double.TryParse(txtBox.Text, out val))
            {
                MessageBox.Show(name + " is invalid. Must contain a number!");
                txtBox.Focus();        
                return;
            }

        }

        //this makes it so that the delete item button will be active only when a cell is pressed
        //probably could have done something else but I couldnt find an alternative
        private void dgvFoodTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            btnDeleteItem.Enabled = true;

        }

        /// <summary>  
        ///This variable flags the textboxes if their goals are surpassed. It also enables and disables the suggest item  button.
        /// </summary>  
        /// <param name="txt"> The textbox to be flagged</param>
        /// <param name="goal"> The goal to be met</param>

        private void Flag(TextBox txt, int goal)
        {
                
            if (Convert.ToInt32(txt.Text) > goal && goal!=0)
            {               
                txt.BackColor = Color.Salmon;

            }
            else if(Convert.ToInt32(txt.Text) < goal)
            {

                txt.BackColor = Color.White;
               
            }

            if(intCaloriesGoal < intTotalCalories || intCarbsGoal < intTotalCarbs|| intProteinGoal < intTotalProtein|| intFatGoal < intTotalFat )
            {
                btnSuggestItem.Enabled = true;

            }
            else if(intCaloriesGoal > intTotalCalories && intCarbsGoal > intTotalCarbs && intProteinGoal > intTotalProtein && intFatGoal > intTotalFat)
            {
                btnSuggestItem.Enabled = false;

            }

            if (goal == 0)
            {
                btnSuggestItem.Enabled = false;
            }


        }

    }


}
