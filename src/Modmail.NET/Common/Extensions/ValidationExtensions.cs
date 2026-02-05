using FluentValidation;

namespace Modmail.NET.Common.Extensions;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, string> NotEmptyNotNull<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
            .NotNull();
    }

    public static IRuleBuilderOptions<T, string?> Url<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Matches(Const.UrlRegex);
    }
}