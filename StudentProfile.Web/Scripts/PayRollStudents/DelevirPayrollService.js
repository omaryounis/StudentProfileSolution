app.service("DelevirPayrollSrvc", ['$http', 'GenericService', function ($http, GenericService) {
    this.GetPayrollWithNotReceiveChecksYet = function () {
        return GenericService.GetAll("/PayrollStudents/GetPayrollWithNotReceiveChecksYet");
    };

    this.GetPayrollWithNotReceiveChecksYet = function () {
        return GenericService.GetAll("/PayrollStudents/GetPayrollWithNotReceiveChecksYet");
    };

    this.GetUsers = function (payrollID) {
        return GenericService.GetAll("/PayrollStudents/GetPayrollUsers?payrollID=" + payrollID);
    };
    this.GetYears = function () {
        return GenericService.GetAll("/PayrollStudents/GetYearsData");
    };
    this.GetMonths = function (YearID) {
        return GenericService.GetAll("/PayrollStudents/GetMonthsData?year="+YearID);
    };
    this.GetPayroll = function (YearID, MonthID) {
        return GenericService.GetAll("/PayrollStudents/GetPayrollNumberData?year=" + YearID + "&month=" + MonthID);
    };

    this.GetDelevirData = function (yearID,monthID,payrollID) {
        return GenericService.GetAll("/PayrollStudents/GetDelevirData?yearID=" + yearID + "&monthID=" + monthID + "&payrollID=" + payrollID);
    };
    this.UpdateStudentPayrollState = function (studentids, payrollids, peneficiaryIds,payrollID, yearID, monthID) {
        return $http({
            method: 'POST',
            url: '/PayRollStudents/UpdateStudentPayrollState',
            data: {
                "studentids": studentids, "payrollids": payrollids, "peneficiaryIds": peneficiaryIds
                , "payrollID": payrollID, "yearID": yearID, "monthID": monthID
                }
        });
    };
}]);