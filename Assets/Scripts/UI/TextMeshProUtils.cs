using UnityEngine;
using TMPro;
using System.Collections.Generic;

public static class TextMeshProUtils {
    public static int GetClippedCharacterIndex(TextMeshProUGUI textMeshPro) {
        string originalText = textMeshPro.text;
        string generatedText = textMeshPro.GetParsedText();

        // Find the point where the generated text differs from the original text
        int clippedIndex = FindClippedIndex(originalText, generatedText);
        //
        // if (clippedIndex >= 0)
        // {
        //     Debug.Log($"Text is clipped at index: {clippedIndex} ({originalText.Substring(clippedIndex)})");
        // }
        // else
        // {
        //     Debug.Log("Text is not clipped.");
        // }
        return clippedIndex;
        
        static int FindClippedIndex(string originalText, string generatedText)
        {
            int maxLength = Mathf.Min(originalText.Length, generatedText.Length);

            for (int i = 0; i < maxLength; i++)
            {
                if (originalText[i] != generatedText[i])
                {
                    return i;
                }
            }

            // Text is not clipped
            return -1;
        }
    }
    
    public static float LineSpacingToRectTransformHeight (this TMP_Text textComponent, float emLineHeight) {
        float currentEmScale = textComponent.fontSize * 0.01f * (textComponent.isOrthographic ? 1 : 0.1f);
        return currentEmScale * emLineHeight;
    }

    public static float LineHeightToRectTransformHeight (this TMP_Text textComponent) {
        var font = textComponent.font;
        return textComponent.fontSize * (font.faceInfo.lineHeight / font.faceInfo.pointSize);
    }

    // Untested!
    public static float CharacterSpacingToRectTransformWidth (this TMP_Text textComponent, float emCharacterSpacing) {
        var font = textComponent.font;
        var lineHeightMultiplier = font.faceInfo.lineHeight / font.faceInfo.pointSize;
        return lineHeightMultiplier * emCharacterSpacing;
    }
    
    public static string ReplaceUnsupportedQuoteMarks (string textString, TMP_FontAsset font) {
        if(!font.glyphLookupTable.ContainsKey('”') || !font.glyphLookupTable.ContainsKey('“')) {
            textString = textString.Replace('”', '"');
            textString = textString.Replace('“', '"');
        }
        if(!font.glyphLookupTable.ContainsKey('’')) {
            textString = textString.Replace('’', '\'');
        }
        return textString;
    }
    
    public static float GetTotalWidth (TMP_TextInfo textInfo, bool visibleOnly) {
        var totalWidth = 0f;
        for (int i = 0; i < textInfo.characterInfo.Length; i++) {
            TMP_CharacterInfo characterInfo = textInfo.characterInfo[i];
            if (visibleOnly && !characterInfo.isVisible) continue;
            var characterWidth = GetCharacterWidth(characterInfo);
            totalWidth += characterWidth;    
        }
        return totalWidth;
    }

    public static float GetCharacterWidth(TMP_CharacterInfo characterInfo) {
        return characterInfo.topRight.x - characterInfo.topLeft.x;
    }
    public static int GetNumCharacters(TMP_TextInfo textInfo, bool visibleOnly) {
        if(!visibleOnly) return textInfo.characterInfo.Length;
        int numCharacters = 0;
        for (int i = 0; i < textInfo.characterInfo.Length; i++) 
            if(textInfo.characterInfo[i].isVisible) 
                numCharacters++;
        return numCharacters;
    }

