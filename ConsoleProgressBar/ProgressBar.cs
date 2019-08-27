using System;
using System.Text;

namespace TehGM.ConsoleProgressBar
{
    /// <summary>
    /// A single console progress bar.
    /// </summary>
    public class ProgressBar
    {
        private static object _defaultLock = new object();

        // properties defaults
        private int _barLength = 30;
        private int _textSpace = 50;
        private object _lock = _defaultLock;

        // work vars
        private int _lastLength = 0;
        private int _reservedLine = 0;
        private const char _spaceChar = ' ';
        private StringBuilder _builder = new StringBuilder();

        // properties
        /// <summary>Number of segments inside bar.</summary>
        /// <remarks>Default is 30.</remarks>
        public int BarLength
        {
            get { return _barLength; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("BarLength cannot be a negative number.");
                _barLength = value;
            }
        }
        /// <summary>Character used for filled segments.</summary>
        /// <remarks>Default is '#'.</remarks>
        public char CharFill { get; set; } = '#';
        /// <summary>Character used for empty segments. Default is </summary>
        /// <remarks>Default is '-'.</remarks>
        public char CharEmpty { get; set; } = '-';
        /// <summary>Amount of space reserved for text before the bar.</summary>
        /// <remarks>If text is shorter than this value, the difference will be filled with empty spaces.
        /// This prevents bar jumping as text changes, making it easier to read.<br/>
        /// If text is null, the empty spaces won't be put, and bar will be put at the beginning of the line.<br/><br/>
        /// Default is 50.</remarks>
        public int TextSpace
        {
            get { return _textSpace; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("TextSpace cannot be negative.");
                _textSpace = value;
            }
        }
        /// <summary>Determines whether percentage value should be displayed after the bar.</summary>
        /// <remarks>Default is true.</remarks>
        public bool ShowPercentage { get; set; } = true;
        /// <summary>Format of the percentage to display.</summary>
        /// <remarks>Defined using <see cref="double.ToString(string)"/>. See 
        /// <see href="https://msdn.microsoft.com/pl-pl/library/dwhawy9k(v=vs.110).aspx"> Standard Numeric Format Strings</see>
        /// for reference.<br/>
        /// Default is "0%".</remarks>
        public string PercentageFormat { get; set; } = "0%";
        /// <summary>Left side of the progress bar.</summary>
        /// <remarks>Default is "[ ".</remarks>
        public string BarOpening { get; set; } = "[ ";
        /// <summary>Right side of the progress bar.</summary>
        /// <remarks>Default is " ]".</remarks>
        public string BarClosing { get; set; } = " ] ";
        /// <summary>Text currently displayed with the bar. Read-only.</summary>
        /// <seealso cref="Update(double, string, string[])"/>
        /// <seealso cref="Write(string, string[])"/>
        public string Text { get; private set; } = null;
        /// <summary>Replace lock object used for this bar with new one. Doesn't affect any other bars.</summary>
        /// <remarks>By default, all progress bars use default lock created internally. Lock prevents bars to get messed up when
        /// multiple bars are used at once on multiple threads. Using <see cref="LockObject"/>, it is possible to assign
        /// another lock object. This can be useful, if another threads update console for other purpose than progress bars as well.<br/><br/>
        /// Please note that this will only affect one specific progress bar. Others will use their default lock object, unless changed manually.
        /// In general, it's not recommended to change <see cref="LockObject"/>, unless used on each created bar, and used 
        /// <see cref="SetDefaultLockObject(object)"/> as well.</remarks>
        /// <param name="obj">Object to use as lock.</param>
        /// <seealso cref="SetDefaultLockObject(object)"/>
        public object LockObject
        {
            get { return _lock; }
            set
            {
                _lock = value ?? throw new ArgumentNullException("value");
            }
        }

        /// <summary>Creates new instance of progress bar.</summary>
        /// <remarks>Creating new bar doesn't start writing it to console - use <see cref="Start"/>.</remarks>
        /// <param name="text">Initial text to display with the bar.</param>
        /// <param name="barLength">Length of the bar.</param>
        /// <param name="textSpace">Space for text before the bar.</param>
        public ProgressBar(string text, int barLength, int textSpace)
            : this(text, barLength)
        {
            this.TextSpace = textSpace;
        }

        /// <summary>Creates new instance of progress bar.</summary>
        /// <remarks>Creating new bar doesn't start writing it to console - use <see cref="Start"/>.</remarks>
        /// <param name="text">Initial text to display with the bar.</param>
        public ProgressBar(string text)
            : this()
        {
            this.Text = text;
        }

