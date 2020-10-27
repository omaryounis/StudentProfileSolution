app.service("IssuesSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetIssuesCategories = function () {
        return GenericService.GetAll("/Issues/GetIssuesCategories");
    };

    this.GetIssues = function () {
        return GenericService.GetAll("/Issues/GetIssues");
    };

    this.SaveIssues = function (model) {
        return GenericService.Post("/Issues/SaveIssues", model);
    };

    this.GetIssuesById = function (id) {
        return GenericService.GetByID("/Issues/GetIssuesById", id);
    };

    this.DeleteIssues = function (id) {
        return GenericService.Delete("/Issues/DeleteIssues", id);
    };

    //IssuesCategories
    this.GetAllIssuesCategories = function () {
        return GenericService.GetAll("/Issues/GetAllIssuesCategories");
    };
    this.SaveIssuesCategories = function (model) {
        return GenericService.Post("/Issues/SaveIssuesCategories", model);
    };

    this.GetIssuesCategoriesById = function (id) {
        return GenericService.GetByID("/Issues/GetIssuesCategoriesById", id);
    };

    this.DeleteIssuesCategories = function (id) {
        return GenericService.Delete("/Issues/DeleteIssuesCategories", id);
    };

}]);