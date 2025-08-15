using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace StudentRegistrationSystem.Forms
{
    public partial class AssignCourse : Form
    {
        string connectionString = @"Data Source=DESKTOP-3SD4HVT\SQLEXPRESS;Initial Catalog=Student;Integrated Security=True";

        public AssignCourse()
        {
            InitializeComponent();
        }

        private void AssignCourse_Load(object sender, EventArgs e)
        {
            dgvAssignedCourses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAssignedCourses.MultiSelect = false;

            LoadStudents();   // load reg numbers
            LoadCourses();    // load course list

            cmbRegNo.SelectionChangeCommitted += cmbRegNo_SelectionChangeCommitted;
        }

        private void LoadStudents()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT regNo, firstName + ' ' + lastName AS FullName FROM Registration", con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbRegNo.DataSource = dt;
                cmbRegNo.DisplayMember = "regNo";
                cmbRegNo.ValueMember = "regNo";
            }
        }
        private void cmbRegNo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmbRegNo.SelectedValue == null || cmbRegNo.SelectedValue is DataRowView)
                return;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT firstName + ' ' + lastName FROM Registration WHERE regNo=@regNo", con);
                cmd.Parameters.AddWithValue("@regNo", cmbRegNo.SelectedValue);
                lblStudentName.Text = cmd.ExecuteScalar()?.ToString() ?? "";
            }

            LoadAssignedCourses();
        }


        private void LoadAssignedCourses()
        {
            if (cmbRegNo.SelectedValue == null) return;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT sc.AssignID, c.courseID, c.courseName " +
                    "FROM StudentCourses sc " +
                    "INNER JOIN Courses c ON sc.courseID = c.courseID " +
                    "WHERE sc.regNo = @regNo", con);
                da.SelectCommand.Parameters.AddWithValue("@regNo", cmbRegNo.SelectedValue);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvAssignedCourses.DataSource = dt;
            }
        }

        private void LoadCourses()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT courseID, courseName FROM Courses", con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbCourses.DataSource = dt;
                cmbCourses.DisplayMember = "courseName";
                cmbCourses.ValueMember = "courseID";
            }
        }

        private void btnAddCourse_Click(object sender, EventArgs e)
        {
            if (cmbCourses.SelectedValue == null)
            {
                MessageBox.Show("Please select a course first.");
                return;
            }

            int selectedCourseId = Convert.ToInt32(cmbCourses.SelectedValue);
            string selectedCourseName = cmbCourses.Text;

            DataTable dt;
            if (dgvAssignedCourses.DataSource == null)
            {
                dt = new DataTable();
                dt.Columns.Add("AssignID", typeof(int));
                dt.Columns.Add("courseID", typeof(int));
                dt.Columns.Add("courseName", typeof(string));
                dgvAssignedCourses.DataSource = dt;
            }
            else
            {
                dt = (DataTable)dgvAssignedCourses.DataSource;
            }

            // Avoid duplicates
            foreach (DataRow row in dt.Rows)
            {
                if (row["courseID"] != DBNull.Value && (int)row["courseID"] == selectedCourseId)
                {
                    MessageBox.Show("This course is already assigned to the student.");
                    return;
                }
            }

            DataRow newRow = dt.NewRow();
            newRow["AssignID"] = DBNull.Value;
            newRow["courseID"] = selectedCourseId;
            newRow["courseName"] = selectedCourseName;
            dt.Rows.Add(newRow);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbRegNo.SelectedValue == null) return;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                foreach (DataGridViewRow row in dgvAssignedCourses.Rows)
                {
                    if (row.IsNewRow) continue;

                    var assignIdObj = row.Cells["AssignID"].Value;
                    if (assignIdObj == null || assignIdObj == DBNull.Value)
                    {
                        SqlCommand cmd = new SqlCommand(
                            "INSERT INTO StudentCourses (regNo, courseID) VALUES (@regNo, @courseID)", con);
                        cmd.Parameters.AddWithValue("@regNo", cmbRegNo.SelectedValue);
                        cmd.Parameters.AddWithValue("@courseID", row.Cells["courseID"].Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            MessageBox.Show("Courses assigned successfully!");
            LoadAssignedCourses();
        }

        private void btnRemove_Click_1(object sender, EventArgs e)
        {
            if (dgvAssignedCourses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a course to remove.");
                return;
            }

            DataGridViewRow selectedRow = dgvAssignedCourses.SelectedRows[0];

            DialogResult result = MessageBox.Show("Are you sure you want to remove the selected course?",
                                                  "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            var assignIdObj = selectedRow.Cells["AssignID"].Value;

            if (assignIdObj != DBNull.Value && assignIdObj != null)
            {
                int assignID = Convert.ToInt32(assignIdObj);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM StudentCourses WHERE AssignID=@id", con);
                    cmd.Parameters.AddWithValue("@id", assignID);
                    cmd.ExecuteNonQuery();
                }
            }

            dgvAssignedCourses.Rows.Remove(selectedRow);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
