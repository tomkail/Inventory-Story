using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Various utils for styling UI in the BetterUp style
public static class Styling {
    public const float StandardPadding = 16;
    public const float StandardPanelSpacing = 32;
    
    public const float FastAnimationTime = 0.1f;
    public const float QuickAnimationTime = 0.15f;
    public const float StandardAnimationTime = 0.2f;
    public const float MediumAnimationTime = 0.3f;
    public const float ModerateAnimationTime = 0.35f;
    public const float LongAnimationTime = 0.4f;
    public const float VeryLongAnimationTime = 0.5f;
    
    public static readonly Color OffWhite = ColorX.HexToColor("F9F8F8");
    public static readonly Color Rubine = ColorX.HexToColor("CE0058");
    public static readonly Color Midnight = ColorX.HexToColor("1D1925");
    public static readonly Color Dawn = ColorX.HexToColor("2E2836");
    public static readonly Color GreenAA = ColorX.HexToColor("007D4D");
    public static readonly Color Warning = ColorX.HexToColor("FFCE19");
    public static readonly Color Error = ColorX.HexToColor("E52600");
    public static readonly Color Red = ColorX.HexToColor("E52600");

    public static readonly Color BGSwitchOff = ColorX.HexToColor("767676");
    public static readonly Color BGSwitchOn = Rubine;
    
    public static readonly Color BorderDefault = Midnight.WithAlpha(0.08f);
    public static readonly Color BorderInput = ColorX.HexToColor("CDCDCD");
    public static readonly Color IconDefault = Midnight;
    public static readonly Color TextHeading = Midnight;
    public static readonly Color TextBody = ColorX.HexToColor("4D4D4D");

    
    
    
    public enum TextStyle {
        HeadingXL,
        HeadingL,
        HeadingM,
        HeadingS,
        HeadingXS,
        BodyL,
        BodyM,
        BodyS,
        BodyXS,
        BodyXXS,
        BodyAllCaps,
        BodyL_Link,
        BodyL_Bold,
        BodyL_Italic,
        BodyM_Link,
        BodyM_Bold,
        BodyM_Italic,
        BodyS_Link,
        BodyS_Bold,
        BodyS_Italic,
        BodyXS_Link,
        BodyXS_Bold,
        BodyXS_Italic,
        BodyXXS_Link,
        BodyXXS_Bold,
        BodyXXS_Italic,
        Normal,
    }

    public static Dictionary<TextStyle, string> TextStyleNameDatabase = new Dictionary<TextStyle, string>() {
        {TextStyle.HeadingXL, "Heading XL"},
        {TextStyle.HeadingL, "Heading L"},
        {TextStyle.HeadingM, "Heading M"},
        {TextStyle.HeadingS, "Heading S"},
        {TextStyle.HeadingXS, "Heading XS"},
        {TextStyle.BodyL, "Body L"},
        {TextStyle.BodyM, "Body M"},
        {TextStyle.BodyS, "Body S"},
        {TextStyle.BodyXS, "Body XS"},
        {TextStyle.BodyXXS, "Body XXS"},
        {TextStyle.BodyAllCaps, "Body All Caps"},
        {TextStyle.BodyL_Link, "Body Special L (link)"},
        {TextStyle.BodyL_Bold, "Body Special L (bold)"},
        {TextStyle.BodyL_Italic, "Body Special L (italic)"},
        {TextStyle.BodyM_Link, "Body Special M (link)"},
        {TextStyle.BodyM_Bold, "Body Special M (bold)"},
        {TextStyle.BodyM_Italic, "Body Special M (italic)"},
        {TextStyle.BodyS_Link, "Body Special S (link)"},
        {TextStyle.BodyS_Bold, "Body Special S (bold)"},
        {TextStyle.BodyS_Italic, "Body Special S (italic)"},
        {TextStyle.BodyXS_Link, "Body Special XS (link)"},
        {TextStyle.BodyXS_Bold, "Body Special XS (bold)"},
        {TextStyle.BodyXS_Italic, "Body Special XS (italic)"},
        {TextStyle.BodyXXS_Link, "Body Special XXS (link)"},
        {TextStyle.BodyXXS_Bold, "Body Special XXS (bold)"},
        {TextStyle.BodyXXS_Italic, "Body Special XXS (italic)"},
        {TextStyle.Normal, "Normal"},
    };

    public static TMP_StyleSheet StyleSheet => TMP_Settings.GetStyleSheet();
    public static string GetStyleName(TextStyle textStyle) => TextStyleNameDatabase[textStyle];
    public static TMP_Style GetStyle(TextStyle textStyle) => StyleSheet.GetStyle(GetStyleName(textStyle));

    public static string WithTextStyle(string text, TextStyle style) => $"<style=\"{GetStyleName(style)}\">{text}</style>";
}