app.service("StudentsStudiesSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetFaculties = function () {
        return GenericService.GetAll("/StudentsStudies/GetFaculties");
    };

    this.GetDegrees = function () {
        return GenericService.GetAll("/StudentsStudies/GetDegrees");
    };

    this.GetMajors = function () {
        return GenericService.GetAll("/StudentsStudies/GetMajors");
    };

    this.GetStatus = function () {
        return GenericService.GetAll("/StudentsStudies/GetStatus");
    };
    this.GetStudies = function () {
        return GenericService.GetAll("/StudentsStudies/GetStudies");
    };

    this.GetDepartments = function () {
        return GenericService.GetAll("/StudentsStudies/GetDepartments");
    };

    this.GetStudentsForStudentsStudies = function (model) {
        return GenericService.Post("/StudentsStudies/GetStudentsForStudentsStudies", model);
    };

    this.GetStudentWarning = function (id) {
        return GenericService.GetByID("/StudentsStudies/GetStudentWarning", id);
    };


    this.GetStudentsDetailMessageDetailForStudentsStudies = function (model) {
        return GenericService.Post("/StudentsStudies/GetStudentsDetailMessageDetailForStudentsStudies", model);
    };

    this.UpdateStudentsData = function () {
        return GenericService.GetAll("/StudentsStudies/UpdateStudentsData");
    };

}]);