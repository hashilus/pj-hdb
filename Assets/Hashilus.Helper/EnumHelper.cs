using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumHelper
{
    public static EnumT Random<EnumT>()
    {
        return Enum.GetValues(typeof(EnumT)).Cast<EnumT>().Sample();
    }

    public static EnumT RandomExcept<EnumT>(EnumT v)
    {
        return Enum.GetValues(typeof(EnumT)).Cast<EnumT>().Where(e => !e.Equals(v)).Sample();
    }

    public static IEnumerable<string> GetNames<EnumT>()
    {
        return Enum.GetNames(typeof(EnumT));
    }

    public static IEnumerable<EnumT> GetValues<EnumT>()
    {
        return Enum.GetValues(typeof(EnumT)).Cast<EnumT>();
    }
}
