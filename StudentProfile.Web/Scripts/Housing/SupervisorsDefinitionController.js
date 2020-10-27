app.controller("SupervisorsDefinitionsCtrl", ["$scope", 'SupervisorsDefinitionsSrvc', '$http',
    function ($scope, SupervisorsDefinitionsSrvc,$http ) {

        //controls
        $scope.UserSelectBox = {
            bindingOptions: {
                items: 'UsersArrey',
                value: 'userId',
                dataSource: 'UsersArrey'
            },
            placeholder: "--أختر--",
            displayExpr: "Text",
            valueExpr: "Value",
            onInitialized: function (e) {

                SupervisorsDefinitionsSrvc.GetUsers().then(function (dataReturned) {

                    $scope.UsersArrey = dataReturned.data;
                });
            }
        };

        $scope.txtFileNumber = {
            bindingOptions: {
                value: "fileNumber"
            }
        };

        $scope.txtTelephoneNumber = {
            bindingOptions: {
                value: "telephoneNumber"
            }
        };

        $scope.isPresidentCheckBox = {
            bindingOptions: {
                value: "isPresident"
            }
        };
        $scope.txtNotes = {
            bindingOptions: {
                value: "notes"
            }
        };

        $scope.saveButton = {
            text: "حفظ",
            type: 'success',
            onClick: function () {
                if ($scope.userId === null || $scope.userId === '') {
                    DevExpress.ui.notify({
                        message: 'عفوا ادخل اسم المستخدم',
                        type: "error",
                        displayTime: 3000,
                        closeOnClick: true
                    });
                    return;
                }

                if ($scope.fileNumber === null || $scope.fileNumber === '') {
                    DevExpress.ui.notify({
                        message: 'عفوا ادخل الرقم الوظيفي',
                        type: "error",
                        displayTime: 3000,
                        closeOnClick: true
                    });
                    return;
                }

                if ($scope.telephoneNumber === null || $scope.telephoneNumber === '') {
                    DevExpress.ui.notify({
                        message: 'عفوا ادخل رقم الهاتف',
                        type: "error",
                        displayTime: 3000,
                        closeOnClick: true
                    });
                    return;
                }

                if ($scope.locationId === null || $scope.locationId=== "") {
                    DevExpress.ui.notify({
                        message: 'عفوا اختر مكان التسكين',
                        type: "error",
                        displayTime: 3000,
                        closeOnClick: true
                    });
                    return;
                } 

                if ($scope.buildingId === null || $scope.buildingId === "") {
                    DevExpress.ui.notify({
                        message: 'عفوا اختر مبنى التسكين',
                        type: "error",
                        displayTime: 3000,
                        closeOnClick: true
                    });
                    return;
                }

                if ($scope.floorId === null || $scope.floorId === "") {
                    DevExpress.ui.notify({
                        message: 'عفوا اختر دور التسكين',
                        type: "error",
                        displayTime: 3000,
                        closeOnClick: true
                    });
                    return;
                }

                if ($scope.roomsIds === null || $scope.roomsIds.length === 0) {
                    DevExpress.ui.notify({
                        message: 'عفوا اختر غرف التسكين',
                        type: "error",
                        displayTime: 3000,
                        closeOnClick: true
                    });
                    return;
                }

                SupervisorsDefinitionsSrvc.SaveSuperVisor(
                    $scope.userId, $scope.fileNumber,
                    $scope.telephoneNumber, $scope.isPresident,
                    $scope.notes, $scope.roomsIds)
                    .then(function (returnedData) {
                    if (returnedData.data === '') {
                    DevExpress.ui.notify({
                        message: 'تم الحفظ بنجاح',
                        type: "success",
                        displayTime: 3000,
                        closeOnClick: true
                        });
                    SupervisorsDefinitionsSrvc.GetSupervisors().then(function (dataReturned) {
                        $scope.supervisors = dataReturned.data;
                    });

                    } else {
                        DevExpress.ui.notify({
                            message: returnedData.data,
                            type: "error",
                            displayTime: 3000,
                            closeOnClick: true
                        });
                    }
                });
            }
        };

        $scope.supervisorsDataGrid = {
            bindingOptions: {
                dataSource: 'supervisors'
            },
            showBorders: true,
            columns: [{
                dataField: "Username",
                width: 200,
                caption: "اسم المشرف"
            },
            {
                dataField: "FileNumber",
                width: 130,
                caption: "الرقم الوظيفي"
            },
            {
                dataField: "TelephoneNumber",
                width: 130,
                caption: "رقم الهاتف"
            },
            {
                dataField: "IsPresident",
                width: 130,
                caption: "رئيس وحدة اشراف"
            },
            {
                dataField: "Notes",
                width: 130,
                caption: "ملاحظات"
            }],
            masterDetail: {
                enabled: true,
                template: "detail"
            },
            onInitialized: function () {
                
                SupervisorsDefinitionsSrvc.GetSupervisors().then(function (dataReturned) {
                    $scope.supervisors = dataReturned.data;
                });
            }
        };

       

        $scope.getRoomsForeachSupervisor = function (key) {
            $scope.roomsForSupervisor = key.Rooms;
            return {
                dataSource: $scope.roomsForSupervisor,
                columnAutoWidth: true,
                showBorders: true,
                columns: [{
                    dataField: "RoomName",
                    caption: "غرف تحت الاشراف"
                },
                {
                    dataField: "HousingFloorName",
                    caption: "دور التسكين"
                },
                {
                    dataField: "HousingBuildingName",
                    caption: "مبنى التسكين"
                },
                {
                    dataField: "HousingPlaceName",
                    caption: "مكان التسكين"
                }]
            };
        };

        $scope.locationsSelectBox = {
            bindingOptions: {
                items: "locationsArray",
                value: "locationId",
                dataSource: "locationsArray"
            },
            placeholder: "--اختر--",
            displayExpr: "LocationName", 
            valueExpr: "LocationId",
            onInitialized: function (e) {
                SupervisorsDefinitionsSrvc.getHousingLocations().then(function (dataReturned) {

                    $scope.locationsArray = dataReturned.data;
                });
            },
            onValueChanged: function (e) {
                $scope.locationId = Number(e.value);
                $scope.buildingId = null;

                SupervisorsDefinitionsSrvc.GetHousingBuildings($scope.locationId).
                    then(function (dataReturned) {
                        $scope.buildingArray = dataReturned.data;
                    });
                $scope.floorsArray = [];
                $scope.roomsArray = [];
            }
        };



        $scope.BuildingSelectBox = {
            bindingOptions: {
                items: "buildingArray",
                value: "buildingId",
                dataSource: "buildingArray"
            },
            placeholder: "--اختر--",
            displayExpr: "BuildingName",
            valueExpr: "BuildingId",
            
            onValueChanged: function (e) {
                $scope.buildingId = Number(e.value);
                $scope.floorId = null;

                SupervisorsDefinitionsSrvc.GetHousingBuildings($scope.buildingId).
                    then(function (dataReturned) {
                        $scope.floorsArray = dataReturned.data;
                    });
                
                $scope.roomsArray = [];
            }
        };



        $scope.FloorSelectBox = {
            bindingOptions: {
                items: "floorsArray",
                value: "floorId",
                dataSource: "floorsArray"
            },
            placeholder: "--اختر--",
            displayExpr: "BuildingName",
            valueExpr: "BuildingId",
            onValueChanged: function (e) {
                $scope.floorId = Number(e.value);

                SupervisorsDefinitionsSrvc.GetHousingRooms($scope.floorId).
                    then(function (dataReturned) {
                        $scope.roomsArray = dataReturned.data;
                    });
            }
        };


        $scope.roomsList = {
            bindingOptions: {
                dataSource: "roomsArray",
                selectedItemKeys: "selectedRoomsArray"
            },
            selectionMode: "selectionMode",
            selectAllMode: "selectAllMode",
            showSelectionControls: true,
            keyExpr: "id",
            noDataText: "لا يوجد غرف متاحه",
            onInitialized: function (e) {
                $scope.roomsIds = [];
            },
            onSelectionChanged: function (e) {
                //check if there were items added
                if (e.addedItems.length > 0) {
                    for (var i = 0; i < e.addedItems.length; i++) {
                        $scope.roomsIds.push(Number(e.addedItems[i].id));
                    }
                }

                //check if there were items removed
                if (e.removedItems.length > 0) {
                    var roomsIdsCopy = $scope.roomsIds; //copy of roomsIds to remove items from
                    for (k = 0; k < $scope.roomsIds.length; k++) {
                        for (var l = 0; l < e.removedItems.length; l++) {
                            if ($scope.roomsIds[k] === Number(e.removedItems[l].id)) {
                                roomsIdsCopy.splice(k, 1);
                            }
                        }
                    }
                    $scope.roomsIds = roomsIdsCopy; //new chosen roomsIds
                }
            }
        };


     



        //fields
        
        $scope.userId = null; //used with users select box
        $scope.UsersArrey = []; //used with users select box
        $scope.fileNumber = null;
        $scope.telephoneNumber = null;
        $scope.isPresident = false;
        $scope.notes = null;
        $scope.supervisors = []; //used with supervisors data grid
        $scope.roomsForSupervisor = []; //used with detaild data grid
        $scope.locationsArray = []; //used with locations selectbox
        $scope.locationId = null; //used with locations selectbox
        $scope.buildingArray = []; //used with buildings selectbox
        $scope.buildingId = null; //used with buildings selectbox
        $scope.floorsArray = []; //used with floors selectbox
        $scope.floorId = null; //used with floors selectbox
        $scope.roomsArray = []; //used with rooms list
        $scope.selectedRoomsArray = []; //used with rooms list
        $scope.roomsIds = []; //used with rooms list

        //validators
        $scope.UserSelectBoxValidator = {
            validationRules: [{
                type: "required",
                message: "رجاء اختيار اسم المشرف"
            }]
        };

        $scope.txtFileNumberValidator = {
            validationRules: [{
                type: "required",
                message: "رجاء ادخال الرقم الوظيفي"
            }]
        };

        $scope.txtTelephoneNumberValidator = {
            validationRules: [{
                type: "required",
                message: "رجاء ادخال رقم الهاتف"
            }]
        };


    }]);