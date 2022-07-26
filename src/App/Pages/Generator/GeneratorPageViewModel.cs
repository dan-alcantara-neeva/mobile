﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Bit.App.Resources;
using Bit.App.Utilities;
using Bit.Core.Abstractions;
using Bit.Core.Models.Domain;
using Bit.Core.Utilities;
using Xamarin.Forms;

namespace Bit.App.Pages
{
    public class GeneratorPageViewModel : BaseViewModel
    {
        private readonly IPasswordGenerationService _passwordGenerationService;
        private readonly IPlatformUtilsService _platformUtilsService;
        private readonly IClipboardService _clipboardService;

        private PasswordGenerationOptions _options;
        private PasswordGeneratorPolicyOptions _enforcedPolicyOptions;
        private string _password;
        private bool _isPassword;
        private bool _isUsername;
        private bool _uppercase;
        private bool _lowercase;
        private bool _number;
        private bool _special;
        private bool _allowAmbiguousChars;
        private int _minNumber;
        private int _minSpecial;
        private int _length = 5;
        private int _numWords = 3;
        private string _wordSeparator;
        private bool _capitalize;
        private bool _includeNumber;
        private int _typeSelectedIndex;
        private int _passwordTypeSelectedIndex;
        private int _usernameTypeSelectedIndex;
        private int _serviceTypeSelectedIndex;
        private bool _doneIniting;
        private string _plusAddressedEmail;
        private string _catchAllEmailDomain;
        private string _anonAddyApiAccessToken;
        private string _anonAddyDomainName;
        private string _firefoxRelayApiAccessToken;
        private string _simpleLoginApiKey;
        private bool _randomWordUsernameCapitalize;
        private bool _randomWordUsernameIncludeNumber;
        private bool _showTypePicker;

        public GeneratorPageViewModel()
        {
            _passwordGenerationService = ServiceContainer.Resolve<IPasswordGenerationService>(
                "passwordGenerationService");
            _platformUtilsService = ServiceContainer.Resolve<IPlatformUtilsService>("platformUtilsService");
            _clipboardService = ServiceContainer.Resolve<IClipboardService>("clipboardService");

            PageTitle = AppResources.Generator;
            TypeOptions = new List<string> { AppResources.Password, AppResources.Username };
            PasswordTypeOptions = new List<string> { AppResources.Password, AppResources.Passphrase };
            UsernameTypeOptions = new List<string> { AppResources.PlusAddressedEmail, AppResources.CatchAllEmail, AppResources.ForwardedEmailAlias, AppResources.RandomWord };
            ServiceTypeOptions = new List<string> { AppResources.AnonAddy, AppResources.FirefoxRelay, AppResources.SimpleLogin};

            UsernameTypePromptHelpCommand = new Command(UsernameTypePromptHelp);
        }

        public List<string> TypeOptions { get; set; }
        public List<string> PasswordTypeOptions { get; set; }
        public List<string> UsernameTypeOptions { get; set; }
        public List<string> ServiceTypeOptions { get; set; }

