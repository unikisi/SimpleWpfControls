using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleWpfControls
{
    public static class ValidationScope
    {
        public static readonly DependencyProperty TrackErrorsProperty =
            DependencyProperty.RegisterAttached(
                "TrackErrors",
                typeof(bool),
                typeof(ValidationScope),
                new PropertyMetadata(false, OnTrackErrorsChanged));

        private static readonly DependencyPropertyKey ErrorCountPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "ErrorCount",
                typeof(int),
                typeof(ValidationScope),
                new PropertyMetadata(0, OnErrorCountChanged));

        public static readonly DependencyProperty ErrorCountProperty = ErrorCountPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey HasErrorsPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "HasErrors",
                typeof(bool),
                typeof(ValidationScope),
                new PropertyMetadata(false, OnHasErrorsChanged));

        public static readonly DependencyProperty HasErrorsProperty = HasErrorsPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey IsValidPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "IsValid",
                typeof(bool),
                typeof(ValidationScope),
                new PropertyMetadata(true));

        public static readonly DependencyProperty IsValidProperty = IsValidPropertyKey.DependencyProperty;

        public static readonly DependencyProperty ReportTokenProperty =
            DependencyProperty.RegisterAttached(
                "ReportToken",
                typeof(int),
                typeof(ValidationScope),
                new PropertyMetadata(0, OnReportTokenChanged));

        public static readonly DependencyProperty ValidateTokenProperty =
            DependencyProperty.RegisterAttached(
                "ValidateToken",
                typeof(int),
                typeof(ValidationScope),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.Inherits, OnValidateTokenChanged));

        private static readonly DependencyPropertyKey ReportTextPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "ReportText",
                typeof(string),
                typeof(ValidationScope),
                new PropertyMetadata(""));

        public static readonly DependencyProperty ReportTextProperty = ReportTextPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey ErrorMessagesPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "ErrorMessages",
                typeof(IReadOnlyList<string>),
                typeof(ValidationScope),
                new PropertyMetadata(Array.Empty<string>()));

        public static readonly DependencyProperty ErrorMessagesProperty = ErrorMessagesPropertyKey.DependencyProperty;

        public static void SetTrackErrors(DependencyObject element, bool value) => element.SetValue(TrackErrorsProperty, value);
        public static bool GetTrackErrors(DependencyObject element) => (bool)element.GetValue(TrackErrorsProperty);

        public static int GetErrorCount(DependencyObject element) => (int)element.GetValue(ErrorCountProperty);
        public static bool GetHasErrors(DependencyObject element) => (bool)element.GetValue(HasErrorsProperty);
        public static bool GetIsValid(DependencyObject element) => (bool)element.GetValue(IsValidProperty);
        public static void SetReportToken(DependencyObject element, int value) => element.SetValue(ReportTokenProperty, value);
        public static int GetReportToken(DependencyObject element) => (int)element.GetValue(ReportTokenProperty);
        public static void SetValidateToken(DependencyObject element, int value) => element.SetValue(ValidateTokenProperty, value);
        public static int GetValidateToken(DependencyObject element) => (int)element.GetValue(ValidateTokenProperty);
        public static string GetReportText(DependencyObject element) => (string)element.GetValue(ReportTextProperty);
        public static IReadOnlyList<string> GetErrorMessages(DependencyObject element) => (IReadOnlyList<string>)element.GetValue(ErrorMessagesProperty);

        private static void OnTrackErrorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not UIElement element)
                return;

            if ((bool)e.NewValue)
            {
                element.SetValue(ErrorCountPropertyKey, 0);
                element.SetValue(HasErrorsPropertyKey, false);
                element.SetValue(IsValidPropertyKey, true);
                element.SetValue(ReportTextPropertyKey, "");
                element.SetValue(ErrorMessagesPropertyKey, Array.Empty<string>());
                Validation.AddErrorHandler(element, OnValidationError);
            }
            else
            {
                Validation.RemoveErrorHandler(element, OnValidationError);
                element.ClearValue(ErrorCountPropertyKey);
                element.ClearValue(HasErrorsPropertyKey);
                element.ClearValue(IsValidPropertyKey);
                element.ClearValue(ReportTextPropertyKey);
                element.ClearValue(ErrorMessagesPropertyKey);
            }
        }

        private static void OnReportTokenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!GetTrackErrors(d))
                return;

            RefreshReport(d);
        }

        private static void OnValidateTokenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ValidatedTextBox tb)
                tb.Validate(false);
        }

        private static void OnValidationError(object? sender, ValidationErrorEventArgs e)
        {
            if (sender is not DependencyObject scope)
                return;

            var count = (int)scope.GetValue(ErrorCountProperty);
            count += e.Action == ValidationErrorEventAction.Added ? 1 : -1;
            if (count < 0)
                count = 0;
            scope.SetValue(ErrorCountPropertyKey, count);
        }

        private static void OnErrorCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var count = (int)e.NewValue;
            d.SetValue(HasErrorsPropertyKey, count > 0);
        }

        private static void OnHasErrorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var hasErrors = (bool)e.NewValue;
            d.SetValue(IsValidPropertyKey, !hasErrors);
        }

        private static void RefreshReport(DependencyObject scope)
        {
            var messages = new List<string>();
            CollectErrors(scope, messages);
            var unique = messages.Where(m => !string.IsNullOrWhiteSpace(m)).Distinct().ToArray();

            scope.SetValue(ErrorMessagesPropertyKey, unique);
            scope.SetValue(ReportTextPropertyKey, unique.Length == 0 ? "无错误" : string.Join(Environment.NewLine, unique));
        }

        private static void CollectErrors(DependencyObject node, List<string> messages)
        {
            var errors = Validation.GetErrors(node);
            for (int i = 0; i < errors.Count; i++)
            {
                var content = errors[i].ErrorContent;
                messages.Add(content?.ToString() ?? "");
            }

            var childrenCount = VisualTreeHelper.GetChildrenCount(node);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(node, i);
                CollectErrors(child, messages);
            }
        }
    }
}
