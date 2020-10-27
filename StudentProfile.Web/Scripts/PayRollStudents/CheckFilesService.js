app.service("CheckFilesSrvc", ['$http', 'GenericService', function ($http, GenericService) {
    this.GetPayrollWithNotReceiveChecksYet = function () {
        return GenericService.GetAll("/PayrollStudents/GetPayRollsWithNotReceivedAfterExportedChecks");
    };  

    this.GetUsers = function (payrollID) {
        return GenericService.GetAll("/PayrollStudents/GetPayrollMutiUsers?payrollID=" + payrollID);
    };


    this.GetChecksData = function (userIDS,payrollID) {
        return GenericService.GetAll("/PayrollStudents/GetPayRollsExportedChecksForManyUsers?userID=" + userIDS + "&payrollID=" + payrollID);
    };
    this.UpdateChecksState = function (PayrollCheckIDS) {
        return $http({
            method: 'POST',
            url: '/PayRollStudents/UpdateChecksState',
            data: {"PayrollCheckIDS": PayrollCheckIDS }
        });
    };
}]);