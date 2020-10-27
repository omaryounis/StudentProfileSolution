app.service("NewAcademicActivitiesSrvc", ['$http', function ($http) {



    this.RefuseActivityRequest = function (ActivityRequestId, recommendations) {
        return $http({
            method: "Post",
            url: "/AcademicActivities/RejectNewActivityRequest",
            data: {
                activityRequestId: ActivityRequestId,
                recommendations: recommendations
            }
        });
    };

    this.ApproveActivityRequest = function (ActivityRequestId, recommendations) {
        return $http({
            method: "Post",
            url: "/AcademicActivities/AcceptActivityRequest",
            data: {
                activityRequestId: ActivityRequestId,
                recommendations: recommendations
            }
        });
    };
    

}]);

