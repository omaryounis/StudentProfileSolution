
    app.controller("advanceReceive", function ($scope, $http, Service) {
        debugger
        // Definitions
        var Types = [
            { "ID": 1, "Name": "نقدي" },
            { "ID": 2, "Name": "خصم من المكافئات" }
        ];
        //$.getJSON("/Advances/AdvenceFilters", function (data) {
        //    debugger;
        //    $scope.filters = data.data;
        //    console.log("filters", data);
        //});
        $http({
            method: "GET",
            url: "/Advances/AdvenceFilters"
        }).then(function (data) {
            debugger;
            $scope.filters = data.data;

        });
        // CONTROLS
        $scope.dateBox = {
                type: "date",
                showClearButton: true,
                value: new Date()
        };// end date
        
        $scope.ShowAdvance = {
            type: "success",
            text: "عرض",
            onClick: function (e) {
                DevExpress.ui.notify("The Done button was clicked");
            }
        };// end : show btn

        $scope.selectBox = {
            advanceName: {},
            advanceNumber: {},
            payWay: {
                dataSource: Types,
                displayExpr: "Name",
                valueExpr: "ID",
                showClearButton: true
            }

        };// end: select
    });// end controller

    //Service
    app.service("Service", ["$http", function ($http) {
        this.getReceiptData = function (id) {
            return $http({
                method: 'Get',
                url: '/Advances/PaidDocPrintDataSource?DocMasterIds=' + id
            });
        };// end function

    }]);// End service