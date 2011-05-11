using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PcPv2
{
    public partial class Dummy : Form
    {

        string filename = "";

        public Dummy(string newFilename)
        {
            filename = newFilename;
            InitializeComponent();
        }

        // this dummy forms loads the real form because debut
        // likes to hide the starting window of any external app
        private void DummyForm_Load(object sender, EventArgs e)
        {
            InputForm form = new InputForm(filename);
            form.ShowDialog();
            Close();
        }
    }
}
