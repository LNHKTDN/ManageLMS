using ManageLMS.BLL.Manager.OtherManager;
using ManageLMS.BLL.SyncEngine;
using ManageLMS.Common.DTO.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManageLMS.UI.Sync
{
    public partial class MasterInitUC : UserControl
    {
        private MasterSyncEngine _engine;
        private BackgroundWorker _bgwLoad;
        private OtherManager _otherMgr;
        private int _currentPage = 1;
        private int _pageSize = 100;
        private int _totalLoaded = 0;
        
        private string _currentMaKyHoc;
        private int _currentKyHocInt;
        // Thêm 2 biến lưu trạng thái lọc
        private string _currentMaHe = "";
        private string _currentMaKhoa = "";
        private List<MasterSyncViewModel> _fullDataPage;
        private bool _isShowMissingOnly = false;
        public MasterInitUC()
        {
            InitializeComponent();
            _otherMgr = new OtherManager();
            InitializeCustom();
        }

        private void InitializeCustom()
        {
            SetupGrid();
            
            _bgwLoad = new BackgroundWorker();
            _bgwLoad.WorkerReportsProgress = true;
            _bgwLoad.DoWork += _bgwLoad_DoWork;
            _bgwLoad.RunWorkerCompleted += _bgwLoad_RunWorkerCompleted;

            btnFilterMaster.Click += btnFilterMaster_Click;
            btnSyncMaster.Click += btnSyncMaster_Click;
            btnNextPage.Click += btnNextPage_Click;
            btnPrevPage.Click += btnPrevPage_Click;

            chkShowOnlyMissing.CheckedChanged += ChkShowOnlyMissing_CheckedChanged;

            dgvMasterPreview.CellFormatting += DgvMasterPreview_CellFormatting;

            _engine = new MasterSyncEngine();
            // Đăng ký sự kiện Log & Progress
            _engine.OnLogMessage += AppendLog;
            _engine.OnProgress += UpdateProgress;
            LoadComboBoxes();
        }
        private void LoadComboBoxes()
        {
            try
            {
                // Load Hệ Đào Tạo
                var listHe = _otherMgr.GetListHeDaoTao(true); // true = thêm "Tất cả"
                cboHeDaoTao.DataSource = listHe;
                cboHeDaoTao.DisplayMember = "tenHe"; // Tên hiển thị (Property trong class HeDaoTao)
                cboHeDaoTao.ValueMember = "maHe";    // Giá trị (Property trong class HeDaoTao)

                // Load Khoa
                var listKhoa = _otherMgr.GetListKhoa(true);
                cboKhoa.DataSource = listKhoa;
                cboKhoa.DisplayMember = "TenKhoa";
                cboKhoa.ValueMember = "MaKhoa";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load danh mục: " + ex.Message);
            }
        }
        private void SetupGrid()
        {
            dgvMasterPreview.AutoGenerateColumns = false;
            dgvMasterPreview.AllowUserToAddRows = false;
            dgvMasterPreview.ReadOnly = true;
            dgvMasterPreview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMasterPreview.Columns.Clear();

            AddCol("MaHocPhan", "Mã Học Phần", 120);
            AddCol("TenHocPhan", "Tên Học Phần", 250, true);
            AddCol("SiSoSQL", "Sĩ số SQL", 80);
            AddCol("SiSoMoodle", "Sĩ số Moodle", 80);
            AddCol("SoLuongThieu", "Thiếu", 80);
            AddCol("TrangThai", "Trạng thái", 150);
            AddCol("MoodleShortname", "Shortname", 150);
        }
        private void ChkShowOnlyMissing_CheckedChanged(object sender, EventArgs e)
        {
            _isShowMissingOnly = chkShowOnlyMissing.Checked;

            // Khi check vào -> Disable phân trang, Load lại dữ liệu toàn bộ
            // Khi bỏ check -> Enable phân trang, Load lại trang 1
            pnlPagination.Enabled = !_isShowMissingOnly;
            _currentPage = 1;

            LoadData(false); // Gọi hàm LoadData để chạy Background Worker
        }

        
        private void ApplyLocalFilter()
        {
            if (_fullDataPage == null || _fullDataPage.Count == 0) return;

            List<MasterSyncViewModel> displayedList;

            if (chkShowOnlyMissing.Checked)
            {
                // Chỉ lấy các lớp có số lượng thiếu > 0 hoặc chưa tạo ID
                displayedList = _fullDataPage
                    .Where(x => x.SoLuongThieu > 0 || x.MoodleCourseId == 0)
                    .ToList();
            }
            else
            {
                // Hiển thị tất cả
                displayedList = _fullDataPage;
            }

            // Gán lại nguồn dữ liệu cho Grid
            dgvMasterPreview.DataSource = displayedList;

            // Cập nhật lại label đếm số lượng
            lblPageNumber.Text = string.Format("Trang {0} - Hiển thị {1}/{2} lớp",
                                               _currentPage, displayedList.Count, _totalLoaded);
        }
        private void AddCol(string propertyName, string headerText, int width, bool isFill = false)
        {
            var col = new DataGridViewTextBoxColumn();
            col.DataPropertyName = propertyName;
            col.HeaderText = headerText;
            if (isFill) col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            else col.Width = width;
            dgvMasterPreview.Columns.Add(col);
        }

        // LOGGING & PROGRESS
        private void AppendLog(string msg)
        {
            if (rtbLog.InvokeRequired)
                rtbLog.Invoke((Action)(() => AppendLog(msg)));
            else
            {
                rtbLog.AppendText(string.Format("[{0}] {1}\n", DateTime.Now.ToString("HH:mm:ss"), msg));
                rtbLog.ScrollToCaret();
            }
        }

        private void UpdateProgress(int current, int total)
        {
            if (pbSyncProgress.InvokeRequired)
                pbSyncProgress.Invoke((Action)(() => UpdateProgress(current, total)));
            else
            {
                pbSyncProgress.Maximum = total;
                pbSyncProgress.Value = current > total ? total : current;
            }
        }

        // EVENTS
        private void btnFilterMaster_Click(object sender, EventArgs e)
        {
            string rawYear = txtSemYear.Text.Trim();
            if (string.IsNullOrEmpty(rawYear)) { MessageBox.Show("Vui lòng nhập Mã năm học (VD: 241)"); return; }

            _currentMaKyHoc = rawYear.Length > 3 ? rawYear.Substring(rawYear.Length - 3) : rawYear;

            if (!int.TryParse(rawYear, out _currentKyHocInt))
            {
                MessageBox.Show("Mã năm học phải là số.");
                return;
            }

            // [MỚI] Lấy giá trị lọc từ ComboBox
            _currentMaHe = cboHeDaoTao.SelectedValue != null ? cboHeDaoTao.SelectedValue.ToString() : "";
            _currentMaKhoa = cboKhoa.SelectedValue != null ? cboKhoa.SelectedValue.ToString() : "";

            _currentPage = 1;
            rtbLog.Clear();
            LoadData(true);
        }

        private void LoadData(bool isReload)
        {
            if (_bgwLoad.IsBusy) return;

            btnFilterMaster.Enabled = false;
            dgvMasterPreview.DataSource = null;
            lblPageNumber.Text = isReload ? "Đang tải SQL..." : "Đang tải trang...";

            var args = new LoadArgs
            {
                IsReload = isReload,
                KyHoc = _currentKyHocInt,
                MaKyHoc = _currentMaKyHoc,
                MaHe = _currentMaHe,      // Truyền vào
                MaKhoa = _currentMaKhoa,  // Truyền vào
                Page = _currentPage,
                Size = _pageSize
            };

            _bgwLoad.RunWorkerAsync(args);
        }

        private void _bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = (LoadArgs)e.Argument;

            // Nếu có yêu cầu Reload (Lọc mới)
            if (args.IsReload)
            {
                _engine.LoadAndGroupMasterData(args.KyHoc, args.MaKyHoc, "", args.MaHe, args.MaKhoa);
            }

            // CHECK LOGIC TẠI ĐÂY
            if (_isShowMissingOnly)
            {
                // Nếu đang check "Chỉ hiện thiếu" -> Gọi hàm lấy toàn bộ
                e.Result = _engine.GetAllMissingData();
            }
            else
            {
                // Nếu không check -> Lấy phân trang bình thường
                e.Result = _engine.GetPageData(args.Page, args.Size, MasterSyncEngine.SyncStatusFilter.All);
            }
        }

        // Sửa RunWorkerCompleted
        private void _bgwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnFilterMaster.Enabled = true;
            if (e.Error != null) { MessageBox.Show("Lỗi: " + e.Error.Message); return; }

            var list = e.Result as List<MasterSyncViewModel>;
            dgvMasterPreview.DataSource = list;

            if (_isShowMissingOnly)
            {
                lblPageNumber.Text = string.Format("Đang hiển thị TOÀN BỘ {0} lớp thiếu (Mode: All)", list.Count);
                btnNextPage.Enabled = false;
                btnPrevPage.Enabled = false;
            }
            else
            {
                _totalLoaded = list != null ? list.Count : 0;
                lblPageNumber.Text = string.Format("Trang {0} - {1} lớp", _currentPage, _totalLoaded);
                btnNextPage.Enabled = _totalLoaded == _pageSize;
                btnPrevPage.Enabled = _currentPage > 1;
            }
        }

        private void btnSyncMaster_Click(object sender, EventArgs e)
        {
            var selected = new List<MasterSyncViewModel>();
            foreach (DataGridViewRow row in dgvMasterPreview.SelectedRows)
            {
                var item = row.DataBoundItem as MasterSyncViewModel;
                if (item != null) selected.Add(item);
            }

            if (selected.Count == 0) { MessageBox.Show("Chưa chọn lớp nào."); return; }

            if (MessageBox.Show(string.Format("Đồng bộ {0} lớp Master?",selected.Count), "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                btnSyncMaster.Enabled = false;
                Task.Factory.StartNew(() => _engine.SyncList(selected, _currentMaKyHoc))
                    .ContinueWith(t => 
                    {
                        this.Invoke((Action)(() => 
                        { 
                            btnSyncMaster.Enabled = true; 
                            MessageBox.Show("Hoàn tất!"); 
                            LoadData(false); // Reload page
                        }));
                    });
            }
        }

        private void btnNextPage_Click(object sender, EventArgs e) { _currentPage++; LoadData(false); }
        private void btnPrevPage_Click(object sender, EventArgs e) { if (_currentPage > 1) { _currentPage--; LoadData(false); } }

        private void DgvMasterPreview_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var item = dgvMasterPreview.Rows[e.RowIndex].DataBoundItem as MasterSyncViewModel;
            if (item != null)
            {
                if (item.MoodleCourseId == 0) dgvMasterPreview.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red; // Chưa tạo
                else if (item.TrangThai.Contains("Thiếu")) dgvMasterPreview.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Orange; // Lệch
                else dgvMasterPreview.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Green; // OK
            }
        }

        private class LoadArgs
        {
            public bool IsReload;
            public int KyHoc;
            public string MaKyHoc;
            public string MaHe;    // Mới
            public string MaKhoa;  // Mới
            public int Page;
            public int Size;
        }

        private void btnSyncAll_Click(object sender, EventArgs e)
        {
            
            string rawYear = txtSemYear.Text.Trim();

            if (string.IsNullOrEmpty(rawYear) || rawYear.Length < 3)
            {
                MessageBox.Show("Mã năm học phải có ít nhất 3 ký tự (VD: 241).", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSemYear.Focus();
                return;
            }

            int kyHocInt;
            if (!int.TryParse(rawYear, out kyHocInt))
            {
                MessageBox.Show("Mã năm học phải là số.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. CẬP NHẬT CÁC BIẾN CỤC BỘ (Lấy giá trị hiện tại trên UI)
            // Logic lấy mã kỳ (VD: 20241 -> 241)
            string maKyHoc = rawYear.Length > 3 ? rawYear.Substring(rawYear.Length - 3) : rawYear;
            
            // Lấy giá trị từ ComboBox (phải lấy trên UI Thread trước khi chạy Task)
            string maHe = cboHeDaoTao.SelectedValue != null ? cboHeDaoTao.SelectedValue.ToString() : "";
            string maKhoa = cboKhoa.SelectedValue != null ? cboKhoa.SelectedValue.ToString() : "";

            // Lưu lại vào biến toàn cục để đồng bộ trạng thái
            _currentKyHocInt = kyHocInt;
            _currentMaKyHoc = maKyHoc;
            _currentMaHe = maHe;
            _currentMaKhoa = maKhoa;

            // 3. XÁC NHẬN
            string msg = string.Format("Bạn có chắc chắn muốn đồng bộ TOÀN BỘ dữ liệu?\n\n" +
                                     "- Năm học: {0}\n- Hệ: {1}\n- Khoa: {2}\n\n" +
                                     "Quá trình này sẽ TỰ ĐỘNG TẢI DỮ LIỆU và xử lý ngầm.", 
                                     rawYear, 
                                     string.IsNullOrEmpty(maHe) ? "Tất cả" : cboHeDaoTao.Text, 
                                     string.IsNullOrEmpty(maKhoa) ? "Tất cả" : cboKhoa.Text);

            if (MessageBox.Show(msg, "Xác nhận Sync All", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            
            SetUIState(false);
            rtbLog.Clear();

            
            Task.Factory.StartNew(() =>
            {

                int count = _engine.LoadAndGroupMasterData(_currentKyHocInt, _currentMaKyHoc, "", _currentMaHe, _currentMaKhoa);
                
                if (count == 0)
                {
                    this.Invoke((Action)(() => 
                    { 
                        MessageBox.Show("Không tìm thấy dữ liệu nào phù hợp với điều kiện lọc."); 
                        SetUIState(true);
                    }));
                    return;
                }

                
                _engine.SyncAllDatabase(_currentMaKyHoc);
            })
            .ContinueWith(t =>
            {
                this.Invoke((Action)(() =>
                {
                    SetUIState(true);
                    
                    if (t.Exception != null)
                    {
                        MessageBox.Show("Có lỗi xảy ra: " + t.Exception.InnerException.Message);
                    }
                    else
                    {
                        MessageBox.Show("Đã hoàn tất đồng bộ toàn bộ!");
                        // Tự động load dữ liệu trang 1 lên lưới để người dùng xem kết quả
                        _currentPage = 1;
                        LoadData(false); 
                    }
                }));
            });
        }
        private void SetUIState(bool enabled)
        {
            btnFilterMaster.Enabled = enabled;
            btnSyncMaster.Enabled = enabled;
            btnSyncAll.Enabled = enabled;
            pnlPagination.Enabled = enabled;
            cboHeDaoTao.Enabled = enabled;
            cboKhoa.Enabled = enabled;
        }
    }
}