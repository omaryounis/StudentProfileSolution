using System;
using NUnit.Framework;
using StudentProfile.Controllers;

namespace StudentProfile.Web.Test
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void MusaferController()
        {
            MusaferController  mus = new MusaferController();
            var result = mus.GetActiveAdvertisement();
            Assert.That(result, Is.EqualTo(100));
        }

        [Test]
        public void GetStudents()
        {
            MusaferController mus = new MusaferController();
            var result = mus.GetStudents();
            Assert.That(result, Is.EqualTo(100));
        }

        [Test]
        public void GetStudentByadvertisementIdForAdmin()
        {
            MusaferController mus = new MusaferController();
            var result = mus.GetStudentByadvertisementIdForAdmin(27);
            Assert.That(result, Is.EqualTo(100));
        }
    }
}