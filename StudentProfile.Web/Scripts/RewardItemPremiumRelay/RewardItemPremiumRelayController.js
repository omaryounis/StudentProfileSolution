app.controller("RewardItemPremiumRelaysCtrl", ["$scope", '$rootScope', '$filter', 'RewardItemPremiumRelaysrvc', function ($scope, $rootScope, $filter, RewardItemPremiumRelaysrvc) {

    //Filed
    $scope.PopupRelaysShow = false;

    $scope.RewardItemPremiumRelaysList = [];

    $scope.selectedRowKeys = [];
    $scope.gridSelectedRowsData = [];
    $scope.selectedRowDetailsKeys = [];

    $scope.ConfirmselectedRowsList = [];

    //dataGrid
    $scope.gridRewardItemPremiumRelays = {
        keyExpr: "ID",
        bindingOptions: {
            dataSource: "RewardItemPremiumRelaysList"
        },
        noDataText: "لا يوجد بيانات",
        showBorders: true,
        paging: {
            pageSize: 10
        },
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [5, 10, 20],
            showInfo: true
        },
        selection: {
            mode: "multiple",
            showCheckBoxesMode: "always",
            selectAllMode: "allPages",
            width: "40px"
        },
        "export": {
            enabled: true,
            fileName: "ترحيل اقسلط السلف"
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
            headerFilter: {
                visible: true,
                allowSearch: true
            },
            showRowLines: false,
            groupPanel: {
                visible: true,
                emptyPanelText: "اسحب عمود هنا"
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
        columns: [
            {
                dataField: "STUDENT_NAME",
                caption: "الطالب",
                alignment: "right"
            },
            {
                dataField: "NATIONAL_ID",
                caption: "رقم الهوية",
                alignment: "right",
                width: 120
            },
            {
                dataField: "STUDENT_ID",
                caption: "الرقم الجامعي",
                alignment: "right",
                width: 130
            },
            {
                dataField: "FACULTY_NAME",
                caption: "الكلية",
                alignment: "right",
                width: 130
            },
            {
                dataField: "ItemName",
                caption: "البند",
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
                caption: "تاريخ الصرف",
                alignment: "right", 
                width: 120
            },
            {
                dataField: "PaidAccountName",
                caption: "اسم حساب الصرف",
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

        masterDetail: {
            enabled: true,
            template: "detail"
        },
        onContentReady: function (e) {
            e.component.columnOption("command:edit",
                {
                    visibleIndex: -1
                });
        },
        onSelectionChanged: function (selectedItems) {
            debugger;
            $scope.selectedRowDetailsKeys = [];
            $scope.selectedRowKeys = selectedItems.selectedRowKeys.join(',');
            $scope.gridselectedRowsData = selectedItems.selectedRowsData;

            for (var i = 0; i < selectedItems.selectedRowsData.length; i++)
                $scope.selectedRowDetailsKeys.push(selectedItems.selectedRowsData[i].ID);

        },
        onInitialized: function (e) {
            debugger;
            RewardItemPremiumRelaysrvc.GetRewardItemPremiumRelays().then(function (data) {
                debugger;
                $scope.RewardItemPremiumRelaysList = data.data;
            });
        }
    };

    $scope.gridRewardItemPremiumRelaysDetais = function (key) {
        debugger;
        return {
            dataSource: new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: false,
                key: "ID",
                loadMode: "raw",
                load: function () {

                    return $.getJSON("/Advances/RewardItemPremiumRelaysDetais/" + key, function (data) { debugger; });
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
                mode: "row"
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
            ],
        };
    };


    $scope.btnConfirm = {
        text: "ترحيل الاقساط المحددة",
        icon: "fa fa-money",
        type: 'success',
        onClick: function (e) {
            debugger;
            if ($scope.gridselectedRowsData === undefined || $scope.gridselectedRowsData === 'undefined')
                return DevExpress.ui.notify({ message: 'عفوا لم يتم التحديد', type: 'error', displayTime: 3000, closeOnClick: true });

            if ($scope.gridselectedRowsData.length === 0)
                return DevExpress.ui.notify({ message: 'عفوا لم يتم التحديد', type: 'error', displayTime: 3000, closeOnClick: true });

            var ObjectData = [];

            const data = obj => obj.reduce((elem, { ID, ItemName, DocTotalValue, Count }) => {
                let group = elem[ID] = elem[ID] || {};

                group[ID] = group[ID] || { DocTotalValue: 0, Count: 0 };
                group[ID].DocTotalValue += DocTotalValue;
                group[ID].Count = group[ID].Count + 1;
                group[ID].ItemName = ItemName;
                group[ID].ID = ID;
                return elem;
            }, []);
            var final = data($scope.gridselectedRowsData);

            var reformattedArray = final.map(obj => {
                var nestedObject = obj[Object.keys(obj)[0]];
                var CurrentAdvance = {
                    ID: nestedObject["ID"],
                    ItemName: nestedObject["ItemName"],
                    DocTotalValue: nestedObject["DocTotalValue"],
                    Count: nestedObject["Count"]
                };
                ObjectData.push(CurrentAdvance);
            });

            $scope.ConfirmselectedRowsList = ObjectData;
            //Show Popup
            $scope.PopupRelaysShow = true;
        }
    };


    //PopupRelays
    $scope.PopupRelays = {
        title: "ترحيل حسم الصندوق",
        showTitle: true,
        bindingOptions: {
            visible: "PopupRelaysShow"
        },
        rtlEnabled: true,
        width: 680,
        height: 460,
        onHiding: function () {

        }
    };


    //gridConfirmDetais
    $scope.gridConfirmDetais = {
        bindingOptions: {
            dataSource: "ConfirmselectedRowsList"
        },
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
            mode: "row"
        },
        allowColumnResizing: true,
        columnResizingMode: "widget",
        allowColumnReordering: true,
        showColumnLines: true,
        rowAlternationEnabled: true,
        columns: [
            {
                dataField: "ItemName",
                caption: "البند",
                alignment: "center"
            },
            {
                dataField: "Count",
                caption: "عددالاقساط",
                alignment: "center"
            },
            {
                dataField: "DocTotalValue",
                caption: "قيمة الاقساط",
                alignment: "center"
            }
        ],
        summary: {
            totalItems: [
                {
                    column: "Count",
                    displayFormat: "العدد :{0}",
                    summaryType: "sum"
                },

                {
                    column: "DocTotalValue",
                    summaryType: "sum",
                    displayFormat: "المجموع :{0}"
                }
            ]
        }
    };


    $scope.btnRelay = {
        text: "تاكيد الترحيل للحسابات العامة",
        icon: "fa fa-money",
        type: 'success',
        onClick: function (e) {
            $scope.PostedJournalPopupShow = true;
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

                        if ($scope.selectedRowKeys === undefined || $scope.selectedRowKeys === 'undefined')
                            return DevExpress.ui.notify({ message: 'عفوا لم يتم تحديد أي سلف للترحيل', type: 'error', displayTime: 3000, closeOnClick: true });

                        if ($scope.selectedRowKeys.length === 0)
                            return DevExpress.ui.notify({ message: 'عفوا لم يتم تحديد أي سلف للترحيل', type: 'error', displayTime: 3000, closeOnClick: true });
                        debugger;
                        RewardItemPremiumRelaysrvc.SaveRewardItemPremiumRelaysToAccount({ ReceiveIds: $scope.selectedRowDetailsKeys.join(','), paramPostedDate: $scope.MDL_PostedDate}).then(function (data) {
                            debugger;
                            if (data.data.status === 200) {
                                DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });

                                //Redferesh Grid
                                RewardItemPremiumRelaysrvc.GetRewardItemPremiumRelays().then(function (data) {
                                    $scope.RewardItemPremiumRelaysList = data.data;
                                });
                                //Close Popup
                                $scope.PopupRelaysShow = false;
                                $scope.PostedJournalPopupShow = false;
                                $scope.selectedRowDetailsKeys = [];
                            }
                            if (data.data.status === 500) {
                                DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                            }
                        });
                    }
                }
            }
        }
    };

}]);
