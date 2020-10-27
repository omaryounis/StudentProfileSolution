app.service("PayrollApprovalWithMoneyForIssuingChecksService", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetPayrolls = function () {
        return GenericService.GetAll("/PayRollStudents/GetPayRolls");
    };

    this.GetPayrollsDetails = function () {
        return GenericService.GetAll("/PayRollStudents/GetDegrees");
    };
    this.PayrollApprovallAction = function (payrollID, notes, payNo, dafPayNo, isApproved, isexportedcheck) {
        return GenericService.Post("/PayRollStudents/PayrollApprovallActionMonetary", { payrollID: payrollID, notes: notes, PayNo: payNo, DafPayNo: dafPayNo, isApproved: isApproved, isexportedcheck: isexportedcheck  } );
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
    this.GetFaculties = function (userID, payrollID) {
        return GenericService.GetAll("/PayRollStudents/GetFaculties?UserId=" + userID + "&payrollID=" + payrollID);
    };
    this.GetFacultiesForCheck = function (userID, payrollID, checkID) {
        return GenericService.GetAll("/PayRollStudents/GetFacultiesForCheck?UserId=" + userID + "&payrollID=" + payrollID + "&checkID=" + checkID);
    };
    this.saveCheck = function (Check) {
        return $http({
            method: 'POST',
            url: '/PayRollStudents/saveCheck',
            data: {
                "ID": Check.ID, "BenfName": Check.BenfName, "BeneficiaryID": Check.BeneficiaryID, "PayrollID": Check.PayrollID, "FacultyID": Check.FacultyID, "CheckNumber": Check.txtCheckNumberName, "CheckValue": Check.txtCheckValueName, "Description": Check.byanTextAreaValue
            }
        });
    };
    this.cancelCheck = function (ID) {
        return $http({
            method: 'POST',
            url: '/PayRollStudents/cancelCheck',
            data: {
                "ID": ID
            }
        });
    };
    this.GetPayrollCheckById = function (id) {
        return $http({
            method: 'GET',
            url: '/PayRollStudents/GetPayrollCheckById?ID=' + id
        });
    };
}]);