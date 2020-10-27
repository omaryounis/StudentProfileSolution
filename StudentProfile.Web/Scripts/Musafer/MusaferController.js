app.controller("MusaferCtrl", ["$scope", '$location', 'MusaferSrvc', function ($scope, $location, MusaferSrvc) {
    /*Start Virify*/
    var urlParams = new URLSearchParams(window.location.search);
    $scope.id = urlParams.get('adID');

    //Filed
    $scope.MailCode = null;
    $scope.MobileCode = null;
    $scope.StData = {};
    AdvertisementList = [];

    $scope.StPassportImage = null;

    // For Confirm Code Send
    $scope.CheckConfirmMail = 0;
    $scope.CheckConfirmPhone = 0;

    //Advertisement Data
    $scope.ValidateMinDeparting = null;
    $scope.ValidateMaxDeparting = null;
    $scope.ValidateMinReturning = null;
    $scope.ValidateMaxReturning = null;
    $scope.AdName = null;
    $scope.TravelClass = null;
    $scope.FlightsType = null;
    $scope.Purpose = null;
    $scope.RequestId = 0;

    $scope.disabledDateReturning = false;


    //GetAdvertisementById
    MusaferSrvc.GetAdvertisementById($scope.id).then(function (data) {
        debugger;
        $scope.ValidateMinDeparting = new Date(data.data.DepartingStart);
        $scope.ValidateMaxDeparting = new Date(data.data.DepartingEnd);
        $scope.ValidateMinReturning = new Date(data.data.ReturningStart);
        $scope.ValidateMaxReturning = new Date(data.data.ReturningEnd);
        $scope.AdName = data.data.AdName;
        $scope.TravelClass = data.data.TravelClass;
        $scope.FlightsType = data.data.FlightsType;
        $scope.Purpose = data.data.Purpose;
        $scope.LevelID = data.data.TravelClass.toString();
        $scope.TransportationTrackingID = data.data.TransportationTrackingID;
        MusaferSrvc.GetTransportationTracking($scope.FlightsType).then(function (data) {
            debugger;
            $scope.TransportationTrackingList = data.data;
        });
        if (data.data.FlightsType === "O")
        {
            $scope.disabledDateReturning = true;
        }
        else
        {
            $scope.disabledDateReturning = false;
        }
            
    });

    //Control
    $scope.txtMailCode = {
        bindingOptions: {
            value: "MailCode"
        },
        placeholder: "كود البريد الالكتروني",
        onValueChanged: function (e) {
            $scope.MailCode = e.value;
        }
    }

    $scope.txtMobileCode = {
        bindingOptions: {
            value: "MobileCode"
        },
        placeholder: "كود الجوال",
        onValueChanged: function (e) {
            $scope.MobileCode = e.value;
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

    $scope.btnVerify = {
        text: 'ارسال الكود',
        type: 'primary',
        onClick: function (e) {
            MusaferSrvc.SendEmailConfirmedCode().then(function (data) {
                if (data.data.status === 200) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    $scope.CheckConfirmMail = data.data.status;
                }
                else if (data.data.status === 500) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    $scope.CheckConfirmMail = data.data.status;
                }
            });

            MusaferSrvc.SendSMSConfirmedCode().then(function (data) {
                if (data.data.status === 200) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    $scope.CheckConfirmPhone = $scope.CheckConfirmMail = data.data.status;
                }
                else if (data.data.status === 500) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    $scope.CheckConfirmPhone = $scope.CheckConfirmMail = data.data.status;
                }
            });
        }
    };

    $scope.btnConfirm = {
        text: 'تاكيد',
        type: 'primary',
        onClick: function (e) {
            //MusaferSrvc.ConfirmCode({ MailCode: $scope.MailCode, MobileCode: $scope.MobileCode }).then(function (data) {
            //    if (data.data.status === 500) {
            //        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
            //    }
            //    else {
            //        debugger;
            //        //Get Stdata 
            //        $scope.StData = data.data;

            //        //Move Tab 
            //        enable_tab(2, 1);
            //        //$("#txtMailCode").prop('disabled', true);
            //        //$("#txtMobileCode").prop('disabled', true);
            //    }
            //});
        }
    };

    $scope.GetStudentPassportImage = function () {
        MusaferSrvc.GetStudentPassportImage().then(function (data) {
            //Get Stdata 
            $scope.StPassportImage = data.data;
        });
    }
    $scope.GetStudentPassportImage();
    /*End Virify*/

    /*Start Confirm Student Data*/
    //Filed
    $scope.ChkStData = false;

    //Control
    $scope.StDataCheckBox = {
        bindingOptions: { value: "ChkStData" },
        text: "اقر ان هذة البيانات هي البيانات الخاصة بي",
        rtlEnabled: true
    };

    $scope.btnConfirmStData = {
        text: 'التالي',
        type: 'primary',
        onClick: function (e) {
            debugger;
            if ($scope.StData.UserImage === null) {
                DevExpress.ui.notify({
                    message: "بانتظار مراجعة الصورة من قبل الادارة",
                    type: "seccuss",
                    displayTime: 3000,
                    closeOnClick: true
                });
            }
            else {
                //Move Tab 
                enable_tab(3, 2);

                //Get Advertisement By Id
                //MusaferSrvc.GetAdvertisementById(id).then(function (data) {
                //    debugger;
                //    $scope.ValidateMinDeparting = data.data.DepartingStart;
                //    $scope.ValidateMaxDeparting = data.data.DepartingEnd;
                //    $scope.ValidateMinReturning = data.data.ReturningStart;
                //    $scope.ValidateMaxReturning = data.data.ReturningEnd;
                //});
            }
        }
    };
    //Function
    /*End Confirm Student Data*/

    /* Start Send Request*/
    //Filed
    $scope.TransportationTrackingList = [];
    $scope.AirportList = [];
    $scope.LevelList = [];
    $scope.CountryID = null;
    $scope.TransportationTrackingID = null;
    $scope.AirportID = null;
    $scope.LevelID = null;
    $scope.Reson = null;
    $scope.StartDate = null;
    $scope.EndDate = null;
    //RadioPriorities
    $scope.TravelWayPriorities = [{ value: 'R', text: "ذهاب وعودة" }, { value: 'O', text: "ذهاب فقط" }];
    $scope.TravelWay = 'R'; // value TravelWay


    //Check Student Ticket Advertisement For Edit
    MusaferSrvc.GetStudentTicketAdvertisementForEdit($scope.id).then(function (data) {
        debugger;
        if (data.data.status !== 500) {
            //Fill Data 
            $scope.RequestId = data.data.ID;
            //$scope.CountryID = data.data.Country.toString();
            //$scope.AirportID = data.data.AirPortTo.toString();
            $scope.StartDate = data.data.Departing;
            $scope.EndDate = data.data.Returning;
            $scope.TransportationTrackingID = data.data.TransportationTrackingID.toString();
          //  $scope.Nationality_ID = data.data.Nationality_ID;
        }
    });

    //Control
    //$scope.CountrySelectBox = {
    //    bindingOptions: {
    //        dataSource: "TransportationTrackingList",
    //        value: "CountryID",
    //        items: "TransportationTrackingList"
    //    },

    //    placeholder: "--أختر--",
    //    noDataText: "لا يوجد بيانات",
    //    pagingEnabled: true, //Pagenation
    //    showClearButton: true,
    //    searchEnabled: true,
    //    displayExpr: "Text",
    //    valueExpr: "Value",
    //    disabled: false,

    //    onInitialized: function (e) {
    //        debugger;
    //        MusaferSrvc.GetCountries().then(function (data) {
    //            debugger;
    //            $scope.TransportationTrackingList = data.data;
    //        });
    //    },
    //    onValueChanged: function (e) {
    //        $scope.CountryID = e.value;

    //        //Get Airport
    //        MusaferSrvc.GetAirports(e.value).then(function (data) {
    //            $scope.AirportList = data.data;
    //        });

    //    },
    //};
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
        onOpened: function (e) {
            debugger;
            MusaferSrvc.GetTransportationTracking($scope.FlightsType).then(function (data) {
                debugger;
                $scope.TransportationTrackingList = data.data;
            });
        },
        onValueChanged: function (e) {
            $scope.TransportationTrackingID = e.value;
            //Get Airport
            //MusaferSrvc.GetAirports(e.value).then(function (data) {
            //    $scope.AirportList = data.data;
            //});

        },
    };

    //$scope.AirportSelectBox = {
    //    bindingOptions: {
    //        dataSource: "AirportList",
    //        value: "AirportID",
    //        items: "AirportList"
    //    },

    //    placeholder: "--أختر--",
    //    noDataText: "لا يوجد بيانات",
    //    pagingEnabled: true, //Pagenation
    //    showClearButton: true,
    //    searchEnabled: true,
    //    displayExpr: "Text",
    //    valueExpr: "Value",
    //    disabled: false,

    //    //onInitialized: function (e) {

    //    //},
    //    onValueChanged: function (e) {
    //        $scope.AirportID = e.value;
    //    }
    //};

    $scope.LevelSelectBox = {
        bindingOptions: {
            dataSource: "LevelList",
            value: "LevelID",
            items: "LevelList"
        },

        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        pagingEnabled: true, //Pagenation
        showClearButton: true,
        searchEnabled: true,
        displayExpr: "Text",
        valueExpr: "Value",
        disabled: true,

        onInitialized: function (e) {
            MusaferSrvc.GetLevels().then(function (data) {
                $scope.LevelList = data.data;
            });
        },
        onValueChanged: function (e) {
            $scope.LevelID = e.value;
        }
    };

    $scope.txtReson = {
        bindingOptions: {
            value: "Purpose"
        },
        disabled: true,
        placeholder: "الغرض من السفر",
        onValueChanged: function (e) {
            $scope.Reson = e.value;
        }
    }

    //Date
    $scope.dateBoxFrom = {
        bindingOptions: {
            value: "StartDate",
            min: "ValidateMinDeparting",
            max: "ValidateMaxDeparting"
        },
        type: "date",

        onValueChanged: function (e) {
            $scope.StartDate = convert(e.value);
            if ($scope.id === 0) {
                $scope.EndDate = null;
            }
        }
    };

    $scope.dateBoxTo = {
        bindingOptions: {
            value: "EndDate",
            min: "ValidateMinReturning",
            max: "ValidateMaxReturning",
            disabled:"disabledDateReturning"
        },
        type: "date",

        onValueChanged: function (e) {
            debugger;
            $scope.EndDate = convert(e.value);
        }
    };
    function convert(str) {
        var date = new Date(str),
            mnth = ("0" + (date.getMonth() + 1)).slice(-2),
            day = ("0" + date.getDate()).slice(-2);
        return [mnth, day, date.getFullYear()].join("/");
    }
    $scope.radioTravelWay = {
        dataSource: $scope.TravelWayPriorities,
        value: $scope.FlightsType === "R" ? $scope.TravelWayPriorities[1] : $scope.TravelWayPriorities[0],
        layout: "vertical",
        disabled: true,
        onValueChanged: function (e) {
            $scope.TravelWay = e.value.value;
        }
    };

    //btn Save
    $scope.btnSave = {
        text: 'حفظ',
        type: 'success',
        onClick: function () {
            MusaferSrvc.SaveRequest({
                ID: $scope.RequestId, TransportationTrackingID: $scope.TransportationTrackingID,
                Departing: $scope.StartDate, Returning: $scope.EndDate, AdvertisementID: $scope.id
            }).then(function (data) {
                if (data.data.status === 500) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                }
                if (data.data.status === 200) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    location.href = "/Musafer/StudentAdvertisement";
                }
            });
        }
    };

    //Function
    $scope.SaveRequest = function () {
        debugger;
        MusaferSrvc.SaveRequest({
            ID: $scope.RequestId, TransportationTrackingID: $scope.TransportationTrackingID,           
            Departing: $scope.StartDate, Returning: $scope.EndDate, AdvertisementID: $scope.id
        }).then(function (data) {
            if (data.data.status === 500) {
                DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
            }
            if (data.data.status === 200) {
                DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                location.href = "/Musafer/StudentAdvertisement";
            }
        });
    }
    /* End Send Request*/

    //UploadImage
    $scope.UploadImage = function (e) {
        debugger;
        var file = (event.target.files[0]);
        var formdata = new FormData();
        var fileInput = document.getElementById('UploadImage');
        formdata.append(fileInput.files[0].name, fileInput.files[0]);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/Musafer/UploadImage');
        xhr.send(formdata);
        //success
        DevExpress.ui.notify({
            message: "تم رفع الصورة بانتظار مراجعتها من قبل الادارة",
            type: "success",
            displayTime: 3000,
            closeOnClick: true
        });
    }

}]);

//click event
function eventFire(el, etype) {
    if (el.fireEvent) {
        el.fireEvent('on' + etype);
    } else {
        var evObj = document.createEvent('Events');
        evObj.initEvent(etype, true, false);
        el.dispatchEvent(evObj);
    }
}

//ENABLE WIZARD TAB
function enable_tab(activeID, oldID) {
    $("#step" + activeID + "_tab").attr('data-toggle', 'tab');
    $("#step" + activeID + "_tab").attr('aria-controls', 'step' + activeID);
    $("#step" + activeID + "_tab").attr('role', 'tab');
    $("#step" + activeID + "_tab").attr('title', 'Step ' + activeID);
    eventFire(document.getElementById("step" + activeID + "_tab"), 'click');
    $("#step" + oldID + "_tab span").addClass('success-step');
}