using ManageLMS.BLL.Manager.CourseManager;
using ManageLMS.Common.DTO.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManageLMS.UI.Course.UserCourse
{
    public partial class ListUserCourse : Form
    {
        private long _courseId;
        private string _courseName;
        private CourseManager _courseMgr;

        // Biến lưu danh sách gốc để phục vụ tìm kiếm và filtering
        private List<EnrolledUserViewModel> _originalList;
        private List<EnrolledUserViewModel> _filteredList;

        // Loading state management
        private bool _isLoading = false;

        public ListUserCourse(long courseId, string courseName)
        {
            InitializeComponent();

            _courseId = courseId;
            _courseName = courseName;
            _courseMgr = new CourseManager();
            _originalList = new List<EnrolledUserViewModel>();
            _filteredList = new List<EnrolledUserViewModel>();

            InitializeFormSettings();
            RegisterEventHandlers();
        }

        #region Initialization

        private void InitializeFormSettings()
        {
            // Form settings
            this.Text = string.Format("👥 Thành viên: {0}", _courseName);
            //this.Icon = Properties.Resources.app_icon; // Nếu có icon

            // Setup DataGridView
            dgvUsers.AutoGenerateColumns = false;

            // Setup placeholder text
            SetPlaceholderText();

            // Initial title
            lblTitle.Text = "👥 Đang tải dữ liệu...";
            lblStats.Text = "📊 Đang tải...";
        }

        private void RegisterEventHandlers()
        {
            // Form events
            this.Load += ListUserCourse_Load;
            this.FormClosing += ListUserCourse_FormClosing;

            // Search events
            this.txtSearch.TextChanged += TxtSearch_TextChanged;
            this.txtSearch.KeyPress += TxtSearch_KeyPress;

            // Button events
            this.btnRefresh.Click += BtnRefresh_Click;
            this.btnExport.Click += BtnExport_Click;

            // DataGridView events
            this.dgvUsers.SelectionChanged += DgvUsers_SelectionChanged;
            this.dgvUsers.RowPrePaint += DgvUsers_RowPrePaint;
        }

        private void SetPlaceholderText()
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text) ||
                txtSearch.Text == "Tìm theo tên hoặc username...")
            {
                txtSearch.Text = "Tìm theo tên hoặc username...";
                txtSearch.ForeColor = Color.Gray;
            }
        }

        #endregion

        #region Event Handlers

        private void ListUserCourse_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void ListUserCourse_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Cancel any running background operations
            if (_isLoading)
            {
                e.Cancel = true;
                MessageBox.Show("Vui lòng đợi quá trình tải dữ liệu hoàn tất.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Placeholder event handlers - .NET 4.0 compatible
        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Tìm theo tên hoặc username..." &&
                txtSearch.ForeColor == Color.Gray)
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
            }
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Tìm theo tên hoặc username...";
                txtSearch.ForeColor = Color.Gray;
            }
        }

        private void TxtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Enter key to focus grid
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                if (dgvUsers.Rows.Count > 0)
                {
                    dgvUsers.Focus();
                    dgvUsers.CurrentCell = dgvUsers.Rows[0].Cells[0];
                }
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            if (_isLoading)
            {
                MessageBox.Show("Đang trong quá trình tải dữ liệu. Vui lòng đợi...", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            LoadData();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                ExportToCSV();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất dữ liệu: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            // Optional: Show selected user details in status bar
            if (dgvUsers.SelectedRows.Count > 0)
            {
                var selectedUser = dgvUsers.SelectedRows[0].DataBoundItem as EnrolledUserViewModel;
                if (selectedUser != null)
                {
                    lblStats.Text = string.Format("📊 Hiển thị: {0}/{1} • Đã chọn: {2}",
                        _filteredList.Count, _originalList.Count, selectedUser.Username);
                }
            }
        }

        private void DgvUsers_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            // Alternate row coloring for better readability
            if (e.RowIndex % 2 == 0)
            {
                dgvUsers.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(249, 249, 249);
            }
            else
            {
                dgvUsers.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
            }

            // Color coding based on role or last access
            var rowData = dgvUsers.Rows[e.RowIndex].DataBoundItem as EnrolledUserViewModel;
            if (rowData != null)
            {
                // Highlight teachers/instructors
                if (!string.IsNullOrEmpty(rowData.Roles) &&
                    rowData.Roles.ToLower().Contains("teacher"))
                {
                    dgvUsers.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(142, 68, 173);
                    dgvUsers.Rows[e.RowIndex].DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                }

                // Highlight users who haven't accessed recently (if LastAccess is available)
                // This would require parsing LastAccess date and checking if it's old
            }
        }

        #endregion

        #region Data Operations

        private void LoadData()
        {
            SetLoadingState(true);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    return _courseMgr.GetEnrolledUser(_courseId);
                }
                catch (Exception ex)
                {
                    // Re-throw to be handled in continuation
                    throw ex;
                }
            })
            .ContinueWith(t =>
            {
                SetLoadingState(false);

                if (t.Exception != null)
                {
                    // .NET 4.0 error handling
                    string errorMessage = "Lỗi tải danh sách: " +
                        (t.Exception.InnerException != null ?
                         t.Exception.InnerException.Message : t.Exception.Message);

                    MessageBox.Show(errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Set empty state
                    _originalList = new List<EnrolledUserViewModel>();
                    _filteredList = new List<EnrolledUserViewModel>();
                    BindGrid(_filteredList);
                    return;
                }

                // Success - save data and perform initial binding
                _originalList = t.Result ?? new List<EnrolledUserViewModel>();
                _filteredList = new List<EnrolledUserViewModel>(_originalList);

                PerformSearch(); // This will bind the grid with current filter

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void PerformSearch()
        {
            if (_originalList == null) return;

            string keyword = "";
            if (txtSearch.Text != "Tìm theo tên hoặc username..." &&
                txtSearch.ForeColor != Color.Gray)
            {
                keyword = txtSearch.Text.Trim().ToLower();
            }

            if (string.IsNullOrEmpty(keyword))
            {
                // Show all data
                _filteredList = new List<EnrolledUserViewModel>(_originalList);
            }
            else
            {
                // Filter data
                _filteredList = _originalList.Where(u =>
                    (!string.IsNullOrEmpty(u.Username) && u.Username.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.Fullname) && u.Fullname.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.Email) && u.Email.ToLower().Contains(keyword))
                ).ToList();
            }

            BindGrid(_filteredList);
        }

        private void BindGrid(List<EnrolledUserViewModel> data)
        {
            dgvUsers.DataSource = data;

            // Update UI labels
            UpdateLabels(data.Count);

            // Auto-select first row if available
            if (data.Count > 0 && dgvUsers.Rows.Count > 0)
            {
                dgvUsers.CurrentCell = dgvUsers.Rows[0].Cells[0];
            }
        }

        private void UpdateLabels(int displayedCount)
        {
            lblTitle.Text = string.Format("👥 Thành viên - {0}", _courseName);

            if (displayedCount == _originalList.Count)
            {
                lblStats.Text = string.Format("📊 Tổng số: {0} thành viên", _originalList.Count);
            }
            else
            {
                lblStats.Text = string.Format("📊 Hiển thị: {0}/{1} thành viên",
                    displayedCount, _originalList.Count);
            }
        }

        #endregion

        #region UI State Management

        private void SetLoadingState(bool isLoading)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool>(SetLoadingState), isLoading);
                return;
            }

            _isLoading = isLoading;

            if (isLoading)
            {
                this.Cursor = Cursors.WaitCursor;
                btnRefresh.Enabled = false;
                btnExport.Enabled = false;
                txtSearch.Enabled = false;
                dgvUsers.Enabled = false;

                lblTitle.Text = "👥 Đang tải dữ liệu...";
                lblStats.Text = "📊 Đang xử lý...";
            }
            else
            {
                this.Cursor = Cursors.Default;
                btnRefresh.Enabled = true;
                btnExport.Enabled = true;
                txtSearch.Enabled = true;
                dgvUsers.Enabled = true;
            }
        }

        #endregion

        #region Export Functionality

        private void ExportToCSV()
        {
            if (_filteredList == null || _filteredList.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "Xuất danh sách thành viên";
            saveDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            saveDialog.FileName = string.Format("ThanhVien_{0}_{1:yyyyMMdd_HHmmss}.csv",
                _courseName.Replace(" ", "_"), DateTime.Now);

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var writer = new StreamWriter(saveDialog.FileName, false, System.Text.Encoding.UTF8))
                    {
                        // Header
                        writer.WriteLine("Username,Họ và tên,Email,Vai trò,Truy cập cuối");

                        // Data rows
                        foreach (var user in _filteredList)
                        {
                            writer.WriteLine(string.Format("{0},{1},{2},{3},{4}",
                                EscapeCSV(user.Username ?? ""),
                                EscapeCSV(user.Fullname ?? ""),
                                EscapeCSV(user.Email ?? ""),
                                EscapeCSV(user.Roles ?? ""),
                                EscapeCSV(user.LastAccess ?? "")));
                        }
                    }

                    MessageBox.Show(string.Format("Đã xuất thành công {0} bản ghi vào file:\n{1}",
                        _filteredList.Count, saveDialog.FileName), "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    throw new Exception("Không thể ghi file: " + ex.Message);
                }
            }
        }

        private string EscapeCSV(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            // Escape quotes and wrap in quotes if contains comma or quote
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }

        #endregion

        #region Keyboard Shortcuts

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // F5 - Refresh
            if (keyData == Keys.F5)
            {
                BtnRefresh_Click(null, null);
                return true;
            }

            // Ctrl+E - Export
            if (keyData == (Keys.Control | Keys.E))
            {
                BtnExport_Click(null, null);
                return true;
            }

            // Ctrl+F - Focus search
            if (keyData == (Keys.Control | Keys.F))
            {
                txtSearch.Focus();
                txtSearch.SelectAll();
                return true;
            }

            // Escape - Clear search
            if (keyData == Keys.Escape && txtSearch.Focused)
            {
                txtSearch.Text = "Tìm theo tên hoặc username...";
                txtSearch.ForeColor = Color.Gray;
                dgvUsers.Focus();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion
    }
}