        /// <summary>Creates new instance of progress bar.</summary>
        /// <remarks>Creating new bar doesn't start writing it to console - use <see cref="Start"/>.</remarks>
        /// <param name="text">Initial text to display with the bar.</param>
        /// <param name="barLength">Length of the bar.</param>
        public ProgressBar(string text, int barLength)
            : this(text)
        {
            this.BarLength = barLength;
        }

        /// <summary>Creates new instance of progress bar.</summary>
        /// <remarks>Creating new bar doesn't start writing it to console - use <see cref="Start"/>.</remarks>
        /// <param name="barLength">Length of the bar.</param>
        public ProgressBar(int barLength)
            : this()
        {
            this.BarLength = barLength;
        }

        /// <summary>Creates new instance of progress bar.</summary>
        /// <remarks>Creating new bar doesn't start writing it to console - use <see cref="Start"/>.</remarks>
        public ProgressBar() { }

        /// <summary>Starts the progress bar, writing it to console.</summary>
        /// <remarks>This automatically creates new line if current line is not empty, and automatically creates empty line after.
        /// Reserves one console line for the bar.</remarks>
        public void Start()
        {
            lock (_lock)
            {
                if (Console.CursorLeft != 0)
                    Console.WriteLine();
                _reservedLine = Console.CursorTop;
                Console.WriteLine();
            }
        }

        #region additional type overloads
        /// <summary>Updates progress bar, additionally changing <see cref="Text"/>.</summary>
        /// <param name="progressCurrent">Current amount of steps done.</param>
        /// <param name="progressTotal">Total amount of steps to do.</param>
        /// <param name="textFormat">Format string to use.</param>
        /// <param name="args">Params for <paramref name="textFormat"/>.</param>
        /// <seealso cref="String.Format(string, object[])"/>
        /// <see cref="Update(double, string, string[])"/>
        public void Update(int progressCurrent, int progressTotal, string textFormat, params string[] args)
        {
            Update((double)progressCurrent / (double)progressTotal, textFormat, args);
        }

        /// <summary>Updates progress bar.</summary>
        /// <param name="progressCurrent">Current amount of steps done.</param>
        /// <param name="progressTotal">Total amount of steps to do.</param>
        /// <seealso cref="Update(double)"/>
        public void Update(int progressCurrent, int progressTotal)
        {
            Update((double)progressCurrent / (double)progressTotal);
        }

        /// <summary>Updates progress bar, additionally changing <see cref="Text"/>.</summary>
        /// <param name="progressCurrent">Current amount of steps done.</param>
        /// <param name="progressTotal">Total amount of steps to do.</param>
        /// <param name="textFormat">Format string to use.</param>
        /// <param name="args">Params for <paramref name="textFormat"/>.</param>
        /// <seealso cref="String.Format(string, object[])"/>
        /// <see cref="Update(double, string, string[])"/>
        public void Update(long progressCurrent, long progressTotal, string textFormat, params string[] args)
        {
            Update((double)progressCurrent / (double)progressTotal, textFormat, args);
        }

        /// <summary>Updates progress bar.</summary>
        /// <param name="progressCurrent">Current amount of steps done.</param>
        /// <param name="progressTotal">Total amount of steps to do.</param>
        /// <seealso cref="Update(double)"/>
        public void Update(long progressCurrent, long progressTotal)
        {
            Update((double)progressCurrent / (double)progressTotal);
        }

        /// <summary>Updates progress bar, additionally changing <see cref="Text"/>.</summary>
        /// <param name="progressCurrent">Current amount of steps done.</param>
        /// <param name="progressTotal">Total amount of steps to do.</param>
        /// <param name="textFormat">Format string to use.</param>
        /// <param name="args">Params for <paramref name="textFormat"/>.</param>
        /// <seealso cref="String.Format(string, object[])"/>
        /// <see cref="Update(double, string, string[])"/>
        public void Update(short progressCurrent, short progressTotal, string textFormat, params string[] args)
        {
            Update((double)progressCurrent / (double)progressTotal, textFormat, args);
        }

        /// <summary>Updates progress bar.</summary>
        /// <param name="progressCurrent">Current amount of steps done.</param>
        /// <param name="progressTotal">Total amount of steps to do.</param>
        /// <seealso cref="Update(double)"/>
        public void Update(short progressCurrent, short progressTotal)
        {
            Update((double)progressCurrent / (double)progressTotal);
        }

        /// <summary>Updates progress bar, additionally changing <see cref="Text"/>.</summary>
        /// <param name="progressCurrent">Current amount of steps done.</param>
        /// <param name="progressTotal">Total amount of steps to do.</param>
        /// <param name="textFormat">Format string to use.</param>
        /// <param name="args">Params for <paramref name="textFormat"/>.</param>
        /// <seealso cref="String.Format(string, object[])"/>
        /// <see cref="Update(double, string, string[])"/>
        public void Update(byte progressCurrent, byte progressTotal, string textFormat, params string[] args)
        {
            Update((double)progressCurrent / (double)progressTotal, textFormat, args);
        }

