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
                propertyChanged: (bindable, oldvalue, newvalue) => ((WrapLayout)bindable).InvalidateMeasure()
            );
        /// <summary>
        /// Spacing added between elements
        /// </summary>
        public double Spacing {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }


        public static BindableProperty UniformColumnsProperty =
            BindableProperty.Create(
                nameof(UniformColumns),
                typeof(int),
                typeof(WrapLayout),
                default(int),
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: (bindable, oldvalue, newvalue) => ((WrapLayout)bindable).InvalidateMeasure()
        );
        /// <summary>
        ///  number for uniform child width 
        /// </summary>
        public int UniformColumns {
            get { return (int)GetValue(UniformColumnsProperty); }
            set { SetValue(UniformColumnsProperty, value); }
        }

        public static BindableProperty IsSquareProperty =
            BindableProperty.Create(
                nameof(IsSquare),
                typeof(bool),
                typeof(WrapLayout),
                false,
                propertyChanged: (bindable, oldvalue, newvalue) => ((WrapLayout)bindable).InvalidateMeasure()
        );
        /// <summary>
        ///  make item height equal to item width when UniformColums > 0
        /// </summary>
        public bool IsSquare {
            get { return (bool)GetValue(IsSquareProperty); }
            set { SetValue(IsSquareProperty, value); }
        }

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
            double totalWidth = 0;
            double totalHeight = 0;
            double rowHeight = 0;
            double minWidth = 0;
            double minHeight = 0;
            double xPos = x;
            double yPos = y;

            totalWidth = widthConstraint;
            var exceptedWidth = widthConstraint - (UniformColumns - 1) * Spacing; //excepted spacing width

            var columsSize = exceptedWidth / UniformColumns;
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

                rowHeight = Math.Max(rowHeight, itemHeight + Spacing);

                minHeight = Math.Max(minHeight, itemHeight);
                minWidth = Math.Max(minWidth, itemWidth);

                if (doLayout) {
                    var region = new Rectangle(xPos, yPos, itemWidth, itemHeight);
                    LayoutChildIntoBoundingRegion(child, region);
                }

                xPos += itemWidth + Spacing;

                if (xPos + columsSize - x > widthConstraint) {
                    xPos = x;
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

            double totalWidth = 0;
            double totalHeight = 0;
            double rowHeight = 0;
            double rowWidth = 0;
            double minWidth = 0;
            double minHeight = 0;
            double xPos = x;
            double yPos = y;

            var visibleChildren = Children.Where(c => c.IsVisible).Select(c => new {
                child = c,
                size = c.Measure(widthConstraint, heightConstraint)
            });

            var nextChildren = visibleChildren.Skip(1).ToList();
            nextChildren.Add(null); //make element count same

            var zipChildren = visibleChildren.Zip(nextChildren, (c, n) => new { current = c, next = n });

            foreach (var childBlock in zipChildren) {

                var child = childBlock.current.child;
                var size = childBlock.current.size;
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

                if (childBlock.next == null) {
                    totalHeight += rowHeight;
                    totalWidth = Math.Max(totalWidth, rowWidth);
                    break;
                }

                xPos += itemWidth + Spacing;
                var nextWitdh = childBlock.next.size.Request.Width;

                if (xPos + nextWitdh - x > widthConstraint) {
                    xPos = x;
                    yPos += rowHeight;
                    totalHeight += rowHeight;
                    totalWidth = Math.Max(totalWidth, rowWidth);
                    rowHeight = 0;
                    rowWidth = 0;
                }
            }

            totalWidth = Math.Max(totalWidth - Spacing, 0);
            totalHeight = Math.Max(totalHeight - Spacing, 0);

            return new SizeRequest(new Size(totalWidth, totalHeight), new Size(minWidth, minHeight));
        }
    }
}
