(function () {
    app.controller("AdvanceSettingsCtrl",
        [
            "$scope", "$http", function ($scope, $http) {

                $scope.selectedRowsKeys = [];
                var AdvanceSettingConfigGridDataSource = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: false,
                    key: "ID",
                    loadMode: "raw",
                    load: function () {
                        return $.getJSON("/Advances/GetAdvanceSettingConfig", function (data) { });
                    }
                });

                var DataSourceLevels = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: false,
                    key: "ID",
                    loadMode: "raw",
                    load: function () {
                        debugger;
                        return $.getJSON("/Advances/GetLevels", function (data) {
                            $scope.LevelsArray = data;
                        });
                    }
                });

                /*********************************** Permissions *********************************/
                $scope.Permissions = {
                    View: false,
                    Save: false
                };


                $scope.GetPermssions = function () {
                    debugger;
                    return $http({
                        method: "Get",
                        url: "/Advances/GetStudentAdvanceSettingConfigPermissions?screenId=80"
                    }).then(function (data) {
                        debugger;
                        $scope.Permissions.View = data.data.View ? data.data.View : false;
                        $scope.Permissions.Save = data.data.Save ? data.data.Save : false;

                        $scope.SubsidiesGridInstance.refresh();
                    });
                }();
                /*--------------------------------* Permissions *--------------------------------*/


                //=================================
                // قائمة ضوابط عدم استحقاق السلف   
                //=================================
                $scope.AdvanceSettingConfigGrid = {
                    dataSource: AdvanceSettingConfigGridDataSource,
                    noDataText: "لا يوجد بيانات",
                    selection: {
                        mode: "single"
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
                        visible: false
                    },
                    showRowLines: true,
                    groupPanel: {
                        visible: true,
                        emptyPanelText: "اسحب عمود هنا"
                    },
                    showBorders: true,
                    "export": {
                        enabled: true,
                        fileName: "AdvanceSettingConfig"
                    },
                    columnChooser: {
                        enabled: true
                    },
                    columnAutoWidth: true,
                    sorting: {
                        mode: "multiple"
                    },
                    wordWrapEnabled: false,
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
                    rowAlternationEnabled: true,
                    allowColumnReordering: true,
                    allowColumnResizing: true,
                    columns: [
                        {
                            caption: '#',
                            cssClass: "text-center",
                            cellTemplate: function (cellElement, cellInfo) {
                                cellElement.text(cellInfo.row.rowIndex + 1);
                            },
                            width: 30
                        },
                        {
                            caption: "العناصر",
                            dataField: "KeyAr",
                            cssClass: "text-right"
                        },
                        {
                            caption: "القيمة",
                            width: 400,
                            dataField: "Value",
                            cssClass: "text-right",
                            visible: true,
                            cellTemplate: function (container, options) {
                                debugger;
                                var key = options.key;
                                $scope.selectedRowsKeys["item" + key.toString()] = options.value.join(',');

                                var div = $("<div />").appendTo(container);
                                if (options.data.Type === "Text" && options.data.Key === "MiniGPA") {

                                    div.dxNumberBox({
                                        type: "text",
                                        min: 0,
                                        max: 5,
                                        value: options.value,
                                        onValueChanged: function (e) {                                           
                                            $scope.selectedRowsKeys["item" + key.toString()] = e.value;                                           
                                        }
                                    });
                                }
                                else if (options.data.Type === "MultiSelect" && options.data.Key === "Levels") {
                                    debugger;

                                    div.dxTagBox({

                                        dataSource: DataSourceLevels,
                                        value: options.value,
                                        displayExpr: "LEVEL_DESC",
                                        valueExpr: "LEVEL_CODE",
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
                                            debugger;                                           
                                            $scope.selectedRowsKeys["item" + key.toString()] = e.value.join(',');                                         
                                            
                                        }
                                    });
                                }
                            }
                        },
                        {
                            caption: "إجراء",
                            width: 100,
                            cssClass: "text-center",
                            cellTemplate: function (container, options) {
                                $("<div />").dxButton({
                                    text: "حفظ",
                                    type: "success",
                                    hint: "حفظ",
                                    disabled: !$scope.Permissions.Save,
                                    elementAttr: { "class": "btn btn-success" },
                                
                                   
                                    onClick: function (e) {
                                        debugger;
                                        $http({
                                            method: "POST",
                                            url: "/Advances/UpdateAdvanceSettingConfig/",
                                            data: {
                                                ID: options.data.ID,
                                                Key: options.data.Key,
                                                Type: options.data.Type,
                                                Value: $scope.selectedRowsKeys["item" + options.key.toString()]
                                            }
                                        }).then(function (data) {
                                            if (data.data === "") {
                                                swal("Done!", "تم الحفظ بنجاح", "success");
                                            } else {
                                                swal("حدث خطأ", data.data, "error");
                                            }
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
                        // $scope.GridRowData = info.selectedRowKeys;
                    },
                    onInitialized: function (e) {

                    }
                };
            }
        ]);
})();