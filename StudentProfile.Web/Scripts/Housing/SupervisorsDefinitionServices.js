app.service("SupervisorsDefinitionsSrvc", ['$http', function($http) {

    this.GetUsers = function () {
        return  $http({
            method: "GET",
            url: "/Housing/GetUsers"
        });
    };

    this.SaveSuperVisor = function (userId, fileNumber, telephoneNumber, isPresident, notes, locationIds) {

        var dataToSend = JSON.stringify({
            'UserId': userId,
            'FileNumber': fileNumber,
            'TelephoneNumber': telephoneNumber,
            'IsPresident': isPresident,
            'Notes': notes,
            'LocationIds': locationIds
        });

        return $http({
            method: "Post",
            url: "/Housing/SaveSuperVisor",
            dataType: 'json',
            contentType: "application/ json; charset=utf-8",
            data: dataToSend
        });
    };


    this.getHousingLocations = function () {
        return $http({
            method: "Get",
            url: "/Housing/GetHousingLocations"
        });
    };

    this.GetHousingBuildings = function (locationId) {
        return $http({
            method: "Get",
            url: "/Housing/GetHousingBuildings",
            params: {
                parentId: locationId
            }
        });
    };


    this.GetHousingRooms = function (floorId) {
        return $http({
            method: "Get",
            url: "/Housing/GetHousingRooms",
            params: {
                floorId: floorId
            }
        });
    };

    this.GetSupervisors = function () {
        return $http({
            method: "Get",
            url: "/Housing/GetSupervisors"
        });
    };

  
}]);