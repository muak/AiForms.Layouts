using System;
using System.Linq;
using Xamarin.Forms;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections;

namespace AiForms.Layouts
{
    /// <summary>
    /// Layout which performs wrapping on the boundaries.
    /// Base Code -> https://forums.xamarin.com/discussion/comment/57486/#Comment_57486 
    ///              http://bit.ly/xf-custompanel
    /// </summary>
    public class WrapLayout : Layout<View>
    {
        public static BindableProperty SpacingProperty =
            BindableProperty.Create(
                nameof(Spacing),
                typeof(double),
                typeof(WrapLayout),
                default(double),
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: (bindable, oldvalue, newvalue) => ((WrapLayout)bindable).OnSizeChanged()
            );
        /// <summary>
        /// Spacing added between elements (both directions)
        /// </summary>
        public double Spacing {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }


        public static readonly BindableProperty UniformColumnsProperty =
            BindableProperty.Create(
                propertyName: "UniformColumns",
                returnType: typeof(int),
                declaringType: typeof(WrapLayout),
                defaultValue: default(int),
                propertyChanged: (bindable, oldvalue, newvalue) => ((WrapLayout)bindable).OnSizeChanged()
        );
        /// <summary>
        ///  number for uniform child width 
        /// </summary>
        public int UniformColumns {
            get { return (int)GetValue(UniformColumnsProperty); }
            set { SetValue(UniformColumnsProperty, value); }
        }

        public static readonly BindableProperty IsSquareProperty =
            BindableProperty.Create(
                propertyName: "IsSquare",
                returnType: typeof(bool),
                declaringType: typeof(WrapLayout),
                defaultValue: false
        );
        /// <summary>
        ///  make item height equal to item width when UniformColums > 0
        /// </summary>
        public bool IsSquare {
            get { return (bool)GetValue(IsSquareProperty); }
            set { SetValue(IsSquareProperty, value); }
        }


        /// <summary>
        /// This method is called during the measure pass of a layout cycle to get the desired size of an element.
        /// </summary>
        /// <param name="widthConstraint">The available width for the element to use.</param>
        /// <param name="heightConstraint">The available height for the element to use.</param>
        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            if (WidthRequest > 0)
                widthConstraint = Math.Min(widthConstraint, WidthRequest);
            if (HeightRequest > 0)
                heightConstraint = Math.Min(heightConstraint, HeightRequest);

            double internalWidth = double.IsPositiveInfinity(widthConstraint) ? double.PositiveInfinity : Math.Max(0, widthConstraint);
            double internalHeight = double.IsPositiveInfinity(heightConstraint) ? double.PositiveInfinity : Math.Max(0, heightConstraint);

            return UniformColumns > 0 ? UniformMeasureAndLayout(internalWidth, internalHeight) :
                                        VariableMeasureAndLayout(internalWidth, internalHeight);

        }


        private SizeRequest DoUniformMeasure(double widthConstraint, double heightConstraint)
        {
            return UniformMeasureAndLayout(widthConstraint, heightConstraint);

            //int rowCount = 0;

            //double totalWidth = 0;
            //double totalHeight = 0;
            //double lastHeight = 0;
            //double rowHeight = 0;
            //double minWidth = 0;
            //double minHeight = 0;
            //double xPos = 0;

            //var remainder = 0;
            //var remainderAlt = 0;

            //totalWidth = widthConstraint;
            //var exceptedWidth = (int)widthConstraint - (UniformColumns - 1) * Spacing; //excepted spacing width

            //// Divide remainder and put it both ends
            //remainder = (int)exceptedWidth % UniformColumns;
            //remainderAlt = remainder / 2;
            //remainder = remainder % 2 + remainderAlt;

            //var columsSize = (int)exceptedWidth / UniformColumns;
            //if (columsSize < 1) {
            //    columsSize = 1;
            //}

            //foreach (var child in Children.Where(c => c.IsVisible)) {

            //    var size = child.Measure(widthConstraint, heightConstraint);
            //    var itemWidth = (double)columsSize;
            //    var itemHeight = size.Request.Height;

            //    lastHeight = rowHeight;

            //    if (IsSquare) {
            //        itemHeight = columsSize;
            //    }

            //    if (xPos <= double.Epsilon) {
            //        itemWidth += remainder;
            //    }
            //    if (xPos + itemWidth + Spacing > widthConstraint) {
            //        itemWidth += remainderAlt;
            //    }

            //    rowHeight = Math.Max(rowHeight, itemHeight+Spacing);

            //    minHeight = Math.Max(minHeight, itemHeight);
            //    minWidth = Math.Max(minWidth, itemWidth);



            //    xPos += itemWidth + Spacing;

            //    if (xPos + itemWidth > widthConstraint) {
            //        xPos = 0;
            //        rowCount++;
            //        totalHeight += rowHeight;
            //        rowHeight = 0;
            //    }
            //}

            //totalHeight += lastHeight - Spacing;

            //return new SizeRequest(new Size(totalWidth, totalHeight), new Size(minWidth, minHeight));
        }

