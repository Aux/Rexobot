namespace Rexobot
{
    public struct Email
    {
        private readonly string _value;

        public bool IsValid { get; }

        public Email(string value)
        {
            _value = value;
            IsValid = StringUtils.IsValidEmail(_value);
        }

        public override string ToString()
            => _value;
    }
}
