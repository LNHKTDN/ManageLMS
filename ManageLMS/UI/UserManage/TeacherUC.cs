using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ManageLMS.BLL.SyncEngine;
using ManageLMS.BLL.Manager.UserManager;
using ManageLMS.Common.DTO.ViewModel;
using System.Threading.Tasks;
using ManageLMS.DTO.Model;
using ManageLMS.UI.Course.UserCourse;

namespace ManageLMS.UI.UserManage
{
    public partial class TeacherUC : UserControl
    {
        #region Fields

        private TeacherSyncEngine _teacherEngine;
        private UserManager _userManager;

        private int _currentPage = 1;
        private int _pageSize = 50;
        private int _totalRecords = 0;

        #endregion

        #region Constructor

        public TeacherUC()
        {
            InitializeComponent();
            InitializeGridColumns();

            // Initialize Engine
            _teacherEngine = new TeacherSyncEngine();
            _userManager = new UserManager();

            _teacherEngine.OnLogMessage += Engine_OnLogMessage;
            _teacherEngine.OnProgress += Engine_OnProgress;

            // Register event handlers
            RegisterEventHandlers();
        }

        #endregion

        #region Initialization

        private void InitializeGridColumns()
        {
            // Xóa hết cột cũ (nếu có) và chặn tự sinh cột
            dgvTeachers.Columns.Clear();
            dgvTeachers.AutoGenerateColumns = false;

            // 1. Cột Checkbox (Chọn)
            DataGridViewCheckBoxColumn colSelect = new DataGridViewCheckBoxColumn();
            colSelect.Name = "colSelect";
            colSelect.DataPropertyName = "IsSelected"; // Map với property IsSelected
            colSelect.HeaderText = "Chọn";
            colSelect.Width = 50;
            colSelect.ReadOnly = false; // Cho phép tick
            dgvTeachers.Columns.Add(colSelect);

            // 2. Cột Mã GV
            DataGridViewTextBoxColumn colMaGV = new DataGridViewTextBoxColumn();
            colMaGV.Name = "colMaGV";
            colMaGV.DataPropertyName = "MaGiaoVien";
            colMaGV.HeaderText = "Mã Giảng Viên";
            colMaGV.Width = 120;
            colMaGV.ReadOnly = true;
            dgvTeachers.Columns.Add(colMaGV);

            // 3. Cột Họ Tên
            DataGridViewTextBoxColumn colHoTen = new DataGridViewTextBoxColumn();
            colHoTen.Name = "colHoTen";
            colHoTen.DataPropertyName = "HoTen";
            colHoTen.HeaderText = "Họ và Tên";
            colHoTen.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // Tự giãn
            colHoTen.ReadOnly = true;
            dgvTeachers.Columns.Add(colHoTen);

            // 4. Cột Email
            DataGridViewTextBoxColumn colEmail = new DataGridViewTextBoxColumn();
            colEmail.Name = "colEmail";
            colEmail.DataPropertyName = "E_Mail";
            colEmail.HeaderText = "Email";
            colEmail.Width = 200;
            colEmail.ReadOnly = true;
            dgvTeachers.Columns.Add(colEmail);

            // 5. Cột Khoa
            DataGridViewTextBoxColumn colKhoa = new DataGridViewTextBoxColumn();
            colKhoa.Name = "colKhoa";
            colKhoa.DataPropertyName = "TenKhoa";
            colKhoa.HeaderText = "Khoa / Đơn vị";
            colKhoa.Width = 180;
            colKhoa.ReadOnly = true;
            dgvTeachers.Columns.Add(colKhoa);

            // 6. Cột Trạng thái Moodle
            DataGridViewTextBoxColumn colStatus = new DataGridViewTextBoxColumn();
            colStatus.Name = "colStatus";
            colStatus.DataPropertyName = "TrangThaiMoodle";
            colStatus.HeaderText = "Trạng thái";
            colStatus.Width = 120;
            colStatus.ReadOnly = true;
            dgvTeachers.Columns.Add(colStatus);
        }

