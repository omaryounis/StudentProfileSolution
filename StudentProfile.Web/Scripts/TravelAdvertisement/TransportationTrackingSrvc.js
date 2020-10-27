app.service('TransportationTrackingSrvc', ["$http", 'GenericService', function ($http, GenericService) {
    this.GetAllTransportationTracking = function () {
        return $http({
            method: 'GET',
            url: '/TravelAdvertisement/GetAllTransportationTracking'
        });
    };

    this.GetByID = function (id) {
        return $http({
            method: 'GET',
            url: '/TravelAdvertisement/GetTransportationTrackingByID?ID=' + id
        });
    };
    this.DeleteTransportationTracking = function (id) {
        return GenericService.Delete("/TravelAdvertisement/DeleteTransportationTracking", id);
    };
    this.addTransportationTracking = function (TrackingName, NationalityID, IsActive, FlightsType) {
        return $http({
            method: 'POST',
            url: '/TravelAdvertisement/addTransportationTracking',
            data: {
                "TrackingName": TrackingName, "NationalityID": NationalityID, "IsActive": IsActive, "FlightsType": FlightsType}
        });
    };
    this.updateTransportationTracking = function (ID, TrackingName, NationalityID, IsActive, FlightsType) {
        debugger;
        return $http({
            method: 'POST',
            url: '/TravelAdvertisement/updateTransportationTracking',
            data: { "ID": ID, "TrackingName": TrackingName, "NationalityID": NationalityID, "IsActive": IsActive, "FlightsType": FlightsType }
        });
    };

    this.GetAllNationalities = function () {
        return $http({
            method: 'GET',
            url: '/TravelAdvertisement/GetAllNationalities'
        });
    };
}]);
