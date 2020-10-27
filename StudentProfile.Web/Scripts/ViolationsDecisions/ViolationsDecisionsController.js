app.controller("ViolationsDecisionsCtrl", ["$scope", '$rootScope', '$filter', 'ViolationsDecisionsSrvc', function ($scope, $rootScope, $filter, ViolationsDecisionsSrvc) {

    //Fields
    $scope.ViolationId = 0;
    $scope.Name = null;
    $scope.Notes = null;
    $scope.ViolationsDecisionsList = [];
    //Controls

    $scope.txtName = {
        bindingOptions: {
            value: "Name"
        },
        placeholder: "اسم القرار",
        onValueChanged: function (e) {
            $scope.Name = e.value;
        }
    }

    $scope.txtNotes = {
        bindingOptions: {
            value: "Notes"
        },
        placeholder: "السبب",
        onValueChanged: function (e) {
            $scope.Notes = e.value;
        }
    }

    //validation 
    $scope.validationRequired = {
        validationRules: [
            {
                type: "required",
                message: "الحقل مطلوب"
            }
        ]
    };

    //btn save
    $scope.btnSave = {
        text: 'حفظ',
        type: 'success',
        useSubmitBehavior: true,
    };

    //Function
    $scope.SaveViolationsDecisions = function () {
        ViolationsDecisionsSrvc.SaveViolationsDecisions({ Id: $scope.ViolationId, Name: $scope.Name, Notes: $scope.Notes })
            .then(function (data) {
                if (data.data.status == 200) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    //Referesh Grid
                    GetViolationsDecisions()
                    //Clear Data   
                    $scope.Name = null;
                    $scope.Notes = null;
                }
                if (data.data.status == 500) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                }
            });
    }

    /***************** List **********************/
    //dataGrid
    $scope.gridViolationsDecisions = {
        bindingOptions: {
            dataSource: "ViolationsDecisionsList"
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
        columns: [
            {
                caption: "اسم القرار",
                dataField: "Name",
                cssClass: "text-right"
            },
            {
                caption: "السبب",
                dataField: "Notes",
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
                            ViolationsDecisionsSrvc.GetViolationsDecisionsById(options.data.Id).then(function (data) {
                                if (data) {
                                    debugger;
                                    $scope.ViolationId = options.data.Id;
                                    $scope.Name = options.data.Name;
                                    $scope.Notes = options.data.Notes;
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
                        icon: "fa fa-trash-o",
                        //text: "حذف",
                        type: "danger",
                        hint: "حذف",
                        elementAttr: { "class": "btn btn-danger btn-sm" },
                        onClick: function (e) {
                            var result = DevExpress.ui.dialog.confirm("<i>هل انت متاكد؟</i>", "تاكيد الحذف");
                            result.done(function (dialogResult) {
                                if (dialogResult) {
                                    ViolationsDecisionsSrvc.DeleteViolationsDecisions(options.data.Id).then(function (data) {
                                        if (data.data.status == 500) {
                                            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                        }
                                        if (data.data.status == 200) {
                                            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                            //Referesh Grid
                                            GetViolationsDecisions();
                                        }
                                    });
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
            $scope.GridRowData = info.selectedRowKeys;
        },
        onInitialized: function (e) {
            GetViolationsDecisions();
        }
    }

    function GetViolationsDecisions() {
        ViolationsDecisionsSrvc.GetViolationsDecisions().then(function (data) {
            $scope.ViolationsDecisionsList = data.data;
        });
    }

}]);
