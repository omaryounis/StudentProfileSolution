app.controller("AssetLocationsCtrl",
    function (AssetService, $scope, $cookies, $http, $location, $timeout) {


        $scope.Branchies = [];
        $scope.Departments = [];
        $scope.DepartmentFloors = [];
        $scope.Floors = [];
        $scope.MDL_Site = "";
        $scope.MDL_NewBranchText = "";
        $scope.MDL_DepartmentText = "";
        $scope.MDL_FloorsText = "";
        $scope.BranchiesForNew = [];
        $scope.BranchiesNotInAssetLocation = [];
        $scope.BranchiesInAssetLocation = [];
        $scope.LocationTreeArrey = [];
        $scope.MDL_NewDepartmentName = "";
        $scope.MDL_Branch = null;
        $scope.MDL_NewBranch = null;
        $scope.MDL_Department = null;

        $scope.MDL_Floors = null;
        $scope.MDL_Site = "";
        $scope.MDL_SiteNumber = "";

        $scope.MDL_BranchText = "";
        $scope.NewBranchIdSelectBoxDisable = true;
        $scope.ResetNewBranch = "اختر";

        $scope.AssetLocationClass = [];
        $scope.ClassID = null;
        $scope.LocationName = "";
        $scope.COMName = "";

        $scope.Height = 1;
        $scope.Width = 1;
        $scope.Depth = 1;
        $scope.BedsPerRoom = null;
        $scope.txtBedsPerRoomdisabled = false;

        $scope.UnitNumber = null;
        $scope.IsFamilial = false;
        $scope.CheckIsFamilialText = "فردي";

        $scope.PopUpHosuingFurnitureShow = false;
        $scope.FurnitureCategiesList = [];
        $scope.FurnitureCategoryId = null;

        $scope.HosuingFurnitureList = [];
        $scope.HosuingFurnitureId = null;

        $scope.FurnitureCount = null;

        $scope.LocationId = null;

        $scope.MDL_NewFloorNumber = null;

        $scope.MDL_AdministrativeUnitt = {
            bindingOptions: {
                items: "Branchies",
                value: "MDL_Branch",
                text: "MDL_BranchText"
            },
            displayExpr: "LocationName",
            valueExpr: "ID",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            itemTemplate: function (data) {
                return data.LocationName + " - " + data.COMName;
            }  ,
            noDataText: "لا يوجد بيانات",
            onItemClick: function (e) {
                $scope.Departments = [];
                $scope.Floors = [];
                $scope.GetDepartments();
                $scope.MDL_Department = null;
                $scope.MDL_Floors = null;
            }
        };

        $scope.DepartmentBranchSelectBox = {
            bindingOptions: {
                items: "Branchies",
                value: "MDL_DepartmentBranch",
                disabled: "DepartmentBranchdisabled"
            },
            displayExpr: "LocationName",
            valueExpr: "ID",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            onItemClick: function (e) {
                $scope.GetDepartments();
            }
        };

        $scope.FloorBranchSelectBox = {
            bindingOptions: {
                items: "Branchies",
                value: "MDL_FloorBranch"
            },
            displayExpr: "LocationName",
            valueExpr: "ID",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            onItemClick: function (e) {
                $scope.GetDepartmentsInFloorsPopUp();
            }
        };

        $scope.FloorDepartmentSelectBox = {
            bindingOptions: {
                items: "DepartmentFloors",
                value: "MDL_FloorDepartment"
            },
            displayExpr: "LocationName",
            valueExpr: "ID",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات"
        };


        $scope.NewBranchIdSelectBox = {
            bindingOptions: {
                items: "BranchiesForNew",
                value: "MDL_NewBranch",
                disabled: "NewBranchIdSelectBoxDisable",
                text: "MDL_NewBranchText"
            },
            displayExpr: "COMName",
            valueExpr: "COMID",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            allowNull: true,
            noDataText: "لا يوجد بيانات",
            onItemClick: function (e) {
                $scope.SetDDLtEXT();
            }
        };

        $scope.DepartmentIddd = {
            bindingOptions: {
                items: "Departments",
                value: "MDL_Department",
                text: "MDL_DepartmentText"
            },
            displayExpr: "LocationName",
            valueExpr: "ID",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            onItemClick: function (e) {
                $scope.Floors = [];
                $scope.GetFloors();
                $scope.MDL_Floors = null;
            }
        };

        $scope.FloorIddd = {
            bindingOptions: {
                items: "Floors",
                value: "MDL_Floors",
                text: "MDL_FloorsText"
            },
            displayExpr: "LocationName",
            valueExpr: "ID",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات"
        };

        // Asset Location Class
        $scope.AssetLocationClassSelectBox = {
            bindingOptions: {
                dataSource: "AssetLocationClass",
                value: "ClassID", //$scope.AssetLocationsID
                items: "AssetLocationClass" //$scope.AssetLocations
            },
            placeholder: "أختر",
            noDataText: "لا يوجد بيانات",
            showClearButton: true,
            searchEnabled: true,
            pagingEnabled: true,
            displayExpr: "Text",
            valueExpr: "Value",
            disabled: false,

            onInitialized: function (e) {
                $http({
                    method: "Get",
                    url: "/Asset/GetAssetLocationClass"
                }).then(function (data) {
                    $scope.AssetLocationClass = data.data;
                });
            },
            onValueChanged: function (e) {
                $scope.ClassID = e.value;
            }
        };

        /////////////////////Add Asset Location Class//////////////////////////
        $scope.txtLocationName = {
            LocationName: {
                bindingOptions: {
                    value: "LocationName" //$scope.LocationName
                },
                placeholder: "اسم التصنيف"
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

        //Save Asset Location Class
        $scope.SaveAssetLocationClass = function () {
            if ($scope.LocationName === "") {
                DevExpress.ui.notify("ادخل اسم التصنيف", "Error", 2000);
                return false;
            }
            $http({
                method: "Post",
                url: "/Asset/SaveAssetLocationClass",
                data: { Name: $scope.LocationName }
            }).then(function (data) {
                DevExpress.ui.notify({
                    message: data.data.Message,
                    type: data.data.Type,
                    displayTime: 3000,
                    closeOnClick: true
                });
                $scope.LocationName = "";
                $http({
                    method: "Get",
                    url: "/Asset/GetAssetLocationClass"
                }).then(function (data) {
                    $scope.AssetLocationClass = data.data;
                });
            });
        }

        /********************Popup********************/
        $scope.togglePopupAssetLocationClass = function () {
            $("#popupClass").dxPopup("toggle", true);
        };


        $scope.SiteIddddd = {
            bindingOptions: {
                value: "MDL_Site"
            },
            placeholder: "...",
            showClearButton: true,
            rtlEnabled: true
        };

        $scope.SiteNumber = {
            bindingOptions: {
                value: "MDL_SiteNumber"
            },
            placeholder: "..",
            showClearButton: true,
            rtlEnabled: true,
            onValueChanged: function (e) {
                $scope.MDL_SiteNumber = e.value;
            }
        };


        $scope.NewDepartmentNameTxt = {
            bindingOptions: {
                value: "MDL_NewDepartmentName"
            },
            placeholder: "ادخل اسم القسم",
            showClearButton: true,
            rtlEnabled: true
        };
        $scope.NewFloorNameTxt = {
            bindingOptions: {
                value: "MDL_NewFloorName"
            },
            placeholder: "..",
            showClearButton: true,
            rtlEnabled: true
        };

        $scope.NewFloorNumberTxt = {
            bindingOptions: {
                value: "MDL_NewFloorNumber"
            },
            placeholder: "..",
            showClearButton: true,
            rtlEnabled: true,
            onValueChanged: function (e) {
                $scope.MDL_NewFloorNumber = e.value;
            }
        };
        $scope.NewBranchName = {
            bindingOptions: {
                value: "MDL_NewBranchName"
            },
            placeholder: "..",
            showClearButton: true,
            rtlEnabled: true
        };
        $scope.GetBranchies = function () {
            $scope.Floors = [];
            $scope.Departments = [];
            $http({
                method: "Get",
                url: "/Asset/GetAssetBranchies"
            }).then(function (data) {
                debugger;
                $scope.Branchies = data.data;

                var GetDefaultCompany = AssetService.GetDefaultCompany();

                GetDefaultCompany.then(function (dataDefault) {
                    if (dataDefault.data === -1) {
                        window.location.href = "/Login/Login";
                    } else {
                        $("#BranchId").val(dataDefault.data).change();
                        //$scope.MDL_Branch = dataDefault.data;
                        // $scope.GetDepartments();
                    }
                });

            });
        };
        $scope.GetBranchiesNotInAssetLocations = function () {
            $http({
                method: "Get",
                url: "/Asset/GetAssetBranchiesNotInAssetLocations"
            }).then(function (data) {

                $scope.BranchiesNotInAssetLocation = data.data.BranchiesNotInQ;
                $scope.BranchiesInAssetLocation = data.data.BranchiesInQ;
            });
        };
        $scope.EdittedSiteId = "";


        //Tree
        $scope.GetLocationsTree = function () {
            $http({
                method: "Get",
                url: "/Asset/GetLocationsTree"
            }).then(function (data) {

                $scope.LocationTreeArrey = data.data;
                $scope.treeListOptions = {///////////////////////////////////////////////////
                    bindingOptions: {
                        dataSource: "LocationTreeArrey"
                        //"editing.editingEnabled": "Permissions.UpdateSite",
                        //"editing.allowUpdating": "Permissions.UpdateSite",
                        //"editing.allowDeleting": "Permissions.DeleteSite"
                    },
                    keyExpr: "ID",
                    parentIdExpr: "ParentID",
                    columns: [
                        {
                            dataField: "LocationName",
                            caption: "اسم الموقع"
                        }
                    ],
                    showRowLines: true,
                    columnAutoWidth: true,
                    rtlEnabled: true,

                    searchPanel: {
                        visible: true,
                        placeholder: "بحث"
                    },
                    wordWrapEnabled: true,
                    sorting: {
                        mode: "multiple"
                    },
                    selection: {
                        mode: "single"
                    },
                    editing: {
                        allowDeleting: true,
                        allowUpdating: true,
                        allowAdding: true,
                        mode: "form",
                        texts: {
                            confirmDeleteMessage: "تأكيد حذف  !",
                            deleteRow: "حذف",
                            editRow: "تعديل",
                            addRow: "اضافة"
                        }
                    },
                    onInitNewRow: function (e) {
                        debugger;
                        $scope.LocationId = e.data.ParentID;
                        $scope.PopUpHosuingFurnitureShow = true;
                        window.setTimeout(function () { e.component.cancelEditData(); }, 0);
                    },
                    onRowRemoving: function (e) {
                        debugger;
                        $http({
                            method: "Post",
                            url: "/Asset/DeleteSite",
                            data: { SiteId: e.data.ID }
                        }).then(function (dataD) {
                            if (dataD.data !== "") {
                                DevExpress.ui.notify({ message: dataD.data }, "Error", 2000);
                                //$scope.AlertMessage = dataD.data;
                                //var myEl = angular.element(document.querySelector("#divMsg"));
                                //myEl.addClass("alert alert-danger");
                            }
                        });
                    },
                    onRowRemoved: function (e) {
                        $scope.refresh();
                    },
                    onEditingStart: function (e) {
                        debugger;
                        $http({
                            method: "Get",
                            url: "/Asset/GetFloorSite",
                            params: { _SiteId: e.data.ID }
                        }).then(function (dataSite) {
                            debugger;
                            $scope.txtBedsPerRoomdisabled = true;
                            $scope.Height = dataSite.data.Height;
                            $scope.Width = dataSite.data.width;
                            $scope.Depth = dataSite.data.Depth;
                            $scope.MDL_Branch = dataSite.data.CompanyId;
                            $scope.ClassID = dataSite.data.ClassID == null ? "" : dataSite.data.ClassID.toString();
                            $scope.BedsPerRoom = dataSite.data.BedsPerRoom;
                            $scope.Floors = [];
                            $http({
                                method: "Get",
                                url: "/Asset/GetAssetDepartment",
                                params: { BranchId: $scope.MDL_Branch }
                            }).then(function (dataDep) {
                                $scope.Departments = dataDep.data;
                                $scope.DepartmentFloors = dataDep.data;




                                $timeout(function () {
                                    $scope.MDL_Department = dataSite.data.DepartmentId;
                                    $http({
                                        method: "Get",
                                        url: "/Asset/GetAssetFloors",
                                        params: { DepartmentId: $scope.MDL_Department }
                                    }).then(function (dataFl) {
                                        $scope.Floors = dataFl.data;
                                        $timeout(function () {
                                            $scope.MDL_Floors = dataSite.data.FloorId;
                                            $scope.MDL_Site = dataSite.data.SiteName;
                                            $scope.MDL_SiteNumber = dataSite.data.UnitNumber;
                                            $scope.UpdateSiteButtonShow = true;
                                            $scope.NewSiteButtonShow = true;
                                            $scope.SaveSiteButtonShow = false;
                                            $scope.EdittedSiteId = e.data.ID;
                                        });
                                    });
                                });
                            }).finally(function () {
                            });
                        });
                        e.cancel = true;
                    },
                    onCellPrepared: function (e) {
                        if (e.rowType === "data" && e.column.command === "edit") {
                            var isEditing = e.row.isEditing, $links = e.cellElement.find(".dx-link");
                            $links.text("");
                            if (isEditing) {
                                $links.filter(".dx-link-save").addClass("dx-icon-save");
                                $links.filter(".dx-link-cancel").addClass("dx-icon-revert");
                            } else {
                                $links.filter(".dx-link-edit").addClass("btn btn-dark-orange btn-sm fa fa-pencil");
                                $links.filter(".dx-link-delete").addClass("btn btn-danger btn-sm fa fa-trash-o");
                                //$links.filter(".dx-link-add").addClass("btn btn-success btn-sm fa fa-plus");
                            }
                            if (e.row.node.level < 3) {
                                if (e.column.command === "edit" || e.column.command === "delete" || e.column.command === "add") {
                                    $links.filter(".dx-link-edit").removeClass("btn btn-dark-orange btn-sm fa fa-pencil");
                                    $links.filter(".dx-link-delete").removeClass("btn btn-danger btn-sm fa fa-trash-o");
                                    //$links.filter(".dx-link-add").removeClass("btn btn-success btn-sm fa fa-plus");
                                }
                            }
                        }
                    },
                    rowAlternationEnabled: true,
                    showBorders: true
                    //,
                    //cacheEnabled: false
                };
                //$scope.LocationTreeArrey = data.data;
            });
        };

        $scope.NewSite = function () {
            $scope.UpdateSiteButtonShow = false;
            $scope.NewSiteButtonShow = false;
            $scope.SaveSiteButtonShow = true;
            $scope.EdittedSiteId = "";
            $scope.MDL_Site = "";
            $scope.MDL_SiteNumber = "";
            $scope.txtBedsPerRoomdisabled = false;
        };

        $scope.UpdateSite = function () {
            if ($scope.EdittedSiteId !== "") {
                if ($scope.MDL_Branch == null) {
                    DevExpress.ui.notify({ message: "عفوا ادخل  الوحدة الإدارية" }, "Error", 2000);
                    return;
                }

                if ($scope.MDL_Department == null) {
                    DevExpress.ui.notify({ message: "عفوا ادخل  القسم" }, "Error", 2000);
                    return;
                }

                if ($scope.MDL_Floors == null) {
                    DevExpress.ui.notify({ message: "عفوا ادخل  الدور" }, "Error", 2000);
                    return;
                }

                if ($scope.MDL_Site == "") {
                    DevExpress.ui.notify({ message: "عفوا ادخل  الموقع الفرعى" }, "Error", 2000);
                    return;
                }
                if ($scope.MDL_SiteNumber  == "") {
                    DevExpress.ui.notify({ message: "عفوا ادخل رقم الموقع الفرعى" }, "Error", 2000);
                    return;
                }

                if ($scope.BedsPerRoom == null) {
                    DevExpress.ui.notify({ message: "عفوا ادخل  السعة السريرية" }, "Error", 2000);
                    return;
                }

                $http({
                    method: "Post",
                    url: "/Asset/UpdateSite",
                    data: {
                        LocationName: $scope.MDL_Site, FloorId: $scope.MDL_Floors, SiteId: $scope.EdittedSiteId,
                        ClassID: $scope.ClassID, Height: $scope.Height, Width: $scope.Width, Depth: $scope.Depth, BedsPerRoom: $scope.BedsPerRoom, SiteNumber: $scope.MDL_SiteNumber
                    }
                }).then(function (data) {
                    if (data.data != "") {
                        DevExpress.ui.notify({ message: data.data }, "Error", 2000);
                    } else {
                        DevExpress.ui.notify({ message: "تم التعديل بنجاح" }, "success", 2000);
                        $scope.refresh();
                    }
                });
            }


        };


        $scope.refresh = function () {
            $http({
                method: "Get",
                url: "/Asset/GetLocationsTree"
            }).then(function (data) {
                $scope.LocationTreeArrey = data.data;
                var tree = $("#AssetLocations").dxTreeList("instance");
                tree.option({ dataSource: $scope.LocationTreeArrey });
            });
        };
        $scope.GetLocationsTree();
        $scope.GetBranchies();
        $scope.GetDepartments = function () {
            if ($scope.MDL_Branch > 0) {
                $scope.Floors = [];
                $http({
                    method: "Get",
                    url: "/Asset/GetAssetDepartment",
                    params: { BranchId: $scope.MDL_Branch }
                }).then(function (data) {
                    $scope.Departments = data.data;
                    $scope.DepartmentFloors = data.data;
                });
            }
        };
        $scope.GetFloors = function () {
            $http({
                method: "Get",
                url: "/Asset/GetAssetFloors",
                params: { DepartmentId: $scope.MDL_Department }
            }).then(function (data) {
                $scope.Floors = data.data;
            });
        };
        $scope.SaveSite = function () {
            if ($scope.MDL_Branch == null) {
                DevExpress.ui.notify({ message: "عفوا ادخل  الوحدة الإدارية" }, "Error", 2000);
                return;
            }

            if ($scope.MDL_Department == null) {
                DevExpress.ui.notify({ message: "عفوا ادخل  القسم" }, "Error", 2000);
                return;
            }

            if ($scope.MDL_Floors == null) {
                DevExpress.ui.notify({ message: "عفوا ادخل  الدور" }, "Error", 2000);
                return;
            }


            if ($scope.MDL_Site == "") {
                DevExpress.ui.notify({ message: "عفوا ادخل  الموقع الفرعى" }, "Error", 2000);
                return;
            }

            if ($scope.MDL_SiteNumber  == "") {
                DevExpress.ui.notify({ message: "عفوا ادخل رقم الموقع الفرعى" }, "Error", 2000);
                return;
            }

            if ($scope.ClassID == null) {
                DevExpress.ui.notify({ message: "عفوا ادخل  تصنيف الموقع" }, "Error", 2000);
                return;
            }

            if ($scope.Height == null) {
                DevExpress.ui.notify({ message: "عفوا ادخل  طول الموقع" }, "Error", 2000);
                return;
            }

            if ($scope.Width == null) {
                DevExpress.ui.notify({ message: "عفوا ادخل  عرض الموقع" }, "Error", 2000);
                return;
            }

            if ($scope.Depth == null) {
                DevExpress.ui.notify({ message: "عفوا ادخل  ارتفاع الموقع" }, "Error", 2000);
                return;
            }

            if ($scope.BedsPerRoom == null) {
                DevExpress.ui.notify({ message: "عفوا ادخل  السعة السريرية" }, "Error", 2000);
                return;
            }

            let result = confirm(`هل انت متاكد من ان السعة السريرية = ${$scope.BedsPerRoom}`);
            if (result === true) {
                debugger;
                $http({
                    method: "Post",
                    url: "/Asset/SaveSite",
                    data: {
                        LocationName: $scope.MDL_Site, FloorId: $scope.MDL_Floors, BranchId: $scope.MDL_Branch, ClassID: $scope.ClassID, Height: $scope.Height, Width: $scope.Width, Depth: $scope.Depth, BedsPerRoom: $scope.BedsPerRoom, SiteNumber: $scope.MDL_SiteNumber
                    }
                }).then(function (data) {
                    if (data.data !== "") {
                        DevExpress.ui.notify({ message: data.data }, "Error", 2000);
                    } else if (data.data === "") {
                        $scope.GetLocationsTree();
                        DevExpress.ui.notify({ message: "تم الحفظ بنجاح" }, "success", 2000);
                        $scope.refresh();

                        $scope.MDL_Site = "";

                        $scope.MDL_Floors = null;
                        $scope.MDL_Department = null;
                        $scope.ClassID = null;
                        $scope.Height = 1;
                        $scope.Width = 1;
                        $scope.Depth = 1;
                        $scope.ClassID = null;
                        $scope.MDL_Branch = null;
                        $scope.BedsPerRoom = null;
                        $scope.MDL_Site = "";
                        $scope.MDL_SiteNumber = "";
                    }
                });

            }
            else
                return;
      
        };
        $scope.PopUpBranchies = {
            width: 700,
            contentTemplate: "info",
            showTitle: true,
            dragEnabled: false,
            bindingOptions: {
                visible: "CompanyPopUpShow",
                title: "BranchesPopUpTitle"
            },
            rtlEnabled: true,
            onHiding: function (e) {
            }
        };
        $scope.PopUpFloors = {
            width: 700,
            height: 700,
            contentTemplate: "info",
            showTitle: true,
            dragEnabled: false,
            bindingOptions: {
                visible: "FloorPopUpShow",
                title: "FloorsPopUpTitle"
            },
            rtlEnabled: true,
            onHiding: function (e) {
            }
        };
        $scope.PopUpDepartments = {
            width: 700,
            height: 700,
            contentTemplate: "info",
            showTitle: true,
            dragEnabled: false,
            bindingOptions: {
                visible: "DepartmentPopUpShow",
                title: "DepartmentsPopUpTitle"
            },
            rtlEnabled: true,
            onHiding: function (e) {
            }
        };
        $scope.ShowNewBranchPopUp = function () {
            $scope.MDL_NewBranchName = "";
            if ($scope.MDL_Branch == null) {
                $scope.SaveNewBranchButtonShow = true;
                $scope.UpDateNewBranchButtonShow = false;
                $scope.DeleteNewBranchButtonShow = false;
                $scope.AddNewBranchButtonShow = false;
                $scope.NewBranchIdSelectBoxDisable = false;
                $http({
                    method: "Get",
                    url: "/Asset/GetAssetBranchiesNotInAssetLocations"
                }).then(function (data) {
                    debugger;
                    $scope.BranchiesNotInAssetLocation = data.data.BranchiesNotInQ;
                    $scope.BranchiesForNew = $scope.BranchiesNotInAssetLocation;
                    $scope.BranchesPopUpTitle = "اضافة اسم وحدة إدارية فى دليل المواقع";
                    $scope.CompanyPopUpShow = true;
                });
            } else {
                $http({
                    method: "Get",
                    url: "/Asset/GetAssetBranchiesNotInAssetLocations"
                }).then(function (data) {
                    debugger;
                    $scope.BranchiesForNew = data.data.BranchiesInQ;
                    $scope.SaveNewBranchButtonShow = false;
                    $scope.UpDateNewBranchButtonShow = true;
                    $scope.DeleteNewBranchButtonShow = true;
                    $scope.AddNewBranchButtonShow = true;
                    $scope.BranchesPopUpTitle = "تعديل اسم وحدة إدارية فى دليل المواقع";
                    $scope.MDL_NewBranchName = $scope.MDL_BranchText;
                    $http({
                        method: "Get",
                        url: "/Asset/GetBranchOriginalBranch",
                        params: { BranchId: $scope.MDL_Branch }
                    }).then(function (Originaldata) {
                        $scope.MDL_NewBranch = parseInt(Originaldata.data);
                        $scope.NewBranchIdSelectBoxDisable = true;
                        $scope.CompanyPopUpShow = true;
                    });
                });
            }
        };

        $scope.ResetNewBranchPopupControls = function () {
            debugger;
            $scope.CompanyPopUpShow = false;
            $scope.UnitNumber = "";
            $scope.MDL_NewBranchName = "";
            $scope.MDL_NewBranchName = "";
            $scope.NewBranchIdSelectBoxDisable = false;
            $scope.ResetNewBranch = "اختر";

        };

        $scope.AddNewBrach = function () {
            $scope.SaveNewBranchButtonShow = true;
            $scope.UpDateNewBranchButtonShow = false;
            $scope.DeleteNewBranchButtonShow = false;
            $scope.AddNewBranchButtonShow = false;
            $scope.NewBranchIdSelectBoxDisable = false;
            $scope.BranchesPopUpTitle = "اضافة اسم وحدة إدارية فى دليل المواقع";
            $scope.MDL_NewBranchName = "";
            $http({
                method: "Get",
                url: "/Asset/GetAssetBranchiesNotInAssetLocations"
            }).then(function (data) {
                $scope.BranchiesNotInAssetLocation = data.data.BranchiesNotInQ;

                $scope.BranchiesForNew = $scope.BranchiesNotInAssetLocation;
            });
        };

        $scope.UpDateBranchLocation = function () {
            if ($scope.MDL_NewBranchName == "") {
                DevExpress.ui.notify({ message: "عفوا اسم الوحدة الإدارية المراد تعديلها فى دليل مواقع الاصول" }, "Error", 10000);
                return;
            }
            $http({
                method: "Post",
                url: "/Asset/UpDateNewBranchToAssetLocations",
                data: { BranchLocationId: $scope.MDL_Branch, BranchLocationName: $scope.MDL_NewBranchName }
            }).then(function (data) {
                if (data.data !== "") {
                    DevExpress.ui.notify({ message: data.data }, "Error", 10000);
                } else {
                    $http({
                        method: "Get",
                        url: "/Asset/GetAssetBranchies"
                    }).then(function (data1) {
                        $scope.Branchies = data1.data;
                        $scope.refresh();
                        DevExpress.ui.notify({ message: "تم التعديل بنجاح" }, "success", 10000);
                    });
                }
            });
        };

        $scope.AddNewBranchToAssetLocations = function () {
            debugger;
            if ($scope.MDL_NewBranch == null) {
                DevExpress.ui.notify({
                    message: "عفوا اختر الوحدة الإدارية المراد ادخالها فى دليل مواقع الاصول"
                },
                    "Error",
                    2000);
                return;
            }

            if ($scope.MDL_NewBranchName === "") {
                DevExpress.ui.notify({
                    message: "عفوا ادخل اسم الوحدة الإدارية المراد ادخالها فى دليل الحسابات"
                },
                    "Error",
                    2000);
                return;
            }

            if ($scope.UnitNumber == "") {
                DevExpress.ui.notify({ message: "عفوا ادخل  رقم الوحدة" }, "Error", 10000);
                return;
            }

            $http({
                method: "Post",
                url: "/Asset/AddNewBranchToAssetLocations",
                data: { BranchId: $scope.MDL_NewBranch, BranchName: $scope.MDL_NewBranchName, UnitNumber: $scope.UnitNumber, IsFamilial: $scope.IsFamilial }

            }).then(function (data) {
                if (data.data !== "") {
                    DevExpress.ui.notify({
                        message: data.data
                    },
                        "Error",
                        2000);
                } else if (data.data === "") {


                    $http({
                        method: "Get",
                        url: "/Asset/GetAssetBranchiesNotInAssetLocations"
                    }).then(function (data) {
                        $scope.BranchiesNotInAssetLocation = data.data.BranchiesNotInQ;

                        $scope.BranchiesForNew = $scope.BranchiesNotInAssetLocation;

                        $scope.GetBranchies();

                        $scope.refresh();
                        DevExpress.ui.notify({
                            message: "تم الحفظ بنجاح"
                        },
                            "success",
                            2000);
                        $scope.MDL_NewBranchName = "";
                    });

                }
            });


        };

        $scope.DeleteNewBranchToAssetLocations = function () {
            if ($scope.MDL_Branch == null) {

                DevExpress.ui.notify({
                    message: "عفوا لا يمكن الحذف"
                },
                    "Error",
                    2000);
                return;
            }
            $http({
                method: "Post",
                url: "/Asset/DeleteNewBranchToAssetLocations",
                data: { BranchLocationId: $scope.MDL_Branch }
            }).then(function (data) {
                if (data.data !== "") {
                    DevExpress.ui.notify({
                        message: data.data
                    },
                        "Error",
                        2000);
                    return;
                } else {


                    DevExpress.ui.notify({
                        message: "تم الحذف بنجاح"
                    },
                        "success",
                        2000);
                    $scope.refresh();

                    $scope.GetBranchies();
                    $scope.AddNewBrach();
                    $scope.MDL_NewBranch = null;
                }

            });

        };


        $scope.SetDDLtEXT = function () {
            debugger;
            $scope.MDL_NewBranchName = $scope.MDL_NewBranchText;
        };


        $scope.ShowNewDepartmentPopUp = function () {


            $scope.MDL_NewDepartmentName = "";
            if ($scope.MDL_Department == null) {
                $scope.SaveNewDepartmentButtonShow = true;
                $scope.UpDateNewDepartmentButtonShow = false;
                $scope.DeleteNewDepartmentButtonShow = false;
                $scope.AddNewDepartmentButtonShow = false;
                $scope.DepartmentBranchdisabled = false;


                $scope.DepartmentsPopUpTitle = "اضافة قسم فى دليل المواقع";
                $scope.DepartmentPopUpShow = true;


            } else {

                $scope.SaveNewDepartmentButtonShow = false;
                $scope.UpDateNewDepartmentButtonShow = true;
                $scope.DeleteNewDepartmentButtonShow = true;
                $scope.AddNewDepartmentButtonShow = true;
                $scope.DepartmentsPopUpTitle = "تعديل قسم فى دليل المواقع";

                $scope.MDL_DepartmentBranch = $scope.MDL_Branch;
                $scope.MDL_NewDepartmentName = $scope.MDL_DepartmentText;

                $scope.DepartmentPopUpShow = true;


            }
        };

        $scope.UpDateDepartment = function () {
            if ($scope.MDL_DepartmentBranch == null) {


                DevExpress.ui.notify({
                    message: "عفوا ادخل الوحدة الإدارية"
                },
                    "Error",
                    2000);
                return;
            }


            if ($scope.MDL_NewDepartmentName === "") {
                DevExpress.ui.notify({
                    message: "عفوا اسم القسم"
                },
                    "Error",
                    2000);
                return;

            }


            debugger;
            $http({
                method: "Post",
                url: "/Asset/UpDateDepartment",
                data: {
                    DepartmentId: $scope.MDL_Department,
                    BranchId: $scope.MDL_DepartmentBranch,
                    DepartmentName: $scope.MDL_NewDepartmentName
                }
            }).then(function (data) {
                if (data.data !== "") {
                    DevExpress.ui.notify({
                        message: data.data
                    },
                        "Error",
                        2000);
                } else if (data.data === "") {
                    $scope.refresh();
                    DevExpress.ui.notify({
                        message: "تم التعديل بنجاح"
                    },
                        "success",
                        2000);
                    $scope.GetDepartments();
                }
            });

        };


        $scope.PrepareAddingDepartment = function () {
            $scope.SaveNewDepartmentButtonShow = true;
            $scope.UpDateNewDepartmentButtonShow = false;
            $scope.DeleteNewDepartmentButtonShow = false;
            $scope.AddNewDepartmentButtonShow = false;
            $scope.DepartmentesPopUpTitle = "اضافة قسم فى دليل المواقع";


            $scope.MDL_NewDepartmentName = "";
            $scope.GetDepartments();
        };


        $scope.AddNewDepartmentToAssetLocations = function () {
            if ($scope.MDL_DepartmentBranch == null) {
                DevExpress.ui.notify({
                    message: "عفوا ادخل اسم الوحدة الإدارية"
                },
                    "Error",
                    2000);
                return;

            }


            if ($scope.MDL_NewDepartmentName === "") {
                DevExpress.ui.notify({
                    message: "عفوا ادخل اسم القسم"
                },
                    "Error",
                    2000);
                return;
            }


            $http({
                method: "Post",
                url: "/Asset/InsertDepartment",
                data: { BranchId: $scope.MDL_DepartmentBranch, DepartmentName: $scope.MDL_NewDepartmentName }
            }).then(function (data) {
                if (data.data !== "") {
                    DevExpress.ui.notify({
                        message: data.data
                    },
                        "Error",
                        2000);

                } else if (data.data === "") {
                    $scope.GetDepartments();
                    $scope.refresh();


                    DevExpress.ui.notify({
                        message: "تم الحفظ بنجاح"
                    },
                        "success",
                        2000);
                    $scope.MDL_NewDepartmentName = "";
                }

            });


        };


        $scope.DeleteNewDepartmentToAssetLocations = function () {
            if ($scope.MDL_Department == null) {
                DevExpress.ui.notify({
                    message: "عفوا ادخل اسم الوحدة الإدارية"
                },
                    "Error",
                    2000);
                return;
            }

            $http({
                method: "Post",
                url: "/Asset/DeleteDepartment",
                data: { DepartmentId: $scope.MDL_Department }
            }).then(function (data) {
                if (data.data !== "") {
                    DevExpress.ui.notify({
                        message: data.data
                    },
                        "Error",
                        2000);
                } else if (data.data === "") {
                    $scope.GetDepartments();
                    $scope.PrepareAddingDepartment();
                    $scope.refresh();
                    DevExpress.ui.notify({
                        message: "تم الحذف بنجاح"
                    },
                        "success",
                        2000);
                }
            });


        };


        $scope.ShowNewFloorPopUp = function () {


            $scope.MDL_NewFloorName = "";
            $scope.MDL_NewFloorNumber = null;

            if ($scope.MDL_Floors == null) {
                $scope.SaveNewFloorButtonShow = true;
                $scope.UpDateNewFloorButtonShow = false;
                $scope.DeleteNewFloorButtonShow = false;
                $scope.AddNewFloorButtonShow = false;
                $scope.FloorsPopUpTitle = "اضافة دور فى دليل المواقع";
                $scope.FloorPopUpShow = true;
            } else {

                $scope.SaveNewFloorButtonShow = false;
                $scope.UpDateNewFloorButtonShow = true;
                $scope.DeleteNewFloorButtonShow = true;
                $scope.AddNewFloorButtonShow = true;
                $scope.FloorsPopUpTitle = "تعديل دور فى دليل المواقع";


                $scope.MDL_FloorBranch = $scope.MDL_Branch;


                $scope.MDL_FloorDepartment = $scope.MDL_Department;

                $scope.MDL_NewFloorName = $scope.MDL_FloorsText;



                $scope.FloorPopUpShow = true;


            }
        };
        $scope.AddNewFloorToAssetLocations = function () {
            if ($scope.MDL_FloorDepartment == null) {
                DevExpress.ui.notify({
                    message: "عفوا ادخل القسم"
                },
                    "Error",
                    2000);
                return;
            }

            if ($scope.MDL_NewFloorName === "") {

                DevExpress.ui.notify({
                    message: "عفوا ادخل اسم الدور"
                },
                    "Error",
                    2000);
                return;
            }

            if ($scope.MDL_NewFloorNumber === "") {

                DevExpress.ui.notify({
                    message: "عفوا ادخل رقم الدور"
                },
                    "Error",
                    2000);
                return;
            }

            $http({
                method: "Post",
                url: "/Asset/InsertFloor",
                data: { DepartmentId: $scope.MDL_FloorDepartment, FloorName: $scope.MDL_NewFloorName, FloorNumber:$scope.MDL_NewFloorNumber}

            }).then(function (data) {
                if (data.data !== "") {


                    DevExpress.ui.notify({
                        message: data.data
                    },
                        "Error",
                        2000);

                } else {

                    $scope.GetFloors();
                    $scope.refresh();


                    DevExpress.ui.notify({
                        message: "تم الحفظ بنجاح"
                    },
                        "success",
                        2000);

                    $scope.MDL_NewFloorName = "";

                }
            });
        };

        $scope.GetDepartmentsInFloorsPopUp = function () {
            $http({
                method: "Get",
                url: "/Asset/GetAssetDepartment",
                params: { BranchId: $scope.MDL_FloorBranch }
            }).then(function (data) {

                $scope.DepartmentFloors = data.data;
            });
        };

        $scope.PrepareAddingFloors = function () {
            $scope.SaveNewFloorButtonShow = true;
            $scope.UpDateNewFloorButtonShow = false;
            $scope.DeleteNewFloorButtonShow = false;
            $scope.AddNewFloorButtonShow = false;
            $scope.FloorsPopUpTitle = "اضافة دور فى دليل المواقع";


            $scope.MDL_NewFloorName = "";

            $scope.MDL_NewFloorNumber = null;

        };

        $scope.UpDateNewFloorButton = function () {

            if ($scope.MDL_FloorDepartment == null) {

                DevExpress.ui.notify({
                    message: "عفوا ادخل القسم"
                },
                    "Error",
                    2000);
                return;
            }
            if ($scope.MDL_NewFloorName == "") {


                DevExpress.ui.notify({
                    message: "عفوا ادخل اسم الدور"
                },
                    "Error",
                    2000);


                return;
            }


            if ($scope.MDL_NewFloorNumber == "") {


                DevExpress.ui.notify({
                    message: "عفوا ادخل رقم الدور"
                },
                    "Error",
                    2000);


                return;
            }

            debugger;
            $http({
                method: "Post",
                url: "/Asset/UpDateFloor",
                data: {
                    FloorId: $scope.MDL_Floors,
                    DepartmentId: $scope.MDL_FloorDepartment,
                    FloorName: $scope.MDL_NewFloorName,
                    FloorNumber: $scope.MDL_NewFloorNumber

                }

            }).then(function (data) {
                if (data.data !== "") {
                    DevExpress.ui.notify({
                        message: data.data
                    },
                        "Error",
                        2000);

                } else {

                    $scope.GetFloors();
                    $scope.refresh();


                    DevExpress.ui.notify({
                        message: "تم التعديل بنجاح"
                    },
                        "success",
                        2000);

                }
            });

        };

        $scope.DeleteNewFloorToAssetLocations = function () {
            if ($scope.MDL_Floors == null) {
                DevExpress.ui.notify({
                    message: "عفوا لا يوجد دور"
                },
                    "Error",
                    2000);


                return;
            }


            $http({
                method: "Post",
                url: "/Asset/DeleteFloor",
                data: { FloorId: $scope.MDL_Floors }
            }).then(function (data) {
                if (data.data !== "") {
                    DevExpress.ui.notify({
                        message: data.data
                    },
                        "Error",
                        2000);
                } else {
                    $scope.GetFloors();
                    $scope.refresh();


                    DevExpress.ui.notify({
                        message: "تم الحذف بنجاح"
                    },
                        "success",
                        2000);


                    $scope.PrepareAddingFloors();

                }
            });
        };

        //---------------------------------
        $scope.CheckIsFamilial = {
            bindingOptions: { value: "!IsFamilial", text: "CheckIsFamilialText" },
            rtlEnabled: true,
            items: [
                { text: 'فردي', value: true },
                { text: 'عائلي', value: false }
            ],
            onValueChanged: function (e) {
                debugger;

                if (e.value == true)
                    $scope.CheckIsFamilialText = "فردي";
                else
                    $scope.CheckIsFamilialText = "عائلي";

            },
            valueExpr: 'value',
            layout: 'horizontal'

        }


       
        // Width - Height - Depth
        $scope.txtHeight = {
            Height: {
                bindingOptions: {
                    value: "Height" //$scope.Height
                },
                placeholder: "..",

                onValueChanged: function (e) {
                    $scope.Height = e.value;
                }
            }
        }
        $scope.txtWidth = {
            Width: {
                bindingOptions: {
                    value: "Width" //$scope.Width
                },
                placeholder: "..",

                onValueChanged: function (e) {
                    $scope.Width = e.value;
                }
            }
        }
        $scope.txtDepth = {
            Depth: {
                bindingOptions: {
                    value: "Depth" //$scope.Depth
                },
                placeholder: "..",

                onValueChanged: function (e) {
                    $scope.Depth = e.value;
                }
            }
        }

        $scope.txtBedsPerRoom = {
            BedsPerRoom: {
                bindingOptions: {
                    value: "BedsPerRoom" ,//$scope.BedsPerRoom
                    disabled:"txtBedsPerRoomdisabled"
                },
                placeholder: "..",

                onValueChanged: function (e) {
                    $scope.BedsPerRoom = e.value;
                }
            }
        }

        $scope.txtUnitNumber = {
            UnitNumber: {
                bindingOptions: {
                    value: "UnitNumber" //$scope.UnitNumber
                },
                placeholder: "..",

                onValueChanged: function (e) {
                    debugger;
                    $scope.UnitNumber = e.value;
                }
            }
        }


        //HosuingFurnitureCategory
        $scope.PopUpHosuingFurniture = {
            width: 700,
            height: 700,
            contentTemplate: "info",
            showTitle: true,
            dragEnabled: false,
            bindingOptions: {
                visible: "PopUpHosuingFurnitureShow"
            },
            title: "الاثاث",
            rtlEnabled: true,
            onHiding: function (e) {
            }
        };

        $scope.HosuingFurnitureCategorySelectBox = {
            bindingOptions: {
                items: "FurnitureCategiesList",
                value: "FurnitureCategoryId"
            },
            displayExpr: "Text",
            valueExpr: "Value",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            onInitialized: function (e) {
                $http({
                    method: "Get",
                    url: "/Asset/HosuingFurnitureCategory"
                }).then(function (data) {
                    $scope.FurnitureCategiesList = data.data;
                });
            },
            onValueChanged: function (e) {
                debugger;
                $scope.FurnitureCategoryId = e.value;

                if (e.value != null) {
                    $http({
                        method: "Get",
                        url: "/Asset/HosuingFurnitureByCategory/" + Number(e.value)
                    }).then(function (data) {
                        $scope.HosuingFurnitureList = data.data;
                    });
                }
            }
        };

        $scope.HosuingFurnitureSelectBox = {
            bindingOptions: {
                items: "HosuingFurnitureList",
                value: "HosuingFurnitureId"
            },
            displayExpr: "Text",
            valueExpr: "Value",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            //onInitialized: function (e) {

            //},
            onValueChanged: function (e) {
                $scope.HosuingFurnitureId = e.value;
            }
        };

        $scope.txtFurnitureCount = {
            bindingOptions: {
                value: "FurnitureCount" //$scope.FurnitureCount
            },
            placeholder: "عدد القطع",

            onValueChanged: function (e) {
                $scope.FurnitureCount = e.value;
            }
        }


        $scope.AddNewHosuingFurniture = function () {
            debugger
            if ($scope.FurnitureCategoryId == null) {
                DevExpress.ui.notify({
                    message: "عفوا ادخل تصنيف الاثاث"
                },
                    "Error",
                    10000);
                return;
            }

            if ($scope.HosuingFurnitureId == null) {

                DevExpress.ui.notify({
                    message: "عفوا ادخل اسم الاثاث"
                },
                    "Error",
                    2000);
                return;
            }

            if ($scope.FurnitureCount == null) {

                DevExpress.ui.notify({
                    message: "عفوا ادخل عدد القطع"
                },
                    "Error",
                    2000);
                return;
            }


            $http({
                method: "Post",
                url: "/Asset/SaveHosuingFurniture",
                data: { LocationId: $scope.LocationId, FurnitureId: $scope.HosuingFurnitureId, Count: Number($scope.FurnitureCount) }

            }).then(function (data) {
                if (data.data !== "") {


                    DevExpress.ui.notify({
                        message: data.data
                    },
                        "Error",
                        2000);

                } else {

                    $scope.refresh();

                    DevExpress.ui.notify({
                        message: "تم الحفظ بنجاح"
                    },
                        "success",
                        2000);

                    $scope.FurnitureCategoryId = null;
                    $scope.HosuingFurnitureId = null;
                    $scope.FurnitureCount = null;

                }
            });
        };


    });