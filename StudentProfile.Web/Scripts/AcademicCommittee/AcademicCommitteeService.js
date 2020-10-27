app.service("AcademicCommitteeSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetFaculties = function () {
        return GenericService.GetAll("/AcademicCommittee/GetFaculties");
    };

    this.GetDegrees = function () {
        return GenericService.GetAll("/AcademicCommittee/GetDegrees");
    };
    this.GetStudentsForAcademicCommittee = function (facultyIds, degreeIds) {
        return GenericService.GetAll("/AcademicCommittee/GetStudentsForAcademicCommittee?facultyIds=" + facultyIds + "&degreeIds=" + degreeIds);
    };

    this.AcceptStudents = function (model) {
        return GenericService.Post("/AcademicCommittee/AcceptStudents", model);
    };

    this.RejectStudents = function (model) {
        return GenericService.Post("/AcademicCommittee/RejectStudents", model);
    };


    //Upload Files
    this.GetAcademicCommitteeStudents = function () {
        return GenericService.GetAll("/AcademicCommittee/GetAcademicCommitteeStudents");
    };

    this.SaveDecisions = function (model) {
      return GenericService.Post("/AcademicCommittee/SaveDecisions", model);
    };


}]);