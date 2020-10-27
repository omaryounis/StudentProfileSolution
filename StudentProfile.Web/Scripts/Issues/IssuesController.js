app.controller("IssuesCtrl", ["$scope", '$rootScope', '$filter', 'IssuesSrvc', function ($scope, $rootScope, $filter, IssuesSrvc) {

    //Fields
    $scope.IssuesId = 0;
    $scope.Name = null;
    $scope.Notes = null;
    Issues = [];

    $scope.IssuesCategoryId = null;
    $scope.IssuesCategoryList = [];

    //Controls
    $scope.IssuesCategorySelectBox = {
        bindingOptions: {
            dataSource: "IssuesCategoryList",
            value: "IssuesCategoryId",
            items: "IssuesCategoryList"
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
            //Get GetIssuesCategories
            IssuesSrvc.GetIssuesCategories().then(function (data) {
                $scope.IssuesCategoryList = data.data;
            });
        },
        onValueChanged: function (e) {
            $scope.IssuesCategoryId = e.value;
        }
    };

    $scope.txtName = {
        bindingOptions: {
            value: "Name"
        },
        placeholder: "اسم المخالفة",
        onValueChanged: function (e) {
            $scope.Name = e.value;
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
    $scope.SaveIssues = function () {
        IssuesSrvc.SaveIssues({ Id: $scope.IssuesId, IssueDescription: $scope.Name, CategoryId: $scope.IssuesCategoryId })
            .then(function (data) {
                if (data.data.status == 200) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    //Referesh Grid
                    GetIssues()
                    //Clear Data   
                    $scope.Name = null;
                    $scope.IssuesCategoryId = null;
                }
                if (data.data.status == 500) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                }
            });
    }

    /***************** List **********************/
    //dataGrid
    $scope.gridIssues = {
        bindingOptions: {
            dataSource: "Issues"
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
                caption: "التصنيف",
                dataField: "CategoryName",
                cssClass: "text-right",
                groupIndex: 0
            },
            {
                caption: "المخالفة",
                dataField: "Name",
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
                            IssuesSrvc.GetIssuesById(options.data.Id).then(function (data) {
                                if (data) {
                                    debugger;
                                    $scope.IssuesId = options.data.Id;
                                    $scope.Name = options.data.Name;
                                    $scope.IssuesCategoryId = options.data.CategoryId.toString();
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
                                    IssuesSrvc.DeleteIssues(options.data.Id).then(function (data) {
                                        if (data.data.status == 500) {
                                            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                        }
                                        if (data.data.status == 200) {
                                            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                            //Referesh Grid
                                            GetIssues();
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
            GetIssues();
        }
    }

    function GetIssues() {
        IssuesSrvc.GetIssues().then(function (data) {
            $scope.Issues = data.data;
        });
    }



    /***************** IssuesCategory **********************/
    //Feild
    $scope.IssuessCategoryListGrid = [];

    $scope.IssuesCategoryId = null;
    $scope.IssuesCategoryName = null;
    $scope.PoupIssuesCategoriesShow = false;

    //Control
    $scope.txtIssuesCategoryName = {
        bindingOptions: {
            value: "IssuesCategoryName"
        },
        disabled: false,
        placeholder: "اسم التصنيف",
        onValueChanged: function (e) {
            $scope.IssuesCategoryName = e.value;
        }
    };

    $scope.PoupIssuesCategories = {
        width: 1100,
        scroll: true,
        contentTemplate: 'information',
        showTitle: true,
        dragEnabled: false,
        shadingColor: "rgba(0, 0, 0, 0.69)",
        closeOnOutsideClick: false,
        bindingOptions: {
            visible: "PoupIssuesCategoriesShow"
        },
        title: "تصنيفات المخالفات",
        rtlEnabled: true
    };


    $scope.btnOpenPoup = {
        icon: 'fa fa-plus',
        type: 'primary',
        onClick: function (e) {
            $scope.PoupIssuesCategoriesShow = true;
        }
    };


    $scope.btnSaveIssuesCategory = {
        //icon: 'fa fa-plus',
        text: "حفظ",
        type: 'success',
        onClick: function (e) {

            if ($scope.IssuesCategoryName == null || $scope.IssuesCategoryName == "") {
                DevExpress.ui.notify({ message: "ادخل اسم التصنيف", type: "error", displayTime: 3000, closeOnClick: true });
                return;
            }

            IssuesSrvc.SaveIssuesCategories({ Id: $scope.IssuesCategoryId, Name: $scope.IssuesCategoryName })
                .then(function (data) {
                    if (data.data.status == 200) {
                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                        //Referesh Grid
                        GetAllIssuesCategories();
                        //GetIssuesCategories
                        IssuesSrvc.GetIssuesCategories().then(function (data) {
                            $scope.IssuesCategoryList = data.data;
                        });
                        //Clear Data   
                        $scope.IssuesCategoryId = null;
                        $scope.IssuesCategoryName = null;
                    }
                    if (data.data.status == 500) {
                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    }
                });
        }
    };

    //dataGrid
    $scope.gridIssuessCategory = {
        bindingOptions: {
            dataSource: "IssuessCategoryListGrid"
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
                caption: "الاسم",
                dataField: "Name",
                cssClass: "text-right",
                width: "800px"
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
                            IssuesSrvc.GetIssuesCategoriesById(options.data.Id).then(function (data) {
                                if (data) {

                                    $scope.IssuesCategoryId = options.data.Id;
                                    $scope.IssuesCategoryName = options.data.Name;
                                }
                            });
                        }
                    }).appendTo(container);
                }
            }
            ,
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
                                    IssuesSrvc.DeleteIssuesCategories(options.data.Id).then(function (data) {
                                        if (data.data.status == 500) {
                                            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                        }
                                        if (data.data.status == 200) {
                                            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                            //Referesh Grid
                                            GetAllIssuesCategories();
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
            GetAllIssuesCategories();
        }
    }

    function GetAllIssuesCategories() {
        IssuesSrvc.GetAllIssuesCategories().then(function (data) {
            $scope.IssuessCategoryListGrid = data.data;
        });
    }

}]);
