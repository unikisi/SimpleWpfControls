namespace SimpleWpfControls
{
    /// <summary>
    /// 校验触发时机。
    /// </summary>
    public enum ValidationTriggerMode
    {
        /// <summary>
        /// 文本每次变化时触发校验。
        /// </summary>
        OnTextChanged,

        /// <summary>
        /// 控件失去焦点时触发校验。
        /// </summary>
        OnLostFocus
    }
}
