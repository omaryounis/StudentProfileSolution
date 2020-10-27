using Newtonsoft.Json;
using StudentProfile.Components;
using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace StudentProfile.Web.Controllers
{
    public partial class HousingController : Controller
    {

        [HttpGet]
        public ActionResult HousingOfStudents()
        {
            var json = new JavaScriptSerializer().Serialize(GetPermissions(70).Data);
            var permissions = JsonConvert.DeserializeObject<CustomPermissions>(json);

            if (permissions.Home)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }

        [HttpGet]
        public async Task<ActionResult> GetNextHousingNo(int studentId, int operationTypeId)
        {
            var db = new SchoolAccGam3aEntities();
            return Json(await Task.Run(() => db.NextHousingNo_Sp(studentId, operationTypeId)), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetStudentDataByID(int studentId)
        {
            var db = new SchoolAccGam3aEntities();
            return Json(await Task.Run(() => db.INTEGRATION_All_Students
               .Where(x => x.STUDENT_ID == studentId).ToList().Select(x => new
               {
                   NationalityName = x.NATIONALITY_DESC,
                   MobileNumber = x.MOBILE_PHONE,
                   FacultiyName = x.FACULTY_NAME,
                   BirthCityName = x.BIRTH_CITY_TEXT,
                   BirthDate = x.BIRTH_DATE,
                   DegreeName = x.DEGREE_DESC,
                   LevelName = x.LEVEL_DESC,
                   StudyTypeName = x.STUDY_DESC,
                   StatusName = x.STATUS_DESC,
                   GenderName = x.GENDER == 1 ? "ذكر" : x.GENDER == 2 ? "أنثي" : "غير متوفر"
               }).FirstOrDefault()), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllStudents()
        {
            var db = new SchoolAccGam3aEntities();
            return new JsonResult()
            {
                Data = await Task.Run(() => db.SP_GetAllStudent_Names_Customize().ToList()),
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpGet]
        public async Task<ActionResult> GetAllHousingStudents(int id)
        {
            var db = new SchoolAccGam3aEntities();
            return new JsonResult()
            {
                Data = await Task.Run(() => db.SP_GetAllHousingStudents_Names().Where(x => x.STUDENT_ID != id).ToList()),
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };

        }

        [HttpGet]
        public async Task<string> ToHijriDate(DateTime gregorianDate)
        {
            return await Task.Run(() => gregorianDate.ToString("dd dddd , MMMM, yyyy", new CultureInfo("ar-SA")));
        }

        [HttpGet]
        public async Task<ActionResult> GetALLComsBasedOnIsFamilialFlag(bool IsFamilial)
        {
            var db = new EsolERPEntities();
            var xx = db.VW_UnitParents.ToList();
            return Json(await Task.Run(() => db.VW_UnitParents.Select(x => new
            {
                ComId = x.CompanyId,
                ComName = x.CompanyName
            }).Distinct().ToList()), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetALLUnitsBasedOnCom(bool IsFamilial, int ComId)
        {
            var db = new EsolERPEntities();

            return Json(await Task.Run(() => db.VW_UnitParents.Where(x => x.CompanyId == ComId /*&& x.IsFamilial == IsFamilial*/)
          .Select(x => new
          {
              UnitId = x.DepartmentId,
              UnitName = x.DepartementName
          }).Distinct().ToList()), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetALLFloorsBasedOnUnit(bool IsFamilial, int ComId, int UnitId)
        {
            var db = new EsolERPEntities();
            return Json(await Task.Run(() => db.VW_UnitParents.Where(x => x.DepartmentId == UnitId && x.CompanyId == ComId /* && x.IsFamilial == IsFamilial*/)
          .Select(x => new
          {
              FloorId = x.FloorId,
              FloorName = x.FloorName
          }).Distinct().ToList()), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetALLRoomsBasedOnFloor(bool IsFamilial, int ComId, int UnitId, int FloorId)
        {
            var db = new SchoolAccGam3aEntities();
            var dbACC = new EsolERPEntities();

            var locationsIsUsed = db.HousingLocationFurniture.Where(x => x.IsUsed == true).Select(x => x.LocationId).ToList();
            return Json(await Task.Run(() => dbACC.VW_UnitParents
                                            .Where(x => x.FloorId == FloorId &&
                                                        x.DepartmentId == UnitId &&
                                                        x.CompanyId == ComId &&
                                                        //x.IsFamilial == IsFamilial &&
                                                        (locationsIsUsed.Any(c => c != x.SiteId) || locationsIsUsed.Count == 0)
                                                  )
                                            .Select(x => new
                                            {
                                                RoomId = x.SiteId,
                                                RoomName = x.SiteName
                                            }).Distinct().ToList()), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetALLBedsBasedOnRoomId(int RoomId)
        {
            var db = new SchoolAccGam3aEntities();
            return Json(await Task.Run(() => db.GetALLBedLocationsBasedOnRoomId_SP(RoomId).ToList()), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetALLCategories()
        {
            var db = new SchoolAccGam3aEntities();
            return Json(await Task.Run(() => db.HousingFurnitureCategories.Select(x => new { CategoryId = x.Id, CategoryName = x.Name }).ToList()), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetALLFurnituresBasedOnCategoryId(int CategoryId)
        {
            var db = new SchoolAccGam3aEntities();
            return Json(await Task.Run(() => db.HousingFurnitures_SP(CategoryId).ToList()), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public  ActionResult GetHousingStudent(int StudentId)
        {
            var AccContext = new EsolERPEntities();
            var ScContext = new SchoolAccGam3aEntities();

            var StudentHousingInDB = ScContext.HousingOfSudents.FirstOrDefault(x => x.StudentId == StudentId && x.LeaveDate == null);
            if (StudentHousingInDB == null)
                return Json(@"عفوا هذا الطالب لم يتم تسكينه حتي الآن برجاء تسكين الطالب أولا", JsonRequestBehavior.AllowGet);

            var LocationDetailsInDB = AccContext.VW_UnitParents.FirstOrDefault(p => p.SiteId == StudentHousingInDB.HousingLocationFurniture.LocationId);

            if(LocationDetailsInDB == null)
                return Json(@"تم تعديل بيانات السكن لهذا اللطالب .. برجاء مراجعة التسكين", JsonRequestBehavior.AllowGet);


            var data = new
            {
                LocationDetails = LocationDetailsInDB,
                BedLocationId = StudentHousingInDB.HousingLocationFurniture.Id,
                BedLocationCode = StudentHousingInDB.HousingLocationFurniture.Barcode,
                NotesOfHosing = StudentHousingInDB.NotesOfHosing,
                HousingNumber = StudentHousingInDB.Id,
                HousingDate = StudentHousingInDB.HousingDate.ToString("dd/MM/yyyy"),
                LastUpdated = StudentHousingInDB.LastEdittededBy == null ? ScContext.DashBoard_Users.FirstOrDefault(c => c.ID == StudentHousingInDB.userId).Name :
                ScContext.DashBoard_Users.FirstOrDefault(c => c.ID == StudentHousingInDB.LastEdittededBy).Name,
                SiteId = StudentHousingInDB.LocationId
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetStudentHousingFurniture(int StudentId)
        {
            var SchoolDB = new SchoolAccGam3aEntities();
            var AccountDB = new EsolERPEntities();

            var studentFurnituresInDB = await Task.Run(() => SchoolDB.GetStudentFurnitures_SP(StudentId).ToList());
            var StudentHousingInDB = await Task.Run(() => SchoolDB.HousingOfSudents.Where(x => x.StudentId == StudentId && x.LeaveDate == null).FirstOrDefault());

            if (StudentHousingInDB == null)
            {
                return Json(@"", JsonRequestBehavior.AllowGet);
            }
            var locationDetailsInDB = await Task.Run(() => AccountDB.VW_UnitParents.Where(p => p.SiteId == StudentHousingInDB.HousingLocationFurniture.LocationId).FirstOrDefault());

            return Json(new
            {
                StudentFurnitures = studentFurnituresInDB,
                LocationDetails = locationDetailsInDB,
                BedLocationId = StudentHousingInDB.HousingLocationFurniture.Id,
                BedLocationCode = StudentHousingInDB.HousingLocationFurniture.Barcode,
            }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult SaveStudentHousing(int StudentId, DateTime HousingDate, string HousingNotes, int LocationId, int? BedLocationId)
        {
            var db = new SchoolAccGam3aEntities();

            if (StudentId <= 0)
            {
                return Content(@"برجاء إختيار الطالب");
            }

            if (BedLocationId == null)
            {
                var housingLocationFurniture = db.HousingLocationFurniture.FirstOrDefault(x => x.LocationId == LocationId);
                if (housingLocationFurniture != null)
                {
                    if (housingLocationFurniture.IsUsed == false)
                    {
                        BedLocationId = housingLocationFurniture.Id;

                        housingLocationFurniture.IsUsed = true;
                        db.Entry(housingLocationFurniture).State = EntityState.Modified;
                    }
                    else
                    {
                        return Content(@"هذه الغرفة غير متاحة برجاء اختيار غرفة أخري");
                    }
                }
                else
                {
                    var newHousingLocation = new HousingLocationFurniture
                    {
                        IsUsed = true,
                        Barcode = null,
                        userId = UserID,
                        FurnitureId = null,
                        LocationId = LocationId,
                        InsertionDate = DateTime.Now
                    };
                    db.HousingLocationFurniture.Add(newHousingLocation);
                    db.SaveChanges();

                    BedLocationId = newHousingLocation.Id;
                }
            }
            var housingOfSudents = db.HousingOfSudents.Where(x => x.StudentId == StudentId && x.LeaveDate == null).FirstOrDefault();
            if (housingOfSudents != null)
            {
                housingOfSudents.userId = UserID;
                housingOfSudents.NotesOfHosing = HousingNotes;
                housingOfSudents.HousingDate = HousingDate;
                housingOfSudents.StudentId = StudentId;
                housingOfSudents.LocationId = (int)BedLocationId;
                housingOfSudents.LeaveDate = null;
                housingOfSudents.SupervisorIdWhoMakeLeave = null;
                housingOfSudents.NotesOfLeave = null;
                housingOfSudents.LastEdittededBy = UserID;
                housingOfSudents.LastEdittedDate = DateTime.Now;

                db.Entry(housingOfSudents).State = EntityState.Modified;
                db.SaveChanges();

                return Content(@"");
            }
            var OperationTypeId = db.OperationTypes.FirstOrDefault(x => x.KeyEn == "StudentHousing").Id;
            housingOfSudents = new HousingOfSudents
            {
                userId = UserID,
                NotesOfHosing = HousingNotes,
                HousingDate = HousingDate,
                InsertionDate = DateTime.Now,
                StudentId = StudentId,
                LocationId = (int)BedLocationId,
                SupervisorIdWhoMakeHousing = UserID,
                LeaveDate = null,
                SupervisorIdWhoMakeLeave = null,
                NotesOfLeave = null,
                LastEdittededBy = null,
                LastEdittedDate = null,
                Opt_Id = OperationTypeId,
                Opt_No = db.HousingOfSudents.Any(x => x.Opt_Id == OperationTypeId) ? db.HousingOfSudents.Where(x => x.Opt_Id == OperationTypeId).Max(x => x.Opt_No) + 1 : 1
            };
            db.HousingOfSudents.Add(housingOfSudents);
            db.SaveChanges();

            return Content(@"");
        }

        [HttpPost]
        public ActionResult SaveStudentHousingTransfer(int StudentId, DateTime HousingDate_Trans, string HousingNotes_Trans, int LocationId_Trans, int? BedLocationId_Trans)
        {
            try
            {

                if (StudentId <= 0)
                {
                    return Content(@"برجاء إختيار الطالب");
                }

                var db = new SchoolAccGam3aEntities();
                var housingOfSudents = db.HousingOfSudents.Where(x => x.StudentId == StudentId && x.LeaveDate == null).FirstOrDefault();

                if (housingOfSudents != null)
                {

                    if (db.HousingFurnituresOfStudent.Where(x => x.HousingOfSudentId == housingOfSudents.Id && x.ReturnDate == null).Count() > 0)
                    {
                        return Content(@"لا يمكن نقل الطالب لسكن اخر إلا بعد استلام كامل العهد المسندة عليه فى هذا السكن ");
                    }
                    using (TransactionScope scope = new TransactionScope())
                    {

                        if (BedLocationId_Trans == null)
                        {
                            var housingLocationFurniture = db.HousingLocationFurniture.FirstOrDefault(x => x.LocationId == LocationId_Trans);
                            if (housingLocationFurniture != null)
                            {
                                if (housingLocationFurniture.IsUsed == false)
                                {
                                    BedLocationId_Trans = housingLocationFurniture.Id;

                                    housingLocationFurniture.IsUsed = true;
                                    db.Entry(housingLocationFurniture).State = EntityState.Modified;
                                }
                                else
                                {
                                    return Content(@"هذه الغرفة غير متاحة برجاء اختيار غرفة أخري");
                                }
                            }
                            else
                            {
                                var newHousingLocation = new HousingLocationFurniture
                                {
                                    IsUsed = true,
                                    Barcode = null,
                                    userId = UserID,
                                    FurnitureId = null,
                                    LocationId = LocationId_Trans,
                                    InsertionDate = DateTime.Now
                                };
                                db.HousingLocationFurniture.Add(newHousingLocation);
                                db.SaveChanges();

                                BedLocationId_Trans = newHousingLocation.Id;
                            }
                        }

                        housingOfSudents.LeaveDate = HousingDate_Trans;
                        housingOfSudents.SupervisorIdWhoMakeLeave = UserID;
                        housingOfSudents.NotesOfLeave = HousingNotes_Trans;
                        housingOfSudents.LastEdittededBy = UserID;
                        housingOfSudents.LastEdittedDate = DateTime.Now;

                        db.Entry(housingOfSudents).State = EntityState.Modified;


                        var Location = db.HousingLocationFurniture.FirstOrDefault(x => x.Id == housingOfSudents.LocationId && x.IsUsed == true);
                        if (Location != null)
                        {
                            Location.IsUsed = false;
                            db.Entry(Location).State = EntityState.Modified;
                        }
                        db.SaveChanges();

                        var OperationTypeId = db.OperationTypes.FirstOrDefault(x => x.KeyEn == "StudentTransfering").Id;
                        housingOfSudents = new HousingOfSudents
                        {
                            userId = UserID,
                            NotesOfHosing = HousingNotes_Trans,
                            HousingDate = HousingDate_Trans,
                            InsertionDate = DateTime.Now,
                            StudentId = StudentId,
                            LocationId = (int)BedLocationId_Trans,
                            SupervisorIdWhoMakeHousing = UserID,
                            LeaveDate = null,
                            SupervisorIdWhoMakeLeave = null,
                            NotesOfLeave = null,
                            LastEdittededBy = null,
                            LastEdittedDate = null,
                            Opt_Id = OperationTypeId,
                            Opt_No = db.HousingOfSudents.Any(x => x.Opt_Id == OperationTypeId) ? db.HousingOfSudents.Where(x => x.Opt_Id == OperationTypeId).Max(x => x.Opt_No) + 1 : 1
                        };
                        db.HousingOfSudents.Add(housingOfSudents);
                        db.SaveChanges();
                        scope.Complete();
                        return Content(@"");
                    }
                }

                return Content(@"هذا الطالب غير مسكن من قبل");
            }
            catch (Exception ex)
            {
                return Json($"حدث خطأ :  { ex.Message }", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveHousingFurniture(int StudentId, int BedLocationId, List<HousingFurnitureVM> HousingFurnitureModel)
        {
            if (HousingFurnitureModel.Count <= 0)
            {
                return Content(@"برجاء إسناد قطعة أثاث واحدة على الأقل");
            }
            if (StudentId <= 0)
            {
                return Content(@"برجاء إختيار الطالب");
            }

            var db = new SchoolAccGam3aEntities();
            var housingOfSudents = db.HousingOfSudents.Where(x => x.StudentId == StudentId && x.LeaveDate == null).FirstOrDefault();
            if (housingOfSudents != null)
            {
                foreach (var Part in HousingFurnitureModel)
                {
                    if (Part.HousingFurnituresOfStudent_Id != 0) { continue; }

                    var housingFurnituresOfStudent = db.HousingFurnituresOfStudent.Where(x => x.HousingOfSudentId == housingOfSudents.Id && x.HousingFurnitureId == Part.FurnitureId && x.ReturnDate == null).FirstOrDefault();
                    if (housingFurnituresOfStudent != null) { continue; }

                    housingFurnituresOfStudent = new HousingFurnituresOfStudent
                    {
                        HousingOfSudentId = housingOfSudents.Id,
                        HousingFurnitureId = Part.FurnitureId,
                        Number = housingOfSudents.Id,/**/
                        Notes = housingOfSudents.NotesOfHosing,
                        ReceivingDate = housingOfSudents.HousingDate,
                        ReceivingNotes = housingOfSudents.NotesOfHosing,
                        SupervisorIdWhoGive = UserID,
                        ReturnDate = null,
                        ReturnNotes = null,
                        SupervisorIdWhoTake = null,
                        InsertedBy = UserID,
                        InsertionDate = DateTime.Now,
                        LastEdittedBy = null,
                        LastEditDate = null
                    };
                    db.HousingFurnituresOfStudent.Add(housingFurnituresOfStudent);
                }
                db.SaveChanges();

                return Json(new { StudentFurnitures = await Task.Run(() => db.GetStudentFurnitures_SP(StudentId).ToList()) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Content(@"برجاء تسكين الطالب أولا قبل تسليمه أي عهد");
            }


        }

        [HttpPost]
        public async Task<ActionResult> SaveHousingFurnitureReturn(int StudentId, int HousingFurnituresOfStudent_Id, DateTime HousingReturnDate, string HousingReturnNotes)
        {
            if (StudentId <= 0)
            {
                return Content(@"برجاء إختيار الطالب");
            }

            var db = new SchoolAccGam3aEntities();
            var HousingFurnituresOfStudent = await Task.Run(() => db.HousingFurnituresOfStudent.Where(x => x.Id == HousingFurnituresOfStudent_Id).FirstOrDefault());
            if (HousingFurnituresOfStudent != null)
            {
                if (HousingFurnituresOfStudent.ReceivingDate > HousingReturnDate)
                {
                    return Content(@"لا يمكن أن يكون تاريخ إستلام العهدة من الطالب قبل تاريخ تسليمها للطالب");
                }
                HousingFurnituresOfStudent.ReturnDate = HousingReturnDate;
                HousingFurnituresOfStudent.ReturnNotes = HousingReturnNotes;
                HousingFurnituresOfStudent.SupervisorIdWhoTake = UserID;
                HousingFurnituresOfStudent.LastEditDate = DateTime.Now;
                HousingFurnituresOfStudent.LastEdittedBy = UserID;

                db.Entry(HousingFurnituresOfStudent).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { StudentFurnitures = await Task.Run(() => db.GetStudentFurnitures_SP(StudentId).ToList()) }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Content(@"هذا الأصل غير موجود");
            }

        }

        [HttpPost]
        public async Task<ActionResult> SaveHousingReplacement(DateTime HousingReplacementDate,
                int First_StudentId, int Second_StudentId, string First_HousingReplacementNotes,
                string Second_HousingReplacementNotes)
        {
            if (First_StudentId <= 0)
            {
                return Content(@"برجاء إختيار الطالب الأول");
            }
            if (Second_StudentId <= 0)
            {
                return Content(@"برجاء إختيار الطالب الثاني");
            }
            if (First_StudentId == Second_StudentId)
            {
                return Content(@"لا يمكن إجراء عملية التبديل بين طالب واحد");
            }

            var db = new SchoolAccGam3aEntities();

            var housingOfFirstSudent = await Task.Run(() => db.HousingOfSudents.Where(x => x.StudentId == First_StudentId && x.LeaveDate == null).FirstOrDefault());
            var housingOfSecondSudent = await Task.Run(() => db.HousingOfSudents.Where(x => x.StudentId == Second_StudentId && x.LeaveDate == null).FirstOrDefault());

            if (housingOfFirstSudent != null && housingOfSecondSudent != null)
            {
                if (housingOfFirstSudent.HousingDate > HousingReplacementDate && housingOfSecondSudent.HousingDate > HousingReplacementDate)
                {
                    return Content(@"لايمكن أن يكون تاريخ عملية تبديل السكن قبل تاريخ تسكين الطلاب");
                }

                var HousingReplacementStudentsList = new List<HousingReplacementStudentsVM>() {
                        new HousingReplacementStudentsVM{ StudentId =First_StudentId , ReplacementDate=HousingReplacementDate ,ReplacementNotes = First_HousingReplacementNotes , housingOfSudent = housingOfFirstSudent},
                        new HousingReplacementStudentsVM{ StudentId =Second_StudentId, ReplacementDate=HousingReplacementDate ,ReplacementNotes = Second_HousingReplacementNotes , housingOfSudent = housingOfSecondSudent}
                };

                var OperationTypeId = db.OperationTypes.FirstOrDefault(x => x.KeyEn == "HouseReplacement").Id;

                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        foreach (var HousingReplacementStudent in HousingReplacementStudentsList)
                        {

                            //Update HousingOfSudent
                            HousingReplacementStudent.housingOfSudent.LeaveDate = HousingReplacementDate;
                            HousingReplacementStudent.housingOfSudent.SupervisorIdWhoMakeLeave = UserID;
                            HousingReplacementStudent.housingOfSudent.NotesOfLeave = HousingReplacementStudent.ReplacementNotes == null ? "لا يوجد ملاحظات خاصة بعملية التبديل" : HousingReplacementStudent.ReplacementNotes;
                            HousingReplacementStudent.housingOfSudent.LastEdittededBy = UserID;
                            HousingReplacementStudent.housingOfSudent.LastEdittedDate = DateTime.Now;
                            HousingReplacementStudent.housingOfSudent.HousingRepacement_Id = HousingReplacementStudentsList.Where(x => x.housingOfSudent.StudentId != HousingReplacementStudent.StudentId).Select(p => p.housingOfSudent.Id).FirstOrDefault();

                            db.Entry(HousingReplacementStudent.housingOfSudent).State = EntityState.Modified;

                            //Add New HousingOfSudent
                            var housingOfSudents = new HousingOfSudents
                            {
                                userId = UserID,
                                NotesOfHosing = HousingReplacementStudent.ReplacementNotes == null ? "لا يوجد" : HousingReplacementStudent.ReplacementNotes,
                                HousingDate = HousingReplacementDate,
                                InsertionDate = DateTime.Now,
                                StudentId = HousingReplacementStudent.StudentId,
                                LocationId = HousingReplacementStudentsList.Where(x => x.housingOfSudent.StudentId != HousingReplacementStudent.StudentId).Select(p => p.housingOfSudent.LocationId).FirstOrDefault(),
                                SupervisorIdWhoMakeHousing = UserID,
                                LeaveDate = null,
                                SupervisorIdWhoMakeLeave = null,
                                NotesOfLeave = null,
                                LastEdittededBy = null,
                                LastEdittedDate = null,
                                Opt_Id = OperationTypeId,
                                Opt_No = db.HousingOfSudents.Any(x => x.Opt_Id == OperationTypeId) ? db.HousingOfSudents.Where(x => x.Opt_Id == OperationTypeId).Max(x => x.Opt_No) + 1 : 1

                            };
                            db.HousingOfSudents.Add(housingOfSudents);
                            db.SaveChanges();

                            // Add New Furnitures
                            var housingFurnituresOfStudent = HousingReplacementStudentsList.Where(x => x.housingOfSudent.StudentId != HousingReplacementStudent.StudentId).FirstOrDefault().housingOfSudent.HousingFurnituresOfStudent.Where(x => x.ReturnDate == null).ToList();
                            if (housingFurnituresOfStudent != null)
                            {
                                db.HousingFurnituresOfStudent.AddRange(
                                     housingFurnituresOfStudent.Select(x =>
                                     {
                                         x.HousingOfSudentId = housingOfSudents.Id;
                                         return x;
                                     }).ToList()
                                );
                            }
                            db.SaveChanges();
                        };
                        foreach (var HousingReplacementStudent in HousingReplacementStudentsList)
                        {
                            // Update Furnitures Of Sudent
                            var HousingFurnituresOfStudent = db.HousingFurnituresOfStudent.Where(x => x.HousingOfSudentId == HousingReplacementStudent.housingOfSudent.Id && x.ReturnDate == null).ToList();
                            if (HousingFurnituresOfStudent != null)
                            {
                                if (HousingFurnituresOfStudent.Any(x => x.ReceivingDate > HousingReplacementDate))
                                {
                                    return Content(@"لا يمكن أن يكون تاريخ إستلام العهدة من الطالب قبل تاريخ تسليمها للطالب");
                                }
                                else
                                {
                                    foreach (var FurnitureOfStudent in HousingFurnituresOfStudent)
                                    {

                                        FurnitureOfStudent.ReturnDate = HousingReplacementDate;
                                        FurnitureOfStudent.ReturnNotes = HousingReplacementStudent.ReplacementNotes == null ? "لا يوجد" : HousingReplacementStudent.ReplacementNotes;
                                        FurnitureOfStudent.SupervisorIdWhoTake = UserID;
                                        FurnitureOfStudent.LastEditDate = DateTime.Now;
                                        FurnitureOfStudent.LastEdittedBy = UserID;
                                        db.Entry(FurnitureOfStudent).State = EntityState.Modified;
                                    }
                                }
                            }
                        }
                        db.SaveChanges();
                        scope.Complete();
                        return Json("", JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    return Content(@" حدث خطأ " + ex.Message);
                }
            }
            else
            {
                if (housingOfFirstSudent == null && housingOfSecondSudent == null)
                {
                    return Content(@"لايمكن إتمام عملية التبديل حيث أن هؤلاء الطلاب غير مسكنين حاليا");
                }
                else if (housingOfFirstSudent == null)
                {
                    return Content(@"لايمكن إتمام عملية التبديل حيث أن الطالب الأول غير مسكن حاليا");
                }
                else
                {
                    return Content(@"لايمكن إتمام عملية التبديل حيث أن الطالب الثاني غير مسكن حاليا");
                }

            }


        }

        #region View Models
        public class HousingFurnitureVM
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public int FurnitureId { get; set; }
            public string FurnitureName { get; set; }
            public int LocationId { get; set; }
            public int HousingFurnituresOfStudent_Id { get; set; }

        }

        public class HousingReplacementStudentsVM
        {
            internal HousingOfSudents housingOfSudent;

            public int StudentId { get; set; }
            public DateTime ReplacementDate { get; set; }
            public string ReplacementNotes { get; set; }
        }
        #endregion

        #region Permissions

        private class CustomPermissions
        {
            public bool Home { get; set; }

            public bool Housing { get; set; }

            public bool Replacing { get; set; }

            public bool DeliveryTo { get; set; }

            public bool DeliveryFrom { get; set; }

            public bool Transfering { get; set; }
            public bool ClearanceFrom { get; set; }

        }


        private int UserID
        {
            get
            {
                var user = HttpContext.Session["UserId"] as DashBoard_Users;

                if (user != null && user.ID != 0)
                {
                    return user.ID;
                }
                else
                {
                    return 0;
                }
            }

        }


        [HttpPost]
        public JsonResult GetPermissions(int screenId)
        {

            if (UserID == 0)
            {
                return null;
            }

            var perm = CheckPermissions.IsAuthorized(UserID, screenId);

            var permissions = new CustomPermissions();
            foreach (var permission in perm)
            {
                if (permission == "البحث الأساسية")
                {
                    permissions.Home = true;
                }
                if (permission == "تسكين الطالب")
                {
                    permissions.Housing = true;
                }
                else if (permission == "تبديل السكن")
                {
                    permissions.Replacing = true;
                }
                else if (permission == "تسليم العهد")
                {
                    permissions.DeliveryTo = true;
                }
                else if (permission == "نقل الطالب")
                {
                    permissions.Transfering = true;
                }
                else if (permission == "إستلام العهد")
                {
                    permissions.DeliveryFrom = true;
                }
                else if (permission == "إخلاء الطرف")
                {
                    permissions.ClearanceFrom = true;
                }

            }

            return Json(permissions, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}