        public Command UsernameTypePromptHelpCommand { get; set; }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value,
                additionalPropertyNames: new string[]
                {
                    nameof(ColoredPassword)
                });
        }

        public string ColoredPassword => PasswordFormatter.FormatPassword(Password);

        public bool IsPassword
        {
            get => _isPassword;
            set => SetProperty(ref _isPassword, value);
        }

        public bool IsUsername
        {
            get => _isUsername;
            set => SetProperty(ref _isUsername, value);
        }

        public bool ShowTypePicker
        {
            get => _showTypePicker;
            set => SetProperty(ref _showTypePicker, value);
        }

        public int Length
        {
            get => _length;
            set
            {
                if (SetProperty(ref _length, value))
                {
                    _options.Length = value;
                    var task = SliderInputAsync();
                }
            }
        }

        public bool Uppercase
        {
            get => _uppercase;
            set
            {
                if (SetProperty(ref _uppercase, value))
                {
                    _options.Uppercase = value;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public bool Lowercase
        {
            get => _lowercase;
            set
            {
                if (SetProperty(ref _lowercase, value))
                {
                    _options.Lowercase = value;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public bool Number
        {
            get => _number;
            set
            {
                if (SetProperty(ref _number, value))
                {
                    _options.Number = value;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public bool Special
        {
            get => _special;
            set
            {
                if (SetProperty(ref _special, value))
                {
                    _options.Special = value;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public bool AllowAmbiguousChars
        {
            get => _allowAmbiguousChars;
            set
            {
                if (SetProperty(ref _allowAmbiguousChars, value,
                    additionalPropertyNames: new string[]
                    {
                        nameof(AvoidAmbiguousChars)
                    }))
                {
                    _options.AllowAmbiguousChar = value;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public bool AvoidAmbiguousChars
        {
            get => !AllowAmbiguousChars;
            set => AllowAmbiguousChars = !value;
        }

        public int MinNumber
        {
            get => _minNumber;
            set
            {
                if (SetProperty(ref _minNumber, value))
                {
                    _options.MinNumber = value;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public int MinSpecial
        {
            get => _minSpecial;
            set
            {
                if (SetProperty(ref _minSpecial, value))
                {
                    _options.MinSpecial = value;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public int NumWords
        {
            get => _numWords;
            set
            {
                if (SetProperty(ref _numWords, value))
                {
                    _options.NumWords = value;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public string WordSeparator
        {
            get => _wordSeparator;
            set
            {
                if (value == null)
                {
                    return;
                }
                var val = value.Trim();
                if (SetProperty(ref _wordSeparator, val))
                {
                    _options.WordSeparator = val;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public bool Capitalize
        {
            get => _capitalize;
            set
            {
                if (SetProperty(ref _capitalize, value))
                {
                    _options.Capitalize = value;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public bool IncludeNumber
        {
            get => _includeNumber;
            set
            {
                if (SetProperty(ref _includeNumber, value))
                {
                    _options.Number = value;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public string PlusAddressedEmail
        {
            get => _plusAddressedEmail;
            set
            {
                if(SetProperty(ref _plusAddressedEmail, value))
                {
                    _plusAddressedEmail = value;
                }
            }
        }

        public PasswordGeneratorPolicyOptions EnforcedPolicyOptions
        {
            get => _enforcedPolicyOptions;
            set => SetProperty(ref _enforcedPolicyOptions, value,
                additionalPropertyNames: new[]
                {
                    nameof(IsPolicyInEffect)
                });
        }

        public bool IsPolicyInEffect => _enforcedPolicyOptions.InEffect();

        public int TypeSelectedIndex
        {
            get => _typeSelectedIndex;
            set
            {
                if (SetProperty(ref _typeSelectedIndex, value))
                {
                    IsUsername = value == 1;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public int PasswordTypeSelectedIndex
        {
            get => _passwordTypeSelectedIndex;
            set
            {
                if (SetProperty(ref _passwordTypeSelectedIndex, value))
                {
                    IsPassword = value == 0;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public int UsernameTypeSelectedIndex
        {
            get => _usernameTypeSelectedIndex;
            set
            {
                if (SetProperty(ref _usernameTypeSelectedIndex, value))
                {
                    _usernameTypeSelectedIndex = value;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public int ServiceTypeSelectedIndex
        {
            get => _serviceTypeSelectedIndex;
            set
            {
                if (SetProperty(ref _serviceTypeSelectedIndex, value))
                {
                    _serviceTypeSelectedIndex = value;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public string CatchAllEmailDomain
        {
            get => _catchAllEmailDomain;
            set
            {
                if (SetProperty(ref _catchAllEmailDomain, value))
                {
                    _catchAllEmailDomain = value;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public string AnonAddyApiAccessToken
        {
            get => _anonAddyApiAccessToken;
            set
            {
                if (SetProperty(ref _anonAddyApiAccessToken, value))
                {
                    _anonAddyApiAccessToken = value;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public string AnonAddyDomainName
        {
            get => _anonAddyDomainName;
            set
            {
                if (SetProperty(ref _anonAddyDomainName, value))
                {
                    _anonAddyDomainName = value;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public string FirefoxRelayApiAccessToken
        {
            get => _firefoxRelayApiAccessToken;
            set
            {
                if (SetProperty(ref _firefoxRelayApiAccessToken, value))
                {
                    _firefoxRelayApiAccessToken = value;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public string SimpleLoginApiKey
        {
            get => _simpleLoginApiKey;
            set
            {
                if (SetProperty(ref _simpleLoginApiKey, value))
                {
                    _simpleLoginApiKey = value;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public bool RandomWordUsernameCapitalize
        {
            get => _randomWordUsernameCapitalize;
            set
            {
                if (SetProperty(ref _randomWordUsernameCapitalize, value))
                {
                    _randomWordUsernameCapitalize = value;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public bool RandomWordUsernameIncludeNumber
        {
            get => _randomWordUsernameIncludeNumber;
            set
            {
                if (SetProperty(ref _randomWordUsernameIncludeNumber, value))
                {
                    _randomWordUsernameIncludeNumber = value;
                    var task = SaveOptionsAsync();
                }
            }
        }

        public async Task InitAsync()
        {
            (_options, EnforcedPolicyOptions) = await _passwordGenerationService.GetOptionsAsync();
            LoadFromOptions();
            await RegenerateAsync();
            _doneIniting = true;
        }

        public async Task RegenerateAsync()
        {
            Password = await _passwordGenerationService.GeneratePasswordAsync(_options);
            await _passwordGenerationService.AddHistoryAsync(Password);
        }

        public void RedrawPassword()
        {
            if (!string.IsNullOrEmpty(_password))
            {
                TriggerPropertyChanged(nameof(ColoredPassword));
            }
        }

        public async Task SaveOptionsAsync(bool regenerate = true)
        {
            if (!_doneIniting)
            {
                return;
            }
            SetOptions();
            _passwordGenerationService.NormalizeOptions(_options, _enforcedPolicyOptions);
            await _passwordGenerationService.SaveOptionsAsync(_options);
            LoadFromOptions();
            if (regenerate)
            {
                await RegenerateAsync();
            }
        }

        public async Task SliderChangedAsync()
        {
            await SaveOptionsAsync(false);
            await _passwordGenerationService.AddHistoryAsync(Password);
        }

        public async Task SliderInputAsync()
        {
            if (!_doneIniting)
            {
                return;
            }
            SetOptions();
            _passwordGenerationService.NormalizeOptions(_options, _enforcedPolicyOptions);
            Password = await _passwordGenerationService.GeneratePasswordAsync(_options);
        }

        public async Task CopyAsync()
        {
            await _clipboardService.CopyTextAsync(Password);
            _platformUtilsService.ShowToastForCopiedValue(AppResources.Password);
        }

        public void UsernameTypePromptHelp()
        {
            _platformUtilsService.LaunchUri("https://bitwarden.com/help/generator/#username-types");
        }

        private void LoadFromOptions()
        {
            AllowAmbiguousChars = _options.AllowAmbiguousChar.GetValueOrDefault();
            PasswordTypeSelectedIndex = _options.Type == "passphrase" ? 1 : 0;
            IsPassword = PasswordTypeSelectedIndex == 0;
            MinNumber = _options.MinNumber.GetValueOrDefault();
            MinSpecial = _options.MinSpecial.GetValueOrDefault();
            Special = _options.Special.GetValueOrDefault();
            Number = _options.Number.GetValueOrDefault();
            NumWords = _options.NumWords.GetValueOrDefault();
            WordSeparator = _options.WordSeparator;
            Uppercase = _options.Uppercase.GetValueOrDefault();
            Lowercase = _options.Lowercase.GetValueOrDefault();
            Length = _options.Length.GetValueOrDefault(5);
            Capitalize = _options.Capitalize.GetValueOrDefault();
            IncludeNumber = _options.IncludeNumber.GetValueOrDefault();
            RandomWordUsernameCapitalize = _options.RandomWordUsernameCapitalize.GetValueOrDefault();
            RandomWordUsernameIncludeNumber = _options.RandomWordUsernameIncludeNumber.GetValueOrDefault();
        }

        private void SetOptions()
        {
            _options.AllowAmbiguousChar = AllowAmbiguousChars;
            _options.Type = PasswordTypeSelectedIndex == 1 ? "passphrase" : "password";
            _options.MinNumber = MinNumber;
            _options.MinSpecial = MinSpecial;
            _options.Special = Special;
            _options.NumWords = NumWords;
            _options.Number = Number;
            _options.WordSeparator = WordSeparator;
            _options.Uppercase = Uppercase;
            _options.Lowercase = Lowercase;
            _options.Length = Length;
            _options.Capitalize = Capitalize;
            _options.IncludeNumber = IncludeNumber;
            _options.RandomWordUsernameCapitalize = RandomWordUsernameCapitalize;
            _options.RandomWordUsernameIncludeNumber = RandomWordUsernameIncludeNumber;
        }
    }
}
