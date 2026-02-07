using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ManageLMS.UI.Sync
{
    public partial class SyncControl : UserControl
    {
        public SyncControl()
        {
            InitializeComponent();
            // Thêm SemesterSyncUC vào Tab Semester
            var semesterSyncView = new ManageLMS.UI.Sync.SemesterSyncUC();
            semesterSyncView.Dock = DockStyle.Fill;
            this.tabSemester.Controls.Add(semesterSyncView);

            // Thêm MasterInitUC vào Sub-tab Master Init
            var masterInitView = new ManageLMS.UI.Sync.MasterInitUC();
            masterInitView.Dock = DockStyle.Fill;
            this.tabMasterInit.Controls.Add(masterInitView);

            // Thêm MasterStudentSyncUC vào Sub-tab Master Student
            var masterStudentView = new ManageLMS.UI.Sync.MasterStudentSyncUC();
            masterStudentView.Dock = DockStyle.Fill;
            this.tabMasterStudent.Controls.Add(masterStudentView);
        }
    }
}