        private SizeRequest UniformMeasureAndLayout(double widthConstraint, double heightConstraint, bool doLayout = false, double x = 0, double y = 0)
        {
            int rowCount = 0;

            double totalWidth = 0;
            double totalHeight = 0;
            double rowHeight = 0;
            double minWidth = 0;
            double minHeight = 0;
            double xPos = x;
            double yPos = y;

            var remainder = 0;
            var remainderAlt = 0;

            totalWidth = widthConstraint;
            var exceptedWidth = (int)widthConstraint - (UniformColumns - 1) * Spacing; //excepted spacing width

            // Divide remainder and put it both ends
            remainder = (int)exceptedWidth % UniformColumns;
            remainderAlt = remainder / 2;
            remainder = remainder % 2 + remainderAlt;

            var columsSize = (int)exceptedWidth / UniformColumns;
            if (columsSize < 1) {
                columsSize = 1;
            }

            foreach (var child in Children.Where(c => c.IsVisible)) {

                var size = child.Measure(widthConstraint, heightConstraint);
                var itemWidth = (double)columsSize;
                var itemHeight = size.Request.Height;

                if (IsSquare) {
                    itemHeight = columsSize;
                }

                if (Math.Abs(xPos - x) <= double.Epsilon) {
                    itemWidth += remainder;
                }
                if (xPos + itemWidth + Spacing > widthConstraint) {
                    itemWidth += remainderAlt;
                }

                rowHeight = Math.Max(rowHeight, itemHeight + Spacing);

                minHeight = Math.Max(minHeight, itemHeight);
                minWidth = Math.Max(minWidth, itemWidth);

                if (doLayout) {
                    if (IsSquare) {
                        child.HeightRequest = columsSize;
                    }
                    child.WidthRequest = itemWidth;

                    var region = new Rectangle(xPos, yPos, itemWidth, itemHeight);
                    LayoutChildIntoBoundingRegion(child, region);
                }

                xPos += itemWidth + Spacing;

                if (xPos + itemWidth - x > widthConstraint) {
                    xPos = x;
                    rowCount++;
                    yPos += rowHeight;
                    totalHeight += rowHeight;
                    rowHeight = 0;
                }
            }

            totalHeight = Math.Max(totalHeight + rowHeight - Spacing, 0);

            return new SizeRequest(new Size(totalWidth, totalHeight), new Size(minWidth, minHeight));

        }

        private SizeRequest VariableMeasureAndLayout(double widthConstraint, double heightConstraint, bool doLayout = false, double x = 0, double y = 0)
        {
            int rowCount = 0;

            double totalWidth = 0;
            double totalHeight = 0;
            double rowHeight = 0;
            double rowWidth = 0;
            double minWidth = 0;
            double minHeight = 0;
            double xPos = x;
            double yPos = y;

            foreach (var child in Children.Where(c => c.IsVisible)) {

                var size = child.Measure(widthConstraint, heightConstraint);
                var itemWidth = size.Request.Width;
                var itemHeight = size.Request.Height;

                rowHeight = Math.Max(rowHeight, itemHeight + Spacing);
                rowWidth += itemWidth + Spacing;

                minHeight = Math.Max(minHeight, itemHeight);
                minWidth = Math.Max(minWidth, itemWidth);

                if (doLayout) {
                    var region = new Rectangle(xPos, yPos, itemWidth, itemHeight);
                    LayoutChildIntoBoundingRegion(child, region);
                }

                xPos += itemWidth + Spacing;

                if (xPos + itemWidth - x > widthConstraint) {
                    xPos = x;
                    rowCount++;
                    yPos += rowHeight;
                    totalHeight += rowHeight;
                    totalWidth = Math.Max(totalWidth, rowWidth);
                    rowHeight = 0;
                    rowWidth = 0;
                }
            }

            totalWidth = Math.Max(totalWidth + rowWidth - Spacing, 0);
            totalHeight = Math.Max(totalHeight + rowHeight - Spacing, 0);

            return new SizeRequest(new Size(totalWidth, totalHeight), new Size(minWidth, minHeight));
        }


