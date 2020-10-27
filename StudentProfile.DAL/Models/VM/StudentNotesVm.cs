using System;
using System.Collections.Generic;

public class StudentNotesVm
{
    public int NoteId { get; set; }
    public DateTime NoteDate { get; set; }
    public string NoteDetails { get; set; }
    public int IssueId { get; set; }
    public bool IsSecret { get; set; }
    public List<StudentFilesVm> StudentFiles { get; set; }
}