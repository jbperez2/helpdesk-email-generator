using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using HelpDesk_Submit.Properties;

namespace HelpDesk_Submit
{
    public partial class Form1 : Form
    {
        ArrayList numbersCITY1 = new ArrayList();
        ArrayList numbersCITY2 = new ArrayList();
        ArrayList numbersCITY3 = new ArrayList();
        ArrayList numbersREMOTE = new ArrayList();
        int lastLocation;

        public string[] configs = new string[]
        {

            "Item 1",    // 0
            "Item 2",  // 1
            "Item 3", // 2
            "Item 4",     // 3
            "Item 5", // 4
            "Item 6",    // 5
            "Item 7",    // 6
            "Item 8",// 7
            "Item 9", // 8
            "Item 10", // 9
            "Item 11",  // 10
            "Item 12", // 11
            "Item 13",  // 12
            "Item 14",   // 13
            "Item 15",  // 14
            "Item 16",      // 15
            "Item 17",   // 16
            "Item 18", // 17
            "Item 19", // 18
            "Item 20",  // 19
            "Item 21",   // 20
            "Item 22",  // 21
            "Item 23",  // 22
            "Other"     // 23
        };

        public string splitLine(string input)
        {
            string result = "";
            string checkString = input;

            while(checkString.Length > 70)
            {
                int i = 70;
                while(checkString.ElementAt(i) != ' ')
                {
                    i--;
                    if(i == -1)
                    {
                        i = 60;
                        break;
                    }
                }
                result += checkString.Substring(0, i) + "%0d%0A                                         ";
                
                checkString = checkString.Substring((i+1), (checkString.Length - (i + 1)));
            }
            result += checkString + "%0d%0A";

            return result;
        }

        public Form1()
        {
            InitializeComponent();
            urgentBox.SelectedIndex = 3;

            machineBox.Text = Environment.MachineName;

            numbersCITY1 = Settings.Default.savedNumbersCITY1;
            numbersCITY2 = Settings.Default.savedNumbersCITY2;
            numbersCITY3 = Settings.Default.savedNumbersCITY3;
            numbersREMOTE = Settings.Default.savedNumbersREMOTE;
            lastLocation = Settings.Default.lastLocation;

            if (lastLocation != -1)
            {
                assignmentBox.SelectedIndex = lastLocation;
            }
            else
            {
                assignmentBox.SelectedIndex = -1;
            }
        }

        //function to change text size given a int input
        private void changeTextSize(int newSize)
        {
            //officeLabel.Text = new Font(FontFamily.Families, newSize);
        }

