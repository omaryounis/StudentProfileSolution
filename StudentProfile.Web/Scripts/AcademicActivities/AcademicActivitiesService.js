app.service("AcademicActivitiesSrvc", ['$http', function ($http) {

    this.SaveAcademicActivity = function (Degree, Duration, Location, Type, StartDate, EndDate, Name, Ratio) {
        debugger;
        return $http({
            method: "Post",
            url: "/AcademicActivities/SaveAcademicActivity",
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

    this.EditActivityRequest = function (ActivityRequestId, Degree, Duration, Location, Type, StartDate, EndDate, Name, Ratio) {
        return $http({
            method: "Post",
            url: "/AcademicActivities/EditActivityRequest",
            data: {
                ActivityRequestId: ActivityRequestId,
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

    this.GetActivityRequestById = function (ActivityRequestId) {
        return $http({
            method: "Get",
            url: "/AcademicActivities/GetActivityRequestById",
            params: {
                ActivityRequestId: ActivityRequestId
            }
        });
    };

    this.SaveActivityPhases = function (ActivityPhaseId, ActivityPhaseName, PhaseOrder) {
        debugger;
        return $http({
            method: "Post",
            url: "/AcademicActivities/SaveActivityPhase",
            data: {
                ActivityPhaseId: ActivityPhaseId,
                ActivityPhaseName: ActivityPhaseName,
                phaseOrder: PhaseOrder
            }
        });
    };

    this.GetActivityPhaseById = function (ActivityPhaseId) {
        return $http({
            method: "Get",
            url: "/AcademicActivities/GetActivityPhaseById",
            params: {
                ActivityPhaseId: ActivityPhaseId
            }
        });
    };

    this.SaveActivityUser = function (ActivityUserId, ActivityPhaseId, UserId) {
        return $http({
            method: "Post",
            url: "/AcademicActivities/SaveActivityUser",
            data: {
                ActivityUserId: ActivityUserId,
                ActivityPhaseId: ActivityPhaseId,
                userId: UserId
            }
        });
    };

    this.GetActivityUser = function (ActivityUserId) {
        debugger;
        return $http({
            method: "GET",
            url: "/AcademicActivities/GetActivityUser",
            params: {
                ActivityUserId: ActivityUserId
            }
        });
    };

    this.EditActivityUserStatus = function (ActivityUserId, Status) {
        debugger;
        return $http({
            method: "Post",
            url: "/AcademicActivities/EditActivityUserStatus",
            data: {
                ActivityUserId: ActivityUserId,
                status: Status
            }
        });
    };

    this.ConfirmActivityRequest = function (ActivityRequestId, RecommendationsNotes, Type) {
        debugger;
        return $http({
            method: "Post",
            url: "/AcademicActivities/ConfirmActivityRequest",
            data: {
                type: Type,
                ActivityRequestId: ActivityRequestId,
                recommendationsNotes: RecommendationsNotes
            }
        });
    };

 
    //مش تعدل Controller
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

    this.SaveActivityArchiveByAdmin = function (StudentId, Degree, Duration, Location, Type, StartDate, EndDate, Name, Ratio) {
        debugger;
        return $http({
            method: "Post",
            url: "/AcademicActivities/SaveActivityArchiveByAdmin",
            data: {
                StudentId: StudentId,
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

    this.SaveActivityArchiveByStudent = function (Degree, Duration, Location, Type, StartDate, EndDate, Name, Ratio) {
        debugger;
        return $http({
            method: "Post",
            url: "/AcademicActivities/SaveActivityArchiveByStudent",
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


    //this.GetActivityRequestById = function (ActivityRequestId) {
    //    return $http({
    //        method: "Get",
    //        url: "/AcademicActivities/GetActivityRequestById",
    //        params: {
    //            activityRequestId: ActivityRequestId
    //        }
    //    });
    //};

    //this.SaveActivityPhases = function (ActivityPhaseId, ActivityPhaseName, PhaseOrder) {
    //    debugger;
    //    return $http({
    //        method: "Post",
    //        url: "/AcademicActivities/SaveActivityPhase",
    //        data: {
    //            activityPhaseId: ActivityPhaseId,
    //            activityPhaseName: ActivityPhaseName,
    //            phaseOrder: PhaseOrder
    //        }
    //    });
    //};

    //this.GetActivityPhaseById = function (ActivityPhaseId) {
    //    return $http({
    //        method: "Get",
    //        url: "/AcademicActivities/GetActivityPhaseById",
    //        params: {
    //            activityPhaseId: ActivityPhaseId
    //        }
    //    });
    //};

    //this.SaveActivityUser = function (ActivityUserId, ActivityPhaseId, UserId) {
    //    debugger;
    //    return $http({
    //        method: "Post",
    //        url: "/AcademicActivities/SaveActivityUser",
    //        data: {
    //            activityUserId: ActivityUserId,
    //            activityPhaseId: ActivityPhaseId,
    //            userId: UserId
    //        }
    //    });
    //};

    //this.GetActivityUser = function (ActivityUserId) {
    //    debugger;
    //    return $http({
    //        method: "GET",
    //        url: "/AcademicActivities/GetActivityUser",
    //        params: {
    //            activityUserId: ActivityUserId
    //        }
    //    });
    //};

    //this.EditActivityUserStatus = function (ActivityUserId, Status) {
    //    debugger;
    //    return $http({
    //        method: "Post",
    //        url: "/AcademicActivities/EditActivityUserStatus",
    //        data: {
    //            activityUserId: ActivityUserId,
    //            status: Status
    //        }
    //    });
    //};

    this.RefuseActivityRequest = function (ActivityRequestId, recommendationsOfRefuse) {
        return $http({
            method: "Post",
            url: "/AcademicActivities/RefuseActivityRequest",
            data: {
                activityRequestId: ActivityRequestId,
                recommendationsOfRefuse: recommendationsOfRefuse
            }
        });
    };

    this.ApproveActivityRequest = function (ActivityRequestId, recommendationsOfAccept) {
        return $http({
            method: "Post",
            url: "/AcademicActivities/ApproveActivityRequest",
            data: {
                activityRequestId: ActivityRequestId,
                recommendationsOfAccept: recommendationsOfAccept
            }
        });
    };

}]);

