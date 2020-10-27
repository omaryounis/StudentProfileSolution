app.service("AcademicCommitteeUsersSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetUsers = function () {
        return GenericService.GetAll("/AcademicCommitteeUsers/GetUsers");
    };

    this.GetFaculties = function () {
        return GenericService.GetAll("/AcademicCommitteeUsers/GetFaculties");
    };

    this.GetAcademicCommitteeUsers = function () {
        return GenericService.GetAll("/AcademicCommitteeUsers/GetAcademicCommitteeUsers");
    };

    this.SaveAcademicCommitteeUsers = function (model) {
        return GenericService.Post("/AcademicCommitteeUsers/SaveAcademicCommitteeUsers", model);
    };

    this.ActiveAcademicCommitteeUsers = function (id) {
        return GenericService.GetByID("/AcademicCommitteeUsers/ActiveAcademicCommitteeUsers", id);
    };

    this.DeleteAcademicCommitteeUsers = function (id) {
        return GenericService.Delete("/AcademicCommitteeUsers/DeleteAcademicCommitteeUsers", id);
    };
    this.GetDegrees = function (FacultionIds) {
        return GenericService.GetAll("/AcademicCommitteeUsers/GetDegrees?FacultionIds="+FacultionIds);
    };

}]);