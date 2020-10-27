app.service("StudentAdvanceSrvc", ['$http', 'GenericService', function ($http, GenericService) {

    //this.GetAllPermssions = function (screenId) {
    //    debugger;
    //    var response = $http({
    //        method: "Get",
    //        url: "/Advances/GetStudentAdvancePhasesPermissions?screenId=" + screenId
    //    });
    //    return response;
    //};// end function


    this.GetStudentDataByID = function (StudentId) {
        debugger;
        var response = $http({
            method: "GET",
            url: "/Advances/GetStudentDataByID/",
            params: {
                studentId: StudentId
            }
        });
        return response;
    };

    this.SaveAcademicAdvance = function (Degree, Duration, Location, Type, StartDate, EndDate, Name, Ratio) {
        debugger;
        return $http({
            method: "Post",
            url: "/Advances/SaveAcademicAdvance",
            data: {
                Degree: Degree,
                Duration: Duration,
                Location: Location,
                Type: Type,
                StartDate: StartDate,
                EndDate: EndDate,
                Name: Name,
                Ratio: Ratio
            }
        });
    };

    this.EditAdvanceRequest = function (AdvanceRequestId, studentId, Degree, Duration, Location, Type, StartDate, EndDate, Name, Ratio) {
        debugger;
        return $http({
            method: "Post",
            url: "/Advances/EditAdvanceRequest",
            data: {
                AdvanceRequestId: AdvanceRequestId,
                studentId: studentId,
                Degree: Degree,
                Duration: Duration,
                Location: Location,
                Type: Type,
                StartDate: StartDate,
                EndDate: EndDate,
                Name: Name,
                Ratio: Ratio
            }
        });
    };

    this.GetAdvanceRequestById = function (AdvanceRequestId) {
        return $http({
            method: "Get",
            url: "/Advances/GetAdvanceRequestById",
            params: {
                AdvanceRequestId: AdvanceRequestId
            }
        });
    };

    this.SaveAdvancePhases = function (AdvancePhaseId, AdvancePhaseName, PhaseOrder, IsFinancialPhase) {
        debugger;
        return $http({
            method: "Post",
            url: "/Advances/SaveAdvancePhase",
            data: {
                AdvancePhaseId: AdvancePhaseId,
                AdvancePhaseName: AdvancePhaseName,
                phaseOrder: PhaseOrder,
                isFinancialPhase: IsFinancialPhase
            }
        });
    };

    this.GetAdvancePhaseById = function (AdvancePhaseId) {
        return $http({
            method: "Get",
            url: "/Advances/GetAdvancePhaseById",
            params: {
                AdvancePhaseId: AdvancePhaseId
            }
        });
    };

    this.SaveAdvanceUser = function (AdvanceUserId, AdvancePhaseId, UserId) {
        debugger;
        return $http({
            method: "Post",
            url: "/Advances/SaveAdvanceUser",
            data: {
                AdvanceUserId: AdvanceUserId,
                AdvancePhaseId: AdvancePhaseId,
                userId: UserId
            }
        });
    };

    this.GetAdvanceUser = function (AdvanceUserId) {
        debugger;
        return $http({
            method: "GET",
            url: "/Advances/GetAdvanceUser",
            params: {
                AdvanceUserId: AdvanceUserId
            }
        });
    };


    this.EditAdvanceUserStatus = function (AdvanceUserId, Status) {
        debugger;
        return $http({
            method: "Post",
            url: "/Advances/EditAdvanceUserStatus",
            data: {
                advanceUserId: AdvanceUserId,
                status: Status
            }
        });
    };

    this.ConfirmAdvanceRequest = function (AdvanceRequestId, RecommendationsNotes, Type, ApprovedValue) {
        debugger;
        return $http({
            method: "Post",
            url: "/Advances/ConfirmAdvanceRequest",
            data: {
                type: Type,
                approvedValue: ApprovedValue,
                advanceRequestId: AdvanceRequestId,
                recommendationsNotes: RecommendationsNotes
            }
        });
    };

    this.DeleteAdvanceRequests = function (id) {
        return GenericService.Delete("/Advances/DeleteAdvanceRequests", id);
    };


    this.GetAdvancesRequestTraking = function (model) {
        return GenericService.Post("/Advances/GetAdvancesRequestTraking", model);
    };

    this.CheckValidationOfAdvance = function (advanceRequestId) {
        debugger;
        return $http({
            method: "GET",
            url: "/Advances/CheckValidationOfAdvance",
            params: {
                advanceRequestId: advanceRequestId
            }
        });
    };
}]);

