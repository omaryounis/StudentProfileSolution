app.service("RewardItemPremiumRelaysrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetRewardItemPremiumRelays = function () {
        return GenericService.GetAll("/Advances/GetRewardItemPremiumRelays");
    };

    this.SaveRewardItemPremiumRelaysToAccount = function (model) {
        return GenericService.Post("/Advances/SaveRewardItemPremiumRelaysToAccount" , model);
    };
}]);