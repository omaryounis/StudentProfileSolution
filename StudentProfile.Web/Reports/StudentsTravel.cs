using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.IO;

namespace StudentProfile.Reports
{
    public partial class StudentsTravel : DevExpress.XtraReports.UI.XtraReport
    {
        public StudentsTravel()
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

        private void StudentsTravel_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }
    }
}
