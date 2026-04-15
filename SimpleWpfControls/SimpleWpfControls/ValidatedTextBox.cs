using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleWpfControls
{
    /// <summary>
    /// 带校验的输入框控件。
    /// 支持：文本/整数/小数模式、必填、最小/最大长度、最小/最大数值、小数位数等校验，
    /// 校验状态与提示可绑定，兼容 MVVM。
    /// </summary>
    public class ValidatedTextBox : TextBox
    {
        private sealed class ManualValidationRule : ValidationRule
        {
            public override ValidationResult Validate(object value, CultureInfo cultureInfo) =>
                ValidationResult.ValidResult;
        }

        private static readonly ManualValidationRule WpfManualValidationRule = new();
        private bool _validateOnLoadedByToken;

        /// <summary>
        /// 鍙€夌殑鍗曚綅鏂囨湰锛岃缃悗浼氬湪杈撳叆妗嗗彸渚у唴閮ㄦ樉绀恒€?
        /// </summary>
        public string? Units
        {
            get => (string?)GetValue(UnitsProperty);
            set => SetValue(UnitsProperty, value);
        }

        public static readonly DependencyProperty UnitsProperty =
            DependencyProperty.Register(
                nameof(Units),
                typeof(string),
                typeof(ValidatedTextBox),
                new PropertyMetadata(null));

        static ValidatedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ValidatedTextBox),
                new FrameworkPropertyMetadata(typeof(ValidatedTextBox)));
        }

        public bool ValidateOnLoaded
        {
            get => (bool)GetValue(ValidateOnLoadedProperty);
            set => SetValue(ValidateOnLoadedProperty, value);
        }

        public static readonly DependencyProperty ValidateOnLoadedProperty =
            DependencyProperty.Register(
                nameof(ValidateOnLoaded),
                typeof(bool),
                typeof(ValidatedTextBox),
                new PropertyMetadata(false));

        public int ValidationToken
        {
            get => (int)GetValue(ValidationTokenProperty);
            set => SetValue(ValidationTokenProperty, value);
        }

        public static readonly DependencyProperty ValidationTokenProperty =
            DependencyProperty.Register(
                nameof(ValidationToken),
                typeof(int),
                typeof(ValidatedTextBox),
                new PropertyMetadata(0, OnValidationTokenChanged));

        private static void OnValidationTokenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = (ValidatedTextBox)d;
            if (tb.IsLoaded)
                tb.Validate(false);
            else
                tb._validateOnLoadedByToken = true;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Loaded += ValidatedTextBox_OnLoaded;
        }

        private void ValidatedTextBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= ValidatedTextBox_OnLoaded;
            if (ValidateOnLoaded || _validateOnLoadedByToken)
            {
                _validateOnLoadedByToken = false;
                Validate(false);
            }
        }

        #region 输入模式

        /// <summary>
        /// 输入模式：文本、整数、小数。
        /// </summary>
        public InputMode Mode
        {
            get => (InputMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(
                nameof(Mode),
                typeof(InputMode),
                typeof(ValidatedTextBox),
                new PropertyMetadata(InputMode.Text));

        #endregion

        #region 校验触发

        /// <summary>
        /// 校验触发时机：文本变化时或失去焦点时。
        /// </summary>
        public ValidationTriggerMode ValidationTrigger
        {
            get => (ValidationTriggerMode)GetValue(ValidationTriggerProperty);
            set => SetValue(ValidationTriggerProperty, value);
        }

        public static readonly DependencyProperty ValidationTriggerProperty =
            DependencyProperty.Register(
                nameof(ValidationTrigger),
                typeof(ValidationTriggerMode),
                typeof(ValidatedTextBox),
                new PropertyMetadata(ValidationTriggerMode.OnLostFocus));

        #endregion

        #region 通用校验（文本长度 / 必填）

        /// <summary>
        /// 是否必填；为空时触发必填校验。
        /// </summary>
        public bool IsRequired
        {
            get => (bool)GetValue(IsRequiredProperty);
            set => SetValue(IsRequiredProperty, value);
        }

        public static readonly DependencyProperty IsRequiredProperty =
            DependencyProperty.Register(
                nameof(IsRequired),
                typeof(bool),
                typeof(ValidatedTextBox),
                new PropertyMetadata(false));

        /// <summary>
        /// 文本模式下的最小长度（字符数）。
        /// </summary>
        public int? MinimumLength
        {
            get => (int?)GetValue(MinimumLengthProperty);
            set => SetValue(MinimumLengthProperty, value);
        }

        public static readonly DependencyProperty MinimumLengthProperty =
            DependencyProperty.Register(
                nameof(MinimumLength),
                typeof(int?),
                typeof(ValidatedTextBox),
                new PropertyMetadata(null));

        /// <summary>
        /// 文本模式下的最大长度（字符数）；同时会限制输入框可输入长度。
        /// </summary>
        public int? MaximumLength
        {
            get => (int?)GetValue(MaximumLengthProperty);
            set => SetValue(MaximumLengthProperty, value);
        }

        public static readonly DependencyProperty MaximumLengthProperty =
            DependencyProperty.Register(
                nameof(MaximumLength),
                typeof(int?),
                typeof(ValidatedTextBox),
                new PropertyMetadata(null, OnMaximumLengthChanged));

        private static void OnMaximumLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (ValidatedTextBox)d;
            if (ctrl.MaximumLength.HasValue)
                ctrl.MaxLength = ctrl.MaximumLength.Value;
        }

        #endregion

        #region 数值校验（Integer/Decimal）

        /// <summary>
        /// 数值模式下的最小值。
        /// </summary>
        public double? Minimum
        {
            get => (double?)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                nameof(Minimum),
                typeof(double?),
                typeof(ValidatedTextBox));

        /// <summary>
        /// 数值模式下的最大值。
        /// </summary>
        public double? Maximum
        {
            get => (double?)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                nameof(Maximum),
                typeof(double?),
                typeof(ValidatedTextBox));

        /// <summary>
        /// 小数模式下最多显示/输入几位小数（默认 2）。输入时不允许输入超过该位数；失焦时会将显示格式化为最多该位小数。
        /// </summary>
        public int DecimalPlaces
        {
            get => (int)GetValue(DecimalPlacesProperty);
            set => SetValue(DecimalPlacesProperty, value);
        }

        public static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.Register(
                nameof(DecimalPlaces),
                typeof(int),
                typeof(ValidatedTextBox),
                new PropertyMetadata(2));

        /// <summary>
        /// 数值模式是否允许负数。
        /// </summary>
        public bool AllowNegative
        {
            get => (bool)GetValue(AllowNegativeProperty);
            set => SetValue(AllowNegativeProperty, value);
        }

        public static readonly DependencyProperty AllowNegativeProperty =
            DependencyProperty.Register(
                nameof(AllowNegative),
                typeof(bool),
                typeof(ValidatedTextBox),
                new PropertyMetadata(true));

        #endregion

        #region 校验状态与文案覆盖

        /// <summary>
        /// 当前是否有校验错误（只读，由控件内部设置）。
        /// </summary>
        public bool HasValidationError
        {
            get => (bool)GetValue(HasValidationErrorProperty);
            private set => SetValue(HasValidationErrorPropertyKey, value);
        }

        private static readonly DependencyPropertyKey HasValidationErrorPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(HasValidationError),
                typeof(bool),
                typeof(ValidatedTextBox),
                new PropertyMetadata(false));

        public static readonly DependencyProperty HasValidationErrorProperty =
            HasValidationErrorPropertyKey.DependencyProperty;

        private static IValidationMessageProvider _defaultMessageProvider = new DefaultValidationMessageProvider();

        /// <summary>
        /// 全局默认文案提供者，根据当前 UI 文化返回中/英等。不可设为 null，赋值 null 将抛 <see cref="ArgumentNullException"/>。
        /// </summary>
        public static IValidationMessageProvider DefaultMessageProvider
        {
            get => _defaultMessageProvider;
            set => _defaultMessageProvider = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 必填校验失败时使用的自定义提示；不设置则使用 <see cref="DefaultMessageProvider"/>。
        /// </summary>
        public string? RequiredMessage
        {
            get => (string?)GetValue(RequiredMessageProperty);
            set => SetValue(RequiredMessageProperty, value);
        }

        public static readonly DependencyProperty RequiredMessageProperty =
            DependencyProperty.Register(nameof(RequiredMessage), typeof(string), typeof(ValidatedTextBox), new PropertyMetadata(null));

        /// <summary>
        /// 最小长度校验失败时的自定义提示。
        /// </summary>
        public string? MinimumLengthMessage
        {
            get => (string?)GetValue(MinimumLengthMessageProperty);
            set => SetValue(MinimumLengthMessageProperty, value);
        }

        public static readonly DependencyProperty MinimumLengthMessageProperty =
            DependencyProperty.Register(nameof(MinimumLengthMessage), typeof(string), typeof(ValidatedTextBox), new PropertyMetadata(null));

        /// <summary>
        /// 最大长度校验失败时的自定义提示。
        /// </summary>
        public string? MaximumLengthMessage
        {
            get => (string?)GetValue(MaximumLengthMessageProperty);
            set => SetValue(MaximumLengthMessageProperty, value);
        }

        public static readonly DependencyProperty MaximumLengthMessageProperty =
            DependencyProperty.Register(nameof(MaximumLengthMessage), typeof(string), typeof(ValidatedTextBox), new PropertyMetadata(null));

        /// <summary>
        /// 长度区间校验失败时的自定义提示（同时设置了最小、最大长度时）。
        /// </summary>
        public string? LengthRangeMessage
        {
            get => (string?)GetValue(LengthRangeMessageProperty);
            set => SetValue(LengthRangeMessageProperty, value);
        }

        public static readonly DependencyProperty LengthRangeMessageProperty =
            DependencyProperty.Register(nameof(LengthRangeMessage), typeof(string), typeof(ValidatedTextBox), new PropertyMetadata(null));

        /// <summary>
        /// 数值小于最小值时的自定义提示（Integer/Decimal 模式）。
        /// </summary>
        public string? MinimumValueMessage
        {
            get => (string?)GetValue(MinimumValueMessageProperty);
            set => SetValue(MinimumValueMessageProperty, value);
        }

        public static readonly DependencyProperty MinimumValueMessageProperty =
            DependencyProperty.Register(nameof(MinimumValueMessage), typeof(string), typeof(ValidatedTextBox), new PropertyMetadata(null));

        /// <summary>
        /// 数值大于最大值时的自定义提示（Integer/Decimal 模式）。
        /// </summary>
        public string? MaximumValueMessage
        {
            get => (string?)GetValue(MaximumValueMessageProperty);
            set => SetValue(MaximumValueMessageProperty, value);
        }

        public static readonly DependencyProperty MaximumValueMessageProperty =
            DependencyProperty.Register(nameof(MaximumValueMessage), typeof(string), typeof(ValidatedTextBox), new PropertyMetadata(null));

        /// <summary>
        /// 当前校验错误提示文案（只读，由控件在校验失败时设置）。
        /// </summary>
        public string? ValidationMessage
        {
            get => (string?)GetValue(ValidationMessageProperty);
            private set => SetValue(ValidationMessagePropertyKey, value);
        }

        private static readonly DependencyPropertyKey ValidationMessagePropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ValidationMessage),
                typeof(string),
                typeof(ValidatedTextBox),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ValidationMessageProperty =
            ValidationMessagePropertyKey.DependencyProperty;

        #endregion

        #region 输入过滤与校验触发

        /// <inheritdoc />
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            var newText = GetProposedText(e.Text);
            e.Handled = !IsInputFormatValid(newText);
        }

        /// <inheritdoc />
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            if (ValidationTrigger == ValidationTriggerMode.OnTextChanged)
                Validate();
        }

        /// <inheritdoc />
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            if (Mode == InputMode.Decimal && double.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
            {
                int places = Math.Max(0, Math.Min(DecimalPlaces, 15));
                string format = places == 0 ? "0" : "0." + new string('#', places);
                Text = value.ToString(format, CultureInfo.InvariantCulture);
            }

            if (ValidationTrigger == ValidationTriggerMode.OnLostFocus)
                Validate();
        }

        /// <summary>
        /// 计算若应用当前输入后的完整文本，用于格式校验。
        /// </summary>
        private string GetProposedText(string input)
        {
            var text = Text.Remove(SelectionStart, SelectionLength);
            return text.Insert(SelectionStart, input);
        }

        /// <summary>
        /// 按当前 Mode 判断文本格式是否合法（整数/小数格式及小数位数）。
        /// </summary>
        private bool IsInputFormatValid(string text)
        {
            if (Mode == InputMode.Text)
                return true;

            if (Mode == InputMode.Integer)
            {
                var pattern = AllowNegative ? @"^-?\d*$" : @"^\d*$";
                return Regex.IsMatch(text, pattern);
            }

            if (Mode == InputMode.Decimal)
            {
                var pattern = AllowNegative
                    ? @"^-?\d*(\.\d*)?$"
                    : @"^\d*(\.\d*)?$";

                if (!Regex.IsMatch(text, pattern))
                    return false;

                if (text.Contains("."))
                {
                    var decimals = text.Split('.')[1];
                    if (decimals.Length > DecimalPlaces)
                        return false;
                }

                return true;
            }

            return true;
        }

        #endregion

        #region 校验逻辑与文案解析

        /// <summary>
        /// 解析必填提示：优先实例属性，否则 <see cref="DefaultMessageProvider"/>。
        /// </summary>
        /// <returns></returns>
        private string ResolveRequiredMessage() =>
            !string.IsNullOrEmpty(RequiredMessage) ? RequiredMessage! :
            DefaultMessageProvider.GetRequiredMessage();

        /// <summary>
        /// 解析最小长度提示。
        /// </summary>
        /// <param name="min"></param>
        /// <returns></returns>
        private string ResolveMinimumLengthMessage(int min) =>
            !string.IsNullOrEmpty(MinimumLengthMessage) ? MinimumLengthMessage! :
            DefaultMessageProvider.GetMinimumLengthMessage(min);

        /// <summary>
        /// 解析最大长度提示。
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        private string ResolveMaximumLengthMessage(int max) =>
            !string.IsNullOrEmpty(MaximumLengthMessage) ? MaximumLengthMessage! :
            DefaultMessageProvider.GetMaximumLengthMessage(max);

        /// <summary>
        /// 解析长度区间提示。
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private string ResolveLengthRangeMessage(int min, int max) =>
            !string.IsNullOrEmpty(LengthRangeMessage) ? LengthRangeMessage! :
            DefaultMessageProvider.GetLengthRangeMessage(min, max);

        /// <summary>
        /// 解析数值最小值提示。
        /// </summary>
        /// <param name="min"></param>
        /// <returns></returns>
        private string ResolveMinimumValueMessage(double min) =>
            !string.IsNullOrEmpty(MinimumValueMessage) ? MinimumValueMessage! :
            DefaultMessageProvider.GetMinimumValueMessage(min);

        /// <summary>
        /// 解析数值最大值提示。
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        private string ResolveMaximumValueMessage(double max) =>
            !string.IsNullOrEmpty(MaximumValueMessage) ? MaximumValueMessage! :
            DefaultMessageProvider.GetMaximumValueMessage(max);

        /// <summary>
        /// 执行校验逻辑：必填、文本长度、数值范围等，失败时设置错误状态并弹出提示。
        /// </summary>
        public void Validate()
        {
            Validate(true);
        }

        public void Validate(bool showToolTip)
        {
            ClearError();

            if (IsRequired && string.IsNullOrWhiteSpace(Text))
            {
                SetError(ResolveRequiredMessage(), showToolTip);
                return;
            }

            if (Mode == InputMode.Text)
            {
                var len = Text.Length;
                if (MinimumLength.HasValue && MaximumLength.HasValue)
                {
                    if (len < MinimumLength.Value || len > MaximumLength.Value)
                    {
                        SetError(ResolveLengthRangeMessage(MinimumLength.Value, MaximumLength.Value), showToolTip);
                        return;
                    }
                }
                else if (len < MinimumLength)
                {
                    SetError(ResolveMinimumLengthMessage(MinimumLength.Value), showToolTip);
                    return;
                }
                else if (len > MaximumLength)
                {
                    SetError(ResolveMaximumLengthMessage(MaximumLength.Value), showToolTip);
                    return;
                }
            }

            if (Mode != InputMode.Text && double.TryParse(Text, out var value))
            {
                if (value < Minimum)
                {
                    SetError(ResolveMinimumValueMessage(Minimum.Value), showToolTip);
                    return;
                }

                if (value > Maximum)
                {
                    SetError(ResolveMaximumValueMessage(Maximum.Value), showToolTip);
                    return;
                }
            }
        }

        /// <summary>
        /// 设置校验错误：更新状态、提示文案，并在控件上方自动弹出 ToolTip。
        /// </summary>
        /// <param name="message">错误提示文案。</param>
        private void SetError(string message, bool showToolTip)
        {
            HasValidationError = true;
            ValidationMessage = message;

            var bindingExpression = GetBindingExpression(TextProperty);
            if (bindingExpression != null)
            {
                var error = new ValidationError(WpfManualValidationRule, bindingExpression, message, null);
                Validation.MarkInvalid(bindingExpression, error);
            }

            if (!showToolTip)
            {
                if (ToolTip is ToolTip tt2)
                    tt2.Content = message;
                else
                    ToolTip = message;
                return;
            }

            if (ToolTip is ToolTip tt)
            {
                tt.PlacementTarget = this;
                tt.Content = message;
                tt.StaysOpen = true;
                tt.VerticalOffset = -6;
                // 失焦后再打开，避免被焦点切换吞掉
                Dispatcher.BeginInvoke(new Action(() => tt.IsOpen = true),
                    System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                return;
            }

            var generatedToolTip = new ToolTip
            {
                Placement = System.Windows.Controls.Primitives.PlacementMode.Top,
                PlacementTarget = this,
                Content = message,
                StaysOpen = true,
                VerticalOffset = -6
            };

            ToolTip = generatedToolTip;
            Dispatcher.BeginInvoke(new Action(() => generatedToolTip.IsOpen = true),
                System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }

        /// <summary>
        /// 清除校验错误：关闭 ToolTip 并重置错误状态与文案。
        /// </summary>
        private void ClearError()
        {
            var bindingExpression = GetBindingExpression(TextProperty);
            if (bindingExpression != null)
                Validation.ClearInvalid(bindingExpression);

            if (ToolTip is ToolTip tt)
                tt.IsOpen = false;
            HasValidationError = false;
            ValidationMessage = null;
        }

        #endregion
    }
}
