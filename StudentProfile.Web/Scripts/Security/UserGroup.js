(function() {
    app.controller("UserGroupCtrl",
        [
            "$scope", "$http", "$timeout", function($scope, $http, $timeout) {

                $scope.GroupId = null;
                $scope.UserId = null;

                $scope.groups = [];
                $scope.Group = "";

                $scope.users = [];
                $scope.User = "";

                $scope.GroupUsers = [];
                $scope.disableSave = true;

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

                        onSelectionChanged: function(e) {
                            if (e.selectedItem.ID !== null) {
                                $scope.GroupId = e.selectedItem.ID;
                            }
                        },
                        onItemClick: function() {
                            $scope.Refresh();
                            //$scope.GetUsersNotInSelectedGroup();
                        }
                    },
                    Users: {
                        bindingOptions: {
                            items: "users",
                            value: "User"
                        },
                        displayExpr: "Username",
                        valueExpr: "ID",
                        placeholder: "اختر المستخدم",
                        noDataText: "لا يوجد بيانات",
                        searchEnabled: true,
                        rtlEnabled: true,

                        onSelectionChanged: function(e) {
                            if (e.selectedItem !== null) {
                                if (e.selectedItem.ID !== null) {
                                    $scope.UserId = e.selectedItem.ID;
                                    $scope.disableSave = false;
                                }
                            }
                        }
                    }
                };


                $scope.UserValidationRules = {
                    validationRules: [
                        {
                            type: "required",
                            message: "من فضلك اختر المستخدم"
                        }
                    ]
                };

                $scope.GroupValidationRules = {
                    validationRules: [
                        {
                            type: "required",
                            message: "من فضلك اختر المجموعة"
                        }
                    ]
                };

                $scope.SaveButton = {
                    bindingOptions: {
                        disabled: "disableSave"
                    },
                    useSubmitBehavior: true,
                    text: "حفظ",
                    type: "success"
                };


                $http({
                    method: "Get",
                    url: "/Security/GetGroups"
                }).then(function(data) {
                    $scope.groups = data.data;
                });
                $scope.GetUsersNotInSelectedGroup = function() {
                    debugger;
                    $http({
                        method: "POST",
                        url: "/Security/GetUsersNotInSelectedGroup",
                        data: { groupId: $scope.GroupId }
                    }).then(function(data) {
                        $scope.users = data.data;
                    });
                };

                $scope.Refresh = function() {
                    $http({
                        method: "POST",
                        url: "/Security/GetGroupUsers",
                        data: { groupId: $scope.GroupId }
                    }).then(function(data) {
                        $scope.GroupUsers = data.data;
                    });

                    $http({
                        method: "POST",
                        url: "/Security/GetUsersNotInSelectedGroup",
                        data: { groupId: $scope.GroupId }
                    }).then(function(data) {
                        $scope.users = data.data;
                    });

                    $scope.UserGroup.Users.value = "undefined";
                };

                $scope.Permissions = {
                    Create: false,
                    Read: false,
                    Update: false,
                    Delete: false,
                    View: false
                };

                $scope.GetPermssions = function() {
                    $http({
                        method: "POST",
                        url: "/Security/GetPermissions",
                        params: { screenId: 15 }
                    }).then(function(data) {
                        debugger;
                        $scope.Permissions.Create = data.data.Create;
                        $scope.Permissions.Read = data.data.Read;
                        $scope.Permissions.Update = data.data.Update;
                        $scope.Permissions.Delete = data.data.Delete;
                        $scope.Permissions.View = data.data.View;
                    });
                };
                $scope.GetPermssions();

                $scope.dataGridOptions = {
                    bindingOptions: {
                        dataSource: "GroupUsers",
                        //"editing.editingEnabled": "Permissions.Update",
                        //"editing.allowUpdating": "Permissions.Update",
                        //"editing.allowDeleting": "Permissions.Delete"
                    },
                    columns: [
                        {
                            dataField: "Name",
                            caption: "اسم المستخدم"
                        }
                    ],
                    selection: {
                        mode: "single"
                    },
                    showRowLines: true,
                    columnAutoWidth: true,
                    scrolling: {
                        rtlEnabled: true,
                        useNative: true,
                        scrollByContent: true,
                        scrollByThumb: true,
                        showScrollbar: "onHover",
                        mode: "standard", // or "virtual"
                        direction: "both"
                    },
                    rtlEnabled: true,
                    paging: {
                        enabled: true,
                        pageSize: 10
                    },
                    pager: {
                        showPageSizeSelector: true,
                        allowedPageSizes: [5, 10, 20],
                        showInfo: true
                    },
                    showBorders: true,
                    onCellPrepared: function(e) {
                        if (e.rowType === "data" && e.column.command === "edit") {
                            var isEditing = e.row.isEditing,
                                $links = e.cellElement.find(".dx-link");
                            $links.text("");
                            if (isEditing) {
                                $links.filter(".dx-link-save").addClass("dx-icon-save");
                                $links.filter(".dx-link-cancel").addClass("dx-icon-revert");
                            } else {
                                $links.filter(".dx-link-edit").addClass("btn btn-dark-orange btn-sm fa fa-pencil");
                                $links.filter(".dx-link-delete").addClass("btn btn-danger btn-sm fa fa-trash-o");
                            }
                        }
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
                    noDataText: "لا يوجد مستخدمين لهذه المجموعة"
                };


                $scope.SaveUserToGroup = function () {

                    if ($scope.GroupId == null) {
                        DevExpress.ui.dialog.alert("عفوا ادخل اسم المجموعة", "خطأ");
                        return;
                    }
                    if ($scope.UserId == null) {
                        DevExpress.ui.dialog.alert("عفوا ادخل اسم المستخدم", "خطأ");
                        return;
                    }


                    $http({
                        method: "Get",
                        url: "/Security/CheckUserGroupBeforeSave/",
                        params: { groupId: $scope.GroupId, userId: $scope.UserId }

                    }).then(function(data) {
                        if (data.data === "عفوا نفس المستخدم مسجل من قبل على نفس المجموعة") {
                            DevExpress.ui.dialog.alert(data.data, "خطأ");

                        } else {
                     
                   
                            var resultt = DevExpress.ui.dialog.confirm(data.data + " . " + "هل تريد الاستمرار ؟", "تأكيد");

                            resultt.done(function (dialogResult) {
                                debugger;
                                if (dialogResult) {


                                    $http({
                                        method: "POST",
                                        url: "/Security/SaveUserGroup/",
                                        data: { groupId: $scope.GroupId, userId: $scope.UserId }
                                    }).then(function (data) {
                                        if (data.data === "") {
                                            DevExpress.ui.notify({ message: "تم الحفظ بنجاح" }, "success", 3000);
                                            $scope.disableSave = true;
                                            $scope.GetUsersNotInSelectedGroup();
                                            $scope.Refresh();
                                        } else if (data.data !== "") {
                                            DevExpress.ui.notify({ message: data.data }, "error", 3000);
                                        }
                                    });

                                }
                            });

                        }





                    });












   
                
                };


            }
        ]);
})();