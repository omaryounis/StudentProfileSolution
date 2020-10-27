app.service("PayRollApprovalSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetPayRolls = function () {
        return GenericService.GetAll("/PayRollStudents/GetPayRolls");
    };

    this.GetPayRollDetails = function (id) {
        return GenericService.GetByID("/PayRollStudents/GetPayRollDetails",id);
    };

    //this.ApprovePayRoll = function () {
    //    return GenericService.GetAll("/PayRollStudents/GetYears");
    //};

    //this.CancelPayRoll = function () {
    //   return GenericService.GetAll("/PayRollStudents/GetMonths");
    //};

}]);