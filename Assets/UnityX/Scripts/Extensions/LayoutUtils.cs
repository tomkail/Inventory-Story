using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LayoutUtils {
    // Wraps layout item params and the layouts to apply the output to, if any.
    // Also contains arguments defining how the layout params should be applied to the layouts


    public static List<Vector2> GetLayoutRanges(FlexLayoutParams layoutParamsParams, List<LayoutItemParams> items) {
        float fixedTotal = items.Where(i => !i.flexible).Sum(i => i.fixedSize);
        float initialFlexSpace = items.Where(i => i.flexible).Sum(i => i.minSize);
        float totalFixedSpacing = layoutParamsParams.spacing * (items.Count - 1);
        float availableFlexibleSpace = layoutParamsParams.innerSize - fixedTotal - initialFlexSpace - totalFixedSpacing;

        // Map to hold final sizes for each flexible item
        var flexItemSizes = items.Where(i => i.flexible).ToDictionary(i => i, i => i.minSize);

        while (availableFlexibleSpace > 0) {
            float totalWeight = items.Where(i => i.flexible && flexItemSizes[i] < i.maxSize).Sum(i => i.weight);

            if (totalWeight == 0) break;

            float spaceAllocatedThisIteration = 0;

            foreach (var item in items.Where(i => i.flexible)) {
                if (flexItemSizes[item] >= item.maxSize)
                    continue; 

                float weightFraction = item.weight / totalWeight;
                float spaceForThisItem = weightFraction * availableFlexibleSpace;
                float spaceActuallyUsed = Math.Min(spaceForThisItem, item.maxSize - flexItemSizes[item]);
                
                flexItemSizes[item] += spaceActuallyUsed;
                spaceAllocatedThisIteration += spaceActuallyUsed;
            }

            // Reduce the available space by the space that was allocated in this iteration
            availableFlexibleSpace -= spaceAllocatedThisIteration;
        }

        float contentSizeAndFixedSpacing = fixedTotal + flexItemSizes.Values.Sum() + totalFixedSpacing;
        float flexibleSpacing = layoutParamsParams.innerSize - contentSizeAndFixedSpacing;

        
        float startOffsetPadding = 0;
        float totalItemSpacing = layoutParamsParams.spacing;
        
        if (layoutParamsParams.surplusMode == FlexLayoutParams.SurplusMode.Offset) {
            startOffsetPadding = flexibleSpacing * layoutParamsParams.surplusOffsetPivot;
        } else if (layoutParamsParams.surplusMode == FlexLayoutParams.SurplusMode.Space) {
            // When justifySpacePaddingRatio is 1 we're effectively pretending there are 2 zero-size items at the start and end of the list.
            var fakeItemCountForFlexibleSpacing = (items.Count - 1) + layoutParamsParams.surplusSpacePaddingRatio * 2;
            var flexibleItemSpacing = flexibleSpacing / fakeItemCountForFlexibleSpacing;
            startOffsetPadding = flexibleItemSpacing * layoutParamsParams.surplusSpacePaddingRatio;
            totalItemSpacing += flexibleItemSpacing;
        }
        float currentPosition = layoutParamsParams.paddingMin+startOffsetPadding;
        if (layoutParamsParams.reversed) {
            currentPosition = (layoutParamsParams.containerSize - layoutParamsParams.paddingMax) - startOffsetPadding;
        }

        var ranges = new List<Vector2>();
		
        foreach (var item in items) {
            float itemSize = item.flexible ? flexItemSizes[item] : item.fixedSize;
            if (layoutParamsParams.reversed) {
                ranges.Add(new Vector2(currentPosition - itemSize, currentPosition));
                currentPosition -= itemSize + totalItemSpacing;
            } else {
                ranges.Add(new Vector2(currentPosition, currentPosition + itemSize));
                currentPosition += itemSize + totalItemSpacing;
            }
        }

        return ranges;
    }

    public static List<Vector2> GetLayoutRanges(float containerSize, List<LayoutItemParams> items, float fixedItemSpacing = 0, float justifyPivot = 0.5f) {
        return GetLayoutRanges(new FlexLayoutParams(containerSize).SetSpacing(fixedItemSpacing).SetSurplusOffsetPivot(justifyPivot), items);
    }
}

[System.Serializable]
public class FlexLayout {
    public FlexLayoutParams layoutParams;
    public SLayout layout;
    public FlexLayout(FlexLayoutParams layoutParams, SLayout layout) {
        this.layoutParams = layoutParams;
        this.layout = layout;
    }
    
    public static FlexLayout Fixed(SLayout layout, SLayoutUtils.Axis axis) {
        return new FlexLayout(new FlexLayoutParams(layout.size[(int)axis]), layout);
    }
}

[System.Serializable]
public class LayoutItem {
    public LayoutItemParams layoutItemParams;
    // This may be null.
    public List<SLayout> layouts;

