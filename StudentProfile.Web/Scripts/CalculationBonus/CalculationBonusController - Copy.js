app.controller("CalculationBonusCtrl", ["$scope", 'CalculationBonusSrvc', function ($scope, CalculationBonusSrvc) {

    //DDL
    $scope.PayRollList = [];

    $scope.FacultionList = [];
    $scope.FacultionID = null;
    $scope.FacultionIds = null;
    $scope.loadOptions = {};
    $scope.DegreeList = [];
    $scope.DegreeID = null;
    $scope.DegreeIds = null;

    $scope.StudentList = [];
    var StudentList = [];
    $scope.StudentId = null;
    $scope.StudentIds = null;


    $scope.PayRollNumber = null;
    $scope.BounsDate = new Date();

    $scope.visiblePopup = false;

    $scope.loadOptions = {};

    $scope.MDL_Allowance = '';
    $scope.RewardItemsSelectedValues = '';
    $scope.StudentSelectedValues = '';
    $scope.DataSourceRewardItems = '';

    $scope.studentCount = '';
    $scope.message = 'تم إجراء عملية الاحتساب بنجاح';

    // DataSource
    $scope.Listradiogroup = ["احتساب آلي", "احتساب عند الطلب"];
    $scope.Listradiogroup2 = ["احتساب لطالب واحد", "احتساب لكل الطلاب"];

    //Control

    $scope.DL_RewardItems = {
        bindingOptions: {
            value: "MDL_Allowance",
            items: "DataSourceRewardItems"
        },
        displayExpr: "AllowanceName",
        valueExpr: "AllowanceId",
        searchEnabled: true,
        showClearButton: true,
        rtlEnabled: true,
        placeholder: "--اختر--",
        noDataText: "لا يوجد بيانات",
        multiline: false,
        showBorders: true,
        showSelectionControls: true,
        maxDisplayedTags: 1,
        paginate: true,
        showDropDownButton: true,
        onValueChanged: function (e) {
            CalculationBonusSrvc.CheckIfAdvanceItemAlreadySelected(e.value).then(function (data) {
                debugger;
                if (!data.data) {
                    $scope.RewardItemsSelectedValues = e.value.join(',');
                }
            });

        },
        onClosed: function (e) {
        },
        onInitialized: function (e) {
            $scope.DL_RewardItemsInstance = e.component;
            CalculationBonusSrvc.GetAllRewardItems({ RewardItemExpensesType: null }).then(function (data) {
                $scope.DataSourceRewardItems = data.data;
                $scope.MDL_Allowance = data.data.map(x => x.AllowanceId);
            });
        },
        onContentReady: function (e) {

        }
    };

    $(function () {
        $("#widget").dxRadioGroup({
            layout: "horizontal",
            items: $scope.Listradiogroup,
            value: $scope.Listradiogroup[0],
            onOptionChanged: function (e) {
                $scope.RewardItemsSelectedValues = '';
                $scope.MDL_Allowance = '';
            },
            onValueChanged: function (e) {
                if (e.value === $scope.Listradiogroup[0]) {

                    $scope.RewardItemsSelectedValues = '';
                    $scope.MDL_Allowance = '';
                    CalculationBonusSrvc.GetAllRewardItems({ RewardItemExpensesType: null }).then(function (data) {
                        $scope.DataSourceRewardItems = data.data;
                        $scope.MDL_Allowance = data.data.map(x => x.AllowanceId);
                    });
                }
                else {
                    $scope.RewardItemsSelectedValues = '';
                    $scope.MDL_Allowance = '';
                    CalculationBonusSrvc.GetAllRewardItems({ RewardItemExpensesType: "عند الطلب" }).then(function (data) {
                        $scope.DataSourceRewardItems = data.data;

                        $scope.MDL_Allowance = '';
                        $scope.RewardItemsSelectedValues = '';
                    });
                }
            },
            onInitialized: function (e) {
            }
        });


        $("#widget2").dxRadioGroup({
            layout: "horizontal",
            items: $scope.Listradiogroup2,
            value: $scope.Listradiogroup2[0],
            onOptionChanged: function (e) {
                debugger;
                $scope.StudentSelectedValues = '';
            },
            onValueChanged: function (e) {
                debugger;
                if (e.value === $scope.Listradiogroup2[0]) {
                    debugger;
                    $scope.StudentSelectedValues = '';
                    CalculationBonusSrvc.GetAllRewardItems({ RewardItemExpensesType: null }).then(function (data) {
                        $scope.DataSourceRewardItems = data.data;
                        $scope.MDL_Allowance = data.data.map(x => x.AllowanceId);
                    });
                }
                else {
                    debugger;
                    $scope.StudentSelectedValues = '';
                    CalculationBonusSrvc.GetAllRewardItems({ RewardItemExpensesType: "عند الطلب" }).then(function (data) {
                        $scope.DataSourceRewardItems = data.data; 
                        $scope.StudentSelectedValues = '';
                    });
                }
            },
            onInitialized: function (e) {

            }
        });
    });

    $scope.txtPayRollNum = {
        bindingOptions: {
            value: "PayRollNumber"
        },
        disabled: true,
        placeholder: "رقم المسير",
        onValueChanged: function (e) {
            $scope.PayRollNumber = e.value;
        }
    };


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
        showMultiTagOnly: false,
        selectAllMode: "allPages",

        onInitialized: function (e) {
            CalculationBonusSrvc.GetFaculties().then(function (data) {
                $scope.FacultionList = data.data;
            });
        },
        onClosed: function (e) {
            debugger;
            $scope.FacultionIds = '';
            if (e.model.FacultionID) {
                $scope.FacultionIds = e.model.FacultionID.join(',');
            }
        }
    };

    $scope.DegreeSelectBox = {
        bindingOptions: {
            value: "DegreeID",
            items: "DegreeList"
        },
        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        displayExpr: "Text",
        valueExpr: "Value",
        paginate: true, //Pagenation
        //selectAllMode: "page",
        showBorders: true,
        searchEnabled: true,
        showSelectionControls: true,
        maxDisplayedTags: 2,
        showMultiTagOnly: false,
        selectAllMode: "allPages",
        onInitialized: function (e) {
            CalculationBonusSrvc.GetDegrees().then(function (data) {
                $scope.DegreeList = data.data;
            });
        },
        onClosed: function (e) {
            $scope.DegreeIds = '';
            if (e.model.DegreeID) {
                $scope.DegreeIds = e.model.DegreeID.join(',');
            }

        }
    };

    var dataSourceStudents = new DevExpress.data.DataSource({
        //store: new DevExpress.data.CustomStore({
        //paginate: true,
        remoteOperations: true,
        cacheRawData: false,
        key: "STUDENT_ID",
        //loadMode: "raw",
        load: function (loadOptions) {
            debugger;
            var d = new $.Deferred(); 
            if ($scope.FacultionIds !== null && $scope.DegreeIds !== null) {
            loadOptions.requireTotalCount = false;
            $scope.loadOptions = loadOptions;
            
            debugger;
            
                var skip = null;
                var take = 500;
                if (loadOptions.skip != undefined)
                    skip = loadOptions.skip;
                if (loadOptions.take != undefined)
                    take = loadOptions.take;
                 
                  $.getJSON("/CalculationBonus/GetStudents?facultyIds="
                    + $scope.FacultionIds + "&degreeIds=" + $scope.DegreeIds + "&skip=" + skip + "&take=" + take + "&studentID=" + null)
                    .done(function (response)
                    {
                        d.resolve(response, { totalCount: response.length });  
                    });
                 
            }
            return d.promise();
        },
        byKey: function (key) {
            return key;
            //return $.getJSON("/CalculationBonus/GetStudents?facultyIds="
            //    + $scope.FacultionIds + "&degreeIds=" + $scope.DegreeIds + "&skip=" + null + "&take=" + null + "&studentID=" + key);
        }  
        //})
    });

    $scope.StudentSelectBox = {
        bindingOptions: {
            //items: "StudentList",

            value: "StudentId"
        },

        dataSource: dataSourceStudents,
        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        displayExpr: "STUDENT_NAME",
        valueExpr: "STUDENT_ID",
        paginate: true, //Pagenation
        showBorders: true,
        searchEnabled: true,
        searchExpr: ['STUDENT_NAME', 'STUDENT_ID', 'NATIONAL_ID'],
        showSelectionControls: true,
        maxDisplayedTags: 2,
        showMultiTagOnly: false,
        selectAllMode: "allPages",
        onOpened: function (e) {
            debugger;
            dataSourceStudents.load($scope.loadOptions);
        },
        //onValueChanged: function (e) {
        //    $scope.StudentIds = e.value;
        //},
        //onSelectAllValueChanged: function (e) {
        //    debugger;
        //    //$scope.StudentIds = e.value;
        //}
    };


    //Date
    $scope.DtBounsDate = {
        bindingOptions: {
            value: "BounsDate"
        },
        type: "date",
        displayFormat: "dd/MM/yyyy",

        onValueChanged: function (e) {
            $scope.BounsDate = e.value;
            $scope.GetPayRollNumber();
        }
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

    //btn Save
    $scope.btnSave = {
        text: 'احتساب',
        type: 'success',
        useSubmitBehavior: true
    };


    //dataGrid
    $scope.gridStudentPayrollInstance = '';
    $scope.gridStudentPayroll = {
        bindingOptions: {
            dataSource: "PayRollList",
            rtlEnabled: true,
            rowAlternationEnabled: true 
        },

        noDataText: "لا يوجد بيانات",
        selection: {
            mode: "single"
        },
        paging: {
            pageSize: 10

        },
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [5, 10, 20],
            showInfo: true
        },
        "export": {
            enabled: true,
            fileName: "مسير مكافئات الطلاب"
            //allowExportSelectedData: true
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
            scrolling: {
                rtlEnabled: true,
                useNative: true,
                scrollByContent: true,
                scrollByThumb: true,
                showScrollbar: "onHover",
                mode: "standard", // or "virtual"
                direction: "both"
            },
            resetOperationText: "الوضع الافتراضى"
        },
        allowColumnResizing: true,
        columnResizingMode: "widget",
        allowColumnReordering: true,
        showColumnLines: true,
        sorting: {
            mode: "multiple"
        },
        rowAlternationEnabled: true,
        hoverStateEnabled: true,
        columnChooser: {
            enabled: false
        }, headerFilter: {
            visible: true
        },
        showRowLines: true,
        columns: [{
                caption: "رقم المسير",
                dataField: "Payrollltem",
                cssClass: "text-right"
            },
            {
                caption: "التاريخ",
                dataField: "Date",
                cssClass: "text-right",
                dataType: "date"
            },
            {
                caption: "القيمة الكلية",
                dataField: "TotalValue",
                cssClass: "text-right"
            },
            {
                caption: "قيمة أقساط السلف",
                dataField: "AdvancesValue",
                cssClass: "text-right"
            },
            {
                caption: "صافي قيمة المسير",
                dataField: "RealValue",
                cssClass: "text-right"
            },
            {
                caption: "عدد الطلاب",
                dataField: "StudentsCount",
                cssClass: "text-right"
            },
            {
                caption: "مراجعات",
                dataField: "Notes",
                cssClass: "text-right"
            },
            {
                caption: "#",
                width: 100,
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    debugger;
                    if (options.data.Notes == null) {
                        $("<div />").dxButton({
                            //icon: "fa fa-save",
                            text: "",
                            type: "success",
                            icon: "chevrondoubleleft",
                            hint: "ترحيل للاعتماد",
                            elementAttr: { "class": "btn btn-danger" },

                            onClick: function (e) {

                                CalculationBonusSrvc.PostPayrollForApproval(options.data.PayrollNumber).then(function (data) {
                                    if (data.data == "") {
                                        DevExpress.ui.notify({
                                            message: "تم الترحيل للاعتماد بنجاح",
                                            type: "success",
                                            displayTime: 3000,
                                            closeOnClick: true
                                        });
                                    }
                                    else {
                                        DevExpress.ui.notify({
                                            message: "حدث خطأ أثناء الترحيل",
                                            type: "danger",
                                            displayTime: 3000,
                                            closeOnClick: true
                                        });
                                        $scope.GetPayRollNumber();
                                    }

                                    CalculationBonusSrvc.GetStudentPayroll().then(function (data) {
                                        $scope.PayRollList = data.data;
                                    });
                                });
                            }
                        }).appendTo(container);
                    }
                        $("<div />").dxButton({
                            text: "",
                            type: "danger",
                            icon: "trash",
                            hint: "حذف",
                            elementAttr: { "class": "btn btn-danger" },

                            onClick: function (e) {
                                CalculationBonusSrvc.DeletePayroll(options.data.PayrollNumber).then(function (data) {
                                    if (data.data.status === 500) {
                                        DevExpress.ui.notify({
                                            message: data.data.Message,
                                            type: data.data.Type,
                                            displayTime: 3000,
                                            closeOnClick: true
                                        });
                                    }
                                    if (data.data.status === 200) {
                                        DevExpress.ui.notify({
                                            message: data.data.Message,
                                            type: data.data.Type,
                                            displayTime: 3000,
                                            closeOnClick: true
                                        });
                                        $scope.GetPayRollNumber();
                                    }

                                    CalculationBonusSrvc.GetStudentPayroll().then(function (data) {
                                        $scope.PayRollList = data.data;
                                    });
                                });
                            }
                        }).appendTo(container);
                }
            }],

        onContentReady: function (e) {
            e.component.columnOption("command:edit",
                {
                    visibleIndex: -1
                });
        },
        onSelectionChanged: function (info) {
            $scope.GridRowData = info.selectedRowKeys;
        },
        onInitialized: function (e) {
            $scope.gridStudentPayrollInstance = e.component;
            CalculationBonusSrvc.GetStudentPayroll().then(function (data) {
                $scope.PayRollList = data.data;
            });
        }
    };

    $scope.getstDetailGrid = function (key) {
        return {
            dataSource: new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: true,
                key: "ID",
                loadMode: "raw",
                load: function () {
                    return $.getJSON("/CalculationBonus/GetPayRollsDetails?payrollID=" + key, function (data) {
                    });
                },
                filter: ["ID", "=", key]
            }),
            sorting: {
                mode: "multiple"
            },
            wordWrapEnabled: false,
            showBorders: true,
            paging: {
                pageSize: 5
            },
            pager: {
                allowedPageSizes: "auto",
                infoText: "(صفحة  {0} من {1} ({2} عنصر",
                showInfo: true,
                showNavigationButtons: true,
                showPageSizeSelector: true,
                visible: "auto"
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
                    dataField: "StudentFaculty",
                    caption: "الكلية",
                    alignment: "center"
                },
                {
                    dataField: "StudentDegree",
                    caption: "الدرجة العلمية",
                    alignment: "center"
                },
                {
                    dataField: "StudentName",
                    caption: "اسم الطالب",
                    alignment: "center"
                },
                {
                    dataField: "StudentNationality",
                    caption: "الجنسية",
                    alignment: "center"
                },
                {
                    dataField: "StudentNationalID",
                    caption: "رقم الهوية",
                    alignment: "center"
                }, {
                    dataField: "StudentStatus",
                    caption: "حالة الطالب",
                    alignment: "center"
                },
                {
                    dataField: "TotalValue",
                    caption: "إجمالي المكافأة"
                },
                {
                    dataField: "AdvanceValue",
                    caption: "إجمالي أقساط السلف"
                },
                {
                    dataField: "RealValue",
                    caption: "صافي المكافأة"
                },
            ]
        };
    };
    $scope.GetPayRollNumber = function () {
        CalculationBonusSrvc.GetPayRollNumber().then(function (data) {
            $scope.PayRollNumber = formatDate($scope.BounsDate) + "-" + data.data;
        });
    };
    $scope.GetPayRollNumber();


    $scope.SaveCalculationBonus = function () {
        if ($scope.RewardItemsSelectedValues === null || $scope.RewardItemsSelectedValues === undefined || $scope.RewardItemsSelectedValues === '') {
            return DevExpress.ui.notify({ message: "برجاء إختيار بند مكافأة واحد على الأقل" }, "Error", 10000);
        }
        debugger;
        var studentsBox = $('#StudentSelect').dxTagBox('instance');
        $scope.StudentIds = studentsBox._getValue();
        var object = new Object();
        object.StudentIds = $scope.StudentIds;
        object.BounsDate = $scope.BounsDate;
        object.RewardItems = $scope.RewardItemsSelectedValues;


        CalculationBonusSrvc.SaveCalculationBonus(object).then(
            function (data) {
                if (data.data.status === 500) {
                    DevExpress.ui.notify({
                        message: data.data.Message,
                        type: data.data.Type,
                        displayTime: 3000,
                        closeOnClick: true
                    });
                }
                if (data.data.status === 200) {
                    $scope.studentCount = data.data.Message;
                    DevExpress.ui.notify({
                        message: data.data.Message,
                        type: data.data.Type,
                        displayTime: 3000,
                        closeOnClick: true
                    });
                    $scope.visiblePopup = true;
                    $scope.GetPayRollNumber();
                }
            });
    };

    $scope.visiblePopup = '';

    $scope.popupOptions = {
        width: 350,
        height: 380,
        contentTemplate: "info",
        showTitle: true,
        shadingColor: "rgba(0, 0, 0, 0.69)",

        dragEnabled: false,
        closeOnOutsideClick: true,
        bindingOptions: {
            visible: "visiblePopup"
            //title: "message"
        }
    };

    // Popup btns...
    $scope.btnClose = {
        //icon: "check",
        type: "danger",
        text: "إغلاق",
        onClick: function (e) {
            $scope.visiblePopup = false;
            CalculationBonusSrvc.GetStudentPayroll().then(function (data) {
                debugger;
                $scope.PayRollList = data.data;
                $scope.gridStudentPayrollInstance.refresh();
            });
        }
    };

    $scope.btnViewPayRoll = {
        icon: "search",
        type: "default",
        text: "عرض المسير",
        onClick: function (e) {
            window.location.href = "/PayRollStudents/index";
            $scope.visiblePopup = false;
        }
    };

    $scope.btnAdd = {
        icon: "plus",
        type: "success",
        text: "إضافة جديد",
        onClick: function (e) {
            $scope.visiblePopup = false;
            $scope.FacultionID = null;
            $scope.FacultionIds = null;
            $scope.DegreeID = null;
            $scope.DegreeIds = null;

            $scope.StudentList = [];
            $scope.StudentId = null;
            $scope.StudentIds = null;

            $scope.BounsDate = new Date();

            CalculationBonusSrvc.GetStudentPayroll().then(function (data) {
                debugger;
                $scope.PayRollList = data.data;
                $scope.gridStudentPayrollInstance.refresh();
            });
        }
    };

    function formatDate(date) {
        var d = new Date(date),
            month = '' + (d.getMonth() + 1),
            day = '' + d.getDate(),
            year = d.getFullYear();

        if (month.length < 2) month = '0' + month;
        if (day.length < 2) day = '0' + day;

        return [day, month, year].join('-');
    }

    $scope.StudentPayrollPDF = function () {
        window.open('/CalculationBonus/StudentPayrollPDF', '_blank');
    };

}]);





















                    //DataSourceRewardItems.filter("RewardItemExpensesTypeName", "<>", "عند الطلب");
                    //DataSourceRewardItems.reload();
                    //$scope.DL_RewardItemsInstance.content().find(".dx-list").dxList("instance").selectAll();

                //DataSourceRewardItems.filter("RewardItemExpensesTypeName", "<>", "عند الطلب");
                //DataSourceRewardItems.reload();

                    //DataSourceRewardItems.filter("RewardItemExpensesTypeName", "=", "عند الطلب");
                    //DataSourceRewardItems.reload();
                    //$scope.DL_RewardItemsInstance.content().find(".dx-list").dxList("instance").unselectAll();

//e.component.content().find(".dx-list").dxList("instance").selectAll(); 


    //var DataSourceRewardItems = new DevExpress.data.DataSource({
    //    paginate: false,
    //    cacheRawData: true,
    //    key: "AllowanceId",
    //    loadMode: "raw",
    //    load: function () {
    //        return $.getJSON("/CalculationBonus/GetAllRewardItems", function (data) { });
    //    }
    //});