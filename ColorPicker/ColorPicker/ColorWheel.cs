﻿using System;
using Xamarin.Forms;

using ColorPicker.BaseClasses;
using ColorPicker.Interfaces;

namespace ColorPicker
{
    public class ColorWheel : ColorPickerViewBase
    {
        private readonly ColorCircle colorCircle = new ColorCircle();
        private readonly AlphaSlider alphaSlider = new AlphaSlider();
        private readonly LuminositySlider luminositySlider = new LuminositySlider();

        protected const double LuminositySliderRowHeight = 12;
        protected const double AlphaSliderRowHeight = 12;

        public ColorWheel()
        {
            colorCircle.ConnectedColorPicker = this;
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Center;
            Children.Add(colorCircle);

            alphaSlider.ConnectedColorPicker = this;
            luminositySlider.ConnectedColorPicker = this;

            UpdateAlphaSlider(ShowAlphaSlider);

            UpdateLuminositySlider(ShowLuminositySlider);
        }

        public static readonly BindableProperty ShowLuminosityWheelProperty = BindableProperty.Create(
           nameof(ShowLuminosityWheel),
           typeof(bool),
           typeof(ColorWheel),
           true,
           propertyChanged: new BindableProperty.BindingPropertyChangedDelegate(HandleShowLuminositySet));

        static void HandleShowLuminositySet(BindableObject bindable, object oldValue, object newValue)
        {
            ((ColorWheel)bindable).colorCircle.ShowLuminosityWheel = (bool)newValue;
        }

        public bool ShowLuminosityWheel
        {
            get
            {
                return (bool)GetValue(ShowLuminosityWheelProperty);
            }
            set
            {
                SetValue(ShowLuminosityWheelProperty, value);
            }
        }

        public static readonly BindableProperty ShowLuminositySliderProperty = BindableProperty.Create(
           nameof(ShowLuminositySlider),
           typeof(bool),
           typeof(ColorWheel),
           false,
           propertyChanged: new BindableProperty.BindingPropertyChangedDelegate(HandleShowLuminositySliderSet));

        static void HandleShowLuminositySliderSet(BindableObject bindable, object oldValue, object newValue)
        {
            ((ColorWheel)bindable).UpdateLuminositySlider((bool)newValue);
        }

        public bool ShowLuminositySlider
        {
            get
            {
                return (bool)GetValue(ShowLuminositySliderProperty);
            }
            set
            {
                SetValue(ShowLuminositySliderProperty, value);
            }
        }

        public static readonly BindableProperty ShowAlphaSliderProperty = BindableProperty.Create(
           nameof(ShowAlphaSlider),
           typeof(bool),
           typeof(ColorWheel),
           false,
           propertyChanged: new BindableProperty.BindingPropertyChangedDelegate(HandleShowAlphaSliderSet));

        static void HandleShowAlphaSliderSet(BindableObject bindable, object oldValue, object newValue)
        {
            ((ColorWheel)bindable).UpdateAlphaSlider((bool)newValue);
        }

        public bool ShowAlphaSlider
        {
            get
            {
                return (bool)GetValue(ShowAlphaSliderProperty);
            }
            set
            {
                SetValue(ShowAlphaSliderProperty, value);
            }
        }

        public static readonly BindableProperty WheelBackgroundColorProperty = BindableProperty.Create(
           nameof(WheelBackgroundColor),
           typeof(Color),
           typeof(IColorPicker),
           Color.Transparent,
           propertyChanged: new BindableProperty.BindingPropertyChangedDelegate(HandleWheelBackgroundColorSet));

        static void HandleWheelBackgroundColorSet(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue != oldValue)
            {
                ((ColorWheel)bindable).colorCircle.WheelBackgroundColor = (Color)newValue;
            }
        }

        public Color WheelBackgroundColor
        {
            get
            {
                return (Color)GetValue(WheelBackgroundColorProperty);
            }
            set
            {
                SetValue(WheelBackgroundColorProperty, value);
            }
        }

