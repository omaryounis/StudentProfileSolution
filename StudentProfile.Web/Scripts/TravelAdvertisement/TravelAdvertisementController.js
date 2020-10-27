app.controller("TravelAdvertisementCtrl", ["$scope", 'TravelAdvertisementSrvc', "$http", 'MusaferSrvc', function ($scope, TravelAdvertisementSrvc, MusaferSrvc) {
    //Edit Advertisement
    var urlParams = new URLSearchParams(window.location.search);
    $scope.id = Number(urlParams.get('adID'));

    //Filed
    $scope.AdName = null;
    $scope.Nots = null;
    //DDL
    $scope.LevelList = [];
    $scope.CustomFieldList = [];
    $scope.MajorCustomFieldList = [];
    $scope.LevelID = null;
    $scope.CustomFieldId = null;
    $scope.MajorCustomFieldId = null;
    $scope.PurposeList = [];
    $scope.PurposeID = null;
    $scope.AgentList = [];
    $scope.AgentID = null;
    $scope.StudentData = [];

   


    //Date
    $scope.AdDateFromDate = null;
    $scope.AdDateToDate = null;
    $scope.DepartingStartFromDate = null;
    $scope.DepartingStartToDate = null;
    $scope.DepartingEndFromDate = null;
    $scope.DepartingEndToDate = null;
    //MultiTag
    $scope.NationalityList = [];
    $scope.NationalityId = null;
    $scope.NationalityIds = null;
    $scope.DegreeList = [];
    $scope.DegreeId = null;
    $scope.DegreeIds = null;
    $scope.StStatusList = [];
    $scope.StStatusId = null;
    $scope.StStatusIds = null;
    $scope.ScholarshipTypeList = [];
    $scope.ScholarshipTypeId = null;
    $scope.ScholarshipTypeIds = null;
    //RadioPriorities
    $scope.TravelWayPriorities = [{ value: 'R', text: "ذهاب وعودة" }, { value: 'O', text: "ذهاب فقط" }];
    $scope.SelectTravelWay = $scope.TravelWayPriorities[0]; // value TravelWay

    $scope.popupConfigShow = false;

    // Date Validation
    $scope.ValidateMinAdDateFromDate = Date.now();
    $scope.ValidateMinAdDateToDate = null;
    $scope.ValidateMinDepartingStartFromDate = Date.now();
    $scope.ValidateMinDepartingStartToDate = null;
    $scope.ValidateMinDepartingEndFromDate = null;
    $scope.ValidateMinDepartingEndToDate = null;

    //Edit Advertisement
    if ($scope.id != 0) {
        TravelAdvertisementSrvc.GetAdvertisementById($scope.id).then(function (data) {
            debugger;
            $scope.CustomFieldId = data.data.CustomFieldId;
            $scope.IsAppearToExpectedGradutedPeople = data.data.IsAppearToExpectedGradutedPeople;
            $scope.AdName = data.data.AdName;
            $scope.Nots = data.data.Notes;
            $scope.LevelID = data.data.TravelClass.toString();
            $scope.PurposeID = data.data.PurposeID.toString();
            $scope.AgentID = data.data.AgentID.toString();
            $scope.AdDateFromDate = new Date(data.data.AdStartDate);
            $scope.AdDateToDate = new Date(data.data.AdEndDate);
            $scope.DepartingStartFromDate = new Date(data.data.DepartingStart);
            $scope.DepartingStartToDate = new Date(data.data.DepartingEnd);
            $scope.DepartingEndFromDate = new Date(data.data.ReturningStart);
            $scope.DepartingEndToDate = new Date(data.data.ReturningEnd);
            $scope.TravelWay = data.data.flightsType;
            $scope.TravelWayPriorities.value = data.data.flightsType;

            $scope.ScholarshipTypeId = data.data.ScholarshipType.split(",");
            $scope.NationalityId = data.data.NationalityID.split(",");
            $scope.DegreeId = data.data.DegreeID.split(",");
            $scope.StStatusId = data.data.StStatusID.split(",");

        });
    }


    //Control
    $scope.txtAdName = {
        bindingOptions: {
            value: "AdName"
        },
        placeholder: "اسم الاعلان",
        onValueChanged: function (e) {
            $scope.AdName = e.value;
        }
    }

    $scope.txtNots = {
        bindingOptions: {
            value: "Nots"
        },
        placeholder: "ملاحظات",
        onValueChanged: function (e) {
            $scope.Nots = e.value;
        }
    }

    $scope.radioTravelWay = {
        bindingOptions: {
            dataSource: "TravelWayPriorities",
            value: "SelectTravelWay",
            items: "TravelWayPriorities"
        },

        //dataSource: $scope.TravelWayPriorities,
        //value: $scope.TravelWayPriorities[0],
        layout: "horizontal",
        onValueChanged: function (e) {
            debugger;
            $scope.TravelWay = e.value.value;
        }
    };
        $scope.LevelSelectBox = {
            bindingOptions: {
                dataSource: "LevelList",
                value: "LevelID",
                items: "LevelList"
            },

            placeholder: "--أختر--",
            noDataText: "لا يوجد بيانات",
            paginate: true, //Pagenation
            showClearButton: true,
            searchEnabled: true,
            displayExpr: "Text",
            valueExpr: "Value",
            disabled: false,

            onInitialized: function (e) {
                TravelAdvertisementSrvc.GetLevels().then(function (data) {
                    $scope.LevelList = data.data;
                });
            },
            onValueChanged: function (e) {
                $scope.LevelID = e.value;
            }
    };

    $scope.MajorCustomFieldSelectBox = {
        bindingOptions: {
            dataSource: "MajorCustomFieldList",
            value: "MajorCustomFieldId",
            items: "MajorCustomFieldList"
        },
        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        paginate: true, //Pagenation
        displayExpr: "Text",
        valueExpr: "Value",
        showBorders: true,
        searchEnabled: true,
        showSelectionControls: true,
        maxDisplayedTags: 2,
        showMultiTagOnly: false,
        onInitialized: function (e) {
            TravelAdvertisementSrvc.MajorGetCustomFields().then(function (data) {
                $scope.MajorCustomFieldList = data.data;
            });
        },
        onValueChanged: function (e) {
            debugger;
            if (e != undefined && e != null && e != '') {
                if (e.value != null) {
                    if (e.value.length != 0) {
                        $scope.MajorCustomFieldId = e.value.join(',');
                        $scope.CustomFieldList = '';
                        TravelAdvertisementSrvc.GetCustomFields($scope.MajorCustomFieldId).then(function (data) {
                            debugger;
                            $scope.CustomFieldList = data.data;
                        });
                    }
                } else if (e.value === null) {

                    $scope.CustomFieldList = null;
                }

            }
           
        }
    };


    $scope.CustomFieldSelectBox = {
        bindingOptions: {
            dataSource: "CustomFieldList",
            value: "CustomFieldId",
            items: "CustomFieldList"
        },

        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        paginate: true, //Pagenation
        displayExpr: "Text",
        valueExpr: "Value",
        showBorders: true,
        searchEnabled: true,
        showSelectionControls: true,
        maxDisplayedTags: 2,
        showMultiTagOnly: false,
        onInitialized: function (e) {
            if ($scope.MajorCustomFieldId != null) {
                TravelAdvertisementSrvc.GetCustomFields($scope.MajorCustomFieldId).then(function (data) {
                    debugger;
                    $scope.CustomFieldList = data.data;
                });
            }
        },
        onValueChanged: function (e) {
            debugger;
            $scope.CustomFieldId = e.value.join(',');
            //if ($scope.MajorCustomFieldId != null) {
            //    TravelAdvertisementSrvc.GetCustomFields($scope.MajorCustomFieldId).then(function (data) {
            //        debugger;
            //        $scope.CustomFieldList = data.data;
            //    });
            //}
        }//,
        //onClosed: function (e) {
        //    debugger;
        //    $scope.CustomFieldId = e.value;
        //}
    };

        $scope.PurposeSelectBox = {
            bindingOptions: {
                dataSource: "PurposeList",
                value: "PurposeID",
                items: "PurposeList"
            },

            placeholder: "--أختر--",
            noDataText: "لا يوجد بيانات",
            paginate: true, //Pagenation
            showClearButton: true,
            searchEnabled: true,
            displayExpr: "Text",
            valueExpr: "Value",
            disabled: false,

            onInitialized: function (e) {
                TravelAdvertisementSrvc.GetPurpose().then(function (data) {
                    $scope.PurposeList = data.data;
                });
            },
            onValueChanged: function (e) {
                $scope.PurposeID = e.value;
            }
        };

        $scope.AgentSelectBox = {
            bindingOptions: {
                dataSource: "AgentList",
                value: "AgentID",
                items: "AgentList"
            },

            placeholder: "--أختر--",
            noDataText: "لا يوجد بيانات",
            paginate: true, //Pagenation
            showClearButton: true,
            searchEnabled: true,
            displayExpr: "Text",
            valueExpr: "Value",
            disabled: false,

            onInitialized: function (e) {
                debugger;
                TravelAdvertisementSrvc.GetTravelAgent().then(function (data) {
                    $scope.AgentList = data.data;
                });
            },
            onValueChanged: function (e) {
                $scope.AgentID = e.value;
            }
        };

        //Date
        $scope.AdDateFrom = {
            bindingOptions: {
                value: "AdDateFromDate",
                min: "ValidateMinAdDateFromDate"
            },
            type: "date",

            onValueChanged: function (e) {
                $scope.AdDateFromDate = e.value;
                if ($scope.id === 0) {
                    $scope.AdDateToDate = null;
                }
                var minDate = e.value.getMonth() + 1 + '/' + Number(e.value.getDate() + 1) + '/' + e.value.getFullYear();
                $scope.ValidateMinAdDateToDate = new Date(minDate);

            }
        };

        $scope.AdDateTo = {
            bindingOptions: {
                value: "AdDateToDate",
                min: "ValidateMinAdDateToDate",
            },
            type: "date",

            onValueChanged: function (e) {
                $scope.AdDateToDate = e.value;
                if ($scope.id == 0 || $scope.id == null) {
                    $scope.DepartingStartFromDate = null;
                }
                var minDate = e.value.getMonth() + 1 + '/' + Number(e.value.getDate() + 1) + '/' + e.value.getFullYear();
                $scope.ValidateMinDepartingStartFromDate = new Date(minDate);

            }
        };

        $scope.DepartingStartFrom = {
            bindingOptions: {
                value: "DepartingStartFromDate",
                min: "ValidateMinDepartingStartFromDate"
            },
            type: "date",

            onValueChanged: function (e) {
                $scope.DepartingStartFromDate = e.value;
                if ($scope.id == 0 || $scope.id == null) {
                    $scope.DepartingStartToDate = null;
                }
                var minDate = e.value.getMonth() + 1 + '/' + Number(e.value.getDate() + 1) + '/' + e.value.getFullYear();
                $scope.ValidateMinDepartingStartToDate = new Date(minDate);
                $scope.ValidateMinDepartingEndFromDate = new Date(minDate);

            }
        };

        $scope.DepartingStartTo = {
            bindingOptions: {
                value: "DepartingStartToDate",
                min: "ValidateMinDepartingStartToDate"
            },
            type: "date",

            onValueChanged: function (e) {
                $scope.DepartingStartToDate = e.value;

                var minDate = e.value.getMonth() + 1 + '/' + Number(e.value.getDate() + 1) + '/' + e.value.getFullYear();
                $scope.ValidateMinDepartingEndFromDate = new Date(minDate);
            }
        };

        $scope.DepartingEndFrom = {
            bindingOptions: {
                value: "DepartingEndFromDate",
                min: "ValidateMinDepartingEndFromDate"
            },
            type: "date",
            onValueChanged: function (e) {
                debugger;
                $scope.DepartingEndFromDate = e.value;
                if ($scope.id == 0 || $scope.id == null) {
                    $scope.DepartingEndToDate = null;
                    var minDate = e.value.getMonth() + 1 + '/' + Number(e.value.getDate() + 1) + '/' + e.value.getFullYear();
                    $scope.ValidateMinDepartingEndToDate = new Date(minDate);
                }
            }
        };

        $scope.DepartingEndTo = {
            bindingOptions: {
                value: "DepartingEndToDate",
                min: "ValidateMinDepartingEndToDate"
            },
            type: "date",

            onValueChanged: function (e) {
                debugger;
                $scope.DepartingEndToDate = e.value;
            }
        };

        //MultiTag 
        $scope.NationalityMultiTag = {
            bindingOptions: {
                value: "NationalityId",
                items: "NationalityList"
            },

            placeholder: "--أختر--",
            noDataText: "لا يوجد بيانات",
            paginate: true, //Pagenation
            displayExpr: "Text",
            valueExpr: "Value",
            showBorders: true,
            searchEnabled: true,
            showSelectionControls: true,
            maxDisplayedTags: 2,
            showMultiTagOnly: false,

            onInitialized: function (e) {
                TravelAdvertisementSrvc.GetNationalities().then(function (data) {
                    $scope.NationalityList = data.data;
                });
            },
            onValueChanged: function (e) {
                debugger;
                $scope.NationalityId = e.value;
                $scope.NationalityIds = e.value.join(',');
            }
        };

        $scope.DegreeMultiTag = {
            bindingOptions: {
                value: "DegreeId",
                items: "DegreeList"
            },

            placeholder: "--أختر--",
            noDataText: "لا يوجد بيانات",
            paginate: true, //Pagenation
            displayExpr: "Text",
            valueExpr: "Value",
            showBorders: true,
            searchEnabled: true,
            showSelectionControls: true,
            maxDisplayedTags: 2,
            showMultiTagOnly: false,

            onInitialized: function (e) {
                TravelAdvertisementSrvc.GetDegrees().then(function (data) {
                    $scope.DegreeList = data.data;
                });
            },
            onValueChanged: function (e) {
                $scope.DegreeId = e.value;
                $scope.DegreeIds = e.value.join(',');
            }
        };

        $scope.StStatusMultiTag = {
            bindingOptions: {
                value: "StStatusId",
                items: "StStatusList"
            },

            placeholder: "--أختر--",
            noDataText: "لا يوجد بيانات",
            paginate: true, //Pagenation
            displayExpr: "Text",
            valueExpr: "Value",
            showBorders: true,
            searchEnabled: true,
            showSelectionControls: true,
            maxDisplayedTags: 2,
            showMultiTagOnly: false,

            onInitialized: function (e) {
                TravelAdvertisementSrvc.GetStatus().then(function (data) {
                    $scope.StStatusList = data.data;
                });
            },
            onValueChanged: function (e) {
                $scope.StStatusId = e.value;
                $scope.StStatusIds = e.value.join(',');
            }
        };

        $scope.ScholarshipTypeMultiTag = {
            bindingOptions: {
                value: "ScholarshipTypeId",
                items: "ScholarshipTypeList"
            },

            placeholder: "--أختر--",
            noDataText: "لا يوجد بيانات",
            paginate: true, //Pagenation
            displayExpr: "Text",
            valueExpr: "Value",
            showBorders: true,
            searchEnabled: true,
            showSelectionControls: true,
            maxDisplayedTags: 2,
            showMultiTagOnly: false,

            onInitialized: function (e) {
                TravelAdvertisementSrvc.GetScholarships().then(function (data) {
                    $scope.ScholarshipTypeList = data.data;
                });
            },
            onValueChanged: function (e) {
                $scope.ScholarshipTypeId = e.value;
                $scope.ScholarshipTypeIds = e.value.join(',');
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
            useSubmitBehavior: true,
        };

        //Function
        $scope.SaveTravelAd = function () {
            debugger;
           
            TravelAdvertisementSrvc.SaveTravelAdvertisement({
                ID: $scope.id,
                IsAppearToExpectedGradutedPeople: $scope.MDL_IsAppearToExpectedGradutedPeople,
                CustomFieldId: $scope.CustomFieldId,
                AdName: $scope.AdName, TravelClass: $scope.LevelID, flightsType: $scope.TravelWay, PurposeID: $scope.PurposeID,
                AgentID: $scope.AgentID, AdStartDate: $scope.AdDateFromDate, AdEndDate: $scope.AdDateToDate,
                DepartingStart: $scope.DepartingStartFromDate, DepartingEnd: $scope.DepartingStartToDate,
                ReturningStart: $scope.TravelWay !== "O" ? $scope.DepartingEndFromDate : null, ReturningEnd: $scope.TravelWay !== "O" ? $scope.DepartingEndToDate : null,
                NationalityID: $scope.NationalityIds, DegreeID: $scope.DegreeIds, StStatusID: $scope.StStatusIds,
                ScholarshipType: $scope.ScholarshipTypeIds, Notes: $scope.Nots
            }).then(function (data) {
                debugger;
                if (data.data.status === 500) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });

                   $scope.MDL_IsAppearToExpectedGradutedPeople = false;
                }
                if (data.data.status === 200) {
                    debugger;
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    //Clear Data
                    //$scope.id = null;
                    //$scope.AdName = null;
                    //$scope.Nots = null;
                    //$scope.LevelID = null;
                    //$scope.PurposeID = null;
                    //$scope.AgentID = null;
                    //$scope.AdDateFromDate = null;
                    //$scope.AdDateToDate = null;
                    //$scope.DepartingStartFromDate = null;
                    //$scope.DepartingStartToDate = null;
                    //$scope.DepartingEndFromDate = null;
                    //$scope.DepartingEndToDate = null;
                    //$scope.NationalityId = null;
                    //$scope.NationalityIds = null;
                    //$scope.DegreeId = null;
                    //$scope.DegreeIds = null;
                    //$scope.StStatusId = null;
                    //$scope.StStatusIds = null;
                    //$scope.ScholarshipTypeId = null;
                    //$scope.ScholarshipTypeIds = null;
                    //$scope.TravelWay = 'R';

                    window.location.reload();
                }
            });
        }

   //Control Fild
    $scope.ConfigName = null;
        $scope.ConfigID = 0;
        $scope.ConfigKay = null;
        $scope.TravelConfigList = [];

        //popup Config 
        $scope.popupConfig = {
            width: 1100, 
            contentTemplate: 'information',
            showTitle: true,
            dragEnabled: false,
            shadingColor: "rgba(0, 0, 0, 0.69)",
            closeOnOutsideClick: false,
            bindingOptions: {
                visible: "popupConfigShow",
                title: "popupConfigTitle"
            },
            rtlEnabled: true
        };



        // Button Config  

        $scope.btnLevel = {
            icon: 'fa fa-plus',
            type: 'primary',
            onClick: function (e) {
                $scope.ConfigID = 0;
                $scope.TravelConfigList = [];
                $scope.ConfigKay = "Level";
                //GetTravelConfigByKay
                TravelAdvertisementSrvc.GetTravelConfigByKay($scope.ConfigKay).then(function (data) {
                    $scope.TravelConfigList = data.data;
                });
                //Show Popup
                //$("#popupConfig").show();
                $scope.popupConfigShow = true;
            }
        };

        $scope.btnPurpose = {
            icon: 'fa fa-plus',
            type: 'primary',
            onClick: function (e) {
                $scope.ConfigID = 0;
                $scope.TravelConfigList = [];
                $scope.ConfigKay = "purpose";
                //GetTravelConfigByKay
                TravelAdvertisementSrvc.GetTravelConfigByKay($scope.ConfigKay).then(function (data) {
                    $scope.TravelConfigList = data.data;
                });
                //Show Popup
                //$("#popupConfig").modal("show");
                $scope.popupConfigShow = true;
            }
        };

        $scope.btnAgent = {
            icon: 'fa fa-plus',
            type: 'primary',
            onClick: function (e) {
                $scope.ConfigID = 0;
                $scope.TravelConfigList = [];
                $scope.ConfigKay = "Agent";
                //GetTravelConfigByKay
                TravelAdvertisementSrvc.GetTravelConfigByKay($scope.ConfigKay).then(function (data) {
                    $scope.TravelConfigList = data.data;
                });
                //Show Popup
                //$("#popupConfig").modal("show");
                $scope.popupConfigShow = true;
            }
        };

        $scope.btnScholarship = {
            icon: 'fa fa-plus',
            type: 'primary',
            onClick: function (e) {
                $scope.ConfigID = 0;
                $scope.TravelConfigList = [];
                $scope.ConfigKay = "Scholarship";
                //GetTravelConfigByKay
                TravelAdvertisementSrvc.GetTravelConfigByKay($scope.ConfigKay).then(function (data) {
                    $scope.TravelConfigList = data.data;
                });
                //Show Popup
                //$("#popupConfig").modal("show");
                $scope.popupConfigShow = true;
            }
        };
    $scope.MDL_IsAppearToExpectedGradutedPeople = false;
    $scope.IsAppearToExpectedGradutedPeople = {
        bindingOptions: {
            value: "MDL_IsAppearToExpectedGradutedPeople"
        },
        onValueChanged: function (e) {
            $scope.MDL_IsAppearToExpectedGradutedPeople = e.value;
        }
    };
        //Control Config
        $scope.txtConfig = {
            bindingOptions: {
                value: "ConfigName"
            },
            placeholder: "الاسم",
            onValueChanged: function (e) {
                $scope.ConfigName = e.value;
            }
        }

    //Config  List
    $scope.gridTravelConfig = {
            bindingOptions: {
                dataSource: "TravelConfigList"
            },

            noDataText: "لا يوجد بيانات",
            selection: {
                mode: "single"
            },
            showBorders: true,
            paging: {
                pageSize: 5
            },
            pager: {
                showPageSizeSelector: true,
                allowedPageSizes: [5, 7, 10],
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
                    dataField: "Value",
                    cssClass: "text-right",
                },
                {
                    caption: "تعديل",
                    width: 200,
                    cssClass: "text-center",
                    cellTemplate: function (container, options) {

                        $("<div />").dxButton({
                            //icon: "fa fa-save",
                            text: "تعديل",
                            type: "warning",
                            hint: "تعديل",
                            elementAttr: { "class": "btn btn-warning" },

                            onClick: function (e) {
                                $scope.ConfigID = options.data.ID;
                                $scope.ConfigName = options.data.Value;
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
            }//,
            //onInitialized: function (e) {
            //}
        }

    $scope.btnSaveConfig = {
            text: 'حفظ',
            type: 'success',
            onClick: function (e) {
                debugger;
                TravelAdvertisementSrvc.SaveTravelConfig({ ID: $scope.ConfigID, Kay: $scope.ConfigKay, Value: $scope.ConfigName }).then(function (data) {
                    if (data.data.status == 500) {
                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    }
                    if (data.data.status == 200) {
                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                        $scope.ConfigName = null;

                        if ($scope.ConfigKay == "Level") {
                            TravelAdvertisementSrvc.GetLevels().then(function (data) {
                                $scope.LevelList = data.data;
                            });
                            //GetTravelConfigByKay
                            TravelAdvertisementSrvc.GetTravelConfigByKay($scope.ConfigKay).then(function (data) {
                                $scope.TravelConfigList = data.data;
                            });
                        }
                        if ($scope.ConfigKay == "purpose") {
                            TravelAdvertisementSrvc.GetPurpose().then(function (data) {
                                $scope.PurposeList = data.data;
                            });
                            //GetTravelConfigByKay
                            TravelAdvertisementSrvc.GetTravelConfigByKay($scope.ConfigKay).then(function (data) {
                                $scope.TravelConfigList = data.data;
                            });
                        }

                        if ($scope.ConfigKay == "Agent") {
                            TravelAdvertisementSrvc.GetTravelAgent().then(function (data) {
                                $scope.AgentList = data.data;
                            });
                            //GetTravelConfigByKay
                            TravelAdvertisementSrvc.GetTravelConfigByKay($scope.ConfigKay).then(function (data) {
                                $scope.TravelConfigList = data.data;
                            });
                        }

                        if ($scope.ConfigKay == "Scholarship") {
                            TravelAdvertisementSrvc.GetScholarships().then(function (data) {
                                $scope.ScholarshipTypeList = data.data;
                            });
                            //GetTravelConfigByKay
                            TravelAdvertisementSrvc.GetTravelConfigByKay($scope.ConfigKay).then(function (data) {
                                $scope.TravelConfigList = data.data;
                            });
                        }
                        
                    }
                });
            }
        };
        // Advertisement List
        //dataGrid
        $scope.gridAdvertisement = {
            bindingOptions: {
                dataSource: "AdvertisementList"
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
                    caption: "اسم الاعلان",
                    dataField: "AdName",
                    cssClass: "text-right",
                },
                {
                    caption: "بداية الاعلان",
                    dataField: "AdStartDate",
                    cssClass: "text-right",
                    dataType: 'date',
                    format: 'dd/MM/yyyy'
                },
                {
                    caption: "نهاية الاعلان",
                    dataField: "AdEndDate",
                    cssClass: "text-right",
                    dataType: 'date',
                    format: 'dd/MM/yyyy' 
                }, {
                    caption: "بدية السفر",
                    dataField: "DepartingStart",
                    cssClass: "text-right",
                    dataType: 'date',
                    format: 'dd/MM/yyyy' 
                }, {
                    caption: "نهاية السفر",
                    dataField: "DepartingEnd",
                    cssClass: "text-right",
                    dataType: 'date',
                    format: 'dd/MM/yyyy' 
                },
                {
                    caption: "بداية العودة",
                    dataField: "ReturningStart",
                    cssClass: "text-right",
                    dataType: 'date',
                    format: 'dd/MM/yyyy' 
                },
                {
                    caption: "نهاية العودة",
                    dataField: "ReturningEnd",
                    cssClass: "text-right",
                    dataType: 'date',
                    format: 'dd/MM/yyyy' 
                },
                {
                    caption: "المستخدم",
                    dataField: "User",
                    cssClass: "text-right",
                },
                {
                    caption: "الحالة",
                    dataField: "IsActive",
                    cssClass: "text-right",
                },
                {
                    caption: "تعديل",
                    width: 100,
                    cssClass: "text-center",
                    cellTemplate: function (container, options) {
                        $("<div />").dxButton({
                            //icon: "fa fa-save",
                            text: "تعديل",
                            type: "success",
                            hint: "تعديل",
                            elementAttr: { "class": "btn btn-success" },

                            onClick: function (e) {
                                location.href = "/TravelAdvertisement/Index?adID=" + options.data.ID;
                            }
                        }).appendTo(container);
                    }
                },
                {
                    caption: "تنشيط",
                    width: 100,
                    cssClass: "text-center",
                    cellTemplate: function (container, options) {
                        if (options.data.IsActive == true) {
                            $("<div />").dxButton({
                                //icon: "fa fa-save",
                                text: "ايقاف",
                                type: "danger",
                                hint: "ايقاف",
                                elementAttr: { "class": "btn btn-danger" },

                                onClick: function (e) {
                                    TravelAdvertisementSrvc.ActiveAdvertisement(options.data.ID).then(function (data) {
                                        if (data.data.status == 500) {

                                            swal("خطأ", data.data.Message, "error")
                                            // DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                        }
                                        if (data.data.status == 200) {

                                            swal("تم!", data.data.Message, "success")
                                            // DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                            //GetAdvertisement
                                            TravelAdvertisementSrvc.GetAdvertisement().then(function (data) {
                                                $scope.AdvertisementList = data.data;
                                            });
                                        }
                                    });
                                }
                            }).appendTo(container);
                        }
                        else {
                            $("<div />").dxButton({
                                //icon: "fa fa-save",
                                text: "تنشيط",
                                type: "warning",
                                hint: "تنشيط",
                                elementAttr: { "class": "btn btn-warning" },
                                onClick: function (e) {
                                    TravelAdvertisementSrvc.ActiveAdvertisement(options.data.ID).then(function (data) {
                                        if (data.data.status == 500) {

                                            swal("خطأ", data.data.Message, "error")
                                         //   DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                        }
                                        if (data.data.status == 200) {
                                            swal("تم!", data.data.Message, "success")
                                            // DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                            //GetAdvertisement
                                            TravelAdvertisementSrvc.GetAdvertisement().then(function (data) {
                                                $scope.AdvertisementList = data.data;
                                            });
                                        }
                                    });
                                }
                            }).appendTo(container);
                        }
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
                TravelAdvertisementSrvc.GetAdvertisement().then(function (data) {
                    $scope.AdvertisementList = data.data;
                });
            }
    }
    


    }]);
