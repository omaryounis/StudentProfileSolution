app.service("StudentsCashService", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetStudentPayrollsByPayrollID = function (payrollID) {
        return $http({
            method: 'GET',
            url: '/ExchangeOrder/GetStudentPayrollsByPayrollID?payrollID=' + payrollID
        });
    };
    this.GetExchangeOrdersByCheckID = function (checkID) {
        return $http({
            method: 'GET',
            url: '/ExchangeOrder/GetExchangeOrdersByCheckID?checkID=' + checkID
        });
    };
    this.GetPayrollsByExchangeOrders = function (exchangeOrderList) {
        debugger;
        return $http({
            method: 'GET',
            url: '/ExchangeOrder/GetPayrollsByExchangeOrders?exchangeOrderIds=' + exchangeOrderList
            
        });
    };
    this.PayRewardtoStudents = function (checkid,selectedStudentList) {
        debugger;
        return $http({
            method: 'POST',
            url: '/ExchangeOrder/PayRewardtoStudents',
            data: { "checkID": checkid, "selectedStudents": selectedStudentList}

        });
    };
}]);