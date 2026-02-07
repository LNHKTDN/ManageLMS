using ManageLMS.BLL.SyncEngine;
using ManageLMS.Common.DTO.ViewModel;
using ManageLMS.DTO.Model;
using ManageLMS.UI.Course.UserCourse;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks; // Cần thêm cái này để chạy Task Sync
using System.Windows.Forms;

namespace ManageLMS.UI.Sync
{
    public partial class SemesterSyncUC : UserControl
    {
        // Engine & Worker
        private SemesterSyncEngine _engine;
        private BackgroundWorker _bgwLoad;
        private DataGridView _currentClickedDgv = null;
        // Pagination State
        private int _currentPage = 1;
        private int _pageSize = 100; // Mặc định 100 lớp/trang
        private int _totalLoaded = 0;

        // Search Params
        private int _currentKyHoc;
        private string _currentMaKyHoc;
        private string _currentKeyword;
        private SemesterSyncEngine.SyncStatusFilter _currentFilter;

        public SemesterSyncUC()
        {
            InitializeComponent();
            InitializeCustomControls();

            _engine.OnLogMessage += AppendLog;
            _engine.OnProgress += UpdateProgress;
        }

        private void InitializeCustomControls()
        {
            // 1. Setup GridView
            SetupGrid();

            // 2. Setup BackgroundWorker (Load Data)
            _bgwLoad = new BackgroundWorker();
            _bgwLoad.WorkerReportsProgress = true;
            _bgwLoad.DoWork += _bgwLoad_DoWork;
            _bgwLoad.ProgressChanged += _bgwLoad_ProgressChanged;
            _bgwLoad.RunWorkerCompleted += _bgwLoad_RunWorkerCompleted;

            // 3. Events
            btnSemFilter.Click += btnSemFilter_Click;
            btnNextPage.Click += btnNextPage_Click;
            btnPrevPage.Click += btnPrevPage_Click;

            // [MỚI] Sự kiện nút Đồng bộ
            btnSyncSemStudents.Click += btnSyncSemStudents_Click;

            // Xử lý sự kiện vẽ row để tô màu
            dgvSemesterPreview.CellFormatting += DgvSemesterPreview_CellFormatting;

            // 4. Setup ComboBox Filter
            cboStatusFilter.Items.Clear();
            cboStatusFilter.Items.Add(new ComboBoxItem("Tất cả", (int)SemesterSyncEngine.SyncStatusFilter.All));
            cboStatusFilter.Items.Add(new ComboBoxItem("Chưa tạo trên Moodle", (int)SemesterSyncEngine.SyncStatusFilter.NotCreated));
            cboStatusFilter.Items.Add(new ComboBoxItem("Đã tạo (Tất cả)", (int)SemesterSyncEngine.SyncStatusFilter.Created));
            cboStatusFilter.Items.Add(new ComboBoxItem("Đã tạo (Lệch data)", (int)SemesterSyncEngine.SyncStatusFilter.Diff));
            cboStatusFilter.Items.Add(new ComboBoxItem("Đã tạo (Khớp data)", (int)SemesterSyncEngine.SyncStatusFilter.Synced));

            cboStatusFilter.SelectedIndex = 0;
            cboStatusFilter.DisplayMember = "Text";
            cboStatusFilter.ValueMember = "Value";

            // Init Engine
            _engine = new SemesterSyncEngine();
            // Đăng ký log (nếu bạn có TextBox Log trên UI thì gán vào đây)
            // _engine.OnLogMessage += (msg) => { Invoke((Action)(() => txtLog.AppendText(msg + Environment.NewLine))); };
        }

        private void SetupGrid()
        {
            dgvSemesterPreview.AutoGenerateColumns = false;
            dgvSemesterPreview.AllowUserToAddRows = false;
            dgvSemesterPreview.ReadOnly = true;
            dgvSemesterPreview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSemesterPreview.Columns.Clear();

            AddCol("MaLopTinChi", "Mã Lớp", 100);
            AddCol("MonHoc", "Tên Môn Học", 200, true); // Fill
            AddCol("SiSoSQL", "Sĩ số SQL", 80);
            AddCol("SiSoMoodle", "Sĩ số Moodle", 80);
            AddCol("SoLuongThieu", "Thiếu (Cần Enroll)", 120);
            AddCol("SoLuongThua", "Thừa (Cần Unenroll)", 120);
            AddCol("TrangThai", "Trạng Thái", 150);
            AddCol("MoodleShortname", "Moodle Shortname", 150);
        }

        private void AddCol(string propertyName, string headerText, int width, bool isFill = false)
        {
            var col = new DataGridViewTextBoxColumn();
            col.DataPropertyName = propertyName;
            col.HeaderText = headerText;
            if (isFill) col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            else col.Width = width;
            dgvSemesterPreview.Columns.Add(col);
        }

        // ==========================================
        // 1. EVENT HANDLERS (FILTER & NAV)
        // ==========================================

