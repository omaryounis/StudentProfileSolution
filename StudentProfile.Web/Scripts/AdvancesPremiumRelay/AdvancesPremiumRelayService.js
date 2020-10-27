app.service("AdvancesPremiumRelaySrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetAdvancesPremiumRelays = function () {
        return GenericService.GetAll("/Advances/GetAdvancesPremiumRelays");
    };

    this.SaveAdvancesPremiumRelaysToAccount = function (model) {
        return GenericService.Post("/Advances/SaveAdvancesPremiumRelaysToAccount" , model);
    };
}]);