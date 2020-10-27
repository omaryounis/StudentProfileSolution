app.service("PayrollApprovalService", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetPayrolls = function () {
        return GenericService.GetAll("/PayRollStudents/GetPayRolls");
    };

    this.GetPayrollsDetails = function () {
        return GenericService.GetAll("/PayRollStudents/GetDegrees");
    };
    this.PayrollApprovallAction = function (payrollID, notes, payNo, dafPayNo, dafPayNo2, isApproved) {
        return GenericService.Post("/PayRollStudents/PayrollApprovallAction", { payrollID: payrollID, notes: notes, PayNo: payNo, DafPayNo: dafPayNo, DafPayNo2: dafPayNo2, isApproved: isApproved  } );
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