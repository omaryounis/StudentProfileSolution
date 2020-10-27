app.service("PayrollApprovalWithMoneyService", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetPayrolls = function () {
        return GenericService.GetAll("/PayRollStudents/GetPayRolls");
    };

    this.GetPayrollsDetails = function () {
        return GenericService.GetAll("/PayRollStudents/GetDegrees");
    };
    this.PayrollApprovallAction = function (payrollID, notes, payNo, dafPayNo, isApproved, isexportedcheck) {
        return GenericService.Post("/PayRollStudents/PayrollApprovallActionMonetary", { payrollID: payrollID, notes: notes, PayNo: payNo, DafPayNo: dafPayNo, isApproved: isApproved, isexportedcheck: isexportedcheck } );
    };
    this.GetPayrollApprovalHistory = function (payrollID) {
        return GenericService.GetAll("/PayRollStudents/GetPayrollApprovalHistory?payrollID=" + payrollID);
    };
    this.GetPayRollFiles = function (payrollID) {
        return GenericService.GetAll("/PayRollStudents/GetPayRollFiles?payrollID=" + payrollID);
    };
    this.GetPayRollPhase = function (payrollID) {
        return GenericService.GetAll("/PayRollStudents/GetPayRollPhase?payrollID=" + payrollID);
    };
}]);