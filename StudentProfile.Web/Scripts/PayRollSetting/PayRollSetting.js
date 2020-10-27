app.controller("PayRollSettingController", ["$scope", "$http", "PayRollSettingSrvc", function ($scope, $http, PayRollSettingSrvc) {

    $scope.PhaseNameValidationRules = {
        validationRules: [{
            type: "required",
            message: "يرجى ادخال اسم المرحله"
        }]
    };


    $scope.IsFinancialApproveCheck = false;
    $scope.IssuingExchangeOrderCheck = false;
    $scope.IssuingPaymentOrderCheck = false;

    $scope.PayrollPhaseId = null;
    $scope.PayrollPhasesList = [];

    $scope.IsActive = false;

    $scope.SelectedPayRollPhasesId = null;
    $scope.selctedUsrID = null;
    $scope.SelectPayRollPhasesUser = null;
    $scope.PayRollPhases = [];
    $scope.DashBoard_Users = [];
    $scope.PayRollPhasesUsers = [];
    $scope.visiblePopupForPayRollPhases = false;
    $scope.maxOrder = null;
    $scope.visiblePopupForPhaseEditing = false; 
    $scope.IsFinancialApproveEditing = false;
    $scope.IssuingExchangeOrderEditing = false;
    $scope.IssuingPaymentOrderEditing = false;

    $scope.PayRollPhase = { ID: 0, PhaseName: "", PhaseOrder: "", IsFinancialApprove: false, IssuingExchangeOrder: false, IssuingPaymentOrder:false };
    $scope.onClick = function (e) {
        $scope.visiblePopupForPhaseEditing = true;
        $scope.PhaseUserID = e.value;
        PayRollSettingSrvc.GetPhaseName(e.value).then(function (data) {
            debugger;
            $scope.SelectedPayRollPhasesEDId = data.data;
        });
        PayRollSettingSrvc.GetUserName(e.value).then(function (data) {
            $scope.selctedUsrEDID = data.data;
        });
    };
    $scope.txtPhaseOrderreadOnly = true;
    $scope.txtPhaseOrderreadOnly = true;

    $scope.txtPhaseName = {
        placeholder: "ادخل اسم المرحله",
        bindingOptions: {
            value: "PayRollPhase.PhaseName",
            readOnly: "txtPhaseNamereadOnly"
        },
        onInitialized: function (e) {
            $scope.txtPhaseNamereadOnly = false;
        }
    };


    $scope.txtPhaseOrder = {
        bindingOptions: {
            value: "PayRollPhase.PhaseOrder",
            readOnly: "txtPhaseOrderreadOnly"
        },
        onInitialized: function (e) {
            $scope.txtPhaseOrderreadOnly = true;
        }
    };


    $scope.SelectedPayRollPhasesEDId = null;
    $scope.selctedUsrEDID = null;
    $scope.PhaseUserID = null;

    $scope.PhaseEditingPopUp = {
        width: 480,
        height: 400,
        contentTemplate: "PhaseEditingContent",
        showTitle: true,
        title: "تعديل ",
        dragEnabled: false,
        closeOnOutsideClick: true,
        bindingOptions: {
            visible: "visiblePopupForPhaseEditing"
        }
    };
    $scope.selectBoxPayRollPhasesEditing = {
        bindingOptions: {
            items: "PayRollPhases",
            value: "SelectedPayRollPhasesEDId"
        },
        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        pagingEnabled: true, //Pagenation
        showClearButton: true,
        searchEnabled: true,
        disabled: false,
        displayExpr: "PhaseName",
        valueExpr: "ID",
        onValueChanged: function (e) {
        }
    }; 
    $scope.selectBoxUserEditing = {
        bindingOptions: {
            items: "DashBoard_Users",
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
    

    $scope.btnSaveEditing = {
        text: 'حفظ',
        visible: true,
        type: 'success',
        useSubmitBehavior: true,
        onClick: function (e) {
            PayRollSettingSrvc.UpdateRow($scope.SelectedPayRollPhasesEDId, $scope.selctedUsrEDID, $scope.PhaseUserID).then(function (data) {
                if (data.data !== "")
                    DevExpress.ui.notify(data.data, "error", 5000);
                else {
                    DevExpress.ui.notify("تم التعديل بنجاح", "success", 5000);
                    $scope.visiblePopupForPhaseEditing = false;
                    $scope.IsFinancialApproveEditing = false;
                    $scope.IssuingExchangeOrderEditing = false;
                    $scope.IssuingPaymentOrderEditing = false;
                    PayRollSettingSrvc.GetPayRollPhasesUsers().then(function (data) {
                        $scope.PayRollPhasesUsers = data.data;
                    });
                }
            }
            );
            debugger;
            
        }       
    };
    $scope.checkBoxValue = false;
    
    PayRollSettingSrvc.GetAllPayRollPhases().then(function (data) {
        $scope.PayRollPhases = data.data;
    });
    PayRollSettingSrvc.GetAllUsers().then(function (data) {
        $scope.DashBoard_Users = data.data;
    });
    PayRollSettingSrvc.GetPayRollPhasesUsers().then(function (data) {
        $scope.PayRollPhasesUsers = data.data;
    });

    //dataGrid
    $scope.gridPayrollPhases = {
        bindingOptions: {
            dataSource: "PayrollPhasesList"
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
                caption: "اسم المرحلة",
                dataField: "PhaseName",
                cssClass: "text-right"
            },
            {
                caption: "الترتيب",
                dataField: "PhaseOrder",
                cssClass: "text-right"
            },
            {
                caption: "إعتماد خصم القروض",
                dataField: "IsFinancialApprove",
                cssClass: "text-right"
            },
            {
                caption: "إصدار أمر الصرف",
                dataField: "IssuingExchangeOrder",
                cssClass: "text-right"
            },
            {
                caption: "إصدار أمر الدفع",
                dataField: "IssuingPaymentOrder",
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
                            PayRollSettingSrvc.GetPayrollPhasesId(options.data.ID).then(function (data) {
                                if (data) {
                                    debugger;
                                    $scope.PayRollPhase.ID = options.data.ID;
                                    $scope.PayrollPhaseId = options.data.ID;
                                    $scope.PayRollPhase.PhaseName = options.data.PhaseName;
                                    $scope.PayRollPhase.PhaseOrder = options.data.PhaseOrder;
                                    $scope.PayRollPhase.IsFinancialApprove = options.data.IsFinancialApprove;
                                    $scope.PayRollPhase.IssuingExchangeOrder = options.data.IssuingExchangeOrder;
                                    $scope.PayRollPhase.IssuingPaymentOrder = options.data.IssuingPaymentOrder;
                                    $scope.txtPhaseOrderreadOnly = false;
                                    $scope.txtPhaseNamereadOnly = true;

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
            PayRollSettingSrvc.GetAllPayRollPhases().then(function (data) {
                if (data) {
                    $scope.PayrollPhasesList = data.data;
                }
            });
        }
    }

    $scope.selectBoxPayRollPhases = {
        bindingOptions: {
            items: "PayRollPhases",
            value: "SelectedPayRollPhasesId"
        },
        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        pagingEnabled: true, //Pagenation
        showClearButton: true,
        searchEnabled: true,
        disabled: false,
        displayExpr: "PhaseName",
        valueExpr: "ID",
        onValueChanged: function (e) {
        }
    };  
        $scope.PayRollPhasesPopUp = {
            width: 1200,
            height: 600,
            contentTemplate: "PayRollPhasesContent",
            showTitle: true,
            title: "اضافة مرحله جديده ",
            dragEnabled: false,
            closeOnOutsideClick: false,
            bindingOptions: {
                visible: "visiblePopupForPayRollPhases"
            }
        };

        $scope.showPayRollPhasesPopUp = function (data) {
            $scope.PayRollPhase.PhaseName = "";
            //$scope.PayRollPhase.IsFinancialApprove = false;
            PayRollSettingSrvc.GetmaxOrder().then(function (data) {
                $scope.maxOrder = data.data;
                $scope.PayRollPhase.PhaseOrder = $scope.maxOrder;  
            });
    
            $scope.visiblePopupForPayRollPhases = true;

            
        };
        $scope.selectBoxUser = {
            bindingOptions: {
                items: "DashBoard_Users",
                value: "selctedUsrID"
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
            if ($scope.checkBoxValue === true) {
                $scope.IsActive = false;
                $scope.checkBoxValue = false;
               // DevExpress.ui.notify("تم الايقاف", "success", 5000);
            }
            
            else {
                $scope.IsActive = true;
                $scope.checkBoxValue = true;
              //  DevExpress.ui.notify("تم التفعيل", "success", 5000);
            }
        };



        $scope.btnSaveOptions = {
            text: 'حفظ',
            visible: true,
            type: 'success',
            useSubmitBehavior: true,
            onClick: function (e) {
                PayRollSettingSrvc.addPayRollPhasesUsers($scope.SelectedPayRollPhasesId, $scope.selctedUsrID, $scope.IsActive).then(function (data) {
                    if (data.data !== "") {
                        if (data.data === "يرجى اختيار مرحله")
                            DevExpress.ui.notify(data.data, "warning", 5000);
                        else if (data.data === "يرجى اختيار مسؤول")
                            DevExpress.ui.notify(data.data, "warning", 5000);
                        else {
                            DevExpress.ui.notify(data.data, "error", 5000);
                            $scope.visiblePopupButtons = true;
                        }
                    }
                    else {
                        DevExpress.ui.notify("تم الحفظ بنجاح", "success", 5000);
                        $scope.visiblePopupButtons = false;
                        //$scope.IsFinancialApprove = false;
                        PayRollSettingSrvc.GetPayRollPhasesUsers().then(function (data) {
                            $scope.PayRollPhasesUsers = data.data;
                        });
                    }
                }
                );
                debugger;                
            }
        };

        $scope.btnSave = {
            text: 'حفظ',
            visible: true,
            type: 'success',
            useSubmitBehavior: true,

            onClick: function (e) {
                debugger;
                    PayRollSettingSrvc.addNewPayRollPhase($scope.PayRollPhase).then(function (data) {
                        if (data.data !== "")
                            DevExpress.ui.notify(data.data, "error", 5000);
                        else {
                            $scope.PayRollPhase.PhaseName = "";

                            PayRollSettingSrvc.GetmaxOrder().then(function (data) {
                                $scope.maxOrder = data.data;
                                $scope.PayRollPhase.PhaseOrder = $scope.maxOrder;  
                            });
                            PayRollSettingSrvc.GetAllPayRollPhases().then(function (data) {
                                $scope.PayRollPhases = data.data;
                            });
                           
                            //PayRollSettingSrvc.GetPayRollPhasesUsers().then(function (data) {
                            //    if (data) {
                            //        $scope.PayrollPhasesList = data.data;
                            //    }
                            //});

                            PayRollSettingSrvc.GetAllPayRollPhases().then(function (data) {                                
                                    $scope.PayrollPhasesList = data.data;                                
                            });
                            $scope.txtPhaseOrderreadOnly = true;
                            $scope.txtPhaseNamereadOnly = false;
                            DevExpress.ui.notify("تم الحفظ بنجاح", "success", 5000);
                            $scope.PayRollPhase.ID = 0;
                            $scope.PayRollPhase.IsFinancialApprove = false;
                            $scope.PayRollPhase.IssuingExchangeOrder = false;
                            $scope.PayRollPhase.IssuingPaymentOrder = false;

                        }
                    });
                

            }
        };
    $scope.grouping = {
            autoExpandAll: true
        };

    $scope.PayRollPhasesUserGrid = {      
        bindingOptions: {
            dataSource: "PayRollPhasesUsers",
            grouping: "grouping",
            value: "SelectPayRollPhasesUser",
            
        },
        reshapeOnPush: true,
        keyExpr: "ID",
        onValueChanged: function (e) {
        },
        allowColumnReordering: true,
        showBorders: true,

        searchPanel: {
            visible: true
        },

        paging: {
            pageSize: 10
        },
        groupPanel: {
            visible: true
        },
        columns: [
            {
                dataField: "ID",
                visible: false
            },
            {
                dataField: "PhaseOrder",
                caption: "ترتيب المرحلة",
                alignment: "center",
                groupIndex: 0
            },
            {
                dataField: "PhaseName",
                caption: "اسم المرحلة",
                alignment: "center"
               
            },
            {
                dataField: "Name",
                caption: "إسم المسؤول",
                alignment: "center"
            },            
            {
                dataField: "IsActive",
                alignment: "center",
                caption: "تفعيل و إيقاف",
                cssClass: "text-center",
                width: 220,
                cellTemplate: function (container, options) {
                    var div = $("<div />").appendTo(container);
                    div.dxSwitch({
                        value: options.value,
                        width: 50,
                        onOptionChanged: function (e) {
                            debugger;
                            PayRollSettingSrvc.IsActiveEditing(options.data.ID, e.value)
                                .then(function (data) {
                                    debugger;
                                    if (data.data !== "") { swal("حدث خطأ", data.data, "error"); }
                                    else {                                       
                                        swal("Done!", e.value === true ? "تم تفعيل الموظف" : "تم إيقاف الموظف", "success");
                                  PayRollSettingSrvc.GetPayRollPhasesUsers().then(function (data) {
                                            $scope.PayRollPhasesUsers = data.data;
                                        });
                                    }
                                });
                        }
                    });
                }
            },
            {
                dataField:"ID",
                alignment: "center",
                caption: "تعديل",
                cssClass: "text-center ",
                width: 220,
                cellTemplate: function (cellElement, cellInfo) {
                    var $button = $("<button>")
                        .text("تعديل")
                        .addClass("btn btn-info")
                        .on("click", $.proxy($scope.onClick, this, cellInfo));

                    cellElement.append($button);
                }
            }
        ],
        rtlEnabled: true,
        editing: {
            mode: "row"
        }
    };

    $scope.FinancialApprovechckBox = {
        text: "اعتماد خصم القروض",
        bindingOptions: {
            value: "PayRollPhase.IsFinancialApprove"
        }
    };
    $scope.IssuingExchangeOrderchckBox = {
        text: "إصدار أمر الصرف",
        bindingOptions: {
            value: "PayRollPhase.IssuingExchangeOrder"
        }
    }; $scope.IssuingPaymentOrderchckBox = {
        text: "إصدار أمر الدفع",
        bindingOptions: {
            value: "PayRollPhase.IssuingPaymentOrder"
        }
    };



    $scope.checkBoxOptions = {
        text: "Expand All Groups",
        bindingOptions: {
            value: "grouping.autoExpandAll"
        }
    };
    }]);

    

