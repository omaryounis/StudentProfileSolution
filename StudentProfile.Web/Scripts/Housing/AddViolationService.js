app.service("AddViolatioSrvc", ['$http', function ($http) {

    this.GetStudentForHousingViolations = function () {
        return  $http({
            method: "GET",
            url: "/Housing/GetStudentForHousingViolations"
        });
       
    };


    this.GetIssuesCategories = function() {
        return $http({
            method: "Get",
            url: "/Housing/GetIssuesCategories"
        });
    };


    this.GetIssuesByCategoryId = function (categoryId) {
        return $http({
            method: "Get",
            url: "/Housing/GetIssuesByCategoryId",
            params: { categoryId : categoryId}
        });
    };


    
    this.GetIssueNumber = function (issueId) {
        return $http({
            method: "Get",
            url: "/Housing/GetIssueNumber",
            params: { issueId: issueId}
        });
    };

    this.SaveViolation = function(studentId, issueId, violationDate, violationDescription) {
        return $http({
            method: "POST",
            url: "/ViolationsDecisions/SaveViolation",
            data: {
                studentId: studentId,
                issueId: issueId,
                violationDate: violationDate,
                violationDescription: violationDescription
            }
        });
    };


}]);