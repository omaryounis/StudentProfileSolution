/// <reference path="../dx.web.debug.js" />
/// <reference path="../jquery-2.2.3.js" />
(function() {
    app.controller("FilterGroupsCtrl",
        [
            "$scope", "$http", "$timeout", function($scope, $http, $timeout) {
                $scope.UsersIds = [];
                var dataSourceGroup = new DevExpress.data.DataSource({
                    loadMode: "raw",
                    key: "Value",
                    paginate: true,
                    load: function() {
                        return $.getJSON("/Config/GetFilterGroupsGroups");
                    }
                });
                var dataSourceUsers = new DevExpress.data.DataSource({
                    loadMode: "raw",
                    key: "Value",
                    paginate: true,
                    load: function() {
                        return $.getJSON("/Config/GetFilterGroupUsers",
                            function(data) {
                            });
                    }
                });
                $scope.FilterGroup = {
                    ValidationRules: {
                        Group: {
                            validationGroup: "FilterGroup",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        Users: {
                            validationGroup: "FilterGroup",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        }
                    },
                    //اسم المجموعة
                    Group:
                    {
                        dataSource: dataSourceGroup,
                        bindingOptions: { value: "GroupId" },
                        valueExpr: "Value",
                        displayExpr: "Text",
                        placeholder: "اسم المجموعة",
                        showClearButton: true,
                        itemTemplate: function(data) {
                            return "<div title='" + data.Text + "' value='" + data.Value + "'>" + data.Text + "</div>";
                        },
                        onValueChanged: function(e) {
                            if (e.value) {
                                $http({
                                    method: "GET",
                                    url: "/Config/GetGroupUsers/",
                                    params: { groupId: e.value }
                                }).then(function(response) {
                                    if (response.data != null) {
                                        //dataSourceUsers.reload();
                                        $scope.UsersIds = response.data;
                                    }
                                });
                            }
                        }
                    },
                    //المستخدمين
                    Users:
                    {
                        dataSource: dataSourceUsers,
                        bindingOptions: { "selectedItemKeys": "UsersIds" },
                        showSelectionControls: true,
                        keyExpr: "Value",
                        //applyValueMode: "useButtons",
                        noDataText: "لا يوجد بيانات",
                        valueExpr: "Value",
                        displayExpr: "Text",
                        placeholder: "المستخدمين",
                        selectAllText: "تحديد الكل",
                        nextButtonText: "المزيد",
                        showClearButton: true,
                        selectionMode: "all",
                        selectAllMode: "allPages",
                        itemTemplate: function(data) {
                            return "<div title='" + data.Text + "' value='" + data.Value + "'>" + data.Text + "</div>";
                        }
                    },
                    SaveButton: {
                        text: "حفظ",
                        hint: "حفظ",
                        icon: "save",
                        type: "success",
                        validationGroup: "FilterGroup",
                        useSubmitBehavior: true
                    }
                };
                $scope.onFormSubmit = function(e) {
                    var data = {
                        groupId: $scope.GroupId,
                        usersIds: $scope.UsersIds
                    };
                    DevExpress.ui.notify({
                        message: "جارى الحفظ",
                        type: "success",
                        displayTime: 3000,
                        closeOnClick: true
                    });
                    var validationGroup = DevExpress.validationEngine.getGroupConfig("FilterGroup");
                    if (validationGroup) {
                        var result = validationGroup.validate();
                        if (result.isValid) {
                            $http({
                                method: "POST",
                                url: "/Config/AddFilterGroups/",
                                data: data
                            }).then(function(response) {
                                if (response.data === "") {
                                    DevExpress.ui.notify({
                                        message: "تم الحفظ بنجاح",
                                        type: "success",
                                        displayTime: 3000,
                                        closeOnClick: true
                                    });
                                } else {
                                    DevExpress.ui.notify({
                                        message: response.data,
                                        type: "error",
                                        displayTime: 3000,
                                        closeOnClick: true
                                    });
                                }
                            });
                        }
                    }
                };
            }
        ]);
})();