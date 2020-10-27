app.controller("StudentAdvertisementCtrl", ["$scope", "$http", '$rootScope', 'StudentAdvertisementSrvc', function ($scope, $http, $rootScope, StudentAdvertisementSrvc) {
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
                    //var partDate = options.data.AdEndDate.split('/');
                    //if (new Date(partDate[2], partDate[1] - 1, partDate[0]) >= ToDayDate && options.data.IsActive == true) {
                   
                    //}

                    debugger;
                    StudentAdvertisementSrvc.CheckStudentAllowAdvertisement(options.data.ID).then(function (data) {
                        debugger;
                        if (data.data.status == 200) {
                            debugger;
                            StudentAdvertisementSrvc.CheckStudentTicketAdvertisement(options.data.ID).then(function (data) {
                                debugger;
                                //if (options.data.CanEdit == true) {
                                if (data.data.status == 500 && data.data.canEdit == true && options.data.IsActive === true && data.data.isExpiredAd === false) {
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
                                //}
                                if (data.data.status == 200 && data.data.count === 0 && options.data.IsActive === true && data.data.isExpiredAd ===false) {
                                    debugger;
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
            },
            {
                caption: "تفاصيل الرغبات",
                cssClass: "text-center",
                cellTemplate: function (container, options) {

                    var partDate = options.data.AdEndDate.split('/');
                  //  if (/*new Date(partDate[2], partDate[1] - 1, partDate[0]) >= ToDayDate &&*/ options.data.IsActive == true) {

                        $("<div />").dxButton({
                            text: 'متابعه حالة الرغبه',
                            icon: "fa fa-info",
                            width: 150,
                            type: "default",
                            hint: 'متابعه حالة الرغبه',
                            onClick: function (e) {

                                return $http({
                                    method: "Get",
                                    url: "/Musafer/GetStudentTravelRequestData",
                                    params: {
                                        ID: options.data.ID
                                    }
                                }).then(function (data) {
                                    debugger;
                                    $scope.TravelRequestsDetailsList = data.data;
                                    $scope._RequestID = options.data.ID;
                                    DataSourceRequestsDetailsGrid.reload();
                                    $scope._PopUpShow = true;
                                    $('body').css('overflow', 'hidden');
                                });


                            }
                        }).appendTo(container);

                   // }
                }
            }
            ,
            {
                caption: "حذف",
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    debugger;

                    if ((options.data.TravelOrder === 0) && options.data.TravelRequestId !== 0) {
                        $scope._RequestID = options.data.TravelRequestId;
                    $("<div />").dxButton({
                        //text: 'متابعه حالة الرغبه',
                        icon: "fa fa-trash",
                        //width: 150,
                        type: "danger",
                        hint: 'حذف الرغبة',
                        onClick: function (e) {
                            swal({
                                title: 'تأكيد',
                                text: "هل أنت متأكد من حذف هذة الرغبة؟",
                                icon: 'warning',
                                buttons: ["إلغاء", "تأكيد"],
                                confirmButtonText: 'نعم، حذف',
                                cancelButtonText: 'تراجع'
                            }).then((result) => {
                                debugger;
                                if (result) {
                                    return $http({
                                        method: "Post",
                                        url: "/Musafer/DeleteStudentTravelRequestData",
                                        data: {
                                            travelRequestId: options.data.TravelRequestId
                                        }
                                    }).then(function (data) {
                                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                        return $http({
                                            method: "Get",
                                            url: "/Musafer/GetStudentTravelRequestData",
                                            params: {
                                                ID: $scope._RequestID
                                            }
                                        }).then(function (data) {
                                            debugger;
                                            //$scope.TravelRequestsDetailsList = data.data;
                                            //DataSourceRequestsDetailsGrid.reload();
                                            StudentAdvertisementSrvc.GetStudentAdvertisement().then(function (data) {
                                                $scope.StudentAdvertisementList = data.data;
                                            });
                                        });
                                    });
                                }
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
                } else if (data.data.status == 100) {
                    $scope.IsValidStudentData = true; 
                    swal({
                        title: "تنبيه!",
                        text: data.data.Message,
                        icon: "warning",
                        className: "red-bg"

                    }).then((value) => {
                        StudentAdvertisementSrvc.GetStudentAdvertisement().then(function (data) {
                            $scope.StudentAdvertisementList = data.data;
                        });
                    });

                }
                 else {
                        $scope.IsValidStudentData = false;
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


    //var DataSourceRequestsDetailsGrid = new DevExpress.data.DataSource({
    //    paginate: true,
    //    loadMode: "raw",
    //    load: function () {
    //        return $.getJSON(`/Musafer/GetStudentTravelRequestData?ID=${$scope._RequestID}`, function (data) {
    //            debugger;
    //            $scope._RequestID = null;
    //        });
    //    }
    //});

    var DataSourceRequestsDetailsGrid = new DevExpress.data.DataSource({

        paginate: true,
        cacheRawData: true,
        key: "ID",
        loadMode: "raw",
        load: function () {
            if ($scope.TravelRequestsDetailsList.length > 0) {
                debugger;
                return $scope.TravelRequestsDetailsList;
            } else {
                return [];
            }
        }
    });

    $scope.RequestsDetailsGrid = function () {
        return {
            bindingOptions: {
                dataSource: "TravelRequestsDetailsList"
            },
            sorting: {
                mode: "multiple"
            },
            wordWrapEnabled: false,
            showBorders: false,
            width: "100%",
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
            columnChooser: {
                enabled: false
            },
            columns: [
                {
                    dataField: "Tracking",
                    caption: "خط السير",
                    alignment: "right"
                },
                {
                    dataField: "InsertDate",
                    caption: "تاريخ الطلب",
                    alignment: "right"
                }
                ,
                {
                    dataField: "TravelOrder",
                    caption: "قسم التذاكر ",
                    alignment: "right"
                }
                ,
                {
                    dataField: "GivenAmount",
                    caption: "اداره الطيار ",
                    alignment: "right"
                }
                ,
                {
                    dataField: "AgentRefNumber",
                    caption: "الاداره الماليه ",
                    alignment: "right",
                }
                ,
                {
                    dataField: "IsTicketNumber",
                    caption: "اداره السفر ",
                    alignment: "right"
                }
                ,
                {
                    dataField: "TicketNumber",
                    caption: "رقم التذكرة ",
                    alignment: "right"
                },
                {
                    dataField: "Departing",
                    caption: "تاريخ الذهاب ",
                    alignment: "right",
                    width: 110
                },
                {
                    dataField: "Returning",
                    caption: "تاريخ العوده ",
                    alignment: "right"
                }
            ],
            allowColumnResizing: true,
            columnResizingMode: "widget",
            allowColumnReordering: true,
            showColumnLines: true,
            rowAlternationEnabled: true,
            onInitialized: function (e) {

            }
        }

    };

    $scope._PopupOptions = {
        showTitle: true,
        dragEnabled: false,
        shadingColor: "rgba(0, 0, 0, 0.69)",
        closeOnOutsideClick: false,
        bindingOptions: {
            visible: "_PopUpShow"
        },
        contentTemplate: '_PopupContent',
        title: "",
        width: "90%",
        height: 500,
        rtlEnabled: true,
        onHiding: function (e) {
            debugger;
            StudentAdvertisementSrvc.GetStudentAdvertisement().then(function (data) {
                $scope.StudentAdvertisementList = data.data;
            });
        }
    };



}]);
