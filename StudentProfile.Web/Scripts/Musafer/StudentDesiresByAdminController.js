app.controller("StudentDesiresByAdminCtrl", ["$scope", '$location', 'StudentsDesiresByAdminSrvc', 'MusaferSrvc',
    'TravelAdvertisementSrvc',
    function ($scope, $location, StudentsDesiresByAdminSrvc, MusaferSrvc, TravelAdvertisementSrvc) {

        $scope.StudentDesireId = null;

        $scope.dateBoxToDisabled = false;


        $scope.CountryList = [];
        $scope.CountryToID = null;

        $scope.CountryFromID = null;

        $scope.AirportToList = [];
        $scope.AirportToID = null;


        $scope.AirportFromList = [];
        $scope.AirportFromID = null;

        $scope.studentsList = [];
        $scope.studentID = null;

        $scope.advertisementsList = [];
        $scope.advertisementId = null;

        $scope.LevelList = [];
        $scope.LevelID = null;


        $scope.PurposeList = [];
        $scope.PurposeID = null;


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
                //Get Airport
                //MusaferSrvc.GetAirports(e.value).then(function (data) {
                //    $scope.AirportList = data.data;
                //});

            },
        };


        $scope.setAdvertisementDefaultSettings = function () {
            debugger;
            $scope.StartDate = new Date();
            $scope.EndDate = new Date();

            $scope.ValidateMinDeparting = new Date();
            $scope.ValidateMaxDeparting = null;
            $scope.ValidateMinReturning = new Date();
            $scope.ValidateMaxReturning = null;


            $scope.PurposeID = null;
            $scope.LevelID = null;

            $scope.levelSelectBoxDisabled = false;
            $scope.PurposeSelectBoxDisabled = false;
            $scope.radioTravelWayDisabled = false;
        };

        $scope.setAdvertisementDefaultSettings();




        //GetAdvertisementById
        $scope.GetAdvertisementById = function (advertisementId) {

            MusaferSrvc.GetAdvertisementById(advertisementId).then(function (data) {
                debugger;
                $scope.StartDate = new Date(data.data.DepartingStart);
                $scope.EndDate = new Date(data.data.ReturningEnd);
                $scope.ValidateMinDeparting = new Date(data.data.DepartingStart);
                $scope.ValidateMaxDeparting = new Date(data.data.DepartingEnd);
                $scope.ValidateMinReturning = new Date(data.data.ReturningStart);
                $scope.ValidateMaxReturning = new Date(data.data.ReturningEnd);
                $scope.TransportationTrackingID = data.data.TransportationTrackingID;

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


            });
        };

        $scope.TravelWayPriorities = [{ value: 'R', text: "ذهاب وعودة" }, { value: 'O', text: "ذهاب فقط" }];
        $scope.TravelWay = $scope.TravelWayPriorities[0];
        $scope.isPassportValid = true;


        var DataSourceStudentList = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: true,
            key: "Value",
            loadMode: "raw",
            load: function () {
                return $.getJSON("/Musafer/GetStudents", function (data) { debugger; });
            }
        });


        $scope.StudentsSelectBox = {
            dataSource: DataSourceStudentList,
            bindingOptions: {

                value: "studentID",
                //items: "studentsList"
            },
            //onInitialized: function (e) {
            //    StudentsDesiresByAdminSrvc.GetStudents().then(function (data) {
            //        //$scope.studentsList = data.data;
            //    });
            //},

            placeholder: "--أختر--",
            noDataText: "لا يوجد بيانات",
            pagingEnabled: true, //Pagenation
            showClearButton: true,
            searchEnabled: true,
            displayExpr: "Text",
            valueExpr: "Value",
            searchExpr: ['Text', 'Value', 'NationalID'],
            onValueChanged: function (e) {
                debugger;
                if (e.value !== null && e.value !== "") {
                    StudentsDesiresByAdminSrvc.GetTransportationTrackingByStudent($scope.TravelWay.value, $scope.studentID).then(function (data) {
                        debugger;
                        $scope.TransportationTrackingList = data.data;
                    });
                }
            }
        }

        $scope.AdvertismentSelectBox = {
            bindingOptions: {
                dataSource: "advertisementsList",
                value: "advertisementId",
                items: "advertisementsList"
            },
            onInitialized: function (e) {
                debugger;
                StudentsDesiresByAdminSrvc.GetActiveAdvertisement().then(function (data) {
                    $scope.advertisementsList = data.data;
                });
                //if ($scope.studentID != null) {
                //    StudentsDesiresByAdminSrvc.GetTransportationTrackingByStudent($scope.TravelWay.value, $scope.studentID).then(function (data) {
                //        debugger;
                //        $scope.TransportationTrackingList = data.data;
                //    });
                //}

            },
            onValueChanged: function (e) {
                debugger;
                if (e.value !== null && e.value !== "") {
                    //StudentsDesiresByAdminSrvc.GetTransportationTrackingByStudent($scope.TravelWay.value, $scope.studentID).then(function (data) {
                    //    debugger;
                    //    $scope.TransportationTrackingList = data.data;
                    //});

                    $scope.GetAdvertisementById(e.value);

                    $scope.levelSelectBoxDisabled = true;
                    $scope.PurposeSelectBoxDisabled = true;
                    $scope.radioTravelWayDisabled = true;
                } else {
                    $scope.setAdvertisementDefaultSettings();
                }

                StudentsDesiresByAdminSrvc.GetStudentAllowTicketByAdvertisementId(e.value).then(function (data) {
                    debugger
                    DataSourceStudentList.load();
                });
            },
            placeholder: "--أختر--",
            noDataText: "لا يوجد بيانات",
            pagingEnabled: true, //Pagenation
            showClearButton: true,
            searchEnabled: true,
            displayExpr: "Text",
            valueExpr: "Value"
        };

        $scope.CountryToSelectBox = {
            bindingOptions: {
                dataSource: "CountryList",
                value: "CountryToID",
                items: "CountryList"
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

                MusaferSrvc.GetCountries().then(function (data) {


                    $scope.CountryList = data.data;
                });
            },
            onValueChanged: function (e) {
                $scope.CountryToID = e.value;

                //Get Airport
                MusaferSrvc.GetAirports(e.value).then(function (data) {
                    $scope.AirportToList = data.data;
                });

            }
        };

        $scope.AirportToSelectBox = {
            bindingOptions: {
                dataSource: "AirportToList",
                value: "AirportToID",
                items: "AirportToList"
            },

            placeholder: "--أختر--",
            noDataText: "لا يوجد بيانات",
            pagingEnabled: true, //Pagenation
            showClearButton: true,
            searchEnabled: true,
            displayExpr: "Text",
            valueExpr: "Value",
            disabled: false,

            //onInitialized: function (e) {

            //},
            onValueChanged: function (e) {
                $scope.AirportToID = e.value;
            }
        };


        $scope.CountryFromSelectBox = {
            bindingOptions: {
                dataSource: "CountryList",
                value: "CountryFromID",
                items: "CountryList"
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
                debugger
                MusaferSrvc.GetCountries().then(function (data) {
                    $scope.CountryFromList = data.data;
                });
                $scope.CountryFromID = "83";
            },
            onValueChanged: function (e) {
                $scope.CountryFromID = e.value;

                //Get Airport
                MusaferSrvc.GetAirports(e.value).then(function (data) {
                    $scope.AirportFromList = data.data;
                });
                $scope.AirportFromID = "1882";

            }
        };

        $scope.AirportFromSelectBox = {
            bindingOptions: {
                dataSource: "AirportFromList",
                value: "AirportFromID",
                items: "AirportFromList"
            },

            placeholder: "--أختر--",
            noDataText: "لا يوجد بيانات",
            pagingEnabled: true, //Pagenation
            showClearButton: true,
            searchEnabled: true,
            displayExpr: "Text",
            valueExpr: "Value",
            disabled: false,

            //onInitialized: function (e) {

            //},
            onValueChanged: function (e) {
                $scope.AirportFromID = e.value;
            }
        };


        $scope.LevelSelectBox = {
            bindingOptions: {
                dataSource: "LevelList",
                value: "LevelID",
                items: "LevelList",
                disabled: "levelSelectBoxDisabled"
            },

            placeholder: "--أختر--",
            noDataText: "لا يوجد بيانات",
            paginate: true, //Pagenation
            showClearButton: true,
            searchEnabled: true,
            displayExpr: "Text",
            valueExpr: "Value",


            onInitialized: function (e) {
                MusaferSrvc.GetLevels().then(function (data) {
                    $scope.LevelList = data.data;
                });
            }
            //,
            //onValueChanged: function (e) {
            //    
            //    $scope.LevelID = e.value;
            //}
        };

        $scope.PurposeSelectBox = {
            bindingOptions: {
                dataSource: "PurposeList",
                value: "PurposeID",
                items: "PurposeList",
                disabled: "PurposeSelectBoxDisabled"
            },

            placeholder: "--أختر--",
            noDataText: "لا يوجد بيانات",
            paginate: true, //Pagenation
            showClearButton: true,
            searchEnabled: true,
            displayExpr: "Text",
            valueExpr: "Value",
            onInitialized: function (e) {
                TravelAdvertisementSrvc.GetPurpose().then(function (data) {
                    $scope.PurposeList = data.data;
                });
            }
            //,
            //onValueChanged: function (e) {
            //    $scope.PurposeID = e.value;
            //}
        };

        $scope.radioTravelWay = {
            bindingOptions: {
                value: 'TravelWay',
                disabled: 'radioTravelWayDisabled'
            },
            dataSource: $scope.TravelWayPriorities,
            // value: $scope.TravelWayPriorities[0],
            layout: "horizontal",
            onValueChanged: function (e) {
                debugger;
                if ($scope.studentID != null) {
                    StudentsDesiresByAdminSrvc.GetTransportationTrackingByStudent($scope.TravelWay.value, $scope.studentID).then(function (data) {
                        debugger;
                        $scope.TransportationTrackingList = data.data;
                    });
                }
            }

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
                e.value;
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
                disabled: "dateBoxToDisabled"
            },
            type: "date",
            showClearButton: true,
            onValueChanged: function (e) {
                $scope.EndDate = e.value;
            }
        };


        $scope.btnSave = {
            text: 'حفظ',
            type: 'success',
            onClick: function (e) {

                //if ($scope.advertisementId === null || $scope.advertisementId === '') {
                //    DevExpress.ui.notify({
                //        message: 'عفوا ادخل الاعلان',
                //        type: "error",
                //        displayTime: 3000,
                //        closeOnClick: true 
                //    });
                //    return;
                //}


                if ($scope.studentID === null || $scope.studentID === '') {
                    DevExpress.ui.notify({
                        message: 'عفوا ادخل طالب',
                        type: "error",
                        displayTime: 3000,
                        closeOnClick: true
                    });
                    return;
                }



                //if ($scope.CountryFromID === null || $scope.CountryFromID === '') {
                //    DevExpress.ui.notify({
                //        message: 'عفوا ادخل دولة المغادرة',
                //        type: "error",
                //        displayTime: 3000,
                //        closeOnClick: true
                //    });
                //    return;
                //}

                //if ($scope.AirportFromID === null || $scope.AirportFromID === '') {
                //    DevExpress.ui.notify({
                //        message: 'عفو ادخل مطار المغادرة',
                //        type: "error",
                //        displayTime: 3000,
                //        closeOnClick: true
                //    });
                //    return;
                //}



                //if ($scope.CountryToID === null || $scope.CountryToID === '') {
                //    DevExpress.ui.notify({
                //        message: 'عفواادخل دولة الوصول',
                //        type: "error",
                //        displayTime: 3000,
                //        closeOnClick: true
                //    });
                //    return;
                //}



                //if ($scope.AirportToID === null || $scope.AirportToID === '') {
                //    DevExpress.ui.notify({
                //        message: 'عفوادخل مطار الوصول',
                //        type: "error",
                //        displayTime: 3000,
                //        closeOnClick: true
                //    });
                //    return;
                //}


                //if ($scope.AirportFromID === $scope.AirportToID) {
                //    DevExpress.ui.notify({
                //        message: 'عفوا مطار المغادرة هو نفس مطار الوصول',
                //        type: "error",
                //        displayTime: 3000,
                //        closeOnClick: true
                //    });
                //    return;
                //}

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
                if ($scope.TravelWay.value === 'O') {
                    MusaferSrvc.CheckStudentPayAdvanceAmount(Number($scope.studentID)).then(function (data) {
                        if (data.data.status == 500) {
                            DevExpress.ui.notify({
                                message: data.data.Message,
                                type: data.data.Type,
                                displayTime: 3000,
                                closeOnClick: true
                            });
                            return;
                        }

                    });
                }

                StudentsDesiresByAdminSrvc.SaveStudentDesiresByAdmin(
                    $scope.StudentDesireId,
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
                    if (data.data !== '') {
                        DevExpress.ui.notify({
                            message: data.data,
                            type: "error",
                            displayTime: 3000,
                            closeOnClick: true
                        });
                    } else {
                        DevExpress.ui.notify({
                            message: 'تم الحفظ بنجاح',
                            type: "success",
                            displayTime: 3000,
                            closeOnClick: true
                        });
                        $scope.advertisementId = null;
                        $scope.StudentDesireId = null;
                        $scope.studentID = null;
                        $scope.LevelID = null;
                        $scope.StartDate = null;
                        $scope.EndDate = null;
                        $scope.PurposeID = null;
                        $scope.AirportToID = null;
                        $scope.CountryToID = null;
                        $scope.TransportationTrackingID = null;
                    }

                });


            }
        };

        $scope.validationRequired = {
            validationRules: [
                {
                    type: "required",
                    message: "الحقل مطلوب"
                }
            ]
        };


        /********Passport*******/
        $scope.visiblePassportPopUp = false;
        $scope.PassportNumber = null;
        $scope.ExpiryDate = null;

        $scope.PassportPopUp = {
            width: 480,
            height: 400,
            contentTemplate: "PassportContent",
            showTitle: true,
            title: "تحديث جواز السفر ",
            dragEnabled: false,
            closeOnOutsideClick: false,
            bindingOptions: {
                visible: "visiblePassportPopUp"
            },
            onShowing: function (e) {
                $scope.PassportNumber = null;
                $scope.ExpiryDate = null;
                $("#imgPassport").attr("src", "");
            }
        };


        $scope.txt_PassportNumber = {

            placeholder: "ادخل رقم جواز السفر",
            bindingOptions: {
                value: "PassportNumber"
            },
            onValueChanged: function (e) {
                debugger;
                $scope.PassportNumber = e.value;
            }
        };

        $scope.dP_ExpireDate = {
            bindingOptions: {
                value: "ExpiryDate"
            },
            type: "date",

            onValueChanged: function (e) {
                $scope.ExpiryDate = e.value;
            }
        };


        $scope.multiple = false;
        $scope.accept = "image/*";  //*,.pdf,.doc,.docx,.xls,.xlsx
        $scope.uploadMode = "useButtons";

        $scope.AcceptedPassportFilesvalue = [];
        $scope.UrlUploadPassport = "";
        //var passportImg = new Image();

        $scope.fileUploadPassport = {
            name: "UploadPassport",
            //uploadUrl: "UrlUploadSignature",
            allowCanceling: true,
            rtlEnabled: true,
            readyToUploadMessage: "استعداد للرفع",
            selectButtonText: "اختر الصورة",
            labelText: "",
            uploadButtonText: "رفع",
            uploadedMessage: "تم الرفع",
            invalidFileExtensionMessage: "نوع الصورة غير مسموح",
            uploadFailedMessage: "خطأ أثناء الرفع",
            onInitialized: function (e) {
                $scope.UploadPassportInstance = e.component;
            },
            onValueChanged: function (e) {
                debugger;
                $scope.UrlUploadPassport = "/Musafer/UploadImagePassportByAdmin/" + Number($scope.studentID);

                if (e.value !== null) {
                    var input = $("#fileUploadPassport").dxFileUploader("instance")._$fileInput[0];
                    readUrl(input);

                    if (e.element.find(".dx-fileuploader-files-container .dx-fileuploader-cancel-button"))
                        $("#imgPassport").attr("src", "");
                }
            },
            onUploaded: function (e) {
                debugger;
                var xhttp = e.request;
                if (xhttp.status === 200) {
                    //$scope.AcceptedSignatureFilesvalue = '';
                    //$scope.AcceptedSignatureFilesvalue.reset();
                } else if (xhttp.status === 404) {
                    DevExpress.ui.notify({ message: "الملف المرفوع لايحتوي على بيانات" }, "Error", 10000);
                    $scope.AcceptedPassportFilesvalue = '';
                    $scope.AcceptedPassportFilesvalue.reset();

                } else {
                    DevExpress.ui.notify({ message: "حدث خطأ أثناء رفع الصورة" }, "Error", 10000);
                    $scope.AcceptedPassportFilesvalue = '';
                    $scope.AcceptedPassportFilesvalue.reset();
                }
            },
            onUploadError: function (e) {
                debugger;
                var xhttp = e.request;
                if (xhttp.status === 500) {
                    DevExpress.ui.notify({ message: "حدث خطأ أثناء رفع الملف" }, "Error", 10000);
                    $scope.AcceptedPassportFilesvalue = '';
                    $scope.AcceptedPassportFilesvalue.reset();
                }
            },
            bindingOptions: {
                uploadUrl: "UrlUploadPassport",
                multiple: "multiple",
                accept: "accept",
                value: "AcceptedPassportFilesvalue",
                uploadMode: "uploadMode",
            }
        };

        $("#imgPassport").attr("src", "");

        $scope.btnOpenUpdatesPassport = {
            text: "تحديث جواز السفر",
            onClick: function () {

                if ($scope.studentID === null || $scope.studentID === '')
                    return DevExpress.ui.notify({ message: 'عفوا اختر طالب', type: "error", displayTime: 3000, closeOnClick: true });
                else
                    $scope.visiblePassportPopUp = true;
            }
        };

        $scope.btnUpdatePassport = {
            text: 'حفظ',
            type: 'success',
            onClick: function (e) {
                debugger;
                if ($scope.studentID === null || $scope.studentID === '')
                    return DevExpress.ui.notify({ message: 'عفوا اختر طالب', type: "error", displayTime: 3000, closeOnClick: true });

                if ($scope.PassportNumber === null || $scope.PassportNumber === '')
                    return DevExpress.ui.notify({ message: 'عفوا ادخل رقم جواز السفر', type: "error", displayTime: 3000, closeOnClick: true });

                if ($scope.ExpiryDate === null || $scope.ExpiryDate === '')
                    return DevExpress.ui.notify({ message: 'عفوا ادخل تاريخ انتهاء جواز السفر', type: "error", displayTime: 3000, closeOnClick: true });

                if ($scope.AcceptedPassportFilesvalue.length <= 0)
                    return DevExpress.ui.notify({ message: "ارفع صورة جواز السفر", type: "error", displayTime: 3000, closeOnClick: true });

                StudentsDesiresByAdminSrvc.SaveUpdatePassport({ Student_ID: $scope.studentID, IdentityNumber: $scope.PassportNumber, ExpiryDate: $scope.ExpiryDate })
                    .then(function (data) {
                        if (data.data.status === 200) {
                            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                            //Clear Data
                            $scope.studentID = null;
                            $scope.PassportNumber = null;
                            $scope.ExpiryDate = null;
                            $scope.AcceptedPassportFilesvalue = [];

                            $scope.visiblePassportPopUp = false;
                        }
                        if (data.data.status === 500) {
                            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                        }
                    });
            }
        };

        function readUrl(input) {
            debugger;
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    debugger;
                    $("#imgPassport").attr("src", e.target.result);
                };
                reader.readAsDataURL(input.files[0]);
            }
        };
    }]);