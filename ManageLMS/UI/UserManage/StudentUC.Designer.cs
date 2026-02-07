using System.Windows.Forms;
using System.Drawing;
using System;

namespace ManageLMS.UI.UserManage
{
    partial class StudentUC : UserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Main Layout Panels
        private SplitContainer splitMain;
        private Panel pnlLeft;
        private Panel pnlRight;

        // Left Panel - Grid Area
        private Panel pnlGridHeader;
        private Label lblGridTitle;
        private Panel pnlGridContent;
        private DataGridView dgvStudents;
        private Panel pnlPaging;
        private Button btnPrevPage;
        private Label lblPageInfo;
        private Button btnNextPage;
        private Label lblTotalRecords;

        // Right Panel - Controls Area
        private Panel pnlRightHeader;
        private Label lblControlTitle;
        private Panel pnlFilters;
        private GroupBox grpSearch;
        private Label lblKhoaNhapHoc;
        private TextBox tbxKhoaNhapHoc;
        private Label lblLop;
        private ComboBox cbxLop;
        private Label lblKhoa;
        private ComboBox cbxKhoa;
        private Label lblKeyword;
        private TextBox txtStudentSearch;
        private Button btnStudentSearch;
        private CheckBox chkStudentOnlyMissing;

        private GroupBox grpActions;
        private CheckBox cbxSelectAll;
        private Button btnStudentSyncSelected;
        private Button btnStudentSyncAll;
        private Button btnReload;

        // Context Menu
        private ContextMenuStrip ctxSVAction;
        private ToolStripMenuItem mnu_UpdateInfor;
        private ToolStripMenuItem mnuDelete;
        private ToolStripMenuItem mnuChangePass;
        private ToolStripMenuItem mnuLockAccount;
        private ToolStripMenuItem mnuUnlockAccount;
        private ToolStripMenuItem mnu_UserCourse;

        // DataGridView Columns
        private DataGridViewTextBoxColumn colMaSV;
        private DataGridViewTextBoxColumn colHoTen;
        private DataGridViewTextBoxColumn colEmail;
        private DataGridViewTextBoxColumn colLop;
        private DataGridViewTextBoxColumn colStatus;
        private DataGridViewCheckBoxColumn colSelect;

        // Tooltip
        private ToolTip toolTip;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();

