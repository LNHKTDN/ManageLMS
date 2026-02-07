using ManageLMS.BLL.Manager.OtherManager;
using ManageLMS.BLL.Manager.UserManager;
using ManageLMS.BLL.SyncEngine;
using ManageLMS.Common.DTO.ViewModel;
using ManageLMS.DTO.Model;
using ManageLMS.UI.Course.UserCourse;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks; // Cần cái này để chạy đa luồng
using System.Windows.Forms;

namespace ManageLMS.UI.UserManage
{
    public partial class StudentUC : UserControl
    {
        // 1. Khai báo các biến quản lý
        private StudentSyncEngine _syncEngine;
        private OtherManager _otherMgr;
        private UserManager _userManager;
        // Biến phân trang
        private int _currentPage = 1;
        private int _pageSize = 100;

        public StudentUC()
        {
            InitializeComponent();
            dgvStudents.AutoGenerateColumns = false; // Đảm bảo không tự sinh cột rác

            colMaSV.DataPropertyName = "MaSinhVien";       // Map vào cột Mã
            colHoTen.DataPropertyName = "HoTen";           // Map vào thuộc tính HoTen vừa tạo
            colEmail.DataPropertyName = "MaKhoaHoc";          // Map vào cột Email
            colLop.DataPropertyName = "LopSV";          // Map vào cột Khoa
            colStatus.DataPropertyName = "TrangThaiMoodle";// Map vào cột Trạng thái

            // Khởi tạo các Manager & Engine
            _otherMgr = new OtherManager();
            _syncEngine = new StudentSyncEngine();
            _userManager = new UserManager();

            // Đăng ký nhận Log từ Engine để hiện lên UI (nếu cần) hoặc Debug
            _syncEngine.OnLogMessage += (msg) =>
            {
                // Nếu muốn hiện log ra status bar thì viết vào đây
                System.Diagnostics.Debug.WriteLine(msg);
            };

            // Gán sự kiện
            this.Load += StudentUC_Load;

            // Sự kiện tìm kiếm / Lọc
            btnStudentSearch.Click += (s, e) => { _currentPage = 1; LoadDataToGrid(); };
            btnReload.Click += (s, e) => { LoadDataToGrid(); };

            // Sự kiện thay đổi bộ lọc -> Tự động load lại lớp
            cbxKhoa.SelectedIndexChanged += (s, e) => { LoadClasses(); };
            tbxKhoaNhapHoc.TextChanged += (s, e) => { LoadClasses(); };

            // Sự kiện Phân trang
            btnNextPage.Click += btnNextPage_Click;
            btnPrevPage.Click += btnPrevPage_Click;

            // Sự kiện Đồng bộ
            btnStudentSyncSelected.Click += btnStudentSyncSelected_Click;
            btnStudentSyncAll.Click += btnStudentSyncAll_Click;

            // Sự kiện Chọn tất cả
            cbxSelectAll.CheckedChanged += cbxSelectAll_CheckedChanged;

            // [QUAN TRỌNG] Sự kiện tô màu lưới
            dgvStudents.CellFormatting += dgvStudents_CellFormatting;

            // Sự kiện chuột phải để hiện context menu
            dgvStudents.CellMouseDown += dgvStudents_CellMouseDown;
        }

        private void StudentUC_Load(object sender, EventArgs e)
        {
            // Cấu hình GridView ban đầu (nếu Designer chưa chỉnh)
            dgvStudents.AutoGenerateColumns = false;

            LoadComboBoxData();

            // Xóa trắng lưới lúc đầu cho nhẹ
            dgvStudents.DataSource = null;
            lblTotalRecords.Text = "Vui lòng chọn điều kiện và bấm Tìm kiếm";
        }

        private void LoadComboBoxData()
        {
            try
            {
                // Load Khoa
                var listKhoa = _otherMgr.GetListKhoa(true);
                cbxKhoa.DataSource = listKhoa;
                cbxKhoa.DisplayMember = "TenKhoa";
                cbxKhoa.ValueMember = "MaKhoa";

                // Load Lớp (ban đầu load rỗng hoặc all)
                LoadClasses();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message);
            }
        }

        private void LoadClasses()
        {
            try
            {
                string selectedKhoa = "";
                if (cbxKhoa.SelectedValue != null)
                {
                    // Xử lý an toàn khi binding chưa xong
                    var val = cbxKhoa.SelectedValue;
                    if (val is string) selectedKhoa = val.ToString();
                    else if (val is ManageLMS.Common.DTO.Database.Khoa) selectedKhoa = ((ManageLMS.Common.DTO.Database.Khoa)val).MaKhoa;
                }

                string khoaHoc = tbxKhoaNhapHoc.Text.Trim();

                // Lấy danh sách lớp
                var listLop = _otherMgr.GetListLop(khoaHoc, selectedKhoa, true);
                cbxLop.DataSource = listLop;
            }
            catch { }
        }

