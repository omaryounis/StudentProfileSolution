app.service("CashCheckSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetExchangeOrder = function () {
        return GenericService.GetAll("/ExchangeOrder/GetExchangeOrder");
    };

    this.GetExchangeOrderIsActive = function (id) {
        return GenericService.GetByID("/ExchangeOrder/GetExchangeOrderIsActive",id);
    };

    this.GetExchangeOrderValueById = function (id) {
        return GenericService.GetByID("/ExchangeOrder/GetExchangeOrderValueById", id);
    };

    this.GetExchangeOrderValueByIdForEdit = function (id, CurrentOrderId) {
        return GenericService.GetByID("/ExchangeOrder/GetExchangeOrderValueByIdForEdit", id +"?CurrentOrderId=" + CurrentOrderId);
    };

    this.SaveSaveCheks = function (model) {
        return GenericService.Post("/ExchangeOrder/SaveSaveCheks", model);
    };

    this.GetCashChecks = function () {
        return GenericService.GetAll("/ExchangeOrder/GetCashChecks");
    };

    this.GetExchangeOrdersChecksByCheckId = function (id) {
        return GenericService.GetByID("/ExchangeOrder/GetExchangeOrdersChecksByCheckId", id);
    };

    this.DeleteCheck = function (id) {
        return GenericService.Delete("/ExchangeOrder/DeleteCheck", id);
    };

}]);