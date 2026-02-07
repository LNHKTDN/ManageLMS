using ManageLMS.DTO.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ManageLMS.UI.UserManage
{
    public partial class UserEditForm : Form
    {
        public MoodleUser UpdatedUser { get; private set; }
        public UserEditForm(MoodleUser currentUser)
        {
            InitializeComponent();
            // Đổ dữ liệu cũ vào Form
            if (currentUser != null)
            {
                
                UpdatedUser = currentUser;

                txtUsername.Text = currentUser.username;
                txtUsername.ReadOnly = true; 
                txtFirstname.Text = currentUser.firstname;
                txtLastname.Text = currentUser.lastname;
                txtEmail.Text = currentUser.email;
                txtDepartment.Text = currentUser.department;
                txtCity.Text = currentUser.city;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(txtFirstname.Text) || string.IsNullOrEmpty(txtLastname.Text))
            {
                MessageBox.Show("Tên và Họ không được để trống!");
                return;
            }

            // Cập nhật thông tin mới vào Object
            UpdatedUser.firstname = txtFirstname.Text.Trim();
            UpdatedUser.lastname = txtLastname.Text.Trim();
            UpdatedUser.email = txtEmail.Text.Trim();
            UpdatedUser.department = txtDepartment.Text.Trim();
            UpdatedUser.city = txtCity.Text.Trim();

            // Đóng Form và báo OK
            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();   
        }
    }
}
