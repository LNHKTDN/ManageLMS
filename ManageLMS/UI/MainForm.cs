using System;
using System.Collections.Generic;
using System.ComponentModel; // Để dùng BackgroundWorker
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using ManageLMS.BLL.SyncEngine;
using ManageLMS.Common.DTO.ViewModel;
using ManageLMS.BLL.Manager.UserManager;
using System.Threading.Tasks;
using ManageLMS.UI.UserManage;
using ManageLMS.DTO.Model;
using ManageLMS.UI.Course.UserCourse;
using ManageLMS.UI.Sync;

namespace ManageLMS
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            
            InitializeComponent();
            // Thêm TeacherUC vào Tab Teacher
            var teacherView = new TeacherUC();
            teacherView.Dock = DockStyle.Fill; // Để nó tràn đầy màn hình tab
            this.tabTeacher.Controls.Add(teacherView);

            // Thêm StudentUC vào Tab Student
            var studentView = new StudentUC();
            studentView.Dock = DockStyle.Fill;
            this.tabStudent.Controls.Add(studentView);

            // Thêm SyncUI vào Tab Sync
            var SyncView = new SyncControl();
            SyncView.Dock = DockStyle.Fill;
            this.tabSync.Controls.Add(SyncView);
        }

    }
}