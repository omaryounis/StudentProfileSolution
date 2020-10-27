(function () {

    var GenericService = function ($rootScope, $http) {

        $rootScope.actionUrl = "";
        var actionUrl = $rootScope.actionUrl;

        function GetAll(Url){
            return $http({
                method: "GET",
                url: actionUrl + Url,
                contentType: "application/json;"
            }).then(function (response) {
                return response;
            }, function (err) {
                return err;
            });
        }

        function GetByID(Url ,ID) {
            return $http({
                method: "GET",
                url: actionUrl + Url + "/" + ID,
                contentType: "application/json;" 
                //header: { },
            }).then(function (response) {
                return response;
            }, function (err) {
                return err;
            });
        }

        function GetByQuery(Url ,Query) {
            return $http({
                method: "GET",
                url: actionUrl + Url,
                contentType: "application/json;"
            }).then(function (response) {
                return response;
            }, function (err) {
                return err;
            });
        }


        function GetByModel(Url, model) {
            return $http({
                method: "GET",
                url: actionUrl + Url,
                params: model,
                contentType: "application/json;"
                //header: { },
            }).then(function (response) {
                return response;
            }, function (err) {
                return err;
            });
        }

        function GetAllPages(Url, PageSize , PageNumber , data) {
            return $http({
                method: "GET",
                url: actionUrl + Url + "/" + PageSize + "/" + PageNumber,
                params: data,
                contentType: "application/json;" 
                //header: { },
            }).then(function (response) {
                return response;
            }, function (err) {
                return err;
            });
        }

        function Post(Url, Data) {
            return $http({
                method: "POST",
                url: actionUrl + Url,
                data:JSON.stringify(Data),
                contentType: "application/json;" 
                //header: { },
            }).then(function (response) {
                return response;
            }, function (err) {
                return err;
            });
        }


        function Delete(Url, ID) {
            return $http({
                method: "DELETE",
                url: actionUrl + Url + "/" + ID,
                header: { "Content-Type": "application/json;" },
            }).then(function (response) {
                return response;
            }, function (err) {
                return err;
            });
        }

        return {
            GetAll: GetAll,
            GetByID: GetByID,
            GetByQuery: GetByQuery,
            GetByModel: GetByModel,
            GetAllPages: GetAllPages,
            Post: Post,
            Delete: Delete
        }

    };

    var module = angular.module("app");
    module.factory("GenericService", ["$rootScope", "$http", GenericService])

}());
