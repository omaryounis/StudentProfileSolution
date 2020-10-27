app.controller("AdvancesReportsCtrl", ["$scope", 'AdvancesReportsSrvc', function ($scope, AdvancesReportsSrvc) {
    //Filed
    $scope.StudentsList = [];
    $scope.StudentId = null;
    $scope.StudentIds = null;

    $scope.AdvanceStartDateValue = null;
    $scope.AdvanceEndDateValue = null;
    $scope.ReturnStartDateValue = null;
    $scope.ReturnEndDateValue = null;

    $scope.AdvancesList = null;


    //Countrols
    $scope.StudentsMultiTag = {
        bindingOptions: {
            value: "StudentId",
            items: "StudentsList"
        },

        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        paginate: true, //Pagenation
        displayExpr: "Text",
        valueExpr: "Value",
        searchExpr: ['Text', 'Value'],
        showBorders: true,
        searchEnabled: true,
        showSelectionControls: true,
        maxDisplayedTags: 2,
        showMultiTagOnly: false,

        onInitialized: function (e) {
            AdvancesReportsSrvc.GetStudents().then(function (data) {
                $scope.StudentsList = data.data;
            });
        },
        onValueChanged: function (e) {
            $scope.StudentId = e.value;
            $scope.StudentIds = e.value.join(',');
        }
    };

    $scope.AdvanceStartDate = {
        bindingOptions: {
            value: "AdvanceStartDateValue"
        },
        type: "date",

        onValueChanged: function (e) {
            $scope.AdvanceStartDateValue = e.value;
        }
    };

    $scope.AdvanceEndDate = {
        bindingOptions: {
            value: "AdvanceEndDateValue"
        },
        type: "date",

        onValueChanged: function (e) {
            $scope.AdvanceEndDateValue = e.value;
        }
    };

    $scope.ReturnStartDate = {
        bindingOptions: {
            value: "ReturnStartDateValue"
        },
        type: "date",

        onValueChanged: function (e) {
            $scope.ReturnStartDateValue = e.value;
        }
    };

    $scope.ReturnEndDate = {
        bindingOptions: {
            value: "ReturnEndDateValue"
        },
        type: "date",

        onValueChanged: function (e) {
            $scope.ReturnEndDateValue = e.value;
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

    //btn View
    $scope.btnView = {
        text: 'عرض',
        type: 'primary',
        useSubmitBehavior: true
    };

    //dataGrid
    $scope.gridAdvances = {
        bindingOptions: {
            dataSource: "AdvancesList"
        },

        noDataText: "لا يوجد بيانات",
        selection: {
            mode: "single"
        },
        showBorders: true,
        paging: {
            pageSize: 10
        },
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [5, 10, 20],
            showInfo: true
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
                caption: "الرقم الجامعة",
                dataField: "Student_ID",
                cssClass: "text-right"
            },
            {
                caption: "رقم الهوية",
                dataField: "NATIONAL_ID",
                cssClass: "text-right"
            },
            {
                caption: "نوع السلفة",
                dataField: "AdvanceSettingName",
                cssClass: "text-right"
            },
            {
                caption: "تاريخ السلفة",
                dataField: "PaymentDate",
                cssClass: "text-right"
            },
            {
                caption: "تاريخ السداد",
                dataField: "ReturningDate",
                cssClass: "text-right"
            },
            {
                caption: "رقم الدفتر",
                dataField: "DocNumber",
                cssClass: "text-right"
            },
            {
                caption: "اجمالي المبلغ",
                dataField: "TotalAdvanceValue",
                cssClass: "text-right"
            },
            {
                caption: "طريقة السداد",
                dataField: "ReturnMethod",
                cssClass: "text-right"
            },
            {
                caption: "اجمالي المسدد",
                dataField: "ReturnedValue",
                cssClass: "text-right"
            },
            {
                caption: "اجمالي المتبقي",
                dataField: "RemainingValue",
                cssClass: "text-right"
            },
            {
                caption: "رقم المسير",
                dataField: "Number",
                cssClass: "text-right"
            }
        ],
        "export": {
            enabled: true,
            fileName: "تقرير السلف التفصيلي"
        },
        onContentReady: function (e) {
            e.component.columnOption("command:edit", {
                visibleIndex: -1
            });
        },
        onSelectionChanged: function (info) {
            $scope.GridRowData = info.selectedRowKeys;
        }//,
        //onInitialized: function (e) {
        //}
    };


    //Function
    $scope.GetAdvancesRecievingByStudents = function () {

        AdvancesReportsSrvc.AdvancesRecievingByStudents({ AdvanceStartDate: $scope.AdvanceStartDateValue, AdvanceEndDate: $scope.AdvanceEndDateValue, ReturnStartDate: $scope.ReturnStartDateValue, ReturnEndDate: $scope.ReturnEndDateValue, StudentIDs:$scope.StudentIds}).then(function (data) {
            if (data) $scope.AdvancesList = data.data;
        });
    };

}]);
