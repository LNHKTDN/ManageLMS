using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ManageLMS.UI.Course.UserCourse
{
    public partial class UserCourseView : Form
    {
        #region Designer Components
        private IContainer components = null;
        private SplitContainer splitContainer1;
        private GroupBox grpSemester;
        private GroupBox grpMaster;
        private Panel pnlSemesterFilter;
        private Panel pnlHeader;
        private Label lblSemesterCode;
        private Label lblUserInfo;
        private TextBox txtSemesterCode;
        private Button btnLoadSemester;
        private Button btnRefreshMaster;
        private Button btnExportSemester;
        private Button btnExportMaster;
        private DataGridView dgvSemesterCourses;
        private DataGridView dgvMasterCourses;
        private Panel pnlSemesterStats;
        private Panel pnlMasterStats;
        private Label lblSemesterStats;
        private Label lblMasterStats;
        #endregion

        #region Windows Form Designer generated code
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.grpSemester = new System.Windows.Forms.GroupBox();
            this.dgvSemesterCourses = new System.Windows.Forms.DataGridView();
            this.pnlSemesterStats = new System.Windows.Forms.Panel();
            this.lblSemesterStats = new System.Windows.Forms.Label();
            this.btnExportSemester = new System.Windows.Forms.Button();
            this.pnlSemesterFilter = new System.Windows.Forms.Panel();
            this.btnLoadSemester = new System.Windows.Forms.Button();
            this.txtSemesterCode = new System.Windows.Forms.TextBox();
            this.lblSemesterCode = new System.Windows.Forms.Label();
            this.grpMaster = new System.Windows.Forms.GroupBox();
            this.dgvMasterCourses = new System.Windows.Forms.DataGridView();
            this.pnlMasterStats = new System.Windows.Forms.Panel();
            this.lblMasterStats = new System.Windows.Forms.Label();
            this.btnExportMaster = new System.Windows.Forms.Button();
            this.btnRefreshMaster = new System.Windows.Forms.Button();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblUserInfo = new System.Windows.Forms.Label();
            this.menuStripUserCourse = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.xemThànhViênToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.grpSemester.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSemesterCourses)).BeginInit();
            this.pnlSemesterStats.SuspendLayout();
            this.pnlSemesterFilter.SuspendLayout();
            this.grpMaster.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMasterCourses)).BeginInit();
            this.pnlMasterStats.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.menuStripUserCourse.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 60);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.grpSemester);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.grpMaster);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.splitContainer1.Size = new System.Drawing.Size(1400, 640);
            this.splitContainer1.SplitterDistance = 320;
            this.splitContainer1.SplitterWidth = 8;
            this.splitContainer1.TabIndex = 0;
            // 
            // grpSemester
            // 
            this.grpSemester.Controls.Add(this.dgvSemesterCourses);
            this.grpSemester.Controls.Add(this.pnlSemesterStats);
            this.grpSemester.Controls.Add(this.pnlSemesterFilter);
            this.grpSemester.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSemester.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.grpSemester.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.grpSemester.Location = new System.Drawing.Point(10, 5);
            this.grpSemester.Name = "grpSemester";
            this.grpSemester.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10);
            this.grpSemester.Size = new System.Drawing.Size(1380, 310);
            this.grpSemester.TabIndex = 0;
            this.grpSemester.TabStop = false;
            this.grpSemester.Text = "📅 Khóa học theo Học kỳ/Danh mục";
            // 
            // dgvSemesterCourses
            // 
            this.dgvSemesterCourses.AllowUserToAddRows = false;
            this.dgvSemesterCourses.AllowUserToDeleteRows = false;
            this.dgvSemesterCourses.AllowUserToResizeRows = false;
            this.dgvSemesterCourses.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSemesterCourses.BackgroundColor = System.Drawing.Color.White;
            this.dgvSemesterCourses.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvSemesterCourses.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvSemesterCourses.EnableHeadersVisualStyles = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSemesterCourses.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSemesterCourses.ColumnHeadersHeight = 40;
            this.dgvSemesterCourses.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvSemesterCourses.ContextMenuStrip = this.menuStripUserCourse;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(242)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSemesterCourses.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSemesterCourses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSemesterCourses.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.dgvSemesterCourses.Location = new System.Drawing.Point(15, 95);
            this.dgvSemesterCourses.MultiSelect = false;
            this.dgvSemesterCourses.Name = "dgvSemesterCourses";
            this.dgvSemesterCourses.ReadOnly = true;
            this.dgvSemesterCourses.RowHeadersVisible = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(242)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.dgvSemesterCourses.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvSemesterCourses.RowTemplate.Height = 35;
            this.dgvSemesterCourses.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSemesterCourses.Size = new System.Drawing.Size(1350, 165);
            this.dgvSemesterCourses.TabIndex = 1;
            this.dgvSemesterCourses.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgv_CellMouseDown);
            // 
            // pnlSemesterStats
            // 
            this.pnlSemesterStats.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.pnlSemesterStats.Controls.Add(this.lblSemesterStats);
            this.pnlSemesterStats.Controls.Add(this.btnExportSemester);
            this.pnlSemesterStats.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlSemesterStats.Location = new System.Drawing.Point(15, 260);
            this.pnlSemesterStats.Name = "pnlSemesterStats";
            this.pnlSemesterStats.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.pnlSemesterStats.Size = new System.Drawing.Size(1350, 40);
            this.pnlSemesterStats.TabIndex = 3;
            // 
            // lblSemesterStats
            // 
            this.lblSemesterStats.AutoSize = true;
            this.lblSemesterStats.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSemesterStats.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.lblSemesterStats.Location = new System.Drawing.Point(10, 12);
            this.lblSemesterStats.Name = "lblSemesterStats";
            this.lblSemesterStats.Size = new System.Drawing.Size(187, 20);
            this.lblSemesterStats.TabIndex = 0;
            this.lblSemesterStats.Text = "📊 Chọn học kỳ để tải dữ liệu";
            // 
            // btnExportSemester
            // 
            this.btnExportSemester.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportSemester.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(68)))), ((int)(((byte)(173)))));
            this.btnExportSemester.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportSemester.FlatAppearance.BorderSize = 0;
            this.btnExportSemester.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(60)))), ((int)(((byte)(152)))));
            this.btnExportSemester.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(89)))), ((int)(((byte)(182)))));
            this.btnExportSemester.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btnExportSemester.ForeColor = System.Drawing.Color.White;
            this.btnExportSemester.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportSemester.Location = new System.Drawing.Point(1270, 6);
            this.btnExportSemester.Name = "btnExportSemester";
            this.btnExportSemester.Size = new System.Drawing.Size(70, 28);
            this.btnExportSemester.TabIndex = 1;
            this.btnExportSemester.Text = "📊 Export";
            this.btnExportSemester.UseVisualStyleBackColor = false;
            // 
            // pnlSemesterFilter
            // 
            this.pnlSemesterFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.pnlSemesterFilter.Controls.Add(this.btnLoadSemester);
            this.pnlSemesterFilter.Controls.Add(this.txtSemesterCode);
            this.pnlSemesterFilter.Controls.Add(this.lblSemesterCode);
            this.pnlSemesterFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSemesterFilter.Location = new System.Drawing.Point(15, 35);
            this.pnlSemesterFilter.Name = "pnlSemesterFilter";
            this.pnlSemesterFilter.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.pnlSemesterFilter.Size = new System.Drawing.Size(1350, 60);
            this.pnlSemesterFilter.TabIndex = 0;
            // 
            // btnLoadSemester
            // 
            this.btnLoadSemester.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnLoadSemester.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadSemester.FlatAppearance.BorderSize = 0;
            this.btnLoadSemester.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(97)))), ((int)(((byte)(141)))));
            this.btnLoadSemester.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnLoadSemester.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnLoadSemester.ForeColor = System.Drawing.Color.White;
            this.btnLoadSemester.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoadSemester.Location = new System.Drawing.Point(300, 20);
            this.btnLoadSemester.Name = "btnLoadSemester";
            this.btnLoadSemester.Size = new System.Drawing.Size(120, 32);
            this.btnLoadSemester.TabIndex = 2;
            this.btnLoadSemester.Text = "🔍 Tải khóa học";
            this.btnLoadSemester.UseVisualStyleBackColor = false;
            // 
            // txtSemesterCode
            // 
            this.txtSemesterCode.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSemesterCode.ForeColor = System.Drawing.Color.Gray;
            this.txtSemesterCode.Location = new System.Drawing.Point(10, 22);
            this.txtSemesterCode.Name = "txtSemesterCode";
            this.txtSemesterCode.Size = new System.Drawing.Size(280, 30);
            this.txtSemesterCode.TabIndex = 1;
            this.txtSemesterCode.Text = "Nhập mã học kỳ (VD: 241, 242...)";
            this.txtSemesterCode.Enter += new System.EventHandler(this.txtSemesterCode_Enter);
            this.txtSemesterCode.Leave += new System.EventHandler(this.txtSemesterCode_Leave);
            // 
            // lblSemesterCode
            // 
            this.lblSemesterCode.AutoSize = true;
            this.lblSemesterCode.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSemesterCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.lblSemesterCode.Location = new System.Drawing.Point(10, 2);
            this.lblSemesterCode.Name = "lblSemesterCode";
            this.lblSemesterCode.Size = new System.Drawing.Size(236, 20);
            this.lblSemesterCode.TabIndex = 0;
            this.lblSemesterCode.Text = "📅 Mã kỳ (Category IDNumber):";
            // 
            // grpMaster
            // 
            this.grpMaster.Controls.Add(this.dgvMasterCourses);
            this.grpMaster.Controls.Add(this.pnlMasterStats);
            this.grpMaster.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpMaster.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.grpMaster.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.grpMaster.Location = new System.Drawing.Point(10, 5);
            this.grpMaster.Name = "grpMaster";
            this.grpMaster.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10);
            this.grpMaster.Size = new System.Drawing.Size(1380, 302);
            this.grpMaster.TabIndex = 0;
            this.grpMaster.TabStop = false;
            this.grpMaster.Text = "📚 Khóa học Chung (Master/Tài liệu)";
            // 
            // dgvMasterCourses
            // 
            this.dgvMasterCourses.AllowUserToAddRows = false;
            this.dgvMasterCourses.AllowUserToDeleteRows = false;
            this.dgvMasterCourses.AllowUserToResizeRows = false;
            this.dgvMasterCourses.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMasterCourses.BackgroundColor = System.Drawing.Color.White;
            this.dgvMasterCourses.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvMasterCourses.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvMasterCourses.EnableHeadersVisualStyles = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvMasterCourses.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvMasterCourses.ColumnHeadersHeight = 40;
            this.dgvMasterCourses.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvMasterCourses.ContextMenuStrip = this.menuStripUserCourse;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(242)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvMasterCourses.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvMasterCourses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMasterCourses.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.dgvMasterCourses.Location = new System.Drawing.Point(15, 35);
            this.dgvMasterCourses.MultiSelect = false;
            this.dgvMasterCourses.Name = "dgvMasterCourses";
            this.dgvMasterCourses.ReadOnly = true;
            this.dgvMasterCourses.RowHeadersVisible = false;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(242)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.dgvMasterCourses.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvMasterCourses.RowTemplate.Height = 35;
            this.dgvMasterCourses.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMasterCourses.Size = new System.Drawing.Size(1350, 217);
            this.dgvMasterCourses.TabIndex = 0;
            this.dgvMasterCourses.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgv_CellMouseDown);
            // 
            // pnlMasterStats
            // 
            this.pnlMasterStats.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.pnlMasterStats.Controls.Add(this.lblMasterStats);
            this.pnlMasterStats.Controls.Add(this.btnExportMaster);
            this.pnlMasterStats.Controls.Add(this.btnRefreshMaster);
            this.pnlMasterStats.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlMasterStats.Location = new System.Drawing.Point(15, 252);
            this.pnlMasterStats.Name = "pnlMasterStats";
            this.pnlMasterStats.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.pnlMasterStats.Size = new System.Drawing.Size(1350, 40);
            this.pnlMasterStats.TabIndex = 2;
            // 
            // lblMasterStats
            // 
            this.lblMasterStats.AutoSize = true;
            this.lblMasterStats.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblMasterStats.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.lblMasterStats.Location = new System.Drawing.Point(10, 12);
            this.lblMasterStats.Name = "lblMasterStats";
            this.lblMasterStats.Size = new System.Drawing.Size(104, 20);
            this.lblMasterStats.TabIndex = 0;
            this.lblMasterStats.Text = "📊 Đang tải...";
            // 
            // btnExportMaster
            // 
            this.btnExportMaster.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportMaster.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(68)))), ((int)(((byte)(173)))));
            this.btnExportMaster.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportMaster.FlatAppearance.BorderSize = 0;
            this.btnExportMaster.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(60)))), ((int)(((byte)(152)))));
            this.btnExportMaster.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(89)))), ((int)(((byte)(182)))));
            this.btnExportMaster.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btnExportMaster.ForeColor = System.Drawing.Color.White;
            this.btnExportMaster.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportMaster.Location = new System.Drawing.Point(1270, 6);
            this.btnExportMaster.Name = "btnExportMaster";
            this.btnExportMaster.Size = new System.Drawing.Size(70, 28);
            this.btnExportMaster.TabIndex = 2;
            this.btnExportMaster.Text = "📊 Export";
            this.btnExportMaster.UseVisualStyleBackColor = false;
            // 
            // btnRefreshMaster
            // 
            this.btnRefreshMaster.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshMaster.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnRefreshMaster.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshMaster.FlatAppearance.BorderSize = 0;
            this.btnRefreshMaster.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.btnRefreshMaster.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(231)))), ((int)(((byte)(128)))));
            this.btnRefreshMaster.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btnRefreshMaster.ForeColor = System.Drawing.Color.White;
            this.btnRefreshMaster.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefreshMaster.Location = new System.Drawing.Point(1190, 6);
            this.btnRefreshMaster.Name = "btnRefreshMaster";
            this.btnRefreshMaster.Size = new System.Drawing.Size(70, 28);
            this.btnRefreshMaster.TabIndex = 1;
            this.btnRefreshMaster.Text = "🔄 Tải lại";
            this.btnRefreshMaster.UseVisualStyleBackColor = false;
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.pnlHeader.Controls.Add(this.lblUserInfo);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(20, 15, 20, 15);
            this.pnlHeader.Size = new System.Drawing.Size(1400, 60);
            this.pnlHeader.TabIndex = 1;
            // 
            // lblUserInfo
            // 
            this.lblUserInfo.AutoSize = true;
            this.lblUserInfo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblUserInfo.ForeColor = System.Drawing.Color.White;
            this.lblUserInfo.Location = new System.Drawing.Point(20, 18);
            this.lblUserInfo.Name = "lblUserInfo";
            this.lblUserInfo.Size = new System.Drawing.Size(348, 32);
            this.lblUserInfo.TabIndex = 0;
            this.lblUserInfo.Text = "🎓 Khóa học của người dùng";
            // 
            // menuStripUserCourse
            // 
            this.menuStripUserCourse.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStripUserCourse.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.xemThànhViênToolStripMenuItem});
            this.menuStripUserCourse.Name = "menuStripUserCourse";
            this.menuStripUserCourse.Size = new System.Drawing.Size(181, 28);
            // 
            // xemThànhViênToolStripMenuItem
            // 
            this.xemThànhViênToolStripMenuItem.Name = "xemThànhViênToolStripMenuItem";
            this.xemThànhViênToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
            this.xemThànhViênToolStripMenuItem.Text = "👥 Xem thành viên";
            this.xemThànhViênToolStripMenuItem.Click += new System.EventHandler(this.mnuViewMembers_Click);
            // 
            // UserCourseView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1400, 700);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.MinimumSize = new System.Drawing.Size(1200, 700);
            this.Name = "UserCourseView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "🎓 Xem khóa học của người dùng";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.grpSemester.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSemesterCourses)).EndInit();
            this.pnlSemesterStats.ResumeLayout(false);
            this.pnlSemesterStats.PerformLayout();
            this.pnlSemesterFilter.ResumeLayout(false);
            this.pnlSemesterFilter.PerformLayout();
            this.grpMaster.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMasterCourses)).EndInit();
            this.pnlMasterStats.ResumeLayout(false);
            this.pnlMasterStats.PerformLayout();
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.menuStripUserCourse.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private ContextMenuStrip menuStripUserCourse;
        private ToolStripMenuItem xemThànhViênToolStripMenuItem;
    }
}