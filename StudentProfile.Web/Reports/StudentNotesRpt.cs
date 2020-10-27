using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using DevExpress.XtraReports.UI;

namespace StudentProfile.Web.Reports
{
    public partial class StudentNotesRpt : DevExpress.XtraReports.UI.XtraReport
    {
        public StudentNotesRpt()
        {
            InitializeComponent();
        }

        private void xrPictureBox1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var pictureBox = (XRPictureBox)sender;

            const string path = "~\\Content\\logo.png";

            var relativePath = System.Web.Hosting.HostingEnvironment.MapPath(path);
            if (File.Exists(relativePath))
            {
                pictureBox.ImageUrl = path;
            }
        }
    }
}
