app.service("SeparatedStudentsSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetFaculties = function () {
        return GenericService.GetAll("/AcademicCommittee/GetFaculties");
    };

    this.GetDegrees = function (FacultionIds) {
        return GenericService.GetAll("/AcademicCommittee/GetDegrees?FacultionIds=" + FacultionIds);
    };

    this.GetStudents = function (facultyIds, degreeIds) {
        return GenericService.GetAll("/AcademicCommittee/GetStudents?facultyIds=" + facultyIds + "&degreeIds=" + degreeIds);
    };

    this.RelaySeparatedStudents = function (model) {
        return GenericService.Post("/AcademicCommittee/RelaySeparatedStudents", model);
    };


    this.GetStudentsSemesterTrans = function (id) {
        //return GenericService.GetByID("/AcademicCommittee/GetStudentsSemesterTrans", id);
        return GenericService.GetAll("/AcademicCommittee/GetStudentsSemesterTrans?StudentId=" + id);
    };

    this.GetStudentsChances = function (id) {
        //return GenericService.GetByID("/AcademicCommittee/GetStudentsChances", id);

        return GenericService.GetAll("/AcademicCommittee/GetStudentsChances?StudentId="+id);
    };

    this.GetStudentWarning = function (id) {
        //return GenericService.GetByID("/AcademicCommittee/GetStudentWarning", id);
        return GenericService.GetAll("/AcademicCommittee/GetStudentWarning?StudentId=" + id);
    };
    this.UpdateStudentsData = function () {
        return GenericService.GetAll("/GraduateStudies/UpdateStudentsData");
    };

}]);