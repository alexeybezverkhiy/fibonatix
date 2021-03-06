﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MerchantAPI.Helpers
{
    public class CountryConverter
    {
        public static string ConvertCountryToCommDooSpace(string c) {
            return countryAlpha3String(c);
        }

        public static string countryAlpha2String(string c) {
            if (c == null) {
                throw new ArgumentNullException("Argument 'c' cann't be [null]");
            }
            string code = c.ToUpper();
            if (code == "AU" || code == "AUS") return "AU";
            else if (code == "AT" || code == "AUT") return "AT";
            else if (code == "AZ" || code == "AZE") return "AZ";
            else if (code == "AX" || code == "ALA") return "AX";
            else if (code == "AL" || code == "ALB") return "AL";
            else if (code == "DZ" || code == "DZA") return "DZ";
            else if (code == "VI" || code == "VIR") return "VI";
            else if (code == "AS" || code == "ASM") return "AS";
            else if (code == "AI" || code == "AIA") return "AI";
            else if (code == "AO" || code == "AGO") return "AO";
            else if (code == "AD" || code == "AND") return "AD";
            else if (code == "AQ" || code == "ATA") return "AQ";
            else if (code == "AG" || code == "ATG") return "AG";
            else if (code == "AR" || code == "ARG") return "AR";
            else if (code == "AM" || code == "ARM") return "AM";
            else if (code == "AW" || code == "ABW") return "AW";
            else if (code == "AF" || code == "AFG") return "AF";
            else if (code == "BS" || code == "BHS") return "BS";
            else if (code == "BD" || code == "BGD") return "BD";
            else if (code == "BB" || code == "BRB") return "BB";
            else if (code == "BH" || code == "BHR") return "BH";
            else if (code == "BZ" || code == "BLZ") return "BZ";
            else if (code == "BY" || code == "BLR") return "BY";
            else if (code == "BE" || code == "BEL") return "BE";
            else if (code == "BJ" || code == "BEN") return "BJ";
            else if (code == "BM" || code == "BMU") return "BM";
            else if (code == "BG" || code == "BGR") return "BG";
            else if (code == "BO" || code == "BOL") return "BO";
            else if (code == "BQ" || code == "BES") return "BQ";
            else if (code == "BA" || code == "BIH") return "BA";
            else if (code == "BW" || code == "BWA") return "BW";
            else if (code == "BR" || code == "BRA") return "BR";
            else if (code == "IO" || code == "IOT") return "IO";
            else if (code == "VG" || code == "VGB") return "VG";
            else if (code == "BN" || code == "BRN") return "BN";
            else if (code == "BF" || code == "BFA") return "BF";
            else if (code == "BI" || code == "BDI") return "BI";
            else if (code == "BT" || code == "BTN") return "BT";
            else if (code == "VU" || code == "VUT") return "VU";
            else if (code == "VA" || code == "VAT") return "VA";
            else if (code == "GB" || code == "GBR") return "GB";
            else if (code == "HU" || code == "HUN") return "HU";
            else if (code == "VE" || code == "VEN") return "VE";
            else if (code == "UM" || code == "UMI") return "UM";
            else if (code == "TL" || code == "TLS") return "TL";
            else if (code == "VN" || code == "VNM") return "VN";
            else if (code == "GA" || code == "GAB") return "GA";
            else if (code == "HT" || code == "HTI") return "HT";
            else if (code == "GY" || code == "GUY") return "GY";
            else if (code == "GM" || code == "GMB") return "GM";
            else if (code == "GH" || code == "GHA") return "GH";
            else if (code == "GP" || code == "GLP") return "GP";
            else if (code == "GT" || code == "GTM") return "GT";
            else if (code == "GF" || code == "GUF") return "GF";
            else if (code == "GN" || code == "GIN") return "GN";
            else if (code == "GW" || code == "GNB") return "GW";
            else if (code == "DE" || code == "DEU") return "DE";
            else if (code == "GG" || code == "GGY") return "GG";
            else if (code == "GI" || code == "GIB") return "GI";
            else if (code == "HN" || code == "HND") return "HN";
            else if (code == "HK" || code == "HKG") return "HK";
            else if (code == "GD" || code == "GRD") return "GD";
            else if (code == "GL" || code == "GRL") return "GL";
            else if (code == "GR" || code == "GRC") return "GR";
            else if (code == "GE" || code == "GEO") return "GE";
            else if (code == "GU" || code == "GUM") return "GU";
            else if (code == "DK" || code == "DNK") return "DK";
            else if (code == "JE" || code == "JEY") return "JE";
            else if (code == "DJ" || code == "DJI") return "DJ";
            else if (code == "DM" || code == "DMA") return "DM";
            else if (code == "DO" || code == "DOM") return "DO";
            else if (code == "CD" || code == "COD") return "CD";
            else if (code == "EU" || code == "EU ") return "EU";
            else if (code == "EG" || code == "EGY") return "EG";
            else if (code == "ZM" || code == "ZMB") return "ZM";
            else if (code == "EH" || code == "ESH") return "EH";
            else if (code == "ZW" || code == "ZWE") return "ZW";
            else if (code == "IL" || code == "ISR") return "IL";
            else if (code == "IN" || code == "IND") return "IN";
            else if (code == "ID" || code == "IDN") return "ID";
            else if (code == "JO" || code == "JOR") return "JO";
            else if (code == "IQ" || code == "IRQ") return "IQ";
            else if (code == "IR" || code == "IRN") return "IR";
            else if (code == "IE" || code == "IRL") return "IE";
            else if (code == "IS" || code == "ISL") return "IS";
            else if (code == "ES" || code == "ESP") return "ES";
            else if (code == "IT" || code == "ITA") return "IT";
            else if (code == "YE" || code == "YEM") return "YE";
            else if (code == "CV" || code == "CPV") return "CV";
            else if (code == "KZ" || code == "KAZ") return "KZ";
            else if (code == "KY" || code == "CYM") return "KY";
            else if (code == "KH" || code == "KHM") return "KH";
            else if (code == "CM" || code == "CMR") return "CM";
            else if (code == "CA" || code == "CAN") return "CA";
            else if (code == "QA" || code == "QAT") return "QA";
            else if (code == "KE" || code == "KEN") return "KE";
            else if (code == "CY" || code == "CYP") return "CY";
            else if (code == "KG" || code == "KGZ") return "KG";
            else if (code == "KI" || code == "KIR") return "KI";
            else if (code == "TW" || code == "TWN") return "TW";
            else if (code == "KP" || code == "PRK") return "KP";
            else if (code == "CN" || code == "CHN") return "CN";
            else if (code == "CC" || code == "CCK") return "CC";
            else if (code == "CO" || code == "COL") return "CO";
            else if (code == "KM" || code == "COM") return "KM";
            else if (code == "CR" || code == "CRI") return "CR";
            else if (code == "CI" || code == "CIV") return "CI";
            else if (code == "CU" || code == "CUB") return "CU";
            else if (code == "KW" || code == "KWT") return "KW";
            else if (code == "CW" || code == "CUW") return "CW";
            else if (code == "LA" || code == "LAO") return "LA";
            else if (code == "LV" || code == "LVA") return "LV";
            else if (code == "LS" || code == "LSO") return "LS";
            else if (code == "LR" || code == "LBR") return "LR";
            else if (code == "LB" || code == "LBN") return "LB";
            else if (code == "LY" || code == "LBY") return "LY";
            else if (code == "LT" || code == "LTU") return "LT";
            else if (code == "LI" || code == "LIE") return "LI";
            else if (code == "LU" || code == "LUX") return "LU";
            else if (code == "MU" || code == "MUS") return "MU";
            else if (code == "MR" || code == "MRT") return "MR";
            else if (code == "MG" || code == "MDG") return "MG";
            else if (code == "YT" || code == "MYT") return "YT";
            else if (code == "MO" || code == "MAC") return "MO";
            else if (code == "MK" || code == "MKD") return "MK";
            else if (code == "MW" || code == "MWI") return "MW";
            else if (code == "MY" || code == "MYS") return "MY";
            else if (code == "ML" || code == "MLI") return "ML";
            else if (code == "MV" || code == "MDV") return "MV";
            else if (code == "MT" || code == "MLT") return "MT";
            else if (code == "MA" || code == "MAR") return "MA";
            else if (code == "MQ" || code == "MTQ") return "MQ";
            else if (code == "MH" || code == "MHL") return "MH";
            else if (code == "MX" || code == "MEX") return "MX";
            else if (code == "FM" || code == "FSM") return "FM";
            else if (code == "MZ" || code == "MOZ") return "MZ";
            else if (code == "MD" || code == "MDA") return "MD";
            else if (code == "MC" || code == "MCO") return "MC";
            else if (code == "MN" || code == "MNG") return "MN";
            else if (code == "MS" || code == "MSR") return "MS";
            else if (code == "MM" || code == "MMR") return "MM";
            else if (code == "NA" || code == "NAM") return "NA";
            else if (code == "NR" || code == "NRU") return "NR";
            else if (code == "NP" || code == "NPL") return "NP";
            else if (code == "NE" || code == "NER") return "NE";
            else if (code == "NG" || code == "NGA") return "NG";
            else if (code == "NL" || code == "NLD") return "NL";
            else if (code == "NI" || code == "NIC") return "NI";
            else if (code == "NU" || code == "NIU") return "NU";
            else if (code == "NZ" || code == "NZL") return "NZ";
            else if (code == "NC" || code == "NCL") return "NC";
            else if (code == "NO" || code == "NOR") return "NO";
            else if (code == "AE" || code == "ARE") return "AE";
            else if (code == "OM" || code == "OMN") return "OM";
            else if (code == "BV" || code == "BVT") return "BV";
            else if (code == "IM" || code == "IMN") return "IM";
            else if (code == "CK" || code == "COK") return "CK";
            else if (code == "NF" || code == "NFK") return "NF";
            else if (code == "CX" || code == "CXR") return "CX";
            else if (code == "PN" || code == "PCN") return "PN";
            else if (code == "SH" || code == "SHN") return "SH";
            else if (code == "PK" || code == "PAK") return "PK";
            else if (code == "PW" || code == "PLW") return "PW";
            else if (code == "PS" || code == "PSE") return "PS";
            else if (code == "PA" || code == "PAN") return "PA";
            else if (code == "PG" || code == "PNG") return "PG";
            else if (code == "PY" || code == "PRY") return "PY";
            else if (code == "PE" || code == "PER") return "PE";
            else if (code == "PL" || code == "POL") return "PL";
            else if (code == "PT" || code == "PRT") return "PT";
            else if (code == "PR" || code == "PRI") return "PR";
            else if (code == "CG" || code == "COG") return "CG";
            else if (code == "KR" || code == "KOR") return "KR";
            else if (code == "RE" || code == "REU") return "RE";
            else if (code == "RU" || code == "RUS") return "RU";
            else if (code == "RW" || code == "RWA") return "RW";
            else if (code == "RO" || code == "ROU") return "RO";
            else if (code == "SV" || code == "SLV") return "SV";
            else if (code == "WS" || code == "WSM") return "WS";
            else if (code == "SM" || code == "SMR") return "SM";
            else if (code == "ST" || code == "STP") return "ST";
            else if (code == "SA" || code == "SAU") return "SA";
            else if (code == "SZ" || code == "SWZ") return "SZ";
            else if (code == "MP" || code == "MNP") return "MP";
            else if (code == "SC" || code == "SYC") return "SC";
            else if (code == "BL" || code == "BLM") return "BL";
            else if (code == "MF" || code == "MAF") return "MF";
            else if (code == "PM" || code == "SPM") return "PM";
            else if (code == "SN" || code == "SEN") return "SN";
            else if (code == "VC" || code == "VCT") return "VC";
            else if (code == "KN" || code == "KNA") return "KN";
            else if (code == "LC" || code == "LCA") return "LC";
            else if (code == "RS" || code == "SRB") return "RS";
            else if (code == "SG" || code == "SGP") return "SG";
            else if (code == "SX" || code == "SXM") return "SX";
            else if (code == "SY" || code == "SYR") return "SY";
            else if (code == "SK" || code == "SVK") return "SK";
            else if (code == "SI" || code == "SVN") return "SI";
            else if (code == "SB" || code == "SLB") return "SB";
            else if (code == "SO" || code == "SOM") return "SO";
            else if (code == "SD" || code == "SDN") return "SD";
            else if (code == "SU" || code == "SUN") return "SU";
            else if (code == "SR" || code == "SUR") return "SR";
            else if (code == "US" || code == "USA") return "US";
            else if (code == "SL" || code == "SLE") return "SL";
            else if (code == "TJ" || code == "TJK") return "TJ";
            else if (code == "TH" || code == "THA") return "TH";
            else if (code == "TZ" || code == "TZA") return "TZ";
            else if (code == "TC" || code == "TCA") return "TC";
            else if (code == "TG" || code == "TGO") return "TG";
            else if (code == "TK" || code == "TKL") return "TK";
            else if (code == "TO" || code == "TON") return "TO";
            else if (code == "TT" || code == "TTO") return "TT";
            else if (code == "TV" || code == "TUV") return "TV";
            else if (code == "TN" || code == "TUN") return "TN";
            else if (code == "TM" || code == "TKM") return "TM";
            else if (code == "TR" || code == "TUR") return "TR";
            else if (code == "UG" || code == "UGA") return "UG";
            else if (code == "UZ" || code == "UZB") return "UZ";
            else if (code == "UA" || code == "UKR") return "UA";
            else if (code == "WF" || code == "WLF") return "WF";
            else if (code == "UY" || code == "URY") return "UY";
            else if (code == "FO" || code == "FRO") return "FO";
            else if (code == "FJ" || code == "FJI") return "FJ";
            else if (code == "PH" || code == "PHL") return "PH";
            else if (code == "FI" || code == "FIN") return "FI";
            else if (code == "FK" || code == "FLK") return "FK";
            else if (code == "FR" || code == "FRA") return "FR";
            else if (code == "PF" || code == "PYF") return "PF";
            else if (code == "TF" || code == "ATF") return "TF";
            else if (code == "HM" || code == "HMD") return "HM";
            else if (code == "HR" || code == "HRV") return "HR";
            else if (code == "CF" || code == "CAF") return "CF";
            else if (code == "TD" || code == "TCD") return "TD";
            else if (code == "ME" || code == "MNE") return "ME";
            else if (code == "CZ" || code == "CZE") return "CZ";
            else if (code == "CL" || code == "CHL") return "CL";
            else if (code == "CH" || code == "CHE") return "CH";
            else if (code == "SE" || code == "SWE") return "SE";
            else if (code == "SJ" || code == "SJM") return "SJ";
            else if (code == "LK" || code == "LKA") return "LK";
            else if (code == "EC" || code == "ECU") return "EC";
            else if (code == "GQ" || code == "GNQ") return "GQ";
            else if (code == "ER" || code == "ERI") return "ER";
            else if (code == "EE" || code == "EST") return "EE";
            else if (code == "ET" || code == "ETH") return "ET";
            else if (code == "ZA" || code == "ZAF") return "ZA";
            else if (code == "GS" || code == "SGS") return "GS";
            else if (code == "SS" || code == "SSD") return "SS";
            else if (code == "JM" || code == "JAM") return "JM";
            else if (code == "JP" || code == "JPN") return "JP";
            else
                throw new ArgumentException($"Argument 'c' has unknown value [{c}]");
        }

        public static string countryAlpha3String(string c) {
            if (c == null)
            {
                throw new ArgumentNullException("Argument 'c' cann't be null");
            }
            string code = c.ToUpper();
            if (code == "AU" || code == "AUS") return "AUS";
            else if (code == "AT" || code == "AUT") return "AUT";
            else if (code == "AZ" || code == "AZE") return "AZE";
            else if (code == "AX" || code == "ALA") return "ALA";
            else if (code == "AL" || code == "ALB") return "ALB";
            else if (code == "DZ" || code == "DZA") return "DZA";
            else if (code == "VI" || code == "VIR") return "VIR";
            else if (code == "AS" || code == "ASM") return "ASM";
            else if (code == "AI" || code == "AIA") return "AIA";
            else if (code == "AO" || code == "AGO") return "AGO";
            else if (code == "AD" || code == "AND") return "AND";
            else if (code == "AQ" || code == "ATA") return "ATA";
            else if (code == "AG" || code == "ATG") return "ATG";
            else if (code == "AR" || code == "ARG") return "ARG";
            else if (code == "AM" || code == "ARM") return "ARM";
            else if (code == "AW" || code == "ABW") return "ABW";
            else if (code == "AF" || code == "AFG") return "AFG";
            else if (code == "BS" || code == "BHS") return "BHS";
            else if (code == "BD" || code == "BGD") return "BGD";
            else if (code == "BB" || code == "BRB") return "BRB";
            else if (code == "BH" || code == "BHR") return "BHR";
            else if (code == "BZ" || code == "BLZ") return "BLZ";
            else if (code == "BY" || code == "BLR") return "BLR";
            else if (code == "BE" || code == "BEL") return "BEL";
            else if (code == "BJ" || code == "BEN") return "BEN";
            else if (code == "BM" || code == "BMU") return "BMU";
            else if (code == "BG" || code == "BGR") return "BGR";
            else if (code == "BO" || code == "BOL") return "BOL";
            else if (code == "BQ" || code == "BES") return "BES";
            else if (code == "BA" || code == "BIH") return "BIH";
            else if (code == "BW" || code == "BWA") return "BWA";
            else if (code == "BR" || code == "BRA") return "BRA";
            else if (code == "IO" || code == "IOT") return "IOT";
            else if (code == "VG" || code == "VGB") return "VGB";
            else if (code == "BN" || code == "BRN") return "BRN";
            else if (code == "BF" || code == "BFA") return "BFA";
            else if (code == "BI" || code == "BDI") return "BDI";
            else if (code == "BT" || code == "BTN") return "BTN";
            else if (code == "VU" || code == "VUT") return "VUT";
            else if (code == "VA" || code == "VAT") return "VAT";
            else if (code == "GB" || code == "GBR") return "GBR";
            else if (code == "HU" || code == "HUN") return "HUN";
            else if (code == "VE" || code == "VEN") return "VEN";
            else if (code == "UM" || code == "UMI") return "UMI";
            else if (code == "TL" || code == "TLS") return "TLS";
            else if (code == "VN" || code == "VNM") return "VNM";
            else if (code == "GA" || code == "GAB") return "GAB";
            else if (code == "HT" || code == "HTI") return "HTI";
            else if (code == "GY" || code == "GUY") return "GUY";
            else if (code == "GM" || code == "GMB") return "GMB";
            else if (code == "GH" || code == "GHA") return "GHA";
            else if (code == "GP" || code == "GLP") return "GLP";
            else if (code == "GT" || code == "GTM") return "GTM";
            else if (code == "GF" || code == "GUF") return "GUF";
            else if (code == "GN" || code == "GIN") return "GIN";
            else if (code == "GW" || code == "GNB") return "GNB";
            else if (code == "DE" || code == "DEU") return "DEU";
            else if (code == "GG" || code == "GGY") return "GGY";
            else if (code == "GI" || code == "GIB") return "GIB";
            else if (code == "HN" || code == "HND") return "HND";
            else if (code == "HK" || code == "HKG") return "HKG";
            else if (code == "GD" || code == "GRD") return "GRD";
            else if (code == "GL" || code == "GRL") return "GRL";
            else if (code == "GR" || code == "GRC") return "GRC";
            else if (code == "GE" || code == "GEO") return "GEO";
            else if (code == "GU" || code == "GUM") return "GUM";
            else if (code == "DK" || code == "DNK") return "DNK";
            else if (code == "JE" || code == "JEY") return "JEY";
            else if (code == "DJ" || code == "DJI") return "DJI";
            else if (code == "DM" || code == "DMA") return "DMA";
            else if (code == "DO" || code == "DOM") return "DOM";
            else if (code == "CD" || code == "COD") return "COD";
            else if (code == "EU" || code == "EU ") return "EU ";
            else if (code == "EG" || code == "EGY") return "EGY";
            else if (code == "ZM" || code == "ZMB") return "ZMB";
            else if (code == "EH" || code == "ESH") return "ESH";
            else if (code == "ZW" || code == "ZWE") return "ZWE";
            else if (code == "IL" || code == "ISR") return "ISR";
            else if (code == "IN" || code == "IND") return "IND";
            else if (code == "ID" || code == "IDN") return "IDN";
            else if (code == "JO" || code == "JOR") return "JOR";
            else if (code == "IQ" || code == "IRQ") return "IRQ";
            else if (code == "IR" || code == "IRN") return "IRN";
            else if (code == "IE" || code == "IRL") return "IRL";
            else if (code == "IS" || code == "ISL") return "ISL";
            else if (code == "ES" || code == "ESP") return "ESP";
            else if (code == "IT" || code == "ITA") return "ITA";
            else if (code == "YE" || code == "YEM") return "YEM";
            else if (code == "CV" || code == "CPV") return "CPV";
            else if (code == "KZ" || code == "KAZ") return "KAZ";
            else if (code == "KY" || code == "CYM") return "CYM";
            else if (code == "KH" || code == "KHM") return "KHM";
            else if (code == "CM" || code == "CMR") return "CMR";
            else if (code == "CA" || code == "CAN") return "CAN";
            else if (code == "QA" || code == "QAT") return "QAT";
            else if (code == "KE" || code == "KEN") return "KEN";
            else if (code == "CY" || code == "CYP") return "CYP";
            else if (code == "KG" || code == "KGZ") return "KGZ";
            else if (code == "KI" || code == "KIR") return "KIR";
            else if (code == "TW" || code == "TWN") return "TWN";
            else if (code == "KP" || code == "PRK") return "PRK";
            else if (code == "CN" || code == "CHN") return "CHN";
            else if (code == "CC" || code == "CCK") return "CCK";
            else if (code == "CO" || code == "COL") return "COL";
            else if (code == "KM" || code == "COM") return "COM";
            else if (code == "CR" || code == "CRI") return "CRI";
            else if (code == "CI" || code == "CIV") return "CIV";
            else if (code == "CU" || code == "CUB") return "CUB";
            else if (code == "KW" || code == "KWT") return "KWT";
            else if (code == "CW" || code == "CUW") return "CUW";
            else if (code == "LA" || code == "LAO") return "LAO";
            else if (code == "LV" || code == "LVA") return "LVA";
            else if (code == "LS" || code == "LSO") return "LSO";
            else if (code == "LR" || code == "LBR") return "LBR";
            else if (code == "LB" || code == "LBN") return "LBN";
            else if (code == "LY" || code == "LBY") return "LBY";
            else if (code == "LT" || code == "LTU") return "LTU";
            else if (code == "LI" || code == "LIE") return "LIE";
            else if (code == "LU" || code == "LUX") return "LUX";
            else if (code == "MU" || code == "MUS") return "MUS";
            else if (code == "MR" || code == "MRT") return "MRT";
            else if (code == "MG" || code == "MDG") return "MDG";
            else if (code == "YT" || code == "MYT") return "MYT";
            else if (code == "MO" || code == "MAC") return "MAC";
            else if (code == "MK" || code == "MKD") return "MKD";
            else if (code == "MW" || code == "MWI") return "MWI";
            else if (code == "MY" || code == "MYS") return "MYS";
            else if (code == "ML" || code == "MLI") return "MLI";
            else if (code == "MV" || code == "MDV") return "MDV";
            else if (code == "MT" || code == "MLT") return "MLT";
            else if (code == "MA" || code == "MAR") return "MAR";
            else if (code == "MQ" || code == "MTQ") return "MTQ";
            else if (code == "MH" || code == "MHL") return "MHL";
            else if (code == "MX" || code == "MEX") return "MEX";
            else if (code == "FM" || code == "FSM") return "FSM";
            else if (code == "MZ" || code == "MOZ") return "MOZ";
            else if (code == "MD" || code == "MDA") return "MDA";
            else if (code == "MC" || code == "MCO") return "MCO";
            else if (code == "MN" || code == "MNG") return "MNG";
            else if (code == "MS" || code == "MSR") return "MSR";
            else if (code == "MM" || code == "MMR") return "MMR";
            else if (code == "NA" || code == "NAM") return "NAM";
            else if (code == "NR" || code == "NRU") return "NRU";
            else if (code == "NP" || code == "NPL") return "NPL";
            else if (code == "NE" || code == "NER") return "NER";
            else if (code == "NG" || code == "NGA") return "NGA";
            else if (code == "NL" || code == "NLD") return "NLD";
            else if (code == "NI" || code == "NIC") return "NIC";
            else if (code == "NU" || code == "NIU") return "NIU";
            else if (code == "NZ" || code == "NZL") return "NZL";
            else if (code == "NC" || code == "NCL") return "NCL";
            else if (code == "NO" || code == "NOR") return "NOR";
            else if (code == "AE" || code == "ARE") return "ARE";
            else if (code == "OM" || code == "OMN") return "OMN";
            else if (code == "BV" || code == "BVT") return "BVT";
            else if (code == "IM" || code == "IMN") return "IMN";
            else if (code == "CK" || code == "COK") return "COK";
            else if (code == "NF" || code == "NFK") return "NFK";
            else if (code == "CX" || code == "CXR") return "CXR";
            else if (code == "PN" || code == "PCN") return "PCN";
            else if (code == "SH" || code == "SHN") return "SHN";
            else if (code == "PK" || code == "PAK") return "PAK";
            else if (code == "PW" || code == "PLW") return "PLW";
            else if (code == "PS" || code == "PSE") return "PSE";
            else if (code == "PA" || code == "PAN") return "PAN";
            else if (code == "PG" || code == "PNG") return "PNG";
            else if (code == "PY" || code == "PRY") return "PRY";
            else if (code == "PE" || code == "PER") return "PER";
            else if (code == "PL" || code == "POL") return "POL";
            else if (code == "PT" || code == "PRT") return "PRT";
            else if (code == "PR" || code == "PRI") return "PRI";
            else if (code == "CG" || code == "COG") return "COG";
            else if (code == "KR" || code == "KOR") return "KOR";
            else if (code == "RE" || code == "REU") return "REU";
            else if (code == "RU" || code == "RUS") return "RUS";
            else if (code == "RW" || code == "RWA") return "RWA";
            else if (code == "RO" || code == "ROU") return "ROU";
            else if (code == "SV" || code == "SLV") return "SLV";
            else if (code == "WS" || code == "WSM") return "WSM";
            else if (code == "SM" || code == "SMR") return "SMR";
            else if (code == "ST" || code == "STP") return "STP";
            else if (code == "SA" || code == "SAU") return "SAU";
            else if (code == "SZ" || code == "SWZ") return "SWZ";
            else if (code == "MP" || code == "MNP") return "MNP";
            else if (code == "SC" || code == "SYC") return "SYC";
            else if (code == "BL" || code == "BLM") return "BLM";
            else if (code == "MF" || code == "MAF") return "MAF";
            else if (code == "PM" || code == "SPM") return "SPM";
            else if (code == "SN" || code == "SEN") return "SEN";
            else if (code == "VC" || code == "VCT") return "VCT";
            else if (code == "KN" || code == "KNA") return "KNA";
            else if (code == "LC" || code == "LCA") return "LCA";
            else if (code == "RS" || code == "SRB") return "SRB";
            else if (code == "SG" || code == "SGP") return "SGP";
            else if (code == "SX" || code == "SXM") return "SXM";
            else if (code == "SY" || code == "SYR") return "SYR";
            else if (code == "SK" || code == "SVK") return "SVK";
            else if (code == "SI" || code == "SVN") return "SVN";
            else if (code == "SB" || code == "SLB") return "SLB";
            else if (code == "SO" || code == "SOM") return "SOM";
            else if (code == "SD" || code == "SDN") return "SDN";
            else if (code == "SU" || code == "SUN") return "SUN";
            else if (code == "SR" || code == "SUR") return "SUR";
            else if (code == "US" || code == "USA") return "USA";
            else if (code == "SL" || code == "SLE") return "SLE";
            else if (code == "TJ" || code == "TJK") return "TJK";
            else if (code == "TH" || code == "THA") return "THA";
            else if (code == "TZ" || code == "TZA") return "TZA";
            else if (code == "TC" || code == "TCA") return "TCA";
            else if (code == "TG" || code == "TGO") return "TGO";
            else if (code == "TK" || code == "TKL") return "TKL";
            else if (code == "TO" || code == "TON") return "TON";
            else if (code == "TT" || code == "TTO") return "TTO";
            else if (code == "TV" || code == "TUV") return "TUV";
            else if (code == "TN" || code == "TUN") return "TUN";
            else if (code == "TM" || code == "TKM") return "TKM";
            else if (code == "TR" || code == "TUR") return "TUR";
            else if (code == "UG" || code == "UGA") return "UGA";
            else if (code == "UZ" || code == "UZB") return "UZB";
            else if (code == "UA" || code == "UKR") return "UKR";
            else if (code == "WF" || code == "WLF") return "WLF";
            else if (code == "UY" || code == "URY") return "URY";
            else if (code == "FO" || code == "FRO") return "FRO";
            else if (code == "FJ" || code == "FJI") return "FJI";
            else if (code == "PH" || code == "PHL") return "PHL";
            else if (code == "FI" || code == "FIN") return "FIN";
            else if (code == "FK" || code == "FLK") return "FLK";
            else if (code == "FR" || code == "FRA") return "FRA";
            else if (code == "PF" || code == "PYF") return "PYF";
            else if (code == "TF" || code == "ATF") return "ATF";
            else if (code == "HM" || code == "HMD") return "HMD";
            else if (code == "HR" || code == "HRV") return "HRV";
            else if (code == "CF" || code == "CAF") return "CAF";
            else if (code == "TD" || code == "TCD") return "TCD";
            else if (code == "ME" || code == "MNE") return "MNE";
            else if (code == "CZ" || code == "CZE") return "CZE";
            else if (code == "CL" || code == "CHL") return "CHL";
            else if (code == "CH" || code == "CHE") return "CHE";
            else if (code == "SE" || code == "SWE") return "SWE";
            else if (code == "SJ" || code == "SJM") return "SJM";
            else if (code == "LK" || code == "LKA") return "LKA";
            else if (code == "EC" || code == "ECU") return "ECU";
            else if (code == "GQ" || code == "GNQ") return "GNQ";
            else if (code == "ER" || code == "ERI") return "ERI";
            else if (code == "EE" || code == "EST") return "EST";
            else if (code == "ET" || code == "ETH") return "ETH";
            else if (code == "ZA" || code == "ZAF") return "ZAF";
            else if (code == "GS" || code == "SGS") return "SGS";
            else if (code == "SS" || code == "SSD") return "SSD";
            else if (code == "JM" || code == "JAM") return "JAM";
            else if (code == "JP" || code == "JPN") return "JPN";
            else
                throw new ArgumentException($"Argument 'c' has unknown value [{c}]");
        }
    }
}