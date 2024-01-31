using System;
using TMPro;

public class WordHighlightTextEffect : BaseTextMeshProEffect
{
    protected override void OnPreRenderText(TMP_TextInfo textInfo) {
        if (textInfo.characterCount == 0) return;

        // var revealParams = TextRevealAnimatorCalculatedParams.Calculate(textInfo, animationProgress, characterEffectWidth);
        //
        // var currentCharacterDistance = 0f;
        // // Debug.Log(revealParams.totalWidth+" "+revealParams.normalizedEffectWidth+" "+revealParams.effectStart+" "+revealParams.effectEnd);
        // foreach (var characterInfo in textInfo.characterInfo) {
        //     if (!characterInfo.isVisible) continue;
        //     
        //     // Get the index of the material used by the current character.
        //     int materialIndex = characterInfo.materialReferenceIndex;
        //
        //     // Get the index of the first vertex used by this text element.
        //     int vertexIndex = characterInfo.vertexIndex;
        //
        //     var characterWidth = TMPUtils.GetCharacterWidth(characterInfo);
        //
        //     var strength = revealParams.GetStrengthMinusOne(currentCharacterDistance);
        //     var heightOffset = Mathf.Sin(strength * Mathf.PI);
        //     var vertices = textInfo.meshInfo[materialIndex].vertices;
        //     vertices[vertexIndex + 0] += Vector3.up * 10 * heightOffset;
        //     vertices[vertexIndex + 1] += Vector3.up * 10 * heightOffset;
        //     vertices[vertexIndex + 2] += Vector3.up * 10 * heightOffset;
        //     vertices[vertexIndex + 3] += Vector3.up * 10 * heightOffset;
        //
        //     currentCharacterDistance += characterWidth;
        // }
    }
}