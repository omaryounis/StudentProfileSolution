app.service("CalculationBonusSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetDegrees = function () {
        return GenericService.GetAll("/CalculationBonus/GetDegrees");
    };

    this.GetFaculties = function () {
        return GenericService.GetAll("/CalculationBonus/GetFaculties");
    };

    this.GetAllRewardItems = function (model) {
        return GenericService.GetByModel("/CalculationBonus/GetAllRewardItems", model);
    };

    this.GetStudents = function (model) {
        return GenericService.Post("/CalculationBonus/GetStudents", model);
    };


    this.GetPayRollNumber = function () {
        return GenericService.GetAll("/CalculationBonus/GetPayRollNumber");
    };


    this.SaveCalculationBonus = function (modelObj, StudentStatusAcademySelectedValues) {
        debugger;
        if (StudentStatusAcademySelectedValues == null || StudentStatusAcademySelectedValues == undefined) {
            debugger;
            return GenericService.Post("/CalculationBonus/CalculateStudentReward", { model: modelObj });
        } else {
            debugger;
            return GenericService.Post("/CalculationBonus/CalculateStudentRewardForAcaedmy", { model: modelObj });
        }
    };


    this.GetStudentPayroll = function () {
        return GenericService.GetAll("/CalculationBonus/GetStudentPayroll");
    };

    this.DeletePayroll = function (PayrollNumber) {
        //return GenericService.Delete("/CalculationBonus/DeletePayroll", id);
        return GenericService.GetAll("/CalculationBonus/DeletePayroll?payrollNumber=" + PayrollNumber);
    };

    this.StudentPayrollPDF = function () {
        return GenericService.GetAll("/CalculationBonus/StudentPayrollPDF");
    };
    this.CheckIfAdvanceItemAlreadySelected = function (selectedItems) {
        return GenericService.GetAll("/CalculationBonus/CheckIfAdvanceItemAlreadySelected?selectedItemList=" + selectedItems);
    };
    this.PostPayrollForApproval = function (payrollNum) {
        return GenericService.GetAll("/CalculationBonus/PostPayrollForApproval?payrollNumber=" + payrollNum);
    };
}]);