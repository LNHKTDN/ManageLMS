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

        // State variables
        private int _currentPage = 1;
        private int _pageSize = 100;
        private int _totalLoaded = 0;

        private string _currentMaKyHoc;
        private int _currentKyHocInt;

        // Lưu TrainingType thay vì string MaHe
        private MasterSyncEngine.TrainingType _currentTrainingType;
        private string _currentMaKhoa = "";

        private bool _isShowMissingOnly = false;

        // Class Helper cho ComboBox
        public class TrainingTypeItem
        {
            public string DisplayName { get; set; }
            public MasterSyncEngine.TrainingType Value { get; set; }
            public string SqlFilterCode { get; set; } // Mã dùng để lọc SQL (nếu cần)
        }

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
            btnSyncAll.Click += btnSyncAll_Click;
            btnNextPage.Click += btnNextPage_Click;
            btnPrevPage.Click += btnPrevPage_Click;

            chkShowOnlyMissing.CheckedChanged += ChkShowOnlyMissing_CheckedChanged;
            dgvMasterPreview.CellFormatting += DgvMasterPreview_CellFormatting;

            _engine = new MasterSyncEngine();
            _engine.OnLogMessage += AppendLog;
            _engine.OnProgress += UpdateProgress;

            LoadComboBoxes();
        }

        private void LoadComboBoxes()
        {
            try
            {
                // [THAY ĐỔI] Load Hệ Đào Tạo từ Enum TrainingType
                var listTrainingTypes = new List<TrainingTypeItem>
                {
                    new TrainingTypeItem { DisplayName = "Đại học - Chính Quy", Value = MasterSyncEngine.TrainingType.DaiHoc_ChinhQuy, SqlFilterCode = "CQ" },
                    new TrainingTypeItem { DisplayName = "Đại học - Vừa Làm Vừa Học", Value = MasterSyncEngine.TrainingType.DaiHoc_VHVL, SqlFilterCode = "VHVL" },
                    new TrainingTypeItem { DisplayName = "Sau Đại Học", Value = MasterSyncEngine.TrainingType.SauDaiHoc, SqlFilterCode = "SDH" }
                };

                cboHeDaoTao.DataSource = listTrainingTypes;
                cboHeDaoTao.DisplayMember = "DisplayName";
                cboHeDaoTao.ValueMember = "Value";
                cboHeDaoTao.SelectedIndex = 0;

                // Load Khoa (Giữ nguyên lấy từ DB để lọc)
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
            AddCol("TenHe", "Hệ", 100);
            AddCol("MaKhoa", "Khoa", 80);
            AddCol("SiSoSQL", "Sĩ số SQL", 80);
            AddCol("SiSoMoodle", "Sĩ số Moodle", 80);
            AddCol("SoLuongThieu", "Thiếu", 80);
            AddCol("TrangThai", "Trạng thái", 150);
            AddCol("MoodleShortname", "Shortname", 150);
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

        // =========================================================
        // 1. FILTER & LOAD DATA
        // =========================================================
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

            // Lấy giá trị lọc từ ComboBox (TrainingTypeItem)
            var selectedItem = cboHeDaoTao.SelectedItem as TrainingTypeItem;
            if (selectedItem == null) return;

            _currentTrainingType = selectedItem.Value; // Lưu TrainingType
            _currentMaKhoa = cboKhoa.SelectedValue != null ? cboKhoa.SelectedValue.ToString() : "";

            _currentPage = 1;
            rtbLog.Clear();
            LoadData(true);
        }

        private void LoadData(bool isReload)
        {
            if (_bgwLoad.IsBusy) return;

            SetUIState(false);
            dgvMasterPreview.DataSource = null;
            lblPageNumber.Text = isReload ? "Đang tải SQL..." : "Đang tải trang...";

            var args = new LoadArgs
            {
                IsReload = isReload,
                KyHoc = _currentKyHocInt,
                MaKyHoc = _currentMaKyHoc,
                TrainingType = _currentTrainingType, // Truyền Enum
                MaKhoa = _currentMaKhoa,
                Page = _currentPage,
                Size = _pageSize
            };

            _bgwLoad.RunWorkerAsync(args);
        }

        private void _bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = (LoadArgs)e.Argument;

            // Nếu Reload -> Gọi Engine tải lại từ SQL
            if (args.IsReload)
            {
                // Engine sẽ tự lấy MaHe lọc SQL dựa trên TrainingType
                _engine.LoadAndGroupMasterData(args.KyHoc, args.MaKyHoc, "", args.TrainingType, args.MaKhoa);
            }

            if (_isShowMissingOnly)
            {
                e.Result = _engine.GetAllMissingData();
            }
            else
            {
                e.Result = _engine.GetPageData(args.Page, args.Size, MasterSyncEngine.SyncStatusFilter.All);
            }
        }

        private void _bgwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetUIState(true);
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

        // =========================================================
        // 2. SYNC ACTIONS
        // =========================================================

        // A. Đồng bộ danh sách đang chọn (Manual Sync)
        private void btnSyncMaster_Click(object sender, EventArgs e)
        {
            var selected = new List<MasterSyncViewModel>();
            foreach (DataGridViewRow row in dgvMasterPreview.SelectedRows)
            {
                var item = row.DataBoundItem as MasterSyncViewModel;
                if (item != null) selected.Add(item);
            }

            if (selected.Count == 0) { MessageBox.Show("Chưa chọn lớp nào."); return; }

            // Lấy Hệ đang chọn trên ComboBox để đồng bộ đúng thư mục
            var selectedItem = cboHeDaoTao.SelectedItem as TrainingTypeItem;
            var targetType = selectedItem != null ? selectedItem.Value : MasterSyncEngine.TrainingType.DaiHoc_ChinhQuy;

            if (MessageBox.Show(string.Format("Đồng bộ {0} lớp Master đã chọn vào hệ [{1}]?", selected.Count, selectedItem.DisplayName), 
                "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                SetUIState(false);
                
                
                Task.Factory.StartNew(() => _engine.SyncList(selected, _currentMaKyHoc, targetType))
                    .ContinueWith(t =>
                    {
                        this.Invoke((Action)(() =>
                        {
                            SetUIState(true);
                            if (t.Exception != null) MessageBox.Show("Lỗi: " + t.Exception.InnerException.Message);
                            else MessageBox.Show("Hoàn tất!");
                            
                            LoadData(false); // Reload page
                        }));
                    });
            }
        }

        // B. Đồng bộ toàn bộ theo Hệ (Sync All)
        private void btnSyncAll_Click(object sender, EventArgs e)
        {
            string rawYear = txtSemYear.Text.Trim();
            if (string.IsNullOrEmpty(rawYear)) { MessageBox.Show("Vui lòng nhập Mã năm học."); return; }

            // 1. Lấy TrainingType từ ComboBox
            var selectedItem = cboHeDaoTao.SelectedItem as TrainingTypeItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn Hệ Đào Tạo.");
                return;
            }

            var trainingType = selectedItem.Value;

            // 2. Confirm
            string msg = string.Format("Bạn có chắc chắn muốn đồng bộ TOÀN BỘ dữ liệu?\n\n" +
                                       "- Hệ: {0}\n- Năm học: {1}\n- Thư mục đích: {2}\n\n",
                                       selectedItem.DisplayName, rawYear, trainingType.ToString());

            if (MessageBox.Show(msg, "Xác nhận Sync All", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            SetUIState(false);
            rtbLog.Clear();

            _currentMaKyHoc = rawYear.Length > 3 ? rawYear.Substring(rawYear.Length - 3) : rawYear;
            if (!int.TryParse(rawYear, out _currentKyHocInt)) _currentKyHocInt = 0;
            _currentTrainingType = trainingType;
            _currentMaKhoa = cboKhoa.SelectedValue != null ? cboKhoa.SelectedValue.ToString() : "";

            Task.Factory.StartNew(() =>
            {
                // B1: Tải lại dữ liệu (chỉ lấy đúng hệ này để sync)
                int count = _engine.LoadAndGroupMasterData(_currentKyHocInt, _currentMaKyHoc, "", _currentTrainingType, _currentMaKhoa);

                if (count == 0)
                {
                    this.Invoke((Action)(() => MessageBox.Show("Không tìm thấy dữ liệu nào.")));
                    return;
                }

                // B2: Gọi hàm Sync với TrainingType
                _engine.SyncAllDatabase(_currentMaKyHoc, trainingType);
            })
            .ContinueWith(t =>
            {
                this.Invoke((Action)(() =>
                {
                    SetUIState(true);
                    if (t.Exception != null) MessageBox.Show("Lỗi: " + t.Exception.InnerException.Message);
                    else
                    {
                        MessageBox.Show("Đã hoàn tất đồng bộ toàn bộ!");
                        _currentPage = 1;
                        LoadData(false);
                    }
                }));
            });
        }

        // =========================================================
        // UI HELPERS
        // =========================================================
        private void ChkShowOnlyMissing_CheckedChanged(object sender, EventArgs e)
        {
            _isShowMissingOnly = chkShowOnlyMissing.Checked;
            pnlPagination.Enabled = !_isShowMissingOnly;
            _currentPage = 1;
            LoadData(false);
        }

        private void btnNextPage_Click(object sender, EventArgs e) { _currentPage++; LoadData(false); }
        private void btnPrevPage_Click(object sender, EventArgs e) { if (_currentPage > 1) { _currentPage--; LoadData(false); } }

        private void DgvMasterPreview_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var item = dgvMasterPreview.Rows[e.RowIndex].DataBoundItem as MasterSyncViewModel;
            if (item != null)
            {
                if (item.MoodleCourseId == 0)
                {
                    dgvMasterPreview.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;
                    dgvMasterPreview.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.MistyRose;
                }
                else if (item.SoLuongThieu > 0)
                {
                    dgvMasterPreview.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.DarkOrange;
                    dgvMasterPreview.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                }
                else
                {
                    dgvMasterPreview.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Green;
                    dgvMasterPreview.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                }
            }
        }

        private void SetUIState(bool enabled)
        {
            btnFilterMaster.Enabled = enabled;
            btnSyncMaster.Enabled = enabled;
            btnSyncAll.Enabled = enabled;
            pnlPagination.Enabled = enabled && !_isShowMissingOnly;
            cboHeDaoTao.Enabled = enabled;
            cboKhoa.Enabled = enabled;
            txtSemYear.Enabled = enabled;
        }

        private void AppendLog(string msg)
        {
            if (rtbLog.InvokeRequired) rtbLog.Invoke((Action)(() => AppendLog(msg)));
            else
            {
                rtbLog.AppendText(string.Format("[{0}] {1}\n", DateTime.Now.ToString("HH:mm:ss"), msg));
                rtbLog.ScrollToCaret();
            }
        }

        private void UpdateProgress(int current, int total)
        {
            if (pbSyncProgress.InvokeRequired) pbSyncProgress.Invoke((Action)(() => UpdateProgress(current, total)));
            else
            {
                pbSyncProgress.Maximum = total;
                pbSyncProgress.Value = current > total ? total : current;
            }
        }

        private class LoadArgs
        {
            public bool IsReload;
            public int KyHoc;
            public string MaKyHoc;
            public MasterSyncEngine.TrainingType TrainingType; // Đã đổi sang Enum
            public string MaKhoa;
            public int Page;
            public int Size;
        }
    }
}