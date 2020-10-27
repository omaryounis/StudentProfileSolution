app.service("GraduateStudiesSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetFaculties = function () {
        return GenericService.GetAll("/GraduateStudies/GetFaculties");
    };

    this.GetDegrees = function () {
        return GenericService.GetAll("/GraduateStudies/GetDegrees");
    };

    this.GetStudentsForGraduateStudies = function (model) {
        return GenericService.Post("/GraduateStudies/GetStudentsForGraduateStudies", model);
    };

    this.GetStudentWarning = function (id) {
        return GenericService.GetByID("/GraduateStudies/GetStudentWarning", id);
    };


    this.GetStudentsDetailMessageDetailForGraduateStudies = function (model) {
        return GenericService.Post("/GraduateStudies/GetStudentsDetailMessageDetailForGraduateStudies", model);
    };

    this.UpdateStudentsData = function () {
        return GenericService.GetAll("/GraduateStudies/UpdateStudentsData");
    };

}]);