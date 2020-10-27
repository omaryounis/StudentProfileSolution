using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.IO;

namespace StudentProfile.Web.Reports
{
    public partial class StudentsWithoutIdNumbers : DevExpress.XtraReports.UI.XtraReport
    {
        public StudentsWithoutIdNumbers()
        {
            InitializeComponent();
        }


        private void xrPictureBox1_BeforePrint_1(object sender, System.Drawing.Printing.PrintEventArgs e)
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
