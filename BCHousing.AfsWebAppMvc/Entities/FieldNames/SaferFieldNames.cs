namespace BCHousing.AfsWebAppMvc.Entities.FieldNames
{
    public static class SaferFieldNames
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
            "Pad Rent",                      //146 - end page 4
            "Yes",                      //147
            "No",                       //148
            "If yes, please describe",   //149
            "Yes",                      //150
            "No",                       //151
            "If yes, when did you last work? (Month/Year)",      //152
            "Yes",                      //153
            "No",                       //154
            "Yes",                      //155
            "No",                       //156
            "If yes, when was the last payment received? (Month/Year)",      //157
            "Yes",                      //158
            "No",                       //159
            "Yes",                      //160
            "No",                       //161
            "If yes, please describe and attach supporting documentation",    //162
            "Yes",                      //163
            "No",                       //164
            "List all currente Income Sources including any regular ongoing funds received from non-taxable Sources:",       //165
            "APPLICANT",                //166
            "SPOUSE",                   //167
            "List all currente Income Sources including any regular ongoing funds received from non-taxable Sources:",       //168
            "APPLICANT",                //169
            "SPOUSE",                   //170
            "List all currente Income Sources including any regular ongoing funds received from non-taxable Sources:",       //171
            "APPLICANT",                //172
            "SPOUSE",                   //173
            "List all currente Income Sources including any regular ongoing funds received from non-taxable Sources:",       //174
            "APPLICANT",                //175
            "SPOUSE",                   //176
            "List all currente Income Sources including any regular ongoing funds received from non-taxable Sources:",       //177
            "APPLICANT",                //178
            "SPOUSE",                   //179
            "List all currente Income Sources including any regular ongoing funds received from non-taxable Sources:",       //180
            "APPLICANT",                //181
            "SPOUSE",                   //182 End of page 5
            "Signature of Applicant",    //183
            "Date",                     //184
            "Signature of Spouse (if applicable)",      //185
            "Date",                     //186 End of page 6
            "A printed, personalized blank cheque marked VOID; or",         //187
            "A Preauthorized Debit Form provided by your financial institution; or",        //188
            "Have your financial institution complete the information below:",              //189
            "Name of Applicant",                //190
            "Transit Number",                   //191
            "Bank Number",                      //192
            "Account Number",                   //193
            "Name(s) on the account",           //194
            "Phone number of financial institution",     //195
            "Financial Institution Stamp",          //196
            "Landlord / Building Manager Name (Print)",         //197
            "Rental address (Unit #, Street#, City)",            //198
            "Print Tenant's Name(s)",                           //199    
            "Date tenancy started (MM/DD/YY)",                  //200
            "The Rent is $",                                    //201
            "Month",                                            //202
            "Week",                                             //203
            "Night",                                            //204
            "Yes",                                              //205
            "No",                                               //206
            "Landlord Signature",                               //207
            "Landlord Phone #",                                 //208
            "Date",                                             //209
            "Birth or baptismal certificate, Passport, Driver's License or a BC ID Card",       //210
            "If born in Canada, Copy of Canadian birth of baptismal certificate, or Passport",  //211
            "If not born in Canada, documentation showing date of birth as well as your status in Canada and that you are not under private sposorship. For more information, please call 604-433-2218 or toll-free at 1-800-257-7756",  //212
            "Attach Power of Attorney authorizing documents",                   //213
            "Attach a personalized blank cheque marked VOID to the application form; or",       //214
            "Attach a Preauthorized Debit Form provided by your financial institution; or",     //215
            "Have your financial institution complete the SAFER Direct Deposit section of this application",    //216
            "Rent Receipt showing address, rent amount, date and landlord name; or",            //217
            "Copy of recent Rent Increase Notice; or",            //218
            "Copy of Lease or Tenancy Agreement (if signed within the past 12 months); or",              //219
            "Have your landlord complete the Proof of Rent - Landlord Declaration section of this application",                                                                 //220
            "Provide consent for release of tax information from Canada Revenue Agency (CRA) on page 2 of this  application; or",                                               //221
            "Provide copies of last year's Income Tax Notice of Assessment AND detailed Income Tax Return (include all pages); or T-slips from all income sources.",            //222
            "Statement of Income and Expenses from last year's Income Tax return and all related worksheets (form T2125)",                                                      //223
            "Proof of CURRENT gross monthly income, from all sources (cheque stubs, letter from employer bank statements showing direct deposits or other income statement",    //224
            "The T5007 tax slip that indicates the bus pass benefit amount"                                                                                                     //225
        };

    }
}
