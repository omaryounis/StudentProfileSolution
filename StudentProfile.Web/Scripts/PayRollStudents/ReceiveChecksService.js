app.service("ReceiveChecksSrvc", ['$http', 'GenericService', function ($http, GenericService) {
    this.GetPayrollWithNotReceiveChecksYet = function () {
        return GenericService.GetAll("/PayrollStudents/GetPayrollWithNotReceiveChecksYet");
    };

    this.GetUsers = function (payrollID) {
        return GenericService.GetAll("/PayrollStudents/GetPayrollUsers?payrollID=" + payrollID);
    };


    this.GetChecksData = function (userIDS,payrollID) {
        return GenericService.GetAll("/PayrollStudents/GetChecksData?userID=" + userIDS + "&payrollID=" + payrollID);
    };
    this.UpdateChecksState = function (PayrollCheckIDS , payrollID) {
        return $http({
            method: 'POST',
            url: '/PayRollStudents/UpdateChecksState',
            data: { "PayrollCheckIDS": PayrollCheckIDS, PayrollID: payrollID }
        });
    };
}]);