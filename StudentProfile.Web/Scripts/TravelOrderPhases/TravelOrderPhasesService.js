app.service("TravelOrderPhasesSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetUsers = function () {
        return GenericService.GetAll("/TravelOrderPhases/GetUsers");
    };

    this.GetPhases = function () {
        return GenericService.GetAll("/TravelOrderPhases/GetPhases");
    };

    this.GetPhasesUsers = function () {
        return GenericService.GetAll("/TravelOrderPhases/GetPhasesUsers");
    };

    this.SavePhasesUsers = function (model) {
        return GenericService.Post("/TravelOrderPhases/SavePhasesUsers", model);
    };

    this.ActivePhasesUsers = function (id) {
        return GenericService.GetByID("/TravelOrderPhases/ActivePhasesUsers", id);
    };

    this.DeletePhasesUsers = function (id) {
        return GenericService.Delete("/TravelOrderPhases/DeletePhasesUsers", id);
    };

}]);