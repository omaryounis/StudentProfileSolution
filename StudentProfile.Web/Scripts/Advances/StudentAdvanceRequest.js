/// <reference path="../dx.all.debug.js" />
/// <reference path="../jquery-2.2.3.js" />
(function() {
    app.controller("StudentAdvanceRequestCtrl",
        [
            "$scope", "$http", "$timeout", function($scope, $http, $timeout) {
                var amounts = [
                    100, 150, 200, 250, 300, 350, 400, 450, 500, 550, 600, 650, 700, 750, 800, 850, 900, 950, 1000
                ];
                var today = new Date();
                $scope.Advance = {
                    ValidationRules: {
                        AdvanceType: {
                            validationGroup: "addAdvance",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        AdvanceDueDate: {
                            validationGroup: "addAdvance",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        AdvanceDate: {
                            validationGroup: "addAdvance",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        AdvanceValue: {
                            validationGroup: "addAdvance",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        }
                    },
                    //نوع السلفة
                    AdvanceType:
                    {
                        bindingOptions: { value: "AdvanceTypeId" },
                        dataSource: new DevExpress.data.DataSource({
                            loadMode: "raw",
                            paginate: true,
                            cacheRawData: true,
                            key: "Value",
                            load: function() {
                                return $.getJSON("/Advances/GetAdvancesTypes/");
                            }
                        }),
                        valueExpr: "Value",
                        displayExpr: "Text",
                        placeholder: "نوع السلفة",
                        showClearButton: true,
                        itemTemplate: function(data) {
                            return "<div title='" + data.Text + "' value='" + data.Value + "'>" + data.Text + "</div>";
                        }
                    },
                    //شهر بداية الخصم
                    AdvanceDueDate:
                    {
                        bindingOptions: { value: "AdvanceDueDate" },
                        placeholder: "شهر بداية الخصم",
                        showClearButton: true,
                        displayFormat: "monthAndYear",
                        maxZoomLevel: "year",
                        minZoomLevel: "century",
                        disabled: true,
                        width: "100%"
                    },
                    //تاريخ الطلب
                    AdvanceDate:
                    {
                        bindingOptions: { value: "AdvanceDate" },
                        placeholder: "تاريخ الطلب",
                        displayFormat: "dd/MM/yyyy",
                        showClearButton: true,
                        min: today,
                        onValueChanged: function() {
                            if ($scope.AdvanceDate) {
                                $scope.AdvanceDueDate =
                                    new Date($scope.AdvanceDate.getFullYear(), $scope.AdvanceDate.getMonth() + 1, 1);
                            }
                        },
                        width: "100%"
                    },
                    //المبلغ
                    AdvanceValue:
                    {
                        items: amounts,
                        bindingOptions: { value: "AdvanceValue" },
                        placeholder: "مبلغ السلفة",
                        showClearButton: true,
                        max: 1000
                    },
                    SaveButton: {
                        bindingOptions: { visible: "!isUpdate" },
                        text: "إضافة",
                        hint: "إضافة",
                        icon: "add",
                        type: "success",
                        validationGroup: "addAdvance",
                        useSubmitBehavior: true,
                        onClick: function(e) {
                            var validationGroup = DevExpress.validationEngine.getGroupConfig("addAdvance");
                            if (validationGroup) {
                                var result = validationGroup.validate();
                                if (result.isValid) {
                                    var advance = {
                                        studentId: $scope.StudentId,
                                        advanceType: $scope.AdvanceTypeId,
                                        advanceDate: $scope.AdvanceDate,
                                        value: $scope.AdvanceValue
                                    };
                                    DevExpress.ui.notify({
                                        message: "جارى الحفظ",
                                        type: "success",
                                        displayTime: 3000,
                                        closeOnClick: true
                                    });
                                    $http({
                                        method: "POST",
                                        url: "/Advances/AddAdvance/",
                                        data: advance
                                    }).then(function(data) {
                                        if (data.data === "") {
                                            DevExpress.ui.notify({
                                                message: "تم الحفظ بنجاح",
                                                type: "success",
                                                displayTime: 3000,
                                                closeOnClick: true
                                            });
                                            validationGroup.reset();
                                        } else {
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
                        }
                    }
                };
            }
        ]);
})();