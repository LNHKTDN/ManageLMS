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
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabSemester = new System.Windows.Forms.TabPage();
            this.tabMaster = new System.Windows.Forms.TabPage();
            this.tabMasterSub = new System.Windows.Forms.TabControl();
            this.tabMasterInit = new System.Windows.Forms.TabPage();
            this.tabMasterStudent = new System.Windows.Forms.TabPage();
            this.tabMain.SuspendLayout();
            this.tabMaster.SuspendLayout();
            this.tabMasterSub.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabSemester);
            this.tabMain.Controls.Add(this.tabMaster);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 0);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(900, 600);
            this.tabMain.TabIndex = 0;
            // 
            // tabSemester
            // 
            this.tabSemester.Location = new System.Drawing.Point(4, 25);
            this.tabSemester.Name = "tabSemester";
            this.tabSemester.Size = new System.Drawing.Size(892, 571);
            this.tabSemester.TabIndex = 0;
            this.tabSemester.Text = "Đồng bộ Lớp Tín Chỉ (Semester)";
            // 
            // tabMaster
            // 
            this.tabMaster.Controls.Add(this.tabMasterSub);
            this.tabMaster.Location = new System.Drawing.Point(4, 25);
            this.tabMaster.Name = "tabMaster";
            this.tabMaster.Size = new System.Drawing.Size(892, 571);
            this.tabMaster.TabIndex = 1;
            this.tabMaster.Text = "Quản lý Lớp Học Phần (Master Course)";
            // 
            // tabMasterSub
            // 
            this.tabMasterSub.Controls.Add(this.tabMasterInit);
            this.tabMasterSub.Controls.Add(this.tabMasterStudent);
            this.tabMasterSub.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMasterSub.Location = new System.Drawing.Point(0, 0);
            this.tabMasterSub.Name = "tabMasterSub";
            this.tabMasterSub.SelectedIndex = 0;
            this.tabMasterSub.Size = new System.Drawing.Size(892, 571);
            this.tabMasterSub.TabIndex = 0;
            // 
            // tabMasterInit
            // 
            this.tabMasterInit.Location = new System.Drawing.Point(4, 25);
            this.tabMasterInit.Name = "tabMasterInit";
            this.tabMasterInit.Size = new System.Drawing.Size(884, 542);
            this.tabMasterInit.TabIndex = 0;
            this.tabMasterInit.Text = "1. Khởi tạo & Gán GV";
            // 
            // tabMasterStudent
            // 
            this.tabMasterStudent.Location = new System.Drawing.Point(4, 25);
            this.tabMasterStudent.Name = "tabMasterStudent";
            this.tabMasterStudent.Size = new System.Drawing.Size(884, 542);
            this.tabMasterStudent.TabIndex = 1;
            this.tabMasterStudent.Text = "2. Đồng bộ Sinh viên (Lớp chung)";
            // 
            // SyncControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabMain);
            this.Name = "SyncControl";
            this.Size = new System.Drawing.Size(900, 600);
            this.tabMain.ResumeLayout(false);
            this.tabMaster.ResumeLayout(false);
            this.tabMasterSub.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        // Main Tabs Only
        private TabControl tabMain;
        private TabPage tabSemester;
        private TabPage tabMaster;
        private TabControl tabMasterSub;
        private TabPage tabMasterInit;
        private TabPage tabMasterStudent;
    }
}