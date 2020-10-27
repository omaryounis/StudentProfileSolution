app.service("ViolationDescisionByUserSrvc", ['$http', 'GenericService', function ($http, GenericService) {


    this.GetViolationsForDecisionByUser = function () {
        return $http({
            method: "GET",
            url: "/ViolationsDecisions/GetViolationsForDecisionByUser"
        });

    };


    this.GetDecisionList = function() {
        return $http({
            method: "GET",
            url: "/ViolationsDecisions/GetDecisionList"
        });
    };


    this.ApproveViolation = function(violationOfStudentId,  decisionId,  recommendations) {
        return $http({
            method: "post",
            url: "/ViolationsDecisions/ApproveViolation",
            data: {
                violationOfStudentId: violationOfStudentId,
                decisionId: decisionId,
                recommendations: recommendations
            }
        });
    };


}]);