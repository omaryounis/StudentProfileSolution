app.service("StudentsDesiresByAdminSrvc", ['$http', 'GenericService', function ($http, GenericService) {
    this.GetActiveAdvertisement = function () {
        return GenericService.GetAll("/Musafer/GetActiveAdvertisement");
    };

    this.GetStudentDesireDetails = function (ID) {
        return $http({
            method: "GET",
            url: "/Musafer/GetStudentDesireDetails",
            params: { StudentDesireId: Number(ID) },
            contentType: "application/json;"
        }).then(function (response) {
            return response;
        }, function (err) {
            return err;
        });
    };

    //this.GetStudents = function () {
    //    return $http({
    //        method: "GET",
    //        url: "/Musafer/GetStudents"
    //    });
       

    //};

    this.GetStudents = function () {
        return GenericService.GetAll("/Musafer/GetStudents");
    };




    this.GetStudentByadvertisementIdForAdmin = function (advertisementId) {
        var response = $http({
            method: "GET",
            url: "/Musafer/GetStudentByadvertisementIdForAdmin",
            params: {
                advertisementId: Number(advertisementId)
            }
        });

        return response;
    };

    this.GetStudentAllowTicketByAdvertisementId = function (id) {
        return GenericService.GetByID("/Musafer/GetStudentAllowTicketByAdvertisementId", id);
    };
    this.GetTransportationTrackingByStudent = function (FlightsType, StudentID) {
        return GenericService.GetAll("/Musafer/GetTransportationTrackingByStudent?FlightsType=" + FlightsType + "&StudentID=" + StudentID);
    };

    this.SaveStudentDesiresByAdmin = function (StudentDesireId, studentId, travelClasses, flightType, departureDate, returnDate, travelPurpose, advertisementId, isPassportVerified,  TransportationTrackingID) {
        var response = $http({
            method: "POST",
            url: "/Musafer/SaveStudentDesiresByAdmin",
            data: {
                'StudentDesireId': StudentDesireId,
                'studentId': studentId,
                'travelClasses': travelClasses,
                'flightType': flightType,
                'departureDate': departureDate,
                'returnDate': returnDate,
                'travelPurpose': travelPurpose,
                'advertisementId': advertisementId,
                'isPassportVerified': isPassportVerified,
                'TransportationTrackingID': TransportationTrackingID
            }
        });

  
        return response;
    };

      this.SaveUpdatePassport = function (model) {
            return GenericService.Post("/Musafer/SaveUpdatePassport", model);
        };

}]);