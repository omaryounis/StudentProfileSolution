(function () {
    app.controller("ScreensPermissionsCtrl",
        [
            "$scope", "$http", function ($scope, $http) {
                $scope.GroupId = null;
                $scope.groups = [];
                $scope.screens = [];
                $scope.screenActionIdsDetail = [];
                $scope.screenActionIdsDetailOutPut = [];
                //$scope.screenActions = [];


                $scope.Group = "";
                $scope.GroupUsers = [];
                $scope.disableSave = true;
                $scope.disableSearch = true;
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
                            debugger;
                            $scope.GroupId = e.selectedItem.ID;
                            if ($scope.ScreenIds!= null) {
                                debugger;
                                var grid = $('#GroupScreens').dxDataGrid('instance');
                                //grid.option('columns', []);
                                grid.option('dataSource', []);
                            }
                        },
                        onItemClick: function () {

                           


                        }
                    }
                };
 

                $scope.ScreenList = [];
                $scope.ScreenID = null;
                $scope.ScreenIds = null;
                $scope.ScreenSelectBox = {
                    bindingOptions: {
                        value: "ScreenID",
                        items: "ScreenList"
                    },
                    placeholder: "--أختر--",
                    noDataText: "لا يوجد بيانات",
                    displayExpr: "Text",
                    valueExpr: "Value",
                    paginate: true, //Pagenation
                    showBorders: true,
                    searchEnabled: true,
                    showSelectionControls: true,
                    maxDisplayedTags: 2,
                    showMultiTagOnly: false,
                    onInitialized: function (e) {
                        return $.getJSON("/Security/GetScreens", function (data) {
                            debugger;
                            $scope.ScreenList = data;
                        });
                    },
                    onValueChanged: function (e) {
                        debugger;
                        if (e != undefined && e != null && e != '') {
                            if (e.value != null) {
                                if (e.value.length != 0) {
                                    $scope.ScreenIds = e.value.join(',');
                                    if ($scope.GroupId != null) {
                                        debugger;
                                        $scope.disableSearch = false;
                                    } else {
                                        debugger;
                                        $scope.disableSearch = true;
                                    }
                                }
                                else {
                                    debugger;
                                    $scope.disableSearch = true;
                                }
                            }

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

                //validation 
                $scope.validationRequired = {
                    validationRules: [
                        {
                            type: "required",
                            message: "الحقل مطلوب"
                        }
                    ]
                };
                $scope.SaveButton = {
                    bindingOptions: {
                        disabled: "disableSave"
                    },
                    //useSubmitBehavior: true,
                    text: "حفظ",
                    hint: "حفظ",
                    icon: "save",
                    type: "success",
                    onClick: function (e) {

                        debugger;
                        DevExpress.ui.notify({ message: "جارى الحفظ" }, "success", 3000);
                        $scope.screenActionIds = [];
                        var selectedItms = $scope.selectedItems;
                        angular.forEach($scope.selectedItems,
                            function (value, key) {
                                debugger;
                                $scope.screenActionIds.push(value);
                            });
                        $http({
                            method: "POST",
                            url: "/Security/SaveScreensPermissions/",
                            data: {
                                groupId: $scope.GroupId,
                                screenActionIds: $scope.screenActionIds,
                                screenIds: $scope.ScreenIds
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

                    }

                };

                $scope.SearchButton = {
                    bindingOptions: {
                        disabled: "disableSearch"
                    },
                    text: "بحث",
                    hint: "بحث",
                    icon: "search",
                    type: "default",
                    useSubmitBehavior: false,
                    onClick: function (e) {
                         $http({
                                method: "GET",
                                url: "/Security/GetGroupScreenActionIds/",
                                params: { grpId: $scope.GroupId, screens: $scope.ScreenIds}
                            }).then(function (data) {
                                $scope.screenActions = data.data;
                                if ($scope.screenActions.length === 0) {
                                    $http({
                                        method: "GET",
                                        url: "/Security/GetScreenActions/",
                                        params: { screenIds: $scope.ScreenIds }
                                    }).then(function (data) {
                                        debugger;
                                        $scope.screenActions = data.data;
                                        angular.forEach(data.data,
                                            function (value, key) {
                                                debugger;
                                                if (value.id ==null) {
                                                } else {

                                                    $scope.screenActionIdsDetail.push(value.id);
                                                }
                                            });
                                    });
                                } else {
                                    debugger;
                                    angular.forEach(data.data,
                                        function (value, key) {
                                            debugger;
                                            if (value.id == null) {
                                            } else {

                                                $scope.screenActionIdsDetail.push(value.id);
                                            }
                                        });
                                    $scope.screenActions = data.data;
                                    angular.forEach($scope.screenActions,
                                        function (value, key) {
                                            debugger;
                                            if (value.IsExist == true) {
                                                $scope.screenActionIdsDetailOutPut.push(value.id);
                                            } 
                                        });
                                    console.log($scope.screenActionIdsDetailOutPut)
                                } 
                            });
                    }
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
                    bindingOptions: {
                        dataSource: "screenActions",
                        selectedRowKeys: "screenActionIdsDetailOutPut",
                        grouping: "grouping"
                    },
                    cacheEnabled: false,
                    keyExpr: "id",
                    columns: [
                        {
                            dataField: "ScreenId",
                            caption: "اسم الشاشة",
                            groupIndex: 0,
                            visible: false,
                            allowSorting: false,
                            allowEditing: false,
                            showWhenGrouped: true,
                            customizeText: function (e) {
                                var item = $scope.screenActions.find(x => x.ScreenId === e.value);
                                return item.ScreenName;
                            },
                            sortIndex: 0
                        },
                        {
                            dataField: "ActionName",
                            caption: "اسم الصلاحية",
                            allowEditing: false
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
                        showCheckBoxesMode: "always",
                        selectAllMode: "allPages",
                    },
                    searchPanel: {
                        highlightCaseSensitive: false,
                        highlightSearchText: true,
                        placeholder: "بحث",
                        searchVisibleColumnsOnly: true,
                        text: "",
                        visible: true
                    },
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
                        debugger;
                        $scope.selectedItems = [];
                        angular.forEach(data.selectedRowsData,
                        function (value, key) {
                            debugger;
                            $scope.selectedItems.push(value.id);
                        });
                        $scope.disableSave = !$scope.selectedItems.length;
                    },
                    //OnRowPrepared: function (e) {
                    //    debugger;
                    //    //screenActionIdsDetail
                    //    angular.forEach($scope.screenActions,
                    //        function (value, key) {
                    //            debugger;
                    //            if (value.IsExist == true) {
                    //                $scope.screenActionIdsDetailOutPut.push(value.id);
                    //            } else {

                    //                $scope.screenActionIdsDetailOutPut = [];
                    //            }
                    //        });
                       
                    //},
                    onInitialized: function (e) {
                        debugger;
                        //screenActionIdsDetail

                    }
                };




            }
        ]);
})();