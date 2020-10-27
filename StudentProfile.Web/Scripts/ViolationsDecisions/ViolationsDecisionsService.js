app.service("ViolationsDecisionsSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetViolationsDecisions = function () {
        return GenericService.GetAll("/ViolationsDecisions/GetViolationsDecisions");
    };

    this.SaveViolationsDecisions = function (model) {
        return GenericService.Post("/ViolationsDecisions/SaveViolationsDecisions", model);
    };

    this.GetViolationsDecisionsById = function (id) {
        return GenericService.GetByID("/ViolationsDecisions/GetViolationsDecisionsById", id);
    };


    this.DeleteViolationsDecisions = function (id) {
        return GenericService.Delete("/ViolationsDecisions/DeleteViolationsDecisions", id);
    };

}]);