app.service('ExchangeOrderSrvc', ["$http","GenericService", function ($http, GenericService) {

    this.GetAllPayRollExchaneOrders = function () {
        return $http({
            method: 'GET',
            url: '/PayRollStudents/GetAllPayRollExchaneOrders'
        });
    };

    this.GetAllPayRolls = function (exchangeOrderId) {
        return $http({
            method: 'GET',
            url: '/PayRollStudents/GetAllPayRolls?exchangeOrderId=' + exchangeOrderId
        });
    };

    //this.addNewPayRollExchaneOrders = function (model) {
    ////this.addNewPayRollExchaneOrders = function (exchangeOrder, payllosId, textValue, ExchangeOrderID) {
    //    return $http({
    //        method: 'POST',
    //        url: '/PayRollStudents/addNewPayRollExchaneOrders',
    //        data: JSON.stringify(model),
    //        contentType: "application/json;" 
    //    });
    //};

    this.addNewPayRollExchaneOrders = function (model) {
        return GenericService.Post("/PayRollStudents/addNewPayRollExchaneOrders", model);
    };

    this.ActiveAndNotActive = function (ID) {
        return $http({
            method: 'POST',
            url: '/PayRollStudents/ActiveAndNotActive',
            data: { "ID": ID}
        });
    };
    this.GetEditExChangeOrder = function (ID) {
        return $http({
            method: 'POST',
            url: '/PayRollStudents/GetEditExChangeOrder',
            data: { "ID": ID }
        });
    };

    this.GetPayrollNumbers = function (ID) {
        return $http({
            method: 'POST',
            url: '/PayRollStudents/GetPayrollNumbers',
            data: { "ID": ID }
        });
    };

}]);
