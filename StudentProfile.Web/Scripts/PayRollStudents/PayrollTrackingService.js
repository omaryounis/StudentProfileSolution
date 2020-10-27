app.service("PayrollTrackingSrvc", ['$http', 'GenericService', function ($http, GenericService) {
    this.GetActivePayRolls = function () {
        return GenericService.GetAll("/PayrollStudents/GetActivePayRolls");
    };
}]);