        private void btnSemFilter_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSemYear.Text))
            {
                MessageBox.Show("Vui lòng nhập Năm học (Ví dụ: 241)");
                return;
            }

            if (!int.TryParse(txtSemYear.Text.Trim(), out _currentKyHoc))
            {
                MessageBox.Show("Năm học phải là số nguyên.");
                return;
            }

            string yearStr = txtSemYear.Text.Trim();
            // Logic lấy 3 số cuối làm mã kỳ (VD: 20241 -> 241)
            _currentMaKyHoc = yearStr.Length > 3 ? yearStr.Substring(yearStr.Length - 3) : yearStr;
            _currentKeyword = cboSemClass.Text.Trim();

            var selectedItem = cboStatusFilter.SelectedItem as ComboBoxItem;
            _currentFilter = selectedItem != null
                ? (SemesterSyncEngine.SyncStatusFilter)selectedItem.Value
                : SemesterSyncEngine.SyncStatusFilter.All;

            // Reset về trang 1
            _currentPage = 1;

            // [QUAN TRỌNG] True = Load lại từ SQL
            LoadData(true);
        }

        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                // [QUAN TRỌNG] False = Chỉ lấy trang khác từ RAM
                LoadData(false);
            }
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (_totalLoaded >= _pageSize)
            {
                _currentPage++;
                // [QUAN TRỌNG] False = Chỉ lấy trang khác từ RAM
                LoadData(false);
            }
        }

        // ==========================================
        // 2. DATA LOADING (BACKGROUND WORKER)
        // ==========================================

        private void LoadData(bool isReload)
        {
            if (_bgwLoad.IsBusy) return;

            SetUIState(false);
            dgvSemesterPreview.DataSource = null;

            lblPageNumber.Text = isReload ? "Đang tải dữ liệu từ SQL..." : string.Format("Đang tải trang {0}...", _currentPage);

            var args = new LoadArgs
            {
                IsReload = isReload,
                KyHoc = _currentKyHoc,
                MaKyHoc = _currentMaKyHoc,
                Keyword = _currentKeyword,
                Page = _currentPage,
                Size = _pageSize,
                FilterMode = _currentFilter
            };

            _bgwLoad.RunWorkerAsync(args);
        }

        private void _bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = (LoadArgs)e.Argument;

            // Bước 1: Nếu là Reload -> Gọi hàm Load SQL (Nặng)
            if (args.IsReload)
            {
                _engine.LoadAndGroupSqlData(args.KyHoc, args.MaKyHoc, args.Keyword);
            }

            // Bước 2: Luôn gọi hàm GetPage (Lấy từ RAM + Check Moodle)
            e.Result = _engine.GetPageData(args.Page, args.Size, args.FilterMode);
        }

        private void _bgwLoad_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Có thể update progress bar
        }

        private void _bgwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetUIState(true);

            if (e.Error != null)
            {
                MessageBox.Show("Lỗi: " + e.Error.Message);
                lblPageNumber.Text = "Lỗi tải dữ liệu";
                return;
            }

            var list = e.Result as List<SemesterSyncViewModel>;
            _totalLoaded = list != null ? list.Count : 0;

            dgvSemesterPreview.DataSource = list;
            lblPageNumber.Text = string.Format("Trang {0} - Hiển thị {1} lớp", _currentPage, _totalLoaded);

            btnPrevPage.Enabled = _currentPage > 1;
            // Nếu load đủ trang thì khả năng còn trang sau
            btnNextPage.Enabled = _totalLoaded == _pageSize;
        }

        // ==========================================
        // 3. SYNC ACTION (NÚT ĐỒNG BỘ)
        // ==========================================
        private void btnSyncSemStudents_Click(object sender, EventArgs e)
        {
            // Lấy danh sách các dòng được chọn
            var selectedItems = new List<SemesterSyncViewModel>();
            foreach (DataGridViewRow row in dgvSemesterPreview.SelectedRows)
            {
                var item = row.DataBoundItem as SemesterSyncViewModel;
                if (item != null) selectedItems.Add(item);
            }

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một lớp để đồng bộ (Giữ Ctrl hoặc Shift để chọn nhiều).");
                return;
            }

            if (MessageBox.Show(string.Format("Bạn có chắc chắn muốn đồng bộ {0} lớp đã chọn?", selectedItems.Count),
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            // Khóa UI
            SetUIState(false);
            lblPageNumber.Text = "Đang thực hiện đồng bộ...";

            // Chạy Task riêng để không đơ UI (SyncList chạy khá lâu)
            Task.Factory.StartNew(() =>
            {
                try
                {
                    // Gọi hàm SyncList mới (chỉ cần truyền MaKyHoc)
                    _engine.SyncList(selectedItems, _currentMaKyHoc);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi đồng bộ: " + ex.Message);
                }
            }).ContinueWith(t =>
            {
                // Khi xong thì reload lại trang hiện tại để cập nhật trạng thái xanh/đỏ
                // Chạy trên UI Thread
                this.Invoke((Action)(() =>
                {
                    MessageBox.Show("Đồng bộ hoàn tất!");
                    LoadData(false); // Load lại trang hiện tại (không cần load lại SQL)
                }));
            });
        }

        // ==========================================
        // Helpers & Styling
        // ==========================================

        private void SetUIState(bool enabled)
        {
            btnSemFilter.Enabled = enabled;
            btnNextPage.Enabled = enabled;
            btnPrevPage.Enabled = enabled;
            cboStatusFilter.Enabled = enabled;
            btnSyncSemStudents.Enabled = enabled;
            dgvSemesterPreview.Enabled = enabled;
        }

        private void DgvSemesterPreview_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var item = dgvSemesterPreview.Rows[e.RowIndex].DataBoundItem as SemesterSyncViewModel;
            if (item != null)
            {
                if (item.MoodleCourseId == 0)
                {
                    dgvSemesterPreview.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.MistyRose;
                    dgvSemesterPreview.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.DarkRed;
                }
                else if (item.SoLuongThieu > 0 || item.SoLuongThua > 0)
                {
                    dgvSemesterPreview.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                    dgvSemesterPreview.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.DarkOrange;
                }
                else
                {
                    dgvSemesterPreview.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    dgvSemesterPreview.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }
        private void AppendLog(string msg)
        {
            if (rtbLog.InvokeRequired)
            {
                rtbLog.Invoke((Action)(() => AppendLog(msg)));
            }
            else
            {
                rtbLog.AppendText(string.Format("[{0}] {1}\n", DateTime.Now.ToString("HH:mm:ss"), msg));
                rtbLog.ScrollToCaret();
            }
        }

        // Hàm update progress
        private void UpdateProgress(int current, int total)
        {
            if (pbSyncProgress.InvokeRequired)
            {
                pbSyncProgress.Invoke((Action)(() => UpdateProgress(current, total)));
            }
            else
            {
                pbSyncProgress.Maximum = total;
                pbSyncProgress.Value = current > total ? total : current;
            }
        }
        private void btnSyncAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đồng bộ TOÀN BỘ dữ liệu?\nQuá trình này có thể mất nhiều thời gian.",
        "Xác nhận Sync All", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            // Khóa UI
            SetUIState(false);
            rtbLog.Clear();

            // Chạy Task
            Task.Factory.StartNew(() =>
            {
                try
                {
                    // Kiểm tra xem đã load dữ liệu chưa, nếu chưa thì load
                    // (Nhưng thường người dùng đã bấm Lọc rồi mới bấm Sync All)

                    // Gọi hàm SyncAll
                    _engine.SyncAllDatabase(_currentMaKyHoc);
                }
                catch (Exception ex)
                {
                    AppendLog("Lỗi Fatal: " + ex.Message);
                }
            }).ContinueWith(t =>
            {
                this.Invoke((Action)(() =>
                {
                    MessageBox.Show("Đã hoàn tất đồng bộ toàn bộ!");
                    SetUIState(true);

                    // Reload lại Grid trang hiện tại để thấy kết quả
                    LoadData(false);
                }));
            });
        }

        private void dgv_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var dgv = sender as DataGridView;
                _currentClickedDgv = dgv;
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    dgv.CurrentCell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    
                    dgv.Rows[e.RowIndex].Selected = true;
                }
            }
        }
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
                
                else if (rawItem is MasterSyncViewModel)
                {
                    var item = rawItem as MasterSyncViewModel;
                    
                    return Tuple.Create(item.MoodleCourseId, item.TenHocPhan);
                }
                
                else if (rawItem is SemesterSyncViewModel)
                {
                    var item = rawItem as SemesterSyncViewModel;
                    return Tuple.Create(item.MoodleCourseId, item.MonHoc);
                }
            }
            return Tuple.Create(0L, (string)null);
        }
        private void mnu_ViewEnrolledUser_Click(object sender, EventArgs e)
        {

            DataGridView targetGrid = null;

            var sourceControl = menuStripUserCourse.SourceControl as DataGridView;

            if (sourceControl == null)
            {
                // Fallback: Kiểm tra focus
                if (dgvSemesterPreview.Focused) targetGrid = dgvSemesterPreview;
                else if (dgvSemesterPreview.Focused) targetGrid = dgvSemesterPreview;
            }
            else
            {
                targetGrid = sourceControl;
            }

            if (targetGrid == null) return;

            // Gọi hàm chung để lấy ID
            var courseInfo = GetSelectedCourseInfo(targetGrid);
            long courseId = courseInfo.Item1;
            string courseName = courseInfo.Item2;

            if (courseId > 0)
            {
                // Mở form xem danh sách thành viên (Form bạn vừa tạo ở câu trước)
                var frm = new ListUserCourse(courseId, courseName);
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một khóa học.");
            }
        }


    }
}