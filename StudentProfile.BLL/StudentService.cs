using StudentProfile.DAL;
using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentProfile.Persistence
{
    public class StudentService
    {
        private IGenericRepository<INTEGRATION_All_Students, SchoolAccGam3aEntities> studentRepository;
        public StudentService()
        {
            //studentRepository = new GenericRepository<INTEGRATION_All_Students>(new SchoolAccGam3aEntities());
        }
        public INTEGRATION_All_Students GetStudent(int id)
        {
            return studentRepository.GetById(id);
        }

        //public IEnumerable<PR_GetStudentAlertsCount_Result> GetStudentAlerts(int studentID, string NationalID)
        //{
        //    return (List<PR_GetStudentAlertsCount_Result>)studentRepository.ExecuteQuery(typeof(PR_GetStudentAlertsCount_Result), "Exec PR_GetStudentAlertsCount",null);
        //}
        //public int? GetStudentAlertsCountBytableName(int studentID, string NationalID, string tableName)
        //{
        //    return GetStudentAlerts(studentID, NationalID).Where(x => x.tableName == tableName).SingleOrDefault()?.ItemsCount;
        //}
        public INTEGRATION_All_Students GetByStudentID(int studentID)
        {
            return studentRepository.GetList(x => x.STUDENT_ID == studentID).FirstOrDefault();
        }
        public string GetNationalIDByStudentID(int studentID)
        {
            return GetByStudentID(studentID)?.NATIONAL_ID;
        }

        public IEnumerable<INTEGRATION_All_Students> GetHistoryByNationalID(string nationalID)
        {
            return studentRepository.GetList(x => x.NATIONAL_ID == nationalID);
        }
        public IEnumerable<INTEGRATION_All_Students> GetHistoryByStudentID(int studentID)
        {
            return GetHistoryByNationalID(GetNationalIDByStudentID(studentID));
        }
        //public GetStBasicDataByStudentID_Result GetAllStudentBasicData(int studentID)
        //{
        //    return _context.GetStBasicDataByStudentID(studentID).FirstOrDefault();
        //}
        //public IEnumerable<proc_GetStudentGrades_Result> GetStudentGrades(int studentID, int? semesterID)
        //{
        //    return _context.proc_GetStudentGrades(studentID, semesterID);
        //}
        //public IEnumerable<ST_AcademicData> GetStudentAcademicRecords(int studentID)
        //{
        //    return _context.ST_AcademicData
        //        .Where(x => x.student_id == studentID)
        //        .OrderBy(x => x.semester);
        //}
        //public IEnumerable<V_SIS_VIOLATION_SINGLE> GetStudentViolationRecords(int studentID)
        //{
        //    return _context.V_SIS_VIOLATION_SINGLE
        //        .Where(x => x.STUDENT_ID == studentID);
        //}
        //public IEnumerable<PR_GetAdvancedSearchResults_Result> GetAdvancedSearchResults(string fName, string inName, string lName, string IdentityNum, decimal? StudentNum, string PhoneNum, int[] nationality, int[] degree, decimal? faculty,
        //    decimal? level, int[] StatusType, decimal? StudyType, int[] fieldtype, string sysType, int[] NotInfieldtype)
        //{

        //    bool validButNotExist = false;
        //    string statustype = null;
        //    string fieldType = null;
        //    string notfieldType = null;
        //    string nationalities = null;
        //    string degrees = null;
        //    if (nationality != null && nationality.Count() > 0)
        //        nationalities = string.Join(",", nationality);
        //    if (degree != null && degree.Count() > 0)
        //        degrees = string.Join(",", degree);
        //    if (NotInfieldtype != null && NotInfieldtype.Count() > 0)
        //        notfieldType = string.Join(",", NotInfieldtype);
        //    if (fieldtype != null && fieldtype.Count() > 0)
        //        fieldType = string.Join(",", fieldtype);
        //    if (StatusType != null && StatusType.Count() > 0)
        //        statustype = string.Join(",", StatusType);
        //    if (!string.IsNullOrEmpty(sysType))
        //        validButNotExist = true;


        //    return _context.PR_GetAdvancedSearchResults(fName, inName, lName, IdentityNum, StudentNum, PhoneNum, fieldType, nationalities, degrees, faculty, level, statustype, StudyType, validButNotExist, sysType, notfieldType);
        //}

        //public IEnumerable<usp_getNationalities_Result> GetNationalities()
        //{
        //    return _context.usp_getNationalities();
        //}

        //public IEnumerable<usp_getFaculties_Result> GetFaculities()
        //{
        //    return _context.usp_getFaculties();
        //}

        //public IEnumerable<usp_getMajors_Result> GetMajors()
        //{
        //    return _context.usp_getMajors();
        //}

        //public IEnumerable<usp_getLevels_Result> GetLevels()
        //{
        //    return _context.usp_getLevels();
        //}

        //public IEnumerable<usp_getStatus_Result> GetStatus()
        //{
        //    return _context.usp_getStatus();
        //}

        //public IEnumerable<usp_getStudyTypes_Result> GetStudyTypes()
        //{
        //    return _context.usp_getStudyTypes();
        //}

        //public IEnumerable<usp_getDegrees_Result> GetDegrees()
        //{
        //    return _context.usp_getDegrees();
        //}
    }
}
