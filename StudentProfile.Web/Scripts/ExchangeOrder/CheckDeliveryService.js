app.service("CheckDeliveryService", ['$http', 'GenericService', function ($http, GenericService)
{

    this.AssignCheckToUser = function (checkIds, deliveredTo) {
        return $http({
            method: 'POST',
            url: '/ExchangeOrder/AssignCheckToUser',
            data: { "checkIds": checkIds, "deliveredTo": deliveredTo }
        });
    };

    this.GetCurrentChecksIDs = function (userID) {
        return $http({
            method: 'Get',
            url: '/ExchangeOrder/GetCurrentChecksByUserID?userID=' + userID
        });
    };
    this.GetUsers = function () {
        return $http({
            method: 'Get',
            url: '/ExchangeOrder/GetUsers'
        });
    };
}]);