app.controller("ReceiveAdvancesAggregateJournalCtrl",
    ["$scope", "$http", "$timeout",
        function ($scope, $http, $timeout) {

            $scope.selectedRowsKeys = [];

            /*********************************** Permissions *********************************/
            $scope.Permissions = {
                View: false,
                ExchangeSelected: false,
                CheckVoucher: false
            };


            debugger;
            $http({
                method: "Get",
                url: "/Advances/GetAdvancePaymentPermissions?screenId=97"
            }).then(function (data) {
                debugger;
                $scope.Permissions.View = data.data.View ? data.data.View : false;
                $scope.Permissions.ExchangeSelected = data.data.ExchangeSelected ? data.data.ExchangeSelected : false;
                $scope.Permissions.CheckVoucher = data.data.CheckVoucher ? data.data.CheckVoucher : false;


            });
            /*--------------------------------* Permissions *--------------------------------*/

            //-----------Button ترحيل--------------

            $scope.CollectibalMultiDocAdvancesbtnText = "ترحيل قيد مجمع بالسندات";
            $scope.CollectiblePostDocAdvancesBtn = {
                bindingOptions: { text: "CollectibalMultiDocAdvancesbtnText" },
                hint: "ترحيل",
                type: "danger",
                icon: "clear",
                rtlEnables: true,
                useSubmitBehavior: false,
                onClick: function () {
                    if (!$scope.Permissions.ExchangeSelected) {
                        return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه العملية", "warning");

                    }
                    if ($scope.selectedRowsKeys.length === 0) {
                        swal("تنبيه", "عفوا لم يتم اختيار أي سند لتنفيذ عملية الترحيل", "warning");
                        return;
                    } else {
                        $scope.PostedJournalPopupShow = true; 
                    }
                }

            };


            //----------MainGrid DataSource----------------
            var DataSourceCollectibleDocAdvancesGrid = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: false,
                key: "MasterID",
                loadMode: "raw",
                load: function () {
                    return $.getJSON("/Advances/GetDocsOfCollectiblAdvances", function (data) {
                    });
                }
            });
            $scope.CollectibleDocAdvancesGrid = {
                dataSource: DataSourceCollectibleDocAdvancesGrid,
                bindingOptions: { rtlEnabled: true },
                sorting: { model: "multiple" },
                wordWrapEnabled: true,
                showBoarders: true,
                searchPanel: { visible: true, placeholder: "بحث", width: 300 },
                paging: { pageSize: 10 },
                pager: {
                    allowedPageSizes: "auto", infoText: "( صفحة  {0} من {1} ( {2} عنصر", showInfo: true, showNavigationButtons: true,
                    showPageSizeSelector: true, visible: "auto"
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
                }, headerFilter: {
                    visible: true
                },
                showRowLines: true,
                groupPanel: {
                    visible: false,
                    emptyPanelText: "اسحب عمود هنا"
                },
                noDataText: "لايوجد بيانات",
                columnAutoWidth: true,
                width: '100%',
                columnChooser: {
                    enabled: true
                },
                "export": {
                    enabled: true,
                    fileName: "PaidDocAdvances"
                },
                onCellPrepared: function (e) {
                    if (e.rowType === "header" && e.column.command === "edit") {
                        e.column.width = 80;
                        e.column.alignment = "center";
                        e.cellElement.text(" حذف ");
                    }

                    if (e.rowType === "data" && e.column.command === "edit") {
                        $links = e.cellElement.find(".dx-link");
                        $links.text("");
                        $links.filter(".dx-link-delete").addClass("btn btn-danger btn-sm fa fa-trash-o");
                    }
                },
                editing: {
                    allowUpdating: false,
                    allowAdding: false,
                    allowDeleting: false,
                    mode: "row",
                    texts: {
                        confirmDeleteMessage: "",
                        deleteRow: "",
                        editRow: "",
                        addRow: ""
                    },
                    useIcons: true
                },
                selection: {
                    mode: "multiple",
                    showCheckBoxesMode: "always",
                    selectAllMode: "allPages",
                    width: "40px"
                },
                columns: [
                    {
                        dataField: "StudentAcademicData.STUDENT_NAME",
                        caption: "الطالب",
                        alignment: "right"
                    },
                    {
                        dataField: "StudentAcademicData.NATIONAL_ID",
                        caption: "رقم الهوية",
                        alignment: "right",
                        width: 120
                    },
                    {
                        dataField: "StudentAcademicData.STUDENT_ID",
                        caption: "الرقم الجامعي",
                        alignment: "right",
                        width: 130
                    },
                    {
                        dataField: "StudentAcademicData.FACULTY_NAME",
                        caption: "الكلية",
                        alignment: "right",
                        width: 130
                    },
                    {
                        dataField: "DocNumber",
                        caption: "رقم السند",
                        alignment: "right",
                        width: 130
                    },
                    {
                        dataField: "DocTotalValue",
                        caption: "المبلغ الكلي",
                        alignment: "right",
                        width: 120
                    },
                    {
                        dataField: "InsertionDate",
                        caption: "تاريخ التحصيل",
                        alignment: "right",
                        width: 120
                    },
                    {
                        dataField: "AccountName",
                        caption: "اسم حساب التحصيل",
                        alignment: "right",
                        width: 230
                    },
                    {
                        dataField: "DocHeader",
                        caption: "بيان السند",
                        alignment: "right",
                        width: 130,
                        visible: false
                    }
                ],
                allowColumnResizing: true,
                columnResizingMode: "widget",
                allowColumnReordering: true,
                showColumnLines: true,
                rowAlternationEnabled: true,
                onInitialized: function (e) {
                    $scope.PaidDocAdvancesGridInstance = e.component;
                },
                onSelectionChanged: function (selectedItems) {
                    $scope.selectedRowsKeys = selectedItems.selectedRowKeys.join(',');

                    $scope.CollectibalMultiDocAdvancesbtnText = selectedItems.currentSelectedRowKeys.length > 1 ? "ترحيل القيد للسندات المحددة" : "ترحيل القيد للسند المحدد";
                },
                masterDetail: {
                    enabled: true,
                    template: "DocDetailContent"
                }


            };
            //-------------End---------------


            //----------Detailed Grid DataSource --------------

            $scope.GetDocDetaisGrid = function (key) {
                return {
                    dataSource: new DevExpress.data.DataSource({
                        paginate: true,
                        cacheRawData: false,
                        key: "ID",
                        loadMode: "raw",
                        load: function () {

                            return $.getJSON("/Advances/GetDocDetailsReceiveAdvance?type=P&masterId=" + key, function (data) { });
                        }
                    }),
                    columnAutoWidth: true,
                    showBorders: true,
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
                    headerFilter: {
                        visible: false
                    },
                    showRowLines: true,
                    groupPanel: {
                        visible: false,
                        emptyPanelText: "اسحب عمود هنا"
                    },
                    noDataText: "لايوجد بيانات",
                    editing: {
                        allowUpdating: false,
                        allowAdding: false,
                        allowDeleting: false,
                        mode: "row",
                        texts: {
                            confirmDeleteMessage: "",
                            deleteRow: "",
                            editRow: "",
                            addRow: ""
                        },
                        useIcons: true
                    },
                    allowColumnResizing: true,
                    columnResizingMode: "widget",
                    allowColumnReordering: true,
                    showColumnLines: true,
                    rowAlternationEnabled: true,
                    columns: [
                        {
                            dataField: "AccountName",
                            caption: "اسم الحساب",
                            alignment: "center"
                        },
                        {
                            dataField: "GJDDebitAmount",
                            caption: "مدين",
                            alignment: "center"
                        },
                        {
                            dataField: "GJDCreditAmount",
                            caption: "دائن",
                            alignment: "center"
                        },
                        {
                            dataField: "GJDDescrition",
                            caption: "البيان",
                            alignment: "center"
                        }
                    ]

                };
            };
            //----------END ----------------------------


            var PreviewDataSource = new DevExpress.data.DataSource({
                //key: "Dr",
                paginate: true,
                cacheRawData: false,
                loadMode: "raw",
                load: function () {
                    debugger;
                    if ($scope.selectedRowsKeys.length > 0) {
                        return $.getJSON("/Advances/PreviewReceiveDocs?DocMasterIds=" + $scope.selectedRowsKeys, function (data) { debugger; });
                    } else {
                        return [];
                    }
                }
            });


            //-------------- ButtonShow -------------------------
            $scope.DocAdvancesBtnShowText = "معاينة القيد";
            $scope.DocAdvancesBtnShow = {
                bindingOptions: { text: "DocAdvancesBtnShowText" },
                hint: "عرض",
                icon: "clear",
                rtlEnables: true,
                useSubmitBehavior: false,
                onClick: function () {
                    debugger;

                    if (!$scope.Permissions.CheckVoucher) {
                        return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه العملية", "warning");

                    }
                    debugger;
                    if ( $scope.selectedRowsKeys.length === 0) {
                        swal("تنبيه", "عفوا لم يتم اختيار أي سند لتنفيذ عملية الترحيل", "warning");
                        return;
                    }
                    PreviewDataSource.reload();
                }
            };
            //------------------ End


           
            //------------------Grid Preview ----------------------
            $scope.GetDocDetaisGridPreviewInstance = '';
            $scope.GetDocDetaisGridPreview = {

                dataSource: PreviewDataSource,
                columnAutoWidth: true,
                showBorders: true,
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
                headerFilter: {
                    visible: false
                },
                showRowLines: true,
                groupPanel: {
                    visible: false,
                    emptyPanelText: "اسحب عمود هنا"
                },
                noDataText: "لايوجد بيانات",
                editing: {
                    allowUpdating: false,
                    allowAdding: false,
                    allowDeleting: false,
                    mode: "row",
                    texts: {
                        confirmDeleteMessage: "",
                        deleteRow: "",
                        editRow: "",
                        addRow: ""
                    },
                    useIcons: true
                },
                allowColumnResizing: true,
                columnResizingMode: "widget",
                allowColumnReordering: true,
                showColumnLines: true,
                rowAlternationEnabled: true,
                columns: [
                    {
                        dataField: "AccountName",
                        caption: "اسم الحساب",
                        alignment: "center"
                    },
                    {
                        dataField: "Dr",
                        caption: "مدين",
                        alignment: "center"
                    },
                    {
                        dataField: "Cr",
                        caption: "دائن",
                        alignment: "center"
                    },
                    {
                        dataField: "Notes",
                        caption: "البيان",
                        alignment: "center"
                    }
                ],
                onInitialized: function (e) {
                    debugger;
                    $scope.GetDocDetaisGridPreviewInstance = e.component;
                }

            };


            $scope.MDL_PostedDate = new Date();
            $scope.PostJournal = {
                Options: {
                    bindingOptions: { visible: "PostedJournalPopupShow" },
                    width: 650,
                    height: 200,
                    deferRendering: false,
                    showTitle: true,
                    dragEnabled: false,
                    closeOnOutsideClick: false,
                    title: "ترحيل القيد المجمع للحسابات العامة",
                    shadingColor: "rgba(0, 0, 0, 0.69)",
                    contentTemplate: "PostedJournalPopupContent",
                    onHiding: function (e) {

                        // Reset Controls...                          
                        $scope.MDL_PostedDate = new Date();

                        var validationGroup = DevExpress.validationEngine.getGroupConfig("PostedJournalPopup_vg");
                        if (validationGroup) {
                            validationGroup.reset();
                        }

                    }
                },
                DB_PostJournalDate: {
                    bindingOptions: {
                        value: "MDL_PostedDate"
                    },
                    type: "date",
                    displayFormat: "dd/MM/yyyy",
                    onValueChanged: function (e) {
                        $scope.MDL_PostedDate = e.value;
                    }
                },
                ValidationRules: {
                    Required: {
                        validationGroup: "PostedJournalPopup_vg",
                        validationRules: [
                            {
                                type: "required",
                                message: "حقل اجبارى"
                            }
                        ]
                    }
                },
                SaveButton: {
                    text: "ترحيل",
                    hint: "ترحيل",
                    type: "success",
                    validationGroup: "PostedJournalPopup_vg",
                    useSubmitBehavior: true,
                    onClick: function () {
                        var validationGroup = DevExpress.validationEngine.getGroupConfig("PostedJournalPopup_vg");
                        if (validationGroup) {

                            var result = validationGroup.validate();
                            if (result.isValid) {

                                $http({
                                    method: "POST",
                                    url: "/Advances/PostAggregateJournalForReceiveDocs/",
                                    data: {
                                        paramPostedDate: $scope.MDL_PostedDate,
                                        DocMasterIds: $scope.selectedRowsKeys
                                    }
                                }).then(function (data) {
                                    debugger;
                                    if (data.data === "") {
                                        swal("Done", "تم ترحيل القيد المجمع بنجاح", "success");
                                        $scope.PostedJournalPopupShow = false;

                                        //Refresh Grids
                                        $scope.selectedRowsKeys = [];

                                        PreviewDataSource.reload();
                                        DataSourceCollectibleDocAdvancesGrid.reload();
                                    } else {
                                        return swal("تنبيه", data.data, "warning");
                                    }
                                });
                            }
                        }
                    }
                }
            };

        }
    ]);


