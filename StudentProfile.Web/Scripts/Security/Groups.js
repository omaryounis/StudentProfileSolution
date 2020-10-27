(function() {
    app.controller("GroupsCtrl",
        [
            "$scope", "$http", function($scope, $http) {
                $scope.defaultitem = {};
                $scope.GroupId = null;
                $scope.GroupName = "";
                $scope.School = "";
                $scope.IsAdminGroup = true;
                $scope.IsActive = true;


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
                        params: { screenId: 14 } //شاشة المجموعات
                    }).then(function (data) {
                        debugger;
                        $scope.Permissions.Create = data.data.Create;
                        $scope.Permissions.Read = data.data.Read;
                        $scope.Permissions.Update = data.data.Update;
                        $scope.Permissions.Delete = data.data.Delete;
                        $scope.Permissions.View = data.data.View;
                    });
                };
                $scope.GetPermssions();

                $scope.Group = {
                    GroupName:
                    {
                        bindingOptions: {
                            value: "GroupName"
                        },
                        placeholder: "ادخل اسم المجموعة",
                        showClearButton: true,
                        rtlEnabled: true
                    },

                    IsAdminGroup:
                    {
                        bindingOptions: {
                            value: "IsAdminGroup"
                        },
                        text: "مجموعة مديرين",
                        rtlEnabled: true
                    },
                    IsActive:
                    {
                        bindingOptions: {
                            value: "IsActive"
                        },
                        text: "نشطة",
                        rtlEnabled: true
                    }
                };

                $scope.GroupNameValidationRules = {
                    validationRules: [
                        {
                            type: "required",
                            message: "من فضلك ادخل الاسم"
                        },
                        {
                            type: "stringLength",
                            min: 2,
                            message: "الاسم يجب ان يكون حرفان على الاقل"
                        },
                        {
                            type: "custom",
                            validationCallback: function(params) {
                                debugger;
                                if ($scope.UpdateButtonShow === true) {
                                    if (params.value !== $scope.oldGroupName) {
                                        $http({
                                            method: "GET",
                                            url: "/Security/IsGroupExist",
                                            params: { groupName: params.value }
                                        }).then(function(isExist) {
                                            debugger;
                                            if (isExist.data === true) {
                                                params.rule.message = "الاسم موجود مسبقاً";
                                                params.rule.isValid = false;
                                                params.validator.validate();
                                                return false;
                                            } else {
                                                return true;
                                            }
                                        });
                                    }
                                } else {
                                    $http({
                                        method: "GET",
                                        url: "/Security/IsGroupExist",
                                        params: { groupName: params.value }
                                    }).then(function(isExist) {
                                        debugger;
                                        if (isExist.data === true) {
                                            params.rule.message = "الاسم موجود مسبقاً";
                                            params.rule.isValid = false;
                                            params.validator.validate();
                                            return false;
                                        } else {
                                            return true;
                                        }
                                    });
                                }
                                return true;
                            }
                        }
                    ]
                };


                $scope.SaveButton = {
                    text: "حفظ",
                    hint: "حفظ",
                    icon: "save",
                    type: "success",
                    useSubmitBehavior: true
                };

                $scope.UpdateButton = {
                    text: "تعديل",
                    hint: "تعديل",
                    icon: "edit",
                    type: "default",
                    useSubmitBehavior: true
                };


                $scope.Refresh = function() {
                    $http({
                        method: "Get",
                        url: "/Security/GetGroups"
                    }).then(function(data) {
                        $scope.Groups = data.data;
                    });
                };

                $scope.Refresh();

                $scope.dataGridOptions = {
                    bindingOptions: {
                        dataSource: "Groups",
                        "editing.editingEnabled": "Permissions.Update",
                        //"editing.allowAdding": "Permissions.Create",
                        "editing.allowUpdating": "Permissions.Update",
                        "editing.allowDeleting": "Permissions.Delete"
                    },
                    columns: [
                        {
                            dataField: "ID",
                            visible: false
                        },
                        {
                            dataField: "Name",
                            caption: "اسم المجموعة"
                        },
                        {
                            dataField: "IsActive",
                            caption: "نشطة"
                        },
                        {
                            dataField: "IsAdminGroup",
                            caption: "مجموعة مديرين"
                        }
                    ],
                    searchPanel: {
                        visible: true,
                        placeholder: "بحث"
                    },
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
                    allowColumnReordering: true,
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
                        allowUpdating: true,
                        allowDeleting: true,
                        texts: {
                            confirmDeleteMessage: "تأكيد حذف  !",
                            deleteRow: "حذف",
                            editRow: "تعديل",
                            addRow: "اضافة"
                        }
                    },
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
                    onEditingStart: function(e) {
                        e.cancel = true;
                        $scope.GroupId = e.data.ID;
                        $scope.GroupName = e.data.Name;
                        $scope.oldGroupName = e.data.Name;
                        $scope.IsAdminGroup = e.data.IsAdminGroup;
                        $scope.IsActive = e.data.IsActive;

                        $scope.SaveButtonShow = false;
                        $scope.UpdateButtonShow = true;
                    },
                    onRowRemoving: function(e) {
                        $scope.GroupId = e.data.ID;
                        $http({
                            method: "Post",
                            url: "/Security/DeleteGroup",
                            data: { id: $scope.GroupId }
                        }).then(function(data) {
                            if (data.data === "") {
                                DevExpress.ui.notify({ message: "تم الحذف بنجاح" }, "success", 3000);
                                $scope.GroupId = null;
                                $scope.Refresh();
                            } else if (data.data !== "") {
                                DevExpress.ui.notify({ message: data.data }, "Error", 3000);
                            }
                        });
                        e.cancel = true;
                    }
                };

                $scope.onFormSubmit = function(e) {
                    DevExpress.ui.notify({ message: "جارى الحفظ" }, "success", 3000);
                    e.preventDefault();
                    if ($scope.GroupId === null) {
                        $http({
                            method: "POST",
                            url: "/Security/SaveGroup/",
                            data: {
                                groupName: $scope.GroupName,
                                school: $scope.School,
                                isAdminGroup: $scope.IsAdminGroup,
                                isActive: $scope.IsActive
                            }
                        }).then(function(data) {
                            if (data.data === "") {
                                DevExpress.ui.notify({ message: "تم الحفظ بنجاح" }, "success", 3000);
                                $scope.SaveButtonShow = true;
                                $scope.UpdateButtonShow = false;

                                $scope.GroupId = null;
                                $scope.GroupName = "";
                                $scope.IsAdminGroup = true;
                                $scope.IsActive = true;
                                $scope.Refresh();
                            } else if (data.data !== "") {
                                DevExpress.ui.notify({ message: data.data }, "error", 3000);
                            }
                        });
                    } else {
                        $http({
                            method: "POST",
                            url: "/Security/EditGroup/",
                            data: {
                                groupName: $scope.GroupName,
                                isAdminGroup: $scope.IsAdminGroup,
                                isActive: $scope.IsActive,
                                id: $scope.GroupId
                            }
                        }).then(function(data) {
                            if (data.data === "") {
                                DevExpress.ui.notify({ message: "تم الحفظ بنجاح" }, "success", 3000);
                                $scope.SaveButtonShow = true;
                                $scope.UpdateButtonShow = false;

                                $scope.GroupId = null;
                                $scope.GroupName = "";
                                $scope.IsAdminGroup = true;
                                $scope.IsActive = true;

                                $scope.Refresh();
                                e.preventDefault();
                            } else if (data.data !== "") {
                                DevExpress.ui.notify({ message: data.data }, "error", 3000);
                            }
                        });

                    }

                };
            }
        ]);
})();