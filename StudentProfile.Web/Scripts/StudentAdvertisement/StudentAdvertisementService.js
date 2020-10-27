app.service("StudentAdvertisementSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetStudentAdvertisement = function () {
        return GenericService.GetAll("/Musafer/GetStudentAdvertisement");
    };
    this.GetStudentTravelRequestData = function (ID) {
        return GenericService.GetByID("/Musafer/GetStudentTravelRequestData", ID);
    };
    this.IsValidStudentData = function () {
        return GenericService.GetAll("/Musafer/IsValidStudentData");
    };
    this.CheckAllowStudent = function () {
        return GenericService.GetAll("/Musafer/CheckAllowStudent");
    };

    this.CheckStudentData = function (ID) {
        return GenericService.GetByID("/Musafer/CheckStudentData",ID);
    };

    this.CheckStudentTicketAdvertisement = function (id) {
        return GenericService.GetByID("/Musafer/CheckStudentTicketAdvertisement", id);
    };

    this.CheckStudentAllowAdvertisement = function (id) {
        return GenericService.GetByID("/Musafer/CheckStudentAllowAdvertisement", id);
    };


    this.GetStudentTravelRequestData = function (id) {
        return GenericService.GetByID("/Musafer/GetStudentTravelRequestData", id);
    };

}]);