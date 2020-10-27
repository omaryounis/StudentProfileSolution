app.service("ViolationsDecisionsTakingSrvc", ['$http', 'GenericService', function ($http, GenericService) {


    this.GetHangedViolationsForDecisionTaking = function () {
        return $http({
            method: "GET",
            url: "/ViolationsDecisions/GetHangedViolationsForDecisionTaking"
        });

    };


    this.RefuseViolation = function (id, recommendations) {
        return $http({
            method: "POST",
            url: "/ViolationsDecisions/RefuseViolation",
            data: {
                 'id':id ,  'recommendations':recommendations
            }
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


   /*************************************** Forword Violation ***************************************/

    this.GetUsersToForword = function () {
      return GenericService.GetAll("/ViolationsDecisions/GetUsersToForword");
    };

    this.GetUserForForwordByViolationId = function (id) {
        return GenericService.GetByID("/ViolationsDecisions/GetUserForForwordByViolationId", id);
    };

    this.ForwordViolationToUser = function (model) {
        return GenericService.Post("/ViolationsDecisions/ForwordViolationToUser", model);
    };

}]);