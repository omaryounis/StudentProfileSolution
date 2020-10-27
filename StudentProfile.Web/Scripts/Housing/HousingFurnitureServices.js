app.service("HousingFurnituresSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    //HousingFurniture
    this.GetHousingFurnitureCategories = function () {
        return GenericService.GetAll("/Housing/GetHousingFurnitureCategories");
    };

    this.GetHousingFurnitures = function () {
        return GenericService.GetAll("/Housing/GetHousingFurnitures");
    };
    this.SaveHousingFurnitures = function (model) {
        return GenericService.Post("/Housing/SaveHousingFurnitures", model);
    };

    this.GetHousingFurnituresById = function (id) {
        return GenericService.GetByID("/Housing/GetHousingFurnituresById", id);
    };

    this.DeleteHousingFurnitures = function (id) {
        return GenericService.Delete("/Housing/DeleteHousingFurnitures", id);
    };

    //HousingFurnitureCategories
    this.GetAllHousingFurnitureCategories = function () {
        return GenericService.GetAll("/Housing/GetAllHousingFurnitureCategories");
    };
    this.SaveHousingFurnitureCategories = function (model) {
        return GenericService.Post("/Housing/SaveHousingFurnitureCategories", model);
    };

    this.GetHousingFurnitureCategoriesById = function (id) {
        return GenericService.GetByID("/Housing/GetHousingFurnitureCategoriesById", id);
    };

    this.DeleteHousingFurnitureCategories = function (id) {
        return GenericService.Delete("/Housing/DeleteHousingFurnitureCategories", id);
    };



}]);