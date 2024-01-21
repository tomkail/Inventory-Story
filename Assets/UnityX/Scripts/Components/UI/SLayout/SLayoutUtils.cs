using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SLayoutUtils { 
    public enum LayoutDirection {
        Row, // left to right
        RowReverse, // right to left
        Column, // top to bottom
        ColumnReverse // bottom to top
    }

    public enum Axis {
        X = 0,
        Y = 1,
        Both = 2,
    }
    
    #region Alignment
    // Aligns the items with the parent layout, using an anchor and a pivot to determine position. Takes transform hierarchy into account so that this function may be animated.
    public static void Align(SLayout parentLayout, IEnumerable<SLayout> layouts, Axis axis, float parentAnchor, float layoutPivot, float offset = 0) {
        if (layouts.Any(x => x == null)) {
            Debug.LogError("Null layout found in AutoLayoutWithSpacing");
            return;
        }
        
        var parentAnchorPos = Vector2.zero;
        parentAnchorPos[(int)axis] = parentLayout.size[(int)axis] * parentAnchor;
        var orderedLayouts = OrderByTransformDepth(layouts);
        foreach (var layout in orderedLayouts) {
            var anchorPos = parentLayout.ConvertPositionToTarget(parentAnchorPos, layout.parent);
            var position = anchorPos[(int)axis] - layout.targetSize[(int)axis] * layoutPivot + offset;
            SetPositionOnAxis(layout, position, axis);
        }
    }
    
    // Aligns the left edge of all the items to the left edge of the parent layout
    public static void AlignLeft(SLayout parentLayout, IEnumerable<SLayout> layouts, float offset = 0) {
        AlignHorizontal(parentLayout, layouts, 0f, 0f, offset);
    }
    
    // Aligns the right edge of all the items to the right edge of the parent layout
    public static void AlignRight(SLayout parentLayout, IEnumerable<SLayout> layouts, float offset = 0) {
        AlignHorizontal(parentLayout, layouts, 1f, 1f, offset);
    }

    // Aligns the center of all the items to the center of the parent layout
    public static void AlignHorizontalCenters(SLayout parentLayout, IEnumerable<SLayout> layouts, float offset = 0) {
        AlignHorizontal(parentLayout, layouts, 0.5f, 0.5f, offset);
    }

    // Aligns the x coordinate of all the layouts to the x coordinate of the parent layout, according the the anchor and pivot settings
    public static void AlignHorizontal(SLayout parentLayout, IEnumerable<SLayout> layouts, float parentAnchor, float layoutPivot, float offset = 0) {
        Align(parentLayout, layouts, Axis.X, parentAnchor, layoutPivot, offset);
    }
    
    // Aligns the center of all the items to the center of the parent layout
    public static void AlignVerticalCenters(SLayout parentLayout, IEnumerable<SLayout> layouts, float offset = 0) {
        AlignVertical(parentLayout, layouts, 0.5f, 0.5f, offset);
    }
    
    // Aligns the y coordinate of all the layouts to the y coordinate of the parent layout, according the the anchor and pivot settings
    public static void AlignVertical(SLayout parentLayout, IEnumerable<SLayout> layouts, float parentAnchor, float layoutPivot, float offset = 0) {
        Align(parentLayout, layouts, Axis.Y, parentAnchor, layoutPivot, offset);
    }
    #endregion
    

    #region Layout
    // A quick and simple auto-layout function. Returns the total width of the layout, ignoring the offset.
    public static float AutoLayout(SLayout parentLayout, IList<SLayout> layouts, Axis axis, float offset, float spacing, float minPadding, float maxPadding) {
        var localLayoutPositions = new List<Vector2>();
        float axisPos = minPadding + offset;
        for (var index = 0; index < layouts.Count; index++) {
            var layout = layouts[index];
            var localLayoutPos = Vector2.zero;
            localLayoutPos[(int)axis] = axisPos;

            localLayoutPositions.Add(localLayoutPos);

            axisPos += layout.targetSize[(int)axis] + (index < layouts.Count - 1 ? spacing : 0);
        }

        ApplyLocalLayoutPositionsInTransformDepthOrder(parentLayout, layouts, localLayoutPositions, axis);
        return axisPos + maxPadding - offset;
    }
    
    
    
    // Automatically lays out items within the parent using a pre-determined spacing, independent of the transform hierarchy of the items.
    public static float AutoLayoutWithSpacing(SLayout parentLayout, IList<SLayout> layouts, Axis axis, float spacing, bool expandItemsToFill = false, float minPadding = 0, float maxPadding = 0, float pivot = 0.5f) {
        if (layouts.Any(x => x == null)) {
            Debug.LogError("Null layout found in AutoLayoutWithSpacing");
            return 0;
        }
        
        var offset = 0f;
        if (expandItemsToFill) {
            var totalSpacingAndPadding = spacing * (layouts.Count - 1) + minPadding + maxPadding;
            var elementSize = (parentLayout.targetSize[(int)axis] - totalSpacingAndPadding) / layouts.Count;
            for (var index = 0; index < layouts.Count; index++) {
                var layout = layouts[index];
                var size = layout.targetSize;
                size[(int)axis] = elementSize;
                layout.size = size;
            }
        } else {
            float totalContentSize = layouts.Sum(x => x.targetSize[(int)axis]) + spacing * (layouts.Count() - 1) + minPadding + maxPadding;
            var spareSpace = parentLayout.targetSize[(int)axis] - totalContentSize;
            offset = spareSpace * pivot;
        }

        return AutoLayout(parentLayout, layouts, axis, offset, spacing, minPadding, maxPadding);
    }

    public static float AutoLayoutXWithSpacing (SLayout parentLayout, IList<SLayout> layouts, float spacing, bool expandItemsToFill = false, float minPadding = 0, float maxPadding = 0, float pivot = 0.5f) {
        return AutoLayoutWithSpacing(parentLayout, layouts, Axis.X, spacing, expandItemsToFill, minPadding, maxPadding, pivot);
    }
    public static float AutoLayoutYWithSpacing (SLayout parentLayout, IList<SLayout> layouts, float spacing, bool expandItemsToFill = false, float minPadding = 0, float maxPadding = 0, float pivot = 0.5f) {
        return AutoLayoutWithSpacing(parentLayout, layouts, Axis.Y, spacing, expandItemsToFill, minPadding, maxPadding, pivot);
    }

    
    // Automatically lays out items to fill the parent, independent of the transform hierarchy of the items.
    public static float AutoLayoutAndFillSpace (SLayout parentLayout, IList<SLayout> layouts, Axis axis, float minPadding = 0, float maxPadding = 0) {
        float totalContentSize = layouts.Sum(x => x.targetSize[(int)axis]) + minPadding + maxPadding;
        var spareSpace = parentLayout.targetSize[(int)axis] - totalContentSize;
        var spacing = spareSpace / (layouts.Count() - 1);
        
        return AutoLayout(parentLayout, layouts, axis, 0, spacing, minPadding, maxPadding);
    }

    public static float AutoLayoutXAndFillSpace (SLayout parentLayout, IList<SLayout> layouts, float minPadding = 0, float maxPadding = 0) {
        return AutoLayoutAndFillSpace(parentLayout, layouts, Axis.X, minPadding, maxPadding);
    }
    public static float AutoLayoutYAndFillSpace (SLayout parentLayout, IList<SLayout> layouts, float minPadding = 0, float maxPadding = 0) {
        return AutoLayoutAndFillSpace(parentLayout, layouts, Axis.Y, minPadding, maxPadding);
    }
    

    // Automatically lays out items to fill the parent, independent of the transform hierarchy of the items. Adds extra spacing at the top/bottom, according to the amount of space left over.
    public static float AutoLayoutAndFillSpaceWithSpacingAtEdge(SLayout parentLayout, IList<SLayout> layouts, Axis axis, float minPadding = 0, float maxPadding = 0, float pivot = 0.5f) {
        float totalContentSize = layouts.Sum(x => x.targetSize[(int)axis]);
        var spareSpace = parentLayout.targetSize[(int)axis] - totalContentSize;
        var spacing = spareSpace / layouts.Count();
        
        return AutoLayout(parentLayout, layouts, axis, spacing * pivot, spacing, minPadding, maxPadding);
    }
    
    public static float AutoLayoutXAndFillSpaceWithSpacingAtEdge (SLayout parentLayout, IList<SLayout> layouts, float minPadding = 0, float maxPadding = 0, float pivot = 0.5f) {
        return AutoLayoutAndFillSpaceWithSpacingAtEdge(parentLayout, layouts, Axis.X, minPadding, maxPadding, pivot);
    }
    public static float AutoLayoutYAndFillSpaceWithSpacingAtEdge (SLayout parentLayout, IList<SLayout> layouts, float minPadding = 0, float maxPadding = 0, float pivot = 0.5f) {
        return AutoLayoutAndFillSpaceWithSpacingAtEdge(parentLayout, layouts, Axis.Y, minPadding, maxPadding, pivot);
    }

    
    // Automatically lays out items within the parent using a dynamic sizing of items according to their individual layout params, independent of the transform hierarchy of the items.
    public static float AutoLayoutWithDynamicSizing(SLayout parentLayout, IList<(SLayout layout, LayoutItemParams layoutParams)> layoutsAndParams, Axis axis, float spacing, float minPadding = 0, float maxPadding = 0, float pivot = 0.5f) {
        if (layoutsAndParams.Any(x => x.layout == null)) {
            Debug.LogError("Null layout found in AutoLayoutWithSpacing");
            return 0;
        }
        
        var totalPadding = minPadding + maxPadding;
        var ranges = LayoutUtils.GetLayoutRanges(parentLayout.width - totalPadding, layoutsAndParams.Select(x => x.layoutParams).ToList(), spacing, pivot);
        var layouts = layoutsAndParams.Select(x => x.layout).ToList();
        // Set sizes
        List<Vector2> positions = new List<Vector2>();
        for (var index = 0; index < layouts.Count; index++) {
            var layout = layouts[index];
            
            var size = layout.targetSize;
            size[(int)axis] = ranges[index].y - ranges[index].x;
            layout.size = size;
            
            Vector2 position = layout.position;
            position[(int)axis] = ranges[index].x + minPadding;
            positions.Add(position);
        }
        ApplyLocalLayoutPositionsInTransformDepthOrder(parentLayout, layouts, positions, axis);
        return ranges.Last().y;
    }

    public static float AutoLayoutWithDynamicSizing(FlexLayout flexLayout, List<LayoutItem> layoutItems, Axis axis) {
        if (flexLayout == null) {
            Debug.LogError("AutoLayoutWithDynamicSizing can't run because parentLayout is null");
            return 0;
        }
        if (layoutItems.IsNullOrEmpty()) return flexLayout.layoutParams.totalPadding;
        
        var ranges = LayoutUtils.GetLayoutRanges(flexLayout.layoutParams, layoutItems.Select(x => x.layoutItemParams).ToList());

        List<SLayout> layouts = new List<SLayout>();
        List<Vector2> positions = new List<Vector2>();
        for (var index = 0; index < ranges.Count; index++) {
            var layoutItem = layoutItems[index];
            if(layoutItem.layouts == null) continue;
            foreach (var layout in layoutItem.layouts) {
                // We might want to warn if this occurs but it doesn't affect the layout so it's not a big deal
                if (layout == null) continue;
                
                Vector2 position = layout.position;
                
                if (layoutItem.expandSize) {
                    var size = layout.targetSize;
                    size[(int) axis] = ranges[index].y - ranges[index].x;
                    layout.size = size;
                    
                    position[(int) axis] = ranges[index].x;
                }
                // If not setting size, then set position according to the pivot of the item 
                else {
                    position[(int) axis] = (Mathf.Lerp(ranges[index].x, ranges[index].y, layoutItem.pivot) - layout.targetSize[(int) axis] * layoutItem.pivot);
                }
                positions.Add(position);
                

                layouts.Add(layout);
            }
        }
        ApplyLocalLayoutPositionsInTransformDepthOrder(flexLayout.layout, layouts, positions, (Axis)axis);
        var totalSizeConsumedIncludingPadding = ranges.Last().y - ranges.First().x + flexLayout.layoutParams.totalPadding;
        return totalSizeConsumedIncludingPadding;
    }
    

    // Automatically lays out a single layout item within the parent using a dynamic sizing of items according to their individual layout params, independent of the transform hierarchy of the items.
    // Allows multiple layouts to take the same layout params.
    public static float AutoLayoutWithDynamicSizing(SLayout parentLayout, LayoutItem layoutItem, Axis axis, float minPadding = 0, float maxPadding = 0, float pivot = 0.5f) {
        return AutoLayoutWithDynamicSizing(parentLayout, new List<LayoutItem>() {layoutItem}, axis, 0, minPadding, maxPadding, pivot);
    }

    // Automatically lays out items within the parent using a dynamic sizing of items according to their individual layout params, independent of the transform hierarchy of the items.
    // Allows multiple layouts to take the same layout params.
    public static float AutoLayoutWithDynamicSizing(SLayout parentLayout, List<LayoutItem> layoutItems, Axis axis, float spacing = 0, float minPadding = 0, float maxPadding = 0, float pivot = 0.5f) {
        if (parentLayout == null) {
            Debug.LogError("AutoLayoutWithDynamicSizing can't run because parentLayout is null");
            return 0;
        }
        var totalPadding = minPadding + maxPadding;
        if (layoutItems.IsNullOrEmpty()) return totalPadding;
        
        var ranges = LayoutUtils.GetLayoutRanges(parentLayout.size[(int)axis] - totalPadding, layoutItems.Select(x => x.layoutItemParams).ToList(), spacing, pivot);
        
        List<SLayout> layouts = new List<SLayout>();
        List<Vector2> positions = new List<Vector2>();
        for (var index = 0; index < ranges.Count; index++) {
            var layoutItem = layoutItems[index];
            if(layoutItem.layouts == null) continue;
            foreach (var layout in layoutItem.layouts) {
                // We might want to warn if this occurs but it doesn't affect the layout so it's not a big deal
                if (layout == null) continue;
                
                Vector2 position = layout.position;
                
                if (layoutItem.expandSize) {
                    var size = layout.targetSize;
                    size[(int)axis] = ranges[index].y - ranges[index].x;
                    layout.size = size;
                    
                    position[(int)axis] = ranges[index].x + minPadding;
                }
                // If not setting size, then set position according to the pivot of the item 
                else {
                    position[(int)axis] = (Mathf.Lerp(ranges[index].x, ranges[index].y, layoutItem.pivot) - layout.targetSize[(int) axis] * layoutItem.pivot) + minPadding;
                }
                positions.Add(position);
                

                layouts.Add(layout);
            }
        }
        ApplyLocalLayoutPositionsInTransformDepthOrder(parentLayout, layouts, positions, axis);
        return ranges.Last().y-ranges.First().x + totalPadding;
    }
    
    
    public static void FillContainer(SLayout parentLayout, IEnumerable<SLayout> layouts, Axis axis, float minPadding, float maxPadding) {
        AutoLayoutWithDynamicSizing(parentLayout, new LayoutItem(LayoutItemParams.Flexible(), layouts.ToArray()), axis, minPadding, maxPadding);
    }
    #endregion
    
    #region Utils
    // Layout the items according to positions relative to the parentLayout. Takes transform hierarchy into account so that this function may be animated.
    public static void ApplyLocalLayoutPositionsInTransformDepthOrder(SLayout parentLayout, IList<SLayout> layouts, IList<Vector2> localLayoutPositions, Axis axis = Axis.Both) {
        if (parentLayout == null) {
            Debug.LogError("AutoLayoutWithDynamicSizing can't run because parentLayout is null");
            return;
        }
        var order = GetOrderTransformationByTransformDepth(layouts);
        var orderedLayouts = ReorderedList(layouts, order);
        var orderedLocalLayoutPositions = ReorderedList(localLayoutPositions, order);
        for (var index = 0; index < localLayoutPositions.Count; index++) {
            var layout = orderedLayouts[index];
            var localLayoutPos = orderedLocalLayoutPositions[index];
            var convertedPosition = parentLayout.ConvertPositionToTarget(localLayoutPos, layout.parent);
            if (axis == Axis.Both) layout.position = convertedPosition;
            else SetPositionOnAxis(layout, convertedPosition[(int)axis], axis);
        }
    }
    
    static void SetPositionOnAxis(SLayout layout, float position, Axis axis) {
        var targetPos = layout.position;
        targetPos[(int)axis] = position;
        layout.position = targetPos;
    }
    
    
    // Ordering layouts by depth ensures that animations are applied in the correct order
    static int[] GetOrderTransformationByTransformDepth(IList<SLayout> layouts) {
        return GetOrder(layouts, (item) => GetTransformDepth(item.transform));
        static int GetTransformDepth(Transform transform) {
            if (transform.parent == null) return 0;
            else return 1 + GetTransformDepth(transform.parent);
        }
    }
    
    
    static int[] GetOrder<T, TKey>(IList<T> list, Func<T, TKey> selector)
    {
        return list
            .Select((value, index) => new { Value = value, Index = index })
            .OrderBy(item => selector(item.Value))
            .Select(item => item.Index)
            .ToArray();
    }

    static List<T> ReorderedList<T>(IList<T> list, int[] order) {
        return order.Select(index => list[index]).ToList();
    }
    
    static IOrderedEnumerable<SLayout> OrderByTransformDepth(IEnumerable<SLayout> layouts) {
        return layouts.OrderBy(x => GetTransformDepth(x.transform));
        static int GetTransformDepth(Transform transform) {
            if (transform.parent == null) return 0;
            else return 1 + GetTransformDepth(transform.parent);
        }
    }

    #endregion
}