        private void LoadDataToGrid()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                btnStudentSearch.Enabled = false;

                // 1. Lấy tham số
                string keyword = txtStudentSearch.Text.Trim();
                // Kiểm tra nếu là placeholder text thì bỏ qua
                if (keyword == "Mã SV hoặc họ tên..." && txtStudentSearch.ForeColor == Color.Gray)
                {
                    keyword = "";
                }

                bool onlyMissing = chkStudentOnlyMissing.Checked;

                string maKhoa = tbxKhoaNhapHoc.Text.Trim();
                // Kiểm tra nếu là placeholder text thì bỏ qua
                if (maKhoa == "VD: 2020, 2021..." && tbxKhoaNhapHoc.ForeColor == Color.Gray)
                {
                    maKhoa = "";
                }

                string lop = "";
                if (cbxLop.SelectedValue != null)
                    lop = cbxLop.SelectedValue.ToString();

                // 2. Chạy Search trên Thread khác để không đơ UI
                Task.Factory.StartNew(() =>
                {
                    return _syncEngine.SearchAndCompare(maKhoa, lop, keyword, _currentPage, _pageSize, onlyMissing);
                })
                .ContinueWith(t =>
                {
                    // Quay về luồng UI
                    this.Cursor = Cursors.Default;
                    btnStudentSearch.Enabled = true;

                    if (t.Exception != null)
                    {
                        MessageBox.Show("Lỗi tải dữ liệu: " + t.Exception.InnerException.Message);
                        return;
                    }

                    var resultList = t.Result;

                    // Gán vào Grid
                    dgvStudents.DataSource = resultList;

                    // Update UI Label - SỬA TÊN LABEL
                    lblPageInfo.Text = string.Format("Trang {0}", _currentPage);
                    lblTotalRecords.Text = string.Format("📊 Tổng: {0} bản ghi", resultList.Count);

                    // Xử lý nút Next/Prev
                    btnPrevPage.Enabled = _currentPage > 1;
                    btnNextPage.Enabled = resultList.Count >= _pageSize; // Nếu lấy đủ trang thì khả năng còn trang sau

                }, TaskScheduler.FromCurrentSynchronizationContext());

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
                this.Cursor = Cursors.Default;
                btnStudentSearch.Enabled = true;
            }
        }

        private void dgvStudents_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var item = dgvStudents.Rows[e.RowIndex].DataBoundItem as StudentSyncViewModel;
            if (item != null)
            {
                // Tô màu cả dòng
                DataGridViewRow row = dgvStudents.Rows[e.RowIndex];

                if (item.IsMissing)
                {
                    // Chưa có trên Moodle -> Màu vàng nhạt
                    row.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
                    row.DefaultCellStyle.ForeColor = Color.DarkGoldenrod;
                }
                else if (item.IsSuspended)
                {
                    // Bị khóa -> Màu đỏ nhạt
                    row.DefaultCellStyle.BackColor = Color.MistyRose;
                    row.DefaultCellStyle.ForeColor = Color.Red;
                }
                else
                {
                    // Bình thường -> Màu trắng
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }

        // Đồng bộ danh sách đang chọn (Checkbox)
        private void btnStudentSyncSelected_Click(object sender, EventArgs e)
        {
            var source = dgvStudents.DataSource as List<StudentSyncViewModel>;
            if (source == null || source.Count == 0) return;

            // Lọc ra những người được tick chọn
            var selectedItems = source.Where(x => x.IsSelected).ToList();

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng tích chọn sinh viên cần đồng bộ trên lưới.");
                return;
            }

            if (MessageBox.Show(string.Format("Bạn có chắc muốn đồng bộ {0} sinh viên đã chọn?", selectedItems.Count),
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            PerformSyncTask(() => _syncEngine.SyncFromViewList(source));
        }

        // Đồng bộ toàn bộ Database (Vét cạn)
        private void btnStudentSyncAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Chức năng này sẽ quét TOÀN BỘ CƠ SỞ DỮ LIỆU và tạo tài khoản cho tất cả sinh viên chưa có trên Moodle.\n\nViệc này có thể mất nhiều thời gian. Bạn có chắc chắn không?",
                "Cảnh báo Sync All", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            PerformSyncTask(() => _syncEngine.SyncAllDatasbase());
        }

        // Hàm chạy Sync chung (bọc Try/Catch và Loading)
        private void PerformSyncTask(Action syncAction)
        {
            ToggleUI(false); // Khóa nút

            Task.Factory.StartNew(() =>
            {
                syncAction(); // Chạy hàm sync
            })
            .ContinueWith(t =>
            {
                ToggleUI(true); // Mở lại nút

                if (t.Exception != null)
                {
                    MessageBox.Show("Có lỗi khi đồng bộ: " + t.Exception.InnerException.Message);
                }
                else
                {
                    MessageBox.Show("Đồng bộ hoàn tất!");
                    LoadDataToGrid(); // Load lại dữ liệu để thấy thay đổi (IsMissing = false)
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ToggleUI(bool enable)
        {
            // SỬA TÊN PANEL - Dùng panel mới
            pnlFilters.Enabled = enable;
            pnlPaging.Enabled = enable;
            dgvStudents.Enabled = enable;
            this.Cursor = enable ? Cursors.Default : Cursors.WaitCursor;
        }

        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadDataToGrid();
            }
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            _currentPage++;
            LoadDataToGrid();
        }

        private void cbxSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            var source = dgvStudents.DataSource as List<StudentSyncViewModel>;
            if (source == null) return;

            bool isChecked = cbxSelectAll.Checked;

            // Chỉ chọn những người Thiếu (IsMissing = true) theo logic UI
            foreach (var item in source)
            {
                if (item.IsMissing)
                {
                    item.IsSelected = isChecked;
                }
            }
            dgvStudents.Refresh(); // Vẽ lại lưới để thấy dấu tick
        }

        private void btnStudentSearch_Click(object sender, EventArgs e)
        {
            _currentPage = 1; // Reset về trang 1 mỗi khi bấm tìm mới
            LoadDataToGrid(); // Gọi hàm tải dữ liệu chính
        }

        private List<string> GetTargetIdNumbers()
        {
            var allData = dgvStudents.DataSource as List<StudentSyncViewModel>;
            if (allData == null) return new List<string>();

            var checkedIds = allData.Where(x => x.IsSelected).Select(x => x.MaSinhVien).ToList();
            if (checkedIds.Count > 0) return checkedIds;

            if (dgvStudents.CurrentRow != null)
            {
                var currentItem = dgvStudents.CurrentRow.DataBoundItem as StudentSyncViewModel;
                if (currentItem != null)
                {
                    return new List<string> { currentItem.MaSinhVien }; // SỬA: Đã sửa từ HoTen thành MaSinhVien
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
            Label textLabel = new Label() { Left = 20, Top = 20, Text = text, Width = 350 };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 340, UseSystemPasswordChar = true }; // Ẩn pass
            Button confirmation = new Button() { Text = "Đồng ý", Left = 240, Width = 100, Top = 90, DialogResult = DialogResult.OK };

            prompt.Controls.Add(textLabel); prompt.Controls.Add(textBox); prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        private void mnu_UpdateInfor_Click(object sender, EventArgs e)
        {
            var listUsernames = GetTargetIdNumbers();
            if (listUsernames.Count == 0) return;
            if (listUsernames.Count > 1)
            {
                MessageBox.Show("Vui lòng chỉ chọn 1 tài khoản.");
                return;
            }

            string username = listUsernames[0];
            var currentUser = ManageLMS.BLL.Cache.MoodleCache.GetUser(username);

            if (currentUser == null)
            {
                MessageBox.Show("Không tìm thấy thông tin user này.");
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

            if (MessageBox.Show(msg, "Xác nhận Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No) return;

            this.Cursor = Cursors.WaitCursor;
            this.Enabled = false;

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
                this.Cursor = Cursors.Default;
                this.Enabled = true;

                if (t.Exception != null)
                {
                    MessageBox.Show("Lỗi: " + t.Exception.InnerException.Message);
                }
                else
                {
                    MessageBox.Show(string.Format("Đã xoá thành công {0}/{1} tài khoản.", t.Result, listUsernames.Count));
                    LoadDataToGrid();
                }
            },
            TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void mnuChangePass_Click(object sender, EventArgs e)
        {
            var listUsernames = GetTargetIdNumbers();
            if (listUsernames.Count == 0) return;

            string msgUser = listUsernames.Count == 1 ? string.Format("cho {0}", listUsernames[0]) : string.Format("cho {0} tài khoản", listUsernames.Count);
            string newPass = ShowInputDialog(string.Format("Nhập mật khẩu mới {0}:", msgUser), "Đổi mật khẩu");

            if (string.IsNullOrEmpty(newPass)) return;
            if (newPass.Length < 8)
            {
                MessageBox.Show("Mật khẩu phải dài ít nhất 8 ký tự!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            this.Enabled = false; // Khóa Form tạm thời

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
                this.Cursor = Cursors.Default;
                this.Enabled = true; // Mở lại Form

                if (t.Exception != null)
                {
                    MessageBox.Show("Lỗi: " + t.Exception.InnerException.Message);
                }
                else
                {
                    MessageBox.Show(string.Format("Đã cập nhật mật khẩu thành công cho {0}/{1} người.", t.Result, listUsernames.Count));
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void mnuLockAccount_Click(object sender, EventArgs e)
        {
            var listUsernames = GetTargetIdNumbers();
            if (listUsernames.Count == 0) return;

            if (MessageBox.Show(string.Format("Bạn có chắc muốn KHÓA {0} tài khoản?", listUsernames.Count),
                "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;

            this.Cursor = Cursors.WaitCursor;
            this.Enabled = false;

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
                this.Cursor = Cursors.Default;
                this.Enabled = true;

                if (t.Exception != null)
                {
                    MessageBox.Show("Lỗi: " + t.Exception.InnerException.Message);
                }
                else
                {
                    MessageBox.Show(string.Format("Đã khóa thành công {0}/{1} tài khoản.", t.Result, listUsernames.Count));
                    LoadDataToGrid();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void mnuUnlockAccount_Click(object sender, EventArgs e)
        {
            var listUsernames = GetTargetIdNumbers();
            if (listUsernames.Count == 0) return;

            this.Cursor = Cursors.WaitCursor;
            this.Enabled = false;

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
                this.Cursor = Cursors.Default;
                this.Enabled = true;
                if (t.Exception != null)
                {
                    MessageBox.Show("Lỗi: " + t.Exception.InnerException.Message);
                }
                else
                {
                    MessageBox.Show(string.Format("Đã mở khóa thành công {0}/{1} tài khoản.", t.Result, listUsernames.Count));
                    LoadDataToGrid();
                }
            },
            TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void dgvStudents_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    if (!dgvStudents.Rows[e.RowIndex].Selected)
                    {
                        dgvStudents.ClearSelection();
                        dgvStudents.CurrentCell = dgvStudents.Rows[e.RowIndex].Cells[e.ColumnIndex];
                        dgvStudents.Rows[e.RowIndex].Selected = true;
                    }

                    var relativeMousePosition = dgvStudents.PointToClient(Cursor.Position);
                    ctxSVAction.Show(dgvStudents, relativeMousePosition);
                }
            }
        }

        private void mnu_UserCourse_Click(object sender, EventArgs e)
        {
            var listUsernames = GetTargetIdNumbers();

            if (listUsernames.Count == 0) return;

            // Chỉ cho phép xem từng người một
            if (listUsernames.Count > 1)
            {
                MessageBox.Show("Vui lòng chỉ chọn 1 tài khoản để xem khóa học.");
                return;
            }

            string username = listUsernames[0];
            var moodleUser = ManageLMS.BLL.Cache.MoodleCache.GetUser(username);

            if (moodleUser == null)
            {
                MessageBox.Show("Không tìm thấy thông tin user này trong bộ nhớ đệm.\nVui lòng đảm bảo user đã tồn tại trên Moodle (Trạng thái: Đã có).");
                return;
            }

            UserCourseView frm = new UserCourseView(moodleUser);
            frm.Show();
        }

        private void PerformUpdateUser(MoodleUser user)
        {
            this.Cursor = Cursors.WaitCursor;
            this.Enabled = false;

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    // 1. Gửi API Update lên Server
                    _userManager.UpdateUsersBatch(new List<MoodleUser> { user });

                    // 2. Cập nhật Cache cục bộ (Ghi đè bản cũ bằng bản mới)
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
                this.Cursor = Cursors.Default;
                this.Enabled = true;

                if (t.Exception != null)
                {
                    MessageBox.Show("Lỗi cập nhật: " + t.Exception.InnerException.Message);
                }
                else
                {
                    MessageBox.Show("Cập nhật thông tin thành công!");
                    UpdateGridRowClientSide(user);
                }
            }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void UpdateGridRowClientSide(MoodleUser moodleUser)
        {
            var currentList = dgvStudents.DataSource as List<StudentSyncViewModel>;
            if (currentList == null) return;

            var rowItem = currentList.FirstOrDefault(x => x.MaSinhVien.Trim().ToLower() == moodleUser.idnumber.Trim().ToLower());

            if (rowItem != null)
            {
                rowItem.IsSuspended = (moodleUser.suspended == true);

                string moodleEmail = moodleUser.email != null ? moodleUser.email.Trim().ToLower() : "";

                if (rowItem.IsSuspended)
                {
                    rowItem.TrangThaiMoodle = "Đã bị khóa";
                }
                else
                {
                    rowItem.TrangThaiMoodle = "Đang hoạt động";
                }

                dgvStudents.Refresh();
            }
        }
    }
}