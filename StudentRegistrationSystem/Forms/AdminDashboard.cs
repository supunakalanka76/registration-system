using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace StudentRegistrationSystem.Forms
{
    public partial class AdminDashboard : Form
    {
        // Connection string
        string connectionString = "Server=DESKTOP-3SD4HVT\\SQLEXPRESS;Database=Student;Trusted_Connection=True;";

        public AdminDashboard()
        {
            InitializeComponent();
        }

        private void AdminDashboard_Load(object sender, EventArgs e)
        {
            LoadStudents();
            LoadDashboardSummary();
            LoadCourses();
            FormatCoursesGrid();
            LoadCourseSummary();

            // Double-click row to edit
            dgvStudents.CellDoubleClick += DgvStudents_CellDoubleClick;
        }

        private void LoadDashboardSummary()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Total Students
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Registration", con))
                    {
                        lblTotalStudents.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Recent Registrations - last 30 days
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Registration WHERE registrationDate >= DATEADD(DAY, -30, GETDATE())", con))
                    {
                        lblRecentRegistration.Text = cmd.ExecuteScalar().ToString();
                    }

                    // All Courses
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Courses", con))
                    {
                        lblAllCourses.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Available Courses
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Courses WHERE status = 'Available'", con))
                    {
                        lblAvailableCourses.Text = cmd.ExecuteScalar().ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dashboard summary: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCourseSummary()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    // Total Courses
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Courses", con))
                    {
                        lblTotalCourses.Text = cmd.ExecuteScalar().ToString();
                    }
                    // Available Courses
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Courses WHERE status = 'Available'", con))
                    {
                        lblAvailableCourse.Text = cmd.ExecuteScalar().ToString();
                    }
                    // Upcoming Courses
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Courses WHERE status = 'Upcoming'", con))
                    {
                        lblUpcoming.Text = cmd.ExecuteScalar().ToString();
                    }

                    //Enrolled Students
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(DISTINCT regNo) FROM StudentCourses", con))
                    {
                        lblEnrolled.Text = cmd.ExecuteScalar().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading course summary: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOverviewRefresh_Click(object sender, EventArgs e)
        {
            LoadDashboardSummary();
        }


        // Load student data into DataGridView
        private void LoadStudents()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT regNo AS [Reg No], firstName AS [First Name], lastName AS [Last Name], email AS [Email], mobilePhone AS [Mobile], homePhone AS [Home] FROM Registration";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvStudents.DataSource = dt;
                }

                dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvStudents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvStudents.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading student data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Register new student
        private void btnRegister_Click(object sender, EventArgs e)
        {
            var registrationForm = new RegistrationForm();
            registrationForm.FormClosed += (s, args) => LoadStudents(); // Refresh grid after closing
            registrationForm.ShowDialog();
            this.Close();
        }

        // Double-click row to update student
        private void DgvStudents_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // ignore header clicks
            {
                int regNo = Convert.ToInt32(dgvStudents.Rows[e.RowIndex].Cells["Reg No"].Value);
                var registrationForm = new RegistrationForm(regNo);
                registrationForm.FormClosed += (s, args) => LoadStudents();
                registrationForm.ShowDialog();
            }
        }

        private void btnAssignCourse_Click(object sender, EventArgs e)
        {
            AssignCourse frm = new AssignCourse();
            frm.ShowDialog();  // modal form
        }

        // Back to login
        private void btnBackToLogin_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                new LoginForm().Show();
                this.Hide();
            }
        }

        // Refresh student list
        private void btnStudentRefresh_Click(object sender, EventArgs e)
        {
            LoadStudents();
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void lblTotalStudents_Click(object sender, EventArgs e)
        {

        }

        private void LoadCourses()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Courses", con))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvCourses.DataSource = dt;
                }
            }
        }

        private void FormatCoursesGrid()
        {
            dgvCourses.Columns["courseID"].HeaderText = "ID";
            dgvCourses.Columns["courseName"].HeaderText = "Course Name";
            dgvCourses.Columns["durationMonths"].HeaderText = "Duration (Months)";
            dgvCourses.Columns["fee"].HeaderText = "Fee (LKR)";
            dgvCourses.Columns["status"].HeaderText = "Status";
            dgvCourses.Columns["description"].HeaderText = "Description";

            dgvCourses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnAddCourse_Click(object sender, EventArgs e)
        {
            using (AddCourse addCourseForm = new AddCourse())
            {
                var result = addCourseForm.ShowDialog(); // Open form as modal

                if (result == DialogResult.OK)
                {
                    // Refresh DataGridView after adding new course
                    LoadCourses();
                }
            }
        }

        private void btnEditCourse_Click(object sender, EventArgs e)
        {
            if (dgvCourses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a course to edit.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int courseID;
            if (int.TryParse(dgvCourses.SelectedRows[0].Cells["courseID"].Value.ToString(), out courseID))
            {
                using (EditCourse editForm = new EditCourse(courseID))
                {
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadCourses(); // Refresh DataGridView after editing
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid Course ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteCourse_Click(object sender, EventArgs e)
        {
            if (dgvCourses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a course to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int courseID;
            if (!int.TryParse(dgvCourses.SelectedRows[0].Cells["courseID"].Value.ToString(), out courseID))
            {
                MessageBox.Show("Invalid Course ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Confirmation dialog
            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to delete this course?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM Courses WHERE courseID=@id", con))
                        {
                            cmd.Parameters.AddWithValue("@id", courseID);
                            int rows = cmd.ExecuteNonQuery();

                            if (rows > 0)
                                MessageBox.Show("Course deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            else
                                MessageBox.Show("No course found with this ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    // Refresh DataGridView
                    LoadCourses();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting course: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCourseRefresh_Click(object sender, EventArgs e)
        {
            LoadCourses();
        }

        private void btnShowCourse_Click(object sender, EventArgs e)
        {
            var showCourses = new Courses();
            showCourses.ShowDialog();
        }

        private void btnViewCourses_Click(object sender, EventArgs e)
        {
            if (dgvStudents.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a student first.");
                return;
            }

            string regNo = dgvStudents.SelectedRows[0].Cells[0].Value.ToString();

            ViewStudentCourses viewForm = new ViewStudentCourses(regNo);
            viewForm.ShowDialog();
        }

        private void lblRecentRegistration_Click(object sender, EventArgs e)
        {

        }

        private void dgvStudents_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
