(function () {
    app.controller("AdvanceCashPaymentCtrl",
        ["$scope", "$http", function ($scope, $http) {

            $scope.message = 'تم تحصيل السلف بنجاح';
            $scope.selectedRowsKeys = [];
            $scope.selectedRowsData = [];
            $scope.MDL_PaymentDate = new Date();


            /*********************************** Permissions *********************************/
            $scope.Permissions = {
                View: false,
                ExchangeSelected: false
            };

            $http({
                method: "Get",
                url: "/Advances/GetAdvancePaymentPermissions?screenId=94"
            }).then(function (data) {
                debugger;
                $scope.Permissions.View = data.data.View ? data.data.View : false;
                $scope.Permissions.ExchangeSelected = data.data.ExchangeSelected ? data.data.ExchangeSelected : false;

            });
            /*--------------------------------* Permissions *--------------------------------*/



            //=========================================================
            // القائمة الخاصة بالسلف المعتمدة والتي تم صرفها بالفعل
            //=========================================================
            var DataSourcePaidAdvancesRequest = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: false,
                key: "AdvanceRequestId",
                loadMode: "raw",
                load: function () {
                    return $.getJSON("/Advances/DataSourcePaidAdvancesRequest", function (data) { debugger; });
                }
            });
            $scope.PaidAdvancesRequestGrid = {
                dataSource: DataSourcePaidAdvancesRequest,
                bindingOptions: {
                    rtlEnabled: true
                },
                sorting: {
                    mode: "multiple"
                },
                wordWrapEnabled: true,
                showBorders: true,
                searchPanel: {
                    visible: true,
                    placeholder: "بحث",
                    width: 300
                },
                paging: {
                    pageSize: 10
                },
                pager: {
                    allowedPageSizes: "auto",
                    infoText: "( صفحة  {0} من {1} ( {2} عنصر",
                    showInfo: true,
                    showNavigationButtons: true,
                    showPageSizeSelector: true,
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
                width: '100%',
                columnChooser: {
                    enabled: true
                },
                "export": {
                    enabled: true,
                    fileName: "PaidAdvancesRequest"
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
                        confirmDeleteMessage: "تأكيد حذف هذا الطلب ؟",
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
                    //width: "20px",
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
                        width: 110
                    },
                    {
                        dataField: "StudentAcademicData.STUDENT_ID",
                        caption: "رقم جامعي",
                        alignment: "right",
                        width: 110
                    },
                    {
                        dataField: "StudentAcademicData.NATIONALITY_DESC",
                        caption: "الجنسية",
                        alignment: "right",
                        width: 110
                    },
                    {
                        dataField: "StudentAcademicData.FACULTY_NAME",
                        caption: "الكلية",
                        alignment: "right",
                        width: 110
                    },
                    {
                        dataField: "PaidDate",
                        caption: "تاريخ الصرف",
                        alignment: "right",
                        dataType: "date",
                        width: 120
                    },
                    {
                        dataField: "AdvanceName",
                        caption: "السلفة",
                        alignment: "right",
                        width: 120
                    },
                    {
                        dataField: "LastReceiveDate",
                        caption: "تاريخ أخر سداد",
                        alignment: "right",
                        dataType: "date",
                        width: 120
                    },
                    {
                        dataField: "ApprovedValue",
                        caption: "مبلغ السلفة",
                        alignment: "right",
                        width: 110
                    },
                    {
                        dataField: "ReceiveAmount",
                        caption: "المحصل",
                        alignment: "right",
                        width: 90
                    },
                    {
                        dataField: "RemainAmount",
                        caption: "المتبقي",
                        alignment: "right",
                        width: 90
                    }
                ],
                allowColumnResizing: true,
                columnResizingMode: "widget",
                allowColumnReordering: true,
                showColumnLines: true,
                rowAlternationEnabled: true,
                onInitialized: function (e) {
                    $scope.PaidAdvancesRequestGridInstance = e.component;
                },
                onSelectionChanged: function (selectedItems) {
                    debugger;
                    $scope.selectedRowsKeys = selectedItems.selectedRowKeys.join(',');
                    $scope.selectedRowsData = selectedItems.selectedRowsData.map(i => i.RemainAmount);

                    $scope.PayMultiAdvancebtnText = selectedItems.currentSelectedRowKeys.length > 1 ? "تحصيل أكثر من سلفة" : "تحصيل السلفة المختارة";
                },
                masterDetail: {
                    enabled: true,
                    template: "RemainAmountDetailContent"
                }
            };

            $scope.GetRemainAmountDetailGrid = function (key) {
                debugger;
                return {
                    dataSource: new DevExpress.data.DataSource({
                        paginate: true,
                        cacheRawData: false,
                        key: "AdvanceRequestId",
                        loadMode: "raw",
                        load: function () {
                            return $.getJSON("/Advances/GetAdvanceRemainAmount?advanceRequestId=" + key, function (data) { debugger; });
                        }
                    }),
                    sorting: {
                        mode: "multiple"
                    },
                    wordWrapEnabled: false,
                    showBorders: true,
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
                        //{
                        //    dataField: "AdvanceRequestValue",
                        //    caption: "الملبغ المطلوب",
                        //    alignment: "center"
                        //},
                        {
                            dataField: "AdvanceApprovedValue",
                            caption: "مبلغ السلفة",
                            alignment: "center"
                        },
                        {
                            dataField: "PaidAmount",
                            caption: "المبلغ المحصل",
                            alignment: "center"
                        },
                        {
                            dataField: "RemainAmount",
                            caption: "المبلغ المتبقي",
                            alignment: "center"
                        }
                    ],
                    masterDetail: {
                        enabled: true,
                        template: "PaidAmountDetailGridContent"
                    }
                };
            };

            $scope.GetPaidAmountDetailGrid = function (key) {
                return {
                    dataSource: new DevExpress.data.DataSource({
                        paginate: true,
                        cacheRawData: false,
                        key: "AdvanceRequestId",
                        loadMode: "raw",
                        load: function () {
                            return $.getJSON("/Advances/GetAdvancePaidAmount?advanceRequestId=" + key, function (data) { debugger; });
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
                            dataField: "PaidAmount",
                            caption: "القيمة المحصلة",
                            alignment: "center"
                        },
                        {
                            dataField: "PaymentType",
                            caption: "طريقة السداد",
                            alignment: "center"
                        },
                        {
                            dataField: "PayRollNumber",
                            caption: "رقم المستند",
                            alignment: "center"
                        },
                        {
                            dataField: "PayrollDate",
                            caption: "تاريخ السداد",
                            alignment: "center"
                        }
                    ],
                    summary: {
                        totalItems: [{
                            column: "PaidAmount",
                            summaryType: "sum",
                            displayFormat: "الإجمالي :{0}"
                        }]
                    }
                };
            };

            $scope.PayMultiAdvancebtnText = "تحصيل السلفة المختارة";

            $scope.PayMultiAdvance = {
                payMultiAdvancebuttonOptions: {
                    bindingOptions: { text: "PayMultiAdvancebtnText" },
                    hint: "تحصيل السلفة",
                    icon: "fa fa-money",
                    type: 'success',
                    rtlEnabled: true,
                    useSubmitBehavior: false,
                    onClick: function () {
                        debugger;
                        if (!$scope.Permissions.ExchangeSelected) {
                            swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه العملية", "warning");
                            return;
                        }

                        if ($scope.selectedRowsKeys.length === 0) {
                            swal("تنبيه", "عفوا لم يتم اختيار أي سلف لتنفيذ عملية للسداد", "warning");
                            return;
                        } else {

                            $http({
                                method: "GET",
                                url: "/Advances/GetTreasuryAccount/"
                            }).then(function (data) {
                                if (data.data !== "") {
                                    debugger;
                                    $scope.PopUpShow = true;
                                    $scope.MDL_PaymentCOAID_Disabled = data.data.IsAccountant;
                                    $scope.MDL_PaymentCOAID = data.data.COAID;

                                } else {
                                    $scope.PopUpShow = true;
                                }
                            });
                        }
                    }

                }
            };

            $scope.PopupOptions = {
                showTitle: true,
                dragEnabled: false,
                shadingColor: "rgba(0, 0, 0, 0.69)",
                closeOnOutsideClick: false,
                bindingOptions: {
                    visible: "PopUpShow"
                },
                rtlEnabled: true,
                title: "تحصيل السلف المختارة",
                width: 430,
                height: 520,
                contentTemplate: "PopupContent",
                onHiding: function () {

                }
            };

            $scope.MDL_PayAllRemain = false;
            $scope.MDL_IsPosted = false;
            $scope.MDL_IsNotPosted = !$scope.MDL_IsPosted;

            var PaymentCOAIDDataSource = new DevExpress.data.DataSource({
                key: "Value",
                paginate: true,
                cacheRawData: true,
                loadMode: "raw",
                load: function (loadOptions) {
                    return $.getJSON("/Advances/GetChildAcctounts", function (data) { });
                }
            });
            PaymentCOAIDDataSource.reload();

            $scope.AdvancePaymentPopUp = {
                ValidationRules: {
                    PaymentCOAID: {
                        validationGroup: "AdvancePayment",
                        validationRules: [
                            {
                                type: "required",
                                message: "حقل اجبارى"
                            }
                        ]
                    }
                },
                PaymentCOAID: {
                    dataSource: PaymentCOAIDDataSource,
                    itemTemplate: function (data) {
                        return "<div title='" + data.Text + "' value='" + data.Value + "'>" + data.Text + "</div>";
                    },
                    displayExpr: "Text",
                    valueExpr: "Value",
                    showBorders: true,
                    searchEnabled: true,
                    showClearButton: true,
                    rtlEnabled: true,
                    bindingOptions: { value: "MDL_PaymentCOAID"/*, disabled: "!MDL_PaymentCOAID_Disabled"*/ },
                    placeholder: "اختر حساب الصرف"
                },
                PaymentValue:
                {
                    bindingOptions: { value: "MDL_PaymentValue", disabled: "PaymentValueDisabled" },
                    showSpinButtons: true,
                    showClearButton: true,
                    placeholder: "أدخل المبلغ المحصل",
                    rtlEnabled: true,
                    min: 1,
                    onValueChanged: function (e) {
                        $scope.MDL_PaymentValue = e.value;
                    }
                },
                JournalDesc:
                {
                    bindingOptions: { value: "MDL_JournalDesc" },
                    placeholder: "أدخل ملاحظات تحصيل السلفة",
                    rtlEnabled: true,
                    height: 120,
                    onValueChanged: function (e) {
                        $scope.MDL_JournalDesc = e.value;
                    }
                },
                PayAllRemain:
                {
                    bindingOptions: { value: "MDL_PayAllRemain" },
                    rtlEnabled: true,
                    onValueChanged: function (e) {
                        if (e.value === true)
                            $scope.MDL_PaymentValue = $scope.selectedRowsData.reduce((a, b) => a + b, 0);
                        else $scope.MDL_PaymentValue = '';

                        $scope.PaymentValueDisabled = e.value;
                        $scope.MDL_PayAllRemain = e.value;
                    },
                    text: "تحصيل مبلغ السلفه كامل"
                },
                IsPosted:
                {
                    bindingOptions: { value: "MDL_IsPosted" },
                    rtlEnabled: true,
                    onValueChanged: function (e) {
                        $scope.MDL_IsPosted = e.value;
                        $scope.MDL_IsNotPosted = !e.value;
                    },
                    text: "ترحيل القيد للحسابات العامة مباشرة"
                },
                IsNotPosted:
                {
                    bindingOptions: { value: "MDL_IsNotPosted" },
                    rtlEnabled: true,
                    onValueChanged: function (e) {
                        $scope.MDL_IsNotPosted = e.value;
                        $scope.MDL_IsPosted = !e.value;
                    },
                    text: "ترحيل القيد للقيود المجمعة"
                },
                PaymentDate:
                {
                    rtlEnabled: true,
                    type: "date",
                    disabled: false,
                    bindingOptions: { value: "MDL_PaymentDate" },
                    onValueChanged: function (e) {
                        $scope.MDL_PaymentDate = e.value;
                    }
                },
                SaveButton: {
                    text: "تأكيد التحصيل",
                    hint: "تحصيل",
                    icon: "save",
                    type: "success",
                    validationGroup: "AdvancePayment",
                    useSubmitBehavior: true,
                    onClick: function (e) {
                        debugger;
                        var validationGroup = DevExpress.validationEngine.getGroupConfig("AdvancePayment");
                        if (validationGroup) {
                            var result = validationGroup.validate();
                            if (result.isValid) {
                                debugger;
                                if ($scope.MDL_PayAllRemain !== true &&
                                    //Math.min(...$scope.selectedRowsData)
                                    $scope.MDL_PaymentValue > $scope.selectedRowsData.reduce((a, b) => a + b, 0)) {
                                    return swal("تنبيه", "لايمكن أن يتعدي المبلغ المحصل القيمة المتبقية", "warning");
                                }

                                $http({
                                    method: "POST",
                                    url: "/Advances/PayAdvances/",
                                    data: {
                                        AdvanceRequestIds: $scope.selectedRowsKeys,
                                        PaymentCoaId: $scope.MDL_PaymentCOAID,
                                        PaymentValue: $scope.MDL_PaymentValue,
                                        DocDesc: $scope.MDL_JournalDesc,
                                        PayAllRemain: $scope.MDL_PayAllRemain,
                                        postJournal: $scope.MDL_IsPosted,
                                        PaymentDate: $scope.MDL_PaymentDate
                                    }
                                }).then(function (data) {
                                    if (data.data.status === 200) {
                                        debugger;
                                        DevExpress.ui.notify({ message: "تم التحصيل السلف بنجاح", type: data.data.Type, displayTime: 3000, closeOnClick: true });

                                        $scope.studentCount = data.data.Message.split(',').length;
                                        $scope.MDL_AdvanceReceiveMasterIds = data.data.Message;

                                        $scope.MDL_IsPosted = '';
                                        $scope.MDL_JournalDesc = '';
                                        $scope.MDL_PaymentCOAID = '';
                                        $scope.MDL_PaymentValue = '';
                                        $scope.MDL_PayAllRemain = '';
                                        Location.reload();
                                        //validationGroup.reset();
                                        //DataSourcePaidAdvancesRequest.reload();

                                        //$scope.PopUpShow = false;
                                        //$scope.visibleAlertPopUp = true;
                                    } else {
                                        if (data.data !== "") { swal("حدث خطأ", data.data, "error"); }
                                    }
                                });
                            }
                        }
                    }
                },
                CancelButton: {
                    text: "إلغاء العملية",
                    hint: "إلغاء",
                    icon: "close",
                    type: "danger",
                    onClick: function (e) {
                        var validationGroup = DevExpress.validationEngine.getGroupConfig("AdvancePayment");

                        $scope.MDL_PaymentCOAID = '';
                        $scope.MDL_PaymentValue = '';
                        $scope.MDL_JournalDesc = '';
                        $scope.MDL_PayAllRemain = '';
                        $scope.MDL_IsPosted = '';

                        $scope.PopUpShow = '';
                        validationGroup.reset();
                    }
                }

            };

            $scope.AlertPopupOptions = {
                width: 640,
                height: 215,
                contentTemplate: "AlertPopupContent",
                showTitle: false,
                shadingColor: "rgba(0, 0, 0, 0.69)",

                dragEnabled: false,
                closeOnOutsideClick: true,
                bindingOptions: {
                    visible: "visibleAlertPopUp"
                }

            };

            // Popup btns...
            $scope.btnClose = {
                type: "danger",
                text: "إغلاق",
                onClick: function (e) {
                    $scope.visibleAlertPopUp = false;
                    $scope.selectedRowsData = null;
                    $scope.selectedRowsKeys = null;
                    $scope.PopUpShow = false;
                    $scope.Refresh();
                }
            };

            $scope.btnPrintDoc = {
                icon: "print",
                type: "default",
                text: "",
                onClick: function (e) {
                    window.location.href = "/Advances/ReceiveDocAdvance?DocMasterIds=" + $scope.MDL_AdvanceReceiveMasterIds;
                    $scope.PopUpShow = false;

                    $scope.Refresh();
                }
            };

            $scope.btnHome = {
                icon: "search",
                type: "success",
                text: "",
                onClick: function (e) {
                    window.location.href = "/Students/AdvancedSearch";
                    $scope.visibleAlertPopUp = false;
                    $scope.PopUpShow = false;
                    $scope.Refresh();
                }
            };

            $scope.Refresh = function () {
                var validationGroup = DevExpress.validationEngine.getGroupConfig("AdvancePayment");

                $scope.MDL_PaymentCOAID = '';
                $scope.MDL_PaymentValue = '';
                $scope.MDL_JournalDesc = '';
                $scope.MDL_PayAllRemain = '';
                $scope.MDL_IsPosted = '';
                validationGroup.reset();
            };

        }]);

})();