# ConsoleProgressBar
[![Nuget](https://img.shields.io/nuget/v/TehGM.ConsoleProgressBar?style=flat)](https://www.nuget.org/packages/TehGM.ConsoleProgressBar/) [![GitHub](https://img.shields.io/github/license/TehGM/ConsoleProgressBar)](https://github.com/TehGM/ConsoleProgressBar/blob/master/LICENSE)

A flexible and ready to use library for a progress bar for terminal applications.

The ProgressBar class keeps track of the terminal line the progress bar occupies, and rewrites only that single line when a change is made. This allows for multiple progress bars working at once, writing to console in lines after the bar, and keeps the performance high.
The bar is highly customizable using the properties in ProgressBar instance.
Thread-safety is maintained between progress bar instances, and can be enabled application-wide by providing custom lock object.

# Usage
1. Add as a dependency to your project.
2. Add `using TehGM.ConsoleProgressBar;` and create a new `ProgressBar` instance.
```csharp
ProgressBar myBar = new ProgressBar();
```
3. ***(optional)*** Set custom settings for the bar.
```csharp
myBar.BarLength = 35;
myBar.CharFill = '+';
myBar.PercentageFormat = "0.00%";
```
4. Call `Start` to write the bar to the terminal at current line.
```csharp
myBar.Start();
```
5. Update the bar whenever progress changes.
```csharp
// 10% of progress is now done
myBar.Update(0.1);
```
6. ***(optional)*** Replace bar with text when done.
```csharp
// replace bar with text!
myBar.Write("Done!");
```
# Thread-safety
The ProgressBar instances by default use shared lock object, so if bars are updated from multiple threads, console text will not get all messy. In some situations, it may be required to write other data to terminal from multiple threads. To keep the console clean, bars can be provided custom objects to use as lock. This can be done by either:
- Provide lock object to each bar instance.
```csharp
object myLockObject = new object();
myBar1.LockObject = myLockObject;
myBar2.LockObject = myLockObject;
```
- Set default lock object. *Note that this will only affect bar instances that haven't been yet created using a constructor*.
```csharp
object myLockObject = new object();
ProgressBar.SetDefaultLockObject(myLockObject);
ProgressBar myBar1 = new ProgressBar();
ProgressBar myBar2 = new ProgressBar();
```

# License
Copyright (c) 2019 TehGM 

Licensed under [MIT License](https://github.com/TehGM/ConsoleProgressBar/blob/master/LICENSE).

