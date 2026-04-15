using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using SimpleWpfControls;

namespace SimpleWpfControlsTests
{
    public sealed class MainViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public sealed record CultureOption(string Name);

        public sealed record ProviderOption(string Key, string DisplayName);

        public IReadOnlyList<CultureOption> Cultures { get; } =
            new List<CultureOption>
            {
                new("zh-CN"),
                new("en-US")
            };

        public IReadOnlyList<ProviderOption> Providers { get; } =
            new List<ProviderOption>
            {
                new("default", "DefaultValidationMessageProvider"),
                new("custom", "CustomValidationMessageProvider")
            };

        private CultureOption? _selectedCulture;
        public CultureOption? SelectedCulture
        {
            get => _selectedCulture;
            set
            {
                if (_selectedCulture == value) return;
                _selectedCulture = value;
                OnPropertyChanged();
                ApplyCulture();
            }
        }

        private ProviderOption? _selectedProvider;
        public ProviderOption? SelectedProvider
        {
            get => _selectedProvider;
            set
            {
                if (_selectedProvider == value) return;
                _selectedProvider = value;
                OnPropertyChanged();
                ApplyProvider();
            }
        }

        private string _currentCultureText = "";
        public string CurrentCultureText
        {
            get => _currentCultureText;
            private set { if (_currentCultureText == value) return; _currentCultureText = value; OnPropertyChanged(); }
        }

        private string _lastActionText = "";
        public string LastActionText
        {
            get => _lastActionText;
            private set { if (_lastActionText == value) return; _lastActionText = value; OnPropertyChanged(); }
        }

        private string _reportText = "";
        public string ReportText
        {
            get => _reportText;
            private set { if (_reportText == value) return; _reportText = value; OnPropertyChanged(); }
        }

        public RelayCommand ValidateAllCommand { get; }
        public RelayCommand CheckErrorsCommand { get; }
        public RelayCommand NextCommand { get; }
        public RelayCommand SetValidSamplesCommand { get; }
        public RelayCommand SetInvalidSamplesCommand { get; }

        private bool _showErrors;

        private readonly Dictionary<string, List<string>> _errors = new();
        private static readonly string[] ValidatedProperties =
        [
            nameof(TextRequiredRange),
            nameof(TextCustomMessages),
            nameof(IntAge),
            nameof(IntTemperature),
            nameof(IntTriggerTextChanged),
            nameof(IntTriggerLostFocus),
            nameof(DecWeight),
            nameof(DecRate)
        ];

