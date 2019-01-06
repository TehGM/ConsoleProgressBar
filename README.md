# ConsoleProgressBar
A flexible and ready to use library for a progress bar for terminal applications.

The ProgressBar class keeps track of the terminal line the progress bar occupies, and rewrites only that single line when a change is made. This allows for multiple progress bars working at once, writing to console in lines after the bar, and keeps the performance high.
The bar is highly customizable using the properties in ProgressBar instance.
Thread-safety is maintained between progress bar instances, and can be enabled application-wide by providing custom lock object.

# Usage
1. Clone or download.
2. Build library project.
3. As a dependency to your project.
4. Add `using TehGM.ConsoleProgressBar;` and create a new `ProgressBar` instance.
```
ProgressBar myBar = new ProgressBar();
```
5. ***(optional)*** Set custom settings for the bar.
```
myBar.BarLength = 35;
myBar.CharFill = '+';
myBar.PercentageFormat = "0.00%";
```
6. Call `Start` to write the bar to the terminal at current line.
```
myBar.Start();
```
7. Update the bar whenever progress changes.
```
// 10% of progress is now done
myBar.Update(0.1);
```
# Thread-safety
The ProgressBar instances by default use shared lock object, so if bars are updated from multiple threads, console text will not get all messy. In some situations, it may be required to write other data to terminal from multiple threads. To keep the console clean, bars can be provided custom objects to use as lock. This can be done by either:
- Provide lock object to each bar instance.
```
object myLockObject = new object();
myBar1.LockObject = myLockObject;
myBar2.LockObject = myLockObject;
```
- Set default lock object. *Note that this will only affect bars that haven't been yet created*.
```
object myLockObject = new object();
ProgressBar.SetDefaultLockObject(myLockObject);
ProgressBar myBar1 = new ProgressBar();
ProgressBar myBar2 = new ProgressBar();
```
