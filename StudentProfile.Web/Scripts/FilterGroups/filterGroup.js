app.controller("filterGroupsCtrl",
    function($scope, $http, $location, $timeout, filterGroupService) {
        $scope.groupsArray = [];
        $scope.UsersArray = [];
        $scope.fillUsersArray = function() {
            filterGroupService.getUsers().then(function(data) {
                $scope.UsersArray = data.data;
                console.log($scope.UsersArray);
            });
        };
        //$scope.fillGroupsArray();
        $scope.fillUsersArray();
        $scope.GroupsGridArray = [];
        var users = [];
        $scope.groupFilterNameSelectedItems = {
            dataSource: new DevExpress.data.DataSource({
                paginate: true,
                casheRowData: true,
                loadMode: "raw",
                load: function() {
                    var d = $.Deferred();
                    $.getJSON("/Config/getfilterGroups").done(function(result) {
                        debugger;
                        d.resolve(result);
                    });
                    return d.promise();
                }
            }),
            //bindingOptions: { value: "GroupID" },
            valueExpr: "GroupID",
            displayExpr: "GroupName",
            onValueChanged: function(e) {
                if (e.value != undefined) {
                    $scope.GroupID = e.value;
                    $scope.GroupName = e.component.option('text');
                }
            },
            onIntialized: function(e) {
                $scope.groupFilterNameSelectedItems = e.component.getDataSource();
            }
        };
        $scope.UsersTagBox = {
            bindingOptions: {
                items: "UsersArray",
                value: "userID",
            },
            valueExpr: "userID",
            displayExpr: "userName",
            onValueChanged: function(e) {
                if (e.value != undefined) {
                    $scope.userID = e.value;
                    $scope.userName = e.component.option("text");
                    users.push({ GroupID: $scope.GroupID, userID: $scope.userID, userName: $scope.userName });
                }
            }
        };
        $scope.filterGroupsSaveBtn = {
            text: "save",
            onClick: function() {
                //$scope.GroupsGridArray.push({ filtergroupID: $scope.GroupID, userID: $scope.userID })
                var groupGrid = $("#filterGroupGridContainer").dxDataGrid("instance");
                groupGrid.option("dataSource", $scope.GroupsGridArray);
            }
        };
        $scope.filterGroupGridOptions = {
            bindingOptions: {
                dataSource: "",
            },
            columns: [
                {
                    dataField: "",
                    visible: false
                },
                {
                    dataField: "",
                },
            ],
        };
    })