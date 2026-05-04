using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace cvapp.ui.web.mvc.Extensions;

public static class EnumDisplayExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        if (member == null) return value.ToString();

        var displayAttribute = member.GetCustomAttribute<DisplayAttribute>();
        return displayAttribute?.GetName() ?? value.ToString();
    }
}

