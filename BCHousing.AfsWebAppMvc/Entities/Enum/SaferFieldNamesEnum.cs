namespace BCHousing.AfsWebAppMvc.Entities.Enum
{
    public static class SaferFieldNamesEnum
    {
        public static long GetNumberOfFields()
        {
            return EnumFieldNames.Count;
        }

        public static string GetFieldNameBySequence(int sequence)
        {
            return EnumFieldNames[sequence - 1];
        }

        private static List<string> EnumFieldNames { get; set; } = new List<string>()
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
            "Address(es)",                  //32
            "From Date \n (dd/mm/yyyy)",
            "To Date \n (dd/mm/yyyy)",
            "Landlord Name",
            "Landlord Phone #",             //36
            "Address(es)",                  //37
            "From Date \n (dd/mm/yyyy)",
            "To Date \n (dd/mm/yyyy)",
            "Landlord Name",
            "Landlord Phone #",             //41
            "Name",                                     //42
            "Date moved to Canada (dd/mm/yyyy)",
            "Current status in Canada",
            "Name of Sponsor", 
            "End Date of Sponsorship Agreement",        //46
            "Name",                                     //47
            "Date moved to Canada (dd/mm/yyyy)",
            "Current status in Canada",
            "Name of Sponsor",
            "End Date of Sponsorship Agreement",                //51
            "Living Alone",                                     //52
            "Living with a spouse or common-law partner",
            "Sharing with another adult(s)",
            "Other",                                            //55
            "describe:",
            "Last Name",                                        //56
            "Given Names",                                      //57
            "Replationship to Applicant",                        //58
            "Birth Date* (dd/mm/yyyy)",                          //59
            "Age",                                              //60
            "Gender* (M/F)",                                    //61
            "Last Name",                                        //63
            "Given Names",                                      //64
            "Replationship to Applicant",                        //65
            "Birth Date* (dd/mm/yyyy)",                          //66
            "Age",                                              //67
            "Gender* (M/F)",                                    //68
            "Last Name",                                        //69
            "Given Names",                                      //70
            "Replationship to Applicant",                        //71
            "Birth Date* (dd/mm/yyyy)",                          //72
            "Age",                                              //73
            "Gender* (M/F)",                                    //74
            "Last Name",                                        //75
            "Given Names",                                      //76
            "Replationship to Applicant",                        //77
            "Birth Date* (dd/mm/yyyy)",                          //78
            "Age",                                              //79
            "Gender* (M/F)",                                    //80
            "Last Name",                                        //81
            "Given Names",                                      //82
            "Replationship to Applicant",                        //83
            "Birth Date* (dd/mm/yyyy)",                          //84
            "Age",                                              //85
            "Gender* (M/F)",                                    //86
            "Yes",                                              //87
            "No",                                               //88
            "First Nations",                                    //89
            "Metis",                                            //90
            "Inuit",                                            //91
            "Other",                                            //92
            "Home Phone #",                                     //93
            "Work Phone #",                                     //94
            "Cell Phone #",                                     //95
            "Email",                                            //96
            "Optional: Name of person we can leave messages with",      //97
            "Message person phone number",                      //98
            "Optional: Authorized Contact* name and relationship to you",   //99
            "Authorized Contact phone number",                  //100
            "If Applicable: Power of Attorney name",            //101
            "Power of Attorney phone number",                   //102
            "Apt #",                                            //103
            "Street #",                                         //104
            "Street Name",                                      //105
            "City",                                             //106
            "Postal Code",                                      //107
            "Apt #",                                            //108
            "Street #",                                         //109
            "Street Name",                                      //110
            "City",                                             //111
            "Postal Code",                                      //112
            "Landlord Name",                                    //113
            "Landlord Phone",                                   //114
            "Landlord Address",                                 //115
            "Rent",                                             //116
            "Own",                                              //117
            "Life Lease",                                       //118
            "Rent-to-own",                                      //119
            "How much is your rent",                            //120
            "Monthly",                                          //121
            "Weekly",                                           //122
            "Nightly/Daily",                                    //123
            "Yes",                                              //124
            "No",                                               //125
            "Yes",                                              //126
            "No",                                               //127
            "Yes",                                              //128
            "No",                                               //129
            "If Yes, how many meals per day?",                  //130
            "Yes",                                              //131
            "No",                                               //132
            "I live a self-contained unit (apartment, house, townhouse)",               //133
            "I live with family or friends (other than spouse common law partner)",      //134
            "I live in a self-contained basement suite",                                //135
            "I live in a Housing Co-operative",                                         //136
            "I live in a Manufactured/Trailer/Mobile home",                             //137
            "I live in a Hotel/Motel",                                                  //138
            "Other",                                                                    //139
            "(describe)",                                                               //140
            "Own",                                                                      //141
            "Rent",                                                                     //142
            "Trailer Rent",                                                             //143
            "Yes",                                                                      //144
            "No",                                                                       //145
            "Pad Rent"                      //146 - end page 4



        };

    }
}
