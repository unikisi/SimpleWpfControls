using SimpleWpfControls;

namespace SimpleWpfControlsTests
{
    public sealed class CustomValidationMessageProvider : IValidationMessageProvider
    {
        public string GetRequiredMessage() => "CUSTOM: required";

        public string GetMinimumLengthMessage(int min) => $"CUSTOM: min length = {min}";

        public string GetMaximumLengthMessage(int max) => $"CUSTOM: max length = {max}";

        public string GetLengthRangeMessage(int min, int max) => $"CUSTOM: length {min}-{max}";

        public string GetMinimumValueMessage(double min) => $"CUSTOM: min value = {min}";

        public string GetMaximumValueMessage(double max) => $"CUSTOM: max value = {max}";
    }
}
