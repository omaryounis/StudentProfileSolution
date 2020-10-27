app.controller("TransportationTrackingController", ["$scope", "$http", "TransportationTrackingSrvc", function ($scope, $http, TransportationTrackingSrvc) {

    $scope.TrackingNameValidationRules = {
        validationRules: [{
            type: "required",
            message: "يرجى ادخال خط السير"
        }]
    };


    $scope.TransportationTrackingList = [];
    $scope.IsActive = false;
    $scope.NationalityID = null;
    $scope.Nationality = [];
    $scope.ID = 0;
    $scope.txtTrackingName = {
        placeholder: "ادخل خط السير",
        bindingOptions: {
            value: "TrackingName",
            readOnly: "txtTrackingNamereadOnly"
        },
        onInitialized: function (e) {
            $scope.txtTrackingNamereadOnly = false;
        }
    };
    $scope.selectBoxNationalityEditing = {
        bindingOptions: {
            items: "Nationality",
            value: "selctedUsrEDID"
        },
        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        pagingEnabled: true, //Pagenation
        showClearButton: true,
        searchEnabled: true,
        disabled: false,
        displayExpr: "Name",
        valueExpr: "ID",
        onValueChanged: function (e) {
        }
    };

    $scope.FlightsTypePriorities = [{ value: 'R', text: "ذهاب وعودة" }, { value: 'O', text: "ذهاب فقط" }];
    $scope.FlightsType = $scope.FlightsTypePriorities[0];
    $scope.radioFlightsType = {
        bindingOptions: {
            value: 'FlightsType'
        },
        dataSource: $scope.FlightsTypePriorities,
        layout: "horizontal"
    };
    $scope.checkBoxValue = false;
    TransportationTrackingSrvc.GetAllTransportationTracking().then(function (data) {
        debugger;
        $scope.TransportationTracking = data.data;
    });
    TransportationTrackingSrvc.GetAllNationalities().then(function (data) {
        $scope.Nationality = data.data;
    });
    //dataGrid
    $scope.gridTransportationTracking = {
        bindingOptions: {
            dataSource: "TransportationTrackingList"
        },
        keyExpr: "ID",
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
        columnChooser: {
            enabled: true
        },
        columns: [
            {
                caption: "الجنسيه",
                dataField: "NationalityName",
                cssClass: "text-right"
            },
            {
                caption: "خط السير",
                dataField: "Tracking",
                cssClass: "text-right"
            },
            {
                caption: "ظهور عند الطالب",
                dataField: "IsActive",
                cssClass: "text-right"
            },
            {
                caption: "طريقة الحجز",
                dataField: "FlightsType",
                cssClass: "text-right"
            },
            {
                caption: "تعديل",
                width: 100,
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    $("<div />").dxButton({
                        icon: "fa fa-pencil",
                        //text: "تعديل",
                        type: "warning",
                        hint: "تعديل",
                        elementAttr: { "class": "btn btn-warning btn-sm" },
                        onClick: function (e) {
                            TransportationTrackingSrvc.GetByID(options.data.ID).then(function (data) {
                                if (data) {
                                    debugger;
                                    $scope.ID = data.data.ID;
                                    $scope.TrackingName = data.data.Tracking;
                                    $scope.NationalityID = data.data.Nationality_ID;
                                    $scope.IsActive = data.data.IsActive;
                                    $scope.checkBoxValue = data.data.IsActive;
                                    if (data.data.FlightsType === "R" || data.data.FlightsType ===null) {
                                        $scope.FlightsType = $scope.FlightsTypePriorities[0];
                                    }

                                    if (data.data.FlightsType === "O") {
                                        $scope.FlightsType = $scope.FlightsTypePriorities[1];
                                    }
                                }
                            });
                        }
                    }).appendTo(container);
                }

            },
            {
                caption: "حذف",
                width: 100,
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    $("<div />").dxButton({
                        text: "",
                        type: "danger",
                        icon: "trash",
                        hint: "حذف",
                        elementAttr: { "class": "btn btn-danger" },
                        onClick: function (e) {
                            debugger;
                            TransportationTrackingSrvc.DeleteTransportationTracking(options.data.ID).then(function (data) {
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
                                }
                                $scope.IsActive = false;
                                $scope.checkBoxValue = false;
                                $scope.TrackingName = "";
                                $scope.NationalityID = null;
                                $scope.ID = 0;
                                $scope.FlightsType = $scope.FlightsTypePriorities[0];
                                TransportationTrackingSrvc.GetAllTransportationTracking().then(function (data) {
                                    debugger;
                                    if (data) {
                                        $scope.TransportationTrackingList = data.data;
                                    }
                                });
                            });
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
            TransportationTrackingSrvc.GetAllTransportationTracking().then(function (data) {
                if (data) {
                    $scope.TransportationTrackingList = data.data;
                }
            });
        }
    }

    $scope.selectBoxNationality = {
        bindingOptions: {
            items: "Nationality",
            value: "NationalityID"
        },
        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        pagingEnabled: true, //Pagenation
        showClearButton: true,
        searchEnabled: true,
        disabled: false,
        displayExpr: "Name",
        valueExpr: "ID",
        onValueChanged: function (e) {
        }
    };


    $scope.ISActive = function () {
        debugger;
        if ($scope.checkBoxValue === true) {
            $scope.IsActive = false;
            $scope.checkBoxValue = false;
        }

        else {
            $scope.IsActive = true;
            $scope.checkBoxValue = true;
        }
    };



    $scope.btnSaveOptions = {
        text: 'حفظ',
        visible: true,
        type: 'success',
        useSubmitBehavior: true,
        onClick: function (e) {
            debugger;
            if ($scope.ID == 0) {
                TransportationTrackingSrvc.addTransportationTracking($scope.TrackingName, $scope.NationalityID, $scope.IsActive, $scope.FlightsType.value).then(function (data) {
                    debugger;
                    if (data.data !== "") {
                        DevExpress.ui.notify(data.data, "warning", 5000);
                    }
                    else {
                        DevExpress.ui.notify("تم الحفظ بنجاح", "success", 5000);
                        $scope.IsActive = false;
                        $scope.checkBoxValue = false;
                        $scope.TrackingName = "";
                        $scope.NationalityID = null;
                        $scope.ID = 0;
                        $scope.FlightsType = $scope.FlightsTypePriorities[0];
                        TransportationTrackingSrvc.GetAllTransportationTracking().then(function (data) {
                            if (data) {
                                $scope.TransportationTrackingList = data.data;
                            }
                        });
                    }
                });
            } else {
                TransportationTrackingSrvc.updateTransportationTracking($scope.ID, $scope.TrackingName, $scope.NationalityID, $scope.IsActive, $scope.FlightsType.value).then(function (data) {
                    debugger;
                    if (data.data !== "") {
                        DevExpress.ui.notify(data.data, "warning", 5000);
                    }
                    else {
                        DevExpress.ui.notify("تم التعديل بنجاح", "success", 5000);
                        $scope.IsActive = false;
                        $scope.checkBoxValue = false;
                        $scope.TrackingName = "";
                        $scope.NationalityID = null;
                        $scope.ID = 0;
                        $scope.FlightsType = $scope.FlightsTypePriorities[0];
                        TransportationTrackingSrvc.GetAllTransportationTracking().then(function (data) {
                            if (data) {
                                $scope.TransportationTrackingList = data.data;
                            }
                        });
                    }
                });
            }
           
        }
    };
   
}]);



