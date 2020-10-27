(function () {
    app.controller("TravelRequestReviewCtrl", ["$scope", "$http", "$timeout", function ($scope, $http, $timeout) {

        /*********************************** Permissions *********************************/
        $scope.Permissions = {
            Create: false,
            Read: false,
            Update: false,
            Delete: false,
            View: false
        };
        $scope.GetPermssions = function () {
            $http({
                method: "POST",
                url: "/Musafer/TravelRequestReview",
                params: { screenId: 91 }
            }).then(function (data) {
                $scope.Permissions.Create = data.data.Create;
                $scope.Permissions.Read = data.data.Read;
                $scope.Permissions.Update = data.data.Update;
                $scope.Permissions.Delete = data.data.Delete;
                $scope.Permissions.View = data.data.View;
            });
        };
        $scope.GetPermssions();
        /*--------------------------------* Permissions *--------------------------------*/


        /********************************** initial Variables *****************************/
        $scope.AdvertisementNameSelectedValues = '';
        $scope.MDL_AdvertisementName = '';
        $scope.TravelAdvertisementNamesInstance = '';
        $scope.FirstLoad = true;
        $scope.TravelRequestsList = [];
        $scope.RewardGridInstance = '';
        $scope.TravelRequestsGridInstance = '';

        /*--------------------------------* initial Variables *--------------------------------*/


        /********************************** Data Source *****************************/

        var DataSourceTravelAdvertisementNames = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: true,
            key: "Value",
            loadMode: "raw",
            load: function () {
                return $.getJSON("/Musafer/GetTravelAdvertisement", function (data) { debugger; });
            }
        });


        var DataSourceTravelRequestsGrid = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: true,
            type: "array",
            key: "ID",
            loadMode: "raw",
            load: function () {
                debugger;
                if ($scope.FirstLoad) {
                    $scope.FirstLoad = false;
                    return [];
                } else {
                    $scope.Values = $scope.AdvertisementNameSelectedValues === "" ? undefined : $scope.AdvertisementNameSelectedValues;
                    return $.getJSON("/Musafer/GetTravelRequests",
                        { TravelAdvertisementIds: $scope.Values},
                        function (data) { debugger; });
                }

            }
        });
        /*--------------------------------* Data Source *--------------------------------*/


        $scope.DL_TravelAdvertisementNames = {
            dataSource: DataSourceTravelAdvertisementNames,
            bindingOptions: {
                value: "MDL_AdvertisementName"
            },
            displayExpr: "Text",
            valueExpr: "Value",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            onInitialized: function (e) {
                $scope.TravelAdvertisementNamesInstance = e.component;
            },
            multiline: false,
            showBorders: true,
            showSelectionControls: true,
            showDropDownButton: true,
            onValueChanged: function (e) {
                $scope.AdvertisementNameSelectedValues = e.value.join(',');
            },
            onClosed: function (e) {
            }
        };

        $scope.btn_Search = {
            text: "بحث",
            type: 'success',
            icon: 'search',
            useSubmitBehavior: false,
            onClick: function (e) {
                debugger;
                //$http({
                //    method: "GET",
                //    url: "/Musafer/GetTravelRequests",
                //    params: {
                //        TravelAdvertisementIds: $scope.AdvertisementNameSelectedValues === '' ? null : $scope.FacultiesSelectedValues
                //    }
                //}).then(function (data) {
                //    debugger;
                //    $scope.TravelRequestsList = '';
                //    $scope.TravelRequestsList = data.data;
                //    $scope.TravelRequestsGridInstance.refresh();
                //    });

                DataSourceTravelRequestsGrid.reload();
                //$scope.TravelRequestsGridInstance.repaint();
                //$scope.TravelRequestsGridInstance.refresh();
            }
        };

        /********************************* Grid ***********************************/

        $scope.TravelRequestsGrid = {
            dataSource: DataSourceTravelRequestsGrid,
            bindingOptions: {
                rtlEnabled: true
            },
            sorting: {
                mode: "multiple"
            },
            wordWrapEnabled: false,
            showBorders: true,
            searchPanel: {
                visible: true,
                placeholder: "بحث",
                width: 300
            },
            paging: {
                pageSize: 15
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
                visible: true,
                emptyPanelText: "اسحب عمود هنا"
            },
            cssClass: "text-center",
            noDataText: 'لايوجد بيانات',
            columnAutoWidth: true,
            width: "100%",
            columnChooser: {
                enabled: true
            },
            "export": {
                enabled: true,
                fileName: "رغبات السفر"
                //allowExportSelectedData: true
            },
            onCellPrepared: function (e) {
                if (e.rowType === "data" && e.column.command === "edit") {
                    var isEditing = e.row.isEditing,
                        $links = e.cellElement.find(".dx-link");
                    $links.text("");
                    if (isEditing) {
                        $links.filter(".dx-link-save").addClass("dx-icon-save");
                        $links.filter(".dx-link-cancel").addClass("dx-icon-revert");
                    } else {
                        $links.filter(".dx-link-edit").addClass("btn btn-dark-orange btn-sm fa fa-pencil");
                        $links.filter(".dx-link-delete").addClass("btn btn-danger btn-sm fa fa-trash-o");
                    }
                }
            },
            columns: [
                {
                    dataField: "ID",
                    visible: false

                }, {
                    dataField: "Student_Name",
                    caption: "اسم الطالب"

                },
                {
                    dataField: "National_ID",
                    caption: "رقم الهوية"
                    //groupIndex: 0
                },
                {
                    dataField: "Student_ID",
                    caption: "الرقم الجامعي",
                    width: "115"

                },
                {
                    dataField: "STATUS_DESC",
                    caption: "الحالة الأكاديمية",
                    width: "115"

                },
                {
                    dataField: "REMAININGCREDITHOURSCOUNT",
                    caption: "الساعات المتبقيه",
                    width: "115"

                },
                {
                    dataField: "AdName",
                    caption: "إسم الإعلان"
                },
                {
                    dataField: "AdvertisementID",
                    visible: false
                },
                {
                    dataField: "NATIONALITY_DESC",
                    caption: "الجنسية"
                },
                {
                    dataField: "Level_Desc",
                    caption: "المستوي",
                    visible: false
                },
                {
                    dataField: "Faculty_Name",
                    caption: "الكلية",
                    visible: false
                },
                {
                    dataField: "InsertDate",
                    caption: "تاريخ الاضافه",
                    dataType: "date",
                    format: "MM/dd/yyyy"

                },
                
                {
                    dataField: "DepartingDate",
                    caption: "تاريخ المغادرة",
                    dataType: "date",
                    format: "MM/dd/yyyy"

                },
                {
                    dataField: "ReturningDate",
                    caption: "تاريخ العودة",
                    dataType: "date",
                    format: "MM/dd/yyyy"

                },
                {
                    dataField: "Tracking",
                    caption: "خط السير"

                }
                

            ],
            allowColumnResizing: true,
            columnResizingMode: "widget",
            allowColumnReordering: true,
            showColumnLines: true,
            rowAlternationEnabled: true,
            onInitialized: function (e) {
                $scope.TravelRequestsGridInstance = e.component;
            },
            editing: {
                allowUpdating: true,
                allowAdding: true,
                allowDeleting: true,
                mode: "row",
                texts: {
                    confirmDeleteMessage: "تأكيد حذف  !",
                    addRow: "اضافة"
                },
                useIcons: true
            },
            onRowRemoving: function (e) {
                e.cancel = true;
                debugger;
                $http({
                    method: "Delete",
                    url: "/Musafer/DeleteStudentDesire",
                    data: { StudentDesireId: e.data.ID }
                }).then(function (data) {
                    if (data.data !== "") {
                        return DevExpress.ui.notify({ message: data.data }, "Error", 10000);
                    } else {
                        DevExpress.ui.notify({ message: "تم الحذف بنجاح" }, "success", 10000);
                        //var Index = $scope.TravelRequestsList.findIndex(function (Item) {
                        //    return Item.ID === e.data.ID;
                        //});

                        // $scope.TravelRequestsList.splice(Index, 1);
                        // $scope.TravelRequestsGridInstance._options.dataSource.splice(Index, 1);

                        DataSourceTravelRequestsGrid.reload();
                        $scope.TravelRequestsGridInstance.repaint();
                        $scope.TravelRequestsGridInstance.refresh();
                    }
                });
            },
            onEditingStart: function (e) {

                e.cancel = true;
                window.open("/Musafer/EditStudentDesireByAdmin?StudentDesireId=" + e.data.ID, '_self');
            },
            onInitNewRow: function (e) {
                e.cancel = true;
                window.open('/Musafer/StudentDesiresByAdmin', '_self');
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
        };
    }]);
})();

