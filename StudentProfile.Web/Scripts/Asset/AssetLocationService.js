app.service("AssetService", function ($http) {

    //get All Journal Entry
    this.GetAllJournalEntries = function () {
        return $http.get("/JournalEntry/GetAllJournalEntries");
    };
    this.GetDefaultCompany = function () {
        return $http.get("/Asset/GetDefaultCompany");
    };
    // get Journal Entry by Journal Entry Id
    this.getJournalEntry = function (journalEntryId) { 
        var response = $http({
            method: "post",
            url: "/JournalEntry/GetJournalEntryById",
            params: {
                id: JSON.stringify(journalEntryId)
            }
        });
        return response;
    } 

    // Update Journal Entry 
    this.updateJournalEntry = function (journalEntry) {
        var response = $http({
            method: "post",
            url: "/JournalEntry/UpdateJournalEntry",
            data: JSON.stringify(journalEntry),
            dataType: "json"
        });
        return response;
    }

    // Add Journal Entry
    this.AddJournalEntry = function (journalEntry) {
        var response = $http({
            method: "post",
            url: "/JournalEntry/AddJournalEntry",
            data: JSON.stringify(journalEntry),
            dataType: "json"
        });
        return response;
    }
   
    //Delete Journal Entry

    this.DeleteJournalEntry = function (journalEntryId) {
        var response = $http({
            method: "post",
            url: "/JournalEntry/DeleteJournalEntry",
            params: {
                journalEntryId: JSON.stringify(journalEntryId)
            }
        });
        return response;
    }

    this.getDetails = function (id) {
        var response = $http({
            method: "GET",
            url: "/JournalEntry/GetListDetails/"+id,
            params: {
                //JobListId: JSON.stringify(listId)
            }
        });
    
        return response;
    }
    this.GetOperationNo = function (id) {
        var response = $http({
            method: "GET",
            url: "/JournalEntry/GetOperationNo/" + id,
            params: {
                //JobListId: JSON.stringify(listId)
            }
        });

        return response;
    }
});