	public static bool IsAnyWordSplit (TMP_TextInfo textInfo) {
		for(int i = 0; i < textInfo.characterInfo.Length-1; i++) {
			if(textInfo.characterInfo[i].lineNumber == textInfo.characterInfo[i+1].lineNumber) continue;
			var lastCharInLine = textInfo.characterInfo[i].character;
			if(!char.IsSeparator(lastCharInLine) && !char.IsPunctuation(lastCharInLine)) {
				// Debug.LogWarning(textInfo.characterInfo[i].character +" "+ textInfo.characterInfo[i+1].character+" "+textInfo.characterInfo[i].lineNumber+" "+textInfo.characterInfo[i+1].lineNumber);
				return true;
			}
		}
		return false;
		// foreach(var line in text.textInfo.lineInfo) {
		// 	if(line.characterCount == 0) continue;
		// 	Debug.Log(line.lastCharacterIndex+" "+text.textInfo.characterInfo[line.lastCharacterIndex].character);
		// }
		// foreach(var word in text.textInfo.wordInfo) {
		// 	// if(word.characterCount == 0) continue;
		// 	Debug.Log(word.firstCharacterIndex+", "+word.lastCharacterIndex+": "+text.text.Substring(word.firstCharacterIndex, word.lastCharacterIndex-word.firstCharacterIndex+1));
		// 	// Debug.Log(word.characterCount);
		// 	if(text.textInfo.characterInfo[word.firstCharacterIndex].lineNumber != text.textInfo.characterInfo[word.lastCharacterIndex].lineNumber) {
		// 		Debug.LogWarning(text.text.Substring(word.firstCharacterIndex, word.lastCharacterIndex-word.firstCharacterIndex+1));
		// 	}
		// }	
	}
    
    // Applying float.MaxValue to a rectTransform can cause crashes (not sure why) so we just use a very big number instead.
    const float veryLargeNumber = 10000000;
    // I long for a day when this is no longer necessary.
    static Vector2 BetterGetRenderedValues(TMP_Text textComponent, float maxWidth = veryLargeNumber, float maxHeight = veryLargeNumber, bool onlyVisibleCharacters = true) {
        // If width/height is Infinity/<0 renderedSize can be NaN. In that case, use preferredValues
        var renderedSize = textComponent.GetRenderedValues(onlyVisibleCharacters);
        if(IsInvalidFloat(renderedSize.x) || IsInvalidFloat(renderedSize.y)) {
            var preferredSize = textComponent.GetPreferredValues(textComponent.text, maxWidth, maxHeight);
            // I've seen this come out as -4294967000.00 when the string has only a zero-width space (\u200B) with onlyVisibleCharacters true. In any case it makes no sense for the size to be < 0.
            preferredSize = new Vector2(Mathf.Max(preferredSize.x, 0), Mathf.Max(preferredSize.y, 0));
            if(IsInvalidFloat(renderedSize.x)) renderedSize.x = preferredSize.x;
            if(IsInvalidFloat(renderedSize.y)) renderedSize.y = preferredSize.y;
        }
        
        // 1.7E+38f is half 3.40282347E+38f, which seems to be a possible return value for GetRenderedValues when maxHeight = veryLargeNumber (I guess because it's half)
        bool IsInvalidFloat (float f) {return float.IsNaN(f) || f == Mathf.Infinity || f >= 1.7E+38f  || f < 0;}
        return renderedSize;
    }
    
    // Applies the tightest bounds for the current text using GetRenderedValues
    // Note this uses sizeDelta for sizing so won't work when using anchors.
    // This is wayyyy more reliable than the actual GetRenderedValues because it won't return stupid values, as GetRenderedValues is prone to doing. 
    public static void RenderAndApplyTightSize (this TMP_Text textComponent, float maxWidth = veryLargeNumber, float maxHeight = veryLargeNumber, bool onlyVisibleCharacters = true) {
        var originalRenderMode = textComponent.renderMode;
        
        textComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
        textComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxHeight);
        textComponent.ForceMeshUpdate(true);

        var renderedSize = Vector2.zero;
        if(!string.IsNullOrEmpty(textComponent.text)) renderedSize = BetterGetRenderedValues(textComponent, maxWidth, maxHeight, onlyVisibleCharacters);
        
