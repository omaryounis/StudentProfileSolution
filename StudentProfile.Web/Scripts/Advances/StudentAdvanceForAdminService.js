app.service("StudentAdvanceForAdminSrvc", ['$http','GenericService', function ($http, GenericService) {


    this.GetStudents = function () {
        return GenericService.GetAll("/AdvancesForAdmin/GetStudents");
    };

    this.SaveAdvanceRequestByAdmin = function (advanceTypeId, advanceValue,advanceRequestNotes , studentId) {
        return $http({
            method: "Post",
            url: "/AdvancesForAdmin/SaveAdvanceRequestByAdmin",
            data: {
                advanceTypeId: advanceTypeId,
                advanceValue: advanceValue,
                advanceRequestNotes: advanceRequestNotes,
                studentId: studentId
            }
        });
    };



}]);

