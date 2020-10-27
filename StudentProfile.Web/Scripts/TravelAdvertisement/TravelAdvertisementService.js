app.service("TravelAdvertisementSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetPurpose = function () {
        return GenericService.GetAll("/TravelAdvertisement/GetPurpose");
    };

    this.GetTravelAgent = function () {
        return GenericService.GetAll("/TravelAdvertisement/GetTravelAgent");
    };

    this.GetNationalities = function () {
        return GenericService.GetAll("/TravelAdvertisement/GetNationalities");
    };
    this.GetLevels = function () {
        return GenericService.GetAll("/Musafer/GetLevels");
    };
    this.GetDegrees = function () {
        return GenericService.GetAll("/TravelAdvertisement/GetDegrees");
    };
    this.GetCustomFields = function (ParentId) {
        return GenericService.GetAll("/TravelAdvertisement/GetCustomFields?ParentId="+ParentId);
    };

    this.MajorGetCustomFields = function () {
        return GenericService.GetAll("/TravelAdvertisement/MajorGetCustomFields");
    };
    this.GetStatus = function () {
        return GenericService.GetAll("/TravelAdvertisement/GetStatus");
    };

    this.GetScholarships = function () {
        return GenericService.GetAll("/TravelAdvertisement/GetScholarships");
    };

    this.SaveTravelAdvertisement = function (model) {
        return GenericService.Post("/TravelAdvertisement/SaveTravelAdvertisement", model);
    };

    //Advertisement List
    this.GetAdvertisement = function () {
        return GenericService.GetAll("/TravelAdvertisement/GetAdvertisement");
    };

    this.ActiveAdvertisement = function (id) {
        return GenericService.GetByID("/TravelAdvertisement/ActiveAdvertisement" , id);
    };

    this.GetAdvertisementById = function (id) {
        return GenericService.GetByID("/TravelAdvertisement/GetAdvertisementById", id);
    };

    //SaveTravelConfig
    this.SaveTravelConfig = function (model) {
        return GenericService.Post("/TravelAdvertisement/SaveTravelConfig", model);
    };

    this.GetTravelConfigByKay = function (kay) {
        return GenericService.GetByQuery("/TravelAdvertisement/GetTravelConfigByKay?kay=" + kay);
    };

}]);