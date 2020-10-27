app.service('PayRollSettingSrvc', ["$http", function ($http) {
    this.GetAllPayRollPhases = function () {
        return $http({
            method: 'GET',
            url: '/PayRollSetting/GetAllPayRollPhases'
        });
    };


    this.GetPayrollPhasesId = function (id) {
        return $http({
            method: 'GET',
            url: '/PayRollSetting/GetPayrollPhasesId/' + id
        });
    };

    this.addNewPayRollPhase = function (PayRollPhase) {
        return $http({
            method: 'POST',
            url: '/PayRollSetting/addNewPayRollPhase',
            data: {
                "ID": PayRollPhase.ID, "PhaseName": PayRollPhase.PhaseName, "PhaseOrder": PayRollPhase.PhaseOrder, "IsFinancialApprove": PayRollPhase.IsFinancialApprove
                , "IssuingExchangeOrder": PayRollPhase.IssuingExchangeOrder , "IssuingPaymentOrder": PayRollPhase.IssuingPaymentOrder
            }
        });
    };

    this.addPayRollPhasesUsers = function (SelectedPhasesId, SelectedUserId, IsActive) {
        return $http({
            method: 'POST',
            url: '/PayRollSetting/addPayRollPhasesUsers',
            data: { "PayrollPhaseID": SelectedPhasesId, "UserID": SelectedUserId, "IsActive": IsActive}
        });
    };

    this.UpdateRow = function (PayrollPhaseID, UserID, ID) {
        return $http({
            method: 'POST',
            url: '/PayRollSetting/UpdateRow',
            data: { "PayrollPhaseID": PayrollPhaseID, "UserID": UserID, "ID": ID}
        });
    };

    this.GetAllUsers = function () {
        return $http({
            method: 'GET',
            url: '/PayRollSetting/GetAllUsers'
        });
    };
    this.GetPayRollPhasesUsers = function () {
        return $http({
            method: 'GET',
            url: '/PayRollSetting/GetPayRollPhasesUsers'
        });
    };
    this.GetmaxOrder = function () {
        return $http({
            method: 'GET',
            url: '/PayRollSetting/GetmaxOrder'
        });
    };

    this.IsActiveEditing = function (PayrollPhasesUsersId, Status) {
        debugger;
        return $http({
            method: "POST",
            url: "/PayRollSetting/IsActiveEditing",
            data: {
                PayrollPhasesUsersId: PayrollPhasesUsersId,
                status: Status
            }
        });
    };

    this.GetPhaseName = function (ID) {
        return $http({
            method: 'POST',
            url: '/PayRollSetting/GetPhaseName',
            data: { "ID": ID }
        });
    };

    this.GetUserName = function (ID) {
        return $http({
            method: 'POST',
            url: '/PayRollSetting/GetUserName',
            data: { "ID": ID}
        });
    };
    }]);