            // Initialize all components
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.pnlGridContent = new System.Windows.Forms.Panel();
            this.dgvStudents = new System.Windows.Forms.DataGridView();
            this.colSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colMaSV = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHoTen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEmail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLop = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ctxSVAction = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnu_UpdateInfor = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuChangePass = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLockAccount = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUnlockAccount = new System.Windows.Forms.ToolStripMenuItem();
            this.mnu_UserCourse = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlPaging = new System.Windows.Forms.Panel();
            this.lblTotalRecords = new System.Windows.Forms.Label();
            this.btnNextPage = new System.Windows.Forms.Button();
            this.lblPageInfo = new System.Windows.Forms.Label();
            this.btnPrevPage = new System.Windows.Forms.Button();
            this.pnlGridHeader = new System.Windows.Forms.Panel();
            this.lblGridTitle = new System.Windows.Forms.Label();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.pnlFilters = new System.Windows.Forms.Panel();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.cbxSelectAll = new System.Windows.Forms.CheckBox();
            this.btnStudentSyncSelected = new System.Windows.Forms.Button();
            this.btnStudentSyncAll = new System.Windows.Forms.Button();
            this.btnReload = new System.Windows.Forms.Button();
            this.grpSearch = new System.Windows.Forms.GroupBox();
            this.lblKhoaNhapHoc = new System.Windows.Forms.Label();
            this.tbxKhoaNhapHoc = new System.Windows.Forms.TextBox();
            this.lblLop = new System.Windows.Forms.Label();
            this.cbxLop = new System.Windows.Forms.ComboBox();
            this.lblKhoa = new System.Windows.Forms.Label();
            this.cbxKhoa = new System.Windows.Forms.ComboBox();
            this.lblKeyword = new System.Windows.Forms.Label();
            this.txtStudentSearch = new System.Windows.Forms.TextBox();
            this.btnStudentSearch = new System.Windows.Forms.Button();
            this.chkStudentOnlyMissing = new System.Windows.Forms.CheckBox();
            this.pnlRightHeader = new System.Windows.Forms.Panel();
            this.lblControlTitle = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);

            // Suspend layouts
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.pnlGridContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStudents)).BeginInit();
            this.ctxSVAction.SuspendLayout();
            this.pnlPaging.SuspendLayout();
            this.pnlGridHeader.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlFilters.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.grpSearch.SuspendLayout();
            this.pnlRightHeader.SuspendLayout();
            this.SuspendLayout();

            // 
            // splitMain - RESPONSIVE SETUP
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 0);
            this.splitMain.Name = "splitMain";
            this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitMain.Panel1.Controls.Add(this.pnlLeft);
            this.splitMain.Panel2.Controls.Add(this.pnlRight);
            this.splitMain.Size = new System.Drawing.Size(1200, 700); // Giảm kích thước mặc định
            this.splitMain.SplitterDistance = 450; // 65% cho grid
            this.splitMain.SplitterWidth = 5;
            this.splitMain.TabIndex = 1;
            this.splitMain.TabStop = false;
            // Responsive behavior
            this.splitMain.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitMain_SplitterMoved);

            // 
            // pnlLeft - GRID AREA
            // 
            this.pnlLeft.BackColor = System.Drawing.Color.White;
            this.pnlLeft.Controls.Add(this.pnlGridContent);
            this.pnlLeft.Controls.Add(this.pnlPaging);
            this.pnlLeft.Controls.Add(this.pnlGridHeader);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Padding = new System.Windows.Forms.Padding(5); // Giảm padding
            this.pnlLeft.Size = new System.Drawing.Size(1200, 450);
            this.pnlLeft.TabIndex = 0;

            // 
            // pnlGridContent
            // 
            this.pnlGridContent.Controls.Add(this.dgvStudents);
            this.pnlGridContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGridContent.Location = new System.Drawing.Point(5, 40); // Giảm header height
            this.pnlGridContent.Name = "pnlGridContent";
            this.pnlGridContent.Size = new System.Drawing.Size(1190, 365);
            this.pnlGridContent.TabIndex = 0;

            // 
            // dgvStudents - OPTIMIZED
            // 
            this.dgvStudents.AllowUserToAddRows = false;
            this.dgvStudents.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.dgvStudents.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvStudents.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStudents.BackgroundColor = System.Drawing.Color.White;
            this.dgvStudents.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;

            // Header Style
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold); // Nhỏ hơn
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvStudents.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvStudents.ColumnHeadersHeight = 28; // Giảm height
            this.dgvStudents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Columns Setup
            this.dgvStudents.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colSelect, this.colMaSV, this.colHoTen, this.colEmail, this.colLop, this.colStatus});
            this.dgvStudents.ContextMenuStrip = this.ctxSVAction;

            // Cell Style
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 8.25F); // Nhỏ hơn
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(237)))), ((int)(((byte)(243)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvStudents.DefaultCellStyle = dataGridViewCellStyle3;

            this.dgvStudents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvStudents.EnableHeadersVisualStyles = false;
            this.dgvStudents.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(221)))), ((int)(((byte)(225)))));
            this.dgvStudents.Location = new System.Drawing.Point(0, 0);
            this.dgvStudents.MultiSelect = false;
            this.dgvStudents.Name = "dgvStudents";
            this.dgvStudents.RowHeadersVisible = false;
            this.dgvStudents.RowTemplate.Height = 24; // Giảm row height
            this.dgvStudents.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStudents.Size = new System.Drawing.Size(1190, 365);
            this.dgvStudents.TabIndex = 0;

            // DataGridView Columns - RESPONSIVE
            this.colSelect.DataPropertyName = "IsSelected";
            this.colSelect.FillWeight = 30F; // Giảm từ 40F
            this.colSelect.HeaderText = "☑";
            this.colSelect.Name = "colSelect";
            this.colSelect.Resizable = System.Windows.Forms.DataGridViewTriState.False;

            this.colMaSV.FillWeight = 70F; // Giảm từ 80F
            this.colMaSV.HeaderText = "Mã SV";
            this.colMaSV.Name = "colMaSV";
            this.colMaSV.ReadOnly = true;

            this.colHoTen.FillWeight = 100F; // Giảm từ 120F
            this.colHoTen.HeaderText = "Họ và tên";
            this.colHoTen.Name = "colHoTen";
            this.colHoTen.ReadOnly = true;

            this.colEmail.HeaderText = "Email";
            this.colEmail.Name = "colEmail";
            this.colEmail.ReadOnly = true;
            this.colEmail.FillWeight = 90F; // Thêm weight

            this.colLop.FillWeight = 60F; // Giảm từ 80F
            this.colLop.HeaderText = "Lớp";
            this.colLop.Name = "colLop";
            this.colLop.ReadOnly = true;

            this.colStatus.FillWeight = 80F; // Giảm từ 90F
            this.colStatus.HeaderText = "Trạng thái";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;

            // Context Menu
            this.ctxSVAction.ImageScalingSize = new System.Drawing.Size(16, 16); // Giảm size
            this.ctxSVAction.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnu_UpdateInfor, this.mnuDelete, this.mnuChangePass,
                this.mnuLockAccount, this.mnuUnlockAccount, this.mnu_UserCourse});
            this.ctxSVAction.Name = "ctxSVAction";
            this.ctxSVAction.Size = new System.Drawing.Size(200, 148); // Giảm size

            // Context Menu Items
            this.mnu_UpdateInfor.Name = "mnu_UpdateInfor";
            this.mnu_UpdateInfor.Size = new System.Drawing.Size(199, 22);
            this.mnu_UpdateInfor.Text = "✏️ Cập nhật thông tin";
            this.mnu_UpdateInfor.Click += new System.EventHandler(this.mnu_UpdateInfor_Click);

            this.mnuDelete.Name = "mnuDelete";
            this.mnuDelete.Size = new System.Drawing.Size(199, 22);
            this.mnuDelete.Text = "🗑️ Xóa tài khoản";
            this.mnuDelete.Click += new System.EventHandler(this.mnuDelete_Click);

            this.mnuChangePass.Name = "mnuChangePass";
            this.mnuChangePass.Size = new System.Drawing.Size(199, 22);
            this.mnuChangePass.Text = "🔑 Đổi mật khẩu";
            this.mnuChangePass.Click += new System.EventHandler(this.mnuChangePass_Click);

            this.mnuLockAccount.Name = "mnuLockAccount";
            this.mnuLockAccount.Size = new System.Drawing.Size(199, 22);
            this.mnuLockAccount.Text = "🔒 Khóa tài khoản";
            this.mnuLockAccount.Click += new System.EventHandler(this.mnuLockAccount_Click);

            this.mnuUnlockAccount.Name = "mnuUnlockAccount";
            this.mnuUnlockAccount.Size = new System.Drawing.Size(199, 22);
            this.mnuUnlockAccount.Text = "🔓 Mở khóa";
            this.mnuUnlockAccount.Click += new System.EventHandler(this.mnuUnlockAccount_Click);

            this.mnu_UserCourse.Name = "mnu_UserCourse";
            this.mnu_UserCourse.Size = new System.Drawing.Size(199, 22);
            this.mnu_UserCourse.Text = "📚 Xem khóa học";
            this.mnu_UserCourse.Click += new System.EventHandler(this.mnu_UserCourse_Click);

            // 
            // pnlPaging - COMPACT
            // 
            this.pnlPaging.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.pnlPaging.Controls.Add(this.lblTotalRecords);
            this.pnlPaging.Controls.Add(this.btnNextPage);
            this.pnlPaging.Controls.Add(this.lblPageInfo);
            this.pnlPaging.Controls.Add(this.btnPrevPage);
            this.pnlPaging.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlPaging.Location = new System.Drawing.Point(5, 405);
            this.pnlPaging.Name = "pnlPaging";
            this.pnlPaging.Size = new System.Drawing.Size(1190, 40); // Giảm height
            this.pnlPaging.TabIndex = 1;

            // Paging Controls - RESPONSIVE
            this.btnPrevPage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnPrevPage.FlatAppearance.BorderSize = 0;
            this.btnPrevPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrevPage.Font = new System.Drawing.Font("Segoe UI", 10F); // Giảm font
            this.btnPrevPage.ForeColor = System.Drawing.Color.White;
            this.btnPrevPage.Location = new System.Drawing.Point(5, 5);
            this.btnPrevPage.Name = "btnPrevPage";
            this.btnPrevPage.Size = new System.Drawing.Size(35, 30); // Giảm size
            this.btnPrevPage.TabIndex = 3;
            this.btnPrevPage.Text = "◀";
            this.toolTip.SetToolTip(this.btnPrevPage, "Trang trước");
            this.btnPrevPage.UseVisualStyleBackColor = false;

            this.lblPageInfo.Font = new System.Drawing.Font("Segoe UI", 8.25F); // Giảm font
            this.lblPageInfo.Location = new System.Drawing.Point(45, 10);
            this.lblPageInfo.Name = "lblPageInfo";
            this.lblPageInfo.Size = new System.Drawing.Size(80, 20); // Giảm width
            this.lblPageInfo.TabIndex = 2;
            this.lblPageInfo.Text = "Trang 1";
            this.lblPageInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.btnNextPage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnNextPage.FlatAppearance.BorderSize = 0;
            this.btnNextPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNextPage.Font = new System.Drawing.Font("Segoe UI", 10F); // Giảm font
            this.btnNextPage.ForeColor = System.Drawing.Color.White;
            this.btnNextPage.Location = new System.Drawing.Point(130, 5);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(35, 30); // Giảm size
            this.btnNextPage.TabIndex = 1;
            this.btnNextPage.Text = "▶";
            this.toolTip.SetToolTip(this.btnNextPage, "Trang sau");
            this.btnNextPage.UseVisualStyleBackColor = false;

            // Total Records - RESPONSIVE ANCHOR
            this.lblTotalRecords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalRecords.Font = new System.Drawing.Font("Segoe UI", 8.25F); // Giảm font
            this.lblTotalRecords.Location = new System.Drawing.Point(950, 10); // Dynamic positioning
            this.lblTotalRecords.Name = "lblTotalRecords";
            this.lblTotalRecords.Size = new System.Drawing.Size(230, 20);
            this.lblTotalRecords.TabIndex = 0;
            this.lblTotalRecords.Text = "📊 Tổng: 0 bản ghi";
            this.lblTotalRecords.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // pnlGridHeader - COMPACT
            // 
            this.pnlGridHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.pnlGridHeader.Controls.Add(this.lblGridTitle);
            this.pnlGridHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGridHeader.Location = new System.Drawing.Point(5, 5);
            this.pnlGridHeader.Name = "pnlGridHeader";
            this.pnlGridHeader.Size = new System.Drawing.Size(1190, 35); // Giảm height
            this.pnlGridHeader.TabIndex = 2;

            this.lblGridTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGridTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold); // Giảm font
            this.lblGridTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.lblGridTitle.Location = new System.Drawing.Point(0, 0);
            this.lblGridTitle.Name = "lblGridTitle";
            this.lblGridTitle.Size = new System.Drawing.Size(1190, 35);
            this.lblGridTitle.TabIndex = 0;
            this.lblGridTitle.Text = "📋 DANH SÁCH SINH VIÊN";
            this.lblGridTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // pnlRight - CONTROLS AREA (RESPONSIVE)
            // 
            this.pnlRight.BackColor = System.Drawing.Color.White;
            this.pnlRight.Controls.Add(this.pnlFilters);
            this.pnlRight.Controls.Add(this.pnlRightHeader);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Location = new System.Drawing.Point(0, 0);
            this.pnlRight.MinimumSize = new System.Drawing.Size(300, 0); // Giảm min width
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Padding = new System.Windows.Forms.Padding(5); // Giảm padding
            this.pnlRight.Size = new System.Drawing.Size(1200, 245);
            this.pnlRight.TabIndex = 0;

            // 
            // pnlFilters - HORIZONTAL LAYOUT
            // 
            this.pnlFilters.Controls.Add(this.grpActions);
            this.pnlFilters.Controls.Add(this.grpSearch);
            this.pnlFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFilters.Location = new System.Drawing.Point(5, 35); // Giảm header
            this.pnlFilters.Name = "pnlFilters";
            this.pnlFilters.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.pnlFilters.Size = new System.Drawing.Size(1190, 205);
            this.pnlFilters.TabIndex = 0;

            // 
            // grpSearch - COMPACT HORIZONTAL
            // 
            this.grpSearch.BackColor = System.Drawing.Color.White;
            this.grpSearch.Controls.Add(this.lblKhoaNhapHoc);
            this.grpSearch.Controls.Add(this.tbxKhoaNhapHoc);
            this.grpSearch.Controls.Add(this.lblLop);
            this.grpSearch.Controls.Add(this.cbxLop);
            this.grpSearch.Controls.Add(this.lblKhoa);
            this.grpSearch.Controls.Add(this.cbxKhoa);
            this.grpSearch.Controls.Add(this.lblKeyword);
            this.grpSearch.Controls.Add(this.txtStudentSearch);
            this.grpSearch.Controls.Add(this.btnStudentSearch);
            this.grpSearch.Controls.Add(this.chkStudentOnlyMissing);
            this.grpSearch.Dock = System.Windows.Forms.DockStyle.Left;
            this.grpSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold); // Giảm font
            this.grpSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.grpSearch.Location = new System.Drawing.Point(0, 5);
            this.grpSearch.Name = "grpSearch";
            this.grpSearch.Padding = new System.Windows.Forms.Padding(8); // Giảm padding
            this.grpSearch.Size = new System.Drawing.Size(750, 195); // Width cố định
            this.grpSearch.TabIndex = 1;
            this.grpSearch.TabStop = false;
            this.grpSearch.Text = "🔍 Bộ lọc tìm kiếm";

            // Search Controls - HORIZONTAL LAYOUT
            int searchY = 25;
            int searchSpacing = 150; // Khoảng cách giữa các cột
            int labelHeight = 20;
            int controlHeight = 23;

            // Column 1
            this.lblKhoaNhapHoc.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblKhoaNhapHoc.Location = new System.Drawing.Point(10, searchY);
            this.lblKhoaNhapHoc.Name = "lblKhoaNhapHoc";
            this.lblKhoaNhapHoc.Size = new System.Drawing.Size(80, labelHeight);
            this.lblKhoaNhapHoc.TabIndex = 0;
            this.lblKhoaNhapHoc.Text = "Khóa nhập học:";

            this.tbxKhoaNhapHoc.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.tbxKhoaNhapHoc.ForeColor = System.Drawing.Color.Gray;
            this.tbxKhoaNhapHoc.Location = new System.Drawing.Point(10, searchY + 20);
            this.tbxKhoaNhapHoc.Name = "tbxKhoaNhapHoc";
            this.tbxKhoaNhapHoc.Size = new System.Drawing.Size(130, controlHeight);
            this.tbxKhoaNhapHoc.TabIndex = 1;
            this.tbxKhoaNhapHoc.Text = "VD: 2020, 2021...";
            this.tbxKhoaNhapHoc.Enter += new System.EventHandler(this.tbxKhoaNhapHoc_Enter);
            this.tbxKhoaNhapHoc.Leave += new System.EventHandler(this.tbxKhoaNhapHoc_Leave);

            // Column 2
            this.lblLop.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblLop.Location = new System.Drawing.Point(10 + searchSpacing, searchY);
            this.lblLop.Name = "lblLop";
            this.lblLop.Size = new System.Drawing.Size(40, labelHeight);
            this.lblLop.TabIndex = 2;
            this.lblLop.Text = "Lớp:";

            this.cbxLop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLop.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.cbxLop.Location = new System.Drawing.Point(10 + searchSpacing, searchY + 20);
            this.cbxLop.Name = "cbxLop";
            this.cbxLop.Size = new System.Drawing.Size(130, 25);
            this.cbxLop.TabIndex = 3;

            // Column 3
            this.lblKhoa.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblKhoa.Location = new System.Drawing.Point(10 + searchSpacing * 2, searchY);
            this.lblKhoa.Name = "lblKhoa";
            this.lblKhoa.Size = new System.Drawing.Size(40, labelHeight);
            this.lblKhoa.TabIndex = 4;
            this.lblKhoa.Text = "Khoa:";

            this.cbxKhoa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxKhoa.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.cbxKhoa.Location = new System.Drawing.Point(10 + searchSpacing * 2, searchY + 20);
            this.cbxKhoa.Name = "cbxKhoa";
            this.cbxKhoa.Size = new System.Drawing.Size(130, 25);
            this.cbxKhoa.TabIndex = 5;

            // Column 4 - Search
            this.lblKeyword.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblKeyword.Location = new System.Drawing.Point(10 + searchSpacing * 3, searchY);
            this.lblKeyword.Name = "lblKeyword";
            this.lblKeyword.Size = new System.Drawing.Size(60, labelHeight);
            this.lblKeyword.TabIndex = 6;
            this.lblKeyword.Text = "Từ khóa:";

            this.txtStudentSearch.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.txtStudentSearch.ForeColor = System.Drawing.Color.Gray;
            this.txtStudentSearch.Location = new System.Drawing.Point(10 + searchSpacing * 3, searchY + 20);
            this.txtStudentSearch.Name = "txtStudentSearch";
            this.txtStudentSearch.Size = new System.Drawing.Size(120, controlHeight);
            this.txtStudentSearch.TabIndex = 7;
            this.txtStudentSearch.Text = "Mã SV hoặc họ tên...";
            this.txtStudentSearch.Enter += new System.EventHandler(this.txtStudentSearch_Enter);
            this.txtStudentSearch.Leave += new System.EventHandler(this.txtStudentSearch_Leave);

            // Search Button - Row 2
            this.btnStudentSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnStudentSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStudentSearch.FlatAppearance.BorderSize = 0;
            this.btnStudentSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStudentSearch.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnStudentSearch.ForeColor = System.Drawing.Color.White;
            this.btnStudentSearch.Location = new System.Drawing.Point(10, searchY + 60);
            this.btnStudentSearch.Name = "btnStudentSearch";
            this.btnStudentSearch.Size = new System.Drawing.Size(80, 28);
            this.btnStudentSearch.TabIndex = 8;
            this.btnStudentSearch.Text = "🔍 Tìm";
            this.toolTip.SetToolTip(this.btnStudentSearch, "Tìm kiếm sinh viên");
            this.btnStudentSearch.UseVisualStyleBackColor = false;
            this.btnStudentSearch.Click += new System.EventHandler(this.btnStudentSearch_Click);

            // Checkbox - Row 2
            this.chkStudentOnlyMissing.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkStudentOnlyMissing.Location = new System.Drawing.Point(10 + searchSpacing, searchY + 60);
            this.chkStudentOnlyMissing.Name = "chkStudentOnlyMissing";
            this.chkStudentOnlyMissing.Size = new System.Drawing.Size(200, 20);
            this.chkStudentOnlyMissing.TabIndex = 9;
            this.chkStudentOnlyMissing.Text = "Chỉ hiển thị SV chưa có TK Moodle";

            // 
            // grpActions - COMPACT VERTICAL
            // 
            this.grpActions.BackColor = System.Drawing.Color.White;
            this.grpActions.Controls.Add(this.cbxSelectAll);
            this.grpActions.Controls.Add(this.btnStudentSyncSelected);
            this.grpActions.Controls.Add(this.btnStudentSyncAll);
            this.grpActions.Controls.Add(this.btnReload);
            this.grpActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpActions.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpActions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.grpActions.Location = new System.Drawing.Point(750, 5);
            this.grpActions.Name = "grpActions";
            this.grpActions.Padding = new System.Windows.Forms.Padding(8);
            this.grpActions.Size = new System.Drawing.Size(440, 195);
            this.grpActions.TabIndex = 0;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "⚡ Thao tác";

            // Action Controls - HORIZONTAL LAYOUT
            int actionY = 25;
            int actionSpacing = 200;

            // Column 1
            this.cbxSelectAll.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.cbxSelectAll.Location = new System.Drawing.Point(15, actionY);
            this.cbxSelectAll.Name = "cbxSelectAll";
            this.cbxSelectAll.Size = new System.Drawing.Size(180, 25);
            this.cbxSelectAll.TabIndex = 0;
            this.cbxSelectAll.Text = "☑️ Chọn tất cả SV chưa có TK";

            this.btnStudentSyncSelected.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnStudentSyncSelected.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStudentSyncSelected.FlatAppearance.BorderSize = 0;
            this.btnStudentSyncSelected.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStudentSyncSelected.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnStudentSyncSelected.ForeColor = System.Drawing.Color.White;
            this.btnStudentSyncSelected.Location = new System.Drawing.Point(15, actionY + 35);
            this.btnStudentSyncSelected.Name = "btnStudentSyncSelected";
            this.btnStudentSyncSelected.Size = new System.Drawing.Size(180, 32);
            this.btnStudentSyncSelected.TabIndex = 1;
            this.btnStudentSyncSelected.Text = "📤 Đồng bộ được chọn";
            this.toolTip.SetToolTip(this.btnStudentSyncSelected, "Đồng bộ các sinh viên đã chọn lên Moodle");
            this.btnStudentSyncSelected.UseVisualStyleBackColor = false;

            // Column 2
            this.btnStudentSyncAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(196)))), ((int)(((byte)(15)))));
            this.btnStudentSyncAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStudentSyncAll.FlatAppearance.BorderSize = 0;
            this.btnStudentSyncAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStudentSyncAll.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnStudentSyncAll.ForeColor = System.Drawing.Color.White;
            this.btnStudentSyncAll.Location = new System.Drawing.Point(15 + actionSpacing, actionY);
            this.btnStudentSyncAll.Name = "btnStudentSyncAll";
            this.btnStudentSyncAll.Size = new System.Drawing.Size(180, 32);
            this.btnStudentSyncAll.TabIndex = 2;
            this.btnStudentSyncAll.Text = "🔄 Đồng bộ toàn bộ";
            this.toolTip.SetToolTip(this.btnStudentSyncAll, "Đồng bộ toàn bộ database sinh viên lên Moodle");
            this.btnStudentSyncAll.UseVisualStyleBackColor = false;

            this.btnReload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnReload.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReload.FlatAppearance.BorderSize = 0;
            this.btnReload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReload.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnReload.ForeColor = System.Drawing.Color.White;
            this.btnReload.Location = new System.Drawing.Point(15 + actionSpacing, actionY + 35);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(180, 32);
            this.btnReload.TabIndex = 3;
            this.btnReload.Text = "🔃 Làm mới";
            this.toolTip.SetToolTip(this.btnReload, "Làm mới dữ liệu từ database");
            this.btnReload.UseVisualStyleBackColor = false;

            // 
            // pnlRightHeader - COMPACT
            // 
            this.pnlRightHeader.Controls.Add(this.lblControlTitle);
            this.pnlRightHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlRightHeader.Location = new System.Drawing.Point(5, 5);
            this.pnlRightHeader.Name = "pnlRightHeader";
            this.pnlRightHeader.Size = new System.Drawing.Size(1190, 30); // Giảm height
            this.pnlRightHeader.TabIndex = 1;

            this.lblControlTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblControlTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold); // Giảm font
            this.lblControlTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.lblControlTitle.Location = new System.Drawing.Point(0, 0);
            this.lblControlTitle.Name = "lblControlTitle";
            this.lblControlTitle.Size = new System.Drawing.Size(1190, 30);
            this.lblControlTitle.TabIndex = 0;
            this.lblControlTitle.Text = "🔧 BỘ LỌC & CHỨC NĂNG";
            this.lblControlTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // StudentUC - RESPONSIVE MAIN
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.splitMain);
            this.Name = "StudentUC";
            this.Size = new System.Drawing.Size(1200, 700); // Giảm size mặc định
            this.Resize += new System.EventHandler(this.StudentUC_Resize); // Handle resize

            // Resume layouts
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            this.pnlGridContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStudents)).EndInit();
            this.ctxSVAction.ResumeLayout(false);
            this.pnlPaging.ResumeLayout(false);
            this.pnlGridHeader.ResumeLayout(false);
            this.pnlRight.ResumeLayout(false);
            this.pnlFilters.ResumeLayout(false);
            this.grpActions.ResumeLayout(false);
            this.grpSearch.ResumeLayout(false);
            this.grpSearch.PerformLayout();
            this.pnlRightHeader.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #region Responsive Event Handlers

        private void StudentUC_Resize(object sender, EventArgs e)
        {
            // Adjust layout based on form size
            if (this.Width < 1000)
            {
                // Vertical layout for small screens
                this.splitMain.Orientation = Orientation.Vertical;
                this.splitMain.SplitterDistance = (int)(this.Width * 0.6);
            }
            else
            {
                // Horizontal layout for large screens
                this.splitMain.Orientation = Orientation.Horizontal;
                this.splitMain.SplitterDistance = (int)(this.Height * 0.65);
            }

            // Adjust lblTotalRecords position
            if (this.pnlPaging.Width > 400)
            {
                this.lblTotalRecords.Location = new Point(this.pnlPaging.Width - 240, 10);
            }
        }

        private void splitMain_SplitterMoved(object sender, SplitterEventArgs e)
        {
            // Maintain minimum sizes
            if (splitMain.Orientation == Orientation.Horizontal)
            {
                if (splitMain.SplitterDistance < 300)
                    splitMain.SplitterDistance = 300;
                if (splitMain.Height - splitMain.SplitterDistance < 180)
                    splitMain.SplitterDistance = splitMain.Height - 180;
            }
            else
            {
                if (splitMain.SplitterDistance < 600)
                    splitMain.SplitterDistance = 600;
                if (splitMain.Width - splitMain.SplitterDistance < 300)
                    splitMain.SplitterDistance = splitMain.Width - 300;
            }
        }

        #endregion

        // Placeholder event handlers
        private void tbxKhoaNhapHoc_Enter(object sender, EventArgs e)
        {
            if (tbxKhoaNhapHoc.Text == "VD: 2020, 2021..." && tbxKhoaNhapHoc.ForeColor == Color.Gray)
            {
                tbxKhoaNhapHoc.Text = "";
                tbxKhoaNhapHoc.ForeColor = Color.Black;
            }
        }

        private void tbxKhoaNhapHoc_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbxKhoaNhapHoc.Text))
            {
                tbxKhoaNhapHoc.Text = "VD: 2020, 2021...";
                tbxKhoaNhapHoc.ForeColor = Color.Gray;
            }
        }

        private void txtStudentSearch_Enter(object sender, EventArgs e)
        {
            if (txtStudentSearch.Text == "Mã SV hoặc họ tên..." && txtStudentSearch.ForeColor == Color.Gray)
            {
                txtStudentSearch.Text = "";
                txtStudentSearch.ForeColor = Color.Black;
            }
        }

        private void txtStudentSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStudentSearch.Text))
            {
                txtStudentSearch.Text = "Mã SV hoặc họ tên...";
                txtStudentSearch.ForeColor = Color.Gray;
            }
        }

        #endregion
    }
}