(function() {
    app.controller("SecretPermissionsCtrl",
        [
            "$scope", "$http", function($scope, $http) {

                $scope.Permission = {
                    ValidationRules: {
                        User: {
                            validationRules: [{ type: "required", message: "حقل اجبارى" }],
                            validationGroup: "SecretPermissions"
                        }
                    },
                    User: {
                        dataSource: new DevExpress.data.DataSource({
                            loadMode: "raw",
                            key: "ID",
                            load: function() {
                                return $.getJSON("/Security/GetSecretUsers", function(data) {});
                            }
                        }),
                        bindingOptions: { value: "UserId" },
                        displayExpr: "Text",
                        valueExpr: "Value",
                        placeholder: "اختر مستخدم",
                        noDataText: "لا يوجد بيانات",
                        searchEnabled: true,
                        rtlEnabled: true,
                        onSelectionChanged: function(e) {
                            if (e.selectedItem) {
                                if (e.selectedItem.Value !== null) {
                                    $scope.disableSave = false;
                                }
                            }
                        },
                        onItemClick: function() {
                            $http({
                                method: "GET",
                                url: "/Security/GetUserSecretPermissions/",
                                params: { userId: $scope.UserId }
                            }).then(function(data) {
                                $scope.IsSecret = false;
                                $scope.IsTopSecret = false;
                                if (data.data !== "") {
                                    $scope.IsSecret = data.data.isSecret;
                                    $scope.IsTopSecret = data.data.isTopSecret;
                                }
                            });
                        }
                    },
                    IsSecret: {
                        bindingOptions: { value: "IsSecret" },
                        text: "سري",
                        rtlEnabled: true
                    },
                    IsTopSecret: {
                        bindingOptions: { value: "IsTopSecret" },
                        text: "سري جداً",
                        rtlEnabled: true
                    },
                    SaveButtonOptions: {
                        bindingOptions: { disabled: "disableSave" },
                        useSubmitBehavior: false,
                        validationGroup: "SecretPermissions",
                        text: "حفظ",
                        hint: "حفظ",
                        icon: "save",
                        type: "success"
                    },
                    init: function(e) {
                        $scope.UserId = null;
                        $scope.IsSecret = false;
                        $scope.IsTopSecret = false;
                        $scope.disableSave = true;
                        var secretPermissionsvalidationGroup =
                            DevExpress.validationEngine.getGroupConfig("SecretPermissions");
                        if (secretPermissionsvalidationGroup) {
                            secretPermissionsvalidationGroup.reset();
                        }

                    }
                };


                $scope.Permission.init();

                $scope.onFormSubmit = function(e) {
                    var secretPermissionsvalidationGroup =
                        DevExpress.validationEngine.getGroupConfig("SecretPermissions");

                    if (secretPermissionsvalidationGroup) {
                        var validationResult = secretPermissionsvalidationGroup.validate();
                        if (validationResult.isValid) {
                            debugger;
                            DevExpress.ui.notify({
                                message: "جارى الحفظ",
                                type: "success",
                                displayTime: 3000,
                                closeOnClick: true
                            });
                            $http({
                                method: "POST",
                                url: "/Security/SaveSecretPermissions/",
                                data: {
                                    userId: $scope.UserId,
                                    isSecret: $scope.IsSecret,
                                    isTopSecret: $scope.IsTopSecret
                                },
                                headers: {
                                'RequestVerificationToken':  $scope.antiForgeryToken 
                            }    
                            }).then(function(data) {
                                if (data.data === "") {
                                    DevExpress.ui.notify({
                                        message: "تم الحفظ بنجاح",
                                        type: "success",
                                        displayTime: 3000,
                                        closeOnClick: true
                                    });

                                    $scope.Permission.init();
                                } else if (data.data !== "") {
                                    DevExpress.ui.notify({
                                        message: data.data,
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