    // If true, sets the size of the layouts to the space allocated for them.
    public bool expandSize = true;
    // If expandSize is false, determines where the layouts are positioned within the space allocated for them.
    public float pivot = 0.5f;
        
    public LayoutItem(LayoutItemParams layoutItemParams, params SLayout[] layouts) {
        this.layoutItemParams = layoutItemParams;
        if(layouts is {Length: > 0}) this.layouts = new List<SLayout>(layouts);
    }
    
    public LayoutItem(LayoutItemParams layoutItemParams, bool expandSize, float pivot, params SLayout[] layouts) {
        this.layoutItemParams = layoutItemParams;
        this.expandSize = expandSize;
        this.pivot = pivot;
        if(layouts is {Length: > 0}) this.layouts = new List<SLayout>(layouts);
    }

    // Not sure if this is a good idea or not!
    public static LayoutItem Fixed(SLayout layout, SLayoutUtils.Axis axis, bool expandSize = true, float pivot = 0.5f) {
        return new LayoutItem(LayoutItemParams.Fixed(layout.size[(int)axis]), expandSize, pivot, layout);
    }
}





// Define the properties of the container that LayoutItemParams are laid out in.
// The container's size is always fixed, and the items are laid out in that space according to the properties. 
[Serializable]
public class FlexLayoutParams {
    // The size of the area the items should be laid out in.
    public float containerSize;
    public float innerSize => containerSize - totalPadding;
        
    public float paddingMin;
    public float paddingMax;
    public float totalPadding => paddingMin + paddingMax;
        
    // The fixed spacing between the elements. Extra spacing may be added if justifyMode is Space.
    public float spacing = 0f;
        
    // Describes what happens to extra space when the items don't fill the container.
    // Offset will add extra padding (at the start or end depending on the justifyPivot setting).
    public SurplusMode surplusMode = SurplusMode.Offset;
    public enum SurplusMode {
        Offset,
        Space,
    }
    // When using SurplusMode.Offset
    // This corresponds to flexbox's justify-content flex-start/flex-right/center options.
    // Set to 0 to add the spacing at the start, 0.5 for center, and 1 at the end.
    // Note the use of "Start and end" vs "min and max". They are relative to direction - if direction is reversed, 0 is the right edge instead of the left edge.
    public float surplusOffsetPivot = 0.5f;
        
    // When using SurplusMode.Space
    // This corresponds to flexbox's justify-content space options.
    // Set to 0 for space-between, 0.5 for space-around, and 1 for space-evenly.
    public float surplusSpacePaddingRatio = 0f;

    // When reversed, the layout starts at the max rather than the min, and starts with the last layout item rather than the first.
    // Note that surplusOffsetPivot is not reversed.
    public bool reversed = false;
    
    public FlexLayoutParams(float containerSize) {
        this.containerSize = containerSize;
    }
    
    public FlexLayoutParams SetContainerSize (float value) { containerSize = value; return this; }
    
    public FlexLayoutParams SetPadding (float value) { paddingMin = paddingMax = value; return this; }
    public FlexLayoutParams SetPaddingMin (float value) { paddingMin = value; return this; }
    public FlexLayoutParams SetPaddingMax (float value) { paddingMax = value; return this; }
    
    public FlexLayoutParams SetSpacing (float value) { spacing = value; return this; }
    public FlexLayoutParams SetSurplusOffsetPivot (float value) { surplusMode = SurplusMode.Offset; surplusOffsetPivot = value; return this; }
    public FlexLayoutParams SetSurplusSpacePaddingRatio (float value) { surplusMode = SurplusMode.Space; surplusSpacePaddingRatio = value; return this; }
    
    public FlexLayoutParams SetReversed (bool value) { this.reversed = value; return this; }
}

// LayoutItemParams determine the sizes of the layouts when using GetLayoutRanges, which is the base of other layout functions.
// It allows fixed and flexible sizes.
// Flexible is similar to CSS Flexbox and can have a min/max size and a weight.
[System.Serializable]
public class LayoutItemParams {
    // This is present in flexbox, and it might be an upgrade to consider.
    // public int order;
    
    // If the item is flexible
    public bool flexible;
    
    // If the item isn't flexible, this is the size that is used.
    public float fixedSize;
    
    // Min/Max sizes for flexible items.
    public float minSize;
    public float maxSize;
    // This is similar to flex-grow in CSS.
    public float weight;

    public static LayoutItemParams Fixed(float size) {
        var layoutItem = new LayoutItemParams {
            fixedSize = size
        };
        return layoutItem;
    }
    
    // I'm not 100% on this interface, but it's simple enough that it doesn't matter too much.
    public static LayoutItemParams Flexible(float minSize = 0, float maxSize = float.MaxValue, float weight = 1) {
        var layoutItem = new LayoutItemParams {
            flexible = true,
            minSize = minSize,
            maxSize = maxSize,
            weight = weight
        };
        return layoutItem;
    }
}