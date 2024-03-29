﻿namespace BCHousing.AfsWebAppMvc.Entities.FieldNames
{
    public static class RapFieldNames
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
            "Social Insurance Number",      //1
            "Last Name",                    //2
            "First Name(s)",                //3
            "Birth Date (dd/mm/yyyy)",      //4
            "Age",                          //5
            "Gender",                       //6
            "Born in Canada? (Yes/No)",     //7
            "Social Insurance Number",      //8
            "Last Name",                    //9
            "First Name(s)",                //10
            "Birth Date (dd/mm/yyyy)",      //11
            "Age",                          //12
            "Gender",                       //13
            "Born in Canada? (Yes/No)",     //14
            "Option 1: Consent Granted",    //15
            "Option 2: Consent Not Granted",        //16
            "Print Name",                   //17
            "Signature",                    //18
            "Date",                         //19
            "Print Name",                   //20
            "Signature",                    //21
            "Date",                         //22 End of page 1
            "Yes",                          //23
            "No",                           //24
            "If no, when did you move to B.C",      //25
            "How long have you lived in Canada?",   //26
            "Address(es)",                          //27
            "From Date (dd/mm/yyyy)",               //28
            "To Date (dd/mm/yyyy)",                 //29
            "Landlord Name",                        //30
            "Landlord Phone #",                     //31
            "Address(es)",                          //32
            "From Date (dd/mm/yyyy)",               //33
            "To Date (dd/mm/yyyy)",                 //34
            "Landlord Name",                        //35
            "Landlord Phone #",                     //36
            "Address(es)",                          //37
            "From Date (dd/mm/yyyy)",               //38
            "To Date (dd/mm/yyyy)",                 //39
            "Landlord Name",                        //40
            "Landlord Phone #",                     //41
            "Name",                                     //42
            "Date moved to Canada (dd/mm/yyyy)",        //43
            "Current status in Canada",                 //44
            "Name of Sponsor",                          //45
            "End Date of Sponsorship Agreement",        //46
            "Name",                                     //47
            "Date moved to Canada (dd/mm/yyyy)",        //48
            "Current status in Canada",                 //49
            "Name of Sponsor",                          //50
            "End Date of Sponsorship Agreement",        //51
            "Single - Never Married",                   //52
            "Widowed",                                  //53
            "Divorced or Separated",                    //54
            "Date Separated or Divorced",               //55
            "Married or Common Law",                    //56
            "Yes",                                      //57
            "No",                                       //58
            "If No, provide their address",             //59
            "Relationship To Applicant",                //60
            "Last Name",                                //61
            "Given Names",                              //62
            "Birth Date* (dd/mm/yyyy)",                 //63
            "Age*",                                     //64
            "Gender* (M/F/O)",                          //65
            "Rent Contribution**",                      //66
            "Relationship To Applicant",                //67
            "Last Name",                                //68
            "Given Names",                              //69
            "Birth Date* (dd/mm/yyyy)",                 //70
            "Age*",                                     //71
            "Gender* (M/F/O)",                          //72
            "Rent Contribution**",                      //73
            "Relationship To Applicant",                //74
            "Last Name",                                //75
            "Given Names",                              //76
            "Birth Date* (dd/mm/yyyy)",                 //77
            "Age*",                                     //78
            "Gender* (M/F/O)",                          //79
            "Rent Contribution**",                      //80
            "Relationship To Applicant",                //81
            "Last Name",                                //82
            "Given Names",                              //83
            "Birth Date* (dd/mm/yyyy)",                 //84
            "Age*",                                     //85
            "Gender* (M/F/O)",                          //86
            "Rent Contribution**",                      //87
            "Relationship To Applicant",                //88
            "Last Name",                                //89
            "Given Names",                              //90
            "Birth Date* (dd/mm/yyyy)",                 //91
            "Age*",                                     //92
            "Gender* (M/F/O)",                          //93
            "Rent Contribution**",                      //94
            "Relationship To Applicant",                //95
            "Last Name",                                //96
            "Given Names",                              //97
            "Birth Date* (dd/mm/yyyy)",                 //98
            "Age*",                                     //99
            "Gender* (M/F/O)",                          //100
            "Rent Contribution**",                      //101 End of Page 3
            "Yes",                          //102
            "No",                           //103
            "First Nations",                //104
            "Metis",                        //105
            "Inuit",                        //106
            "Other",                        //107
            "Name",                                     //108
            "Date moved to Canada (dd/mm/yyyy)",        //109
            "Status in Canada",                         //110
            "Name of Sponsor",                          //111
            "Date Sponsorship Agreement Ends",          //112
            "Name",                                     //113
            "Date moved to Canada (dd/mm/yyyy)",        //114
            "Status in Canada",                         //115
            "Name of Sponsor",                          //116
            "Date Sponsorship Agreement Ends",          //117
            "Name",                                     //118
            "Date moved to Canada (dd/mm/yyyy)",        //119
            "Status in Canada",                         //120
            "Name of Sponsor",                          //121
            "Date Sponsorship Agreement Ends",          //122
            "Name",                                     //123
            "Date moved to Canada (dd/mm/yyyy)",        //124
            "Status in Canada",                         //125
            "Name of Sponsor",                          //126
            "Date Sponsorship Agreement Ends",          //127
            "Yes",              //128
            "No",               //129
            "Name",                             //130
            "Days per week",                    //131
            "Shared custody? (Yes/No)",         //132
            "If not shared custody, why does the person not live with you full-time",       //133
            "Name",                             //134
            "Days per week",                    //135
            "Shared custody? (Yes/No)",         //136
            "If not shared custody, why does the person not live with you full-time",       //137
            "Name",                             //138
            "Days per week",                    //139
            "Shared custody? (Yes/No)",         //140
            "If not shared custody, why does the person not live with you full-time",       //141
            "Yes",                          //142
            "No",                           //143
            "If yes, list names",           //144
            "Yes",                          //145
            "No",                           //146
            "If yes, list names",           //147
            "Home Phone",                   //148
            "Work Phone",                   //149
            "Cell Phone",                   //150
            "Email",                        //151
            "Optional: Name of person we can leave messages with",          //152
            "Message person phone number",                                  //153
            "Optional: Authorized Contact* name and relationship to you",   //154
            "Authorized Contact phone number",                              //155
            "Apt #",                        //156
            "Street #",                     //157
            "Street Name",                  //158
            "City",                         //159
            "Postal Code",                  //160  End of Page 4
            "Apt #",                        //161
            "Street #",                     //162
            "Street Name",                  //163
            "City",                         //164
            "Postal Code",                  //165
            "Landlord Name",                //166
            "Landlord Phone",               //167
            "Landlord Address",             //168
            "Rent",                         //169
            "Own",                          //170
            "Rent-to-own",                  //171
            "How much is your rent?",           //172
            "Monthly",                          //173
            "Weekly",                           //174
            "Nightly/Daily",                    //175
            "Yes",              //176
            "No",               //177
            "Yes",              //178
            "No",               //179
            "Yes",              //180
            "No",               //181
            "I live in a self-contained unit (apartment, house townhouse)",                         //182
            "I live with family or friends (other than spouse/common law partner)",                 //183
            "I live in a self-contained basement suite",                                            //184
            "I live in a Housing Co-operative",                                                     //185
            "I live in a Manufactured/Trailer/Mobile home",                                         //186
            "I live in a Hotel/Motel",                                                              //187
            "Other",                                                                     //188
            "(describe)",               //189
            "Own",                      //190
            "Rent",                     //191
            "Trailer Rent",             //192
            "Yes",                      //193
            "No",                       //194
            "Pad Rent",                 //195
            "Yes",                  //196
            "No",                   //197
            "If yes, when was the last payment received",           //198
            "Yes",                  //199
            "No",                   //200
            "Yes",                  //201
            "No",                   //202
            "Yes",                  //203
            "No",                   //204
            "Income or Payment Type",                       //205
            "Last Year's Gross Total Amount",               //206
            "Current Gross Monthly Amount",                 //207
            "Income or Payment Type",                       //208
            "Last Year's Gross Total Amount",               //209
            "Current Gross Monthly Amount",                 //210
            "Income or Payment Type",                       //211
            "Last Year's Gross Total Amount",               //212
            "Current Gross Monthly Amount",                 //213
            "Income or Payment Type",                       //214
            "Last Year's Gross Total Amount",               //215
            "Current Gross Monthly Amount",                 //216
            "Income or Payment Type",                       //217
            "Last Year's Gross Total Amount",               //218
            "Current Gross Monthly Amount",                 //219 End of Page 5
            "Yes",                  //220
            "No",                   //221
            "Yes",                  //222
            "No",                   //223
            "Income Source (Employment, Employment Insurance, Pensions, Support Income, Other)",             //224
            "Applicant",            //225
            "Spouse",               //226
            "Income Source (Employment, Employment Insurance, Pensions, Support Income, Other)",             //227
            "Applicant",            //228
            "Spouse",               //229
            "Income Source (Employment, Employment Insurance, Pensions, Support Income, Other)",             //230
            "Applicant",            //231
            "Spouse",               //232
            "Income Source (Employment, Employment Insurance, Pensions, Support Income, Other)",             //233
            "Applicant",            //234
            "Spouse",               //235
            "Income Source (Employment, Employment Insurance, Pensions, Support Income, Other)",             //236
            "Applicant",            //237
            "Spouse",               //238
            "Type of Assets (including all bank accounts, even with negative balances)",                 //239
            "Yes",                  //240
            "No",                   //241
            "Bank, financial institution or company name",              //242
            "Applicant",            //243
            "Spouse",               //244
            "Type of Assets (including all bank accounts, even with negative balances)",                 //245
            "Yes",                  //246
            "No",                   //247
            "Bank, financial institution or company name",              //248
            "Applicant",            //249
            "Spouse",               //250
            "Type of Assets (including all bank accounts, even with negative balances)",                 //251
            "Yes",                  //252
            "No",                   //253
            "Bank, financial institution or company name",              //254
            "Applicant",            //255
            "Spouse",               //256
            "Type of Assets (including all bank accounts, even with negative balances)",                 //257
            "Yes",                  //258
            "No",                   //259
            "Bank, financial institution or company name",              //260
            "Applicant",            //261
            "Spouse",               //262
            "Type of Assets (including all bank accounts, even with negative balances)",                 //263
            "Yes",                  //264
            "No",                   //265
            "Bank, financial institution or company name",              //266
            "Applicant",            //267
            "Spouse",               //268
            "Type of Assets (including all bank accounts, even with negative balances)",                 //269
            "Yes",                  //270
            "No",                   //271
            "Bank, financial institution or company name",              //272
            "Applicant",            //273
            "Spouse",               //274
            "Type of Assets (including all bank accounts, even with negative balances)",                 //275
            "Yes",                  //276
            "No",                   //277
            "Bank, financial institution or company name",              //278
            "Applicant",            //279
            "Spouse",               //280
            "Type of Assets (including all bank accounts, even with negative balances)",                 //281
            "Yes",                  //282
            "No",                   //283
            "Bank, financial institution or company name",              //284
            "Applicant",            //285
            "Spouse",               //286
            "If you shares in a company or business provide legal name",         //287
            "Yes",                  //288
            "No",                   //289
            "Type of Property",                 //290
            "Location (Address)",               //291
            "Year Purchased",                   //292
            "Value($)",                         //293
            "Equity($)",                        //294
            "Type of Property",                 //295
            "Location (Address)",               //296
            "Year Purchased",                   //297
            "Value($)",                         //298
            "Equity($)",                        //299 End of Page 6
            "Signature of Applicant",           //300
            "Date",                             //301
            "Signature of Spouse (if applicable)",          //302
            "Date",                             //303 End of Page 7
            "Provide consent for release of tax information from Canada Revenue Agency (CRA) on page 2 of this application; or",                                                                                            //304
            "Provide copies of last year's Income Tax Notice of Assessment AND detailed Income Tax return (include all pages); or T-slips from all income sources",                                                         //305
            "Note: if bankruptcy was declared within the last two taxation years, provide copies of the Income Tax Notices of Assessment and detailed Income Tax returns for both the pre- and post-bankruptcy",            //306
            "Proof of any regular ongoing funds from non-taxable sources (alimony, family support, on-reserve income, private disability pension, etc.)",                       //307
            "Proof of current income from all sources if you answered No to questions 10c or 10d",                              //308
            "Blank cheque marked VOID; or",                                     //309
            "A Preauthorized Debit Form provided by your financial institution; or",                    //310
            "The Direct Deposit page included with this application completed by your financial institution",                   //311
            "Copy of Canadian birth certificate(s) for all family members born in Canada; and",                                 //312
            "For family members not born in Canada, provide copies of Permanent Residences documents",                          //313
            "Rent Receipt showing address, rent amount, date and landlord name; or",                                            //314
            "Copy of recent Rent Increase Notice; or",                                                                          //315
            "Copy of Lease or Tenancy Agreement (if signed within the past 12 months)",                                         //316
            "Copies of bank statements from all bank accounts",                                                                 //317
            "Property tax assessments for value of property owned and proof of outstanding mortgage(s) for equity calculation.",        //318
            "Other statement showing total value of asset(s)",                                                                  //319
            "If you have shares in a business, please include proof of corporate shares",                                       //320
            "Statement of Income and Expenses from last year's Income Tax return and all related worksheets (formT2125)",       //321 End of page 8
            "A printed, personalized blank cheque marked VOID; or",                                 //322
            "A Preauthorized Debit Form provided by your financial institution; or",                //323
            "Have your financial institution complete the information below",                       //324
            "Name of Applicant",                                //325
            "Transit Number",                                   //326
            "Bank Number",                                      //327
            "Account Number",                                   //328
            "Name(s) on the account",                           //329
            "Phone number of financial institution",            //330
            "Financial Institution Stamp"                       //331 End of Page 9
        };
    }
}
