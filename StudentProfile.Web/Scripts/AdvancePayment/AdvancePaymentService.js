app.service("AdvancePaymentSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetAdvanceSettings = function () {
        return GenericService.GetAll("/AdvancePayment/GetAdvanceSettings");
    };

    this.GetAdvanceRequests = function (Type,StudentID) {
        return GenericService.GetAll("/AdvancePayment/GetAdvanceRequests?Type=" + Type + "&StudentID=" + StudentID);
    };
    this.GetStudentDataByID = function (StudentId) {
        debugger;
        var response = $http({
            method: "GET",
            url: "/Advances/GetStudentDataByID/",
            params: {
                studentId: StudentId
            }
        });
        return response;
    };

    //PopupCash
    this.GetAcctounts = function () {
        return GenericService.GetAll("/AdvancePayment/GetAcctounts");
    };
    this.SaveAdvances = function (model) {
        return GenericService.Post("/AdvancePayment/SaveAdvances", model);
    };


    //GetTreasuryAccount
    this.GetTreasuryAccount = function () {
        return GenericService.GetAll("/Advances/GetTreasuryAccount");
    };
 

}]);