/// <reference path="../dx.web.debug.js" />
/// <reference path="../jquery-2.2.3.js" />
Array.prototype.sum = function(prop) {
    var total = 0;
    for (var i = 0, len = this.length; i < len; i++) {
        total += this[i][prop];
    }
    return total;
};
(function() {
    app.controller("AdvancesCtrl",
        [
            "$scope", "$http", "$timeout", function($scope, $http, $timeout) {
                var today = new Date();
                var firstDayOfCurrentMonth = new Date(today.getFullYear(), today.getMonth(), 1);
                $scope.Permissions = {
                    Create: false,
                    Read: false,
                    Update: false,
                    Delete: false,
                    View: false
                };
                $http({
                    method: "POST",
                    url: "/Security/GetPermissions",
                    params: { screenId: 14 } //شاشة المجموعات
                }).then(function(data) {
                    $scope.Permissions.Create = data.data.Create;
                    $scope.Permissions.Read = data.data.Read;
                    $scope.Permissions.Update = data.data.Update;
                    $scope.Permissions.Delete = data.data.Delete;
                    $scope.Permissions.View = data.data.View;
                });
                $http.get("/Advances/GetActiveYearMonthes").then(function(data) {
                    $scope.ActiveYearMonthes = data.data;
                    var year = [];
                    $.map($scope.ActiveYearMonthes,
                        function(month) {
                            year.push(new Date(month.StartDate));
                        });
                    $scope.firstYearMonth = new Date(Math.min.apply(null, year));
                    $scope.lastYearMonth = new Date(Math.max.apply(null, year));
                    $scope.AutoCalc = 1;
                    $scope.AdvanceDueDate = firstDayOfCurrentMonth;
                });
                $scope.Init = function() {
                    $scope.StudentName = null;
                    $scope.AdvanceType = null;
                    $scope.AdvanceDate = today;;
                    $scope.AdvanceValue = 1;
                    $scope.MonthlyPremium = 1;
                    $scope.AutoCalc = null;
                    $scope.ActiveYearMonthes = [];
                    $scope.autoCalcItems = [];
                    $scope.customCalcItems = [];
                    $scope.autoCalcFormItems = [];
                    $scope.customCalcFormItems = [];
                    $scope.showAutoCalcForm = true;
                    $scope.showCustomCalcForm = false;
                    $scope.readOnlyForm = false;
                };
                $scope.Init();

                function updateMonthlyPremium() {
                    $scope.MonthlyPremium = $scope.AdvanceValue / $scope.autoCalcItems.length;
                }

                $scope.Advance = {
                    ValidationRules: {
                        StudentNumber: {
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        StudentName: {
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        AdvanceType: {
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        AdvanceDueDate: {
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        AdvanceDate: {
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        AdvanceValue: {
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        MonthlyPremium: {
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        AutoCalc: {
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        }
                    },
                    //بحث بواسطة
                    SearchByStudentNumber:
                    {
                        bindingOptions: { value: "SearchByStudentNumber" },
                        offText: "رقم الطالب",
                        onText: "اسم الطالب",
                        width: "100%",
                        onValueChanged: function(e) {

                        }
                    },
                    //رقم الطالب
                    StudentNumber:
                    {
                        bindingOptions: { value: "StudentName" },
                        dataSource: new DevExpress.data.DataSource({
                            paginate: true,
                            cacheRawData: true,
                            key: "Value",
                            load: function(loadOptions) {
                                var d = $.Deferred();
                                $.getJSON("/Advances/GetEmployees/",
                                    {
                                        skip: loadOptions.skip,
                                        take: loadOptions.take,
                                        searchValue: loadOptions.searchValue
                                            ? JSON.stringify(loadOptions.searchValue)
                                            : ""
                                    }).done(function(result) {
                                    d.resolve(result);
                                });
                                return d.promise();
                            }
                        }),
                        valueExpr: "Value",
                        displayExpr: "Text",
                        placeholder: "رقم الطالب",
                        noDataText: "لا يوجد بيانات",
                        searchEnabled: true,
                        showClearButton: true,
                        itemTemplate: function(data) {
                            return "<div title='" + data.Text + "' value='" + data.Value + "'>" + data.Text + "</div>";
                        }
                    },
                    //اسم الطالب
                    StudentName:
                    {
                        bindingOptions: { value: "StudentName" },
                        dataSource: new DevExpress.data.DataSource({
                            paginate: true,
                            cacheRawData: true,
                            key: "Value",
                            load: function(loadOptions) {
                                var d = $.Deferred();
                                $.getJSON("/Advances/GetEmployees/",
                                    {
                                        skip: loadOptions.skip,
                                        take: loadOptions.take,
                                        searchValue: loadOptions.searchValue
                                            ? JSON.stringify(loadOptions.searchValue)
                                            : ""
                                    }).done(function(result) {
                                    d.resolve(result);
                                });
                                return d.promise();
                            }
                        }),
                        valueExpr: "Value",
                        displayExpr: "Text",
                        placeholder: "اسم الطالب",
                        noDataText: "لا يوجد بيانات",
                        searchEnabled: true,
                        showClearButton: true,
                        itemTemplate: function(data) {
                            return "<div title='" + data.Text + "' value='" + data.Value + "'>" + data.Text + "</div>";
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
                        bindingOptions: { value: "AdvanceDueDate", min: "firstYearMonth", max: "lastYearMonth" },
                        placeholder: "شهر بداية الخصم",
                        //displayFormat: "dd/MM/yyyy",
                        showClearButton: true,
                        displayFormat: "monthAndYear",
                        maxZoomLevel: "year",
                        minZoomLevel: "century",
                        //min: firstDayOfCurrentMonth,
                        onValueChanged: function(newDate) {
                            $scope.autoCalcFormItems = [];
                            $scope.customCalcFormItems = [];
                            if (newDate.value) {
                                debugger;
                                //تلقائي
                                $scope.ActiveYearMonthes.forEach(function(element) {
                                    var month = new Date(element.StartDate);
                                    var isValidMonth = month >= new Date(newDate.value.toDateString());
                                    if (isValidMonth) {
                                        var field = element.Name;
                                        $scope.autoCalcFormItems.push(
                                            {
                                                dataField: "dxCheckBox" + field,
                                                name: field,
                                                label: {
                                                    text: (month.getMonth() + 1) + "/" + month.getFullYear(),
                                                    location: "left"
                                                },
                                                editorType: "dxCheckBox",
                                                editorOptions: {
                                                    value: false,
                                                    name: field,
                                                    hint: (month.getMonth() + 1) + "/" + month.getFullYear(),
                                                    showClearButton: true,
                                                    width: "100%",
                                                    onValueChanged: function(eve) {
                                                        if (eve.value === true) {
                                                            $scope.autoCalcItems.push(field);
                                                        } else {
                                                            var index = $scope.autoCalcItems.indexOf(field);
                                                            if (index > -1) {
                                                                $scope.autoCalcItems.splice(index, 1);
                                                            }
                                                        }
                                                        updateMonthlyPremium();
                                                    }
                                                }
                                            }
                                        );
                                    }
                                });
                                // مخصص
                                $scope.ActiveYearMonthes.forEach(function(element) {
                                    var month = new Date(element.StartDate);
                                    var isValidMonth = month >= new Date(newDate.value.toDateString());
                                    if (isValidMonth) {
                                        var field = element.Name;
                                        var firstDay = new Date(month.getFullYear(), month.getMonth(), 1);
                                        var lastDay = new Date(month.getFullYear(), month.getMonth() + 1, 0);
                                        $scope.customCalcFormItems.push(
                                            {
                                                dataField: "dxCheckBox" + field,
                                                name: field,
                                                label: {
                                                    text: "تفعيل/الغاء",
                                                    location: "left"
                                                },
                                                editorType: "dxCheckBox",
                                                editorOptions: {
                                                    value: false,
                                                    name: field,
                                                    hint: "تفعيل/الغاء",
                                                    showClearButton: true,
                                                    width: "100%",
                                                    onValueChanged: function(eve) {
                                                        var dateEditor = "dxDateBox" + eve.component._options.name;
                                                        var numberEditor = "dxNumberBox" + eve.component._options.name;
                                                        var index;
                                                        if (eve.value === true) {
                                                            $scope.CustomCalcFormInstance.getEditor(dateEditor)
                                                                .option("disabled", false);
                                                            $scope.CustomCalcFormInstance.getEditor(numberEditor)
                                                                .option("disabled", false);
                                                            var isValid = $scope.CustomCalcFormInstance
                                                                .getEditor(dateEditor)
                                                                .option("isValid");
                                                            if (isValid) {
                                                                var dateValue = $scope.CustomCalcFormInstance
                                                                    .getEditor(dateEditor)
                                                                    .option("value");
                                                                var numberValue = $scope.CustomCalcFormInstance
                                                                    .getEditor(numberEditor)
                                                                    .option("value");
                                                                //لو موجود من قبل يحدث القيمة غير ذلك يضيفها
                                                                index = $scope.customCalcItems.findIndex(
                                                                    i => i.MonthId === element.ID);
                                                                if (index > -1) {
                                                                    $scope.customCalcItems[index] = {
                                                                        MonthId: element.ID,
                                                                        date: dateValue,
                                                                        value: numberValue
                                                                    };
                                                                } else {
                                                                    $scope.customCalcItems.push({
                                                                        MonthId: element.ID,
                                                                        date: dateValue,
                                                                        value: numberValue
                                                                    });
                                                                }
                                                            }
                                                        } else {
                                                            //لما يلغى التشيك يحذفه من الarray
                                                            $scope.CustomCalcFormInstance.getEditor(dateEditor)
                                                                .option("disabled", true);
                                                            $scope.CustomCalcFormInstance.getEditor(numberEditor)
                                                                .option("disabled", true);
                                                            $scope.CustomCalcFormInstance.getEditor(dateEditor)
                                                                .option("value", 0);
                                                            $scope.CustomCalcFormInstance.getEditor(numberEditor)
                                                                .option("value", 0);
                                                            index = $scope.customCalcItems.findIndex(
                                                                i => i.MonthId === element.ID);
                                                            if (index > -1) {
                                                                $scope.customCalcItems.splice(index, 1);
                                                            }
                                                        }
                                                    }
                                                }
                                            },
                                            {
                                                dataField: "dxDateBox" + field,
                                                name: "dxDateBox" + field,
                                                label: {
                                                    text: (month.getMonth() + 1) + "/" + month.getFullYear(),
                                                    location: "left"
                                                },
                                                editorType: "dxDateBox",
                                                editorOptions: {
                                                    value: new Date(month),
                                                    name: field,
                                                    disabled: true,
                                                    displayFormat: "dd/MM/yyyy",
                                                    rtlEnabled: true,
                                                    showClearButton: true,
                                                    min: firstDay,
                                                    max: lastDay,
                                                    width: "100%",
                                                    dateOutOfRangeMessage: " التاريخ يجب ان يكون فى الفترة من" +
                                                        firstDay.toLocaleDateString("en-GB") +
                                                        " الى " +
                                                        lastDay.toLocaleDateString("en-GB"),
                                                    onValueChanged: function(e) {
                                                        var editor = e.component;
                                                        var editorName = editor.option("name");
                                                        var editorIsValid = editor.option("isValid");
                                                        if (editorIsValid) {
                                                            var dateEditor = "dxDateBox" + editorName;
                                                            var numberEditor = "dxNumberBox" + editorName;
                                                            var dateValue = $scope.CustomCalcFormInstance
                                                                .getEditor(dateEditor)
                                                                .option("value");
                                                            var numberValue = $scope.CustomCalcFormInstance
                                                                .getEditor(numberEditor)
                                                                .option("value");
                                                            if (index > -1) {
                                                                $scope.customCalcItems[index] = {
                                                                    MonthId: element.ID,
                                                                    date: dateValue,
                                                                    value: numberValue
                                                                };
                                                            } else {
                                                                $scope.customCalcItems.push({
                                                                    MonthId: element.ID,
                                                                    date: dateValue,
                                                                    value: numberValue
                                                                });
                                                            }
                                                        }
                                                    }
                                                },
                                                validationRules: [
                                                    {
                                                        type: "range",
                                                        min: firstDay,
                                                        message:
                                                            " التاريخ يجب ان يكون فى الفترة من" +
                                                                firstDay.toLocaleDateString("en-GB") +
                                                                " الى " +
                                                                lastDay.toLocaleDateString("en-GB")
                                                    }
                                                ],
                                                validationGroup: "CustomCalc"
                                            },
                                            {
                                                dataField: "dxNumberBox" + field,
                                                name: field,
                                                label: {
                                                    text: "المبلغ ",
                                                    location: "left"
                                                },
                                                editorType: "dxNumberBox",
                                                editorOptions: {
                                                    bindingOptions: { max: "AdvanceValue" },
                                                    min: 1,
                                                    name: field,
                                                    hint: "المبلغ",
                                                    placeholder: "المبلغ",
                                                    value: 1,
                                                    disabled: true,
                                                    showSpinButtons: true,
                                                    showClearButton: true,
                                                    width: "100%",
                                                    onValueChanged: function(e) {
                                                        var editor = e.component;
                                                        var editorName = editor.option("name");
                                                        var editorIsValid = editor.option("isValid");
                                                        if (editorIsValid) {
                                                            var dateEditor = "dxDateBox" + editorName;
                                                            var numberEditor = "dxNumberBox" + editorName;
                                                            var dateValue = $scope.CustomCalcFormInstance
                                                                .getEditor(dateEditor)
                                                                .option("value");
                                                            var numberValue = $scope.CustomCalcFormInstance
                                                                .getEditor(numberEditor)
                                                                .option("value");
                                                            //لو موجود من قبل يحدث القيمة غير ذلك يضيفها
                                                            var index = $scope.customCalcItems.findIndex(
                                                                i => i.MonthId === element.ID);
                                                            if (index > -1) {
                                                                $scope.customCalcItems[index] = {
                                                                    MonthId: element.ID,
                                                                    date: dateValue,
                                                                    value: numberValue
                                                                };
                                                            } else {
                                                                $scope.customCalcItems.push({
                                                                    MonthId: element.ID,
                                                                    date: dateValue,
                                                                    value: numberValue
                                                                });
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        );
                                    }
                                });
                            }
                        }
                    },
                    //تاريخ الطلب
                    AdvanceDate:
                    {
                        bindingOptions: { value: "AdvanceDate", min: "firstYearMonth", max: "lastYearMonth" },
                        placeholder: "تاريخ الطلب",
                        displayFormat: "dd/MM/yyyy",
                        showClearButton: true,
                        min: today
                    },
                    //المبلغ
                    AdvanceValue:
                    {
                        bindingOptions: { value: "AdvanceValue" },
                        placeholder: "مبلغ السلفة",
                        showClearButton: true,
                        showSpinButtons: true,
                        min: 0,
                        onValueChanged: function(e) {
                            updateMonthlyPremium();
                        }
                    },
                    MonthlyPremium:
                    {
                        bindingOptions: { value: "MonthlyPremium" },
                        placeholder: "قيمة القسط",
                        showClearButton: true,
                        showSpinButtons: true,
                        disabled: true,
                        min: 0
                    },
                    AutoCalc:
                    {
                        bindingOptions: { value: "AutoCalc" },
                        items: [{ text: "احتساب مخصص", value: 0 }, { text: "احتساب تلقائى", value: 1 }],
                        displayExpr: "text",
                        valueExpr: "value",
                        text: "احتساب تلقائي",
                        layout: "horizontal",
                        rtlEnabled: true,
                        onValueChanged: function(e) {
                            //$scope.customCalcItems = [];
                            //$scope.autoCalcItems = [];
                            if (e.value === 1) {
                                //تلقائي
                                $scope.showAutoCalcForm = true;
                                $scope.showCustomCalcForm = false;
                            } else {
                                //مخصص
                                $scope.MonthlyPremium = 0;
                                $scope.showAutoCalcForm = false;
                                $scope.showCustomCalcForm = true;
                            }
                        }
                    },
                    AutoCalcFormOptions: {
                        bindingOptions: { items: "autoCalcFormItems", visible: "showAutoCalcForm" },
                        showColonAfterLabel: true,
                        showValidationSummary: true,
                        validationGroup: "AutoCalc",
                        labelLocation: "left",
                        scrollingEnabled: true,
                        alignItemLabels: true,
                        alignItemLabelsInAllGroups: true,
                        colCount: 3,
                        onInitialized: function(e) {
                            $scope.AutoCalcFormInstance = e.component;
                        }
                    },
                    CustomCalcFormOptions: {
                        bindingOptions: { items: "customCalcFormItems", visible: "showCustomCalcForm" },
                        showColonAfterLabel: true,
                        showValidationSummary: true,
                        validationGroup: "CustomCalc",
                        labelLocation: "left",
                        scrollingEnabled: true,
                        alignItemLabels: true,
                        alignItemLabelsInAllGroups: true,
                        colCount: 3,
                        onInitialized: function(e) {
                            $scope.CustomCalcFormInstance = e.component;
                        }
                    },
                    SaveButton: {
                        text: "حفظ",
                        hint: "حفظ",
                        icon: "save",
                        type: "success",
                        useSubmitBehavior: true
                    },
                    UpdateButton: {
                        text: "تعديل",
                        hint: "تعديل",
                        icon: "edit",
                        type: "default",
                        useSubmitBehavior: true
                    }
                };
                $scope.onFormSubmit = function(e) {
                    var advance = {
                        studentId: $scope.StudentName,
                        advanceType: $scope.AdvanceTypeId,
                        dueDate: $scope.AdvanceDueDate,
                        date: $scope.AdvanceDate,
                        value: $scope.AdvanceValue
                    };
                    if ($scope.AutoCalc === 0) {
                        //احتساب مخصص
                        if (Object.keys($scope.customCalcItems).length === 0) {
                            DevExpress.ui.notify({
                                message: "برجاء اختيار شهر واحد عالاقل",
                                type: "error",
                                displayTime: 3000,
                                closeOnClick: true
                            });
                            return;
                        } else {
                            var totalsum = $scope.customCalcItems.sum("value");
                            if (totalsum !== $scope.AdvanceValue) {
                                DevExpress.ui.notify({
                                    message: "مجموع المبالغ للأشهر لا يساوى مبلغ السلفة",
                                    type: "error",
                                    displayTime: 3000,
                                    closeOnClick: true
                                });
                                return;
                            }
                            advance.monthlyPremiums = $scope.customCalcItems;
                            $http({
                                method: "POST",
                                url: "/Advances/AddCustomCalcAdvance/",
                                data: advance
                            }).then(function(data) {
                                if (data.data === "") {
                                    DevExpress.ui.notify({
                                        message: "تم الحفظ بنجاح",
                                        type: "success",
                                        displayTime: 3000,
                                        closeOnClick: true
                                    });
                                    window.location.href = "/Advances/AdvancesRequestsList";
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
                    } else {
                        //احتساب تلقائى
                        if ($scope.autoCalcItems.length === 0) {
                            DevExpress.ui.notify({
                                message: "برجاء اختيار شهر واحد عالاقل",
                                type: "error",
                                displayTime: 3000,
                                closeOnClick: true
                            });
                            return;
                        } else {
                            DevExpress.ui.notify({
                                message: "جارى الحفظ",
                                type: "success",
                                displayTime: 3000,
                                closeOnClick: true
                            });
                            advance.monthsIds = $scope.autoCalcItems;
                            $http({
                                method: "POST",
                                url: "/Advances/AddAutoCalcAdvance/",
                                data: advance
                            }).then(function(data) {
                                if (data.data === "") {
                                    DevExpress.ui.notify({
                                        message: "تم الحفظ بنجاح",
                                        type: "success",
                                        displayTime: 3000,
                                        closeOnClick: true
                                    });
                                    window.location.href = "/Advances/AdvancesRequestsList";
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
                };
            }
        ]);
})();