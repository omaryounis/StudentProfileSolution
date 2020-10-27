(function() {
    app.controller("UserPermissionsCtrl",
        [
            "$scope", "$http", function($scope, $http) {

                $scope.Reset = function() {
                    $scope.disableSave = true;
                    $scope.disableList = true;
                    $scope.User = null;
                    $scope.selectedItemKeys = [];
                }
                $scope.Reset();


                var dataSourceUsers = new DevExpress.data.DataSource({
                    loadMode: "raw",
                    paginate: true,
                    cacheRawData: true,
                    sort: "Name",
                    key: "ID",
                    load: function() {
                        return $.getJSON("/Security/GetUsers/");
                    }
                });

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
                        params: { screenId: 36 }
                    }).then(function(data) {
                        $scope.Permissions.Create = data.data.Create;
                        $scope.Permissions.Read = data.data.Read;
                        $scope.Permissions.Update = data.data.Update;
                        $scope.Permissions.Delete = data.data.Delete;
                        $scope.Permissions.View = data.data.View;
                    });
                };
                $scope.GetPermssions();

                $scope.UserPermissions = {
                    Users: {
                        dataSource: dataSourceUsers,
                        bindingOptions: { value: "User" },
                        displayExpr: "Name",
                        valueExpr: "ID",
                        placeholder: "اختر المستخدم",
                        noDataText: "لا يوجد بيانات",
                        searchEnabled: true,
                        rtlEnabled: true,
                        onSelectionChanged: function(e) {
                            if (e.selectedItem.ID !== null) {
                                $http({
                                    method: "GET",
                                    url: "/Security/GetUserPermissions",
                                    params: { id: $scope.User }
                                }).then(function(data) {
                                    $scope.selectedItemKeys = data.data;
                                    $scope.disableSave = false;
                                    $scope.disableList = false;
                                });
                            } else {
                                $scope.disableSave = true;
                                $scope.disableList = true;
                            }
                        }
                    }
                };

                $scope.listOptions = {
                    dataSource: new DevExpress.data.DataSource({
                        loadMode: "raw",
                        paginate: true,
                        cacheRawData: true,
                        sort: "text",
                        key: "id",
                        map: function(item) {
                            var userDefaultCompany = $scope.userDefaultCompany;
                            if (item.id === userDefaultCompany) {
                                return { text: item.text, disabled: true, id: item.id };
                            } else {
                                return { text: item.text, disabled: false, id: item.id };
                            }
                        },
                        load: function() {
                            $.getJSON("/Security/GetUserDefaultCompany/",
                                function(data) {
                                    $scope.userDefaultCompany = data;
                                });

                            return $.getJSON("/Security/GetCompanies/", function(data) {});
                        }
                    }),
                    bindingOptions: {
                        selectedItemKeys: "selectedItemKeys",
                        disabled: "disableList"
                    },
                    itemTemplate: function(item) {
                        return "<div title='" +
                            item.text +
                            "' value='" +
                            item.id +
                            "'>" +
                            item.text +
                            "</div>";
                    },
                    onInitialized: function(e) {
                        $scope.listInstance = e.component;
                    },
                    keyExpr: "id",
                    selectAllMode: "allPages",
                    selectionMode: "multiple",
                    pageLoadMode: "scrollBottom",
                    showScrollbar: "always",
                    pageLoadingText: "تحميل...",
                    nextButtonText: "المزيد",
                    refreshingText: "تحديث...",
                    pullingDownText: "اسحب للاسفل للتحديث...",
                    showSelectionControls: true,
                    noDataText: "لا يوجد بيانات",
                    selectAllText: "تحديد الكل",
                    rtlEnabled: true
                };


                $scope.UserValidationRules = {
                    validationRules: [
                        {
                            type: "required",
                            message: "من فضلك اختر المدرسة"
                        }
                    ]
                };


                $scope.SaveButton = {
                    bindingOptions: { disabled: "disableSave" },
                    text: "حفظ",
                    icon: "save",
                    rtlEnabled: true,
                    type: "success",
                    useSubmitBehavior: true
                };

                $scope.onFormSubmit = function(e) {
                    e.preventDefault();
                    DevExpress.ui.notify({
                        message: "جارى الحفظ",
                        type: "success",
                        displayTime: 3000,
                        closeOnClick: true
                    });
                    $http({
                        method: "POST",
                        url: "/Security/SaveUserPermissions/",
                        data: { userId: $scope.User, comIds: $scope.selectedItemKeys }
                    }).then(function(data) {
                        if (data.data === "") {
                            DevExpress.ui.notify({
                                message: "تم الحفظ بنجاح",
                                type: "success",
                                displayTime: 3000,
                                closeOnClick: true
                            });
                            $scope.Reset();
                        }
                    });
                };
            }
        ]);
})();