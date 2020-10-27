app.service('RewardService', ['GenericService', function (GenericService) {

    var ApiURL = '';

    this.GetAllSchools = function () {
        return GenericService.GetAll(ApiURL + "Api/StudentRegister/GetSchools");
    };

    this.GetAllNationalities = function () {
        return GenericService.GetAll(ApiURL + "Api/School/GetNationalities");
    };

    this.GetAllStudentsByFatherId = function (FatherId) {
        return GenericService.Post(ApiURL + "Api/StudentRegister/GetAllStudentsByFatherId", FatherId);
    };

    this.AddFather = function (Obj) {
        debugger;
        return GenericService.Post(ApiURL + "Api/StudentRegister/AddFather", Obj);
    };

}]);
