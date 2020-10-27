app.controller("PaySubsidyCtrl", ["$scope", 'AdvancePaymentSrvc', '$http', function ($scope, AdvancePaymentSrvc, $http) {

    $scope.AdvanceRequestsList = [];
    $scope.gridSelectedRowsData = [];
    $scope.message = 'تم صرف الإعانات بنجاح';

    $scope.MainTitle = null;
    $scope.BtnCachTitle = null;
    $scope.PopupCashTitle = null;
    $scope.PopupCashShow = false;

    $scope.JournalentrycheckValue = true;


    //validation 
    $scope.validationRequired = {
        validationRules: [
            {
                type: "required",
                message: "الحقل مطلوب"
            }
        ]
    };

    /*********************************** Permissions *********************************/
    $scope.Permissions = {
        View: false,
        ExchangeSelected: false
    };

    
    debugger;
    $http({
        method: "Get",
        url: "/AdvancePayment/GetAdvancePaymentPermissions?screenId=93"
    }).then(function (data) {
        debugger;
        $scope.Permissions.View = data.data.View ? data.data.View : false;
        $scope.Permissions.ExchangeSelected = data.data.ExchangeSelected ? data.data.ExchangeSelected : false;
            
    });
            /*--------------------------------* Permissions *--------------------------------*/
    let DDL_StudentsDataSource = new DevExpress.data.DataSource({
        paginate: true,
        cacheRawData: true,
        key: "STUDENT_ID",
        loadMode: "raw",
        load: function () {
            debugger;
            return $.getJSON("/AdvancePayment/GetAllStudentsHaveAdvances?Type=S", function (data) {
            });
        }
    });

    DDL_StudentsDataSource.load();

    $scope.DDL_Students = {
        dataSource: DDL_StudentsDataSource,
        bindingOptions: {
            value: "MDL_StudentId"
        },
        placeholder: 'بحث ...',
        noDataText: "لا يوجد بيانات",
        displayExpr: "STUDENT_NAME",
        valueExpr: "STUDENT_ID",
        showBorders: true,
        searchEnabled: true,
        searchExpr: ['STUDENT_ID', 'NATIONAL_ID', 'STUDENT_NAME'],
        showClearButton: true,
        rtlEnabled: true,
        onValueChanged: function (e) {
            debugger;
            $scope.MDL_StudentId = e.value;
            if (e.value !== null && e.value !== '' && e.value !== undefined) {

                debugger;
                $scope.GetAdvanceRequests($scope.SelectedType, $scope.MDL_StudentId);
            }

        }
    };



    //dataGrid
    $scope.gridAdvanceRequests = {
        keyExpr: "ID",
        bindingOptions: {
            dataSource: "AdvanceRequestsList"
        },
        noDataText: "لا يوجد بيانات",
        showBorders: true,
        paging: {
            pageSize: 10
        },
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [5, 10, 20],
            showInfo: true
        },
        selection: {
            mode: "multiple",
            showCheckBoxesMode: "always",
            selectAllMode: "allPages",
            width: "40px"
        },
        "export": {
            enabled: true,
            fileName: "طلبات الإعانات"
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
            headerFilter: {
                visible: true,
                allowSearch: true
            },
            showRowLines: false,
            groupPanel: {
                visible: true,
                emptyPanelText: "اسحب عمود هنا"
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
        rowAlternationEnabled: true,
        allowColumnReordering: true,
        allowColumnResizing: true,
        columnAutoWidth: true,
        columnChooser: {
            enabled: false
        },
        columns: [
            {
                caption: "اسم الطالب",
                dataField: "STUDENT_NAME",
                cssClass: "text-right"
            },
            {
                caption: "رقم الهوية",
                dataField: "NATIONAL_ID",
                cssClass: "text-right"
            },
            {
                caption: "رقم الطالب",
                dataField: "STUDENT_ID",
                cssClass: "text-right"
            },
            {
                caption: "نوع الإعانة",
                dataField: "AdvanceSettingName",
                cssClass: "text-right"
            },
            {
                caption: "القيمة المعتمدة",
                dataField: "ApprovedValue",
                cssClass: "text-right"
            },
            {
                caption: "تاريخ الطلب",
                dataField: "InsertionDate",
                cssClass: "text-right",
                type: "date"
            },
            {
                caption: "القائم بالعملية",
                dataField: "UserName",
                cssClass: "text-right"
            },
            {
                caption: "ملاحظات الطلب",
                dataField: "RequestNotes",
                cssClass: "text-right"
            }
        ],
        summary: {
            totalItems: [
                {
                    column: "RequestedValue",
                    displayFormat: "العدد :{0}",
                    summaryType: "count"
                },

                {
                    column: "ApprovedValue",
                    summaryType: "sum",
                    displayFormat: "المجموع :{0}"
                }
            ]
        },
        masterDetail: {
            enabled: true,
            template: "detail"
        },
        onContentReady: function (e) {
            e.component.columnOption("command:edit",
                {
                    visibleIndex: -1
                });
        },
        onSelectionChanged: function (selectedItems) {
            debugger;
            $scope.gridSelectedRowsData = selectedItems.selectedRowKeys.join(',');
        },
        onInitialized: function (e) {
            $scope.SelectedType = "S";
            $scope.MainTitle = "صرف الإعانة";
            $scope.BtnCachTitle = "صرف الإعانة المحددة";
            $scope.PopupCashTitle = "صرف الإعانات للطلاب";
        }
    };


    $scope.gridDetailAdvanceApprovedPhases = function (key) {
        debugger;
        return {
            dataSource: new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: false,
                key: "AdvanceRequested_ID",
                loadMode: "raw",
                load: function () {
                    debugger;
                    return $.getJSON("/AdvancePayment/GetAdvanceApprovedPhasesByRequestId/" + key, function (data) { debugger; });
                }
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
            headerFilter: {
                visible: false
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
                mode: "row"
            },
            allowColumnResizing: true,
            columnResizingMode: "widget",
            allowColumnReordering: true,
            showColumnLines: true,
            rowAlternationEnabled: true,
            columns: [
                {
                    caption: "المرحلة",
                    dataField: "PhaseName",
                    alignment: "center"
                },
                {
                    caption: "القائم بالعملية",
                    dataField: "UserName",
                    alignment: "center"
                },
                {
                    caption: "تاريخ الاعتماد",
                    dataField: "ResponseDate",
                    alignment: "center"
                },
                {
                    caption: "القيمة المعتمدة",
                    dataField: "ApprovedValue",
                    alignment: "center"
                },
                {
                    caption: "ملاحظات الاعتماد",
                    dataField: "Reason",
                    alignment: "center"
                },
                {
                    caption: "حالة الاعتماد",
                    dataField: "ApprovedStatus",
                    alignment: "center"
                }
            ]
        };
    };


    $scope.btnCash = {
        bindingOptions: {
            text: "BtnCachTitle"
        },
        icon: "fa fa-money",
        type: 'success',
        onClick: function (e) {
            debugger;
            if (!$scope.Permissions.ExchangeSelected) {
                return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه العملية", "warning");

            }

            if ($scope.gridSelectedRowsData.length === 0) {
                DevExpress.ui.notify({ message: 'عفوا لم يتم تحديد أي إعانات للصرف', type: 'error', displayTime: 3000, closeOnClick: true });
                return;
            } else {
                AdvancePaymentSrvc.GetTreasuryAccount().then(function (data) {
                    if (data.data !== "") {

                        $scope.AccountsDisabled = data.data.IsAccountant;
                        $scope.AccountId = data.data.COAID;
                    }
                    $scope.PopupCashShow = true;
                });
            }
        }
    };


    //PopupCash
    $scope.AccountingList = [];
    $scope.AccountId = null;

    $scope.Nots = null;
    $scope.JournalentryDateValue = null;
    $scope.JournalentryDescription = null;
    //$scope.JournalentrycheckValue = false;




    $scope.PopupCash = {
        showTitle: true,
        bindingOptions: {
            title: "PopupCashTitle",
            visible: "PopupCashShow"
        },
        rtlEnabled: true,
        width: 450,
        height: 500,
        onHiding: function () {

        }
    };

    $scope.AccountsSelectBox = {
        bindingOptions: {
            dataSource: "AccountingList",
            value: "AccountId",
            items: "AccountingList",
            //disabled: "!AccountsDisabled"
        },
        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        paginate: true, //Pagenation
        showClearButton: true,
        searchEnabled: true,
        displayExpr: "Text",
        valueExpr: "Value",
        disabled: false,
        onInitialized: function (e) {
            AdvancePaymentSrvc.GetAcctounts().then(function (data) {
                debugger;
                $scope.AccountingList = data.data;
            });
        },
        onValueChanged: function (e) {
            $scope.AccountId = e.value;
        }
    };

    //$scope.txtNots = {
    //    bindingOptions: {
    //        value: "Note"
    //    },
    //    placeholder: "ملاحظات",
    //    onValueChanged: function (e) {
    //        $scope.Note = e.value;
    //    }
    //};

    $scope.JournalentryUncheckValue = true;
    $scope.JournalentrycheckValue = !$scope.JournalentryUncheckValue;

    $scope.Journalentrycheck = {
        bindingOptions: {
            value: "JournalentrycheckValue"
        },
        text: "ترحيل القيد للحسابات العامة مباشرة",
        onValueChanged: function (e) {
            $scope.JournalentrycheckValue = e.value;
            $scope.JournalentryUncheckValue = !e.value;
        }
    };

    $scope.JournalentryUncheck = {
        bindingOptions: { value: "JournalentryUncheckValue" },
        rtlEnabled: true,
        onValueChanged: function (e) {
            $scope.JournalentryUncheckValue = e.value;
            $scope.JournalentrycheckValue = !e.value;
        },
        text: "ترحيل القيد للقيود المجمعة"
    };

    $scope.JournalentryDate = {
        bindingOptions: {
            value: "JournalentryDateValue"
        },
        type: "date",
        //disabled: true,
        onValueChanged: function (e) {
            $scope.JournalentryDateValue = e.value;
        },
        onInitialized: function (e) {
            $scope.JournalentryDateValue = new Date();
        }
    };

    $scope.txtJournalentryDescription = {
        bindingOptions: {
            value: "JournalentryDescription"
        },
        placeholder: "بيان السند",
        onValueChanged: function (e) {
            $scope.JournalentryDescription = e.value;
        }
    };

    //btn View
    $scope.btnSave = {
        text: 'صرف',
        type: 'success',
        useSubmitBehavior: true
    };

   

    $scope.GetAdvanceRequests = function (Type, StudentID) {
        debugger;
        AdvancePaymentSrvc.GetAdvanceRequests(Type, StudentID).then(function (data) {
            debugger;
            $scope.AdvanceRequestsList = data.data;
        });
    };


    //SaveAdvances
    $scope.SaveAdvances = function () {
        AdvancePaymentSrvc.SaveAdvances({ AccountId: $scope.AccountId, AdvanceRequests: $scope.gridSelectedRowsData, Notes: $scope.Note, PostJournal: $scope.JournalentrycheckValue, PaymentDate: $scope.JournalentryDateValue })
            .then(function (data) {
                if (data.data.status === 200) {
                    DevExpress.ui.notify({ message: "تم صرف الإعانات بنجاح", type: data.data.Type, displayTime: 3000, closeOnClick: true });

                    //Clear Data
                    $scope.AccountId = null;
                    $scope.Note = null;
                    $scope.JournalentrycheckValue = false;

                    $scope.PopupCashShow = false;

                    //Referesh Grid
                    $scope.GetAdvanceRequests($scope.SelectedType, $scope.MDL_StudentId);

                    $scope.studentCount = data.data.Message.split(',').length;
                    $scope.MDL_AdvancePaymentMasterIds = data.data.Message;

                    //Dispaly Alert PopUp...
                    $scope.visibleAlertPopUp = true;
                }
                if (data.data.status === 500) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                }
            });
    };

    $scope.AlertPopupOptions = {
        width: 640,
        height: 215,
        contentTemplate: "AlertPopupContent",
        showTitle: false,
        shadingColor: "rgba(0, 0, 0, 0.69)",

        dragEnabled: false,
        closeOnOutsideClick: true,
        bindingOptions: {
            visible: "visibleAlertPopUp"
        }
    };

    // Popup btns...
    $scope.btnClose = {
        type: "danger",
        text: "إغلاق",
        onClick: function (e) {
            $scope.visibleAlertPopUp = false;
            $scope.PopupCashShow = false;
            $scope.Refresh();
        }
    };

    $scope.btnPrintDoc = {
        icon: "print",
        type: "default",
        text: "",
        onClick: function (e) {
            window.location.href = "/Advances/PrintDocAdvance?DocMasterIds=" + $scope.MDL_AdvancePaymentMasterIds;
            $scope.PopupCashShow = false;

            $scope.Refresh();
        }
    };

    $scope.btnHome = {
        icon: "search",
        type: "success",
        text: "",
        onClick: function (e) {
            window.location.href = "/Students/AdvancedSearch";
            $scope.visibleAlertPopUp = false;
            $scope.PopupCashShow = false;
            $scope.Refresh();
        }
    };

    $scope.Refresh = function () {
        //Clear Data
        $scope.AccountId = null;
        $scope.Note = null;
        $scope.JournalentrycheckValue = false;
        $scope.PopupCashShow = false;

        //Referesh Grid
        $scope.GetAdvanceRequests($scope.SelectedType, $scope.MDL_StudentId);
    };

}]);