        textComponent.renderMode = originalRenderMode;
        textComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, renderedSize.x);
        textComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, renderedSize.y);
        textComponent.ForceMeshUpdate(true);
    }
    
    // Gets the tightest bounds for the current text using GetRenderedValues
    // Note this uses sizeDelta for sizing so won't work when using anchors.
    // This is wayyyy more reliable than the actual GetRenderedValues because it won't return stupid values, as GetRenderedValues is prone to doing. 
    public static Vector2 RenderAndGetTightSize (this TMP_Text textComponent, float maxWidth = veryLargeNumber, float maxHeight = veryLargeNumber, bool onlyVisibleCharacters = true) {
        var originalRenderMode = textComponent.renderMode;
        var originalSize = textComponent.rectTransform.rect.size;
        
        textComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
        textComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxHeight);
        textComponent.ForceMeshUpdate(true);
        
        if(string.IsNullOrEmpty(textComponent.text)) return Vector2.zero;
        // This doesn't work if the component is disabled - but it's better! I'm not even sure this function works while disabled...
        // if(textComponent.textInfo.characterCount == 0) return Vector2.zero;
        var renderedSize = BetterGetRenderedValues(textComponent, maxWidth, maxHeight, onlyVisibleCharacters);
        
        textComponent.renderMode = originalRenderMode;
        textComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize.x);
        textComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originalSize.y);
        textComponent.ForceMeshUpdate(true);
		
        return renderedSize;
    }

    // Gets the tightest bounds for the text by updating the text and using GetRenderedValues
    // Note this uses sizeDelta for sizing so won't work when using anchors.
    // This is wayyyy more reliable than the actual GetRenderedValues because it won't return stupid values, as GetRenderedValues is prone to doing.
    public static Vector2 GetRenderedValues (this TMP_Text textComponent, string text, float maxWidth = veryLargeNumber, float maxHeight = veryLargeNumber, bool onlyVisibleCharacters = true) {
        if(string.IsNullOrEmpty(text)) return Vector2.zero;
        // Setting RT size to Infinity can lead to weird results, so we use a very large number instead. 
        if(maxWidth > veryLargeNumber) maxWidth = veryLargeNumber;
        if(maxHeight > veryLargeNumber) maxHeight = veryLargeNumber;

        var originalRenderMode = textComponent.renderMode;
        var originalText = textComponent.text;
        var originalSize = textComponent.rectTransform.rect.size;
        
        textComponent.renderMode = TextRenderFlags.DontRender;
        textComponent.text = text;
        textComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
        textComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxHeight);
        textComponent.ForceMeshUpdate(true);

        // This doesn't work if the component is disabled - but it's better! I'm not even sure this function works while disabled...
        // if(textComponent.textInfo.characterCount == 0) return Vector2.zero;

        var renderedSize = BetterGetRenderedValues(textComponent, maxWidth, maxHeight, onlyVisibleCharacters);
        
        textComponent.renderMode = originalRenderMode;
        textComponent.text = originalText;
        textComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize.x);
        textComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originalSize.y);
        textComponent.ForceMeshUpdate(true);    
		
        return renderedSize;
    }
    
    
    // Applies tight preferred values for the current text using GetTightPreferredValues
    // Note this uses sizeDelta for sizing so won't work when using anchors.
    public static void ApplyTightPreferredSize (this TMP_Text textComponent, float maxWidth = veryLargeNumber, float maxHeight = veryLargeNumber) {
        var preferredSize = textComponent.GetTightPreferredValues(maxWidth, maxHeight);
        textComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preferredSize.x);
        textComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredSize.y);
    }
    
    // Applies tight preferred values for the current text using GetTightPreferredValues
    // Note this uses sizeDelta for sizing so won't work when using anchors.
    public static void ApplyTightPreferredWidth (this TMP_Text textComponent, float maxWidth = veryLargeNumber, float maxHeight = veryLargeNumber) {
        var preferredSize = textComponent.GetTightPreferredValues(maxWidth, maxHeight);
        textComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preferredSize.x);
    }
    
    // Applies tight preferred values for the current text using GetTightPreferredValues
    // Note this uses sizeDelta for sizing so won't work when using anchors.
    public static void ApplyTightPreferredHeight (this TMP_Text textComponent, float maxWidth = veryLargeNumber, float maxHeight = veryLargeNumber) {
        var preferredSize = textComponent.GetTightPreferredValues(maxWidth, maxHeight);
        textComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredSize.y);
    }
    
    // Width for GetPreferredValues always returns the length of the text as if it was on one line.
    // This function additionally clamps the returned width of GetPreferredValues to the input width.
    // It does this for both width and height, but probably doesn't need to (?) because this issue is specific to width.
    public static Vector2 GetTightPreferredValues (this TMP_Text textComponent, float maxWidth = veryLargeNumber, float maxHeight = veryLargeNumber) {
        var preferredSize = textComponent.GetPreferredValues(maxWidth, maxHeight);
        preferredSize.x = Mathf.Min(preferredSize.x, maxWidth);
        preferredSize.y = Mathf.Min(preferredSize.y, maxHeight);
        return preferredSize;
    }
    
    // Width for GetPreferredValues always returns the length of the text as if it was on one line.
    // This function additionally clamps the returned width of GetPreferredValues to the input width.
    // It does this for both width and height, but probably doesn't need to (?) because this issue is specific to width.
    public static Vector2 GetTightPreferredValues (this TMP_Text textComponent, string text, float maxWidth = veryLargeNumber, float maxHeight = veryLargeNumber) {
        var preferredSize = textComponent.GetPreferredValues(text, maxWidth, maxHeight);
        preferredSize.x = Mathf.Min(preferredSize.x, maxWidth);
        preferredSize.y = Mathf.Min(preferredSize.y, maxHeight);
        return preferredSize;
    }
    
    
    
    // I actually can't recall why this exists, given that we have the above functions. Perhaps it's an earlier iteration?
    public static Vector2 GetBestFitWidth (this TMP_Text textComponent, float targetHeight, float widthStep) {
        textComponent.renderMode = TextRenderFlags.DontRender;
        var originalSize = textComponent.rectTransform.rect.size;
        
        Debug.Assert(widthStep > 1, "Width step must be larger than 1 or this will take forever/ages to execute!");
        
        float width = 0;
        textComponent.rectTransform.sizeDelta = new Vector2(width, targetHeight);
        textComponent.ForceMeshUpdate(true);

        int numIterations = 0;
        while(textComponent.isTextOverflowing) {
            width += widthStep;
			textComponent.rectTransform.sizeDelta = new Vector2(width, targetHeight);
			textComponent.ForceMeshUpdate(true);
            numIterations++;
            if(numIterations > 50) Debug.LogError("Max num iterations reached for GetBestFitWidth with targetHeight "+targetHeight+" and widthStep "+widthStep);
        }
        
        // The "tight" values for the rect - rendered width will be smaller than width used to calculate.
        var renderedSize = textComponent.GetRenderedValues(true);
		
        // Reset to how we started
        textComponent.renderMode = TextRenderFlags.Render;
        textComponent.rectTransform.sizeDelta = originalSize;
		
        return new Vector2(renderedSize.x, targetHeight);
    }

    
    
    
    
    
    static Vector2[] fourCornersArray = new Vector2[4];
    public static Rect GetScreenRectOfTextBounds(TMP_Text tmpText) {
        // Set local corners
        {
            var bounds = tmpText.textBounds;
            float x = bounds.min.x;
            float y = bounds.min.y;
            float xMax = bounds.max.x;
            float yMax = bounds.max.y;
            fourCornersArray[0] = new Vector3(x, y, 0.0f);
            fourCornersArray[1] = new Vector3(x, yMax, 0.0f);
            fourCornersArray[2] = new Vector3(xMax, yMax, 0.0f);
            fourCornersArray[3] = new Vector3(xMax, y, 0.0f);
        }

        // Set world corners
        {
            Matrix4x4 localToWorldMatrix = tmpText.transform.localToWorldMatrix;
            for (int index = 0; index < 4; ++index) fourCornersArray[index] = localToWorldMatrix.MultiplyPoint(fourCornersArray[index]);
        }
		    
        // Set screen corners
        {
            var canvas = tmpText.canvas;
            Camera cam = canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace ? canvas.worldCamera : null;
            for (int i = 0; i < 4; i++) fourCornersArray[i] = RectTransformUtility.WorldToScreenPoint(cam, fourCornersArray[i]);
        }

        // Create rect that encapsulates screen corners
        {
            float xMin = fourCornersArray[0].x;
            float xMax = fourCornersArray[0].x;
            float yMin = fourCornersArray[0].y;
            float yMax = fourCornersArray[0].y;
            for(int i = 1; i < fourCornersArray.Length; i++) {
                var vector = fourCornersArray[i];
                xMin = Mathf.Min (xMin, vector.x);
                xMax = Mathf.Max (xMax, vector.x);
                yMin = Mathf.Min (yMin, vector.y);
                yMax = Mathf.Max (yMax, vector.y);
            }
            return Rect.MinMaxRect (xMin, yMin, xMax, yMax);
        }
    }
    
    // Given a range of characters (inclusive) returns the screen rects that they occupy. Several rects are returned if the characters span multiple lines.
    public static IEnumerable<Rect> GetScreenRectsForCharacterRange(TMP_Text textComponent, int startCharacterIndex, int endCharacterIndex) {
        if (textComponent == null) yield break;
        var textInfo = textComponent.textInfo;
        
        int currentLineIndex = -1;
        Rect firstCharacterInLineScreenRect = Rect.zero;
        TMP_CharacterInfo lastCharacterInfo = default(TMP_CharacterInfo);

        startCharacterIndex = Mathf.Clamp(startCharacterIndex, 0, textInfo.characterCount);
        endCharacterIndex = Mathf.Clamp(endCharacterIndex, 0, textInfo.characterCount);
        
        for (int c = startCharacterIndex; c <= endCharacterIndex; c++) {
            var characterInfo = textInfo.characterInfo[c];
            if (currentLineIndex == -1) {
                currentLineIndex = characterInfo.lineNumber;
                firstCharacterInLineScreenRect = TextMeshProUtils.GetScreenRectForCharacter(textComponent, characterInfo);
            }
            if (characterInfo.lineNumber != currentLineIndex) {
                var currentCharacterScreenRect = TextMeshProUtils.GetScreenRectForCharacter(textComponent, lastCharacterInfo);
                yield return RectX.CreateEncapsulating(firstCharacterInLineScreenRect, currentCharacterScreenRect);
                    
                currentLineIndex = characterInfo.lineNumber;
                firstCharacterInLineScreenRect = TextMeshProUtils.GetScreenRectForCharacter(textComponent, characterInfo);
            }

            lastCharacterInfo = characterInfo;
        }

        {
            var currentCharacterScreenRect = TextMeshProUtils.GetScreenRectForCharacter(textComponent, lastCharacterInfo);
            yield return RectX.CreateEncapsulating(firstCharacterInLineScreenRect, currentCharacterScreenRect);
        }
    }
    
    // Given a character index returns the screen rect that it occupies.
    public static Rect GetScreenRectForCharacter(TMP_Text textComponent, TMP_CharacterInfo cInfo) {
        var m_Transform = textComponent.transform;
        var topLeft = m_Transform.TransformPoint(new Vector3(cInfo.topLeft.x, cInfo.ascender, 0));
        var bottomLeft = m_Transform.TransformPoint(new Vector3(cInfo.bottomLeft.x, cInfo.descender, 0));
        var bottomRight = m_Transform.TransformPoint(new Vector3(cInfo.topRight.x, cInfo.descender, 0));
        var topRight = m_Transform.TransformPoint(new Vector3(cInfo.topRight.x, cInfo.ascender, 0));
        return WorldToScreenRect(textComponent, bottomLeft, topLeft, topRight, bottomRight);
    }
    
    // Given a word index returns the screen rect that it occupies.
    public static Rect GetScreenRectForWord(TMP_WordInfo wInfo) {
        var m_TextComponent = wInfo.textComponent;
        if(m_TextComponent == null) return Rect.zero;
        var m_Transform = m_TextComponent.transform;
        var m_TextInfo = m_TextComponent.textInfo;

        bool isBeginRegion = false;

        Vector3 bottomLeft = Vector3.zero;
        Vector3 topLeft = Vector3.zero;
        Vector3 bottomRight = Vector3.zero;
        Vector3 topRight = Vector3.zero;

        float maxAscender = -Mathf.Infinity;
        float minDescender = Mathf.Infinity;

        // Iterate through each character of the word
        for (int j = 0; j < wInfo.characterCount; j++)
        {
            int characterIndex = wInfo.firstCharacterIndex + j;
            TMP_CharacterInfo currentCharInfo = m_TextInfo.characterInfo[characterIndex];
            int currentLine = currentCharInfo.lineNumber;

            bool isCharacterVisible = characterIndex > m_TextComponent.maxVisibleCharacters ||
                                      currentCharInfo.lineNumber > m_TextComponent.maxVisibleLines ||
                                     (m_TextComponent.overflowMode == TextOverflowModes.Page && currentCharInfo.pageNumber + 1 != m_TextComponent.pageToDisplay) ? false : true;

            // Track Max Ascender and Min Descender
            maxAscender = Mathf.Max(maxAscender, currentCharInfo.ascender);
            minDescender = Mathf.Min(minDescender, currentCharInfo.descender);

            if (isBeginRegion == false && isCharacterVisible)
            {
                isBeginRegion = true;

                bottomLeft = new Vector3(currentCharInfo.bottomLeft.x, currentCharInfo.descender, 0);
                topLeft = new Vector3(currentCharInfo.bottomLeft.x, currentCharInfo.ascender, 0);

                //Debug.Log("Start Word Region at [" + currentCharInfo.character + "]");

                // If Word is one character
                if (wInfo.characterCount == 1)
                {
                    isBeginRegion = false;

                    topLeft = m_Transform.TransformPoint(new Vector3(topLeft.x, maxAscender, 0));
                    bottomLeft = m_Transform.TransformPoint(new Vector3(bottomLeft.x, minDescender, 0));
                    bottomRight = m_Transform.TransformPoint(new Vector3(currentCharInfo.topRight.x, minDescender, 0));
                    topRight = m_Transform.TransformPoint(new Vector3(currentCharInfo.topRight.x, maxAscender, 0));

                    break;
                }
            }

            // Last Character of Word
            if (isBeginRegion && j == wInfo.characterCount - 1)
            {
                isBeginRegion = false;

                topLeft = m_Transform.TransformPoint(new Vector3(topLeft.x, maxAscender, 0));
                bottomLeft = m_Transform.TransformPoint(new Vector3(bottomLeft.x, minDescender, 0));
                bottomRight = m_Transform.TransformPoint(new Vector3(currentCharInfo.topRight.x, minDescender, 0));
                topRight = m_Transform.TransformPoint(new Vector3(currentCharInfo.topRight.x, maxAscender, 0));

                break;
            }
            // If Word is split on more than one line.
            else if (isBeginRegion && currentLine != m_TextInfo.characterInfo[characterIndex + 1].lineNumber)
            {
                isBeginRegion = false;

                topLeft = m_Transform.TransformPoint(new Vector3(topLeft.x, maxAscender, 0));
                bottomLeft = m_Transform.TransformPoint(new Vector3(bottomLeft.x, minDescender, 0));
                bottomRight = m_Transform.TransformPoint(new Vector3(currentCharInfo.topRight.x, minDescender, 0));
                topRight = m_Transform.TransformPoint(new Vector3(currentCharInfo.topRight.x, maxAscender, 0));

                break;

            }
        }
        return WorldToScreenRect(m_TextComponent, bottomLeft, topLeft, topRight, bottomRight);
    }
    
    // Utility function for other parts of this class.
    static Rect WorldToScreenRect(TMP_Text textComponent, Vector3 topLeft, Vector3 bottomLeft, Vector3 bottomRight, Vector3 topRight) {
        return RectX.CreateEncapsulating(RectTransformUtility.WorldToScreenPoint(textComponent.canvas.rootCanvas.worldCamera, topLeft), RectTransformUtility.WorldToScreenPoint(textComponent.canvas.rootCanvas.worldCamera, bottomRight));
    }
}