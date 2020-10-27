/// <reference path="../dx.web.debug.js" />
/// <reference path="../jquery-2.2.3.js" />
/// <reference path="../jquery-2.2.3.intellisense.js" />
(function() {
    app.controller("IssuesConfigCtrl",
        [
            "$scope", "$http", "$timeout", function($scope, $http, $timeout) {
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
                        params: { screenId: 40 }
                    }).then(function(data) {
                        $scope.Permissions.Create = data.data.Create;
                        $scope.Permissions.Read = data.data.Read;
                        $scope.Permissions.Update = data.data.Update;
                        $scope.Permissions.Delete = data.data.Delete;
                        $scope.Permissions.View = data.data.View;
                    });
                };
                $scope.GetPermssions();
                $scope.issueId = null;
                $scope.IssueDescription = "";
                $scope.Issue = {
                    IssueDescription:
                    {
                        bindingOptions: { value: "IssueDescription" },
                        placeholder: "اسم التصينف",
                        showClearButton: true
                    }
                };
                $scope.ValidationRules = {
                    IssueDescription: {
                        validationGroup: "IssuesConfig",
                        validationRules: [
                            {
                                type: "required",
                                message: "من فضلك ادخل الاسم"
                            },
                            {
                                type: "stringLength",
                                min: 2,
                                message: "الاسم يجب ان يكون حرفان على الاقل"
                            }
                        ]
                    }
                };

                function reset() {
                    $scope.issueId = null;
                    $scope.IssueDescription = "";
                    var validationGroup = DevExpress.validationEngine.getGroupConfig("IssuesConfig");
                    if (validationGroup) {
                        validationGroup.reset();
                    }
                    $scope.isUpdating = false;
                    $scope.dataGridInstance.refresh();
                }

                $scope.isUpdating = false;
                $scope.Buttons = {
                    SaveButtonOptions: {
                        bindingOptions: { visible: "!isUpdating" },
                        text: "حفظ",
                        hint: "حفظ",
                        icon: "save",
                        type: "success",
                        useSubmitBehavior: true
                    },
                    UpdateButtonOptions: {
                        bindingOptions: { visible: "isUpdating" },
                        text: "تعديل",
                        hint: "تعديل",
                        icon: "edit",
                        type: "default",
                        useSubmitBehavior: true
                    },
                    CancelButtonOptions: {
                        bindingOptions: { visible: "isUpdating" },
                        text: "الغاء",
                        hint: "الغاء",
                        icon: "close",
                        type: "danger",
                        useSubmitBehavior: false,
                        onClick: function(e) {
                            reset();
                        }
                    }
                }
                $scope.dataGridOptions = {
                    dataSource: new DevExpress.data.DataSource({
                        key: "Id",
                        loadMode: "raw",
                        load: function(loadOptions) {
                            return $.getJSON("/Config/GetIssues");
                        },
                        remove: function(key) {
                            return $.ajax({
                                url: "/Config/DeleteIssue/",
                                data: { issueId: key },
                                method: "POST"
                            });
                        }
                    }),
                    bindingOptions: {
                        "editing.editingEnabled": "Permissions.Update",
                        //"editing.allowAdding": "Permissions.Create",
                        "editing.allowUpdating": "Permissions.Update",
                        "editing.allowDeleting": "Permissions.Delete"
                    },
                    columns: [
                        {
                            dataField: "IssueDescription",
                            caption: "اسم التصينف"
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
                        texts: {
                            confirmDeleteMessage: "تأكيد حذف  !",
                            deleteRow: "حذف",
                            editRow: "تعديل",
                            addRow: "اضافة"
                        }
                    },
                    noDataText: "لا يوجد بيانات",
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
                    onInitialized: function(e) {
                        $scope.dataGridInstance = e.component;
                    },
                    onEditingStart: function(e) {
                        e.cancel = true;
                        $scope.issueId = e.key;
                        $scope.isUpdating = true;
                        $scope.IssueDescription = e.data.IssueDescription;
                        $scope.issueId = e.key;
                    },
                    onRowRemoving: function(e) {
                        $http({
                            method: "POST",
                            url: "/Config/DeleteIssue",
                            data: { issueId: e.key }
                        }).then(function(data) {
                            if (data.data === "") {
                                DevExpress.ui.notify({ message: "تم الحذف بنجاح" }, "success", 3000);
                                $scope.dataGridInstance.refresh();
                            } else {
                                DevExpress.ui.notify({ message: data.data }, "error", 3000);
                            }
                        });
                    }
                };
                $scope.onFormSubmit = function(e) {
                    var validationGroup = DevExpress.validationEngine.getGroupConfig("IssuesConfig");
                    if (validationGroup) {
                        var validation = validationGroup.validate();
                        if (validation.isValid) {
                            DevExpress.ui.notify({ message: "جارى الحفظ" }, "success", 3000);
                            e.preventDefault();
                            if ($scope.issueId === null) {
                                $http({
                                    method: "POST",
                                    url: "/Config/AddIssue/",
                                    data: { issueDescription: $scope.IssueDescription }
                                }).then(function(data) {
                                    if (data.data === "") {
                                        DevExpress.ui.notify({ message: "تم الحفظ بنجاح" }, "success", 3000);
                                        reset();
                                    } else {
                                        DevExpress.ui.notify({ message: data.data }, "error", 3000);
                                    }
                                });
                            } else {
                                $http({
                                    method: "POST",
                                    url: "/Config/UpdateIssue/",
                                    data: {
                                        issueId: $scope.issueId,
                                        issueDescription: $scope.IssueDescription
                                    }
                                }).then(function(data) {
                                    if (data.data === "") {
                                        DevExpress.ui.notify({ message: "تم الحفظ بنجاح" }, "success", 3000);
                                        reset();
                                    } else {
                                        DevExpress.ui.notify({ message: data.data }, "error", 3000);
                                    }
                                });
                            }
                        }
                    }
                };
            }
        ]);
})();