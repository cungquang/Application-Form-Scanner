namespace BCHousing.AfsWebAppMvc.Entities.Enum
{
    public static class SaferFieldNamesEnum
    {
        public static string GetFieldNameBySequence(int sequence)
        {
            return EnumValues[sequence - 1];
        }

        private static List<string> EnumValues { get; set; } = new List<string>()
        {
            "Social Insurance Number",
            "Last Name",
            "First Name(s)",
            "Birth Date (dd/mm/yyyy)",
            "Age",
            "Gender",
            "Born in Canada? (Yes/No)",
            "Social Insurance Number",
            "Last Name",
            "First Name(s)",
            "Birth Date (dd/mm/yyy)",
            "Age",
            "Gender",
            "Born in Canada? (Yes/No)",
            "Option 1: Consent Granted",
            "Option 2: Consent Not Granted",
            "Applicant Print Name",
            "Signature",
            "Date",
            "Spouse Print Name",
            "Signature",
            "Date",
            "Yes",
            "No",
            "If no, when did you move to B.C.?",
            "How long have you lived in Canada?",
            "Address(es)",
            "From Date \n (dd/mm/yyyy)",
            "To Date \n (dd/mm/yyyy)",
            "Landlord Name",
            "Landlord Phone #",
            "Address(es)",
            "From Date \n (dd/mm/yyyy)",
            "To Date \n (dd/mm/yyyy)",
            "Landlord Name",
            "Landlord Phone #",
            "Name",
            "Date moved to Canada (dd/mm/yyyy)",
            "Current status in Canada",
            "Name of Sponsor", 
            "End Date of Sponsorship Agreement",
            "Name",
            "Date moved to Canada (dd/mm/yyyy)",
            "Current status in Canada",
            "Name of Sponsor",
            "End Date of Sponsorship Agreement",
            "Living Alone",
            "Living with a spouse or common-law partner",
            "Sharing with another adult(s)",
            "Other, describe:",
            "Other, describe:",


        };

    }
}
