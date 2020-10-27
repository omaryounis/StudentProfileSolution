app.service("MusaferReportsSrvc", ['$http', 'GenericService', function ($http, GenericService) {
    this.GetAllAdvertisement = function () {
        return GenericService.GetAll("/Reports/GetAllAdvertisement");
    };

    this.GetAllStudentsByAdvertisement = function (id) {
        return GenericService.GetByID("/Reports/GetAllStudentsByAdvertisement", id);
    };
}]);