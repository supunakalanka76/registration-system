using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentRegistrationSystem
{
    public partial class RegistrationForm : Form
    {
        string connectionString = @"Data Source=DESKTOP-3SD4HVT\SQLEXPRESS;Initial Catalog=Student;Integrated Security=True";
        private int? updateRegNo = null; // null = new registration, number = update mode

        // Constructor for new registration
        public RegistrationForm()
        {
            InitializeComponent();
            LoadRegNoList();
            GenerateRegNo();
            SetFormMode(false); // Add mode
        }


        // Constructor for update mode
        public RegistrationForm(int regNo)
        {
            InitializeComponent();
            LoadRegNoList();
            updateRegNo = regNo;
            LoadStudentData(regNo);
            SetFormMode(true); // Edit mode
        }

        // Method to set form mode
        private void SetFormMode(bool isEditMode)
        {
            if (isEditMode)
            {
                cmbRegNo.Enabled = true;
                cmbRegNo.TabStop = true;
                cmbRegNo.Focus();
            }
            else
            {
                cmbRegNo.Enabled = false;
                cmbRegNo.TabStop = false;
                txtFirstName.Focus();
            }
        }

        private void LoadStudentData(int regNo)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Registration WHERE regNo=@regNo", con);
                cmd.Parameters.AddWithValue("@regNo", regNo);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    cmbRegNo.Text = reader["regNo"].ToString();
                    txtFirstName.Text = reader["firstName"].ToString();
                    txtLastName.Text = reader["lastName"].ToString();
                    dtpDOB.Value = Convert.ToDateTime(reader["dateOfBirth"]);
                    if (reader["gender"].ToString() == "Male")
                        rdoMale.Checked = true;
                    else
                        rdoFemale.Checked = true;
                    txtAddress.Text = reader["address"].ToString();
                    txtEmail.Text = reader["email"].ToString();
                    txtMobilePhone.Text = reader["mobilePhone"].ToString();
                    txtHomePhone.Text = reader["homePhone"].ToString();
                    txtParentName.Text = reader["parentName"].ToString();
                    txtNIC.Text = reader["nic"].ToString();
                    txtContactNumber.Text = reader["contactNo"].ToString();
                }
            }
        }

        // Generate new RegNo for registration
        private void GenerateRegNo()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(regNo), 0) + 1 FROM Registration", con);
                int newRegNo = (int)cmd.ExecuteScalar();
                cmbRegNo.Text = newRegNo.ToString();
            }
        }

        // Load existing RegNo list into ComboBox
        private void LoadRegNoList()
        {
            cmbRegNo.Items.Clear();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT regNo FROM Registration", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cmbRegNo.Items.Add(reader["regNo"].ToString());
                }
            }
        }

        // Clear all fields
        private void ClearFields()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtAddress.Clear();
            txtEmail.Clear();
            txtMobilePhone.Clear();
            txtHomePhone.Clear();
            txtParentName.Clear();
            txtNIC.Clear();
            txtContactNumber.Clear();
            rdoMale.Checked = false;
            rdoFemale.Checked = false;
        }

        // Validate all inputs
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtMobilePhone.Text) ||
                string.IsNullOrWhiteSpace(txtParentName.Text) ||
                string.IsNullOrWhiteSpace(txtNIC.Text) ||
                string.IsNullOrWhiteSpace(txtContactNumber.Text))
            {
                MessageBox.Show("Please fill all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email format.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Phone number validation (start with 0 and exactly 10 digits)
            Regex phonePattern = new Regex(@"^0\d{9}$");
            if (!phonePattern.IsMatch(txtMobilePhone.Text))
            {
                MessageBox.Show("Mobile phone must start with 0 and be exactly 10 digits.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!string.IsNullOrWhiteSpace(txtHomePhone.Text) && !phonePattern.IsMatch(txtHomePhone.Text))
            {
                MessageBox.Show("Home phone must start with 0 and be exactly 10 digits.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!phonePattern.IsMatch(txtContactNumber.Text))
            {
                MessageBox.Show("Contact number must start with 0 and be exactly 10 digits.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!rdoMale.Checked && !rdoFemale.Checked)
            {
                MessageBox.Show("Please select gender.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        // Register new student
        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Registration (firstName, lastName, dateOfBirth, gender, address, email, mobilePhone, homePhone, parentName, nic, contactNo) " +
                    "VALUES (@firstName, @lastName, @dob, @gender, @address, @email, @mobile, @home, @parent, @nic, @contact)", con);

                cmd.Parameters.AddWithValue("@firstName", txtFirstName.Text);
                cmd.Parameters.AddWithValue("@lastName", txtLastName.Text);
                cmd.Parameters.AddWithValue("@dob", dtpDOB.Value);
                cmd.Parameters.AddWithValue("@gender", rdoMale.Checked ? "Male" : "Female");
                cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@mobile", txtMobilePhone.Text);
                cmd.Parameters.AddWithValue("@home", string.IsNullOrWhiteSpace(txtHomePhone.Text) ? (object)DBNull.Value : txtHomePhone.Text);
                cmd.Parameters.AddWithValue("@parent", txtParentName.Text);
                cmd.Parameters.AddWithValue("@nic", txtNIC.Text);
                cmd.Parameters.AddWithValue("@contact", txtContactNumber.Text);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Registration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearFields();
                LoadRegNoList();
                GenerateRegNo();
            }
        }

        // Update existing student
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            if (!updateRegNo.HasValue)
            {
                MessageBox.Show("No student selected for update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Registration SET firstName=@firstName, lastName=@lastName, dateOfBirth=@dob, gender=@gender, address=@address, email=@email, " +
                    "mobilePhone=@mobile, homePhone=@home, parentName=@parent, nic=@nic, contactNo=@contact WHERE regNo=@regNo", con);

                cmd.Parameters.AddWithValue("@regNo", updateRegNo.Value);
                cmd.Parameters.AddWithValue("@firstName", txtFirstName.Text);
                cmd.Parameters.AddWithValue("@lastName", txtLastName.Text);
                cmd.Parameters.AddWithValue("@dob", dtpDOB.Value);
                cmd.Parameters.AddWithValue("@gender", rdoMale.Checked ? "Male" : "Female");
                cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@mobile", txtMobilePhone.Text);
                cmd.Parameters.AddWithValue("@home", string.IsNullOrWhiteSpace(txtHomePhone.Text) ? (object)DBNull.Value : txtHomePhone.Text);
                cmd.Parameters.AddWithValue("@parent", txtParentName.Text);
                cmd.Parameters.AddWithValue("@nic", txtNIC.Text);
                cmd.Parameters.AddWithValue("@contact", txtContactNumber.Text);

                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                    MessageBox.Show("Record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("No record found with that RegNo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Close();
        }

        // Delete student
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (cmbRegNo.SelectedItem == null && !updateRegNo.HasValue)
            {
                MessageBox.Show("Please select a RegNo to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int regNoToDelete = updateRegNo ?? Convert.ToInt32(cmbRegNo.Text);

            if (MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Registration WHERE regNo=@regNo", con);
                    cmd.Parameters.AddWithValue("@regNo", regNoToDelete);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                        MessageBox.Show("Record deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("No record found with that RegNo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    LoadRegNoList();
                    GenerateRegNo();

                }
            }
            this.Close();
        }

// When selecting RegNo from ComboBox, fill fields
        
        private void cmbRegNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updateRegNo.HasValue) return;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Registration WHERE regNo=@regNo", con);
                cmd.Parameters.AddWithValue("@regNo", cmbRegNo.Text);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtFirstName.Text = reader["firstName"].ToString();
                    txtLastName.Text = reader["lastName"].ToString();
                    dtpDOB.Value = Convert.ToDateTime(reader["dateOfBirth"]);
                    if (reader["gender"].ToString() == "Male")
                        rdoMale.Checked = true;
                    else
                        rdoFemale.Checked = true;
                    txtAddress.Text = reader["address"].ToString();
                    txtEmail.Text = reader["email"].ToString();
                    txtMobilePhone.Text = reader["mobilePhone"].ToString();
                    txtHomePhone.Text = reader["homePhone"].ToString();
                    txtParentName.Text = reader["parentName"].ToString();
                    txtNIC.Text = reader["nic"].ToString();
                    txtContactNumber.Text = reader["contactNo"].ToString();
                }
            }
        }
    }
}
