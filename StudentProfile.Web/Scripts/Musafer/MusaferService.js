app.service("MusaferSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.SendEmailConfirmedCode = function () {
        return GenericService.GetAll("/Musafer/SendEmailConfirmedCode");
    };

    this.SendSMSConfirmedCode = function () {
        return GenericService.GetAll("/Musafer/SendSMSConfirmedCode");
    };

    this.ConfirmCode = function (model) {
        return GenericService.Post("/Musafer/ConfirmCode", model);
    };

    /*Send Request*/

    this.GetCountries = function () {
        return GenericService.GetAll("/Musafer/GetCountries");
    };
    this.GetTransportationTracking = function (FlightsType) {
        return GenericService.GetAll("/Musafer/GetTransportationTracking?FlightsType=" + FlightsType);
    };

   
    this.GetAirports = function (id) {
        return GenericService.GetByID("/Musafer/GetAirports", id);
    };



    this.GetLevels = function () {
        return GenericService.GetAll("/Musafer/GetLevels");
    };

    this.SaveRequest = function (model) {
        return GenericService.Post("/Musafer/SaveRequest", model);
    };

    //this.GetAdvertisementById = function (id) {
    //    return GenericService.GetByID("/Musafer/GetAdvertisementById",id);
    //};

    this.GetAdvertisementById = function (advertisementId) {


        var response = $http({
            method: "GET",
            url: "/Musafer/GetAdvertisementById",
            params: {
                ID: Number(advertisementId)
            }
        });

        return response;
    };



    //GetStudentTicketAdvertisementForEdit
    this.GetStudentTicketAdvertisementForEdit = function (id) {
        return GenericService.GetByID("/Musafer/GetStudentTicketAdvertisementForEdit", id);
    };

    this.GetStudentPassportImage = function () {
        return GenericService.GetAll("/Musafer/GetStudentPassportImage");
    };

    //CheckStudentPayAdvanceAmount
    this.CheckStudentPayAdvanceAmount = function (id) {
        return GenericService.GetByID("/Musafer/CheckStudentPayAdvanceAmount",id);
    };

}]);