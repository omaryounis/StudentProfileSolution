using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.IO;
using static StudentProfile.Web.Controllers.AcademicCommitteeController;

namespace StudentProfile.Web.Reports
{
    public partial class AcademicCommitteeStudentsRPT : DevExpress.XtraReports.UI.XtraReport
    {
        public AcademicCommitteeStudentsRPT()
        {
            InitializeComponent();
        }

        private void AcademicCommitteeStudentsRPT_AfterPrint(object sender, EventArgs e)
        {
            ((XtraReport)sender).PrintingSystem.Document.AutoFitToPagesWidth = 1;
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

        private void xrTableCell12_EvaluateBinding(object sender, BindingEventArgs e)
        {
            var label = (XRLabel)sender;
            var currentRow = (AcademicCommitteeStudentsReportVM)GetCurrentRow();
            if (currentRow != null)
            {
                if (currentRow.DecisionFile != null)
                {
                    if (currentRow.DecisionFile.Contains(".pdf") || currentRow.DecisionFile.Contains(".doc") || currentRow.DecisionFile.Contains(".docx") ||
                        currentRow.DecisionFile.Contains(".xls") || currentRow.DecisionFile.Contains(".xlsx"))
                    {
                        label.NavigateUrl = "~/Content/AcademicDecisionsFiles/" + currentRow.DecisionFile;
                    }
                    else
                    {
                        label.Target = "_blank";
                        label.NavigateUrl = "~/Content/AcademicDecisionsFiles/" + currentRow.DecisionFile;
                    }
                }
            }
        }

        private void xrPageBreak1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRPageBreak control = sender as XRPageBreak;
            int value = CurrentRowIndex + 1;
            if (value % 14 == 0)
                control.Visible = true;
            else
                control.Visible = false;
        }
    }
}
