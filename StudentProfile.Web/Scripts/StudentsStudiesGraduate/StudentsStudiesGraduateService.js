app.service("StudentsStudiesGraduateSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetFaculties = function () {
        return GenericService.GetAll("/StudentsStudiesGraduate/GetFaculties");
    };

    this.GetDegrees = function () {
        return GenericService.GetAll("/StudentsStudiesGraduate/GetDegrees");
    };

    this.GetMajors = function () {
        return GenericService.GetAll("/StudentsStudiesGraduate/GetMajors");
    };

    this.GetStatus = function () {
        return GenericService.GetAll("/StudentsStudiesGraduate/GetStatus");
    };
    this.GetStudies = function () {
        return GenericService.GetAll("/StudentsStudiesGraduate/GetStudies");
    };

    this.GetDepartments = function () {
        return GenericService.GetAll("/StudentsStudiesGraduate/GetDepartments");
    };

    this.GetStudentsForStudentsStudiesGraduate = function (model) {
        debugger;
        return GenericService.Post("/StudentsStudiesGraduate/GetStudentsForStudentsStudiesGraduate", model);
    };

    this.GetStudentWarning = function (id) {
        return GenericService.GetByID("/StudentsStudiesGraduate/GetStudentWarning", id);
    };


    this.GetStudentsDetailMessageDetailForStudentsStudiesGraduate = function (model) {
        return GenericService.Post("/StudentsStudiesGraduate/GetStudentsDetailMessageDetailForStudentsStudiesGraduate", model);
    };

    this.UpdateStudentsData = function () {
        return GenericService.GetAll("/StudentsStudiesGraduate/UpdateStudentsData");
    };

}]);