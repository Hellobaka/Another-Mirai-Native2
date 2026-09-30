// Generated from CQP/XiaoLiZI_API.cs.
extern "C" void __stdcall Function_1(const char* authCode)
{
    return;
}
extern "C" void __stdcall Function_2(const char* authCode)
{
    return;
}
extern "C" int32_t __stdcall Function_3(const char* authCode, const char* arg0)
{
    return 0;
}
extern "C" void __stdcall Function_4(const char* authCode, const char* arg0, const char* arg1)
{
    return;
}
extern "C" const char* __stdcall Function_5(const char* authCode, const char* arg0, int32_t arg1, int32_t arg2)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Argument(arg2)};
    const Json result = xlz::Invoke(5, args);
    return xlz::ReturnText(amn::Text(result));
}
extern "C" const char* __stdcall Function_6(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, int64_t* arg3, int32_t* arg4)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Argument(arg2), Argument(arg3 ? *arg3 : 0), Argument(arg4 ? *arg4 : 0)};
    const Json result = xlz::Invoke(6, args);
    if (arg3) *arg3 = static_cast<int64_t>(amn::Integer(args[4]));
    if (arg4) *arg4 = static_cast<int32_t>(amn::Integer(args[5]));
    return xlz::ReturnText(amn::Text(result));
}
extern "C" const char* __stdcall Function_7(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, int32_t arg3)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Argument(arg2), Argument(arg3)};
    const Json result = xlz::Invoke(7, args);
    return xlz::ReturnText(amn::Text(result));
}
extern "C" const char* __stdcall Function_8(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3, int64_t arg4, int32_t arg5)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Argument(arg2), Argument(arg3), Argument(arg4), Argument(arg5)};
    const Json result = xlz::Invoke(8, args);
    return xlz::ReturnText(amn::Text(result));
}
extern "C" const char* __stdcall Function_9(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, const char* arg3)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_10(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_11(const char* authCode, int64_t arg0, int64_t arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_12(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_13(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_14(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, int64_t* arg3, int32_t* arg4)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Argument(arg2), Argument(arg3 ? *arg3 : 0), Argument(arg4 ? *arg4 : 0)};
    const Json result = xlz::Invoke(14, args);
    if (arg3) *arg3 = static_cast<int64_t>(amn::Integer(args[4]));
    if (arg4) *arg4 = static_cast<int32_t>(amn::Integer(args[5]));
    return xlz::ReturnText(amn::Text(result));
}
extern "C" const char* __stdcall Function_15(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, int32_t arg3)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Argument(arg2), Argument(arg3)};
    const Json result = xlz::Invoke(15, args);
    return xlz::ReturnText(amn::Text(result));
}
extern "C" const char* __stdcall Function_16(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2, const void* arg3, int32_t arg4, int32_t arg5, int32_t arg6)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_17(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2, const void* arg3, int32_t arg4, int32_t arg5, int32_t arg6)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_18(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2, const char* arg3, const void* arg4, int32_t arg5)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_19(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2, const char* arg3, const void* arg4, int32_t arg5)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_20(const char* authCode, int64_t arg0, const void* arg1)
{
    return xlz::ReturnText("");
}
extern "C" void* __stdcall Function_21(const char* authCode, const char* arg0)
{
    return nullptr;
}
extern "C" void* __stdcall Function_22(const char* authCode, const char* arg0)
{
    return nullptr;
}
extern "C" void* __stdcall Function_23(const char* authCode, const char* arg0)
{
    return nullptr;
}
extern "C" const char* __stdcall Function_24(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Argument(arg2), Argument(arg3)};
    const Json result = xlz::Invoke(24, args);
    return xlz::ReturnText(amn::Text(result));
}
extern "C" const char* __stdcall Function_25(const char* authCode, const char* arg0)
{
    JsonArray args{Argument(authCode), Argument(arg0)};
    const Json result = xlz::Invoke(25, args);
    return xlz::ReturnText(amn::Text(result));
}
extern "C" const char* __stdcall Function_26(const char* authCode, int64_t arg0, const char* arg1)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1)};
    const Json result = xlz::Invoke(26, args);
    return xlz::ReturnText(amn::Text(result));
}
extern "C" const char* __stdcall Function_27(const char* authCode, const char* arg0)
{
    JsonArray args{Argument(authCode), Argument(arg0)};
    const Json result = xlz::Invoke(27, args);
    return xlz::ReturnText(amn::Text(result));
}
extern "C" const char* __stdcall Function_28(const char* authCode, int64_t arg0)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_29(const char* authCode, int64_t arg0, const char* arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_30(const char* authCode, int64_t arg0)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_31(const char* authCode)
{
    JsonArray args{Argument(authCode)};
    const Json result = xlz::Invoke(31, args);
    return xlz::ReturnText(amn::Text(result));
}
extern "C" int32_t __stdcall Function_32(const char* authCode, int64_t arg0, void** arg1)
{
    JsonArray args{Argument(authCode), Argument(arg0), Json(static_cast<int64_t>(arg1 ? reinterpret_cast<intptr_t>(*arg1) : 0))};
    const Json result = xlz::Invoke(32, args);
    if (arg1) *arg1 = reinterpret_cast<void*>(static_cast<intptr_t>(amn::Integer(args[2])));
    return static_cast<int32_t>(amn::Integer(result));
}
extern "C" int32_t __stdcall Function_33(const char* authCode, int64_t arg0, void** arg1)
{
    JsonArray args{Argument(authCode), Argument(arg0), Json(static_cast<int64_t>(arg1 ? reinterpret_cast<intptr_t>(*arg1) : 0))};
    const Json result = xlz::Invoke(33, args);
    if (arg1) *arg1 = reinterpret_cast<void*>(static_cast<intptr_t>(amn::Integer(args[2])));
    return static_cast<int32_t>(amn::Integer(result));
}
extern "C" int32_t __stdcall Function_34(const char* authCode, int64_t arg0, int64_t arg1, void** arg2)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Json(static_cast<int64_t>(arg2 ? reinterpret_cast<intptr_t>(*arg2) : 0))};
    const Json result = xlz::Invoke(34, args);
    if (arg2) *arg2 = reinterpret_cast<void*>(static_cast<intptr_t>(amn::Integer(args[3])));
    return static_cast<int32_t>(amn::Integer(result));
}
extern "C" int32_t __stdcall Function_35(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Argument(arg2), Argument(arg3)};
    const Json result = xlz::Invoke(35, args);
    return static_cast<int32_t>(amn::Integer(result));
}
extern "C" const char* __stdcall Function_36(const char* authCode, int64_t arg0, int64_t arg1)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1)};
    const Json result = xlz::Invoke(36, args);
    return xlz::ReturnText(amn::Text(result));
}
extern "C" const char* __stdcall Function_37(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Argument(arg2)};
    const Json result = xlz::Invoke(37, args);
    return xlz::ReturnText(amn::Text(result));
}
extern "C" const char* __stdcall Function_38(const char* authCode, int64_t arg0, int64_t arg1)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_39(const char* authCode, int64_t arg0, const char* arg1)
{
    return 0;
}
extern "C" int32_t __stdcall Function_40(const char* authCode, int64_t arg0, const char* arg1, const char* arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_41(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Argument(arg2), Argument(arg3)};
    const Json result = xlz::Invoke(41, args);
    return static_cast<int32_t>(amn::Integer(result));
}
extern "C" int32_t __stdcall Function_42(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Argument(arg2), Argument(arg3)};
    const Json result = xlz::Invoke(42, args);
    return static_cast<int32_t>(amn::Integer(result));
}
extern "C" int32_t __stdcall Function_43(const char* authCode, int64_t arg0, int64_t arg1)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1)};
    const Json result = xlz::Invoke(43, args);
    return static_cast<int32_t>(amn::Integer(result));
}
extern "C" int32_t __stdcall Function_44(const char* authCode, int64_t arg0, int64_t arg1)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1)};
    const Json result = xlz::Invoke(44, args);
    return static_cast<int32_t>(amn::Integer(result));
}
extern "C" int32_t __stdcall Function_45(const char* authCode, int64_t arg0, int64_t arg1, const void* arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_46(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Argument(arg2)};
    const Json result = xlz::Invoke(46, args);
    return static_cast<int32_t>(amn::Integer(result));
}
extern "C" int32_t __stdcall Function_47(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_48(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_49(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_50(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_51(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_52(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_53(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_54(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_55(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_56(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Argument(arg2), Argument(arg3)};
    const Json result = xlz::Invoke(56, args);
    return static_cast<int32_t>(amn::Integer(result));
}
extern "C" int32_t __stdcall Function_57(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3, int32_t arg4)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Argument(arg2), Argument(arg3), Argument(arg4)};
    const Json result = xlz::Invoke(57, args);
    return static_cast<int32_t>(amn::Integer(result));
}
extern "C" int32_t __stdcall Function_58(const char* authCode, int64_t arg0, int64_t arg1, double arg2, double arg3, int32_t arg4)
{
    return 0;
}
extern "C" int32_t __stdcall Function_59(const char* authCode, int64_t arg0, int64_t arg1, double arg2, double arg3)
{
    return 0;
}
extern "C" int64_t __stdcall Function_60(const char* authCode, int64_t arg0, int64_t arg1)
{
    return 0;
}
extern "C" void __stdcall Function_61(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int64_t arg3, int32_t arg4, int32_t arg5, const char* arg6)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Argument(arg2), Argument(arg3), Argument(arg4), Argument(arg5), Argument(arg6)};
    const Json result = xlz::Invoke(61, args);
    return;
}
extern "C" void __stdcall Function_62(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Argument(arg2), Argument(arg3)};
    const Json result = xlz::Invoke(62, args);
    return;
}
extern "C" void __stdcall Function_63(const char* authCode, int64_t arg0, const char* arg1, void* arg2)
{
    return;
}
extern "C" const char* __stdcall Function_64(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, const char* arg3)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_65(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_66(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, const char* arg3)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_67(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_68(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, const char* arg3)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_69(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_70(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, const char* arg3, const char* arg4)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_71(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, void* arg3)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_72(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int32_t arg3)
{
    return 0;
}
extern "C" const char* __stdcall Function_73(const char* authCode)
{
    JsonArray args{Argument(authCode)};
    const Json result = xlz::Invoke(73, args);
    return xlz::ReturnText(amn::Text(result));
}
extern "C" const char* __stdcall Function_74(const char* authCode, int64_t arg0, int64_t arg1)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1)};
    const Json result = xlz::Invoke(74, args);
    return xlz::ReturnText(amn::Text(result));
}
extern "C" const char* __stdcall Function_75(const char* authCode, const char* arg0, int64_t arg1, int64_t arg2)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Argument(arg2)};
    const Json result = xlz::Invoke(75, args);
    return xlz::ReturnText(amn::Text(result));
}
extern "C" int32_t __stdcall Function_76(const char* authCode, int64_t arg0, int64_t arg1, void** arg2)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Json(static_cast<int64_t>(arg2 ? reinterpret_cast<intptr_t>(*arg2) : 0))};
    const Json result = xlz::Invoke(76, args);
    if (arg2) *arg2 = reinterpret_cast<void*>(static_cast<intptr_t>(amn::Integer(args[3])));
    return static_cast<int32_t>(amn::Integer(result));
}
extern "C" int32_t __stdcall Function_77(const char* authCode, int64_t arg0, int64_t arg1, void** arg2)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Json(static_cast<int64_t>(arg2 ? reinterpret_cast<intptr_t>(*arg2) : 0))};
    const Json result = xlz::Invoke(77, args);
    if (arg2) *arg2 = reinterpret_cast<void*>(static_cast<intptr_t>(amn::Integer(args[3])));
    return static_cast<int32_t>(amn::Integer(result));
}
extern "C" void __stdcall Function_78(const char* authCode)
{
    return;
}
extern "C" int32_t __stdcall Function_79(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_80(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3, const char* arg4, int64_t arg5, int32_t arg6, int64_t arg7, int32_t arg8)
{
    return 0;
}
extern "C" int32_t __stdcall Function_81(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, const char* arg3, int64_t arg4, int32_t arg5, int64_t arg6, int32_t arg7)
{
    return 0;
}
extern "C" int32_t __stdcall Function_82(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" const char* __stdcall Function_83(const char* authCode, int64_t arg0, int64_t arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_84(const char* authCode, int64_t arg0, void* arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_85(const char* authCode, int64_t arg0, const char* arg1, void* arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_86(const char* authCode, int64_t arg0, void* arg1, const char* arg2, const char* arg3)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_87(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, const char* arg3, const char* arg4, const char* arg5, const char* arg6, int32_t arg7, int32_t arg8)
{
    return 0;
}
extern "C" int32_t __stdcall Function_88(const char* authCode, int32_t arg0, const char* arg1)
{
    return 0;
}
extern "C" int32_t __stdcall Function_89(const char* authCode, int32_t arg0, const char* arg1)
{
    return 0;
}
extern "C" const char* __stdcall Function_90(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, const char* arg5, int32_t arg6, void* arg7)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_91(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, int32_t arg5, const char* arg6, int32_t arg7, void* arg8)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_92(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, int32_t arg5, const char* arg6, int32_t arg7, void* arg8)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_93(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, const char* arg5, int32_t arg6, void* arg7)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_94(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, const char* arg5, int32_t arg6, void* arg7)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_95(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, const char* arg5, int32_t arg6, void* arg7)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_96(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, const char* arg5, int32_t arg6, const char* arg7, int32_t arg8, void* arg9)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_97(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, const char* arg5, int32_t arg6, void* arg7)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_98(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, int32_t arg5, const char* arg6, int32_t arg7, void* arg8)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_99(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, const char* arg5, int32_t arg6, void* arg7)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_100(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, const char* arg5, int32_t arg6, void* arg7)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_101(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, const char* arg5, int32_t arg6, void* arg7)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_102(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_103(const char* authCode, int64_t arg0)
{
    return 0;
}
extern "C" int32_t __stdcall Function_104(const char* authCode, int64_t arg0)
{
    return 0;
}
extern "C" int32_t __stdcall Function_105(const char* authCode, int64_t arg0, int64_t arg1, void* arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_106(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_107(const char* authCode, int64_t arg0, const char* arg1, int32_t arg2, const char* arg3, int32_t arg4, const char* arg5, const char* arg6, const char* arg7, const char* arg8, const char* arg9)
{
    return 0;
}
extern "C" const char* __stdcall Function_108(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, const char* arg3)
{
    return xlz::ReturnText("");
}
extern "C" void __stdcall Function_109(const char* authCode, int64_t arg0, int64_t arg1)
{
    return;
}
extern "C" int32_t __stdcall Function_110(const char* authCode, int64_t arg0, int64_t arg1)
{
    return 0;
}
extern "C" int32_t __stdcall Function_111(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2)
{
    return 0;
}
extern "C" const char* __stdcall Function_112(const char* authCode, int64_t arg0, int64_t arg1, void* arg2)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_113(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_114(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" const char* __stdcall Function_115(const char* authCode, int64_t arg0, int64_t arg1)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_116(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2, int64_t arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_117(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_118(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_119(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_120(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int64_t arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_121(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_122(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2)
{
    return 0;
}
extern "C" void __stdcall Function_123(const char* authCode, const char* arg0)
{
    return;
}
extern "C" void __stdcall Function_124(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2)
{
    return;
}
extern "C" int32_t __stdcall Function_125(const char* authCode, int64_t arg0, const char* arg1, const char* arg2, const char* arg3, const char* arg4)
{
    return 0;
}
extern "C" const char* __stdcall Function_126(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, const char* arg3, const void* arg4, const char* arg5, int32_t arg6, int32_t arg7, int32_t arg8, int32_t arg9, int32_t arg10)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_127(const char* authCode)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_128(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, void** arg3)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Argument(arg2), Json(static_cast<int64_t>(arg3 ? reinterpret_cast<intptr_t>(*arg3) : 0))};
    const Json result = xlz::Invoke(128, args);
    if (arg3) *arg3 = reinterpret_cast<void*>(static_cast<intptr_t>(amn::Integer(args[4])));
    return xlz::ReturnText(amn::Text(result));
}
extern "C" const char* __stdcall Function_129(const char* authCode, int64_t arg0)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_130(const char* authCode, int64_t arg0)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_131(const char* authCode, int64_t arg0, int32_t arg1, int64_t arg2, const char* arg3, int32_t arg4, const char* arg5, int32_t arg6, void* arg7)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_132(const char* authCode, int64_t arg0, int32_t arg1, const char* arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_133(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3, const char* arg4, const char* arg5)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_134(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, const char* arg3, const char* arg4)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_135(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, const void* arg3, int32_t arg4, int32_t arg5, int32_t arg6)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_136(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, int64_t arg3, int32_t arg4)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Argument(arg2), Argument(arg3), Argument(arg4)};
    const Json result = xlz::Invoke(136, args);
    return xlz::ReturnText(amn::Text(result));
}
extern "C" const char* __stdcall Function_137(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, int32_t arg3)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1), Argument(arg2), Argument(arg3)};
    const Json result = xlz::Invoke(137, args);
    return xlz::ReturnText(amn::Text(result));
}
extern "C" const char* __stdcall Function_138(const char* authCode, int64_t arg0, int64_t arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_139(const char* authCode, int64_t arg0, int64_t arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_140(const char* authCode, int64_t arg0, int64_t arg1)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_141(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_142(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2)
{
    return 0;
}
extern "C" const char* __stdcall Function_143(const char* authCode, int64_t arg0, const char* arg1, int64_t arg2, int32_t arg3)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_144(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, const void* arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_145(const char* authCode, int64_t arg0)
{
    return 0;
}
extern "C" const char* __stdcall Function_146(const char* authCode, int64_t arg0)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_147(const char* authCode, int64_t arg0, const char* arg1)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_148(const char* authCode, int64_t arg0, int32_t arg1, const char* arg2)
{
    return 0;
}
extern "C" const char* __stdcall Function_149(const char* authCode, int64_t arg0, const void* arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_150(const char* authCode, int64_t arg0, const char* arg1, int32_t arg2, const char* arg3, void* arg4)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_151(const char* authCode, int64_t arg0, const char* arg1, const char* arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_152(const char* authCode, int64_t arg0, const char* arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_153(const char* authCode, int64_t arg0, int64_t arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_154(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, int32_t arg3)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_155(const char* authCode, int64_t arg0, const char* arg1, const char* arg2)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_156(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3)
{
    return 0;
}
extern "C" const char* __stdcall Function_157(const char* authCode, int64_t arg0, int64_t arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_158(const char* authCode, int64_t arg0, int64_t arg1, void* arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_159(const char* authCode, int64_t arg0, void* arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_160(const char* authCode, int64_t arg0, void* arg1)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_161(const char* authCode, int64_t arg0, const char* arg1, const char* arg2)
{
    return 0;
}
extern "C" const char* __stdcall Function_162(const char* authCode)
{
    return xlz::ReturnText("");
}
extern "C" void __stdcall Function_163(const char* authCode, const void* arg0, const void* arg1)
{
    return;
}
extern "C" void __stdcall Function_164(const char* authCode, const void* arg0, const void* arg1)
{
    return;
}
extern "C" const char* __stdcall Function_165(const char* authCode, const char* arg0, int32_t arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_166(const char* authCode, const char* arg0, int32_t arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_167(const char* authCode, int64_t arg0)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_168(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2, int64_t arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_169(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2, const char* arg3, const char* arg4)
{
    return 0;
}
extern "C" int32_t __stdcall Function_170(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_171(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" const char* __stdcall Function_172(const char* authCode, int64_t arg0)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_173(const char* authCode)
{
    return 0;
}
extern "C" int32_t __stdcall Function_174(const char* authCode, int64_t arg0, const char* arg1)
{
    return 0;
}
extern "C" void __stdcall Function_175(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int64_t arg3, int32_t arg4, int32_t arg5, const char* arg6)
{
    return;
}
extern "C" int32_t __stdcall Function_176(const char* authCode, int64_t arg0, const char* arg1)
{
    return 0;
}
extern "C" const char* __stdcall Function_177(const char* authCode, int64_t arg0, int64_t arg1, void* arg2, int64_t arg3, int32_t arg4, const char* arg5)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_178(const char* authCode, int64_t arg0, int64_t arg1, void* arg2, int32_t arg3, const char* arg4)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_179(const char* authCode, const char* arg0)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_180(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, const void* arg3, int32_t arg4)
{
    return 0;
}
extern "C" int32_t __stdcall Function_181(const char* authCode, const char* arg0, const char* arg1, void* arg2, const char* arg3, int32_t arg4)
{
    return 0;
}
extern "C" const char* __stdcall Function_182(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, int32_t arg3)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_183(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_184(const char* authCode, const char* arg0)
{
    return 0;
}
extern "C" void __stdcall Function_185(const char* authCode, const char* arg0, const char* arg1, const char* arg2, int32_t arg3)
{
    return;
}
extern "C" void __stdcall Function_186(const char* authCode, const char* arg0, const char* arg1, const char* arg2, int32_t arg3, const char* arg4)
{
    return;
}
extern "C" const char* __stdcall Function_187(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_188(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_189(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_190(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3, int64_t arg4, int32_t arg5)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_191(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3)
{
    return 0;
}
extern "C" const char* __stdcall Function_192(const char* authCode, int64_t arg0, int64_t arg1, const void* arg2, const char* arg3, int64_t arg4, int32_t arg5)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_193(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, int64_t arg3, int32_t arg4)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_194(const char* authCode, const char* arg0)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_195(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_196(const char* authCode, int64_t arg0, int64_t arg1, void* arg2)
{
    return 0;
}
extern "C" int64_t __stdcall Function_197(const char* authCode, int64_t arg0, int64_t arg1)
{
    return 0;
}
extern "C" int32_t __stdcall Function_198(const char* authCode, int64_t arg0, void* arg1)
{
    return 0;
}
extern "C" int32_t __stdcall Function_199(const char* authCode, int64_t arg0, void* arg1)
{
    return 0;
}
extern "C" int32_t __stdcall Function_200(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int64_t arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_201(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int64_t arg3)
{
    return 0;
}
extern "C" const char* __stdcall Function_202(const char* authCode)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_203(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, const char* arg5, int32_t arg6, void* arg7)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_204(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, int32_t arg5, const char* arg6, int32_t arg7, void* arg8)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_205(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, int32_t arg5, const char* arg6, int32_t arg7, void* arg8)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_206(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, const char* arg5, int32_t arg6, void* arg7)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_207(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, const char* arg5, int32_t arg6, void* arg7)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_208(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, const char* arg5, int32_t arg6, void* arg7)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_209(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, const char* arg5, int32_t arg6, const char* arg7, int32_t arg8, void* arg9)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_210(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_211(const char* authCode, int64_t arg0, int64_t arg1, void* arg2)
{
    return 0;
}
extern "C" const char* __stdcall Function_212(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, const char* arg3)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_213(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, int64_t arg3, int32_t arg4)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_214(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int64_t arg3)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_215(const char* authCode, int64_t arg0, int64_t arg1, void* arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_216(const char* authCode, int64_t arg0, int64_t arg1)
{
    return 0;
}
extern "C" int32_t __stdcall Function_217(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_218(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_219(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_220(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_221(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3, const char* arg4, int64_t arg5)
{
    return 0;
}
extern "C" int32_t __stdcall Function_222(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3, const char* arg4, int64_t arg5, int32_t arg6, int64_t arg7, int32_t arg8)
{
    return 0;
}
extern "C" const char* __stdcall Function_223(const char* authCode, int64_t arg0, int32_t arg1)
{
    JsonArray args{Argument(authCode), Argument(arg0), Argument(arg1)};
    const Json result = xlz::Invoke(223, args);
    return xlz::ReturnText(amn::Text(result));
}
extern "C" const char* __stdcall Function_224(const char* authCode, int64_t arg0)
{
    JsonArray args{Argument(authCode), Argument(arg0)};
    const Json result = xlz::Invoke(224, args);
    return xlz::ReturnText(amn::Text(result));
}
extern "C" const char* __stdcall Function_225(const char* authCode, const char* arg0, int32_t arg1, int32_t arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_226(const char* authCode, int64_t arg0, int64_t arg1, void* arg2, const char* arg3, const char* arg4)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_227(const char* authCode, int64_t arg0, const char* arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_228(const char* authCode, int64_t arg0, const char* arg1, void* arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_229(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, int32_t arg3, const char* arg4, int32_t arg5, void* arg6)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_230(const char* authCode, int64_t arg0, int64_t arg1, void* arg2, const char* arg3)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_231(const char* authCode, int64_t arg0, const char* arg1)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_232(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2)
{
    return 0;
}
extern "C" const char* __stdcall Function_233(const char* authCode, int64_t arg0, const char* arg1)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_234(const char* authCode)
{
    return 0;
}
extern "C" const char* __stdcall Function_235(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, const char* arg5, int32_t arg6, void* arg7)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_236(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, const char* arg5, int32_t arg6, void* arg7)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_237(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, const char* arg4, const char* arg5, int32_t arg6, void* arg7)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_238(const char* authCode, int64_t arg0, const char* arg1, int32_t arg2, const char* arg3, int32_t arg4, void* arg5)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_239(const char* authCode, int64_t arg0, const char* arg1, const char* arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_240(const char* authCode, int64_t arg0, const char* arg1, const char* arg2)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_241(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2, const char* arg3)
{
    return 0;
}
extern "C" void* __stdcall Function_242(const char* authCode)
{
    return nullptr;
}
extern "C" const char* __stdcall Function_243(const char* authCode, int64_t arg0, const char* arg1, const void* arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_244(const char* authCode, int64_t arg0, const char* arg1, const char* arg2, const char* arg3, const char* arg4)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_245(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_246(const char* authCode, int64_t arg0, const char* arg1, const char* arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_247(const char* authCode, int64_t arg0, const void* arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_248(const char* authCode, int64_t arg0)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_249(const char* authCode, int64_t arg0, int64_t arg1)
{
    return 0;
}
extern "C" const char* __stdcall Function_250(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, int64_t arg3, int32_t arg4)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_251(const char* authCode, int64_t arg0, int64_t arg1, const void* arg2, const char* arg3, int64_t arg4, int32_t arg5)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_252(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int64_t arg3)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_253(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int64_t arg3)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_254(const char* authCode, int64_t arg0, int32_t arg1, const void* arg2)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_255(const char* authCode, int64_t arg0, int64_t arg1, void* arg2)
{
    return 0;
}
extern "C" const char* __stdcall Function_256(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, int32_t arg3, int64_t arg4, int32_t arg5)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_257(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, const char* arg3, int32_t arg4)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_258(const char* authCode, int64_t arg0, int64_t arg1)
{
    return 0;
}
extern "C" int32_t __stdcall Function_259(const char* authCode, int64_t arg0, int64_t arg1)
{
    return 0;
}
extern "C" const char* __stdcall Function_260(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, const char* arg3)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_261(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_262(const char* authCode, int64_t arg0, int64_t arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_263(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, int32_t arg3)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_264(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, const char* arg3)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_265(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, const char* arg3, const char* arg4, int32_t arg5)
{
    return xlz::ReturnText("");
}
extern "C" int64_t __stdcall Function_266(const char* authCode, int64_t arg0)
{
    return 0;
}
extern "C" const char* __stdcall Function_267(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const void* arg3, int32_t arg4, int32_t arg5, int32_t arg6)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_268(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_269(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_270(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int64_t arg3, int64_t arg4)
{
    return 0;
}
extern "C" int32_t __stdcall Function_271(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3, const char* arg4, int32_t arg5, int32_t arg6)
{
    return 0;
}
extern "C" const char* __stdcall Function_272(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_273(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_274(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3, int32_t arg4)
{
    return 0;
}
extern "C" int32_t __stdcall Function_275(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_276(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_277(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_278(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_279(const char* authCode, int64_t arg0, int64_t arg1)
{
    return 0;
}
extern "C" int32_t __stdcall Function_280(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, int64_t arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_281(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_282(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_283(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3, int64_t arg4, int32_t arg5, int64_t arg6, int32_t arg7)
{
    return 0;
}
extern "C" int32_t __stdcall Function_284(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3, int64_t arg4, int32_t arg5, int64_t arg6, int32_t arg7)
{
    return 0;
}
extern "C" int32_t __stdcall Function_285(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_286(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_287(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_288(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_289(const char* authCode, int64_t arg0, const char* arg1)
{
    return 0;
}
extern "C" int32_t __stdcall Function_290(const char* authCode, int64_t arg0, int32_t arg1)
{
    return 0;
}
extern "C" int32_t __stdcall Function_291(const char* authCode, int64_t arg0, int32_t arg1)
{
    return 0;
}
extern "C" int32_t __stdcall Function_292(const char* authCode, int64_t arg0, int32_t arg1, const char* arg2, int32_t arg3, const char* arg4, int32_t arg5, const char* arg6)
{
    return 0;
}
extern "C" int32_t __stdcall Function_293(const char* authCode, int64_t arg0, int32_t arg1)
{
    return 0;
}
extern "C" int32_t __stdcall Function_294(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2, const char* arg3, const char* arg4)
{
    return 0;
}
extern "C" int32_t __stdcall Function_295(const char* authCode, int64_t arg0, const char* arg1, int32_t arg2, void* arg3)
{
    return 0;
}
extern "C" const char* __stdcall Function_296(const char* authCode, int64_t arg0)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_297(const char* authCode, int64_t arg0, int32_t arg1)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_298(const char* authCode, int64_t arg0, int64_t arg1, void* arg2, int32_t arg3, const char* arg4)
{
    return 0;
}
extern "C" int32_t __stdcall Function_299(const char* authCode, int64_t arg0, int64_t arg1, void* arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_300(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2, const char* arg3)
{
    return 0;
}
extern "C" const char* __stdcall Function_301(const char* authCode, int64_t arg0, int32_t arg1, int64_t arg2, const char* arg3, const char* arg4)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_302(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3, const char* arg4)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_303(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, int64_t arg4, const char* arg5, int32_t arg6, const char* arg7, int32_t arg8, void* arg9)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_304(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, int64_t arg4, const char* arg5, int32_t arg6, const char* arg7, int32_t arg8, void* arg9)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_305(const char* authCode, int64_t arg0, int32_t arg1, int32_t arg2, int64_t arg3, int64_t arg4, int64_t arg5, const char* arg6, const char* arg7, int32_t arg8, void* arg9)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_306(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int64_t arg3, const char* arg4)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_307(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, void* arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_308(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int64_t arg3, int32_t arg4)
{
    return 0;
}
extern "C" int32_t __stdcall Function_309(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3, int64_t arg4, int32_t arg5)
{
    return 0;
}
extern "C" int32_t __stdcall Function_310(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2)
{
    return 0;
}
extern "C" int64_t __stdcall Function_311(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, int64_t arg3, int32_t arg4)
{
    return 0;
}
extern "C" int32_t __stdcall Function_312(const char* authCode, int64_t arg0, int64_t arg1, void* arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_313(const char* authCode, int64_t arg0, int64_t arg1, void* arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_314(const char* authCode, int64_t arg0, int64_t arg1, void* arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_315(const char* authCode, int64_t arg0, int64_t arg1, void* arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_316(const char* authCode, int64_t arg0)
{
    return 0;
}
extern "C" const char* __stdcall Function_317(const char* authCode, int64_t arg0)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_318(const char* authCode, const char* arg0)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_319(const char* authCode, const char* arg0)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_320(const char* authCode, const char* arg0, const char* arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_321(const char* authCode, const char* arg0, const char* arg1)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_322(const char* authCode, int64_t arg0, int64_t arg1, void* arg2)
{
    return 0;
}
extern "C" const char* __stdcall Function_323(const char* authCode, int64_t arg0)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_324(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3, int32_t arg4, int32_t arg5, int32_t arg6)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_325(const char* authCode, int32_t arg0, const char* arg1)
{
    return 0;
}
extern "C" const char* __stdcall Function_326(const char* authCode, const char* arg0)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_327(const char* authCode, const char* arg0)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_328(const char* authCode, const char* arg0)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_329(const char* authCode, const char* arg0)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_330(const char* authCode, const char* arg0)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_331(const char* authCode, const char* arg0)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_332(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, void* arg3, int32_t arg4)
{
    return 0;
}
extern "C" const char* __stdcall Function_333(const char* authCode, int64_t arg0, int64_t arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_334(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_335(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_336(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, const char* arg3, int32_t arg4, int32_t arg5, int32_t arg6, int32_t arg7, int32_t arg8, int64_t arg9, int64_t arg10, int32_t arg11, int32_t arg12)
{
    return 0;
}
extern "C" const char* __stdcall Function_337(const char* authCode, const char* arg0, int32_t arg1, int32_t arg2)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_338(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3, const char* arg4, const char* arg5, const char* arg6, const char* arg7, int32_t arg8)
{
    return 0;
}
extern "C" int32_t __stdcall Function_339(const char* authCode, int64_t arg0, int64_t arg1)
{
    return 0;
}
extern "C" int32_t __stdcall Function_340(const char* authCode, int64_t arg0, const char* arg1, int32_t arg2)
{
    return 0;
}
extern "C" const char* __stdcall Function_341(const char* authCode, int64_t arg0, int64_t arg1)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_342(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, void* arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_343(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int64_t arg3, int32_t arg4)
{
    return 0;
}
extern "C" int32_t __stdcall Function_344(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int64_t arg3, int32_t arg4)
{
    return 0;
}
extern "C" int32_t __stdcall Function_345(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int64_t arg3, int32_t arg4)
{
    return 0;
}
extern "C" int32_t __stdcall Function_346(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int64_t arg3, int32_t arg4)
{
    return 0;
}
extern "C" const char* __stdcall Function_347(const char* authCode, int64_t arg0, int64_t arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_348(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_349(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3)
{
    return 0;
}
extern "C" const char* __stdcall Function_350(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3, int32_t arg4, int32_t arg5)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_351(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, void* arg3, const char* arg4)
{
    return 0;
}
extern "C" int32_t __stdcall Function_352(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int64_t arg3, void* arg4)
{
    return 0;
}
extern "C" const char* __stdcall Function_353(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int64_t arg3)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_354(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int64_t arg3, void* arg4)
{
    return 0;
}
extern "C" int32_t __stdcall Function_355(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3, const char* arg4, int64_t arg5, int64_t arg6, int32_t arg7, int64_t arg8, void* arg9)
{
    return 0;
}
extern "C" const char* __stdcall Function_356(const char* authCode, int64_t arg0, int64_t arg1)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_357(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, void* arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_358(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_359(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_360(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_361(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" const char* __stdcall Function_362(const char* authCode, int64_t arg0, const char* arg1, const char* arg2, const char* arg3, int32_t arg4, const char* arg5)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_363(const char* authCode, int64_t arg0, int32_t arg1)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_364(const char* authCode, int64_t arg0)
{
    return 0;
}
extern "C" int32_t __stdcall Function_365(const char* authCode, int64_t arg0)
{
    return 0;
}
extern "C" const char* __stdcall Function_366(const char* authCode)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_367(const char* authCode, int64_t arg0)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_368(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int32_t arg3)
{
    return 0;
}
extern "C" const char* __stdcall Function_369(const char* authCode, int64_t arg0, int64_t arg1)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_370(const char* authCode, int64_t arg0, int64_t arg1, double arg2, double arg3, int32_t arg4, int32_t arg5)
{
    return 0;
}
extern "C" int32_t __stdcall Function_371(const char* authCode, int64_t arg0, int64_t arg1, double arg2, double arg3, double arg4)
{
    return 0;
}
extern "C" int32_t __stdcall Function_372(const char* authCode, int64_t arg0, int64_t arg1, int32_t arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_373(const char* authCode, int64_t arg0, int32_t arg1, const char* arg2)
{
    return 0;
}
extern "C" int32_t __stdcall Function_374(const char* authCode, int64_t arg0, int32_t arg1)
{
    return 0;
}
extern "C" const char* __stdcall Function_375(const char* authCode, int64_t arg0)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_376(const char* authCode, int64_t arg0, const char* arg1)
{
    return 0;
}
extern "C" const char* __stdcall Function_377(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, const char* arg3, const char* arg4, const char* arg5, int64_t arg6)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_378(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, const char* arg3, const char* arg4, const char* arg5, int64_t arg6, int32_t arg7)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_379(const char* authCode, int64_t arg0, int64_t arg1, const void* arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_380(const char* authCode, int64_t arg0, const char* arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_381(const char* authCode, int64_t arg0, const char* arg1, const char* arg2, const char* arg3)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_382(const char* authCode, int64_t arg0, const char* arg1, const char* arg2)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_383(const char* authCode, int64_t arg0, const char* arg1, int32_t arg2, const void* arg3)
{
    return 0;
}
extern "C" const char* __stdcall Function_384(const char* authCode, int64_t arg0, const char* arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_385(const char* authCode, int64_t arg0)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_386(const char* authCode, int64_t arg0, const char* arg1)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_387(const char* authCode, int64_t arg0)
{
    return 0;
}
extern "C" int32_t __stdcall Function_388(const char* authCode, int64_t arg0)
{
    return 0;
}
extern "C" const char* __stdcall Function_389(const char* authCode, int64_t arg0, int32_t arg1, const char* arg2, int32_t arg3)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_390(const char* authCode, int64_t arg0)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_391(const char* authCode, int64_t arg0)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_392(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, int32_t arg3)
{
    return 0;
}
extern "C" int32_t __stdcall Function_393(const char* authCode, int64_t arg0, int64_t arg1, const char* arg2, const char* arg3)
{
    return 0;
}
extern "C" const char* __stdcall Function_394(const char* authCode, int64_t arg0)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_395(const char* authCode, int64_t arg0, const char* arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_396(const char* authCode, int64_t arg0, const char* arg1)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_397(const char* authCode, int64_t arg0, double arg1, double arg2)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_398(const char* authCode)
{
    return xlz::ReturnText("");
}
extern "C" const char* __stdcall Function_399(const char* authCode, int64_t arg0, int64_t arg1, int64_t arg2, int64_t arg3, const char* arg4, int32_t arg5, int64_t arg6, int32_t arg7)
{
    return xlz::ReturnText("");
}
extern "C" int32_t __stdcall Function_400(const char* authCode, int64_t arg0, const char* arg1)
{
    return 0;
}
