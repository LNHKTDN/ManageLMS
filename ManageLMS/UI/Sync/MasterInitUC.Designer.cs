using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ManageLMS.UI.Sync
{
    public partial class MasterInitUC : UserControl
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
            this.pnlTopControls = new System.Windows.Forms.Panel();
            this.btnSyncMaster = new System.Windows.Forms.Button();
            this.btnFilterMaster = new System.Windows.Forms.Button();
            this.cboKhoa = new System.Windows.Forms.ComboBox();
            this.lblKhoa = new System.Windows.Forms.Label();
            this.cboHeDaoTao = new System.Windows.Forms.ComboBox();
            this.lblHeDaoTao = new System.Windows.Forms.Label();
            this.txtSemYear = new System.Windows.Forms.TextBox();
            this.lblSemYear = new System.Windows.Forms.Label();
            this.chkShowOnlyMissing = new System.Windows.Forms.CheckBox();
            this.btnSyncAll = new System.Windows.Forms.Button();
            this.dgvMasterPreview = new System.Windows.Forms.DataGridView();
            this.pnlLogArea = new System.Windows.Forms.Panel();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.pbSyncProgress = new System.Windows.Forms.ProgressBar();
            this.pnlPagination = new System.Windows.Forms.Panel();
            this.btnNextPage = new System.Windows.Forms.Button();
            this.lblPageNumber = new System.Windows.Forms.Label();
            this.btnPrevPage = new System.Windows.Forms.Button();
            this.pnlTopControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMasterPreview)).BeginInit();
            this.pnlLogArea.SuspendLayout();
            this.pnlPagination.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTopControls
            // 
            this.pnlTopControls.Controls.Add(this.btnSyncMaster);
            this.pnlTopControls.Controls.Add(this.btnFilterMaster);
            this.pnlTopControls.Controls.Add(this.cboKhoa);
            this.pnlTopControls.Controls.Add(this.lblKhoa);
            this.pnlTopControls.Controls.Add(this.cboHeDaoTao);
            this.pnlTopControls.Controls.Add(this.lblHeDaoTao);
            this.pnlTopControls.Controls.Add(this.txtSemYear);
            this.pnlTopControls.Controls.Add(this.lblSemYear);
            this.pnlTopControls.Controls.Add(this.chkShowOnlyMissing);
            this.pnlTopControls.Controls.Add(this.btnSyncAll);
            this.pnlTopControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopControls.Location = new System.Drawing.Point(0, 0);
            this.pnlTopControls.Name = "pnlTopControls";
            this.pnlTopControls.Size = new System.Drawing.Size(900, 100);
            this.pnlTopControls.TabIndex = 0;
            // 
            // btnSyncMaster
            // 
            this.btnSyncMaster.BackColor = System.Drawing.Color.LightBlue;
            this.btnSyncMaster.Location = new System.Drawing.Point(550, 55);
            this.btnSyncMaster.Name = "btnSyncMaster";
            this.btnSyncMaster.Size = new System.Drawing.Size(180, 30);
            this.btnSyncMaster.TabIndex = 0;
            this.btnSyncMaster.Text = "Đồng bộ Lớp Master";
            this.btnSyncMaster.UseVisualStyleBackColor = false;
            // 
            // btnFilterMaster
            // 
            this.btnFilterMaster.Location = new System.Drawing.Point(410, 55);
            this.btnFilterMaster.Name = "btnFilterMaster";
            this.btnFilterMaster.Size = new System.Drawing.Size(120, 30);
            this.btnFilterMaster.TabIndex = 1;
            this.btnFilterMaster.Text = "Lọc dữ liệu";
            this.btnFilterMaster.UseVisualStyleBackColor = true;
            // 
            // cboKhoa
            // 
            this.cboKhoa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboKhoa.Location = new System.Drawing.Point(140, 57);
            this.cboKhoa.Name = "cboKhoa";
            this.cboKhoa.Size = new System.Drawing.Size(250, 24);
            this.cboKhoa.TabIndex = 2;
            // 
            // lblKhoa
            // 
            this.lblKhoa.AutoSize = true;
            this.lblKhoa.Location = new System.Drawing.Point(15, 60);
            this.lblKhoa.Name = "lblKhoa";
            this.lblKhoa.Size = new System.Drawing.Size(89, 17);
            this.lblKhoa.TabIndex = 3;
            this.lblKhoa.Text = "Khoa/Đơn vị:";
            // 
            // cboHeDaoTao
            // 
            this.cboHeDaoTao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHeDaoTao.Location = new System.Drawing.Point(350, 17);
            this.cboHeDaoTao.Name = "cboHeDaoTao";
            this.cboHeDaoTao.Size = new System.Drawing.Size(200, 24);
            this.cboHeDaoTao.TabIndex = 4;
            // 
            // lblHeDaoTao
            // 
            this.lblHeDaoTao.AutoSize = true;
            this.lblHeDaoTao.Location = new System.Drawing.Point(260, 20);
            this.lblHeDaoTao.Name = "lblHeDaoTao";
            this.lblHeDaoTao.Size = new System.Drawing.Size(89, 17);
            this.lblHeDaoTao.TabIndex = 5;
            this.lblHeDaoTao.Text = "Hệ Đào Tạo:";
            // 
            // txtSemYear
            // 
            this.txtSemYear.Location = new System.Drawing.Point(140, 17);
            this.txtSemYear.Name = "txtSemYear";
            this.txtSemYear.Size = new System.Drawing.Size(100, 22);
            this.txtSemYear.TabIndex = 6;
            // 
            // lblSemYear
            // 
            this.lblSemYear.AutoSize = true;
            this.lblSemYear.Location = new System.Drawing.Point(15, 20);
            this.lblSemYear.Name = "lblSemYear";
            this.lblSemYear.Size = new System.Drawing.Size(133, 17);
            this.lblSemYear.TabIndex = 7;
            this.lblSemYear.Text = "Năm học (VD: 241):";
            // 
            // chkShowOnlyMissing
            // 
            this.chkShowOnlyMissing.AutoSize = true;
            this.chkShowOnlyMissing.ForeColor = System.Drawing.Color.Red;
            this.chkShowOnlyMissing.Location = new System.Drawing.Point(564, 20);
            this.chkShowOnlyMissing.Name = "chkShowOnlyMissing";
            this.chkShowOnlyMissing.Size = new System.Drawing.Size(166, 21);
            this.chkShowOnlyMissing.TabIndex = 5;
            this.chkShowOnlyMissing.Text = "Chỉ hiện lớp Thiếu SV";
            this.chkShowOnlyMissing.UseVisualStyleBackColor = true;
            // 
            // btnSyncAll
            // 
            this.btnSyncAll.BackColor = System.Drawing.Color.Orange;
            this.btnSyncAll.ForeColor = System.Drawing.Color.White;
            this.btnSyncAll.Location = new System.Drawing.Point(740, 55);
            this.btnSyncAll.Name = "btnSyncAll";
            this.btnSyncAll.Size = new System.Drawing.Size(140, 30);
            this.btnSyncAll.TabIndex = 10;
            this.btnSyncAll.Text = "Đồng bộ TẤT CẢ";
            this.btnSyncAll.UseVisualStyleBackColor = false;
            this.btnSyncAll.Click += new System.EventHandler(this.btnSyncAll_Click);
            // 
            // dgvMasterPreview
            // 
            this.dgvMasterPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMasterPreview.Location = new System.Drawing.Point(0, 100);
            this.dgvMasterPreview.Name = "dgvMasterPreview";
            this.dgvMasterPreview.Size = new System.Drawing.Size(900, 310);
            this.dgvMasterPreview.TabIndex = 1;
            // 
            // pnlLogArea
            // 
            this.pnlLogArea.Controls.Add(this.rtbLog);
            this.pnlLogArea.Controls.Add(this.pbSyncProgress);
            this.pnlLogArea.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlLogArea.Location = new System.Drawing.Point(0, 410);
            this.pnlLogArea.Name = "pnlLogArea";
            this.pnlLogArea.Size = new System.Drawing.Size(900, 150);
            this.pnlLogArea.TabIndex = 2;
            // 
            // rtbLog
            // 
            this.rtbLog.BackColor = System.Drawing.Color.Black;
            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.rtbLog.ForeColor = System.Drawing.Color.Lime;
            this.rtbLog.Location = new System.Drawing.Point(0, 10);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(900, 140);
            this.rtbLog.TabIndex = 0;
            this.rtbLog.Text = "Sẵn sàng...";
            // 
            // pbSyncProgress
            // 
            this.pbSyncProgress.Dock = System.Windows.Forms.DockStyle.Top;
            this.pbSyncProgress.Location = new System.Drawing.Point(0, 0);
            this.pbSyncProgress.Name = "pbSyncProgress";
            this.pbSyncProgress.Size = new System.Drawing.Size(900, 10);
            this.pbSyncProgress.TabIndex = 1;
            // 
            // pnlPagination
            // 
            this.pnlPagination.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlPagination.Controls.Add(this.btnNextPage);
            this.pnlPagination.Controls.Add(this.lblPageNumber);
            this.pnlPagination.Controls.Add(this.btnPrevPage);
            this.pnlPagination.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlPagination.Location = new System.Drawing.Point(0, 560);
            this.pnlPagination.Name = "pnlPagination";
            this.pnlPagination.Size = new System.Drawing.Size(900, 40);
            this.pnlPagination.TabIndex = 3;
            // 
            // btnNextPage
            // 
            this.btnNextPage.Location = new System.Drawing.Point(200, 8);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(40, 23);
            this.btnNextPage.TabIndex = 0;
            this.btnNextPage.Text = ">";
            // 
            // lblPageNumber
            // 
            this.lblPageNumber.AutoSize = true;
            this.lblPageNumber.Location = new System.Drawing.Point(70, 11);
            this.lblPageNumber.Name = "lblPageNumber";
            this.lblPageNumber.Size = new System.Drawing.Size(58, 17);
            this.lblPageNumber.TabIndex = 1;
            this.lblPageNumber.Text = "Trang 1";
            // 
            // btnPrevPage
            // 
            this.btnPrevPage.Location = new System.Drawing.Point(15, 8);
            this.btnPrevPage.Name = "btnPrevPage";
            this.btnPrevPage.Size = new System.Drawing.Size(40, 23);
            this.btnPrevPage.TabIndex = 2;
            this.btnPrevPage.Text = "<";
            // 
            // MasterInitUC
            // 
            this.Controls.Add(this.dgvMasterPreview);
            this.Controls.Add(this.pnlLogArea);
            this.Controls.Add(this.pnlPagination);
            this.Controls.Add(this.pnlTopControls);
            this.Name = "MasterInitUC";
            this.Size = new System.Drawing.Size(900, 600);
            this.pnlTopControls.ResumeLayout(false);
            this.pnlTopControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMasterPreview)).EndInit();
            this.pnlLogArea.ResumeLayout(false);
            this.pnlPagination.ResumeLayout(false);
            this.pnlPagination.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion
        // Khai báo biến Controls
        private Panel pnlTopControls;
        private Label lblSemYear;
        private TextBox txtSemYear;
        private Label lblHeDaoTao;
        private ComboBox cboHeDaoTao;
        private Label lblKhoa;
        private ComboBox cboKhoa;
        private Button btnFilterMaster;
        private Button btnSyncMaster;
        private DataGridView dgvMasterPreview;
        private Panel pnlPagination;
        private Button btnPrevPage;
        private Button btnNextPage;
        private Label lblPageNumber;
        private Panel pnlLogArea;
        private RichTextBox rtbLog;
        private ProgressBar pbSyncProgress;
        private Button btnSyncAll;
        private CheckBox chkShowOnlyMissing;
    }
}