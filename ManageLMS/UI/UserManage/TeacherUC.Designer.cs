using System.Windows.Forms;
using System.Drawing;
using System;

namespace ManageLMS.UI.UserManage
{
    partial class TeacherUC : UserControl
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
        private DataGridView dgvTeachers;
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
        private Label lblKeyword;
        private TextBox txtTeacherSearch;
        private Button btnTeacherSearch;
        private CheckBox chkTeacherOnlyMissing;

        private GroupBox grpActions;
        private CheckBox cbxSelectAll;
        private Button btnTeacherSyncSelected;
        private Button btnTeacherSyncAll;
        private Button btnReload;

        // Context Menu
        private ContextMenuStrip ctxGVAction;
        private ToolStripMenuItem mnu_UpdateUser;
        private ToolStripMenuItem mnuDelete;
        private ToolStripMenuItem mnuChangePass;
        private ToolStripMenuItem mnuLockAccount;
        private ToolStripMenuItem mnuUnlockAccount;
        private ToolStripMenuItem mnu_UserCourse;

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
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.pnlGridContent = new System.Windows.Forms.Panel();
            this.dgvTeachers = new System.Windows.Forms.DataGridView();
            this.ctxGVAction = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnu_UpdateUser = new System.Windows.Forms.ToolStripMenuItem();
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
            this.btnTeacherSyncSelected = new System.Windows.Forms.Button();
            this.btnTeacherSyncAll = new System.Windows.Forms.Button();
            this.btnReload = new System.Windows.Forms.Button();
            this.grpSearch = new System.Windows.Forms.GroupBox();
            this.lblKeyword = new System.Windows.Forms.Label();
            this.txtTeacherSearch = new System.Windows.Forms.TextBox();
            this.btnTeacherSearch = new System.Windows.Forms.Button();
            this.chkTeacherOnlyMissing = new System.Windows.Forms.CheckBox();
            this.pnlRightHeader = new System.Windows.Forms.Panel();
            this.lblControlTitle = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.pnlGridContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTeachers)).BeginInit();
            this.ctxGVAction.SuspendLayout();
            this.pnlPaging.SuspendLayout();
            this.pnlGridHeader.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlFilters.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.grpSearch.SuspendLayout();
            this.pnlRightHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitMain
            // 
            this.splitMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 0);
            this.splitMain.Name = "splitMain";
            this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitMain.Panel1MinSize = 300;
            this.splitMain.Panel2MinSize = 200;
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.pnlLeft);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.pnlRight);
            this.splitMain.Size = new System.Drawing.Size(1200, 700);
            this.splitMain.SplitterDistance = 450;
            this.splitMain.SplitterWidth = 8;
            this.splitMain.TabIndex = 1;
            this.splitMain.TabStop = false;
            this.splitMain.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitMain_SplitterMoved);
            // 
            // pnlLeft
            // 
            this.pnlLeft.BackColor = System.Drawing.Color.White;
            this.pnlLeft.Controls.Add(this.pnlGridContent);
            this.pnlLeft.Controls.Add(this.pnlPaging);
            this.pnlLeft.Controls.Add(this.pnlGridHeader);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Padding = new System.Windows.Forms.Padding(5);
            this.pnlLeft.Size = new System.Drawing.Size(1200, 450);
            this.pnlLeft.TabIndex = 0;
            // 
            // pnlGridContent
            // 
            this.pnlGridContent.Controls.Add(this.dgvTeachers);
            this.pnlGridContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGridContent.Location = new System.Drawing.Point(5, 40);
            this.pnlGridContent.Name = "pnlGridContent";
            this.pnlGridContent.Size = new System.Drawing.Size(1190, 365);
            this.pnlGridContent.TabIndex = 0;
            // 
            // dgvTeachers
            // 
            this.dgvTeachers.AllowUserToAddRows = false;
            this.dgvTeachers.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.dgvTeachers.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvTeachers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTeachers.BackgroundColor = System.Drawing.Color.White;
            this.dgvTeachers.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTeachers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvTeachers.ColumnHeadersHeight = 28;
            this.dgvTeachers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvTeachers.ContextMenuStrip = this.ctxGVAction;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(237)))), ((int)(((byte)(243)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvTeachers.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvTeachers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTeachers.EnableHeadersVisualStyles = false;
            this.dgvTeachers.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(221)))), ((int)(((byte)(225)))));
            this.dgvTeachers.Location = new System.Drawing.Point(0, 0);
            this.dgvTeachers.MultiSelect = false;
            this.dgvTeachers.Name = "dgvTeachers";
            this.dgvTeachers.RowHeadersVisible = false;
            this.dgvTeachers.RowTemplate.Height = 24;
            this.dgvTeachers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTeachers.Size = new System.Drawing.Size(1190, 365);
            this.dgvTeachers.TabIndex = 0;
            this.dgvTeachers.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvTeachers_CellMouseDown);
            // 
            // ctxGVAction
            // 
            this.ctxGVAction.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxGVAction.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnu_UpdateUser,
            this.mnuDelete,
            this.mnuChangePass,
            this.mnuLockAccount,
            this.mnuUnlockAccount,
            this.mnu_UserCourse});
            this.ctxGVAction.Name = "ctxGVAction";
            this.ctxGVAction.Size = new System.Drawing.Size(227, 148);
            // 
            // mnu_UpdateUser
            // 
            this.mnu_UpdateUser.Name = "mnu_UpdateUser";
            this.mnu_UpdateUser.Size = new System.Drawing.Size(226, 24);
            this.mnu_UpdateUser.Text = "✏️ Cập nhật thông tin";
            this.mnu_UpdateUser.Click += new System.EventHandler(this.mnuUserUpdate_Click);
            // 
            // mnuDelete
            // 
            this.mnuDelete.Name = "mnuDelete";
            this.mnuDelete.Size = new System.Drawing.Size(226, 24);
            this.mnuDelete.Text = "🗑️ Xóa tài khoản";
            this.mnuDelete.Click += new System.EventHandler(this.mnuDelete_Click);
            // 
            // mnuChangePass
            // 
            this.mnuChangePass.Name = "mnuChangePass";
            this.mnuChangePass.Size = new System.Drawing.Size(226, 24);
            this.mnuChangePass.Text = "🔑 Đổi mật khẩu";
            this.mnuChangePass.Click += new System.EventHandler(this.mnuChangePass_Click);
            // 
            // mnuLockAccount
            // 
            this.mnuLockAccount.Name = "mnuLockAccount";
            this.mnuLockAccount.Size = new System.Drawing.Size(226, 24);
            this.mnuLockAccount.Text = "🔒 Khóa tài khoản";
            this.mnuLockAccount.Click += new System.EventHandler(this.mnuLockAccount_Click_1);
            // 
            // mnuUnlockAccount
            // 
            this.mnuUnlockAccount.Name = "mnuUnlockAccount";
            this.mnuUnlockAccount.Size = new System.Drawing.Size(226, 24);
            this.mnuUnlockAccount.Text = "🔓 Mở khóa";
            this.mnuUnlockAccount.Click += new System.EventHandler(this.mnuUnlockAccount_Click);
            // 
            // mnu_UserCourse
            // 
            this.mnu_UserCourse.Name = "mnu_UserCourse";
            this.mnu_UserCourse.Size = new System.Drawing.Size(226, 24);
            this.mnu_UserCourse.Text = "📚 Xem khóa học";
            this.mnu_UserCourse.Click += new System.EventHandler(this.mnu_UserCourse_Click_1);
            // 
            // pnlPaging
            // 
            this.pnlPaging.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.pnlPaging.Controls.Add(this.lblTotalRecords);
            this.pnlPaging.Controls.Add(this.btnNextPage);
            this.pnlPaging.Controls.Add(this.lblPageInfo);
            this.pnlPaging.Controls.Add(this.btnPrevPage);
            this.pnlPaging.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlPaging.Location = new System.Drawing.Point(5, 405);
            this.pnlPaging.Name = "pnlPaging";
            this.pnlPaging.Size = new System.Drawing.Size(1190, 40);
            this.pnlPaging.TabIndex = 1;
            // 
            // lblTotalRecords
            // 
            this.lblTotalRecords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalRecords.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblTotalRecords.Location = new System.Drawing.Point(950, 10);
            this.lblTotalRecords.Name = "lblTotalRecords";
            this.lblTotalRecords.Size = new System.Drawing.Size(230, 20);
            this.lblTotalRecords.TabIndex = 0;
            this.lblTotalRecords.Text = "📊 Tổng: 0 bản ghi";
            this.lblTotalRecords.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnNextPage
            // 
            this.btnNextPage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnNextPage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNextPage.FlatAppearance.BorderSize = 0;
            this.btnNextPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNextPage.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnNextPage.ForeColor = System.Drawing.Color.White;
            this.btnNextPage.Location = new System.Drawing.Point(130, 5);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(35, 30);
            this.btnNextPage.TabIndex = 1;
            this.btnNextPage.Text = "▶";
            this.toolTip.SetToolTip(this.btnNextPage, "Trang sau");
            this.btnNextPage.UseVisualStyleBackColor = false;
            // 
            // lblPageInfo
            // 
            this.lblPageInfo.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblPageInfo.Location = new System.Drawing.Point(45, 10);
            this.lblPageInfo.Name = "lblPageInfo";
            this.lblPageInfo.Size = new System.Drawing.Size(80, 20);
            this.lblPageInfo.TabIndex = 2;
            this.lblPageInfo.Text = "Trang 1";
            this.lblPageInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnPrevPage
            // 
            this.btnPrevPage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnPrevPage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrevPage.FlatAppearance.BorderSize = 0;
            this.btnPrevPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrevPage.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnPrevPage.ForeColor = System.Drawing.Color.White;
            this.btnPrevPage.Location = new System.Drawing.Point(5, 5);
            this.btnPrevPage.Name = "btnPrevPage";
            this.btnPrevPage.Size = new System.Drawing.Size(35, 30);
            this.btnPrevPage.TabIndex = 3;
            this.btnPrevPage.Text = "◀";
            this.toolTip.SetToolTip(this.btnPrevPage, "Trang trước");
            this.btnPrevPage.UseVisualStyleBackColor = false;
            // 
            // pnlGridHeader
            // 
            this.pnlGridHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.pnlGridHeader.Controls.Add(this.lblGridTitle);
            this.pnlGridHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGridHeader.Location = new System.Drawing.Point(5, 5);
            this.pnlGridHeader.Name = "pnlGridHeader";
            this.pnlGridHeader.Size = new System.Drawing.Size(1190, 35);
            this.pnlGridHeader.TabIndex = 2;
            // 
            // lblGridTitle
            // 
            this.lblGridTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGridTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblGridTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.lblGridTitle.Location = new System.Drawing.Point(0, 0);
            this.lblGridTitle.Name = "lblGridTitle";
            this.lblGridTitle.Size = new System.Drawing.Size(1190, 35);
            this.lblGridTitle.TabIndex = 0;
            this.lblGridTitle.Text = "👨‍🏫 DANH SÁCH GIẢNG VIÊN";
            this.lblGridTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlRight
            // 
            this.pnlRight.BackColor = System.Drawing.Color.White;
            this.pnlRight.Controls.Add(this.pnlFilters);
            this.pnlRight.Controls.Add(this.pnlRightHeader);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Location = new System.Drawing.Point(0, 0);
            this.pnlRight.MinimumSize = new System.Drawing.Size(300, 0);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Padding = new System.Windows.Forms.Padding(5);
            this.pnlRight.Size = new System.Drawing.Size(1200, 242);
            this.pnlRight.TabIndex = 0;
            // 
            // pnlFilters
            // 
            this.pnlFilters.Controls.Add(this.grpActions);
            this.pnlFilters.Controls.Add(this.grpSearch);
            this.pnlFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFilters.Location = new System.Drawing.Point(5, 35);
            this.pnlFilters.Name = "pnlFilters";
            this.pnlFilters.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.pnlFilters.Size = new System.Drawing.Size(1190, 202);
            this.pnlFilters.TabIndex = 0;
            // 
            // grpActions
            // 
            this.grpActions.BackColor = System.Drawing.Color.White;
            this.grpActions.Controls.Add(this.cbxSelectAll);
            this.grpActions.Controls.Add(this.btnTeacherSyncSelected);
            this.grpActions.Controls.Add(this.btnTeacherSyncAll);
            this.grpActions.Controls.Add(this.btnReload);
            this.grpActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpActions.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpActions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.grpActions.Location = new System.Drawing.Point(600, 5);
            this.grpActions.Name = "grpActions";
            this.grpActions.Padding = new System.Windows.Forms.Padding(8);
            this.grpActions.Size = new System.Drawing.Size(590, 192);
            this.grpActions.TabIndex = 0;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "⚡ Thao tác";
            // 
            // cbxSelectAll
            // 
            this.cbxSelectAll.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.cbxSelectAll.Location = new System.Drawing.Point(15, 30);
            this.cbxSelectAll.Name = "cbxSelectAll";
            this.cbxSelectAll.Size = new System.Drawing.Size(180, 25);
            this.cbxSelectAll.TabIndex = 0;
            this.cbxSelectAll.Text = "☑️ Chọn tất cả GV chưa có TK";
            this.cbxSelectAll.Click += new System.EventHandler(this.chxSelectAll_CheckedChanged);
            // 
            // btnTeacherSyncSelected
            // 
            this.btnTeacherSyncSelected.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnTeacherSyncSelected.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTeacherSyncSelected.FlatAppearance.BorderSize = 0;
            this.btnTeacherSyncSelected.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTeacherSyncSelected.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnTeacherSyncSelected.ForeColor = System.Drawing.Color.White;
            this.btnTeacherSyncSelected.Location = new System.Drawing.Point(6, 95);
            this.btnTeacherSyncSelected.Name = "btnTeacherSyncSelected";
            this.btnTeacherSyncSelected.Size = new System.Drawing.Size(570, 32);
            this.btnTeacherSyncSelected.TabIndex = 1;
            this.btnTeacherSyncSelected.Text = "📤 Đồng bộ được chọn";
            this.toolTip.SetToolTip(this.btnTeacherSyncSelected, "Đồng bộ các giảng viên đã chọn lên Moodle");
            this.btnTeacherSyncSelected.UseVisualStyleBackColor = false;
            this.btnTeacherSyncSelected.Click += new System.EventHandler(this.BtnTeacherSyncSelected_Click);
            // 
            // btnTeacherSyncAll
            // 
            this.btnTeacherSyncAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(196)))), ((int)(((byte)(15)))));
            this.btnTeacherSyncAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTeacherSyncAll.FlatAppearance.BorderSize = 0;
            this.btnTeacherSyncAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTeacherSyncAll.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnTeacherSyncAll.ForeColor = System.Drawing.Color.White;
            this.btnTeacherSyncAll.Location = new System.Drawing.Point(6, 133);
            this.btnTeacherSyncAll.Name = "btnTeacherSyncAll";
            this.btnTeacherSyncAll.Size = new System.Drawing.Size(258, 32);
            this.btnTeacherSyncAll.TabIndex = 2;
            this.btnTeacherSyncAll.Text = "🔄 Đồng bộ toàn bộ";
            this.toolTip.SetToolTip(this.btnTeacherSyncAll, "Đồng bộ toàn bộ database giảng viên lên Moodle");
            this.btnTeacherSyncAll.UseVisualStyleBackColor = false;
            this.btnTeacherSyncAll.Click += new System.EventHandler(this.BtnTeacherSyncAll_Click);
            // 
            // btnReload
            // 
            this.btnReload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnReload.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReload.FlatAppearance.BorderSize = 0;
            this.btnReload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReload.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnReload.ForeColor = System.Drawing.Color.White;
            this.btnReload.Location = new System.Drawing.Point(289, 133);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(267, 32);
            this.btnReload.TabIndex = 3;
            this.btnReload.Text = "🔃 Làm mới";
            this.toolTip.SetToolTip(this.btnReload, "Làm mới dữ liệu từ database");
            this.btnReload.UseVisualStyleBackColor = false;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // grpSearch
            // 
            this.grpSearch.BackColor = System.Drawing.Color.White;
            this.grpSearch.Controls.Add(this.lblKeyword);
            this.grpSearch.Controls.Add(this.txtTeacherSearch);
            this.grpSearch.Controls.Add(this.btnTeacherSearch);
            this.grpSearch.Controls.Add(this.chkTeacherOnlyMissing);
            this.grpSearch.Dock = System.Windows.Forms.DockStyle.Left;
            this.grpSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.grpSearch.Location = new System.Drawing.Point(0, 5);
            this.grpSearch.Name = "grpSearch";
            this.grpSearch.Padding = new System.Windows.Forms.Padding(8);
            this.grpSearch.Size = new System.Drawing.Size(600, 192);
            this.grpSearch.TabIndex = 1;
            this.grpSearch.TabStop = false;
            this.grpSearch.Text = "🔍 Bộ lọc tìm kiếm";
            // 
            // lblKeyword
            // 
            this.lblKeyword.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblKeyword.Location = new System.Drawing.Point(15, 30);
            this.lblKeyword.Name = "lblKeyword";
            this.lblKeyword.Size = new System.Drawing.Size(88, 20);
            this.lblKeyword.TabIndex = 0;
            this.lblKeyword.Text = "Từ khóa:";
            // 
            // txtTeacherSearch
            // 
            this.txtTeacherSearch.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.txtTeacherSearch.ForeColor = System.Drawing.Color.Gray;
            this.txtTeacherSearch.Location = new System.Drawing.Point(94, 24);
            this.txtTeacherSearch.Name = "txtTeacherSearch";
            this.txtTeacherSearch.Size = new System.Drawing.Size(303, 26);
            this.txtTeacherSearch.TabIndex = 1;
            this.txtTeacherSearch.Text = "   Mã GV hoặc họ tên...";
            this.txtTeacherSearch.Click += new System.EventHandler(this.TxtTeacherSearch_Enter);
            this.txtTeacherSearch.Enter += new System.EventHandler(this.txtTeacherSearch_Enter);
            this.txtTeacherSearch.Leave += new System.EventHandler(this.txtTeacherSearch_Leave);
            // 
            // btnTeacherSearch
            // 
            this.btnTeacherSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnTeacherSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTeacherSearch.FlatAppearance.BorderSize = 0;
            this.btnTeacherSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTeacherSearch.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnTeacherSearch.ForeColor = System.Drawing.Color.White;
            this.btnTeacherSearch.Location = new System.Drawing.Point(440, 22);
            this.btnTeacherSearch.Name = "btnTeacherSearch";
            this.btnTeacherSearch.Size = new System.Drawing.Size(80, 28);
            this.btnTeacherSearch.TabIndex = 2;
            this.btnTeacherSearch.Text = "🔍 Tìm";
            this.toolTip.SetToolTip(this.btnTeacherSearch, "Tìm kiếm giảng viên");
            this.btnTeacherSearch.UseVisualStyleBackColor = false;
            this.btnTeacherSearch.Click += new System.EventHandler(this.BtnTeacherSearch_Click);
            // 
            // chkTeacherOnlyMissing
            // 
            this.chkTeacherOnlyMissing.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkTeacherOnlyMissing.Location = new System.Drawing.Point(19, 77);
            this.chkTeacherOnlyMissing.Name = "chkTeacherOnlyMissing";
            this.chkTeacherOnlyMissing.Size = new System.Drawing.Size(280, 20);
            this.chkTeacherOnlyMissing.TabIndex = 3;
            this.chkTeacherOnlyMissing.Text = "Chỉ hiển thị GV chưa có TK Moodle";
            // 
            // pnlRightHeader
            // 
            this.pnlRightHeader.Controls.Add(this.lblControlTitle);
            this.pnlRightHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlRightHeader.Location = new System.Drawing.Point(5, 5);
            this.pnlRightHeader.Name = "pnlRightHeader";
            this.pnlRightHeader.Size = new System.Drawing.Size(1190, 30);
            this.pnlRightHeader.TabIndex = 1;
            // 
            // lblControlTitle
            // 
            this.lblControlTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblControlTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblControlTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.lblControlTitle.Location = new System.Drawing.Point(0, 0);
            this.lblControlTitle.Name = "lblControlTitle";
            this.lblControlTitle.Size = new System.Drawing.Size(1190, 30);
            this.lblControlTitle.TabIndex = 0;
            this.lblControlTitle.Text = "🔧 BỘ LỌC  &  CHỨC NĂNG";
            this.lblControlTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TeacherUC
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.splitMain);
            this.Name = "TeacherUC";
            this.Size = new System.Drawing.Size(1200, 700);
            this.Load += new System.EventHandler(this.TeacherUC_Load);
            this.Resize += new System.EventHandler(this.TeacherUC_Resize);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            this.pnlGridContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTeachers)).EndInit();
            this.ctxGVAction.ResumeLayout(false);
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

        #endregion
    }
}