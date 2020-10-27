app.controller("MusaferReportsCtrl", ["$scope", 'MusaferReportsSrvc', function ($scope, MusaferReportsSrvc) {
    //Filed
    $scope.ReportAdvertisementList = [];
    $scope.AdvertisementList = [];
    $scope.AdvertisementID = null;


    //Countrols
    $scope.AdvertisementSelectBox = {
        bindingOptions: {
            dataSource: "AdvertisementList",
            value: "AdvertisementID",
            items: "AdvertisementList"
        },

        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        pagingEnabled: true, //Pagenation
        showClearButton: true,
        searchEnabled: true,
        displayExpr: "Text",
        valueExpr: "Value",
        disabled: false,

        onInitialized: function (e) {
            MusaferReportsSrvc.GetAllAdvertisement().then(function (data) {
                $scope.AdvertisementList = data.data;
            });
        },
        onValueChanged: function (e) {
            $scope.AdvertisementID = e.value;
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
    $scope.gridReportAdvertisement = {
        bindingOptions: {
            dataSource: "ReportAdvertisementList"
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
        headerFilter: {
            visible: true,
            allowSearch: true
        },
        columns: [
            {
                caption: "الرقم الجامعي",
                dataField: "StudentID",
                cssClass: "text-right"
            },
            {
                caption: "اسم الطالب",
                dataField: "IdentityName",
                cssClass: "text-right"
            },
            {
                caption: "اسم الجواز",
                dataField: "PassportName",
                cssClass: "text-right"
            },
            {
                caption: "رقم الهويه",
                dataField: "National_ID",
                cssClass: "text-right"
            },
            {
                caption: "الحالة الأكاديمية",
                dataField: "STATUS_DESC",
                cssClass: "text-right"
            },
            {
                caption: "عدد الساعات المتبقيه",
                dataField: "REMAININGCREDITHOURSCOUNT",
                cssClass: "text-right"
            },
            {
                caption: "الجنسية",
                dataField: "Nationality",
                cssClass: "text-right"
            },
            {
                caption: "رقم الجوال",
                dataField: "MobileNumber",
                cssClass: "text-right"
            },
            {
                caption: "البريد الالكتروني",
                dataField: "UniversityEmail",
                cssClass: "text-right"
            },

            {
                caption: "خط السير",
                dataField: "Tracking",
                cssClass: "text-right"
            }
            ,
            {
                caption: "تاريخ تقديم الرغبه",
                dataField: "InsertDate",
                cssClass: "text-right"
            },
            {
                caption: "رقم التذكرة",
                dataField: "TicketNumber",
                cssClass: "text-right"
            },

        ],
        summary: {
            totalItems: [{
                displayFormat: "عدد الطلاب : {0}",
                column: "IdentityName",
                summaryType: "count"
            }]
        },
        "export": {
            enabled: true,
            fileName: "تقرير الطلاب المسجلين في الاعلانات"
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



    $scope.btnPrint = {
        icon: "print",
        //text: "طباعة",
        onClick: function () {
            var contents = $("#ReportAdvertisementList").html();
            var frame1 = $('<iframe />');
            frame1[0].name = "frame1";
            frame1.css({ "position": "absolute", "top": "-1000000px" });
            $("body").append(frame1);
            var frameDoc = frame1[0].contentWindow ? frame1[0].contentWindow : frame1[0].contentDocument.document ? frame1[0].contentDocument.document : frame1[0].contentDocument;
            frameDoc.document.open();
            //Create a new HTML document.
            frameDoc.document.write('<html><head><title> تقرير الطلاب المسجلين في الاعلان </title>');
            frameDoc.document.write('</head><body>');
            //Append the external CSS file.
            frameDoc.document.write('<link href="../../assets/vendors/bootstrap/dist/css/bootstrap.min.css" rel="stylesheet"><link href="../../assets/css/rtl.css" rel="stylesheet"><link href="../../assets/css/style.css" rel="stylesheet">');
            //Append the DIV contents.
            frameDoc.document.write(contents);
            frameDoc.document.write('</body></html>');
            frameDoc.document.close();
            setTimeout(function () {
                window.frames["frame1"].focus();
                window.frames["frame1"].print();
                frame1.remove();
            }, 500);
        }
    };

    //Function
    $scope.GetAllStudentsByAdvertisement = function () {
        MusaferReportsSrvc.GetAllStudentsByAdvertisement($scope.AdvertisementID).then(function (data) {
            if (data.data.status === 500) {
                DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
            }
            else {
                $scope.ReportAdvertisementList = data.data;
            }
        });
    };

}]);
