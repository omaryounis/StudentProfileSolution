(function () {
    app.controller("ActivityDecisionTakingCtrl",
        ["$scope", "$http", "NewAcademicActivitiesSrvc",
            function ($scope, $http, NewAcademicActivitiesSrvc) {

                /* By omar 16/11/2019 */
                // var NewActivityRequestsArray = '';
                var DataSourceNewActivityRequestsGrid = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: true,
                    key: "ActivityRequestId",
                    loadMode: "raw",
                    load: function () {
                        return $.getJSON("/AcademicActivities/GetNewActivitiesRequests", function (data) {

                        });
                    }
                });

                $scope.NewActivityRequestsGrid = {
                    dataSource: DataSourceNewActivityRequestsGrid,
                    //keyExpr: "ActivityRequestId",
                    showBorders: false,
                    bindingOptions: {
                        rtlEnabled: true
                    },
                    sorting: {
                        mode: "multiple"
                    },
                    wordWrapEnabled: false,
                    searchPanel: {
                        width: 1200,
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
                    "export": {
                        enabled: true,
                        fileName: "NewActivitiesRequests"
                    },
                    columns: [
                        {
                            caption: 'مسلسل',
                            cssClass: "text-right",
                            cellTemplate: function (cellElement, cellInfo) {
                                cellElement.text(cellInfo.row.rowIndex + 1);
                            },
                            width: 70
                        },
                        {
                            dataField: "ActivityLocation",
                            caption: "مكان المشاركة",
                            alignment: "right",
                            width: 150
                        },
                        {
                            dataField: "ActivityName",
                            caption: "إسم المشاركة",
                            alignment: "right",
                            width: 170
                        },
                        {
                            dataField: "ActivityNo",
                            caption: "رقم المشاركة",
                            alignment: "right",
                            width: 130
                        },
                        {
                            dataField: "ActivityStartDate",
                            caption: "تاريخ المشاركة",
                            alignment: "right",
                            dataType: "date",
                            width: 150
                        },
                        {
                            dataField: "ActivityStudentNo",
                            caption: "عدد الطلاب المشاركين",
                            alignment: "right",
                            width: 190
                        },
                        {
                            caption: "تحميل المرفقات",
                            cssClass: "text-center",
                            cellTemplate: function (container, options) {
                                $("<div />").dxButton({
                                    text: '',
                                    hint: "تحميل",
                                    type: 'default',
                                    icon: 'fa fa-download',
                                    useSubmitBehavior: false,
                                    onClick: function (e) {
                                        return window.open('/AcademicActivities/DownloadActivityAttachment?AcademicActivitiesId=' + options.data.ActivityRequestId, '_blank');
                                    }
                                }).appendTo(container);
                            }
                        }, {
                            caption: "تفاصيل المشاركة",
                            cssClass: "text-right",
                            cellTemplate: function (container, options) {
                                $("<div />").dxButton({
                                    text: 'تفاصيل المشاركة',
                                    icon: "fa fa-info",
                                    width: 150,
                                    type: "default",
                                    hint: "تفاصيل المشاركة",
                                    onClick: function (e) {
                                        $scope._PopUpShow = true;
                                        $scope._ActivityRequestId = options.data.ActivityRequestId;
                                        DataSourceRequestsDetailsGrid.reload();

                                    }
                                }).appendTo(container);
                            }
                        }
                        , {
                            caption: "إعتماد",
                            cssClass: "text-right",
                            cellTemplate: function (container, options) {
                                $("<div />").dxButton({
                                    text: 'إعتماد المشاركه',
                                    icon: "fa fa-check",
                                    width: 150,
                                    type: "success",
                                    hint: "إعتماد المشاركه",
                                    onClick: function (e) {
                                        // alert(options.data.ActivityRequestId);
                                      //  $scope._decisionPopupVisible = true;
                                        $('#_decisionPopupVisible').dxPopup('instance').option('visible', true);
                                        $scope.ActivityToBeEditted = options.data;
                                        $scope.recommendations = options.data.StatusNotes;

                                    }
                                }).appendTo(container);
                            }
                        }
                    ],
                    allowColumnResizing: true,
                    columnResizingMode: "widget",
                    allowColumnReordering: true,
                    showColumnLines: true,
                    rowAlternationEnabled: false,
                    onInitialized: function (e) {
                        $scope.ActivitiesGridInstance = e.component;
                    },
                    onRowPrepared: function (info) {

                        if (info.rowType === 'data') {
                            if (info.data.IsPosted == true && info.data.ApprovedStatus == false && info.data.IsReturned == true) {
                                info.rowElement.attr('style', 'background-color: #F4CACA !important;');
                            }
                        }
                    }
                };

                //var ActivityRequestsDetailsArray = '';
                var DataSourceActivityRequestsDetailsGrid = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: true,
                    key: "ActivityRequestId",
                    loadMode: "raw",
                    load: function () {
                        return $.getJSON("/AcademicActivities/GetActivityDetails?ActivityId=" + key, function (data) {
                            // ActivityRequestsDetailsArray = data;
                        });
                    }
                });

                /* end  */


                // Popup ...
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
                    width: 950,
                    height: 500,
                    rtlEnabled: true,
                    onHiding: function () {
                        //$scope.ResetControls();
                        //$scope.ActivityToBeEditted = '';
                        //   DataSourceNewActivityRequestsGrid.reload();
                    }
                };

                $scope.decisionPopup = {
                    bindingOptions: {
                        visible: "_decisionPopupVisible"
                    },
                    shadingColor: "rgba(0, 0, 0, 0.69)",
                    closeOnOutsideClick: false,
                    height: 500,
                    showTitle: true,
                    title: "إعتماد المشاركة",
                    dragEnabled: false
                };


                $scope.recommendationsOfTextArea = {
                    bindingOptions: {
                        value: "recommendations"
                    },
                    height: 120,
                    placeholder: "ملاحظات"
                };

                $scope.refuseButton = {
                    text: "رفض",
                    type: "danger",
                    width: "150",
                    onClick: function (e) {

                        if ($scope.recommendations === "" || $scope.recommendations === null || $scope.recommendations === undefined) {
                            DevExpress.ui.notify({
                                message: "عفوا ادخل توصيات رفض المشاركة",
                                type: "error",
                                displayTime: 10000,
                                closeOnClick: true
                            });
                            return;
                        }

                        NewAcademicActivitiesSrvc.RefuseActivityRequest($scope.ActivityToBeEditted.ActivityRequestId, $scope.recommendations)
                            .then(function (data) {
                            //    $scope._decisionPopupVisible = false;
                                $('#_decisionPopupVisible').dxPopup('instance').option('visible', false);
                                if (data.data === "") {
                                    DataSourceNewActivityRequestsGrid.reload();
                                    swal("Done!", "تم الحفظ بنجاح", "success").then((value) => {
                                        $scope.ActivityToBeEditted = '';
                                        return value;
                                    });
                                } else {
                                    swal("حدث خطأ", data.data, "error");
                                }
                            });
                    }
                };


                $scope.acceptButton = {
                    text: "إعتماد المشاركه",
                    type: "success",
                    width: "150",
                    onClick: function (e) {
                        NewAcademicActivitiesSrvc.ApproveActivityRequest($scope.ActivityToBeEditted.ActivityRequestId, $scope.recommendations)
                            .then(function (data) {
                              //  $scope._decisionPopupVisible = false;
                                $('#_decisionPopupVisible').dxPopup('instance').option('visible', false);
                                if (data.data === "") {
                                    DataSourceNewActivityRequestsGrid.reload();
                                    swal("Done!", "تم الحفظ بنجاح", "success").then((value) => {
                                        $scope.ActivityToBeEditted = '';

                                        return value;
                                    });

                                } else {
                                    swal("حدث خطأ", data.data, "error");
                                }
                            });
                    }
                };




                var DataSourceRequestsDetailsGrid = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: true,
                    key: "ActivityRequestId",
                    loadMode: "raw",
                    load: function () {
                        return $.getJSON(`/AcademicActivities/GetActivityDetails?ActivityRequestId=${$scope._ActivityRequestId}`, function (data) {
                            $scope._ActivityRequestId = null;
                        });
                    }
                });

                $scope.RequestsDetailsGrid = {

                    dataSource: DataSourceRequestsDetailsGrid,
                    bindingOptions: {
                        rtlEnabled: true
                    },
                    sorting: {
                        mode: "multiple"
                    },
                    wordWrapEnabled: false,
                    showBorders: false,
                    searchPanel: {
                        width: 790,
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
                    "export": {
                        enabled: true,
                        fileName: "NewActivitiesRequests"
                    },
                    columns: [
                        {
                            caption: 'مسلسل',
                            cssClass: "text-right",
                            cellTemplate: function (cellElement, cellInfo) {
                                cellElement.text(cellInfo.row.rowIndex + 1);
                            },
                            width: 80
                        },
                        {
                            dataField: "FacultyName",
                            caption: "الكلية",
                            alignment: "right",
                            width: 100
                        }
                        ,
                        {
                            dataField: "DegreeName",
                            caption: "المرحلة العلمية",
                            alignment: "right",
                            width: 200
                        }
                        ,
                        {
                            dataField: "StudentName",
                            caption: "الطالب",
                            alignment: "right",
                            width: 200
                        }
                        ,
                        {
                            dataField: "FacultyId",
                            caption: "الرقم الجامعي",
                            alignment: "right",
                            width: 150
                        }
                        ,
                        {
                            dataField: "EvaluationName",
                            caption: "التقييم",
                            alignment: "right",
                            width: 100
                        }
                    ],
                    allowColumnResizing: true,
                    columnResizingMode: "widget",
                    allowColumnReordering: true,
                    showColumnLines: true,
                    rowAlternationEnabled: true,
                    onInitialized: function (e) {
                        $scope.ActivitiesGridInstance = e.component;
                    }
                };
                /*17/11/2019 */
                var selectedRows = [];
                function SelectionChanged(s, e) {
                    var id = s.GetRowKey(e.visibleIndex);
                    s.GetSelectedFieldValues('ActivityRequestId', SelectionCallback);
                }
                function SelectionCallback(values) {
                    selectedRows = [];
                    for (var i = 0; i < values.length; i++) {
                        var Id = values[i];
                        selectedRows.push(Id);
                    }
                }

            }]);
}());