namespace ManageLMS
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            // 1. Đổi tên biến TabControl tổng thành tabMain cho chuẩn
            this.tabMain = new System.Windows.Forms.TabControl();

            // Các Tab con hiện có
            this.tabTeacher = new System.Windows.Forms.TabPage();
            this.tabStudent = new System.Windows.Forms.TabPage();

            // [MỚI] Thêm Tab Sync vào đây
            this.tabSync = new System.Windows.Forms.TabPage();

            this.tabMain.SuspendLayout();
            this.SuspendLayout();

            // 
            // tabMain (TabControl tổng)
            // 
            this.tabMain.Controls.Add(this.tabTeacher);
            this.tabMain.Controls.Add(this.tabStudent);
            this.tabMain.Controls.Add(this.tabSync); // Add Tab Sync vào danh sách
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 0);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(1344, 897);
            this.tabMain.TabIndex = 0;

            // 
            // tabTeacher
            // 
            this.tabTeacher.Location = new System.Drawing.Point(4, 25);
            this.tabTeacher.Name = "tabTeacher";
            this.tabTeacher.Padding = new System.Windows.Forms.Padding(3);
            this.tabTeacher.Size = new System.Drawing.Size(1336, 868);
            this.tabTeacher.TabIndex = 0;
            this.tabTeacher.Text = "Quản lý Giảng viên";
            this.tabTeacher.UseVisualStyleBackColor = true;

            // 
            // tabStudent
            // 
            this.tabStudent.Location = new System.Drawing.Point(4, 25);
            this.tabStudent.Name = "tabStudent";
            this.tabStudent.Padding = new System.Windows.Forms.Padding(3);
            this.tabStudent.Size = new System.Drawing.Size(1336, 868);
            this.tabStudent.TabIndex = 1;
            this.tabStudent.Text = "Quản lý Sinh viên";
            this.tabStudent.UseVisualStyleBackColor = true;

            // 
           
            // 
            this.tabSync.Location = new System.Drawing.Point(4, 25);
            this.tabSync.Name = "tabSync";
            this.tabSync.Padding = new System.Windows.Forms.Padding(3);
            this.tabSync.Size = new System.Drawing.Size(1336, 868);
            this.tabSync.TabIndex = 2;
            this.tabSync.Text = "Đồng bộ dữ liệu khoá học"; 
            this.tabSync.UseVisualStyleBackColor = true;

            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1344, 897);
            this.Controls.Add(this.tabMain); 
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hệ thống quản lý LMS";
            this.tabMain.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        // Khai báo biến
        private System.Windows.Forms.TabControl tabMain; // Đổi tên từ tpSyncManager -> tabMain
        private System.Windows.Forms.TabPage tabTeacher;
        private System.Windows.Forms.TabPage tabStudent;
        private System.Windows.Forms.TabPage tabSync; // Biến mới cho Tab Sync
    }
}