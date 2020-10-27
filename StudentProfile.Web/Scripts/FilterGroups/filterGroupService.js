app.service("filterGroupService", function ($http) {

    this.getfilterGroups = function () {

        return $http({
            method: "Get",
            url: "/Config/getfilterGroups"
        })
    },
    this.getUsers = function () {
        return $http({
            method: "Get",
            url: "/Config/getUsers",
           
        })

    }


});