# AiForms.Layouts for Xamarin.Forms

This is a collection of  Xamarin.Forms custom layouts

## Referenced source code

* https://forums.xamarin.com/discussion/comment/57486/#Comment_57486
* https://forums.xamarin.com/discussion/21635/xforms-needs-an-itemscontrol/p2

## Features

* [RepeatableFlex](#repeatableflex)
* [WrapLayout](#wraplayout)
* [RepeatableWrapLayout](#repeatablewraplayout)
* [RepeatableStack](#repeatablestack)

<img src="images/1.png" width=200 /><img src="images/2.png" width=200 /><img src="images/3.png" width=200 />

## Demo

https://twitter.com/muak_x/status/830061279330996224

## Nuget Installation

[https://www.nuget.org/packages/AiForms.Layouts/](https://www.nuget.org/packages/AiForms.Layouts/)

```bash
Install-Package AiForms.Layouts
```

You need to install this package to .NETStandard / PCL project and **each platform project**.

### iOS

If you don't use XamlCompilationOptions.Compile, need to write following code in AppDelegate.cs; Otherwise needn't.

```cs
public override bool FinishedLaunching(UIApplication app, NSDictionary options) {
    global::Xamarin.Forms.Forms.Init();

    AiForms.Layouts.LayoutsInit.Init();  //need to write here

    LoadApplication(new App(new iOSInitializer()));

    return base.FinishedLaunching(app, options);
}
```

## RepeatableFlex

This layout is a FlexLayout corresponding to DataTemplate and DataTemplateSelector.

### Parameters

* ItemsSource
* ItemTemplate

### How to write with Xaml

```xml
<ScrollView Orientation="Virtical" HeightRequest="86">
<al:RepeatableFlex Direction="Row" Wrap="Wrap" AlignItems="Start" JustifyContent="Start" ItemsSource="{Binding BoxList}">
	<al:RepeatableFlex.ItemTemplate>
		<DataTemplate>
			<ContentView BackgroundColor="{Binding Color}" WidthRequest="80" HeightRequest="80" Padding="3" />
		</DataTemplate>
	</al:RepeatableFlex.ItemTemplate>
</al:RepeatableFlex>
</ScrollView>
```

## WrapLayout

This Layout performs wrapping on the boundaries.

_By Flex Layout having come, there is seldom opportunity using this layout. But it can be used when you want to arrange uniformly each items depending on screen width or make it square._

### Parameters

* Spacing
    * added between elements
* UniformColumns
    * number for uniform child width (default 0)
    * If it is 0,it will obey WidthRequest value.
    * If it is more than 0 ,a child width will be  width which divide parent width by this number.
* IsSquare
    * If it is true,it make item height equal to item width when UniformColums > 0 (default false)

### How to write with Xaml

```xml
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
		xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
		xmlns:l="clr-namespace:AiForms.Layouts;assembly=AiForms.Layouts"
		x:Class="Sample.Views.MainPage">
    <StackLayout>
        <l:WrapLayout Spacing="4" UniformColumns="3" IsSquare="true" HorizontalOptions="FillAndExpand">
    		<BoxView Color="Red" />
            <BoxView Color="Blue" />
            <BoxView Color="Green" />
            <BoxView Color="Black" />
            <BoxView Color="Yellow" />
        </l:WrapLayout>
    </StackLayout>
</ContentPage>
```

## RepeatableWrapLayout

This Layout is WrapLayout corresponding to DataTemplate and DataTemplateSelector.

> If a lot of items are arranged, you should use [CollectionView](https://github.com/muak/AiForms.CollectionView) that can recycle items.

### Parameters

* ItemTapCommandProperty
    * Command invoked when a item is tapped.
* ItemsSource
* ItemTemplate

### How to write with Xaml

```xml
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
		xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
		xmlns:l="clr-namespace:AiForms.Layouts;assembly=AiForms.Layouts"
		x:Class="Sample.Views.MainPage">
	<StackLayout>
		<ScrollView HorizontalOptions="FillAndExpand">
			<l:RepeatableWrapLayout
				ItemTapCommand="{Binding TapCommand}"
				ItemsSource="{Binding BoxList}"
				Spacing="3" UniformColumns="{Binding UniformColumns}"
				IsSquare="{Binding IsSquare}" >
				<l:RepeatableWrapLayout.ItemTemplate>
					<DataTemplate>
						<StackLayout BackgroundColor="{Binding Color}" >
							<Label
								VerticalTextAlignment="Center" HorizontalTextAlignment="Center"
								Text="{Binding Name}"  />
						</StackLayout>
					</DataTemplate>
				</l:RepeatableWrapLayout.ItemTemplate>
			</l:RepeatableWrapLayout>
		</ScrollView>
	</StackLayout>
</ContentPage>
```

## RepeatableStack

This layout is a StackLayout corresponding to DataTemplate and DataTemplateSelector.

### Parameters

* ItemsSource
* ItemTemplate

### How to write with Xaml

```xml
<!-- Horizontal -->
<ScrollView Orientation="Horizontal" HeightRequest="86">
<al:RepeatableStack Orientation="Horizontal" ItemsSource="{Binding BoxList}" HeightRequest="86">
	<al:RepeatableStack.ItemTemplate>
		<DataTemplate>
			<ContentView BackgroundColor="{Binding Color}" WidthRequest="80" HeightRequest="80" Padding="3" />
		</DataTemplate>
	</al:RepeatableStack.ItemTemplate>
</al:RepeatableStack>
</ScrollView>

<!-- Vertical -->
<ScrollView>
<al:RepeatableStack Orientation="Vertical" ItemsSource="{Binding BoxList}">
	<al:RepeatableStack.ItemTemplate>
		<DataTemplate>
			<ContentView BackgroundColor="{Binding Color}" WidthRequest="80" HeightRequest="80" Padding="3" />
		</DataTemplate>
	</al:RepeatableStack.ItemTemplate>
</al:RepeatableStack>
</ScrollView>
```
## Contributors

* [predalpha](https://github.com/predalpha)


## License

MIT Licensed.
