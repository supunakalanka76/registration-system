using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentRegistrationSystem
{
    public partial class Courses : Form
    {
        public Courses()
        {
            InitializeComponent();
        }

        private void Courses_Load(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Redirect to Englishc.cs
            Englishc English = new Englishc();
            English.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Redirect to Softwareengc.cs
            softwareengc Software = new softwareengc();
            Software.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Redirect to Managementc.cs
            Managementc Manage = new Managementc();
            Manage.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Redirect to Accountacy.cs
            Accountacyc Account = new Accountacyc();
            Account.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Redirect to RoboticForm.cs
            RoboticForm Robotic = new RoboticForm();
            Robotic.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Redirect to DatascienceForm.cs
            DatascienceForm datascience = new DatascienceForm();
            datascience.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Redirect to AgriculForm.cs
            AgriculForm agricul= new AgriculForm();
            agricul.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // Redirect to HospitalityForm.cs
            HospitalityForm hospitality = new HospitalityForm();
            hospitality.ShowDialog();
        }
    }
}
