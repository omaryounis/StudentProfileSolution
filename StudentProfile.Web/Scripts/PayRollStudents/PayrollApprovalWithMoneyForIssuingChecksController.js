app.controller("PayrollApprovalWithMoneyForIssuingChecksController", ["$scope", "$http", "PayrollApprovalWithMoneyForIssuingChecksService", function ($scope, $http, PayrollApprovalService) {
 
    $scope.ClearControls = function () {
        $scope.acceptPopupVisible = false;
        $scope.isRequiredNotes = false;
        $scope.isRequiredpayNo = false;
        $scope.currentKey = 0;
        $scope.recommendationNotes = '';
        $scope.payNo = '';
        $scope.dafPayNo = '';
        $scope.MDL_UploadingFilesvalue = '';
        //$scope.acceptPopup.hide();
    }
    $scope.ClearControls();
    $scope.FacultionList = [];
    $scope.FacultionID = null;
    $scope.FacultionIds = null;
    $scope.FacultionSelectBox = {
        bindingOptions: {
            value: "FacultionID",
            items: "FacultionList"
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
        multiline: false,
        showDropDownButton: true,
        showMultiTagOnly: false,
        onInitialized: function (e) {
            debugger;
            $scope.byanTextAreaValue = '';
            PayrollApprovalService.GetFaculties($scope.userID,$scope.payrollID).then(function (data) {
                $scope.FacultionList = data.data;
            });
        },
        onValueChanged: function (e) {
            debugger;
            if (e != undefined && e != null && e != '') {
                if (e.value != null) {
                    if (e.value.length != 0) {
                        if (e.component._selectedItems != null || e.component._selectedItems != undefined || e.component._selectedItems != '') {
                            var faculties_names = '';
                            for (i = 0; i < e.component._selectedItems.length; i++) {
                                if (faculties_names == '') {
                                    faculties_names = e.component._selectedItems[i].Text;
                                } else {
                                    faculties_names = faculties_names + '-' + e.component._selectedItems[i].Text;
                                }
                            }
                            $scope.FacultionIds = e.value.join(',');
                            $scope.Check.byanTextAreaValue = 'مكافأت المسير رقم ' + $scope.payrollNumber + 'لكلية ' + '(' + faculties_names + ')';

                        } else {
                            $scope.FacultionIds = e.value.join(',');
                        }
                       
                    }
                }
            }

        }
    };
    $scope.validationRequired = {
        validationRules: [
            {
                type: "required",
                message: "الحقل مطلوب"
            }
        ]
    }; 

    $scope.byanTextAreaValue = '';
    $scope.byanTextArea = {
        bindingOptions: {
            required: true,
            value: "Check.byanTextAreaValue"
        },
        inputAttr: { "rows": 4 },
        AutoResizeEnabled: true,
        placeholder: "البيان"
    };

    $scope.Check = { ID: 0, BenfName: "", BeneficiaryID: "", PayrollID: "", FacultyID: "", txtCheckNumberName: "", txtCheckValueName: "", TotalValueWithoutChecks: "", byanTextAreaValue:''};

    $scope.txtBenfName = {
        placeholder: "مندوب الصرف",
        bindingOptions: {
            value: "Check.BenfName",
            readOnly: "txtBenfNamereadOnly"
        },
        onInitialized: function (e) {
            $scope.txtBenfNamereadOnly = true;
        }
    };
    $scope.txtTotalValueWithoutChecksName = {
        placeholder: "مبالغ بدون شيكات",
        bindingOptions: {
            value: "Check.TotalValueWithoutChecks",
            readOnly: "txtTotalValueWithoutChecksNamereadOnly"
        },
        onInitialized: function (e) {
            $scope.txtTotalValueWithoutChecksNamereadOnly = true;
        }
    };

    $scope.txtCheckNumberName = {
        placeholder: "رقم الشيك ",
        bindingOptions: {
            value: "Check.txtCheckNumberName",
            readOnly: "txtCheckNumberNamereadOnly"
        },
        onInitialized: function (e) {
            $scope.txtCheckNumberNamereadOnly = false;
        }
    };

    $scope.CheckNumberValidationRules = {
        validationRules: [{
            type: "required",
            message: "يرجى ادخال رقم الشيك"
        }]
    };

    $scope.txtCheckValueName = {
        placeholder: "مبلغ الشيك ",
        bindingOptions: {
            value: "Check.txtCheckValueName",
            readOnly: "txtCheckValueNamereadOnly"
        },
        onInitialized: function (e) {
            $scope.txtCheckValueNamereadOnly = false;
        }
    };
    $scope.CheckValueValidationRules = {
        validationRules: [{
            type: "required",
            message: "يرجى ادخال مبلغ الشيك"
        }]
    };
    $scope.btnSave = {
        text: 'حفظ',
        visible: true,
        type: 'success',
        useSubmitBehavior: true,
        onClick: function (e) {
            debugger;
            if ($scope.Check.ID == 0) {
                    $scope.Check.PayrollID = $scope.payrollID;
                    $scope.Check.BeneficiaryID = $scope.userID;
                    $scope.Check.FacultyID = $scope.FacultionIds;
                }
                if ($scope.Check.txtCheckNumberName == '' || $scope.Check.txtCheckNumberName == undefined) {
                    return DevExpress.ui.notify({ message: " (لابد من كتابة رقم الشيك)" }, "Error", 5000);
                }
                else if ($scope.Check.txtCheckValueName == '' || $scope.Check.txtCheckValueName == undefined) {

                    return DevExpress.ui.notify({ message: " (لابد من كتابة مبلغ الشيك)" }, "Error", 5000);
                }
                else if ($scope.Check.FacultyID == '' || $scope.Check.FacultyID == undefined) {

                    return DevExpress.ui.notify({ message: " (لابد من إختيار كلية أو أكثر)" }, "Error", 5000);
                }
                else if ($scope.Check.byanTextAreaValue == '' || $scope.Check.byanTextAreaValue == undefined) {

                    return DevExpress.ui.notify({ message: " (لابد من كتابة البيان)" }, "Error", 5000);
                }
                else if ($scope.Check.TotalValueWithoutChecks < $scope.Check.txtCheckValueName) {

                    return DevExpress.ui.notify({ message: " (قيمة الشيك لابد أن تكون أصغر من أو تساوي قيمة المبالغ بدون الشيكات)" }, "Error", 5000);
                }
                else if ($scope.Check.TotalValueWithoutChecks == '0') {

                    return DevExpress.ui.notify({ message: " (تم إصدار شيكات علي المبلغ المستحق)" }, "Error", 5000);
                }
                else {
                    PayrollApprovalService.saveCheck($scope.Check).then(function (data) {
                        if (data.data !== "")
                            DevExpress.ui.notify(data.data, "error", 5000);
                        else {
                            $.getJSON("/PayRollStudents/GetChecksValues?payrollID=" + $scope.Check.PayrollID + "&userID=" + $scope.Check.BeneficiaryID, function (data1) {
                                debugger;
                                if (data1.length > 0) {

                                    $scope.Check.BenfName = data1[0].username;
                                    $scope.Check.TotalValueWithoutChecks = data1[0].totalvaluewithoutchecks;
                                    $scope.Check.txtCheckValueName = data1[0].totalvaluewithoutchecks;
                                } else {


                                    $scope.Check.BenfName = '';
                                    $scope.Check.TotalValueWithoutChecks = '';
                                }
                            });
                            return $http({
                                method: "Get",
                                url: "/PayRollStudents/GetPayRollChecks",
                                params: {
                                    payrollID: $scope.Check.PayrollID,
                                    userID: $scope.Check.BeneficiaryID
                                }
                            }).then(function (data) {
                                debugger;
                                $scope.PayRollChecksDetails = data.data;
                                PayRollChecksDataGridDataSource.reload();
                                //***************
                                DevExpress.ui.notify("تم الحفظ بنجاح", "success", 5000);
                                $scope.txtTotalValueWithoutChecksNamereadOnly = true;
                                $scope.txtCheckValueNamereadOnly = false;
                                $scope.Check.ID = 0;
                                $scope.Check.FacultyID = '';
                                $scope.Check.txtCheckNumberName = '';
                                $scope.Check.byanTextAreaValue = '';
                                //$scope.byanTextAreaValue = '';
                                $scope.FacultionID = null;
                            });
                        }
                    });
                }
           
        }
    };

    var DataSourcePayrollsGrid = new DevExpress.data.DataSource({
        paginate: true,
        cacheRawData: true,
        key: "PayrollID",
        loadMode: "raw",
        load: function () {
            return $.getJSON("/PayRollStudents/GetPayRollsMoneyForIssuingChecks", function (data) {
                debugger;
            });
        }
    });


    $scope.PayrollsGrid = {
        dataSource: DataSourcePayrollsGrid,
        keyExpr: "PayrollID",
        bindingOptions: {
            rtlEnabled: true
        },
        sorting: {
            mode: "multiple"
        },
        wordWrapEnabled: false,
        showBorders: false,
        searchPanel: {
            width: 1300,
            visible: true,
            placeholder: "بحث"
        }, 
        paging: {
        pageSize: 10
        },
        pager: {
            allowedPageSizes: "auto",
            infoText: "(صفحة  {0} من {1} ({2} عنصر",
            showInfo: true,
            showNavigationButtons: true,
            showPageSizeSelector: true,
            visible: true
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
        groupPanel: {
            visible: false,
            emptyPanelText: "اسحب عمود هنا"
        },
        noDataText: "لايوجد بيانات",
        columnAutoWidth: true,
        columnChooser: {
            enabled: true
        },
        "export": {
            enabled: true,
            fileName: "Payrolls"
        },
        columns: [
            {
                caption: 'رقم المسير',
                cssClass: "text-center",
                dataField: "PayrollNumber"
            },

            {
                dataField: "PayNumber",
                caption: "رقم أمر الصرف"

            },
            {
                dataField: "DafPayNumber",
                caption: "رقم أمر الدفع"
            },
            {
                dataField: "PayrollMoneyValue",
                caption: "قيمة المسير النقدي"
            },
            {
                dataField: "StudentNumber",
                caption: "عدد الطلاب"
            },
            {
                dataField: "LastStageDate",
                caption: "تاريخ أخر مرحلة إعتماد"
            }
            , {
                caption: "التفاصيل",
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    $("<div />").dxButton({
                        text: 'التفاصيل',
                        icon: "fa fa-info",
                        width: 150,
                        type: "success",
                        hint: "التفاصيل",
                        onClick: function (e) {
                            debugger;
                            $scope.ResetControls();
                            $scope.currentKey = options.key;
                            return $http({
                                method: "Get",
                                url: "/PayRollStudents/GetPayRollsMoneyAfterBeingMonterayDetails",
                                params: {
                                    payrollID: options.data.PayrollID
                                }
                            }).then(function (data) {
                                debugger;
                                if (data.data.length > 0) {
                                    $scope.PayRollsMoneyDetails = data.data;
                                    $scope.payNo = data.data[0].PayNumber;
                                    $scope.dafPayNo = data.data[0].DafPayNumber;
                                    //***************
                                    return $http({
                                        method: "Get",
                                        url: "/PayRollStudents/GetFacultiesThatNotRegisteredWithMoneyExchangers",
                                        params: {
                                            payrollID: options.data.PayrollID
                                        }
                                    }).then(function (data1) {
                                        debugger;
                                        $scope.isFacultyMsgShow = false;
                                        if (data1.data[0] != null) {
                                            $scope.isFacultyMsgShow = true;
                                            $scope.faculty = "قيمة إصدار الشيكات للمسير لا تساوي قيمة المسير" + " " + "يوجد كليات " + " " + "(" + data1.data[0] + ")" + " " + "غير مرتبطه ب مسؤولي صرف";
                                        }
                                        $scope.PayRollsMoneyDetailsPopupVisible = true; 
                                        $scope.acceptBtnVisible = true;
                                        $scope.RemainValue = data.data[0].RemainValue;
                                        $scope.ChecksNotHaveFilesCount = data.data[0].ChecksNotHaveFilesCount;
                                        $scope.ChecksIsNotPrintedCount = data.data[0].ChecksIsNotPrinted;

                                        PayRollsMoneyDataGridDataSource.reload();
                                        $('body').css('overflow', 'hidden');
                                    });
                                    //******************************************
                          
                                }
                            });
                        }
                    }).appendTo(container);
                }
            }, {
                caption: "#",
                cssClass: "text-center",
                cellTemplate: function (container, options) { 
                        $("<div />").dxButton({
                            text: 'الطباعه',
                            icon: "fa fa-print",
                            width: 150,
                            type: "default",
                            hint: "الطباعه",
                            onClick: function (e) {
                                debugger;
                                $scope.ResetControls();
                                if (options.data.TotalChecksValues == options.data.PayrollMoneyValue) {
                                    $scope.currentKey = options.key;
                                    $scope.PayrollID = options.data.PayrollID;
                                    $('#PrintCheckPopup').dxPopup('instance').option('visible', true);
                                    $('body').css('overflow', 'hidden');
                                } else {
                                    return $http({
                                        method: "Get",
                                        url: "/PayRollStudents/GetFacultiesThatNotRegisteredWithMoneyExchangers",
                                        params: {
                                            payrollID: options.data.PayrollID
                                        }
                                    }).then(function (data) {
                                        debugger;
                                        $('#PrintCheckPopup').dxPopup('instance').option('visible', false);
                                        if (data.data[0] != null) {
                                            return DevExpress.ui.notify({ message: "قيمة إصدار الشيكات للمسير لا تساوي قيمة المسير" + " " + "يوجد كليات " + " " + "(" + data.data[0] + ")" + " " + "غير مرتبطه ب مسؤولي صرف" }, "Error", 5000);
                                        } else {
                                            return DevExpress.ui.notify({ message: "قيمة إصدار الشيكات للمسير لا تساوي قيمة المسير"  }, "Error", 5000);
                                        }
                                        
                                    });
                                }
                            }
                        }).appendTo(container);
                    //}
                }
            }
        ],
        allowColumnResizing: true,
        columnResizingMode: "widget",
        allowColumnReordering: true,
        showColumnLines: true,
        rowAlternationEnabled: true,
        onInitialized: function (e) {
        }
    };
    var DataSourcePayrollFilesGrid = new DevExpress.data.DataSource({
       
        paginate: true,
        cacheRawData: true,
        key: "ID",
        loadMode: "raw",
        load: function () { 
            if ($scope.FilesDetailsArray.length > 0) {
                debugger;
                return $scope.FilesDetailsArray;
            } else {
                return [];
            }
        }
    });

    $scope.PayrollsFilesGrid = function () {
        return {
            dataSource: DataSourcePayrollFilesGrid,
            keyExpr: "ID",
            bindingOptions: {
                rtlEnabled: true
            },
            sorting: {
                mode: "multiple"
            },
            wordWrapEnabled: false,
            showBorders: false,
            searchPanel: {
                width: 900,
                visible: true,
                placeholder: "بحث"
            },
            paging: {
                pageSize: 10
            },
            pager: {
                allowedPageSizes: "auto",
                infoText: "(صفحة  {0} من {1} ({2} عنصر",
                showInfo: true,
                showNavigationButtons: true,
                showPageSizeSelector: true,
                visible: true
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
            groupPanel: {
                visible: false,
                emptyPanelText: "اسحب عمود هنا"
            },
            noDataText: "لايوجد بيانات",
            columnAutoWidth: true,
            columnChooser: {
                enabled: true
            },
            "export": {
                enabled: true,
                fileName: "Payrolls"
            },
            columns: [
                {
                    caption: 'المرحلة',
                    cssClass: "text-center",
                    dataField: "PhaseName"
                },
                {
                    dataField: "InsertDate",
                    caption: "تاريخ المرحلة"
                },
                {
                    caption: "#",
                    cssClass: "text-center",
                    cellTemplate: function (container, options) {
                        $("<div />").dxButton({
                            text: 'تنزيل الملف',
                            icon: "fa fa-download",
                            width: 150,
                            type: "default",
                            hint: 'تنزيل الملف',
                            onClick: function (e) {
                                debugger;
                                return window.open('/PayrollStudents/DownloadFile?PayrollApprovalId=' + options.data.ID, '_blank');


                            }
                        }).appendTo(container);
                    }
                }
            ]
        };
    };

    $scope.PayRollsMoneyDetails = '';
    var PayRollsMoneyDataGridDataSource = new DevExpress.data.DataSource({
        paginate: true,
        cacheRawData: false,
        loadMode: "raw",
        key: "PayrollID",
        load: function () {
            if ($scope.PayRollsMoneyDetails.length > 0) {
                debugger;
                return $scope.PayRollsMoneyDetails;
            } else {
                return [];
            }
        }
    }); 

    $scope.PayRollsMoneyDataGrid = function () {
        return {
            dataSource: PayRollsMoneyDataGridDataSource,
            "export": {
                enabled: true,
                fileName: "تفاصيل المسير مع مسؤولي الصراف"
            },
            sorting: {
                mode: "multiple"
            },
            columnAutoWidth: true,
            columnChooser: {
                enabled: true
            },
            wordWrapEnabled: true,
            showBorders: true, 
            headerFilter: {
                visible: false
            },
            showRowLines: true,
            groupPanel: {
                visible: false,
                emptyPanelText: "اسحب عمود هنا"
            },
            paging: {
                pageSize: 10
            },
            pager: {
                allowedPageSizes: "auto",
                infoText: "(صفحة  {0} من {1} ({2} عنصر",
                showInfo: true,
                showNavigationButtons: true,
                showPageSizeSelector: true,
                visible: true
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
            noDataText: "لا يوجد بيانات",
            editing: {
                allowUpdating: false,
                allowAdding: false,
                allowDeleting: false,
                mode: "row",
                texts: {
                    confirmDeleteMessage: "",
                    deleteRow: "",
                    editRow: "",
                    addRow: ""
                },
                useIcons: true
            },
            allowColumnResizing: true,
            columnResizingMode: "widget",
            allowColumnReordering: true,
            showColumnLines: true,
            rowAlternationEnabled: true,
            columns: [
                {
                    caption: 'مندوب الصراف',
                    cssClass: "text-center",
                    dataField: "Name"//,
                   // groupIndex: 0
                },
                {
                    dataField: "PayrollNumber",
                    caption: "رقم المسير",
                    alignment: "center",
                    width: 130
                },
                {
                    dataField: "PayNumber",
                    caption: "رقم أمر الصرف",
                    alignment: "center",
                    width: 80
                }, {
                    dataField: "DafPayNumber",
                    caption: "رقم أمر الدفع",
                    alignment: "center",
                    width: 80
                },
                {
                    dataField: "StudentNumber",
                    caption: "عدد الطلاب",
                    alignment: "center",
                    width: 80
                },
                {
                    dataField: "PayrollMoneyValue",
                    caption: "قيمة المسير",
                    alignment: "center",
                    width: 120
                },
                {
                    dataField: "TotalChecksValues",
                    caption: "مبالغ الشيكات",
                    alignment: "center",
                    width: 120
                },
                {
                    dataField: "RemainValue",
                    caption: "مبالغ بدون شيكات",
                    alignment: "center",
                    width: 120
                },
                {
                    dataField: "NoOfChecks",
                    caption: "عدد الشيكات",
                    alignment: "center",
                    width: 120
                },
                {
                    dataField: "CheckNumbers",
                    caption: "أرقام الشيكات التي صدرت",
                    alignment: "center",
                    width: 120
                },
                {
                    caption: "التفاصيل",
                    cssClass: "text-center",
                    allowExporting:false,
                    cellTemplate: function (container, options) {
                        $("<div />").dxButton({
                            text: '',
                            icon: "fa fa-tags",
                            width: 50,
                            type: "default",
                            hint: "التفاصيل",
                            onClick: function (e) {
                                debugger;
                                $scope.ResetControls();
                                $scope.currentKey = options.key;
                                $scope.PayRollsMoneyUserDetailsPopupVisible = true;
                                return $http({
                                    method: "Get",
                                    url: "/PayRollStudents/GetPayRollsMoneyUsersAfterBeingMonterayDetails",
                                    params: {
                                        payrollID: options.data.PayrollID,
                                        userID: options.data.UserID
                                    }
                                }).then(function (data) {
                                    debugger;
                                    $scope.PayRollsMoneyUsersDetails = data.data;
                                    PayRollsMoneyUsersDataGridDataSource.reload();
                                    //***************
                                    $scope.PayRollsMoneyUserDetailsPopupVisible = true;
                                    $('body').css('overflow', 'hidden');
                                });
                            }
                        }).appendTo(container);
                    }
                },
                {
                    caption: "إصدار الشيكات",
                    cssClass: "text-center",
                    allowExporting: false,
                    cellTemplate: function (container, options) {
                        $("<div />").dxButton({
                            text: 'إصدار الشيكات',
                            icon: "fa fa-money",
                            cssClass: "text-center",
                            width: 50,
                            type: "success",
                            hint: "إصدار الشيكات",
                            onClick: function (e) {
                                debugger; 
                                $scope.ResetControls();
                                $scope.FacultionID = null;
                                $scope.currentKey = options.key;
                                $scope.PayRollChecksPopupVisible = true;
                                $scope.userID = options.data.UserID;
                                $scope.payrollID = options.data.PayrollID;
                                $scope.payrollNumber = options.data.PayrollNumber;
                                $scope.PayrollMoneyValue = options.data.PayrollMoneyValue;
                                $.getJSON("/PayRollStudents/GetChecksValues?payrollID=" + options.data.PayrollID + "&userID=" + options.data.UserID, function (data1) {
                                    debugger;
                                    if (data1.length > 0) {

                                        $scope.Check.BenfName = data1[0].username;
                                        $scope.Check.TotalValueWithoutChecks = data1[0].totalvaluewithoutchecks;
                                        $scope.Check.txtCheckValueName = data1[0].totalvaluewithoutchecks;
                                    } else {


                                        $scope.Check.BenfName = '';
                                        $scope.Check.TotalValueWithoutChecks = '';
                                    }
                                });
                                return $http({
                                    method: "Get",
                                    url: "/PayRollStudents/GetPayRollChecks",
                                    params: {
                                        payrollID: options.data.PayrollID,
                                        userID: options.data.UserID
                                    }
                                }).then(function (data) {
                                    debugger;
                                    $scope.PayRollChecksDetails = data.data;
                                    PayRollChecksDataGridDataSource.reload();
                                    //***************
                                    $scope.PayRollChecksPopupVisible = true;
                                    $('body').css('overflow', 'hidden');
                                });
                            }
                        }).appendTo(container);
                    }
                }

            ],
            summary: {
                totalItems: [{
                    column: "StudentNumber",
                    summaryType: "sum",
                    displayFormat: "الإجمالي: {0}",
                }, {
                    column: "PayrollMoneyValue",
                        summaryType: "sum",
                    displayFormat: "الإجمالي: {0}",
                    }, {
                        column: "TotalChecksValues",
                        summaryType: "sum",
                        displayFormat: "الإجمالي: {0}",
                    }, {
                        column: "RemainValue",
                        summaryType: "sum",
                        displayFormat: "الإجمالي: {0}",
                    }]
            }
        };
    };

/*******************************************************************************/



    $scope.PayRollChecksDetails = '';

    var PayRollChecksDataGridDataSource = new DevExpress.data.DataSource({
        paginate: true,
        cacheRawData: false,
        loadMode: "raw",
        load: function () {
            if ($scope.PayRollChecksDetails.length > 0) {
                debugger;
                return $scope.PayRollChecksDetails;
            } else {
                return [];
            }
        }
    });


    $scope.PayRollsChecksGrid = function () {
        return {
            dataSource: PayRollChecksDataGridDataSource,
           keyExpr: "ID",
            sorting: {
                mode: "multiple"
            },
            paginate: true,
            columnAutoWidth: true,
            columnChooser: {
                enabled: true
            },
            wordWrapEnabled: true,
            showBorders: true,
            pager: {
                allowedPageSizes: "auto",
                infoText: "(صفحة  {0} من {1} ({2} عنصر",
                showInfo: true,
                showNavigationButtons: true,
                showPageSizeSelector: true,
                visible: true
            },
            headerFilter: {
                visible: false
            },
            showRowLines: true,
            groupPanel: {
                visible: false,
                emptyPanelText: "اسحب عمود هنا"
            },
            paging: {
            pageSize:10
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
            noDataText: "لا يوجد بيانات",
            editing: {
                allowUpdating: false,
                allowAdding: false,
                allowDeleting: false,
                mode: "row",
                texts: {
                    confirmDeleteMessage: "",
                    deleteRow: "",
                    editRow: "",
                    addRow: ""
                },
                useIcons: true
            },
            allowColumnResizing: true,
            columnResizingMode: "widget",
            allowColumnReordering: true,
            showColumnLines: true,
            rowAlternationEnabled: false,
            "export": {
                enabled: true,
                fileName: "تفاصيل اصدار الشيكات"
            },
            columns: [
                {
                    caption: 'مندوب الصرف',
                    dataField: "Name",
                    groupIndex: 0

                },
                {
                    caption: 'التاريخ',
                    dataField: "ExportDate"
                },
                {
                    dataField: "CheckNumber",
                    caption: "رقم الشيك"
                },
                {
                    dataField: "CheckValue",
                    caption: "مبلغ الشيك"
                },
                {
                    dataField: "Description",
                    caption: "بيان الشيك"
                },
                {
                    dataField: "IsReceived",
                    caption: "حالة الشيك"
                },
                {
                    caption: "تعديل الشيك ",
                    width: 100,
                    cssClass: "text-center",
                    allowExporting: false,
                    cellTemplate: function (container, options) {
                        debugger;
                        if (options.data.IsActive == true && (options.data.IsPrinted == null || options.data.IsPrinted == false)) {
                            $("<div />").dxButton({
                                icon: "fa fa-pencil",
                                type: "warning",
                                hint: "تعديل الشيك ",
                                elementAttr: { "class": "btn btn-warning btn-sm" },
                                onClick: function (e) {

                                    debugger;
                                    PayrollApprovalService.GetPayrollCheckById(options.data.ID).then(function (result) {
                                        if (result) {
                                            debugger;
                                            $scope.Check.ID = result.data.ID;
                                            $scope.Check.PayrollID = result.data.PayrollID;
                                            $scope.Check.BeneficiaryID = result.data.BeneficiaryID;
                                            $scope.Check.Description = result.data.Description;
                                            $scope.Check.byanTextAreaValue = result.data.Description;
                                            $scope.txtCheckValueName = result.data.CheckValue;
                                            $scope.Check.txtCheckValueName = result.data.CheckValue;
                                            $scope.Check.TotalValueWithoutChecks = result.data.CheckValue;
                                            $scope.txtCheckNumberName = result.data.CheckNumber;
                                            $scope.Check.txtCheckNumberName = result.data.CheckNumber;
                                            $scope.Check.FacultyID = result.data.FacultyID;
                                            $scope.FacultionIds = result.data.FacultyID;
                                            var arr = result.data.FacultyID.toString().split(",");
                                            $scope.FacultionID = arr;
                                        }
                                    });
                                }
                            }).appendTo(container);
                        }
                    }
                },
                {
                    caption: "إلغاء الشيك",
                    cssClass: "text-center",
                    allowExporting: false,
                    cellTemplate: function (container, options) {
                       if (options.data.IsActive == true) {
                        $("<div />").dxButton({
                            text: 'إلغاء الشيك',
                            icon: "fa fa-remove",
                            width: 50,
                            type: "default",
                            hint: "إلغاء الشيك",
                            onClick: function (e) {

                                debugger;
                                $('#ConfirmCancelCheckPopup').dxPopup('instance').option('visible', true);
                                $scope.CheckID = options.data.ID;
                                $scope.PayrollID = options.data.PayrollID;
                                $scope.UserID = options.data.BeneficiaryID;
                                //PayrollApprovalService.cancelCheck(options.data.ID).then(function (data) {
                                //    if (data.data !== "")
                                //        DevExpress.ui.notify(data.data, "error", 5000);
                                //    else {
                                //        $.getJSON("/PayRollStudents/GetChecksValues?payrollID=" + options.data.PayrollID + "&userID=" + options.data.BeneficiaryID, function (data1) {
                                //            debugger;
                                //            if (data1.length > 0) {

                                //                $scope.Check.BenfName = data1[0].username;
                                //                $scope.Check.TotalValueWithoutChecks = data1[0].totalvaluewithoutchecks;
                                //                $scope.Check.txtCheckValueName = data1[0].totalvaluewithoutchecks;
                                //            } else {


                                //                $scope.Check.BenfName = '';
                                //                $scope.Check.TotalValueWithoutChecks = '';
                                //            }
                                //        });
                                //        return $http({
                                //            method: "Get",
                                //            url: "/PayRollStudents/GetPayRollChecks",
                                //            params: {
                                //                payrollID: options.data.PayrollID,
                                //                userID: options.data.BeneficiaryID
                                //            }
                                //        }).then(function (data) {
                                //            debugger;
                                //            $scope.PayRollChecksDetails = data.data;
                                //            PayRollChecksDataGridDataSource.reload();
                                //            //***************
                                //            DevExpress.ui.notify("تم الألغاء بنجاح", "success", 5000);
                                //            $scope.txtTotalValueWithoutChecksNamereadOnly = true;
                                //            $scope.txtCheckValueNamereadOnly = false;
                                //            $scope.Check.ID = 0;
                                //            $scope.Check.FacultyID = '';
                                //            $scope.Check.txtCheckNumberName = '';
                                //            $scope.Check.byanTextAreaValue = '';
                                //            $scope.FacultionID = null;
                                //        });
                                //    }
                                //});

                            }

                        }).appendTo(container);
                    }
                }
                },
                {
                    caption: "معايتة الشيك",
                    cssClass: "text-center",
                    allowExporting: false,
                    cellTemplate: function (container, options) {
                        if (options.data.IsActive == true) {
                            $("<div />").dxButton({
                                text: 'معاينة قبل الطباعه',
                                icon: "fa fa-print",
                                width: 50,
                                type: "success",
                                hint: "معاينة قبل الطباعه",
                                onClick: function (e) {
                                    window.open("/Reports/CheckReportIndex?CheckID=" + options.data.ID, '_blank');
                                }
                            }).appendTo(container);
                        }
                    }
                },
                {
                    caption: "رفع مرفق الشيك ",
                    width: 100,
                    cssClass: "text-center",
                    allowExporting: false,
                    cellTemplate: function (container, options) {
                        debugger;
                        if (options.data.IsActive == true) {
                            $("<div />").dxButton({
                                icon: "fa fa-upload",
                                type: "default",
                                hint: "رفع مرفق الشيك ",
                                elementAttr: { "class": "btn btn-warning btn-sm" },
                                onClick: function (e) {
                                    debugger;
                                    $scope.MDL_UploadingFilesvalue1 = '';
                                    $scope.UploadingFilesvalue1 = [];
                                  //  $scope.UploadPopUpShow1 = true;
                                    $('#UploadPopUpShow1').dxPopup('instance').option('visible', true);
                                    $scope.ID = options.data.ID;
                                }
                            }).appendTo(container);
                        }
                    }
                }
                ,
                {
                    caption: "تحميل مرفق الشيك ",
                    width: 100,
                    cssClass: "text-center",
                    allowExporting: false,
                    cellTemplate: function (container, options) {
                        debugger;
                        if (options.data.FilePath != null /*|| options.data.FilePath != '' || options.data.FilePath != undefined*/ && options.data.IsActive==true) {
                            $("<div />").dxButton({
                                icon: "fa fa-download",
                                type: "default",
                                hint: "تحميل مرفق الشيك ",
                                elementAttr: { "class": "btn btn-warning btn-sm" },
                                onClick: function (e) {
                                    return window.open('/PayrollStudents/DownloadCheckFile?ID=' + options.data.ID, '_blank');
                                 }
                            }).appendTo(container);
                        }
                    }
                }
            ],
            onRowPrepared: function (info) {
                debugger;
             
                if (info.rowType === 'data') {
                    if (info.data.IsActive == false) {
                        info.rowElement.attr('style', 'background-color: #F4CACA !important;');
                    }
                }
            }
        };
    };

















    /***************************************************************************/
    $scope.PayRollsMoneyUsersDetails = '';
    
    var PayRollsMoneyUsersDataGridDataSource = new DevExpress.data.DataSource({
        paginate: true,
        cacheRawData: false,
        loadMode: "raw",
        key: "PayrollID",
        load: function () {
            if ($scope.PayRollsMoneyUsersDetails.length > 0) {
                debugger;
                return $scope.PayRollsMoneyUsersDetails;
            } else {
                return [];
            }
        }
    });



    $scope.PayRollsMoneyUserDetailsGrid = function () {
        return {
            dataSource: PayRollsMoneyUsersDataGridDataSource,
            keyExpr: "ID",
            sorting: {
                mode: "multiple"
            },
            columnAutoWidth: true,
            columnChooser: {
                enabled: true
            },
            wordWrapEnabled: false,
            showBorders: true,
             
            headerFilter: {
                visible: false
            },
            showRowLines: true,
            groupPanel: {
                visible: false,
                emptyPanelText: "اسحب عمود هنا"
            }, 
            paging: {
            pageSize: 10
            },
            pager: {
                allowedPageSizes: "auto",
                infoText: "(صفحة  {0} من {1} ({2} عنصر",
                showInfo: true,
                showNavigationButtons: true,
                showPageSizeSelector: true,
                visible: true
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
            noDataText: "لا يوجد بيانات",
            editing: {
                allowUpdating: false,
                allowAdding: false,
                allowDeleting: false,
                mode: "row",
                texts: {
                    confirmDeleteMessage: "",
                    deleteRow: "",
                    editRow: "",
                    addRow: ""
                },
                useIcons: true
            },
            allowColumnResizing: true,
            columnResizingMode: "widget",
            allowColumnReordering: true,
            showColumnLines: true,
            rowAlternationEnabled: true,
            "export": {
                enabled: true,
                fileName: "مسؤول الصرافة وربطه بالكليات والمراحل العلمية للمسير"
            },
            columns: [
                {
                    caption: 'مندوب الصراف',
                    cssClass: "text-center",
                    dataField: "name",
                    groupIndex: 0
                },
                {
                    dataField: "PayrollNumber",
                    caption: "رقم المسير"
                },
                {
                    dataField: "faculty_name",
                    caption: "الكليه"
                },
                {
                    dataField: "DEGREE_DESC",
                    caption: "المرحله العلمية"
                },
                {
                    dataField: "PayrollMoneyValue",
                    caption: "قيمة المسير"
                },
                {
                    dataField: "StudentNumber",
                    caption: "عدد الطلاب"
                }
            ],
            summary: {
                totalItems: [{
                    column: "StudentNumber",
                    summaryType: "sum",
                    displayFormat: "الإجمالي: {0}",
                }, {
                        column: "PayrollMoneyValue",
                        summaryType: "sum",
                    displayFormat: "الإجمالي: {0}",
                }
                    // ...
                ]
            }
                //groupItems: [{
                //   column: "StudentNumber",
                //    summaryType: "count",
                //    displayFormat: "Total: {0}",
                //    showInGroupFooter: true,

                //},{
                //        column: "PayrollMoneyValue",
                //        summaryType: "sum",
                //        displayFormat: "Total: {0}",
                //        showInGroupFooter: true,

                //    }]
        };
    };

     
/*omar*/
     
    $scope.PayRollsMoneyUserDetailsPopup = {
        bindingOptions: {
            visible: "PayRollsMoneyUserDetailsPopupVisible"
        },
        shadingColor: "rgba(0, 0, 0, 0.69)",
        closeOnOutsideClick: true,
        height: 600,
        width: 1300,
        showTitle: false,
        title: "",
        dragEnabled: false,
        onHiding: function () {
            debugger;
        }
    };


    $scope.PayRollChecksPopup = {
        bindingOptions: {
            visible: "PayRollChecksPopupVisible"
        },
        shadingColor: "rgba(0, 0, 0, 0.69)",
        closeOnOutsideClick: false,
        title: "إصدار الشيكات",
        width: 1200,
        height: 800,
        contentTemplate: "PayRollsCheckContent",
        showTitle: true,
        dragEnabled: false,
        onShown: function (options) {
            debugger;
            $scope.MDL_UploadingFilesvalue1 = '';
        },
        UploadFilePopupOptions1:{
            showTitle: true,
            dragEnabled: false,
            shadingColor: "rgba(0, 0, 0, 0.69)",
            closeOnOutsideClick: false,
            bindingOptions: {
                visible: "UploadPopUpShow1"
            },
            contentTemplate: 'UploadFileContent1',
            title: "رفع الملف",
            width: 800,
            height: 500,
            rtlEnabled: true,
            onShowing: function () {
            }
        },
        onHiding: function (options) {
            debugger;
            $scope.Check = {};
            return $http({
                method: "Get",
                url: "/PayRollStudents/GetPayRollsMoneyAfterBeingMonterayDetails",
                params: {
                    payrollID: $scope.payrollID
                }
            }).then(function (data) {
                debugger;
                $scope.PayRollsMoneyDetails = data.data;
                $scope.payNo = data.data[0].PayNumber;
                $scope.dafPayNo = data.data[0].DafPayNumber;
                //***************
                $scope.PayRollsMoneyDetailsPopupVisible = true;
                $scope.acceptBtnVisible = true;
                $scope.RemainValue = data.data[0].RemainValue;
                $scope.ChecksNotHaveFilesCount = data.data[0].ChecksNotHaveFilesCount;
                $scope.ChecksIsNotPrintedCount = data.data[0].ChecksIsNotPrinted;

                PayRollsMoneyDataGridDataSource.reload();
                $('body').css('overflow', 'hidden');
            });
        }
    };

    $scope.PrintCheckPopup = {
        bindingOptions: {
            visible: "PrintCheckPopupVisible"
        },
        shadingColor: "rgba(0, 0, 0, 0.69)",
        closeOnOutsideClick: true,
        height: 200,
        width: 750,
        showTitle: false,
        title: "",
        dragEnabled: false,
        onHiding: function () {
            debugger; 
            $scope.currentKey = null;
            $scope.ResetControls();
            DataSourcePayrollsGrid.reload();
            $('body').css('overflow', 'auto');
        }
    };
    $scope.printButton = {
        text: "طباعة",
        type: "success",
        icon: "fa fa-print",
        width: "150",
        onClick: function (e) {
            window.open("/Reports/AllChecksReportIndex?payrollID=" + $scope.PayrollID, '_blank');
        }
    };

    $scope.printSandButton = {
        text: "طباعة خطاب تسليم الشيكات",
        type: "default",
        icon: "fa fa-print",
        width: "250",
        onClick: function (e) {
           window.open("/Reports/SandCheckReportIndex?payrollID=" + $scope.PayrollID, '_blank');
        }
    };
    $scope.printLetterButton = {
        text: "طباعة سندات إستلام الشيكات",
        type: "default",
        icon: "fa fa-print",
        width: "250",
        onClick: function (e) {
            window.open("/Reports/SandEstlamCheckReportIndex?payrollID=" + $scope.PayrollID, '_blank');
        }
    };




    $scope.ConfirmCancelCheckPopup = {
        bindingOptions: {
            visible: "ConfirmCancelCheckPopupVisible"
        },
        shadingColor: "rgba(0, 0, 0, 0.69)",
        closeOnOutsideClick: true,
        height: 200,
        width: 600,
        showTitle: false,
        title: "هل أنت متأكد من الإلغاء",
        dragEnabled: false
    };


 


    $scope.saveBtn = {
        text: "نعم",
        type: "success",
        icon: "fa fa-save",
        width: "150",
        onClick: function (e) {
            PayrollApprovalService.cancelCheck($scope.CheckID).then(function (data) {
                                    if (data.data !== "")
                                        DevExpress.ui.notify(data.data, "error", 5000);
                                    else {
                                        $.getJSON("/PayRollStudents/GetChecksValues?payrollID=" + $scope.PayrollID + "&userID=" + $scope.UserID, function (data1) {
                                            debugger;
                                            if (data1.length > 0) {

                                                $scope.Check.BenfName = data1[0].username;
                                                $scope.Check.TotalValueWithoutChecks = data1[0].totalvaluewithoutchecks;
                                                $scope.Check.txtCheckValueName = data1[0].totalvaluewithoutchecks;
                                            } else {
                                                $scope.Check.BenfName = '';
                                                $scope.Check.TotalValueWithoutChecks = '';
                                            }
                                        });
                                        return $http({
                                            method: "Get",
                                            url: "/PayRollStudents/GetPayRollChecks",
                                            params: {
                                                payrollID: $scope.PayrollID,
                                                userID: $scope.UserID
                                            }
                                        }).then(function (data) {
                                            debugger;
                                            $scope.PayRollChecksDetails = data.data;
                                            PayRollChecksDataGridDataSource.reload();
                                            //***************
                                            DevExpress.ui.notify("تم الألغاء بنجاح", "success", 5000);
                                            $('#ConfirmCancelCheckPopup').dxPopup('instance').option('visible', false);
                                            $scope.txtTotalValueWithoutChecksNamereadOnly = true;
                                            $scope.txtCheckValueNamereadOnly = false;
                                            $scope.Check.ID = 0;
                                            $scope.Check.FacultyID = '';
                                            $scope.Check.txtCheckNumberName = '';
                                            $scope.Check.byanTextAreaValue = '';
                                            $scope.FacultionID = null;
                                        });
                                    }
                                });
        }
    };
    $scope.cancelBtn = {
        text: "لا",
        type: "default",
        icon: "fa fa-cancel",
        width: "150",
        onClick: function (e) {
            $('#ConfirmCancelCheckPopup').dxPopup('instance').option('visible', false);
        }
    };

    $scope.PayRollsMoneyDetailsPopup = {
        bindingOptions: {
            visible: "PayRollsMoneyDetailsPopupVisible"
        },
        shadingColor: "rgba(0, 0, 0, 0.69)",
        closeOnOutsideClick: true,
        height: 800,
        width: 1300,
        showTitle: false,
        title: "",
        dragEnabled: false,
        onHiding: function () {
            debugger;
            $scope.PayRollsMoneyDetails = [];
            $scope.currentKey = null;
            $scope.ResetControls();
            DataSourcePayrollsGrid.reload();
            $('body').css('overflow', 'auto');
        },
        onShown: function (options) {
            debugger;
            $scope.MDL_UploadingFilesvalue = '';
        },
        UploadFilePopupOptions: {
            showTitle: true,
            dragEnabled: false,
            shadingColor: "rgba(0, 0, 0, 0.69)",
            closeOnOutsideClick: false,
            bindingOptions: {
                visible: "UploadPopUpShow"
            },
            contentTemplate: 'UploadFileContent',
            title: "رفع الملف",
            width: 800,
            height: 500,
            rtlEnabled: true,
            onShowing: function () {
            }
        },
        filesPopup: {
            showTitle: true,
            dragEnabled: false,
            shadingColor: "rgba(0, 0, 0, 0.69)",
            closeOnOutsideClick: false,
            bindingOptions: {
                visible: "filesPopupVisible"
            },
            height: 500,
            title: "تحميل المرفقات للمراحل السابقه"
        },
        textAreaPopup: {
            showTitle: true,
            dragEnabled: false,
            shadingColor: "rgba(0, 0, 0, 0.69)",
            closeOnOutsideClick: false,
            bindingOptions: {
                visible: "textAreaVisible"
            },
            title: "الملاحظات",
            width: 1200,
            height: 500,
            rtlEnabled: true
        }
        //,PayRollsMoneyUserDetailsPopup : {
        //    bindingOptions: {
        //        visible: "PayRollsMoneyUserDetailsPopupVisible"
        //    },
        //    shadingColor: "rgba(0, 0, 0, 0.69)",
        //    closeOnOutsideClick: true,
        //    height: 600,
        //    width: 1300,
        //    showTitle: false,
        //    title: "",
        //    dragEnabled: false,
        //    onHiding: function () {
        //        debugger;
        //    }
        //}
    };

    $scope.btnShowFiles = {
        text: "إستعراض المرفقات",
        type: "default",
        width: "150",
        onClick: function (e) { 
            $scope.FilesDetailsArray = '';
            debugger;
            //$scope.currentKey = options.key; 
            $scope.payrollID = $scope.currentKey;
        /*Omar*/
            return $http({
                method: "Get",
                url: "/PayRollStudents/GetPayRollFiles",
                params: {
                    payrollID: $scope.payrollID
                }
            }).then(function (data) {
                debugger;
                $scope.FilesDetailsArray = data.data;
                DataSourcePayrollFilesGrid.reload(); 
                $scope.filesPopupVisible = true;
                $('body').css('overflow', 'hidden');
            });
        }
    };
    //
    $scope.acceptButton = {
        text: "اعتماد",
        type: "success",
        width: "150",
        bindingOptions: {
            visible: "acceptBtnVisible"
        },
        onClick: function (e) {
            debugger;
            $scope.recommendationNotes = '';
            $scope.textAreaVisible = true;
        }
    };


    $scope.saveButton = {
        text: "حفظ",
        type: "success",
        width: "150",
        onClick: function (e) {
            debugger;
            if ($scope.RemainValue != 0) {

                return DevExpress.ui.notify({ message: "قيمة إصدار الشيكات للمسير لا تساوي قيمة المسير" }, "Error", 5000);
            }
            //if ($scope.ChecksNotHaveFilesCount > 0) {

            //    return DevExpress.ui.notify({ message: "لابد من رفع مرفق لكل شيك مفعل" }, "Error", 5000);
            //}
            if ($scope.ChecksIsNotPrintedCount > 0) {

                return DevExpress.ui.notify({ message: "لابد من طباعة الشيكات" }, "Error", 5000);
            }
            if (($scope.MDL_UploadingFilesvalue === '' || $scope.MDL_UploadingFilesvalue === null || $scope.MDL_UploadingFilesvalue === undefined)
                || ($scope.IsUploaded != true || $scope.IsUploaded === undefined || $scope.IsUploaded === null))
             {
                 return DevExpress.ui.notify({ message: " (لابد من رفع الملف المرفق)" }, "Error", 5000);
             }
            
            if ($scope.recommendationNotes == '' || $scope.recommendationNotes == undefined) {

                return DevExpress.ui.notify({ message: " (لابد من كتابة الملاحظات)" }, "Error", 5000);
            }
            PayrollApprovalService.PayrollApprovallAction($scope.currentKey, $scope.recommendationNotes, $scope.payNo, $scope.dafPayNo,true,true)
                .then(function (data) {
                    if (data.data === "") {
                        $scope.textAreaVisible = false;
                        $scope.PayRollsMoneyDetailsPopupVisible = false;
                        DataSourcePayrollsGrid.reload();
                        swal("تم!", "تم الاعتماد بنجاح", "success").then((value) => {
                            $scope.ResetControls();
                            return value;
                        });
 
                    } else {
                        swal("حدث خطأ", data.data, "error");
                    }
                });
        }
    };
    $scope.recommendationsTextArea = {
        bindingOptions: {
            required: "isRequiredNotes",
            value: "recommendationNotes"
        },
        inputAttr: {"rows":10},
        AutoResizeEnabled: true,
        placeholder:"ملاحظات"
    };
    $scope.payNoText = {
        bindingOptions: {
            required: "isRequiredpayNo",
            value: "payNo",
            visible: "payNoTextVisible"
        }, 
        AutoResizeEnabled: true,
        placeholder: "رقم أمر الصرف"
    };
    $scope.dafPayNoText = {
        bindingOptions: {
            value: "dafPayNo",
            visible: "dafPayNoTextVisible"
        },
        AutoResizeEnabled: true,
        placeholder: "رقم أمر الدفع"
    };
    $scope.filesPopup = {
        bindingOptions: {
            visible: "filesPopupVisible"
        },
        shadingColor: "rgba(0, 0, 0, 0.69)",
        closeOnOutsideClick: false,
        height: 500,
        showTitle: true,
        title: "تحميل المرفقات للمراحل السابقه",
        dragEnabled: false
    };
    //==========================================================
    // الجزء الخاص برفع المرفقات والتابعة لشاشة إضافة مشاركة
    //==========================================================
    $scope.uploadMode = "useButtons";
    $scope.btnUploadText = "رفع المرفق";
    $scope.btnUploadOptions = {
        bindingOptions: {
            text: 'btnUploadText',
            icon: 'btnUploadIcon'
        },
        visible: true,
        type: 'default',
        useSubmitBehavior: false,
        onClick: function (e) {
            debugger;
            $scope.MDL_UploadingFilesvalue = '';
            $scope.UploadingFilesvalue = [];  
            $scope.UploadPopUpShow = true;
        }
    };

    //Upload Options...
    $scope.multiple = false;
    $scope.accept = ".pdf";
    $scope.btnUploadIcon = "upload";
    $scope.UploadingFilesvalue = [];
    $scope.FileUploadingOptions = {
        name: "fileSent",
        uploadUrl: "/PayRollStudents/UploadFiles",
        allowCanceling: true,
        rtlEnabled: true,
        readyToUploadMessage: "استعداد للرفع",
        selectButtonText: "رفع المرفق",
        labelText: "",
        uploadMode: "useButtons",
        uploadButtonText: "رفع",
        uploadedMessage: "تم الرفع",
        invalidFileExtensionMessage: "نوع الملف غير مسموح",
        uploadFailedMessage: "نوع الملف غير مسموح",
        allowedFileExtensions: [".pdf"],
        accept: ".pdf",
        multiple: false,
        bindingOptions: {
            value: "UploadingFilesvalue"
        },
        onValueChanged: function (e) {
            debugger;
            if (e.value.length > 0) {
                if (e.value[0].type === "application/pdf") {
                    $scope.MDL_UploadingFilesvalue = e.value;
                } else {
                    $scope.MDL_UploadingFilesvalue = '';
                    $scope.FileUploadingOptionsInstance.reset();
                    return swal("تنبيه", "غير مسموح برفع هذا النوع من الملفات يسمح فقط بإمتداد" + ".pdf", "warning");
                }
            } else {
                $scope.MDL_UploadingFilesvalue = e.value;
            }
        },
        onInitialized: function (e) {
            debugger;
            //$scope.MDL_UploadingFilesvalue = '';
            $scope.FileUploadingOptionsInstance = e.component;
        },
        onUploaded: function (e) {
            debugger;
            if (e.request.status === 200) {
                debugger;
                $scope.IsUploaded = true;
                $scope.UploadPopUpShow = false;
                $scope.btnUploadText = "تم رفع المرفق بنجاح";
                $scope.btnUploadIcon = "check";
              
            }
            if (e.request.status === 400) {
                $scope.UploadPopUpShow = false;
                return DevExpress.ui.notify({ message: "غير مسموح برفع هذا الملف" }, "Error", 10000);
            }

        }
    };

    //Remove Uploaded File...
    $scope.RemoveUploadingFile = function (hashkey) {
        $scope.MDL_UploadingFilesvalue = '';
        $scope.FileUploadingOptionsInstance.reset();
        $http({
            method: "POST",
            url: "/PayRollStudents/UploadFiles"
        });
        $scope.btnUploadText = "رفع المرفق";
        $scope.btnUploadIcon = "upload";
    };
    $scope.ResetUploadingControls = function () {

        $scope.MDL_UploadingFilesvalue = '';
        if ($scope.FileUploadingOptionsInstance) {
            $scope.FileUploadingOptionsInstance.reset();
        }
    };
    /*******************************************************omar_19/12/2019*******************************************************/
    //$scope.UploadFilePopupOptions1 = {
    //    showTitle: true,
    //    dragEnabled: false,
    //    shadingColor: "rgba(0, 0, 0, 0.69)",
    //    closeOnOutsideClick: false,
    //    bindingOptions: {
    //        visible: "UploadPopUpShow1"
    //    },
    //    contentTemplate: 'UploadFileContent1',
    //    title: "رفع الملف",
    //    width: 800,
    //    height: 500,
    //    rtlEnabled: true,
    //    onShowing: function () {
    //    }
    //};
    $scope.btnUploadText1 = "رفع المرفق";
    //Upload Options...
    $scope.multiple = false;
    $scope.accept = ".jpg,.jpeg,.png,.tif,.gif";
    $scope.btnUploadIcon1 = "upload";
    $scope.UploadingFilesvalue1 = [];
    $scope.FileUploadingOptions1 = {
        name: "fileSent",
        uploadUrl: "/PayRollStudents/UploadCheckFiles",
        allowCanceling: true,
        rtlEnabled: true,
        readyToUploadMessage: "استعداد للرفع",
        selectButtonText: "رفع المرفق",
        labelText: "",
        uploadMode: "useButtons",
        uploadButtonText: "رفع",
        uploadedMessage: "تم الرفع",
        invalidFileExtensionMessage: "نوع الملف غير مسموح",
        uploadFailedMessage: "نوع الملف غير مسموح",
        allowedFileExtensions: [".jpg", ".jpeg", ".tif", ".png",".gif"],
        accept: ".jpg,.jpeg,.png,.tif,.gif",
        multiple: false,
        bindingOptions: {
            value: "UploadingFilesvalue1"
        },
        onValueChanged: function (e) {
            debugger;
            if (e.value.length > 0) {
                 $scope.MDL_UploadingFilesvalue1 = e.value;
                //if (e.value[0].type === "application/pdf")
                if ((e.value[0].type === "image/jpeg")
                    || (e.value[0].type === "image/jpg")
                    || (e.value[0].type === "image/tif")
                    || (e.value[0].type === "image/png")
                    || (e.value[0].type === "image/gif")
                )
                {
                    $scope.MDL_UploadingFilesvalue1 = e.value;
                } else {
                    $scope.MDL_UploadingFilesvalue1 = '';
                    $scope.FileUploadingOptionsInstance1.reset();
                    return swal("تنبيه", "غير مسموح برفع هذا النوع من الملفات يسمح فقط بإمتداد" + e.value[0].type.toString() , "warning");
                }
            } else {
                $scope.MDL_UploadingFilesvalue1 = e.value;
            }
        },
        onInitialized: function (e) {
            debugger;
            //$scope.MDL_UploadingFilesvalue = '';
            $scope.FileUploadingOptionsInstance1 = e.component;
        },
        onUploaded: function (e) {
            debugger;
            if (e.request.status === 200) {
                debugger;
                $scope.IsUploaded1 = true;
                //$scope.UploadPopUpShow1 = false;
                $scope.btnUploadText1 = "تم رفع المرفق بنجاح";
                $scope.btnUploadIcon1 = "check";

            }
            if (e.request.status === 400) {
                //$scope.UploadPopUpShow1 = false;
                $('#UploadPopUpShow1').dxPopup('instance').option('visible', false);
                return DevExpress.ui.notify({ message: "غير مسموح برفع هذا الملف" }, "Error", 10000);
            }

        }
    };

    //Remove Uploaded File...
    $scope.RemoveUploadingFile1 = function (hashkey) {
        $scope.MDL_UploadingFilesvalue1 = '';
        $scope.FileUploadingOptionsInstance1.reset();
        $http({
            method: "POST",
            url: "/PayRollStudents/UploadCheckFiles"
        });
        $scope.btnUploadText1 = "رفع المرفق";
        $scope.btnUploadIcon1 = "upload";
    };
    $scope.ResetUploadingControls1 = function () {

        $scope.MDL_UploadingFilesvalue1 = '';
        if ($scope.FileUploadingOptionsInstance1) {
            $scope.FileUploadingOptionsInstance1.reset();
        }
    };
    $scope.SaveUploadingFile1 = function (hashkey) {
        debugger;
        //$scope.MDL_UploadingFilesvalue1 = '';
        //$scope.FileUploadingOptionsInstance1.reset();
        $http({
            method: "POST",
            url: "/PayRollStudents/SaveCheckFiles",
            data: {
                "ID": $scope.ID
            }
        }).then(function (data) {
            debugger;
            if (data.data != "") {
                DevExpress.ui.notify(data.data, "error", 5000);
            } else {

                DevExpress.ui.notify("تم الحفظ بنجاح", "success", 5000);
               // $scope.UploadPopUpShow1 = false;
                $('#UploadPopUpShow1').dxPopup('instance').option('visible', false);
            }

        });
        //
    };

    /*******************************************************end omar_19/12/2019 *******************************************************/

    $scope.validationRequired = {
        validationRules: [
            {
                type: "required",
                message: "الحقل مطلوب"
            }
        ]
    };

    $scope.ResetControls = function () { 
        $scope.MDL_UploadingFilesvalue = '';
        $scope.btnUploadIcon = "upload";
        $scope.btnUploadText = "رفع المرفق";
        $scope.acceptPopupVisible = false;
        $scope.isRequiredNotes = false;
        $scope.isRequiredpayNo = false;
        $scope.currentKey = 0;
        $scope.PayrollMoneyValue = 0;
        $scope.FilesDetailsArray = '';
        $scope.MDL_UploadingFilesvalue = '';
        $scope.MDL_UploadingFilesvalue1 = '';
        $scope.payrollID = null;
        $scope.Check = { ID: 0, BenfName: "", BeneficiaryID: "", PayrollID: "", FacultyID: "", txtCheckNumberName: "", txtCheckValueName: "", TotalValueWithoutChecks: "", byanTextAreaValue: '' };

    };



}
 ]);
