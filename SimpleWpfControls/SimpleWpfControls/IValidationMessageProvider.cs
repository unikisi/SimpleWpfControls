namespace SimpleWpfControls
{
    /// <summary>
    /// 提供校验错误文案，用于多语言或自定义提示。
    /// 涵盖 Text 模式的长度校验，以及 Integer/Decimal 模式的数值范围校验。
    /// </summary>
    public interface IValidationMessageProvider
    {
        /// <summary>
        /// 必填项为空时的提示。用于 <see cref="ValidatedTextBox.IsRequired"/> 且输入为空时。
        /// </summary>
        string GetRequiredMessage();

        /// <summary>
        /// 文本长度不足时的提示。用于 Mode=Text 且仅设置了 <see cref="ValidatedTextBox.MinimumLength"/> 时。
        /// </summary>
        /// <param name="min">最小长度。</param>
        string GetMinimumLengthMessage(int min);

        /// <summary>
        /// 文本长度超出时的提示。用于 Mode=Text 且仅设置了 <see cref="ValidatedTextBox.MaximumLength"/> 时。
        /// </summary>
        /// <param name="max">最大长度。</param>
        string GetMaximumLengthMessage(int max);

        /// <summary>
        /// 文本长度不在区间内的提示。用于 Mode=Text 且同时设置了 MinimumLength 与 MaximumLength 时。
        /// </summary>
        /// <param name="min">最小长度。</param>
        /// <param name="max">最大长度。</param>
        string GetLengthRangeMessage(int min, int max);

        /// <summary>
        /// 数值小于最小值时的提示。用于 Mode=Integer 或 Decimal 且设置了 <see cref="ValidatedTextBox.Minimum"/> 时。
        /// </summary>
        /// <param name="min">最小值。</param>
        string GetMinimumValueMessage(double min);

        /// <summary>
        /// 数值大于最大值时的提示。用于 Mode=Integer 或 Decimal 且设置了 <see cref="ValidatedTextBox.Maximum"/> 时。
        /// </summary>
        /// <param name="max">最大值。</param>
        string GetMaximumValueMessage(double max);
    }
}
