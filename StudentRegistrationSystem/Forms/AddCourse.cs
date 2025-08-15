using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentRegistrationSystem.Forms
{
    public partial class AddCourse : Form
    {
        // Connection string
        string connectionString = "Server=DESKTOP-3SD4HVT\\SQLEXPRESS;Database=Student;Trusted_Connection=True;";
        public AddCourse()
        {
            InitializeComponent();
        }

        private void AddCourse_Load(object sender, EventArgs e)
        {
            txtCourseID.Enabled = false; // Disable typing
            txtCourseID.ReadOnly = true; // Optional extra safety
            txtCourseID.Clear(); // Make sure it's empty for a new course

            // Populate Status combo box
            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("Available");
            cmbStatus.Items.Add("Not Available");
            cmbStatus.Items.Add("Upcoming");

            // Optional: select default value
            cmbStatus.SelectedIndex = 0;

            LoadNextCourseID(); // Load next CourseID   
        }

        private void LoadNextCourseID()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(CourseID), 0) + 1 FROM Courses", con))
                    {
                        object result = cmd.ExecuteScalar();
                        txtCourseID.Text = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Course ID: " + ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "INSERT INTO Courses (courseName, durationMonths, fee, status, description) " +
                        "VALUES (@name, @duration, @fee, @status, @desc)", con))
                    {
                        cmd.Parameters.AddWithValue("@name", txtCourseName.Text);
                        cmd.Parameters.AddWithValue("@duration", (int)numDuration.Value);
                        cmd.Parameters.AddWithValue("@fee", decimal.Parse(txtFee.Text));
                        cmd.Parameters.AddWithValue("@status", cmbStatus.Text);
                        cmd.Parameters.AddWithValue("@desc", txtDescription.Text);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Course added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK; // So AdminDashboard reloads
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            // Clear all input fields except CourseID (since it’s auto-increment)
            txtCourseName.Clear();
            numDuration.Value = 0; // or your default duration
            txtFee.Clear();
            cmbStatus.SelectedIndex = 0; // reset to first item
            txtDescription.Clear();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
