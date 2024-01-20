using System;

namespace MG.Http.Urls.Internal
{
    internal static class LengthConstants
    {
        internal const int INT_MAX = 11;
        internal const int LONG_MAX = 20;
        internal const int UINT_MAX = INT_MAX - 1;
        internal const int ULONG_MAX = LONG_MAX;
        internal const int DOUBLE_MAX = 24;    // double.MinValue.ToString().Length
        internal const int DECIMAL_MAX = 30;
        internal const int INT128_MAX = 40;

        internal const int GUID_FORM_B_OR_P = 38;
        internal const int GUID_FORM_N = 32;
        internal const int GUID_FORM_D = 36;
        internal const int GUID_FORM_X = 68;

        internal const int HTTP_STATUS_CODE_MAX = 29; // Enum.GetNames<HttpStatusCode>().Max(x => x.Length)
    }
}