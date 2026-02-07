using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ManageLMS.UI.Sync
{
    public partial class MasterStudentSyncUC : UserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlMasterSyncControls = new System.Windows.Forms.Panel();
            this.prgMasterSync = new System.Windows.Forms.ProgressBar();
            this.btnSyncMasterStudents = new System.Windows.Forms.Button();
            this.cboMasterCourses = new System.Windows.Forms.ComboBox();
            this.lblMasterSelect = new System.Windows.Forms.Label();
            this.dgvMasterStudents = new System.Windows.Forms.DataGridView();
            this.btnNextPage = new System.Windows.Forms.Button();
            this.lblPageNumber = new System.Windows.Forms.Label();
            this.btnPrevPage = new System.Windows.Forms.Button();
            this.pnlSemesterActions = new System.Windows.Forms.Panel();
            this.pnlMasterSyncControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMasterStudents)).BeginInit();
            this.pnlSemesterActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMasterSyncControls
            // 
            this.pnlMasterSyncControls.Controls.Add(this.prgMasterSync);
            this.pnlMasterSyncControls.Controls.Add(this.btnSyncMasterStudents);
            this.pnlMasterSyncControls.Controls.Add(this.cboMasterCourses);
            this.pnlMasterSyncControls.Controls.Add(this.lblMasterSelect);
            this.pnlMasterSyncControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMasterSyncControls.Location = new System.Drawing.Point(0, 0);
            this.pnlMasterSyncControls.Name = "pnlMasterSyncControls";
            this.pnlMasterSyncControls.Size = new System.Drawing.Size(900, 60);
            this.pnlMasterSyncControls.TabIndex = 1;
            // 
            // prgMasterSync
            // 
            this.prgMasterSync.Location = new System.Drawing.Point(650, 17);
            this.prgMasterSync.Name = "prgMasterSync";
            this.prgMasterSync.Size = new System.Drawing.Size(200, 23);
            this.prgMasterSync.TabIndex = 0;
            this.prgMasterSync.Visible = false;
            // 
            // btnSyncMasterStudents
            // 
            this.btnSyncMasterStudents.BackColor = System.Drawing.Color.LightSalmon;
            this.btnSyncMasterStudents.Location = new System.Drawing.Point(466, 15);
            this.btnSyncMasterStudents.Name = "btnSyncMasterStudents";
            this.btnSyncMasterStudents.Size = new System.Drawing.Size(164, 26);
            this.btnSyncMasterStudents.TabIndex = 1;
            this.btnSyncMasterStudents.Text = "Đồng bộ SV vào Lớp này";
            this.btnSyncMasterStudents.UseVisualStyleBackColor = false;
            // 
            // cboMasterCourses
            // 
            this.cboMasterCourses.Location = new System.Drawing.Point(146, 17);
            this.cboMasterCourses.Name = "cboMasterCourses";
            this.cboMasterCourses.Size = new System.Drawing.Size(300, 24);
            this.cboMasterCourses.TabIndex = 2;
            // 
            // lblMasterSelect
            // 
            this.lblMasterSelect.AutoSize = true;
            this.lblMasterSelect.Location = new System.Drawing.Point(20, 20);
            this.lblMasterSelect.Name = "lblMasterSelect";
            this.lblMasterSelect.Size = new System.Drawing.Size(120, 17);
            this.lblMasterSelect.TabIndex = 3;
            this.lblMasterSelect.Text = "Chọn Lớp Master:";
            // 
            // dgvMasterStudents
            // 
            this.dgvMasterStudents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMasterStudents.Location = new System.Drawing.Point(0, 60);
            this.dgvMasterStudents.Name = "dgvMasterStudents";
            this.dgvMasterStudents.Size = new System.Drawing.Size(900, 540);
            this.dgvMasterStudents.TabIndex = 0;
            // 
            // btnNextPage
            // 
            this.btnNextPage.Location = new System.Drawing.Point(170, 7);
            this.btnNextPage.Margin = new System.Windows.Forms.Padding(4);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(40, 28);
            this.btnNextPage.TabIndex = 5;
            this.btnNextPage.Text = ">";
            this.btnNextPage.UseVisualStyleBackColor = true;
            // 
            // lblPageNumber
            // 
            this.lblPageNumber.AutoSize = true;
            this.lblPageNumber.Location = new System.Drawing.Point(83, 13);
            this.lblPageNumber.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPageNumber.Name = "lblPageNumber";
            this.lblPageNumber.Size = new System.Drawing.Size(58, 17);
            this.lblPageNumber.TabIndex = 4;
            this.lblPageNumber.Text = "Trang 1";
            // 
            // btnPrevPage
            // 
            this.btnPrevPage.Location = new System.Drawing.Point(23, 7);
            this.btnPrevPage.Margin = new System.Windows.Forms.Padding(4);
            this.btnPrevPage.Name = "btnPrevPage";
            this.btnPrevPage.Size = new System.Drawing.Size(40, 28);
            this.btnPrevPage.TabIndex = 3;
            this.btnPrevPage.Text = "<";
            this.btnPrevPage.UseVisualStyleBackColor = true;
            // 
            // pnlSemesterActions
            // 
            this.pnlSemesterActions.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlSemesterActions.Controls.Add(this.btnNextPage);
            this.pnlSemesterActions.Controls.Add(this.lblPageNumber);
            this.pnlSemesterActions.Controls.Add(this.btnPrevPage);
            this.pnlSemesterActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlSemesterActions.Location = new System.Drawing.Point(0, 552);
            this.pnlSemesterActions.Name = "pnlSemesterActions";
            this.pnlSemesterActions.Size = new System.Drawing.Size(900, 48);
            this.pnlSemesterActions.TabIndex = 2;
            // 
            // MasterStudentSyncUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlSemesterActions);
            this.Controls.Add(this.dgvMasterStudents);
            this.Controls.Add(this.pnlMasterSyncControls);
            this.Name = "MasterStudentSyncUC";
            this.Size = new System.Drawing.Size(900, 600);
            this.pnlMasterSyncControls.ResumeLayout(false);
            this.pnlMasterSyncControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMasterStudents)).EndInit();
            this.pnlSemesterActions.ResumeLayout(false);
            this.pnlSemesterActions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        // Controls
        private Panel pnlMasterSyncControls;
        private Label lblMasterSelect;
        private ComboBox cboMasterCourses;
        private Button btnSyncMasterStudents;
        private ProgressBar prgMasterSync;
        private DataGridView dgvMasterStudents;
        private Button btnNextPage;
        private Label lblPageNumber;
        private Button btnPrevPage;
        private Panel pnlSemesterActions;
    }
}