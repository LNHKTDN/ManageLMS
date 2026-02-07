using ManageLMS.BLL.Manager.CourseManager;
using ManageLMS.Common.DTO.ViewModel;
using ManageLMS.DTO.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManageLMS.UI.Course.UserCourse
{
    public partial class UserCourseView : Form
    {
        #region Private Fields

        private MoodleUser _currentUser;
        private CourseManager _courseMgr;

        // Loading state management
        private bool _isLoadingMaster = false;
        private bool _isLoadingSemester = false;

        // Data cache
        private List<MoodleCourse> _masterCourses;
        private List<UserCourseViewModel> _semesterCourses;

        #endregion

        #region Constructor & Initialization

        public UserCourseView(MoodleUser user)
        {
            InitializeComponent();

            _currentUser = user;
            _courseMgr = new CourseManager();

            InitializeFormSettings();
            ConfigureDataGridViews();
            RegisterEventHandlers();
            SetInitialPlaceholder();
        }

        private void InitializeFormSettings()
        {
            if (_currentUser != null)
            {
                this.Text = string.Format("📚 Khóa học: {0} {1} ({2})",
                    _currentUser.lastname ?? "", _currentUser.firstname ?? "", _currentUser.username ?? "");
            }

            // Initialize data collections
            _masterCourses = new List<MoodleCourse>();
            _semesterCourses = new List<UserCourseViewModel>();

            // Set initial state
            lblMasterStats.Text = "📊 Đang tải khóa học chung...";
            lblSemesterStats.Text = "📊 Chưa tải dữ liệu học kỳ";
        }

        private void ConfigureDataGridViews()
        {
            // Configure semester grid
            dgvSemesterCourses.AutoGenerateColumns = false;
            dgvSemesterCourses.Columns.Clear();
            dgvSemesterCourses.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "📋 Mã HP",
                DataPropertyName = "Shortname",
                Width = 120,
                FillWeight = 20
            });
            dgvSemesterCourses.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "📚 Tên Khóa học",
                DataPropertyName = "Fullname",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 80
            });

            // Configure master grid
            dgvMasterCourses.AutoGenerateColumns = false;
            dgvMasterCourses.Columns.Clear();
            dgvMasterCourses.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "📋 Shortname",
                DataPropertyName = "shortname",
                Width = 150,
                FillWeight = 25
            });
            dgvMasterCourses.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "🏛️ Tên Khóa học",
                DataPropertyName = "fullname",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 75
            });
        }

        private void RegisterEventHandlers()
        {
            // Form events
            this.Load += UserCourseView_Load;
            this.FormClosing += UserCourseView_FormClosing;

            // Button events
            this.btnLoadSemester.Click += BtnLoadSemester_Click;
            //this.btnClearFilter.Click += BtnClearFilter_Click;

            // DataGridView events
            this.dgvSemesterCourses.SelectionChanged += DgvSemesterCourses_SelectionChanged;
            this.dgvMasterCourses.SelectionChanged += DgvMasterCourses_SelectionChanged;
        }

        private void SetInitialPlaceholder()
        {
            txtSemesterCode.Text = "Ví dụ: 241, 242...";
            txtSemesterCode.ForeColor = Color.Gray;
        }

        #endregion

        #region Placeholder Event Handlers

        // ✅ LOGIC CHO PLACEHOLDER EVENTS
        private void txtSemesterCode_Enter(object sender, EventArgs e)
        {
            // Xóa placeholder khi người dùng click vào textbox
            if (txtSemesterCode.Text == "Ví dụ: 241, 242..." &&
                txtSemesterCode.ForeColor == Color.Gray)
            {
                txtSemesterCode.Text = "";
                txtSemesterCode.ForeColor = Color.Black;
            }
        }

        private void txtSemesterCode_Leave(object sender, EventArgs e)
        {
            // Hiển thị lại placeholder nếu textbox trống
            if (string.IsNullOrWhiteSpace(txtSemesterCode.Text))
            {
                txtSemesterCode.Text = "Ví dụ: 241, 242...";
                txtSemesterCode.ForeColor = Color.Gray;
            }
        }

        private void txtSemesterCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Enter key để tự động tải dữ liệu
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                BtnLoadSemester_Click(sender, e);
            }

            // Chỉ cho phép nhập số và một số ký tự đặc biệt
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        #endregion

        #region Form Event Handlers

        private void UserCourseView_Load(object sender, EventArgs e)
        {
            LoadMasterCourses();
        }

        private void UserCourseView_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Prevent closing while loading
            if (_isLoadingMaster || _isLoadingSemester)
            {
                e.Cancel = true;
                MessageBox.Show("Vui lòng đợi quá trình tải dữ liệu hoàn tất.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnLoadSemester_Click(object sender, EventArgs e)
        {
            string semesterCode = txtSemesterCode.Text.Trim();

            // Validate input
            if (string.IsNullOrEmpty(semesterCode) ||
                semesterCode == "Ví dụ: 241, 242..." ||
                txtSemesterCode.ForeColor == Color.Gray)
            {
                MessageBox.Show("Vui lòng nhập Mã kỳ (Category IDNumber)\n\nVí dụ: 241, 242, 251...",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSemesterCode.Focus();
                return;
            }

            if (_currentUser == null) return;

            LoadSemesterCourses(semesterCode);
        }

        private void BtnClearFilter_Click(object sender, EventArgs e)
        {
            // Reset semester filter
            txtSemesterCode.Text = "Ví dụ: 241, 242...";
            txtSemesterCode.ForeColor = Color.Gray;

            // Clear semester grid
            dgvSemesterCourses.DataSource = null;
            _semesterCourses.Clear();

            lblSemesterStats.Text = "📊 Chưa tải dữ liệu học kỳ";

            txtSemesterCode.Focus();
        }

        private void DgvSemesterCourses_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSemesterCourses.SelectedRows.Count > 0)
            {
                var selectedCourse = dgvSemesterCourses.SelectedRows[0].DataBoundItem as UserCourseViewModel;
                if (selectedCourse != null)
                {
                    lblSemesterStats.Text = string.Format("📊 Hiển thị: {0} khóa học • Đã chọn: {1}",
                        _semesterCourses.Count, selectedCourse.Shortname);
                }
            }
        }

        private void DgvMasterCourses_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMasterCourses.SelectedRows.Count > 0)
            {
                var selectedCourse = dgvMasterCourses.SelectedRows[0].DataBoundItem as MoodleCourse;
                if (selectedCourse != null)
                {
                    lblMasterStats.Text = string.Format("📊 Tổng: {0} khóa học • Đã chọn: {1}",
                        _masterCourses.Count, selectedCourse.shortname);
                }
            }
        }

        #endregion

        #region Data Loading Operations

        private void LoadMasterCourses()
        {
            if (_currentUser == null) return;

            SetMasterLoadingState(true);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    return _courseMgr.GetMasterCourses(_currentUser.id);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            })
            .ContinueWith(t =>
            {
                SetMasterLoadingState(false);

                if (t.Exception != null)
                {
                    // .NET 4.0 error handling
                    string errorMessage = "Lỗi tải Master Course: " +
                        (t.Exception.InnerException != null ?
                         t.Exception.InnerException.Message : t.Exception.Message);

                    MessageBox.Show(errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblMasterStats.Text = "❌ Lỗi tải dữ liệu";
                }
                else
                {
                    _masterCourses = t.Result ?? new List<MoodleCourse>();
                    dgvMasterCourses.DataSource = _masterCourses;

                    lblMasterStats.Text = string.Format("📊 Tổng: {0} khóa học chung", _masterCourses.Count);

                    if (_masterCourses.Count == 0)
                    {
                        lblMasterStats.Text = "📊 Không có khóa học chung nào";
                    }
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void LoadSemesterCourses(string semesterCode)
        {
            if (_currentUser == null) return;

            SetSemesterLoadingState(true);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    return _courseMgr.GetUserCoursesBySemester(_currentUser.id, semesterCode);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            })
            .ContinueWith(t =>
            {
                SetSemesterLoadingState(false);

                if (t.Exception != null)
                {
                    // .NET 4.0 error handling
                    string errorMessage = "Lỗi tải Semester Course: " +
                        (t.Exception.InnerException != null ?
                         t.Exception.InnerException.Message : t.Exception.Message);

                    MessageBox.Show(errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblSemesterStats.Text = string.Format("❌ Lỗi tải học kỳ {0}", semesterCode);
                }
                else
                {
                    _semesterCourses = t.Result ?? new List<UserCourseViewModel>();
                    dgvSemesterCourses.DataSource = _semesterCourses;

                    if (_semesterCourses.Count == 0)
                    {
                        lblSemesterStats.Text = string.Format("📊 Không có khóa học trong kỳ {0}", semesterCode);
                        MessageBox.Show(string.Format("Không tìm thấy khóa học nào trong học kỳ '{0}'.\n\n" +
                            "Vui lòng kiểm tra:\n" +
                            "• Mã kỳ có chính xác không?\n" +
                            "• Người dùng có tham gia khóa học trong kỳ này không?", semesterCode),
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        lblSemesterStats.Text = string.Format("📊 Học kỳ {0}: {1} khóa học",
                            semesterCode, _semesterCourses.Count);
                    }
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion

        #region UI State Management

        private void SetMasterLoadingState(bool isLoading)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool>(SetMasterLoadingState), isLoading);
                return;
            }

            _isLoadingMaster = isLoading;

            if (isLoading)
            {
                dgvMasterCourses.Enabled = false;
                lblMasterStats.Text = "⏳ Đang tải khóa học chung...";
                grpMaster.Text = "🏛️ Đang tải...";
            }
            else
            {
                dgvMasterCourses.Enabled = true;
                grpMaster.Text = "🏛️ Khóa học Chung (Tài liệu/Master)";
            }
        }

        private void SetSemesterLoadingState(bool isLoading)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool>(SetSemesterLoadingState), isLoading);
                return;
            }

            _isLoadingSemester = isLoading;

            if (isLoading)
            {
                btnLoadSemester.Enabled = false;
                //btnClearFilter.Enabled = false;
                txtSemesterCode.Enabled = false;
                dgvSemesterCourses.Enabled = false;

                btnLoadSemester.Text = "⏳ Đang tải...";
                lblSemesterStats.Text = "⏳ Đang tải dữ liệu...";
            }
            else
            {
                btnLoadSemester.Enabled = true;
                //btnClearFilter.Enabled = true;
                txtSemesterCode.Enabled = true;
                dgvSemesterCourses.Enabled = true;

                btnLoadSemester.Text = "🔍 Tải học kỳ";
            }
        }

        #endregion

        #region Context Menu & Course Selection

        private Tuple<long, string> GetSelectedCourseInfo(DataGridView dgv)
        {
            if (dgv != null && dgv.CurrentRow != null && dgv.CurrentRow.DataBoundItem != null)
            {
                var rawItem = dgv.CurrentRow.DataBoundItem;

                if (rawItem is MoodleCourse)
                {
                    var item = rawItem as MoodleCourse;
                    return Tuple.Create(item.id, item.fullname);
                }
                else if (rawItem is UserCourseViewModel)
                {
                    var item = rawItem as UserCourseViewModel;
                    return Tuple.Create(item.CourseId, item.Fullname);
                }
            }
            return Tuple.Create(0L, (string)null);
        }

        private void dgv_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var dgv = sender as DataGridView;
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    // Select the row that was right-clicked
                    dgv.ClearSelection();
                    dgv.CurrentCell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    dgv.Rows[e.RowIndex].Selected = true;
                }
            }
        }

        private void mnuViewMembers_Click(object sender, EventArgs e)
        {
            // Determine which grid the context menu was opened from
            DataGridView targetGrid = null;
            var sourceControl = menuStripUserCourse.SourceControl as DataGridView;

            if (sourceControl != null)
            {
                targetGrid = sourceControl;
            }
            else
            {
                // Fallback: Check which grid has focus
                if (dgvMasterCourses.Focused || dgvMasterCourses.ContainsFocus)
                    targetGrid = dgvMasterCourses;
                else if (dgvSemesterCourses.Focused || dgvSemesterCourses.ContainsFocus)
                    targetGrid = dgvSemesterCourses;
            }

            if (targetGrid == null || targetGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một khóa học để xem thành viên.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Get course information
            var courseInfo = GetSelectedCourseInfo(targetGrid);
            long courseId = courseInfo.Item1;
            string courseName = courseInfo.Item2;

            if (courseId > 0)
            {
                try
                {
                    // Open member list form
                    var frm = new ListUserCourse(courseId, courseName);
                    frm.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi mở danh sách thành viên: " + ex.Message, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Không thể lấy thông tin khóa học. Vui lòng thử lại.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        #region Keyboard Shortcuts

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // F5 - Refresh master courses
            if (keyData == Keys.F5)
            {
                if (!_isLoadingMaster)
                    LoadMasterCourses();
                return true;
            }

            // Ctrl+R - Reload semester
            if (keyData == (Keys.Control | Keys.R))
            {
                if (!_isLoadingSemester && !string.IsNullOrWhiteSpace(txtSemesterCode.Text) &&
                    txtSemesterCode.Text != "Ví dụ: 241, 242...")
                {
                    BtnLoadSemester_Click(null, null);
                }
                return true;
            }

            // Ctrl+L - Focus semester input
            if (keyData == (Keys.Control | Keys.L))
            {
                txtSemesterCode.Focus();
                txtSemesterCode.SelectAll();
                return true;
            }

            // Delete/Escape - Clear filter
            if (keyData == Keys.Delete || keyData == Keys.Escape)
            {
                if (txtSemesterCode.Focused)
                {
                    BtnClearFilter_Click(null, null);
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion
    }
}