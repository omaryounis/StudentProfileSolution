(function () {
    app.controller("ScreensPermissionsCtrl",
        [
            "$scope", "$http", function ($scope, $http) {
                $scope.GroupId = null;
                $scope.groups = [];
                //$scope.screenActions = [];


                $scope.Group = "";
                $scope.GroupUsers = [];
                $scope.disableSave = true;
                $scope.selectedItems = [];
                $scope.UserGroup = {
                    Groups: {
                        bindingOptions: {
                            dataSource: "groups",
                            items: "groups",
                            value: "Group"
                        },
                        displayExpr: "Name",
                        valueExpr: "ID",
                        placeholder: "اختر المجموعة",
                        noDataText: "لا يوجد بيانات",
                        searchEnabled: true,
                        rtlEnabled: true,
                        onSelectionChanged: function (e) {
                            if (e.selectedItem.ID !== null) {
                                debugger;
                                $scope.GroupId = e.selectedItem.ID;
                            }
                        },
                        onItemClick: function () {

                            $http({
                                method: "GET",
                                url: "/Security/GetGroupScreenActionIds/",
                                params: { grpId: $scope.GroupId }
                            }).then(function (data) {
                                debugger;
                                $scope.screenActionIds = data.data;
                            });


                        }
                    }
                };
                $scope.GroupValidationRules = {
                    validationRules: [
                        {
                            type: "required",
                            message: "حقل اجبارى"
                        }
                    ]
                };
                $scope.SaveButton = {
                    bindingOptions: {
                        disabled: "disableSave"
                    },
                    useSubmitBehavior: true,
                    text: "حفظ",
                    hint: "حفظ",
                    icon: "save",
                    type: "success"
                };
                $http({
                    method: "Get",
                    url: "/Security/GetGroups"
                }).then(function (data) {
                    $scope.groups = data.data;
                });

                $scope.Refresh = function () {
                    debugger;
                    $http({
                        method: "POST",
                        url: "/Security/GetGroupUsers",
                        data: { groupId: $scope.GroupId }
                    }).then(function (data) {
                        $scope.GroupUsers = data.data;
                    });
                };
                if ($scope.GroupId !== null) {
                    $scope.Refresh();
                }
                $scope.grouping = {
                    autoExpandAll: false
                };
                $scope.checkBoxOptions = {
                    text: "اظهار الكل",
                    bindingOptions: {
                        value: "grouping.autoExpandAll"
                    }
                };

                 


                $scope.dataGridOptions = {
                    dataSource: new DevExpress.data.DataSource({
                        loadMode: "raw",
                        //sort: "order",
                        key: "ScreenActionId",
                        load: function () {
                            return $.getJSON("/Security/GetScreenActions",
                                function (data) {
                                    debugger;
                                    $scope.screenActions = data;
                                });
                        }
                    }),
                    bindingOptions: {
                        selectedRowKeys: "screenActionIds",
                        grouping: "grouping"
                    },
                    cacheEnabled: false,
                    keyExpr: "ScreenActionId",
                    columns: [
                        {
                            dataField: "ParentId",
                            caption: "اسم الشاشة",
                            groupIndex: 0,
                            allowSorting: false,
                            allowEditing: false,
                            showWhenGrouped: true,
                            allowFiltering: true,
                            customizeText: function (e) {
                                debugger;
                                var item = $scope.screenActions.find(x => x.ParentId === e.value);
                                return item.ScreenName;
                            },
                        },
                        //{
                        //    dataField: "ScreenId",
                        //    caption: "اسم الشاشة",
                        //    groupIndex: 1,
                        //    allowSorting: false,
                        //    allowEditing: false,
                        //    showWhenGrouped: true,
                        //    allowFiltering: true,
                        //    customizeText: function (e) {
                        //        debugger;
                        //        var item = $scope.screenActions.find(x => x.ScreenId === e.value);
                        //        return item.ScreenName;
                        //    },
                        //},
                        {
                            dataField: "ActionName",
                            caption: "اسم الصلاحية",
                            allowEditing: false,
                            allowFiltering: false

                        }
                    ],
                    grouping: {
                        autoExpandAll: false
                    },
                    groupPanel: {
                        visible: false
                    },
                    selection: {
                        mode: "multiple",
                        showCheckBoxesMode: "always"
                    },
                    searchPanel: {
                        visible: true,
                        placeholder: "بحث"
                    },
                    filterRow: {
                        visible: true,
                        operationDescriptions: {
                            between: "بين",
                            contains: "تحتوى على",
                            endsWith: "تنتهي بـ",
                            equal: "يساوى",
                            greaterThan: "اكبر من",
                            greaterThanOrEqual: "اكبر من او يساوى",
                            lessThan: "اصغر من",
                            lessThanOrEqual: "اصغر من او يساوى",
                            notContains: "لا تحتوى على",
                            notEqual: "لا يساوى",
                            startsWith: "تبدأ بـ"
                        },
                        resetOperationText: "الوضع الافتراضى"
                    },
                    headerFilter: {
                        visible: true
                    },
                    showRowLines: true,
                    scrolling: {
                        rtlEnabled: true,
                        useNative: true,
                        scrollByContent: true,
                        scrollByThumb: true,
                        showScrollbar: "onHover",
                        mode: "standard", // or "virtual"
                        direction: "both"
                    },
                    paging: {
                        enabled: false,
                        pageSize: 10
                    },
                    pager: {
                        showPageSizeSelector: true,
                        allowedPageSizes: [5, 10, 20],
                        showInfo: true
                    },
                    editing: {
                        mode: "cell",
                        allowAdding: false,
                        allowUpdating: true,
                        allowDeleting: false
                    },
                    showBorders: true,
                    showRowLines: true,
                    columnAutoWidth: true,
                    rtlEnabled: true,
                    noDataText: "لا يوجد مستخدمين لهذه المجموعة",
                    onSelectionChanged: function (data) {
                        $scope.selectedItems = data.selectedRowsData;
                        $scope.disableSave = !$scope.selectedItems.length;
                    }
                };


                $scope.onFormSubmit = function (e) {
                    debugger;
                    DevExpress.ui.notify({ message: "جارى الحفظ" }, "success", 3000);
                    e.preventDefault();
                    $scope.screenActionIds = [];
                    var selectedItms = $scope.selectedItems;
                    angular.forEach($scope.selectedItems,
                        function (value, key) {
                            debugger;
                            $scope.screenActionIds.push(value.ScreenActionId);
                        });
                    $http({
                        method: "POST",
                        url: "/Security/SaveScreensPermissions/",
                        data: {
                            groupId: $scope.GroupId,
                            screenActionIds: $scope.screenActionIds
                        }
                    }).then(function (data) {
                        if (data.data === "") {
                            DevExpress.ui.notify({ message: "تم الحفظ بنجاح" }, "success", 3000);
                            $scope.disableSave = true;
                            $scope.Refresh();
                        } else if (data.data !== "") {
                            DevExpress.ui.notify({ message: data.data }, "error", 3000);
                        }
                    });
                };
            }
        ]);
})();