//MIT License

//Copyright (c) 2021-2021 Peter Kirmeier

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using Avalonia.Controls;
using HitCounterManager.Common;
using ReactiveUI;
using System.Windows.Input;

namespace HitCounterManager.ViewModels
{
    public class ProfileAttemptsPageViewModel : NotifyPropertyChangedImpl
    {
        public Window? OwnerWindow { get; set; }

        public ProfileAttemptsPageViewModel()
        {
            Submit = ReactiveCommand.Create(() => {
                if (null == _UserInput) return; // Error

                int val;
                if (!Extensions.TryParseMinMaxNumber(_UserInput, out val, 0, int.MaxValue)) return; // Error

                _Origin?.ProfileSetAttempts.Execute(val);

                OwnerWindow?.Close(); // Success
            });
        }

        private ProfileViewModel? _Origin;
        public ProfileViewModel? Origin
        {
            get => _Origin;
            set
            {
                if (_Origin != value)
                {
                    _Origin = value;
                    CallPropertyChanged(this, nameof(Origin));
                }
            }
        }

        private string? _UserInput;
        public string? UserInput
        {
            get => _UserInput;
            set
            {
                if (_UserInput != value)
                {
                    _UserInput = value;
                    CallPropertyChanged(this, nameof(UserInput));
                }
            }
        }

        public ICommand Submit { get; }
    }
}
