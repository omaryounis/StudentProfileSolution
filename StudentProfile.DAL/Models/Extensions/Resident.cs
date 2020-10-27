using System;

public class Resident
{
    public string IdNumber { get; set; }
    public string Name { get; set; }
    public string Gender { get; set; }
    public string Nationality { get; set; }
    public string Profession { get; set; }
    public string PassportNumber { get; set; }
    public DateTime PassportEndDateHijri { get; set; }
    public DateTime PassportEndDate { get; set; }
    public DateTime ResidenceEndDateHijri { get; set; }
    public DateTime ResidenceEndDate { get; set; }
    public string IsValid { get; set; }
}