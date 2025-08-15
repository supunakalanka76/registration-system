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
    public partial class ViewStudentCourses : Form
    {
        private string _regNo;
        string connectionString = @"Data Source=DESKTOP-3SD4HVT\SQLEXPRESS;Initial Catalog=Student;Integrated Security=True";
        public ViewStudentCourses(string regNo)
        {
            InitializeComponent();
            _regNo = regNo;
        }

        private void ViewStudentCourses_Load(object sender, EventArgs e)
        {
            LoadStudentDetails();
            LoadStudentCourses();
        }

        private void LoadStudentDetails()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT firstName + ' ' + lastName AS FullName " +
                    "FROM Registration WHERE regNo = @regNo", con);
                cmd.Parameters.AddWithValue("@regNo", _regNo);

                string fullName = cmd.ExecuteScalar()?.ToString() ?? "";

                lblStudentRegNo.Text = "Reg No: " + _regNo;
                lblStudentName.Text = "Name: " + fullName;
            }
        }

        private void LoadStudentCourses()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT c.courseID, c.courseName, c.fee " +
                    "FROM StudentCourses sc " +
                    "INNER JOIN Courses c ON sc.courseID = c.courseID " +
                    "WHERE sc.regNo = @regNo", con);
                da.SelectCommand.Parameters.AddWithValue("@regNo", _regNo);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvCourses.DataSource = dt;

                // Calculate total fee
                decimal totalFee = 0;
                foreach (DataRow row in dt.Rows)
                {
                    if (row["fee"] != DBNull.Value)
                        totalFee += Convert.ToDecimal(row["fee"]);
                }
                lblTotalFee.Text = totalFee.ToString();
            }
        }
    }
}
