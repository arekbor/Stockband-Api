using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Stockband.Domain.Exceptions;

namespace Stockband.Domain.Common;

public class DictionaryResponse<TEnum>
    where TEnum:Enum
{
    public TEnum EnumId { get; set; }
    public string Value { get; set; }

    public static List<DictionaryResponse<TEnum>> GetEnumDictionary()
    {
        List<DictionaryResponse<TEnum>> dictionaryResponses = 
            new List<DictionaryResponse<TEnum>>();
        
        foreach (var i in Enum.GetValues(typeof(TEnum)))
        {
            dictionaryResponses.Add(new DictionaryResponse<TEnum>
            {
                EnumId = (TEnum)i,
                Value = GetDisplayEnumDictionaryField((TEnum)i)
            });
        }
        return dictionaryResponses;
    }

    private static string GetDisplayEnumDictionaryField(Enum enumValue)
    {
        MemberInfo? memberInfo = enumValue
            .GetType()
            .GetMember(enumValue.ToString())
            .FirstOrDefault();

        if (memberInfo == null)
        {
            throw new ObjectNotFound(typeof(MemberInfo));
        }

        DisplayAttribute? displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>();
        if (displayAttribute == null)
        {
            throw new ObjectNotFound(typeof(DisplayAttribute));
        }
        
        string? name = displayAttribute.Name;
        if (name == null)
        {
            throw new ObjectNotFound(typeof(string));
        }
        return name;
    }
}