        /// <summary>
        /// Does the horizontal measure.
        /// </summary>
        /// <returns>The horizontal measure.</returns>
        /// <param name="widthConstraint">Width constraint.</param>
        /// <param name="heightConstraint">Height constraint.</param>
        private SizeRequest DoVariableMeasure(double widthConstraint, double heightConstraint)
        {
            int rowCount = 0;

            double totalWidth = 0;
            double totalHeight = 0;
            double rowWidth = 0;
            double rowHeight = 0;
            double lastHeight = 0;
            double minWidth = 0;
            double minHeight = 0;

            foreach (var child in Children.Where(c => c.IsVisible)) {

                var size = child.Measure(widthConstraint, heightConstraint);
                var itemHeight = size.Request.Height;
                var itemWidth = size.Request.Width;

                lastHeight = rowHeight;

                var nextWidth = rowWidth + itemWidth + Spacing;
                if (nextWidth > widthConstraint + Spacing) {
                    rowCount++;
                    totalHeight += rowHeight;
                    totalWidth = Math.Max(totalWidth, rowWidth);
                    rowWidth = 0;
                    rowHeight = 0;
                }


                minHeight = Math.Max(minHeight, size.Minimum.Height);
                minWidth = Math.Max(minWidth, size.Minimum.Width);

                rowWidth += itemWidth + Spacing;
                rowHeight = Math.Max(rowHeight, itemHeight + Spacing); // always take more higher
            }


            if (rowCount > 0) {
                totalWidth -= Spacing;
                totalHeight += lastHeight - Spacing;
            }

            return new SizeRequest(new Size(totalWidth, totalHeight), new Size(minWidth, minHeight));
        }





        /// <summary>
        /// Positions and sizes the children of a Layout.
        /// </summary>
        /// <param name="x">A value representing the x coordinate of the child region bounding box.</param>
        /// <param name="y">A value representing the y coordinate of the child region bounding box.</param>
        /// <param name="width">A value representing the width of the child region bounding box.</param>
        /// <param name="height">A value representing the height of the child region bounding box.</param>
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            if (UniformColumns > 0) {
                UniformMeasureAndLayout(width, height, true, x, y);
            }
            else {
                VariableMeasureAndLayout(width, height, true, x, y);
            }

            //double rowHeight = 0;
            //double yPos = y, xPos = x;

            //foreach (var child in Children.Where(c => c.IsVisible)) {

            //    var request = child.Measure(width, height);

            //    double childWidth = request.Request.Width;
            //    double childHeight = request.Request.Height;

            //    var divOver = 0;
            //    var divOverAlt = 0;
            //    if (UniformColumns != default(int)) {
            //        var exceptWidth = (int)width - (UniformColumns - 1) * Spacing;
            //        divOver = (int)exceptWidth % UniformColumns;
            //        divOverAlt = divOver / 2;
            //        divOver = divOver % 2 + divOverAlt;
            //        var columsSize = (int)exceptWidth / UniformColumns;
            //        if (columsSize < 1) {
            //            columsSize = 1;
            //        }

            //        childWidth = columsSize;

            //        if (IsSquare) {
            //            childHeight = columsSize;
            //            child.HeightRequest = columsSize;
            //        }

            //        if (Math.Abs(xPos - x) <= double.Epsilon) {
            //            childWidth += divOver;
            //        }
            //        if (xPos + childWidth + Spacing > width) {
            //            childWidth += divOverAlt;
            //        }
            //        child.WidthRequest = childWidth;
            //    }

            //    rowHeight = Math.Max(rowHeight, childHeight);



            //    var region = new Rectangle(xPos, yPos, childWidth, childHeight);

            //    LayoutChildIntoBoundingRegion(child, region);

            //    xPos += region.Width + Spacing;

            //    if (xPos + childWidth - x > width) {
            //        xPos = x;
            //        yPos += rowHeight + Spacing;
            //        rowHeight = 0;
            //    }


            //}


        }

        private void OnSizeChanged()
        {
            this.ForceLayout();
        }
    }
}