        private void RegisterEventHandlers()
        {
            // Form events
            this.Load += TeacherUC_Load;

            // DataGridView events
            this.dgvTeachers.RowPrePaint += DgvTeachers_RowPrePaint;
            this.dgvTeachers.CurrentCellDirtyStateChanged += DgvTeachers_CurrentCellDirtyStateChanged;
            this.dgvTeachers.CellMouseDown += dgvTeachers_CellMouseDown;

            // Search events
            this.btnTeacherSearch.Click += BtnTeacherSearch_Click;
            this.chkTeacherOnlyMissing.CheckedChanged += ChkTeacherOnlyMissing_CheckedChanged;

            // Paging events
            this.btnNextPage.Click += BtnNextPage_Click;
            this.btnPrevPage.Click += BtnPrevPage_Click;

            // Action events
            this.btnTeacherSyncSelected.Click += BtnTeacherSyncSelected_Click;
            this.btnTeacherSyncAll.Click += BtnTeacherSyncAll_Click;
            this.btnReload.Click += btnReload_Click;
            this.cbxSelectAll.CheckedChanged += chxSelectAll_CheckedChanged;

            // TextBox events
            this.txtTeacherSearch.Enter += TxtTeacherSearch_Enter;
            this.txtTeacherSearch.Leave += TxtTeacherSearch_Leave;
            this.txtTeacherSearch.KeyPress += TxtTeacherSearch_KeyPress;
        }

        #endregion

        #region Load Event

        private void TeacherUC_Load(object sender, EventArgs e)
        {
            // Set initial placeholder text
            SetPlaceholderText();
            LoadDataToGrid();
        }

        #endregion

        #region Responsive Event Handlers

        private void TeacherUC_Resize(object sender, EventArgs e)
        {
            try
            {
                if (this.Width < 200 || this.Height < 200) return;

                int panel1MinSize = Math.Max(300, splitMain.Panel1MinSize);
                int panel2MinSize = Math.Max(200, splitMain.Panel2MinSize);

                if (this.Width < 1000)
                {
                    if (splitMain.Orientation != Orientation.Vertical)
                    {
                        splitMain.Orientation = Orientation.Vertical;
                        int safeDistance = Math.Max(panel1MinSize,
                            Math.Min((int)(this.Width * 0.65), this.Width - panel2MinSize));
                        splitMain.SplitterDistance = safeDistance;
                    }
                }
                else
                {
                    if (splitMain.Orientation != Orientation.Horizontal)
                    {
                        splitMain.Orientation = Orientation.Horizontal;
                        int safeDistance = Math.Max(panel1MinSize,
                            Math.Min((int)(this.Height * 0.65), this.Height - panel2MinSize));
                        splitMain.SplitterDistance = safeDistance;
                    }
                }

                if (pnlPaging != null && lblTotalRecords != null && pnlPaging.Width > 400)
                {
                    lblTotalRecords.Location = new Point(pnlPaging.Width - 240, 10);
                }
            }
            catch { }
        }

        private void splitMain_SplitterMoved(object sender, SplitterEventArgs e)
        {
            try
            {
                int panel1MinSize = Math.Max(300, splitMain.Panel1MinSize);
                int panel2MinSize = Math.Max(200, splitMain.Panel2MinSize);

                if (splitMain.Orientation == Orientation.Horizontal)
                {
                    int maxDistance = splitMain.Height - panel2MinSize - splitMain.SplitterWidth;
                    if (splitMain.SplitterDistance < panel1MinSize)
                        splitMain.SplitterDistance = panel1MinSize;
                    else if (splitMain.SplitterDistance > maxDistance)
                        splitMain.SplitterDistance = Math.Max(panel1MinSize, maxDistance);
                }
                else
                {
                    int maxDistance = splitMain.Width - panel2MinSize - splitMain.SplitterWidth;
                    if (splitMain.SplitterDistance < panel1MinSize)
                        splitMain.SplitterDistance = panel1MinSize;
                    else if (splitMain.SplitterDistance > maxDistance)
                        splitMain.SplitterDistance = Math.Max(panel1MinSize, maxDistance);
                }
            }
            catch { }
        }

        #endregion

        #region TextBox Event Handlers

        private void SetPlaceholderText()
        {
            if (string.IsNullOrWhiteSpace(txtTeacherSearch.Text) ||
                txtTeacherSearch.Text == "   Mã GV hoặc họ tên...")
            {
                txtTeacherSearch.Text = "   Mã GV hoặc họ tên...";
                txtTeacherSearch.ForeColor = Color.Gray;
            }
        }

        private void txtTeacherSearch_Enter(object sender, EventArgs e)
        {
            if (txtTeacherSearch.Text == "   Mã GV hoặc họ tên..." &&
                txtTeacherSearch.ForeColor == Color.Gray)
            {
                txtTeacherSearch.Text = "";
                txtTeacherSearch.ForeColor = Color.Black;
            }
        }