        public MainViewModel()
        {
            ValidateAllCommand = new RelayCommand(ValidateAll);
            CheckErrorsCommand = new RelayCommand(CheckErrors);
            NextCommand = new RelayCommand(Next);
            SetValidSamplesCommand = new RelayCommand(SetValidSamples);
            SetInvalidSamplesCommand = new RelayCommand(SetInvalidSamples);

            SelectedCulture = Cultures.FirstOrDefault(c => CultureInfo.CurrentUICulture.Name.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
                             ?? Cultures.FirstOrDefault();
            SelectedProvider = Providers.FirstOrDefault(p => p.Key == "default") ?? Providers.FirstOrDefault();

            LastActionText = "";
            ReportText = "";
            _showErrors = false;
        }

        private string? _textRequiredRange;
        public string? TextRequiredRange
        {
            get => _textRequiredRange;
            set
            {
                if (_textRequiredRange == value) return;
                _textRequiredRange = value;
                OnPropertyChanged();
                ValidateTextRequiredRange();
                UpdateReportTextIfShowing();
            }
        }

        private string? _textCustomMessages;
        public string? TextCustomMessages
        {
            get => _textCustomMessages;
            set
            {
                if (_textCustomMessages == value) return;
                _textCustomMessages = value;
                OnPropertyChanged();
                ValidateTextCustomMessages();
                UpdateReportTextIfShowing();
            }
        }

        private string? _intAge;
        public string? IntAge
        {
            get => _intAge;
            set
            {
                if (_intAge == value) return;
                _intAge = value;
                OnPropertyChanged();
                ValidateIntAge();
                UpdateReportTextIfShowing();
            }
        }

        private string? _intTemperature;
        public string? IntTemperature
        {
            get => _intTemperature;
            set
            {
                if (_intTemperature == value) return;
                _intTemperature = value;
                OnPropertyChanged();
                ValidateIntTemperature();
                UpdateReportTextIfShowing();
            }
        }

        private string? _intTriggerTextChanged;
        public string? IntTriggerTextChanged
        {
            get => _intTriggerTextChanged;
            set
            {
                if (_intTriggerTextChanged == value) return;
                _intTriggerTextChanged = value;
                OnPropertyChanged();
                ValidateIntTriggerTextChanged();
                UpdateReportTextIfShowing();
            }
        }

        private string? _intTriggerLostFocus;
        public string? IntTriggerLostFocus
        {
            get => _intTriggerLostFocus;
            set
            {
                if (_intTriggerLostFocus == value) return;
                _intTriggerLostFocus = value;
                OnPropertyChanged();
                ValidateIntTriggerLostFocus();
                UpdateReportTextIfShowing();
            }
        }

        private string? _decWeight;
        public string? DecWeight
        {
            get => _decWeight;
            set
            {
                if (_decWeight == value) return;
                _decWeight = value;
                OnPropertyChanged();
                ValidateDecWeight();
                UpdateReportTextIfShowing();
            }
        }

        private string? _decRate;
        public string? DecRate
        {
            get => _decRate;
            set
            {
                if (_decRate == value) return;
                _decRate = value;
                OnPropertyChanged();
                ValidateDecRate();
                UpdateReportTextIfShowing();
            }
        }

        private void SetValidSamples()
        {
            TextRequiredRange = "abcd";
            TextCustomMessages = "abcd";
            IntAge = "18";
            IntTemperature = "-10";
            IntTriggerTextChanged = "5";
            IntTriggerLostFocus = "5";
            DecWeight = "12.34";
            DecRate = "-1.234";
        }

        private void SetInvalidSamples()
        {
            TextRequiredRange = "";
            TextCustomMessages = "ab";
            IntAge = "200";
            IntTemperature = "100";
            IntTriggerTextChanged = "0";
            IntTriggerLostFocus = "0";
            DecWeight = "200";
            DecRate = "10.001";
        }

        private void ApplyCulture()
        {
            var name = SelectedCulture?.Name;
            if (string.IsNullOrWhiteSpace(name))
                return;

            var culture = new CultureInfo(name);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            CurrentCultureText = $"CurrentCulture={CultureInfo.CurrentCulture.Name}  CurrentUICulture={CultureInfo.CurrentUICulture.Name}";
            ValidateAll();
            UpdateReportTextIfShowing();
        }

        private void ApplyProvider()
        {
            var key = SelectedProvider?.Key;
            ValidatedTextBox.DefaultMessageProvider = key == "custom"
                ? new CustomValidationMessageProvider()
                : new DefaultValidationMessageProvider();

            ValidateAll();
            UpdateReportTextIfShowing();
        }

        private void ValidateAll()
        {
            foreach (var name in ValidatedProperties)
                ValidateProperty(name);
        }

        private void CheckErrors()
        {
            _showErrors = true;
            ValidateAll();
            RaiseAllErrorsChanged();
            ReportText = BuildReportText();
            LastActionText = HasErrors ? "已执行检查：发现错误（见验证报告）" : "已执行检查：无错误";
        }

        private void Next()
        {
            _showErrors = true;
            ValidateAll();
            RaiseAllErrorsChanged();
            ReportText = BuildReportText();
            LastActionText = HasErrors ? "已点击 Next：存在错误，已拦截（见验证报告）" : "已点击 Next：无错误，可继续";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public bool HasErrors => _showErrors && _errors.Values.Any(list => list.Count > 0);

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (!_showErrors)
                return Enumerable.Empty<string>();

            if (string.IsNullOrWhiteSpace(propertyName))
                return _errors.Values.SelectMany(x => x);

            return _errors.TryGetValue(propertyName, out var list) ? list : Enumerable.Empty<string>();
        }

        private void RaiseAllErrorsChanged()
        {
            foreach (var name in ValidatedProperties)
                RaiseErrorsChanged(name);
            OnPropertyChanged(nameof(HasErrors));
        }

        private void RaiseErrorsChanged(string propertyName) =>
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

        private void SetErrors(string propertyName, List<string> errors)
        {
            _errors[propertyName] = errors;
            if (_showErrors)
                RaiseErrorsChanged(propertyName);
            OnPropertyChanged(nameof(HasErrors));
        }

        private static bool IsNullOrWhiteSpace(string? s) => string.IsNullOrWhiteSpace(s);

        private void ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(TextRequiredRange):
                    ValidateTextRequiredRange();
                    break;
                case nameof(TextCustomMessages):
                    ValidateTextCustomMessages();
                    break;
                case nameof(IntAge):
                    ValidateIntAge();
                    break;
                case nameof(IntTemperature):
                    ValidateIntTemperature();
                    break;
                case nameof(IntTriggerTextChanged):
                    ValidateIntTriggerTextChanged();
                    break;
                case nameof(IntTriggerLostFocus):
                    ValidateIntTriggerLostFocus();
                    break;
                case nameof(DecWeight):
                    ValidateDecWeight();
                    break;
                case nameof(DecRate):
                    ValidateDecRate();
                    break;
                default:
                    _errors[propertyName] = new List<string>();
                    break;
            }
        }

        private void ValidateTextRequiredRange()
        {
            var errors = new List<string>();
            if (IsNullOrWhiteSpace(TextRequiredRange))
                errors.Add(ValidatedTextBox.DefaultMessageProvider.GetRequiredMessage());
            else
            {
                var len = TextRequiredRange!.Length;
                if (len < 2 || len > 5)
                    errors.Add(ValidatedTextBox.DefaultMessageProvider.GetLengthRangeMessage(2, 5));
            }
            SetErrors(nameof(TextRequiredRange), errors);
        }

        private void ValidateTextCustomMessages()
        {
            var errors = new List<string>();
            if (IsNullOrWhiteSpace(TextCustomMessages))
                errors.Add("请填写此项（自定义）");
            else if (TextCustomMessages!.Length < 3)
                errors.Add("至少 3 个字符（自定义）");
            SetErrors(nameof(TextCustomMessages), errors);
        }

        private void ValidateIntAge()
        {
            var errors = new List<string>();
            if (IsNullOrWhiteSpace(IntAge))
                errors.Add(ValidatedTextBox.DefaultMessageProvider.GetRequiredMessage());
            else if (!int.TryParse(IntAge, out var v))
                errors.Add("请输入整数");
            else if (v < 0)
                errors.Add(ValidatedTextBox.DefaultMessageProvider.GetMinimumValueMessage(0));
            else if (v > 120)
                errors.Add(ValidatedTextBox.DefaultMessageProvider.GetMaximumValueMessage(120));
            SetErrors(nameof(IntAge), errors);
        }

        private void ValidateIntTemperature()
        {
            var errors = new List<string>();
            if (!IsNullOrWhiteSpace(IntTemperature))
            {
                if (!int.TryParse(IntTemperature, out var v))
                    errors.Add("请输入整数");
                else if (v < -50)
                    errors.Add(ValidatedTextBox.DefaultMessageProvider.GetMinimumValueMessage(-50));
                else if (v > 50)
                    errors.Add(ValidatedTextBox.DefaultMessageProvider.GetMaximumValueMessage(50));
            }
            SetErrors(nameof(IntTemperature), errors);
        }

        private void ValidateIntTriggerTextChanged()
        {
            var errors = new List<string>();
            if (!IsNullOrWhiteSpace(IntTriggerTextChanged))
            {
                if (!int.TryParse(IntTriggerTextChanged, out var v))
                    errors.Add("请输入整数");
                else if (v < 1)
                    errors.Add(ValidatedTextBox.DefaultMessageProvider.GetMinimumValueMessage(1));
                else if (v > 10)
                    errors.Add(ValidatedTextBox.DefaultMessageProvider.GetMaximumValueMessage(10));
            }
            SetErrors(nameof(IntTriggerTextChanged), errors);
        }

        private void ValidateIntTriggerLostFocus()
        {
            var errors = new List<string>();
            if (!IsNullOrWhiteSpace(IntTriggerLostFocus))
            {
                if (!int.TryParse(IntTriggerLostFocus, out var v))
                    errors.Add("请输入整数");
                else if (v < 1)
                    errors.Add(ValidatedTextBox.DefaultMessageProvider.GetMinimumValueMessage(1));
                else if (v > 10)
                    errors.Add(ValidatedTextBox.DefaultMessageProvider.GetMaximumValueMessage(10));
            }
            SetErrors(nameof(IntTriggerLostFocus), errors);
        }

        private static bool HasTooManyDecimals(string text, int maxPlaces)
        {
            var idx = text.IndexOf('.');
            if (idx < 0)
                return false;
            var decimals = text[(idx + 1)..];
            return decimals.Length > maxPlaces;
        }

        private void ValidateDecWeight()
        {
            var errors = new List<string>();
            if (!IsNullOrWhiteSpace(DecWeight))
            {
                if (!double.TryParse(DecWeight, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
                    errors.Add("请输入数值");
                else if (HasTooManyDecimals(DecWeight!, 2))
                    errors.Add("最多 2 位小数");
                else if (v < 0)
                    errors.Add(ValidatedTextBox.DefaultMessageProvider.GetMinimumValueMessage(0));
                else if (v > 100)
                    errors.Add(ValidatedTextBox.DefaultMessageProvider.GetMaximumValueMessage(100));
            }
            SetErrors(nameof(DecWeight), errors);
        }

        private void ValidateDecRate()
        {
            var errors = new List<string>();
            if (!IsNullOrWhiteSpace(DecRate))
            {
                if (!double.TryParse(DecRate, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
                    errors.Add("请输入数值");
                else if (HasTooManyDecimals(DecRate!, 3))
                    errors.Add("最多 3 位小数");
                else if (v < -10)
                    errors.Add(ValidatedTextBox.DefaultMessageProvider.GetMinimumValueMessage(-10));
                else if (v > 10)
                    errors.Add(ValidatedTextBox.DefaultMessageProvider.GetMaximumValueMessage(10));
            }
            SetErrors(nameof(DecRate), errors);
        }

        private string BuildReportText()
        {
            if (!_showErrors)
                return "";

            var all = _errors.Values.SelectMany(x => x).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray();
            return all.Length == 0 ? "无错误" : string.Join(Environment.NewLine, all);
        }

        private void UpdateReportTextIfShowing()
        {
            if (_showErrors)
                ReportText = BuildReportText();
        }
    }
}
