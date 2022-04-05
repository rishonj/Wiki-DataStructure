using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

// Name & ID: Rishon Jacobs, 30048001
// Date: 9th March 2022
// Program Name: Wiki Data Structure

//A wiki prototype application for storing names, categories, structures and definitions for data structures


namespace DataStructuresWiki
{
    public partial class DataStructureWikiForm : Form
    {
        public DataStructureWikiForm()
        {
            InitializeComponent();
        }

        //8.1 Create a global 2D string array, use static variables for the dimensions (row, column).
        static int rowSize = 12;  //
        static int colSize = 4;  //
        static string[,] myWikiArray = new string[rowSize, colSize];
        int counter = 0;
        
        // 8.8 and 8.9 file name for save/load
        

        // Valid Categories
        string[] validCategories = new string[] { "Array", "List", "Tree", "Graph", "Abstract", "Hash" };
        // Valid Structures
        string[] validStructures = new string[] { "Linear", "Non-linear" };

        private void DisplayArray()
        {
            listViewNameCategory.Items.Clear();
            for (int x = 0; x < rowSize; x++)//iterate rows of 2D string array
            {
                ListViewItem listViewItem = new ListViewItem(myWikiArray[x, 0]);//add first column ("name")
                listViewItem.SubItems.Add(myWikiArray[x, 1]);//add second column ("category")
                listViewNameCategory.Items.Add(listViewItem);// add to listView to render
            }
        }

        #region List, Textbox & Form
        // 8.7 User can select a definition (Name) from the Listbox and all the information is displayed in the appropriate Textboxes
        private void listViewNameCategory_MouseClick(object sender, MouseEventArgs e)
        {
            //TODO, check array has items, has been displayed (is in memory?
            //TODO don't need to clear textboxes first, it repopulates by itself. overwrites whatever was there when select new item

            textBoxName.Text = myWikiArray[listViewNameCategory.SelectedItems[0].Index, 0];
            textBoxCategory.Text = myWikiArray[listViewNameCategory.SelectedItems[0].Index, 1];
            textBoxStructure.Text = myWikiArray[listViewNameCategory.SelectedItems[0].Index, 2];
            textBoxDefinition.Text = myWikiArray[listViewNameCategory.SelectedItems[0].Index, 3];
        }

        //	A double mouse click in the Search text box will clear the Search input box
        private void textBoxSearch_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textBoxSearch.Clear();
        }