        //opens email application with forms completed
        private void submitButton_Click(object sender, EventArgs e)
        {
            //bool that determines if any empty fields are present
            bool pass = true;

            //strings used to alert user which fields are empty
            string cError = "", pError = "", mError = "";

            //if-statements that checks for any empty fields
            //set pass = false and strings are renamed if any are empty
            if(configBox.Text == "")
            {
                cError = "Configuration ";
                pass = false;
            }
            if(phoneBox.Text == "" || phoneBox.Text == "602-555-" 
                || phoneBox.Text == "623-555-" || phoneBox.Text == "480-555-")
            {
                if (cError == "")
                {
                    pError += "Phone Number";
                }
                //add a comma and space if there are more than one error
                else
                {
                    pError += ", Phone Number";
                }
                pass = false;
            }
            if(messageBox.Text == "")
            {
                if (cError == "" && pError == "")
                {
                    mError += "Message";
                }
                //add a comma and space if there are more than one error
                else
                {
                    mError += ", Message";
                }
                pass = false;
            }

            //if pass = false, show message box with error message
            if (!pass)
            {
                MessageBox.Show("Please fill out the following field(s): "
                    + cError + pError + mError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //else, gather information and open Mail
            else {
                //grab text from all text fields/comboboxes
                string phone = phoneBox.Text;
                string location = assignmentBox.Text;
                string issue = configBox.Text;
                string message = messageBox.Text;
                string urgent = urgentBox.Text;

                //create string for mailto command
                string completeEmail = String.Empty;

                //assign string
                string assign = "";

                string cc = "";

                string localSupport = "";

                //string for reading message box
                string allText = "";

                //if-else statements that return proper assignments for each location. DEFAULT = CDI Helpdesk
                if (assignmentBox.Text == "City 1")
                {
                    assign += "[Local Place 1]";
                    cc += "cc=local1_1@email.com; local1_2@email.com&";
                }
                else if (assignmentBox.Text == "City 2" || assignmentBox.Text == "City 3")
                {
                    assign += "[Local Place 2/3]";
                    cc += "cc=local23_1@email.com; local23_2@email.com&";
                }
                else if (assignmentBox.Text == "Remote")
                {
                    assign += "Remote";
                    //cc += "cc=CDI-ESITSupport@cdicorp.com";
                }
                else
                {
                    assign += "Remote";
                    //cc += "cc=CDI-ESITSupport@cdicorp.com";
                }
                
                if(location != "Remote")
                {
                    localSupport += "If local support is required, assign to:           " + assign + "%0d%0A%0d%0A%0d%0A%0d%0A";
                }
                else
                {
                    localSupport += "%0d%0A";
                }

                //Urgency Default = Low
                if(urgent == "")
                {
                    urgent = "Low";
                }

                //If there are newlines in messageBox, read lines individually and manually add newline
                if (messageBox.Lines.Length > 0)
                {

                    for (int i = 0; i < messageBox.Lines.Length; i++)
                    {
                        if(i != 0)
                        {
                            allText += "                      ";
                        }
                        allText += "                   ";
                        if(messageBox.Lines[i].Length > 60)
                        {
                            allText += splitLine(messageBox.Lines[i]) + "%0d%0A";
                        }
                        else
                        {
                            allText += messageBox.Lines[i] + "%0d%0A";
                        }
                        
                        //allText += messageBox.Lines[i] + "%0d%0A";
                    }
                }

                //complete mailto command with proper formatting.
               
                    completeEmail += "mailto:HelpDeskEmail@email.com?" + cc + "subject="
                        + "Help Desk Request: " + issue + " Issue for " + location
                        + "&body=Short Description:" + "        " + "Help Desk Request: "
                        + issue + " Issue for " + location + "%0d%0AConfiguration Item:" + "     " + issue
                        + "%0d%0AUrgency:" + "                        " + urgent
                        + "%0d%0APhone Number:" + "            " + phone
                        + "%0d%0AWorkstation Name:" + "     " + Environment.MachineName
                        + "%0d%0A%0d%0ADescription:" + allText + "%0d%0A%0d%0A"
                        + localSupport;

                String addition = phoneBox.Text;

                //save number in phone number field
                if (location == "City 1")
                {
                    if (numbersCITY1 == null)
                    {
                        numbersCITY1 = new ArrayList();
                        numbersCITY1.Add(addition);
                    }
                    else if (numbersCITY1.Contains(addition))
                    {
                        numbersCITY1.RemoveAt(numbersCITY1.IndexOf(addition));
                        numbersCITY1.Insert(0, addition);
                    }
                    else
                    {
                        numbersCITY1.Insert(0, addition);
                    }

                    //update the program settings
                    Settings.Default.savedNumbersCITY1 = numbersCITY1;
                }
                else if (location == "City 2")
                {
                    if (numbersCITY2 == null)
                    {
                        numbersCITY2 = new ArrayList();
                        numbersCITY2.Add(addition);
                    }
                    else if (numbersCITY2.Contains(addition))
                    {
                        numbersCITY2.RemoveAt(numbersCITY2.IndexOf(addition));
                        numbersCITY2.Insert(0, addition);
                    }
                    else
                    {
                        numbersCITY2.Insert(0, addition);
                    }
                    Settings.Default.savedNumbersCITY2 = numbersCITY2;
                }
                else if (location == "City 3")
                {
                    if (numbersCITY3 == null)
                    {
                        numbersCITY3 = new ArrayList();
                        numbersCITY3.Add(addition);
                    }
                    else if (numbersCITY3.Contains(addition))
                    {
                        numbersCITY3.RemoveAt(numbersCITY3.IndexOf(addition));
                        numbersCITY3.Insert(0, addition);
                    }
                    else
                    {
                        numbersCITY3.Insert(0, addition);
                    }

                    //update the program settings
                    Settings.Default.savedNumbersCITY3 = numbersCITY3;
                }
                else
                {
                    if (numbersREMOTE == null)
                    {
                        numbersREMOTE = new ArrayList();
                        numbersREMOTE.Add(addition);
                    }
                    else if (numbersREMOTE.Contains(addition))
                    {
                        numbersREMOTE.RemoveAt(numbersREMOTE.IndexOf(addition));
                        numbersREMOTE.Insert(0, addition);
                    }
                    else
                    {
                        numbersREMOTE.Insert(0, addition);
                    }

                    Settings.Default.savedNumbersREMOTE = numbersREMOTE;
                }

                lastLocation = assignmentBox.SelectedIndex;

                Settings.Default.lastLocation = lastLocation;

                Settings.Default.Save();

                //open Mail
                System.Diagnostics.Process.Start(completeEmail);
            }
        }

        //Application closes on cancel button
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        //When assignmentBox is changed, the proper configBox options are displayed
        private void locationBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (assignmentBox.Text == "City 1")
            {

                configBox.DataSource = new[]
                {
                    configs[0],
                    configs[1],
                    configs[2],
                    configs[3],
                    configs[4],
                    configs[5],
                    configs[6],
                    configs[9],
                    configs[10],
                    configs[11],
                    configs[12],
                    configs[13],
                    configs[14],
                    configs[15],
                    configs[16],
                    configs[17],
                    configs[19],
                    configs[20],
                    configs[21]
                };
                //phoneBox.Text = "602-555-";
                configBox.SelectedIndex = -1;
            }
            else if(assignmentBox.Text == "City 2" || assignmentBox.Text == "City 3")
            {
                configBox.DataSource = new[]
                {
                    configs[0],
                    configs[1],
                    configs[3],
                    configs[4],
                    configs[5],
                    configs[7],
                    configs[8],
                    configs[9],
                    configs[10],
                    configs[11],
                    configs[12],
                    configs[13],
                    configs[14],
                    configs[15],
                    configs[18],
                    configs[19],
                    configs[20],
                    configs[21]
                };
               /* if (assignmentBox.Text == "City 2")
                {
                    phoneBox.Text = "623-555-";//springdale
                }
                else
                {
                    phoneBox.Text = "480-555-";
                }*/
                configBox.SelectedIndex = -1;

            }
            else if(assignmentBox.Text == "Remote")
            {
                configBox.DataSource = new[]
                {
                    configs[4],
                    configs[5],
                    configs[20],
                    configs[21]
                };
                //phoneBox.Text = "";
                configBox.SelectedIndex = -1;
            }
        }

        private void clearHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "You are about to clear all previous phone number entries.\n\nContinue?"
                    , "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                Settings.Default.Reset();
                assignmentBox.SelectedIndex = -1;
                phoneBox.Text = "";
                messageBox.Text = "";
                urgentBox.SelectedIndex = 3;
                machineBox.Text = Environment.MachineName;
                configBox.SelectedIndex = -1;
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
