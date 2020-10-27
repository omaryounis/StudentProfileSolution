app.service("ViolationSrvc", ["$http", function ($http) {
    this.GetUsers = function () {
        return $http({
            method: "GET",
            url: "/Housing/GetUsers"
        });
    };

    this.GetCategories = function () {
        return $http({
            method: "Get",
            url: "/Housing/GetCategories"
        });
    };


    this.getcategoriesPermissionsForUser = function (userId) {

        var dataToSend = JSON.stringify({
            "userId": userId
        });

        return $http({
            
            method: "Get",
            url: "/Housing/GetCategoriesPermissionsForUser",
            params: { userId: Number(userId) }
        });
    };


    this.getAllUsersCategoriesPermissions = function () {

        return $http({
            method: "Get",
            url: "/Housing/GetAllUsersCategoriesPermissions"
        });
    };

    this.AddViolations = function (userId, selectedItems) {
        var jsonString = JSON.stringify({
            'UserId': userId,
            'CategoryIds': selectedItems
        });
        return $http({
            dataType: 'json',
            method: 'POST',
            contentType: "application/ json; charset=utf-8",
            url: "/Housing/AddViolation",
            data: jsonString
        });
    };

    this.DeleteCategoriesPermissionForUser = function (userId, categoryId) {
        var jsonString = JSON.stringify({
            'userId': userId,
            'categoryId': categoryId
        });
        return $http({
            dataType: 'json',
            method: 'Delete',
            contentType: "application/ json; charset=utf-8",
            url: "/Housing/DeleteCategoriesPermissionForUser",
            data: jsonString
        });
    };

}]);