        private void txtTeacherSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTeacherSearch.Text))
            {
                txtTeacherSearch.Text = "   Mã GV hoặc họ tên...";
                txtTeacherSearch.ForeColor = Color.Gray;
            }
        }

        private void TxtTeacherSearch_Enter(object sender, EventArgs e)
        {
            txtTeacherSearch_Enter(sender, e);
        }

        private void TxtTeacherSearch_Leave(object sender, EventArgs e)
        {
            txtTeacherSearch_Leave(sender, e);
        }

        private void TxtTeacherSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                BtnTeacherSearch_Click(sender, e);
            }
        }

        #endregion

        #region DataGridView Event Handlers

        private void DgvTeachers_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvTeachers.IsCurrentCellDirty)
            {
                dgvTeachers.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void DgvTeachers_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvTeachers.Rows.Count)
            {
                var rowData = dgvTeachers.Rows[e.RowIndex].DataBoundItem as TeacherSyncViewModel;

                if (rowData != null)
                {
                    // Priority 1: Missing accounts -> Yellow
                    if (rowData.IsMissing)
                    {
                        dgvTeachers.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                        dgvTeachers.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                    }
                    // Priority 2: Suspended accounts -> Pink/Red
                    else if (rowData.IsSuspended)
                    {
                        dgvTeachers.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightPink;
                        dgvTeachers.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;
                    }
                    // Default: White
                    else
                    {
                        dgvTeachers.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                        dgvTeachers.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                    }
                }
            }
        }

        private void dgvTeachers_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    if (!dgvTeachers.Rows[e.RowIndex].Selected)
                    {
                        dgvTeachers.ClearSelection();
                        dgvTeachers.CurrentCell = dgvTeachers.Rows[e.RowIndex].Cells[e.ColumnIndex];
                        dgvTeachers.Rows[e.RowIndex].Selected = true;
                    }

                    var relativeMousePosition = dgvTeachers.PointToClient(Cursor.Position);
                    ctxGVAction.Show(dgvTeachers, relativeMousePosition);
                }
            }
        }

        #endregion

        #region Search & Filter Event Handlers

        private void BtnTeacherSearch_Click(object sender, EventArgs e)
        {
            _currentPage = 1;
            LoadDataToGrid();
        }

        private void ChkTeacherOnlyMissing_CheckedChanged(object sender, EventArgs e)
        {
            _currentPage = 1;
            LoadDataToGrid();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadDataToGrid();
        }

        private void chxSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            var list = dgvTeachers.DataSource as List<TeacherSyncViewModel>;

            if (list != null)
            {
                bool checkState = cbxSelectAll.Checked;
                foreach (var item in list)
                {
                    if (item.IsMissing)
                    {
                        item.IsSelected = checkState;
                    }
                }
                dgvTeachers.Refresh();
            }
        }

        #endregion

        #region Paging Event Handlers

        private void BtnNextPage_Click(object sender, EventArgs e)
        {
            if (_currentPage * _pageSize < _totalRecords)
            {
                _currentPage++;
                LoadDataToGrid();
            }
        }

        private void BtnPrevPage_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadDataToGrid();
            }
        }

        #endregion

        #region Sync Event Handlers

        private void BtnTeacherSyncSelected_Click(object sender, EventArgs e)
        {
            var currentList = dgvTeachers.DataSource as List<TeacherSyncViewModel>;
            if (currentList == null || currentList.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để đồng bộ.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var listToSync = currentList
                .Where(x => x.IsSelected == true && x.IsMissing == true)
                .ToList();

            if (listToSync.Count == 0)
            {
                MessageBox.Show("Vui lòng tích chọn ít nhất 1 giảng viên cần tạo mới (Màu vàng).",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string confirmMsg = string.Format("Bạn có chắc muốn tạo tài khoản cho {0} giảng viên đã chọn?", listToSync.Count);
            if (MessageBox.Show(confirmMsg, "Xác nhận đồng bộ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            RunSyncWorker(() => _teacherEngine.SyncFromViewList(currentList));
        }

        private void BtnTeacherSyncAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("CẢNH BÁO: Chức năng này sẽ duyệt toàn bộ CSDL và đồng bộ lên Moodle.\n" +
                "Thời gian có thể lâu.\nBạn có chắc chắn không?",
                "Xác nhận Sync All", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            RunSyncWorker(() => _teacherEngine.SyncAllDatasbase());
        }

        #endregion

        #region Context Menu Event Handlers

        private void mnuUserUpdate_Click(object sender, EventArgs e)
        {
            var listUsernames = GetTargetIdNumbers();
            if (listUsernames.Count == 0) return;
            if (listUsernames.Count > 1)
            {
                MessageBox.Show("Vui lòng chỉ chọn 1 tài khoản.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string username = listUsernames[0];
            var currentUser = ManageLMS.BLL.Cache.MoodleCache.GetUser(username);

            if (currentUser == null)
            {
                MessageBox.Show("Không tìm thấy thông tin user này.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MoodleUser userToEdit = currentUser.Clone();

            using (var frm = new UserEditForm(userToEdit))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    PerformUpdateUser(frm.UpdatedUser);
                }
            }
        }

        private void mnuDelete_Click(object sender, EventArgs e)
        {
            var listUsernames = GetTargetIdNumbers();
            if (listUsernames.Count == 0) return;

            string msg = string.Format("CẢNH BÁO NGUY HIỂM:\nBạn có chắc chắn muốn XÓA VĨNH VIỄN {0} tài khoản này trên Moodle không?\n\nDữ liệu bài làm và điểm số sẽ bị mất và KHÔNG THỂ khôi phục!", listUsernames.Count);

            if (MessageBox.Show(msg, "Xác nhận Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                return;

            SetLoadingState(true);

            Task.Factory.StartNew(() =>
            {
                int successCount = 0;
                foreach (var username in listUsernames)
                {
                    if (_userManager.DeleteUser(username))
                        successCount++;
                }
                return successCount;
            })
            .ContinueWith(t =>
            {
                SetLoadingState(false);

                if (t.Exception != null)
                {
                    string errorMessage = "Lỗi: " + (t.Exception.InnerException != null ?
                        t.Exception.InnerException.Message : t.Exception.Message);
                    MessageBox.Show(errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string successMsg = string.Format("Đã xoá thành công {0}/{1} tài khoản.",
                        t.Result, listUsernames.Count);
                    MessageBox.Show(successMsg, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataToGrid();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void mnuChangePass_Click(object sender, EventArgs e)
        {
            var listUsernames = GetTargetIdNumbers();
            if (listUsernames.Count == 0) return;

            string msgUser = listUsernames.Count == 1 ?
                string.Format("cho {0}", listUsernames[0]) :
                string.Format("cho {0} tài khoản", listUsernames.Count);

            string dialogTitle = string.Format("Nhập mật khẩu mới {0}:", msgUser);
            string newPass = ShowInputDialog(dialogTitle, "Đổi mật khẩu");

            if (string.IsNullOrEmpty(newPass)) return;
            if (newPass.Length < 8)
            {
                MessageBox.Show("Mật khẩu phải dài ít nhất 8 ký tự!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetLoadingState(true);

            Task.Factory.StartNew(() =>
            {
                int successCount = 0;
                foreach (var username in listUsernames)
                {
                    if (_userManager.UpdateUserPassword(username, newPass))
                        successCount++;
                }
                return successCount;
            })
            .ContinueWith(t =>
            {
                SetLoadingState(false);

                if (t.Exception != null)
                {
                    string errorMessage = "Lỗi: " + (t.Exception.InnerException != null ?
                        t.Exception.InnerException.Message : t.Exception.Message);
                    MessageBox.Show(errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string successMsg = string.Format("Đã cập nhật mật khẩu thành công cho {0}/{1} người.",
                        t.Result, listUsernames.Count);
                    MessageBox.Show(successMsg, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void mnuLockAccount_Click_1(object sender, EventArgs e)
        {
            var listUsernames = GetTargetIdNumbers();
            if (listUsernames.Count == 0) return;

            string confirmMsg = string.Format("Bạn có chắc muốn KHÓA {0} tài khoản?", listUsernames.Count);
            if (MessageBox.Show(confirmMsg, "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            SetLoadingState(true);

            Task.Factory.StartNew(() =>
            {
                int successCount = 0;
                foreach (var username in listUsernames)
                {
                    if (_userManager.SuspendUser(username))
                        successCount++;
                }
                return successCount;
            })
            .ContinueWith(t =>
            {
                SetLoadingState(false);

                if (t.Exception != null)
                {
                    string errorMessage = "Lỗi: " + (t.Exception.InnerException != null ?
                        t.Exception.InnerException.Message : t.Exception.Message);
                    MessageBox.Show(errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string successMsg = string.Format("Đã khóa thành công {0}/{1} tài khoản.",
                        t.Result, listUsernames.Count);
                    MessageBox.Show(successMsg, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataToGrid();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void mnuUnlockAccount_Click(object sender, EventArgs e)
        {
            var listUsernames = GetTargetIdNumbers();
            if (listUsernames.Count == 0) return;

            SetLoadingState(true);

            Task.Factory.StartNew(() =>
            {
                int successCount = 0;
                foreach (var username in listUsernames)
                {
                    if (_userManager.UnlockUser(username))
                        successCount++;
                }
                return successCount;
            })
            .ContinueWith(t =>
            {
                SetLoadingState(false);
                if (t.Exception != null)
                {
                    string errorMessage = "Lỗi: " + (t.Exception.InnerException != null ?
                        t.Exception.InnerException.Message : t.Exception.Message);
                    MessageBox.Show(errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string successMsg = string.Format("Đã mở khóa thành công {0}/{1} tài khoản.",
                        t.Result, listUsernames.Count);
                    MessageBox.Show(successMsg, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataToGrid();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void mnu_UserCourse_Click_1(object sender, EventArgs e)
        {
            var listUsernames = GetTargetIdNumbers();

            if (listUsernames.Count == 0) return;

            if (listUsernames.Count > 1)
            {
                MessageBox.Show("Vui lòng chỉ chọn 1 tài khoản để xem khóa học.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string username = listUsernames[0];
            var moodleUser = ManageLMS.BLL.Cache.MoodleCache.GetUser(username);

            if (moodleUser == null)
            {
                MessageBox.Show("Không tìm thấy thông tin user này trong bộ nhớ đệm.\n" +
                    "Vui lòng đảm bảo user đã tồn tại trên Moodle (Trạng thái: Đã có).",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UserCourseView frm = new UserCourseView(moodleUser);
            frm.Show();
        }

        #endregion

        #region Data Loading and UI Updates

        private void SetLoadingState(bool isLoading)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool>(SetLoadingState), isLoading);
                return;
            }

            if (isLoading)
            {
                this.Cursor = Cursors.WaitCursor;
                this.Enabled = false;
            }
            else
            {
                this.Cursor = Cursors.Default;
                this.Enabled = true;
            }
        }

        private void LoadDataToGrid()
        {
            try
            {
                SetLoadingState(true);

                string keyword = "";
                if (txtTeacherSearch.Text != "   Mã GV hoặc họ tên..." &&
                    txtTeacherSearch.ForeColor != Color.Gray)
                {
                    keyword = txtTeacherSearch.Text.Trim();
                }

                bool onlyMissing = chkTeacherOnlyMissing.Checked;
                cbxSelectAll.Checked = false;

                var listData = _teacherEngine.SearchAndCompare(keyword, _currentPage, _pageSize, onlyMissing);
                dgvTeachers.DataSource = listData;

                if (listData.Count > 0)
                {
                    _totalRecords = listData[0].TotalRecords;
                    lblTotalRecords.Text = string.Format("📊 Tổng: {0} bản ghi", _totalRecords);
                }
                else
                {
                    lblTotalRecords.Text = "📊 Không tìm thấy dữ liệu";
                    _totalRecords = 0;
                }

                int totalPages = (_totalRecords + _pageSize - 1) / _pageSize;
                lblPageInfo.Text = string.Format("Trang {0}/{1}", _currentPage, Math.Max(1, totalPages));

                btnPrevPage.Enabled = _currentPage > 1;
                btnNextPage.Enabled = _currentPage < totalPages;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        #endregion

        #region Sync Operations

        private void RunSyncWorker(Action syncAction)
        {
            SetButtonsEnabled(false);

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, args) =>
            {
                try
                {
                    syncAction();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi trong quá trình Sync: " + ex.Message, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            worker.RunWorkerCompleted += (s, args) =>
            {
                SetButtonsEnabled(true);
                MessageBox.Show("Quy trình đồng bộ hoàn tất!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDataToGrid();
            };

            worker.RunWorkerAsync();
        }

        private void PerformUpdateUser(MoodleUser user)
        {
            SetLoadingState(true);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _userManager.UpdateUsersBatch(new List<MoodleUser> { user });
                    ManageLMS.BLL.Cache.MoodleCache.AddToCache(user.username, user);
                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            })
            .ContinueWith(t =>
            {
                SetLoadingState(false);

                if (t.Exception != null)
                {
                    string errorMessage = "Lỗi cập nhật: " + (t.Exception.InnerException != null ?
                        t.Exception.InnerException.Message : t.Exception.Message);
                    MessageBox.Show(errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Cập nhật thông tin thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateGridRowClientSide(user);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void UpdateGridRowClientSide(MoodleUser moodleUser)
        {
            var currentList = dgvTeachers.DataSource as List<TeacherSyncViewModel>;
            if (currentList == null) return;

            var rowItem = currentList.FirstOrDefault(x =>
                x.MaGiaoVien.Trim().ToLower() == moodleUser.idnumber.Trim().ToLower());

            if (rowItem != null)
            {
                rowItem.IsSuspended = (moodleUser.suspended == true);

                if (rowItem.IsSuspended)
                {
                    rowItem.TrangThaiMoodle = "Đã bị khóa";
                    rowItem.HanhDongGoiY = "Mở khóa";
                }
                else
                {
                    rowItem.TrangThaiMoodle = "Đang hoạt động";
                    rowItem.HanhDongGoiY = "Bỏ qua";
                }

                dgvTeachers.Refresh();
            }
        }

        #endregion

        #region Helper Methods

        private void Engine_OnLogMessage(string msg)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(Engine_OnLogMessage), msg);
                return;
            }
            Console.WriteLine("[TeacherSync] " + msg);
        }

        private void Engine_OnProgress(int current, int total)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int, int>(Engine_OnProgress), current, total);
                return;
            }
            lblTotalRecords.Text = string.Format("⏳ Xử lý: {0}/{1}", current, total);
        }

        private void UpdateStatus(string msg)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateStatus), msg);
                return;
            }
            this.Text = "Hệ thống quản lý LMS - " + msg;
        }

        private void SetButtonsEnabled(bool enabled)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool>(SetButtonsEnabled), enabled);
                return;
            }

            btnTeacherSearch.Enabled = enabled;
            btnTeacherSyncSelected.Enabled = enabled;
            btnTeacherSyncAll.Enabled = enabled;
            btnReload.Enabled = enabled;
            dgvTeachers.Enabled = enabled;
        }

        private List<TeacherSyncViewModel> GetSelectedItemsForAction()
        {
            var allData = dgvTeachers.DataSource as List<TeacherSyncViewModel>;
            if (allData == null) return new List<TeacherSyncViewModel>();

            var checkedItems = allData.Where(x => x.IsSelected).ToList();
            if (checkedItems.Count > 0)
            {
                return checkedItems;
            }

            if (dgvTeachers.CurrentRow != null)
            {
                var currentItem = dgvTeachers.CurrentRow.DataBoundItem as TeacherSyncViewModel;
                if (currentItem != null)
                {
                    return new List<TeacherSyncViewModel> { currentItem };
                }
            }

            return new List<TeacherSyncViewModel>();
        }

        private List<string> GetTargetIdNumbers()
        {
            var allData = dgvTeachers.DataSource as List<TeacherSyncViewModel>;
            if (allData == null) return new List<string>();

            var checkedIds = allData.Where(x => x.IsSelected).Select(x => x.MaGiaoVien).ToList();
            if (checkedIds.Count > 0) return checkedIds;

            if (dgvTeachers.CurrentRow != null)
            {
                var currentItem = dgvTeachers.CurrentRow.DataBoundItem as TeacherSyncViewModel;
                if (currentItem != null)
                {
                    return new List<string> { currentItem.MaGiaoVien };
                }
            }
            return new List<string>();
        }

        public static string ShowInputDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 180,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false
            };

            Label textLabel = new Label()
            {
                Left = 20,
                Top = 20,
                Text = text,
                Width = 350
            };

            TextBox textBox = new TextBox()
            {
                Left = 20,
                Top = 50,
                Width = 340,
                UseSystemPasswordChar = true
            };

            Button confirmation = new Button()
            {
                Text = "Đồng ý",
                Left = 240,
                Width = 100,
                Top = 90,
                DialogResult = DialogResult.OK
            };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        #endregion
    }
}