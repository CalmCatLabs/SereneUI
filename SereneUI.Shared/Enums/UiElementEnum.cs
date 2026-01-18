// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Serene.Common.Helpers;
// using SereneUI.Shared.DataStructures;
//
// namespace SereneUI.Shared.Enums;
//
// public delegate void Validate(UiNode node);
//
// public class __UiElementEnum : StrongTypedEnumerable<UiElementEnum>
// {
//     public Validate IsValid { get; init; } = EmptyValidate;
//
//     private UiElementEnum(string name) : base(name)
//     {
//     }
//
//     public static List<UiElementEnum?>? GetList()
//     {
//         return typeof(UiElementEnum)
//             .GetFields()
//             .Where(p => p.FieldType == typeof(UiElementEnum))?
//             .Select(p => p.GetValue(null) as UiElementEnum)
//             .ToList();
//     }
//     
//     public static UiElementEnum? GetByName(string name)
//     {
//         var uiEnum = typeof(UiElementEnum)
//             .GetFields()
//             .FirstOrDefault(p => p.Name == name && p.FieldType == typeof(UiElementEnum))?
//             .GetValue(null) as UiElementEnum;
//         return uiEnum;
//     }
//     
//     public static readonly UiElementEnum Page = new(nameof(Page));
//     public static readonly UiElementEnum Panel = new (nameof(Panel))
//     {
//         IsValid = element =>
//         {
//             if (element.Children.Count > 1)
//             {
//                 throw new InvalidOperationException($"{element.Type.Name} can only have one child.");
//             }
//         }
//     };
//
//     public static readonly UiElementEnum TextBlock = new(nameof(TextBlock));
//     public static readonly UiElementEnum Button = new(nameof(Button));
//     public static readonly UiElementEnum StackPanel = new(nameof(StackPanel));
//
//     
//     private static void EmptyValidate(UiNode node) { }
// }