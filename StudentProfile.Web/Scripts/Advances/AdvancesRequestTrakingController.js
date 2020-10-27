app.controller("AdvancesRequestTrakingCtrl", ["$scope", '$rootScope', '$filter', 'StudentAdvanceSrvc', function ($scope, $rootScope, $filter, StudentAdvanceSrvc) {

    //Fields
    $scope.AdvanceTypeList = [{ Text: "الكل", Value: null }, { Text: "سلفة", Value: "A" }, { Text: "إعانة", Value: "S" }];
    $scope.AdvanceTypeValue = null;

    $scope.DateFromValue = null;
    $scope.DateToValue = null;

    $scope.StudentId = null;

   
    $scope.AdvancesRequestTrakingList = [];

    $scope.RequestList = [{ Text: "الملغي", Value: 1 }, { Text: "المرفوض", Value: 2 }, { Text: "الكل", Value: 3 }];
    $scope.RequestValue = null;
    //Controls
    $scope.RequestSelectBox = {
        bindingOptions: {
            value: "RequestValue"
        },
        dataSource: $scope.RequestList,
        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        paginate: true, //Pagenation
        showClearButton: true,
        searchEnabled: true,
        displayExpr: "Text",
        valueExpr: "Value",
        disabled: false,
        onValueChanged: function (e) {
            $scope.RequestValue = e.value;
        }
    };


    $scope.AdvanceTypeSelectBox = {
        bindingOptions: {
            value: "AdvanceTypeValue"
        },
        dataSource: $scope.AdvanceTypeList,
        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        paginate: true, //Pagenation
        showClearButton: true,
        searchEnabled: true,
        displayExpr: "Text",
        valueExpr: "Value",
        disabled: false,
        onValueChanged: function (e) {
            $scope.LevelID = e.value;
        }
    };

    $scope.DateFrom = {
        bindingOptions: {
            value: "DateFromValue"
        },
        type: "date",
        onValueChanged: function (e) {
            $scope.DateFromValue = e.value;
        }
    };

    $scope.DateTo = {
        bindingOptions: {
            value: "DateToValue"
        },
        type: "date",
        onValueChanged: function (e) {
            $scope.DateToValue = e.value;
        }
    };

    let StudentsDataSource = new DevExpress.data.DataSource({
        paginate: true,
        cacheRawData: true,
        key: "STUDENT_ID",
        loadMode: "raw",
        load: function () {
            debugger;
            return $.getJSON("/Advances/GetAllStudents", function (data) {
            });
        }
    });
    StudentsDataSource.load();

    $scope.StudentSelectBox = {
        dataSource: StudentsDataSource,
        bindingOptions: {
            value: "StudentId"
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
            $scope.StudentId = e.value;
        }
    };



    //btn Search
    $scope.btnSearch = {
        text: 'بحث',
        type: 'success',
        useSubmitBehavior: true,
    };


    /***************** List **********************/
    //dataGrid
    $scope.gridAdvancesRequestTraking = {
        bindingOptions: {
            dataSource: "AdvancesRequestTrakingList"
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
        }, headerFilter: {
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
        groupPanel: {
            visible: false,
            emptyPanelText: "اسحب عمود هنا"
        },
        showRowLines: true,
        rowAlternationEnabled: true,
        allowColumnReordering: true,
        allowColumnResizing: true,
        columnAutoWidth: true,
        columnChooser: {
            enabled: true
        },

        "export": {
            enabled: true,
            fileName: "advancesTracking"
        },
        columns: [
            {
                caption: "رقم الطالب",
                dataField: "STUDENT_ID",
                alignment: "right",
                width: 120
            },
            {
                caption: "اسم الطالـب",
                dataField: "STUDENT_NAME",
                alignment: "right"
            },
            {
                caption: "رقم الجوال",
                dataField: "MobileNumber",
                alignment: "center",
                width: 120
            },
            {
                caption: "رقم الهوية",
                dataField: "NATIONAL_ID",
                alignment: "right",
                width: 120
            },
            {
                caption: "الجنسية",
                dataField: "NATIONALITY_DESC",
                alignment: "right",
                width: 120
            },
            {
                caption: "النوع",
                dataField: "AdvanceName",
                alignment: "right",
                width: 140
            },
            {
                caption: "المبلغ المطلوب",
                dataField: "RequestedValue",
                alignment: "right",
                width: 140
            },
            {
                caption: "المبلغ المعتمد",
                dataField: "ApprovedValue",
                alignment: "right",
                width: 140
            },
            {
                caption: "تاريخ الطلب",
                dataField: "RequestedDate",
                alignment: "right",
                //dataType: "date",
                //format: "MM/dd/yyyy hh:mm " ,
                width: 120
            },
            {
                caption: "اسم المرسل",
                dataField: "CreatedBy",
                alignment: "right",
            },
            {
                caption: "حالة الاعتماد",
                dataField: "ApprovedStatus",
                alignment: "right",
                width: 120
            },
            {
                caption: "حالة الصرف",
                dataField: "AdvanceIsPaid",
                alignment: "right",
                width: 120
            },
            {
                caption: "مرحلة الاعتماد",
                dataField: "ApprovedPhase",
                alignment: "right",
            },
            {
                //caption: "المرفق",
                cssClass: "text-center",
                width: 45,
                cellTemplate: function (container, options) {
                    $("<div />").dxButton({
                        text: '',
                        hint: "تحميل",
                        type: 'default',
                        icon: 'fa fa-download',
                        useSubmitBehavior: false,
                        onClick: function (e) {
                            return window.open('/Advances/DownloadAttachments?id=' + options.data.AdvanceRequestId + "&studentid=" + options.data.STUDENT_ID, '_self');
                        }
                    }).appendTo(container);
                }
            }
        ],

        onContentReady: function (e) {
            e.component.columnOption("command:edit", {
                visibleIndex: -1
            });
        },
        onSelectionChanged: function (info) {
            $scope.GridRowData = info.selectedRowKeys;
        },
        onInitialized: function (e) {
        }
    }

    $scope.SubmitAdvancesRequestTraking = function () {
        StudentAdvanceSrvc.GetAdvancesRequestTraking({
            StudentId: $scope.StudentId, AdvancesType: $scope.AdvanceTypeValue, DateFrom: $scope.DateFromValue, DateTo: $scope.DateToValue
            , RequestValue: $scope.RequestValue
        }).then(function (data) {
            $scope.AdvancesRequestTrakingList = data.data;
        });
    }

}]);
