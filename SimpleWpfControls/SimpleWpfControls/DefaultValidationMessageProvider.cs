using System.Globalization;

namespace SimpleWpfControls
{
    /// <summary>
    /// 根据当前 UI 文化返回中文或英文校验提示，默认支持中英文。
    /// </summary>
    public sealed class DefaultValidationMessageProvider : IValidationMessageProvider
    {
        /// <summary>
        /// 判断当前文化是否为中文（名称以 zh 开头视为中文）。
        /// </summary>
        private static bool IsChinese(CultureInfo culture)
        {
            var name = culture.Name;
            return name.StartsWith("zh", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 必填项为空时的提示文案。
        /// </summary>
        public string GetRequiredMessage()
        {
            return IsChinese(CultureInfo.CurrentUICulture)
                ? "此项为必填"
                : "This field is required";
        }

        /// <summary>
        /// 文本长度不足最小长度时的提示文案。
        /// </summary>
        /// <param name="min">最小长度。</param>
        public string GetMinimumLengthMessage(int min)
        {
            return IsChinese(CultureInfo.CurrentUICulture)
                ? $"长度至少 {min} 个字符"
                : $"Minimum length is {min}";
        }

        /// <summary>
        /// 文本长度超过最大长度时的提示文案。
        /// </summary>
        /// <param name="max">最大长度。</param>
        public string GetMaximumLengthMessage(int max)
        {
            return IsChinese(CultureInfo.CurrentUICulture)
                ? $"长度最多 {max} 个字符"
                : $"Maximum length is {max}";
        }

        /// <summary>
        /// 文本长度不在最小、最大区间内的提示文案。
        /// </summary>
        /// <param name="min">最小长度。</param>
        /// <param name="max">最大长度。</param>
        public string GetLengthRangeMessage(int min, int max)
        {
            return IsChinese(CultureInfo.CurrentUICulture)
                ? $"输入长度应在 {min} 到 {max} 个字符之间"
                : $"Length must be between {min} and {max} characters";
        }

        /// <summary>
        /// 数值小于最小值时的提示文案（用于 Integer/Decimal 模式）。
        /// </summary>
        /// <param name="min">最小值。</param>
        public string GetMinimumValueMessage(double min)
        {
            return IsChinese(CultureInfo.CurrentUICulture)
                ? $"数值不能小于 {min}"
                : $"Minimum value is {min}";
        }

        /// <summary>
        /// 数值大于最大值时的提示文案（用于 Integer/Decimal 模式）。
        /// </summary>
        /// <param name="max">最大值。</param>
        public string GetMaximumValueMessage(double max)
        {
            return IsChinese(CultureInfo.CurrentUICulture)
                ? $"数值不能大于 {max}"
                : $"Maximum value is {max}";
        }
    }
}
