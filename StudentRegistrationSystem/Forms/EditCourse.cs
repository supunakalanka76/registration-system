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
    public partial class EditCourse : Form
    {
        private int courseID;

        // Connection string
        string connectionString = "Server=DESKTOP-3SD4HVT\\SQLEXPRESS;Database=Student;Trusted_Connection=True;";
        public EditCourse(int courseID)
        {
            InitializeComponent();
            this.courseID = courseID;
            LoadCourseData();
        }

        private void EditCourse_Load(object sender, EventArgs e)
        {

        }

        private void LoadCourseData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Courses WHERE courseID=@id", con))
                    {
                        cmd.Parameters.AddWithValue("@id", courseID);
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            txtCourseID.Text = reader["courseID"].ToString();
                            txtCourseName.Text = reader["courseName"].ToString();
                            numDuration.Value = Convert.ToInt32(reader["durationMonths"]);
                            txtFee.Text = reader["fee"].ToString();
                            cmbStatus.Items.Clear();
                            cmbStatus.Items.Add("Available");
                            cmbStatus.Items.Add("Not Available");
                            cmbStatus.Items.Add("Upcoming");
                            cmbStatus.SelectedItem = reader["status"].ToString();
                            txtDescription.Text = reader["description"].ToString();

                            // Disable editing CourseID
                            txtCourseID.Enabled = false;
                            txtCourseID.ReadOnly = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading course: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "UPDATE Courses SET courseName=@name, durationMonths=@duration, fee=@fee, status=@status, description=@desc " +
                        "WHERE courseID=@id", con))
                    {
                        cmd.Parameters.AddWithValue("@id", courseID);
                        cmd.Parameters.AddWithValue("@name", txtCourseName.Text);
                        cmd.Parameters.AddWithValue("@duration", (int)numDuration.Value);
                        cmd.Parameters.AddWithValue("@fee", decimal.Parse(txtFee.Text));
                        cmd.Parameters.AddWithValue("@status", cmbStatus.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@desc", txtDescription.Text);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                            MessageBox.Show("Course updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show("No course found with this ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                this.DialogResult = DialogResult.OK; // Signals AdminDashboard to reload dgvCourses
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating course: " + ex.Message);
            }
        }
    }
}
