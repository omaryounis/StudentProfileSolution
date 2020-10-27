app.service("HousingOfStudentsSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    this.GetAllPermssions = function () {
        debugger;
        var response =  $http({
            method: "POST",
            url: "/Housing/GetPermissions",
            data: { screenId: 70 }
        });
        return response;
    };

    this.GetHousingStudent = function (id) {
        var response = $http({
            method: "GET",
            url: "/Housing/GetHousingStudent",
            params: { StudentId: id }
        });
        return response;
    };


    this.GetStudentHousingFurniture = function (id) {
        var response = $http({
            method: "GET",
            url: "/Housing/GetStudentHousingFurniture",
            params: { StudentId: id }
        });
        return response;
    };


    this.SaveStudentHousing = function (StudentId, HousingDate, HousingNotes, RoomId, BedLocationId) {
        debugger;
        var response = $http({
            method: "Post",
            url: "/Housing/SaveStudentHousing",
            data: {
                StudentId: StudentId,
                HousingDate: HousingDate,
                HousingNotes: HousingNotes,
                LocationId: RoomId,
                BedLocationId: BedLocationId
            }
        });
        return response;
    };


    this.SaveStudentHousingTransfer = function (StudentId, HousingDate_Trans, HousingNotes_Trans, LocationId_Trans ,BedLocationId_Trans) {
        var response = $http({
            method: "Post",
            url: "/Housing/SaveStudentHousingTransfer",
            data: {
                StudentId: StudentId,
                HousingDate_Trans: HousingDate_Trans,
                HousingNotes_Trans: HousingNotes_Trans,
                LocationId_Trans: LocationId_Trans,
                BedLocationId_Trans: BedLocationId_Trans
            }
        });
        return response;
    };


    this.SaveHousingFurniture = function (StudentId, BedLocationId, DataSourceHousingFurnitureGrid) {
        var response = $http({
            method: "Post",
            url: "/Housing/SaveHousingFurniture",
            data: {
                StudentId: StudentId,
                BedLocationId: BedLocationId,
                HousingFurnitureModel: DataSourceHousingFurnitureGrid
            }
        });
        return response;
    };


    this.SaveHousingFurnitureReturn = function (StudentId, HousingFurnituresOfStudent_Id, HousingDate, HousingNotes) {
        var response = $http({
            method: "Post",
            url: "/Housing/SaveHousingFurnitureReturn",
            data: {
                StudentId: StudentId,
                HousingFurnituresOfStudent_Id: HousingFurnituresOfStudent_Id,
                HousingReturnDate: HousingDate,
                HousingReturnNotes: HousingNotes
            }
        });
        return response;
    };


    this.ToHijriDate = function (HousingDate) {
        if (!Number.isNaN(new Date(HousingDate).getTime())) {
            debugger;
            var response = $http({
                method: "GET",
                url: "/Housing/ToHijriDate",
                params: { GregorianDate: HousingDate }
            });
            return response;
        } else {
            return '';
        }
    };


    this.GetStudentDataByID = function (StudentId) {
        var response = $http({
            method: "GET",
            url: "/Housing/GetStudentDataByID/",
            params: {
                studentId: StudentId
            }
        });
        return response;
    };


    this.SaveHousingReplacement = function (ReplacementDate, FirstStudentId, SecondStudentId, ReplacementNotes_FirstSt, ReplacementNotes_SecondSt) {
        var response = $http({
            method: "Post",
            url: "/Housing/SaveHousingReplacement",
            data: {
                HousingReplacementDate: ReplacementDate,
                First_StudentId: FirstStudentId,
                Second_StudentId: SecondStudentId,
                First_HousingReplacementNotes: ReplacementNotes_FirstSt,
                Second_HousingReplacementNotes: ReplacementNotes_SecondSt
            }
        });
        return response;
    };

    this.GetNextHousingNo = function (StudentId, OperationTypeId) {
        var response = $http({
            method: "GET",
            url: "/Housing/GetNextHousingNo",
            params: {
                studentId: StudentId,
                operationTypeId: OperationTypeId
            }
        });
        return response;
    };

    this.CheckStudentHousingFurnitures = function (id) {
        return GenericService.GetByID("/Housing/CheckStudentHousingFurnitures", id);
    };

    this.SaveClearanceOfSudents = function (model) {
        return GenericService.Post("/Housing/SaveClearanceOfSudents", model);
    };

}]);

