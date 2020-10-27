/// <reference path="../angular.js" />
/// <reference path="../dx.web.debug.js" />

(function () {
    app.controller("EditStudentDesiresByAdminCtrl", ['$scope', '$http', '$location',
        'StudentsDesiresByAdminSrvc', 'MusaferSrvc', 'TravelAdvertisementSrvc',
        function ($scope, $location, $http, StudentsDesiresByAdminSrvc, MusaferSrvc, TravelAdvertisementSrvc) {

            $scope.dateBoxToDisabled = false;

            $scope.advertisementsList = [];

            $scope.AdvertismentSelectBoxInstance = '';
            $scope.GetData = function (id) {
                if (id !== null) {
                    // Student Desire Id For Editing
                    $scope.StudentDesireIdForEdit = Number(id);

                    StudentsDesiresByAdminSrvc.GetStudentDesireDetails($scope.StudentDesireIdForEdit).then(function (response) {
                        debugger;
                        if (response.data !== "") {

                            $scope.advertisementId = String(response.data.AdvertisementID);
                            $scope.TransportationTrackingID = String(response.data.TransportationTrackingID);
                           // $scope.TravelWay.value = String(response.data.FlightsType);
                            if (response.data.FlightsType === "R") {
                                $scope.TravelWay = $scope.TravelWayPriorities[0];
                                $scope.dateBoxToDisabled = false;
                            }

                            if (response.data.FlightsType === "O") {
                                $scope.TravelWay = $scope.TravelWayPriorities[1];
                                $scope.EndDate = null;
                                $scope.dateBoxToDisabled = true;
                            }
                            $scope.isPassportValid = response.data.IsPassportVerified;
                            $scope.studentID = String(response.data.StudentID);
                            $scope.StartDate = String(response.data.Departing);
                            $scope.EndDate = String(response.data.Returning);
                            
                        //    $scope.AdvertismentSelectBoxInstance.repaint();
                        }
                    });
                }
            };

            // DATASOURCES
            $scope.TravelWayPriorities = [{ value: 'R', text: "ذهاب وعودة" }, { value: 'O', text: "ذهاب فقط" }];

            var DataSourceLevels = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: true,
                key: "Value",
                loadMode: "raw",
                load: function () {
                    return $.getJSON("/Musafer/GetLevels", function (data) { });
                }
            });
            var DataSourcePurposes = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: true,
                key: "Value",
                loadMode: "raw",
                load: function () {
                    return $.getJSON("/TravelAdvertisement/GetPurpose", function (data) { });
                }
            });
            var DataSourceStudents = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: true,
                key: "Value",
                loadMode: "raw",
                load: function () {
                    return $.getJSON("/Musafer/GetStudents", function (data) { });
                }
            });
            var DataSourceAirportTo = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: true,
                key: "Value",
                loadMode: "raw",
                load: function () {
                    if ($scope.CountryToID !== null || $scope.CountryToID !== '') {
                        return $.getJSON("/Musafer/GetAirports",
                            { id: $scope.CountryToID },
                            function (data) { });
                    }
                    return [];
                }
            });
            var DataSourceCountries = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: true,
                key: "Value",
                loadMode: "raw",
                load: function () {
                    return $.getJSON("/Musafer/GetCountries", function (data) { });
                }
            });
            var DataSourceAirportFrom = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: true,
                key: "Value",
                loadMode: "raw",
                load: function () {
                    if ($scope.CountryFromID !== null || $scope.CountryFromID !== '') {
                        return $.getJSON("/Musafer/GetAirports",
                            { id: $scope.CountryFromID },
                            function (data) { });
                    }
                    return [];
                }
            });

            function initDataSource() {
                DataSourceStudents.reload();
            }
            initDataSource();

            // FUNCTIONS
            $scope.setAdvertisementDefaultSettings = function () {
                debugger;

                var date = new Date();

                $scope.StartDate = new Date(date.getFullYear(), date.getMonth(), date.getDate());
                $scope.EndDate = new Date(date.getFullYear(), date.getMonth(), date.getDate());

                function formatDate(date) {
                    var d = new Date(),
                        month = d.getMonth() + 1,
                        day = d.getDate(),
                        year = d.getFullYear();
                    return [day, month, year].join('/');
                }

                $scope.ValidateMinDeparting = new Date(date.getFullYear(), date.getMonth(), date.getDate());
                $scope.ValidateMaxDeparting = null;
                $scope.ValidateMinReturning = new Date(date.getFullYear(), date.getMonth(), date.getDate());
                $scope.ValidateMaxReturning = null;


                $scope.PurposeID = null;
                $scope.LevelID = null;

                $scope.levelSelectBoxDisabled = false;
                $scope.PurposeSelectBoxDisabled = false;
                $scope.radioTravelWayDisabled = false;
            };
            $scope.GetAdvertisementById = function (advertisementId) {

                MusaferSrvc.GetAdvertisementById(advertisementId).then(function (data) {
                    $scope.ValidateMinDeparting = new Date(data.data.DepartingStart);
                    $scope.ValidateMaxDeparting = new Date(data.data.DepartingEnd);
                    $scope.ValidateMinReturning = new Date(data.data.ReturningStart);
                    $scope.ValidateMaxReturning = new Date(data.data.ReturningEnd);


                    $scope.LevelID = String(data.data.TravelClass);

                    if (data.data.FlightsType === "R") {
                        $scope.TravelWay = $scope.TravelWayPriorities[0];
                        $scope.dateBoxToDisabled = false;

                    }

                    if (data.data.FlightsType === "O") {
                        $scope.TravelWay = $scope.TravelWayPriorities[1];
                        $scope.EndDate = null;
                        $scope.dateBoxToDisabled = true;
                    }

                    $scope.PurposeID = String(data.data.Purpose);

                    if ($scope.AdvertismentSelectBoxInstance) {
                        $scope.AdvertismentSelectBoxInstance._options.value = advertisementId;
                       // $scope.AdvertismentSelectBoxInstance.repaint();
                    }

                });
            };

            // CONTROLS
            $scope.StudentsSelectBox = {
                dataSource: DataSourceStudents,
                bindingOptions: {
                    value: "studentID"
                },
                placeholder: "--أختر--",
                noDataText: "لا يوجد بيانات",
                showClearButton: true,
                searchEnabled: true,
                displayExpr: "Text",
                valueExpr: "Value",
                onInitialized: function (e) {
                },
                onValueChanged: function (e) {
                    debugger;
                    if (e.value !== null && e.value !== "") {
                        StudentsDesiresByAdminSrvc.GetTransportationTrackingByStudent($scope.TravelWay.value, $scope.studentID).then(function (data) {
                            debugger;
                            $scope.TransportationTrackingList = data.data;
                        });
                    }
                }
            };

            $scope.TransportaionTrackingSelectBox = {
                bindingOptions: {
                    dataSource: "TransportationTrackingList",
                    value: "TransportationTrackingID",
                    items: "TransportationTrackingList"
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
                    debugger;
                    if ($scope.studentID != null && $scope.studentID != "" && $scope.studentID != undefined) {
                        StudentsDesiresByAdminSrvc.GetTransportationTrackingByStudent($scope.TravelWay.value, $scope.studentID).then(function (data) {
                            debugger;
                            $scope.TransportationTrackingList = data.data;
                        });
                    }
                },
                onValueChanged: function (e) {
                    $scope.TransportationTrackingID = e.value;

                },
            };


            $scope.AdvertismentSelectBox = {
                bindingOptions: {
                    dataSource: "advertisementsList",
                    value: "advertisementId",
                    items: "advertisementsList"
                },
                onInitialized: function (e) {

                    StudentsDesiresByAdminSrvc.GetActiveAdvertisement().then(function (data) {
                        $scope.advertisementsList = data.data;
                    });
                },

                onValueChanged: function (e) {
                    if (e.value !== null && e.value !== "") {
                        $scope.GetAdvertisementById(e.value);

                        $scope.levelSelectBoxDisabled = true;
                        $scope.PurposeSelectBoxDisabled = true;
                        $scope.radioTravelWayDisabled = true;
                    } else {
                        $scope.setAdvertisementDefaultSettings();
                    }

                },

                placeholder: "--أختر--",
                noDataText: "لا يوجد بيانات",
                pagingEnabled: true, //Pagenation
                showClearButton: true,
                searchEnabled: true,
                displayExpr: "Text",
                valueExpr: "Value"
            };

            $scope.LevelSelectBox = {
                dataSource: DataSourceLevels,
                bindingOptions: {
                    value: "LevelID",
                    disabled: "levelSelectBoxDisabled"
                },
                placeholder: "--أختر--",
                noDataText: "لا يوجد بيانات",
                showClearButton: true,
                searchEnabled: true,
                displayExpr: "Text",
                valueExpr: "Value",
                onInitialized: function (e) {
                }
            };

            $scope.PurposeSelectBox = {
                dataSource: DataSourcePurposes,
                bindingOptions: {
                    value: "PurposeID",
                    disabled: "PurposeSelectBoxDisabled"
                },
                placeholder: "--أختر--",
                noDataText: "لا يوجد بيانات",
                showClearButton: true,
                searchEnabled: true,
                displayExpr: "Text",
                valueExpr: "Value",
                onInitialized: function (e) {
                }
            };

            $scope.radioTravelWay = {
                dataSource: $scope.TravelWayPriorities,
                bindingOptions: {
                    value: 'TravelWay',
                    disabled: 'radioTravelWayDisabled'
                },
                layout: "horizontal"
            };

            $scope.passportValidCheckBox = {
                bindingOptions: {
                    value: "isPassportValid"
                },
                text: "جواز سفر صالح"
            };

            $scope.dateBoxFrom = {
                bindingOptions: {
                    value: "StartDate",
                    min: "ValidateMinDeparting",
                    max: "ValidateMaxDeparting"
                },
                type: "date",
                onValueChanged: function (e) {
                    $scope.StartDate =  e.value;
                    if (e.value !== null || e.value !== '') {
                        $scope.ValidateMinReturning = e.value;
                    } else {
                        $scope.ValidateMinReturning = new Date();
                    }
                }
            };

            $scope.dateBoxTo = {
                bindingOptions: {
                    value: "EndDate",
                    min: "ValidateMinReturning",
                    max: "ValidateMaxReturning",
                    disabled : "dateBoxToDisabled"
                },
                type: "date",
                showClearButton: true,
                onValueChanged: function (e) {
                    debugger;
                    $scope.EndDate = e.value;

                }
            };

            // SUBMIT
            $scope.btnSave = {
                text: 'تعديل',
                type: 'success',
                onClick: function (e) {
                    debugger;
                    if ($scope.studentID === null || $scope.studentID === '') {
                        DevExpress.ui.notify({
                            message: 'عفوا ادخل طالب',
                            type: "error",
                            displayTime: 3000,
                            closeOnClick: true
                        });
                        return;
                    }
                    if ($scope.TransportationTrackingID === null || $scope.TransportationTrackingID === '') {
                        DevExpress.ui.notify({
                            message: 'عفوا ادخل خطوط السير',
                            type: "error",
                            displayTime: 3000,
                            closeOnClick: true
                        });
                        return;
                    }


                   

                    if ($scope.StartDate === null || $scope.StartDate === '') {
                        DevExpress.ui.notify({
                            message: 'عفوا ادخل تاريخ السفر',
                            type: "error",
                            displayTime: 3000,
                            closeOnClick: true
                        });
                        return;
                    }

                    if ($scope.TravelWay.value === 'R' && ($scope.EndDate === null || $scope.EndDate === '')) {
                        DevExpress.ui.notify({
                            message: 'عفوا ادخل تاريخ العودة',
                            type: "error",
                            displayTime: 3000,
                            closeOnClick: true
                        });
                        return;
                    }

                    if ($scope.LevelID === null || $scope.LevelID === '') {
                        DevExpress.ui.notify({
                            message: 'عفوا ادخل درجة السفر',
                            type: "error",
                            displayTime: 3000,
                            closeOnClick: true
                        });
                        return;
                    }

                    if ($scope.PurposeID === null || $scope.PurposeID === '') {
                        DevExpress.ui.notify({
                            message: 'عفوا الغرض من السفر',
                            type: "error",
                            displayTime: 3000,
                            closeOnClick: true
                        });
                        return;
                    }

                    if ($scope.TravelWay.value === null || $scope.TravelWay.value === '') {
                        DevExpress.ui.notify({
                            message: 'عفوا ادخل اتجاة السفر',
                            type: "error",
                            displayTime: 3000,
                            closeOnClick: true
                        });
                        return;
                    }

                    if ($scope.isPassportValid === null || $scope.isPassportValid === '') {
                        DevExpress.ui.notify({
                            message: 'عفوا ادخل حالة جواز السفر',
                            type: "error",
                            displayTime: 3000,
                            closeOnClick: true
                        });
                        return;
                    }
                    debugger;
                    StudentsDesiresByAdminSrvc.SaveStudentDesiresByAdmin(
                        $scope.StudentDesireIdForEdit,
                        $scope.studentID,
                        $scope.LevelID,
                        $scope.TravelWay.value,
                        $scope.StartDate,
                        $scope.EndDate,
                        $scope.PurposeID,
                        $scope.advertisementId,
                        $scope.isPassportValid,
                        $scope.TransportationTrackingID

                    ).then(function (data) {
                        debugger;
                        if (data.data !== '') {
                            DevExpress.ui.notify({
                                message: data.data,
                                type: "error",
                                displayTime: 3000,
                                closeOnClick: true
                            });
                        } else {
                            DevExpress.ui.notify({
                                message: 'تم التعديل بنجاح',
                                type: "success",
                                displayTime: 3000,
                                closeOnClick: true
                            });
                            $scope.Reset();
                            window.open('/Musafer/TravelRequestReview', '_self');
                        }

                    });
                }
            };

            // RESET
            $scope.Reset = function () {
                $scope.StudentDesireId = null;
                $scope.TravelWay.value = null;
                $scope.advertisementId = null;
                $scope.isPassportValid = null;
                $scope.studentID = null;
                $scope.StartDate = null;
                $scope.PurposeID = null;
                $scope.LevelID = null;
                $scope.EndDate = null;
            };
        }]);
}());