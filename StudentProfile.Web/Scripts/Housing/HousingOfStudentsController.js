(function () {
    app.controller("HousingOfStudentsCtrl", ["$scope", "HousingOfStudentsSrvc", function ($scope, HousingOfStudentsSrvc) {

        /*********************************** Permissions *********************************/
        $scope.Permissions = {
            View: {
                Housing: false,
                Replacing: false,
                DeliveryTo: false,
                Transfering: false,
                DeliveryFrom: false,
                ClearanceFrom: false
            }
        };
        $scope.GetPermssions = function () {
            HousingOfStudentsSrvc.GetAllPermssions().then(function (data) {
                $scope.Permissions.View.Housing = data.data.Housing;
                $scope.Permissions.View.Replacing = data.data.Replacing;
                $scope.Permissions.View.DeliveryTo = data.data.DeliveryTo;
                $scope.Permissions.View.Transfering = data.data.Transfering;
                $scope.Permissions.View.DeliveryFrom = data.data.DeliveryFrom;
                $scope.Permissions.View.ClearanceFrom = data.data.ClearanceFrom;
            });
        };
        $scope.GetPermssions();
        /*--------------------------------* Permissions *--------------------------------*/


        // Formating Date
        let date = new Date();
        const dateFormated = date.getMonth() + 1 + '/' + date.getDate() + '/' + date.getFullYear();


        // Initailize Variables
        const Config = [
            { Id: 1, Text: "Housing" },
            { Id: 2, Text: "Transfering" },
            { Id: 3, Text: "Replacement" }
        ];

        $scope.InitialVariables = function () {

            $scope.TabPopUpWidth = '';
            $scope.TabPopUpHeight = '';
            $scope.TabPopUpTitle = '';
            $scope.tabDisabled = false;
            $scope.TabPopUpShow = false;
            $scope.PageIsEditing = true;

            $scope.MDL_HousingDate = '';
            $scope.MDL_ReceivingDate = '';
            $scope.ContentTemplateForTabPopup = '';
            $scope.TA_HousingNotes_ReadOnly = true;
            $scope.DB_HousingDateMeladi_Disabled = true;
            $scope.btnSaveHousingOfStudentsVisible = true;

            $scope.HousingFurnitureGridInstance = '';
            $scope.DataSourceHousingFurnitureGrid = [];
            $scope.DataSourceHousingFurnitureGridTemp = [];
            $scope.OldDataLength = 0;
        };
        $scope.InitialVariables();


        // MenuHandling
        const Menu = [
            { key: "StudentHousing", text: "تسكين الطالب" },
            { key: "FurnitureDeliveryTo", text: "تسليم العهد إلي الطالب" },
            { key: "FurnitureDeliveryFrom", text: "استلام العهد من الطالب" },
            { key: "StudentTransfering", text: "نقل الطالب" },
            { key: "StudentReplacement", text: "تبديل السكن مع طالب اخر" },
            { key: "StudentHousingClearance", text: "اخلاء طرف طالب" }

        ];



        // DataSources
        $scope.DataSourceHousingTypes = [{ HousingTypeId: 1, HousingTypeName: 'فردي' }, { HousingTypeId: 2, HousingTypeName: 'عائلي' }];

        var DataSourceStudents = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: true,
            key: "STUDENT_ID",
            loadMode: "raw",
            load: function () {
                return $.getJSON("/Housing/GetAllStudents", function (data) { });
            }
        });
        DataSourceStudents.reload();

        var DataSourceStudents_Instance = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: false,
            key: "STUDENT_ID",
            loadMode: "raw",
            load: function () {

                return $.getJSON("/Housing/GetAllHousingStudents/" + $scope.MDL_StudentId, function (data) { });
            }
        });


        var DataSourceComs = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: false,
            key: "ComId",
            loadMode: "raw",
            load: function () {

                if ($scope.MDL_HousingTypeId !== '' && $scope.MDL_HousingTypeId !== null && $scope.MDL_HousingTypeId !== undefined) {
                    return $.getJSON("/Housing/GetALLComsBasedOnIsFamilialFlag", { IsFamilial: $scope.MDL_HousingTypeId === 1 ? false : true }, function (data) { });
                }
                $scope.MDL_ComId = '';
                return [];

            }
        });
        var DataSourceComs_Instance = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: false,
            key: "ComId",
            loadMode: "raw",
            load: function () {

                if ($scope.MDL_HousingTypeId_Instance !== '' && $scope.MDL_HousingTypeId_Instance !== null && $scope.MDL_HousingTypeId_Instance !== undefined) {
                    return $.getJSON("/Housing/GetALLComsBasedOnIsFamilialFlag", { IsFamilial: $scope.MDL_HousingTypeId_Instance === 1 ? false : true }, function (data) { });
                }
                $scope.MDL_ComId_Instance = '';
                return [];

            }
        });


        var DataSourceUnits = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: false,
            key: "UnitId",
            loadMode: "raw",
            load: function () {

                if ($scope.MDL_ComId !== '' && $scope.MDL_ComId !== null && $scope.MDL_HousingTypeId !== '' && $scope.MDL_HousingTypeId !== null) {
                    return $.getJSON("/Housing/GetALLUnitsBasedOnCom", { IsFamilial: $scope.MDL_HousingTypeId === 1 ? false : true, ComId: $scope.MDL_ComId }, function (data) { });
                }
                $scope.MDL_UnitId = '';
                return [];
            }
        });

        var DataSourceUnits_Instance = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: false,
            key: "UnitId",
            loadMode: "raw",
            load: function () {

                if ($scope.MDL_ComId_Instance !== '' && $scope.MDL_ComId_Instance !== null && $scope.MDL_HousingTypeId_Instance !== '' && $scope.MDL_HousingTypeId_Instance !== null) {
                    return $.getJSON("/Housing/GetALLUnitsBasedOnCom", { IsFamilial: $scope.MDL_HousingTypeId_Instance === 1 ? false : true, ComId: $scope.MDL_ComId_Instance }, function (data) { });
                }
                $scope.MDL_UnitId_Instance = '';
                return [];
            }
        });


        var DataSourceFloors = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: false,
            key: "FloorId",
            loadMode: "raw",
            load: function () {

                if ($scope.MDL_UnitId !== '' && $scope.MDL_UnitId !== null && $scope.MDL_ComId !== '' && $scope.MDL_ComId !== null && $scope.MDL_HousingTypeId !== '' && $scope.MDL_HousingTypeId !== null) {
                    return $.getJSON("/Housing/GetALLFloorsBasedOnUnit", { IsFamilial: $scope.MDL_HousingTypeId === 1 ? false : true, ComId: $scope.MDL_ComId, UnitId: $scope.MDL_UnitId }, function (data) { });
                }
                $scope.MDL_FloorId = '';
                return [];
            }
        });
        var DataSourceFloors_Instance = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: false,
            key: "FloorId",
            loadMode: "raw",
            load: function () {

                if ($scope.MDL_UnitId_Instance !== '' && $scope.MDL_UnitId_Instance !== null && $scope.MDL_ComId_Instance !== '' && $scope.MDL_ComId_Instance !== null && $scope.MDL_HousingTypeId_Instance !== '' && $scope.MDL_HousingTypeId_Instance !== null) {
                    return $.getJSON("/Housing/GetALLFloorsBasedOnUnit", { IsFamilial: $scope.MDL_HousingTypeId_Instance === 1 ? false : true, ComId: $scope.MDL_ComId_Instance, UnitId: $scope.MDL_UnitId_Instance }, function (data) { });
                }
                $scope.MDL_FloorId_Instance = '';
                return [];
            }
        });


        var DataSourceRooms = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: false,
            key: "RoomId",
            loadMode: "raw",
            load: function () {

                if ($scope.PageIsEditing === true) {
                    debugger;
                    if ($scope.MDL_RoomId !== '' && $scope.MDL_RoomId !== null && $scope.MDL_RoomId !== undefined) {
                        return [{ RoomId: $scope.MDL_RoomId, RoomName: $scope.MDL_RoomName }];
                    } else {
                        return [];
                    }
                } else {
                    if ($scope.MDL_FloorId !== '' && $scope.MDL_FloorId !== null && $scope.MDL_UnitId !== '' && $scope.MDL_UnitId !== null && $scope.MDL_ComId !== '' && $scope.MDL_ComId !== null && $scope.MDL_HousingTypeId !== '' && $scope.MDL_HousingTypeId !== null) {
                        return $.getJSON("/Housing/GetALLRoomsBasedOnFloor", { IsFamilial: $scope.MDL_HousingTypeId === 1 ? false : true, ComId: $scope.MDL_ComId, UnitId: $scope.MDL_UnitId, FloorId: $scope.MDL_FloorId }, function (data) { });
                    }
                    $scope.MDL_RoomId = '';
                    return [];
                }

            }
        });
        var DataSourceRooms_Instance = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: false,
            key: "RoomId",
            loadMode: "raw",
            load: function () {

                if ($scope.PageIsEditing === true) {
                    debugger;
                    if ($scope.MDL_RoomId_Instance !== '' && $scope.MDL_RoomId_Instance !== null && $scope.MDL_RoomId_Instance !== undefined) {
                        return [{ RoomId: $scope.MDL_RoomId_Instance, RoomName: $scope.MDL_RoomName_Instance }];
                    } else {
                        return [];
                    }
                } else {
                    if ($scope.MDL_FloorId_Instance !== '' && $scope.MDL_FloorId_Instance !== null && $scope.MDL_UnitId_Instance !== '' && $scope.MDL_UnitId_Instance !== null && $scope.MDL_ComId_Instance !== '' && $scope.MDL_ComId_Instance !== null && $scope.MDL_HousingTypeId_Instance !== '' && $scope.MDL_HousingTypeId_Instance !== null) {
                        return $.getJSON("/Housing/GetALLRoomsBasedOnFloor", { IsFamilial: $scope.MDL_HousingTypeId_Instance === 1 ? false : true, ComId: $scope.MDL_ComId_Instance, UnitId: $scope.MDL_UnitId_Instance, FloorId: $scope.MDL_FloorId_Instance }, function () { });
                    }
                    $scope.MDL_RoomId_Instance = '';
                    return [];
                }
            }
        });


        var DataSourceBeds = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: false,
            key: "BedLocationId",
            loadMode: "raw",
            load: function () {

                if ($scope.PageIsEditing === true) {
                    debugger;
                    if ($scope.MDL_BedLocationId !== '' && $scope.MDL_BedLocationId !== null && $scope.MDL_HousingTypeId === 1) {
                        return [{ BedLocationId: $scope.MDL_BedLocationId, BedLocationCode: 'Location ' + $scope.MDL_BedLocationCode }];
                    } else {
                        return [];
                    }
                } else {
                    if ($scope.MDL_RoomId !== '' && $scope.MDL_RoomId !== null) {
                        return $.getJSON("/Housing/GetALLBedsBasedOnRoomId", { RoomId: $scope.MDL_RoomId }, function (data) { });
                    }
                    $scope.MDL_BedLocationId = '';
                    return [];
                }

            }
        });
        var DataSourceBeds_Instance = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: false,
            key: "BedLocationId",
            loadMode: "raw",
            load: function () {

                if ($scope.PageIsEditing === true) {
                    if ($scope.MDL_BedLocationId_Instance !== '' && $scope.MDL_BedLocationId_Instance !== null && $scope.MDL_HousingTypeId_Instance === 1) {
                        return [{ BedLocationId: $scope.MDL_BedLocationId_Instance, BedLocationCode: 'Location ' + $scope.MDL_BedLocationCode_Instance }];
                    } else {
                        return [];
                    }
                } else {
                    if ($scope.MDL_RoomId_Instance !== '' && $scope.MDL_RoomId_Instance !== null) {
                        return $.getJSON("/Housing/GetALLBedsBasedOnRoomId", { RoomId: $scope.MDL_RoomId_Instance }, function (data) { });
                    }
                    $scope.MDL_BedLocationId_Instance = '';
                    return [];
                }
            }
        });


        var DataSourceCategories = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: false,
            key: "CategoryId",
            loadMode: "raw",
            load: function () {

                return $.getJSON("/Housing/GetALLCategories", function (data) { });

            }
        });


        var DataSourceFurnitures = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: false,
            key: "FurnitureId",
            loadMode: "raw",
            load: function () {

                if ($scope.MDL_CategoryId !== '' && $scope.MDL_CategoryId !== null) {
                    return $.getJSON("/Housing/GetALLFurnituresBasedOnCategoryId", { CategoryId: $scope.MDL_CategoryId }, function (data) { });
                }
                $scope.MDL_FurnitureId = '';
                return [];
            }
        });

        //Controls
        $scope.UserLastUpdated = null;
        $scope.txt_UserLastUpdated = {
            bindingOptions: {
                value: "UserLastUpdated"
            },
            placeholder: "القائم بالعمل",
            rtlEnabled: true,
            readOnly: true
        };



        $scope.DL_Students = {
            dataSource: DataSourceStudents,
            bindingOptions: {
                value: "MDL_StudentId",
                readOnly: 'DL_Students_ReadOnly'
            },
            placeholder: 'بحث ...',
            noDataText: "لا يوجد بيانات",
            displayExpr: "STUDENT_NAME",
            valueExpr: "STUDENT_ID",
            showBorders: true,
            searchExpr: ['STUDENT_ID', 'NATIONAL_ID', 'STUDENT_NAME'],
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            onValueChanged: function (e) {

                debugger;
                // * ClearHomePage
                $scope.ClearHomePage_Fn();

                $scope.PageIsEditing = true;
                $scope.TA_HousingNotes_ReadOnly = true;
                $scope.DB_HousingDateMeladi_Disabled = true;

                if (e.value !== null && e.value !== '' && e.value !== undefined) {

                    // * Filling Personal Controls...
                    HousingOfStudentsSrvc.GetStudentDataByID(e.value).then(function (data) {
                        $scope.MDL_LevelName = data.data.LevelName;
                        $scope.MDL_DegreeName = data.data.DegreeName;
                        $scope.MDL_GenderName = data.data.GenderName;
                        $scope.MDL_StatusName = data.data.StatusName;
                        $scope.MDL_FacultiyName = data.data.FacultiyName;
                        $scope.MDL_MobileNumber = data.data.MobileNumber;
                        $scope.MDL_BirthCityName = data.data.BirthCityName;
                        $scope.MDL_StudyTypeName = data.data.StudyTypeName;
                        $scope.MDL_NationalityName = data.data.NationalityName;
                        $scope.MDL_BirthDate = $scope.Date_Format_Fn(data.data.BirthDate);

                    });

                    // * Filling Housing Controls...
                    HousingOfStudentsSrvc.GetHousingStudent($scope.MDL_StudentId).then(function (data) {
                        debugger;
                        if (data.data !== '' && data.data !== null && data.data !== undefined) {
                            if (data.data.BedLocationId !== null && data.data.BedLocationId !== undefined && data.data.BedLocationId !== '') {
                                debugger;
                                $scope.MDL_BedLocationId = data.data.BedLocationId;
                                $scope.MDL_BedLocationCode = data.data.BedLocationCode;

                                $scope.UserLastUpdated = data.data.LastUpdated;
                                $scope.MDL_HousingNotes = data.data.NotesOfHosing;
                                $scope.MDL_RoomId = data.data.LocationDetails.SiteId;
                                $scope.MDL_FloorId = data.data.LocationDetails.FloorId;
                                $scope.MDL_ComId = data.data.LocationDetails.CompanyId;
                                $scope.MDL_RoomName = data.data.LocationDetails.SiteName;
                                $scope.MDL_UnitId = data.data.LocationDetails.DepartmentId;
                                debugger;
                                $scope.MDL_HousingDate = $scope.Date_Format_Fn(data.data.HousingDate);
                                $scope.MDL_HousingDateMeladi = new Date($scope.MDL_HousingDate);
                                $scope.MDL_HousingTypeId = data.data.LocationDetails.IsFamilial === false ? 1 : 2;
                            }
                            //else
                            //    return swal("تنبيه", data.data, "warning");

                        }
                    });

                    // * Next Housing No...
                    HousingOfStudentsSrvc.GetNextHousingNo($scope.MDL_StudentId, 0).then(function (data) {
                        debugger;
                        $scope.MDL_HousingNumberAuto = data.data;
                    });
                }

            }
        };
        $scope.DL_Students_Instance = {
            dataSource: DataSourceStudents_Instance,
            bindingOptions: {
                value: "MDL_StudentId_Instance",
                readOnly: 'DL_Students_Instance_ReadOnly'

            },
            placeholder: 'بحث ...',
            noDataText: "لا يوجد بيانات",
            displayExpr: "STUDENT_NAME",
            valueExpr: "STUDENT_ID",
            showBorders: true,
            searchExpr: ['STUDENT_ID', 'NATIONAL_ID', 'STUDENT_NAME'],
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            onValueChanged: function (e) {
                debugger;
                if (e.value !== null && e.value !== '' && e.value !== undefined) {

                    $scope.PageIsEditing = true;
                    $scope.Fn_DisabledLocationsDetails(true);

                    // * Filling Personal Controls
                    HousingOfStudentsSrvc.GetStudentDataByID($scope.MDL_StudentId_Instance).then(function (data) {
                        $scope.MDL_LevelName_Instance = data.data.LevelName;
                        $scope.MDL_DegreeName_Instance = data.data.DegreeName;
                        $scope.MDL_GenderName_Instance = data.data.GenderName;
                        $scope.MDL_StatusName_Instance = data.data.StatusName;
                        $scope.MDL_FacultiyName_Instance = data.data.FacultiyName;
                        $scope.MDL_MobileNumber_Instance = data.data.MobileNumber;
                        $scope.MDL_StudyTypeName_Instance = data.data.StudyTypeName;
                        $scope.MDL_BirthCityName_Instance = data.data.BirthCityName;
                        $scope.MDL_NationalityName_Instance = data.data.NationalityName;
                        $scope.MDL_BirthDate_Instance = $scope.Date_Format_Fn(data.data.BirthDate);
                    });

                    // * Filling Housing Controls...
                    HousingOfStudentsSrvc.GetHousingStudent($scope.MDL_StudentId_Instance).then(function (data) {
                        debugger;
                        if (data.data !== '' && data.data !== null) {
                            $scope.MDL_BedLocationId_Instance = data.data.BedLocationId;
                            $scope.MDL_BedLocationCode_Instance = data.data.BedLocationCode;

                            $scope.UserLastUpdated = data.data.LastUpdated;
                            $scope.MDL_HousingNotes_Instance = data.data.NotesOfHosing;
                            $scope.MDL_RoomId_Instance = data.data.LocationDetails.SiteId;
                            $scope.MDL_FloorId_Instance = data.data.LocationDetails.FloorId;
                            $scope.MDL_ComId_Instance = data.data.LocationDetails.CompanyId;
                            $scope.MDL_RoomName_Instance = data.data.LocationDetails.SiteName;
                            $scope.MDL_UnitId_Instance = data.data.LocationDetails.DepartmentId;
                            $scope.MDL_HousingDateMeladi_Replacement = $scope.Date_Format_Fn(data.data.HousingDate);
                            //housingofstudentssrvc.tohijridate($scope.MDL_HousingDateMeladi_Replacement).then(function (date) {
                            //    debugger;
                            //    $scope.MDL_HousingDateHigri_Replacement = date.data;
                            //});
                            //$scope.MDL_HousingDate_Instance = $scope.Date_Format_Fn(data.data.HousingDate);
                            $scope.MDL_HousingTypeId_Instance = data.data.LocationDetails.IsFamilial === false ? 1 : 2;
                        }
                    });

                    // * Next Housing No...
                    HousingOfStudentsSrvc.GetNextHousingNo(e.value, 0).then(function (data) {
                        $scope.MDL_HousingNumberAuto_Instance = data.data;
                    });

                }
                else {

                    // * Clear Personal Controls...
                    $scope.MDL_LevelName_Instance = '';
                    $scope.MDL_BirthDate_Instance = '';
                    $scope.MDL_DegreeName_Instance = '';
                    $scope.MDL_GenderName_Instance = '';
                    $scope.MDL_StatusName_Instance = '';
                    $scope.MDL_FacultiyName_Instance = '';
                    $scope.MDL_MobileNumber_Instance = '';
                    $scope.MDL_BirthCityName_Instance = '';
                    $scope.MDL_StudyTypeName_Instance = '';
                    $scope.MDL_NationalityName_Instance = '';


                    // * Clear Housing Controls 
                    $scope.MDL_ComId_Instance = '';
                    $scope.MDL_UnitId_Instance = '';
                    $scope.MDL_RoomId_Instance = '';
                    $scope.MDL_FloorId_Instance = '';
                    $scope.MDL_RoomName_Instance = '';
                    $scope.MDL_HousingDate_Instance = '';
                    $scope.MDL_HousingNotes_Instance = '';
                    $scope.MDL_HousingTypeId_Instance = '';
                    $scope.MDL_HousingNumberAuto_Instance = '';

                    $scope.MDL_BedLocationId_Instance = '';
                    $scope.MDL_BedLocationCode_Instance = '';
                }
            }
        };



        $scope.TB_Facultiy = {
            bindingOptions: {
                value: "MDL_FacultiyName"
            },
            //placeholder: "اسم الكلية أو المعهد",
            rtlEnabled: true,
            readOnly: true
        };
        $scope.TB_Facultiy_Instance = {
            bindingOptions: {
                value: "MDL_FacultiyName_Instance"
            },
            //placeholder: "اسم الكلية أو المعهد",
            rtlEnabled: true,
            readOnly: true
        };



        $scope.TB_Nationality = {
            bindingOptions: {
                value: "MDL_NationalityName"
            },
            //placeholder: "الجنسية",
            rtlEnabled: true,
            readOnly: true
        };
        $scope.TB_Nationality_Instance = {
            bindingOptions: {
                value: "MDL_NationalityName_Instance"
            },
            //placeholder: "الجنسية",
            rtlEnabled: true,
            readOnly: true
        };



        $scope.TB_MobileNumber = {
            bindingOptions: {
                value: "MDL_MobileNumber"
            },
            //placeholder: "رقم الجوال",
            rtlEnabled: true,
            readOnly: true
        };
        $scope.TB_MobileNumber_Instance = {
            bindingOptions: {
                value: "MDL_MobileNumber_Instance"
            },
            //placeholder: "رقم الجوال",
            rtlEnabled: true,
            readOnly: true
        };



        $scope.TB_BirthCity = {
            bindingOptions: {
                value: "MDL_BirthCityName"
            },
            //placeholder: "محل الميلاد",
            rtlEnabled: true,
            readOnly: true
        };
        $scope.TB_BirthCity_Instance = {
            bindingOptions: {
                value: "MDL_BirthCityName_Instance"
            },
            //placeholder: "محل الميلاد",
            rtlEnabled: true,
            readOnly: true
        };



        $scope.TB_LevelName = {
            bindingOptions: {
                value: "MDL_LevelName"
            },
            //placeholder: "المستوي",
            rtlEnabled: true,
            readOnly: true
        };
        $scope.TB_LevelName_Instance = {
            bindingOptions: {
                value: "MDL_LevelName_Instance"
            },
            //placeholder: "المستوي",
            rtlEnabled: true,
            readOnly: true
        };



        $scope.TB_StatusName = {
            bindingOptions: {
                value: "MDL_StatusName"
            },
            //placeholder: "الحالة الأكاديمية",
            rtlEnabled: true,
            readOnly: true
        };
        $scope.TB_StatusName_Instance = {
            bindingOptions: {
                value: "MDL_StatusName_Instance"
            },
            //placeholder: "الحالة الأكاديمية",
            rtlEnabled: true,
            readOnly: true
        };



        $scope.TB_DegreeName = {
            bindingOptions: {
                value: "MDL_DegreeName"
            },
            //placeholder: "الدرجة العلمية",
            rtlEnabled: true,
            readOnly: true
        };
        $scope.TB_DegreeName_Instance = {
            bindingOptions: {
                value: "MDL_DegreeName_Instance"
            },
            //placeholder: "الدرجة العلمية",
            rtlEnabled: true,
            readOnly: true
        };



        $scope.TB_StudyTypeName = {
            bindingOptions: {
                value: "MDL_StudyTypeName"
            },
            //placeholder: "نوع الدراسة",
            rtlEnabled: true,
            readOnly: true
        };
        $scope.TB_StudyTypeName_Instance = {
            bindingOptions: {
                value: "MDL_StudyTypeName_Instance"
            },
            //placeholder: "نوع الدراسة",
            rtlEnabled: true,
            readOnly: true
        };



        $scope.TB_GenderName = {
            bindingOptions: {
                value: "MDL_GenderName"
            },
            //placeholder: "النوع",
            rtlEnabled: true,
            readOnly: true
        };
        $scope.TB_GenderName_Instance = {
            bindingOptions: {
                value: "MDL_GenderName_Instance"
            },
            //placeholder: "النوع",
            rtlEnabled: true,
            readOnly: true
        };

        //---------------------------------------- Start : Date -----------------------------------
        $scope.todayDate = new Date();
        $scope.todayHijriDate = null;
        $scope.MDL_HousingReceiveDateMeladi = new Date();
        $scope.MDL_HousingReceiveDateHijri = null;
        $scope.MDL_HousingReceiveFromDateMeladi = new Date();
        $scope.MDL_HousingReceiveFromDateHijri = null;
        $scope.MDL_HousingTransferingDateMeladi = new Date();
        $scope.MDL_HousingTransferingDateHijri = null;
        $scope.MDL_HousingDateMeladi = null;
        $scope.MDL_HousingDateHijri = null;
        $scope.MDL_HousingDateMeladi_Replacement = null;
        $scope.MDL_HousingDateHijri_Replacement = null;
        $scope.MDL_ReplacementDateMeladi = new Date();
        $scope.MDL_ReplacementDateHigri = null;

        $scope.DB_BirthDate = {
            bindingOptions: {
                value: "MDL_BirthDate"
            },
            type: "date",
            disabled: true,
            displayFormat: "dd/MM/yyyy"
        };
        $scope.DB_BirthDate_Instance = {
            bindingOptions: {
                value: "MDL_BirthDate_Instance"
            },
            disabled: true,
            type: "date",
            displayFormat: "dd/MM/yyyy"
        };
        
        $scope.DB_HousingDateMeladi = {
            bindingOptions: {
                value: "MDL_HousingDateMeladi",
                disabled: "DB_HousingDateMeladi_Disabled"
            },
            type: "date",
            readOnly: false,
            min: new Date(),
            displayFormat: "dd/MM/yyyy",
            onValueChanged: function (e) {
                debugger;
                if ($scope.MDL_HousingDateMeladi !== "") {
                    HousingOfStudentsSrvc.ToHijriDate($scope.MDL_HousingDateMeladi).then(function (date) {
                        debugger;
                        $scope.MDL_HousingDateHijri = date.data;
                    });
                } else {
                    $scope.MDL_HousingDateHijri = '';
                }
            },
            onInitialized: function (e) {
                debugger;
                if ($scope.MDL_HousingDateMeladi !== "") {
                    $scope.MDL_HousingDateMeladi = new Date();
                    HousingOfStudentsSrvc.ToHijriDate($scope.MDL_HousingDateMeladi).then(function (date) {
                        debugger;
                        $scope.MDL_HousingDateHijri = date.data;
                    });
                } else {
                    $scope.MDL_HousingDateHijri = '';
                }
            },
            onShowing: function (e) {
                debugger;
                if (!$scope.MDL_HousingDateMeladi)
                    $scope.MDL_HousingDateMeladi = new Date();
            }
        };
        $scope.TB_HousingDateHigri = {
            bindingOptions: {
                value: "MDL_HousingDateHijri"
            },
            //placeholder: "التاريخ الهجري",
            rtlEnabled: true,
            readOnly: true,
            onInitialized: function () {
            }
        };

        $scope.DB_HousingReceivedFromDateMeladi = {
            bindingOptions: {
                value: "MDL_HousingReceiveFromDateMeladi"
            },
            type: "date",
            readOnly: false,
            min: new Date(),
            displayFormat: "dd/MM/yyyy",
            onValueChanged: function (e) {
                debugger;
                if ($scope.MDL_HousingReceiveFromDateMeladi !== "") {
                    HousingOfStudentsSrvc.ToHijriDate($scope.MDL_HousingReceiveFromDateMeladi).then(function (date) {
                        debugger;
                        $scope.MDL_HousingReceiveFromDateHijri = date.data;
                    });
                } else {
                    $scope.MDL_HousingReceiveFromDateHijri = '';
                }
            },
            onInitialized: function (e) {
                debugger;
                $scope.MDL_HousingReceiveFromDateMeladi = new Date();
                if ($scope.MDL_HousingReceiveFromDateMeladi !== "") {
                    HousingOfStudentsSrvc.ToHijriDate($scope.MDL_HousingReceiveFromDateMeladi).then(function (date) {
                        debugger;
                        $scope.MDL_HousingReceiveFromDateHijri = date.data;
                    });
                } else {
                    $scope.MDL_HousingReceiveFromDateHijri = '';
                }
            },
            onShowing: function (e) {
                debugger;
                if (!$scope.MDL_HousingReceiveFromDateMeladi)
                    $scope.MDL_HousingReceiveFromDateMeladi = new Date();
            }
        };    
        $scope.TB_HousingReceivedFromDateHigri = {
            bindingOptions: {
                value: "MDL_HousingDateHijri"
            },
            //placeholder: "التاريخ الهجري",
            rtlEnabled: true,
            readOnly: true,
            onInitialized: function () {
            }
        };

        $scope.DB_HousingTransferingDateMeladi = {
            bindingOptions: {
                value: "MDL_HousingTransferingDateMeladi"
            },
            type: "date",
            readOnly: false,
            min: new Date(),
            displayFormat: "dd/MM/yyyy",
            onValueChanged: function (e) {
                debugger;
                if ($scope.MDL_HousingTransferingDateMeladi !== "") {
                    HousingOfStudentsSrvc.ToHijriDate($scope.MDL_HousingTransferingDateMeladi).then(function (date) {
                        debugger;
                        $scope.MDL_HousingTransferingDateHijri = date.data;
                    });
                } else {
                    $scope.MDL_HousingTransferingDateHijri = '';
                }
            },
            onInitialized: function (e) {
                debugger;
                $scope.MDL_HousingTransferingDateMeladi = new Date();
                if ($scope.MDL_HousingTransferingDateMeladi !== "") {
                    HousingOfStudentsSrvc.ToHijriDate($scope.MDL_HousingTransferingDateMeladi).then(function (date) {
                        debugger;
                        $scope.MDL_HousingTransferingDateHijri = date.data;
                    });
                } else {
                    $scope.MDL_HousingTransferingDateHijri = '';
                }
            },
            onShowing: function (e) {
                debugger;
                $scope.MDL_HousingTransferingDateMeladi = new Date();
            }
        };    
        $scope.TB_HousingTransferingDateHigri = {
            bindingOptions: {
                value: "MDL_HousingTransferingDateHijri"
            },
            //placeholder: "التاريخ الهجري",
            rtlEnabled: true,
            readOnly: true,
            onInitialized: function () {
            }
        };

        $scope.DB_HousingDateMeladi_Instance = {
            bindingOptions: {
                value: "MDL_HousingDate_Instance"
            },
            disabled: true,
            readOnly: true,
            value: new Date(),
            type: "date",
            displayFormat: "dd/MM/yyyy",
            onValueChanged: function (e) {
                HousingOfStudentsSrvc.ToHijriDate($scope.MDL_HousingDate_Instance).then(function (date) {
                    $scope.MDL_hijriDate_Instance = date.data;
                });

            },
            onInitialized: function (e) {
                $scope.MDL_HousingDate_Instance = new Date();
                if ($scope.MDL_HousingDate_Instance !== "") {
                    HousingOfStudentsSrvc.ToHijriDate($scope.todayDate).then(function (date) {
                        debugger;
                        $scope.todayHijriDate = date.data;
                    });
                } else {
                    $scope.todayHijriDate = '';
                }
            }

        };
        $scope.TB_HousingDateHigri_Instance = {
            bindingOptions: {
                value: "todayHijriDate"
            },
            //placeholder: "التاريخ الهجري",
            rtlEnabled: true,
            readOnly: true,
            onInitialized: function () {
            }
        };

        $scope.DB_HousingDateMeladi_Replacement = {
            bindingOptions: {
                value: "MDL_HousingDateMeladi_Replacement"
            },
            disabled: true,
            readOnly: true,
            value: new Date(),
            type: "date",
            displayFormat: "dd/MM/yyyy",
            onValueChanged: function (e) {
                HousingOfStudentsSrvc.ToHijriDate($scope.MDL_HousingDateMeladi_Replacement).then(function (date) {
                    $scope.MDL_ReplacementDateHigri = date.data;
                });

            },
            onInitialized: function (e) {
                $scope.MDL_HousingDate_Instance = null;
                if ($scope.MDL_HousingDate_Instance !== "") {
                    HousingOfStudentsSrvc.ToHijriDate($scope.MDL_HousingDateMeladi_Replacement).then(function (date) {
                        debugger;
                        $scope.MDL_ReplacementDateHigri = date.data;
                    });
                } else {
                    $scope.MDL_ReplacementDateHigri = '';
                }
            }

        };
        $scope.TB_HousingDateHigri_Replacement = {
            bindingOptions: {
                value: "MDL_ReplacementDateHigri"
            },
            //placeholder: "التاريخ الهجري",
            rtlEnabled: true,
            readOnly: true,
            onInitialized: function () {
            }
        };


        $scope.DB_ReceivingDateMeladi = {
            //bindingOptions: {
            //    value: "todayDate"
            //    //disabled: "DB_HousingDateMeladi_Disabled"
            //},
            value: new Date(),
            type: "date",
            disabled: true,

            displayFormat: "dd/MM/yyyy"
            ,oninitialized: function (e) {
                debugger;
                housingofstudentssrvc.tohijridate($scope.todayDate).then(function (date) {
                    debugger;
                    $scope.todayHijriDate = date.data;
                });
                
            }
        };
        $scope.TB_ReceivingDateHigri = {
            bindingOptions: {
                value: "todayHijriDate"
            },
            type: "date",

            //placeholder: "التاريخ الهجري",
            rtlEnabled: true,
            disabled: true
        };

        $scope.DB_ReceivingDateMeladi_Instance = {
            bindingOptions: {
                value: "todayDate"
            },
            disabled: true,
            type: "date",
            displayFormat: "dd/MM/yyyy",
            onInitialized: function (e) {
                $scope.MDL_ReceivingDate_Instance = new Date();
                if ($scope.MDL_ReceivingDate_Instance !== "") {
                    HousingOfStudentsSrvc.ToHijriDate($scope.MDL_ReceivingDate_Instance).then(function (date) {
                        debugger;
                        $scope.MDL_receivingHijriDate = date.data;
                    });
                } else {
                    $scope.MDL_receivingHijriDate = '';
                }
            }

        };
        $scope.TB_ReceivingDateHigri_Instance = {
            bindingOptions: {
                value: "todayHijriDate"
            },
            type: "date",

            //placeholder: "التاريخ الهجري",
            rtlEnabled: true,
            disabled: true
        };

        $scope.DB_ReplacementDateMeladi = {
            bindingOptions: {
                value: "MDL_ReplacementDateMeladi"
            },
            readOnly: true,
            type: "date",
            displayFormat: "dd/MM/yyyy",
            onValueChanged: function (e) {
                if (!Number.isNaN(new Date($scope.MDL_ReplacementDateMeladi).getTime())) {
                    HousingOfStudentsSrvc.ToHijriDate($scope.MDL_ReplacementDateMeladi).then(function (date) {
                        $scope.MDL_ReplacementDateHigri = date.data;
                    });
                } else {
                    $scope.MDL_ReplacementDateHigri = '';
                }
            },
            onInitialized: function () {
                $scope.MDL_ReplacementDateMeladi = new Date();
            }
        };
        $scope.TB_ReplacementDateHigri = {
            bindingOptions: {
                value: "MDL_ReplacementDateHigri"
            },
            //placeholder: "التاريخ الهجري",
            rtlEnabled: true,
            readOnly: true,
            onInitialized: function () {
            }
        };

        //------------------------------------------------ End : Date ------------------------------------

        $scope.TB_HousingNumberAuto = {
            bindingOptions: {
                value: "MDL_HousingNumberAuto"
            },
            //placeholder: "الحركة",
            rtlEnabled: true,
            readOnly: true,
            onInitialized: function () {
            }
        };
        $scope.TB_HousingNumberAuto_Instance = {
            bindingOptions: {
                value: "MDL_HousingNumberAuto_Instance"
            },
            //placeholder: "الحركة",
            rtlEnabled: true,
            readOnly: false,
            onInitialized: function () {
            }
        };



        $scope.TB_ReplacementNumberAuto = {
            bindingOptions: {
                value: "MDL_ReplacementNumberAuto"
            },
            //placeholder: "الحركة",
            rtlEnabled: true,
            readOnly: true,
            onInitialized: function () {
                HousingOfStudentsSrvc.GetNextHousingNo(0, $scope.FoundId_Fn("Replacement")).then(function (data) {
                    $scope.MDL_ReplacementNumberAuto = data.data;
                });
            }
        };



        $scope.TA_HousingNotes = {
            height: 80,
            bindingOptions: {
                value: "MDL_HousingNotes",
                readOnly: 'TA_HousingNotes_ReadOnly'
            }
        };
        $scope.TA_HousingNotes_Instance = {
            height: 80,
            bindingOptions: {
                value: "MDL_HousingNotes_Instance",
                readOnly: "TA_HousingNotes_ReadOnly_Instance"
            }
        };



        $scope.TA_ReplacementNotes_FirstSt = {
            height: 120,
            bindingOptions: {
                value: "MDL_ReplacementNotes_FirstSt"
            }
        };
        $scope.TA_ReplacementNotes_SecondSt = {
            height: 120,
            bindingOptions: {
                value: "MDL_ReplacementNotes_SecondSt"
            }
        };



        $scope.DL_HousingTypes = {
            bindingOptions: {
                value: "MDL_HousingTypeId",
                text: "HousingTypeName",
                items: "DataSourceHousingTypes",
                disabled: "DL_HousingTypesDisabled"
            },
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            displayExpr: "HousingTypeName",
            valueExpr: "HousingTypeId",
            showBorders: true,
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            onValueChanged: function (e) {
                if ($scope.PageIsEditing === true) {
                    DataSourceComs.reload();
                } else {
                    if (e.value === 2) { $scope.DL_BedsDisabled = true } else { $scope.DL_BedsDisabled = false }
                    $scope.MDL_ComId = '';
                    DataSourceComs.reload();
                }
            }
        };
        $scope.DL_HousingTypes_Instance = {
            bindingOptions: {
                value: "MDL_HousingTypeId_Instance",
                text: "HousingTypeName",
                items: "DataSourceHousingTypes",
                disabled: "DL_HousingTypesDisabled_Instance"
            },
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            displayExpr: "HousingTypeName",
            valueExpr: "HousingTypeId",
            showBorders: true,
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            onValueChanged: function (e) {

                if ($scope.PageIsEditing === true) {
                    if ($scope.ContentTemplateForTabPopup === 'StudentHousingTransferContent') {
                        $scope.PageIsEditing = false;
                    }
                    DataSourceComs_Instance.reload();
                } else {
                    if (e.value === 2) { $scope.DL_BedsDisabled_Instance = true } else { $scope.DL_BedsDisabled_Instance = false }
                    $scope.MDL_ComId_Instance = '';
                    DataSourceComs_Instance.reload();
                }
            }
        };



        $scope.DL_Categories = {
            dataSource: DataSourceCategories,
            bindingOptions: {
                value: "MDL_CategoryId",
                text: "MDL_CategoryText"
            },
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            displayExpr: "CategoryName",
            valueExpr: "CategoryId",
            showBorders: true,
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            onValueChanged: function (e) {
                $scope.MDL_FurnitureId = '';
                $scope.MDL_FurnitureText = '';
                DataSourceFurnitures.reload();
            }
        };

        

        $scope.DL_Furnitures = {
            dataSource: DataSourceFurnitures,
            bindingOptions: {
                value: "MDL_FurnitureId"

            },
            placeholder: "اختر",
            noDataText: "لا يوجد أثاث متاح حاليا",
            displayExpr: "FurnitureName",
            valueExpr: "FurnitureId",
            showBorders: true,
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            onValueChanged: function (e) {
                debugger;
                $scope.MDL_FurnitureId = e.value;
                $scope.MDL_FurnitureText = $("#DL_Furnitures").dxSelectBox('instance').option('text');
            },
            elementAttr: {
                id: "DL_Furnitures"
            }
        };


        $scope.DL_Beds = {
            dataSource: DataSourceBeds,
            bindingOptions: {
                value: "MDL_BedLocationId",
                disabled: "DL_BedsDisabled"
            },
            placeholder: "اختر",
            noDataText: "لا يوجد أسِرَّة متاحة",
            displayExpr: "BedLocationCode",
            valueExpr: "BedLocationId",
            showBorders: true,
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            onValueChanged: function (e) {
                $scope.MDL_BedLocationId = e.value;
                debugger;
            }
        };
        $scope.DL_Beds_Instance = {
            dataSource: DataSourceBeds_Instance,
            bindingOptions: {
                value: "MDL_BedLocationId_Instance",
                disabled: "DL_BedsDisabled_Instance"
            },
            placeholder: "اختر",
            noDataText: "لا يوجد أسِرَّة متاحة",
            displayExpr: "BedLocationCode",
            valueExpr: "BedLocationId",
            showBorders: true,
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            onValueChanged: function (e) {
                debugger;
                $scope.MDL_BedLocationId_Instance = e.value;
            }
        };

        $scope.DL_Coms = {
            dataSource: DataSourceComs,
            bindingOptions: {
                value: "MDL_ComId",
                disabled: "DL_ComsDisabled"
            },
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            displayExpr: "ComName",
            valueExpr: "ComId",
            showBorders: true,
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            onValueChanged: function (e) {

                if ($scope.PageIsEditing === true) {
                    DataSourceUnits.reload();
                } else {
                    $scope.MDL_UnitId = '';
                    DataSourceUnits.reload();
                }
            }
        };
        $scope.DL_Coms_Instance = {
            dataSource: DataSourceComs_Instance,
            bindingOptions: {
                value: "MDL_ComId_Instance",
                disabled: "DL_ComsDisabled_Instance"
            },
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            displayExpr: "ComName",
            valueExpr: "ComId",
            showBorders: true,
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            onValueChanged: function (e) {

                if ($scope.PageIsEditing === true) {
                    DataSourceUnits_Instance.reload();
                } else {
                    DataSourceUnits_Instance.reload();
                    $scope.MDL_UnitId_Instance = '';
                }
            }
        };



        $scope.DL_Units = {
            dataSource: DataSourceUnits,
            bindingOptions: {
                value: "MDL_UnitId",
                disabled: "DL_UnitsDisabled"
            },
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            displayExpr: "UnitName",
            valueExpr: "UnitId",
            showBorders: true,
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            onValueChanged: function (e) {
                if ($scope.PageIsEditing === true) {
                    DataSourceFloors.reload();
                } else {
                    $scope.MDL_FloorId = '';
                    DataSourceFloors.reload();
                }
            }
        };
        $scope.DL_Units_Instance = {
            dataSource: DataSourceUnits_Instance,
            bindingOptions: {
                value: "MDL_UnitId_Instance",
                disabled: "DL_UnitsDisabled_Instance"
            },
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            displayExpr: "UnitName",
            valueExpr: "UnitId",
            showBorders: true,
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            onValueChanged: function (e) {

                if ($scope.PageIsEditing === true) {
                    DataSourceFloors_Instance.reload();

                } else {
                    $scope.MDL_FloorId_Instance = '';
                    DataSourceFloors_Instance.reload();
                }

            }
        };



        $scope.DL_Floors = {
            dataSource: DataSourceFloors,
            bindingOptions: {
                value: "MDL_FloorId",
                disabled: "DL_FloorsDisabled"
            },
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            displayExpr: "FloorName",
            valueExpr: "FloorId",
            showBorders: true,
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            onValueChanged: function (e) {

                if ($scope.PageIsEditing === true) {
                    DataSourceRooms.reload();
                } else {
                    $scope.MDL_RoomId = '';
                    DataSourceRooms.reload();
                }
            }
        };
        $scope.DL_Floors_Instance = {
            dataSource: DataSourceFloors_Instance,
            bindingOptions: {
                value: "MDL_FloorId_Instance",
                disabled: "DL_FloorsDisabled_Instance"
            },
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            displayExpr: "FloorName",
            valueExpr: "FloorId",
            showBorders: true,
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            onValueChanged: function (e) {

                if ($scope.PageIsEditing === true) {
                    DataSourceRooms_Instance.reload();
                } else {
                    $scope.MDL_RoomId_Instance = '';
                    DataSourceRooms_Instance.reload();
                }
            }
        };



        $scope.DL_Rooms = {
            dataSource: DataSourceRooms,
            bindingOptions: {
                value: "MDL_RoomId",
                disabled: "DL_RoomsDisabled"
            },
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            displayExpr: "RoomName",
            valueExpr: "RoomId",
            showBorders: true,
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            onValueChanged: function (e) {

                if ($scope.PageIsEditing !== true) {
                    $scope.MDL_BedLocationId = '';
                }
                if ($scope.MDL_HousingTypeId === 1) { DataSourceBeds.reload(); }
            }
        };
        $scope.DL_Rooms_Instance = {
            dataSource: DataSourceRooms_Instance,
            bindingOptions: {
                value: "MDL_RoomId_Instance",
                disabled: "DL_RoomsDisabled_Instance"
            },
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            displayExpr: "RoomName",
            valueExpr: "RoomId",
            showBorders: true,
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            onValueChanged: function (e) {
                if ($scope.PageIsEditing !== true) {
                    $scope.MDL_BedLocationId_Instance = '';
                }
                if ($scope.MDL_HousingTypeId === 1) { DataSourceBeds_Instance.reload(); }
            }
        };
        //Clearance Students
        $scope.ClearanceNote = '';
        $scope.ClearanceDateMeladiValue = '';
        $scope.ClearanceDateHigriValue = '';

        $scope.listOptions = {
            
            dataSource: Menu,
            itemTemplate: function (data) {
                return $("<div> <i class='fa fa-long-arrow-left'> </i>" + data.text + "</div>");
            },
            
            onItemClick: function (e) {
                debugger;
                if (!isNaN(parseFloat($scope.MDL_StudentId))) {

                    if (e.itemData.key === "StudentHousing") {
                        debugger;

                        // Permissions...
                        if (!$scope.Permissions.View.Housing) {
                            return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه الشاشة", "warning");
                        }

                        // ClearHomePage...
                        $scope.ClearHomePage_Fn();

                        $scope.TabPopUpShow = true;
                        $scope.TabPopUpWidth = 1300;
                        $scope.TabPopUpTitle = 'حركات تسكين الطالب';
                        $scope.btnSaveHousingOfStudentsText = 'تسكين';
                        $scope.ContentTemplateForTabPopup = 'StudentHousingContent';
                        $scope.TabsPopupInstance.repaint();


                        HousingOfStudentsSrvc.GetHousingStudent($scope.MDL_StudentId).then(function (data) {
                            debugger;
                            if (data.data !== '' && data.data !== null && data.data !== undefined) {
                                if (data.data.BedLocationId !== null && data.data.BedLocationId !== '' && data.data.BedLocationId !== undefined) {

                                    // * Next Housing No...
                                    HousingOfStudentsSrvc.GetNextHousingNo($scope.MDL_StudentId, 0).then(function (data) {
                                        $scope.MDL_HousingNumberAuto = data.data;
                                    });

                                    // * Disable All Controls..
                                    $scope.PageIsEditing = true;
                                    $scope.TA_HousingNotes_ReadOnly = true;
                                    $scope.Fn_DisabledLocationsDetails(true);
                                    $scope.DB_HousingDateMeladi_Disabled = true;
                                    $scope.btnSaveHousingOfStudentsVisible = false;

                                    $scope.MDL_HousingNotes = data.data.NotesOfHosing;
                                    $scope.MDL_BedLocationId = data.data.BedLocationId;
                                    $scope.MDL_BedLocationCode = data.data.BedLocationCode;
                                    $scope.MDL_RoomId = data.data.LocationDetails.SiteId;
                                    $scope.MDL_HousingDate = $scope.Date_Format_Fn(data.data.HousingDate);
                                    $scope.MDL_HousingDateMeladi = new Date($scope.MDL_HousingDate);

                                    $scope.MDL_FloorId = data.data.LocationDetails.FloorId;
                                    $scope.MDL_ComId = data.data.LocationDetails.CompanyId;
                                    debugger;
                                    $scope.MDL_RoomName = data.data.LocationDetails.SiteName;
                                    $scope.MDL_UnitId = data.data.LocationDetails.DepartmentId;
                                    $scope.MDL_HousingTypeId = data.data.LocationDetails.IsFamilial === false ? 1 : 2;

                                    $scope.Fn_DisabledLocationsDetails(true);
                                    $scope.Fn_DisabledLocationsDetails_Instance(true);

                                }
                                else
                                {
                                    $scope.Fn_DisabledLocationsDetails(false);

                                }

                            }
                            else {
                                $scope.MDL_HousingDate = dateFormated;
                                $scope.DB_HousingDateMeladi_Disabled = true;
                                $scope.btnSaveHousingOfStudentsVisible = false;
                                $scope.DB_HousingDateMeladi = new Date();

                                // * NextHousingNo...
                                HousingOfStudentsSrvc.GetNextHousingNo(0, $scope.FoundId_Fn("Housing")).then(function (data) {
                                    $scope.MDL_HousingNumberAuto = data.data;
                                });
                            }
                        });
                    }

                    else if (e.itemData.key === "FurnitureDeliveryTo") {
                        // Permissions...
                        if (!$scope.Permissions.View.DeliveryTo) {
                            return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه الشاشة", "warning");
                        }

                        HousingOfStudentsSrvc.GetStudentHousingFurniture($scope.MDL_StudentId).then(function (data) {

                            if (data.data.BedLocationId !== null && data.data.BedLocationId !== undefined && data.data.BedLocationId !== '') {

                                // * ClearHomePage...
                                $scope.ClearHomePage_Fn();
                                $scope.deleteBtnShow = true;
                                $scope.editBtnShow = false;
                                $scope.PageIsEditing = true;
                                $scope.TabPopUpShow = true;
                                $scope.TabPopUpWidth = 1250;
                                $scope.TabPopUpTitle = 'تسليم العهد إلي الطالب';
                                $scope.btnSaveHousingOfStudentsText = 'تسليم العهد';
                                $scope.ContentTemplateForTabPopup = 'HousingDeliveryToStudentContent';
                                $scope.TabsPopupInstance.repaint();

                                // * Disable All Locations Controls
                                $scope.Fn_DisabledLocationsDetails(true);

                                // * Calling Categories Data Source
                                DataSourceCategories.reload();

                                // * Filling Location Detalis
                                $scope.MDL_BedLocationId = data.data.BedLocationId;
                                $scope.MDL_BedLocationCode = data.data.BedLocationCode;


                                $scope.MDL_RoomId = data.data.LocationDetails.SiteId;
                                $scope.MDL_FloorId = data.data.LocationDetails.FloorId;
                                $scope.MDL_ComId = data.data.LocationDetails.CompanyId;
                                $scope.MDL_RoomName = data.data.LocationDetails.SiteName;
                                $scope.MDL_UnitId = data.data.LocationDetails.DepartmentId;
                                $scope.MDL_HousingTypeId = data.data.LocationDetails.IsFamilial === false ? 1 : 2;

                                // * Filling Student Furnitures
                                if (data.data.StudentFurnitures.length > 0) {
                                    debugger;
                                    debugger;
                                    for (var i = 0; i < data.data.StudentFurnitures.length; i++) {

                                        $scope.MDL_ReceivingDate = data.data.StudentFurnitures[i].ReceivingDate;
                                        $scope.DataSourceHousingFurnitureGrid.push(data.data.StudentFurnitures[i]);
                                    }
                                    $scope.DataSourceHousingFurnitureGridTemp = $scope.DataSourceHousingFurnitureGrid;
                                    $scope.OldDataLength = $scope.DataSourceHousingFurnitureGrid.length;
                                    ////////////////////////////////////////
                                    $scope.deleteBtnShow = true;

                                }
                                else
                                    $scope.deleteBtnShow = true;


                            } else { swal("تنبيه", "عفوا هذا الطالب لم يتم تسكينه حتي الآن برجاء تسكين الطالب أولا", "warning"); }

                        });
                    }
                    else if (e.itemData.key === "FurnitureDeliveryFrom") {
                        // Permissions
                        if (!$scope.Permissions.View.DeliveryFrom) {
                            return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه الشاشة", "warning");
                        }


                        HousingOfStudentsSrvc.GetStudentHousingFurniture($scope.MDL_StudentId).then(function (data) {

                            if (data.data.BedLocationId !== null && data.data.BedLocationId !== '' && data.data.BedLocationId !== undefined) {
                                if (data.data.StudentFurnitures.length !== undefined && data.data.StudentFurnitures.length > 0) {

                                    // * ClearHomePage
                                    $scope.ClearHomePage_Fn();
                                    $scope.deleteBtnShow = false;
                                    $scope.editBtnShow = true;
                                    $scope.PageIsEditing = true;
                                    $scope.TabPopUpShow = true;
                                    $scope.TabPopUpWidth = 1250;
                                    $scope.TabPopUpTitle = 'استلام العهد من الطالب';
                                    $scope.ContentTemplateForTabPopup = 'HousingDeliveryFromStudentContent';
                                    $scope.TabsPopupInstance.repaint();

                                    // * Disable All Locations Controls
                                    $scope.Fn_DisabledLocationsDetails(true);

                                    // * Filling Location Detalis
                                    $scope.MDL_BedLocationId = data.data.BedLocationId;
                                    $scope.MDL_BedLocationCode = data.data.BedLocationCode;
                                    $scope.MDL_RoomId = data.data.LocationDetails.SiteId;
                                    $scope.MDL_FloorId = data.data.LocationDetails.FloorId;
                                    $scope.MDL_ComId = data.data.LocationDetails.CompanyId;
                                    $scope.MDL_RoomName = data.data.LocationDetails.SiteName;
                                    $scope.MDL_UnitId = data.data.LocationDetails.DepartmentId;
                                    $scope.MDL_HousingTypeId = data.data.LocationDetails.IsFamilial === false ? 1 : 2;

                                    // * Filling Student Furnitures
                                    for (var i = 0; i < data.data.StudentFurnitures.length; i++) {
                                        $scope.DataSourceHousingFurnitureGrid.push(data.data.StudentFurnitures[i]);
                                    }

                                } else { swal("تنبيه", "عفوا هذا الطالب لم يتم تسليمه أي عهد حتي الآن برجاء إسناد العهد للطالب أولا", "warning"); }
                            } else { swal("تنبيه", "عفوا هذا الطالب لم يتم تسكينه حتي الآن برجاء تسكين الطالب أولا", "warning"); }

                        });
                    }
                    else if (e.itemData.key === "StudentTransfering") {
                        // Permissions
                        if (!$scope.Permissions.View.Transfering) {
                            return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه الشاشة", "warning");
                        }

                        HousingOfStudentsSrvc.GetHousingStudent($scope.MDL_StudentId).then(function (data) {
                            if (data.data.BedLocationId !== null && data.data.BedLocationId !== '' && data.data.BedLocationId !== undefined) {

                                // * ClearHomePage
                                $scope.ClearHomePage_Fn();

                                // * Disable All Locations And Housing Notes Controls
                                $scope.TA_HousingNotes_ReadOnly = true;
                                $scope.Fn_DisabledLocationsDetails(true);
                                $scope.DB_HousingDateMeladi_Disabled = true;

                                $scope.PageIsEditing = true;
                                $scope.TabPopUpShow = true; 
                                $scope.btnSaveHousingOfStudentsText = 'نقل';
                                $scope.btnSaveHousingOfStudentsVisible = true;
                                $scope.TabPopUpTitle = 'نقل الطالب من سكن لآخر';
                                $scope.ContentTemplateForTabPopup = 'StudentHousingTransferContent';
                                $scope.TabsPopupInstance.repaint();

                                // * Filling Housing Controls
                                $scope.MDL_BedLocationId = data.data.BedLocationId;
                                $scope.MDL_BedLocationCode = data.data.BedLocationCode;

                                $scope.UserLastUpdated = data.data.LastUpdated;
                                $scope.MDL_HousingNotes = data.data.NotesOfHosing;
                                $scope.MDL_RoomId = data.data.LocationDetails.SiteId;
                                $scope.MDL_FloorId = data.data.LocationDetails.FloorId;
                                $scope.MDL_ComId = data.data.LocationDetails.CompanyId;
                                $scope.MDL_RoomName = data.data.LocationDetails.SiteName;
                                $scope.MDL_UnitId = data.data.LocationDetails.DepartmentId;
                                $scope.MDL_HousingDate = $scope.Date_Format_Fn(data.data.HousingDate);
                                $scope.MDL_HousingTypeId = data.data.LocationDetails.IsFamilial === false ? 1 : 2;


                                // * Next Housing No
                                HousingOfStudentsSrvc.GetNextHousingNo($scope.MDL_StudentId, 0).then(function (data) {
                                    $scope.MDL_HousingNumberAuto = data.data;
                                });

                                // * Next Housing No For Instance
                                HousingOfStudentsSrvc.GetNextHousingNo(0, $scope.FoundId_Fn("Transfering")).then(function (data) {
                                    $scope.MDL_HousingNumberAuto_Instance = data.data;
                                });

                            } else { swal("تنبيه", data.data, "warning"); }
                        });
                    }
                    else if (e.itemData.key === "StudentReplacement") {
                        $scope.TabPopUpTitle = 'تبديل السكن مع طالب اخر';
                        $scope.btnShowPopUpReplacementNotes_Text = 'حفظ';
                        $scope.ContentTemplateForTabPopup = 'StudentHousingReplacementContent';
                        $scope.TabsPopupInstance.repaint();
                        // Permissions
                        if (!$scope.Permissions.View.Replacing) {
                            return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه الشاشة", "warning");
                        }

                        HousingOfStudentsSrvc.GetHousingStudent($scope.MDL_StudentId).then(function (data) {
                            if (data.data.BedLocationId !== null && data.data.BedLocationId !== '' && data.data.BedLocationId !== undefined) {
                                debugger;
                                // * ClearHomePage
                                $scope.ClearHomePage_Fn();

                                $scope.PageIsEditing = true;
                                $scope.TabPopUpShow = true; 
                                $scope.TabPopUpTitle = 'تبديل السكن مع طالب اخر';
                                $scope.btnShowPopUpReplacementNotes_Text = 'حفظ';
                                $scope.ContentTemplateForTabPopup = 'StudentHousingReplacementContent';
                                $scope.TabsPopupInstance.repaint();

                                // * Filling Housing Controls
                                $scope.MDL_BedLocationId = data.data.BedLocationId;
                                $scope.MDL_BedLocationCode = data.data.BedLocationCode;

                                $scope.UserLastUpdated = data.data.LastUpdated;
                                $scope.MDL_HousingNotes = data.data.NotesOfHosing;
                                $scope.MDL_RoomId = data.data.LocationDetails.SiteId;
                                $scope.MDL_FloorId = data.data.LocationDetails.FloorId;
                                $scope.MDL_ComId = data.data.LocationDetails.CompanyId;
                                $scope.MDL_RoomName = data.data.LocationDetails.SiteName;
                                $scope.MDL_UnitId = data.data.LocationDetails.DepartmentId;
                                $scope.MDL_HousingDate = $scope.Date_Format_Fn(data.data.HousingDate);
                                $scope.MDL_HousingTypeId = data.data.LocationDetails.IsFamilial === false ? 1 : 2;

                                // * Filling Personal Controls
                                HousingOfStudentsSrvc.GetStudentDataByID($scope.MDL_StudentId).then(function (data) {

                                    $scope.MDL_FacultiyName = data.data.FacultiyName;
                                    $scope.MDL_NationalityName = data.data.NationalityName;
                                    $scope.MDL_MobileNumber = data.data.MobileNumber;
                                    $scope.MDL_BirthCityName = data.data.BirthCityName;
                                    $scope.MDL_LevelName = data.data.LevelName;
                                    $scope.MDL_StatusName = data.data.StatusName;
                                    $scope.MDL_BirthDate = $scope.Date_Format_Fn(data.data.BirthDate);
                                    $scope.MDL_StudyTypeName = data.data.StudyTypeName;
                                    $scope.MDL_DegreeName = data.data.DegreeName;
                                    $scope.MDL_GenderName = data.data.GenderName;
                                });

                                // * Next Housing No
                                HousingOfStudentsSrvc.GetNextHousingNo($scope.MDL_StudentId, 0).then(function (data) {
                                    $scope.MDL_HousingNumberAuto = data.data;
                                });

                                // * Disable All Locations And Housing Notes Controls
                                $scope.DL_Students_ReadOnly = true;
                                $scope.TA_HousingNotes_ReadOnly = true;
                                $scope.Fn_DisabledLocationsDetails(true);
                                $scope.DB_HousingDateMeladi_Disabled = true;

                                // * Disable All Locations And Housing Notes For Instance Controls
                                $scope.TA_HousingNotes_ReadOnly_Instance = true;
                                $scope.Fn_DisabledLocationsDetails_Instance(true);
                                $scope.DB_HousingDateMeladi_Disabled_Instance = true;

                            } else { swal("تنبيه", data.data, "warning"); }
                        });
                    }

                    /******************** Clearance Of Sudents *********************/
                    else if (e.itemData.key === "StudentHousingClearance") {
                        debugger;
                        if (!$scope.Permissions.View.ClearanceFrom) {
                            return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه الشاشة", "warning");
                        }

                        HousingOfStudentsSrvc.CheckStudentHousingFurnitures($scope.MDL_StudentId).then(function (data) {
                            if (data.data.status === 500) {
                                debugger;
                                return swal("تنبيه", data.data.Message, "warning");
                            }
                            else {
                                debugger;

                                $scope.TabPopUpShow = true;
                                $scope.TabPopUpWidth = 1300;
                                $scope.TabPopUpTitle = 'اخلاء طرف طالب';
                                $scope.ContentTemplateForTabPopup = 'StudentHousingClearanceContent';
                                $scope.TabsPopupInstance.repaint();
                            }
                        });





                        $scope.txt_ClearanceNote = {
                            bindingOptions: {
                                value: "ClearanceNote"
                            },
                            placeholder: "ملاحظات اخلاء الطرف",
                            onValueChanged: function (e) {
                                $scope.ClearanceNote = e.value;
                            }
                        };

                        $scope.ClearanceDateMeladi = {
                            bindingOptions: {
                                value: "ClearanceDateMeladiValue"
                            },
                            readOnly: true,
                            type: "date",
                            displayFormat: "dd/MM/yyyy",
                            onValueChanged: function (e) {
                                if (!Number.isNaN(new Date($scope.ClearanceDateMeladiValue).getTime())) {
                                    HousingOfStudentsSrvc.ToHijriDate($scope.ClearanceDateMeladiValue).then(function (date) {
                                        $scope.ClearanceDateHigriValue = date.data;
                                    });
                                } else {
                                    $scope.MDL_hijriDate_Instance = '';
                                }
                            },
                            onInitialized: function (e) {
                                $scope.ClearanceDateMeladiValue = new Date();
                                if ($scope.MDL_HousingDate_Instance !== "") {
                                    HousingOfStudentsSrvc.ToHijriDate($scope.ClearanceDateMeladiValue).then(function (date) {
                                        debugger;
                                        $scope.MDL_hijriDate = date.data;
                                    });
                                } else {
                                    $scope.ClearanceDateHigriValue = '';
                                }
                            }

                        };

                        $scope.ClearanceDateHigri = {
                            bindingOptions: {
                                value: "ClearanceDateHigriValue"
                            },
                            //placeholder: "التاريخ الهجري",
                            rtlEnabled: true,
                            readOnly: true,
                            onInitialized: function () {
                            }
                        };

                        $scope.btnSaveClearance = {
                            text: 'اخلاء الطرف',
                            type: 'primary',
                            onClick: function (e) {

                                HousingOfStudentsSrvc.SaveClearanceOfSudents({ StudentId: $scope.MDL_StudentId, Note: $scope.ClearanceNote }).then(function (data) {
                                    if (data.data.status === 500) {
                                        debugger;

                                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                    }
                                    if (data.data.status === 200) {
                                        debugger;
                                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                        $scope.ReInitializeHomePage_Fn();
                                    }
                                });
                            }
                        };

                    }


                }
                else {
                    swal("تنبيه", "برجاء إختيار الطالب أولا", "warning");
                }
            }
        };


        $scope.AddUpdateHousingFurniture = function () {
            debugger;
            if ($scope.MDL_FurnitureId === null || $scope.MDL_FurnitureId === '' || $scope.MDL_FurnitureId < 0) {
                DevExpress.ui.notify({
                    message: "عفوا ادخل اسم قطعة الأثاث",
                    type: "error",
                    displayTime: 10000,
                    closeOnClick: true
                });
                return;
            }
            if ($scope.MDL_CategoryId === null || $scope.MDL_CategoryId === '' || $scope.MDL_CategoryId < 0) {
                DevExpress.ui.notify({
                    message: "عفوا ادخل اسم التصنيف",
                    type: "error",
                    displayTime: 10000,
                    closeOnClick: true
                });
                return;
            }
            if ($scope.DataSourceHousingFurnitureGrid.length < 1) {
                $scope.DataSourceHousingFurnitureGrid = [];
            }


            //فى حالة ادخال طرف جديد
            var duplicateRows = $scope.DataSourceHousingFurnitureGrid.filter(function (Row) {
                return Row.CategoryId === $scope.MDL_CategoryId && Row.FurnitureId === $scope.MDL_FurnitureId;
            });

            if (duplicateRows.length > 0) {
                DevExpress.ui.notify({
                    message: "عفوا لايمكن  تكرار  إدخال  نفس   قطعة الأثاث  على  نفس  التصنيف",
                    type: "error",
                    displayTime: 10000,
                    closeOnClick: true
                });
                return;
            }
            //$scope.deleteBtnShow = true;
            $scope.DataSourceHousingFurnitureGrid.push({
                HousingFurnituresOfStudent_Id: 0,
                CategoryId: $scope.MDL_CategoryId,
                CategoryName: $scope.MDL_CategoryText,
                FurnitureId: $scope.MDL_FurnitureId,
                FurnitureName: $scope.MDL_FurnitureText,
                LocationId: $scope.MDL_RoomId
            });
            $scope.clearFields();
        };


        $scope.clearFields = function () {
            $scope.MDL_CategoryId = '';
            $scope.MDL_FurnitureId = '';
        };

        /*--------------------------------* Start : Grid *--------------------------------*/

        $scope.HousingFurnitureGrid = {
            bindingOptions: {
                rtlEnabled: true,
                dataSource: "DataSourceHousingFurnitureGrid",
                "editing.allowUpdating": "editBtnShow"
                ,"editing.allowDeleting": "deleteBtnShow"

            },
            sorting: {
                mode: "multiple"
            },
            wordWrapEnabled: false,
            showBorders: false,
            searchPanel: {
                visible: false,
                placeholder: "بحث",
                width: 300
            },
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
            filterRow: {
                visible: false,
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
            width: 1000,
            columnChooser: {
                enabled: true
            },
            "export": {
                enabled: true,
                fileName: "HousingFurnitureGrid"
            },
            onCellPrepared: function (e) {

                debugger;
                if (e.rowType === "header" && e.column.command === "edit") {
                    e.column.width = 100;
                    e.column.alignment = "center";

                    if ($scope.ContentTemplateForTabPopup === 'HousingDeliveryFromStudentContent') {
                        e.cellElement.text(" استلام ");
                    } else {
                        e.cellElement.text(" حذف ");
                    }
                }

                
                if (e.rowType === "data" && e.column.command === "edit") {
                    $links = e.cellElement.find(".dx-link");
                    $links.text("");

                    if (e.row.data.HousingFurnituresOfStudent_Id === 0) {
                        $links.filter(".dx-link-delete").addClass("btn btn-danger btn-sm fa fa-trash-o");
                    }
                    else if ($scope.ContentTemplateForTabPopup === 'HousingDeliveryFromStudentContent') {
                        $links.filter(".dx-link-edit").addClass("btn btn-warning btn-d-edit btn-sm fa fa-lock");
                    }
                    if (e.row.data.HousingFurnituresOfStudent_Id !== 0)
                    {
                    debugger;

                        if (e.row.data !== null) {
                            let editLink = e.cellElement[0].querySelector(".dx-link-delete");
                            if (editLink) {
                                editLink.remove();
                            }
                        }
                    }
                }

                //if (e.rowType === "data" && e.column.command === "delete") {
                //    $links = e.cellElement.find(".dx-link");
                //    $links.text("");

                //    if (e.row.data.HousingFurnituresOfStudent_Id === 0) {
                //        $links.filter(".dx-link-delete").addClass("btn btn-danger btn-sm fa fa-trash-o");
                //    }
                //    else if ($scope.ContentTemplateForTabPopup === 'HousingDeliveryFromStudentContent') {
                //        $links.filter(".dx-link-edit").addClass("btn btn-warning btn-d-edit btn-sm fa fa-lock");
                //    }
                //}

                //if (e.rowType === 'data' && e.column.command === 'delete')
                //{

                //    if ($scope.DataSourceHousingFurnitureGrid.length === $scope.OldDataLength)
                //    {
                //    debugger;

                //        if (e.row.data !== null) {
                //            let editLink = e.cellElement.querySelector(".dx-link-delete");
                //            if (editLink) {
                //                editLink.remove();
                //            }
                //        }
                //    }
                //}
            },
            editing: {
                mode: "row",
                texts: {
                    confirmDeleteMessage: "هل متأكد من حذف العنصر؟",
                    deleteRow: "",
                    editRow: "",
                    addRow: ""
                },
                useIcons: true
            },
            columns: [
                {
                    caption: 'مسلسل',
                    width: 100,
                    alignment: "center",
                    cellTemplate: function (cellElement, cellInfo) {
                        cellElement.text(cellInfo.row.rowIndex + 1);
                    }
                },
                {
                    dataField: "CategoryId",
                    visible: false
                }, {
                    dataField: "CategoryName",
                    caption: "اسم التصنيف",
                    alignment: "center"
                },
                {
                    dataField: "FurnitureId",
                    visible: false
                },
                {
                    dataField: "FurnitureName",
                    caption: "اسم قطعة الأثاث",
                    alignment: "center"
                }
            ],
            allowColumnResizing: true,
            columnResizingMode: "widget",
            allowColumnReordering: true,
            showColumnLines: true,
            rowAlternationEnabled: true,
            onInitialized: function (e) {
                $scope.HousingFurnitureGridInstance = e.component;
            },
            onContentReady: function (e) {
                e.component.columnOption("command:delete",
                    {
                        visibleIndex: -1
                    });
            },
            onEditingStart: function (e) {

                var key = e.key;

                $scope.InternalPopupShow = true;
                $scope.InternalPopupWidth = 450;
                $scope.btnSaveHousingOfStudentsText = 'استلام العهدة';
                $scope.InternalPopupTitle = 'استلام الأصل ' + key.FurnitureName;
                $scope.HousingFurnituresOfStudent_Id = key.HousingFurnituresOfStudent_Id;
                $scope.ContentTemplateForInternalPopup = 'HousingDeliveryFromStudentContent-Return';

                e.cancel = true;
            }
        };

        /*********************************  Grid  ***********************************/

        /********************************** Save BTN *************************************** */
        // * Save Button
        $scope.btnSaveHousingOfStudents = {
            bindingOptions: {
                text: 'btnSaveHousingOfStudentsText',
                visible: 'btnSaveHousingOfStudentsVisible'
            },
            type: 'success',
            useSubmitBehavior: true,
            onClick: function (e) {
                debugger;
                if ($scope.ContentTemplateForTabPopup === 'StudentHousingContent') {
                    if ($scope.MDL_StudentId === null || $scope.MDL_StudentId === undefined || $scope.MDL_StudentId === '') {
                        return DevExpress.ui.notify({ message: "برجاء إختيار الطالب" }, "Error", 10000);
                    }
                    if ($scope.MDL_HousingDateMeladi === null || $scope.MDL_HousingDateMeladi === undefined || $scope.MDL_HousingDateMeladi === '') {
                        return DevExpress.ui.notify({ message: "برجاء إختيار تاريخ التسكين" }, "Error", 10000);
                    }

                    debugger;
                    if ($scope.MDL_HousingTypeId === 1 && ($scope.MDL_BedLocationId === null || $scope.MDL_BedLocationId === undefined || $scope.MDL_BedLocationId === '')) {
                        return DevExpress.ui.notify({ message: "برجاء إختيار موقع السرير" }, "Error", 10000);
                    }

                    HousingOfStudentsSrvc.SaveStudentHousing($scope.MDL_StudentId, $scope.MDL_HousingDateMeladi, $scope.MDL_HousingNotes, $scope.MDL_RoomId, $scope.MDL_BedLocationId).then(function (data) {
                        if (data.data !== "") { swal("حدث خطأ", data.data, "error"); }
                        else {
                            swal("Done!", "تم تسكين الطالب بنجاح", "success");

                            $scope.TA_HousingNotes_ReadOnly = true;
                            $scope.Fn_DisabledLocationsDetails(true);
                            $scope.DB_HousingDateMeladi_Disabled = true;
                            $scope.btnSaveHousingOfStudentsVisible = false;
                        }
                    });
                }
                else if ($scope.ContentTemplateForTabPopup === 'HousingDeliveryToStudentContent') {

                    if ($scope.MDL_StudentId === null || $scope.MDL_StudentId === undefined || $scope.MDL_StudentId === '') {
                        return DevExpress.ui.notify({ message: "برجاء إختيار الطالب" }, "Error", 10000);
                    }

                    if ($scope.MDL_HousingTypeId === 1 && ($scope.MDL_BedLocationId === null || $scope.MDL_BedLocationId === undefined || $scope.MDL_BedLocationId === '')) {
                        debugger;
                        return DevExpress.ui.notify({ message: "برجاء إختيار موقع السرير" }, "Error", 10000);
                    }

                    if ($scope.DataSourceHousingFurnitureGrid === null || $scope.DataSourceHousingFurnitureGrid === undefined || $scope.DataSourceHousingFurnitureGrid === '' || $scope.DataSourceHousingFurnitureGrid.length === 0) {
                        return DevExpress.ui.notify({ message: "برجاء إسناد قطعة أثاث واحدة على الأقل" }, "Error", 10000);
                    }

                    HousingOfStudentsSrvc.SaveHousingFurniture($scope.MDL_StudentId, $scope.MDL_BedLocationId, $scope.DataSourceHousingFurnitureGrid).then(function (data) {
                        if (data.data !== "" && data.data.StudentFurnitures === undefined) { swal("حدث خطأ", data.data, "error"); }
                        else {

                            swal("Done!", "تم تسليم العهد للطالب بنجاح", "success");

                            $scope.DataSourceHousingFurnitureGrid = [];
                            for (var i = 0; i < data.data.StudentFurnitures.length; i++) {
                                $scope.DataSourceHousingFurnitureGrid.push(data.data.StudentFurnitures[i]);
                            }
                        }
                    });
                }
                else if ($scope.ContentTemplateForTabPopup === 'HousingDeliveryFromStudentContent' && $scope.ContentTemplateForInternalPopup === 'HousingDeliveryFromStudentContent-Return') {
                    if ($scope.MDL_StudentId === null || $scope.MDL_StudentId === undefined || $scope.MDL_StudentId === '') {
                        return DevExpress.ui.notify({ message: "برجاء إختيار الطالب" }, "Error", 10000);
                    }

                    //if ($scope.MDL_HousingDate === null || $scope.MDL_HousingDate === undefined || $scope.MDL_HousingDate === '') {
                    //    return DevExpress.ui.notify({ message: "برجاء إختيار تاريخ استلام الأصل" }, "Error", 10000);
                    //}
                    if ($scope.MDL_HousingReceiveFromDateMeladi === null || $scope.MDL_HousingReceiveFromDateMeladi === undefined || $scope.MDL_HousingReceiveFromDateMeladi === '') {
                        return DevExpress.ui.notify({ message: "برجاء إختيار تاريخ استلام الأصل" }, "Error", 10000);
                    }

                    HousingOfStudentsSrvc.SaveHousingFurnitureReturn($scope.MDL_StudentId, $scope.HousingFurnituresOfStudent_Id, $scope.MDL_HousingReceiveFromDateMeladi, $scope.MDL_HousingNotes).then(function (data) {
                        if (data.data !== "" && data.data.StudentFurnitures === undefined) { swal("حدث خطأ", data.data, "error"); }
                        else {
                            swal("Done!", "تم استلام العهد من الطالب بنجاح", "success");
                            // * Clear Controls
                            $scope.MDL_HousingReceiveFromDateMeladi = null;
                            $scope.MDL_HousingNotes = '';
                            $scope.HousingFurnituresOfStudent_Id = '';

                            // * Refresh Housing Furniture Grid After Save
                            $scope.DataSourceHousingFurnitureGrid = [];
                            for (var i = 0; i < data.data.StudentFurnitures.length; i++) {
                                $scope.DataSourceHousingFurnitureGrid.push(data.data.StudentFurnitures[i]);
                            }

                            // * Hide PopUp After Refresh
                            $scope.InternalPopupShow = false;
                            $scope.ContentTemplateForInternalPopup = '';
                        }
                    });
                }
                else if ($scope.ContentTemplateForTabPopup === 'StudentHousingTransferContent') {
                    debugger;
                    if ($scope.MDL_StudentId === null || $scope.MDL_StudentId === undefined || $scope.MDL_StudentId === '') {
                        return DevExpress.ui.notify({ message: "برجاء إختيار الطالب" }, "Error", 10000);
                    }

                    if ($scope.MDL_HousingTransferingDateMeladi === null || $scope.MDL_HousingTransferingDateMeladi === undefined || $scope.MDL_HousingTransferingDateMeladi === '') {
                        return DevExpress.ui.notify({ message: "برجاء إختيار تاريخ الانتقال" }, "Error", 10000);
                    }

                    if ($scope.MDL_HousingTypeId_Instance === 1 && ($scope.MDL_BedLocationId_Instance === null || $scope.MDL_BedLocationId_Instance === undefined || $scope.MDL_BedLocationId_Instance === '')) {
                        debugger;
                        return DevExpress.ui.notify({ message: "برجاء إختيار موقع السرير" }, "Error", 10000);
                    }

                    if ($scope.MDL_HousingTransferingDateMeladi !== null && $scope.MDL_HousingTransferingDateMeladi !== undefined && $scope.MDL_HousingTransferingDateMeladi !== '' && $scope.MDL_HousingDateMeladi !== null && $scope.MDL_HousingDateMeladi !== undefined && $scope.MDL_HousingDateMeladi !== '') {
                        if (new Date($scope.MDL_HousingTransferingDateMeladi) < new Date($scope.MDL_HousingDateMeladi)) {
                            return DevExpress.ui.notify({ message: "لايمكن أن يكون تاريخ الانتقال قبل تاريخ التسكين" }, "Error", 10000);
                        }
                    }
                    debugger;
                    HousingOfStudentsSrvc.SaveStudentHousingTransfer($scope.MDL_StudentId, $scope.MDL_HousingTransferingDateMeladi, $scope.MDL_HousingNotes_Instance, $scope.MDL_RoomId_Instance, $scope.MDL_BedLocationId_Instance).then(function (data) {
                        debugger;
                        if (data.data !== "") { swal("حدث خطأ", data.data, "error"); }
                        else {
                            swal("Done!", "تم نقل الطالب للسكن الجديد بنجاح", "success");

                            // Clear Housing Instance Controls...


                            $scope.MDL_HousingTypeId_Instance = '';

                            $scope.MDL_BedLocationId_Instance = '';
                            $scope.MDL_BedLocationCode_Instance = '';

                            $scope.MDL_HousingNotes_Instance = '';
                            $scope.MDL_HousingNumberAuto_Instance = '';
                            $scope.MDL_HousingDate_Instance = dateFormated;
                            $scope.MDL_HousingTransferingDateMeladi = dateFormated;

                            // Clear Housing Controls...

                            $scope.MDL_RoomName = '';
                            $scope.MDL_HousingTypeId = '';

                            $scope.MDL_BedLocationId = '';
                            $scope.MDL_BedLocationCode = '';

                            $scope.MDL_HousingDate = '';
                            $scope.MDL_HousingNotes = '';
                            $scope.MDL_HousingNumberAuto = '';

                            //$scope.MDL_RoomName = data.data.LocationDetails.SiteName;
                            HousingOfStudentsSrvc.GetHousingStudent($scope.MDL_StudentId).then(function (data) {
                                debugger;

                                if (data.data !== '' && data.data !== null && data.data !== undefined) {
                                    $scope.PageIsEditing = true;

                                    $scope.MDL_BedLocationId = data.data.BedLocationId;
                                    $scope.MDL_BedLocationCode = data.data.BedLocationCode;

                                    $scope.UserLastUpdated = data.data.LastUpdated;
                                    $scope.MDL_HousingNotes = data.data.NotesOfHosing;
                                    $scope.MDL_RoomId = data.data.LocationDetails.SiteId;
                                    $scope.MDL_FloorId = data.data.LocationDetails.FloorId;
                                    $scope.MDL_ComId = data.data.LocationDetails.CompanyId;
                                    $scope.MDL_RoomName = data.data.LocationDetails.SiteName;
                                    $scope.MDL_UnitId = data.data.LocationDetails.DepartmentId;
                                    $scope.MDL_HousingDate = $scope.Date_Format_Fn(data.data.HousingDate);
                                    $scope.MDL_HousingTypeId = data.data.LocationDetails.IsFamilial === false ? 1 : 2;
                                }
                            });

                            // * Next Housing No
                            HousingOfStudentsSrvc.GetNextHousingNo($scope.MDL_StudentId, 0).then(function (data) {
                                $scope.MDL_HousingNumberAuto = data.data;
                            });

                            // * Next Housing No For Instance
                            HousingOfStudentsSrvc.GetNextHousingNo(0, $scope.FoundId_Fn("Transfering")).then(function (data) {
                                $scope.MDL_HousingNumberAuto_Instance = data.data;
                            });
                        }
                    });

                }
                else if ($scope.ContentTemplateForTabPopup === 'StudentHousingReplacementContent' && $scope.ContentTemplateForInternalPopup === 'StudentHousingReplacementContent-Notes') {
                    if ($scope.MDL_StudentId === null || $scope.MDL_StudentId === undefined || $scope.MDL_StudentId === '') {
                        return DevExpress.ui.notify({ message: "برجاء إختيار الطالب الأول" }, "Error", 10000);
                    }

                    if ($scope.MDL_StudentId_Instance === null || $scope.MDL_StudentId_Instance === undefined || $scope.MDL_StudentId_Instance === '') {
                        return DevExpress.ui.notify({ message: " برجاء إختيار الطالب الثاني" }, "Error", 10000);
                    }

                    if ($scope.MDL_ReplacementDateMeladi === null || $scope.MDL_ReplacementDateMeladi === undefined || $scope.MDL_ReplacementDateMeladi === '') {
                        return DevExpress.ui.notify({ message: "برجاء إختيار تاريخ التبديل" }, "Error", 10000);
                    }

                    HousingOfStudentsSrvc.SaveHousingReplacement($scope.MDL_ReplacementDateMeladi, $scope.MDL_StudentId,
                        $scope.MDL_StudentId_Instance, $scope.MDL_ReplacementNotes_FirstSt, $scope.MDL_ReplacementNotes_SecondSt).then(function (data) {
                            if (data.data !== "") { swal("حدث خطأ", data.data, "error"); }
                            else {
                                swal({
                                    closeOnClickOutside: false,
                                    title: "Done!",
                                    text: "تمت عملية تبديل السكن بنجاح",
                                    icon: "success"
                                }).then((value) => {

                                    // * Hide Internal PopUp
                                    $scope.InternalPopupShow = false;

                                    // * Hide Tab PopUp
                                    $scope.TabPopUpShow = false;

                                    // * Clear First Student Control
                                    $scope.MDL_BedLocationId = '';
                                    $scope.MDL_BedLocationCode = '';

                                    $scope.MDL_HousingDate = '';
                                    $scope.MDL_HousingNotes = '';
                                    $scope.MDL_HousingTypeId = '';
                                    $scope.MDL_HousingNumberAuto = '';
                                    $scope.MDL_ReplacementNotes_FirstSt = '';

                                    // * Clear Second Student Control
                                    $scope.MDL_StudentId_Instance = null;
                                    $scope.MDL_ReplacementNotes_SecondSt = '';

                                    return;
                                });
                            }
                        });
                }
            }
        };
        /********************************** Save BTN *************************************** */


        // * Replacement Notes PopUp
        $scope.btnShowPopUpReplacementNotes = {
            bindingOptions: {
                text: 'btnShowPopUpReplacementNotes_Text'
            },
            visible: true,
            type: 'success',
            useSubmitBehavior: true,
            onClick: function (e) {
                if ($scope.MDL_StudentId === null || $scope.MDL_StudentId === undefined || $scope.MDL_StudentId === '') {
                    return DevExpress.ui.notify({ message: "برجاء إختيار الطالب الأول" }, "Error", 10000);
                }
                if ($scope.MDL_StudentId_Instance === null || $scope.MDL_StudentId_Instance === undefined || $scope.MDL_StudentId_Instance === '') {
                    return DevExpress.ui.notify({ message: " برجاء إختيار الطالب الثاني" }, "Error", 10000);
                }

                if ($scope.MDL_ReplacementDateMeladi === null || $scope.MDL_ReplacementDateMeladi === undefined || $scope.MDL_ReplacementDateMeladi === '') {
                    return DevExpress.ui.notify({ message: "برجاء إختيار تاريخ التبديل" }, "Error", 10000);
                }
                // * Set Second Student Drop Down ReadOnly
                $scope.DL_Students_Instance_ReadOnly = true;

                $scope.InternalPopupShow = true;
                $scope.InternalPopupWidth = 1250;
                $scope.btnSaveHousingOfStudentsVisible = true;
                $scope.InternalPopupTitle = 'ملاحظات عملية تبديل السكن';
                $scope.btnSaveHousingOfStudentsText = 'إجراء عملية التبديل';
                $scope.ContentTemplateForInternalPopup = 'StudentHousingReplacementContent-Notes';
            }
        };


        // * Tabs Popup
        $scope.deleteBtnShow = false;
        $scope.editBtnShow = false;

        $scope.TabsPopupOptions = {
            showTitle: true,
            dragEnabled: false,
            shadingColor: "rgba(0, 0, 0, 0.69)",
            closeOnOutsideClick: false,
            bindingOptions: {
                visible: "TabPopUpShow",
                title: "TabPopUpTitle",
                contentTemplate: "ContentTemplateForTabPopup",
                width: 'TabPopUpWidth'
            },
            elementAttr: {
            },
            height: 'auto',
            rtlEnabled: true,
            onHiding: function () {
                debugger;
      
                $scope.deleteBtnShow = false;
                $scope.editBtnShow = false;
                $scope.TabPopUpTitle = '';
                $scope.PageIsEditing = false;
                $scope.btnSaveHousingOfStudentsText = '';
                $scope.btnSaveHousingOfStudentsVisible = true;

                $scope.MDL_BedLocationId = '';
                $scope.MDL_BedLocationCode = '';

                if ($scope.ContentTemplateForTabPopup === 'StudentHousingContent') {

                    $scope.MDL_HousingDate = '';
                    $scope.MDL_HousingNotes = '';
                    $scope.MDL_HousingTypeId = '';
                    $scope.MDL_HousingNumberAuto = '';

                    $scope.TA_HousingNotes_ReadOnly = false;
                    $scope.DB_HousingDateMeladi_Disabled = false;
                    $scope.btnSaveHousingOfStudentsVisible = true;
                }
                else if ($scope.ContentTemplateForTabPopup === 'HousingDeliveryToStudentContent') {
                    debugger;
                    //$scope.deleteBtnShow = true;
                    //$scope.editBtnShow = false;

                    $scope.MDL_CategoryId = '';
                    $scope.MDL_FurnitureId = '';
                    $scope.MDL_HousingTypeId = '';

                    $scope.DataSourceHousingFurnitureGrid = [];
                }
                else if ($scope.ContentTemplateForTabPopup === 'HousingDeliveryFromStudentContent') {
 debugger;
                    //$scope.deleteBtnShow = false;
                    //$scope.editBtnShow = true;

                    $scope.MDL_HousingTypeId = '';
                    $scope.DataSourceHousingFurnitureGrid = [];
                }

                else if ($scope.ContentTemplateForTabPopup === 'StudentHousingTransferContent') {
                    $scope.MDL_HousingNumberAuto = '';
                    $scope.MDL_HousingNumberAuto_Instance = '';

                    $scope.MDL_HousingTypeId = '';
                    $scope.MDL_HousingTypeId_Instance = '';

                    $scope.MDL_BedLocationId_Instance = '';
                    $scope.MDL_BedLocationCode_Instance = '';

                    $scope.MDL_HousingDate = '';
                    $scope.MDL_HousingDate_Instance = dateFormated;

                    $scope.MDL_HousingNotes = '';
                    $scope.MDL_HousingNotes_Instance = '';

                    $scope.MDL_ReplacementNotes_FirstSt = '';
                    $scope.MDL_ReplacementNotes_SecondSt = '';

                    $scope.TA_HousingNotes_ReadOnly = false;
                    $scope.DB_HousingDateMeladi_Disabled = false;
                }

                else if ($scope.ContentTemplateForTabPopup === 'StudentHousingReplacementContent') {

                    $scope.MDL_ReplacementNumberAuto = '';
                    $scope.MDL_ReplacementDateMeladi = dateFormated;

                    $scope.MDL_BedLocationId_Instance = '';
                    $scope.MDL_BedLocationId_Instance = '';

                    $scope.MDL_BedLocationId_Instance = '';
                    $scope.MDL_BedLocationCode_Instance = '';

                    $scope.MDL_ReplacementNotes_FirstSt = '';
                    $scope.MDL_ReplacementNotes_SecondSt = '';

                    $scope.MDL_HousingDate = '';
                    $scope.MDL_HousingNotes = '';
                    $scope.MDL_HousingTypeId = '';
                    $scope.MDL_HousingNumberAuto = '';

                    $scope.DL_Students_ReadOnly = false;
                    $scope.MDL_StudentId_Instance = null;

                    $scope.TA_HousingNotes_ReadOnly = false;
                    $scope.TA_HousingNotes_ReadOnly_Instance = false;
                }

                else if ($scope.ContentTemplateForTabPopup === 'StudentHousingClearanceContent') {
                    //
                }


                // * Clear And ReInitial HomePage
                $scope.ClearHomePage_Fn();
                debugger;
                $scope.ReInitializeHomePage_Fn();
                $scope.ContentTemplateForTabPopup = '';

            },
            onInitialized(e) {
                $scope.TabsPopupInstance = e.component;
            }
        };


        // * Internal Popup
        $scope.InternalPopup = {
            showTitle: true,
            dragEnabled: false,
            shadingColor: "rgba(0, 0, 0, 0.69)",
            closeOnOutsideClick: false,
            bindingOptions: {
                visible: "InternalPopupShow",
                title: "InternalPopupTitle",
                contentTemplate: "ContentTemplateForInternalPopup",
                width: 'InternalPopupWidth'
            },
            height: 'auto',
            rtlEnabled: true,
            onHiding: function () {
                if ($scope.ContentTemplateForInternalPopup === 'HousingDeliveryFromStudentContent-Return') {
                    $scope.MDL_HousingDate = '';
                    $scope.MDL_HousingNotes = '';
                    $scope.HousingFurnituresOfStudent_Id = '';

                    $scope.InternalPopupTitle = '';
                    $scope.btnSaveHousingOfStudentsText = '';
                    $scope.ContentTemplateForInternalPopup = '';
                } else if ($scope.ContentTemplateForInternalPopup === 'StudentHousingReplacementContent-Notes') {
                    $scope.DL_Students_Instance_ReadOnly = false;
                }

            },
            onInitialized(e) {
                $scope.InternalPopupInstance = e.component;
            }
        };


        // * Functions...
        $scope.FoundId_Fn = function (text) { return Config.find(x => x.Text === text).Id; };

        $scope.Fn_DisabledLocationsDetails = function (value) {
            debugger;
            $scope.DL_ComsDisabled = value;
            $scope.DL_UnitsDisabled = value;
            $scope.DL_RoomsDisabled = value;
            $scope.DL_FloorsDisabled = value;
            $scope.DL_HousingTypesDisabled = value;
            $scope.DL_BedsDisabled = value;
        };
        $scope.Fn_DisabledLocationsDetails(true);

        $scope.Fn_DisabledLocationsDetails_Instance = function (value) {
            $scope.DL_ComsDisabled_Instance = value;
            $scope.DL_UnitsDisabled_Instance = value;
            $scope.DL_RoomsDisabled_Instance = value;
            $scope.DL_FloorsDisabled_Instance = value;
            $scope.DL_HousingTypesDisabled_Instance = value;
            $scope.DL_BedsDisabled_Instance = value;
        };

        $scope.Date_Format_Fn = function (date_String) {
            const value = date_String.split('/');
            return value[1] + '/' + value[0] + '/' + value[2];
        };

        $scope.ReInitializeHomePage_Fn = function () {
            if ($scope.MDL_StudentId !== null && $scope.MDL_StudentId !== '') {

                $scope.PageIsEditing = true;
                $scope.TA_HousingNotes_ReadOnly = true;
                $scope.DB_HousingDateMeladi_Disabled = true;
                $scope.MDL_HousingDateMeladi = null;
                $scope.MDL_HousingDateHijri = null;

                HousingOfStudentsSrvc.GetStudentDataByID($scope.MDL_StudentId).then(function (data) {
                    $scope.MDL_FacultiyName = data.data.FacultiyName;
                    $scope.MDL_NationalityName = data.data.NationalityName;
                    $scope.MDL_MobileNumber = data.data.MobileNumber;
                    $scope.MDL_BirthCityName = data.data.BirthCityName;
                    $scope.MDL_LevelName = data.data.LevelName;
                    $scope.MDL_StatusName = data.data.StatusName;
                    $scope.MDL_BirthDate = $scope.Date_Format_Fn(data.data.BirthDate);
                    $scope.MDL_StudyTypeName = data.data.StudyTypeName;
                    $scope.MDL_DegreeName = data.data.DegreeName;
                    $scope.MDL_GenderName = data.data.GenderName;
                });


                HousingOfStudentsSrvc.GetHousingStudent($scope.MDL_StudentId).then(function (data) {
                    if (data.data.BedLocationId !== null && data.data.BedLocationId !== '' && data.data.BedLocationId !== undefined) {

                        $scope.MDL_BedLocationId = data.data.BedLocationId;
                        $scope.MDL_BedLocationCode = data.data.BedLocationCode;

                        $scope.UserLastUpdated = data.data.LastUpdated;
                        $scope.MDL_HousingNotes = data.data.NotesOfHosing;
                        $scope.MDL_RoomId = data.data.LocationDetails.SiteId;
                        $scope.MDL_FloorId = data.data.LocationDetails.FloorId;
                        $scope.MDL_ComId = data.data.LocationDetails.CompanyId;
                        $scope.MDL_RoomName = data.data.LocationDetails.SiteName;
                        $scope.MDL_UnitId = data.data.LocationDetails.DepartmentId;
                        $scope.MDL_HousingDate = $scope.Date_Format_Fn(data.data.HousingDate);
                        $scope.MDL_HousingTypeId = data.data.LocationDetails.IsFamilial === false ? 1 : 2;


                    }
                });

                // * Next Housing No...
                HousingOfStudentsSrvc.GetNextHousingNo($scope.MDL_StudentId, 0).then(function (data) {
                    $scope.MDL_HousingNumberAuto = data.data;
                });
            }
            else {
                $scope.ClearHomePage_Fn();
            }
        };
        

        $scope.ClearHomePage_Fn = function () {
            debugger;
            $scope.PageIsEditing = false;
            $scope.TA_HousingNotes_ReadOnly = false;
            $scope.DB_HousingDateMeladi_Disabled = false;
            //$scope.deleteBtnShow = false;
            //$scope.editBtnShow = false;
            $scope.MDL_HousingDateMeladi = null;
            $scope.MDL_HousingDateHijri = null;
            $scope.MDL_BedLocationId = '';
            $scope.MDL_BedLocationCode = '';

            $scope.MDL_FacultiyName = '';
            $scope.MDL_NationalityName = '';
            $scope.MDL_MobileNumber = '';
            $scope.MDL_BirthCityName = '';
            $scope.MDL_LevelName = '';
            $scope.MDL_StatusName = '';
            $scope.MDL_BirthDate = '';
            $scope.MDL_StudyTypeName = '';
            $scope.MDL_DegreeName = '';
            $scope.MDL_GenderName = '';

            $scope.UserLastUpdated = '';
            $scope.MDL_HousingDate = '';
            $scope.MDL_HousingNotes = '';
            $scope.MDL_HousingTypeId = '';
            $scope.MDL_HousingNumberAuto = '';

            $scope.MDL_RoomId = '';
            $scope.MDL_RoomName = '';
        };



    }]);
})();