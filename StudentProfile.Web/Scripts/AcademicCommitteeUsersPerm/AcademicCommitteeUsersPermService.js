app.service("AcademicCommitteeUsersPermSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetUsers = function () {
        return GenericService.GetAll("/AcademicCommitteeUsersPerm/GetUsers");
    };

    this.GetFaculties = function () {
        return GenericService.GetAll("/AcademicCommitteeUsersPerm/GetFaculties");
    };

    this.GetAcademicCommitteeUsersPerm = function () {
        return GenericService.GetAll("/AcademicCommitteeUsersPerm/GetAcademicCommitteeUsersPerm");
    };

    this.SaveAcademicCommitteeUsersPerm = function (model) {
        return GenericService.Post("/AcademicCommitteeUsersPerm/SaveAcademicCommitteeUsersPerm", model);
    };

    this.ActiveAcademicCommitteeUsersPerm = function (id) {
        return GenericService.GetByID("/AcademicCommitteeUsersPerm/ActiveAcademicCommitteeUsersPerm", id);
    };

    this.DeleteAcademicCommitteeUsersPerm = function (id) {
        return GenericService.Delete("/AcademicCommitteeUsersPerm/DeleteAcademicCommitteeUsersPerm", id);
    };
    this.GetDegrees = function (FacultionIds) {
        return GenericService.GetAll("/AcademicCommitteeUsersPerm/GetDegrees?FacultionIds="+FacultionIds);
    };

}]);