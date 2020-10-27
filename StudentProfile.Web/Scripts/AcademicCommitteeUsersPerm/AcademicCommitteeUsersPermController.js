app.controller("AcademicCommitteeUsersPermCtrl", ["$scope", '$location', 'AcademicCommitteeUsersPermSrvc', function ($scope, $location, AcademicCommitteeUsersPermSrvc) {

    //Filed
    $scope.ID = null;
    $scope.UsersList = [];
    $scope.UserID = null;
    $scope.AcademicCommitteeUsersPermList = [];

    $scope.FacultionList = [];
    $scope.FacultionID = null;
    $scope.FacultionIds = null;
    //Control
    $scope.UsersSelectBox = {
        bindingOptions: {
            dataSource: "UsersList",
            value: "UserID",
            items: "UsersList"
        },

        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        pagingEnabled: true, //Pagenation
        showClearButton: true,
        searchEnabled: true,
        displayExpr: "Text",
        valueExpr: "Value",
        disabled: false,

        onInitialized: function (e) {
            //Get Users
            AcademicCommitteeUsersPermSrvc.GetUsers(e.value).then(function (data) {
                $scope.UsersList = data.data;
            });
        },
        onValueChanged: function (e) {
            $scope.UserID = e.value;
        }
    };

    $scope.FacultionSelectBox = {
        bindingOptions: {
            value: "FacultionID",
            items: "FacultionList"
        },

        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        displayExpr: "Text",
        valueExpr: "Value",
        paginate: true, //Pagenation
        showBorders: true,
        searchEnabled: true,
        showSelectionControls: true,
        maxDisplayedTags: 2,
        showMultiTagOnly: false,
        onInitialized: function (e) {
            debugger;
            AcademicCommitteeUsersPermSrvc.GetFaculties().then(function (data) {
                $scope.FacultionList = data.data;
            });
        },
        onValueChanged: function (e) {
            debugger;
            if (e != undefined && e != null && e != '') {
                if (e.value != null) {
                    if (e.value.length != 0) {
                        $scope.FacultionIds = e.value.join(',');

                        $scope.DegreeList = '';
                        AcademicCommitteeUsersPermSrvc.GetDegrees($scope.FacultionIds).then(function (data) {
                            debugger;
                            $scope.DegreeList = data.data;
                        });
                    }
                }
                
            }
        }
    };

    $scope.DegreeSelectBox = {
        bindingOptions: {
            value: "DegreeID",
            items: "DegreeList"
        },
        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        displayExpr: "Text",
        valueExpr: "Value",
        paginate: true, //Pagenation
        //selectAllMode: "page",
        showBorders: true,
        searchEnabled: true,
        showSelectionControls: true,
        maxDisplayedTags: 2,
        showMultiTagOnly: false,
        selectAllMode: "allPages",
        onValueChanged: function (e) {
            $scope.DegreeIds = e.value.join(',');
        },
        onInitialized: function (e) {
            debugger;
            if ($scope.FacultionIds != null) {
                AcademicCommitteeUsersPermSrvc.GetDegrees($scope.FacultionIds).then(function (data) {
                    debugger;
                    $scope.DegreeList = data.data;
                });
            }
        },
        onClick: function (e) {
            debugger;
            if ($scope.FacultionIds != null) {
                AcademicCommitteeUsersPermSrvc.GetDegrees($scope.FacultionIds).then(function (data) {
                    debugger;
                    $scope.DegreeList = data.data;
                });
            }
        },
        onClosed: function (e) {
            debugger;
            $scope.DegreeIds = e.model.DegreeID.join(',');
        }
    };
    //validation 
    $scope.validationRequired = {
        validationRules: [
            {
                type: "required",
                message: "الحقل مطلوب"
            }
        ]
    };

    //btn Save
    $scope.btnSave = {
        text: 'حفظ',
        type: 'success',
        useSubmitBehavior: true
    };
    $scope.MDL_IsMoneyExchange = false;
    $scope.IsMoneyExchange = {
        bindingOptions: {
            value: "MDL_IsMoneyExchange"
        },
        onValueChanged: function (e) {
            $scope.MDL_IsMoneyExchange = e.value;
        }
    };




  



    //Function
    $scope.SaveAcademicCommitteeUsersPerm = function () {
        debugger;
        IsMoneyExchange = $scope.IsMoneyExchange;
        AcademicCommitteeUsersPermSrvc.SaveAcademicCommitteeUsersPerm({ UserID: Number($scope.UserID), FacultyIds: $scope.FacultionIds, DegreeIds: $scope.DegreeIds, IsMoneyExchange: $scope.MDL_IsMoneyExchange }).then(function (data) {
            if (data.data.status == 500) {
                DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
            }
            if (data.data.status == 200) {
                DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                $scope.ID = null;
                $scope.UserID = null;
                $scope.PhaseID = null;
                $scope.FacultionID = null;
                $scope.FacultionIds = null;
                $scope.DegreeIds = null;
                $scope.DegreeID = null;
                $scope.MDL_IsMoneyExchange = false;
                //GetAcademicCommitteeUsersPerm
                AcademicCommitteeUsersPermSrvc.GetAcademicCommitteeUsersPerm().then(function (data) {
                    $scope.AcademicCommitteeUsersPermList = data.data;
                });
            }
        });

    };
    $scope.ConfirmPopup = {
        bindingOptions: {
            visible: "ConfirmPopupVisible"
        },
        shadingColor: "rgba(0, 0, 0, 0.69)",
        closeOnOutsideClick: true,
        height: 200,
        width: 600,
        showTitle: false,
        dragEnabled: false
    };

    //dataGrid
    $scope.gridAcademicCommitteeUsersPerm  = {
        bindingOptions: {
            dataSource: "AcademicCommitteeUsersPermList"
        },
        grouping: {
            autoExpandAll: false
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
                caption: "اسم المسؤول",
                dataField: "UserName",
                cssClass: "text-right",
                groupIndex: 0
            },
            {
                caption: "الكلية",
                dataField: "Faculty",
                cssClass: "text-right",
                groupIndex: 1
            },
            {
                caption: "",
                dataField: "Degree",
                cssClass: "text-right"
            },
            {
                caption: "مسؤول الصرافة",
                dataField: "IsMoneyExchange",
                cssClass: "text-right"
            }, 
            {
                caption: "تنشيط",
                width: 100,
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    if (options.data.IsActive == true) {
                        $("<div />").dxButton({
                            icon: "fa fa-unlock-alt",
                            //text: "ايقاف",
                            type: "danger",
                            hint: "ايقاف",
                            elementAttr: { "class": "btn btn-danger" },

                            onClick: function (e) {
                                AcademicCommitteeUsersPermSrvc.ActiveAcademicCommitteeUsersPerm(options.data.ID).then(function (data) {
                                    if (data.data.status == 500) {
                                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                    }
                                    if (data.data.status == 200) {
                                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                        //Get Phases Users
                                        AcademicCommitteeUsersPermSrvc.GetAcademicCommitteeUsersPerm().then(function (data) {
                                            $scope.AcademicCommitteeUsersPermList = data.data;
                                        });
                                    }
                                });
                            }
                        }).appendTo(container);
                    }
                    else {
                        $("<div />").dxButton({
                            icon: "fa fa-unlock",
                            //text: "تنشيط",
                            type: "success",
                            hint: "تنشيط",
                            elementAttr: { "class": "btn btn-success" },
                            onClick: function (e) {
                                AcademicCommitteeUsersPermSrvc.ActiveAcademicCommitteeUsersPerm(options.data.ID).then(function (data) {
                                    if (data.data.status == 500) {
                                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                    }
                                    if (data.data.status == 200) {
                                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                        //Get Phases Users
                                        AcademicCommitteeUsersPermSrvc.GetAcademicCommitteeUsersPerm().then(function (data) {
                                            $scope.AcademicCommitteeUsersPermList = data.data;
                                        });
                                    }
                                });
                            }
                        }).appendTo(container);
                    }
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
                                    AcademicCommitteeUsersPermSrvc.DeleteAcademicCommitteeUsersPerm(options.data.ID).then(function (data) {
                                        if (data.data.status == 500) {
                                            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                        }
                                        if (data.data.status == 200) {
                                            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                            //Get Phases Users
                                            AcademicCommitteeUsersPermSrvc.GetAcademicCommitteeUsersPerm().then(function (data) {
                                                $scope.AcademicCommitteeUsersPermList = data.data;
                                            });
                                        } else {
                                            debugger;
                                            //$('#ConfirmPopup').dxPopup('instance').option('visible', true);
                                            var errorMessage = "لا يجوز الإلغاء علي هذه المرحلة لإن مندوب الصرف هذا مسؤول علي شيكات";
                                            var ChecksNumber = "";
                                            var resultErrorMessage = "";
                                            for (i = 0; i < data.data.length; i++) {
                                                //errorMessage = "لا يجوز الإلغاء علي هذه المرحلة لإن مندوب الصرف هذا مسؤول علي شيكات"
                                                if (ChecksNumber === "") {
                                                    ChecksNumber = data.data[i].CheckNumber + " " + "للمسير " + " " + data.data[i].PayrollNumber;
                                                } else {
                                                    ChecksNumber = CheckNumber + " , " + data.data[i].CheckNumber + " " + "للمسير " + " " + data.data[i].PayrollNumber;
                                                }
                                                    
                                            }
                                            resultErrorMessage = errorMessage + " " + ChecksNumber;
                                           var x= DevExpress.ui.dialog.alert("<b style='color:red'>"+resultErrorMessage+"</b>", "خطأ لا يمكن الإلغاء", true);
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
            AcademicCommitteeUsersPermSrvc.GetAcademicCommitteeUsersPerm().then(function (data) {
                $scope.AcademicCommitteeUsersPermList = data.data;
            });
        }
    };

}]);