        // 8.3	Create a CLEAR method to clear the four text boxes so a new definition can be added
        private void textBoxName_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textBoxName.Clear();
            textBoxCategory.Clear();
            textBoxStructure.Clear();
            textBoxDefinition.Clear();
        }


        private void DataStructureWikiForm_Load(object sender, EventArgs e)
        {
            textBoxSearch.Focus();
        }
        #endregion

        #region All Buttons (Open, save, delete, add, edit etc.)
        //8.5	Write the code for a Binary Search for the Name in the 2D array and display the information in the other textboxes when found
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            int startIndex = -1;
            int finalIndex = counter; // set size of data in array
            bool flag = false;
            int foundIndex = -1;

            if (listViewNameCategory.SelectedItems.Count != 0)
            {
                listViewNameCategory.SelectedItems[0].Selected = false;
            }
            if (!string.IsNullOrEmpty(textBoxSearch.Text))
            {
                while (!flag && !((finalIndex - startIndex) <= 1))
                {
                    int newIndex = (finalIndex + startIndex) / 2;
                    // The string.Compare(a,b) method compares 2 strings a and b and returns an integer value
                    // -1 if a precedes b, 0 if they are equal, 1 if a follows b
                    if (string.Compare(myWikiArray[newIndex, 0], textBoxSearch.Text) == 0)
                    {
                        foundIndex = newIndex;
                        flag = true;
                        break;
                    }
                    else
                    {
                        if (string.Compare(myWikiArray[newIndex, 0], textBoxSearch.Text) == 1)
                            finalIndex = newIndex;
                        else
                            startIndex = newIndex;
                    }
                }
                if (flag)
                {
                    textBoxName.Text = myWikiArray[foundIndex, 0];
                    textBoxCategory.Text = myWikiArray[foundIndex, 1];
                    textBoxStructure.Text = myWikiArray[foundIndex, 2];
                    textBoxDefinition.Text = myWikiArray[foundIndex, 3];
                    listViewNameCategory.Items[foundIndex].Selected = true;
                    listViewNameCategory.HideSelection = false;
                    MessageBox.Show("Item found.");
                    toolStripStatusLabel.Text = "Item found at index " + foundIndex + ".";
                }
                else
                {
                    MessageBox.Show("Item not found.");
                    toolStripStatusLabel.Text = "Item not found.";
                    ClearTextboxes();
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid item to search for.");
                toolStripStatusLabel.Text = "Error: input text box cannot be blank.";
            }
        }
        //8.4 Ensure you use a separate swap method that passes(by reference) the array element to be swapped
        public void Swap(int index)
        {
            string temp;
            for (int i = 0; i < colSize; i++)
            {
                temp = myWikiArray[index, i];
                myWikiArray[index, i] = myWikiArray[index + 1, i];
                myWikiArray[index + 1, i] = temp;
            }
        }
        //8.2	Create an ADD button that will store the information from the 4 text boxes into the 2D array
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // check for nulls in 4 x textboxes
            if ((string.IsNullOrWhiteSpace(textBoxName.Text)) || (string.IsNullOrWhiteSpace(textBoxCategory.Text))
                || (string.IsNullOrWhiteSpace(textBoxStructure.Text)) || (string.IsNullOrWhiteSpace(textBoxDefinition.Text)))
            {
                toolStripStatusLabel.Text = "Error: All attributes are must be filled in to Add an item.";
            }
            else
            {
                // confirm Add with dialog
                DialogResult dr = MessageBox.Show("Do you want to add this data structure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr != DialogResult.Yes)
                {
                    toolStripStatusLabel.Text = "Add cancelled.";
                }
                else
                {
                    if (!IsValidCategory(textBoxCategory.Text) || !IsValidStructure(textBoxStructure.Text))
                    { return; }
                    else
                    {
                        for (int x = 0; x < rowSize; x++)
                        {
                            if (myWikiArray[x, 0] == "~")
                            {
                                myWikiArray[x, 0] = textBoxName.Text;
                                myWikiArray[x, 1] = textBoxCategory.Text;
                                myWikiArray[x, 2] = textBoxStructure.Text;
                                myWikiArray[x, 3] = textBoxDefinition.Text;
                                break;
                            }
                            else
                            {
                                toolStripStatusLabel.Text = "Error: Delete an existing data structure to create space for a new one.";
                            }
                        }

                        //TODO sort, does it itself automatically?
                        DisplayArray();

                        toolStripStatusLabel.Text = "Item has been added.";
                    }
                }
            }// end confirmation with dialog box to add
            ClearTextBoxes();
        }// end buttonAdd_Click()

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            // Check an item is selected in ListView
            if (listViewNameCategory.SelectedItems.Count == 0)
            {
                toolStripStatusLabel.Text = "Select item from the list, change the information and then click 'Edit'.";
            }
            else
            {
                // check for nulls in 4 x textboxes
                if ((string.IsNullOrWhiteSpace(textBoxName.Text)) || (string.IsNullOrWhiteSpace(textBoxCategory.Text))
                    || (string.IsNullOrWhiteSpace(textBoxStructure.Text)) || (string.IsNullOrWhiteSpace(textBoxDefinition.Text)))
                {
                    toolStripStatusLabel.Text = "NOTE: All attributes are must be filled in to Edit an item.";
                }
                else
                {
                    // confirm Edit with dialog
                    DialogResult dr = MessageBox.Show("Do you want to edit this Data Structure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr != DialogResult.Yes)
                    {
                        toolStripStatusLabel.Text = "Edit cancelled.";
                    }
                    else
                    {
                        if (!IsValidCategory(textBoxCategory.Text) || !IsValidStructure(textBoxStructure.Text))
                        { return; }
                        else
                        {
                            int currentRecord = listViewNameCategory.SelectedIndices[0];
                            myWikiArray[currentRecord, 0] = textBoxName.Text;
                            myWikiArray[currentRecord, 1] = textBoxCategory.Text;
                            myWikiArray[currentRecord, 2] = textBoxStructure.Text;
                            myWikiArray[currentRecord, 3] = textBoxDefinition.Text;

                            //TODO sort, does it itself automatically?
                            DisplayArray();

                            toolStripStatusLabel.Text = "Item has been updated.";
                        }
                    }
                }//end check for null in TextBoxes
            } //end check have Selected Item in ListView
            ClearTextBoxes();
        } //end buttonEdit_Click()
        private void buttonTest_Click_1(object sender, EventArgs e)
        {
            //Load Test Data
            myWikiArray[0, 0] = "Array";
            myWikiArray[0, 1] = "Array";
            myWikiArray[0, 2] = "Linear";
            myWikiArray[0, 3] = "Description";
            myWikiArray[1, 0] = "Two Dimension Array";
            myWikiArray[1, 1] = "Array";
            myWikiArray[1, 2] = "Linear";
            myWikiArray[1, 3] = "Description";
            myWikiArray[2, 0] = "List";
            myWikiArray[2, 1] = "List";
            myWikiArray[2, 2] = "Linear";
            myWikiArray[2, 3] = "Description";
            myWikiArray[3, 0] = "Linked List";
            myWikiArray[3, 1] = "List";
            myWikiArray[3, 2] = "Linear";
            myWikiArray[3, 3] = "Description";
            myWikiArray[4, 0] = "Self-Balance Tree";
            myWikiArray[4, 1] = "Tree";
            myWikiArray[4, 2] = "Non-Linear";
            myWikiArray[4, 3] = "Description";
            myWikiArray[5, 0] = "Heap";
            myWikiArray[5, 1] = "Tree";
            myWikiArray[5, 2] = "Non-Linear";
            myWikiArray[5, 3] = "Description";
            myWikiArray[6, 0] = "Binary Search Tree";
            myWikiArray[6, 1] = "Tree";
            myWikiArray[6, 2] = "Non-Linear";
            myWikiArray[6, 3] = "Description";
            myWikiArray[7, 0] = "Graph";
            myWikiArray[7, 1] = "Graph";
            myWikiArray[7, 2] = "Non-Linear";
            myWikiArray[7, 3] = "Description";
            myWikiArray[8, 0] = "Set";
            myWikiArray[8, 1] = "Abstract";
            myWikiArray[8, 2] = "Non-Linear";
            myWikiArray[8, 3] = "Description";
            myWikiArray[9, 0] = "Queue";
            myWikiArray[9, 1] = "Abstract";
            myWikiArray[9, 2] = "Linear";
            myWikiArray[9, 3] = "Description";
            myWikiArray[10, 0] = "Stack";
            myWikiArray[10, 1] = "Abstract";
            myWikiArray[10, 2] = "Linear";
            myWikiArray[10, 3] = "Description";
            myWikiArray[11, 0] = "Hash Table";
            myWikiArray[11, 1] = "Hash";
            myWikiArray[11, 2] = "Non-Linear";
            myWikiArray[11, 3] = "Description";
            DisplayArray();
        }
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            // Check an item is selected in ListView
            if (listViewNameCategory.SelectedItems.Count == 0)
            {
                toolStripStatusLabel.Text = "NOTE: No Data Structure selected to Delete. Select a Data Structure first.";
            }
            else
            {
                // confirm Delete with dialog
                DialogResult dr = MessageBox.Show("Do you want to delete this Data Structure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr != DialogResult.Yes)
                {
                    toolStripStatusLabel.Text = "Delete cancelled.";
                }
                else
                {
                    int currentRecord = listViewNameCategory.SelectedIndices[0];
                    string oldName = myWikiArray[listViewNameCategory.SelectedItems[0].Index, 0];

                    myWikiArray[currentRecord, 0] = "~";
                    myWikiArray[currentRecord, 1] = "";
                    myWikiArray[currentRecord, 2] = "";
                    myWikiArray[currentRecord, 3] = "";


                    //TODO sort, does it itself automatically?
                    DisplayArray();

                    toolStripStatusLabel.Text = oldName + " successfully deleted.";

                }
            } 
        }

        //8.3 Create a CLEAR method to clear the four text boxes so a new definition can be added
        private void buttonClear_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
        }
        //8.4	Write the code for a Bubble Sort method to sort the 2D array by Name ascending
        private void buttonSort_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < rowSize - 1; x++)
            {
                for (int i = 0; i < rowSize - 1; i++)
                {
                    if (!(string.IsNullOrEmpty(myWikiArray[i + 1, 0])))
                    {
                        if (string.CompareOrdinal(myWikiArray[i, 0], myWikiArray[i + 1, 0]) > 0)
                        {
                            Swap(i);
                        }
                    }
                }
            }
        }

        // 8.9 Create a LOAD button that will read the information from a binary file
        // called "definitions.dat" into the 2D array
        private void buttonOpen_Click(object sender, EventArgs e)
        {
            BinaryReader br;
            int x = 0;
            listViewNameCategory.Items.Clear();
            try
            {
                br = new BinaryReader(new FileStream("definitions.dat", FileMode.Open));
            }
            catch (Exception fe)
            {
                MessageBox.Show(fe.Message + "\n Cannot open file for reading");
                return;
            }
            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                try
                {
                    myWikiArray[x, 0] = br.ReadString();
                    myWikiArray[x, 1] = br.ReadString();
                    myWikiArray[x, 2] = br.ReadString();
                    myWikiArray[x, 3] = br.ReadString();
                    x++;
                    toolStripStatusLabel.Text = "File loaded successfully.";
                }

                catch (Exception fe)
                {
                    MessageBox.Show("Cannot read data from file or EOF" + fe);
                    break;
                }
                counter = x;
                DisplayArray();
            }
            br.Close();
        }
    

        // 8.8 Create a SAVE button so the information from the 2D array can be written into
        // a binary file called "definitions.dat" which is sorted by Name
        private void buttonSave_Click(object sender, EventArgs e)
        {
            BinaryWriter bw;
            try
            {
                bw = new BinaryWriter(new FileStream("definitions.dat", FileMode.Create));
            }
            catch (Exception fe)
            {
                MessageBox.Show(fe.Message + "\n Cannot append to file.");
                return;
            }
            try
            {
                for (int i = 0; i < counter; i++)
                {
                    bw.Write(myWikiArray[i, 0]);
                    bw.Write(myWikiArray[i, 1]);
                    bw.Write(myWikiArray[i, 2]);
                    bw.Write(myWikiArray[i, 3]);
                }
                toolStripStatusLabel.Text = "File saved successfully.";
            }
            catch (Exception fe)
            {
                MessageBox.Show(fe.Message + "\n Cannot write data to file.");
                return;
            }
            bw.Close();
        }
        #endregion

        #region Utilities
        public void ClearTextboxes()
        {
            textBoxName.Text = "";
            textBoxCategory.Text = "";
            textBoxStructure.Text = "";
            textBoxDefinition.Text = "";
            textBoxSearch.Text = "";
            if (listViewNameCategory.SelectedItems.Count != 0)
            {
                listViewNameCategory.SelectedItems[0].Selected = false;
            }
        }
      

        private bool IsValidCategory(string categoryValue)
        {

            //(validCategories.Any(s => categoryValue.Contains(s))) alternate code, same as below
            //if (validCategories.Any(categoryValue.Contains))
            // cater for case invariance
            if (!validCategories.Any(s => s.IndexOf(categoryValue, StringComparison.CurrentCultureIgnoreCase) > -1))
            {
                //TODO use MsgBox, or StatusStrip? When to put up msg (in coordination with outer Method)?
                MessageBox.Show("Invalid Category entered.\nValid Categories: 'Array', 'List', 'Tree', 'Grpah', 'Abstract' and 'Hash'", "Invalid Category");
                textBoxCategory.Focus();
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsValidStructure(string structureValue)
        {
            // cater for case invariance
            if (!validStructures.Any(s => s.IndexOf(structureValue, StringComparison.CurrentCultureIgnoreCase) > -1))
            {
                //TODO use MsgBox, or StatusStrip? When to put up msg (in coordination with outer Method)?
                MessageBox.Show("Invalid Structure entered.\nValid Structures: 'Linear' and 'Non-Linear'", "Invalid Structure");
                textBoxStructure.Focus();
                return false;
            }
            else
            {
                return true;
            }
        }

        private void ClearTextBoxes()
        {
            textBoxName.Clear();
            textBoxCategory.Clear();
            textBoxStructure.Clear();
            textBoxDefinition.Clear();
        }

        private void SearchBoxClear()
        {
            textBoxSearch.Focus();
            textBoxSearch.Clear();
        }
        #endregion

     
    }
}