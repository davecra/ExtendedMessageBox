using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExtendedMessageBoxLibrary;

namespace ExtendedMessageBoxTestbed
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ExtendedDialogResult LobjResult = 
            ExtendedMessageBox.Show(textBox1.Text, 
                                    textBox2.Text, 
                                    (MessageBoxButtons)Enum.Parse(typeof(MessageBoxButtons),comboBox1.Text), 
                                    (MessageBoxIcon)Enum.Parse(typeof(MessageBoxIcon), comboBox2.Text), 
                                    textBoxCheck.Text,
                                    new Hyperlink(textBox4.Text, textBox3.Text),
                                    textBox5.Text,
                                    trackBar1.Value);

           textBoxResult.Text = "You clicked: " + LobjResult.Result.ToString() +
                                (!string.IsNullOrEmpty(textBoxCheck.Text) ? "\r\nAnd the Checkbox was " +
                                (LobjResult.IsChecked ? "checked" : "unchecked") : "") +
                                (LobjResult.TimedOut?"\r\nAnd the dialog timed out on its own.":"");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (MessageBoxButtons item in Enum.GetValues(typeof(MessageBoxButtons)))
            {
                comboBox1.Items.Add(item.ToString());
            }
            comboBox1.SelectedIndex = 1;

            foreach (MessageBoxIcon item in Enum.GetValues(typeof(MessageBoxIcon)))
            {
                if(!comboBox2.Items.Contains(item.ToString()))
                    comboBox2.Items.Add(item.ToString());
            }
            comboBox2.SelectedIndex = 1;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            labelSeconds.Text = trackBar1.Value.ToString() + " second(s)";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBoxResult.Text = "";
        }
    }
}