        /// <summary>Updates progress bar.</summary>
        /// <param name="progressCurrent">Current amount of steps done.</param>
        /// <param name="progressTotal">Total amount of steps to do.</param>
        /// <seealso cref="Update(double)"/>
        public void Update(byte progressCurrent, byte progressTotal)
        {
            Update((double)progressCurrent / (double)progressTotal);
        }
        #endregion

        /// <summary>Updates progress bar, additionally changing <see cref="Text"/>.</summary>
        /// <param name="progress">Value of progress completion. 0 is 0%, 1 is 100%.</param>
        /// <param name="textFormat">Format string to use.</param>
        /// <param name="args">Params for <paramref name="textFormat"/>.</param>
        /// <seealso cref="String.Format(string, object[])"/>
        public void Update(double progress, string textFormat, params string[] args)
        {
            Text = textFormat != null ? string.Format(textFormat, args) : null;
            Update(progress);
        }

        /// <summary>Writes text, completely replacing and hiding the bar. Useful for displaying final output.</summary>
        /// <param name="textFormat">Format string to use.</param>
        /// <param name="args">Params for <paramref name="textFormat"/>.</param>
        /// <seealso cref="String.Format(string, object[])"/>
        public void Write(string textFormat, params string[] args)
        {
            Text = textFormat != null ? string.Format(textFormat, args) : null;
            lock (_builder)
            {
                _builder.Remove(0, _builder.Length);
                if (Text != null)
                    _builder.Append(Text);
                FinalizeWrite();
            }
        }

        /// <summary>Updates progress bar.</summary>
        /// <param name="progress">Value of progress completion. 0 is 0%, 1 is 100%.</param>
        public void Update(double progress)
        {
            lock (_builder)
            {
                // clear stringbuilder
                _builder.Remove(0, _builder.Length);

                if (Text != null)
                {
                    _builder.Append(Text);
                    // if text is no longer than text space, fill remaining space with blanks
                    int diff = _textSpace - _builder.Length;
                    if (diff > 0)
                        _builder.Append(_spaceChar, diff);
                }


                _builder.Append(BarOpening);
                // calculate amount of fill
                int fill = (int)(progress * _barLength);
                if (fill > _barLength)
                    fill = _barLength;
                // fill bar
                _builder.Append(CharFill, fill);
                _builder.Append(CharEmpty, _barLength - fill);

                _builder.Append(BarClosing);

                // add percentage
                if (ShowPercentage)
                {
                    _builder.Append(_spaceChar); ;
                    _builder.Append(progress.ToString(PercentageFormat));
                }

                FinalizeWrite();
            }
        }

        private void FinalizeWrite()
        {
            // calculate difference in length, store current length, and add blanks to builder if needed
            int diff = _lastLength - _builder.Length;
            _lastLength = _builder.Length;
            if (diff > 0)
            {
                _builder.Append(_spaceChar, diff);
                _builder.Append('\b', diff);
            }

            lock (_lock)
            {
                // store previous cursor position
                int prevCurLine = Console.CursorTop;
                int prevCurColumn = Console.CursorLeft;
                //bool prevCurVisible = Console.CursorVisible;

                // go to bar position and finally write
                Console.CursorVisible = false;
                Console.SetCursorPosition(0, _reservedLine);
                Console.Write(_builder.ToString());

                // go back to previous cursor position so app can continue normally
                Console.SetCursorPosition(prevCurColumn, prevCurLine);
                //Console.CursorVisible = prevCurVisible;
                Console.CursorVisible = true;
            }
        }

        /// <summary>Replaces internal default lock object with new one. Only affects progress bars created after this command.</summary>
        /// <remarks>By default, all progress bars use lock created internally. Lock prevents bars to get messed up when
        /// multiple bars are used at once on multiple threads. Using <see cref="SetDefaultLockObject(object)"/>, it is possible to assign
        /// another lock object. This can be useful, if another threads update console for other purpose than progress bars as well.<br/><br/>
        /// Please note that this method will only affect progress bars created using constructor AFTER this method was used.
        /// To change lock for progress bar that is already instantiated, use <see cref="LockObject"/>.</remarks>
        /// <param name="obj">Object to use as lock.</param>
        /// <seealso cref="LockObject"/>
        public static void SetDefaultLockObject(object obj)
        {
            _defaultLock = obj ?? throw new ArgumentNullException("obj");
        }
    }
}
