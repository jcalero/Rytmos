using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialRytData {

	static string file1C1 = "c0:28;19;171;19;207;19;243;16;270;17;315;25;387;15;434;13;576;18;602;23;674;17;701;16;746;21;818;16;845;17;863;16;889;22;961;19;988;17;1007;24;1033;15;1149;40;1203;37;1221;30;1231;14;1257;30;1284;21;1293;37;1330;28;1356;35;1365;59;1396;13;1436;74;1474;23;1490;57;1508;39;1526;33;1528;21;1544;20;1571;25;1581;29;1643;46;1652;22;1661;15;1714;19;1723;75;1725;14;1777;71;1787;16;1795;51;1813;58;1830;17;1868;18;1904;17;1930;47;1939;19;1948;21;2010;58;2048;26;2064;81;2074;13;2082;35;2145;19;2155;62;2191;16;2217;21;2279;16;2289;21;2297;21;2351;68;2353;20;2361;24;2369;14;2387;72;2389;27;2442;58;2514;25;2577;13;2585;28;2621;17;2637;17;2648;19;2729;77;2766;13;2801;34;2863;19;2872;29;2908;22;2925;19;2931;14;2935;38;2961;47;3007;18;3016;40;3043;15;3053;22;3088;46;3195;27;3222;56;3231;15;3254;14;3267;13;3302;29;3311;45;3320;30;3329;41;3338;66;3347;54;3356;38;3365;33;3374;22;3383;51;3392;13;3410;21;3437;26;3446;29;3473;23;3475;34;3482;25;3509;63;3511;22;3527;21;3581;17;3616;54;3625;81;3652;57;3679;45;3681;27;3697;58;3715;20;3717;13;3724;65;3733;88;3760;26;3762;32;3769;30;3796;42;3798;34;3805;34;3855;14;3877;26;3903;26;3921;70;3939;46;3948;58;3957;38;3966;37;3984;61;3993;58;4020;100;4047;43;4049;34;4056;37;4083;31;4092;44;4110;38;4112;19;4128;30;4155;39;4164;30;4226;20;4253;48;4271;48;4289;48;4298;39;4307;81;4334;65;4336;26;4343;56;4370;42;4372;41;4379;27;4388;22;4397;24;4415;50;4442;31;4451;65;4479;16;4514;21;4540;19;4549;13;4557;23;4567;20;4576;46;4594;50;4621;76;4623;17;4626;14;4630;51;4657;68;4659;24;4666;40;4684;38;4686;31;4702;26;4711;26;4729;35;4738;36;4774;23;4801;27;4863;14;4872;15;4881;36;4908;40;4910;23;4917;44;4944;50;4952;18;4971;49;4973;38;4989;54;4998;42;5000;16;5016;30;5025;17;5043;21;5061;39;5097;50;5130;19;5132;27;5204;37;5231;19;5258;34;5276;63;5285;33;5303;29;5312;45;5339;38;5348;43;5375;50;5404;18;5420;15;5429;17;5455;29;5491;28;5519;25;5527;22;5545;28;5547;15;5572;19;5599;29;5608;55;5635;32;5671;46;5689;16;5725;24;5734;13;5743;28;5824;21;5832;23;5850;48;5865;16;5878;25;5886;75;5913;64;5922;89;5924;20;5949;31;5959;16;5976;41;5978;32;5994;34;6030;18;6093;17;6102;27;6138;21;6148;21;6173;27;6182;47;6191;28;6200;24;6218;68;6227;33;6236;63;6245;68;6246;13;6254;51;6263;60;6283;16;6290;45;6292;21;6317;24;6335;25;6353;29;6452;18;6460;16;6496;52;6523;65;6533;69;6550;62;6568;51;6586;50;6588;40;6604;51;6631;47;6633;14;6640;25;6712;19;6792;18;6801;24;6820;21;6828;26;6837;46;6846;41;6855;63;6873;43;6883;27;6891;37;6918;45;6920;21;6927;60;6954;20;6962;33;6999;23;7008;28;7026;21;7036;16;7063;31;7124;29;7142;37;7160;35;7170;54;7178;29;7205;16;7214;79;7241;55;7243;20;7250;42;7259;14;7268;15;7286;25;7367;17;7431;24;7458;39;7465;49;7492;30;7501;88;7528;54;7536;18;7555;39;7573;46;7582;46;7609;44;7645;15;7672;15;7744;75;7753;34;7788;48;7815;27;7834;28;7842;42;7860;71;7869;40;7896;44;7914;49;7923;27;7932;35;7968;14;7986;29;8001;14;8031;67;8040;18;8182;46;8327;52;8426;14;8470;49;8605;15;8641;19;8713;19;8740;18;8928;30;8964;15;9000;21;9027;20;9072;17;";
	static string file1C2 = "c1:28;17;100;12;171;18;207;29;234;11;243;15;270;10;306;9;315;25;351;9;378;9;387;15;494;27;530;12;593;10;602;28;638;12;665;13;674;17;781;20;880;10;889;31;925;17;952;14;961;21;1033;13;1068;12;1149;41;1150;11;1186;13;1203;25;1257;41;1293;11;1329;22;1330;12;1356;13;1374;24;1436;44;1473;23;1490;34;1526;26;1544;23;1553;10;1617;24;1643;14;1661;26;1723;38;1751;10;1760;29;1777;40;1813;29;1831;10;1866;12;1904;21;1930;11;1948;24;2010;21;2038;14;2047;35;2064;39;2119;19;2127;14;2191;30;2235;26;2279;20;2297;10;2298;10;2325;15;2334;43;2351;30;2361;10;2387;37;2406;22;2414;10;2469;20;2478;22;2522;19;2549;11;2585;15;2612;11;2621;52;2638;10;2648;10;2656;18;2693;27;2729;15;2756;11;2765;24;2810;11;2836;12;2872;23;2908;54;2935;12;2961;15;2980;33;3043;12;3052;16;3097;13;3123;12;3195;46;3213;15;3240;12;3266;17;3275;28;3284;20;3293;42;3302;15;3311;67;3320;21;3329;84;3338;73;3347;71;3356;59;3365;65;3374;55;3383;32;3392;44;3401;25;3410;15;3419;14;3437;26;3446;15;3482;41;3554;14;3625;61;3697;72;3733;10;3760;19;3769;54;3841;43;3877;12;3921;40;3948;42;3984;45;3993;12;4020;24;4047;21;4056;68;4083;15;4128;38;4164;17;4200;21;4218;13;4271;54;4343;76;4370;17;4397;18;4415;47;4505;22;4557;25;4630;52;4657;18;4684;9;4702;76;4774;41;4845;21;4846;10;4863;17;4908;17;4917;41;4944;19;4971;9;4989;58;4998;12;5043;21;5061;28;5070;18;5106;18;5132;47;5169;11;5204;13;5276;47;5285;11;5348;50;5420;31;5492;13;5563;25;5626;28;5680;22;5698;18;5779;42;5850;67;5851;10;5864;16;5886;38;5922;63;5994;47;6066;53;6138;17;6191;27;6218;100;6227;24;6245;63;6254;24;6263;47;6283;11;6317;29;6353;54;6411;23;6425;35;6496;24;6568;77;6604;25;6640;50;6667;14;6712;44;6757;27;6766;15;6792;32;6828;40;6837;56;6839;11;6855;16;6873;21;6927;44;6954;9;6999;47;7142;35;7214;56;7286;32;7322;18;7331;25;7403;11;7501;23;7547;20;7573;55;7610;15;7645;12;7788;20;7792;11;7860;52;7869;12;7914;24;7932;15;7941;19;8003;20;8076;37;8246;19;8282;9;8327;38;8381;21;8426;11;8470;16;8524;11;8533;13;8632;10;8641;31;8677;14;8704;14;8713;19;8785;10;8820;17;8919;9;8928;32;8964;21;8991;15;9000;21;9072;15;9144;10;";
	static string file1C3 = "c2:19;18;55;15;63;12;91;22;126;28;144;18;162;13;171;17;207;48;234;21;270;16;306;22;315;14;342;18;378;26;413;27;431;16;486;12;494;50;521;12;557;19;593;25;602;17;629;17;665;29;700;22;737;16;773;14;781;45;809;13;844;21;862;14;880;23;889;22;916;16;925;20;952;31;987;17;1024;17;1060;13;1068;29;1096;17;1131;18;1149;70;1185;40;1203;37;1239;13;1257;73;1293;25;1329;58;1356;25;1365;14;1374;15;1436;77;1490;62;1526;53;1544;51;1580;19;1616;20;1643;27;1661;16;1687;14;1723;68;1760;12;1777;71;1813;68;1830;30;1831;16;1859;15;1866;28;1903;41;1930;25;1939;17;1948;23;1973;14;1998;26;2010;34;2047;21;2048;16;2064;66;2118;39;2136;12;2154;12;2191;29;2217;20;2235;26;2244;13;2262;22;2279;26;2297;21;2298;17;2334;34;2351;60;2361;15;2387;75;2405;17;2432;27;2469;26;2478;35;2522;24;2531;16;2549;24;2567;17;2585;28;2621;54;2637;19;2648;25;2656;27;2693;14;2710;16;2729;20;2765;41;2809;16;2818;21;2836;28;2854;14;2872;43;2908;65;2925;17;2926;13;2935;25;2961;34;2980;16;2998;13;3017;18;3052;50;3105;20;3123;20;3159;16;3195;76;3222;21;3250;14;3285;16;3295;12;3320;17;3338;36;3347;13;3356;41;3374;32;3392;22;3428;16;3446;12;3482;39;3483;39;3509;19;3536;16;3554;28;3555;16;3589;20;3616;22;3625;45;3626;20;3652;34;3670;17;3679;52;3697;84;3715;38;3733;59;3760;35;3769;80;3796;35;3823;21;3841;61;3842;20;3877;35;3878;12;3948;21;3984;47;3993;14;4002;33;4020;78;4047;20;4056;65;4057;13;4083;48;4092;12;4110;19;4128;66;4129;32;4164;53;4200;45;4253;34;4271;48;4289;22;4307;36;4334;50;4343;100;4370;56;4379;22;4397;28;4415;75;4416;24;4451;23;4461;16;4470;12;4487;17;4496;26;4576;53;4594;22;4621;22;4630;88;4657;52;4666;12;4684;45;4702;88;4703;16;4738;34;4774;66;4792;26;4846;26;4847;16;4908;19;4917;57;4935;15;4944;42;4953;14;4971;39;4989;97;4990;11;5025;33;5061;66;5079;16;5150;35;5204;34;5231;17;5240;16;5258;27;5276;91;5278;18;5312;39;5339;41;5348;87;5349;16;5366;28;5375;19;5420;43;5421;24;5456;25;5492;31;5493;13;5563;66;5599;22;5635;44;5644;44;5653;45;5662;21;5671;20;5779;57;5851;27;5852;15;5886;45;5913;16;5922;47;5923;12;5949;35;5958;28;5976;32;5994;64;6012;12;6066;66;6067;16;6138;41;6173;29;6218;22;6245;34;6263;23;6308;28;6317;65;6353;66;6380;13;6425;28;6426;20;6496;23;6497;22;6550;22;6568;32;6586;28;6604;33;6631;14;6640;49;6641;11;6712;58;6793;13;6837;31;6855;78;6864;20;6873;49;6891;34;6918;32;6927;41;6954;39;6981;27;6999;69;7000;14;7026;17;7072;34;7098;18;7143;11;7178;27;7205;36;7214;53;7215;12;7241;43;7268;41;7286;72;7322;26;7341;12;7451;15;7501;27;7502;13;7528;35;7555;35;7573;59;7609;44;7636;20;7645;44;7646;37;7672;15;7717;40;7735;22;7789;33;7815;20;7842;28;7860;59;7862;16;7869;26;7896;43;7914;22;7932;17;7968;19;8007;14;8021;38;8040;22;8076;48;8130;17;8165;23;8183;58;8246;38;8255;16;8273;15;8309;16;8327;25;8345;14;8363;12;8381;22;8417;24;8452;19;8470;73;8533;44;8542;19;8596;20;8614;18;8632;24;8641;21;8668;17;8677;16;8704;30;8739;16;8776;16;8812;12;8820;40;8848;17;8883;20;8901;16;8919;21;8928;25;8955;12;8964;28;8991;31;9026;12;9063;18;9099;14;9107;21;9135;21;9170;27;";
	static string file1C4 = "c3:55;8;126;10;144;14;171;9;207;17;234;10;342;10;413;9;431;13;494;18;521;8;629;9;665;9;700;8;718;9;781;19;862;10;889;9;916;8;952;10;1068;15;1149;26;1185;33;1239;11;1257;39;1293;13;1329;68;1356;10;1365;20;1436;13;1472;12;1490;12;1526;15;1544;39;1580;14;1616;44;1643;12;1652;14;1723;39;1759;17;1777;9;1813;12;1817;35;1830;57;1854;12;1866;41;1903;77;1930;13;1939;24;1973;44;1998;81;2034;20;2048;52;2064;9;2100;9;2118;33;2154;13;2190;12;2191;11;2217;13;2244;12;2262;10;2279;10;2298;13;2334;23;2361;8;2387;14;2405;21;2432;53;2469;20;2478;27;2522;8;2531;12;2549;13;2585;37;2612;9;2621;30;2637;39;2648;11;2656;10;2693;8;2710;14;2728;26;2765;28;2818;18;2836;15;2872;17;2899;8;2908;39;2926;8;2935;14;2954;10;2980;12;3016;24;3017;18;3052;62;3105;17;3123;13;3159;10;3195;43;3222;13;3267;16;3304;12;3339;38;3365;20;3370;37;3375;14;3392;9;3410;10;3446;21;3482;33;3518;13;3554;16;3590;20;3625;46;3626;7;3661;10;3679;10;3697;30;3715;14;3733;27;3769;53;3796;13;3805;13;3841;30;3877;22;3913;17;3931;11;3984;27;4002;23;4020;21;4056;58;4083;16;4092;15;4128;39;4146;8;4164;32;4200;45;4218;25;4253;9;4271;32;4289;17;4343;85;4361;9;4370;13;4379;21;4415;41;4433;12;4451;27;4487;38;4505;20;4558;13;4576;21;4594;11;4630;67;4666;22;4676;62;4702;60;4720;8;4737;13;4738;12;4773;49;4774;36;4783;23;4792;39;4808;71;4818;29;4844;11;4846;9;4863;11;4873;10;4881;9;4917;57;4953;23;4963;68;4989;63;5024;24;5061;74;5070;10;5079;30;5095;50;5105;15;5131;11;5150;14;5160;13;5168;20;5204;39;5240;18;5250;64;5276;71;5311;36;5348;93;5357;19;5366;36;5382;35;5392;25;5418;12;5420;16;5447;8;5455;21;5456;11;5491;15;5492;10;5493;7;5537;58;5563;46;5598;36;5635;100;5644;10;5653;28;5669;11;5670;9;5679;11;5743;35;5779;31;5815;8;5850;23;5851;17;5886;11;5887;9;5923;43;5958;22;5976;9;5994;35;6030;9;6066;34;6102;8;6138;29;6210;54;6245;15;6263;9;6281;11;6317;47;6353;30;6398;22;6411;39;6424;14;6425;12;6448;34;6452;15;6460;10;6497;61;6533;13;6567;19;6592;47;6640;22;6642;17;6694;10;6712;37;6784;41;6855;17;6872;31;6927;10;6928;8;6981;10;6999;33;7026;26;7062;11;7071;33;7098;10;7107;12;7143;9;7179;35;7214;17;7215;10;7231;11;7249;20;7286;26;7322;38;7412;8;7448;9;7573;28;7610;52;7646;26;7699;8;7717;19;7735;9;7789;18;7825;9;7860;18;7932;28;7933;10;7959;52;7964;8;7969;11;7995;9;8021;8;8040;14;8067;8;8076;36;8165;10;8183;16;8246;18;8273;10;8327;12;8381;12;8417;9;8452;9;8470;20;8524;9;8533;19;8614;10;8641;8;8668;9;8704;11;8820;17;8901;13;8928;11;8991;12;9107;11;9170;9;";
	static string file1LF = "lp:0;0;29696;1;30720;0;65536;1;66560;0;102400;1;104448;0;175104;1;180224;0;249856;1;250880;0;322560;1;326656;0;359424;1;361472;0;443392;1;447488;2;513024;1;515072;2;516096;1;550912;0;578560;1;581632;2;612352;1;616448;2;622592;1;636928;0;652288;1;658432;0;691200;1;692224;0;736256;1;757760;2;779264;1;780288;2;916480;1;943104;0;947200;1;951296;0;984064;1;988160;0;1012736;1;1013760;0;1031168;1;1033216;2;1054720;1;1056768;2;1067008;1;1075200;2;1079296;1;1084416;2;1090560;1;1091584;2;1258496;1;1260544;2;1384448;1;1387520;2;1721344;1;1742848;0;1746944;1;1755136;2;1816576;1;1819648;2;2191360;1;2196480;2;2266112;1;2270208;2;2313216;1;2333696;2;3382272;3;3388416;2;3390464;3;3396608;2;3399680;3;3405824;2;3408896;3;3414016;2;3418112;3;3425280;2;3427328;3;3431424;2;3436544;3;3444736;2;3445760;3;3552256;2;3556352;3;3577856;2;3592192;3;3606528;2;3619840;3;3631104;2;3632128;3;3633152;2;3638272;3;3644416;2;3647488;3;3656704;2;3666944;3;3667968;2;3675136;3;3689472;2;3705856;3;3710976;2;3712000;3;3722240;2;3741696;3;3753984;2;3760128;3;3761152;2;3769344;3;3774464;2;3785728;3;3793920;2;3804160;3;3811328;2;3813376;3;3815424;2;3822592;3;3833856;2;3852288;3;3856384;2;3859456;3;3869696;2;3889152;3;3899392;2;3913728;3;3924992;2;3926016;3;3927040;2;3932160;3;3949568;2;3959808;3;3987456;2;3998720;3;4004864;2;4005888;3;4014080;2;4015104;3;4029440;2;4033536;3;4077568;2;4079616;3;4106240;2;4107264;3;4136960;2;4144128;3;4164608;2;4183040;3;4198400;2;4199424;3;4203520;2;4208640;3;4219904;2;4227072;3;4278272;2;4290560;3;4310016;2;4327424;3;4366336;2;4370432;3;4384768;2;4391936;3;4405248;2;4410368;3;4421632;2;4423680;3;4432896;2;4438016;3;4458496;2;4475904;3;4482048;2;4484096;3;4487168;2;4502528;3;4512768;2;4521984;3;4538368;2;4550656;3;4551680;2;4557824;3;4647936;2;4648960;3;4724736;2;4726784;3;4730880;2;4731904;3;4757504;2;4768768;3;4813824;2;4814848;3;4838400;2;4842496;3;4845568;2;4851712;3;4866048;2;4879360;3;4886528;2;4887552;3;4898816;2;4899840;3;4900864;2;4916224;3;4939776;2;4942848;3;4956160;2;4961280;3;5022720;2;5025792;3;5045248;2;5063680;3;5104640;2;5108736;3;5133312;2;5136384;3;5143552;2;5144576;3;5161984;2;5164032;3;5171200;2;5172224;3;5180416;2;5182464;3;5189632;2;5191680;3;5198848;2;5200896;3;5209088;2;5210112;3;5215232;2;5218304;3;5310464;2;5312512;3;5313536;2;5314560;3;5316608;2;5319680;3;5346304;2;5357568;3;5372928;2;5382144;3;5383168;2;5384192;3;5398528;2;5402624;3;5427200;2;5429248;3;5465088;2;5467136;3;5489664;2;5498880;3;5500928;2;5504000;3;5548032;2;5549056;3;5794816;4;5795840;3;5798912;4;5820416;3;5926912;2;5945344;3;5956608;2;5971968;3;5983232;2;5990400;3;6013952;2;6019072;3;6024192;2;6027264;3;6043648;2;6054912;3;6077440;2;6081536;3;6083584;2;6091776;3;6107136;2;6110208;3;6135808;2;6137856;3;6148096;2;6156288;3;6162432;2;6165504;3;6167552;2;6174720;3;6192128;2;6193152;3;6196224;2;6202368;3;6225920;2;6239232;3;6253568;2;6265856;3;6274048;2;6276096;3;6277120;2;6285312;3;6291456;2;6293504;3;6302720;2;6311936;3;6317056;2;6321152;3;6364160;2;6367232;3;6374400;2;6376448;3;6382592;2;6385664;3;6393856;2;6394880;3;6403072;2;6404096;3;6451200;2;6460416;3;6523904;2;6534144;3;6535168;2;6536192;3;6541312;2;6542336;3;6545408;2;6552576;3;6555648;2;6559744;3;6576128;2;6578176;3;6600704;2;6606848;3;6610944;2;6612992;3;6631424;2;6644736;3;6647808;2;6651904;3;6667264;2;6679552;3;6694912;2;6699008;3;6704128;2;6707200;3;6716416;2;6717440;3;6718464;2;6725632;3;6738944;2;6744064;3;6814720;2;6815744;3;6816768;2;6827008;3;6862848;2;6872064;3;6894592;2;6900736;3;6906880;2;6908928;3;6946816;2;6955008;3;6961152;2;6964224;3;6971392;2;6973440;3;6988800;2;6991872;3;6995968;2;7001088;3;7007232;2;7010304;3;7011328;2;7019520;3;7030784;2;7037952;3;7111680;2;7120896;3;7255040;2;7267328;3;7282688;2;7294976;3;7304192;2;7313408;3;7322624;2;7333888;3;7339008;2;7341056;3;7401472;2;7414784;3;7454720;2;7455744;3;7456768;2;7460864;3;7484416;2;7489536;3;7578624;2;7579648;3;7585792;2;7588864;3;7596032;2;7598080;3;7624704;2;7625728;3;7649280;4;7652352;3;7696384;2;7708672;3;7840768;2;7856128;3;7869440;2;7875584;3;7878656;2;7882752;3;7894016;2;7901184;3;8043520;2;8048640;3;8220672;2;8221696;3;8253440;2;8346624;1;8359936;0;8360960;1;8368128;0;8378368;1;8379392;2;8492032;1;8505344;0;8508416;1;8513536;0;8517632;1;8520704;2;8525824;1;8526848;2;8636416;1;8649728;0;8655872;1;8659968;0;8673280;1;8674304;2;8778752;1;8795136;0;8803328;1;8805376;0;8811520;1;8814592;2;8856576;1;8898560;0;8912896;1;8921088;0;8922112;1;8928256;0;8950784;1;8951808;0;8994816;1;9001984;0;9069568;1;9072640;0;9142272;1;9146368;0;9217024;1;9219072;0;9289728;1;9293824;0;9363456;1;9367552;0;";
	static float file1VF = 0.4413674f;

	public static int[][] getPeaks() {
		
		string[] data = new string[4];
		data[0] = file1C1;
		data[1] = file1C2;
		data[2] = file1C3;
		data[3] = file1C4;
			
		string[][] stringData = new string[data.Length][];
		
		for(int i = 0; i<data.Length; i++) {
			data[i] = data[i].Substring(data[i].IndexOf(':') + 1);
			stringData[i] = data[i].Split(';');
		}
		
		List<int[]> channelData = new List<int[]>();
		
		foreach(string[] sArr in stringData) {
			List<int> tempList = new List<int>();
			foreach(string s in sArr) {
				int result = -1;
				int.TryParse(s, out result);
				if (result > -1)
					tempList.Add(result);
			}
			channelData.Add(tempList.ToArray());
		}
		
		return channelData.ToArray();		
	}
	
	public static int[] getLoudFlags() {
				
		string data = file1LF;
		
		string[] stringData = data.Substring(data.IndexOf(':') + 1).Split(';');
		
		List<int> loudData = new List<int>();
		
		foreach(string s in stringData) {
			int result = -1;
			int.TryParse(s, out result);
			if (result > -1)
				loudData.Add(result);
		}
		
		return loudData.ToArray();	
	}
	
	public static float getVariationFactor() {
		return file1VF;
	}
	
}