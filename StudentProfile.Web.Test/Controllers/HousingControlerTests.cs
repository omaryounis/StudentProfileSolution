using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using StudentProfile.Web.Controllers;

namespace StudentProfile.Web.Test.Controllers
{
    [TestFixture]
    class HousingControlerTests
    {
        [Test]
        public void MusaferController()
        {
            HousingController mus = new HousingController();
            var result = mus.GetIssueNumberString(1);
            Assert.That(result, Is.EqualTo(100));
        }
    }
}