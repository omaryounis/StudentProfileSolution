app.controller("StudentAdvertisementCtrl", ["$scope", '$rootScope', 'StudentAdvertisementSrvc', function ($scope, $rootScope, StudentAdvertisementSrvc) {
    //Filed
    $scope.StudentAdvertisementList = [];
    $scope.TravelRequestsDetailsList = [];
    $scope.AllowStudent = false;
    $scope.IsValidStudentData = false;
    $scope.NotifyMsg = "";
    var date = new Date()
    var ToDayDate = new Date(date.getFullYear(), date.getMonth(), date.getDate());

    $scope.popupStData = {
        title: 'السماح للطالب'
    }
    //dataGrid
    $scope.gridStudentAdvertisement = {
        bindingOptions: {
            dataSource: "StudentAdvertisementList"
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
        //filterRow: {
        //    visible: true,
        //    operationDescriptions: {
        //        between: "بين",
        //        contains: "تحتوى على",
        //        endsWith: "تنتهي بـ",
        //        equal: "يساوى",
        //        greaterThan: "اكبر من",
        //        greaterThanOrEqual: "اكبر من او يساوى",
        //        lessThan: "اصغر من",
        //        lessThanOrEqual: "اصغر من او يساوى",
        //        notContains: "لا تحتوى على",
        //        notEqual: "لا يساوى",
        //        startsWith: "تبدأ بـ"
        //    },
        //    scrolling: {
        //        rtlEnabled: true,
        //        useNative: true,
        //        scrollByContent: true,
        //        scrollByThumb: true,
        //        showScrollbar: "onHover",
        //        mode: "standard", // or "virtual"
        //        direction: "both"
        //    },
        //    resetOperationText: "الوضع الافتراضى"
        //},
        rowAlternationEnabled: true,
        allowColumnReordering: true,
        allowColumnResizing: true,
        columnAutoWidth: true,
        columnChooser: {
            enabled: false
        },
        columns: [
            {
                caption: "اسم الاعلان",
                dataField: "AdName",
                cssClass: "text-center",
            },
            {
                caption: "فترة الاعلان",
                dataField: "AdDate",
                cssClass: "text-center",
            },
            //{
            //    caption: "نهاية الاعلان",
            //    dataField: "AdEndDate",
            //    cssClass: "text-center",
            //},
            {
                caption: "فترة السفر",
                dataField: "DepartingDate",
                cssClass: "text-center"
            },
            //{
            //    caption: "نهاية السفر",
            //    dataField: "DepartingEnd",
            //    cssClass: "text-center",
            //},
            {
                caption: "فترة  العودة",
                dataField: "ReturningDate",
                //calculateCellValue: function (rowData) {

                //    return "("+rowData.ReturningStart+")"+  "-" +"("+ rowData.ReturningEnd+")";
                //},
                cssClass: "text-center"
            },
            {
                caption: "رقم التذكرة",
                dataField: "TicketNumber",
                cssClass: "text-center",
            },
            {
                caption: "ملأ الرغبات",
                width: 200,
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    debugger;
                    var partDate = options.data.AdEndDate.split('/');
                    if (new Date(partDate[2], partDate[1] - 1, partDate[0]) >= ToDayDate && options.data.IsActive == true) {
                        debugger;
                        StudentAdvertisementSrvc.CheckStudentAllowAdvertisement(options.data.ID).then(function (data) {
                            debugger;
                            if (data.data.status == 200) {
                                StudentAdvertisementSrvc.CheckStudentTicketAdvertisement(options.data.ID).then(function (data) {
                                    debugger;
                                    if (data.data.status == 500) {
                                        $("<div />").dxButton({
                                            //icon: "fa fa-save",
                                            text: "تعديل",
                                            type: "success",
                                            hint: "تعديل",
                                            elementAttr: { "class": "btn btn-success" },
                                            onClick: function (e) {
                                                debugger;
                                                location.href = "/Musafer/Index?adID=" + options.data.ID;
                                            }
                                        }).appendTo(container);
                                    }
                                    if (data.data.status == 200) {
                                        $("<div />").dxButton({
                                            //icon: "fa fa-save",
                                            text: "ملأ الرغبات",
                                            type: "primary",
                                            hint: "ملأ الرغبات",
                                            elementAttr: { "class": "btn btn-primary" },

                                            onClick: function (e) {
                                                debugger;
                                                //CheckStudentData
                                                StudentAdvertisementSrvc.CheckStudentData(options.data.ID).then(function (data) {
                                                    debugger;
                                                    if (data.data.status == 500) {
                                                        swal('', data.data.Message, data.data.Type)
                                                        //  DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                                    }
                                                    if (data.data == true)
                                                        location.href = "/Musafer/Index?adID=" + options.data.ID;
                                                });
                                            }
                                        }).appendTo(container);
                                    }

                                });

                            }
                        });

                    }

                }
            },
            {
                caption: "تفاصيل الرغبات",
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    if (options.data.ShowDetalies === true) {
                        $("<div />").dxButton({
                            text: 'متابعه حالة الرغبه',
                            icon: "fa fa-info",
                            width: 150,
                            type: "default",
                            hint: 'متابعه حالة الرغبه',
                            onClick: function (e) {
                                StudentAdvertisementSrvc.GetStudentTravelRequestData(Number(options.data.ID)).then(function (data) {
                                    $scope.TravelRequestsDetailsList = data.data;
                                    $scope.StudentDetaliesPopUpShow = true;
                                });

                            }
                        }).appendTo(container);
                    }
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
            //CheckAllowStudent
            StudentAdvertisementSrvc.CheckAllowStudent().then(function (data) {
                $scope.AllowStudent = data.data;
                //if (data.data == false)
                //    $("#popupStData").modal("show");
            });
            StudentAdvertisementSrvc.IsValidStudentData().then(function (data) {
                debugger;
                if (data.data.status == 200) {
                    $scope.IsValidStudentData = true;
                    swal("تم!", data.data.Message, "success").then((value) => {
                        StudentAdvertisementSrvc.GetStudentAdvertisement().then(function (data) {
                            $scope.StudentAdvertisementList = data.data;
                        });
                    });
                } else {
                    $scope.IsValidStudentData = false;
                    // swal("تنبيه", data.data.Message, "warning");
                    swal({
                        title: "تنبيه!",
                        text: data.data.Message,
                        icon: "warning",
                        className: "red-bg"

                    });

                }

            });




        }
    }

    $scope.btnClose = {
        text: 'اغلاق',
        type: 'danger',
        onClick: function (e) {
            location.href = "/Home/Index";
        }
    };
    $scope.PopupStudentDetalies = {
        showTitle: true,
        dragEnabled: false,
        shadingColor: "rgba(0, 0, 0, 0.69)",
        closeOnOutsideClick: false,
        bindingOptions: {
            visible: "StudentDetaliesPopUpShow"
        },
        contentTemplate: 'PopupStudentDetaliesContent',
        title: "",
        width: 1200,
        height: 500,
        rtlEnabled: true,
        onHiding: function (e) {
            debugger;
            StudentAdvertisementSrvc.GetStudentAdvertisement().then(function (data) {
                $scope.StudentAdvertisementList = data.data;
            });
        }
    };




    //var DataSourceRequestsDetailsGrid = new DevExpress.data.DataSource({
    //    paginate: true,
    //    loadMode: "raw",
    //    load: function () {
    //        return $.getJSON(`/Musafer/GetStudentTravelRequestData?ID=${$scope._RequestID}`, function (data) {
    //            $scope._RequestID = null;
    //        });
    //    }
    //});

    $scope.RequestsDetailsGrid = {
        bindingOptions: {
            dataSource: "TravelRequestsDetailsList"
        },
        sorting: {
            mode: "multiple"
        },
        wordWrapEnabled: false,
        showBorders: false,
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
        //filterRow: {
        //    visible: true,
        //    operationDescriptions: {
        //        between: "بين",
        //        contains: "تحتوى على",
        //        endsWith: "تنتهي بـ",
        //        equal: "يساوى",
        //        greaterThan: "اكبر من",
        //        greaterThanOrEqual: "اكبر من او يساوى",
        //        lessThan: "اصغر من",
        //        lessThanOrEqual: "اصغر من او يساوى",
        //        notContains: "لا تحتوى على",
        //        notEqual: "لا يساوى",
        //        startsWith: "تبدأ بـ"
        //    },
        //    resetOperationText: "الوضع الافتراضى"
        //},
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
        columnChooser: {
            enabled: false
        },
        columns: [
            {
                dataField: "Departing",
                caption: "تاريخ الذهاب",
                alignment: "right",
                width: 110
            }
            ,
            {
                dataField: "Returning",
                caption: "تاريخ العوده",
                alignment: "right",
                width: 120
            }
            ,
            {
                dataField: "InsertDate",
                caption: "تاريخ الطلب",
                alignment: "right",
                width: 100
            }
            ,
            {
                dataField: "TravelOrder",
                caption: " رقم أمر الإركاب ",
                alignment: "right",
                width: 120
            }
            ,
            {
                dataField: "GivenAmount",
                caption: "القيمة ",
                alignment: "right",
                width: 100
            }
            ,
            {
                dataField: "AgentRefNumber",
                caption: "رقم الطيار ",
                alignment: "right",
                width: 100
            }
            ,
            {
                dataField: "TicketNumber",
                caption: "رقم التذكرة ",
                alignment: "right",
                width: 110
            },
            {
                dataField: "Tracking",
                caption: "خط السير ",
                alignment: "right",
                width: 250
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

}]);
