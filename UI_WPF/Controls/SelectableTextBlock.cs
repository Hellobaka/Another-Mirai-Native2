using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Another_Mirai_Native.UI.Controls
{
    // Source - https://stackoverflow.com/a/45627524
    // Posted by torvin, modified by community. See post 'Timeline' for change history
    // Retrieved: 2025-12-03, License - CC BY-SA3.0

    internal sealed class TextEditorWrapper
    {
        // Lazily resolve types to avoid type load failures on unsupported frameworks
        private static readonly Lazy<Type?> TextEditorTypeLazy = new(() =>
            Type.GetType("System.Windows.Documents.TextEditor, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", throwOnError: false));
        private static readonly Lazy<Type?> TextContainerTypeLazy = new(() =>
            Type.GetType("System.Windows.Documents.ITextContainer, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", throwOnError: false));

        private static Type? TextEditorType => TextEditorTypeLazy.Value;
        private static Type? TextContainerType => TextContainerTypeLazy.Value;

        private static PropertyInfo? IsReadOnlyProp => TextEditorType?.GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
        private static PropertyInfo? TextViewProp => TextEditorType?.GetProperty("TextView", BindingFlags.Instance | BindingFlags.NonPublic);
        private static MethodInfo? RegisterMethod => TextEditorType?.GetMethod(
            "RegisterCommandHandlers",
            BindingFlags.Static | BindingFlags.NonPublic,
            binder: null,
            types: new[] { typeof(Type), typeof(bool), typeof(bool), typeof(bool) },
            modifiers: null);

        private static PropertyInfo? TextContainerTextViewProp => TextContainerType?.GetProperty("TextView", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        private static PropertyInfo? TextContainerProp => typeof(TextBlock).GetProperty("TextContainer", BindingFlags.Instance | BindingFlags.NonPublic);

        // Indicates if required internal WPF types are available
        private static bool IsSupported => TextEditorType is not null && TextContainerType is not null;

        public static void RegisterCommandHandlers(Type controlType, bool acceptsRichContent, bool readOnly, bool registerEventListeners)
        {
            if (!IsSupported || RegisterMethod is null)
            {
                // On unsupported platforms/frameworks, do nothing safely
                return;
            }

            try
            {
                RegisterMethod.Invoke(null, new object[] { controlType, acceptsRichContent, readOnly, registerEventListeners });
            }
            catch
            {
                // Reflection invoke might fail due to internal API changes; swallow to remain safe
                // Optionally log if a logging system exists
            }
        }

        public static TextEditorWrapper? CreateFor(TextBlock tb)
        {
            if (tb is null) throw new ArgumentNullException(nameof(tb));

            if (!IsSupported || TextContainerProp is null || IsReadOnlyProp is null || TextViewProp is null || TextContainerTextViewProp is null)
            {
                // Unsupported environment; return null to indicate no editor available
                return null;
            }

            try
            {
                var textContainer = TextContainerProp.GetValue(tb);
                if (textContainer is null)
                {
                    return null;
                }

                var editor = new TextEditorWrapper(textContainer, tb, isUndoEnabled: false);

                // Ensure backing _editor exists before setting properties
                if (editor._editor is null)
                {
                    return null;
                }

                // Set read-only and TextView safely
                IsReadOnlyProp.SetValue(editor._editor, true);
                var textView = TextContainerTextViewProp.GetValue(textContainer);
                TextViewProp.SetValue(editor._editor, textView);

                return editor;
            }
            catch
            {
                // Any reflection failure should not crash the app; just degrade gracefully
                return null;
            }
        }

        private readonly object? _editor;

        private TextEditorWrapper(object textContainer, FrameworkElement uiScope, bool isUndoEnabled)
        {
            // Guard arguments
            if (uiScope is null) throw new ArgumentNullException(nameof(uiScope));
            if (!IsSupported || TextEditorType is null)
            {
                _editor = null;
                return;
            }

            try
            {
                _editor = Activator.CreateInstance(
                    TextEditorType,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                    binder: null,
                    args: new[] { textContainer, uiScope, isUndoEnabled },
                    culture: null);
            }
            catch
            {
                _editor = null;
            }
        }
    }

    public class SelectableTextBlock : Emoji.Wpf.TextBlock
    {
        static SelectableTextBlock()
        {
            FocusableProperty.OverrideMetadata(typeof(SelectableTextBlock), new FrameworkPropertyMetadata(true));
            TextEditorWrapper.RegisterCommandHandlers(typeof(SelectableTextBlock), true, true, true);

            // remove the focus rectangle around the control
            FocusVisualStyleProperty.OverrideMetadata(typeof(SelectableTextBlock), new FrameworkPropertyMetadata((object)null));
        }

        private readonly TextEditorWrapper? _editor;

        public SelectableTextBlock()
        {
            _editor = TextEditorWrapper.CreateFor(this);
        }
    }
}