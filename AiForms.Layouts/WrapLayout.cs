using System;
using System.Linq;
using Xamarin.Forms;

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
        /// Spacing added between elements
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
                defaultValue: false,
                propertyChanged: (bindable, oldvalue, newvalue) => ((WrapLayout)bindable).OnSizeChanged()
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

            totalWidth = Math.Max(Math.Max(totalWidth, rowWidth) - Spacing, 0);
            totalHeight = Math.Max(totalHeight + rowHeight - Spacing, 0);

            return new SizeRequest(new Size(totalWidth, totalHeight), new Size(minWidth, minHeight));
        }

        private void OnSizeChanged()
        {
            this.ForceLayout();
        }
    }
}
