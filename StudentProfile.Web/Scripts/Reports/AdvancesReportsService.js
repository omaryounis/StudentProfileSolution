app.service("AdvancesReportsSrvc", ['$http', 'GenericService', function ($http, GenericService) {
    this.GetStudents = function () {
        return GenericService.GetAll("/Reports/GetStudentsForAdvances");
    };

    this.AdvancesRecievingByStudents = function (model) {
        return GenericService.Post("/Reports/AdvancesRecievingByStudents", model);
    };
}]);