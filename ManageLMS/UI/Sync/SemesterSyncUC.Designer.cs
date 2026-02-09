using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ManageLMS.UI.Sync
{
    public partial class SemesterSyncUC : UserControl
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
            this.components = new System.ComponentModel.Container();
            this.pnlSemesterTop = new System.Windows.Forms.Panel();
            this.grpSemesterFilter = new System.Windows.Forms.GroupBox();
            this.btnSyncSemStudents = new System.Windows.Forms.Button();
            this.btnSemFilter = new System.Windows.Forms.Button();
            this.cboSemClass = new System.Windows.Forms.ComboBox();
            this.lblSemClass = new System.Windows.Forms.Label();
            this.txtSemYear = new System.Windows.Forms.TextBox();
            this.lblSemYear = new System.Windows.Forms.Label();
            this.cboStatusFilter = new System.Windows.Forms.ComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnSyncAll = new System.Windows.Forms.Button();
            this.pnlSemesterActions = new System.Windows.Forms.Panel();
            this.btnNextPage = new System.Windows.Forms.Button();
            this.lblPageNumber = new System.Windows.Forms.Label();
            this.btnPrevPage = new System.Windows.Forms.Button();
            this.dgvSemesterPreview = new System.Windows.Forms.DataGridView();
            this.menuStripUserCourse = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnu_ViewEnrolledUser = new System.Windows.Forms.ToolStripMenuItem();
            this.pbSyncProgress = new System.Windows.Forms.ProgressBar();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbbHDT = new System.Windows.Forms.ComboBox();
            this.pnlSemesterTop.SuspendLayout();
            this.grpSemesterFilter.SuspendLayout();
            this.pnlSemesterActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSemesterPreview)).BeginInit();
            this.menuStripUserCourse.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSemesterTop
            // 
            this.pnlSemesterTop.Controls.Add(this.grpSemesterFilter);
            this.pnlSemesterTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSemesterTop.Location = new System.Drawing.Point(0, 0);
            this.pnlSemesterTop.Name = "pnlSemesterTop";
            this.pnlSemesterTop.Size = new System.Drawing.Size(1200, 120);
            this.pnlSemesterTop.TabIndex = 2;
            // 
            // grpSemesterFilter
            // 
            this.grpSemesterFilter.Controls.Add(this.btnSyncSemStudents);
            this.grpSemesterFilter.Controls.Add(this.btnSemFilter);
            this.grpSemesterFilter.Controls.Add(this.cbbHDT);
            this.grpSemesterFilter.Controls.Add(this.cboSemClass);
            this.grpSemesterFilter.Controls.Add(this.label2);
            this.grpSemesterFilter.Controls.Add(this.lblSemClass);
            this.grpSemesterFilter.Controls.Add(this.txtSemYear);
            this.grpSemesterFilter.Controls.Add(this.lblSemYear);
            this.grpSemesterFilter.Controls.Add(this.cboStatusFilter);
            this.grpSemesterFilter.Controls.Add(this.lblStatus);
            this.grpSemesterFilter.Controls.Add(this.btnSyncAll);
            this.grpSemesterFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSemesterFilter.Location = new System.Drawing.Point(0, 0);
            this.grpSemesterFilter.Name = "grpSemesterFilter";
            this.grpSemesterFilter.Size = new System.Drawing.Size(1200, 120);
            this.grpSemesterFilter.TabIndex = 0;
            this.grpSemesterFilter.TabStop = false;
            this.grpSemesterFilter.Text = "Bộ lọc Lớp Tín chỉ";
            // 
            // btnSyncSemStudents
            // 
            this.btnSyncSemStudents.BackColor = System.Drawing.Color.LightBlue;
            this.btnSyncSemStudents.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.2F);
            this.btnSyncSemStudents.Location = new System.Drawing.Point(984, 68);
            this.btnSyncSemStudents.Name = "btnSyncSemStudents";
            this.btnSyncSemStudents.Size = new System.Drawing.Size(210, 33);
            this.btnSyncSemStudents.TabIndex = 2;
            this.btnSyncSemStudents.Text = "Đồng bộ trang hiện tại";
            this.btnSyncSemStudents.UseVisualStyleBackColor = false;
            // 
            // btnSemFilter
            // 
            this.btnSemFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.2F);
            this.btnSemFilter.Location = new System.Drawing.Point(984, 30);
            this.btnSemFilter.Name = "btnSemFilter";
            this.btnSemFilter.Size = new System.Drawing.Size(210, 30);
            this.btnSemFilter.TabIndex = 0;
            this.btnSemFilter.Text = "Lọc dữ liệu";
            // 
            // cboSemClass
            // 
            this.cboSemClass.Location = new System.Drawing.Point(411, 35);
            this.cboSemClass.Name = "cboSemClass";
            this.cboSemClass.Size = new System.Drawing.Size(150, 24);
            this.cboSemClass.TabIndex = 1;
            // 
            // lblSemClass
            // 
            this.lblSemClass.AutoSize = true;
            this.lblSemClass.Location = new System.Drawing.Point(371, 38);
            this.lblSemClass.Name = "lblSemClass";
            this.lblSemClass.Size = new System.Drawing.Size(36, 17);
            this.lblSemClass.TabIndex = 2;
            this.lblSemClass.Text = "Lớp:";
            // 
            // txtSemYear
            // 
            this.txtSemYear.Location = new System.Drawing.Point(256, 35);
            this.txtSemYear.Name = "txtSemYear";
            this.txtSemYear.Size = new System.Drawing.Size(100, 22);
            this.txtSemYear.TabIndex = 5;
            // 
            // lblSemYear
            // 
            this.lblSemYear.AutoSize = true;
            this.lblSemYear.Location = new System.Drawing.Point(182, 38);
            this.lblSemYear.Name = "lblSemYear";
            this.lblSemYear.Size = new System.Drawing.Size(68, 17);
            this.lblSemYear.TabIndex = 6;
            this.lblSemYear.Text = "Năm học:";
            // 
            // cboStatusFilter
            // 
            this.cboStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatusFilter.FormattingEnabled = true;
            this.cboStatusFilter.Location = new System.Drawing.Point(676, 35);
            this.cboStatusFilter.Name = "cboStatusFilter";
            this.cboStatusFilter.Size = new System.Drawing.Size(150, 24);
            this.cboStatusFilter.TabIndex = 7;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(593, 38);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(77, 17);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Trạng thái:";
            // 
            // btnSyncAll
            // 
            this.btnSyncAll.BackColor = System.Drawing.Color.Orange;
            this.btnSyncAll.Location = new System.Drawing.Point(842, 30);
            this.btnSyncAll.Name = "btnSyncAll";
            this.btnSyncAll.Size = new System.Drawing.Size(125, 33);
            this.btnSyncAll.TabIndex = 9;
            this.btnSyncAll.Text = "Đồng bộ dữ liệu";
            this.btnSyncAll.UseVisualStyleBackColor = false;
            this.btnSyncAll.Click += new System.EventHandler(this.btnSyncAll_Click);
            // 
            // pnlSemesterActions
            // 
            this.pnlSemesterActions.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlSemesterActions.Controls.Add(this.btnNextPage);
            this.pnlSemesterActions.Controls.Add(this.lblPageNumber);
            this.pnlSemesterActions.Controls.Add(this.btnPrevPage);
            this.pnlSemesterActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlSemesterActions.Location = new System.Drawing.Point(0, 756);
            this.pnlSemesterActions.Name = "pnlSemesterActions";
            this.pnlSemesterActions.Size = new System.Drawing.Size(1200, 44);
            this.pnlSemesterActions.TabIndex = 1;
            // 
            // btnNextPage
            // 
            this.btnNextPage.Location = new System.Drawing.Point(251, 7);
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
            // dgvSemesterPreview
            // 
            this.dgvSemesterPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSemesterPreview.ContextMenuStrip = this.menuStripUserCourse;
            this.dgvSemesterPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSemesterPreview.Location = new System.Drawing.Point(0, 120);
            this.dgvSemesterPreview.Name = "dgvSemesterPreview";
            this.dgvSemesterPreview.ReadOnly = true;
            this.dgvSemesterPreview.Size = new System.Drawing.Size(1200, 636);
            this.dgvSemesterPreview.TabIndex = 0;
            // 
            // menuStripUserCourse
            // 
            this.menuStripUserCourse.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStripUserCourse.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnu_ViewEnrolledUser});
            this.menuStripUserCourse.Name = "menuStripUserCourse";
            this.menuStripUserCourse.Size = new System.Drawing.Size(181, 28);
            // 
            // mnu_ViewEnrolledUser
            // 
            this.mnu_ViewEnrolledUser.Name = "mnu_ViewEnrolledUser";
            this.mnu_ViewEnrolledUser.Size = new System.Drawing.Size(180, 24);
            this.mnu_ViewEnrolledUser.Text = "Xem thành viên";
            this.mnu_ViewEnrolledUser.Click += new System.EventHandler(this.mnu_ViewEnrolledUser_Click);
            // 
            // pbSyncProgress
            // 
            this.pbSyncProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pbSyncProgress.Location = new System.Drawing.Point(0, 621);
            this.pbSyncProgress.Name = "pbSyncProgress";
            this.pbSyncProgress.Size = new System.Drawing.Size(1200, 20);
            this.pbSyncProgress.TabIndex = 0;
            // 
            // rtbLog
            // 
            this.rtbLog.BackColor = System.Drawing.Color.Black;
            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.rtbLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.rtbLog.ForeColor = System.Drawing.Color.Lime;
            this.rtbLog.Location = new System.Drawing.Point(0, 641);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(1200, 115);
            this.rtbLog.TabIndex = 1;
            this.rtbLog.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Hệ:";
            // 
            // cbbHDT
            // 
            this.cbbHDT.Location = new System.Drawing.Point(46, 32);
            this.cbbHDT.Name = "cbbHDT";
            this.cbbHDT.Size = new System.Drawing.Size(130, 24);
            this.cbbHDT.TabIndex = 1;
            // 
            // SemesterSyncUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pbSyncProgress);
            this.Controls.Add(this.rtbLog);
            this.Controls.Add(this.dgvSemesterPreview);
            this.Controls.Add(this.pnlSemesterActions);
            this.Controls.Add(this.pnlSemesterTop);
            this.Name = "SemesterSyncUC";
            this.Size = new System.Drawing.Size(1200, 800);
            this.pnlSemesterTop.ResumeLayout(false);
            this.grpSemesterFilter.ResumeLayout(false);
            this.grpSemesterFilter.PerformLayout();
            this.pnlSemesterActions.ResumeLayout(false);
            this.pnlSemesterActions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSemesterPreview)).EndInit();
            this.menuStripUserCourse.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        // Controls
        private Panel pnlSemesterTop;
        private GroupBox grpSemesterFilter;
        private Label lblSemYear;
        private TextBox txtSemYear;
        private Label lblSemClass;
        private ComboBox cboSemClass;
        private Button btnSemFilter;
        private Panel pnlSemesterActions;
        private Button btnSyncSemStudents;
        private DataGridView dgvSemesterPreview;
        private Button btnNextPage;
        private Label lblPageNumber;
        private Button btnPrevPage;
        private ComboBox cboStatusFilter;
        private Label lblStatus;
        private Button btnSyncAll;
        private ProgressBar pbSyncProgress;
        private RichTextBox rtbLog;
        private ContextMenuStrip menuStripUserCourse;
        private ToolStripMenuItem mnu_ViewEnrolledUser;
        private ComboBox cbbHDT;
        private Label label2;
    }
}