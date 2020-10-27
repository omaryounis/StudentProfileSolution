app.controller("HousingFurnitureCtrl", ["$scope", 'HousingFurnituresSrvc',
    function ($scope, HousingFurnituresSrvc) {

        $scope.HousingFurnituresList = [];
        $scope.HousingFurnitureId = 0;

        $scope.HousingFurnituresCategoryList = [];
        $scope.FurnitureCategryId = null;

        $scope.FurnitureName = null;

        $scope.FurnitureNotes = null;


        $scope.furnitureCategorySelectBox = {
            bindingOptions: {
                dataSource: "HousingFurnituresCategoryList",
                value: "FurnitureCategryId",
                items: "HousingFurnituresCategoryList"
            },
            placeholder: "--أختر--",
            noDataText: "لا يوجد بيانات",
            Pagenation: true, //Pagenation
            showClearButton: true,
            searchEnabled: true,
            displayExpr: "Text",
            valueExpr: "Value",

            onInitialized: function (e) {
                HousingFurnituresSrvc.GetHousingFurnitureCategories().then(function (data) {
                    $scope.HousingFurnituresCategoryList = data.data;
                });
            },
            onValueChanged: function (e) {
                $scope.FurnitureCategryId = e.value;
            }
        };

        $scope.FurnitureName_TextBox = {
            bindingOptions: {
                value: "FurnitureName"
            },
            disabled: false,
            placeholder: "اسم الاثاث",
            onValueChanged: function (e) {
                $scope.FurnitureName = e.value;
            }
        };

        $scope.FurnitureNotes_TextArea = {
            bindingOptions: {
                value: "FurnitureNotes"
            },
            disabled: false,
            placeholder: "ملاحظات",
            onValueChanged: function (e) {
                $scope.FurnitureNotes = e.value;
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

        $scope.validationMaxLength = {
            validationRules: [
                {
                    type: "maxlength",
                    message: " مطلوب"
                }
            ]
        };

        // Tried this, nothing stops it
        $scope.LimitTyping = function (event) {
            debugger;
            if (event.target.value.length >= 5)
            {
                //event.cancel();
                event.preventDefault();
                return false;
            }
            
        };

        $scope.btnSave = {
            text: 'حفظ',
            type: 'success',
            useSubmitBehavior: true
        };

        //Function
        $scope.SaveHousingFurnitures = function () {
            HousingFurnituresSrvc.SaveHousingFurnitures({ Id: $scope.HousingFurnitureId, CategoryId: $scope.FurnitureCategryId, Name: $scope.FurnitureName, Notes: $scope.FurnitureNotes })
                .then(function (data) {
                    if (data.data.status == 200) {
                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                        //Referesh Grid
                        GetHousingFurnitures()
                        //Clear Data   
                        $scope.HousingFurnitureId = null;
                        $scope.FurnitureCategryId = null;
                        $scope.FurnitureName = null;
                        $scope.FurnitureNotes = null;
                    }
                    if (data.data.status == 500) {
                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    }
                });
        }

        /***************** List **********************/

        //dataGrid
        $scope.gridHousingFurnitures = {
            bindingOptions: {
                dataSource: "HousingFurnituresList"
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
                    caption: "تصنيف الاثاث",
                    dataField: "FurnitureCategories",
                    cssClass: "text-right"
                },
                {
                    caption: "الاسم",
                    dataField: "Name",
                    cssClass: "text-right"
                },
                {
                    caption: "ملاحظات",
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
                                HousingFurnituresSrvc.GetHousingFurnituresById(options.data.Id).then(function (data) {
                                    if (data) {
                                      
                                        $scope.HousingFurnitureId = options.data.Id;
                                        $scope.FurnitureCategryId = options.data.CategoryId.toString();
                                        $scope.FurnitureName = options.data.Name;
                                        $scope.FurnitureNotes = options.data.Notes;
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
                                        HousingFurnituresSrvc.DeleteHousingFurnitures(options.data.Id).then(function (data) {
                                            if (data.data.status == 500) {
                                                DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                            }
                                            if (data.data.status == 200) {
                                                DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                                //Referesh Grid
                                                GetHousingFurnitures();
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
                GetHousingFurnitures();
            }
        }

        function GetHousingFurnitures() {
            HousingFurnituresSrvc.GetHousingFurnitures().then(function (data) {
                $scope.HousingFurnituresList = data.data;
            });
        }

        /***************** PoupHousingFurnitureCategories **********************/
        //Feild
        $scope.HousingFurnituresCategoryListGrid = [];

        $scope.FurnitureCategoryId = null;
        $scope.FurnitureCategoryName = null;
        $scope.FurnitureCategoryPrefix = null;
        $scope.FurnitureCategoryNotes = null;
        $scope.PoupHousingFurnitureCategoriesShow = false;

        //Control
        $scope.txtFurnitureCategoryName = {
            bindingOptions: {
                value: "FurnitureCategoryName"
            },
            disabled: false,
            placeholder: "اسم التصنيف",
            onValueChanged: function (e) {
                $scope.FurnitureCategoryName = e.value;
            }
        };

        $scope.txtFurnitureCategoryPrefix = {
            bindingOptions: {
                value: "FurnitureCategoryPrefix"
            },
            disabled: false,
            placeholder: "الاختصار",
            onValueChanged: function (e) {
                $scope.FurnitureCategoryPrefix = e.value;
            }
        };

        $scope.txtFurnitureCategoryNotes = {
            bindingOptions: {
                value: "FurnitureCategoryNotes"
            },
            disabled: false,
            placeholder: "الملاحظات",
            onValueChanged: function (e) {
                $scope.FurnitureCategoryNotes = e.value;
            }
        };

        $scope.PoupHousingFurnitureCategories = {
            width: 1100,
            scroll: true,
            contentTemplate: 'information',
            showTitle: true,
            dragEnabled: false,
            shadingColor: "rgba(0, 0, 0, 0.69)",
            closeOnOutsideClick: false,
            bindingOptions: {
                visible: "PoupHousingFurnitureCategoriesShow"
            },
            title: "تصنيفات الاثاث",
            rtlEnabled: true
        };


        $scope.btnOpenPoup = {
            icon: 'fa fa-plus',
            type: 'primary',
            onClick: function (e) {
                $scope.PoupHousingFurnitureCategoriesShow = true;
            }
        };

        //////////////////////////////////////////
        $scope.btnSaveFurnitureCategory = {
            //icon: 'fa fa-plus',
            text: "حفظ",
            type: 'success',
            onClick: function (e) {
              
                if ($scope.FurnitureCategoryName == null || $scope.FurnitureCategoryName == "") {
                    DevExpress.ui.notify({ message: "ادخل اسم التصنيف", type: "error", displayTime: 3000, closeOnClick: true });
                    return;
                }
                if ($scope.FurnitureCategoryPrefix == null || $scope.FurnitureCategoryPrefix == "") {
                    DevExpress.ui.notify({ message: "ادخل اختصار التصنيف", type: "error", displayTime: 3000, closeOnClick: true });
                    return;
                }

                HousingFurnituresSrvc.SaveHousingFurnitureCategories({ Id: $scope.FurnitureCategoryId, Name: $scope.FurnitureCategoryName, Prefix: $scope.FurnitureCategoryPrefix, Notes: $scope.FurnitureCategoryNotes })
                    .then(function (data) {
                        if (data.data.status == 200) {
                            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                            //Referesh Grid
                            GetAllHousingFurnitureCategories()
                            //Clear Data   
                            $scope.FurnitureCategoryId = null;
                            $scope.FurnitureCategoryName = null;
                            $scope.FurnitureCategoryPrefix = null;
                            $scope.FurnitureCategoryNotes = null;

                            // Bind FurnitureCategort ddl
                            HousingFurnituresSrvc.GetHousingFurnitureCategories().then(function (data) {
                                $scope.HousingFurnituresCategoryList = data.data;
                            });
                        }
                        if (data.data.status == 500) {
                            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                        }
                    });
            }
        };

        //dataGrid
        $scope.gridHousingFurnituresCategory = {
            bindingOptions: {
                dataSource: "HousingFurnituresCategoryListGrid"
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
                    cssClass: "text-right"
                },
                {
                    caption: "الاختصار",
                    dataField: "Prefix",
                    cssClass: "text-right"
                },
                {
                    caption: "ملاحظات",
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
                                HousingFurnituresSrvc.GetHousingFurnitureCategoriesById(options.data.Id).then(function (data) {
                                    if (data) {
                                      
                                        $scope.FurnitureCategoryId = options.data.Id;
                                        $scope.FurnitureCategoryName = options.data.Name;
                                        $scope.FurnitureCategoryPrefix = options.data.Prefix;
                                        $scope.FurnitureCategoryNotes = options.data.Notes;
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
                                        HousingFurnituresSrvc.DeleteHousingFurnitureCategories(options.data.Id).then(function (data) {
                                            if (data.data.status == 500) {
                                                DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                            }
                                            if (data.data.status == 200) {
                                                DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                                //Referesh Grid
                                                GetAllHousingFurnitureCategories();
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
                GetAllHousingFurnitureCategories();
            }
        }

        function GetAllHousingFurnitureCategories() {
          

            HousingFurnituresSrvc.GetAllHousingFurnitureCategories().then(function (data) {
              
                $scope.HousingFurnituresCategoryListGrid = data.data;
            });
        }

    }]);