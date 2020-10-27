app.service("RevisionPassportSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetStudentsDocuments = function (isApproved) {
        debugger;
        return GenericService.GetAll("/RevisionPassport/GetStudentsDocuments?isApproved=" + isApproved);
    };

    this.GetRevisionPassportByUser = function (id, IsApproved) {
        return GenericService.GetAll("/RevisionPassport/GetRevisionPassportByUser/" + id + "?IsApproved=" + IsApproved );
    };
    this.RemoveDocument = function (id) {
        return GenericService.GetByID("/RevisionPassport/RemoveDocument", id);
    };
    this.ActiveDocument = function (id) {
        return GenericService.GetByID("/RevisionPassport/ActiveDocument", id);
    };

    this.ApprovedDocuments = function (model) {
        return GenericService.Post("/RevisionPassport/ApprovedDocuments", model);
    };
    this.RefusedDocuments = function (model) {
        return GenericService.Post("/RevisionPassport/RefusedDocuments", model);
    };
    this.PreviewImage = function (id) {
        return GenericService.GetByID("/RevisionPassport/PreviewStudentDocument", id);
    };
}]);