        public static readonly BindableProperty PickerRadiusScaleProperty = BindableProperty.Create(
           nameof(PickerRadiusScale),
           typeof(float),
           typeof(SkiaSharpPickerBase),
           0.05F,
           propertyChanged: new BindableProperty.BindingPropertyChangedDelegate(HandlePickerRadiusScaleSet));

        static void HandlePickerRadiusScaleSet(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue != oldValue)
            {
                ((ColorWheel)bindable).colorCircle.PickerRadiusScale = (float)newValue;
                ((ColorWheel)bindable).alphaSlider.PickerRadiusScale = (float)newValue;
                ((ColorWheel)bindable).luminositySlider.PickerRadiusScale = (float)newValue;
            }
        }

        public float PickerRadiusScale
        {
            get
            {
                return (float)GetValue(PickerRadiusScaleProperty);
            }
            set
            {
                SetValue(PickerRadiusScaleProperty, value);
            }
        }

        public static readonly BindableProperty VerticalProperty = BindableProperty.Create(
           nameof(Vertical),
           typeof(bool),
           typeof(SliderPicker),
           false,
           propertyChanged: new BindableProperty.BindingPropertyChangedDelegate(HandleVerticalSet));

        static void HandleVerticalSet(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue != oldValue)
            {
                ((ColorWheel)bindable).alphaSlider.Vertical = (bool)newValue;
                ((ColorWheel)bindable).luminositySlider.Vertical = (bool)newValue;
            }
        }

        public bool Vertical
        {
            get
            {
                return (bool)GetValue(VerticalProperty);
            }
            set
            {
                SetValue(VerticalProperty, value);
            }
        }

        protected override void ChangeSelectedColor(Color color)
        {
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            if (Double.IsPositiveInfinity(widthConstraint) &&
                Double.IsPositiveInfinity(heightConstraint))
            {
                widthConstraint = 200;
                heightConstraint = 200;
            }

            double aspectRatio = 1;
            if (ShowAlphaSlider)
            {
                aspectRatio -= 0.1;
            }
            if (ShowLuminositySlider)
            {
                aspectRatio -= 0.1;
            }

            double minWidth;
            double minHeight;

            if (Vertical)
            {
                minHeight = Math.Min(heightConstraint, aspectRatio * widthConstraint);
                minWidth = minHeight / aspectRatio;
            }
            else
            {
                minWidth = Math.Min(widthConstraint, aspectRatio * heightConstraint);
                minHeight = minWidth / aspectRatio;
            }

            return new SizeRequest(new Size(minWidth, minHeight));
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            var circleSize = Vertical ? height : width;
            colorCircle.Layout(new Rectangle(x, y, circleSize, circleSize));

            double bottom;
            if (Vertical)
            {
                bottom = x + circleSize;
            }
            else
            {
                bottom = y + width;
            }

            var sliderHeight = colorCircle.GetPickerRadiusPixels(new SkiaSharp.SKSize((float)width, (float)height)) * 2.4F;
            if (ShowLuminositySlider)
            {
                if (Vertical)
                {
                    luminositySlider.Layout(new Rectangle(bottom, x, sliderHeight, circleSize));
                }
                else
                {
                    luminositySlider.Layout(new Rectangle(x, bottom, circleSize, sliderHeight));
                }
                bottom += sliderHeight;
            }
            if (ShowAlphaSlider)
            {
                if (Vertical)
                {
                    alphaSlider.Layout(new Rectangle(bottom, x, sliderHeight, circleSize));
                }
                else
                {
                    alphaSlider.Layout(new Rectangle(x, bottom, circleSize, sliderHeight));
                }
            }
        }

        private void BindedIColorPicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedColor))
            {
                SelectedColor = ((IColorPicker)sender).SelectedColor;
            }
        }

        private void UpdateAlphaSlider(bool show)
        {
            if (show)
            {
                Children.Add(alphaSlider);
            }
            else
            {
                Children.Remove(alphaSlider);
            }
        }

        private void UpdateLuminositySlider(bool show)
        {
            if (show)
            {
                Children.Add(luminositySlider);
            }
            else
            {
                Children.Remove(luminositySlider);
            }
        }
    }
}