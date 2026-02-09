using ManageLMS.BLL.SyncEngine;
using ManageLMS.Common.DTO.ViewModel;
using ManageLMS.DTO.Model;
using ManageLMS.UI.Course.UserCourse;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
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
        private int _pageSize = 100;
        private int _totalLoaded = 0;

        // Search Params
        private int _currentKyHoc;
        private string _currentMaKyHoc;
        private string _currentKeyword;
        private string _currentHeDaoTao; // Lưu mã hệ đang lọc
        private SemesterSyncEngine.SyncStatusFilter _currentFilter;

        // Định nghĩa mã hệ
        private const string HE_CHINH_QUY = "CQ";
        private const string HE_VHVL = "VHVL";

        // Class phụ để bind ComboBox
        public class ComboBoxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }
            public override string ToString() { return Text; }
            public ComboBoxItem(string text, object value) { Text = text; Value = value; }
        }

        // Struct tham số cho BackgroundWorker
        private struct LoadArgs
        {
            public bool IsReload;
            public int KyHoc;
            public string MaKyHoc;
            public string Keyword;
            public string HeDaoTao;
            public int Page;
            public int Size;
            public SemesterSyncEngine.SyncStatusFilter FilterMode;
        }

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

            // 2. Setup BackgroundWorker
            _bgwLoad = new BackgroundWorker();
            _bgwLoad.WorkerReportsProgress = true;
            _bgwLoad.DoWork += _bgwLoad_DoWork;
            _bgwLoad.ProgressChanged += _bgwLoad_ProgressChanged;
            _bgwLoad.RunWorkerCompleted += _bgwLoad_RunWorkerCompleted;

            // 3. Events
            this.Load += SemesterSyncUC_Load;
            btnSemFilter.Click += btnSemFilter_Click;
            btnNextPage.Click += btnNextPage_Click;
            btnPrevPage.Click += btnPrevPage_Click;

            btnSyncSemStudents.Click += btnSyncSemStudents_Click; // Đồng bộ chọn


            // Context Menu Events
            dgvSemesterPreview.CellMouseDown += dgv_CellMouseDown;
            mnu_ViewEnrolledUser.Click += mnu_ViewEnrolledUser_Click;

            // Xử lý vẽ row
            dgvSemesterPreview.CellFormatting += DgvSemesterPreview_CellFormatting;

            // 4. Setup ComboBox Status Filter
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
        }

        private void SemesterSyncUC_Load(object sender, EventArgs e)
        {
            // Setup ComboBox Hệ Đào Tạo
            cbbHDT.Items.Clear();
            cbbHDT.Items.Add(new ComboBoxItem("Hệ Chính Quy", HE_CHINH_QUY));
            cbbHDT.Items.Add(new ComboBoxItem("Hệ Vừa Học Vừa Làm", HE_VHVL));
            cbbHDT.SelectedIndex = 0;
            cbbHDT.DisplayMember = "Text";
            cbbHDT.ValueMember = "Value";



            // Set text mặc định cho btnSyncAll
            btnSyncAll.Text = "Đồng bộ toàn bộ";
        }

        private void SetupGrid()
        {
            dgvSemesterPreview.AutoGenerateColumns = false;
            dgvSemesterPreview.AllowUserToAddRows = false;
            dgvSemesterPreview.ReadOnly = true;
            dgvSemesterPreview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSemesterPreview.Columns.Clear();

            AddCol("MaLopTinChi", "Mã Lớp", 100);
            AddCol("MonHoc", "Tên Môn Học", 200, true);
            AddCol("SiSoSQL", "Sĩ số SQL", 80);
            AddCol("SiSoMoodle", "Sĩ số Moodle", 80);
            AddCol("SoLuongThieu", "Thiếu (Enroll)", 100);
            AddCol("SoLuongThua", "Thừa (Unenroll)", 100);
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
            _currentMaKyHoc = yearStr.Length > 3 ? yearStr.Substring(yearStr.Length - 3) : yearStr;
            _currentKeyword = cboSemClass.Text.Trim();

            // Lấy Hệ đào tạo đang chọn
            var selectedHe = cbbHDT.SelectedItem as ComboBoxItem;
            _currentHeDaoTao = selectedHe != null ? selectedHe.Value.ToString() : HE_CHINH_QUY;

            var selectedStatus = cboStatusFilter.SelectedItem as ComboBoxItem;
            _currentFilter = selectedStatus != null
                ? (SemesterSyncEngine.SyncStatusFilter)selectedStatus.Value
                : SemesterSyncEngine.SyncStatusFilter.All;

            _currentPage = 1;
            LoadData(true); // True = Load lại từ SQL
        }

        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadData(false); // False = Lấy từ RAM
            }
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (_totalLoaded >= _pageSize)
            {
                _currentPage++;
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
                HeDaoTao = _currentHeDaoTao, // Truyền hệ vào worker
                Page = _currentPage,
                Size = _pageSize,
                FilterMode = _currentFilter
            };

            _bgwLoad.RunWorkerAsync(args);
        }

        private void _bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = (LoadArgs)e.Argument;

            // Bước 1: Load SQL (có lọc theo Hệ)
            if (args.IsReload)
            {
                // Gọi hàm Load SQL mới (có tham số HeDaoTao)
                _engine.LoadAndGroupSqlData(args.KyHoc, args.MaKyHoc, args.Keyword, args.HeDaoTao);
            }

            // Bước 2: Get Page
            e.Result = _engine.GetPageData(args.Page, args.Size, args.FilterMode);
        }

        private void _bgwLoad_ProgressChanged(object sender, ProgressChangedEventArgs e) { }

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
            btnNextPage.Enabled = _totalLoaded == _pageSize;
        }

        // ==========================================
        // 3. SYNC ACTION (ĐỒNG BỘ)
        // ==========================================

        // Đồng bộ các lớp được chọn (Manual Sync)
        private void btnSyncSemStudents_Click(object sender, EventArgs e)
        {
            var selectedItems = new List<SemesterSyncViewModel>();
            foreach (DataGridViewRow row in dgvSemesterPreview.SelectedRows)
            {
                var item = row.DataBoundItem as SemesterSyncViewModel;
                if (item != null) selectedItems.Add(item);
            }

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một lớp để đồng bộ.");
                return;
            }

            if (MessageBox.Show(string.Format("Bạn có chắc chắn muốn đồng bộ {0} lớp đã chọn?", selectedItems.Count),
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            SetUIState(false);
            lblPageNumber.Text = "Đang thực hiện đồng bộ...";

            Task.Factory.StartNew(() =>
            {
                try
                {
                    // Gọi hàm SyncList (Manual Sync)
                    // Lưu ý: SyncList sẽ dùng hệ mặc định (hoặc logic trong Engine)
                    // Nếu muốn sync đúng hệ, bạn cần sửa SyncList trong Engine để nhận MaHe, nhưng hiện tại Engine mặc định CQ cho manual
                    _engine.SyncList(selectedItems, _currentMaKyHoc);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi đồng bộ: " + ex.Message);
                }
            }).ContinueWith(t =>
            {
                this.Invoke((Action)(() =>
                {
                    MessageBox.Show("Đồng bộ hoàn tất!");
                    LoadData(false);
                }));
            });
        }

        // Đồng bộ toàn bộ (Sync All) - Theo Hệ đã chọn
        private void btnSyncAll_Click(object sender, EventArgs e)
        {
            string namHoc = txtSemYear.Text.Trim();

            // Xác định hệ đang chọn để hiển thị thông báo đúng
            string heText = (_currentHeDaoTao == HE_CHINH_QUY) ? "CHÍNH QUY" : "VỪA HỌC VỪA LÀM";

            if (MessageBox.Show(string.Format("Bạn có chắc chắn muốn đồng bộ TOÀN BỘ dữ liệu hệ {0} - Học kỳ {1}?\nQuá trình này có thể mất nhiều thời gian.", heText, namHoc),
                "Xác nhận Sync All", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            SetUIState(false);
            rtbLog.Clear();

            Task.Factory.StartNew(() =>
            {
                try
                {
                    // Gọi hàm Sync tương ứng với hệ
                    if (_currentHeDaoTao == HE_CHINH_QUY)
                    {
                        _engine.SyncAllDatabase_CQ(namHoc);
                    }
                    else if (_currentHeDaoTao == HE_VHVL)
                    {
                        _engine.SyncAllDatabase_VHVL(namHoc);
                    }
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
            if (_currentClickedDgv == null)
            {
                MessageBox.Show("Vui lòng chọn lại khóa học.");
                return;
            }

            var courseInfo = GetSelectedCourseInfo(_currentClickedDgv);
            long courseId = courseInfo.Item1;
            string courseName = courseInfo.Item2;

            if (courseId > 0)
            {
                var frm = new ListUserCourse(courseId, courseName);
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Khóa học này chưa được tạo trên Moodle hoặc ID không hợp lệ.");
            }
        }

        // ==========================================
        // 5. HELPERS & STYLING
        // ==========================================

        private void SetUIState(bool enabled)
        {
            grpSemesterFilter.Enabled = enabled;
            pnlSemesterActions.Enabled = enabled;
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
    }
}